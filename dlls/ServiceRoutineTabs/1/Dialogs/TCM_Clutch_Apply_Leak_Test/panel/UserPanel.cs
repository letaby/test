// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clutch_Apply_Leak_Test.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clutch_Apply_Leak_Test.panel;

public class UserPanel : CustomPanel
{
  private const string DesiredClutchValueStartService = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start";
  private const string DesiredClutchValueRequestService = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status";
  private const string DesiredClutchValueStopService = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop";
  private const string ActualClutchPositionInstrumentQualifier = "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung";
  private const string PressureInstrumentQualifier = "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck";
  private UserPanel.State state = UserPanel.State.None;
  private UserPanel.State nextState = UserPanel.State.None;
  private Channel tcm = (Channel) null;
  private Channel mcm = (Channel) null;
  private Instrument actualClutchPositionInstrument = (Instrument) null;
  private Instrument pressureInstrument = (Instrument) null;
  private int intialClutchPosition;
  private int finalClutchPosition;
  private int thresholdLeakPosition = 10;
  private int serviceReturnCode;
  private string faultMessage;
  private Timer waitTimer;
  private TableLayoutPanel tableLayoutPanel1;
  private SeekTimeListView seekTimeListView;
  private DigitalReadoutInstrument digitalReadoutVehicleCheckStatus;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private TableLayoutPanel tableLayoutPanel2;
  private Checkmark checkmarkCanStart;
  private Button buttonStart;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelCanStart;
  private Button buttonStop;
  private DigitalReadoutInstrument digitalReadoutEngineState;
  private DigitalReadoutInstrument digitalReadoutInstrument3;

