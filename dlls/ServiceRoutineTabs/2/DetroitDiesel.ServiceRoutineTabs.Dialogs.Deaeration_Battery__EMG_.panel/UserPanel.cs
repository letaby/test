using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Deaeration_Battery__EMG_.panel;

public class UserPanel : CustomPanel
{
	private const string BatteryPumpCountParameterQualifier = "ethm_p_BattCirc_WtrPmpNum_u8";

	private const string DeaerationValveTimeParameterQualifier = "ethm_p_BattCirc_DeaerValveTime_u16";

	private Channel ecpcChannel;

	private int runTimeSec = 0;

	private bool running = false;

	private bool timespanAdjusted = false;

	private Parameter batteryPumpCountParameter;

	private Parameter deaerationValveTimeParameter;

	private TableLayoutPanel tableLayoutPanelMain;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponent1;

	private Button buttonStart;

	private SharedProcedureSelection sharedProcedureSelection1;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private System.Windows.Forms.Label labelInterlockWarning;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private SeekTimeListView seekTimeListView;

	private TableLayoutPanel tableLayoutPanel1;

	private Checkmark checkmarkSpStatusLabel;

	private ProgressBar progressBarProgress;

	private System.Windows.Forms.Label labelProgress;

	private Button buttonClose;

	private System.Windows.Forms.Label labelSpStatusLabel;

	private System.Windows.Forms.Label label1;

	private RunServicesButton runServicesButtonReturnControl;

	private System.Windows.Forms.Label label41;

	private TableLayoutPanel tableLayoutPanelInstruments;

	private ChartInstrument chartInstrument1;

	private TableLayoutPanel tableLayoutPanel4;

	private DigitalReadoutInstrument digitalReadoutInstrumentCharging;

	private System.Windows.Forms.Label label2;

	private System.Windows.Forms.Label label39;

	private int EstimatedRunTimeSec
	{
		get
		{
			if (ecpcChannel != null && deaerationValveTimeParameter != null && deaerationValveTimeParameter.Value != null && (double)deaerationValveTimeParameter.Value > 0.0)
			{
				return (int)((double)deaerationValveTimeParameter.Value * 3.0);
			}
			return 840;
		}
	}

	private int BatteryWaterPumpCount
	{
		get
		{
			if (ecpcChannel != null && batteryPumpCountParameter != null && batteryPumpCountParameter.Value != null && (double)batteryPumpCountParameter.Value == 2.0)
			{
				return 2;
			}
			return 1;
		}
	}

	private bool EcpcOnline => ecpcChannel != null && ecpcChannel.CommunicationsState == CommunicationsState.Online;

	private bool CanStart => ((int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 || (int)digitalReadoutInstrumentParkBrake.RepresentedState == 1) && (int)digitalReadoutInstrumentCharging.RepresentedState == 1;

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
		UpdateUI();
	}

