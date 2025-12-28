// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel;

public class UserPanel : CustomPanel
{
  private const int MaxDistanceInKm = 1310700;
  private Dictionary<string, UserPanel.SetupInformation> atsServices = new Dictionary<string, UserPanel.SetupInformation>();
  private string serviceRoutineInputUnits;
  private bool newValueWritten = false;
  private Channel mcmChannel;
  private Channel acmChannel;
  private Conversion uiToSr;
  private Conversion srToUi;
  private TableLayoutPanel tableLayoutPanelWholePanel;
  private Button buttonClose;
  private Button buttonSetAtsDistance;
  private TextBox textBoxNewAtsDistance;
  private TableLayoutPanel tableLayoutPanelDistance;
  private System.Windows.Forms.Label label1;
  private TableLayoutPanel tableLayoutPanelDirections;
  private TableLayoutPanel tableLayoutPanelCurrentValues;
  private DigitalReadoutInstrument digitalReadoutInstrumentCurrentMCMAtsDistance;
  private System.Windows.Forms.Label labelDirections;
  private Checkmark checkmarkReady;
  private RadioButton radioButtonCopyValueFromMCM;
  private RadioButton radioButtonCopyValueFromACM;
  private RadioButton radioButtonResetATSToZero;
  private RadioButton radioButtonEnterATSDistance;
  private System.Windows.Forms.Label label2;
  private DigitalReadoutInstrument digitalReadoutInstrumentCurrentACMAtsDistance;

  private float CustomDistance
  {
    get
    {
      float result;
      return !float.TryParse(this.textBoxNewAtsDistance.Text, out result) || (double) result < 0.0 || (double) result > this.srToUi.Convert(1310700.0) ? -1f : result;
    }
  }

