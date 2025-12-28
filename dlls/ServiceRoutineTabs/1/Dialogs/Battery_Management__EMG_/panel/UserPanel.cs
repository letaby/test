// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Battery_Management__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Battery_Management__EMG_.panel;

public class UserPanel : CustomPanel
{
  private const string NumberofStringsQualifier = "ptconf_p_Veh_BatNumOfStrings_u8";
  private const string HighVoltageLockQualifier = "DL_High_Voltage_Lock";
  private const string HighVoltageLockedQualifier = "DT_STO_High_Voltage_Lock_High_Voltage_Lock";
  private static int MaxBatteryCount = 9;
  private Channel ecpc01tChannel = (Channel) null;
  private Parameter numberofStringsParameter = (Parameter) null;
  private int previousBatteryCount = 0;
  private readonly string[] BmsEcus = new string[9]
  {
    "BMS01T",
    "BMS201T",
    "BMS301T",
    "BMS401T",
    "BMS501T",
    "BMS601T",
    "BMS701T",
    "BMS801T",
    "BMS901T"
  };
  private Channel[] Bms = new Channel[9];
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock1;
  private RunServiceButton runServiceButtonBMS1Lock;
  private RunServiceButton runServiceButtonBMS1Unlock;
  private System.Windows.Forms.Label labelBatteryManagement;
  private System.Windows.Forms.Label labelTransportLockBitStatus;
  private System.Windows.Forms.Label labelTransportLockBitControl;
  private RunServiceButton runServiceButtonBMS3Lock;
  private RunServiceButton runServiceButtonBMS2Lock;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock9;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock8;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock7;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock6;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock5;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock4;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock3;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransportLock2;
  private RunServiceButton runServiceButtonBMS9Lock;
  private RunServiceButton runServiceButtonBMS8Lock;
  private RunServiceButton runServiceButtonBMS7Lock;
  private RunServiceButton runServiceButtonBMS6Lock;
  private RunServiceButton runServiceButtonBMS5Lock;
  private RunServiceButton runServiceButtonBMS4Lock;
  private RunServiceButton runServiceButtonBMS9Unlock;
  private RunServiceButton runServiceButtonBMS8Unlock;
  private RunServiceButton runServiceButtonBMS7Unlock;
  private RunServiceButton runServiceButtonBMS6Unlock;
  private RunServiceButton runServiceButtonBMS5Unlock;
  private RunServiceButton runServiceButtonBMS4Unlock;
  private RunServiceButton runServiceButtonBMS3Unlock;
  private RunServiceButton runServiceButtonBMS2Unlock;
  private TableLayoutPanel tableLayoutPanel4;
  private DigitalReadoutInstrument digitalReadoutInstrumentCharging;
  private System.Windows.Forms.Label label1;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private System.Windows.Forms.Label labelInterlockWarning;
  private System.Windows.Forms.Label label39;
  private TableLayoutPanel tableLayoutPanel1;

  private bool EcpcOnline
  {
    get
    {
      return this.ecpc01tChannel != null && (this.ecpc01tChannel.CommunicationsState == CommunicationsState.Online || this.ecpc01tChannel.CommunicationsState == CommunicationsState.LogFilePlayback);
    }
  }

  private int BatteryCount
  {
    get
    {
      int result = 9;
      if (this.EcpcOnline && this.numberofStringsParameter != null && this.numberofStringsParameter.HasBeenReadFromEcu && this.numberofStringsParameter.Value != null)
        int.TryParse(this.numberofStringsParameter.Value.ToString(), out result);
      if (result <= 3)
        result = 4;
      if (result > UserPanel.MaxBatteryCount)
        result = UserPanel.MaxBatteryCount;
      return result;
    }
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.previousBatteryCount = this.BatteryCount;
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.PreconditionRepresentedStateChanged);
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.PreconditionRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock1.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock2.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock3.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock4.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock5.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock6.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock7.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock8.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.digitalReadoutInstrumentTransportLock9.RepresentedStateChanged += new EventHandler(this.TransportLockRepresentedStateChanged);
    this.UpdateUserInterface();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public virtual void OnChannelsChanged()
  {
    for (int bmsNum = 0; bmsNum < ((IEnumerable<string>) this.BmsEcus).Count<string>(); ++bmsNum)
      this.SetBms(bmsNum, this.GetChannel(this.BmsEcus[bmsNum]));
    this.SetECPC(this.GetChannel("ECPC01T", (CustomPanel.ChannelLookupOptions) 3));
    this.UpdateUserInterface();
  }

