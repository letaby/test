// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Replace_Quantity_Control_Valve.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Replace_Quantity_Control_Valve.panel;

public class UserPanel : CustomPanel
{
  private const string fmuAdaptationPositive = "Quantity_Control_Valve_Adaptation_Positive";
  private const string fmuAdaptationNegative = "Quantity_Control_Valve_Adaptation_Negative";
  private const string setEolDefaultService = "RT_SR014_SET_EOL_Default_Values_Start";
  private const int resetChoice = 25;
  private Channel channel;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private TableLayoutPanel tableLayoutPanel2;
  private Button buttonResetLearntData;
  private Button readAccumulatorsButton;
  private Button buttonClose;

  public UserPanel()
  {
    this.InitializeComponent();
    this.buttonResetLearntData.Click += new EventHandler(this.OnResetLearntDataClick);
    this.readAccumulatorsButton.Click += new EventHandler(this.OnReadAccumulatorsClick);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.channel = (Channel) null;
  }

  public virtual void OnChannelsChanged()
  {
    if (this.channel != null)
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.channel = this.GetChannel("MCM");
    if (this.channel == null)
      return;
    this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.ReadParameters();
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.channel == null)
      return;
    this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    bool flag = false;
    if (this.channel != null)
      flag = this.channel.CommunicationsState == CommunicationsState.Online;
    this.buttonResetLearntData.Enabled = flag;
    this.readAccumulatorsButton.Enabled = flag;
  }

  private void ReadParameters()
  {
    if (this.channel == null)
      return;
    this.ReadParameter(this.channel.Parameters["Quantity_Control_Valve_Adaptation_Positive"]);
    this.ReadParameter(this.channel.Parameters["Quantity_Control_Valve_Adaptation_Negative"]);
  }

  private void ReadParameter(Parameter parameter)
  {
    if (parameter == null)
      return;
    string groupQualifier = parameter.GroupQualifier;
    parameter.Channel.Parameters.ReadGroup(groupQualifier, false, false);
  }

  private void OnReadAccumulatorsClick(object sender, EventArgs e) => this.ReadParameters();

  private void OnResetLearntDataClick(object sender, EventArgs e)
  {
    if (this.channel == null || DialogResult.Yes != MessageBox.Show(Resources.Message_AreYouSureYouWantToResetTheLearnedData, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
      return;
    Service service = this.channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
    if (service != (Service) null)
    {
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 25);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      service.Execute(false);
    }
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    if (this.channel != null)
    {
      Service service = this.channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
      if (service != (Service) null)
        service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    }
    this.ReadParameters();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.buttonResetLearntData = new Button();
    this.buttonClose = new Button();
    this.readAccumulatorsButton = new Button();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonResetLearntData, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.readAccumulatorsButton, 1, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.buttonResetLearntData, "buttonResetLearntData");
    this.buttonResetLearntData.Name = "buttonResetLearntData";
    this.buttonResetLearntData.UseCompatibleTextRendering = true;
    this.buttonResetLearntData.UseVisualStyleBackColor = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.readAccumulatorsButton, "readAccumulatorsButton");
    this.readAccumulatorsButton.Name = "readAccumulatorsButton";
    this.readAccumulatorsButton.UseCompatibleTextRendering = true;
    this.readAccumulatorsButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 4, "MCM", "Quantity_Control_Valve_Adaptation_Positive");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 4, "MCM", "Quantity_Control_Valve_Adaptation_Negative");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_ReplaceQuantityControlValve");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }
}
