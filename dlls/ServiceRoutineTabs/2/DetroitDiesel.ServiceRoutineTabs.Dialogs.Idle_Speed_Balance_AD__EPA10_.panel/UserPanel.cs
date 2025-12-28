using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__EPA10_.panel;

public class UserPanel : CustomPanel
{
	private enum IdleStates
	{
		Idling,
		NotIdling,
		Stopped,
		NotDetected
	}

	private const string StartServiceName = "RT_SR066_Idle_Speed_Balance_Test_Start";

	private const string EngineStateInstrumentQualifier = "DT_ASL004_Engine_State";

	private const int MaxCylinders = 6;

	private const double minCylinderTolerance = -99.5;

	private const double maxCylinderTolerance = 99.5;

	private static readonly List<SetupInformation> Setups = new List<SetupInformation>(new SetupInformation[3]
	{
		new SetupInformation("DD15", "DD15"),
		new SetupInformation("DD16", "DD16"),
		new SetupInformation("DD13", "DD13")
	});

	private string TestSuccessfulMessage = Resources.Message_TheTestCompletedSuccessfully;

	private string ErrorMessage = Resources.Message_ErrorsOccurredDuringTheTest;

	private string ServiceNotSupportedMessage = Resources.MessageFormat_TheConnectedMCM2DoesNotSupportTheServiceRoutine0;

	private string EcuNotConnectedMessage = Resources.Message_MCM2IsNotConnected;

	private string EcuNotMbeMessage = Resources.Message_MCM2IsConnectedButEngineTypeIsNotSupported;

	private string EcuReadyMessage = Resources.Message_MCM2IsConnectedAndEngineTypeIsSupported;

	private string EcuBusyMessage = Resources.Message_MCM2IsConnectedButIsBusy;

	private string EngineAtIdleMessage = Resources.Message_EngineIsAtIdle;

	private string EngineStoppedMessage = Resources.Message_EngineIsStoppedStartTheEngineToProceed;

	private string EngineStateNotIdleMessage = Resources.Message_TheEngineIsNotAtIdle;

	private string EngineStateNotDetectedMessage = Resources.Message_CannotDetectIfEngineIsStarted;

	private string VehicleStatusCheckOkMessage = Resources.Message_VehicleStatusIsOK;

	private string VehicleStatusCheckNotOkMessage = Resources.Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON;

	private readonly Qualifier qualifier1 = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_1");

	private readonly Qualifier qualifier2 = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_2");

	private readonly Qualifier qualifier3 = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_3");

	private readonly Qualifier qualifier4 = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_4");

	private readonly Qualifier qualifier5 = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_5");

	private readonly Qualifier qualifier6 = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_6");

	private Dictionary<DataItem, ScalingLabel> labelMap = new Dictionary<DataItem, ScalingLabel>();

	private int[] firingOrder = new int[6] { 3, 5, 4, 1, 0, 2 };

	private WarningManager warningManager;

	private bool adrReturnValue = false;

	private string adrReturnText = "test was not ran";

	private Channel mcm;

	private Instrument engineState;

	private bool success = true;

	private int cylinder;

	private SetupInformation connectedEcuType;

	private IdleStates idlingState = IdleStates.NotDetected;

	private bool testRunning;

	private bool testStarted;

	private TableLayoutPanel tableLayoutPanel1;

	private Label engineIdlingStatus;

	private Label mcmConnectionStatus;

	private Label temperatureStatus;

	private TableLayoutPanel tableLayoutPanel3;

	private DigitalReadoutInstrument vehicleCheckInstrument;

	private Label vehicleStatus;

	private BarInstrument fuelTemperatureInstrument;

	private BarInstrument coolantTemperatureInstrument;

	private Button buttonExecute;

	private Button buttonClose;

	private Checkmark connectionCheck;

	private Checkmark testReadyCheck;

	private TableLayoutPanel tableLayoutPanel2;

	private Label cylinderLabel6;

	private Label cylinderLabel3;

	private Label cylinderLabel4;

	private Label cylinderLabel5;

	private Label cylinderLabel2;

	private Label cylinderLabel1;

	private ScalingLabel scalingLabel1;

	private ScalingLabel scalingLabel2;

	private ScalingLabel scalingLabel3;

