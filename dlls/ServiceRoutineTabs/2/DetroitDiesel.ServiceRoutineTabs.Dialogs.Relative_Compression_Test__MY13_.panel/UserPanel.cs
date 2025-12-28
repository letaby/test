using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__MY13_.panel;

public class UserPanel : CustomPanel, IProvideHtml
{
	private const string ServiceRoutineStart = "RT_SR006_Automatic_Compression_Test_Start_Status";

	private const string ServiceRoutineStop = "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0";

	private const string ServiceRoutineStatus = "RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0";

	private ScalingLabel[] labels;

	private int CylinderCount = 6;

	private bool HasEngineBrakes = true;

	private WarningManager warningManager;

	private string resultString = Resources.Message_The_Test_Has_Not_Started;

	private bool resetJakeBrakes;

	private bool TestIsRunning;

	private Channel channel;

	private bool testSucceed = false;

	private DigitalReadoutInstrument DigitalReadoutInstrumentVehicleCheckStatus;

	private ScalingLabel CylinderOutput3;

	private Label CylinderLabel3;

	private ScalingLabel CylinderOutput2;

	private Label CylinderLabel2;

	private ScalingLabel CylinderOutput1;

	private Label CylinderLabel1;

	private ScalingLabel CylinderOutput6;

	private Label CylinderLabel6;

	private ScalingLabel CylinderOutput4;

	private Label CylinderLabel4;

	private ScalingLabel CylinderOutput5;

	private TableLayoutPanel tableLayoutPanel1;

	private SeekTimeListView seekTimeListView;

	private TableLayoutPanel tableLayoutPanel3;

	private Label labelInstructions;

	private TableLayoutPanel tableLayoutPanel2;

	private Button btnClose;

	private Button buttonRun;

	private Button buttonPrint;

	private ScalingLabel scalingLabelHasEngineBrakes;

	private Label labelHasEngineBrakesTitle;

	private DigitalReadoutInstrument engineSpeedDigitalReadoutInstrument;

	private System.Windows.Forms.Label label2;

	private TableLayoutPanel tableLayoutPanel4;

	private Label CylinderLabel5;

	public override bool CanProvideHtml => !TestIsRunning;

