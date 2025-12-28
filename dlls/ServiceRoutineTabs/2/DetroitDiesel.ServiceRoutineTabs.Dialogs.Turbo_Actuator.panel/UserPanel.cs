using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Turbo_Actuator.panel;

public class UserPanel : CustomPanel
{
	public enum SoftwareComparisonResult
	{
		Older,
		Newer,
		Identical
	}

	public enum CurrentOperation
	{
		Disconnected,
		ConnectedDisabled,
		ConnectedNoHysteresis,
		ConnectedIdle,
		PreInstallation,
		SelfCalibration,
		HysteresisTest
	}

	public enum OperationPhase
	{
		None,
		Start,
		Status,
		Data,
		Stop
	}

	public enum OperationCallState
	{
		Ready,
		Executing
	}

	public enum PictureStatus
	{
		NoChange,
		ShowSuccess,
		ShowError,
		ShowBlank
	}

	public class UIState
	{
		private CurrentOperation currentOperation;

		private int stepNumber;

		private bool startButtonActive;

		private bool startButtonVisible;

		private bool stopButtonActive;

		private bool stopButtonVisible;

		private bool nextButtonActive;

		private bool nextButtonVisible;

		private bool previousButtonActive;

		private bool previousButtonVisible;

		private string instruction;

		private PictureBox picture;

		public CurrentOperation CurrentOperation => currentOperation;

		public int StepNumber => stepNumber;

		public bool StartButtonActive => startButtonActive;

		public bool StartButtonVisible => startButtonVisible;

		public bool StopButtonActive => stopButtonActive;

		public bool StopButtonVisible => stopButtonVisible;

		public bool NextButtonActive => nextButtonActive;

		public bool NextButtonVisible => nextButtonVisible;

		public bool PreviousButtonActive => previousButtonActive;

		public bool PreviousButtonVisible => previousButtonVisible;

		public string Instruction => instruction;

		public PictureBox Picture => picture;

		public UIState(CurrentOperation currentOperation, int stepNumber, bool startButtonActive, bool startButtonVisible, bool stopButtonActive, bool stopButtonVisible, bool nextButtonActive, bool nextButtonVisible, bool previousButtonActive, bool previousButtonVisible, string instruction, PictureBox picture)
		{
			this.currentOperation = currentOperation;
			this.stepNumber = stepNumber;
			this.startButtonActive = startButtonActive;
			this.startButtonVisible = startButtonVisible;
			this.stopButtonActive = stopButtonActive;
			this.stopButtonVisible = stopButtonVisible;
			this.nextButtonActive = nextButtonActive;
			this.nextButtonVisible = nextButtonVisible;
			this.previousButtonActive = previousButtonActive;
			this.previousButtonVisible = previousButtonVisible;
			this.instruction = instruction;
			this.picture = picture;
		}
	}

	public enum HysteresisMotorDirection
	{
		None,
		Outstroke,
		Backstroke
	}

	public enum HysteresisErrorType
	{
		None = 100,
		InconsistentData,
		AverageMotorEffortThreshold,
		PositionDeviationThreshold
	}

	public class HysteresisDataItem
	{
		public DateTime timestamp;

		public int targetPosition;

		public int currentPosition;

		public int motorEffort;

		public bool complete;
	}

	public class HysteresisTest
	{
		private const int motorEffortLowerBound = 50;

		private const int motorEffortUpperBound = 950;

		private const double motorEffortMaxThreshold = 40.0;

		private const double positionDeviationPercentageThreshold = 2.5;

		private ArrayList dataList = new ArrayList();

		private HysteresisDataItem currentDataItem;

		private int outstrokeBeginIndex;

		private int outstrokeEndIndex;

		private int backstrokeBeginIndex;

		private int backstrokeEndIndex;

		private double outstrokeAverageEffort;

		private double backstrokeAverageEffort;

		private HysteresisErrorType errorType;

		private HysteresisMotorDirection errorMotorDirection;

		private int errorTargetPosition;

		private int errorItemIndex;

		public HysteresisDataItem CurrentDataItem => currentDataItem;

		public HysteresisErrorType ErrorType => errorType;

		public HysteresisMotorDirection ErrorMotorDirection => errorMotorDirection;

		public int ErrorTargetPosition => errorTargetPosition;

		public int ErrorItemIndex => errorItemIndex;

		public int OutstrokeBeginIndex => outstrokeBeginIndex;

		public int OutstrokeEndIndex => outstrokeEndIndex;

		public int BackstrokeBeginIndex => backstrokeBeginIndex;

		public int BackstrokeEndIndex => backstrokeEndIndex;

		public double OutstrokeAverageEffort => outstrokeAverageEffort;

		public double BackstrokeAverageEffort => backstrokeAverageEffort;

		public void NewDataItem()
		{
			currentDataItem = new HysteresisDataItem();
			currentDataItem.timestamp = DateTime.Now;
			dataList.Add(currentDataItem);
		}

		public void SetTargetPosition(ushort targetPosition)
		{
			currentDataItem.targetPosition = targetPosition;
		}

		public void SetCurrentPosition(ushort currentPosition)
		{
			currentDataItem.currentPosition = currentPosition;
		}

		public void SetMotorEffort(byte motorEffort)
		{
			currentDataItem.motorEffort = motorEffort * 2 - 248;
		}

		public void MarkAsComplete()
		{
			currentDataItem.complete = true;
		}

		public void Reset()
		{
			dataList.Clear();
			dataList.TrimToSize();
			currentDataItem = null;
			outstrokeAverageEffort = 0.0;
			backstrokeAverageEffort = 0.0;
			outstrokeBeginIndex = -1;
			outstrokeEndIndex = -1;
			backstrokeBeginIndex = -1;
			backstrokeEndIndex = -1;
			errorType = HysteresisErrorType.None;
			errorMotorDirection = HysteresisMotorDirection.None;
			errorTargetPosition = 0;
			errorItemIndex = 0;
		}

		public bool DetermineStrokeRanges()
		{
			int i = 0;
			outstrokeBeginIndex = -1;
			outstrokeEndIndex = -1;
			backstrokeBeginIndex = -1;
			for (backstrokeEndIndex = -1; i < dataList.Count; i++)
			{
				HysteresisDataItem hysteresisDataItem = dataList[i] as HysteresisDataItem;
				if (hysteresisDataItem.targetPosition >= 50)
				{
					outstrokeBeginIndex = i;
					i++;
					break;
				}
			}
			for (; i < dataList.Count; i++)
			{
				HysteresisDataItem hysteresisDataItem = dataList[i] as HysteresisDataItem;
				if (hysteresisDataItem.targetPosition > 950)
				{
					outstrokeEndIndex = i - 1;
					break;
				}
			}
			for (; i < dataList.Count; i++)
			{
				HysteresisDataItem hysteresisDataItem = dataList[i] as HysteresisDataItem;
				if (hysteresisDataItem.targetPosition <= 950)
				{
					backstrokeBeginIndex = i;
					i++;
					break;
				}
			}
			for (; i < dataList.Count; i++)
			{
				HysteresisDataItem hysteresisDataItem = dataList[i] as HysteresisDataItem;
				if (hysteresisDataItem.targetPosition < 50)
				{
					backstrokeEndIndex = i - 1;
					break;
				}
			}
			if (outstrokeBeginIndex < 0 || outstrokeEndIndex < 0)
			{
				errorType = HysteresisErrorType.InconsistentData;
				errorMotorDirection = HysteresisMotorDirection.Outstroke;
				errorTargetPosition = 0;
				errorItemIndex = 0;
				return false;
			}
			if (backstrokeBeginIndex < 0 || backstrokeEndIndex < 0)
			{
				errorType = HysteresisErrorType.InconsistentData;
				errorMotorDirection = HysteresisMotorDirection.Backstroke;
				errorTargetPosition = 0;
				errorItemIndex = 0;
				return false;
			}
			if (outstrokeBeginIndex > outstrokeEndIndex)
			{
				errorType = HysteresisErrorType.InconsistentData;
				errorMotorDirection = HysteresisMotorDirection.Outstroke;
				errorTargetPosition = 0;
				errorItemIndex = 0;
				return false;
			}
			if (backstrokeBeginIndex > backstrokeEndIndex)
			{
				errorType = HysteresisErrorType.InconsistentData;
				errorMotorDirection = HysteresisMotorDirection.Backstroke;
				errorTargetPosition = 0;
				errorItemIndex = 0;
				return false;
			}
			return true;
		}