  private string UiDistanceUnits
  {
    get => Converter.GlobalInstance.GetConversion(this.serviceRoutineInputUnits).OutputUnit;
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.atsServices.Add("MCM21T", new UserPanel.SetupInformation("RT_SR0EB_ATS_lifetime_ageing_strategy_Start(Distance_value={0})", "RT_SR012_Save_EOL_Data_Request_Start(save=2,e2p_block_nr=30)", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"));
    this.atsServices.Add("ACM21T", new UserPanel.SetupInformation("RT_SR02EB_ATS_lifetime_ageing_strategy_Start(Distance_value={0})", "RT_Save_EOL_Data_Start(Save_Type=2,Block_Type=30)", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"));
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    this.textBoxNewAtsDistance.TextChanged += new EventHandler(this.textBoxNewAtsDistance_TextChanged);
    this.textBoxNewAtsDistance.KeyPress += new KeyPressEventHandler(this.textBoxNewAtsDistance_KeyPress);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.UpdateUI();
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateChannel(this.GetChannel("MCM21T"), ref this.mcmChannel);
    this.UpdateChannel(this.GetChannel("ACM21T"), ref this.acmChannel);
    this.UpdateUI();
  }

  private void UpdateChannel(Channel newChannel, ref Channel channel)
  {
    if (channel == newChannel)
      return;
    if (channel != null)
      channel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.channel_ServiceCompleteEvent);
    channel = newChannel;
    if (channel != null)
    {
      channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.channel_ServiceCompleteEvent);
      this.serviceRoutineInputUnits = channel.Services[this.atsServices[channel.Ecu.Name].ReadService].Units;
      this.srToUi = Converter.GlobalInstance.GetConversion(this.serviceRoutineInputUnits, this.UiDistanceUnits);
      this.uiToSr = Converter.GlobalInstance.GetConversion(this.UiDistanceUnits, this.serviceRoutineInputUnits);
      this.RunService(channel, this.atsServices[channel.Ecu.Name].ReadService);
    }
  }

  private void channel_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
    {
      UserPanel.SetupInformation atsService = this.atsServices[service.Channel.Ecu.Name];
      if (atsService.WriteService.StartsWith(service.Qualifier))
      {
        Thread.Sleep(500);
        this.RunService(service.Channel, atsService.CommitService);
        this.RunService(service.Channel, atsService.ReadService);
        this.newValueWritten = true;
      }
    }
    this.UpdateUI();
  }

  private void RunService(Channel channel, string serviceQualifier)
  {
    if (channel == null)
      return;
    Service service = channel.Services[serviceQualifier];
    if (service != (Service) null)
    {
      service.InputValues.ParseValues(serviceQualifier);
      service.Execute(false);
    }
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.textBoxNewAtsDistance.TextChanged -= new EventHandler(this.textBoxNewAtsDistance_TextChanged);
    this.textBoxNewAtsDistance.KeyPress -= new KeyPressEventHandler(this.textBoxNewAtsDistance_KeyPress);
    this.UpdateChannel((Channel) null, ref this.mcmChannel);
    this.UpdateChannel((Channel) null, ref this.acmChannel);
  }

  private void UpdateUI()
  {
    bool flag1 = false;
    bool flag2 = false;
    if (this.mcmChannel == null || !this.mcmChannel.Online || ((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentMCMAtsDistance).DataItem.Value == null)
    {
      this.labelDirections.Text = Resources.Message_MCMNotOnline;
      this.newValueWritten = false;
    }
    else if (this.acmChannel == null || !this.acmChannel.Online || ((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentACMAtsDistance).DataItem.Value == null)
    {
      this.labelDirections.Text = Resources.Message_ACMNotOnline;
      this.newValueWritten = false;
    }
    else
    {
      flag2 = true;
      this.labelDirections.Text = this.newValueWritten ? Resources.Message_ATSSet : Resources.Message_ChooseAction;
      if ((double) this.CustomDistance >= 0.0)
      {
        this.textBoxNewAtsDistance.BackColor = SystemColors.Window;
        flag1 = true;
      }
      else
      {
        this.textBoxNewAtsDistance.BackColor = Color.LightPink;
        if (this.radioButtonEnterATSDistance.Checked)
          this.labelDirections.Text = string.Format(Resources.MessageFormat_PleaseSpecifyAnATSDistanceBetween0And01, (object) (int) this.srToUi.Convert(1310700.0), (object) this.UiDistanceUnits);
      }
    }
    this.radioButtonCopyValueFromACM.Enabled = this.radioButtonCopyValueFromMCM.Enabled = this.radioButtonEnterATSDistance.Enabled = this.radioButtonResetATSToZero.Enabled = flag2;
    this.textBoxNewAtsDistance.Enabled = this.radioButtonEnterATSDistance.Checked;
    this.buttonSetAtsDistance.Enabled = flag1;
    this.checkmarkReady.CheckState = flag1 ? CheckState.Checked : CheckState.Unchecked;
  }

  private void textBoxNewAtsDistance_KeyPress(object sender, KeyPressEventArgs e)
  {
    e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && (e.KeyChar != '.' || this.textBoxNewAtsDistance.Text.Contains<char>('.'));
  }

  private void textBoxNewAtsDistance_TextChanged(object sender, EventArgs e)
  {
    this.newValueWritten = false;
    this.UpdateUI();
  }

  private void buttonSetAtsDistance_Click(object sender, EventArgs e)
  {
    if ((double) this.CustomDistance < 0.0)
      return;
    this.labelDirections.Text = string.Format(Resources.MessageFormat_SettingDistanceTo01, (object) this.CustomDistance, (object) this.UiDistanceUnits);
    this.RunService(this.mcmChannel, string.Format(this.atsServices[this.mcmChannel.Ecu.Name].WriteService, (object) (int) this.uiToSr.Convert((double) this.CustomDistance)));
    this.RunService(this.acmChannel, string.Format(this.atsServices[this.acmChannel.Ecu.Name].WriteService, (object) (int) this.uiToSr.Convert((double) this.CustomDistance)));
  }

  private void radioButtonCopyValueFromACM_CheckedChanged(object sender, EventArgs e)
  {
    if (!(sender as RadioButton).Checked || this.digitalReadoutInstrumentCurrentACMAtsDistance.RepresentedState != 1)
      return;
    this.textBoxNewAtsDistance.Text = this.srToUi.Convert(((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentACMAtsDistance).DataItem.Value).ToString("F1");
  }

  private void radioButtonCopyValueFromMCM_CheckedChanged(object sender, EventArgs e)
  {
    if (!(sender as RadioButton).Checked || this.digitalReadoutInstrumentCurrentMCMAtsDistance.RepresentedState != 1)
      return;
    this.textBoxNewAtsDistance.Text = this.srToUi.Convert(((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentMCMAtsDistance).DataItem.Value).ToString("F1");
  }

  private void radioButtonResetATSToZero_CheckedChanged(object sender, EventArgs e)
  {
    if (!(sender as RadioButton).Checked)
      return;
    this.textBoxNewAtsDistance.Text = "0";
  }

  private void radioButtonEnterATSDistance_CheckedChanged(object sender, EventArgs e)
  {
    this.textBoxNewAtsDistance.Enabled = this.radioButtonEnterATSDistance.Checked;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelCurrentValues = new TableLayoutPanel();
    this.tableLayoutPanelDirections = new TableLayoutPanel();
    this.tableLayoutPanelDistance = new TableLayoutPanel();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.buttonSetAtsDistance = new Button();
    this.textBoxNewAtsDistance = new TextBox();
    this.label1 = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.radioButtonCopyValueFromMCM = new RadioButton();
    this.radioButtonCopyValueFromACM = new RadioButton();
    this.radioButtonResetATSToZero = new RadioButton();
    this.radioButtonEnterATSDistance = new RadioButton();
    this.label2 = new System.Windows.Forms.Label();
    this.labelDirections = new System.Windows.Forms.Label();
    this.checkmarkReady = new Checkmark();
    this.digitalReadoutInstrumentCurrentMCMAtsDistance = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCurrentACMAtsDistance = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this.tableLayoutPanelDistance).SuspendLayout();
    ((Control) this.tableLayoutPanelDirections).SuspendLayout();
    ((Control) this.tableLayoutPanelCurrentValues).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelDistance, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelDirections, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelCurrentValues, 0, 0);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelDistance, "tableLayoutPanelDistance");
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.buttonSetAtsDistance, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.textBoxNewAtsDistance, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.label1, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.buttonClose, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.radioButtonCopyValueFromMCM, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.radioButtonCopyValueFromACM, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.radioButtonResetATSToZero, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.radioButtonEnterATSDistance, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelDistance).Controls.Add((Control) this.label2, 0, 0);
    ((Control) this.tableLayoutPanelDistance).Name = "tableLayoutPanelDistance";
    componentResourceManager.ApplyResources((object) this.buttonSetAtsDistance, "buttonSetAtsDistance");
    this.buttonSetAtsDistance.Name = "buttonSetAtsDistance";
    this.buttonSetAtsDistance.UseCompatibleTextRendering = true;
    this.buttonSetAtsDistance.UseVisualStyleBackColor = true;
    this.buttonSetAtsDistance.Click += new EventHandler(this.buttonSetAtsDistance_Click);
    componentResourceManager.ApplyResources((object) this.textBoxNewAtsDistance, "textBoxNewAtsDistance");
    this.textBoxNewAtsDistance.Name = "textBoxNewAtsDistance";
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.radioButtonCopyValueFromMCM, "radioButtonCopyValueFromMCM");
    ((TableLayoutPanel) this.tableLayoutPanelDistance).SetColumnSpan((Control) this.radioButtonCopyValueFromMCM, 4);
    this.radioButtonCopyValueFromMCM.Name = "radioButtonCopyValueFromMCM";
    this.radioButtonCopyValueFromMCM.TabStop = true;
    this.radioButtonCopyValueFromMCM.UseCompatibleTextRendering = true;
    this.radioButtonCopyValueFromMCM.UseVisualStyleBackColor = true;
    this.radioButtonCopyValueFromMCM.CheckedChanged += new EventHandler(this.radioButtonCopyValueFromMCM_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.radioButtonCopyValueFromACM, "radioButtonCopyValueFromACM");
    ((TableLayoutPanel) this.tableLayoutPanelDistance).SetColumnSpan((Control) this.radioButtonCopyValueFromACM, 4);
    this.radioButtonCopyValueFromACM.Name = "radioButtonCopyValueFromACM";
    this.radioButtonCopyValueFromACM.TabStop = true;
    this.radioButtonCopyValueFromACM.UseCompatibleTextRendering = true;
    this.radioButtonCopyValueFromACM.UseVisualStyleBackColor = true;
    this.radioButtonCopyValueFromACM.CheckedChanged += new EventHandler(this.radioButtonCopyValueFromACM_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.radioButtonResetATSToZero, "radioButtonResetATSToZero");
    ((TableLayoutPanel) this.tableLayoutPanelDistance).SetColumnSpan((Control) this.radioButtonResetATSToZero, 4);
    this.radioButtonResetATSToZero.Name = "radioButtonResetATSToZero";
    this.radioButtonResetATSToZero.TabStop = true;
    this.radioButtonResetATSToZero.UseCompatibleTextRendering = true;
    this.radioButtonResetATSToZero.UseVisualStyleBackColor = true;
    this.radioButtonResetATSToZero.CheckedChanged += new EventHandler(this.radioButtonResetATSToZero_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.radioButtonEnterATSDistance, "radioButtonEnterATSDistance");
    ((TableLayoutPanel) this.tableLayoutPanelDistance).SetColumnSpan((Control) this.radioButtonEnterATSDistance, 4);
    this.radioButtonEnterATSDistance.Name = "radioButtonEnterATSDistance";
    this.radioButtonEnterATSDistance.TabStop = true;
    this.radioButtonEnterATSDistance.UseCompatibleTextRendering = true;
    this.radioButtonEnterATSDistance.UseVisualStyleBackColor = true;
    this.radioButtonEnterATSDistance.CheckedChanged += new EventHandler(this.radioButtonEnterATSDistance_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    ((TableLayoutPanel) this.tableLayoutPanelDistance).SetColumnSpan((Control) this.label2, 4);
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelDirections, "tableLayoutPanelDirections");
    ((TableLayoutPanel) this.tableLayoutPanelDirections).Controls.Add((Control) this.labelDirections, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelDirections).Controls.Add((Control) this.checkmarkReady, 0, 0);
    ((Control) this.tableLayoutPanelDirections).Name = "tableLayoutPanelDirections";
    componentResourceManager.ApplyResources((object) this.labelDirections, "labelDirections");
    this.labelDirections.Name = "labelDirections";
    this.labelDirections.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkReady, "checkmarkReady");
    ((Control) this.checkmarkReady).Name = "checkmarkReady";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelCurrentValues, "tableLayoutPanelCurrentValues");
    ((TableLayoutPanel) this.tableLayoutPanelCurrentValues).Controls.Add((Control) this.digitalReadoutInstrumentCurrentMCMAtsDistance, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelCurrentValues).Controls.Add((Control) this.digitalReadoutInstrumentCurrentACMAtsDistance, 2, 0);
    ((Control) this.tableLayoutPanelCurrentValues).Name = "tableLayoutPanelCurrentValues";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCurrentMCMAtsDistance, "digitalReadoutInstrumentCurrentMCMAtsDistance");
    this.digitalReadoutInstrumentCurrentMCMAtsDistance.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentMCMAtsDistance).FreezeValue = false;
    this.digitalReadoutInstrumentCurrentMCMAtsDistance.Gradient.Initialize((ValueState) 0, 1);
    this.digitalReadoutInstrumentCurrentMCMAtsDistance.Gradient.Modify(1, 0.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentMCMAtsDistance).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "MCM21T", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven");
    ((Control) this.digitalReadoutInstrumentCurrentMCMAtsDistance).Name = "digitalReadoutInstrumentCurrentMCMAtsDistance";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentMCMAtsDistance).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCurrentACMAtsDistance, "digitalReadoutInstrumentCurrentACMAtsDistance");
    this.digitalReadoutInstrumentCurrentACMAtsDistance.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentACMAtsDistance).FreezeValue = false;
    this.digitalReadoutInstrumentCurrentACMAtsDistance.Gradient.Initialize((ValueState) 0, 1);
    this.digitalReadoutInstrumentCurrentACMAtsDistance.Gradient.Modify(1, 0.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentACMAtsDistance).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM21T", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven");
    ((Control) this.digitalReadoutInstrumentCurrentACMAtsDistance).Name = "digitalReadoutInstrumentCurrentACMAtsDistance";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCurrentACMAtsDistance).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).PerformLayout();
    ((Control) this.tableLayoutPanelDistance).ResumeLayout(false);
    ((Control) this.tableLayoutPanelDistance).PerformLayout();
    ((Control) this.tableLayoutPanelDirections).ResumeLayout(false);
    ((Control) this.tableLayoutPanelCurrentValues).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private class SetupInformation
  {
    public readonly string ReadService;
    public readonly string WriteService;
    public readonly string CommitService;

    public SetupInformation(string writeService, string commitService, string readService)
    {
      this.WriteService = writeService;
      this.CommitService = commitService;
      this.ReadService = readService;
    }
  }
}
