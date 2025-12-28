// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Lambda_Sensor_Replacement__MY16_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Lambda_Sensor_Replacement__MY16_.panel;

public class UserPanel : CustomPanel
{
  private RunServiceButton runServiceButton;
  private TableLayoutPanel tableLayoutPanel1;
  private System.Windows.Forms.Label label1;

  public UserPanel() => this.InitializeComponent();

  private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Exception != null)
    {
      int num1 = (int) MessageBox.Show((IWin32Window) this, ((ResultEventArgs) e).Exception.Message, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }
    else
    {
      int num2 = (int) MessageBox.Show((IWin32Window) this, Resources.Message_TheRoutineWasSuccessful, ApplicationInformation.ProductName, MessageBoxButtons.OK);
    }
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.runServiceButton = new RunServiceButton();
    this.label1 = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButton, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.runServiceButton, "runServiceButton");
    ((Control) this.runServiceButton).Name = "runServiceButton";
    this.runServiceButton.ServiceCall = new ServiceCall("MCM21T", "RT_SR081_Reset_Lambda_sensor_Start_Status");
    this.runServiceButton.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label1, 2);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
