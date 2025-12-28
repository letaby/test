using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel;

public class UserPanel : CustomPanel
{
	private double maxObservedEngineSpeed = 0.0;

	private bool showTestModeMessage = false;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentCompressionTestRoutine;

	private SharedProcedureSelection sharedProcedureSelectionCompressionTestRoutine;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentCompressionTestRoutine;

	private Button buttonCompressionTestRoutineStartStop;

	private System.Windows.Forms.Label labelCompressionTestRoutineStatus;

	private Checkmark checkmarkCompressionTestRoutine;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private Button buttonClose;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutBatteryVoltage;

	private SeekTimeListView seekTimeListViewLog;

	private System.Windows.Forms.Label labelTestInstructions;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	public UserPanel()
	{
		InitializeComponent();
		((CustomPanel)this).ParentFormClosing += OnParentFormClosing;
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelectionCompressionTestRoutine.AnyProcedureInProgress)
		{
			LogText(Resources.Message_StopCompressionTest);
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListViewLog.RequiredUserLabelPrefix, text);
	}

	private void sharedProcedureCreatorComponentCompressionTestRoutine_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		maxObservedEngineSpeed = 0.0;
		showTestModeMessage = true;
		LogText(Resources.Message_CompressionTestStarted);
		labelTestInstructions.Text = Resources.Message_PleaseTurnTheIgnition;
		buttonClose.Enabled = false;
	}

	private void sharedProcedureCreatorComponentCompressionTestRoutine_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			if (showTestModeMessage)
			{
				LogText(Resources.Message_EngineIsRunningInCompressionTestMode);
				showTestModeMessage = false;
			}
			InstrumentCollection instruments = e.Service.Channel.Instruments;
			Qualifier instrument = ((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument;
			Instrument instrument2 = instruments[((Qualifier)(ref instrument)).Name];
			double num = Convert.ToDouble(instrument2.InstrumentValues.Current.Value.ToString());
			if (num > maxObservedEngineSpeed)
			{
				maxObservedEngineSpeed = num;
			}
		}
	}

	private void sharedProcedureCreatorComponentCompressionTestRoutine_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			string text = $"{Resources.Message_MaxObservedEngineSpeed} {maxObservedEngineSpeed.ToString()} {Resources.Message_EngineSpeedUnits}";
			LogText(Resources.Message_CompressionTestStopped);
			LogText(text);
			LogText((maxObservedEngineSpeed >= 150.0) ? Resources.Message_Success : Resources.Message_Failed);
			labelTestInstructions.Text = string.Empty;
			buttonClose.Enabled = true;
		}
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Expected O, but got Unknown
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		DataItemCondition val3 = new DataItemCondition();
		DataItemCondition val4 = new DataItemCondition();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		digitalReadoutBatteryVoltage = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		tableLayoutPanel1 = new TableLayoutPanel();
		checkmarkCompressionTestRoutine = new Checkmark();
		buttonCompressionTestRoutineStartStop = new Button();
		buttonClose = new Button();
		labelCompressionTestRoutineStatus = new System.Windows.Forms.Label();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		seekTimeListViewLog = new SeekTimeListView();
		labelTestInstructions = new System.Windows.Forms.Label();
		sharedProcedureSelectionCompressionTestRoutine = new SharedProcedureSelection();
		sharedProcedureCreatorComponentCompressionTestRoutine = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentCompressionTestRoutine = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutBatteryVoltage, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanel1, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)seekTimeListViewLog, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(labelTestInstructions, 0, 2);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		componentResourceManager.ApplyResources(digitalReadoutBatteryVoltage, "digitalReadoutBatteryVoltage");
		digitalReadoutBatteryVoltage.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutBatteryVoltage).FreezeValue = false;
		digitalReadoutBatteryVoltage.Gradient.Initialize((ValueState)3, 1, "V");
		digitalReadoutBatteryVoltage.Gradient.Modify(1, 12.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutBatteryVoltage).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS021_Battery_Voltage");
		((Control)(object)digitalReadoutBatteryVoltage).Name = "digitalReadoutBatteryVoltage";
		((Control)(object)digitalReadoutBatteryVoltage).TabStop = false;
		((SingleInstrumentBase)digitalReadoutBatteryVoltage).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanel1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkCompressionTestRoutine, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonCompressionTestRoutineStartStop, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelCompressionTestRoutineStatus, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(checkmarkCompressionTestRoutine, "checkmarkCompressionTestRoutine");
		((Control)(object)checkmarkCompressionTestRoutine).Name = "checkmarkCompressionTestRoutine";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)checkmarkCompressionTestRoutine, 2);
		componentResourceManager.ApplyResources(buttonCompressionTestRoutineStartStop, "buttonCompressionTestRoutineStartStop");
		buttonCompressionTestRoutineStartStop.Name = "buttonCompressionTestRoutineStartStop";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)buttonCompressionTestRoutineStartStop, 2);
		buttonCompressionTestRoutineStartStop.UseCompatibleTextRendering = true;
		buttonCompressionTestRoutineStartStop.UseVisualStyleBackColor = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)buttonClose, 2);
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelCompressionTestRoutineStatus, "labelCompressionTestRoutineStatus");
		labelCompressionTestRoutineStatus.Name = "labelCompressionTestRoutineStatus";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)labelCompressionTestRoutineStatus, 2);
		labelCompressionTestRoutineStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)1, 5);
		digitalReadoutInstrument3.Gradient.Modify(1, 1.0, (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(2, 256.0, (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(3, "signal not available", (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(4, "No signal from input source i.e sensor/actuator", (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(5, "Values are invalid", (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS012_Vehicle_Speed");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((Control)(object)digitalReadoutInstrument3).TabStop = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)seekTimeListViewLog, 2);
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		seekTimeListViewLog.FilterUserLabels = true;
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "Compression Test Routine";
		seekTimeListViewLog.SelectedTime = null;
		seekTimeListViewLog.ShowChannelLabels = false;
		seekTimeListViewLog.ShowCommunicationsState = false;
		seekTimeListViewLog.ShowControlPanel = false;
		seekTimeListViewLog.ShowDeviceColumn = false;
		seekTimeListViewLog.TimeFormat = "HH:mm:ss";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)labelTestInstructions, 2);
		componentResourceManager.ApplyResources(labelTestInstructions, "labelTestInstructions");
		labelTestInstructions.ForeColor = Color.Red;
		labelTestInstructions.Name = "labelTestInstructions";
		labelTestInstructions.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelectionCompressionTestRoutine, "sharedProcedureSelectionCompressionTestRoutine");
		((Control)(object)sharedProcedureSelectionCompressionTestRoutine).Name = "sharedProcedureSelectionCompressionTestRoutine";
		sharedProcedureSelectionCompressionTestRoutine.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_Compression_Test" });
		sharedProcedureCreatorComponentCompressionTestRoutine.Suspend();
		sharedProcedureCreatorComponentCompressionTestRoutine.MonitorCall = new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status");
		sharedProcedureCreatorComponentCompressionTestRoutine.MonitorGradient.Initialize((ValueState)0, 2);
		sharedProcedureCreatorComponentCompressionTestRoutine.MonitorGradient.Modify(1, 0.0, (ValueState)0);
		sharedProcedureCreatorComponentCompressionTestRoutine.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentCompressionTestRoutine, "sharedProcedureCreatorComponentCompressionTestRoutine");
		sharedProcedureCreatorComponentCompressionTestRoutine.Qualifier = "SP_Compression_Test";
		sharedProcedureCreatorComponentCompressionTestRoutine.StartCall = new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status");
		val.Gradient.Initialize((ValueState)3, 4);
		val.Gradient.Modify(1, 0.0, (ValueState)3);
		val.Gradient.Modify(2, 1.0, (ValueState)1);
		val.Gradient.Modify(3, 2.0, (ValueState)3);
		val.Gradient.Modify(4, 3.0, (ValueState)3);
		val.Qualifier = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status");
		val2.Gradient.Initialize((ValueState)1, 1);
		val2.Gradient.Modify(1, 1.0, (ValueState)3);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		val3.Gradient.Initialize((ValueState)1, 5);
		val3.Gradient.Modify(1, 1.0, (ValueState)3);
		val3.Gradient.Modify(2, 256.0, (ValueState)3);
		val3.Gradient.Modify(3, "signal not available", (ValueState)3);
		val3.Gradient.Modify(4, "No signal from input source i.e sensor/actuator", (ValueState)3);
		val3.Gradient.Modify(5, "Values are invalid", (ValueState)3);
		val3.Qualifier = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS012_Vehicle_Speed");
		val4.Gradient.Initialize((ValueState)3, 1, "V");
		val4.Gradient.Modify(1, 12.0, (ValueState)1);
		val4.Qualifier = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS021_Battery_Voltage");
		sharedProcedureCreatorComponentCompressionTestRoutine.StartConditions.Add(val);
		sharedProcedureCreatorComponentCompressionTestRoutine.StartConditions.Add(val2);
		sharedProcedureCreatorComponentCompressionTestRoutine.StartConditions.Add(val3);
		sharedProcedureCreatorComponentCompressionTestRoutine.StartConditions.Add(val4);
		sharedProcedureCreatorComponentCompressionTestRoutine.StopCall = new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0");
		sharedProcedureCreatorComponentCompressionTestRoutine.StartServiceComplete += sharedProcedureCreatorComponentCompressionTestRoutine_StartServiceComplete;
		sharedProcedureCreatorComponentCompressionTestRoutine.StopServiceComplete += sharedProcedureCreatorComponentCompressionTestRoutine_StopServiceComplete;
		sharedProcedureCreatorComponentCompressionTestRoutine.MonitorServiceComplete += sharedProcedureCreatorComponentCompressionTestRoutine_MonitorServiceComplete;
		sharedProcedureCreatorComponentCompressionTestRoutine.Resume();
		sharedProcedureIntegrationComponentCompressionTestRoutine.ProceduresDropDown = sharedProcedureSelectionCompressionTestRoutine;
		sharedProcedureIntegrationComponentCompressionTestRoutine.ProcedureStatusMessageTarget = labelCompressionTestRoutineStatus;
		sharedProcedureIntegrationComponentCompressionTestRoutine.ProcedureStatusStateTarget = checkmarkCompressionTestRoutine;
		sharedProcedureIntegrationComponentCompressionTestRoutine.ResultsTarget = null;
		sharedProcedureIntegrationComponentCompressionTestRoutine.StartStopButton = buttonCompressionTestRoutineStartStop;
		sharedProcedureIntegrationComponentCompressionTestRoutine.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Mechanical_Compression_Test");
		((Control)this).Controls.Add((Control)(object)sharedProcedureSelectionCompressionTestRoutine);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
