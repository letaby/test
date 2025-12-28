using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Advanced_6x4_Dyno_Routine.panel;

public class UserPanel : CustomPanel
{
	private Channel xmc02tChannel;

	private bool mdcUnderControl = false;

	private bool dogClutchUnderControl = false;

	private Timer statusTimer;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument7;

	private DigitalReadoutInstrument digitalReadoutInstrument8;

	private DigitalReadoutInstrument digitalReadoutInstrument9;

	private DigitalReadoutInstrument digitalReadoutInstrument12;

	private DigitalReadoutInstrument digitalReadoutInstrument11;

	private DigitalReadoutInstrument digitalReadoutInstrument13;

	private DigitalReadoutInstrument digitalReadoutInstrument14;

	private DigitalReadoutInstrument digitalReadoutInstrument15;

	private DigitalReadoutInstrument digitalReadoutInstrument16;

	private DigitalReadoutInstrument digitalReadoutInstrument17;

	private DigitalReadoutInstrument digitalReadoutInstrument18;

	private DigitalReadoutInstrument digitalReadoutInstrument19;

	private TableLayoutPanel tableLayoutPanel3;

	private TableLayoutPanel tableLayoutPanel4;

	private RunServiceButton runServiceButtonActivateMDC;

	private RunServiceButton runServiceButtonDeactivateMDC;

	private RunServiceButton runServiceButtonActivateDogClutch;

	private RunServiceButton runServiceButtonDeactivateDogClutch;

	private SeekTimeListView seekTimeListView1;

