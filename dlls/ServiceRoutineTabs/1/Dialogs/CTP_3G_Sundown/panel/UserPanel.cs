// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_3G_Sundown.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_3G_Sundown.panel;

public class UserPanel : CustomPanel
{
  private const string WifiSsidPrefixQualifier = "WIFI_SSID_prefix";
  private string PadCharacter = char.ConvertFromUtf32((int) byte.MaxValue);
  private List<Control> parameterControls = new List<Control>();
  private Channel ctp;
  private bool writingParameters = false;
  private bool dataChanged = false;
  private int parametersRead = 0;
  private Dictionary<string, string> eventInfos = new Dictionary<string, string>();
  private TableLayoutPanel tableLayoutPanelMain;
  private ScalingLabel scalingLabelStatus;
  private Checkmark checkmarkStatus;
  private Button buttonClose;
  private Button buttonSave;
  private TextBox textBoxSSID;
  private ComboBox comboBoxWifiMode;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label label2;
  private System.Windows.Forms.Label label3;
  private System.Windows.Forms.Label label4;
  private TextBox textBoxWPA2KEY;
  private ComboBox comboBoxIpConfiguration;

  private string[] ParameterQualifiers
  {
    get
    {
      return this.parameterControls.Select<Control, string>((Func<Control, string>) (pc => (string) pc.Tag)).ToArray<string>();
    }
  }

  private bool CtpReady
  {
    get => this.ctp != null && this.ctp.CommunicationsState == CommunicationsState.Online;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
  }

