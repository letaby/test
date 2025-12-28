using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EHPS_Pumps__EMG_.panel;

public class UserPanel : CustomPanel
{
	private static string ehps201tName = "EHPS201T";

	private static string ehps401tName = "EHPS401T";

	private static string startTestQualifier = "RT_Pump_Routine_Start(1250,0)";

	private static string stopPumpQualifier = "RT_Pump_Routine_Stop";

	private Channel ehps201tChannel;

	private Channel ehps401tChannel;

	private bool ehps201tPumpTesting = false;

	private bool ehps401tPumpTesting = false;

	private bool ehpsPumpsTesting = false;

	private Panel panelTest;

	private TableLayoutPanel tableLayoutPanelTest;

	private System.Windows.Forms.Label labelEhps401tPumpTest;

	private System.Windows.Forms.Label labelEhps201tPumpTest;

	private System.Windows.Forms.Label labelBothPumpTest;

	private Button buttonBothPumpsTest;

	private Button buttonEHPS401TPumpTest;

	private Button buttonEHPS201TPumpTest;

	private TableLayoutPanel tableLayoutPanelMain;

	private SeekTimeListView seekTimeListView;

	private Button buttonEHPS201TPumpStopTest;

	private Button buttonEHPS401TPumpStopTest;

	private Button buttonClose;

	private TableLayoutPanel tableLayoutPanel4;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private System.Windows.Forms.Label labelInterlockWarning;

	private System.Windows.Forms.Label label39;

	private Button buttonEHPSPumpsStopTest;

	private bool InterlockOk => (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 || (int)digitalReadoutInstrumentParkBrake.RepresentedState == 1;

	private bool Ehps201TOnline => ehps201tChannel != null && (ehps201tChannel.CommunicationsState == CommunicationsState.Online || ehps201tChannel.CommunicationsState == CommunicationsState.ExecuteService);

	private bool Ehps401TOnline => ehps401tChannel != null && (ehps401tChannel.CommunicationsState == CommunicationsState.Online || ehps401tChannel.CommunicationsState == CommunicationsState.ExecuteService);

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetEhps201tChannel(ehps201tName);
		SetEhps401tChannel(ehps401tName);
		UpdateUI();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = ehps201tPumpTesting || ehps201tPumpTesting || ehpsPumpsTesting;
		if (!e.Cancel)
		{
			SetEhps201tChannel(null);
			SetEhps401tChannel(null);
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	private void AddLogLabel(string text)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
		}
	}

	private void SetEhps201tChannel(string ecuName)
	{
		Channel channel = ((CustomPanel)this).GetChannel(ecuName, (ChannelLookupOptions)3);
		if (ehps201tChannel != channel)
		{
			if (ehps201tChannel != null)
			{
				ehps201tChannel.Services.ServiceCompleteEvent -= ehps201tChannel_ServiceCompleteEvent;
				ehps201tChannel.CommunicationsStateUpdateEvent -= ehps201tChannel_CommunicationsStateUpdateEvent;
			}
			ehps201tChannel = channel;
			if (ehps201tChannel != null)
			{
				ehps201tChannel.Services.ServiceCompleteEvent += ehps201tChannel_ServiceCompleteEvent;
				ehps201tChannel.CommunicationsStateUpdateEvent += ehps201tChannel_CommunicationsStateUpdateEvent;
			}
			ehps201tPumpTesting = false;
			ehpsPumpsTesting = false;
		}
		UpdateUI();
	}

