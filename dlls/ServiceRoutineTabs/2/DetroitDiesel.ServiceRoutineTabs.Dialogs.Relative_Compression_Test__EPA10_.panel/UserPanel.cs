using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__EPA10_.panel;

public class UserPanel : CustomPanel, IProvideHtml
{
	private const int CylinderCount = 6;

	private const string ServiceRoutineStart = "RT_SR006_Automatic_Compression_Test_Start_Status";

	private const string ServiceRoutineStop = "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0";

	private const string ServiceRoutineStatus = "RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0";

	private ScalingLabel[] labels;

	private WarningManager warningManager;

	private string resultString = Resources.Message_The_Test_Has_Not_Started;

	private bool TestIsRunning;

	private Channel channel;

	private bool testSucceed = false;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

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

	private DigitalReadoutInstrument engineSpeedDigitalReadoutInstrument;

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
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		InitializeComponent();
		labels = (ScalingLabel[])(object)new ScalingLabel[6] { CylinderOutput1, CylinderOutput2, CylinderOutput3, CylinderOutput4, CylinderOutput5, CylinderOutput6 };
		warningManager = new WarningManager(string.Empty, Resources.Message_CompressionTest, seekTimeListView.RequiredUserLabelPrefix);
		((Control)(object)labelInstructions).Text = Resources.Message_StartTheTestWithTheEngineOffAndTheIgnitionOn;
		buttonPrint.Click += ButtonPrintClick;
		buttonRun.Click += OnExecuteButtonClick;
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
		for (int i = 0; i < 6; i++)
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
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		buttonRun.Enabled = false;
		if (TestIsRunning)
		{
			((Control)(object)labelInstructions).Text = Resources.Message_TestIsRunning;
		}
		else if (channel != null)
		{
			if (!EngineIsRunning)
			{
				if ((int)DigitalReadoutInstrument1.RepresentedState == 1)
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
		((Control)this).Tag = new object[2] { testSucceed, resultString };
		((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
	}

	public override void OnChannelsChanged()
	{
		warningManager.Reset();
		channel = ((CustomPanel)this).GetChannel("MCM02T", (ChannelLookupOptions)7);
		if (channel == null)
		{
			((Control)(object)labelInstructions).Text = Resources.Message_TheTestCannotBeRunUntilAnMCMIsConnected;
		}
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
				((Control)(object)labelInstructions).Text = Resources.Message_TheTestCannotBeRunUntilAnMCMIsConnected;
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
			for (int i = 1; i <= 6; i++)
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
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel3 = new TableLayoutPanel();
		seekTimeListView = new SeekTimeListView();
		labelInstructions = new Label();
		tableLayoutPanel2 = new TableLayoutPanel();
		btnClose = new Button();
		buttonRun = new Button();
		buttonPrint = new Button();
		tableLayoutPanel1 = new TableLayoutPanel();
		engineSpeedDigitalReadoutInstrument = new DigitalReadoutInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
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
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)seekTimeListView, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)labelInstructions, 0, 0);
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel3, 3);
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
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)engineSpeedDigitalReadoutInstrument, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)DigitalReadoutInstrument1, 0, 4);
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
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 1, 4);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(engineSpeedDigitalReadoutInstrument, "engineSpeedDigitalReadoutInstrument");
		engineSpeedDigitalReadoutInstrument.FontGroup = null;
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).FreezeValue = false;
		engineSpeedDigitalReadoutInstrument.Gradient.Initialize((ValueState)1, 1);
		engineSpeedDigitalReadoutInstrument.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS010_Engine_Speed");
		((Control)(object)engineSpeedDigitalReadoutInstrument).Name = "engineSpeedDigitalReadoutInstrument";
		((SingleInstrumentBase)engineSpeedDigitalReadoutInstrument).UnitAlignment = StringAlignment.Near;
		engineSpeedDigitalReadoutInstrument.RepresentedStateChanged += StartCondition_RepresentedStateChanged;
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		DigitalReadoutInstrument1.Gradient.Initialize((ValueState)0, 4);
		DigitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState)3);
		DigitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState)1);
		DigitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState)0);
		DigitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		DigitalReadoutInstrument1.RepresentedStateChanged += StartCondition_RepresentedStateChanged;
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
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_CompressionTest");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