	private ScalingLabel scalingLabel4;

	private ScalingLabel scalingLabel5;

	private ScalingLabel scalingLabel6;

	private Checkmark temperatureCheck;

	private Checkmark vehicleStatusCheck;

	private Checkmark idlingCheck;

	private System.Windows.Forms.Label testStatusLabel;

	private SeekTimeListView seekTimeListView;

	private bool EcuReady => EcuCorrectType && mcm.CommunicationsState == CommunicationsState.Online;

	private bool EcuCorrectType => IsConnected && connectedEcuType != null;

	private bool IsConnected => mcm != null && mcm.Online;

	private bool ValidTestCondition => EcuReady && IsEngineIdling && TemperaturesAreInRange && VehicleCheckStatusOk;

	public bool IsEngineIdling
	{
		get
		{
			return idlingState == IdleStates.Idling;
		}
		set
		{
			idlingState = ((!value) ? IdleStates.NotIdling : IdleStates.Idling);
			UpdateTestReadiness();
		}
	}

	private bool TemperaturesAreInRange => (int)fuelTemperatureInstrument.RepresentedState == 1 && (int)coolantTemperatureInstrument.RepresentedState == 1;

	private bool VehicleCheckStatusOk => (int)vehicleCheckInstrument.RepresentedState == 1;

