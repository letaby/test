// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Self_Test__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Self_Test__NGC_.panel;

public class UserPanel : CustomPanel
{
  private RunServiceButton runServiceButtonGaugeTestStart;
  private TableLayoutPanel tableLayoutPanelMain;
  private RunServiceButton runServiceButtonDisplayTestStop;
  private System.Windows.Forms.Label labelDisplay;
  private RunServiceButton runServiceButtonLampTestStop;
  private System.Windows.Forms.Label labelIndicatorLamp;
  private RunServiceButton runServiceButtonLampTestStart;
  private RunServiceButton runServiceButtonGaugeTestStop;
  private System.Windows.Forms.Label labelGaugeSweep;
  private RunServiceButton runServiceButtonIllumTestStart;
  private System.Windows.Forms.Label labelIllumination;
  private RunServiceButton runServiceButtonIllumTestStop;
  private RunServiceButton runServiceButtonDisplayTestStart;

  public UserPanel() => this.InitializeComponent();

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    Channel channel = this.GetChannel("ICUC01T");
    if (channel == null || !channel.Online)
      return;
    Service service = channel.Services["FN_HardReset"];
    if (service != (Service) null)
      service.Execute(false);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.runServiceButtonGaugeTestStart = new RunServiceButton();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.runServiceButtonDisplayTestStop = new RunServiceButton();
    this.labelDisplay = new System.Windows.Forms.Label();
    this.runServiceButtonLampTestStop = new RunServiceButton();
    this.labelIndicatorLamp = new System.Windows.Forms.Label();
    this.runServiceButtonLampTestStart = new RunServiceButton();
    this.runServiceButtonGaugeTestStop = new RunServiceButton();
    this.labelGaugeSweep = new System.Windows.Forms.Label();
    this.runServiceButtonIllumTestStart = new RunServiceButton();
    this.labelIllumination = new System.Windows.Forms.Label();
    this.runServiceButtonIllumTestStop = new RunServiceButton();
    this.runServiceButtonDisplayTestStart = new RunServiceButton();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.runServiceButtonGaugeTestStart, "runServiceButtonGaugeTestStart");
    ((Control) this.runServiceButtonGaugeTestStart).Name = "runServiceButtonGaugeTestStart";
    this.runServiceButtonGaugeTestStart.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
    {
      "OptionRecord_Byte5=1",
      "OptionRecord_Byte6=00",
      "OptionRecord_Byte7=00"
    });
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.runServiceButtonDisplayTestStop, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelDisplay, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.runServiceButtonLampTestStop, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelIndicatorLamp, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.runServiceButtonLampTestStart, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.runServiceButtonGaugeTestStop, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.runServiceButtonGaugeTestStart, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelGaugeSweep, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.runServiceButtonIllumTestStart, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelIllumination, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.runServiceButtonIllumTestStop, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.runServiceButtonDisplayTestStart, 1, 3);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.runServiceButtonDisplayTestStop, "runServiceButtonDisplayTestStop");
    ((Control) this.runServiceButtonDisplayTestStop).Name = "runServiceButtonDisplayTestStop";
    this.runServiceButtonDisplayTestStop.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
    {
      "OptionRecord_Byte5=4",
      "OptionRecord_Byte6=00",
      "OptionRecord_Byte7=00"
    });
    componentResourceManager.ApplyResources((object) this.labelDisplay, "labelDisplay");
    this.labelDisplay.Name = "labelDisplay";
    this.labelDisplay.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonLampTestStop, "runServiceButtonLampTestStop");
    ((Control) this.runServiceButtonLampTestStop).Name = "runServiceButtonLampTestStop";
    this.runServiceButtonLampTestStop.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
    {
      "OptionRecord_Byte5=2",
      "OptionRecord_Byte6=00",
      "OptionRecord_Byte7=00"
    });
    componentResourceManager.ApplyResources((object) this.labelIndicatorLamp, "labelIndicatorLamp");
    this.labelIndicatorLamp.Name = "labelIndicatorLamp";
    this.labelIndicatorLamp.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonLampTestStart, "runServiceButtonLampTestStart");
    ((Control) this.runServiceButtonLampTestStart).Name = "runServiceButtonLampTestStart";
    this.runServiceButtonLampTestStart.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
    {
      "OptionRecord_Byte5=2",
      "OptionRecord_Byte6=00",
      "OptionRecord_Byte7=00"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonGaugeTestStop, "runServiceButtonGaugeTestStop");
    ((Control) this.runServiceButtonGaugeTestStop).Name = "runServiceButtonGaugeTestStop";
    this.runServiceButtonGaugeTestStop.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
    {
      "OptionRecord_Byte5=1",
      "OptionRecord_Byte6=00",
      "OptionRecord_Byte7=00"
    });
    componentResourceManager.ApplyResources((object) this.labelGaugeSweep, "labelGaugeSweep");
    this.labelGaugeSweep.Name = "labelGaugeSweep";
    this.labelGaugeSweep.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonIllumTestStart, "runServiceButtonIllumTestStart");
    ((Control) this.runServiceButtonIllumTestStart).Name = "runServiceButtonIllumTestStart";
    this.runServiceButtonIllumTestStart.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
    {
      "OptionRecord_Byte5=3",
      "OptionRecord_Byte6=00",
      "OptionRecord_Byte7=00"
    });
    componentResourceManager.ApplyResources((object) this.labelIllumination, "labelIllumination");
    this.labelIllumination.Name = "labelIllumination";
    this.labelIllumination.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServiceButtonIllumTestStop, "runServiceButtonIllumTestStop");
    ((Control) this.runServiceButtonIllumTestStop).Name = "runServiceButtonIllumTestStop";
    this.runServiceButtonIllumTestStop.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
    {
      "OptionRecord_Byte5=3",
      "OptionRecord_Byte6=00",
      "OptionRecord_Byte7=00"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonDisplayTestStart, "runServiceButtonDisplayTestStart");
    ((Control) this.runServiceButtonDisplayTestStart).Name = "runServiceButtonDisplayTestStart";
    this.runServiceButtonDisplayTestStart.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
    {
      "OptionRecord_Byte5=4",
      "OptionRecord_Byte6=00",
      "OptionRecord_Byte7=00"
    });
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }
}
