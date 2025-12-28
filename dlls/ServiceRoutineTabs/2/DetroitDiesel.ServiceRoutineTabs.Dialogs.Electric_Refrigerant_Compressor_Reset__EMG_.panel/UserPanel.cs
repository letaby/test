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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Electric_Refrigerant_Compressor_Reset__EMG_.panel;

public class UserPanel : CustomPanel
{
	private bool ResetCanceled = false;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehSpeed;

	private TableLayoutPanel tableLayoutPanel2;

	private Checkmark checkmark1;

	private SeekTimeListView seekTimeListView1;

	private DigitalReadoutInstrument digitalReadoutInstrumentResetStatus;

	private SharedProcedureSelection sharedProcedureSelection1;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponent1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private Button button1;

	private System.Windows.Forms.Label labelServiceStartStatus;

	private bool Running => (int)sharedProcedureSelection1.SelectedProcedure.State != 0;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel |= Running;
		if (!e.Cancel)
		{
			((CustomPanel)this).ParentFormClosing -= UserPanel_ParentFormClosing;
		}
	}

	private void UpdateMessage(string message)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, message);
	}

	private void sharedProcedureCreatorComponent1_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		string empty = string.Empty;
		empty = (((ResultEventArgs)(object)e).Succeeded ? Resources.MessageErcResetStarted : ((((ResultEventArgs)(object)e).Exception == null || string.IsNullOrEmpty(((ResultEventArgs)(object)e).Exception.Message)) ? Resources.MessageErcResetStartFailed : $"{Resources.MessageErcResetStartFailed} Error: {((ResultEventArgs)(object)e).Exception.Message}."));
		UpdateMessage(empty);
	}

	private void sharedProcedureCreatorComponent1_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		bool flag = (int)digitalReadoutInstrumentResetStatus.RepresentedState == 1;
		UpdateMessage(ResetCanceled ? Resources.MessageErcResetStopped : (flag ? Resources.MessageErcResetPassed : Resources.MessageErcResetFailed));
	}

	private void button1_Click(object sender, EventArgs e)
	{
		ResetCanceled = Running;
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Expected O, but got Unknown
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehSpeed = new DigitalReadoutInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		checkmark1 = new Checkmark();
		labelServiceStartStatus = new System.Windows.Forms.Label();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		button1 = new Button();
		seekTimeListView1 = new SeekTimeListView();
		digitalReadoutInstrumentResetStatus = new DigitalReadoutInstrument();
		sharedProcedureCreatorComponent1 = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBrake, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentVehSpeed, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentResetStatus, 0, 2);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
		digitalReadoutInstrumentParkingBrake.FontGroup = "MainInstruments";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).FreezeValue = false;
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		((Control)(object)digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehSpeed, "digitalReadoutInstrumentVehSpeed");
		digitalReadoutInstrumentVehSpeed.FontGroup = "MainInstruments";
		((SingleInstrumentBase)digitalReadoutInstrumentVehSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentVehSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentVehSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentVehSpeed.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentVehSpeed.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentVehSpeed.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehSpeed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		((Control)(object)digitalReadoutInstrumentVehSpeed).Name = "digitalReadoutInstrumentVehSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmark1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(labelServiceStartStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)sharedProcedureSelection1, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(button1, 2, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		componentResourceManager.ApplyResources(labelServiceStartStatus, "labelServiceStartStatus");
		labelServiceStartStatus.Name = "labelServiceStartStatus";
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_ElectricRefrigerantCompressorReset" });
		componentResourceManager.ApplyResources(button1, "button1");
		button1.Name = "button1";
		button1.UseVisualStyleBackColor = true;
		button1.Click += button1_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 2);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "ErcReset";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentResetStatus, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentResetStatus, "digitalReadoutInstrumentResetStatus");
		digitalReadoutInstrumentResetStatus.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentResetStatus).FreezeValue = false;
		digitalReadoutInstrumentResetStatus.Gradient.Initialize((ValueState)0, 6);
		digitalReadoutInstrumentResetStatus.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentResetStatus.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentResetStatus.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentResetStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentResetStatus.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrumentResetStatus.Gradient.Modify(6, 21.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentResetStatus).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results_ERC_Reset_Ctrl");
		((Control)(object)digitalReadoutInstrumentResetStatus).Name = "digitalReadoutInstrumentResetStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentResetStatus).UnitAlignment = StringAlignment.Near;
		sharedProcedureCreatorComponent1.Suspend();
		sharedProcedureCreatorComponent1.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results");
		sharedProcedureCreatorComponent1.MonitorGradient.Initialize((ValueState)0, 6);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(1, 0.0, (ValueState)0);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(6, 21.0, (ValueState)3);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponent1, "sharedProcedureCreatorComponent1");
		sharedProcedureCreatorComponent1.Qualifier = "SP_ElectricRefrigerantCompressorReset";
		sharedProcedureCreatorComponent1.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Start");
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		val.Gradient.Initialize((ValueState)2, 4);
		val.Gradient.Modify(1, 0.0, (ValueState)3);
		val.Gradient.Modify(2, 1.0, (ValueState)1);
		val.Gradient.Modify(3, 2.0, (ValueState)2);
		val.Gradient.Modify(4, 3.0, (ValueState)2);
		val.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		val2.Gradient.Initialize((ValueState)3, 2);
		val2.Gradient.Modify(1, 0.0, (ValueState)1);
		val2.Gradient.Modify(2, 1.0, (ValueState)3);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		sharedProcedureCreatorComponent1.StartConditions.Add(val);
		sharedProcedureCreatorComponent1.StartConditions.Add(val2);
		sharedProcedureCreatorComponent1.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Stop");
		sharedProcedureCreatorComponent1.StartServiceComplete += sharedProcedureCreatorComponent1_StartServiceComplete;
		sharedProcedureCreatorComponent1.StopServiceComplete += sharedProcedureCreatorComponent1_StopServiceComplete;
		sharedProcedureCreatorComponent1.Resume();
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelServiceStartStatus;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = button1;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_ERC_Reset");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
