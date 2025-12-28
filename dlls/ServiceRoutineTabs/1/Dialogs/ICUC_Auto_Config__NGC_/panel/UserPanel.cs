// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Auto_Config__NGC_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Auto_Config__NGC_.panel;

public class UserPanel : CustomPanel
{
  private readonly string[] professionalProcedures = new string[1]
  {
    "SP_ICUC01T_AutoConfig"
  };
  private readonly string[] engineeringProcedures = new string[3]
  {
    "SP_ICUC01T_AutoConfig",
    "SP_ICUC01T_AutoConfig_PID20",
    "SP_ICUC01T_AutoConfig_PID25"
  };
  private TableLayoutPanel tableLayoutPanel;
  private SeekTimeListView seekTimeListView;
  private SharedProcedureSelection sharedProcedureSelection;
  private Button button;
  private System.Windows.Forms.Label labelStatus;
  private Checkmark checkmark;
  private System.Windows.Forms.Label labelInstructions;
  private DigitalReadoutInstrument digitalReadoutInstrumentAutoConfigFault;
  private System.Windows.Forms.Label label1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;

  public UserPanel()
  {
    this.InitializeComponent();
    this.seekTimeListView.RequiredUserLabelPrefix = this.sharedProcedureSelection.SelectedProcedure.Name;
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection(ApplicationInformation.IsProductName("Engineering") ? (IEnumerable<string>) this.engineeringProcedures : (IEnumerable<string>) this.professionalProcedures);
    this.UpdateInstruments();
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.sharedProcedureSelection.AnyProcedureInProgress)
      return;
    e.Cancel = true;
  }

  public virtual void OnChannelsChanged() => this.UpdateInstruments();

  private void UpdateInstruments()
  {
    Channel channel = SapiManager.GlobalInstance.Sapi.Channels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu.Identifier == "UDS-23"));
    string str = channel != null ? channel.Ecu.Name : "ICUC01T";
    this.labelInstructions.Text = Resources.Message_ICUCNotPerformed;
    this.label1.Text = Resources.Message_ICUCSMF;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, str, "0DFBFF");
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAutoConfigFault).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, str, "0FFBFF");
  }

  private void sharedProcedureSelection_StatusReport(object sender, StatusReportEventArgs e)
  {
    ((Control) this.sharedProcedureSelection).Enabled = !this.sharedProcedureSelection.AnyProcedureInProgress;
  }

  private void sharedProcedureSelection_SelectionChanged(object sender, EventArgs e)
  {
    this.seekTimeListView.RequiredUserLabelPrefix = this.sharedProcedureSelection.SelectedProcedure.Name;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.label1 = new System.Windows.Forms.Label();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.seekTimeListView = new SeekTimeListView();
    this.button = new Button();
    this.labelStatus = new System.Windows.Forms.Label();
    this.checkmark = new Checkmark();
    this.labelInstructions = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentAutoConfigFault = new DigitalReadoutInstrument();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.label1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrument1, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.button, 4, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelStatus, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.checkmark, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.labelInstructions, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentAutoConfigFault, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.sharedProcedureSelection, 3, 4);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.label1, 3);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "ICUC01T", "0DFBFF");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.seekTimeListView, 5);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "ICUCAutoConfig";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    componentResourceManager.ApplyResources((object) this.button, "button");
    this.button.Name = "button";
    this.button.UseCompatibleTextRendering = true;
    this.button.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.labelStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    ((TableLayoutPanel) this.tableLayoutPanel).SetRowSpan((Control) this.labelStatus, 3);
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmark, "checkmark");
    ((Control) this.checkmark).Name = "checkmark";
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.labelInstructions, 3);
    componentResourceManager.ApplyResources((object) this.labelInstructions, "labelInstructions");
    this.labelInstructions.Name = "labelInstructions";
    this.labelInstructions.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.digitalReadoutInstrumentAutoConfigFault, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAutoConfigFault, "digitalReadoutInstrumentAutoConfigFault");
    this.digitalReadoutInstrumentAutoConfigFault.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAutoConfigFault).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAutoConfigFault).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "ICUC01T", "0FFBFF");
    ((Control) this.digitalReadoutInstrumentAutoConfigFault).Name = "digitalReadoutInstrumentAutoConfigFault";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAutoConfigFault).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[3]
    {
      "SP_ICUC01T_AutoConfig",
      "SP_ICUC01T_AutoConfig_PID20",
      "SP_ICUC01T_AutoConfig_PID25"
    });
    this.sharedProcedureSelection.StatusReport += new EventHandler<StatusReportEventArgs>(this.sharedProcedureSelection_StatusReport);
    this.sharedProcedureSelection.SelectionChanged += new EventHandler(this.sharedProcedureSelection_SelectionChanged);
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.checkmark;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.button;
    this.sharedProcedureIntegrationComponent.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_ICUC_Automatic_Configuration");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
