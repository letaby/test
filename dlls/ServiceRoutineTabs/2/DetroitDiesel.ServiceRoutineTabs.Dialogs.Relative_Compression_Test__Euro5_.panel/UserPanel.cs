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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__Euro5_.panel;

public class UserPanel : CustomPanel, IProvideHtml
{
	private const string ServiceRoutineStart = "RT_SR0207_Automatic_Compression_Detection_Start";

	private const string ServiceRoutineCompressionValue = "RT_SR0207_Automatic_Compression_Detection_Request_Results_Compression_Value_Segment_{0}";

	private const string ServiceRoutineStatus = "RT_SR0207_Automatic_Compression_Detection_Request_Results_State_Byte";

	private ScalingLabel[] labels;

	private int CylinderCount = 6;

	private WarningManager warningManager;

	private string resultString = Resources.Message_The_Test_Has_Not_Started;

	private bool TestIsRunning;

	private Channel channel;

	private bool testSucceed = false;

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

	private Button buttonRun;

	private Button buttonPrint;

	private DigitalReadoutInstrument engineSpeedDigitalReadoutInstrument;

	private System.Windows.Forms.Label label2;

	private TableLayoutPanel tableLayoutPanel6;

	private TableLayoutPanel tableLayoutPanel2;

	private Checkmark checkmark1;