	public override void OnChannelsChanged()
	{
		SetECPC01TChannel("ECPC01T");
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = running;
		if (!e.Cancel)
		{
			if (ecpcChannel != null)
			{
				ecpcChannel.CommunicationsStateUpdateEvent -= ecpcChannel_CommunicationsStateUpdateEvent;
				ecpcChannel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
			}
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

	private void UpdateUI()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (BatteryWaterPumpCount == 1 && ((Collection<Qualifier>)(object)chartInstrument1.Instruments).Count == 3)
		{
			((Collection<Qualifier>)(object)chartInstrument1.Instruments).Remove(new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br2"));
		}
		else if (BatteryWaterPumpCount == 2 && ((Collection<Qualifier>)(object)chartInstrument1.Instruments).Count == 2)
		{
			((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br2"));
		}
		checkmarkSpStatusLabel.Checked = CanStart;
		((Control)(object)runServicesButtonReturnControl).Enabled = !running && CanStart && EcpcOnline;
		buttonStart.Enabled = CanStart && EcpcOnline;
		labelInterlockWarning.Visible = !CanStart;
		if (!CanStart)
		{
			labelSpStatusLabel.Text = Resources.Message_TheParkBrakeMustBeSetOrTheVehicleSpeedMustBe0;
		}
		else if (labelSpStatusLabel.Text == Resources.Message_TheParkBrakeMustBeSetOrTheVehicleSpeedMustBe0)
		{
			labelSpStatusLabel.Text = Resources.Message_Ready;
		}
		if (!timespanAdjusted && ecpcChannel != null && deaerationValveTimeParameter != null)
		{
			chartInstrument1.SpanOfView = new TimeSpan(0, 0, EstimatedRunTimeSec + 30);
			timespanAdjusted = true;
		}
		ProgressBar progressBar = progressBarProgress;
		bool visible = (labelProgress.Visible = running);
		progressBar.Visible = visible;
		buttonClose.Enabled = !running;
	}

	private void SetECPC01TChannel(string ecuName)
	{
		Channel channel = ((CustomPanel)this).GetChannel(ecuName, (ChannelLookupOptions)3);
		if (ecpcChannel != channel)
		{
			deaerationValveTimeParameter = (batteryPumpCountParameter = null);
			if (ecpcChannel != null)
			{
				if (batteryPumpCountParameter != null)
				{
					batteryPumpCountParameter.ParameterUpdateEvent -= batteryPumpCountParameter_ParameterUpdateEvent;
					batteryPumpCountParameter = null;
				}
				if (deaerationValveTimeParameter != null)
				{
					deaerationValveTimeParameter.ParameterUpdateEvent -= deaerationValveTimeParameter_ParameterUpdateEvent;
					deaerationValveTimeParameter = null;
				}
				ecpcChannel.CommunicationsStateUpdateEvent -= ecpcChannel_CommunicationsStateUpdateEvent;
				ecpcChannel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
				running = false;
			}
			ecpcChannel = channel;
			if (ecpcChannel != null)
			{
				batteryPumpCountParameter = ecpcChannel.Parameters["ethm_p_BattCirc_WtrPmpNum_u8"];
				if (batteryPumpCountParameter != null)
				{
					batteryPumpCountParameter.ParameterUpdateEvent += batteryPumpCountParameter_ParameterUpdateEvent;
				}
				deaerationValveTimeParameter = ecpcChannel.Parameters["ethm_p_BattCirc_DeaerValveTime_u16"];
				if (deaerationValveTimeParameter != null)
				{
					deaerationValveTimeParameter.ParameterUpdateEvent += deaerationValveTimeParameter_ParameterUpdateEvent;
				}
				ecpcChannel.CommunicationsStateUpdateEvent += ecpcChannel_CommunicationsStateUpdateEvent;
				ecpcChannel.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
				ReadParameters();
				running = false;
			}
		}
		UpdateUI();
	}

	private void batteryPumpCountParameter_ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateUI();
	}

	private void deaerationValveTimeParameter_ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateUI();
	}

	private void ReadParameters()
	{
		if (EcpcOnline && batteryPumpCountParameter != null && !batteryPumpCountParameter.HasBeenReadFromEcu)
		{
			ecpcChannel.Parameters.ReadGroup(batteryPumpCountParameter.GroupQualifier, fromCache: false, synchronous: false);
		}
		if (EcpcOnline && deaerationValveTimeParameter != null && !deaerationValveTimeParameter.HasBeenReadFromEcu)
		{
			ecpcChannel.Parameters.ReadGroup(deaerationValveTimeParameter.GroupQualifier, fromCache: false, synchronous: false);
		}
		UpdateUI();
	}

	private void ecpcChannel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		ReadParameters();
	}

	private void sharedProcedureCreatorComponent1_MonitorServiceComplete(object sender, MonitorServiceResultEventArgs e)
	{
		runTimeSec += sharedProcedureCreatorComponent1.MonitorInterval / 1000;
		int num = runTimeSec * 100 / EstimatedRunTimeSec;
		if (progressBarProgress.Value != num)
		{
			AddLogLabel(num + Resources.Message_Complete);
		}
		progressBarProgress.Value = ((num > progressBarProgress.Maximum) ? progressBarProgress.Maximum : num);
		UpdateUI();
	}

	private void sharedProcedureCreatorComponent1_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		runTimeSec = 0;
		progressBarProgress.Value = 0;
		running = ((ResultEventArgs)(object)e).Succeeded;
		if (running)
		{
			AddLogLabel(Resources.Message_DeAirationStarted);
			AddLogLabel(Resources.Message_EstimatedRuntime + EstimatedRunTimeSec / 60 + Resources.Message_Min);
		}
		else
		{
			AddLogLabel(Resources.Message_DeAirationFailedToStart);
			AddLogLabel(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void sharedProcedureCreatorComponent1_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		running = false;
		AddLogLabel(Resources.Message_DeAirationStopped);
		UpdateUI();
	}

	private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		UpdateUI();
	}

	private void digitalReadoutInstrumentParkBrake_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void digitalReadoutInstrumentCharging_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Expected O, but got Unknown
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		label41 = new System.Windows.Forms.Label();
		tableLayoutPanel1 = new TableLayoutPanel();
		runServicesButtonReturnControl = new RunServicesButton();
		label1 = new System.Windows.Forms.Label();
		checkmarkSpStatusLabel = new Checkmark();
		labelSpStatusLabel = new System.Windows.Forms.Label();
		progressBarProgress = new ProgressBar();
		labelProgress = new System.Windows.Forms.Label();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		buttonStart = new Button();
		buttonClose = new Button();
		tableLayoutPanelInstruments = new TableLayoutPanel();
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
		label2 = new System.Windows.Forms.Label();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		labelInterlockWarning = new System.Windows.Forms.Label();
		label39 = new System.Windows.Forms.Label();
		chartInstrument1 = new ChartInstrument();
		seekTimeListView = new SeekTimeListView();
		sharedProcedureCreatorComponent1 = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((ISupportInitialize)runServicesButtonReturnControl).BeginInit();
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(label41, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanel1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 1);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(label41, "label41");
		label41.Name = "label41";
		label41.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServicesButtonReturnControl, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmarkSpStatusLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelSpStatusLabel, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(progressBarProgress, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelProgress, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStart, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 4, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)runServicesButtonReturnControl, 2);
		componentResourceManager.ApplyResources(runServicesButtonReturnControl, "runServicesButtonReturnControl");
		((Control)(object)runServicesButtonReturnControl).Name = "runServicesButtonReturnControl";
		runServicesButtonReturnControl.ServiceCalls.Add(new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery1_Circuit_Res", (IEnumerable<string>)new string[2] { "WaterPump_Speed_Battery1_Circuit_Req=0", "WaterPump_Speed_Battery2_Circuit_Req=0" }));
		runServicesButtonReturnControl.ServiceCalls.Add(new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery2_Circuit_Res", (IEnumerable<string>)new string[2] { "WaterPump_Speed_Battery1_Circuit_Req=0", "WaterPump_Speed_Battery2_Circuit_Req=0" }));
		runServicesButtonReturnControl.ServiceCalls.Add(new ServiceCall("ECPC01T", "IOC_IOC_ETHM_Shutoff_ValveControl_Return_Control"));
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkSpStatusLabel, "checkmarkSpStatusLabel");
		((Control)(object)checkmarkSpStatusLabel).Name = "checkmarkSpStatusLabel";
		componentResourceManager.ApplyResources(labelSpStatusLabel, "labelSpStatusLabel");
		labelSpStatusLabel.Name = "labelSpStatusLabel";
		labelSpStatusLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(progressBarProgress, "progressBarProgress");
		progressBarProgress.Name = "progressBarProgress";
		componentResourceManager.ApplyResources(labelProgress, "labelProgress");
		labelProgress.Name = "labelProgress";
		labelProgress.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "ETHM_BatteryCircuitDeaerationCtrl" });
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanel4, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)chartInstrument1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)seekTimeListView, 1, 1);
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentCharging, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(label2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(labelInterlockWarning, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(label39, 0, 2);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetRowSpan((Control)(object)tableLayoutPanel4, 2);
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
		digitalReadoutInstrumentCharging.RepresentedStateChanged += digitalReadoutInstrumentCharging_RepresentedStateChanged;
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
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
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged;
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
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrumentParkBrake_RepresentedStateChanged;
		componentResourceManager.ApplyResources(labelInterlockWarning, "labelInterlockWarning");
		labelInterlockWarning.ForeColor = Color.Red;
		labelInterlockWarning.Name = "labelInterlockWarning";
		labelInterlockWarning.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label39, "label39");
		label39.Name = "label39";
		label39.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(chartInstrument1, "chartInstrument1");
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br1"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br2"));
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS236_BattCircValveActPos_BattCircValveActPos"));
		((Control)(object)chartInstrument1).Name = "chartInstrument1";
		chartInstrument1.SelectedTime = null;
		chartInstrument1.ShowButtonPanel = false;
		chartInstrument1.ShowEvents = false;
		chartInstrument1.ShowLabels = false;
		chartInstrument1.SpanOfView = TimeSpan.Parse("00:13:00");
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "De-AirationBattery";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		sharedProcedureCreatorComponent1.Suspend();
		sharedProcedureCreatorComponent1.AllowStopAlways = true;
		sharedProcedureCreatorComponent1.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Request_Results_ETHM_Battery_Circuit_Deaeration");
		sharedProcedureCreatorComponent1.MonitorGradient.Initialize((ValueState)3, 6);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(1, 0.0, (ValueState)3);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		sharedProcedureCreatorComponent1.MonitorGradient.Modify(6, 255.0, (ValueState)3);
		sharedProcedureCreatorComponent1.MonitorInterval = 5000;
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponent1, "sharedProcedureCreatorComponent1");
		sharedProcedureCreatorComponent1.Qualifier = "ETHM_BatteryCircuitDeaerationCtrl";
		sharedProcedureCreatorComponent1.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Start_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>)new string[1] { "Battery_Circuit_Deaeration_contro_start=1" });
		sharedProcedureCreatorComponent1.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Stop_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>)new string[1] { "Battery_Circuit_Deaeration_control_start=0" });
		sharedProcedureCreatorComponent1.StartServiceComplete += sharedProcedureCreatorComponent1_StartServiceComplete;
		sharedProcedureCreatorComponent1.StopServiceComplete += sharedProcedureCreatorComponent1_StopServiceComplete;
		sharedProcedureCreatorComponent1.MonitorServiceComplete += sharedProcedureCreatorComponent1_MonitorServiceComplete;
		sharedProcedureCreatorComponent1.Resume();
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelSpStatusLabel;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmarkSpStatusLabel;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = buttonStart;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_De-Aeration_Battery");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((ISupportInitialize)runServicesButtonReturnControl).EndInit();
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