	private DigitalReadoutInstrument digitalReadoutInstrument10;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheckStatus;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private System.Windows.Forms.Label labelStatus;

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		UpdateUI();
	}

	public UserPanel()
	{
		InitializeComponent();
		statusTimer = new Timer();
		statusTimer.Interval = 2500;
		statusTimer.Tick += statusTimer_Tick;
	}

	private void statusTimer_Tick(object sender, EventArgs e)
	{
		labelStatus.Text = string.Empty;
		statusTimer.Stop();
	}

	private void AddLogLabel(string text)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
		}
	}

	private void AddLogError(string text)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
			labelStatus.Text = text;
			statusTimer.Start();
		}
	}

	public override void OnChannelsChanged()
	{
		SetXMC02TChannel("XMC02T");
	}

	private void UpdateUI()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		bool flag = digitalReadoutInstrumentVehicleCheckStatus != null && (int)digitalReadoutInstrumentVehicleCheckStatus.RepresentedState == 1 && xmc02tChannel != null && xmc02tChannel.Online;
		((Control)(object)runServiceButtonActivateMDC).Enabled = flag && !mdcUnderControl && !dogClutchUnderControl;
		((Control)(object)runServiceButtonDeactivateMDC).Enabled = mdcUnderControl;
		((Control)(object)runServiceButtonActivateDogClutch).Enabled = flag && !mdcUnderControl && !dogClutchUnderControl;
		((Control)(object)runServiceButtonDeactivateDogClutch).Enabled = dogClutchUnderControl;
	}

	private void SetXMC02TChannel(string ecuName)
	{
		if (xmc02tChannel != ((CustomPanel)this).GetChannel(ecuName))
		{
			xmc02tChannel = ((CustomPanel)this).GetChannel(ecuName);
			mdcUnderControl = false;
			dogClutchUnderControl = false;
		}
		UpdateUI();
	}

	private void runServiceButtonActivateMDC_Started(object sender, PassFailResultEventArgs e)
	{
		mdcUnderControl = ((ResultEventArgs)(object)e).Succeeded;
		dogClutchUnderControl = false;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel("MDC Activated");
		}
		else
		{
			AddLogError(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void runServiceButtonActivateDogClutch_Started(object sender, PassFailResultEventArgs e)
	{
		mdcUnderControl = false;
		dogClutchUnderControl = ((ResultEventArgs)(object)e).Succeeded;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel("Dog Clutch Activated");
		}
		else
		{
			AddLogError(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void digitalReadoutInstrumentVehicleCheckStatus_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void runServiceButtonDeactivateMDC_Started(object sender, PassFailResultEventArgs e)
	{
		mdcUnderControl = false;
		dogClutchUnderControl = false;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel("MDC Deactivated");
		}
		else
		{
			AddLogError(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void runServiceButtonDeactivateDogClutch_Started(object sender, PassFailResultEventArgs e)
	{
		mdcUnderControl = false;
		dogClutchUnderControl = false;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel("Dog Clutch Dactivated");
		}
		else
		{
			AddLogError(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
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
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0948: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc1: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		labelStatus = new System.Windows.Forms.Label();
		tableLayoutPanel4 = new TableLayoutPanel();
		runServiceButtonActivateMDC = new RunServiceButton();
		runServiceButtonDeactivateMDC = new RunServiceButton();
		digitalReadoutInstrument12 = new DigitalReadoutInstrument();
		digitalReadoutInstrument11 = new DigitalReadoutInstrument();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		digitalReadoutInstrument9 = new DigitalReadoutInstrument();
		digitalReadoutInstrument10 = new DigitalReadoutInstrument();
		digitalReadoutInstrument13 = new DigitalReadoutInstrument();
		digitalReadoutInstrument14 = new DigitalReadoutInstrument();
		digitalReadoutInstrument15 = new DigitalReadoutInstrument();
		digitalReadoutInstrument16 = new DigitalReadoutInstrument();
		digitalReadoutInstrument17 = new DigitalReadoutInstrument();
		digitalReadoutInstrument18 = new DigitalReadoutInstrument();
		digitalReadoutInstrument19 = new DigitalReadoutInstrument();
		tableLayoutPanel3 = new TableLayoutPanel();
		runServiceButtonActivateDogClutch = new RunServiceButton();
		runServiceButtonDeactivateDogClutch = new RunServiceButton();
		seekTimeListView1 = new SeekTimeListView();
		digitalReadoutInstrumentVehicleCheckStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel4, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument12, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument11, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument7, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument8, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument9, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument10, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument13, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument14, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument15, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument16, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument17, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument18, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument19, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleCheckStatus, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 5);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelStatus, 4);
		labelStatus.ForeColor = Color.Red;
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)runServiceButtonActivateMDC, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)runServiceButtonDeactivateMDC, 0, 1);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		componentResourceManager.ApplyResources(runServiceButtonActivateMDC, "runServiceButtonActivateMDC");
		((Control)(object)runServiceButtonActivateMDC).Name = "runServiceButtonActivateMDC";
		runServiceButtonActivateMDC.ServiceCall = new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Control", (IEnumerable<string>)new string[4] { "DiagRqData_OC_MDC_Cmd_Enbl=1", "DiagRqData_OC_MDC_Cmd=1", "DiagRqData_OC_DgCltch_Cmd_Enbl=0", "DiagRqData_OC_DgCltch_Cmd=0" });
		((RunSharedProcedureButtonBase)runServiceButtonActivateMDC).Started += runServiceButtonActivateMDC_Started;
		componentResourceManager.ApplyResources(runServiceButtonDeactivateMDC, "runServiceButtonDeactivateMDC");
		((Control)(object)runServiceButtonDeactivateMDC).Name = "runServiceButtonDeactivateMDC";
		runServiceButtonDeactivateMDC.ServiceCall = new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Return_Control");
		((RunSharedProcedureButtonBase)runServiceButtonDeactivateMDC).Started += runServiceButtonDeactivateMDC_Started;
		componentResourceManager.ApplyResources(digitalReadoutInstrument12, "digitalReadoutInstrument12");
		digitalReadoutInstrument12.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument12).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument12).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_Wheelspeed_Wheel_4_Read_Wheelspeed_4");
		((Control)(object)digitalReadoutInstrument12).Name = "digitalReadoutInstrument12";
		((SingleInstrumentBase)digitalReadoutInstrument12).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument11, "digitalReadoutInstrument11");
		digitalReadoutInstrument11.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument11).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument11).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_Wheelspeed_Wheel_3_Read_Wheelspeed_3");
		((Control)(object)digitalReadoutInstrument11).Name = "digitalReadoutInstrument11";
		((SingleInstrumentBase)digitalReadoutInstrument11).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument9, "digitalReadoutInstrument9");
		digitalReadoutInstrument9.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument9).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_Wheelspeed_Wheel_1_Read_Wheelspeed_1");
		((Control)(object)digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
		((SingleInstrumentBase)digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument10, "digitalReadoutInstrument10");
		digitalReadoutInstrument10.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument10).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_Wheelspeed_Wheel_2_Read_Wheelspeed_2");
		((Control)(object)digitalReadoutInstrument10).Name = "digitalReadoutInstrument10";
		((SingleInstrumentBase)digitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument13, "digitalReadoutInstrument13");
		digitalReadoutInstrument13.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument13).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument13).Instrument = new Qualifier((QualifierTypes)1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Sol_Stat");
		((Control)(object)digitalReadoutInstrument13).Name = "digitalReadoutInstrument13";
		((SingleInstrumentBase)digitalReadoutInstrument13).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument14, "digitalReadoutInstrument14");
		digitalReadoutInstrument14.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument14).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument14).Instrument = new Qualifier((QualifierTypes)1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Open_Fb");
		((Control)(object)digitalReadoutInstrument14).Name = "digitalReadoutInstrument14";
		((SingleInstrumentBase)digitalReadoutInstrument14).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument15, "digitalReadoutInstrument15");
		digitalReadoutInstrument15.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument15).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument15).Instrument = new Qualifier((QualifierTypes)1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Closed_Fb");
		((Control)(object)digitalReadoutInstrument15).Name = "digitalReadoutInstrument15";
		((SingleInstrumentBase)digitalReadoutInstrument15).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument16, "digitalReadoutInstrument16");
		digitalReadoutInstrument16.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument16).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument16).Instrument = new Qualifier((QualifierTypes)1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_DogCltch_Sol_Stat");
		((Control)(object)digitalReadoutInstrument16).Name = "digitalReadoutInstrument16";
		((SingleInstrumentBase)digitalReadoutInstrument16).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument17, "digitalReadoutInstrument17");
		digitalReadoutInstrument17.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument17).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument17).Instrument = new Qualifier((QualifierTypes)1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_RA2_Lt_Clutch_Fb");
		((Control)(object)digitalReadoutInstrument17).Name = "digitalReadoutInstrument17";
		((SingleInstrumentBase)digitalReadoutInstrument17).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument18, "digitalReadoutInstrument18");
		digitalReadoutInstrument18.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument18).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument18).Instrument = new Qualifier((QualifierTypes)1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_RA2_Rt_Clutch_Fb");
		((Control)(object)digitalReadoutInstrument18).Name = "digitalReadoutInstrument18";
		((SingleInstrumentBase)digitalReadoutInstrument18).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument19, "digitalReadoutInstrument19");
		digitalReadoutInstrument19.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument19).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument19).Instrument = new Qualifier((QualifierTypes)1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_AxlCurrState_Cval");
		((Control)(object)digitalReadoutInstrument19).Name = "digitalReadoutInstrument19";
		((SingleInstrumentBase)digitalReadoutInstrument19).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)runServiceButtonActivateDogClutch, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)runServiceButtonDeactivateDogClutch, 0, 1);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(runServiceButtonActivateDogClutch, "runServiceButtonActivateDogClutch");
		((Control)(object)runServiceButtonActivateDogClutch).Name = "runServiceButtonActivateDogClutch";
		runServiceButtonActivateDogClutch.ServiceCall = new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Control", (IEnumerable<string>)new string[4] { "DiagRqData_OC_MDC_Cmd_Enbl=0", "DiagRqData_OC_MDC_Cmd=0", "DiagRqData_OC_DgCltch_Cmd_Enbl=1", "DiagRqData_OC_DgCltch_Cmd=1" });
		((RunSharedProcedureButtonBase)runServiceButtonActivateDogClutch).Started += runServiceButtonActivateDogClutch_Started;
		componentResourceManager.ApplyResources(runServiceButtonDeactivateDogClutch, "runServiceButtonDeactivateDogClutch");
		((Control)(object)runServiceButtonDeactivateDogClutch).Name = "runServiceButtonDeactivateDogClutch";
		runServiceButtonDeactivateDogClutch.ServiceCall = new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Return_Control");
		((RunSharedProcedureButtonBase)runServiceButtonDeactivateDogClutch).Started += runServiceButtonDeactivateDogClutch_Started;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 2);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "Advanced6x4DynoRoutine";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView1, 3);
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleCheckStatus, "digitalReadoutInstrumentVehicleCheckStatus");
		digitalReadoutInstrumentVehicleCheckStatus.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).FreezeValue = false;
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutInstrumentVehicleCheckStatus).Name = "digitalReadoutInstrumentVehicleCheckStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleCheckStatus.RepresentedStateChanged += digitalReadoutInstrumentVehicleCheckStatus_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_Wheelspeed_Wheel_5_Read_Wheelspeed_5");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "Advanced_6x4_Dyno_Routine";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_Wheelspeed_Wheel_6_Read_Wheelspeed_6");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
