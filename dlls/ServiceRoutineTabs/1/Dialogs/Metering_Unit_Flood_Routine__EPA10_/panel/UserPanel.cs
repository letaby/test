// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Metering_Unit_Flood_Routine__EPA10_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Metering_Unit_Flood_Routine__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private SharedProcedureSelection sharedProcedureSelection;
  private Label status;
  private Checkmark statusCheckmark;
  private Button buttonStart;
  private TableLayoutPanel tableLayoutPanel;
  private BarInstrument barInstrumentDEFAirPressure;
  private BarInstrument barInstrumentDEFPressure;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private SeekTimeListView seekTimeListView;
  private Label labelNote;

  public UserPanel() => this.InitializeComponent();

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.status = new Label();
    this.buttonStart = new Button();
    this.barInstrumentDEFAirPressure = new BarInstrument();
    this.barInstrumentDEFPressure = new BarInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.seekTimeListView = new SeekTimeListView();
    this.labelNote = new Label();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.statusCheckmark = new Checkmark();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.status, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonStart, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.barInstrumentDEFAirPressure, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.barInstrumentDEFPressure, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelNote, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.sharedProcedureSelection, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.statusCheckmark, 0, 5);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.status, "status");
    this.status.Name = "status";
    this.status.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.barInstrumentDEFAirPressure, 4);
    componentResourceManager.ApplyResources((object) this.barInstrumentDEFAirPressure, "barInstrumentDEFAirPressure");
    this.barInstrumentDEFAirPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentDEFAirPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentDEFAirPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS016_DEF_Air_Pressure");
    ((Control) this.barInstrumentDEFAirPressure).Name = "barInstrumentDEFAirPressure";
    ((SingleInstrumentBase) this.barInstrumentDEFAirPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.barInstrumentDEFPressure, 4);
    componentResourceManager.ApplyResources((object) this.barInstrumentDEFPressure, "barInstrumentDEFPressure");
    this.barInstrumentDEFPressure.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentDEFPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentDEFPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS014_DEF_Pressure");
    ((Control) this.barInstrumentDEFPressure).Name = "barInstrumentDEFPressure";
    ((SingleInstrumentBase) this.barInstrumentDEFPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.digitalReadoutInstrument1, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.seekTimeListView, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Metering Unit Flood Routine";
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
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DEFDoserPurgeRoutine_EPA10"
    });
    componentResourceManager.ApplyResources((object) this.statusCheckmark, "statusCheckmark");
    ((Control) this.statusCheckmark).Name = "statusCheckmark";
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.status;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.statusCheckmark;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_MeteringUnitFloodRoutine");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
