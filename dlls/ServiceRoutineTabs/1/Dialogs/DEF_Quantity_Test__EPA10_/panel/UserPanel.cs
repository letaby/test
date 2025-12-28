// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__EPA10_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private TableLayoutPanel tableLayoutPanel;
  private Button buttonStart;
  private Checkmark statusCheckmark;
  private SeekTimeListView seekTimeListView;
  private Label labelNote;
  private Label status;
  private SharedProcedureSelection sharedProcedureSelection;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private BarInstrument barInstrumentDEFPressure;
  private BarInstrument barInstrumentDEFAirPressure;
  private DigitalReadoutInstrument digitalReadoutInstrumentActualQuantity;
  private DigitalReadoutInstrument digitalReadoutInstrumentDosingQuantity;
  private TableLayoutPanel tableLayoutPanel3;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;

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
    string str = Resources.Message_NoResultsAvailable;
    bool flag = false;
    if (this.sharedProcedureSelection.SelectedProcedure != null)
    {
      flag = this.sharedProcedureSelection.SelectedProcedure.Result == 1;
      str = flag ? Resources.Message_DEFDosingQuantityCheckCompleted : Resources.Message_DEFDosingQuantityCheckFailedOrTerminatedWithUnknownResult;
    }
    ((Control) this).Tag = (object) new object[2]
    {
      (object) flag,
      (object) str
    };
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.barInstrumentDEFPressure = new BarInstrument();
    this.barInstrumentDEFAirPressure = new BarInstrument();
    this.digitalReadoutInstrumentActualQuantity = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDosingQuantity = new DigitalReadoutInstrument();
    this.buttonStart = new Button();
    this.statusCheckmark = new Checkmark();
    this.seekTimeListView = new SeekTimeListView();
    this.labelNote = new Label();
    this.status = new Label();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.digitalReadoutInstrument3, 2, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS003_Enable_DEF_pump");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.tableLayoutPanelInstruments, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.barInstrumentDEFPressure, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.barInstrumentDEFAirPressure, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentActualQuantity, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentDosingQuantity, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanel3, 0, 0);
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.barInstrumentDEFPressure, 2);
    componentResourceManager.ApplyResources((object) this.barInstrumentDEFPressure, "barInstrumentDEFPressure");
    this.barInstrumentDEFPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentDEFPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentDEFPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS014_DEF_Pressure");
    ((Control) this.barInstrumentDEFPressure).Name = "barInstrumentDEFPressure";
    ((SingleInstrumentBase) this.barInstrumentDEFPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetColumnSpan((Control) this.barInstrumentDEFAirPressure, 2);
    componentResourceManager.ApplyResources((object) this.barInstrumentDEFAirPressure, "barInstrumentDEFAirPressure");
    this.barInstrumentDEFAirPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentDEFAirPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentDEFAirPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS016_DEF_Air_Pressure");
    ((Control) this.barInstrumentDEFAirPressure).Name = "barInstrumentDEFAirPressure";
    ((SingleInstrumentBase) this.barInstrumentDEFAirPressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentActualQuantity, "digitalReadoutInstrumentActualQuantity");
    this.digitalReadoutInstrumentActualQuantity.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS085_Actual_DEF_Dosing_Quantity");
    ((Control) this.digitalReadoutInstrumentActualQuantity).Name = "digitalReadoutInstrumentActualQuantity";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualQuantity).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDosingQuantity, "digitalReadoutInstrumentDosingQuantity");
    this.digitalReadoutInstrumentDosingQuantity.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS086_Requested_DEF_Dosing_Quantity");
    ((Control) this.digitalReadoutInstrumentDosingQuantity).Name = "digitalReadoutInstrumentDosingQuantity";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDosingQuantity).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonStart, 4, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.statusCheckmark, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelNote, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.status, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.sharedProcedureSelection, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 2);
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
    this.labelNote.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelNote, "labelNote");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.labelNote, 4);
    ((Control) this.labelNote).Name = "labelNote";
    this.labelNote.Orientation = (Label.TextOrientation) 1;
    this.labelNote.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.status, "status");
    this.status.Name = "status";
    this.status.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DEFQuantityTest_EPA10"
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
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
