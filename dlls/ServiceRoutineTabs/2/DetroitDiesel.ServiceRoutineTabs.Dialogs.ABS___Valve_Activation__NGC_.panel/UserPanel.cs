using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ABS___Valve_Activation__NGC_.panel;

public class UserPanel : CustomPanel
{
	private const string ChannelName = "ABS02T";

	private Channel abs;

	private Timer timer;

	private TableLayoutPanel tableLayoutPanelMain;

	private TableLayoutPanel tableLayoutPanelInterlocks;

	private System.Windows.Forms.Label labelInterlockWarning;

	private System.Windows.Forms.Label labelOr;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private TableLayoutPanel tableLayoutPanelControls;

	private RunServicesButton runServicesButtonHbpvF;

	private RunServicesButton runServicesButtonHbpvE;

	private RunServicesButton runServicesButtonHbpvD;

	private RunServicesButton runServicesButtonHbpvC;

	private RunServicesButton runServicesButtonHoldTrailer;

	private RunServicesButton runServicesButtonHbpvB;

	private RunServicesButton runServicesButtonHbpvA;

	private RunServicesButton runServicesButtonAvsvA;

	private RunServicesButton runServicesButtonAvsvB;

	private TableLayoutPanel tableLayoutPanelRightColumn;

	private System.Windows.Forms.Label labelStatus;

	private System.Windows.Forms.Label labelMonitors;

	private System.Windows.Forms.Label labelControls;

	private System.Windows.Forms.Label labelInterlock;

	private TableLayoutPanel tableLayoutPanelHeading;

	private System.Windows.Forms.Label labelTitle;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private Button buttonClose;

	private SelectablePanel selectablePanel;

	public UserPanel()
	{
		InitializeComponent();
		timer = new Timer();
		timer.Interval = 2500;
		timer.Tick += timer_Tick;
		UpdatePreconditionState();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += ParentForm_FormClosing;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrumentIoInterlock_RepresentedStateChanged;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrumentIoInterlock_RepresentedStateChanged;
		((UserControl)this).OnLoad(e);
	}

