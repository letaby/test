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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Three_By_Two_Way_Valve_Teach_In__EMG_.panel;

public class UserPanel : CustomPanel
{
	private Channel eCpcChannel;

	private bool running = false;

	private TableLayoutPanel tableLayoutPanel1;

	private SeekTimeListView seekTimeListView1;

	private TableLayoutPanel tableLayoutPanelCoolantStart;

	private Button buttonStartCoolant;

	private System.Windows.Forms.Label label3;

	private System.Windows.Forms.Label label2;

	private TableLayoutPanel tableLayoutPanelBatteryStart;

	private Button buttonStartBattery;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentBattery;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentBattery;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentCoolant;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentCoolant;

	private SharedProcedureSelection sharedProcedureSelectionBattery;

	private SharedProcedureSelection sharedProcedureSelectionCoolant;

	private TableLayoutPanel tableLayoutPanelText;

	private TableLayoutPanel tableLayoutPanelText2;

	private System.Windows.Forms.Label label6;

	private TableLayoutPanel tableLayoutPanelBatteryMessages;

	private System.Windows.Forms.Label labelStatusBattery;

	private Checkmark checkmarkStatusBattery;

	private TableLayoutPanel tableLayoutPanelCoolantMessage;

	private System.Windows.Forms.Label labelStatusCoolant;

	private Checkmark checkmarkStatusCoolant;

	private DigitalReadoutInstrument digitalReadoutInstrumentBatteryResults;

	private DigitalReadoutInstrument digitalReadoutInstrumentCoolantResults;

	private TableLayoutPanel tableLayoutPanel4;

	private DigitalReadoutInstrument digitalReadoutInstrumentCharging;

	private System.Windows.Forms.Label label1;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private System.Windows.Forms.Label labelInterlockWarning;

	private System.Windows.Forms.Label label39;

	private Button buttonClose;

	private System.Windows.Forms.Label label5;

