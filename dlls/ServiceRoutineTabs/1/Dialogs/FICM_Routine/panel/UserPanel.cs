// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.FICM_Routine.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.FICM_Routine.panel;

public class UserPanel : CustomPanel
{
  private bool adrResult = false;
  private string adrMessage = Resources.Message_Test_Not_Run;
  private TableLayoutPanel tableLayoutPanelMain;
  private TableLayoutPanel tableLayoutPanelBottom;
  private Checkmark checkmarkStatus;
  private Button buttonStartStop;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheck;
  private SeekTimeListView seekTimeListView;
  private Label labelStatus;
  private SharedProcedureSelection sharedProcedureSelection1;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private TextBox textBoxWarning;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.sharedProcedureSelection1.SelectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.SelectedProcedure_StopComplete);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.sharedProcedureSelection1.SelectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.SelectedProcedure_StopComplete);
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.adrResult,
      (object) this.adrMessage
    };
  }

  private void LogText(string text)
  {
    if (string.IsNullOrEmpty(text))
      return;
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void SelectedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    if (this.sharedProcedureSelection1.SelectedProcedure.Result == 1)
    {
      this.adrMessage = Resources.Message_Success;
      this.adrResult = true;
    }
    else
    {
      this.adrMessage = Resources.Message_Stopped;
      this.adrResult = false;
    }
    this.LogText(this.adrMessage);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.tableLayoutPanelBottom = new TableLayoutPanel();
    this.checkmarkStatus = new Checkmark();
    this.buttonStartStop = new Button();
    this.labelStatus = new Label();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleCheck = new DigitalReadoutInstrument();
    this.seekTimeListView = new SeekTimeListView();
    this.digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    this.textBoxWarning = new TextBox();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelBottom).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelBottom, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentVehicleCheck, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.seekTimeListView, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentParkingBrake, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxWarning, 0, 0);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelBottom, "tableLayoutPanelBottom");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelBottom, 2);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.checkmarkStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.buttonStartStop, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.labelStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.sharedProcedureSelection1, 2, 0);
    ((Control) this.tableLayoutPanelBottom).Name = "tableLayoutPanelBottom";
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    componentResourceManager.ApplyResources((object) this.buttonStartStop, "buttonStartStop");
    this.buttonStartStop.Name = "buttonStartStop";
    this.buttonStartStop.UseCompatibleTextRendering = true;
    this.buttonStartStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[2]
    {
      "SP_Powertrain_Repair_Validation_Routine1",
      "SP_FuelInjectorCleaningMachine_Routine"
    });
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = "DigitalReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(2, 1191.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleCheck, "digitalReadoutInstrumentVehicleCheck");
    this.digitalReadoutInstrumentVehicleCheck.FontGroup = "DigitalReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheck).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Initialize((ValueState) 3, 3);
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Modify(3, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheck).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "VehicleCheckStatus");
    ((Control) this.digitalReadoutInstrumentVehicleCheck).Name = "digitalReadoutInstrumentVehicleCheck";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheck).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "FICM";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.seekTimeListView, 4);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
    this.digitalReadoutInstrumentParkingBrake.FontGroup = "DigitalReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = "DigitalReadouts";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmarkStatus;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonStartStop;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.textBoxWarning, 2);
    componentResourceManager.ApplyResources((object) this.textBoxWarning, "textBoxWarning");
    this.textBoxWarning.Name = "textBoxWarning";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Fuel_Injector_Cleaning_Machine_Routine");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanelBottom).ResumeLayout(false);
    ((Control) this.tableLayoutPanelBottom).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