  public UserPanel()
  {
    this.InitializeComponent();
    this.waitTimer = new Timer();
    this.buttonStart.Click += new EventHandler(this.OnStartButtonClick);
    this.buttonStop.Click += new EventHandler(this.OnStopButtonClick);
    this.digitalReadoutVehicleCheckStatus.RepresentedStateChanged += new EventHandler(this.OnPreconditionStateChanged);
    this.digitalReadoutEngineState.RepresentedStateChanged += new EventHandler(this.OnPreconditionStateChanged);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.digitalReadoutVehicleCheckStatus.RepresentedStateChanged -= new EventHandler(this.OnPreconditionStateChanged);
    this.digitalReadoutEngineState.RepresentedStateChanged -= new EventHandler(this.OnPreconditionStateChanged);
    this.SetTCM((Channel) null);
    this.SetMCM((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetTCM(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 7));
    this.SetMCM(this.GetChannel("MCM21T"));
    this.UpdateUserInterface();
  }

  private void SetMCM(Channel mcm)
  {
    if (this.mcm != mcm && this.mcm != null)
      this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.mcm = mcm;
    if (this.mcm == null)
      return;
    this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
  }

  private void SetTCM(Channel tcm)
  {
    if (this.tcm != tcm && this.tcm != null)
      this.tcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    if (this.pressureInstrument != (Instrument) null)
    {
      this.pressureInstrument.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnPressureInstrumentValueChanged);
      this.pressureInstrument = (Instrument) null;
    }
    this.tcm = tcm;
    if (this.tcm == null)
      return;
    this.tcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.pressureInstrument = this.tcm.Instruments["DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"];
    this.actualClutchPositionInstrument = this.tcm.Instruments["DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung"];
    if (this.pressureInstrument != (Instrument) null)
      this.pressureInstrument.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnPressureInstrumentValueChanged);
    foreach (SingleInstrumentBase singleInstrumentBase1 in CustomPanel.GetControlsOfType(((Control) this).Controls, typeof (SingleInstrumentBase)))
    {
      Qualifier instrument = singleInstrumentBase1.Instrument;
      Ecu ecuByName = SapiManager.GetEcuByName(((Qualifier) ref instrument).Ecu);
      if (ecuByName != null && ecuByName.Identifier == tcm.Ecu.Identifier && ecuByName.Name != tcm.Ecu.Name)
      {
        SingleInstrumentBase singleInstrumentBase2 = singleInstrumentBase1;
        instrument = singleInstrumentBase1.Instrument;
        QualifierTypes type = ((Qualifier) ref instrument).Type;
        string name1 = tcm.Ecu.Name;
        instrument = singleInstrumentBase1.Instrument;
        string name2 = ((Qualifier) ref instrument).Name;
        Qualifier qualifier1 = new Qualifier(type, name1, name2);
        singleInstrumentBase2.Instrument = qualifier1;
        if (singleInstrumentBase1.DataItem == null)
        {
          QualifierTypes qualifierTypes1 = (QualifierTypes) 0;
          instrument = singleInstrumentBase1.Instrument;
          if (((Qualifier) ref instrument).Type == 1)
          {
            qualifierTypes1 = (QualifierTypes) 8;
          }
          else
          {
            instrument = singleInstrumentBase1.Instrument;
            if (((Qualifier) ref instrument).Type == 8)
              qualifierTypes1 = (QualifierTypes) 1;
          }
          SingleInstrumentBase singleInstrumentBase3 = singleInstrumentBase1;
          QualifierTypes qualifierTypes2 = qualifierTypes1;
          string name3 = tcm.Ecu.Name;
          instrument = singleInstrumentBase1.Instrument;
          string name4 = ((Qualifier) ref instrument).Name;
          Qualifier qualifier2 = new Qualifier(qualifierTypes2, name3, name4);
          singleInstrumentBase3.Instrument = qualifier2;
        }
      }
    }
  }

  private void OnStartButtonClick(object sender, EventArgs e)
  {
    this.nextState = UserPanel.State.ExecutingDesiredClutchValue;
    this.Advance();
  }

  private void OnStopButtonClick(object sender, EventArgs e)
  {
    this.nextState = UserPanel.State.StoppingDesiredClutchValue;
    this.Advance();
  }

  private void OnIntialClutchSampleRecordTimerTick(object sender, EventArgs e)
  {
    this.waitTimer.Tick -= new EventHandler(this.OnIntialClutchSampleRecordTimerTick);
    this.waitTimer.Stop();
    this.intialClutchPosition = this.GetActualClutchPosition(this.actualClutchPositionInstrument);
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_IntialClutchPostionObserved0, (object) this.intialClutchPosition));
    this.Advance();
  }

  private void OnFinalClutchSampleRecordTimerTick(object sender, EventArgs e)
  {
    this.waitTimer.Tick -= new EventHandler(this.OnFinalClutchSampleRecordTimerTick);
    this.waitTimer.Stop();
    this.finalClutchPosition = this.GetActualClutchPosition(this.actualClutchPositionInstrument);
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_FinalClutchPostionObserved0, (object) this.finalClutchPosition));
    if (this.finalClutchPosition - this.intialClutchPosition > this.thresholdLeakPosition)
      this.AddLogLabel(Resources.Message_LeakFoundTestFailed);
    else
      this.AddLogLabel(Resources.Message_NoLeakFoundTestPassed);
    this.Advance();
  }

  private void OnRequestClutchValueStatusTick(object sender, EventArgs e)
  {
    this.waitTimer.Tick -= new EventHandler(this.OnRequestClutchValueStatusTick);
    this.waitTimer.Stop();
    this.Advance();
  }

  private void UpdateUserInterface()
  {
    this.buttonStart.Enabled = this.CanStart;
    this.buttonStop.Enabled = this.CanStop;
    this.checkmarkCanStart.CheckState = this.CanStart || this.TestRunning ? CheckState.Checked : CheckState.Unchecked;
    string str = Resources.Message_TestCanStart;
    if (!this.buttonStart.Enabled)
    {
      if (this.TestRunning)
        str = Resources.Message_TestInProgress;
      else if (this.Busy)
        str = Resources.Message_CannotStartTheTestAsTheDeviceIsBusy;
      else if (!this.Online)
        str = Resources.Message_CannotStartTheTestAsTheDeviceIsNotOnline;
      else if (!this.EngineStateOk)
        str = Resources.Message_CannotStartTheTestAsTheEngineIsRunningStopTheEngine;
      else if (!this.VehicleCheckStatusOk)
        str = Resources.Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionIsInNeutral;
      else if (!this.PressureOk)
        str = Resources.Message_CannotStartTheTestAsAirSupplyPressureIsBeGreaterThan90Psi;
    }
    ((Control) this.labelCanStart).Text = str;
  }

  private void Advance()
  {
    this.state = this.nextState;
    switch (this.state)
    {
      case UserPanel.State.None:
        this.UpdateUserInterface();
        break;
      case UserPanel.State.ExecutingDesiredClutchValue:
        this.AddLogLabel(Resources.Message_StartingClutchApplyLeakTest);
        this.nextState = UserPanel.State.RequestingDesiredClutchValueStatus;
        this.AddLogLabel(Resources.Message_RequestingDesiredValueRequirementClutchStartService);
        if (!this.ExecuteService("RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start", new object[1]
        {
          (object) 100
        }))
        {
          this.faultMessage = Resources.Message_ErrorExecutingDesiredValueRequirementClutchStartService;
          this.nextState = UserPanel.State.Fault;
          this.Advance();
          goto case UserPanel.State.None;
        }
        goto case UserPanel.State.None;
      case UserPanel.State.RequestingDesiredClutchValueStatus:
        this.nextState = UserPanel.State.WaitingForDesiredClutchValueStatus;
        if (!this.ExecuteService("RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status", (object[]) null))
        {
          this.faultMessage = Resources.Message_ErrorExecutingDesiredValueRequirementClutchRequestService;
          this.nextState = UserPanel.State.Fault;
          this.Advance();
          goto case UserPanel.State.None;
        }
        goto case UserPanel.State.None;
      case UserPanel.State.WaitingForDesiredClutchValueStatus:
        if (this.serviceReturnCode == 0)
        {
          this.waitTimer.Interval = 5000;
          this.waitTimer.Tick += new EventHandler(this.OnIntialClutchSampleRecordTimerTick);
          this.waitTimer.Start();
          this.nextState = UserPanel.State.RecordSampleClutchPositionStatus;
          goto case UserPanel.State.None;
        }
        if (this.serviceReturnCode == 1)
        {
          this.nextState = UserPanel.State.RequestingDesiredClutchValueStatus;
          this.waitTimer.Interval = 1000;
          this.waitTimer.Tick += new EventHandler(this.OnRequestClutchValueStatusTick);
          this.waitTimer.Start();
          goto case UserPanel.State.None;
        }
        this.nextState = UserPanel.State.StoppingDesiredClutchValue;
        this.Advance();
        goto case UserPanel.State.None;
      case UserPanel.State.RecordSampleClutchPositionStatus:
        this.AddLogLabel(Resources.Message_WaitingToRecordSampleClutchPositionValue);
        this.waitTimer.Interval = 60000;
        this.waitTimer.Tick += new EventHandler(this.OnFinalClutchSampleRecordTimerTick);
        this.waitTimer.Start();
        this.nextState = UserPanel.State.StoppingDesiredClutchValue;
        goto case UserPanel.State.None;
      case UserPanel.State.StoppingDesiredClutchValue:
        this.nextState = UserPanel.State.None;
        this.AddLogLabel(Resources.Message_RequestingDesiredValueRequirementClutchStopService);
        this.ExecuteService("RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop", (object[]) null, true);
        this.Advance();
        this.AddLogLabel(Resources.Message_CompletedClutchApplyLeakTest);
        goto case UserPanel.State.None;
      case UserPanel.State.Fault:
        this.AddLogLabel(this.faultMessage);
        this.nextState = UserPanel.State.None;
        this.Advance();
        goto case UserPanel.State.None;
      default:
        throw new InvalidOperationException("Unknown state");
    }
  }

  private bool ExecuteService(string serviceQualifier, object[] args)
  {
    return this.ExecuteService(serviceQualifier, args, false);
  }

  private bool ExecuteService(string serviceQualifier, object[] args, bool ignoreResults)
  {
    bool flag = false;
    if (this.tcm != null)
    {
      Service service = this.tcm.Services[serviceQualifier];
      if (service != (Service) null)
      {
        if (args != null)
        {
          for (int index = 0; index < args.Length; ++index)
            service.InputValues[index].Value = args[index];
        }
        if (!ignoreResults)
          service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
        try
        {
          service.Execute(ignoreResults);
          flag = true;
        }
        catch (InvalidOperationException ex)
        {
        }
        catch (ArgumentException ex)
        {
        }
        catch (CaesarException ex)
        {
        }
      }
    }
    return flag;
  }

  private void OnServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
    if (!e.Succeeded)
    {
      this.nextState = UserPanel.State.Fault;
      this.faultMessage = e.Exception == null ? string.Empty : e.Exception.Message;
    }
    else if (service.OutputValues.Count > 0)
    {
      Choice choice = service.OutputValues[0].Value as Choice;
      this.serviceReturnCode = !(choice != (object) null) ? -1 : Convert.ToInt32(choice.RawValue);
    }
    this.Advance();
  }

  private int GetActualClutchPosition(Instrument instrument)
  {
    int actualClutchPosition = 0;
    if (instrument != (Instrument) null && instrument.InstrumentValues.Current != null)
      actualClutchPosition = Convert.ToInt32(instrument.InstrumentValues.Current.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    return actualClutchPosition;
  }

  private void AddLogLabel(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnPressureInstrumentValueChanged(object sender, ResultEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnPreconditionStateChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private bool TestRunning => this.state != UserPanel.State.None;

  private bool Online => this.tcm != null && this.tcm.Online && this.mcm != null && this.mcm.Online;

  private bool Busy
  {
    get
    {
      return this.Online && this.mcm.CommunicationsState != CommunicationsState.Online && this.tcm.CommunicationsState != CommunicationsState.Online;
    }
  }

  private bool CanStart
  {
    get
    {
      return !this.TestRunning && this.Online && this.EngineStateOk && this.PressureOk && this.VehicleCheckStatusOk;
    }
  }

  private bool CanStop => this.TestRunning;

  private bool EngineStateOk => this.digitalReadoutEngineState.RepresentedState != 3;

  private bool VehicleCheckStatusOk => this.digitalReadoutVehicleCheckStatus.RepresentedState == 1;

  private bool PressureOk
  {
    get
    {
      if (this.pressureInstrument != (Instrument) null && this.pressureInstrument.InstrumentValues.Current != null)
      {
        try
        {
          if (Convert.ToDouble(this.pressureInstrument.InstrumentValues.Current.Value, (IFormatProvider) CultureInfo.InvariantCulture) > 620.528)
            return true;
        }
        catch (FormatException ex)
        {
          return false;
        }
      }
      return false;
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutEngineState = new DigitalReadoutInstrument();
    this.digitalReadoutVehicleCheckStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.checkmarkCanStart = new Checkmark();
    this.buttonStart = new Button();
    this.labelCanStart = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonStop = new Button();
    this.seekTimeListView = new SeekTimeListView();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutEngineState, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutVehicleCheckStatus, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 1, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutEngineState, "digitalReadoutEngineState");
    this.digitalReadoutEngineState.FontGroup = "InstrumentValue";
    ((SingleInstrumentBase) this.digitalReadoutEngineState).FreezeValue = false;
    this.digitalReadoutEngineState.Gradient.Initialize((ValueState) 0, 8);
    this.digitalReadoutEngineState.Gradient.Modify(1, -1.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(2, 0.0, (ValueState) 1);
    this.digitalReadoutEngineState.Gradient.Modify(3, 1.0, (ValueState) 0);
    this.digitalReadoutEngineState.Gradient.Modify(4, 2.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(5, 3.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(6, 4.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(7, 5.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(8, 6.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutEngineState).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS023_Engine_State");
    ((Control) this.digitalReadoutEngineState).Name = "digitalReadoutEngineState";
    ((SingleInstrumentBase) this.digitalReadoutEngineState).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutVehicleCheckStatus, "digitalReadoutVehicleCheckStatus");
    this.digitalReadoutVehicleCheckStatus.FontGroup = "InstrumentValue";
    ((SingleInstrumentBase) this.digitalReadoutVehicleCheckStatus).FreezeValue = false;
    this.digitalReadoutVehicleCheckStatus.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutVehicleCheckStatus).Name = "digitalReadoutVehicleCheckStatus";
    ((SingleInstrumentBase) this.digitalReadoutVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "InstrumentValue";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 3, 1, "psi");
    this.digitalReadoutInstrument2.Gradient.Modify(1, 90.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "InstrumentValue";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmarkCanStart, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonStart, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelCanStart, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonStop, 3, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.checkmarkCanStart, "checkmarkCanStart");
    ((Control) this.checkmarkCanStart).Name = "checkmarkCanStart";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.labelCanStart.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelCanStart, "labelCanStart");
    ((Control) this.labelCanStart).Name = "labelCanStart";
    this.labelCanStart.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelCanStart.ShowBorder = false;
    this.labelCanStart.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "TCM Clutch Apply Leak Test";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView, 2);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss:fff";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_ClutchApplyLeakTest");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum State
  {
    None,
    ExecutingDesiredClutchValue,
    RequestingDesiredClutchValueStatus,
    WaitingForDesiredClutchValueStatus,
    RecordSampleClutchPositionStatus,
    StoppingDesiredClutchValue,
    Fault,
  }
}
