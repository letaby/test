// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Nox_Sensor_Drift_Verification__MY20_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Nox_Sensor_Drift_Verification__MY20_.panel;

public class UserPanel : CustomPanel
{
  private const string MCMName = "MCM21T";
  private const string ACMName = "ACM301T";
  private Channel acm = (Channel) null;
  private Channel mcm = (Channel) null;
  private Channel cpc = (Channel) null;
  private WarningManager warningManager;
  private UserPanel.NOxSensorDriftTest NOxSensorTest;
  private Timer ignitionTimer;
  private TableLayoutPanel tableLayoutPanelPreReqs;
  private TableLayoutPanel tableLayoutPanelTestControls;
  private TableLayoutPanel tableLayoutPanelMain;
  private SeekTimeListView seekTimeListViewLog;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private BarInstrument barInstrumentNOxRaw;
  private BarInstrument barInstrumentNOxOut;
  private BarInstrument barInstrument1;
  private BarInstrument barInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private Button buttonStart;
  private Checkmark checkmarkStatus;
  private System.Windows.Forms.Label labelStatus;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;
  private TableLayoutPanel tableLayoutPanelDifferenceInstrument;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelInstrumentDifferenceTitle;
  private ScalingLabel scalingLabelInstrumentDifference;
  private BarInstrument barInstrument7;
  private BarInstrument barInstrument8;
  private BarInstrument barInstrument9;
  private DigitalReadoutInstrument digitalReadoutInstrumentNOxDewpointInlet;
  private DigitalReadoutInstrument digitalReadoutInstrumentNeutralSwitch;
  private Button buttonStop;
  private Button buttonClose;
  private DigitalReadoutInstrument digitalReadoutInstrumentNOxDewpointOutlet;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheckStatus;
  private ScalingLabel scalingLabelTestResult;
  private DigitalReadoutInstrument digitalReadoutInstrumentDPFRegenState;
  private TableLayoutPanel tableLayoutPanelNoxHours;
  private DigitalReadoutInstrument digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime;
  private DigitalReadoutInstrument digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime;
  private BarInstrument barInstrument4;
  private TableLayoutPanel tableLayoutPanelThermometers;

  internal bool adrReturnValue { get; set; }

  internal string adrReturnMessage { get; set; }

  private UserPanel.CpcVersion ConnectedCpcVersion { get; set; }

  public UserPanel()
  {
    this.NOxSensorTest = new UserPanel.NOxSensorDriftTest(this);
    this.InitializeComponent();
    this.ignitionTimer = new Timer();
    this.ignitionTimer.Interval = 10000;
    this.ignitionTimer.Tick += new EventHandler(this.OnTimerTick);
    ConnectionManager.GlobalInstance.PropertyChanged += new PropertyChangedEventHandler(this.GlobalInstance_PropertyChanged);
    this.warningManager = new WarningManager(Resources.WarningManagerMessage, Resources.Message_NOx_Sensor_Test, this.seekTimeListViewLog.RequiredUserLabelPrefix);
    this.StartIgnitionTimer();
  }

