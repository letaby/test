// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data_45X_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data_45X_.panel;

public class UserPanel : CustomPanel
{
  private SeekTimeListView seekTimeListView1;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanel2;
  private RunServiceButton runServiceButtonStart;
  private Checkmark checkmark1;
  private System.Windows.Forms.Label label1;
  private Button button2;

  public UserPanel() => this.InitializeComponent();

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (((RunSharedProcedureButtonBase) this.runServiceButtonStart).InProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
  }

  private void LogText(string text)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void runServiceButtonStart_ServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      this.LogText(Resources.Message_CompleteSuccess);
      this.checkmark1.CheckState = CheckState.Checked;
      this.label1.Text = Resources.Message_CompleteSuccess;
    }
    else
    {
      this.checkmark1.CheckState = CheckState.Unchecked;
      this.label1.Text = Resources.Message_Error;
      this.LogText(Resources.Message_Error);
      if (((ResultEventArgs) e).Exception != null)
        this.LogText(((ResultEventArgs) e).Exception.Message);
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.seekTimeListView1 = new SeekTimeListView();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.checkmark1 = new Checkmark();
    this.label1 = new System.Windows.Forms.Label();
    this.button2 = new Button();
    this.runServiceButtonStart = new RunServiceButton();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "MaintenanceSystemTransfer45X";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.button2, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonStart, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.checkmark1.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    this.button2.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.button2, "button2");
    this.button2.Name = "button2";
    this.button2.UseCompatibleTextRendering = true;
    this.button2.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonStart, "runServiceButtonStart");
    ((Control) this.runServiceButtonStart).Name = "runServiceButtonStart";
    this.runServiceButtonStart.ServiceCall = new ServiceCall("CGW05T", "RT_Transfer_data_from_the_mirror_memory_Start");
    this.runServiceButtonStart.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonStart_ServiceComplete);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.seekTimeListView1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.tableLayoutPanel1, 0, 1);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
