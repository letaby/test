// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Quantity_Control_Valve_Reset_FMU__MDEG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Quantity_Control_Valve_Reset_FMU__MDEG_.panel;

public class UserPanel : CustomPanel
{
  private TableLayoutPanel tableLayoutPanelWholePanel;
  private Button buttonClose;
  private System.Windows.Forms.Label labelDirections;
  private SeekTimeListView seekTimeListViewLog;
  private RunServiceButton runServiceButtonResetFMU;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    this.labelDirections.Text = Resources.Label_ClickButtonToResetFMU;
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void runServiceButtonResetFMU_ServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      this.LogText(Resources.Message_SuccessfulExecution);
    }
    else
    {
      StringBuilder stringBuilder = new StringBuilder(Resources.Message_FailedExecution);
      if (((ResultEventArgs) e).Exception != null && !string.IsNullOrEmpty(((ResultEventArgs) e).Exception.Message))
      {
        stringBuilder.Append(Resources.Message_Error);
        stringBuilder.Append(((ResultEventArgs) e).Exception.Message);
      }
      this.LogText(stringBuilder.ToString());
    }
  }

  private void LogText(string text)
  {
    this.LabelLog(this.seekTimeListViewLog.RequiredUserLabelPrefix, text);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.seekTimeListViewLog = new SeekTimeListView();
    this.buttonClose = new Button();
    this.labelDirections = new System.Windows.Forms.Label();
    this.runServiceButtonResetFMU = new RunServiceButton();
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.seekTimeListViewLog, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.buttonClose, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.labelDirections, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.runServiceButtonResetFMU, 0, 1);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    this.seekTimeListViewLog.FilterUserLabels = true;
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "QCV_Reset_FMU";
    this.seekTimeListViewLog.SelectedTime = new DateTime?();
    this.seekTimeListViewLog.ShowChannelLabels = false;
    this.seekTimeListViewLog.ShowCommunicationsState = false;
    this.seekTimeListViewLog.ShowControlPanel = false;
    this.seekTimeListViewLog.ShowDeviceColumn = false;
    this.seekTimeListViewLog.TimeFormat = "HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelDirections, "labelDirections");
    this.labelDirections.Name = "labelDirections";
    this.labelDirections.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonResetFMU, "runServiceButtonResetFMU");
    ((Control) this.runServiceButtonResetFMU).Name = "runServiceButtonResetFMU";
    this.runServiceButtonResetFMU.ServiceCall = new ServiceCall("MCM21T", "RT_SR014_SET_EOL_Default_Values_Start", (IEnumerable<string>) new string[1]
    {
      "E2P_Logical_Block_Number=25"
    });
    this.runServiceButtonResetFMU.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonResetFMU_ServiceComplete);
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