		public bool CalculateAverageOutstrokeEffort()
		{
			double num = 0.0;
			int num2 = outstrokeEndIndex - outstrokeBeginIndex + 1;
			outstrokeAverageEffort = 0.0;
			if (outstrokeBeginIndex < 0 || outstrokeEndIndex < 0 || num2 <= 0)
			{
				errorType = HysteresisErrorType.InconsistentData;
				errorMotorDirection = HysteresisMotorDirection.Outstroke;
				errorTargetPosition = 0;
				errorItemIndex = 0;
				return false;
			}
			for (int i = outstrokeBeginIndex; i <= outstrokeEndIndex; i++)
			{
				HysteresisDataItem hysteresisDataItem = dataList[i] as HysteresisDataItem;
				num += (double)hysteresisDataItem.motorEffort;
			}
			outstrokeAverageEffort = num / (double)num2;
			if (Math.Abs(outstrokeAverageEffort) > 40.0)
			{
				errorType = HysteresisErrorType.AverageMotorEffortThreshold;
				errorMotorDirection = HysteresisMotorDirection.Outstroke;
				errorTargetPosition = 0;
				errorItemIndex = 0;
				return false;
			}
			return true;
		}

		public bool CalculateAverageBackstrokeEffort()
		{
			double num = 0.0;
			int num2 = backstrokeEndIndex - backstrokeBeginIndex + 1;
			backstrokeAverageEffort = 0.0;
			if (backstrokeBeginIndex < 0 || backstrokeEndIndex < 0 || num2 <= 0)
			{
				errorType = HysteresisErrorType.InconsistentData;
				errorMotorDirection = HysteresisMotorDirection.Backstroke;
				errorTargetPosition = 0;
				errorItemIndex = 0;
				return false;
			}
			for (int i = backstrokeBeginIndex; i <= backstrokeEndIndex; i++)
			{
				HysteresisDataItem hysteresisDataItem = dataList[i] as HysteresisDataItem;
				num += (double)hysteresisDataItem.motorEffort;
			}
			backstrokeAverageEffort = num / (double)num2;
			if (Math.Abs(backstrokeAverageEffort) > 40.0)
			{
				errorType = HysteresisErrorType.AverageMotorEffortThreshold;
				errorMotorDirection = HysteresisMotorDirection.Backstroke;
				errorTargetPosition = 0;
				errorItemIndex = 0;
				return false;
			}
			return true;
		}

		public bool TestPositionDeviationThresholds()
		{
			int num = 0;
			double num2 = 0.0;
			for (num = outstrokeBeginIndex; num < outstrokeEndIndex - 1; num++)
			{
				HysteresisDataItem hysteresisDataItem = dataList[num] as HysteresisDataItem;
				if (!((double)hysteresisDataItem.motorEffort > outstrokeAverageEffort))
				{
					continue;
				}
				hysteresisDataItem = dataList[num + 2] as HysteresisDataItem;
				if (!((double)hysteresisDataItem.motorEffort > outstrokeAverageEffort))
				{
					continue;
				}
				hysteresisDataItem = dataList[num + 1] as HysteresisDataItem;
				if ((double)hysteresisDataItem.motorEffort > outstrokeAverageEffort)
				{
					num2 = Math.Abs((hysteresisDataItem.targetPosition - hysteresisDataItem.currentPosition) / hysteresisDataItem.targetPosition) * 100;
					if (num2 > 2.5)
					{
						errorType = HysteresisErrorType.PositionDeviationThreshold;
						errorMotorDirection = HysteresisMotorDirection.Outstroke;
						errorTargetPosition = hysteresisDataItem.targetPosition;
						errorItemIndex = num + 1;
						return false;
					}
				}
			}
			for (num = backstrokeBeginIndex; num < backstrokeEndIndex - 1; num++)
			{
				HysteresisDataItem hysteresisDataItem = dataList[num] as HysteresisDataItem;
				if (!((double)hysteresisDataItem.motorEffort > backstrokeAverageEffort))
				{
					continue;
				}
				hysteresisDataItem = dataList[num + 2] as HysteresisDataItem;
				if (!((double)hysteresisDataItem.motorEffort > backstrokeAverageEffort))
				{
					continue;
				}
				hysteresisDataItem = dataList[num + 1] as HysteresisDataItem;
				if ((double)hysteresisDataItem.motorEffort > backstrokeAverageEffort)
				{
					num2 = Math.Abs((hysteresisDataItem.targetPosition - hysteresisDataItem.currentPosition) / hysteresisDataItem.targetPosition) * 100;
					if (num2 > 2.5)
					{
						errorType = HysteresisErrorType.PositionDeviationThreshold;
						errorMotorDirection = HysteresisMotorDirection.Backstroke;
						errorTargetPosition = hysteresisDataItem.targetPosition;
						errorItemIndex = num + 1;
						return false;
					}
				}
			}
			return true;
		}
	}

	private enum ActuatorResultStatus
	{
		Unknown,
		InProgress,
		Done,
		Aborted,
		NotStarted,
		NoCommunication
	}

	private const string softwareVersionQualifier = "CO_SoftwareVersion";

	private const string firstSoftwareVersion = "7.6.8.0";

	private const string firstSoftwareVersionWithHysteresis = "7.8.2.0";

	private const string sra5StatusCodeInstrumentQualifier = "DT_AS052_SRA5_Status_Code";

	private const string preInstallationStartQualifier = "RT_SR061_Pre_install_Routine_Start_ActuatorStatus";

	private const int preInstallationStartInputValue1ChoiceRawValue = 5;

	private const int preInstallationStartInputValue2Value = 90;

	private const int preInstallationStartOutputValueForSuccess = 1;

	private const int preInstallationStartOutputValueForFailure = 2;

	private const string preInstallationStatusQualifier = "RT_SR061_Pre_install_Routine_Request_Results_ActuatorResult";

	private const string preInstallationStopQualifier = "RT_SR061_Pre_install_Routine_Stop_ActuatorNumber";

	private const int preInstallationStopInputValue1ChoiceRawValue = 5;

	private const string selfCalibrationStartQualifier = "RT_SR062_Self_Calibration_Routine_Start_ActuatorStartStatus";

	private const int selfCalibrationStartInputValue1ChoiceRawValue = 5;

	private const int selfCalibrationStartOutputValueForSuccess = 1;

	private const int selfCalibrationStartOutputValueForFailure = 2;

	private const string selfCalibrationStatusQualifier = "RT_SR062_Self_Calibration_Routine_Request_Results_ActuatorResultStatus";

	private const int selfCalibrationStatusInputValue1ChoiceRawValue = 5;

	private const string selfCalibrationStopQualifier = "RT_SR062_Self_Calibration_Routine_Stop_ActuatorNumber";

	private const int selfCalibrationStopInputValue1ChoiceRawValue = 5;

	private const string hysteresisTestStartQualifier = "RT_SR063_Hysteres_Test_Routine_Start_ActuatorStartStatus";

	private const int hysteresisTestStartInputValue1ChoiceRawValue = 5;

	private const int hysteresisTestStartOutputValueForSuccess = 1;

	private const int hysteresisTestStartOutputValueForFailure = 2;

	private const string hysteresisTestDataQualifier = "RT_SR063_Hysteres_Test_Routine_Request_Results_Data";

	private const int hysteresisTestDataInputValue1ChoiceRawValue = 5;

	private const string hysteresisTestStopQualifier = "RT_SR063_Hysteres_Test_Routine_Stop_ActuatorNumber";

	private const int hysteresisTestStopInputValue1ChoiceRawValue = 5;

	private const int UIStateCount = 14;

	private Dictionary<string, CacheInfo> snapshot;

	private bool forcePreInstallationStop = false;

	private bool forceSelfCalibrationStop = false;

	private int hysteresisTestDataCacheTimeRestore;

	private bool flagDisplaySra5StatusCode = true;

	private bool forceHysteresisTestStop = false;

	private HysteresisTest hysteresisTest = new HysteresisTest();

	private UIState[] stateInfo = new UIState[14];

	private Channel mcmChannel = null;

	private CurrentOperation currentOperation;

	private int stepNumber;

	private OperationPhase operationPhase;

	private OperationCallState operationCallState;

	private Timer operationTimer;

	private bool hysteresisOnly = false;

	private bool hysteresisSuccessful = false;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonStart;

	private Button buttonNext;

	private Button buttonPrevious;

	private Button buttonStop;

	private Button buttonHysteresisTest;

	private Button buttonPreInstallation;

	private Button buttonSelfCalibration;

	private PictureBox picWait;

	private PictureBox picHysteresis;

	private PictureBox picGrease;

	private PictureBox picBlank;

	private PictureBox picError;

	private PictureBox picMount;

	private PictureBox picNotMounted;

