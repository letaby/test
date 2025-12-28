// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Actuator_Functional_Check.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Actuator_Functional_Check.panel;

public class UserPanel : CustomPanel
{
  private TableLayoutPanel tableLayoutPanel;
  private Button buttonStart;
  private DigitalReadoutInstrument digitalReadoutEGRSCGND;
  private DigitalReadoutInstrument digitalReadoutEGRSCBATT;
  private DigitalReadoutInstrument digitalReadoutEGRSRH;
  private DigitalReadoutInstrument digitalReadoutEGRSRL;
  private DigitalReadoutInstrument digitalReadoutEGROL;
  private DigitalReadoutInstrument digitalReadoutHS2SC;
  private DigitalReadoutInstrument digitalReadoutHS2SCUB;
  private SeekTimeListView seekTimeListView;
  private DigitalReadoutInstrument digitalReadoutEngineState;
  private Panel panel1;
  private Checkmark checkmarkCanStart;
  private Label labelCanStart;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private BarInstrument barInstrumentCoolantTemp;
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
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.panel1 = new Panel();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.labelCanStart = new Label();
    this.checkmarkCanStart = new Checkmark();
    this.buttonStart = new Button();
    this.digitalReadoutEGRSCGND = new DigitalReadoutInstrument();
    this.digitalReadoutEGRSCBATT = new DigitalReadoutInstrument();
    this.digitalReadoutEGRSRH = new DigitalReadoutInstrument();
    this.digitalReadoutEGRSRL = new DigitalReadoutInstrument();
    this.digitalReadoutEGROL = new DigitalReadoutInstrument();
    this.digitalReadoutHS2SC = new DigitalReadoutInstrument();
    this.digitalReadoutHS2SCUB = new DigitalReadoutInstrument();
    this.seekTimeListView = new SeekTimeListView();
    this.digitalReadoutEngineState = new DigitalReadoutInstrument();
    this.barInstrumentCoolantTemp = new BarInstrument();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    this.panel1.SuspendLayout();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
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
      "SP_EGR_IAE_EPA07"
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
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutEGRSCGND, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutEGRSCBATT, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutEGRSRH, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutEGRSRL, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutEGROL, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutHS2SC, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutHS2SCUB, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutEngineState, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.panel1, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.barInstrumentCoolantTemp, 1, 1);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.digitalReadoutEGRSCGND, "digitalReadoutEGRSCGND");
    this.digitalReadoutEGRSCGND.FontGroup = "egriaedigitals";
    ((SingleInstrumentBase) this.digitalReadoutEGRSCGND).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutEGRSCGND).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "C80600");
    ((Control) this.digitalReadoutEGRSCGND).Name = "digitalReadoutEGRSCGND";
    ((SingleInstrumentBase) this.digitalReadoutEGRSCGND).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutEGRSCBATT, "digitalReadoutEGRSCBATT");
    this.digitalReadoutEGRSCBATT.FontGroup = "egriaedigitals";
    ((SingleInstrumentBase) this.digitalReadoutEGRSCBATT).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutEGRSCBATT).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "C80700");
    ((Control) this.digitalReadoutEGRSCBATT).Name = "digitalReadoutEGRSCBATT";
    ((SingleInstrumentBase) this.digitalReadoutEGRSCBATT).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutEGRSRH, "digitalReadoutEGRSRH");
    this.digitalReadoutEGRSRH.FontGroup = "egriaedigitals";
    ((SingleInstrumentBase) this.digitalReadoutEGRSRH).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutEGRSRH).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "9A0F00");
    ((Control) this.digitalReadoutEGRSRH).Name = "digitalReadoutEGRSRH";
    ((SingleInstrumentBase) this.digitalReadoutEGRSRH).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutEGRSRL, "digitalReadoutEGRSRL");
    this.digitalReadoutEGRSRL.FontGroup = "egriaedigitals";
    ((SingleInstrumentBase) this.digitalReadoutEGRSRL).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutEGRSRL).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "9A1000");
    ((Control) this.digitalReadoutEGRSRL).Name = "digitalReadoutEGRSRL";
    ((SingleInstrumentBase) this.digitalReadoutEGRSRL).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutEGROL, "digitalReadoutEGROL");
    this.digitalReadoutEGROL.FontGroup = "egriaedigitals";
    ((SingleInstrumentBase) this.digitalReadoutEGROL).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutEGROL).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "C80900");
    ((Control) this.digitalReadoutEGROL).Name = "digitalReadoutEGROL";
    ((SingleInstrumentBase) this.digitalReadoutEGROL).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutHS2SC, "digitalReadoutHS2SC");
    this.digitalReadoutHS2SC.FontGroup = "egriaedigitals";
    ((SingleInstrumentBase) this.digitalReadoutHS2SC).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutHS2SC).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "4E0800");
    ((Control) this.digitalReadoutHS2SC).Name = "digitalReadoutHS2SC";
    ((SingleInstrumentBase) this.digitalReadoutHS2SC).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutHS2SCUB, "digitalReadoutHS2SCUB");
    this.digitalReadoutHS2SCUB.FontGroup = "egriaedigitals";
    ((SingleInstrumentBase) this.digitalReadoutHS2SCUB).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutHS2SCUB).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "4E0500");
    ((Control) this.digitalReadoutHS2SCUB).Name = "digitalReadoutHS2SCUB";
    ((SingleInstrumentBase) this.digitalReadoutHS2SCUB).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "EGR Valve Actuator Functional Check";
    ((TableLayoutPanel) this.tableLayoutPanel).SetRowSpan((Control) this.seekTimeListView, 5);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this.digitalReadoutEngineState, "digitalReadoutEngineState");
    this.digitalReadoutEngineState.FontGroup = "egriaedigitals";
    ((SingleInstrumentBase) this.digitalReadoutEngineState).FreezeValue = false;
    this.digitalReadoutEngineState.Gradient.Initialize((ValueState) 0, 7);
    this.digitalReadoutEngineState.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutEngineState.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(4, 3.0, (ValueState) 1);
    this.digitalReadoutEngineState.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutEngineState.Gradient.Modify(7, 6.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutEngineState).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS023_Engine_State");
    ((Control) this.digitalReadoutEngineState).Name = "digitalReadoutEngineState";
    ((SingleInstrumentBase) this.digitalReadoutEngineState).UnitAlignment = StringAlignment.Near;
    this.barInstrumentCoolantTemp.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentCoolantTemp, "barInstrumentCoolantTemp");
    this.barInstrumentCoolantTemp.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentCoolantTemp).Gradient.Initialize((ValueState) 2, 1, "°C");
    ((AxisSingleInstrumentBase) this.barInstrumentCoolantTemp).Gradient.Modify(1, 50.0, (ValueState) 1);
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS013_Coolant_Temperature");
    ((Control) this.barInstrumentCoolantTemp).Name = "barInstrumentCoolantTemp";
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.labelCanStart;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.checkmarkCanStart;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    this.panel1.ResumeLayout(false);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
