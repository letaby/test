using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Position_Control.panel;

public class UserPanel : CustomPanel
{
	private const string StartQualifier = "RT_SR080_Control_EGR_valve_position_Start_Status";

	private const string StopQualifier = "RT_SR080_Control_EGR_valve_position_Stop_Status";

	private Channel channel;

	private bool testRunning;

	private BarInstrument barInstrumentCoolantTemp;

	private TableLayoutPanel tableLayoutPanelInterface;

	private System.Windows.Forms.Label labelDesiredPosition;

	private TextBox textBoxDesiredPosition;

	private Button buttonStart;

	private Button buttonStop;

	private BarInstrument barInstrumentBatteryVoltage;

	private BarInstrument barInstrumentCommandedValue;

	private BarInstrument barInstrumentActualValue;

	private Panel panelButtons;

	private System.Windows.Forms.Label labelCanStart;

	private Checkmark checkmarkCanStart;

	private SeekTimeListView seekTimeListView1;

	private Button buttonMinus;

	private Button buttonPlus;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineState;

	private TableLayoutPanel tableLayoutPanelMain;

	private bool CanStart => Online && InputPosition.HasValue && ServicesAvailable && !TestRunning && !Busy && !EngineRunning;

	private bool CanStop => Online && TestRunning;

	private bool TestRunning => testRunning;

	private bool Online => channel != null && channel.Online;

	private bool Busy => Online && channel.CommunicationsState != CommunicationsState.Online;

	private bool ServicesAvailable => channel != null && channel.Services["RT_SR080_Control_EGR_valve_position_Start_Status"] != null && channel.Services["RT_SR080_Control_EGR_valve_position_Stop_Status"] != null;

	private int? InputPosition
	{
		get
		{
			if (int.TryParse(textBoxDesiredPosition.Text, out var result) && result >= 0 && result <= 100)
			{
				return result;
			}
			return null;
		}
	}