	private bool CanStart => (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && (int)digitalReadoutInstrumentParkBrake.RepresentedState == 1 && (int)digitalReadoutInstrumentCharging.RepresentedState == 1;

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
		running = false;
		UpdateUI();
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = running;
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	public override void OnChannelsChanged()
	{
		SetECPC01TChannel("ECPC01T");
	}

	private void SetECPC01TChannel(string ecuName)
	{
		if (eCpcChannel != ((CustomPanel)this).GetChannel(ecuName))
		{
			running = false;
			eCpcChannel = ((CustomPanel)this).GetChannel(ecuName);
		}
		UpdateUI();
	}

	private void AddLogLabel(string text)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
		}
	}

	private void UpdateUI()
	{
		labelInterlockWarning.Visible = !CanStart;
		buttonClose.Enabled = !running;
	}

	private void sharedProcedureCreatorComponentBattery_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel(Resources.Message_BatteryCoolantSystem3by2WayValveTeachInStarted);
		}
		else
		{
			running = false;
			AddLogLabel(Resources.Message_BatteryCoolantSystem3by2WayValveTeachInFailedToStart);
			AddLogLabel(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentBattery_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		AddLogLabel(Resources.Message_BatteryCoolantSystem3by2WayValveTeachInStopped);
		running = false;
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentCoolant_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel(Resources.Message_EdriveCoolantSystem3by2WayValveTeachInStarted);
		}
		else
		{
			running = false;
			AddLogLabel(Resources.Message_EdriveCoolantSystem3by2WayValveTeachInFailedToStart);
			AddLogLabel(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentCoolant_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		AddLogLabel(Resources.Message_EdriveCoolantSystem3by2WayValveTeachInStopped);
		running = false;
		UpdateUI();
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		running = true;
		UpdateUI();
	}

	private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
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
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected O, but got Unknown
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c73: Expected O, but got Unknown
		//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cba: Expected O, but got Unknown
		//IL_137b: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1906: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec2: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_21fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_22a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_234f: Unknown result type (might be due to invalid IL or missing references)
		//IL_23c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2422: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		DataItemCondition val3 = new DataItemCondition();
		DataItemCondition val4 = new DataItemCondition();
		DataItemCondition val5 = new DataItemCondition();
		DataItemCondition val6 = new DataItemCondition();
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
		label1 = new System.Windows.Forms.Label();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		labelInterlockWarning = new System.Windows.Forms.Label();
		label39 = new System.Windows.Forms.Label();
		seekTimeListView1 = new SeekTimeListView();
		tableLayoutPanelCoolantStart = new TableLayoutPanel();
		buttonStartCoolant = new Button();
		label3 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		tableLayoutPanelBatteryStart = new TableLayoutPanel();
		buttonStartBattery = new Button();
		sharedProcedureSelectionBattery = new SharedProcedureSelection();
		sharedProcedureSelectionCoolant = new SharedProcedureSelection();
		tableLayoutPanelText = new TableLayoutPanel();
		label5 = new System.Windows.Forms.Label();
		tableLayoutPanelText2 = new TableLayoutPanel();
		label6 = new System.Windows.Forms.Label();
		tableLayoutPanelBatteryMessages = new TableLayoutPanel();
		labelStatusBattery = new System.Windows.Forms.Label();
		checkmarkStatusBattery = new Checkmark();
		tableLayoutPanelCoolantMessage = new TableLayoutPanel();
		labelStatusCoolant = new System.Windows.Forms.Label();
		checkmarkStatusCoolant = new Checkmark();
		digitalReadoutInstrumentBatteryResults = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCoolantResults = new DigitalReadoutInstrument();
		buttonClose = new Button();
		sharedProcedureIntegrationComponentBattery = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentBattery = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentCoolant = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentCoolant = new SharedProcedureCreatorComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)(object)tableLayoutPanelCoolantStart).SuspendLayout();
		((Control)(object)tableLayoutPanelBatteryStart).SuspendLayout();
		((Control)(object)tableLayoutPanelText).SuspendLayout();
		((Control)(object)tableLayoutPanelText2).SuspendLayout();
		((Control)(object)tableLayoutPanelBatteryMessages).SuspendLayout();
		((Control)(object)tableLayoutPanelCoolantMessage).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel4, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 1, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelCoolantStart, 9, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label3, 6, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelBatteryStart, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelectionBattery, 5, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelectionCoolant, 5, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelText, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelText2, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelBatteryMessages, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelCoolantMessage, 6, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentBatteryResults, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentCoolantResults, 6, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 9, 10);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentCharging, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(label1, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(labelInterlockWarning, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(label39, 0, 2);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanel4, 11);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCharging, "digitalReadoutInstrumentCharging");
		digitalReadoutInstrumentCharging.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).FreezeValue = false;
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentCharging.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentCharging.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentCharging.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition");
		((Control)(object)digitalReadoutInstrumentCharging).Name = "digitalReadoutInstrumentCharging";
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentCharging.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)3, 5, "mph");
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(5, 2147483647.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
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
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(labelInterlockWarning, "labelInterlockWarning");
		labelInterlockWarning.ForeColor = Color.Red;
		labelInterlockWarning.Name = "labelInterlockWarning";
		labelInterlockWarning.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label39, "label39");
		label39.Name = "label39";
		label39.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 9);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "3by2WayValveTeachInEMG";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(tableLayoutPanelCoolantStart, "tableLayoutPanelCoolantStart");
		((TableLayoutPanel)(object)tableLayoutPanelCoolantStart).Controls.Add(buttonStartCoolant, 0, 0);
		((Control)(object)tableLayoutPanelCoolantStart).Name = "tableLayoutPanelCoolantStart";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanelCoolantStart, 2);
		componentResourceManager.ApplyResources(buttonStartCoolant, "buttonStartCoolant");
		buttonStartCoolant.Name = "buttonStartCoolant";
		buttonStartCoolant.UseCompatibleTextRendering = true;
		buttonStartCoolant.UseVisualStyleBackColor = true;
		buttonStartCoolant.Click += buttonStart_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label3, 4);
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label2, 4);
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		componentResourceManager.ApplyResources(tableLayoutPanelBatteryStart, "tableLayoutPanelBatteryStart");
		((TableLayoutPanel)(object)tableLayoutPanelBatteryStart).Controls.Add(buttonStartBattery, 0, 0);
		((Control)(object)tableLayoutPanelBatteryStart).Name = "tableLayoutPanelBatteryStart";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)tableLayoutPanelBatteryStart, 2);
		componentResourceManager.ApplyResources(buttonStartBattery, "buttonStartBattery");
		buttonStartBattery.Name = "buttonStartBattery";
		buttonStartBattery.UseCompatibleTextRendering = true;
		buttonStartBattery.UseVisualStyleBackColor = true;
		buttonStartBattery.Click += buttonStart_Click;
		componentResourceManager.ApplyResources(sharedProcedureSelectionBattery, "sharedProcedureSelectionBattery");
		((Control)(object)sharedProcedureSelectionBattery).Name = "sharedProcedureSelectionBattery";
		sharedProcedureSelectionBattery.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_BatteryCoolant3By2WayValveTeachIn" });
		componentResourceManager.ApplyResources(sharedProcedureSelectionCoolant, "sharedProcedureSelectionCoolant");
		((Control)(object)sharedProcedureSelectionCoolant).Name = "sharedProcedureSelectionCoolant";
		sharedProcedureSelectionCoolant.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_EdriveCoolant3By2WayTeachIn" });
		componentResourceManager.ApplyResources(tableLayoutPanelText, "tableLayoutPanelText");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelText, 9);
		((TableLayoutPanel)(object)tableLayoutPanelText).Controls.Add(label5, 0, 0);
		((Control)(object)tableLayoutPanelText).Name = "tableLayoutPanelText";
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		componentResourceManager.ApplyResources(tableLayoutPanelText2, "tableLayoutPanelText2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelText2, 9);
		((TableLayoutPanel)(object)tableLayoutPanelText2).Controls.Add(label6, 0, 0);
		((Control)(object)tableLayoutPanelText2).Name = "tableLayoutPanelText2";
		componentResourceManager.ApplyResources(label6, "label6");
		label6.Name = "label6";
		componentResourceManager.ApplyResources(tableLayoutPanelBatteryMessages, "tableLayoutPanelBatteryMessages");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelBatteryMessages, 4);
		((TableLayoutPanel)(object)tableLayoutPanelBatteryMessages).Controls.Add(labelStatusBattery, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBatteryMessages).Controls.Add((Control)(object)checkmarkStatusBattery, 0, 0);
		((Control)(object)tableLayoutPanelBatteryMessages).Name = "tableLayoutPanelBatteryMessages";
		((TableLayoutPanel)(object)tableLayoutPanelBatteryMessages).SetColumnSpan((Control)labelStatusBattery, 2);
		componentResourceManager.ApplyResources(labelStatusBattery, "labelStatusBattery");
		labelStatusBattery.Name = "labelStatusBattery";
		labelStatusBattery.UseCompatibleTextRendering = true;
		checkmarkStatusBattery.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkmarkStatusBattery, "checkmarkStatusBattery");
		((Control)(object)checkmarkStatusBattery).Name = "checkmarkStatusBattery";
		componentResourceManager.ApplyResources(tableLayoutPanelCoolantMessage, "tableLayoutPanelCoolantMessage");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelCoolantMessage, 4);
		((TableLayoutPanel)(object)tableLayoutPanelCoolantMessage).Controls.Add(labelStatusCoolant, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelCoolantMessage).Controls.Add((Control)(object)checkmarkStatusCoolant, 0, 0);
		((Control)(object)tableLayoutPanelCoolantMessage).Name = "tableLayoutPanelCoolantMessage";
		((TableLayoutPanel)(object)tableLayoutPanelCoolantMessage).SetColumnSpan((Control)labelStatusCoolant, 2);
		componentResourceManager.ApplyResources(labelStatusCoolant, "labelStatusCoolant");
		labelStatusCoolant.Name = "labelStatusCoolant";
		labelStatusCoolant.UseCompatibleTextRendering = true;
		checkmarkStatusCoolant.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkmarkStatusCoolant, "checkmarkStatusCoolant");
		((Control)(object)checkmarkStatusCoolant).Name = "checkmarkStatusCoolant";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentBatteryResults, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBatteryResults, "digitalReadoutInstrumentBatteryResults");
		digitalReadoutInstrumentBatteryResults.FontGroup = "Testing";
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryResults).FreezeValue = false;
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
		digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
		digitalReadoutInstrumentBatteryResults.Gradient.Initialize((ValueState)0, 15);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(7, 6.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(8, 7.0, (ValueState)0);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(9, 8.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(10, 9.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(11, 10.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(12, 11.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(13, 12.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(14, 13.0, (ValueState)3);
		digitalReadoutInstrumentBatteryResults.Gradient.Modify(15, 14.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryResults).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState");
		((Control)(object)digitalReadoutInstrumentBatteryResults).Name = "digitalReadoutInstrumentBatteryResults";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrumentBatteryResults, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryResults).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryResults).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentCoolantResults, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCoolantResults, "digitalReadoutInstrumentCoolantResults");
		digitalReadoutInstrumentCoolantResults.FontGroup = "Testing";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantResults).FreezeValue = false;
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText36"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText37"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText38"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText39"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText40"));
		digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText41"));
		digitalReadoutInstrumentCoolantResults.Gradient.Initialize((ValueState)0, 15);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(7, 6.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(8, 7.0, (ValueState)0);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(9, 8.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(10, 9.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(11, 10.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(12, 11.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(13, 12.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(14, 13.0, (ValueState)3);
		digitalReadoutInstrumentCoolantResults.Gradient.Modify(15, 14.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantResults).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1");
		((Control)(object)digitalReadoutInstrumentCoolantResults).Name = "digitalReadoutInstrumentCoolantResults";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrumentCoolantResults, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantResults).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantResults).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		sharedProcedureIntegrationComponentBattery.ProceduresDropDown = sharedProcedureSelectionBattery;
		sharedProcedureIntegrationComponentBattery.ProcedureStatusMessageTarget = labelStatusBattery;
		sharedProcedureIntegrationComponentBattery.ProcedureStatusStateTarget = checkmarkStatusBattery;
		sharedProcedureIntegrationComponentBattery.ResultsTarget = null;
		sharedProcedureIntegrationComponentBattery.StartStopButton = buttonStartBattery;
		sharedProcedureIntegrationComponentBattery.StopAllButton = null;
		sharedProcedureCreatorComponentBattery.Suspend();
		sharedProcedureCreatorComponentBattery.AllowStopAlways = true;
		sharedProcedureCreatorComponentBattery.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState");
		sharedProcedureCreatorComponentBattery.MonitorGradient.Initialize((ValueState)0, 15);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(1, 0.0, (ValueState)1);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(3, 2.0, (ValueState)0);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(6, 5.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(7, 6.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(8, 7.0, (ValueState)0);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(9, 8.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(10, 9.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(11, 10.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(12, 11.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(13, 12.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(14, 13.0, (ValueState)3);
		sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(15, 14.0, (ValueState)3);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentBattery, "sharedProcedureCreatorComponentBattery");
		sharedProcedureCreatorComponentBattery.Qualifier = "SP_BatteryCoolant3By2WayValveTeachIn";
		sharedProcedureCreatorComponentBattery.StartCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_BatteryCirc", (IEnumerable<string>)new string[3] { "3by2WayValveBatteryCircuit=1", "3by2WayValveExtCircuit1=0", "3by2WayValveExtCircuit2=0" });
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText42"));
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText43"));
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText44"));
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText45"));
		val.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText46"));
		val.Gradient.Initialize((ValueState)0, 4);
		val.Gradient.Modify(1, 0.0, (ValueState)3);
		val.Gradient.Modify(2, 1.0, (ValueState)1);
		val.Gradient.Modify(3, 2.0, (ValueState)3);
		val.Gradient.Modify(4, 3.0, (ValueState)3);
		val.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		val2.Gradient.Initialize((ValueState)3, 5, "mph");
		val2.Gradient.Modify(1, 0.0, (ValueState)3);
		val2.Gradient.Modify(2, 1.0, (ValueState)1);
		val2.Gradient.Modify(3, 2.0, (ValueState)3);
		val2.Gradient.Modify(4, 3.0, (ValueState)3);
		val2.Gradient.Modify(5, 2147483647.0, (ValueState)3);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
		val3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText47"));
		val3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText48"));
		val3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText49"));
		val3.Gradient.Initialize((ValueState)3, 2);
		val3.Gradient.Modify(1, 0.0, (ValueState)1);
		val3.Gradient.Modify(2, 1.0, (ValueState)3);
		val3.Qualifier = new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition");
		sharedProcedureCreatorComponentBattery.StartConditions.Add(val);
		sharedProcedureCreatorComponentBattery.StartConditions.Add(val2);
		sharedProcedureCreatorComponentBattery.StartConditions.Add(val3);
		sharedProcedureCreatorComponentBattery.StopCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_BasicCircValvePosCtrlState", (IEnumerable<string>)new string[3] { "BasicCircValvePosCtrlState=1", "ExtCircValvePosCtrlState_1=0", "ExtCircValvePosCtrlState_2=0" });
		sharedProcedureCreatorComponentBattery.StartServiceComplete += sharedProcedureCreatorComponentBattery_StartServiceComplete;
		sharedProcedureCreatorComponentBattery.StopServiceComplete += sharedProcedureCreatorComponentBattery_StopServiceComplete;
		sharedProcedureCreatorComponentBattery.Resume();
		sharedProcedureIntegrationComponentCoolant.ProceduresDropDown = sharedProcedureSelectionCoolant;
		sharedProcedureIntegrationComponentCoolant.ProcedureStatusMessageTarget = labelStatusCoolant;
		sharedProcedureIntegrationComponentCoolant.ProcedureStatusStateTarget = checkmarkStatusCoolant;
		sharedProcedureIntegrationComponentCoolant.ResultsTarget = null;
		sharedProcedureIntegrationComponentCoolant.StartStopButton = buttonStartCoolant;
		sharedProcedureIntegrationComponentCoolant.StopAllButton = null;
		sharedProcedureCreatorComponentCoolant.Suspend();
		sharedProcedureCreatorComponentCoolant.AllowStopAlways = true;
		sharedProcedureCreatorComponentCoolant.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1");
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Initialize((ValueState)0, 15);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(1, 0.0, (ValueState)1);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(3, 2.0, (ValueState)0);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(6, 5.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(7, 6.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(8, 7.0, (ValueState)0);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(9, 8.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(10, 9.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(11, 10.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(12, 11.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(13, 12.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(14, 13.0, (ValueState)3);
		sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(15, 14.0, (ValueState)3);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentCoolant, "sharedProcedureCreatorComponentCoolant");
		sharedProcedureCreatorComponentCoolant.Qualifier = "SP_EdriveCoolant3By2WayTeachIn";
		sharedProcedureCreatorComponentCoolant.StartCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_ExtensionCkt_1", (IEnumerable<string>)new string[3] { "3by2WayValveBatteryCircuit=0", "3by2WayValveExtCircuit1=1", "3by2WayValveExtCircuit2=0" });
		val4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText50"));
		val4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText51"));
		val4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText52"));
		val4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText53"));
		val4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText54"));
		val4.Gradient.Initialize((ValueState)0, 4);
		val4.Gradient.Modify(1, 0.0, (ValueState)3);
		val4.Gradient.Modify(2, 1.0, (ValueState)1);
		val4.Gradient.Modify(3, 2.0, (ValueState)3);
		val4.Gradient.Modify(4, 3.0, (ValueState)3);
		val4.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		val5.Gradient.Initialize((ValueState)3, 5, "mph");
		val5.Gradient.Modify(1, 0.0, (ValueState)3);
		val5.Gradient.Modify(2, 1.0, (ValueState)1);
		val5.Gradient.Modify(3, 2.0, (ValueState)3);
		val5.Gradient.Modify(4, 3.0, (ValueState)3);
		val5.Gradient.Modify(5, 2147483647.0, (ValueState)3);
		val5.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
		val6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText55"));
		val6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText56"));
		val6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText57"));
		val6.Gradient.Initialize((ValueState)3, 2);
		val6.Gradient.Modify(1, 0.0, (ValueState)1);
		val6.Gradient.Modify(2, 1.0, (ValueState)3);
		val6.Qualifier = new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition");
		sharedProcedureCreatorComponentCoolant.StartConditions.Add(val4);
		sharedProcedureCreatorComponentCoolant.StartConditions.Add(val5);
		sharedProcedureCreatorComponentCoolant.StartConditions.Add(val6);
		sharedProcedureCreatorComponentCoolant.StopCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_ExtCircValvePosCtrlState_1", (IEnumerable<string>)new string[3] { "BasicCircValvePosCtrlState=0", "ExtCircValvePosCtrlState_1=1", "ExtCircValvePosCtrlState_2=0" });
		sharedProcedureCreatorComponentCoolant.StartServiceComplete += sharedProcedureCreatorComponentCoolant_StartServiceComplete;
		sharedProcedureCreatorComponentCoolant.StopServiceComplete += sharedProcedureCreatorComponentCoolant_StopServiceComplete;
		sharedProcedureCreatorComponentCoolant.Resume();
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_3by2_Way_Valve_Teach_In");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		((Control)(object)tableLayoutPanelCoolantStart).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelBatteryStart).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelText).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelText).PerformLayout();
		((Control)(object)tableLayoutPanelText2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelText2).PerformLayout();
		((Control)(object)tableLayoutPanelBatteryMessages).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelCoolantMessage).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