	private bool EngineIsRunning
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			if ((int)engineSpeedDigitalReadoutInstrument.RepresentedState != 1)
			{
				return true;
			}
			return false;
		}
	}

	public UserPanel()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		InitializeComponent();
		labels = (ScalingLabel[])(object)new ScalingLabel[6] { CylinderOutput1, CylinderOutput2, CylinderOutput3, CylinderOutput4, CylinderOutput5, CylinderOutput6 };
		warningManager = new WarningManager(string.Empty, Resources.Message_CompressionTest, seekTimeListView.RequiredUserLabelPrefix);
		((Control)(object)labelInstructions).Text = Resources.Message_StartTheTestWithTheEngineOffAndTheIgnitionOn;
		buttonRun.Click += OnExecuteButtonClick;
		buttonPrint.Click += ButtonPrintClick;
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	private bool ExecuteServiceRoutine(string serviceName, ref Service service)
	{
		bool result = false;
		if (channel == null)
		{
			DisplayStatusString(Resources.Message_TheMCMIsOfflineCannotExecuteService + serviceName + "\".");
		}
		else
		{
			service = channel.Services[serviceName];
			if (service == null)
			{
				DisplayStatusString(Resources.Message_TheMCMDoesNotSupportTheServiceRoutine + serviceName + "\".");
			}
			else
			{
				try
				{
					service.Execute(synchronous: true);
					result = true;
				}
				catch (CaesarException ex)
				{
					DisplayStatusString(ex.Message);
				}
				catch (InvalidOperationException ex2)
				{
					DisplayStatusString(ex2.Message);
				}
			}
		}
		return result;
	}

	private void GetandDisplayCylinderValue(int cylinder)
	{
		Service service = null;
		string empty = string.Empty;
		bool flag = false;
		ScalingLabel val = labels[cylinder - 1];
		string serviceName = $"RT_SR006_Automatic_Compression_Test_Request_Results_acd_cyl_{cylinder}_compression_value";
		string text;
		if (ExecuteServiceRoutine(serviceName, ref service) && service != null && service.OutputValues != null && service.OutputValues.Count > 0)
		{
			float num = (float)service.OutputValues[0].Value;
			flag = num >= 85f;
			text = $"{num,5}%";
		}
		else
		{
			text = Resources.Message_Error;
		}
		((Control)(object)val).Text = text;
		if (cylinder == 1)
		{
			testSucceed = flag;
		}
		else
		{
			testSucceed &= flag;
		}
		SetResultString(string.Format(CultureInfo.CurrentCulture, Resources.Message_Result, cylinder, text, flag ? Resources.Message_ResultPassed : Resources.Message_ResultFailed));
	}

	private void SetResultString(string resultString)
	{
		this.resultString = this.resultString + "<br />" + resultString;
	}

	private void ClearOutputs()
	{
		resultString = string.Empty;
		for (int i = 0; i < CylinderCount; i++)
		{
			((Control)(object)labels[i]).Text = string.Empty;
			labels[i].RepresentedState = (ValueState)0;
		}
	}

	private void DisplayStatusString(string statusString)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, statusString);
	}

	private void UpdateButtonStatus()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		buttonRun.Enabled = false;
		if (TestIsRunning)
		{
			((Control)(object)labelInstructions).Text = Resources.Message_TestIsRunning;
		}
		else if (channel != null)
		{
			if (channel.Parameters.HaveBeenReadFromEcu)
			{
				if (!EngineIsRunning)
				{
					if ((int)DigitalReadoutInstrumentVehicleCheckStatus.RepresentedState == 1)
					{
						buttonRun.Enabled = true;
						((Control)(object)labelInstructions).Text = Resources.Message_TheTestIsReadyToBeStarted;
					}
					else
					{
						((Control)(object)labelInstructions).Text = Resources.Message_ParkingBrakeOnTransInNeutral;
					}
				}
				else
				{
					((Control)(object)labelInstructions).Text = Resources.Message_CannotRunTestWithEngineRunning;
				}
			}
			else
			{
				((Control)(object)labelInstructions).Text = Resources.Message_ReadingEngineBrakeAvailabilityStatus;
			}
		}
		else
		{
			((Control)(object)labelInstructions).Text = Resources.Message_TheTestCannotBeRunUntilAnMCMIsConnected;
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (HasEngineBrakes)
		{
			RestoreJakeBrake();
		}
		((Control)this).Tag = new object[2] { testSucceed, resultString };
		((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
	}

	public override void OnChannelsChanged()
	{
		warningManager.Reset();
		Channel channel = ((CustomPanel)this).GetChannel("MCM21T");
		if (channel != this.channel)
		{
			if (this.channel != null)
			{
				this.channel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
			}
			this.channel = channel;
			if (this.channel != null)
			{
				this.channel.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
				if (!this.channel.Parameters.HaveBeenReadFromEcu)
				{
					this.channel.Parameters.Read(synchronous: false);
				}
				else
				{
					UpdateEngineBrakeAvailibility();
				}
			}
		}
		UpdateConnectedEquipmentType();
		UpdateButtonStatus();
	}

	private bool DisableJakeBrake()
	{
		if (channel == null)
		{
			return false;
		}
		try
		{
			if (HasEngineBrakes)
			{
				Service service = channel.Services["RT_SR003_PWM_Routine_by_Function_Start_Function_Name"];
				if (service != null)
				{
					service.InputValues[0].Value = service.Choices.GetItemFromRawValue(6);
					service.InputValues[1].Value = 0;
					service.InputValues[2].Value = int.MaxValue;
					service.Execute(synchronous: true);
					resetJakeBrakes = true;
					service.InputValues[0].Value = service.Choices.GetItemFromRawValue(7);
					service.InputValues[1].Value = 0;
					service.InputValues[2].Value = int.MaxValue;
					service.Execute(synchronous: true);
				}
			}
		}
		catch (CaesarException ex)
		{
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, ex.Message);
			return false;
		}
		return true;
	}

	private void RestoreJakeBrake()
	{
		try
		{
			if (channel != null && resetJakeBrakes && HasEngineBrakes)
			{
				Service service = channel.Services["RT_SR003_PWM_Routine_by_Function_Stop_Function_Name"];
				if (service != null)
				{
					service.InputValues[0].Value = service.Choices.GetItemFromRawValue(6);
					service.Execute(synchronous: true);
					service.InputValues[0].Value = service.Choices.GetItemFromRawValue(7);
					service.Execute(synchronous: true);
					resetJakeBrakes = false;
				}
			}
		}
		catch (CaesarException)
		{
		}
	}

	public override string ToHtml()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Resources.Print_Header);
		stringBuilder.AppendFormat(Resources.Print_Results_String, resultString);
		stringBuilder.Append(Resources.Print_Test_History);
		stringBuilder.Append(seekTimeListView.Text.Replace(Environment.NewLine, "</ul><ul>"));
		stringBuilder.Append("</p></div>");
		return stringBuilder.ToString();
	}

	private void ButtonPrintClick(object sender, EventArgs e)
	{
		PrintHelper.ShowPrintDialog(Resources.Report_Title, (IProvideHtml)(object)this, (IncludeInfo)3);
	}

	private void OnExecuteButtonClick(object sender, EventArgs e)
	{
		TestIsRunning = true;
		UpdateButtonStatus();
		RunTest();
		TestIsRunning = false;
		UpdateButtonStatus();
	}

	private void RunTest()
	{
		if (warningManager.RequestContinue())
		{
			Service service = null;
			ClearOutputs();
			if (channel == null)
			{
				((Control)(object)labelInstructions).Text = Resources.Message_TheTestCannotBeRunUntilAnMCMIsConnected;
				return;
			}
			if (!DisableJakeBrake())
			{
				DisplayStatusString(Resources.Message_TestWasUnableToStartBecauseTheEngineBrakeIsEnabled);
				return;
			}
			bool flag = ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Start_Status", ref service);
			ServiceOutputValue serviceOutputValue = service.OutputValues[0];
			if (!flag || serviceOutputValue == null)
			{
				DisplayStatusString(Resources.Message_TheTestFailedToRun);
				return;
			}
			Choice choice = serviceOutputValue.Value as Choice;
			string statusString;
			if (choice == null || (int)choice.RawValue != 1)
			{
				statusString = $"{Resources.Message_TheTestFailedToRun} {serviceOutputValue.Value.ToString()}";
				DisplayStatusString(statusString);
				flag = ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", ref service);
				return;
			}
			statusString = Resources.Message_PleaseTurnTheIgnitionKey;
			DialogResult dialogResult = MessageBox.Show(statusString, Resources.Message_AutomaticCompressionTest, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
			if (dialogResult != DialogResult.OK)
			{
				DisplayStatusString(Resources.Message_TheTestWasCancelledByTheUser);
				flag = ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", ref service);
				return;
			}
			flag = ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0", ref service);
			serviceOutputValue = service.OutputValues[0];
			if (!flag || serviceOutputValue == null)
			{
				DisplayStatusString(Resources.Message_TheTestFailedToRun);
				return;
			}
			choice = serviceOutputValue.Value as Choice;
			if (choice == null || (int)choice.RawValue != 1)
			{
				statusString = string.Format(Resources.MessageFormat_ErrorOperatorError, serviceOutputValue.Value.ToString());
				DisplayStatusString(statusString);
				flag = ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", ref service);
				return;
			}
			for (int i = 1; i <= CylinderCount; i++)
			{
				GetandDisplayCylinderValue(i);
			}
			if (ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", ref service))
			{
				DisplayStatusString(Resources.Message_TheTestCompletedSuccessfully);
				testSucceed = true;
			}
			else
			{
				DisplayStatusString(Resources.Message_TheTestFailedToRun);
			}
		}
		else
		{
			DisplayStatusString(Resources.Message_TheTestWasCancelledByTheUser);
		}
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		if (e.Category == "Engine")
		{
			UpdateConnectedEquipmentType();
		}
	}

	private int GetCylinderCount(string equipment)
	{
		switch (equipment)
		{
		case "DD5":
		case "MDEG 4-Cylinder Tier4":
			return 4;
		default:
			return 6;
		}
	}

	private void UpdateConnectedEquipmentType()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category == "Engine";
		});
		if (val != EquipmentType.Empty)
		{
			int cylinderCount = GetCylinderCount(((EquipmentType)(ref val)).Name);
			if (cylinderCount != CylinderCount)
			{
				CylinderCount = cylinderCount;
				Label cylinderLabel = CylinderLabel5;
				bool visible = (((Control)(object)CylinderOutput5).Visible = CylinderCount == 6);
				((Control)(object)cylinderLabel).Visible = visible;
				Label cylinderLabel2 = CylinderLabel6;
				visible = (((Control)(object)CylinderOutput6).Visible = CylinderCount == 6);
				((Control)(object)cylinderLabel2).Visible = visible;
			}
		}
	}

	private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		UpdateEngineBrakeAvailibility();
		UpdateButtonStatus();
	}

	private void UpdateEngineBrakeAvailibility()
	{
		if (channel != null && channel.Parameters.HaveBeenReadFromEcu)
		{
			HasEngineBrakes = channel.Parameters.Where((Parameter p) => p.GroupQualifier.StartsWith("VCD_PGR001_PropValve", StringComparison.Ordinal)).Any((Parameter p) => p.Value != null && p.Value is Choice && Convert.ToInt32(((Choice)p.Value).RawValue) == 6);
			((Control)(object)scalingLabelHasEngineBrakes).Text = (HasEngineBrakes ? Resources.Message_HasEngineBrake : Resources.Message_NoEngineBrake);
		}
		else
		{
			((Control)(object)scalingLabelHasEngineBrakes).Text = Resources.Message_Unknown;
		}
	}

	private void StartCondition_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateButtonStatus();
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
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel3 = new TableLayoutPanel();
		seekTimeListView = new SeekTimeListView();
		labelInstructions = new Label();
		tableLayoutPanel1 = new TableLayoutPanel();
		CylinderLabel4 = new Label();
		CylinderLabel5 = new Label();
		CylinderLabel6 = new Label();
		CylinderOutput6 = new ScalingLabel();
		CylinderOutput5 = new ScalingLabel();
		CylinderOutput4 = new ScalingLabel();
		CylinderLabel3 = new Label();
		CylinderOutput3 = new ScalingLabel();
		CylinderLabel2 = new Label();
		CylinderOutput2 = new ScalingLabel();
		CylinderLabel1 = new Label();
		CylinderOutput1 = new ScalingLabel();
		tableLayoutPanel2 = new TableLayoutPanel();
		btnClose = new Button();
		buttonRun = new Button();
		buttonPrint = new Button();
		engineSpeedDigitalReadoutInstrument = new DigitalReadoutInstrument();
		label2 = new System.Windows.Forms.Label();
		DigitalReadoutInstrumentVehicleCheckStatus = new DigitalReadoutInstrument();
		tableLayoutPanel4 = new TableLayoutPanel();
		labelHasEngineBrakesTitle = new Label();
		scalingLabelHasEngineBrakes = new ScalingLabel();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)this).SuspendLayout();
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)seekTimeListView, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)labelInstructions, 0, 0);
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel3, 4);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Relative Compression Test";
		((TableLayoutPanel)(object)tableLayoutPanel3).SetRowSpan((Control)(object)seekTimeListView, 2);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		labelInstructions.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelInstructions, "labelInstructions");
		((Control)(object)labelInstructions).Name = "labelInstructions";
		labelInstructions.Orientation = (TextOrientation)1;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderLabel4, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderLabel5, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderLabel6, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderOutput6, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderOutput5, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderOutput4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderLabel3, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderOutput3, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderLabel2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderOutput2, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderLabel1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)CylinderOutput1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 2, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)engineSpeedDigitalReadoutInstrument, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label2, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrumentVehicleCheckStatus, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel4, 0, 4);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		CylinderLabel4.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel4, "CylinderLabel4");
		((Control)(object)CylinderLabel4).Name = "CylinderLabel4";
		CylinderLabel4.Orientation = (TextOrientation)1;
		CylinderLabel5.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel5, "CylinderLabel5");
		((Control)(object)CylinderLabel5).Name = "CylinderLabel5";
		CylinderLabel5.Orientation = (TextOrientation)1;
		CylinderLabel6.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel6, "CylinderLabel6");
		((Control)(object)CylinderLabel6).Name = "CylinderLabel6";
		CylinderLabel6.Orientation = (TextOrientation)1;
		CylinderOutput6.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput6, "CylinderOutput6");
		CylinderOutput6.FontGroup = null;
		CylinderOutput6.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput6).Name = "CylinderOutput6";
		CylinderOutput5.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput5, "CylinderOutput5");
		CylinderOutput5.FontGroup = null;
		CylinderOutput5.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput5).Name = "CylinderOutput5";
		CylinderOutput4.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput4, "CylinderOutput4");
		CylinderOutput4.FontGroup = null;
		CylinderOutput4.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput4).Name = "CylinderOutput4";
		CylinderLabel3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel3, "CylinderLabel3");
		((Control)(object)CylinderLabel3).Name = "CylinderLabel3";
		CylinderLabel3.Orientation = (TextOrientation)1;
		CylinderOutput3.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput3, "CylinderOutput3");
		CylinderOutput3.FontGroup = null;
		CylinderOutput3.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput3).Name = "CylinderOutput3";
		CylinderLabel2.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel2, "CylinderLabel2");
		((Control)(object)CylinderLabel2).Name = "CylinderLabel2";
		CylinderLabel2.Orientation = (TextOrientation)1;
		CylinderOutput2.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput2, "CylinderOutput2");
		CylinderOutput2.FontGroup = null;
		CylinderOutput2.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput2).Name = "CylinderOutput2";
		CylinderLabel1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel1, "CylinderLabel1");
		((Control)(object)CylinderLabel1).Name = "CylinderLabel1";
		CylinderLabel1.Orientation = (TextOrientation)1;
		CylinderOutput1.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput1, "CylinderOutput1");
		CylinderOutput1.FontGroup = null;
		CylinderOutput1.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput1).Name = "CylinderOutput1";
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(btnClose, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonRun, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonPrint, 0, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(btnClose, "btnClose");
		btnClose.DialogResult = DialogResult.OK;
		btnClose.Name = "btnClose";
		btnClose.UseCompatibleTextRendering = true;
		btnClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonRun, "buttonRun");
		buttonRun.Name = "buttonRun";
		buttonRun.UseCompatibleTextRendering = true;
		buttonRun.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonPrint, "buttonPrint");
		buttonPrint.Name = "buttonPrint";
		buttonPrint.UseCompatibleTextRendering = true;
		buttonPrint.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(engineSpeedDigitalReadoutInstrument, "engineSpeedDigitalReadoutInstrument");
		engineSpeedDigitalReadoutInstrument.FontGroup = null;
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).FreezeValue = false;
		engineSpeedDigitalReadoutInstrument.Gradient.Initialize((ValueState)1, 1);
		engineSpeedDigitalReadoutInstrument.Gradient.Modify(1, 1.0, (ValueState)0);
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)engineSpeedDigitalReadoutInstrument).Name = "engineSpeedDigitalReadoutInstrument";
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).UnitAlignment = StringAlignment.Near;
		engineSpeedDigitalReadoutInstrument.RepresentedStateChanged += StartCondition_RepresentedStateChanged;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label2, 2);
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(DigitalReadoutInstrumentVehicleCheckStatus, "DigitalReadoutInstrumentVehicleCheckStatus");
		DigitalReadoutInstrumentVehicleCheckStatus.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrumentVehicleCheckStatus).FreezeValue = false;
		DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Initialize((ValueState)0, 4);
		DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState)3);
		DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState)0);
		DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)DigitalReadoutInstrumentVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)DigitalReadoutInstrumentVehicleCheckStatus).Name = "DigitalReadoutInstrumentVehicleCheckStatus";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)DigitalReadoutInstrumentVehicleCheckStatus, 2);
		((SingleInstrumentBase)DigitalReadoutInstrumentVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
		DigitalReadoutInstrumentVehicleCheckStatus.RepresentedStateChanged += StartCondition_RepresentedStateChanged;
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)labelHasEngineBrakesTitle, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)scalingLabelHasEngineBrakes, 0, 1);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		labelHasEngineBrakesTitle.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelHasEngineBrakesTitle, "labelHasEngineBrakesTitle");
		((Control)(object)labelHasEngineBrakesTitle).Name = "labelHasEngineBrakesTitle";
		labelHasEngineBrakesTitle.Orientation = (TextOrientation)1;
		scalingLabelHasEngineBrakes.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelHasEngineBrakes, "scalingLabelHasEngineBrakes");
		scalingLabelHasEngineBrakes.FontGroup = null;
		scalingLabelHasEngineBrakes.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelHasEngineBrakes).Name = "scalingLabelHasEngineBrakes";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_CompressionTest");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
