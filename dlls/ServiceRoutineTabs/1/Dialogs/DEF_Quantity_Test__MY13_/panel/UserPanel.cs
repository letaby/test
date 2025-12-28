// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string AcmName = "ACM21T";
  private const string MeduimDutyEngineDefPressureQualifier = "DT_AS014_DEF_Pressure";
  private const string HeavyDutyEngineDefPressureQualifier = "DT_AS110_ADS_DEF_Pressure_2";
  private const string MediumDutyDEFQuantitySharedProcedureQualifier = "SP_DEFQuantityTest_MY13_MDEG";
  private const string HeavyDutyDEFQuantitySharedProcedureQualifier = "SP_DEFQuantityTest_MY13";
  private Channel channel;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private TableLayoutPanel tableLayoutPanel;
  private Button buttonStart;
  private Checkmark statusCheckmark;
  private SeekTimeListView seekTimeListView;
  private System.Windows.Forms.Label status;
  private SharedProcedureSelection sharedProcedureSelection;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private DigitalReadoutInstrument digitalReadoutInstrumentADSPrimingRequest;
  private DigitalReadoutInstrument digitalReadoutInstrumentEnableAds;
  private DigitalReadoutInstrument digitalReadoutInstrumentActualQuantity;
  private DigitalReadoutInstrument digitalReadoutInstrumentDosingQuantity;
  private DigitalReadoutInstrument digitalReadoutInstrumentAirlessDosingUreaPressure;
  private DigitalReadoutInstrument digitalReadoutInstrumentResults;
  private DigitalReadoutInstrument digitalReadoutInstrumentAdBluePumpSpeed;

  public UserPanel()
  {
    this.InitializeComponent();
    SapiManager.GlobalInstance.EquipmentTypeChanged += new EventHandler<EquipmentTypeChangedEventArgs>(this.GlobalInstance_EquipmentTypeChanged);
  }

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
    SapiManager.GlobalInstance.EquipmentTypeChanged -= new EventHandler<EquipmentTypeChangedEventArgs>(this.GlobalInstance_EquipmentTypeChanged);
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

  public virtual void OnChannelsChanged()
  {
    Channel channel = this.GetChannel("ACM21T");
    if (channel != this.channel)
      this.channel = channel;
    this.UpdateConnectedEquipmentType();
  }

  private void UpdateInstruments(bool isMediumDuty)
  {
    if (this.channel == null)
      return;
    if (isMediumDuty)
      ((SingleInstrumentBase) this.digitalReadoutInstrumentAirlessDosingUreaPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS014_DEF_Pressure");
    else
      ((SingleInstrumentBase) this.digitalReadoutInstrumentAirlessDosingUreaPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS110_ADS_DEF_Pressure_2");
    ((Control) this.digitalReadoutInstrumentAirlessDosingUreaPressure).Refresh();
  }

  private void UpdateSharedProcedureSelection(bool isMediumDuty)
  {
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      isMediumDuty ? "SP_DEFQuantityTest_MY13_MDEG" : "SP_DEFQuantityTest_MY13"
    });
  }

  private bool IsMediumDuty(string equipment)
  {
    switch (equipment.ToUpperInvariant())
    {
      case "DD5":
      case "DD8":
      case "MDEG 4-CYLINDER TIER4":
      case "MDEG 6-CYLINDER TIER4":
        return true;
      default:
        return false;
    }
  }

  private void UpdateConnectedEquipmentType()
  {
    EquipmentType equipmentType = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault<EquipmentType>((Func<EquipmentType, bool>) (et =>
    {
      ElectronicsFamily family = ((EquipmentType) ref et).Family;
      return ((ElectronicsFamily) ref family).Category.Equals("Engine", StringComparison.OrdinalIgnoreCase);
    }));
    if (!EquipmentType.op_Inequality(equipmentType, EquipmentType.Empty))
      return;
    bool isMediumDuty = this.IsMediumDuty(((EquipmentType) ref equipmentType).Name);
    this.UpdateInstruments(isMediumDuty);
    this.UpdateSharedProcedureSelection(isMediumDuty);
  }

  private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
  {
    if (!e.Category.Equals("Engine", StringComparison.OrdinalIgnoreCase))
      return;
    this.UpdateConnectedEquipmentType();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.tableLayoutPanel = new TableLayoutPanel();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.digitalReadoutInstrumentADSPrimingRequest = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEnableAds = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentActualQuantity = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDosingQuantity = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentAirlessDosingUreaPressure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentAdBluePumpSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
    this.buttonStart = new Button();
    this.statusCheckmark = new Checkmark();
    this.seekTimeListView = new SeekTimeListView();
    this.status = new System.Windows.Forms.Label();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonStart, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.statusCheckmark, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.status, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.sharedProcedureSelection, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 1);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
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
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS079_ADS_priming_request");
    ((Control) this.digitalReadoutInstrumentADSPrimingRequest).Name = "digitalReadoutInstrumentADSPrimingRequest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentADSPrimingRequest).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEnableAds, "digitalReadoutInstrumentEnableAds");
    this.digitalReadoutInstrumentEnableAds.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnableAds).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnableAds).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_DS002_Enable_ADS");
    ((Control) this.digitalReadoutInstrumentEnableAds).Name = "digitalReadoutInstrumentEnableAds";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEnableAds).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentActualQuantity, "digitalReadoutInstrumentActualQuantity");
    this.digitalReadoutInstrumentActualQuantity.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS160_Real_Time_ADS_DEF_Dosed_Quantity_g_hr");
    ((Control) this.digitalReadoutInstrumentActualQuantity).Name = "digitalReadoutInstrumentActualQuantity";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDosingQuantity, "digitalReadoutInstrumentDosingQuantity");
    this.digitalReadoutInstrumentDosingQuantity.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS104_ADS_Doser_PWM");
    ((Control) this.digitalReadoutInstrumentDosingQuantity).Name = "digitalReadoutInstrumentDosingQuantity";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAirlessDosingUreaPressure, "digitalReadoutInstrumentAirlessDosingUreaPressure");
    this.digitalReadoutInstrumentAirlessDosingUreaPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirlessDosingUreaPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirlessDosingUreaPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS110_ADS_DEF_Pressure_2");
    ((Control) this.digitalReadoutInstrumentAirlessDosingUreaPressure).Name = "digitalReadoutInstrumentAirlessDosingUreaPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAirlessDosingUreaPressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAdBluePumpSpeed, "digitalReadoutInstrumentAdBluePumpSpeed");
    this.digitalReadoutInstrumentAdBluePumpSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAdBluePumpSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAdBluePumpSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS143_ADS_Pump_Speed");
    ((Control) this.digitalReadoutInstrumentAdBluePumpSpeed).Name = "digitalReadoutInstrumentAdBluePumpSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAdBluePumpSpeed).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.digitalReadoutInstrumentResults, 2);
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
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM21T", "RT_SCR_Dosing_Quantity_Check_Request_Results_status_of_service_function");
    ((Control) this.digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
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
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[2]
    {
      "SP_DEFQuantityTest_MY13",
      "SP_DEFQuantityTest_MY13_MDEG"
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
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
