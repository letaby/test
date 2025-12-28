// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ICC5_Device_Activation.panel.UserPanel
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
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICC5_Device_Activation.panel;

public class UserPanel : CustomPanel
{
  private Checkmark checkmark;
  private Label label;
  private SharedProcedureSelection sharedProcedureSelection;
  private Button button;
  private SeekTimeListView seekTimeListView1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private TableLayoutPanel tableLayoutPanel;

  public UserPanel() => this.InitializeComponent();

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.sharedProcedureSelection.AnyProcedureInProgress)
      return;
    e.Cancel = true;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.checkmark = new Checkmark();
    this.label = new Label();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.button = new Button();
    this.seekTimeListView1 = new SeekTimeListView();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.checkmark, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.label, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.sharedProcedureSelection, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.button, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView1, 0, 0);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.checkmark, "checkmark");
    ((Control) this.checkmark).Name = "checkmark";
    componentResourceManager.ApplyResources((object) this.label, "label");
    this.label.Name = "label";
    this.label.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_ICC5_DeviceActivation"
    });
    componentResourceManager.ApplyResources((object) this.button, "button");
    this.button.Name = "button";
    this.button.UseCompatibleTextRendering = true;
    this.button.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.seekTimeListView1, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "ICC5 Device Activation";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "HH:mm:ss.fff";
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.label;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.checkmark;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.button;
    this.sharedProcedureIntegrationComponent.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
