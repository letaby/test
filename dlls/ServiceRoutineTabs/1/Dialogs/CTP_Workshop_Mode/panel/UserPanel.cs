// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Workshop_Mode.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Workshop_Mode.panel;

public class UserPanel : CustomPanel
{
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonClose;
  private DigitalReadoutInstrument digitalReadoutInstrumentCtpWMMode;
  private Checkmark checkmarkReady;
  private RunServiceButton runServiceButtonWMOff;
  private Label labelStatus;

  public UserPanel()
  {
    this.InitializeComponent();
    this.UpdateUI();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    this.digitalReadoutInstrumentCtpWMMode.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentCtpWMMode_RepresentedStateChanged);
  }

  private void UpdateUI()
  {
    if (((SingleInstrumentBase) this.digitalReadoutInstrumentCtpWMMode).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentCtpWMMode).DataItem.Value != null && this.digitalReadoutInstrumentCtpWMMode.RepresentedState != 0)
    {
      this.checkmarkReady.Checked = ((Control) this.runServiceButtonWMOff).Enabled = this.digitalReadoutInstrumentCtpWMMode.RepresentedState == 2;
      this.labelStatus.Text = this.digitalReadoutInstrumentCtpWMMode.RepresentedState == 2 ? Resources.Message_Ready : Resources.Message_WorkshopModeIsOff;
    }
    else
    {
      this.checkmarkReady.Checked = ((Control) this.runServiceButtonWMOff).Enabled = false;
      this.labelStatus.Text = Resources.Message_UnableToRunRoutine;
    }
  }

  private void runServiceButtonWMOff_ServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    int num = (int) MessageBox.Show(Resources.Message_CTPWorkshopModeHasBeenTurnedOffDiagnosticLinkNeedsToBeClosedAndTheVehicleInterfaceAdaptorNeedsToBeDisconnectedFromTheDiagnosticPort, Resources.Message_WorkshopModeOff, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
  }

  private void digitalReadoutInstrumentCtpWMMode_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.checkmarkReady = new Checkmark();
    this.labelStatus = new Label();
    this.buttonClose = new Button();
    this.digitalReadoutInstrumentCtpWMMode = new DigitalReadoutInstrument();
    this.runServiceButtonWMOff = new RunServiceButton();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkReady, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentCtpWMMode, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonWMOff, 2, 3);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.checkmarkReady, "checkmarkReady");
    ((Control) this.checkmarkReady).Name = "checkmarkReady";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelStatus, 3);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.digitalReadoutInstrumentCtpWMMode.Alignment = StringAlignment.Center;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentCtpWMMode, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCtpWMMode, "digitalReadoutInstrumentCtpWMMode");
    this.digitalReadoutInstrumentCtpWMMode.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCtpWMMode).FreezeValue = false;
    this.digitalReadoutInstrumentCtpWMMode.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentCtpWMMode.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentCtpWMMode.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentCtpWMMode.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentCtpWMMode.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentCtpWMMode.Gradient.Modify(2, 1.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCtpWMMode).Instrument = new Qualifier((QualifierTypes) 1, "CTP01T", "DT_STO_Workshop_Mode_Workshop_Mode");
    ((Control) this.digitalReadoutInstrumentCtpWMMode).Name = "digitalReadoutInstrumentCtpWMMode";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCtpWMMode).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCtpWMMode).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentCtpWMMode.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentCtpWMMode_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.runServiceButtonWMOff, "runServiceButtonWMOff");
    ((Control) this.runServiceButtonWMOff).Name = "runServiceButtonWMOff";
    this.runServiceButtonWMOff.ServiceCall = new ServiceCall("CTP01T", "DL_Workshop_Mode", (IEnumerable<string>) new string[1]
    {
      "Workshop_Mode=0"
    });
    this.runServiceButtonWMOff.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonWMOff_ServiceComplete);
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
