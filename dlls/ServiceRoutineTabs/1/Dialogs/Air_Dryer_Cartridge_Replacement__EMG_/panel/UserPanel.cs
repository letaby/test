// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Air_Dryer_Cartridge_Replacement__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Air_Dryer_Cartridge_Replacement__EMG_.panel;

public class UserPanel : CustomPanel
{
  private TableLayoutPanel tableLayoutPanel;
  private RunServiceButton runServiceButtonResetWetness;
  private System.Windows.Forms.Label label;

  public UserPanel() => this.InitializeComponent();

  private void runServiceButtonResetWetness_ServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    int num = (int) ControlHelpers.ShowMessageBox(((ResultEventArgs) e).Succeeded ? Resources.Message_TheResetOperationSucceeded : Resources.Message_TheResetOperationFailed + ((ResultEventArgs) e).Exception.Message ?? string.Empty, MessageBoxButtons.OK, ((ResultEventArgs) e).Succeeded ? MessageBoxIcon.Asterisk : MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.runServiceButtonResetWetness = new RunServiceButton();
    this.label = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.runServiceButtonResetWetness, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.label, 0, 0);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.runServiceButtonResetWetness, "runServiceButtonResetWetness");
    ((Control) this.runServiceButtonResetWetness).Name = "runServiceButtonResetWetness";
    this.runServiceButtonResetWetness.ServiceCall = new ServiceCall("EAPU03T", "RT_Reset_wetness_Start");
    this.runServiceButtonResetWetness.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonResetWetness_ServiceComplete);
    componentResourceManager.ApplyResources((object) this.label, "label");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.label, 2);
    this.label.Name = "label";
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