  private void SetECPC(Channel ecpc01t)
  {
    if (this.ecpc01tChannel == ecpc01t)
      return;
    if (this.ecpc01tChannel != null)
    {
      this.ecpc01tChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.numberofStringsParameter = (Parameter) null;
    }
    this.ecpc01tChannel = ecpc01t;
    if (this.ecpc01tChannel != null)
    {
      this.ecpc01tChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.numberofStringsParameter = this.ecpc01tChannel.Parameters["ptconf_p_Veh_BatNumOfStrings_u8"];
      if (this.EcpcOnline)
        this.ReadInitialParameters();
    }
  }

  private void SetBms(int bmsNum, Channel channel)
  {
    if (this.Bms[bmsNum] == channel)
      return;
    if (this.Bms[bmsNum] != null)
      this.Bms[bmsNum].Services["DL_High_Voltage_Lock"].ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.BmsServiceCompleteEvent);
    this.Bms[bmsNum] = channel;
    if (this.Bms[bmsNum] != null)
      this.Bms[bmsNum].Services["DL_High_Voltage_Lock"].ServiceCompleteEvent += new ServiceCompleteEventHandler(this.BmsServiceCompleteEvent);
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    if (this.EcpcOnline)
      this.ReadInitialParameters();
    this.UpdateUserInterface();
  }

  private void ReadInitialParameters()
  {
    if (this.EcpcOnline && this.ecpc01tChannel.CommunicationsState == CommunicationsState.Online && this.ecpc01tChannel.Parameters != null && this.numberofStringsParameter != null && !this.numberofStringsParameter.HasBeenReadFromEcu)
    {
      this.ecpc01tChannel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
      this.ecpc01tChannel.Parameters.ReadGroup(this.numberofStringsParameter.GroupQualifier, false, false);
    }
    this.UpdateUserInterface();
  }

  private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.ecpc01tChannel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
    this.UpdateUserInterface();
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    for (int bmsNum = 0; bmsNum < ((IEnumerable<string>) this.BmsEcus).Count<string>(); ++bmsNum)
      this.SetBms(bmsNum, (Channel) null);
  }

  private void UpdateUserInterface()
  {
    if (this.BatteryCount != this.previousBatteryCount)
    {
      ((Control) this.tableLayoutPanel1).SuspendLayout();
      for (int index = 0; index < UserPanel.MaxBatteryCount; ++index)
        ((TableLayoutPanel) this.tableLayoutPanel1).Controls[$"digitalReadoutInstrumentTransportLock{index + 1}"].Visible = ((TableLayoutPanel) this.tableLayoutPanel1).Controls[$"runServiceButtonBMS{index + 1}Lock"].Visible = ((TableLayoutPanel) this.tableLayoutPanel1).Controls[$"runServiceButtonBMS{index + 1}Unlock"].Visible = index < this.BatteryCount;
      ((Control) this.tableLayoutPanel1).ResumeLayout();
      this.previousBatteryCount = this.BatteryCount;
    }
    this.labelInterlockWarning.Visible = !this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS1Lock).Enabled = this.digitalReadoutInstrumentTransportLock1.RepresentedState == 1 && this.VehicleCheckOk && ((Control) this.runServiceButtonBMS1Lock).Visible;
    ((Control) this.runServiceButtonBMS1Unlock).Enabled = this.digitalReadoutInstrumentTransportLock1.RepresentedState == 3 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS2Lock).Enabled = this.digitalReadoutInstrumentTransportLock2.RepresentedState == 1 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS2Unlock).Enabled = this.digitalReadoutInstrumentTransportLock2.RepresentedState == 3 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS3Lock).Enabled = this.digitalReadoutInstrumentTransportLock3.RepresentedState == 1 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS3Unlock).Enabled = this.digitalReadoutInstrumentTransportLock3.RepresentedState == 3 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS4Lock).Enabled = this.digitalReadoutInstrumentTransportLock4.RepresentedState == 1 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS4Unlock).Enabled = this.digitalReadoutInstrumentTransportLock4.RepresentedState == 3 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS5Lock).Enabled = this.digitalReadoutInstrumentTransportLock5.RepresentedState == 1 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS5Unlock).Enabled = this.digitalReadoutInstrumentTransportLock5.RepresentedState == 3 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS6Lock).Enabled = this.digitalReadoutInstrumentTransportLock6.RepresentedState == 1 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS6Unlock).Enabled = this.digitalReadoutInstrumentTransportLock6.RepresentedState == 3 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS7Lock).Enabled = this.digitalReadoutInstrumentTransportLock7.RepresentedState == 1 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS7Unlock).Enabled = this.digitalReadoutInstrumentTransportLock7.RepresentedState == 3 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS8Lock).Enabled = this.digitalReadoutInstrumentTransportLock8.RepresentedState == 1 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS8Unlock).Enabled = this.digitalReadoutInstrumentTransportLock8.RepresentedState == 3 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS9Lock).Enabled = this.digitalReadoutInstrumentTransportLock9.RepresentedState == 1 && this.VehicleCheckOk;
    ((Control) this.runServiceButtonBMS9Unlock).Enabled = this.digitalReadoutInstrumentTransportLock9.RepresentedState == 3 && this.VehicleCheckOk;
  }

  private void PreconditionRepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void TransportLockRepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void BmsServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (!(service != (Service) null) || service.Channel.EcuInfos["DT_STO_High_Voltage_Lock_High_Voltage_Lock"] == null)
      return;
    service.Channel.EcuInfos["DT_STO_High_Voltage_Lock_High_Voltage_Lock"].Read(false);
  }

  private bool VehicleCheckOk
  {
    get
    {
      return this.digitalReadoutInstrumentParkBrake.RepresentedState != 3 && this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && this.digitalReadoutInstrumentCharging.RepresentedState == 1;
    }
  }

  private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
    this.label1 = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.labelInterlockWarning = new System.Windows.Forms.Label();
    this.label39 = new System.Windows.Forms.Label();
    this.runServiceButtonBMS9Unlock = new RunServiceButton();
    this.runServiceButtonBMS8Unlock = new RunServiceButton();
    this.runServiceButtonBMS7Unlock = new RunServiceButton();
    this.runServiceButtonBMS6Unlock = new RunServiceButton();
    this.runServiceButtonBMS5Unlock = new RunServiceButton();
    this.runServiceButtonBMS4Unlock = new RunServiceButton();
    this.runServiceButtonBMS3Unlock = new RunServiceButton();
    this.runServiceButtonBMS2Unlock = new RunServiceButton();
    this.runServiceButtonBMS9Lock = new RunServiceButton();
    this.runServiceButtonBMS8Lock = new RunServiceButton();
    this.runServiceButtonBMS7Lock = new RunServiceButton();
    this.runServiceButtonBMS6Lock = new RunServiceButton();
    this.runServiceButtonBMS5Lock = new RunServiceButton();
    this.runServiceButtonBMS4Lock = new RunServiceButton();
    this.runServiceButtonBMS3Lock = new RunServiceButton();
    this.runServiceButtonBMS2Lock = new RunServiceButton();
    this.digitalReadoutInstrumentTransportLock9 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransportLock8 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransportLock7 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransportLock6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransportLock5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransportLock4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransportLock3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransportLock2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransportLock1 = new DigitalReadoutInstrument();
    this.runServiceButtonBMS1Lock = new RunServiceButton();
    this.runServiceButtonBMS1Unlock = new RunServiceButton();
    this.labelBatteryManagement = new System.Windows.Forms.Label();
    this.labelTransportLockBitStatus = new System.Windows.Forms.Label();
    this.labelTransportLockBitControl = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS9Unlock, 3, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS8Unlock, 3, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS7Unlock, 3, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS6Unlock, 3, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS5Unlock, 3, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS4Unlock, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS3Unlock, 3, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS2Unlock, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS9Lock, 2, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS8Lock, 2, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS7Lock, 2, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS6Lock, 2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS5Lock, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS4Lock, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS3Lock, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS2Lock, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock9, 1, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock8, 1, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock7, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock6, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock5, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock4, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock3, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock2, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentTransportLock1, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS1Lock, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonBMS1Unlock, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelBatteryManagement, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelTransportLockBitStatus, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelTransportLockBitControl, 2, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentCharging, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.label1, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelInterlockWarning, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.label39, 0, 2);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel4, 10);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCharging, "digitalReadoutInstrumentCharging");
    this.digitalReadoutInstrumentCharging.FontGroup = (string) null;
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
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 3, 5, "mph");
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(5, (double) int.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.labelInterlockWarning, "labelInterlockWarning");
    this.labelInterlockWarning.ForeColor = Color.Red;
    this.labelInterlockWarning.Name = "labelInterlockWarning";
    this.labelInterlockWarning.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label39, "label39");
    this.label39.Name = "label39";
    this.label39.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS9Unlock, "runServiceButtonBMS9Unlock");
    ((Control) this.runServiceButtonBMS9Unlock).Name = "runServiceButtonBMS9Unlock";
    this.runServiceButtonBMS9Unlock.ServiceCall = new ServiceCall("BMS901T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS8Unlock, "runServiceButtonBMS8Unlock");
    ((Control) this.runServiceButtonBMS8Unlock).Name = "runServiceButtonBMS8Unlock";
    this.runServiceButtonBMS8Unlock.ServiceCall = new ServiceCall("BMS801T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS7Unlock, "runServiceButtonBMS7Unlock");
    ((Control) this.runServiceButtonBMS7Unlock).Name = "runServiceButtonBMS7Unlock";
    this.runServiceButtonBMS7Unlock.ServiceCall = new ServiceCall("BMS701T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS6Unlock, "runServiceButtonBMS6Unlock");
    ((Control) this.runServiceButtonBMS6Unlock).Name = "runServiceButtonBMS6Unlock";
    this.runServiceButtonBMS6Unlock.ServiceCall = new ServiceCall("BMS601T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS5Unlock, "runServiceButtonBMS5Unlock");
    ((Control) this.runServiceButtonBMS5Unlock).Name = "runServiceButtonBMS5Unlock";
    this.runServiceButtonBMS5Unlock.ServiceCall = new ServiceCall("BMS501T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS4Unlock, "runServiceButtonBMS4Unlock");
    ((Control) this.runServiceButtonBMS4Unlock).Name = "runServiceButtonBMS4Unlock";
    this.runServiceButtonBMS4Unlock.ServiceCall = new ServiceCall("BMS401T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS3Unlock, "runServiceButtonBMS3Unlock");
    ((Control) this.runServiceButtonBMS3Unlock).Name = "runServiceButtonBMS3Unlock";
    this.runServiceButtonBMS3Unlock.ServiceCall = new ServiceCall("BMS301T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS2Unlock, "runServiceButtonBMS2Unlock");
    ((Control) this.runServiceButtonBMS2Unlock).Name = "runServiceButtonBMS2Unlock";
    this.runServiceButtonBMS2Unlock.ServiceCall = new ServiceCall("BMS201T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS9Lock, "runServiceButtonBMS9Lock");
    ((Control) this.runServiceButtonBMS9Lock).Name = "runServiceButtonBMS9Lock";
    this.runServiceButtonBMS9Lock.ServiceCall = new ServiceCall("BMS901T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS8Lock, "runServiceButtonBMS8Lock");
    ((Control) this.runServiceButtonBMS8Lock).Name = "runServiceButtonBMS8Lock";
    this.runServiceButtonBMS8Lock.ServiceCall = new ServiceCall("BMS801T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS7Lock, "runServiceButtonBMS7Lock");
    ((Control) this.runServiceButtonBMS7Lock).Name = "runServiceButtonBMS7Lock";
    this.runServiceButtonBMS7Lock.ServiceCall = new ServiceCall("BMS701T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS6Lock, "runServiceButtonBMS6Lock");
    ((Control) this.runServiceButtonBMS6Lock).Name = "runServiceButtonBMS6Lock";
    this.runServiceButtonBMS6Lock.ServiceCall = new ServiceCall("BMS601T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS5Lock, "runServiceButtonBMS5Lock");
    ((Control) this.runServiceButtonBMS5Lock).Name = "runServiceButtonBMS5Lock";
    this.runServiceButtonBMS5Lock.ServiceCall = new ServiceCall("BMS501T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS4Lock, "runServiceButtonBMS4Lock");
    ((Control) this.runServiceButtonBMS4Lock).Name = "runServiceButtonBMS4Lock";
    this.runServiceButtonBMS4Lock.ServiceCall = new ServiceCall("BMS401T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS3Lock, "runServiceButtonBMS3Lock");
    ((Control) this.runServiceButtonBMS3Lock).Name = "runServiceButtonBMS3Lock";
    this.runServiceButtonBMS3Lock.ServiceCall = new ServiceCall("BMS301T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS2Lock, "runServiceButtonBMS2Lock");
    ((Control) this.runServiceButtonBMS2Lock).Name = "runServiceButtonBMS2Lock";
    this.runServiceButtonBMS2Lock.ServiceCall = new ServiceCall("BMS201T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock9, "digitalReadoutInstrumentTransportLock9");
    this.digitalReadoutInstrumentTransportLock9.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock9).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock9.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock9.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock9.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock9.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock9).Instrument = new Qualifier((QualifierTypes) 8, "BMS901T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock9).Name = "digitalReadoutInstrumentTransportLock9";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock9).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock8, "digitalReadoutInstrumentTransportLock8");
    this.digitalReadoutInstrumentTransportLock8.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock8).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock8.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock8.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock8.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock8.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock8).Instrument = new Qualifier((QualifierTypes) 8, "BMS801T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock8).Name = "digitalReadoutInstrumentTransportLock8";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock7, "digitalReadoutInstrumentTransportLock7");
    this.digitalReadoutInstrumentTransportLock7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock7).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock7.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock7.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock7.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock7.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock7).Instrument = new Qualifier((QualifierTypes) 8, "BMS701T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock7).Name = "digitalReadoutInstrumentTransportLock7";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock6, "digitalReadoutInstrumentTransportLock6");
    this.digitalReadoutInstrumentTransportLock6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock6).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock6.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock6.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock6.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock6.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock6).Instrument = new Qualifier((QualifierTypes) 8, "BMS601T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock6).Name = "digitalReadoutInstrumentTransportLock6";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock5, "digitalReadoutInstrumentTransportLock5");
    this.digitalReadoutInstrumentTransportLock5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock5).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock5.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock5.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock5.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock5.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock5).Instrument = new Qualifier((QualifierTypes) 8, "BMS501T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock5).Name = "digitalReadoutInstrumentTransportLock5";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock4, "digitalReadoutInstrumentTransportLock4");
    this.digitalReadoutInstrumentTransportLock4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock4).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock4.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock4.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock4.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock4.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock4).Instrument = new Qualifier((QualifierTypes) 8, "BMS401T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock4).Name = "digitalReadoutInstrumentTransportLock4";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock3, "digitalReadoutInstrumentTransportLock3");
    this.digitalReadoutInstrumentTransportLock3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock3).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock3.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock3.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock3.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock3.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock3).Instrument = new Qualifier((QualifierTypes) 8, "BMS301T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock3).Name = "digitalReadoutInstrumentTransportLock3";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock2, "digitalReadoutInstrumentTransportLock2");
    this.digitalReadoutInstrumentTransportLock2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock2).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock2.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock2.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock2.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock2.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock2).Instrument = new Qualifier((QualifierTypes) 8, "BMS201T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock2).Name = "digitalReadoutInstrumentTransportLock2";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransportLock1, "digitalReadoutInstrumentTransportLock1");
    this.digitalReadoutInstrumentTransportLock1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock1).FreezeValue = false;
    this.digitalReadoutInstrumentTransportLock1.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentTransportLock1.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransportLock1.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransportLock1.Gradient.Modify(3, 2.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock1).Instrument = new Qualifier((QualifierTypes) 8, "BMS01T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock");
    ((Control) this.digitalReadoutInstrumentTransportLock1).Name = "digitalReadoutInstrumentTransportLock1";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransportLock1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS1Lock, "runServiceButtonBMS1Lock");
    ((Control) this.runServiceButtonBMS1Lock).Name = "runServiceButtonBMS1Lock";
    this.runServiceButtonBMS1Lock.ServiceCall = new ServiceCall("BMS01T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=1"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonBMS1Unlock, "runServiceButtonBMS1Unlock");
    ((Control) this.runServiceButtonBMS1Unlock).Name = "runServiceButtonBMS1Unlock";
    this.runServiceButtonBMS1Unlock.ServiceCall = new ServiceCall("BMS01T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
    {
      "High_Voltage_Lock=0"
    });
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelBatteryManagement, 4);
    componentResourceManager.ApplyResources((object) this.labelBatteryManagement, "labelBatteryManagement");
    this.labelBatteryManagement.Name = "labelBatteryManagement";
    this.labelBatteryManagement.UseCompatibleTextRendering = true;
    this.labelTransportLockBitStatus.BorderStyle = BorderStyle.FixedSingle;
    componentResourceManager.ApplyResources((object) this.labelTransportLockBitStatus, "labelTransportLockBitStatus");
    this.labelTransportLockBitStatus.Name = "labelTransportLockBitStatus";
    this.labelTransportLockBitStatus.UseCompatibleTextRendering = true;
    this.labelTransportLockBitControl.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelTransportLockBitControl, 2);
    componentResourceManager.ApplyResources((object) this.labelTransportLockBitControl, "labelTransportLockBitControl");
    this.labelTransportLockBitControl.Name = "labelTransportLockBitControl";
    this.labelTransportLockBitControl.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Battery_Management");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
