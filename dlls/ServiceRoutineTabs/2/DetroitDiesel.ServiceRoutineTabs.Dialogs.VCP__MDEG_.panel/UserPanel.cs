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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VCP__MDEG_.panel;

public class UserPanel : CustomPanel
{
	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentVCPTestRoutine;

	private SharedProcedureSelection sharedProcedureSelectionVCPTestRoutine;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentVCPTestRoutine;

	private Button buttonVCPTestRoutineStartStop;

	private Label labelVCPTestRoutineStatus;

	private Checkmark checkmarkVCPTestRoutine;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private SeekTimeListView seekTimeListViewLog;

	private Button buttonClose;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

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
		if (sharedProcedureSelectionVCPTestRoutine.AnyProcedureInProgress)
		{
			LogText(Resources.Message_StopVCPTest);
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

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Expected O, but got Unknown
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_0967: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		DataItemCondition val3 = new DataItemCondition();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		seekTimeListViewLog = new SeekTimeListView();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		tableLayoutPanel1 = new TableLayoutPanel();
		checkmarkVCPTestRoutine = new Checkmark();
		buttonVCPTestRoutineStartStop = new Button();
		buttonClose = new Button();
		labelVCPTestRoutineStatus = new Label();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		sharedProcedureSelectionVCPTestRoutine = new SharedProcedureSelection();
		sharedProcedureCreatorComponentVCPTestRoutine = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentVCPTestRoutine = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)seekTimeListViewLog, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanel1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument3, 2, 0);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)seekTimeListViewLog, 3);
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		seekTimeListViewLog.FilterUserLabels = true;
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "VCP Test Routine";
		seekTimeListViewLog.SelectedTime = null;
		seekTimeListViewLog.ShowChannelLabels = false;
		seekTimeListViewLog.ShowCommunicationsState = false;
		seekTimeListViewLog.ShowControlPanel = false;
		seekTimeListViewLog.ShowDeviceColumn = false;
		seekTimeListViewLog.TimeFormat = "HH:mm:ss";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrument1.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
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
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanel1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkVCPTestRoutine, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelVCPTestRoutineStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonVCPTestRoutineStartStop, 2, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(checkmarkVCPTestRoutine, "checkmarkVCPTestRoutine");
		((Control)(object)checkmarkVCPTestRoutine).Name = "checkmarkVCPTestRoutine";
		componentResourceManager.ApplyResources(buttonVCPTestRoutineStartStop, "buttonVCPTestRoutineStartStop");
		buttonVCPTestRoutineStartStop.Name = "buttonVCPTestRoutineStartStop";
		buttonVCPTestRoutineStartStop.UseCompatibleTextRendering = true;
		buttonVCPTestRoutineStartStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelVCPTestRoutineStatus, "labelVCPTestRoutineStatus");
		labelVCPTestRoutineStatus.Name = "labelVCPTestRoutineStatus";
		labelVCPTestRoutineStatus.UseCompatibleTextRendering = true;
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
		componentResourceManager.ApplyResources(sharedProcedureSelectionVCPTestRoutine, "sharedProcedureSelectionVCPTestRoutine");
		((Control)(object)sharedProcedureSelectionVCPTestRoutine).Name = "sharedProcedureSelectionVCPTestRoutine";
		sharedProcedureSelectionVCPTestRoutine.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_VCP_Test" });
		sharedProcedureCreatorComponentVCPTestRoutine.Suspend();
		sharedProcedureCreatorComponentVCPTestRoutine.MonitorCall = new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Request_Results_Routine_status");
		sharedProcedureCreatorComponentVCPTestRoutine.MonitorGradient.Initialize((ValueState)0, 2);
		sharedProcedureCreatorComponentVCPTestRoutine.MonitorGradient.Modify(1, 0.0, (ValueState)3);
		sharedProcedureCreatorComponentVCPTestRoutine.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentVCPTestRoutine, "sharedProcedureCreatorComponentVCPTestRoutine");
		sharedProcedureCreatorComponentVCPTestRoutine.Qualifier = "SP_VCP_Test";
		sharedProcedureCreatorComponentVCPTestRoutine.StartCall = new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Start");
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
		sharedProcedureCreatorComponentVCPTestRoutine.StartConditions.Add(val);
		sharedProcedureCreatorComponentVCPTestRoutine.StartConditions.Add(val2);
		sharedProcedureCreatorComponentVCPTestRoutine.StartConditions.Add(val3);
		sharedProcedureCreatorComponentVCPTestRoutine.StopCall = new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Stop");
		sharedProcedureCreatorComponentVCPTestRoutine.Resume();
		sharedProcedureIntegrationComponentVCPTestRoutine.ProceduresDropDown = sharedProcedureSelectionVCPTestRoutine;
		sharedProcedureIntegrationComponentVCPTestRoutine.ProcedureStatusMessageTarget = labelVCPTestRoutineStatus;
		sharedProcedureIntegrationComponentVCPTestRoutine.ProcedureStatusStateTarget = checkmarkVCPTestRoutine;
		sharedProcedureIntegrationComponentVCPTestRoutine.ResultsTarget = null;
		sharedProcedureIntegrationComponentVCPTestRoutine.StartStopButton = buttonVCPTestRoutineStartStop;
		sharedProcedureIntegrationComponentVCPTestRoutine.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Variable_Camshaft_Phaser_VCP_");
		((Control)this).Controls.Add((Control)(object)sharedProcedureSelectionVCPTestRoutine);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