  public UserPanel()
  {
    this.InitializeComponent();
    foreach (Control control in (ArrangedElementCollection) ((TableLayoutPanel) this.tableLayoutPanelMain).Controls)
    {
      if (!string.IsNullOrEmpty((string) control.Tag))
        this.parameterControls.Add(control);
    }
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && this.writingParameters)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.SetCTP((Channel) null);
  }

  public virtual void OnChannelsChanged() => this.SetCTP(this.GetChannel("CTP01T"));

  private void SetCTP(Channel ctp)
  {
    if (this.ctp != ctp)
    {
      this.parametersRead = 0;
      if (this.ctp != null)
      {
        this.ctp.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        this.ctp.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
        this.ctp.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.ctp_CommunicationsStateUpdateEvent);
      }
      this.ctp = ctp;
      if (this.ctp != null)
      {
        this.ctp.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        this.ctp.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
        this.ctp.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.ctp_CommunicationsStateUpdateEvent);
        this.ReadParameters();
      }
    }
    this.UpdateUserInterface();
  }

  private void ctp_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void ReadParameters()
  {
    this.parametersRead = 0;
    foreach (string parameterQualifier in this.ParameterQualifiers)
    {
      Parameter parameter = this.ctp.Parameters[parameterQualifier];
      if (parameter != null)
        this.ctp.Parameters.ReadGroup(parameter.GroupQualifier, true, false);
    }
  }

  private void UpdateUserInterface()
  {
    if (this.CtpReady)
    {
      ((Control) this.scalingLabelStatus).Text = Resources.Message_Ready;
      this.checkmarkStatus.CheckState = CheckState.Checked;
    }
    else if (this.writingParameters)
    {
      ((Control) this.scalingLabelStatus).Text = Resources.Message_WritingParameters;
      this.checkmarkStatus.CheckState = CheckState.Indeterminate;
    }
    else if (!this.CtpReady && this.ctp != null)
    {
      ((Control) this.scalingLabelStatus).Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_CTPBusy0, (object) this.ctp.CommunicationsState.ToString());
      this.checkmarkStatus.CheckState = CheckState.Unchecked;
    }
    else
    {
      ((Control) this.scalingLabelStatus).Text = Resources.Message_CTPBusy;
      this.checkmarkStatus.CheckState = CheckState.Unchecked;
    }
    this.buttonSave.Enabled = this.CtpReady && !this.writingParameters && this.dataChanged && this.parametersRead == this.parameterControls.Count;
    this.buttonClose.Enabled = this.ctp == null || !this.writingParameters;
  }

  private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    this.writingParameters = false;
    if (e.Succeeded)
      this.dataChanged = false;
    else
      ((Control) this.scalingLabelStatus).Text = Resources.Message_ErrorWritingParameters;
    this.AddStationLogEntry();
    this.UpdateUserInterface();
  }

  private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    ++this.parametersRead;
    foreach (string parameterQualifier in this.ParameterQualifiers)
      this.ParameterToControl(parameterQualifier);
    this.dataChanged = false;
    this.UpdateUserInterface();
  }

  private void ParameterToControl(string parameterQualifier)
  {
    Control control = this.parameterControls.FirstOrDefault<Control>((Func<Control, bool>) (c => (string) c.Tag == parameterQualifier));
    Parameter parameter = this.ctp != null ? this.ctp.Parameters[parameterQualifier] : (Parameter) null;
    if (parameter == null || parameter.Value == null || control == null)
      return;
    if (control.GetType() == typeof (ComboBox))
    {
      ((ComboBox) control).Items.Clear();
      ((ComboBox) control).Items.AddRange((object[]) parameter.Choices.Select<Choice, string>((Func<Choice, string>) (p => p.Name)).ToArray<string>());
      ((ListControl) control).SelectedIndex = ((Choice) parameter.Value).Index;
    }
    else if (control.GetType() == typeof (TextBox))
      control.Text = parameter.Value.ToString().Replace(this.PadCharacter, string.Empty).TrimStart(' ');
  }

  private void ControlToParameter(string parameterQualifier)
  {
    Control control = this.parameterControls.FirstOrDefault<Control>((Func<Control, bool>) (c => (string) c.Tag == parameterQualifier));
    Parameter parameter = this.ctp != null ? this.ctp.Parameters[parameterQualifier] : (Parameter) null;
    if (parameter == null || control == null)
      return;
    if (control.GetType() == typeof (ComboBox))
    {
      parameter.Value = (object) parameter.Choices[((ListControl) control).SelectedIndex];
      this.eventInfos.Add(((IEnumerable<string>) parameter.Qualifier.Replace("_", string.Empty).Split('.')).Last<string>(), parameter.Value.ToString());
    }
    else if (control.GetType() == typeof (TextBox))
    {
      parameter.Value = (object) control.Text.PadRight(32 /*0x20*/, this.PadCharacter[0]);
      this.eventInfos.Add(((IEnumerable<string>) parameter.Qualifier.Replace("_", string.Empty).Split('.')).Last<string>(), control.Text);
    }
  }

  private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.dataChanged = true;
    this.UpdateUserInterface();
  }

  private void textBox_TextChanged(object sender, EventArgs e)
  {
    this.dataChanged = true;
    this.UpdateUserInterface();
  }

  private void buttonSave_Click(object sender, EventArgs e)
  {
    if (!this.CtpReady)
      return;
    this.eventInfos.Clear();
    foreach (string parameterQualifier in this.ParameterQualifiers)
      this.ControlToParameter(parameterQualifier);
    Parameter parameter = this.ctp != null ? this.ctp.Parameters["WIFI_SSID_prefix"] : (Parameter) null;
    if (parameter != null)
      parameter.Value = (object) parameter.Value.ToString().PadRight(32 /*0x20*/, this.PadCharacter[0]);
    this.writingParameters = true;
    this.ctp.Parameters.Write(false);
  }

  private void AddStationLogEntry()
  {
    this.eventInfos["reasontext"] = "ReasonCTP3GSundown";
    ServerDataManager.UpdateEventsFile(this.ctp, (IDictionary<string, string>) this.eventInfos, (IDictionary<string, string>) new Dictionary<string, string>()
    {
      ["CTP_Serial_Number"] = this.ReadEcuInfoData("CO_EcuSerialNumber")
    }, "CTP3GSundown", string.Empty, this.ReadEcuInfoData("CO_VIN"), "OK", "DESCRIPTION", string.Empty);
  }

  private string ReadEcuInfoData(string qualifier)
  {
    string str = string.Empty;
    if (this.ctp != null)
    {
      EcuInfo ecuInfo = this.ctp.EcuInfos[qualifier] ?? this.ctp.EcuInfos.GetItemContaining(qualifier);
      if (ecuInfo != null)
        str = ecuInfo.Value.ToString().Trim();
    }
    return str.Trim();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.scalingLabelStatus = new ScalingLabel();
    this.checkmarkStatus = new Checkmark();
    this.buttonClose = new Button();
    this.buttonSave = new Button();
    this.comboBoxIpConfiguration = new ComboBox();
    this.label1 = new System.Windows.Forms.Label();
    this.comboBoxWifiMode = new ComboBox();
    this.label2 = new System.Windows.Forms.Label();
    this.label3 = new System.Windows.Forms.Label();
    this.label4 = new System.Windows.Forms.Label();
    this.textBoxSSID = new TextBox();
    this.textBoxWPA2KEY = new TextBox();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.scalingLabelStatus, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.checkmarkStatus, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClose, 5, 6);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonSave, 4, 6);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.comboBoxIpConfiguration, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.label1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.comboBoxWifiMode, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.label2, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.label3, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.label4, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxSSID, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxWPA2KEY, 2, 3);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    this.scalingLabelStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.scalingLabelStatus, 5);
    componentResourceManager.ApplyResources((object) this.scalingLabelStatus, "scalingLabelStatus");
    this.scalingLabelStatus.FontGroup = (string) null;
    this.scalingLabelStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelStatus).Name = "scalingLabelStatus";
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    this.checkmarkStatus.IndeterminateImage = (Image) componentResourceManager.GetObject("checkmarkStatus.IndeterminateImage");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonSave, "buttonSave");
    this.buttonSave.Name = "buttonSave";
    this.buttonSave.UseCompatibleTextRendering = true;
    this.buttonSave.UseVisualStyleBackColor = true;
    this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.comboBoxIpConfiguration, 4);
    componentResourceManager.ApplyResources((object) this.comboBoxIpConfiguration, "comboBoxIpConfiguration");
    this.comboBoxIpConfiguration.FormattingEnabled = true;
    this.comboBoxIpConfiguration.Name = "comboBoxIpConfiguration";
    this.comboBoxIpConfiguration.Tag = (object) "VCD_PID_WIFI_BACKEND_MODE.IP_Configuration";
    this.comboBoxIpConfiguration.SelectedIndexChanged += new EventHandler(this.comboBox_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.comboBoxWifiMode, 4);
    componentResourceManager.ApplyResources((object) this.comboBoxWifiMode, "comboBoxWifiMode");
    this.comboBoxWifiMode.FormattingEnabled = true;
    this.comboBoxWifiMode.Name = "comboBoxWifiMode";
    this.comboBoxWifiMode.Tag = (object) "Wifi_mode";
    this.comboBoxWifiMode.SelectedIndexChanged += new EventHandler(this.comboBox_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    componentResourceManager.ApplyResources((object) this.label4, "label4");
    this.label4.Name = "label4";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.textBoxSSID, 4);
    componentResourceManager.ApplyResources((object) this.textBoxSSID, "textBoxSSID");
    this.textBoxSSID.Name = "textBoxSSID";
    this.textBoxSSID.Tag = (object) "SSID";
    this.textBoxSSID.TextChanged += new EventHandler(this.textBox_TextChanged);
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.textBoxWPA2KEY, 4);
    componentResourceManager.ApplyResources((object) this.textBoxWPA2KEY, "textBoxWPA2KEY");
    this.textBoxWPA2KEY.Name = "textBoxWPA2KEY";
    this.textBoxWPA2KEY.Tag = (object) "WPA2KEY";
    this.textBoxWPA2KEY.TextChanged += new EventHandler(this.textBox_TextChanged);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_3G_Sundown_Service_Campaign");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
