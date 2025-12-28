using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Hysteresis_Measurement.panel;

public class UserPanel : CustomPanel
{
	private const string RequestADCPosEGRStatus = "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_is05_adc_pos_egr";

	private string is05AdcPosEGR = string.Empty;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentHysteresisRoutine;

	private SharedProcedureSelection sharedProcedureSelectionHysteresisRoutine;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentVCPTestRoutine;

	private Button buttonHysteresisRoutineStartStop;

	private System.Windows.Forms.Label labelHysteresisRoutineStatus;

	private Checkmark checkmarkHysteresisRoutine;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private SeekTimeListView seekTimeListViewLog;

	private Button buttonClose;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

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
		if (sharedProcedureSelectionHysteresisRoutine.AnyProcedureInProgress)
		{
			LogText(Resources.Message_StopHysteresisTest);
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

	private void sharedProcedureCreatorComponentHysteresisRoutine_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		Service service = e.Service.Channel.Services["RT_SR0CB_EGR_hysteresis_measurement_Request_Results_is05_adc_pos_egr"];
		if (service != null)
		{
			try
			{
				service.Execute(synchronous: true);
			}
			catch (CaesarException)
			{
				((MonitorResultEventArgs)e).Action = (MonitorAction)1;
				return;
			}
			if (!service.OutputValues[0].Value.ToString().Equals(is05AdcPosEGR))
			{
				is05AdcPosEGR = service.OutputValues[0].Value.ToString();
				LogText(is05AdcPosEGR);
			}
		}
	}

	private void sharedProcedureCreatorComponentHysteresisRoutine_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		is05AdcPosEGR = string.Empty;
	}

	private void InitializeComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Expected O, but got Unknown
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		DataItemCondition val3 = new DataItemCondition();
		checkmarkHysteresisRoutine = new Checkmark();
		buttonHysteresisRoutineStartStop = new Button();
		buttonClose = new Button();
		labelHysteresisRoutineStatus = new System.Windows.Forms.Label();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		seekTimeListViewLog = new SeekTimeListView();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		sharedProcedureSelectionHysteresisRoutine = new SharedProcedureSelection();
		sharedProcedureCreatorComponentHysteresisRoutine = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentVCPTestRoutine = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)tableLayoutPanel1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkHysteresisRoutine, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonHysteresisRoutineStartStop, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelHysteresisRoutineStatus, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(checkmarkHysteresisRoutine, "checkmarkHysteresisRoutine");
		((Control)(object)checkmarkHysteresisRoutine).Name = "checkmarkHysteresisRoutine";
		componentResourceManager.ApplyResources(buttonHysteresisRoutineStartStop, "buttonHysteresisRoutineStartStop");
		buttonHysteresisRoutineStartStop.Name = "buttonHysteresisRoutineStartStop";
		buttonHysteresisRoutineStartStop.UseCompatibleTextRendering = true;
		buttonHysteresisRoutineStartStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelHysteresisRoutineStatus, "labelHysteresisRoutineStatus");
		labelHysteresisRoutineStatus.Name = "labelHysteresisRoutineStatus";
		labelHysteresisRoutineStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument6, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument5, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)seekTimeListViewLog, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument4, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanel1, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 1);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)64, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_am_egr_current_valve");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)64, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_is05_adc_pos_egr");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).SetColumnSpan((Control)(object)seekTimeListViewLog, 2);
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "";
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
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS032_EGR_Actual_Valve_Position");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrument2.Gradient.Modify(1, 1.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(2, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ignitionStatus");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS031_EGR_Commanded_Governor_Value");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((Control)(object)digitalReadoutInstrument3).TabStop = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(sharedProcedureSelectionHysteresisRoutine, "sharedProcedureSelectionHysteresisRoutine");
		((Control)(object)sharedProcedureSelectionHysteresisRoutine).Name = "sharedProcedureSelectionHysteresisRoutine";
		sharedProcedureSelectionHysteresisRoutine.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_EGR_Hysteresis" });
		sharedProcedureCreatorComponentHysteresisRoutine.Suspend();
		sharedProcedureCreatorComponentHysteresisRoutine.MonitorCall = new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_am_egr_current_valve");
		sharedProcedureCreatorComponentHysteresisRoutine.MonitorGradient.Initialize((ValueState)0, 2);
		sharedProcedureCreatorComponentHysteresisRoutine.MonitorGradient.Modify(1, 0.0, (ValueState)0);
		sharedProcedureCreatorComponentHysteresisRoutine.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentHysteresisRoutine, "sharedProcedureCreatorComponentHysteresisRoutine");
		sharedProcedureCreatorComponentHysteresisRoutine.Qualifier = "SP_EGR_Hysteresis";
		sharedProcedureCreatorComponentHysteresisRoutine.StartCall = new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Start");
		val.Gradient.Initialize((ValueState)3, 2);
		val.Gradient.Modify(1, 1.0, (ValueState)1);
		val.Gradient.Modify(2, 2.0, (ValueState)3);
		val.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "ignitionStatus");
		val2.Gradient.Initialize((ValueState)1, 1);
		val2.Gradient.Modify(1, 1.0, (ValueState)3);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		val3.Gradient.Initialize((ValueState)1, 1);
		val3.Gradient.Modify(1, 1.0, (ValueState)3);
		val3.Qualifier = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		sharedProcedureCreatorComponentHysteresisRoutine.StartConditions.Add(val);
		sharedProcedureCreatorComponentHysteresisRoutine.StartConditions.Add(val2);
		sharedProcedureCreatorComponentHysteresisRoutine.StartConditions.Add(val3);
		sharedProcedureCreatorComponentHysteresisRoutine.StopCall = new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Stop");
		sharedProcedureCreatorComponentHysteresisRoutine.StartServiceComplete += sharedProcedureCreatorComponentHysteresisRoutine_StartServiceComplete;
		sharedProcedureCreatorComponentHysteresisRoutine.MonitorServiceComplete += sharedProcedureCreatorComponentHysteresisRoutine_MonitorServiceComplete;
		sharedProcedureCreatorComponentHysteresisRoutine.Resume();
		sharedProcedureIntegrationComponentVCPTestRoutine.ProceduresDropDown = sharedProcedureSelectionHysteresisRoutine;
		sharedProcedureIntegrationComponentVCPTestRoutine.ProcedureStatusMessageTarget = labelHysteresisRoutineStatus;
		sharedProcedureIntegrationComponentVCPTestRoutine.ProcedureStatusStateTarget = checkmarkHysteresisRoutine;
		sharedProcedureIntegrationComponentVCPTestRoutine.ResultsTarget = null;
		sharedProcedureIntegrationComponentVCPTestRoutine.StartStopButton = buttonHysteresisRoutineStartStop;
		sharedProcedureIntegrationComponentVCPTestRoutine.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)sharedProcedureSelectionHysteresisRoutine);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