	private void ehps201tChannel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUI();
	}

	private void ehps201tChannel_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		UpdateUI();
	}

	private void SetEhps401tChannel(string ecuName)
	{
		Channel channel = ((CustomPanel)this).GetChannel(ecuName, (ChannelLookupOptions)3);
		if (ehps401tChannel != channel)
		{
			if (ehps401tChannel != null)
			{
				ehps401tChannel.Services.ServiceCompleteEvent -= ehps401tChannel_ServiceCompleteEvent;
				ehps401tChannel.CommunicationsStateUpdateEvent -= ehps401tChannel_CommunicationsStateUpdateEvent;
			}
			ehps401tChannel = channel;
			if (ehps401tChannel != null)
			{
				ehps401tChannel.Services.ServiceCompleteEvent += ehps401tChannel_ServiceCompleteEvent;
				ehps401tChannel.CommunicationsStateUpdateEvent += ehps401tChannel_CommunicationsStateUpdateEvent;
			}
			ehps401tPumpTesting = false;
			ehpsPumpsTesting = false;
		}
		UpdateUI();
	}

	private void ehps401tChannel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUI();
	}

	private void ehps401tChannel_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		labelInterlockWarning.Visible = !InterlockOk;
		labelEhps201tPumpTest.Text = (Ehps201TOnline ? Resources.Message_EHPS201TPumpTest : Resources.Message_EHPS201TOffline);
		labelEhps401tPumpTest.Text = (Ehps401TOnline ? Resources.Message_EHPS401TPumpTest : Resources.Message_EHPS401TOffline);
		buttonEHPS201TPumpTest.Enabled = InterlockOk && Ehps201TOnline && !ehps201tPumpTesting && !ehpsPumpsTesting;
		buttonEHPS401TPumpTest.Enabled = InterlockOk && Ehps401TOnline && !ehps401tPumpTesting && !ehpsPumpsTesting;
		buttonBothPumpsTest.Enabled = InterlockOk && Ehps201TOnline && Ehps401TOnline && !ehpsPumpsTesting;
		buttonEHPS201TPumpStopTest.Enabled = Ehps201TOnline;
		buttonEHPS401TPumpStopTest.Enabled = Ehps401TOnline;
		buttonEHPSPumpsStopTest.Enabled = Ehps201TOnline && Ehps401TOnline;
		buttonClose.Enabled = !ehps201tPumpTesting && !ehps401tPumpTesting && !ehpsPumpsTesting;
	}

	private bool RunService(Channel channel, string serviceQualifier, ServiceCompleteEventHandler serviceCompleteEventHandler)
	{
		if (channel != null && channel.Online)
		{
			Service service = channel.Services[serviceQualifier];
			if (service != null)
			{
				if (serviceCompleteEventHandler != null)
				{
					service.ServiceCompleteEvent += serviceCompleteEventHandler;
				}
				channel.Services.Execute(serviceQualifier, synchronous: false);
				AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ServiceStarted01, channel.Ecu.Name, serviceQualifier));
				return true;
			}
		}
		string arg = ((channel != null && channel.Ecu != null) ? channel.Ecu.Name : Resources.Message_Null);
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ServiceCouldNotBeStarted01, arg, serviceQualifier));
		return false;
	}

	private void textBox_KeyPress(object sender, KeyPressEventArgs e)
	{
		e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
		UpdateUI();
	}

	private void textBox_TextChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void buttonEHPS201TPumpTest_Click(object sender, EventArgs e)
	{
		if (RunService(ehps201tChannel, startTestQualifier, null))
		{
			ehps201tPumpTesting = true;
			if (ehps401tPumpTesting)
			{
				ehps201tPumpTesting = false;
				ehps401tPumpTesting = false;
				ehpsPumpsTesting = true;
			}
		}
		else
		{
			RunService(ehps201tChannel, stopPumpQualifier, null);
			ehps201tPumpTesting = false;
			ehpsPumpsTesting = false;
		}
		UpdateUI();
	}

	private void buttonEHPS401TPumpTest_Click(object sender, EventArgs e)
	{
		if (RunService(ehps401tChannel, startTestQualifier, null))
		{
			ehps401tPumpTesting = true;
			if (ehps201tPumpTesting)
			{
				ehps201tPumpTesting = false;
				ehps401tPumpTesting = false;
				ehpsPumpsTesting = true;
			}
		}
		else
		{
			RunService(ehps401tChannel, stopPumpQualifier, null);
			ehps401tPumpTesting = false;
			ehpsPumpsTesting = false;
		}
		UpdateUI();
	}

	private void buttonBothPumpsTest_Click(object sender, EventArgs e)
	{
		ehps201tPumpTesting = false;
		ehps401tPumpTesting = false;
		if (RunService(ehps201tChannel, startTestQualifier, null) && RunService(ehps401tChannel, startTestQualifier, null))
		{
			ehpsPumpsTesting = true;
		}
		else
		{
			RunService(ehps201tChannel, stopPumpQualifier, null);
			RunService(ehps401tChannel, stopPumpQualifier, null);
			ehpsPumpsTesting = false;
		}
		UpdateUI();
	}

	private void buttonEHPS201TPumpStopTest_Click(object sender, EventArgs e)
	{
		RunService(ehps201tChannel, stopPumpQualifier, null);
		ehps201tPumpTesting = false;
		ehps401tPumpTesting = ehps401tPumpTesting || ehpsPumpsTesting;
		ehpsPumpsTesting = false;
		UpdateUI();
	}

	private void buttonEHPS401TPumpStopTest_Click(object sender, EventArgs e)
	{
		RunService(ehps401tChannel, stopPumpQualifier, null);
		ehps201tPumpTesting = ehps201tPumpTesting || ehpsPumpsTesting;
		ehps401tPumpTesting = false;
		ehpsPumpsTesting = false;
		UpdateUI();
	}

	private void buttonEHPSPumpsStop_Click(object sender, EventArgs e)
	{
		RunService(ehps201tChannel, stopPumpQualifier, null);
		RunService(ehps401tChannel, stopPumpQualifier, null);
		ehps201tPumpTesting = false;
		ehps401tPumpTesting = false;
		ehpsPumpsTesting = false;
		UpdateUI();
	}

	private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void digitalReadoutInstrumentParkBrake_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
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
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		labelInterlockWarning = new System.Windows.Forms.Label();
		label39 = new System.Windows.Forms.Label();
		panelTest = new Panel();
		tableLayoutPanelTest = new TableLayoutPanel();
		buttonBothPumpsTest = new Button();
		buttonEHPS401TPumpTest = new Button();
		labelEhps401tPumpTest = new System.Windows.Forms.Label();
		labelEhps201tPumpTest = new System.Windows.Forms.Label();
		labelBothPumpTest = new System.Windows.Forms.Label();
		buttonEHPS201TPumpTest = new Button();
		buttonEHPS201TPumpStopTest = new Button();
		buttonEHPS401TPumpStopTest = new Button();
		buttonEHPSPumpsStopTest = new Button();
		seekTimeListView = new SeekTimeListView();
		buttonClose = new Button();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		panelTest.SuspendLayout();
		((Control)(object)tableLayoutPanelTest).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanel4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(panelTest, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)seekTimeListView, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClose, 1, 3);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(labelInterlockWarning, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(label39, 0, 2);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)tableLayoutPanel4, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)3, 4, "mph");
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 5.0, (ValueState)3);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(3, 6.0, (ValueState)3);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(4, 2147483647.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState)0, 6);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(6, 5.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrumentParkBrake_RepresentedStateChanged;
		componentResourceManager.ApplyResources(labelInterlockWarning, "labelInterlockWarning");
		labelInterlockWarning.ForeColor = Color.Red;
		labelInterlockWarning.Name = "labelInterlockWarning";
		labelInterlockWarning.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label39, "label39");
		label39.Name = "label39";
		label39.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(panelTest, "panelTest");
		panelTest.BorderStyle = BorderStyle.FixedSingle;
		panelTest.Controls.Add((Control)(object)tableLayoutPanelTest);
		panelTest.Name = "panelTest";
		componentResourceManager.ApplyResources(tableLayoutPanelTest, "tableLayoutPanelTest");
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(buttonBothPumpsTest, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(buttonEHPS401TPumpTest, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(labelEhps401tPumpTest, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(labelEhps201tPumpTest, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(labelBothPumpTest, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(buttonEHPS201TPumpTest, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(buttonEHPS201TPumpStopTest, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(buttonEHPS401TPumpStopTest, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTest).Controls.Add(buttonEHPSPumpsStopTest, 2, 2);
		((Control)(object)tableLayoutPanelTest).Name = "tableLayoutPanelTest";
		componentResourceManager.ApplyResources(buttonBothPumpsTest, "buttonBothPumpsTest");
		buttonBothPumpsTest.Name = "buttonBothPumpsTest";
		buttonBothPumpsTest.UseVisualStyleBackColor = true;
		buttonBothPumpsTest.Click += buttonBothPumpsTest_Click;
		componentResourceManager.ApplyResources(buttonEHPS401TPumpTest, "buttonEHPS401TPumpTest");
		buttonEHPS401TPumpTest.Name = "buttonEHPS401TPumpTest";
		buttonEHPS401TPumpTest.UseVisualStyleBackColor = true;
		buttonEHPS401TPumpTest.Click += buttonEHPS401TPumpTest_Click;
		componentResourceManager.ApplyResources(labelEhps401tPumpTest, "labelEhps401tPumpTest");
		labelEhps401tPumpTest.Name = "labelEhps401tPumpTest";
		componentResourceManager.ApplyResources(labelEhps201tPumpTest, "labelEhps201tPumpTest");
		labelEhps201tPumpTest.Name = "labelEhps201tPumpTest";
		componentResourceManager.ApplyResources(labelBothPumpTest, "labelBothPumpTest");
		labelBothPumpTest.Name = "labelBothPumpTest";
		componentResourceManager.ApplyResources(buttonEHPS201TPumpTest, "buttonEHPS201TPumpTest");
		buttonEHPS201TPumpTest.Name = "buttonEHPS201TPumpTest";
		buttonEHPS201TPumpTest.UseVisualStyleBackColor = true;
		buttonEHPS201TPumpTest.Click += buttonEHPS201TPumpTest_Click;
		componentResourceManager.ApplyResources(buttonEHPS201TPumpStopTest, "buttonEHPS201TPumpStopTest");
		buttonEHPS201TPumpStopTest.Name = "buttonEHPS201TPumpStopTest";
		buttonEHPS201TPumpStopTest.UseVisualStyleBackColor = true;
		buttonEHPS201TPumpStopTest.Click += buttonEHPS201TPumpStopTest_Click;
		componentResourceManager.ApplyResources(buttonEHPS401TPumpStopTest, "buttonEHPS401TPumpStopTest");
		buttonEHPS401TPumpStopTest.Name = "buttonEHPS401TPumpStopTest";
		buttonEHPS401TPumpStopTest.UseVisualStyleBackColor = true;
		buttonEHPS401TPumpStopTest.Click += buttonEHPS401TPumpStopTest_Click;
		componentResourceManager.ApplyResources(buttonEHPSPumpsStopTest, "buttonEHPSPumpsStopTest");
		buttonEHPSPumpsStopTest.Name = "buttonEHPSPumpsStopTest";
		buttonEHPSPumpsStopTest.UseVisualStyleBackColor = true;
		buttonEHPSPumpsStopTest.Click += buttonEHPSPumpsStop_Click;
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "ePowerSteering";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_ePower_Steering");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		panelTest.ResumeLayout(performLayout: false);
		panelTest.PerformLayout();
		((Control)(object)tableLayoutPanelTest).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelTest).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
