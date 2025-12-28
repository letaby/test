// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.DCB_Unlock.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DCB_Unlock.panel;

public class UserPanel : CustomPanel
{
  private Channel dcb01t;
  private Channel dcb02t;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrumentCharging;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransmission;
  private System.Windows.Forms.Label label1;
  private DigitalReadoutInstrument digitalReadoutInstrumentDCB01T;
  private DigitalReadoutInstrument digitalReadoutInstrumentDCB02T;
  private SeekTimeListView seekTimeListView;
  private RunServiceButton runServiceButtonDCB01T;
  private RunServiceButton runServiceButtonDCB02T;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

  public UserPanel() => this.InitializeComponent();

  private void AddLabelLog(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private bool VehicleCharging => this.digitalReadoutInstrumentCharging.RepresentedState != 1;

  private bool VehicleCheckOk
  {
    get
    {
      return this.digitalReadoutInstrumentParkBrake.RepresentedState != 3 && this.digitalReadoutInstrumentTransmission.RepresentedState == 1 && this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && !this.VehicleCharging;
    }
  }

  public virtual void OnChannelsChanged()
  {
    this.dcb01t = this.GetChannel("DCB01T");
    this.dcb02t = this.GetChannel("DCB02T");
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    ((Control) this.digitalReadoutInstrumentDCB02T).Visible = ((Control) this.runServiceButtonDCB02T).Visible = this.dcb02t != null;
    ((Control) this.runServiceButtonDCB02T).Enabled = this.dcb02t != null && this.VehicleCheckOk;
    ((Control) this.runServiceButtonDCB01T).Enabled = this.VehicleCheckOk;
  }

  private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void runServices_Complete(string dcbName, SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
      this.AddLabelLog(string.Format(Resources.MessageFormat_0UnlockComplete, (object) dcbName));
    else
      this.AddLabelLog(string.Format(Resources.MessageFormat_0UnlockFailed1, (object) dcbName, ((ResultEventArgs) e).Exception == null ? (object) string.Empty : (object) (Resources.Message_Error + ((ResultEventArgs) e).Exception.Message)));
  }

  private void runServiceButtonDCB01T_ServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    this.runServices_Complete("DCB01T", e);
  }

  private void runServiceButtonDCB02T_ServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    this.runServices_Complete("DCB02T", e);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.runServiceButtonDCB02T = new RunServiceButton();
    this.seekTimeListView = new SeekTimeListView();
    this.digitalReadoutInstrumentDCB01T = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDCB02T = new DigitalReadoutInstrument();
    this.label1 = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransmission = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.runServiceButtonDCB01T = new RunServiceButton();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonDCB02T, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentDCB01T, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentDCB02T, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentCharging, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransmission, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonDCB01T, 2, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.runServiceButtonDCB02T, "runServiceButtonDCB02T");
    ((Control) this.runServiceButtonDCB02T).Name = "runServiceButtonDCB02T";
    this.runServiceButtonDCB02T.ServiceCall = new ServiceCall("DCB02T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "HV_Lock_Status=0"
    });
    this.runServiceButtonDCB02T.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonDCB02T_ServiceComplete);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "DCB Unlock";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView, 2);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDCB01T, "digitalReadoutInstrumentDCB01T");
    this.digitalReadoutInstrumentDCB01T.FontGroup = "DCB Unlock Text";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCB01T).FreezeValue = false;
    this.digitalReadoutInstrumentDCB01T.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentDCB01T.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentDCB01T.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCB01T).Instrument = new Qualifier((QualifierTypes) 8, "DCB01T", "DT_STO_High_Voltage_Lock_HV_Lock_Status");
    ((Control) this.digitalReadoutInstrumentDCB01T).Name = "digitalReadoutInstrumentDCB01T";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCB01T).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCB01T).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDCB02T, "digitalReadoutInstrumentDCB02T");
    this.digitalReadoutInstrumentDCB02T.FontGroup = "DCB Unlock Text";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCB02T).FreezeValue = false;
    this.digitalReadoutInstrumentDCB02T.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentDCB02T.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentDCB02T.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCB02T).Instrument = new Qualifier((QualifierTypes) 8, "DCB02T", "DT_STO_High_Voltage_Lock_HV_Lock_Status");
    ((Control) this.digitalReadoutInstrumentDCB02T).Name = "digitalReadoutInstrumentDCB02T";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCB02T).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCB02T).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label1, 3);
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCharging, "digitalReadoutInstrumentCharging");
    this.digitalReadoutInstrumentCharging.FontGroup = "DCB Unlock Text";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).FreezeValue = false;
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentCharging.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentCharging.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentCharging.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition");
    ((Control) this.digitalReadoutInstrumentCharging).Name = "digitalReadoutInstrumentCharging";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentCharging.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = "DCB Unlock Text";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 2);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransmission, "digitalReadoutInstrumentTransmission");
    this.digitalReadoutInstrumentTransmission.FontGroup = "DCB Unlock Text";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmission).FreezeValue = false;
    this.digitalReadoutInstrumentTransmission.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentTransmission.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmission.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmission).Instrument = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
    ((Control) this.digitalReadoutInstrumentTransmission).Name = "digitalReadoutInstrumentTransmission";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmission).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentTransmission.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = "DCB Unlock Text";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.runServiceButtonDCB01T, "runServiceButtonDCB01T");
    ((Control) this.runServiceButtonDCB01T).Name = "runServiceButtonDCB01T";
    this.runServiceButtonDCB01T.ServiceCall = new ServiceCall("DCB01T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "HV_Lock_Status=0"
    });
    this.runServiceButtonDCB01T.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonDCB01T_ServiceComplete);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_DCB_Unlock");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
