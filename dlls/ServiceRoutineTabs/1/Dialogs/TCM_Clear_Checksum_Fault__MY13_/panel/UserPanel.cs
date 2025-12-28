// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clear_Checksum_Fault__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clear_Checksum_Fault__MY13_.panel;

public class UserPanel : CustomPanel
{
  private TableLayoutPanel tableLayoutPanel1;
  private RunServiceButton runServiceButton1;
  private Button button1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument1;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.digitalReadoutInstrument1.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument1_RepresentedStateChanged);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrument1_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    ((Control) this.runServiceButton1).Enabled = this.digitalReadoutInstrument1.RepresentedState == 3;
  }

  private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    this.digitalReadoutInstrument1.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrument1_RepresentedStateChanged);
    this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.runServiceButton1 = new RunServiceButton();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.button1 = new Button();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.runServiceButton1, "runServiceButton1");
    ((Control) this.runServiceButton1).Name = "runServiceButton1";
    this.runServiceButton1.ServiceCall = new ServiceCall("TCM01T", "RT_0461_Checksummen_Fehlerzaehler_zuruecksetzen_Start");
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButton1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.button1, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "TCM CCF Font";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "TCM01T", "18F3EE");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    this.button1.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.button1, "button1");
    this.button1.Name = "button1";
    this.button1.UseCompatibleTextRendering = true;
    this.button1.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "TCM CCF Font";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "TCM01T", "00F1EE");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