	private PictureBox picOk;

	private PictureBox picAlignHoles;

	private Panel panel3;

	private TableLayoutPanel tableLayoutPanel2;

	private TableLayoutPanel tableLayoutPanel3;

	private TextBox textboxInstructions;

	private Label lblInstructions;

	private Label lblStatus;

	private Button closeButton;

	private TextBox textboxStatus;

	private bool Connected => mcmChannel != null && mcmChannel.CommunicationsState != CommunicationsState.Offline;

	private bool ValidEngineType
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			IEnumerable<EquipmentType> enumerable = EquipmentType.ConnectedEquipmentTypes("Engine");
			if (CollectionExtensions.Exactly<EquipmentType>(enumerable, 1))
			{
				EquipmentType val = enumerable.First();
				string name = ((EquipmentType)(ref val)).Name;
				return name == "S60";
			}
			return false;
		}
	}

	private bool EcuFullySupported
	{
		get
		{
			bool result = false;
			EcuInfo ecuInfo = mcmChannel.EcuInfos["CO_SoftwareVersion"];
			if (ecuInfo != null)
			{
				string empty = string.Empty;
				empty = ecuInfo.Value;
				if (CompareSoftwareVersions(empty, "7.8.2.0") != SoftwareComparisonResult.Older)
				{
					Service service = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Start_ActuatorStartStatus");
					Service service2 = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data");
					Service service3 = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Stop_ActuatorNumber");
					result = EcuPartiallySupported && service != null && service2 != null && service3 != null;
				}
			}
			return result;
		}
	}

	private bool EcuPartiallySupported
	{
		get
		{
			bool result = false;
			EcuInfo ecuInfo = mcmChannel.EcuInfos["CO_SoftwareVersion"];
			if (ecuInfo != null)
			{
				string value = ecuInfo.Value;
				if (CompareSoftwareVersions(value, "7.6.8.0") != SoftwareComparisonResult.Older)
				{
					Service service = ((CustomPanel)this).GetService("MCM", "RT_SR061_Pre_install_Routine_Start_ActuatorStatus");
					Service service2 = ((CustomPanel)this).GetService("MCM", "RT_SR061_Pre_install_Routine_Request_Results_ActuatorResult");
					Service service3 = ((CustomPanel)this).GetService("MCM", "RT_SR061_Pre_install_Routine_Stop_ActuatorNumber");
					Service service4 = ((CustomPanel)this).GetService("MCM", "RT_SR062_Self_Calibration_Routine_Start_ActuatorStartStatus");
					Service service5 = ((CustomPanel)this).GetService("MCM", "RT_SR062_Self_Calibration_Routine_Request_Results_ActuatorResultStatus");
					Service service6 = ((CustomPanel)this).GetService("MCM", "RT_SR062_Self_Calibration_Routine_Stop_ActuatorNumber");
					result = service != null && service2 != null && service3 != null && service4 != null && service5 != null && service6 != null;
				}
			}
			return result;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	private SoftwareComparisonResult CompareSoftwareVersions(string currentVersion, string referenceVersion)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		bool flag = false;
		bool flag2 = false;
		while (!flag && !flag2)
		{
			num5 = currentVersion.IndexOf('.', num3);
			num6 = referenceVersion.IndexOf('.', num4);
			if (num5 < 0)
			{
				num5 = currentVersion.Length;
				flag = true;
			}
			if (num6 < 0)
			{
				num6 = referenceVersion.Length;
				flag2 = true;
			}
			num = Convert.ToInt32(currentVersion.Substring(num3, num5 - num3));
			num2 = Convert.ToInt32(referenceVersion.Substring(num4, num6 - num4));
			if (num < num2)
			{
				return SoftwareComparisonResult.Older;
			}
			if (num > num2)
			{
				return SoftwareComparisonResult.Newer;
			}
			num3 = num5 + 1;
			num4 = num6 + 1;
		}
		if (flag)
		{
			if (flag2)
			{
				return SoftwareComparisonResult.Identical;
			}
			return SoftwareComparisonResult.Older;
		}
		return SoftwareComparisonResult.Newer;
	}

	protected override void OnLoad(EventArgs e)
	{
		buttonPreInstallation.Click += OnPreInstallation_Click;
		buttonSelfCalibration.Click += OnSelfCalibration_Click;
		buttonHysteresisTest.Click += OnHysteresisTest_Click;
		buttonNext.Click += OnNext_Click;
		buttonPrevious.Click += OnPrevious_Click;
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		stateInfo[0] = new UIState(CurrentOperation.Disconnected, 0, startButtonActive: false, startButtonVisible: false, stopButtonActive: false, stopButtonVisible: false, nextButtonActive: false, nextButtonVisible: false, previousButtonActive: false, previousButtonVisible: false, Resources.Message_MCMNotConnected, picError);
		stateInfo[1] = new UIState(CurrentOperation.ConnectedIdle, 0, startButtonActive: false, startButtonVisible: false, stopButtonActive: false, stopButtonVisible: false, nextButtonActive: false, nextButtonVisible: false, previousButtonActive: false, previousButtonVisible: false, Resources.Message_MCMConnected, null);
		stateInfo[2] = new UIState(CurrentOperation.PreInstallation, 1, startButtonActive: false, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: true, nextButtonVisible: true, previousButtonActive: false, previousButtonVisible: true, Resources.Message_EnsureTheOutputGearIsUnimpeded + "\r\n\r\n" + Resources.ClickNextToProceed, picNotMounted);
		stateInfo[3] = new UIState(CurrentOperation.PreInstallation, 2, startButtonActive: true, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: false, nextButtonVisible: true, previousButtonActive: true, previousButtonVisible: true, Resources.Message_ClickStartToMoveTheGearIntoInitialPositionOrPreviousToGoBack, picBlank);
		stateInfo[4] = new UIState(CurrentOperation.PreInstallation, 3, startButtonActive: false, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: false, nextButtonVisible: true, previousButtonActive: false, previousButtonVisible: true, Resources.Message_PreInstallationRunning + "\r\n\r\n" + Resources.ClickStopToAbortOperation, picWait);
		stateInfo[5] = new UIState(CurrentOperation.SelfCalibration, 1, startButtonActive: false, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: true, nextButtonVisible: true, previousButtonActive: false, previousButtonVisible: true, Resources.Message_ImportantPreInstallationMustBePerformedBeforeRunningASelfCalibration + "\r\n\r\n" + Resources.CleanAnyDebrisFromTheSectorGearAndApplyTheCorrectGrease + "\r\n\r\n" + Resources.ClickNextToProceed, picGrease);
		stateInfo[6] = new UIState(CurrentOperation.SelfCalibration, 2, startButtonActive: false, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: true, nextButtonVisible: true, previousButtonActive: true, previousButtonVisible: true, Resources.Message_PositionTurboSectorGearToCorrespondWithTheActuatorOutputGearByAligningTheReferenceHoles + "\r\n" + Resources.RemoveThePinBeforeMounting + "\r\n\r\n" + Resources.ClickNextToProceedOrPreviousToGoBack, picAlignHoles);
		stateInfo[7] = new UIState(CurrentOperation.SelfCalibration, 3, startButtonActive: false, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: true, nextButtonVisible: true, previousButtonActive: true, previousButtonVisible: true, Resources.Message_MountTheActuatorSecurelyOntoTheTurboMakingSureTheNozzleIsInTheRecommendedPosition + "\r\n\r\n" + Resources.ClickNextToProceedOrPreviousToGoBack, picMount);
		stateInfo[8] = new UIState(CurrentOperation.SelfCalibration, 4, startButtonActive: true, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: false, nextButtonVisible: true, previousButtonActive: true, previousButtonVisible: true, Resources.Message_ClickStartToExecuteTheSelfCalibrationProcessOrPreviousToGoBack, picBlank);
		stateInfo[9] = new UIState(CurrentOperation.SelfCalibration, 5, startButtonActive: false, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: false, nextButtonVisible: true, previousButtonActive: false, previousButtonVisible: true, Resources.Message_SelfCalibrationRunning + "\r\n\r\n" + Resources.ClickStopToAbortOperation, picWait);
		stateInfo[10] = new UIState(CurrentOperation.HysteresisTest, 1, startButtonActive: true, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: false, nextButtonVisible: true, previousButtonActive: false, previousButtonVisible: true, Resources.Message_ClickStartToCheckForFreeNozzleMovementOrPreviousToGoBack, picBlank);
		stateInfo[11] = new UIState(CurrentOperation.HysteresisTest, 2, startButtonActive: false, startButtonVisible: true, stopButtonActive: true, stopButtonVisible: true, nextButtonActive: false, nextButtonVisible: true, previousButtonActive: false, previousButtonVisible: true, Resources.Message_HysteresisTestRunning + "\r\n\r\n" + Resources.ClickStopToAbortOperation, picWait);
		stateInfo[12] = new UIState(CurrentOperation.ConnectedDisabled, 0, startButtonActive: false, startButtonVisible: false, stopButtonActive: false, stopButtonVisible: false, nextButtonActive: false, nextButtonVisible: false, previousButtonActive: false, previousButtonVisible: false, Resources.Message_MCMConnectedPanelDisabled, null);
		stateInfo[13] = new UIState(CurrentOperation.ConnectedNoHysteresis, 0, startButtonActive: false, startButtonVisible: false, stopButtonActive: false, stopButtonVisible: false, nextButtonActive: false, nextButtonVisible: false, previousButtonActive: false, previousButtonVisible: false, Resources.Message_MCMConnectedHysteresisTestDisabled, null);
		ClearUserInterfaceState();
		if (((Control)this).Tag != null)
		{
			hysteresisOnly = true;
			StartHysteresisTest();
		}
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		((Control)this).Tag = new object[2] { hysteresisSuccessful, textboxStatus.Text };
	}

	public override void OnChannelsChanged()
	{
		Channel channel = ((CustomPanel)this).GetChannel("MCM");
		if (mcmChannel != channel)
		{
			if (mcmChannel != null)
			{
				mcmChannel.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			mcmChannel = channel;
			if (mcmChannel != null)
			{
				mcmChannel.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
			UpdateUserInterface();
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (currentOperation == CurrentOperation.ConnectedDisabled)
		{
			UpdateUserInterface();
		}
	}

	private void UpdateUserInterface()
	{
		ClearStatus();
		if (Connected)
		{
			if (!ValidEngineType)
			{
				currentOperation = CurrentOperation.ConnectedDisabled;
				stepNumber = 0;
				AppendStatus(Resources.Message_MCMConnectedButPanelWasNotReleasedForThisEngineType, PictureStatus.NoChange);
			}
			else if (EcuFullySupported)
			{
				currentOperation = CurrentOperation.ConnectedIdle;
				stepNumber = 0;
				ClearStatus();
				Service service = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data");
				hysteresisTestDataCacheTimeRestore = service.CacheTimeout;
				AppendStatus(Resources.Message_MCMConnected1, PictureStatus.ShowBlank);
			}
			else if (EcuPartiallySupported)
			{
				currentOperation = CurrentOperation.ConnectedNoHysteresis;
				stepNumber = 0;
				AppendStatus(Resources.Message_MCMConnectedButHysteresisTestIsDisabled, PictureStatus.ShowBlank);
			}
			else
			{
				currentOperation = CurrentOperation.ConnectedDisabled;
				stepNumber = 0;
				AppendStatus(Resources.Message_MCMConnectedButPanelIsDisabledForThisSofwareVersion, PictureStatus.NoChange);
			}
		}
		else
		{
			currentOperation = CurrentOperation.Disconnected;
			stepNumber = 0;
			AppendStatus(Resources.Message_MCMDisconnected, PictureStatus.NoChange);
		}
		UpdateControls();
	}

	private UIState RetrieveUIStateObject()
	{
		for (int i = 0; i < 14; i++)
		{
			UIState uIState = stateInfo[i];
			if (uIState.CurrentOperation == currentOperation && uIState.StepNumber == stepNumber)
			{
				return uIState;
			}
		}
		string text = string.Format(Resources.MessageFormat_UIStateIsNull01, currentOperation.ToString(), stepNumber.ToString());
		return null;
	}

	private void ClearUserInterfaceState()
	{
		buttonStart.Click -= OnPreInstallationStart_Click;
		buttonStop.Click -= OnPreInstallationStop_Click;
		buttonStart.Click -= OnSelfCalibrationStart_Click;
		buttonStop.Click -= OnSelfCalibrationStop_Click;
		buttonStart.Click -= OnHysteresisTestStart_Click;
		buttonStop.Click -= OnHysteresisTestStop_Click;
		if (Connected)
		{
			if (!ValidEngineType)
			{
				currentOperation = CurrentOperation.ConnectedDisabled;
				stepNumber = 0;
			}
			else if (EcuFullySupported)
			{
				currentOperation = CurrentOperation.ConnectedIdle;
				stepNumber = 0;
			}
			else if (EcuPartiallySupported)
			{
				currentOperation = CurrentOperation.ConnectedNoHysteresis;
				stepNumber = 0;
			}
			else
			{
				currentOperation = CurrentOperation.ConnectedDisabled;
				stepNumber = 0;
			}
		}
		else
		{
			currentOperation = CurrentOperation.Disconnected;
			stepNumber = 0;
		}
		UpdateControls();
	}

	private void UpdateControls()
	{
		UIState uIState = RetrieveUIStateObject();
		switch (currentOperation)
		{
		case CurrentOperation.ConnectedIdle:
			buttonPreInstallation.Enabled = true;
			buttonSelfCalibration.Enabled = true;
			buttonHysteresisTest.Enabled = true;
			break;
		case CurrentOperation.ConnectedNoHysteresis:
			buttonPreInstallation.Enabled = true;
			buttonSelfCalibration.Enabled = true;
			buttonHysteresisTest.Enabled = false;
			break;
		default:
			buttonPreInstallation.Enabled = false;
			buttonSelfCalibration.Enabled = false;
			buttonHysteresisTest.Enabled = false;
			break;
		}
		buttonPreInstallation.Enabled = !hysteresisOnly && buttonPreInstallation.Enabled;
		buttonSelfCalibration.Enabled = !hysteresisOnly && buttonSelfCalibration.Enabled;
		if (uIState != null)
		{
			buttonStart.Visible = uIState.StartButtonVisible;
			buttonStart.Enabled = uIState.StartButtonActive;
			buttonStop.Visible = uIState.StopButtonVisible;
			buttonStop.Enabled = uIState.StopButtonActive;
			buttonNext.Visible = uIState.NextButtonVisible;
			buttonNext.Enabled = uIState.NextButtonActive;
			buttonPrevious.Visible = uIState.PreviousButtonVisible;
			buttonPrevious.Enabled = uIState.PreviousButtonActive;
			textboxInstructions.Text = uIState.Instruction;
			ShowPicture(uIState.Picture);
		}
	}

	private void AppendStatus(string message, PictureStatus pictureStatus)
	{
		TextBox textBox = textboxStatus;
		textBox.Text = textBox.Text + message + Environment.NewLine;
		switch (pictureStatus)
		{
		case PictureStatus.ShowSuccess:
			ShowPicture(picOk);
			break;
		case PictureStatus.ShowError:
			ShowPicture(picError);
			break;
		case PictureStatus.ShowBlank:
			ShowPicture(picBlank);
			break;
		}
		textboxStatus.SelectionStart = textboxStatus.Text.Length - 1;
		textboxStatus.SelectionLength = 0;
		textboxStatus.ScrollToCaret();
	}

	private void ClearStatus()
	{
		textboxStatus.Text = string.Empty;
	}

	private void ShowPicture(PictureBox pictureBox)
	{
		if (pictureBox != null)
		{
			HideAllPictures();
			pictureBox.Visible = true;
		}
	}

	private void HideAllPictures()
	{
		picBlank.Visible = false;
		picWait.Visible = false;
		picOk.Visible = false;
		picError.Visible = false;
		picAlignHoles.Visible = false;
		picGrease.Visible = false;
		picHysteresis.Visible = false;
		picMount.Visible = false;
		picNotMounted.Visible = false;
	}

	private void OnNext_Click(object sender, EventArgs e)
	{
		stepNumber++;
		UpdateControls();
	}

	private void OnPrevious_Click(object sender, EventArgs e)
	{
		stepNumber--;
		UpdateControls();
	}

	private void OnPreInstallation_Click(object sender, EventArgs e)
	{
		currentOperation = CurrentOperation.PreInstallation;
		stepNumber = 1;
		ClearStatus();
		AppendStatus(Resources.Message_PreInstallationOperation, PictureStatus.NoChange);
		buttonStart.Click += OnPreInstallationStart_Click;
		buttonStop.Click += OnPreInstallationStop_Click;
		UpdateControls();
	}

	private void OnSelfCalibration_Click(object sender, EventArgs e)
	{
		currentOperation = CurrentOperation.SelfCalibration;
		stepNumber = 1;
		ClearStatus();
		AppendStatus(Resources.Message_SelfCalibrationOperation, PictureStatus.NoChange);
		buttonStart.Click += OnSelfCalibrationStart_Click;
		buttonStop.Click += OnSelfCalibrationStop_Click;
		UpdateControls();
	}

	private void OnHysteresisTest_Click(object sender, EventArgs e)
	{
		StartHysteresisTest();
	}

	private void StartHysteresisTest()
	{
		currentOperation = CurrentOperation.HysteresisTest;
		stepNumber = 1;
		ClearStatus();
		AppendStatus(Resources.Message_HysteresisTestOperation, PictureStatus.NoChange);
		buttonStart.Click += OnHysteresisTestStart_Click;
		buttonStop.Click += OnHysteresisTestStop_Click;
		UpdateControls();
	}

	private void operationTimer_Tick(object sender, EventArgs e)
	{
		if (!Connected || currentOperation == CurrentOperation.ConnectedIdle)
		{
			StopTimer();
		}
		else
		{
			if (operationCallState != OperationCallState.Ready)
			{
				return;
			}
			if (currentOperation == CurrentOperation.PreInstallation)
			{
				if (forcePreInstallationStop)
				{
					operationPhase = OperationPhase.Stop;
					ExecutePreInstallationStop();
				}
				else if (operationPhase == OperationPhase.Start)
				{
					ExecutePreInstallationStart();
				}
				else if (operationPhase == OperationPhase.Status)
				{
					ExecutePreInstallationStatus();
				}
				else if (operationPhase == OperationPhase.Stop)
				{
					ExecutePreInstallationStop();
				}
			}
			else if (currentOperation == CurrentOperation.SelfCalibration)
			{
				if (forceSelfCalibrationStop)
				{
					operationPhase = OperationPhase.Stop;
					ExecuteSelfCalibrationStop();
				}
				else if (operationPhase == OperationPhase.Start)
				{
					ExecuteSelfCalibrationStart();
				}
				else if (operationPhase == OperationPhase.Status)
				{
					ExecuteSelfCalibrationStatus();
				}
				else if (operationPhase == OperationPhase.Stop)
				{
					ExecuteSelfCalibrationStop();
				}
			}
			else if (currentOperation == CurrentOperation.HysteresisTest)
			{
				if (forceHysteresisTestStop)
				{
					operationPhase = OperationPhase.Stop;
					ExecuteHysteresisTestStop();
				}
				else if (operationPhase == OperationPhase.Start)
				{
					ExecuteHysteresisTestStart();
				}
				else if (operationPhase == OperationPhase.Data)
				{
					ExecuteHysteresisTestData();
				}
				else if (operationPhase == OperationPhase.Stop)
				{
					ExecuteHysteresisTestStop();
				}
			}
		}
	}

	private void StopTimer()
	{
		operationPhase = OperationPhase.None;
		operationCallState = OperationCallState.Ready;
		if (operationTimer != null)
		{
			operationTimer.Stop();
			operationTimer = null;
		}
	}

	private void DisplaySra5StatusCode()
	{
		if (!Connected || !flagDisplaySra5StatusCode)
		{
			return;
		}
		bool flag = false;
		int num;
		int num2;
		do
		{
			Instrument instrument = ((CustomPanel)this).GetInstrument("MCM", "DT_AS052_SRA5_Status_Code");
			num = Convert.ToInt32(instrument.InstrumentValues.Current.ToString());
			num2 = num;
		}
		while (num2 == 7);
		if (num != 0)
		{
			switch (num)
			{
			case 0:
				AppendStatus(Resources.Message_SRA5StatusCode0NormalOperation, PictureStatus.NoChange);
				break;
			case 5:
				AppendStatus(Resources.Message_SRA5StatusCode5MotorDisabledOperationCannotContinueDueToDetectedFaultCondition, PictureStatus.ShowError);
				break;
			case 23:
				AppendStatus(Resources.Message_SRA5StatusCode23ReferenceNotDetectedDuringLearnDueToMechanicalSystemBindingOrInternalConditionTruePositionNotKnown, PictureStatus.ShowError);
				break;
			case 11:
				AppendStatus(Resources.Message_SRA5StatusCode11NoValidCommandSourceHasBeenSeenByTheSRASincePoweringUpAndNoCommandSourceTimeHasPassed, PictureStatus.ShowError);
				break;
			case 17:
				AppendStatus(Resources.Message_SRA5StatusCode17CommandPWMSignalIsNotValid, PictureStatus.ShowError);
				break;
			case 30:
				AppendStatus(Resources.Message_SRA5StatusCode30NoValidUARTCommandFor262MsecAndValidPreviouslyReceived, PictureStatus.ShowError);
				break;
			case 8:
				AppendStatus(Resources.Message_SRA5StatusCode8NoValidCANCommandFor75MsecAndValidPreviouslyReceived, PictureStatus.ShowError);
				break;
			case 1:
				AppendStatus(Resources.Message_SRA5StatusCode1TemperatureInExcessOfFaultThreshold, PictureStatus.ShowError);
				break;
			case 2:
				AppendStatus(Resources.Message_SRA5StatusCode2TemperatureInExcessOfWarningThreshold, PictureStatus.ShowError);
				break;
			case 15:
				AppendStatus(Resources.Message_SRA5StatusCode15IgnitionVoltageHasSustainedAtTooLowLevelForAnExcessivePeriod, PictureStatus.ShowError);
				break;
			case 4:
				AppendStatus(Resources.Message_SRA5StatusCode4RestrictionDetectedAtLearn, PictureStatus.ShowError);
				break;
			case 3:
				AppendStatus(Resources.Message_SRA5StatusCode3SlowToCloseErrorOrUnableToCloseError, PictureStatus.ShowError);
				break;
			case 22:
				AppendStatus(Resources.Message_SRA5StatusCode22ActuationHasTakenInternalActionToAvoidOverheatingAndDamagingItsMotorRequiredTorqueMayNotBeAvailableAtActuatorOutput, PictureStatus.ShowError);
				break;
			case 9:
				AppendStatus(Resources.Message_SRA5StatusCode9DetectedLearnSpanBetween0And100IsTooLarge, PictureStatus.ShowError);
				break;
			case 7:
				AppendStatus(Resources.Message_SRA5StatusCode7TheSRAIsRunningAnInternalTestSequence, PictureStatus.ShowError);
				break;
			case 10:
				AppendStatus(Resources.Message_SRA5StatusCode10Either100StopIsTooHighOr0StopIsTooLowAndSpanTooLargeHasNotOccurred, PictureStatus.ShowError);
				break;
			default:
			{
				string message = string.Format(Resources.MessageFormat_SRA5StatusCode0UnknownValue, num);
				AppendStatus(message, PictureStatus.NoChange);
				break;
			}
			}
		}
	}

	private void SetupInstruments()
	{
		if (Connected)
		{
			snapshot = InstrumentCacheManager.GenerateSnapshot(mcmChannel);
			InstrumentCacheManager.UnmarkAllInstruments(mcmChannel);
			InstrumentCacheManager.MarkInstrument(mcmChannel, "DT_AS052_SRA5_Status_Code", (ushort)10);
			mcmChannel.FaultCodes.AutoRead = false;
		}
	}

	private void RestoreInstruments()
	{
		if (Connected && snapshot != null)
		{
			InstrumentCacheManager.ApplySnapshot(mcmChannel, snapshot);
			mcmChannel.FaultCodes.AutoRead = true;
		}
	}

	private void SetupHysteresisServiceRoutines()
	{
		if (Connected)
		{
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data");
			service.CacheTimeout = 0;
		}
	}

	private void RestoreHysteresisServiceRoutines()
	{
		if (Connected)
		{
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data");
			service.CacheTimeout = hysteresisTestDataCacheTimeRestore;
		}
	}

	private void OnPreInstallationStart_Click(object sender, EventArgs e)
	{
		stepNumber++;
		UpdateControls();
		AppendStatus(Resources.Message_PerformingPreInstallation, PictureStatus.NoChange);
		operationPhase = OperationPhase.Start;
		operationCallState = OperationCallState.Ready;
		forcePreInstallationStop = false;
		operationTimer = new Timer();
		operationTimer.Interval = 1000;
		operationTimer.Tick += operationTimer_Tick;
		operationTimer.Start();
	}

	private void ExecutePreInstallationStart()
	{
		if (Connected)
		{
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR061_Pre_install_Routine_Start_ActuatorStatus");
			service.ServiceCompleteEvent += PreInstallationStart_ServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			service.InputValues[1].Value = 90;
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private void PreInstallationStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		bool flag = false;
		Service service = sender as Service;
		service.ServiceCompleteEvent -= PreInstallationStart_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			Choice choice = service.OutputValues[0].Value as Choice;
			if (Convert.ToInt32(choice.RawValue) == 1)
			{
				AppendStatus(Resources.Message_PreInstallationStartedSuccessfully, PictureStatus.NoChange);
				operationPhase = OperationPhase.Status;
				operationCallState = OperationCallState.Ready;
				flag = true;
			}
			else if (Convert.ToInt32(choice.RawValue) == 2)
			{
				AppendStatus(Resources.Message_ErrorPreInstallationStartProcessFailed, PictureStatus.ShowError);
			}
			else
			{
				string message = string.Format(Resources.MessageFormat_ErrorPreInstallationStartProcessFailedWithOutputValue0, choice);
				AppendStatus(message, PictureStatus.ShowError);
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorPreInstallationStartServiceCallFailed, PictureStatus.ShowError);
		}
		if (!flag)
		{
			StopTimer();
			ClearUserInterfaceState();
		}
	}

	private void ExecutePreInstallationStatus()
	{
		if (Connected)
		{
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR061_Pre_install_Routine_Request_Results_ActuatorResult");
			service.ServiceCompleteEvent += PreInstallationStatus_ServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private ActuatorResultStatus GetResultStatus(ServiceOutputValue output)
	{
		Choice choice = output.Value as Choice;
		if (choice != null)
		{
			switch (Convert.ToInt32(choice.RawValue))
			{
			case 1:
				return ActuatorResultStatus.Done;
			case 2:
			case 3:
			case 4:
				return ActuatorResultStatus.InProgress;
			case 5:
			case 6:
			case 7:
			case 10:
				return ActuatorResultStatus.Aborted;
			case 8:
				return ActuatorResultStatus.NotStarted;
			case 9:
				return ActuatorResultStatus.NoCommunication;
			}
		}
		return ActuatorResultStatus.Unknown;
	}

	private void PreInstallationStatus_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= PreInstallationStatus_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			flagDisplaySra5StatusCode = true;
			string text = service.OutputValues[0].Value.ToString();
			switch (GetResultStatus(service.OutputValues[0]))
			{
			case ActuatorResultStatus.InProgress:
				operationCallState = OperationCallState.Ready;
				AppendStatus(text, PictureStatus.NoChange);
				break;
			case ActuatorResultStatus.Done:
				flagDisplaySra5StatusCode = false;
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				AppendStatus(Resources.Message_PreInstallationCompletedSuccessfully, PictureStatus.ShowSuccess);
				break;
			case ActuatorResultStatus.Aborted:
			{
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				string message = string.Format(Resources.MessageFormat_ErrorPreInstallationStatusAbortedWithTheFollowingMessage0, text);
				AppendStatus(message, PictureStatus.ShowError);
				break;
			}
			case ActuatorResultStatus.NotStarted:
				AppendStatus(Resources.Message_ErrorPreInstallationStatusIndicatesServiceRoutineNotStarted, PictureStatus.ShowError);
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				break;
			case ActuatorResultStatus.NoCommunication:
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				AppendStatus(Resources.Message_ErrorPreInstallationStatusIndicatesNoCommunicationWithSRA, PictureStatus.ShowError);
				break;
			default:
			{
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				string message = string.Format(Resources.MessageFormat_ErrorPreInstallationStatusEncounteredAnUnknownStatusValue0, text);
				AppendStatus(message, PictureStatus.ShowError);
				break;
			}
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorPreInstallationStatusServiceCallFailed, PictureStatus.ShowError);
		}
	}

	private void ExecutePreInstallationStop()
	{
		if ((operationPhase != OperationPhase.Stop || operationCallState != OperationCallState.Executing) && Connected)
		{
			DisplaySra5StatusCode();
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR061_Pre_install_Routine_Stop_ActuatorNumber");
			service.ServiceCompleteEvent += PreInstallationStop_ServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private void PreInstallationStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= PreInstallationStop_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			Choice choice = service.OutputValues[0].Value as Choice;
			if (Convert.ToInt32(choice.RawValue) == 5)
			{
				AppendStatus(Resources.Message_PreInstallationStoppedSuccessfully, PictureStatus.NoChange);
			}
			else
			{
				string message = string.Format(Resources.MessageFormat_ErrorPreInstallationStopProcessFailedWithOutputValue0, choice);
				AppendStatus(message, PictureStatus.ShowError);
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorPreInstallationStopServiceCallFailed, PictureStatus.ShowError);
		}
		StopTimer();
		ClearUserInterfaceState();
	}

	private void OnPreInstallationStop_Click(object sender, EventArgs e)
	{
		buttonStart.Click -= OnPreInstallationStart_Click;
		buttonStop.Click -= OnPreInstallationStop_Click;
		buttonStop.Enabled = false;
		AppendStatus(Resources.Message_PreInstallationStopRequestedByUser, PictureStatus.NoChange);
		if (operationTimer == null)
		{
			ShowPicture(picBlank);
			StopTimer();
			ClearUserInterfaceState();
		}
		else
		{
			forcePreInstallationStop = true;
		}
	}

	private void OnSelfCalibrationStart_Click(object sender, EventArgs e)
	{
		stepNumber++;
		UpdateControls();
		AppendStatus(Resources.Message_PerformingSelfCalibration, PictureStatus.NoChange);
		operationPhase = OperationPhase.Start;
		operationCallState = OperationCallState.Ready;
		forceSelfCalibrationStop = false;
		operationTimer = new Timer();
		operationTimer.Interval = 1000;
		operationTimer.Tick += operationTimer_Tick;
		operationTimer.Start();
	}

	private void ExecuteSelfCalibrationStart()
	{
		if (Connected)
		{
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR062_Self_Calibration_Routine_Start_ActuatorStartStatus");
			service.ServiceCompleteEvent += SelfCalibrationStart_ServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private void SelfCalibrationStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		bool flag = false;
		Service service = sender as Service;
		service.ServiceCompleteEvent -= SelfCalibrationStart_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			Choice choice = service.OutputValues[0].Value as Choice;
			if (Convert.ToInt32(choice.RawValue) == 1)
			{
				AppendStatus(Resources.Message_SelfCalibrationStartedSuccessfully, PictureStatus.NoChange);
				operationPhase = OperationPhase.Status;
				operationCallState = OperationCallState.Ready;
				flag = true;
			}
			else if (Convert.ToInt32(choice.RawValue) == 2)
			{
				AppendStatus(Resources.Message_ErrorSelfCalibrationStartProcessFailed, PictureStatus.ShowError);
			}
			else
			{
				string message = string.Format(Resources.MessageFormat_ErrorSelfCalibrationStartProcessFailedWithOutputValue0, choice);
				AppendStatus(message, PictureStatus.ShowError);
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorSelfCalibrationStartServiceCallFailed, PictureStatus.ShowError);
		}
		if (!flag)
		{
			StopTimer();
			ClearUserInterfaceState();
		}
	}

	private void ExecuteSelfCalibrationStatus()
	{
		if (Connected)
		{
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR062_Self_Calibration_Routine_Request_Results_ActuatorResultStatus");
			service.ServiceCompleteEvent += SelfCalibrationStatus_ServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private void SelfCalibrationStatus_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= SelfCalibrationStatus_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			flagDisplaySra5StatusCode = true;
			string text = service.OutputValues[0].Value.ToString();
			switch (GetResultStatus(service.OutputValues[0]))
			{
			case ActuatorResultStatus.InProgress:
				operationCallState = OperationCallState.Ready;
				AppendStatus(text, PictureStatus.NoChange);
				break;
			case ActuatorResultStatus.Done:
				flagDisplaySra5StatusCode = false;
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				AppendStatus(Resources.Message_SelfCalibrationCompletedSuccessfully, PictureStatus.ShowSuccess);
				break;
			case ActuatorResultStatus.Aborted:
			{
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				string message = string.Format(Resources.MessageFormat_ErrorSelfCalibrationStatusAbortedWithTheFollowingMessage0, text);
				AppendStatus(message, PictureStatus.ShowError);
				break;
			}
			case ActuatorResultStatus.NotStarted:
				AppendStatus(Resources.Message_ErrorSelfCalibrationStatusIndicatesServiceRoutineNotStarted, PictureStatus.ShowError);
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				break;
			case ActuatorResultStatus.NoCommunication:
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				AppendStatus(Resources.Message_ErrorSelfCalibrationStatusIndicatesNoCommunicationWithSRA, PictureStatus.ShowError);
				break;
			default:
			{
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				string message = string.Format(Resources.MessageFormat_ErrorSelfCalibrationStatusEncounteredAnUnknownStatusValue0, text);
				AppendStatus(message, PictureStatus.ShowError);
				break;
			}
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorSelfCalibrationStatusServiceCallFailed, PictureStatus.ShowError);
		}
	}

	private void ExecuteSelfCalibrationStop()
	{
		if ((operationPhase != OperationPhase.Stop || operationCallState != OperationCallState.Executing) && Connected)
		{
			DisplaySra5StatusCode();
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR062_Self_Calibration_Routine_Stop_ActuatorNumber");
			service.ServiceCompleteEvent += SelfCalibrationStop_ServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private void SelfCalibrationStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= SelfCalibrationStop_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			Choice choice = service.OutputValues[0].Value as Choice;
			if (Convert.ToInt32(choice.RawValue) == 5)
			{
				AppendStatus(Resources.Message_SelfCalibrationStoppedSuccessfully, PictureStatus.NoChange);
			}
			else
			{
				string message = string.Format(Resources.MessageFormat_ErrorSelfCalibrationStopProcessFailedWithOutputValue0, choice);
				AppendStatus(message, PictureStatus.ShowError);
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorSelfCalibrationStopServiceCallFailed, PictureStatus.ShowError);
		}
		StopTimer();
		ClearUserInterfaceState();
	}

	private void OnSelfCalibrationStop_Click(object sender, EventArgs e)
	{
		buttonStart.Click -= OnSelfCalibrationStart_Click;
		buttonStop.Click -= OnSelfCalibrationStop_Click;
		buttonStop.Enabled = false;
		AppendStatus(Resources.Message_SelfCalibrationStopRequestedByUser, PictureStatus.NoChange);
		if (operationTimer == null)
		{
			ShowPicture(picBlank);
			StopTimer();
			ClearUserInterfaceState();
		}
		else
		{
			forceSelfCalibrationStop = true;
		}
	}

	private void OnHysteresisTestStart_Click(object sender, EventArgs e)
	{
		stepNumber++;
		UpdateControls();
		AppendStatus(Resources.Message_PerformingHysteresisTest, PictureStatus.NoChange);
		operationPhase = OperationPhase.Start;
		operationCallState = OperationCallState.Ready;
		forceHysteresisTestStop = false;
		hysteresisSuccessful = false;
		operationTimer = new Timer();
		operationTimer.Interval = 1;
		operationTimer.Tick += operationTimer_Tick;
		operationTimer.Start();
	}

	private void ExecuteHysteresisTestStart()
	{
		if (Connected)
		{
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Start_ActuatorStartStatus");
			service.ServiceCompleteEvent += HysteresisTestStart_ServiceCompleteEvent;
			SetupInstruments();
			SetupHysteresisServiceRoutines();
			hysteresisTest.Reset();
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private void HysteresisTestStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		bool flag = false;
		Service service = sender as Service;
		service.ServiceCompleteEvent -= HysteresisTestStart_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			Choice choice = service.OutputValues[0].Value as Choice;
			if (Convert.ToInt32(choice.RawValue) == 1)
			{
				AppendStatus(Resources.Message_HysteresisTestStartedSuccessfully, PictureStatus.NoChange);
				operationPhase = OperationPhase.Data;
				operationCallState = OperationCallState.Ready;
				flag = true;
			}
			else if (Convert.ToInt32(choice.RawValue) == 2)
			{
				AppendStatus(Resources.Message_ErrorHysteresisTestStartProcessFailed, PictureStatus.ShowError);
			}
			else
			{
				string message = string.Format(Resources.MessageFormat_ErrorHysteresisTestStartProcessFailedWithOutputValue0, choice);
				AppendStatus(message, PictureStatus.ShowError);
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorHysteresisTestStartServiceCallFailed, PictureStatus.ShowError);
		}
		if (!flag)
		{
			RestoreInstruments();
			RestoreHysteresisServiceRoutines();
			StopTimer();
			ClearUserInterfaceState();
		}
	}

	private void ExecuteHysteresisTestData()
	{
		if (Connected)
		{
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data");
			service.ServiceCompleteEvent += HysteresisTestData_ServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private void HysteresisTestData_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Expected O, but got Unknown
		Service service = sender as Service;
		service.ServiceCompleteEvent -= HysteresisTestData_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			flagDisplaySra5StatusCode = true;
			string text = service.OutputValues[0].Value.ToString();
			byte b = Convert.ToByte(text.Substring(2, 2));
			byte b2 = b;
			bool flag = false;
			switch (b)
			{
			case 2:
			case 3:
			{
				hysteresisTest.NewDataItem();
				ushort currentPosition = Convert.ToUInt16(text.Substring(4, 4), 16);
				hysteresisTest.SetCurrentPosition(currentPosition);
				ushort targetPosition = Convert.ToUInt16(text.Substring(8, 4), 16);
				hysteresisTest.SetTargetPosition(targetPosition);
				byte motorEffort = Convert.ToByte(text.Substring(12, 2), 16);
				hysteresisTest.SetMotorEffort(motorEffort);
				hysteresisTest.MarkAsComplete();
				operationPhase = OperationPhase.Data;
				operationCallState = OperationCallState.Ready;
				break;
			}
			case 1:
			{
				flagDisplaySra5StatusCode = false;
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				AppendStatus(Resources.Message_HysteresisTestCompletedSuccessfully, PictureStatus.ShowSuccess);
				hysteresisSuccessful = true;
				bool flag2 = hysteresisTest.DetermineStrokeRanges();
				string message2 = string.Format(Resources.MessageFormat_HysteresisTestDataIndexes01234, hysteresisTest.OutstrokeBeginIndex, hysteresisTest.OutstrokeEndIndex, hysteresisTest.BackstrokeBeginIndex, hysteresisTest.BackstrokeEndIndex, flag2 ? Resources.Message_Passed : Resources.Message_Failed3);
				AppendStatus(message2, PictureStatus.NoChange);
				if (flag2)
				{
					flag2 = hysteresisTest.CalculateAverageOutstrokeEffort();
					message2 = string.Format(Resources.MessageFormat_HysteresisTestOutstrokeAverageEffort01, hysteresisTest.OutstrokeAverageEffort.ToString("0.00"), flag2 ? Resources.Message_Passed3 : Resources.Message_Failed6);
					AppendStatus(message2, PictureStatus.NoChange);
				}
				if (flag2)
				{
					flag2 = hysteresisTest.CalculateAverageBackstrokeEffort();
					message2 = string.Format(Resources.MessageFormat_HysteresisTestBackstrokeAverageEffort01, hysteresisTest.BackstrokeAverageEffort.ToString("0.00"), flag2 ? Resources.Message_Passed2 : Resources.Message_Failed5);
					AppendStatus(message2, PictureStatus.NoChange);
				}
				if (flag2)
				{
					flag2 = hysteresisTest.TestPositionDeviationThresholds();
					message2 = string.Format(Resources.MessageFormat_HysteresisTestPercentPositionDeviationCheck0, flag2 ? Resources.Message_Passed1 : Resources.Message_Failed4);
					AppendStatus(message2, PictureStatus.NoChange);
				}
				if (flag2)
				{
					flag = true;
					AppendStatus(Resources.Message_HysteresisTestPassed, PictureStatus.ShowSuccess);
				}
				else
				{
					b2 = (byte)hysteresisTest.ErrorType;
					message2 = ((hysteresisTest.ErrorType != HysteresisErrorType.PositionDeviationThreshold) ? Resources.Message_HysteresisTestFailed : string.Format(Resources.MessageFormat_HysteresisTestFailedOnThe0AtTargetPosition1Index2, (hysteresisTest.ErrorMotorDirection == HysteresisMotorDirection.Outstroke) ? Resources.Message_Outstroke : Resources.Message_Backstroke, hysteresisTest.ErrorTargetPosition, hysteresisTest.ErrorItemIndex));
					AppendStatus(message2, PictureStatus.ShowError);
				}
				break;
			}
			case 5:
			case 6:
			{
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				string message = string.Format(Resources.MessageFormat_ErrorHysteresisTestDataAbortedCode0AbortedTimeout, b);
				AppendStatus(message, PictureStatus.ShowError);
				break;
			}
			case 8:
				AppendStatus(Resources.Message_ErrorHysteresisTestDataIndicatesServiceRoutineNotStarted, PictureStatus.ShowError);
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				break;
			case 9:
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				AppendStatus(Resources.Message_ErrorHysteresisTestStatusIndicatesNoCommunicationWithSRA, PictureStatus.ShowError);
				break;
			default:
			{
				operationPhase = OperationPhase.Stop;
				operationCallState = OperationCallState.Ready;
				string message = string.Format(Resources.MessageFormat_ErrorHysteresisTestStatusEncounteredAnUnknownStatusValue0, b);
				AppendStatus(message, PictureStatus.ShowError);
				break;
			}
			}
			if (operationPhase == OperationPhase.Stop)
			{
				ServiceCode val = new ServiceCode(SapiManager.GetEngineSerialNumber(service.Channel), (byte)3, b2, flag, DateTime.Now);
				val.ShowMessage(Resources.Message_TurboActuatorHysteresisTestEPA07);
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorHysteresisTestDataServiceCallFailed, PictureStatus.ShowError);
		}
	}

	private void ExecuteHysteresisTestStop()
	{
		if ((operationPhase != OperationPhase.Stop || operationCallState != OperationCallState.Executing) && Connected)
		{
			DisplaySra5StatusCode();
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR063_Hysteres_Test_Routine_Stop_ActuatorNumber");
			service.ServiceCompleteEvent += HysteresisTestStop_ServiceCompleteEvent;
			service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(5);
			operationCallState = OperationCallState.Executing;
			service.Execute(synchronous: false);
		}
	}

	private void HysteresisTestStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= HysteresisTestStop_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			Choice choice = service.OutputValues[0].Value as Choice;
			if (Convert.ToInt32(choice.RawValue) == 5)
			{
				AppendStatus(Resources.Message_HysteresisTestStoppedSuccessfully, PictureStatus.NoChange);
			}
			else
			{
				string message = string.Format(Resources.MessageFormat_ErrorHysteresisTestStopProcessFailedWithOutputValue0, choice);
				AppendStatus(message, PictureStatus.ShowError);
			}
		}
		else
		{
			AppendStatus(Resources.Message_ErrorHysteresisTestStopServiceCallFailed, PictureStatus.ShowError);
		}
		RestoreInstruments();
		RestoreHysteresisServiceRoutines();
		StopTimer();
		ClearUserInterfaceState();
	}

	private void OnHysteresisTestStop_Click(object sender, EventArgs e)
	{
		buttonStart.Click -= OnHysteresisTestStart_Click;
		buttonStop.Click -= OnHysteresisTestStop_Click;
		buttonStop.Enabled = false;
		AppendStatus(Resources.Message_HysteresisTestStopRequestedByUser, PictureStatus.NoChange);
		if (operationTimer == null)
		{
			RestoreInstruments();
			RestoreHysteresisServiceRoutines();
			ShowPicture(picBlank);
			StopTimer();
			ClearUserInterfaceState();
		}
		else
		{
			forceHysteresisTestStop = true;
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		tableLayoutPanel3 = new TableLayoutPanel();
		panel3 = new Panel();
		buttonStart = new Button();
		buttonNext = new Button();
		buttonPrevious = new Button();
		buttonHysteresisTest = new Button();
		buttonPreInstallation = new Button();
		buttonSelfCalibration = new Button();
		closeButton = new Button();
		buttonStop = new Button();
		picWait = new PictureBox();
		picAlignHoles = new PictureBox();
		picHysteresis = new PictureBox();
		picError = new PictureBox();
		picGrease = new PictureBox();
		picMount = new PictureBox();
		picBlank = new PictureBox();
		picNotMounted = new PictureBox();
		picOk = new PictureBox();
		textboxInstructions = new TextBox();
		lblInstructions = new Label();
		lblStatus = new Label();
		textboxStatus = new TextBox();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		panel3.SuspendLayout();
		((ISupportInitialize)picWait).BeginInit();
		((ISupportInitialize)picAlignHoles).BeginInit();
		((ISupportInitialize)picHysteresis).BeginInit();
		((ISupportInitialize)picError).BeginInit();
		((ISupportInitialize)picGrease).BeginInit();
		((ISupportInitialize)picMount).BeginInit();
		((ISupportInitialize)picBlank).BeginInit();
		((ISupportInitialize)picNotMounted).BeginInit();
		((ISupportInitialize)picOk).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStart, 7, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonNext, 6, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonPrevious, 5, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonHysteresisTest, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonPreInstallation, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSelfCalibration, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(closeButton, 8, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStop, 4, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonNext, "buttonNext");
		buttonNext.Name = "buttonNext";
		buttonNext.UseCompatibleTextRendering = true;
		buttonNext.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonPrevious, "buttonPrevious");
		buttonPrevious.Name = "buttonPrevious";
		buttonPrevious.UseCompatibleTextRendering = true;
		buttonPrevious.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonHysteresisTest, "buttonHysteresisTest");
		buttonHysteresisTest.Name = "buttonHysteresisTest";
		buttonHysteresisTest.UseCompatibleTextRendering = true;
		buttonHysteresisTest.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonPreInstallation, "buttonPreInstallation");
		buttonPreInstallation.Name = "buttonPreInstallation";
		buttonPreInstallation.UseCompatibleTextRendering = true;
		buttonPreInstallation.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonSelfCalibration, "buttonSelfCalibration");
		buttonSelfCalibration.Name = "buttonSelfCalibration";
		buttonSelfCalibration.UseCompatibleTextRendering = true;
		buttonSelfCalibration.UseVisualStyleBackColor = true;
		closeButton.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(closeButton, "closeButton");
		closeButton.Name = "closeButton";
		closeButton.UseCompatibleTextRendering = true;
		closeButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)tableLayoutPanel3, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)lblInstructions, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)lblStatus, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)tableLayoutPanel1, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxStatus, 0, 3);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(panel3, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(textboxInstructions, 1, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(panel3, "panel3");
		panel3.Controls.Add(picWait);
		panel3.Controls.Add(picAlignHoles);
		panel3.Controls.Add(picHysteresis);
		panel3.Controls.Add(picError);
		panel3.Controls.Add(picGrease);
		panel3.Controls.Add(picMount);
		panel3.Controls.Add(picBlank);
		panel3.Controls.Add(picNotMounted);
		panel3.Controls.Add(picOk);
		panel3.Name = "panel3";
		componentResourceManager.ApplyResources(picWait, "picWait");
		picWait.Name = "picWait";
		picWait.TabStop = false;
		componentResourceManager.ApplyResources(picAlignHoles, "picAlignHoles");
		picAlignHoles.Name = "picAlignHoles";
		picAlignHoles.TabStop = false;
		componentResourceManager.ApplyResources(picHysteresis, "picHysteresis");
		picHysteresis.Name = "picHysteresis";
		picHysteresis.TabStop = false;
		componentResourceManager.ApplyResources(picError, "picError");
		picError.Name = "picError";
		picError.TabStop = false;
		componentResourceManager.ApplyResources(picGrease, "picGrease");
		picGrease.Name = "picGrease";
		picGrease.TabStop = false;
		componentResourceManager.ApplyResources(picMount, "picMount");
		picMount.Name = "picMount";
		picMount.TabStop = false;
		componentResourceManager.ApplyResources(picBlank, "picBlank");
		picBlank.Name = "picBlank";
		picBlank.TabStop = false;
		componentResourceManager.ApplyResources(picNotMounted, "picNotMounted");
		picNotMounted.Name = "picNotMounted";
		picNotMounted.TabStop = false;
		componentResourceManager.ApplyResources(picOk, "picOk");
		picOk.Name = "picOk";
		picOk.TabStop = false;
		componentResourceManager.ApplyResources(textboxInstructions, "textboxInstructions");
		textboxInstructions.Name = "textboxInstructions";
		textboxInstructions.ReadOnly = true;
		lblInstructions.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(lblInstructions, "lblInstructions");
		((Control)(object)lblInstructions).Name = "lblInstructions";
		lblInstructions.Orientation = (TextOrientation)1;
		lblStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(lblStatus, "lblStatus");
		((Control)(object)lblStatus).Name = "lblStatus";
		lblStatus.Orientation = (TextOrientation)1;
		componentResourceManager.ApplyResources(textboxStatus, "textboxStatus");
		textboxStatus.Name = "textboxStatus";
		textboxStatus.ReadOnly = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_TurboActuator");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).PerformLayout();
		panel3.ResumeLayout(performLayout: false);
		panel3.PerformLayout();
		((ISupportInitialize)picWait).EndInit();
		((ISupportInitialize)picAlignHoles).EndInit();
		((ISupportInitialize)picHysteresis).EndInit();
		((ISupportInitialize)picError).EndInit();
		((ISupportInitialize)picGrease).EndInit();
		((ISupportInitialize)picMount).EndInit();
		((ISupportInitialize)picBlank).EndInit();
		((ISupportInitialize)picNotMounted).EndInit();
		((ISupportInitialize)picOk).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