  protected virtual void Dispose(bool disposing)
  {
    try
    {
      if (!disposing)
        return;
      this.NOxSensorTest.StopDosingQuantityCheckTimer();
      this.NOxSensorTest.dosingQuantityCheckTimer.Dispose();
      this.ignitionTimer.Dispose();
      this.ignitionTimer = (Timer) null;
    }
    finally
    {
      base.Dispose(disposing);
    }
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    this.digitalReadoutInstrumentNOxDewpointInlet.RepresentedStateChanged += new EventHandler(this.NOxSensorTest.NOxDewpointSensor_RepresentedStateChanged);
    this.digitalReadoutInstrumentNOxDewpointOutlet.RepresentedStateChanged += new EventHandler(this.NOxSensorTest.NOxDewpointSensor_RepresentedStateChanged);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDPFRegenState).DataChanged += new EventHandler(this.NOxSensorTest.digitalReadoutInstrumentDPFRegenState_DataChanged);
    this.UpdateChannels();
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ConnectionManager.GlobalInstance.PropertyChanged -= new PropertyChangedEventHandler(this.GlobalInstance_PropertyChanged);
    if (this.ignitionTimer != null)
    {
      this.ignitionTimer.Stop();
      this.ignitionTimer.Tick -= new EventHandler(this.OnTimerTick);
    }
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.digitalReadoutInstrumentNOxDewpointInlet.RepresentedStateChanged -= new EventHandler(this.NOxSensorTest.NOxDewpointSensor_RepresentedStateChanged);
    this.digitalReadoutInstrumentNOxDewpointOutlet.RepresentedStateChanged -= new EventHandler(this.NOxSensorTest.NOxDewpointSensor_RepresentedStateChanged);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDPFRegenState).DataChanged -= new EventHandler(this.NOxSensorTest.digitalReadoutInstrumentDPFRegenState_DataChanged);
    this.CleanUpChannels();
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.adrReturnValue,
      (object) this.adrReturnMessage
    };
  }

  public bool CanClose
  {
    get
    {
      return this.NOxSensorTest.TestActiveStatus != UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive;
    }
  }

  private bool TestPreconditionsMet
  {
    get
    {
      return this.digitalReadoutInstrumentEngineSpeed.RepresentedState == 1 && this.digitalReadoutInstrumentVehicleCheckStatus.RepresentedState == 1;
    }
  }

  private void UpdateTestReadyStatus()
  {
    if (this.NOxSensorTest.TestActiveStatus == UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive)
    {
      if (this.TestPreconditionsMet)
      {
        this.labelStatus.Text = this.NOxSensorTest.TestStepMessage;
        this.checkmarkStatus.Checked = true;
      }
      else
        this.StopWork(UserPanel.Reason.TestConditionsNotMet);
    }
    else
    {
      if (this.NOxSensorTest.TestActiveStatus != UserPanel.NOxSensorDriftTest.TestActivityStatus.TestInactive)
        return;
      this.checkmarkStatus.Checked = this.TestPreconditionsMet;
      if (this.TestPreconditionsMet)
        this.labelStatus.Text = Resources.Message_Test_Ready_To_Be_Run;
      else
        this.labelStatus.Text = Resources.Error_Test_Not_Ready_To_Be_Run;
    }
  }

  private void UpdateUserInterface()
  {
    if (this.NOxSensorTest == null)
      return;
    this.UpdateTestReadyStatus();
    this.buttonStart.Enabled = this.NOxSensorTest.TestActiveStatus == UserPanel.NOxSensorDriftTest.TestActivityStatus.TestInactive && this.TestPreconditionsMet && this.AreAllChannelsOnline;
    this.buttonStop.Enabled = this.NOxSensorTest.TestActiveStatus == UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive;
    this.buttonClose.Enabled = this.CanClose;
  }

  private void ClearResults()
  {
    ((Control) this.scalingLabelInstrumentDifference).Text = string.Empty;
    this.scalingLabelInstrumentDifference.RepresentedState = (ValueState) 0;
    ((Control) this.scalingLabelTestResult).Text = string.Empty;
    this.scalingLabelTestResult.RepresentedState = (ValueState) 0;
  }

  private bool UpdateChannels()
  {
    bool flag1 = this.SetCPC(this.GetChannel("CPC302T", (CustomPanel.ChannelLookupOptions) 5));
    bool flag2 = this.SetMCM(UserPanel.GetActiveChannel("MCM21T"));
    bool flag3 = this.SetACM(UserPanel.GetActiveChannel("ACM301T"));
    return flag1 || flag2 || flag3;
  }

  private void CleanUpChannels()
  {
    this.SetCPC((Channel) null);
    this.SetMCM((Channel) null);
    this.SetACM((Channel) null);
  }

  public bool DewpointSensorsReady
  {
    get
    {
      return this.digitalReadoutInstrumentNOxDewpointInlet.RepresentedState == 1 && this.digitalReadoutInstrumentNOxDewpointOutlet.RepresentedState == 1;
    }
  }

  private void StopWork(UserPanel.Reason reason) => this.NOxSensorTest.EndTest(reason);

  private void buttonStart_Click(object sender, EventArgs e)
  {
    if (!this.warningManager.RequestContinue())
      return;
    this.ClearResults();
    this.adrReturnValue = false;
    this.adrReturnMessage = string.Empty;
    this.NOxSensorTest.StartTest();
  }

  private void buttonStop_Click(object sender, EventArgs e)
  {
    this.NOxSensorTest.EndTest(UserPanel.Reason.Canceled);
  }

  private void DisplayResultStatus(ValueState state)
  {
    this.scalingLabelInstrumentDifference.RepresentedState = state;
    ((Control) this.scalingLabelInstrumentDifference).Refresh();
  }

  private void OnTimerTick(object sender, EventArgs e)
  {
    this.ignitionTimer.Stop();
    ((Control) this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).Visible = this.digitalReadoutInstrumentNOxDewpointInlet != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointInlet).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointInlet).DataItem.ValueAsDouble(((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointInlet).DataItem.Value) == 0.0;
    ((Control) this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).Visible = this.digitalReadoutInstrumentNOxDewpointOutlet != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointOutlet).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointOutlet).DataItem.ValueAsDouble(((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointOutlet).DataItem.Value) == 0.0;
  }

  private void StartIgnitionTimer()
  {
    if (this.ignitionTimer == null)
      return;
    if (ConnectionManager.GlobalInstance.IgnitionStatus == null && this.acm != null)
      this.ignitionTimer.Start();
    else
      this.ignitionTimer.Stop();
  }

  private void GlobalInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    this.StartIgnitionTimer();
  }

  public virtual void OnChannelsChanged()
  {
    if (!this.UpdateChannels())
      return;
    UserPanel.UpdateConnectedEquipmentType();
    this.ClearResults();
    this.UpdateUserInterface();
  }

  private void TestPrerequisite_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  public static bool IsMediumDuty { get; set; }

  private static bool IsMediumDutyEquipment(EquipmentType equipment)
  {
    if (EquipmentType.op_Inequality(equipment, EquipmentType.Empty))
    {
      switch (((EquipmentType) ref equipment).Name.ToUpperInvariant())
      {
        case "DD5":
        case "DD8":
        case "MDEG 4-CYLINDER TIER4":
        case "MDEG 6-CYLINDER TIER4":
          return true;
      }
    }
    return false;
  }

  private static void UpdateConnectedEquipmentType()
  {
    UserPanel.IsMediumDuty = UserPanel.IsMediumDutyEquipment(SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault<EquipmentType>((Func<EquipmentType, bool>) (et =>
    {
      ElectronicsFamily family = ((EquipmentType) ref et).Family;
      return ((ElectronicsFamily) ref family).Category.Equals("Engine", StringComparison.OrdinalIgnoreCase);
    })));
  }

  private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
  {
    if (!e.Category.Equals("Engine", StringComparison.OrdinalIgnoreCase))
      return;
    UserPanel.UpdateConnectedEquipmentType();
  }

  private static Channel GetActiveChannel(string channelName)
  {
    return SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (ac => ac.Ecu.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase)));
  }

  private bool SetCPC(Channel cpc)
  {
    if (this.cpc == cpc)
      return false;
    this.warningManager.Reset();
    if (this.NOxSensorTest.TestActiveStatus == UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive)
      this.StopWork(UserPanel.Reason.Disconnected);
    if (this.cpc != null)
      this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    this.ConnectedCpcVersion = UserPanel.CpcVersion.Invalid;
    this.cpc = cpc;
    if (this.cpc != null)
    {
      if (string.Equals(this.cpc.Ecu.Name, "CPC302T", StringComparison.OrdinalIgnoreCase))
        this.ConnectedCpcVersion = UserPanel.CpcVersion.CPC3;
      else if (string.Equals(this.cpc.Ecu.Name, "CPC501T", StringComparison.OrdinalIgnoreCase) || string.Equals(this.cpc.Ecu.Name, "CPC502T", StringComparison.OrdinalIgnoreCase))
        this.ConnectedCpcVersion = UserPanel.CpcVersion.CPC5;
      this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
    return true;
  }

  private bool SetMCM(Channel mcm)
  {
    if (this.mcm == mcm)
      return false;
    this.warningManager.Reset();
    if (this.NOxSensorTest.TestActiveStatus == UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive)
      this.StopWork(UserPanel.Reason.Disconnected);
    if (this.mcm != null)
      this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    this.mcm = mcm;
    if (this.mcm != null)
      this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    return true;
  }

  private bool SetACM(Channel acm)
  {
    if (this.acm == acm)
      return false;
    this.warningManager.Reset();
    if (this.NOxSensorTest.TestActiveStatus == UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive)
      this.StopWork(UserPanel.Reason.Disconnected);
    if (this.acm != null)
      this.acm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    this.acm = acm;
    if (this.acm != null)
    {
      this.acm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.StartIgnitionTimer();
    }
    return true;
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  public static bool IsChannelOnline(Channel channel)
  {
    return channel != null && channel.CommunicationsState == CommunicationsState.Online;
  }

  private bool AreAllChannelsOnline
  {
    get
    {
      return UserPanel.IsChannelOnline(this.cpc) && UserPanel.IsChannelOnline(this.mcm) && UserPanel.IsChannelOnline(this.acm);
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelPreReqs = new TableLayoutPanel();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDPFRegenState = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentNeutralSwitch = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleCheckStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentNOxDewpointInlet = new DigitalReadoutInstrument();
    this.checkmarkStatus = new Checkmark();
    this.barInstrumentNOxRaw = new BarInstrument();
    this.barInstrumentNOxOut = new BarInstrument();
    this.labelStatus = new System.Windows.Forms.Label();
    this.seekTimeListViewLog = new SeekTimeListView();
    this.tableLayoutPanelTestControls = new TableLayoutPanel();
    this.buttonStart = new Button();
    this.buttonStop = new Button();
    this.barInstrument1 = new BarInstrument();
    this.barInstrument2 = new BarInstrument();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.tableLayoutPanelNoxHours = new TableLayoutPanel();
    this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime = new DigitalReadoutInstrument();
    this.tableLayoutPanelThermometers = new TableLayoutPanel();
    this.barInstrument4 = new BarInstrument();
    this.barInstrument7 = new BarInstrument();
    this.barInstrument8 = new BarInstrument();
    this.barInstrument9 = new BarInstrument();
    this.digitalReadoutInstrumentNOxDewpointOutlet = new DigitalReadoutInstrument();
    this.tableLayoutPanelDifferenceInstrument = new TableLayoutPanel();
    this.labelInstrumentDifferenceTitle = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelInstrumentDifference = new ScalingLabel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.buttonClose = new Button();
    this.scalingLabelTestResult = new ScalingLabel();
    ((Control) this.tableLayoutPanelPreReqs).SuspendLayout();
    ((Control) this.tableLayoutPanelTestControls).SuspendLayout();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this.tableLayoutPanelNoxHours).SuspendLayout();
    ((Control) this.tableLayoutPanelThermometers).SuspendLayout();
    ((Control) this.tableLayoutPanelDifferenceInstrument).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelPreReqs, "tableLayoutPanelPreReqs");
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetColumnSpan((Control) this.tableLayoutPanelPreReqs, 4);
    ((TableLayoutPanel) this.tableLayoutPanelPreReqs).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelPreReqs).Controls.Add((Control) this.digitalReadoutInstrumentDPFRegenState, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanelPreReqs).Controls.Add((Control) this.digitalReadoutInstrumentNeutralSwitch, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelPreReqs).Controls.Add((Control) this.digitalReadoutInstrumentVehicleCheckStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelPreReqs).Controls.Add((Control) this.digitalReadoutInstrumentParkingBrake, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelPreReqs).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanelPreReqs).Name = "tableLayoutPanelPreReqs";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = "prereqs";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += new EventHandler(this.TestPrerequisite_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDPFRegenState, "digitalReadoutInstrumentDPFRegenState");
    this.digitalReadoutInstrumentDPFRegenState.FontGroup = "";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDPFRegenState).FreezeValue = false;
    this.digitalReadoutInstrumentDPFRegenState.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentDPFRegenState.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentDPFRegenState.Gradient.Modify(2, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentDPFRegenState.Gradient.Modify(3, 4.0, (ValueState) 2);
    this.digitalReadoutInstrumentDPFRegenState.Gradient.Modify(4, 8.0, (ValueState) 2);
    this.digitalReadoutInstrumentDPFRegenState.Gradient.Modify(5, 16.0, (ValueState) 2);
    this.digitalReadoutInstrumentDPFRegenState.Gradient.Modify(6, 32.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDPFRegenState).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS064_DPF_Regen_State");
    ((Control) this.digitalReadoutInstrumentDPFRegenState).Name = "digitalReadoutInstrumentDPFRegenState";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDPFRegenState).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNeutralSwitch, "digitalReadoutInstrumentNeutralSwitch");
    this.digitalReadoutInstrumentNeutralSwitch.FontGroup = "prereqs";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNeutralSwitch).FreezeValue = false;
    this.digitalReadoutInstrumentNeutralSwitch.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentNeutralSwitch.Gradient.Modify(1, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentNeutralSwitch.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNeutralSwitch).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "NeutralSwitch");
    ((Control) this.digitalReadoutInstrumentNeutralSwitch).Name = "digitalReadoutInstrumentNeutralSwitch";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNeutralSwitch).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleCheckStatus, "digitalReadoutInstrumentVehicleCheckStatus");
    this.digitalReadoutInstrumentVehicleCheckStatus.FontGroup = "prereqs";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutInstrumentVehicleCheckStatus).Name = "digitalReadoutInstrumentVehicleCheckStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleCheckStatus.RepresentedStateChanged += new EventHandler(this.TestPrerequisite_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
    this.digitalReadoutInstrumentParkingBrake.FontGroup = "prereqs";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNOxDewpointInlet, "digitalReadoutInstrumentNOxDewpointInlet");
    this.digitalReadoutInstrumentNOxDewpointInlet.FontGroup = "small";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointInlet).FreezeValue = false;
    this.digitalReadoutInstrumentNOxDewpointInlet.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentNOxDewpointInlet.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentNOxDewpointInlet.Gradient.Initialize((ValueState) 0, 1);
    this.digitalReadoutInstrumentNOxDewpointInlet.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointInlet).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet");
    ((Control) this.digitalReadoutInstrumentNOxDewpointInlet).Name = "digitalReadoutInstrumentNOxDewpointInlet";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointInlet).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointInlet).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    componentResourceManager.ApplyResources((object) this.barInstrumentNOxRaw, "barInstrumentNOxRaw");
    this.barInstrumentNOxRaw.FontGroup = "ShortBar";
    ((SingleInstrumentBase) this.barInstrumentNOxRaw).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentNOxRaw).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS111_NOx_raw_concentration");
    ((Control) this.barInstrumentNOxRaw).Name = "barInstrumentNOxRaw";
    ((AxisSingleInstrumentBase) this.barInstrumentNOxRaw).PreferredAxisRange = new AxisRange(0.0, 500.0, (string) null);
    ((SingleInstrumentBase) this.barInstrumentNOxRaw).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barInstrumentNOxOut, "barInstrumentNOxOut");
    this.barInstrumentNOxOut.FontGroup = "ShortBar";
    ((SingleInstrumentBase) this.barInstrumentNOxOut).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentNOxOut).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS114_NOx_out_concentration");
    ((Control) this.barInstrumentNOxOut).Name = "barInstrumentNOxOut";
    ((AxisSingleInstrumentBase) this.barInstrumentNOxOut).PreferredAxisRange = new AxisRange(0.0, 500.0, (string) null);
    ((SingleInstrumentBase) this.barInstrumentNOxOut).UnitAlignment = StringAlignment.Near;
    this.labelStatus.BackColor = SystemColors.Control;
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    this.seekTimeListViewLog.FilterUserLabels = true;
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "NoxSensorDrift";
    this.seekTimeListViewLog.SelectedTime = new DateTime?();
    this.seekTimeListViewLog.ShowChannelLabels = false;
    this.seekTimeListViewLog.ShowCommunicationsState = false;
    this.seekTimeListViewLog.ShowControlPanel = false;
    this.seekTimeListViewLog.ShowDeviceColumn = false;
    this.seekTimeListViewLog.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTestControls, "tableLayoutPanelTestControls");
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.checkmarkStatus, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.labelStatus, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.buttonStart, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.tableLayoutPanelPreReqs, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.buttonStop, 2, 1);
    ((Control) this.tableLayoutPanelTestControls).Name = "tableLayoutPanelTestControls";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.tableLayoutPanelTestControls, 3);
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    this.buttonStop.Click += new EventHandler(this.buttonStop_Click);
    componentResourceManager.ApplyResources((object) this.barInstrument1, "barInstrument1");
    this.barInstrument1.FontGroup = "ShortBar";
    ((SingleInstrumentBase) this.barInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS036_SCR_Inlet_NOx_Sensor");
    ((Control) this.barInstrument1).Name = "barInstrument1";
    ((AxisSingleInstrumentBase) this.barInstrument1).PreferredAxisRange = new AxisRange(0.0, 500.0, (string) null);
    ((SingleInstrumentBase) this.barInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barInstrument2, "barInstrument2");
    this.barInstrument2.FontGroup = "horizontalBar";
    ((SingleInstrumentBase) this.barInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS035_SCR_Outlet_NOx_Sensor");
    ((Control) this.barInstrument2).Name = "barInstrument2";
    ((AxisSingleInstrumentBase) this.barInstrument2).PreferredAxisRange = new AxisRange(0.0, 500.0, (string) null);
    ((SingleInstrumentBase) this.barInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.seekTimeListViewLog, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelTestControls, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanel1, 0, 5);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanelNoxHours, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanelThermometers, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.barInstrument2, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.barInstrument1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentNOxDewpointOutlet, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.barInstrumentNOxRaw, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentNOxDewpointInlet, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.barInstrumentNOxOut, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanelDifferenceInstrument, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelNoxHours, "tableLayoutPanelNoxHours");
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.tableLayoutPanelNoxHours, 2);
    ((TableLayoutPanel) this.tableLayoutPanelNoxHours).Controls.Add((Control) this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelNoxHours).Controls.Add((Control) this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime, 0, 0);
    ((Control) this.tableLayoutPanelNoxHours).Name = "tableLayoutPanelNoxHours";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime, "digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime");
    this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime.FontGroup = "small";
    ((SingleInstrumentBase) this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).Instrument = new Qualifier((QualifierTypes) 4, "ACM301T", "e2p_nox_out_dia_sens_runtime");
    ((Control) this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).Name = "digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime";
    ((SingleInstrumentBase) this.digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime, "digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime");
    this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime.FontGroup = "small";
    ((SingleInstrumentBase) this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).Instrument = new Qualifier((QualifierTypes) 4, "ACM301T", "e2p_nox_raw_dia_sens_runtime");
    ((Control) this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).Name = "digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime";
    ((SingleInstrumentBase) this.digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelThermometers, "tableLayoutPanelThermometers");
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.barInstrument4, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.barInstrument7, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.barInstrument8, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelThermometers).Controls.Add((Control) this.barInstrument9, 2, 0);
    ((Control) this.tableLayoutPanelThermometers).Name = "tableLayoutPanelThermometers";
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetRowSpan((Control) this.tableLayoutPanelThermometers, 5);
    this.barInstrument4.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument4.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument4, "barInstrument4");
    this.barInstrument4.FontGroup = "ThermometerBar";
    ((SingleInstrumentBase) this.barInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS019_SCR_Outlet_Temperature");
    ((Control) this.barInstrument4).Name = "barInstrument4";
    ((SingleInstrumentBase) this.barInstrument4).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument4).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument4).UnitAlignment = StringAlignment.Near;
    this.barInstrument7.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument7.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument7, "barInstrument7");
    this.barInstrument7.FontGroup = "ThermometerBar";
    ((SingleInstrumentBase) this.barInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS007_DOC_Inlet_Temperature");
    ((Control) this.barInstrument7).Name = "barInstrument7";
    ((SingleInstrumentBase) this.barInstrument7).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument7).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument7).UnitAlignment = StringAlignment.Near;
    this.barInstrument8.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument8.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument8, "barInstrument8");
    this.barInstrument8.FontGroup = "ThermometerBar";
    ((SingleInstrumentBase) this.barInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS008_DOC_Outlet_Temperature");
    ((Control) this.barInstrument8).Name = "barInstrument8";
    ((AxisSingleInstrumentBase) this.barInstrument8).PreferredAxisRange = new AxisRange(-250.0, 1000.0, (string) null);
    ((SingleInstrumentBase) this.barInstrument8).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument8).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument8).UnitAlignment = StringAlignment.Near;
    this.barInstrument9.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument9.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument9, "barInstrument9");
    this.barInstrument9.FontGroup = "ThermometerBar";
    ((SingleInstrumentBase) this.barInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS009_DPF_Outlet_Temperature");
    ((Control) this.barInstrument9).Name = "barInstrument9";
    ((SingleInstrumentBase) this.barInstrument9).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument9).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument9).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNOxDewpointOutlet, "digitalReadoutInstrumentNOxDewpointOutlet");
    this.digitalReadoutInstrumentNOxDewpointOutlet.FontGroup = "small";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointOutlet).FreezeValue = false;
    this.digitalReadoutInstrumentNOxDewpointOutlet.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentNOxDewpointOutlet.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentNOxDewpointOutlet.Gradient.Initialize((ValueState) 0, 1);
    this.digitalReadoutInstrumentNOxDewpointOutlet.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointOutlet).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet");
    ((Control) this.digitalReadoutInstrumentNOxDewpointOutlet).Name = "digitalReadoutInstrumentNOxDewpointOutlet";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointOutlet).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNOxDewpointOutlet).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelDifferenceInstrument, "tableLayoutPanelDifferenceInstrument");
    ((TableLayoutPanel) this.tableLayoutPanelDifferenceInstrument).Controls.Add((Control) this.labelInstrumentDifferenceTitle, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelDifferenceInstrument).Controls.Add((Control) this.scalingLabelInstrumentDifference, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelDifferenceInstrument).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanelDifferenceInstrument).Name = "tableLayoutPanelDifferenceInstrument";
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetRowSpan((Control) this.tableLayoutPanelDifferenceInstrument, 2);
    this.labelInstrumentDifferenceTitle.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelInstrumentDifferenceTitle, "labelInstrumentDifferenceTitle");
    ((Control) this.labelInstrumentDifferenceTitle).Name = "labelInstrumentDifferenceTitle";
    this.labelInstrumentDifferenceTitle.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelInstrumentDifference.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelInstrumentDifference, "scalingLabelInstrumentDifference");
    this.scalingLabelInstrumentDifference.FontGroup = (string) null;
    this.scalingLabelInstrumentDifference.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelInstrumentDifference).Name = "scalingLabelInstrumentDifference";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.scalingLabelTestResult, 0, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.scalingLabelTestResult.Alignment = StringAlignment.Center;
    componentResourceManager.ApplyResources((object) this.scalingLabelTestResult, "scalingLabelTestResult");
    this.scalingLabelTestResult.FontGroup = (string) null;
    this.scalingLabelTestResult.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelTestResult).Name = "scalingLabelTestResult";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_NOx_Sensor_Verification");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelPreReqs).ResumeLayout(false);
    ((Control) this.tableLayoutPanelTestControls).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this.tableLayoutPanelNoxHours).ResumeLayout(false);
    ((Control) this.tableLayoutPanelThermometers).ResumeLayout(false);
    ((Control) this.tableLayoutPanelDifferenceInstrument).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum CpcVersion
  {
    Invalid = 0,
    CPC3 = 3,
    CPC5 = 5,
  }

  private enum Reason
  {
    TestCompleted,
    FailedServiceExecute,
    Closing,
    Disconnected,
    Canceled,
    DewpointSensorsNotOn,
    NOxSensorDataInvalid,
    TestConditionsNotMet,
  }

  private class NOxSensorDriftTest
  {
    private UserPanel ParentUserPanel;
    private string ErrorMessage = string.Empty;
    private UserPanel.NOxSensorDriftTest.TestStep CurrentTestStep = UserPanel.NOxSensorDriftTest.TestStep.Stopped;
    public string TestStepMessage = string.Empty;
    public Timer dosingQuantityCheckTimer;
    private DateTime LastSampleTime;
    private int DpfRegenCount = 0;
    private UserPanel.NOxSensorDriftTest.TestActivityStatus testActiveStatus = UserPanel.NOxSensorDriftTest.TestActivityStatus.TestInactive;

    public NOxSensorDriftTest(UserPanel userPanel)
    {
      this.ParentUserPanel = userPanel;
      this.dosingQuantityCheckTimer = new Timer();
      this.dosingQuantityCheckTimer.Interval = (int) TimeSpan.FromMinutes(4.0).TotalMilliseconds;
    }

    private NOxSensorDriftTest()
    {
    }

    public UserPanel.NOxSensorDriftTest.TestActivityStatus TestActiveStatus
    {
      get => this.testActiveStatus;
      private set => this.testActiveStatus = value;
    }

    private InstrumentValue GetRecentInstrumentValue(
      InstrumentValueCollection instrumentValues,
      DateTime sampleTime)
    {
      InstrumentValue recentInstrumentValue = (InstrumentValue) null;
      InstrumentValue instrumentValue = instrumentValues.First<InstrumentValue>();
      if (instrumentValue != null && instrumentValue.FirstSampleTime <= sampleTime)
        recentInstrumentValue = instrumentValues.GetCurrentAtTime(sampleTime) ?? instrumentValues.Current;
      return recentInstrumentValue;
    }

    public bool TheSensorsWereReady(
      Channel channel,
      string dewPointInletEnabledQualifier,
      string dewPointOutletEnabledQualifier,
      DateTime referenceTime)
    {
      DateTime sampleTime = referenceTime - TimeSpan.FromMinutes(8.0);
      if (channel != null)
      {
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Checking_DewPoint_Sensors_At, (object) sampleTime));
        Instrument instrument1 = channel.Instruments[dewPointInletEnabledQualifier];
        Instrument instrument2 = channel.Instruments[dewPointOutletEnabledQualifier];
        if (instrument1 != (Instrument) null && instrument2 != (Instrument) null)
        {
          InstrumentValueCollection instrumentValues1 = instrument1.InstrumentValues;
          InstrumentValueCollection instrumentValues2 = instrument2.InstrumentValues;
          if (instrumentValues1 != null && instrumentValues2 != null)
          {
            InstrumentValue recentInstrumentValue1 = this.GetRecentInstrumentValue(instrumentValues1, sampleTime);
            InstrumentValue recentInstrumentValue2 = this.GetRecentInstrumentValue(instrumentValues2, sampleTime);
            if (recentInstrumentValue1 != null && recentInstrumentValue2 != null)
            {
              if (recentInstrumentValue1.Value != null && recentInstrumentValue2.Value != null)
              {
                double num1 = Convert.ToDouble(recentInstrumentValue1.Value);
                double num2 = Convert.ToDouble(recentInstrumentValue2.Value);
                this.ReportResult(num1 > 0.0 ? Resources.Message_DewPoint_Inlet_Sensor_On : Resources.Message_DewPoint_Inlet_Sensor_Off);
                this.ReportResult(num2 > 0.0 ? Resources.Message_DewPoint_Outlet_Sensor_On : Resources.Message_DewPoint_Outlet_Sensor_Off);
                return num1 > 0.0 && num2 > 0.0;
              }
            }
            else
            {
              this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_The_Instrument_Had_Value, (object) dewPointInletEnabledQualifier, recentInstrumentValue1 != null ? recentInstrumentValue1.Value : (object) Resources.Text_Null, (object) sampleTime));
              this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_The_Instrument_Had_Value, (object) dewPointOutletEnabledQualifier, recentInstrumentValue2 != null ? recentInstrumentValue2.Value : (object) Resources.Text_Null, (object) sampleTime));
            }
          }
        }
      }
      return false;
    }

    public double InstrumentValueAverage(
      Channel channel,
      string instrumentQualifier,
      DateTime firstSampleTime,
      DateTime lastSampleTime)
    {
      double num1 = 0.0;
      int num2 = 0;
      double num3 = double.NaN;
      if (channel != null)
      {
        Instrument instrument = channel.Instruments[instrumentQualifier];
        if (instrument != (Instrument) null)
        {
          InstrumentValueCollection instrumentValues = instrument.InstrumentValues;
          if (instrumentValues != null)
          {
            InstrumentValue recentInstrumentValue1 = this.GetRecentInstrumentValue(instrumentValues, firstSampleTime);
            InstrumentValue recentInstrumentValue2 = this.GetRecentInstrumentValue(instrumentValues, lastSampleTime);
            if (recentInstrumentValue1 != null && recentInstrumentValue2 != null)
            {
              for (int itemIndex = recentInstrumentValue1.ItemIndex; itemIndex <= recentInstrumentValue2.ItemIndex; ++itemIndex)
              {
                num1 += Convert.ToDouble(instrumentValues[itemIndex].Value, (IFormatProvider) CultureInfo.InvariantCulture) * (double) instrumentValues[itemIndex].ItemSampleCount;
                num2 += instrumentValues[itemIndex].ItemSampleCount;
              }
              if (num2 > 0)
                num3 = num1 / (double) num2;
            }
          }
        }
      }
      return num3;
    }

    private void CalculateAndDisplayResults(DateTime lastSampleTime)
    {
      DateTime firstSampleTime = lastSampleTime - TimeSpan.FromMinutes(3.0);
      double d1 = this.InstrumentValueAverage(this.ParentUserPanel.acm, "DT_AS111_NOx_raw_concentration", firstSampleTime, lastSampleTime);
      double d2 = this.InstrumentValueAverage(this.ParentUserPanel.acm, "DT_AS114_NOx_out_concentration", firstSampleTime, lastSampleTime);
      if (!double.IsNaN(d1) && !double.IsNaN(d2))
      {
        if (d1 != 0.0 && d2 != 0.0)
        {
          double num = Math.Abs(d1 - d2);
          bool flag = num < 50.0;
          ((Control) this.ParentUserPanel.scalingLabelInstrumentDifference).Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0:f3}", (object) num);
          this.ParentUserPanel.DisplayResultStatus(flag ? (ValueState) 1 : (ValueState) 3);
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Results_Average_Sensor_Values, (object) d1, (object) d2));
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Results_Sensor_Value_Difference, (object) num));
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, flag ? Resources.Message_Results_NOx_Sensor_Values_Are_Consistent : Resources.Message_Results_NOx_Sensor_Values_Are_Not_Consistent));
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, flag ? Resources.Message_Test_Result_Passed : Resources.Message_Test_Result_Failed));
          ((Control) this.ParentUserPanel.scalingLabelTestResult).Text = flag ? Resources.Message_Test_Result_Passed : Resources.Message_Test_Result_Failed;
          this.ParentUserPanel.scalingLabelTestResult.RepresentedState = flag ? (ValueState) 1 : (ValueState) 3;
          this.ParentUserPanel.adrReturnValue = flag;
          UserPanel parentUserPanel = this.ParentUserPanel;
          parentUserPanel.adrReturnMessage = parentUserPanel.adrReturnMessage + string.Format((IFormatProvider) CultureInfo.CurrentCulture, flag ? Resources.Message_Results_NOx_Sensor_Values_Are_Consistent : Resources.Message_Results_NOx_Sensor_Values_Are_Not_Consistent) + Environment.NewLine;
          this.ParentUserPanel.UpdateTestReadyStatus();
          this.AdvanceTestStep();
          this.TestMain();
        }
        else
        {
          this.ReportResult(Resources.Error_NOxSensorAverageZero);
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Results_Average_Sensor_Values, (object) d1, (object) d2));
          this.EndTest(UserPanel.Reason.NOxSensorDataInvalid);
        }
      }
      else
      {
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Results_Average_Sensor_Values, (object) d1, (object) d2));
        this.EndTest(UserPanel.Reason.NOxSensorDataInvalid);
      }
    }

    private void DPFRegenEnded()
    {
      this.LastSampleTime = DateTime.Now - TimeSpan.FromSeconds(5.0);
      if (this.TheSensorsWereReady(this.ParentUserPanel.acm, "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet", this.LastSampleTime))
      {
        this.ReportResult(Resources.Message_The_Sensors_Were_Ready_To_be_Compared);
        this.AdvanceTestStep();
        this.TestMain();
      }
      else
      {
        this.ReportResult(Resources.Error_The_DPF_Regen_Ended_Before_The_Sensors_Were_Ready);
        if (this.DpfRegenCount < 1)
        {
          this.ReportResult(Resources.MessageRunningRegenAgain);
          ++this.DpfRegenCount;
          this.ReportResult(Resources.Message_Restarting_The_DPF_Regen);
          this.CurrentTestStep = UserPanel.NOxSensorDriftTest.TestStep.StartDPFRegen;
          this.StartDPFRegen();
        }
        else
          this.EndTest(UserPanel.Reason.DewpointSensorsNotOn);
      }
    }

    private void DPFRegenStarted()
    {
      this.CurrentTestStep = UserPanel.NOxSensorDriftTest.TestStep.WaitForRegenToComplete;
      this.TestMain();
    }

    public void StartTest()
    {
      this.DpfRegenCount = 0;
      this.CurrentTestStep = UserPanel.NOxSensorDriftTest.TestStep.StartTest;
      this.TestMain();
    }

    private void ReportResult(string text)
    {
      this.TestStepMessage = text;
      this.ParentUserPanel.LabelLog(this.ParentUserPanel.seekTimeListViewLog.RequiredUserLabelPrefix, text);
    }

    private static object GetInstrumentCurrentValue(Instrument instrument)
    {
      object instrumentCurrentValue = (object) null;
      if (instrument != (Instrument) null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
        instrumentCurrentValue = instrument.InstrumentValues.Current.Value;
      return instrumentCurrentValue;
    }

    public void digitalReadoutInstrumentDPFRegenState_DataChanged(object sender, EventArgs e)
    {
      this.DPFStatusChanged();
    }

    private void DPFStatusChanged()
    {
      if (this.TestActiveStatus != UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive)
        return;
      DataItem dataItem = ((SingleInstrumentBase) this.ParentUserPanel.digitalReadoutInstrumentDPFRegenState).DataItem;
      if (dataItem != null)
      {
        object instrumentCurrentValue = UserPanel.NOxSensorDriftTest.GetInstrumentCurrentValue(this.ParentUserPanel.acm.Instruments["DT_AS064_DPF_Regen_State"]);
        if (this.CurrentTestStep == UserPanel.NOxSensorDriftTest.TestStep.WaitForRegenToComplete && (instrumentCurrentValue == (object) dataItem.Choices.GetItemFromRawValue((object) 0) || instrumentCurrentValue == (object) dataItem.Choices.GetItemFromRawValue((object) 2)))
          this.DPFRegenEnded();
        else if (this.CurrentTestStep < UserPanel.NOxSensorDriftTest.TestStep.WaitForRegenToComplete && (instrumentCurrentValue == (object) dataItem.Choices.GetItemFromRawValue((object) 4) || instrumentCurrentValue == (object) dataItem.Choices.GetItemFromRawValue((object) 8) || instrumentCurrentValue == (object) dataItem.Choices.GetItemFromRawValue((object) 16 /*0x10*/) || instrumentCurrentValue == (object) dataItem.Choices.GetItemFromRawValue((object) 32 /*0x20*/)))
          this.DPFRegenStarted();
      }
    }

    internal void NOxDewpointSensor_RepresentedStateChanged(object sender, EventArgs e)
    {
      if (this.TestActiveStatus != UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive)
        return;
      this.ReportResult(string.Format(this.ParentUserPanel.digitalReadoutInstrumentNOxDewpointInlet.RepresentedState == 1 ? Resources.Message_NOx_Inlet_Sensor_Is_Ready : Resources.Message_NOx_Inlet_Sensor_Is_Not_Ready));
      this.ReportResult(string.Format(this.ParentUserPanel.digitalReadoutInstrumentNOxDewpointOutlet.RepresentedState == 1 ? Resources.Message_NOx_Outlet_Sensor_Is_Ready : Resources.Message_NOx_Outlet_Sensor_Is_Not_Ready));
      this.ReportResult(Resources.Message_Waiting_For_Regen_To_Complete);
    }

    private void SetDEFValve()
    {
      if (this.ParentUserPanel.acm == null || !this.ParentUserPanel.acm.Online)
        return;
      this.ErrorMessage = Resources.Error_Failed_to_set_the_DEF_Valve;
      Service service = this.ParentUserPanel.GetService("ACM301T", "RT_SCR_Dosing_Quantity_Check_Start_Status");
      if (service != (Service) null)
      {
        service.InputValues["Desired_Dosing_Quantity"].Value = (object) 0.2;
        service.InputValues["Operation_Time"].Value = (object) (int) byte.MaxValue;
        service.InputValues["Deal_with_interrupted_communication"].Value = (object) service.InputValues["Deal_with_interrupted_communication"].Choices.GetItemFromRawValue((object) 0);
        service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.defValveService_ServiceCompleteEvent);
        service.Execute(false);
      }
      else
      {
        this.ReportResult(this.ErrorMessage);
        this.EndTest(UserPanel.Reason.FailedServiceExecute);
      }
    }

    private void defValveService_ServiceCompleteEvent(object sender, ResultEventArgs e)
    {
      Service service = sender as Service;
      if (service != (Service) null)
        service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.defValveService_ServiceCompleteEvent);
      if (e.Succeeded)
      {
        this.dosingQuantityCheckTimer.Tick += new EventHandler(this.dosingQuantityCheckTimer_Tick);
        this.dosingQuantityCheckTimer.Start();
      }
      else
      {
        this.ReportResult(this.ErrorMessage);
        this.EndTest(UserPanel.Reason.FailedServiceExecute);
      }
    }

    private void dosingQuantityCheckTimer_Tick(object sender, EventArgs e)
    {
      this.StopDosingQuantityCheckTimer();
      this.ResetDEFValve();
      this.SetDEFValve();
    }

    public void StopDosingQuantityCheckTimer()
    {
      this.dosingQuantityCheckTimer.Stop();
      this.dosingQuantityCheckTimer.Tick -= new EventHandler(this.dosingQuantityCheckTimer_Tick);
    }

    private void StartDPFRegen()
    {
      if (this.ParentUserPanel.cpc == null || !this.ParentUserPanel.cpc.Online)
        return;
      this.ErrorMessage = Resources.Error_Failed_to_start_DPF_Regen;
      string name = this.ParentUserPanel.cpc.Ecu.Name;
      string str = string.Empty;
      switch (this.ParentUserPanel.ConnectedCpcVersion)
      {
        case UserPanel.CpcVersion.CPC3:
          str = "RT_RC0400_DPF_High_Idle_regeneration_Start";
          break;
        case UserPanel.CpcVersion.CPC5:
          str = "RT_DPF_High_Idle_regeneration_Start";
          break;
      }
      Service service = this.ParentUserPanel.GetService(name, str, (CustomPanel.ChannelLookupOptions) 5);
      if (service != (Service) null)
      {
        service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.StartDPFRegen_ServiceCompleteEvent);
        service.Execute(false);
      }
      else
      {
        this.ReportResult(this.ErrorMessage);
        this.EndTest(UserPanel.Reason.FailedServiceExecute);
      }
    }

    private void StartDPFRegen_ServiceCompleteEvent(object sender, ResultEventArgs e)
    {
      Service service = sender as Service;
      if (service != (Service) null)
        service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.StartDPFRegen_ServiceCompleteEvent);
      if (e.Succeeded && this.CurrentTestStep == UserPanel.NOxSensorDriftTest.TestStep.StartDPFRegen)
      {
        this.AdvanceTestStep();
        this.TestMain();
      }
      else
      {
        this.ReportResult(this.ErrorMessage);
        this.EndTest(UserPanel.Reason.FailedServiceExecute);
      }
    }

    private void ResetDEFValve()
    {
      if (this.ParentUserPanel.acm == null || !this.ParentUserPanel.acm.Online)
        return;
      Service service = this.ParentUserPanel.GetService("ACM301T", "RT_SCR_Dosing_Quantity_Check_Stop_status");
      if (service != (Service) null)
      {
        try
        {
          service.Execute(true);
        }
        catch (CaesarException ex)
        {
          this.ReportResult(Resources.Error_Failed_To_Reset_The_DEF_Valve);
          if (ex != null && !string.IsNullOrEmpty(ex.Message))
            this.ReportResult(string.Format(Resources.Error_Message_Is, (object) ex.Message));
        }
      }
    }

    private void ResetDPFRegen()
    {
      if (this.ParentUserPanel.cpc == null || !this.ParentUserPanel.cpc.Online)
        return;
      string str = string.Empty;
      string name = this.ParentUserPanel.cpc.Ecu.Name;
      switch (this.ParentUserPanel.ConnectedCpcVersion)
      {
        case UserPanel.CpcVersion.CPC3:
          str = "RT_RC0400_DPF_High_Idle_regeneration_Stop";
          break;
        case UserPanel.CpcVersion.CPC5:
          str = "RT_DPF_High_Idle_regeneration_Stop";
          break;
      }
      Service service = this.ParentUserPanel.GetService(name, str);
      if (service != (Service) null)
      {
        try
        {
          service.Execute(true);
        }
        catch (CaesarException ex)
        {
          this.ReportResult(Resources.Error_Failed_To_Stop_the_DPF_Regeneration);
          if (ex != null && !string.IsNullOrEmpty(ex.Message))
            this.ReportResult(string.Format(Resources.Error_Message_Is, (object) ex.Message));
        }
      }
      else
      {
        this.ReportResult(Resources.Error_Failed_To_Stop_the_DPF_Regeneration);
        this.ReportResult(string.Format(Resources.Error_DPF_Regen_Stop_Service_Not_Found, (object) str, (object) name));
      }
    }

    private void ResetTestStep()
    {
      this.ReportResult(Resources.Message_End_Of_Test);
      this.TestStepMessage = string.Empty;
      this.TestActiveStatus = UserPanel.NOxSensorDriftTest.TestActivityStatus.TestInactive;
      this.ParentUserPanel.UpdateTestReadyStatus();
      this.CurrentTestStep = UserPanel.NOxSensorDriftTest.TestStep.Stopped;
    }

    private static string GetReasonString(UserPanel.Reason reason)
    {
      switch (reason)
      {
        case UserPanel.Reason.TestCompleted:
          return Resources.Message_Test_Complete;
        case UserPanel.Reason.FailedServiceExecute:
          return Resources.Error_FailedServiceExecute;
        case UserPanel.Reason.Closing:
          return Resources.Error_Closing;
        case UserPanel.Reason.Disconnected:
          return Resources.Error_Disconnected;
        case UserPanel.Reason.Canceled:
          return Resources.Message_Canceled;
        case UserPanel.Reason.DewpointSensorsNotOn:
          return Resources.Error_DewpointSensorsNotOn;
        case UserPanel.Reason.NOxSensorDataInvalid:
          return Resources.Error_NOxSensorReadingsInvalid;
        case UserPanel.Reason.TestConditionsNotMet:
          return Resources.Error_Test_Not_Ready_To_Be_Run;
        default:
          return (string) null;
      }
    }

    public void EndTest(UserPanel.Reason reason)
    {
      this.TestActiveStatus = UserPanel.NOxSensorDriftTest.TestActivityStatus.TestShuttingDown;
      this.StopDosingQuantityCheckTimer();
      this.ReportResult(Resources.Message_ResettingTheDEFValve);
      this.ResetDEFValve();
      this.ReportResult(Resources.Message_Stopping_the_DPF_Regen);
      this.ResetDPFRegen();
      this.ReportResult(UserPanel.NOxSensorDriftTest.GetReasonString(reason));
      UserPanel parentUserPanel = this.ParentUserPanel;
      parentUserPanel.adrReturnMessage = parentUserPanel.adrReturnMessage + UserPanel.NOxSensorDriftTest.GetReasonString(reason) + Environment.NewLine;
      if (reason != UserPanel.Reason.TestCompleted)
      {
        ((Control) this.ParentUserPanel.scalingLabelTestResult).Text = Resources.Message_Test_Result_Test_Aborted;
        this.ParentUserPanel.scalingLabelTestResult.RepresentedState = (ValueState) 2;
      }
      this.CurrentTestStep = UserPanel.NOxSensorDriftTest.TestStep.ResetUserInterface;
      this.TestMain();
    }

    private void AdvanceTestStep()
    {
      if (this.CurrentTestStep != UserPanel.NOxSensorDriftTest.TestStep.Stopped)
        ++this.CurrentTestStep;
      else
        this.TestActiveStatus = UserPanel.NOxSensorDriftTest.TestActivityStatus.TestInactive;
    }

    private void TestMain()
    {
      switch (this.CurrentTestStep)
      {
        case UserPanel.NOxSensorDriftTest.TestStep.StartTest:
          this.ReportResult(Resources.Message_Starting_NOx_Sensor_Calibration_Drift_Test);
          this.TestActiveStatus = UserPanel.NOxSensorDriftTest.TestActivityStatus.TestActive;
          this.AdvanceTestStep();
          this.TestMain();
          break;
        case UserPanel.NOxSensorDriftTest.TestStep.SetDEFValve:
          this.ReportResult(Resources.Message_Setting_the_DEF_valve);
          this.SetDEFValve();
          this.AdvanceTestStep();
          this.TestMain();
          break;
        case UserPanel.NOxSensorDriftTest.TestStep.StartDPFRegen:
          this.ReportResult(Resources.Message_Starting_DPF_Regen);
          this.StartDPFRegen();
          break;
        case UserPanel.NOxSensorDriftTest.TestStep.WaitForRegenToComplete:
          this.ReportResult(Resources.Message_Waiting_For_Regen_To_Complete);
          break;
        case UserPanel.NOxSensorDriftTest.TestStep.CalculateAndDisplayResults:
          this.CalculateAndDisplayResults(this.LastSampleTime);
          break;
        case UserPanel.NOxSensorDriftTest.TestStep.EndTest:
          this.EndTest(UserPanel.Reason.TestCompleted);
          break;
        case UserPanel.NOxSensorDriftTest.TestStep.ResetUserInterface:
          this.ResetTestStep();
          break;
      }
    }

    private enum TestStep
    {
      Stopped,
      StartTest,
      SetDEFValve,
      StartDPFRegen,
      WaitForRegenToComplete,
      CalculateAndDisplayResults,
      EndTest,
      ResetUserInterface,
    }

    public enum TestActivityStatus
    {
      TestInactive,
      TestActive,
      TestShuttingDown,
    }
  }
}