	public UserPanel()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(string.Empty, "idle speed balance test", seekTimeListView.RequiredUserLabelPrefix);
		scalingLabel1.AddScalingString("-100");
		scalingLabel2.AddScalingString("-100");
		scalingLabel3.AddScalingString("-100");
		scalingLabel4.AddScalingString("-100");
		scalingLabel5.AddScalingString("-100");
		scalingLabel6.AddScalingString("-100");
		buttonExecute.Click += OnButtonClick;
		fuelTemperatureInstrument.RepresentedStateChanged += OnTemperatureStateChanged;
		coolantTemperatureInstrument.RepresentedStateChanged += OnTemperatureStateChanged;
		vehicleCheckInstrument.RepresentedStateChanged += OnVehicleCheckStateChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			((Control)this).Tag = new object[2] { adrReturnValue, adrReturnText };
		}
	}

	public void OnButtonClick(object sender, EventArgs e)
	{
		StartTest();
	}

	protected override void Dispose(bool disposing)
	{
		SetMCM(null);
		DisconnectDataItems();
		((CustomPanel)this).Dispose(disposing);
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM02T"));
		ConnectDataItems();
		UpdateTestReadiness();
	}

	private void OnDataItemUpdate(object sender, ResultEventArgs e)
	{
		DataItem val = (DataItem)((sender is DataItem) ? sender : null);
		if (val != null && labelMap.ContainsKey(val))
		{
			ScalingLabel val2 = labelMap[val];
			if (val2 != null)
			{
				UpdateInstrumentValue(val2, val);
			}
		}
	}

	private void SetMCM(Channel mcm)
	{
		if (this.mcm == mcm)
		{
			return;
		}
		warningManager.Reset();
		testRunning = false;
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			if (engineState != null)
			{
				engineState.InstrumentUpdateEvent -= OnEngineStateUpdate;
			}
			idlingState = IdleStates.NotDetected;
		}
		this.mcm = mcm;
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			engineState = this.mcm.Instruments["DT_ASL004_Engine_State"];
			if (engineState != null)
			{
				engineState.InstrumentUpdateEvent += OnEngineStateUpdate;
				UpdateEngineState();
			}
		}
		UpdateConnectedEcuType();
	}

	private void DisconnectDataItems()
	{
		foreach (DataItem key in labelMap.Keys)
		{
			key.UpdateEvent -= OnDataItemUpdate;
		}
		labelMap.Clear();
	}

	private void ConnectDataItems()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		DisconnectDataItems();
		DataItem val = DataItem.Create(qualifier1, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
		DataItem val2 = DataItem.Create(qualifier2, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
		DataItem val3 = DataItem.Create(qualifier3, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
		DataItem val4 = DataItem.Create(qualifier4, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
		DataItem val5 = DataItem.Create(qualifier5, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
		DataItem val6 = DataItem.Create(qualifier6, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
		if (val != null && val2 != null && val3 != null && val4 != null && val5 != null && val6 != null)
		{
			labelMap.Add(val, scalingLabel1);
			labelMap.Add(val2, scalingLabel2);
			labelMap.Add(val3, scalingLabel3);
			labelMap.Add(val4, scalingLabel4);
			labelMap.Add(val5, scalingLabel5);
			labelMap.Add(val6, scalingLabel6);
			{
				foreach (DataItem key in labelMap.Keys)
				{
					key.UpdateEvent += OnDataItemUpdate;
					UpdateInstrumentValue(labelMap[key], key);
				}
				return;
			}
		}
		((Control)(object)scalingLabel1).Text = string.Empty;
		((Control)(object)scalingLabel2).Text = string.Empty;
		((Control)(object)scalingLabel3).Text = string.Empty;
		((Control)(object)scalingLabel4).Text = string.Empty;
		((Control)(object)scalingLabel5).Text = string.Empty;
		((Control)(object)scalingLabel6).Text = string.Empty;
	}

	private void UpdateTestReadiness()
	{
		UpdateEcuReadyCheck();
		UpdateEngineIdleCheck();
		UpdateTemperatureCheck();
		UpdateVehicleCheckStatus();
		buttonExecute.Enabled = !testRunning && ValidTestCondition;
		testReadyCheck.Checked = ValidTestCondition;
	}

	private string GetOkMinimumString(Gradient gradient)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		foreach (GradientCell cell in gradient.Cells)
		{
			GradientCell current = cell;
			if ((int)((GradientCell)(ref current)).State == 1)
			{
				Conversion conversion = Converter.GlobalInstance.GetConversion(gradient.Units);
				if (conversion == null)
				{
					return string.Format(CultureInfo.CurrentCulture, "{0}{1}", Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)((GradientCell)(ref current)).LowerBoundary, gradient.Units, -1), gradient.Units);
				}
				return string.Format(CultureInfo.CurrentCulture, "{0}{1}", Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)((GradientCell)(ref current)).LowerBoundary, conversion, -1), conversion.OutputUnit);
			}
		}
		return Resources.Message_Unknown;
	}

	private void UpdateTemperatureCheck()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Invalid comparison between Unknown and I4
		temperatureCheck.CheckState = (TemperaturesAreInRange ? CheckState.Checked : CheckState.Unchecked);
		if (TemperaturesAreInRange)
		{
			((Control)(object)temperatureStatus).Text = Resources.Message_FuelAndCoolantTemperaturesAreInRange;
			return;
		}
		string text = string.Empty;
		if ((int)fuelTemperatureInstrument.RepresentedState != 1)
		{
			text += string.Format(Resources.MessageFormat_FuelTemperatureMustBeAtLeast0, GetOkMinimumString(((AxisSingleInstrumentBase)fuelTemperatureInstrument).Gradient));
		}
		if ((int)coolantTemperatureInstrument.RepresentedState != 1)
		{
			text += string.Format(Resources.MessageFormat_CoolantTemperatureMustBeAtLeast0, GetOkMinimumString(((AxisSingleInstrumentBase)coolantTemperatureInstrument).Gradient));
		}
		((Control)(object)temperatureStatus).Text = text;
	}

	private void UpdateEcuReadyCheck()
	{
		connectionCheck.CheckState = (IsConnected ? CheckState.Checked : CheckState.Unchecked);
		if (IsConnected)
		{
			if (EcuCorrectType)
			{
				if (EcuReady)
				{
					((Control)(object)mcmConnectionStatus).Text = EcuReadyMessage;
				}
				else
				{
					((Control)(object)mcmConnectionStatus).Text = EcuBusyMessage;
				}
			}
			else
			{
				((Control)(object)mcmConnectionStatus).Text = EcuNotMbeMessage;
			}
		}
		else
		{
			((Control)(object)mcmConnectionStatus).Text = EcuNotConnectedMessage;
		}
	}

	private void UpdateEngineIdleCheck()
	{
		idlingCheck.CheckState = (IsEngineIdling ? CheckState.Checked : CheckState.Unchecked);
		switch (idlingState)
		{
		case IdleStates.Idling:
			((Control)(object)engineIdlingStatus).Text = EngineAtIdleMessage;
			break;
		case IdleStates.Stopped:
			((Control)(object)engineIdlingStatus).Text = EngineStoppedMessage;
			break;
		case IdleStates.NotIdling:
			((Control)(object)engineIdlingStatus).Text = EngineStateNotIdleMessage;
			break;
		case IdleStates.NotDetected:
			((Control)(object)engineIdlingStatus).Text = EngineStateNotDetectedMessage;
			break;
		default:
			throw new ArgumentOutOfRangeException("Unknown idle state.");
		}
	}

	private void UpdateVehicleCheckStatus()
	{
		vehicleStatusCheck.CheckState = (VehicleCheckStatusOk ? CheckState.Checked : CheckState.Unchecked);
		((Control)(object)vehicleStatus).Text = (VehicleCheckStatusOk ? VehicleStatusCheckOkMessage : VehicleStatusCheckNotOkMessage);
	}

	private void UpdateConnectedEcuType()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (mcm == null)
		{
			return;
		}
		IEnumerable<EquipmentType> enumerable = EquipmentType.ConnectedEquipmentTypes("Engine");
		if (!CollectionExtensions.Exactly<EquipmentType>(enumerable, 1))
		{
			return;
		}
		EquipmentType val = enumerable.First();
		string name = ((EquipmentType)(ref val)).Name;
		foreach (SetupInformation setup in Setups)
		{
			if (setup.Name == name)
			{
				if (setup != connectedEcuType)
				{
					connectedEcuType = setup;
					UpdateTestReadiness();
				}
				break;
			}
		}
	}

	private void StartTest()
	{
		if (warningManager.RequestContinue())
		{
			testRunning = true;
			if (!ValidTestCondition)
			{
				UpdateTestReadiness();
				return;
			}
			AppendDisplayMessage(Resources.Message_StartingTest);
			success = (testStarted = true);
			cylinder = 0;
			testStatusLabel.Text = string.Empty;
			Advance();
		}
	}

	private void Advance()
	{
		Service service = mcm.Services["RT_SR066_Idle_Speed_Balance_Test_Start"];
		if (service != null)
		{
			if (++cylinder <= 6 && testRunning)
			{
				service.InputValues[0].Value = service.InputValues[0].Choices[cylinder - 1];
				if (service.InputValues.Count > 1)
				{
					service.InputValues[1].Value = service.InputValues[1].Choices.GetItemFromRawValue(1);
				}
				service.ServiceCompleteEvent += OnServiceComplete;
				service.Execute(synchronous: false);
			}
			else
			{
				testRunning = false;
				AppendDisplayMessage(success ? TestSuccessfulMessage : ErrorMessage);
				UpdateTestReadiness();
			}
		}
		else
		{
			AppendDisplayMessage(string.Format(ServiceNotSupportedMessage, "RT_SR066_Idle_Speed_Balance_Test_Start"));
		}
	}

	private void AppendDisplayMessage(string txt)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, txt);
		((CustomPanel)this).AddStatusMessage(txt);
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateConnectedEcuType();
		UpdateTestReadiness();
	}

	private void OnEngineStateUpdate(object sender, ResultEventArgs e)
	{
		UpdateEngineState();
	}

	private void OnTemperatureStateChanged(object sender, EventArgs e)
	{
		UpdateTestReadiness();
	}

	private void OnVehicleCheckStateChanged(object sender, EventArgs e)
	{
		UpdateTestReadiness();
	}

	private void UpdateEngineState()
	{
		idlingState = IdleStates.NotIdling;
		if (engineState != null && engineState.InstrumentValues.Current != null && engineState.Choices != null)
		{
			object value = engineState.InstrumentValues.Current.Value;
			if (value == engineState.Choices.GetItemFromRawValue(0))
			{
				idlingState = IdleStates.Stopped;
			}
			else if (value == engineState.Choices.GetItemFromRawValue(3))
			{
				idlingState = IdleStates.Idling;
			}
			else if (value == engineState.Choices.GetItemFromRawValue(-1))
			{
				idlingState = IdleStates.NotDetected;
			}
		}
		UpdateTestReadiness();
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		Service service = (Service)sender;
		service.ServiceCompleteEvent -= OnServiceComplete;
		if (!e.Succeeded)
		{
			success = false;
			if (e.Exception != null && e.Exception.Message != null)
			{
				AppendDisplayMessage(e.Exception.Message + Resources.Message_WhileTestingCylinder + cylinder);
			}
			else
			{
				AppendDisplayMessage(Resources.Message_Cylinder + cylinder + Resources.Message_TestWasNotSuccessful);
			}
			testRunning = false;
		}
		else
		{
			AppendDisplayMessage(Resources.Message_Cylinder1 + cylinder + Resources.Message_TestWasSuccessful);
		}
		Advance();
	}

	private void UpdateInstrumentValue(ScalingLabel scalingLabel, DataItem dataItem)
	{
		string text = string.Empty;
		if (dataItem != null && dataItem.Value != null)
		{
			text = dataItem.ValueAsString(dataItem.Value);
		}
		((Control)(object)scalingLabel).Text = text;
		HighlightValues();
	}

	private void HighlightValues()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<DataItem, ScalingLabel>> list = new List<KeyValuePair<DataItem, ScalingLabel>>(labelMap);
		List<int> list2 = new List<int>();
		List<Tuple<int, int>> list3 = new List<Tuple<int, int>>();
		string text = string.Empty;
		for (int i = 0; i < list.Count; i++)
		{
			ValueState representedState = (ValueState)0;
			double num = list[i].Key.ValueAsDouble(list[i].Key.Value);
			if (num > 99.5 || num < -99.5)
			{
				int num2 = firingOrder[i];
				double num3 = list[num2].Key.ValueAsDouble(list[num2].Key.Value);
				if (num3 > 99.5 || num3 < -99.5)
				{
					representedState = (ValueState)2;
					list3.Add(new Tuple<int, int>(i + 1, num2 + 1));
				}
				else
				{
					representedState = (ValueState)3;
					list2.Add(i + 1);
				}
			}
			list[i].Value.RepresentedState = representedState;
		}
		if (!testStarted)
		{
			return;
		}
		if (success)
		{
			adrReturnValue = list2.Count == 0;
			if (list2.Any())
			{
				text = string.Join(", <br />", list2.Select((int num4) => string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_Cylinder0MayBeInFaultRN, num4)));
			}
			if (list3.Any())
			{
				text += string.Join(", <br />", list3.Select((Tuple<int, int> tuple) => string.Format(CultureInfo.InvariantCulture, Resources.MessageFormat_Cylinder0MightBeCompensatingForCylinder1RN, tuple.Item1, tuple.Item2)));
			}
			adrReturnText = "<br />" + text;
		}
		else
		{
			adrReturnText = ErrorMessage;
		}
		testStatusLabel.Text = Resources.Message_TestCompleteCloseThisWindowToContinueTroubleshooting;
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efc: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel3 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		connectionCheck = new Checkmark();
		vehicleCheckInstrument = new DigitalReadoutInstrument();
		engineIdlingStatus = new Label();
		testReadyCheck = new Checkmark();
		mcmConnectionStatus = new Label();
		cylinderLabel6 = new Label();
		cylinderLabel3 = new Label();
		cylinderLabel4 = new Label();
		cylinderLabel5 = new Label();
		cylinderLabel2 = new Label();
		cylinderLabel1 = new Label();
		scalingLabel1 = new ScalingLabel();
		scalingLabel2 = new ScalingLabel();
		scalingLabel3 = new ScalingLabel();
		scalingLabel4 = new ScalingLabel();
		scalingLabel5 = new ScalingLabel();
		scalingLabel6 = new ScalingLabel();
		temperatureCheck = new Checkmark();
		temperatureStatus = new Label();
		vehicleStatusCheck = new Checkmark();
		vehicleStatus = new Label();
		buttonExecute = new Button();
		idlingCheck = new Checkmark();
		seekTimeListView = new SeekTimeListView();
		buttonClose = new Button();
		fuelTemperatureInstrument = new BarInstrument();
		coolantTemperatureInstrument = new BarInstrument();
		testStatusLabel = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)connectionCheck, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)vehicleCheckInstrument, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)engineIdlingStatus, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)testReadyCheck, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)mcmConnectionStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)temperatureCheck, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)temperatureStatus, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)vehicleStatusCheck, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)vehicleStatus, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonExecute, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)idlingCheck, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 7, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(testStatusLabel, 0, 8);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(connectionCheck, "connectionCheck");
		((Control)(object)connectionCheck).Name = "connectionCheck";
		vehicleCheckInstrument.FontGroup = "idleSpeedBalancePreconditionsEPA10";
		((SingleInstrumentBase)vehicleCheckInstrument).FreezeValue = false;
		vehicleCheckInstrument.Gradient.Initialize((ValueState)0, 3);
		vehicleCheckInstrument.Gradient.Modify(1, 0.0, (ValueState)2);
		vehicleCheckInstrument.Gradient.Modify(2, 1.0, (ValueState)1);
		vehicleCheckInstrument.Gradient.Modify(3, 2.0, (ValueState)0);
		componentResourceManager.ApplyResources(vehicleCheckInstrument, "vehicleCheckInstrument");
		((SingleInstrumentBase)vehicleCheckInstrument).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)vehicleCheckInstrument).Name = "vehicleCheckInstrument";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)vehicleCheckInstrument, 4);
		((SingleInstrumentBase)vehicleCheckInstrument).UnitAlignment = StringAlignment.Near;
		engineIdlingStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(engineIdlingStatus, "engineIdlingStatus");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)engineIdlingStatus, 2);
		((Control)(object)engineIdlingStatus).Name = "engineIdlingStatus";
		engineIdlingStatus.Orientation = (TextOrientation)1;
		engineIdlingStatus.ShowBorder = false;
		engineIdlingStatus.UseSystemColors = true;
		((Control)(object)testReadyCheck).BackColor = Color.Transparent;
		testReadyCheck.CheckedImage = (Image)componentResourceManager.GetObject("testReadyCheck.CheckedImage");
		componentResourceManager.ApplyResources(testReadyCheck, "testReadyCheck");
		((Control)(object)testReadyCheck).Name = "testReadyCheck";
		testReadyCheck.UncheckedImage = (Image)componentResourceManager.GetObject("testReadyCheck.UncheckedImage");
		mcmConnectionStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(mcmConnectionStatus, "mcmConnectionStatus");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)mcmConnectionStatus, 2);
		((Control)(object)mcmConnectionStatus).Name = "mcmConnectionStatus";
		mcmConnectionStatus.Orientation = (TextOrientation)1;
		mcmConnectionStatus.ShowBorder = false;
		mcmConnectionStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 8);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)cylinderLabel6, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)cylinderLabel3, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)cylinderLabel4, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)cylinderLabel5, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)cylinderLabel2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)cylinderLabel1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabel1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabel2, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabel3, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabel4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabel5, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabel6, 2, 3);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		cylinderLabel6.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(cylinderLabel6, "cylinderLabel6");
		((Control)(object)cylinderLabel6).Name = "cylinderLabel6";
		cylinderLabel6.Orientation = (TextOrientation)1;
		cylinderLabel3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(cylinderLabel3, "cylinderLabel3");
		((Control)(object)cylinderLabel3).Name = "cylinderLabel3";
		cylinderLabel3.Orientation = (TextOrientation)1;
		cylinderLabel4.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(cylinderLabel4, "cylinderLabel4");
		((Control)(object)cylinderLabel4).Name = "cylinderLabel4";
		cylinderLabel4.Orientation = (TextOrientation)1;
		cylinderLabel5.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(cylinderLabel5, "cylinderLabel5");
		((Control)(object)cylinderLabel5).Name = "cylinderLabel5";
		cylinderLabel5.Orientation = (TextOrientation)1;
		cylinderLabel2.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(cylinderLabel2, "cylinderLabel2");
		((Control)(object)cylinderLabel2).Name = "cylinderLabel2";
		cylinderLabel2.Orientation = (TextOrientation)1;
		cylinderLabel1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(cylinderLabel1, "cylinderLabel1");
		((Control)(object)cylinderLabel1).Name = "cylinderLabel1";
		cylinderLabel1.Orientation = (TextOrientation)1;
		scalingLabel1.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabel1, "scalingLabel1");
		scalingLabel1.FontGroup = null;
		scalingLabel1.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel1).Name = "scalingLabel1";
		scalingLabel2.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabel2, "scalingLabel2");
		scalingLabel2.FontGroup = null;
		scalingLabel2.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel2).Name = "scalingLabel2";
		scalingLabel3.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabel3, "scalingLabel3");
		scalingLabel3.FontGroup = null;
		scalingLabel3.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel3).Name = "scalingLabel3";
		scalingLabel4.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabel4, "scalingLabel4");
		scalingLabel4.FontGroup = null;
		scalingLabel4.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel4).Name = "scalingLabel4";
		scalingLabel5.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabel5, "scalingLabel5");
		scalingLabel5.FontGroup = null;
		scalingLabel5.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel5).Name = "scalingLabel5";
		scalingLabel6.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabel6, "scalingLabel6");
		scalingLabel6.FontGroup = null;
		scalingLabel6.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel6).Name = "scalingLabel6";
		componentResourceManager.ApplyResources(temperatureCheck, "temperatureCheck");
		((Control)(object)temperatureCheck).Name = "temperatureCheck";
		temperatureStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)temperatureStatus, 2);
		componentResourceManager.ApplyResources(temperatureStatus, "temperatureStatus");
		((Control)(object)temperatureStatus).Name = "temperatureStatus";
		temperatureStatus.Orientation = (TextOrientation)1;
		temperatureStatus.ShowBorder = false;
		temperatureStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(vehicleStatusCheck, "vehicleStatusCheck");
		((Control)(object)vehicleStatusCheck).Name = "vehicleStatusCheck";
		vehicleStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)vehicleStatus, 2);
		componentResourceManager.ApplyResources(vehicleStatus, "vehicleStatus");
		((Control)(object)vehicleStatus).Name = "vehicleStatus";
		vehicleStatus.Orientation = (TextOrientation)1;
		vehicleStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(buttonExecute, "buttonExecute");
		buttonExecute.Name = "buttonExecute";
		buttonExecute.UseCompatibleTextRendering = true;
		buttonExecute.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(idlingCheck, "idlingCheck");
		((Control)(object)idlingCheck).Name = "idlingCheck";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 2);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Idle Speed Balance";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView, 2);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 4);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)fuelTemperatureInstrument, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)coolantTemperatureInstrument, 0, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel3, 6);
		fuelTemperatureInstrument.BarOrientation = (ControlOrientation)1;
		fuelTemperatureInstrument.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(fuelTemperatureInstrument, "fuelTemperatureInstrument");
		fuelTemperatureInstrument.FontGroup = null;
		((SingleInstrumentBase)fuelTemperatureInstrument).FreezeValue = false;
		((AxisSingleInstrumentBase)fuelTemperatureInstrument).Gradient.Initialize((ValueState)2, 1, "°C");
		((AxisSingleInstrumentBase)fuelTemperatureInstrument).Gradient.Modify(1, 10.0, (ValueState)1);
		((SingleInstrumentBase)fuelTemperatureInstrument).Instrument = new Qualifier((QualifierTypes)1, "virtual", "fuelTemp");
		((Control)(object)fuelTemperatureInstrument).Name = "fuelTemperatureInstrument";
		((SingleInstrumentBase)fuelTemperatureInstrument).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)fuelTemperatureInstrument).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)fuelTemperatureInstrument).UnitAlignment = StringAlignment.Near;
		coolantTemperatureInstrument.BarOrientation = (ControlOrientation)1;
		coolantTemperatureInstrument.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(coolantTemperatureInstrument, "coolantTemperatureInstrument");
		coolantTemperatureInstrument.FontGroup = null;
		((SingleInstrumentBase)coolantTemperatureInstrument).FreezeValue = false;
		((AxisSingleInstrumentBase)coolantTemperatureInstrument).Gradient.Initialize((ValueState)2, 1, "°C");
		((AxisSingleInstrumentBase)coolantTemperatureInstrument).Gradient.Modify(1, 70.0, (ValueState)1);
		((SingleInstrumentBase)coolantTemperatureInstrument).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)coolantTemperatureInstrument).Name = "coolantTemperatureInstrument";
		((SingleInstrumentBase)coolantTemperatureInstrument).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)coolantTemperatureInstrument).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)coolantTemperatureInstrument).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)testStatusLabel, 7);
		componentResourceManager.ApplyResources(testStatusLabel, "testStatusLabel");
		testStatusLabel.Name = "testStatusLabel";
		testStatusLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_IdleSpeedBalance");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
