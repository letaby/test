// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EDAC_Oil_Life_Reset__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EDAC_Oil_Life_Reset__EMG_.panel;

public class UserPanel : CustomPanel
{
  private TableLayoutPanel tableLayoutPanel;
  private RunServiceButton runServiceButtonResetOilChange;
  private System.Windows.Forms.Label label;

  public UserPanel() => this.InitializeComponent();

  private void runServiceButtonResetOilChange_ServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    int num = (int) ControlHelpers.ShowMessageBox(((ResultEventArgs) e).Succeeded ? Resources.Message_TheResetOperationSucceeded : Resources.Message_TheResetOperationFailed + ((ResultEventArgs) e).Exception.Message ?? string.Empty, MessageBoxButtons.OK, ((ResultEventArgs) e).Succeeded ? MessageBoxIcon.Asterisk : MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (((RunSharedProcedureButtonBase) this.runServiceButtonResetOilChange).InProgress)
      e.Cancel = true;
    if (e.Cancel || this.runServiceButtonResetOilChange == null)
      return;
    this.runServiceButtonResetOilChange.ServiceComplete -= new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonResetOilChange_ServiceComplete);
    this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.runServiceButtonResetOilChange = new RunServiceButton();
    this.label = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.runServiceButtonResetOilChange, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.label, 0, 0);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.runServiceButtonResetOilChange, "runServiceButtonResetOilChange");
    ((Control) this.runServiceButtonResetOilChange).Name = "runServiceButtonResetOilChange";
    this.runServiceButtonResetOilChange.ServiceCall = new ServiceCall("EAPU03T", "RT_Reset_oil_change_period_Start");
    this.runServiceButtonResetOilChange.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonResetOilChange_ServiceComplete);
    componentResourceManager.ApplyResources((object) this.label, "label");
    this.label.Name = "label";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_EDAC_Oil_Life_Reset");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
