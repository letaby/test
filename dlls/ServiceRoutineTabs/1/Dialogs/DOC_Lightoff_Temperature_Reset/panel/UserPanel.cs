// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Lightoff_Temperature_Reset.panel.UserPanel
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
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Lightoff_Temperature_Reset.panel;

public class UserPanel : CustomPanel
{
  private RunServiceButton runServiceButton;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonClose;
  private DigitalReadoutInstrument digitalRedoutInstrumentEngineSpeed;
  private TableLayoutPanel tableLayoutPanel2;
  private Checkmark checkmark1;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelStatus;
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
      int num2 = (int) MessageBox.Show((IWin32Window) this, Resources.Message_TheValueWasSuccessfullyChanged, ApplicationInformation.ProductName, MessageBoxButtons.OK);
    }
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = !this.CanClose;
  }

  private void UpdateUserInterface()
  {
    ((Control) this.runServiceButton).Enabled = this.CanStart;
    this.checkmark1.Checked = this.CanStart;
    ((Control) this.labelStatus).Text = this.CanStart ? Resources.Message_ProcedureReady : Resources.Message_TurnEngineOff;
    this.buttonClose.Enabled = this.CanClose;
  }

  private bool CanStart
  {
    get
    {
      return !((RunSharedProcedureButtonBase) this.runServiceButton).IsBusy && this.digitalRedoutInstrumentEngineSpeed.RepresentedState == 1;
    }
  }

  private bool CanClose => !((RunSharedProcedureButtonBase) this.runServiceButton).IsBusy;

  private void digitalRedoutInstrumentEngineSpeed_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.runServiceButton = new RunServiceButton();
    this.label1 = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.digitalRedoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.checkmark1 = new Checkmark();
    this.labelStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButton, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalRedoutInstrumentEngineSpeed, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((ContainerControl) this.runServiceButton).AutoValidate = AutoValidate.EnablePreventFocusChange;
    componentResourceManager.ApplyResources((object) this.runServiceButton, "runServiceButton");
    ((Control) this.runServiceButton).Name = "runServiceButton";
    this.runServiceButton.ServiceCall = new ServiceCall("ACM21T", "RT_SR0D4_Reset_Lightoff_Enhancer_Temp_Start");
    this.runServiceButton.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label1, 2);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.digitalRedoutInstrumentEngineSpeed, "digitalRedoutInstrumentEngineSpeed");
    this.digitalRedoutInstrumentEngineSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalRedoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalRedoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalRedoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalRedoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS001_Engine_Speed");
    ((Control) this.digitalRedoutInstrumentEngineSpeed).Name = "digitalRedoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalRedoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalRedoutInstrumentEngineSpeed.RepresentedStateChanged += new EventHandler(this.digitalRedoutInstrumentEngineSpeed_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmark1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    this.labelStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    ((Control) this.labelStatus).Name = "labelStatus";
    this.labelStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_DOC_Lightoff_Temeperature_Reset");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
