// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment__45X_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment__45X_.panel;

public class UserPanel : CustomPanel
{
  private const string RadarEcuName = "RDF03T";
  private const string verticalPosition = "VertPos";
  private SharedProcedureBase selectedProcedure;
  private WarningManager warningManager;
  private bool closing = false;
  private Channel rdf03tChannel;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private System.Windows.Forms.Label labelStatus;
  private Button buttonStartStop;
  private TableLayoutPanel tableLayoutPanel1;
  private BarInstrument barInstrumentProcedureProgress;
  private SharedProcedureSelection sharedProcedureSelection1;
  private DialInstrument dialInstrumentVehicleSpeed;
  private SeekTimeListView seekTimeListView1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrumentVertPos;
  private Button buttonHiddenStart;
  private Checkmark checkmark1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(Resources.WarningManagerMessage, Resources.WarningManagerJobName, this.seekTimeListView1.RequiredUserLabelPrefix);
  }

  private Channel Rdf03tChannel
  {
    get => this.rdf03tChannel;
    set
    {
      if (this.rdf03tChannel == value)
        return;
      this.warningManager.Reset();
      if (this.rdf03tChannel != null)
        this.rdf03tChannel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Rdf03t_ParametersReadCompleteEvent);
      this.rdf03tChannel = value;
      if (this.rdf03tChannel != null)
        this.rdf03tChannel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Rdf03t_ParametersReadCompleteEvent);
    }
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.SubscribeToEvents(this.sharedProcedureSelection1.SelectedProcedure);
    base.OnChannelsChanged();
    this.ReadParameters();
  }

  public virtual void OnChannelsChanged()
  {
    if (!this.closing)
      this.Rdf03tChannel = this.GetChannel("RDF03T");
    base.OnChannelsChanged();
  }

  private void ReadParameters()
  {
    if (this.Rdf03tChannel == null || this.Rdf03tChannel.CommunicationsState != CommunicationsState.Online || this.Rdf03tChannel.Parameters["VertPos"] == null || this.Rdf03tChannel.Parameters["VertPos"].HasBeenReadFromEcu)
      return;
    this.Rdf03tChannel.Parameters.ReadGroup(this.Rdf03tChannel.Parameters["VertPos"].GroupQualifier, true, false);
  }

  private void Rdf03t_ParametersReadCompleteEvent(object sender, ResultEventArgs result)
  {
    this.ReadParameters();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.closing = true;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  private bool CanClose => !this.sharedProcedureSelection1.AnyProcedureInProgress;

  private void SubscribeToEvents(SharedProcedureBase procedure)
  {
    if (procedure == this.selectedProcedure)
      return;
    if (this.selectedProcedure != null)
    {
      this.selectedProcedure.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.OnProcedureStart);
      this.selectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.OnProcedureStop);
    }
    this.selectedProcedure = procedure;
    if (this.selectedProcedure != null)
    {
      this.selectedProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.OnProcedureStart);
      this.selectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.OnProcedureStop);
    }
  }

  private void OnProcedureStart(object sender, PassFailResultEventArgs e)
  {
    if (!((ResultEventArgs) e).Succeeded)
      return;
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.ModifyState(1, (ValueState) 3);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.ModifyState(3, (ValueState) 1);
    ((Control) this.dialInstrumentVehicleSpeed).Invalidate();
  }

  private void OnProcedureStop(object sender, PassFailResultEventArgs e)
  {
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.ModifyState(1, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.ModifyState(3, (ValueState) 3);
    ((Control) this.dialInstrumentVehicleSpeed).Invalidate();
  }

  private void buttonStartStop_Click(object sender, EventArgs e)
  {
    if (this.sharedProcedureSelection1.SelectedProcedure.CanStart)
    {
      if (!this.warningManager.RequestContinue())
        return;
      this.sharedProcedureIntegrationComponent1.StartStopButton.PerformClick();
    }
    else
    {
      this.sharedProcedureIntegrationComponent1.StartStopButton.PerformClick();
      this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, Resources.Message_Cancelled);
    }
  }

  private void buttonHiddenStart_EnabledChanged(object sender, EventArgs e)
  {
    this.buttonStartStop.Enabled = this.buttonHiddenStart.Enabled;
  }

  private void buttonHiddenStart_TextChanged(object sender, EventArgs e)
  {
    this.buttonStartStop.Text = this.buttonHiddenStart.Text;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.dialInstrumentVehicleSpeed = new DialInstrument();
    this.labelStatus = new System.Windows.Forms.Label();
    this.checkmark1 = new Checkmark();
    this.barInstrumentProcedureProgress = new BarInstrument();
    this.buttonStartStop = new Button();
    this.seekTimeListView1 = new SeekTimeListView();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVertPos = new DigitalReadoutInstrument();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.buttonHiddenStart = new Button();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dialInstrumentVehicleSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentProcedureProgress, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStartStop, 5, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentVertPos, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection1, 4, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonHiddenStart, 3, 4);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.dialInstrumentVehicleSpeed.AngleRange = 270.0;
    this.dialInstrumentVehicleSpeed.AngleStart = 135.0;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.dialInstrumentVehicleSpeed, 2);
    componentResourceManager.ApplyResources((object) this.dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
    this.dialInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState) 0, 3, "km/h");
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(2, 1.0, (ValueState) 3);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(3, 40.0, (ValueState) 3);
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.dialInstrumentVehicleSpeed).Name = "dialInstrumentVehicleSpeed";
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).PreferredAxisRange = new AxisRange(0.0, 90.0, "mph");
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrumentProcedureProgress, 2);
    componentResourceManager.ApplyResources((object) this.barInstrumentProcedureProgress, "barInstrumentProcedureProgress");
    this.barInstrumentProcedureProgress.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.Initialize((ValueState) 0, 2, "%");
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.Modify(2, 101.0, (ValueState) 0);
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "RDF03T", "RT_Service_Alignment_Request_Results_Progress");
    ((Control) this.barInstrumentProcedureProgress).Name = "barInstrumentProcedureProgress";
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).ShowValueReadout = false;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonStartStop, "buttonStartStop");
    this.buttonStartStop.Name = "buttonStartStop";
    this.buttonStartStop.UseCompatibleTextRendering = true;
    this.buttonStartStop.UseVisualStyleBackColor = true;
    this.buttonStartStop.Click += new EventHandler(this.buttonStartStop_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "Detroit Assurance Driving Radar Alignment 45X";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView1, 4);
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "HH:mm:ss.f";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "J1939-0", "DT_70");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentVertPos, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVertPos, "digitalReadoutInstrumentVertPos");
    this.digitalReadoutInstrumentVertPos.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVertPos).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVertPos).Instrument = new Qualifier((QualifierTypes) 4, "RDF03T", "VertPos");
    ((Control) this.digitalReadoutInstrumentVertPos).Name = "digitalReadoutInstrumentVertPos";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVertPos).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DrivingRadarAlignment_45X"
    });
    componentResourceManager.ApplyResources((object) this.buttonHiddenStart, "buttonHiddenStart");
    this.buttonHiddenStart.Name = "buttonHiddenStart";
    this.buttonHiddenStart.UseCompatibleTextRendering = true;
    this.buttonHiddenStart.UseVisualStyleBackColor = true;
    this.buttonHiddenStart.EnabledChanged += new EventHandler(this.buttonHiddenStart_EnabledChanged);
    this.buttonHiddenStart.TextChanged += new EventHandler(this.buttonHiddenStart_TextChanged);
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonHiddenStart;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