	private System.Windows.Forms.Label labelInstructions;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;

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
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		InitializeComponent();
		labels = (ScalingLabel[])(object)new ScalingLabel[6] { CylinderOutput1, CylinderOutput2, CylinderOutput3, CylinderOutput4, CylinderOutput5, CylinderOutput6 };
		warningManager = new WarningManager(string.Empty, Resources.Message_CompressionTest, seekTimeListView.RequiredUserLabelPrefix);
		labelInstructions.Text = Resources.Message_StartTheTestWithTheEngineOffAndTheIgnitionOn;
		buttonPrint.Click += ButtonPrintClick;
		buttonRun.Click += OnExecuteButtonClick;
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	private bool ExecuteServiceRoutine(string serviceName, ref Service service)
	{
		bool result = false;
		if (channel == null)
		{
			DisplayStatusString(Resources.Message_TheMR2IsOfflineCannotExecuteService + serviceName + "\".");
		}
		else
		{
			service = channel.Services[serviceName];
			if (service == null)
			{
				DisplayStatusString(Resources.Message_TheMR2DoesNotSupportTheServiceRoutine + serviceName + "\".");
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
		string serviceName = $"RT_SR0207_Automatic_Compression_Detection_Request_Results_Compression_Value_Segment_{cylinder - 1}";
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
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Invalid comparison between Unknown and I4
		buttonRun.Enabled = false;
		checkmark1.Checked = false;
		if (TestIsRunning)
		{
			labelInstructions.Text = Resources.Message_TestIsRunning;
		}
		else if (channel != null)
		{
			if (!EngineIsRunning)
			{
				if ((int)digitalReadoutInstrumentParkingBrake.RepresentedState == 1)
				{
					buttonRun.Enabled = true;
					checkmark1.Checked = true;
					labelInstructions.Text = Resources.Message_TheTestIsReadyToBeStarted;
				}
				else
				{
					labelInstructions.Text = Resources.Message_ParkingBrakeOn;
				}
			}
			else
			{
				labelInstructions.Text = Resources.Message_CannotRunTestWithEngineRunning;
			}
		}
		else
		{
			labelInstructions.Text = Resources.Message_TheTestCannotBeRunUntilAnMR2IsConnected;
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		((Control)this).Tag = new object[2] { testSucceed, resultString };
		((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
	}

	public override void OnChannelsChanged()
	{
		warningManager.Reset();
		Channel channel = ((CustomPanel)this).GetChannel("MR201T");
		if (channel != this.channel)
		{
			this.channel = channel;
		}
		UpdateConnectedEquipmentType();
		UpdateButtonStatus();
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
				labelInstructions.Text = Resources.Message_TheTestCannotBeRunUntilAnMR2IsConnected;
				return;
			}
			if (!ExecuteServiceRoutine("RT_SR0207_Automatic_Compression_Detection_Start", ref service))
			{
				DisplayStatusString(Resources.Message_TheTestFailedToRun);
				return;
			}
			string message_PleaseTurnTheIgnitionKey = Resources.Message_PleaseTurnTheIgnitionKey;
			DialogResult dialogResult = MessageBox.Show(message_PleaseTurnTheIgnitionKey, Resources.Message_AutomaticCompressionTest, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
			if (dialogResult != DialogResult.OK)
			{
				DisplayStatusString(Resources.Message_TheTestWasCancelledByTheUser);
				return;
			}
			ServiceOutputValue serviceOutputValue = null;
			bool flag = ExecuteServiceRoutine("RT_SR0207_Automatic_Compression_Detection_Request_Results_State_Byte", ref service);
			serviceOutputValue = service.OutputValues[0];
			if (!flag || serviceOutputValue == null)
			{
				DisplayStatusString(Resources.Message_TheTestFailedToRun);
				return;
			}
			Choice choice = serviceOutputValue.Value as Choice;
			if (choice == null || (int)choice.RawValue != 1)
			{
				message_PleaseTurnTheIgnitionKey = string.Format(Resources.MessageFormat_ErrorOperatorError, serviceOutputValue.Value.ToString());
				DisplayStatusString(message_PleaseTurnTheIgnitionKey);
				return;
			}
			for (int i = 1; i <= CylinderCount; i++)
			{
				GetandDisplayCylinderValue(i);
			}
			if (testSucceed)
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
		string text = ((channel != null && channel.EcuInfos["CO_CertificationNumber"] != null) ? channel.EcuInfos["CO_CertificationNumber"].Value : string.Empty);
		if (text.StartsWith("OM924"))
		{
			return 4;
		}
		return 6;
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
		if (!(val != EquipmentType.Empty))
		{
			return;
		}
		int cylinderCount = GetCylinderCount(((EquipmentType)(ref val)).Name);
		if (cylinderCount != CylinderCount)
		{
			CylinderCount = cylinderCount;
			((Control)(object)tableLayoutPanel3).SuspendLayout();
			if (CylinderCount == 6)
			{
				Label cylinderLabel = CylinderLabel5;
				bool visible = (((Control)(object)CylinderOutput5).Visible = true);
				((Control)(object)cylinderLabel).Visible = visible;
				Label cylinderLabel2 = CylinderLabel6;
				visible = (((Control)(object)CylinderOutput6).Visible = true);
				((Control)(object)cylinderLabel2).Visible = visible;
				((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel3, 2, 0);
				((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput3, 2, 1);
				((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel4, 0, 2);
				((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput4, 0, 3);
			}
			else
			{
				Label cylinderLabel3 = CylinderLabel6;
				bool visible = (((Control)(object)CylinderOutput6).Visible = false);
				((Control)(object)cylinderLabel3).Visible = visible;
				Label cylinderLabel4 = CylinderLabel5;
				visible = (((Control)(object)CylinderOutput5).Visible = false);
				((Control)(object)cylinderLabel4).Visible = visible;
				((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel3, 0, 2);
				((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput3, 0, 3);
				((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel4, 1, 2);
				((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput4, 1, 3);
			}
			((Control)(object)tableLayoutPanel3).ResumeLayout();
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
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel3 = new TableLayoutPanel();
		CylinderLabel1 = new Label();
		CylinderLabel2 = new Label();
		CylinderLabel3 = new Label();
		CylinderOutput3 = new ScalingLabel();
		CylinderOutput6 = new ScalingLabel();
		CylinderLabel6 = new Label();
		CylinderOutput5 = new ScalingLabel();
		CylinderLabel5 = new Label();
		CylinderOutput4 = new ScalingLabel();
		CylinderLabel4 = new Label();
		CylinderOutput1 = new ScalingLabel();
		CylinderOutput2 = new ScalingLabel();
		seekTimeListView = new SeekTimeListView();
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel6 = new TableLayoutPanel();
		engineSpeedDigitalReadoutInstrument = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
		label2 = new System.Windows.Forms.Label();
		tableLayoutPanel2 = new TableLayoutPanel();
		buttonPrint = new Button();
		buttonRun = new Button();
		checkmark1 = new Checkmark();
		labelInstructions = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel6).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel3, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput3, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput6, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel6, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput5, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel5, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderLabel4, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)CylinderOutput2, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		CylinderLabel1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel1, "CylinderLabel1");
		((Control)(object)CylinderLabel1).Name = "CylinderLabel1";
		CylinderLabel1.Orientation = (TextOrientation)1;
		CylinderLabel2.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel2, "CylinderLabel2");
		((Control)(object)CylinderLabel2).Name = "CylinderLabel2";
		CylinderLabel2.Orientation = (TextOrientation)1;
		CylinderLabel3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel3, "CylinderLabel3");
		((Control)(object)CylinderLabel3).Name = "CylinderLabel3";
		CylinderLabel3.Orientation = (TextOrientation)1;
		CylinderOutput3.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput3, "CylinderOutput3");
		CylinderOutput3.FontGroup = null;
		CylinderOutput3.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput3).Name = "CylinderOutput3";
		CylinderOutput6.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput6, "CylinderOutput6");
		CylinderOutput6.FontGroup = null;
		CylinderOutput6.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput6).Name = "CylinderOutput6";
		CylinderLabel6.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel6, "CylinderLabel6");
		((Control)(object)CylinderLabel6).Name = "CylinderLabel6";
		CylinderLabel6.Orientation = (TextOrientation)1;
		CylinderOutput5.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput5, "CylinderOutput5");
		CylinderOutput5.FontGroup = null;
		CylinderOutput5.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput5).Name = "CylinderOutput5";
		CylinderLabel5.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel5, "CylinderLabel5");
		((Control)(object)CylinderLabel5).Name = "CylinderLabel5";
		CylinderLabel5.Orientation = (TextOrientation)1;
		CylinderOutput4.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput4, "CylinderOutput4");
		CylinderOutput4.FontGroup = null;
		CylinderOutput4.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput4).Name = "CylinderOutput4";
		CylinderLabel4.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(CylinderLabel4, "CylinderLabel4");
		((Control)(object)CylinderLabel4).Name = "CylinderLabel4";
		CylinderLabel4.Orientation = (TextOrientation)1;
		CylinderOutput1.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput1, "CylinderOutput1");
		CylinderOutput1.FontGroup = null;
		CylinderOutput1.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput1).Name = "CylinderOutput1";
		CylinderOutput2.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(CylinderOutput2, "CylinderOutput2");
		CylinderOutput2.FontGroup = null;
		CylinderOutput2.LineAlignment = StringAlignment.Center;
		((Control)(object)CylinderOutput2).Name = "CylinderOutput2";
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Relative Compression Test";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel6, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label2, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 1, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(tableLayoutPanel6, "tableLayoutPanel6");
		((TableLayoutPanel)(object)tableLayoutPanel6).Controls.Add((Control)(object)engineSpeedDigitalReadoutInstrument, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel6).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBrake, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel6).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanel6).Name = "tableLayoutPanel6";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel6, 2);
		componentResourceManager.ApplyResources(engineSpeedDigitalReadoutInstrument, "engineSpeedDigitalReadoutInstrument");
		engineSpeedDigitalReadoutInstrument.FontGroup = null;
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).FreezeValue = false;
		engineSpeedDigitalReadoutInstrument.Gradient.Initialize((ValueState)1, 1);
		engineSpeedDigitalReadoutInstrument.Gradient.Modify(1, 1.0, (ValueState)0);
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).Instrument = new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Engine_Speed");
		((Control)(object)engineSpeedDigitalReadoutInstrument).Name = "engineSpeedDigitalReadoutInstrument";
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).UnitAlignment = StringAlignment.Near;
		engineSpeedDigitalReadoutInstrument.RepresentedStateChanged += StartCondition_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
		digitalReadoutInstrumentParkingBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).FreezeValue = false;
		digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Parking_Brake");
		((Control)(object)digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkingBrake.RepresentedStateChanged += StartCondition_RepresentedStateChanged;
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonPrint, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonRun, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmark1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(labelInstructions, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(buttonPrint, "buttonPrint");
		buttonPrint.Name = "buttonPrint";
		buttonPrint.UseCompatibleTextRendering = true;
		buttonPrint.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonRun, "buttonRun");
		buttonRun.Name = "buttonRun";
		buttonRun.UseCompatibleTextRendering = true;
		buttonRun.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		componentResourceManager.ApplyResources(labelInstructions, "labelInstructions");
		labelInstructions.Name = "labelInstructions";
		labelInstructions.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_CompressionTest");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel6).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
