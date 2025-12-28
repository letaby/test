// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment.panel;

public class UserPanel : CustomPanel
{
  private SharedProcedureBase selectedProcedure;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private System.Windows.Forms.Label labelStatus;
  private Button buttonStartStop;
  private TableLayoutPanel tableLayoutPanel1;
  private BarInstrument barInstrumentProcedureProgress;
  private SharedProcedureSelection sharedProcedureSelection1;
  private DialInstrument dialInstrumentVehicleSpeed;
  private SeekTimeListView seekTimeListView1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private Checkmark checkmark1;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.SubscribeToEvents(this.sharedProcedureSelection1.SelectedProcedure);
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
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

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.dialInstrumentVehicleSpeed = new DialInstrument();
    this.labelStatus = new System.Windows.Forms.Label();
    this.checkmark1 = new Checkmark();
    this.barInstrumentProcedureProgress = new BarInstrument();
    this.buttonStartStop = new Button();
    this.seekTimeListView1 = new SeekTimeListView();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection1, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dialInstrumentVehicleSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentProcedureProgress, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStartStop, 4, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DrivingRadarAlignment"
    });
    this.dialInstrumentVehicleSpeed.AngleRange = 270.0;
    this.dialInstrumentVehicleSpeed.AngleStart = 135.0;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.dialInstrumentVehicleSpeed, 2);
    componentResourceManager.ApplyResources((object) this.dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
    this.dialInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState) 0, 3, "km/h");
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(1, -1.0, (ValueState) 1);
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
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).Instrument = new Qualifier((QualifierTypes) 1, "RDF01T", "DT_Service_Justage_Progress_service_justage_progress");
    ((Control) this.barInstrumentProcedureProgress).Name = "barInstrumentProcedureProgress";
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonStartStop, "buttonStartStop");
    this.buttonStartStop.Name = "buttonStartStop";
    this.buttonStartStop.UseCompatibleTextRendering = true;
    this.buttonStartStop.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "Detroit Assurance Driving Radar Alignment";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView1, 3);
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
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonStartStop;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
