// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__Euro5_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__Euro5_.panel;

public class UserPanel : CustomPanel
{
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private TableLayoutPanel tableLayoutPanel;
  private Button buttonStart;
  private Checkmark statusCheckmark;
  private SeekTimeListView seekTimeListView;
  private Label status;
  private SharedProcedureSelection sharedProcedureSelection;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private DigitalReadoutInstrument digitalReadoutInstrumentADSPrimingRequest;
  private DigitalReadoutInstrument digitalReadoutInstrumentEnableAds;
  private DigitalReadoutInstrument digitalReadoutInstrumentActualQuantity;
  private DigitalReadoutInstrument digitalReadoutInstrumentDosingQuantity;
  private DigitalReadoutInstrument digitalReadoutInstrumentAirlessDosingUreaPressure;
  private DigitalReadoutInstrument digitalReadoutInstrumentResults;
  private DigitalReadoutInstrument digitalReadoutInstrumentAdBluePumpSpeed;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    string resultsAvailable = Resources.Message_NoResultsAvailable;
    bool flag = false;
    if (this.sharedProcedureSelection.SelectedProcedure != null)
    {
      flag = this.sharedProcedureSelection.SelectedProcedure.Result == 1;
      if (((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem.Value != null)
        resultsAvailable = ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem.Value.ToString();
    }
    ((Control) this).Tag = (object) new object[2]
    {
      (object) flag,
      (object) resultsAvailable
    };
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.digitalReadoutInstrumentADSPrimingRequest = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEnableAds = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentActualQuantity = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDosingQuantity = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentAirlessDosingUreaPressure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentAdBluePumpSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.buttonStart = new Button();
    this.statusCheckmark = new Checkmark();
    this.seekTimeListView = new SeekTimeListView();
    this.status = new Label();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.tableLayoutPanelInstruments, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentADSPrimingRequest, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentEnableAds, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentActualQuantity, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentDosingQuantity, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentAirlessDosingUreaPressure, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentAdBluePumpSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentResults, 0, 3);
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentADSPrimingRequest, "digitalReadoutInstrumentADSPrimingRequest");
    this.digitalReadoutInstrumentADSPrimingRequest.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).FreezeValue = false;
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentADSPrimingRequest.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).Instrument = new Qualifier((QualifierTypes) 1, "MR201T", "DT_ADS_Priming_Request");
    ((Control) this.digitalReadoutInstrumentADSPrimingRequest).Name = "digitalReadoutInstrumentADSPrimingRequest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEnableAds, "digitalReadoutInstrumentEnableAds");
    this.digitalReadoutInstrumentEnableAds.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnableAds).FreezeValue = false;
    this.digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentEnableAds.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentEnableAds.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentEnableAds.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentEnableAds.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentEnableAds.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentEnableAds.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnableAds).Instrument = new Qualifier((QualifierTypes) 1, "MR201T", "DT_ADS_Pressure_dosing_enable_UPS");
    ((Control) this.digitalReadoutInstrumentEnableAds).Name = "digitalReadoutInstrumentEnableAds";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnableAds).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnableAds).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentActualQuantity, "digitalReadoutInstrumentActualQuantity");
    this.digitalReadoutInstrumentActualQuantity.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).Instrument = new Qualifier((QualifierTypes) 1, "MR201T", "DT_AAS_Actual_DEF_Dosing_Quantity");
    ((Control) this.digitalReadoutInstrumentActualQuantity).Name = "digitalReadoutInstrumentActualQuantity";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDosingQuantity, "digitalReadoutInstrumentDosingQuantity");
    this.digitalReadoutInstrumentDosingQuantity.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).FreezeValue = false;
    this.digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    this.digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
    this.digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
    this.digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
    this.digitalReadoutInstrumentDosingQuantity.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
    this.digitalReadoutInstrumentDosingQuantity.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentDosingQuantity.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentDosingQuantity.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentDosingQuantity.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentDosingQuantity.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).Instrument = new Qualifier((QualifierTypes) 1, "MR201T", "DT_ADS_Status_DEF_pump");
    ((Control) this.digitalReadoutInstrumentDosingQuantity).Name = "digitalReadoutInstrumentDosingQuantity";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAirlessDosingUreaPressure, "digitalReadoutInstrumentAirlessDosingUreaPressure");
    this.digitalReadoutInstrumentAirlessDosingUreaPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirlessDosingUreaPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirlessDosingUreaPressure).Instrument = new Qualifier((QualifierTypes) 1, "MR201T", "DT_AAS_DEF_Pressure");
    ((Control) this.digitalReadoutInstrumentAirlessDosingUreaPressure).Name = "digitalReadoutInstrumentAirlessDosingUreaPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirlessDosingUreaPressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAdBluePumpSpeed, "digitalReadoutInstrumentAdBluePumpSpeed");
    this.digitalReadoutInstrumentAdBluePumpSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAdBluePumpSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAdBluePumpSpeed).Instrument = new Qualifier((QualifierTypes) 1, "MR201T", "DT_AAS_Urea_Pump_Speed");
    ((Control) this.digitalReadoutInstrumentAdBluePumpSpeed).Name = "digitalReadoutInstrumentAdBluePumpSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAdBluePumpSpeed).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.digitalReadoutInstrumentResults, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentResults, "digitalReadoutInstrumentResults");
    this.digitalReadoutInstrumentResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).FreezeValue = false;
    this.digitalReadoutInstrumentResults.Gradient.Initialize((ValueState) 0, 8);
    this.digitalReadoutInstrumentResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentResults.Gradient.Modify(4, 11.0, (ValueState) 3);
    this.digitalReadoutInstrumentResults.Gradient.Modify(5, 12.0, (ValueState) 1);
    this.digitalReadoutInstrumentResults.Gradient.Modify(6, 13.0, (ValueState) 3);
    this.digitalReadoutInstrumentResults.Gradient.Modify(7, 14.0, (ValueState) 3);
    this.digitalReadoutInstrumentResults.Gradient.Modify(8, 15.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "MR201T", "RT_SR029D_EDU_Diagnosis_Routine_Request_Results_Urea_Quantity_Check");
    ((Control) this.digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonStart, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.statusCheckmark, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.status, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.sharedProcedureSelection, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 1);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.statusCheckmark, "statusCheckmark");
    ((Control) this.statusCheckmark).Name = "statusCheckmark";
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.seekTimeListView, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "DEF Quantity Test";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this.status, "status");
    this.status.Name = "status";
    this.status.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DEFQuantityTest_MR2"
    });
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.status;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.statusCheckmark;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_DEFQuantityTest");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
