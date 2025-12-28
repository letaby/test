// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Inducement_system_activation.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Inducement_system_activation.panel;

public class UserPanel : CustomPanel
{
  private const string AcmInducementSystemNotActivatedQualifier = "96100E";
  private const string InducementActivationStartQualifier = "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start";
  private const string Acm21TName = "ACM21T";
  private Channel acm = (Channel) null;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private RunServiceButton runServiceButtonMCM;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label label2;
  private RunServiceButton runServiceButtonACM;

  public UserPanel()
  {
    this.InitializeComponent();
    ((Control) this.runServiceButtonACM).Enabled = false;
    ((Control) this.runServiceButtonACM).Enabled = false;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetACM(this.GetChannel("ACM21T", (CustomPanel.ChannelLookupOptions) 7));
  }

  private void SetACM(Channel acm)
  {
    if (this.acm == acm)
      return;
    this.acm = acm;
    if (this.acm != null)
    {
      ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, this.acm.Ecu.Name, "96100E");
      this.runServiceButtonACM.ServiceCall = new ServiceCall(this.acm.Ecu.Name, "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start");
    }
    if (acm == null || acm.Services["RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start"] == (Service) null)
      ((Control) this.runServiceButtonACM).Enabled = false;
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  private void digitalReadoutInstrument1_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (this.digitalReadoutInstrument1.RepresentedState == 1)
      ((Control) this.runServiceButtonACM).Enabled = false;
    else
      ((Control) this.runServiceButtonACM).Enabled = true;
  }

  private void digitalReadoutInstrument2_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (this.digitalReadoutInstrument2.RepresentedState == 1)
      ((Control) this.runServiceButtonMCM).Enabled = false;
    else
      ((Control) this.runServiceButtonMCM).Enabled = true;
  }

  private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    if (!((ResultEventArgs) e).Succeeded)
      return;
    e.Service.Channel.FaultCodes.Reset(false);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.runServiceButtonACM = new RunServiceButton();
    this.runServiceButtonMCM = new RunServiceButton();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.label1 = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonACM, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonMCM, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label2, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.runServiceButtonACM, "runServiceButtonACM");
    ((Control) this.runServiceButtonACM).Name = "runServiceButtonACM";
    this.runServiceButtonACM.ServiceCall = new ServiceCall("ACM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start");
    this.runServiceButtonACM.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    componentResourceManager.ApplyResources((object) this.runServiceButtonMCM, "runServiceButtonMCM");
    ((Control) this.runServiceButtonMCM).Name = "runServiceButtonMCM";
    this.runServiceButtonMCM.ServiceCall = new ServiceCall("MCM21T", "RT_SR0CF_Off_High_Way_inducement_EOL_activation_Start");
    this.runServiceButtonMCM.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 0, 64 /*0x40*/);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(3, 4.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(4, 5.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(5, 8.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(6, 9.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(7, 12.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(8, 13.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(9, 32.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(10, 33.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(11, 36.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(12, 37.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(13, 40.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(14, 41.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(15, 44.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(16 /*0x10*/, 45.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(17, 128.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(18, 129.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(19, 132.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(20, 133.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(21, 136.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(22, 137.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(23, 140.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(24, 141.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(25, 160.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(26, 161.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(27, 164.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(28, 165.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(29, 168.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(30, 169.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(31 /*0x1F*/, 172.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(32 /*0x20*/, 173.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(33, 256.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(34, 257.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(35, 260.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(36, 261.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(37, 264.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(38, 265.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(39, 268.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(40, 269.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(41, 288.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(42, 289.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(43, 292.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(44, 293.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(45, 296.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(46, 297.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(47, 300.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(48 /*0x30*/, 301.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(49, 384.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(50, 385.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(51, 388.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(52, 389.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(53, 392.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(54, 393.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(55, 396.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(56, 397.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(57, 416.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(58, 417.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(59, 420.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(60, 421.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(61, 424.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(62, 425.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(63 /*0x3F*/, 428.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(64 /*0x40*/, 429.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "ACM21T", "96100E");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrument1.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument1_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "base";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 0, 64 /*0x40*/);
    this.digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(3, 4.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(4, 5.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(5, 8.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(6, 9.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(7, 12.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(8, 13.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(9, 32.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(10, 33.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(11, 36.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(12, 37.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(13, 40.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(14, 41.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(15, 44.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(16 /*0x10*/, 45.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(17, 128.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(18, 129.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(19, 132.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(20, 133.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(21, 136.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(22, 137.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(23, 140.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(24, 141.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(25, 160.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(26, 161.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(27, 164.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(28, 165.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(29, 168.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(30, 169.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(31 /*0x1F*/, 172.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(32 /*0x20*/, 173.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(33, 256.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(34, 257.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(35, 260.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(36, 261.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(37, 264.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(38, 265.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(39, 268.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(40, 269.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(41, 288.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(42, 289.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(43, 292.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(44, 293.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(45, 296.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(46, 297.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(47, 300.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(48 /*0x30*/, 301.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(49, 384.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(50, 385.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(51, 388.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(52, 389.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(53, 392.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(54, 393.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(55, 396.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(56, 397.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(57, 416.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(58, 417.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(59, 420.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(60, 421.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(61, 424.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(62, 425.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(63 /*0x3F*/, 428.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(64 /*0x40*/, 429.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM21T", "7E140E");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrument2.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument2_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