	private bool EngineRunning => (int)digitalReadoutInstrumentEngineState.RepresentedState != 1;

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel("MCM21T"));
	}

	private void SetChannel(Channel channel)
	{
		if (this.channel != channel)
		{
			if (this.channel != null)
			{
				this.channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			this.channel = channel;
			if (this.channel != null)
			{
				this.channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
			UpdateUserInterface();
		}
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (TestRunning)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			SetChannel(null);
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		buttonStart.Enabled = CanStart;
		buttonStop.Enabled = CanStop;
		checkmarkCanStart.CheckState = ((CanStart || TestRunning) ? CheckState.Checked : CheckState.Unchecked);
		string text = Resources.Message_TestCanStart;
		if (!buttonStart.Enabled)
		{
			if (TestRunning)
			{
				text = Resources.Message_TestIsInProgress;
			}
			else if (Busy)
			{
				text = Resources.Message_CannotStartAsDeviceIsBusy;
			}
			else if (!Online)
			{
				text = Resources.Message_CannotStartAsDeviceIsNotOnline;
			}
			else if (!InputPosition.HasValue)
			{
				text = Resources.Message_CannotStartAsPositionNotValid;
			}
			else if (EngineRunning)
			{
				text = Resources.Message_CannotStartAsEngineIsRunningStopEngine;
			}
		}
		labelCanStart.Text = text;
	}

	private void Output(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
	}

	private void StopTest(string reason)
	{
		testRunning = false;
		Output(reason);
		UpdateUserInterface();
		if (Online)
		{
			Output(Resources.Message_RequestEndEGRManipulation);
			channel.Services["RT_SR080_Control_EGR_valve_position_Stop_Status"].Execute(synchronous: false);
		}
		else
		{
			Output(Resources.Message_UnableToRequestEndEGRManipulation);
		}
	}

	private void SetDesiredPosition(int position)
	{
		if (Online)
		{
			if (!Busy)
			{
				Service service = channel.Services["RT_SR080_Control_EGR_valve_position_Start_Status"];
				service.InputValues[0].Value = 0;
				service.InputValues[1].Value = position;
				service.ServiceCompleteEvent += startService_ServiceCompleteEvent;
				service.Execute(synchronous: false);
			}
			else
			{
				Output(Resources.Message_ECUIsBusy);
			}
		}
		else
		{
			StopTest(Resources.Message_ECUOfflineTestAborted);
		}
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		testRunning = true;
		Output(Resources.Message_TestStarted);
		UpdateUserInterface();
		SetDesiredPosition(InputPosition.Value);
	}

	private void buttonStop_Click(object sender, EventArgs e)
	{
		StopTest(Resources.Message_TestCompleteDueToUserRequest);
	}

	private void buttonPlus_Click(object sender, EventArgs e)
	{
		int? inputPosition = InputPosition;
		if (inputPosition.HasValue && inputPosition.Value <= 95)
		{
			int? num = inputPosition;
			textBoxDesiredPosition.Text = (num + 5).Value.ToString();
		}
	}

	private void buttonMinus_Click(object sender, EventArgs e)
	{
		int? inputPosition = InputPosition;
		if (inputPosition.HasValue && inputPosition.Value >= 5)
		{
			int? num = inputPosition;
			textBoxDesiredPosition.Text = (num - 5).Value.ToString();
		}
	}

	private void textBoxDesiredPosition_TextChanged(object sender, EventArgs e)
	{
		if (testRunning)
		{
			int? inputPosition = InputPosition;
			if (inputPosition.HasValue)
			{
				SetDesiredPosition(inputPosition.Value);
			}
		}
		else
		{
			UpdateUserInterface();
		}
	}

	private void startService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= startService_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			Output(string.Concat(Resources.Message_SetPosition, service.InputValues[1].Value, "%: ", service.OutputValues[0].Value));
		}
		else
		{
			Output(string.Concat(Resources.Message_SetPosition, service.InputValues[1].Value, "%: ", e.Exception.Message));
		}
	}

	private void digitalReadoutInstrumentEngineState_RepresentedStateChanged(object sender, EventArgs e)
	{
		if (testRunning && EngineRunning)
		{
			StopTest(Resources.Message_EngineStateIncorrect);
		}
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelInterface = new TableLayoutPanel();
		tableLayoutPanelMain = new TableLayoutPanel();
		panelButtons = new Panel();
		buttonMinus = new Button();
		labelDesiredPosition = new System.Windows.Forms.Label();
		textBoxDesiredPosition = new TextBox();
		seekTimeListView1 = new SeekTimeListView();
		buttonPlus = new Button();
		barInstrumentCoolantTemp = new BarInstrument();
		barInstrumentBatteryVoltage = new BarInstrument();
		barInstrumentCommandedValue = new BarInstrument();
		barInstrumentActualValue = new BarInstrument();
		digitalReadoutInstrumentEngineState = new DigitalReadoutInstrument();
		labelCanStart = new System.Windows.Forms.Label();
		checkmarkCanStart = new Checkmark();
		buttonStart = new Button();
		buttonStop = new Button();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelInterface).SuspendLayout();
		panelButtons.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelInterface, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)barInstrumentCoolantTemp, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)barInstrumentBatteryVoltage, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)barInstrumentCommandedValue, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)barInstrumentActualValue, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentEngineState, 2, 2);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(tableLayoutPanelInterface, "tableLayoutPanelInterface");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelInterface, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInterface).Controls.Add(buttonMinus, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInterface).Controls.Add(labelDesiredPosition, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInterface).Controls.Add(textBoxDesiredPosition, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInterface).Controls.Add((Control)(object)seekTimeListView1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInterface).Controls.Add(buttonPlus, 3, 0);
		((Control)(object)tableLayoutPanelInterface).Name = "tableLayoutPanelInterface";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)tableLayoutPanelInterface, 3);
		componentResourceManager.ApplyResources(buttonMinus, "buttonMinus");
		buttonMinus.Name = "buttonMinus";
		buttonMinus.UseCompatibleTextRendering = true;
		buttonMinus.UseVisualStyleBackColor = true;
		buttonMinus.Click += buttonMinus_Click;
		componentResourceManager.ApplyResources(labelDesiredPosition, "labelDesiredPosition");
		labelDesiredPosition.Name = "labelDesiredPosition";
		((TableLayoutPanel)(object)tableLayoutPanelInterface).SetRowSpan((Control)labelDesiredPosition, 2);
		labelDesiredPosition.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxDesiredPosition, "textBoxDesiredPosition");
		textBoxDesiredPosition.Name = "textBoxDesiredPosition";
		((TableLayoutPanel)(object)tableLayoutPanelInterface).SetRowSpan((Control)textBoxDesiredPosition, 2);
		textBoxDesiredPosition.TextChanged += textBoxDesiredPosition_TextChanged;
		((TableLayoutPanel)(object)tableLayoutPanelInterface).SetColumnSpan((Control)(object)seekTimeListView1, 4);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "EGRCommand";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "HH:mm:ss.fff";
		componentResourceManager.ApplyResources(buttonPlus, "buttonPlus");
		buttonPlus.Name = "buttonPlus";
		buttonPlus.UseCompatibleTextRendering = true;
		buttonPlus.UseVisualStyleBackColor = true;
		buttonPlus.Click += buttonPlus_Click;
		barInstrumentCoolantTemp.BarOrientation = (ControlOrientation)1;
		barInstrumentCoolantTemp.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentCoolantTemp, "barInstrumentCoolantTemp");
		barInstrumentCoolantTemp.FontGroup = "EGRCommandVertical";
		((SingleInstrumentBase)barInstrumentCoolantTemp).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentCoolantTemp).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature");
		((Control)(object)barInstrumentCoolantTemp).Name = "barInstrumentCoolantTemp";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)barInstrumentCoolantTemp, 6);
		((SingleInstrumentBase)barInstrumentCoolantTemp).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentCoolantTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentCoolantTemp).UnitAlignment = StringAlignment.Near;
		barInstrumentBatteryVoltage.BarOrientation = (ControlOrientation)1;
		componentResourceManager.ApplyResources(barInstrumentBatteryVoltage, "barInstrumentBatteryVoltage");
		barInstrumentBatteryVoltage.FontGroup = "EGRCommandVertical";
		((SingleInstrumentBase)barInstrumentBatteryVoltage).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentBatteryVoltage).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS021_Battery_Voltage");
		((Control)(object)barInstrumentBatteryVoltage).Name = "barInstrumentBatteryVoltage";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)barInstrumentBatteryVoltage, 6);
		((SingleInstrumentBase)barInstrumentBatteryVoltage).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentBatteryVoltage).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentBatteryVoltage).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)barInstrumentCommandedValue, 4);
		componentResourceManager.ApplyResources(barInstrumentCommandedValue, "barInstrumentCommandedValue");
		barInstrumentCommandedValue.FontGroup = null;
		((SingleInstrumentBase)barInstrumentCommandedValue).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentCommandedValue).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS031_EGR_Commanded_Governor_Value");
		((Control)(object)barInstrumentCommandedValue).Name = "barInstrumentCommandedValue";
		((SingleInstrumentBase)barInstrumentCommandedValue).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)barInstrumentActualValue, 4);
		componentResourceManager.ApplyResources(barInstrumentActualValue, "barInstrumentActualValue");
		barInstrumentActualValue.FontGroup = null;
		((SingleInstrumentBase)barInstrumentActualValue).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentActualValue).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS032_EGR_Actual_Valve_Position");
		((Control)(object)barInstrumentActualValue).Name = "barInstrumentActualValue";
		((SingleInstrumentBase)barInstrumentActualValue).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)digitalReadoutInstrumentEngineState, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineState, "digitalReadoutInstrumentEngineState");
		digitalReadoutInstrumentEngineState.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).FreezeValue = false;
		digitalReadoutInstrumentEngineState.Gradient.Initialize((ValueState)0, 8);
		digitalReadoutInstrumentEngineState.Gradient.Modify(1, -1.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(2, 0.0, (ValueState)1);
		digitalReadoutInstrumentEngineState.Gradient.Modify(3, 1.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(4, 2.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(5, 3.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(6, 4.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(7, 5.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(8, 6.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State");
		((Control)(object)digitalReadoutInstrumentEngineState).Name = "digitalReadoutInstrumentEngineState";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentEngineState.RepresentedStateChanged += digitalReadoutInstrumentEngineState_RepresentedStateChanged;
		componentResourceManager.ApplyResources(panelButtons, "panelButtons");
		panelButtons.Controls.Add(labelCanStart);
		panelButtons.Controls.Add((Control)(object)checkmarkCanStart);
		panelButtons.Controls.Add(buttonStart);
		panelButtons.Controls.Add(buttonStop);
		panelButtons.Name = "panelButtons";
		componentResourceManager.ApplyResources(labelCanStart, "labelCanStart");
		labelCanStart.Name = "labelCanStart";
		labelCanStart.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkCanStart, "checkmarkCanStart");
		((Control)(object)checkmarkCanStart).Name = "checkmarkCanStart";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		buttonStart.Click += buttonStart_Click;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		buttonStop.Click += buttonStop_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Controls.Add(panelButtons);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInterface).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInterface).PerformLayout();
		panelButtons.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
