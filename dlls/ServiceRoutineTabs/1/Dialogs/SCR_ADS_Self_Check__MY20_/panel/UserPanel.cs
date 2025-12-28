// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_ADS_Self_Check__MY20_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_ADS_Self_Check__MY20_.panel;

public class UserPanel : CustomPanel
{
  private const string AcmName = "ACM301T";
  private const string DefPressureQualifier = "DT_AS014_DEF_Pressure";
  private TableLayoutPanel tableLayoutPanel;
  private Button buttonStart;
  private SeekTimeListView seekTimeListView;
  private Panel panel1;
  private Checkmark checkmarkCanStart;
  private Label labelCanStart;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentADSPumpSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentADSPrimingRequest;
  private DigitalReadoutInstrument digitalReadoutInstrumentADSDosingValveState;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentResults;
  private BarInstrument barInstrumentDefPressure;
  private SharedProcedureSelection sharedProcedureSelection;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    string noResultAvailable = Resources.Message_NoResultAvailable;
    bool flag = false;
    if (this.sharedProcedureSelection.SelectedProcedure != null)
    {
      flag = this.sharedProcedureSelection.SelectedProcedure.Result == 1;
      if (((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem.Value != null)
        noResultAvailable = ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem.Value.ToString();
    }
    ((Control) this).Tag = (object) new object[2]
    {
      (object) flag,
      (object) noResultAvailable
    };
  }

  public virtual void OnChannelsChanged() => this.UpdateInstruments();

  private void UpdateInstruments()
  {
    ((SingleInstrumentBase) this.barInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS014_DEF_Pressure");
    ((Control) this.barInstrumentDefPressure).Refresh();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.seekTimeListView = new SeekTimeListView();
    this.panel1 = new Panel();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.labelCanStart = new Label();
    this.checkmarkCanStart = new Checkmark();
    this.buttonStart = new Button();
    this.digitalReadoutInstrumentADSPrimingRequest = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentADSDosingValveState = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentADSPumpSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
    this.barInstrumentDefPressure = new BarInstrument();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel).SuspendLayout();
    this.panel1.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.panel1, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentADSPrimingRequest, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentADSDosingValveState, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentADSPumpSpeed, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentResults, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.barInstrumentDefPressure, 0, 3);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "SCR ADS Self-check";
    ((TableLayoutPanel) this.tableLayoutPanel).SetRowSpan((Control) this.seekTimeListView, 7);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.f";
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.panel1, 2);
    this.panel1.Controls.Add((Control) this.sharedProcedureSelection);
    this.panel1.Controls.Add((Control) this.labelCanStart);
    this.panel1.Controls.Add((Control) this.checkmarkCanStart);
    this.panel1.Controls.Add((Control) this.buttonStart);
    componentResourceManager.ApplyResources((object) this.panel1, "panel1");
    this.panel1.Name = "panel1";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_SCR_ADS_Self_Check_MY20"
    });
    componentResourceManager.ApplyResources((object) this.labelCanStart, "labelCanStart");
    this.labelCanStart.Name = "labelCanStart";
    this.labelCanStart.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkCanStart, "checkmarkCanStart");
    ((Control) this.checkmarkCanStart).Name = "checkmarkCanStart";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentADSPrimingRequest, "digitalReadoutInstrumentADSPrimingRequest");
    this.digitalReadoutInstrumentADSPrimingRequest.FontGroup = "SCRADSDigitals";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS079_ADS_priming_request");
    ((Control) this.digitalReadoutInstrumentADSPrimingRequest).Name = "digitalReadoutInstrumentADSPrimingRequest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentADSDosingValveState, "digitalReadoutInstrumentADSDosingValveState");
    this.digitalReadoutInstrumentADSDosingValveState.FontGroup = "SCRADSDigitals";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSDosingValveState).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSDosingValveState).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS011_ADS_dosing_valve_state");
    ((Control) this.digitalReadoutInstrumentADSDosingValveState).Name = "digitalReadoutInstrumentADSDosingValveState";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSDosingValveState).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentADSPumpSpeed, "digitalReadoutInstrumentADSPumpSpeed");
    this.digitalReadoutInstrumentADSPumpSpeed.FontGroup = "SCRADSDigitals";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPumpSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPumpSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS143_ADS_Pump_Speed");
    ((Control) this.digitalReadoutInstrumentADSPumpSpeed).Name = "digitalReadoutInstrumentADSPumpSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPumpSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = "SCRADSDigitals";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS001_Engine_Speed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS077_Vehicle_speed_from_ISP100ms");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentResults, "digitalReadoutInstrumentResults");
    this.digitalReadoutInstrumentResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).FreezeValue = false;
    this.digitalReadoutInstrumentResults.Gradient.Initialize((ValueState) 0, 16 /*0x10*/);
    this.digitalReadoutInstrumentResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(6, 5.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(7, 6.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(8, 7.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(9, 8.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(10, 9.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(11, 10.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(12, 11.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(13, 12.0, (ValueState) 1);
    this.digitalReadoutInstrumentResults.Gradient.Modify(14, 13.0, (ValueState) 3);
    this.digitalReadoutInstrumentResults.Gradient.Modify(15, 14.0, (ValueState) 3);
    this.digitalReadoutInstrumentResults.Gradient.Modify(16 /*0x10*/, 15.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Request_Results_status_of_service_function");
    ((Control) this.digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barInstrumentDefPressure, "barInstrumentDefPressure");
    this.barInstrumentDefPressure.FontGroup = "SCRADSBars";
    ((SingleInstrumentBase) this.barInstrumentDefPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS014_DEF_Pressure");
    ((Control) this.barInstrumentDefPressure).Name = "barInstrumentDefPressure";
    ((SingleInstrumentBase) this.barInstrumentDefPressure).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.labelCanStart;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.checkmarkCanStart;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_SCRADSSelf-Check");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    this.panel1.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