	private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= ParentForm_FormClosing;
			digitalReadoutInstrumentParkBrake.RepresentedStateChanged -= digitalReadoutInstrumentIoInterlock_RepresentedStateChanged;
			digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged -= digitalReadoutInstrumentIoInterlock_RepresentedStateChanged;
		}
	}

	public override void OnChannelsChanged()
	{
		SetAbsChannel("ABS02T");
	}

	private void SetAbsChannel(string ecuName)
	{
		if (abs != null)
		{
			abs.Services.ServiceCompleteEvent -= Services_ServiceCompleteEvent;
		}
		abs = ((CustomPanel)this).GetChannel(ecuName);
		if (abs != null)
		{
			abs.Services.ServiceCompleteEvent += Services_ServiceCompleteEvent;
		}
	}

	private void Services_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			ResetError();
			return;
		}
		labelStatus.Text = ((e.Exception != null) ? e.Exception.Message : Resources.ErrorUnknown);
		timer.Start();
	}

	private void ResetError()
	{
		timer.Stop();
		labelStatus.Text = string.Empty;
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		ResetError();
	}

	private IEnumerable<Control> GetAllControls(Control source)
	{
		yield return source;
		foreach (Control item in source.Controls.OfType<Control>().SelectMany((Control c) => GetAllControls(c)))
		{
			yield return item;
		}
	}

	private void UpdatePreconditionState()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		bool flag = (int)digitalReadoutInstrumentParkBrake.RepresentedState == 1 || (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1;
		bool flag2 = abs != null && abs.Online;
		foreach (RunServicesButton item in GetAllControls((Control)(object)this).OfType<RunServicesButton>())
		{
			((Control)(object)item).Enabled = flag && flag2;
		}
		labelInterlockWarning.Visible = !flag;
	}

	private void digitalReadoutInstrumentIoInterlock_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdatePreconditionState();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
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
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_0989: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		labelStatus = new System.Windows.Forms.Label();
		tableLayoutPanelInterlocks = new TableLayoutPanel();
		labelInterlock = new System.Windows.Forms.Label();
		labelInterlockWarning = new System.Windows.Forms.Label();
		labelOr = new System.Windows.Forms.Label();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		tableLayoutPanelControls = new TableLayoutPanel();
		labelControls = new System.Windows.Forms.Label();
		runServicesButtonHbpvF = new RunServicesButton();
		runServicesButtonHbpvE = new RunServicesButton();
		runServicesButtonHbpvD = new RunServicesButton();
		runServicesButtonHbpvC = new RunServicesButton();
		runServicesButtonHoldTrailer = new RunServicesButton();
		runServicesButtonHbpvB = new RunServicesButton();
		runServicesButtonHbpvA = new RunServicesButton();
		runServicesButtonAvsvA = new RunServicesButton();
		runServicesButtonAvsvB = new RunServicesButton();
		tableLayoutPanelHeading = new TableLayoutPanel();
		labelTitle = new System.Windows.Forms.Label();
		tableLayoutPanelRightColumn = new TableLayoutPanel();
		labelMonitors = new System.Windows.Forms.Label();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		buttonClose = new Button();
		selectablePanel = new SelectablePanel();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelInterlocks).SuspendLayout();
		((Control)(object)tableLayoutPanelControls).SuspendLayout();
		((ISupportInitialize)runServicesButtonHbpvF).BeginInit();
		((ISupportInitialize)runServicesButtonHbpvE).BeginInit();
		((ISupportInitialize)runServicesButtonHbpvD).BeginInit();
		((ISupportInitialize)runServicesButtonHbpvC).BeginInit();
		((ISupportInitialize)runServicesButtonHoldTrailer).BeginInit();
		((ISupportInitialize)runServicesButtonHbpvB).BeginInit();
		((ISupportInitialize)runServicesButtonHbpvA).BeginInit();
		((ISupportInitialize)runServicesButtonAvsvA).BeginInit();
		((ISupportInitialize)runServicesButtonAvsvB).BeginInit();
		((Control)(object)tableLayoutPanelHeading).SuspendLayout();
		((Control)(object)tableLayoutPanelRightColumn).SuspendLayout();
		((Control)(object)selectablePanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelStatus, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelInterlocks, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelControls, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelHeading, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelRightColumn, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(buttonClose, 2, 2);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)labelStatus, 2);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.ForeColor = Color.Red;
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelInterlocks, "tableLayoutPanelInterlocks");
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add(labelInterlock, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add(labelInterlockWarning, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add(labelOr, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 3);
		((Control)(object)tableLayoutPanelInterlocks).Name = "tableLayoutPanelInterlocks";
		componentResourceManager.ApplyResources(labelInterlock, "labelInterlock");
		labelInterlock.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelInterlocks).SetColumnSpan((Control)labelInterlock, 2);
		labelInterlock.Name = "labelInterlock";
		labelInterlock.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelInterlockWarning, "labelInterlockWarning");
		labelInterlockWarning.ForeColor = Color.Red;
		labelInterlockWarning.Name = "labelInterlockWarning";
		labelInterlockWarning.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelOr, "labelOr");
		labelOr.Name = "labelOr";
		labelOr.UseCompatibleTextRendering = true;
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
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)1, 2, "mph");
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 5.0, (ValueState)1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 6.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "J1939-0", "DT_84");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelControls, "tableLayoutPanelControls");
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(labelControls, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHbpvF, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHbpvE, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHbpvD, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHbpvC, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHoldTrailer, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHbpvB, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonHbpvA, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonAvsvA, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)runServicesButtonAvsvB, 0, 2);
		((Control)(object)tableLayoutPanelControls).Name = "tableLayoutPanelControls";
		componentResourceManager.ApplyResources(labelControls, "labelControls");
		labelControls.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelControls).SetColumnSpan((Control)labelControls, 2);
		labelControls.Name = "labelControls";
		labelControls.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServicesButtonHbpvF, "runServicesButtonHbpvF");
		((Control)(object)runServicesButtonHbpvF).Name = "runServicesButtonHbpvF";
		runServicesButtonHbpvF.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_F_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonHbpvE, "runServicesButtonHbpvE");
		((Control)(object)runServicesButtonHbpvE).Name = "runServicesButtonHbpvE";
		runServicesButtonHbpvE.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_E_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonHbpvD, "runServicesButtonHbpvD");
		((Control)(object)runServicesButtonHbpvD).Name = "runServicesButtonHbpvD";
		runServicesButtonHbpvD.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_D_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonHbpvC, "runServicesButtonHbpvC");
		((Control)(object)runServicesButtonHbpvC).Name = "runServicesButtonHbpvC";
		runServicesButtonHbpvC.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_C_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonHoldTrailer, "runServicesButtonHoldTrailer");
		((Control)(object)runServicesButtonHoldTrailer).Name = "runServicesButtonHoldTrailer";
		runServicesButtonHoldTrailer.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Trailer_Control_Pressure_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonHbpvB, "runServicesButtonHbpvB");
		((Control)(object)runServicesButtonHbpvB).Name = "runServicesButtonHbpvB";
		runServicesButtonHbpvB.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_B_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonHbpvA, "runServicesButtonHbpvA");
		((Control)(object)runServicesButtonHbpvA).Name = "runServicesButtonHbpvA";
		runServicesButtonHbpvA.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_A_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonAvsvA, "runServicesButtonAvsvA");
		((Control)(object)runServicesButtonAvsvA).Name = "runServicesButtonAvsvA";
		runServicesButtonAvsvA.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_3_2_Solenoid_valve_A_actuate_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(runServicesButtonAvsvB, "runServicesButtonAvsvB");
		((Control)(object)runServicesButtonAvsvB).Name = "runServicesButtonAvsvB";
		runServicesButtonAvsvB.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_3_2_Solenoid_valve_B_actuate_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }));
		componentResourceManager.ApplyResources(tableLayoutPanelHeading, "tableLayoutPanelHeading");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelHeading, 3);
		((TableLayoutPanel)(object)tableLayoutPanelHeading).Controls.Add(labelTitle, 0, 0);
		((Control)(object)tableLayoutPanelHeading).Name = "tableLayoutPanelHeading";
		componentResourceManager.ApplyResources(labelTitle, "labelTitle");
		labelTitle.Name = "labelTitle";
		labelTitle.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelRightColumn, "tableLayoutPanelRightColumn");
		((TableLayoutPanel)(object)tableLayoutPanelRightColumn).Controls.Add(labelMonitors, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelRightColumn).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelRightColumn).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 2);
		((Control)(object)tableLayoutPanelRightColumn).Name = "tableLayoutPanelRightColumn";
		componentResourceManager.ApplyResources(labelMonitors, "labelMonitors");
		labelMonitors.BorderStyle = BorderStyle.FixedSingle;
		labelMonitors.Name = "labelMonitors";
		labelMonitors.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "IoMonitor";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_PressCrcut1_Stat_EAPU");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "IoMonitor";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_PressCrcut2_Stat_EAPU");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(selectablePanel, "selectablePanel");
		((Control)(object)selectablePanel).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)(object)selectablePanel).Name = "selectablePanel";
		((Panel)(object)selectablePanel).TabStop = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Vehicle_ABSActivationValve");
		((Control)this).Controls.Add((Control)(object)selectablePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanelInterlocks).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInterlocks).PerformLayout();
		((Control)(object)tableLayoutPanelControls).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelControls).PerformLayout();
		((ISupportInitialize)runServicesButtonHbpvF).EndInit();
		((ISupportInitialize)runServicesButtonHbpvE).EndInit();
		((ISupportInitialize)runServicesButtonHbpvD).EndInit();
		((ISupportInitialize)runServicesButtonHbpvC).EndInit();
		((ISupportInitialize)runServicesButtonHoldTrailer).EndInit();
		((ISupportInitialize)runServicesButtonHbpvB).EndInit();
		((ISupportInitialize)runServicesButtonHbpvA).EndInit();
		((ISupportInitialize)runServicesButtonAvsvA).EndInit();
		((ISupportInitialize)runServicesButtonAvsvB).EndInit();
		((Control)(object)tableLayoutPanelHeading).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelHeading).PerformLayout();
		((Control)(object)tableLayoutPanelRightColumn).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelRightColumn).PerformLayout();
		((Control)(object)selectablePanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
