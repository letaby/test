// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Advanced_6x4_Dyno_Routine.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
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

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.UpdateUI();
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.statusTimer = new Timer();
    this.statusTimer.Interval = 2500;
    this.statusTimer.Tick += new EventHandler(this.statusTimer_Tick);
  }

  private void statusTimer_Tick(object sender, EventArgs e)
  {
    this.labelStatus.Text = string.Empty;
    this.statusTimer.Stop();
  }

  private void AddLogLabel(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void AddLogError(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
    this.labelStatus.Text = text;
    this.statusTimer.Start();
  }

  public virtual void OnChannelsChanged() => this.SetXMC02TChannel("XMC02T");

  private void UpdateUI()
  {
    bool flag = this.digitalReadoutInstrumentVehicleCheckStatus != null && this.digitalReadoutInstrumentVehicleCheckStatus.RepresentedState == 1 && this.xmc02tChannel != null && this.xmc02tChannel.Online;
    ((Control) this.runServiceButtonActivateMDC).Enabled = flag && !this.mdcUnderControl && !this.dogClutchUnderControl;
    ((Control) this.runServiceButtonDeactivateMDC).Enabled = this.mdcUnderControl;
    ((Control) this.runServiceButtonActivateDogClutch).Enabled = flag && !this.mdcUnderControl && !this.dogClutchUnderControl;
    ((Control) this.runServiceButtonDeactivateDogClutch).Enabled = this.dogClutchUnderControl;
  }

  private void SetXMC02TChannel(string ecuName)
  {
    if (this.xmc02tChannel != this.GetChannel(ecuName))
    {
      this.xmc02tChannel = this.GetChannel(ecuName);
      this.mdcUnderControl = false;
      this.dogClutchUnderControl = false;
    }
    this.UpdateUI();
  }

  private void runServiceButtonActivateMDC_Started(object sender, PassFailResultEventArgs e)
  {
    this.mdcUnderControl = ((ResultEventArgs) e).Succeeded;
    this.dogClutchUnderControl = false;
    if (((ResultEventArgs) e).Succeeded)
      this.AddLogLabel("MDC Activated");
    else
      this.AddLogError(((ResultEventArgs) e).Exception.Message);
    this.UpdateUI();
  }

  private void runServiceButtonActivateDogClutch_Started(object sender, PassFailResultEventArgs e)
  {
    this.mdcUnderControl = false;
    this.dogClutchUnderControl = ((ResultEventArgs) e).Succeeded;
    if (((ResultEventArgs) e).Succeeded)
      this.AddLogLabel("Dog Clutch Activated");
    else
      this.AddLogError(((ResultEventArgs) e).Exception.Message);
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentVehicleCheckStatus_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUI();
  }

  private void runServiceButtonDeactivateMDC_Started(object sender, PassFailResultEventArgs e)
  {
    this.mdcUnderControl = false;
    this.dogClutchUnderControl = false;
    if (((ResultEventArgs) e).Succeeded)
      this.AddLogLabel("MDC Deactivated");
    else
      this.AddLogError(((ResultEventArgs) e).Exception.Message);
    this.UpdateUI();
  }

  private void runServiceButtonDeactivateDogClutch_Started(object sender, PassFailResultEventArgs e)
  {
    this.mdcUnderControl = false;
    this.dogClutchUnderControl = false;
    if (((ResultEventArgs) e).Succeeded)
      this.AddLogLabel("Dog Clutch Dactivated");
    else
      this.AddLogError(((ResultEventArgs) e).Exception.Message);
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.labelStatus = new System.Windows.Forms.Label();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.runServiceButtonActivateMDC = new RunServiceButton();
    this.runServiceButtonDeactivateMDC = new RunServiceButton();
    this.digitalReadoutInstrument12 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument11 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument8 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument9 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument10 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument13 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument14 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument15 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument16 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument17 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument18 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument19 = new DigitalReadoutInstrument();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.runServiceButtonActivateDogClutch = new RunServiceButton();
    this.runServiceButtonDeactivateDogClutch = new RunServiceButton();
    this.seekTimeListView1 = new SeekTimeListView();
    this.digitalReadoutInstrumentVehicleCheckStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel4, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument12, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument11, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument7, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument8, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument9, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument10, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument13, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument14, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument15, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument16, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument17, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument18, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument19, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentVehicleCheckStatus, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 5);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelStatus, 4);
    this.labelStatus.ForeColor = Color.Red;
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.runServiceButtonActivateMDC, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.runServiceButtonDeactivateMDC, 0, 1);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    componentResourceManager.ApplyResources((object) this.runServiceButtonActivateMDC, "runServiceButtonActivateMDC");
    ((Control) this.runServiceButtonActivateMDC).Name = "runServiceButtonActivateMDC";
    this.runServiceButtonActivateMDC.ServiceCall = new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Control", (IEnumerable<string>) new string[4]
    {
      "DiagRqData_OC_MDC_Cmd_Enbl=1",
      "DiagRqData_OC_MDC_Cmd=1",
      "DiagRqData_OC_DgCltch_Cmd_Enbl=0",
      "DiagRqData_OC_DgCltch_Cmd=0"
    });
    ((RunSharedProcedureButtonBase) this.runServiceButtonActivateMDC).Started += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonActivateMDC_Started);
    componentResourceManager.ApplyResources((object) this.runServiceButtonDeactivateMDC, "runServiceButtonDeactivateMDC");
    ((Control) this.runServiceButtonDeactivateMDC).Name = "runServiceButtonDeactivateMDC";
    this.runServiceButtonDeactivateMDC.ServiceCall = new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Return_Control");
    ((RunSharedProcedureButtonBase) this.runServiceButtonDeactivateMDC).Started += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonDeactivateMDC_Started);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument12, "digitalReadoutInstrument12");
    this.digitalReadoutInstrument12.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_4_Read_Wheelspeed_4");
    ((Control) this.digitalReadoutInstrument12).Name = "digitalReadoutInstrument12";
    ((SingleInstrumentBase) this.digitalReadoutInstrument12).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument11, "digitalReadoutInstrument11");
    this.digitalReadoutInstrument11.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_3_Read_Wheelspeed_3");
    ((Control) this.digitalReadoutInstrument11).Name = "digitalReadoutInstrument11";
    ((SingleInstrumentBase) this.digitalReadoutInstrument11).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument7, "digitalReadoutInstrument7");
    this.digitalReadoutInstrument7.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition");
    ((Control) this.digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument8, "digitalReadoutInstrument8");
    this.digitalReadoutInstrument8.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument9, "digitalReadoutInstrument9");
    this.digitalReadoutInstrument9.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_1_Read_Wheelspeed_1");
    ((Control) this.digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument10, "digitalReadoutInstrument10");
    this.digitalReadoutInstrument10.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument10).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument10).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_2_Read_Wheelspeed_2");
    ((Control) this.digitalReadoutInstrument10).Name = "digitalReadoutInstrument10";
    ((SingleInstrumentBase) this.digitalReadoutInstrument10).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument13, "digitalReadoutInstrument13");
    this.digitalReadoutInstrument13.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument13).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument13).Instrument = new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Sol_Stat");
    ((Control) this.digitalReadoutInstrument13).Name = "digitalReadoutInstrument13";
    ((SingleInstrumentBase) this.digitalReadoutInstrument13).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument14, "digitalReadoutInstrument14");
    this.digitalReadoutInstrument14.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument14).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument14).Instrument = new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Open_Fb");
    ((Control) this.digitalReadoutInstrument14).Name = "digitalReadoutInstrument14";
    ((SingleInstrumentBase) this.digitalReadoutInstrument14).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument15, "digitalReadoutInstrument15");
    this.digitalReadoutInstrument15.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument15).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument15).Instrument = new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Closed_Fb");
    ((Control) this.digitalReadoutInstrument15).Name = "digitalReadoutInstrument15";
    ((SingleInstrumentBase) this.digitalReadoutInstrument15).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument16, "digitalReadoutInstrument16");
    this.digitalReadoutInstrument16.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument16).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument16).Instrument = new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_DogCltch_Sol_Stat");
    ((Control) this.digitalReadoutInstrument16).Name = "digitalReadoutInstrument16";
    ((SingleInstrumentBase) this.digitalReadoutInstrument16).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument17, "digitalReadoutInstrument17");
    this.digitalReadoutInstrument17.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument17).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument17).Instrument = new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_RA2_Lt_Clutch_Fb");
    ((Control) this.digitalReadoutInstrument17).Name = "digitalReadoutInstrument17";
    ((SingleInstrumentBase) this.digitalReadoutInstrument17).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument18, "digitalReadoutInstrument18");
    this.digitalReadoutInstrument18.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument18).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument18).Instrument = new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_RA2_Rt_Clutch_Fb");
    ((Control) this.digitalReadoutInstrument18).Name = "digitalReadoutInstrument18";
    ((SingleInstrumentBase) this.digitalReadoutInstrument18).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument19, "digitalReadoutInstrument19");
    this.digitalReadoutInstrument19.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument19).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument19).Instrument = new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_AxlCurrState_Cval");
    ((Control) this.digitalReadoutInstrument19).Name = "digitalReadoutInstrument19";
    ((SingleInstrumentBase) this.digitalReadoutInstrument19).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.runServiceButtonActivateDogClutch, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.runServiceButtonDeactivateDogClutch, 0, 1);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.runServiceButtonActivateDogClutch, "runServiceButtonActivateDogClutch");
    ((Control) this.runServiceButtonActivateDogClutch).Name = "runServiceButtonActivateDogClutch";
    this.runServiceButtonActivateDogClutch.ServiceCall = new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Control", (IEnumerable<string>) new string[4]
    {
      "DiagRqData_OC_MDC_Cmd_Enbl=0",
      "DiagRqData_OC_MDC_Cmd=0",
      "DiagRqData_OC_DgCltch_Cmd_Enbl=1",
      "DiagRqData_OC_DgCltch_Cmd=1"
    });
    ((RunSharedProcedureButtonBase) this.runServiceButtonActivateDogClutch).Started += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonActivateDogClutch_Started);
    componentResourceManager.ApplyResources((object) this.runServiceButtonDeactivateDogClutch, "runServiceButtonDeactivateDogClutch");
    ((Control) this.runServiceButtonDeactivateDogClutch).Name = "runServiceButtonDeactivateDogClutch";
    this.runServiceButtonDeactivateDogClutch.ServiceCall = new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Return_Control");
    ((RunSharedProcedureButtonBase) this.runServiceButtonDeactivateDogClutch).Started += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonDeactivateDogClutch_Started);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "Advanced6x4DynoRoutine";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView1, 3);
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleCheckStatus, "digitalReadoutInstrumentVehicleCheckStatus");
    this.digitalReadoutInstrumentVehicleCheckStatus.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutInstrumentVehicleCheckStatus).Name = "digitalReadoutInstrumentVehicleCheckStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleCheckStatus.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentVehicleCheckStatus_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_5_Read_Wheelspeed_5");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "Advanced_6x4_Dyno_Routine";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_6_Read_Wheelspeed_6");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
