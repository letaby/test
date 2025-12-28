// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.ConnectionOptionsPanel
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class ConnectionOptionsPanel : OptionsPanel
{
  private SapiManager sapi;
  private IContainer components;
  private ComboBox comboBoxHardware;
  private ToolTip toolTip;
  private Button openSidConfigure;
  private GroupBox groupBox1;
  private PictureBox pictureBoxRollcallSupportWarning;
  private Label labelRollcallSupport;
  private CheckBox checkBoxRollCall;
  private Label labelDevice;
  private TextBox textBoxSelectedDevice;
  private TableLayoutPanel tableLayoutPanelVCI;
  private CheckBox checkBoxPowertrainOnly;
  private GroupBox groupSelectionPanel;
  private TableLayoutPanel tableLayoutPanelProprietaryDevices;
  private Button selectNoneButton;
  private Button selectAllButton;
  private Button resetToDefaultsButton;
  private Panel panelTableLayoutAutoConnect;
  private TableLayoutPanel tableLayoutPanelAutoConnect;
  private CheckBox checkBoxUseMcd;
  private CheckBox checkBoxDoIPAutoConnect;
  private TableLayoutPanel tableLayoutPanelRollCall;
  private CheckBox checkBoxAllowAutoBaud;

  public ConnectionOptionsPanel()
  {
    this.MinAccessLevel = 1;
    this.InitializeComponent();
    this.sapi = SapiManager.GlobalInstance;
    this.sapi.Sapi.Ecus.EcusUpdateEvent += new EcusUpdateEventHandler(this.OnEcusUpdate);
    this.HeaderImage = (Image) new Bitmap(typeof (ConnectionOptionsPanel), "option_connection.png");
    this.tableLayoutPanelAutoConnect.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
  }

  private void OnEcusUpdate(object sender, EventArgs e) => this.BuildConnectionList();

  protected override void OnLoad(EventArgs e)
  {
    this.BuildHardwareList();
    this.BuildConnectionList();
    this.UpdateSelectedTranslator();
    this.checkBoxUseMcd.Checked = SapiManager.GlobalInstance.UseMcd;
    this.checkBoxUseMcd.Enabled = SapiManager.GlobalInstance.McdAvailable;
    this.checkBoxDoIPAutoConnect.Visible = ApplicationInformation.CanRollCallDoIP;
    this.checkBoxDoIPAutoConnect.Enabled = this.checkBoxUseMcd.Checked;
    this.checkBoxDoIPAutoConnect.Checked = this.checkBoxUseMcd.Checked && SapiManager.GlobalInstance.DoIPRollCallEnabled;
    this.checkBoxAllowAutoBaud.Visible = ApplicationInformation.CanDisableAutoBaud;
    this.checkBoxAllowAutoBaud.Enabled = SapiManager.GlobalInstance.IsAutoBaudCapable;
    this.checkBoxAllowAutoBaud.Checked = this.checkBoxAllowAutoBaud.Enabled && SapiManager.GlobalInstance.AllowAutoBaud;
    this.tableLayoutPanelVCI.SetRowSpan((Control) this.checkBoxUseMcd, this.checkBoxAllowAutoBaud.Visible ? 1 : 2);
    base.OnLoad(e);
  }

  private void selectNoneButton_Click(object sender, EventArgs e)
  {
    CheckBox[] array = this.tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>().Where<CheckBox>((Func<CheckBox, bool>) (x => x.Visible && x.Checked)).ToArray<CheckBox>();
    foreach (CheckBox box in array)
      this.SetCheckState(box, CheckState.Unchecked);
    if (!((IEnumerable<CheckBox>) array).Any<CheckBox>())
      return;
    this.MarkDirty();
  }

  private void selectAllButton_Click(object sender, EventArgs e)
  {
    CheckBox[] array = this.tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>().Where<CheckBox>((Func<CheckBox, bool>) (x => x.Visible && !x.Checked)).ToArray<CheckBox>();
    foreach (CheckBox box in array)
      this.SetCheckState(box, CheckState.Checked);
    if (!((IEnumerable<CheckBox>) array).Any<CheckBox>())
      return;
    this.MarkDirty();
  }

  private void resetToDefaultsButton_Click(object sender, EventArgs e)
  {
    CheckBox[] array = this.tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>().Where<CheckBox>((Func<CheckBox, bool>) (x => x.Visible)).ToArray<CheckBox>();
    bool flag = false;
    foreach (CheckBox checkBox in array)
    {
      CheckBox cb = checkBox;
      CheckState state = ((IEnumerable<Ecu>) Sapi.GetSapi().Ecus).Where<Ecu>((Func<Ecu, bool>) (ecu => ecu.Identifier == cb.Name)).Any<Ecu>((Func<Ecu, bool>) (ecu => SapiManager.IsDefaultAutoConnect(ecu))) ? CheckState.Checked : CheckState.Unchecked;
      if (cb.CheckState != state)
      {
        this.SetCheckState(cb, state);
        flag = true;
      }
    }
    if (!flag)
      return;
    this.MarkDirty();
  }

  private void BuildHardwareList()
  {
    this.comboBoxHardware.Items.Clear();
    foreach (Choice hardwareType in (ReadOnlyCollection<Choice>) this.sapi.HardwareTypes)
    {
      this.comboBoxHardware.Items.Add((object) hardwareType);
      if (hardwareType.Name == this.sapi.HardwareType)
        this.comboBoxHardware.SelectedItem = (object) hardwareType;
    }
    this.UpdateSidConfigureButton();
  }

  private bool ConfirmRestartConnection(bool allowCancel)
  {
    return (this.sapi.ActiveChannels == null || ((ReadOnlyCollection<Channel>) this.sapi.ActiveChannels).Count == 0) && (this.sapi.LogFileAllChannels == null || ((ReadOnlyCollection<Channel>) this.sapi.LogFileAllChannels).Count == 0) || MessageBox.Show(Resources.MessageConnectionsAndLogFilesClosedBeforeChangesApplied, ApplicationInformation.ProductName, allowCancel ? MessageBoxButtons.OKCancel : MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2, ControlHelpers.IsRightToLeft((Control) this) ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : (MessageBoxOptions) 0) == DialogResult.OK;
  }

  public override bool ApplySettings()
  {
    bool flag1 = true;
    if (this.IsDirty)
    {
      Cursor.Current = Cursors.WaitCursor;
      bool flag2 = false;
      Choice selectedItem = (Choice) this.comboBoxHardware.SelectedItem;
      if (selectedItem.Name != this.sapi.HardwareType || this.checkBoxUseMcd.Checked != SapiManager.GlobalInstance.UseMcd)
      {
        if (this.ConfirmRestartConnection(true))
        {
          flag2 = true;
          if (selectedItem.Name != this.sapi.HardwareType)
            this.sapi.SetHardwareType(selectedItem.Name, false);
          if (this.checkBoxUseMcd.Checked != SapiManager.GlobalInstance.UseMcd)
            SapiManager.GlobalInstance.UseMcd = this.checkBoxUseMcd.Checked;
        }
        else
          flag1 = false;
      }
      if (flag1)
      {
        bool flag3 = false;
        foreach (CheckBox checkBox in this.tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>())
        {
          switch (checkBox.CheckState)
          {
            case CheckState.Checked:
              this.sapi.SetAutoConnect(checkBox.Name, true, false);
              flag3 = true;
              continue;
            case CheckState.Indeterminate:
              continue;
            default:
              this.sapi.SetAutoConnect(checkBox.Name, false, false);
              flag3 = true;
              continue;
          }
        }
        SapiManager.GlobalInstance.RollCallEnabled = SapiManager.GlobalInstance.MonitoringEnabled = this.checkBoxRollCall.Checked;
        if (this.checkBoxRollCall.Checked)
          flag3 = true;
        SapiManager.GlobalInstance.RollCallPowertrainOnly = this.checkBoxPowertrainOnly.Checked;
        SapiManager.GlobalInstance.DoIPRollCallEnabled = SapiManager.GlobalInstance.DoIPMonitoringEnabled = this.checkBoxDoIPAutoConnect.Checked;
        if (this.checkBoxDoIPAutoConnect.Checked)
          flag3 = true;
        if (this.checkBoxAllowAutoBaud.Enabled)
          SapiManager.GlobalInstance.AllowAutoBaud = this.checkBoxAllowAutoBaud.Checked;
        if (flag2)
        {
          Application.UseWaitCursor = true;
          this.sapi.ResetSapi();
          Application.UseWaitCursor = false;
        }
        else if (flag3 && !this.sapi.LogFileIsOpen)
        {
          this.sapi.SuspendAutoConnect();
          this.sapi.ResumeAutoConnect();
        }
        flag1 = base.ApplySettings();
        Cursor.Current = Cursors.Default;
      }
    }
    return flag1;
  }

  private bool IsIdentifierVisible(string identifier)
  {
    return !this.checkBoxPowertrainOnly.Checked || ((IEnumerable<Ecu>) this.sapi.Sapi.Ecus).Where<Ecu>((Func<Ecu, bool>) (ecu => ecu.Identifier == identifier)).Any<Ecu>((Func<Ecu, bool>) (ecu => ecu.IsPowertrainDevice));
  }

  private void BuildConnectionList()
  {
    if (this.IsDisposed)
      return;
    this.tableLayoutPanelAutoConnect.SuspendLayout();
    this.tableLayoutPanelAutoConnect.Controls.Clear();
    Dictionary<string, List<Ecu>> dictionary = new Dictionary<string, List<Ecu>>();
    foreach (Ecu ecu in (IEnumerable<Ecu>) ((IEnumerable<Ecu>) this.sapi.Sapi.Ecus).Where<Ecu>((Func<Ecu, bool>) (ecu => !ecu.IsRollCall && !ecu.IsVirtual && !ecu.OfflineSupportOnly && !SapiExtensions.ProhibitAutoConnection(ecu))).OrderBy<Ecu, int>((Func<Ecu, int>) (e => e.Priority)))
    {
      if (!dictionary.ContainsKey(ecu.Identifier))
        dictionary[ecu.Identifier] = new List<Ecu>((IEnumerable<Ecu>) new Ecu[1]
        {
          ecu
        });
      else
        dictionary[ecu.Identifier].Add(ecu);
    }
    this.groupSelectionPanel.Visible = dictionary.Count > 1;
    foreach (string key in dictionary.Keys)
    {
      CheckBox box = new CheckBox();
      box.ImageAlign = ContentAlignment.MiddleLeft;
      box.TextAlign = ContentAlignment.MiddleLeft;
      box.TextImageRelation = TextImageRelation.ImageBeforeText;
      box.Text = key;
      box.Name = key;
      box.Margin = new Padding(0);
      box.AutoCheck = false;
      box.AutoSize = true;
      box.Padding = new Padding(3, 0, 0, 0);
      box.Visible = this.IsIdentifierVisible(key);
      if (box.Visible && this.sapi.IsSetForAutoConnect(key))
      {
        if (this.sapi.Sapi.Ecus.GetMarkedForAutoConnect(key))
          this.SetCheckState(box, CheckState.Checked);
        else
          this.SetCheckState(box, CheckState.Indeterminate);
      }
      else
        this.SetCheckState(box, CheckState.Unchecked);
      box.Enabled = true;
      box.Click += new EventHandler(this.OnBoxClick);
      ++this.tableLayoutPanelAutoConnect.RowCount;
      this.tableLayoutPanelAutoConnect.Controls.Add((Control) box);
      Label label1 = ConnectionOptionsPanel.CreateLabel(ConnectionOptionsPanel.GetDescription((IEnumerable<Ecu>) dictionary[key]), box.Visible);
      Label label2 = ConnectionOptionsPanel.CreateLabel(string.Join<Ecu>("/", (IEnumerable<Ecu>) dictionary[key]), box.Visible);
      this.tableLayoutPanelAutoConnect.Controls.Add((Control) label1);
      this.tableLayoutPanelAutoConnect.Controls.Add((Control) label2);
      label1.Click += (EventHandler) ((sender, e) => this.OnBoxClick((object) box, new EventArgs()));
      label2.Click += (EventHandler) ((sender, e) => this.OnBoxClick((object) box, new EventArgs()));
      box.Tag = (object) new Tuple<Control, Control>((Control) label1, (Control) label2);
    }
    this.tableLayoutPanelAutoConnect.ResumeLayout(true);
    this.CheckRollCallSupport();
  }

  private static Label CreateLabel(string text, bool visible)
  {
    Label label = new Label();
    label.AutoSize = true;
    label.TextAlign = ContentAlignment.MiddleLeft;
    label.Dock = DockStyle.Left;
    label.Padding = new Padding(0, 0, 0, 1);
    label.Text = text;
    label.Visible = visible;
    return label;
  }

  private static string GetDescription(IEnumerable<Ecu> ecus)
  {
    return Resources.ResourceManager.GetString("ConnectionOptionsPanel_Identifier" + ecus.First<Ecu>().Identifier.Replace('-', '_')) ?? ecus.First<Ecu>().ShortDescription;
  }

  private void OnSelectedHardwareChanged(object sender, EventArgs e)
  {
    if (!this.IsDirty && ((Choice) this.comboBoxHardware.SelectedItem).Name != this.sapi.HardwareType)
      this.MarkDirty();
    this.UpdateSidConfigureButton();
  }

  private void UpdateSidConfigureButton()
  {
    Choice selectedItem = (Choice) this.comboBoxHardware.SelectedItem;
    string rawValue = Choice.op_Inequality(selectedItem, (object) null) ? selectedItem.RawValue as string : (string) null;
    this.openSidConfigure.Enabled = rawValue != null && rawValue.IndexOf("J", StringComparison.OrdinalIgnoreCase) != -1;
  }

  private void OnBoxClick(object sender, EventArgs e)
  {
    CheckBox box = sender as CheckBox;
    switch (box.CheckState)
    {
      case CheckState.Unchecked:
      case CheckState.Indeterminate:
        this.SetCheckState(box, CheckState.Checked);
        break;
      case CheckState.Checked:
        this.SetCheckState(box, CheckState.Unchecked);
        break;
    }
    this.MarkDirty();
  }

  private void SetCheckState(CheckBox box, CheckState state)
  {
    if (box.Enabled)
    {
      switch (state)
      {
        case CheckState.Unchecked:
          box.Image = (Image) Resources.autoconnect_off;
          if (!SapiManager.GlobalInstance.IsSetForAutoConnect(box.Name))
          {
            this.toolTip.SetToolTip((Control) box, Resources.TooltipAutoConnectIsDisabledForDevice);
            break;
          }
          this.toolTip.SetToolTip((Control) box, Resources.TooltipAutoConnectWillBeDisabledForDevice);
          break;
        case CheckState.Checked:
          box.Image = (Image) Resources.autoconnect_on;
          if (SapiManager.GlobalInstance.IsSetForAutoConnect(box.Name))
          {
            if (this.sapi.Sapi.Ecus.GetMarkedForAutoConnect(box.Name))
            {
              this.toolTip.SetToolTip((Control) box, Resources.TooltipAutoConnectIsEnabledForDevice);
              break;
            }
            this.toolTip.SetToolTip((Control) box, Resources.TooltipAutoConnectWillBeResumedForDevice);
            break;
          }
          this.toolTip.SetToolTip((Control) box, Resources.TooltipAutoConnectWillBeEnabledForDevice);
          break;
        case CheckState.Indeterminate:
          box.Image = (Image) Resources.autoconnect_pause;
          this.toolTip.SetToolTip((Control) box, Resources.TooltipAutoConnectSuspendedForDevice);
          break;
      }
    }
    box.CheckState = state;
  }

  private void ExecuteSidConfigure(object sender, EventArgs e)
  {
    if (!this.ConfirmRestartConnection(false) || !Program.RunSidConfigure(false))
      return;
    bool complete = false;
    int num = (int) new LongOperationForm(ApplicationInformation.ProductName, (Action<BackgroundWorker, DoWorkEventArgs>) ((worker, eventArgs) =>
    {
      worker.ReportProgress(50, (object) Resources.StatusPreparingCommunications);
      Thread.Sleep(500);
      this.BeginInvoke((Delegate) (() =>
      {
        SapiManager.GlobalInstance.ResetSapi();
        complete = true;
      }));
      while (!complete)
        Thread.Sleep(100);
    }), (object) null, false).ShowDialog();
    this.UpdateSelectedTranslator();
    if (this.checkBoxAllowAutoBaud.Enabled == SapiManager.GlobalInstance.IsAutoBaudCapable)
      return;
    this.checkBoxAllowAutoBaud.Checked = this.checkBoxAllowAutoBaud.Enabled = SapiManager.GlobalInstance.IsAutoBaudCapable;
  }

  private void checkBoxUseMcd_CheckedChanged(object sender, EventArgs e)
  {
    if (!this.IsDirty && this.checkBoxUseMcd.Checked != SapiManager.GlobalInstance.UseMcd)
      this.MarkDirty();
    this.checkBoxDoIPAutoConnect.Enabled = this.checkBoxUseMcd.Checked;
    if (this.checkBoxUseMcd.Checked)
      return;
    this.checkBoxDoIPAutoConnect.Checked = false;
  }

  private void checkBoxAllowAutoBaud_CheckedChanged(object sender, EventArgs e)
  {
    if (this.IsDirty || this.checkBoxAllowAutoBaud.Checked == SapiManager.GlobalInstance.AllowAutoBaud)
      return;
    this.MarkDirty();
  }

  private void checkBoxRollCall_CheckedChanged(object sender, EventArgs e)
  {
    if (!this.IsDirty && this.checkBoxRollCall.Checked != SapiManager.GlobalInstance.RollCallEnabled)
      this.MarkDirty();
    if (!this.checkBoxRollCall.Checked)
      this.checkBoxPowertrainOnly.Checked = false;
    this.checkBoxPowertrainOnly.Enabled = this.checkBoxRollCall.Checked;
  }

  private void checkBoxPowertrainOnly_CheckedChanged(object sender, EventArgs e)
  {
    if (!this.IsDirty && this.checkBoxPowertrainOnly.Checked != SapiManager.GlobalInstance.RollCallPowertrainOnly)
      this.MarkDirty();
    this.tableLayoutPanelAutoConnect.SuspendLayout();
    foreach (CheckBox box in this.tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>())
    {
      Tuple<Control, Control> tag = (Tuple<Control, Control>) box.Tag;
      box.Visible = tag.Item1.Visible = tag.Item2.Visible = this.IsIdentifierVisible(box.Name);
      if (!box.Visible)
        this.SetCheckState(box, CheckState.Unchecked);
    }
    this.tableLayoutPanelAutoConnect.ResumeLayout();
  }

  private void checkBoxDoIPAutoConnect_CheckedChanged(object sender, EventArgs e)
  {
    if (this.IsDirty || this.checkBoxDoIPAutoConnect.Checked == SapiManager.GlobalInstance.DoIPRollCallEnabled)
      return;
    this.MarkDirty();
  }

  private void UpdateSelectedTranslator()
  {
    if (AdapterInformation.GlobalInstance == null)
      return;
    this.textBoxSelectedDevice.Text = AdapterInformation.GlobalInstance.ConnectedAdapterNames.FirstOrDefault<string>();
  }

  private void CheckRollCallSupport()
  {
    if (AdapterInformation.GlobalInstance != null && AdapterInformation.GlobalInstance.ConnectedAdapterNames.Any<string>())
    {
      switch ((int) AdapterInformation.SupportsConcurrentProtocols)
      {
        case 0:
          this.labelRollcallSupport.Visible = false;
          this.pictureBoxRollcallSupportWarning.Visible = false;
          this.checkBoxRollCall.Enabled = true;
          break;
        case 1:
          this.labelRollcallSupport.Text = Resources.ConnectionOptionsPanel_CheckRollCallSupport_Unsupported_Feature;
          this.labelRollcallSupport.Visible = true;
          this.pictureBoxRollcallSupportWarning.Image = (Image) Resources.stop;
          this.pictureBoxRollcallSupportWarning.Visible = true;
          this.checkBoxRollCall.Enabled = false;
          if (SapiManager.GlobalInstance.RollCallEnabled)
          {
            SapiManager.GlobalInstance.RollCallEnabled = SapiManager.GlobalInstance.MonitoringEnabled = false;
            if (!this.sapi.LogFileIsOpen)
            {
              this.sapi.SuspendAutoConnect();
              this.sapi.ResumeAutoConnect();
              break;
            }
            break;
          }
          break;
        case 2:
          this.labelRollcallSupport.Visible = true;
          this.labelRollcallSupport.Text = Resources.ConnectionOptionsPanel_CheckRollCallSupport_Untested_Support_Feature;
          this.pictureBoxRollcallSupportWarning.Image = (Image) Resources.warning;
          this.pictureBoxRollcallSupportWarning.Visible = true;
          this.checkBoxRollCall.Enabled = true;
          break;
      }
    }
    else
    {
      this.labelRollcallSupport.Text = Resources.ConnectionOptionsPanel_RollCallSupport_NoSelectedTranslator;
      this.pictureBoxRollcallSupportWarning.Image = (Image) Resources.stop;
      this.labelRollcallSupport.Visible = this.pictureBoxRollcallSupportWarning.Visible = true;
      this.checkBoxRollCall.Enabled = false;
    }
    this.checkBoxRollCall.Checked = SapiManager.GlobalInstance.RollCallEnabled;
    this.checkBoxPowertrainOnly.Enabled = this.checkBoxRollCall.Checked;
    this.checkBoxPowertrainOnly.Checked = this.checkBoxRollCall.Checked && SapiManager.GlobalInstance.RollCallPowertrainOnly;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ConnectionOptionsPanel));
    this.comboBoxHardware = new ComboBox();
    this.toolTip = new ToolTip(this.components);
    this.openSidConfigure = new Button();
    this.groupBox1 = new GroupBox();
    this.tableLayoutPanelRollCall = new TableLayoutPanel();
    this.checkBoxRollCall = new CheckBox();
    this.labelRollcallSupport = new Label();
    this.pictureBoxRollcallSupportWarning = new PictureBox();
    this.checkBoxDoIPAutoConnect = new CheckBox();
    this.checkBoxPowertrainOnly = new CheckBox();
    this.labelDevice = new Label();
    this.textBoxSelectedDevice = new TextBox();
    this.tableLayoutPanelVCI = new TableLayoutPanel();
    this.checkBoxAllowAutoBaud = new CheckBox();
    this.checkBoxUseMcd = new CheckBox();
    this.groupSelectionPanel = new GroupBox();
    this.tableLayoutPanelProprietaryDevices = new TableLayoutPanel();
    this.selectNoneButton = new Button();
    this.selectAllButton = new Button();
    this.resetToDefaultsButton = new Button();
    this.panelTableLayoutAutoConnect = new Panel();
    this.tableLayoutPanelAutoConnect = new TableLayoutPanel();
    Label label = new Label();
    this.groupBox1.SuspendLayout();
    this.tableLayoutPanelRollCall.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxRollcallSupportWarning).BeginInit();
    this.tableLayoutPanelVCI.SuspendLayout();
    this.groupSelectionPanel.SuspendLayout();
    this.tableLayoutPanelProprietaryDevices.SuspendLayout();
    this.panelTableLayoutAutoConnect.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) label, "labelHardware");
    label.Name = "labelHardware";
    this.tableLayoutPanelVCI.SetRowSpan((Control) label, 2);
    componentResourceManager.ApplyResources((object) this.comboBoxHardware, "comboBoxHardware");
    this.comboBoxHardware.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxHardware.FormattingEnabled = true;
    this.comboBoxHardware.Name = "comboBoxHardware";
    this.tableLayoutPanelVCI.SetRowSpan((Control) this.comboBoxHardware, 2);
    this.comboBoxHardware.SelectedIndexChanged += new EventHandler(this.OnSelectedHardwareChanged);
    this.toolTip.AutomaticDelay = 0;
    componentResourceManager.ApplyResources((object) this.openSidConfigure, "openSidConfigure");
    this.openSidConfigure.Name = "openSidConfigure";
    this.tableLayoutPanelVCI.SetRowSpan((Control) this.openSidConfigure, 2);
    this.openSidConfigure.UseVisualStyleBackColor = true;
    this.openSidConfigure.Click += new EventHandler(this.ExecuteSidConfigure);
    componentResourceManager.ApplyResources((object) this.groupBox1, "groupBox1");
    this.groupBox1.Controls.Add((Control) this.tableLayoutPanelRollCall);
    this.groupBox1.Name = "groupBox1";
    this.groupBox1.TabStop = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelRollCall, "tableLayoutPanelRollCall");
    this.tableLayoutPanelRollCall.Controls.Add((Control) this.checkBoxRollCall, 0, 0);
    this.tableLayoutPanelRollCall.Controls.Add((Control) this.labelRollcallSupport, 2, 0);
    this.tableLayoutPanelRollCall.Controls.Add((Control) this.pictureBoxRollcallSupportWarning, 1, 0);
    this.tableLayoutPanelRollCall.Controls.Add((Control) this.checkBoxDoIPAutoConnect, 0, 2);
    this.tableLayoutPanelRollCall.Controls.Add((Control) this.checkBoxPowertrainOnly, 0, 1);
    this.tableLayoutPanelRollCall.Name = "tableLayoutPanelRollCall";
    componentResourceManager.ApplyResources((object) this.checkBoxRollCall, "checkBoxRollCall");
    this.checkBoxRollCall.Name = "checkBoxRollCall";
    this.checkBoxRollCall.UseVisualStyleBackColor = true;
    this.checkBoxRollCall.CheckedChanged += new EventHandler(this.checkBoxRollCall_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.labelRollcallSupport, "labelRollcallSupport");
    this.labelRollcallSupport.Name = "labelRollcallSupport";
    this.tableLayoutPanelRollCall.SetRowSpan((Control) this.labelRollcallSupport, 3);
    componentResourceManager.ApplyResources((object) this.pictureBoxRollcallSupportWarning, "pictureBoxRollcallSupportWarning");
    this.pictureBoxRollcallSupportWarning.Image = (Image) Resources.stop;
    this.pictureBoxRollcallSupportWarning.Name = "pictureBoxRollcallSupportWarning";
    this.tableLayoutPanelRollCall.SetRowSpan((Control) this.pictureBoxRollcallSupportWarning, 3);
    this.pictureBoxRollcallSupportWarning.TabStop = false;
    componentResourceManager.ApplyResources((object) this.checkBoxDoIPAutoConnect, "checkBoxDoIPAutoConnect");
    this.checkBoxDoIPAutoConnect.Name = "checkBoxDoIPAutoConnect";
    this.checkBoxDoIPAutoConnect.UseVisualStyleBackColor = true;
    this.checkBoxDoIPAutoConnect.CheckedChanged += new EventHandler(this.checkBoxDoIPAutoConnect_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxPowertrainOnly, "checkBoxPowertrainOnly");
    this.checkBoxPowertrainOnly.Name = "checkBoxPowertrainOnly";
    this.checkBoxPowertrainOnly.UseVisualStyleBackColor = true;
    this.checkBoxPowertrainOnly.CheckedChanged += new EventHandler(this.checkBoxPowertrainOnly_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.labelDevice, "labelDevice");
    this.labelDevice.Name = "labelDevice";
    this.tableLayoutPanelVCI.SetRowSpan((Control) this.labelDevice, 2);
    componentResourceManager.ApplyResources((object) this.textBoxSelectedDevice, "textBoxSelectedDevice");
    this.textBoxSelectedDevice.Name = "textBoxSelectedDevice";
    this.tableLayoutPanelVCI.SetRowSpan((Control) this.textBoxSelectedDevice, 2);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelVCI, "tableLayoutPanelVCI");
    this.tableLayoutPanelVCI.Controls.Add((Control) this.checkBoxAllowAutoBaud, 5, 1);
    this.tableLayoutPanelVCI.Controls.Add((Control) label, 0, 0);
    this.tableLayoutPanelVCI.Controls.Add((Control) this.comboBoxHardware, 1, 0);
    this.tableLayoutPanelVCI.Controls.Add((Control) this.textBoxSelectedDevice, 3, 0);
    this.tableLayoutPanelVCI.Controls.Add((Control) this.openSidConfigure, 4, 0);
    this.tableLayoutPanelVCI.Controls.Add((Control) this.labelDevice, 2, 0);
    this.tableLayoutPanelVCI.Controls.Add((Control) this.checkBoxUseMcd, 5, 0);
    this.tableLayoutPanelVCI.Name = "tableLayoutPanelVCI";
    componentResourceManager.ApplyResources((object) this.checkBoxAllowAutoBaud, "checkBoxAllowAutoBaud");
    this.checkBoxAllowAutoBaud.Name = "checkBoxAllowAutoBaud";
    this.checkBoxAllowAutoBaud.UseVisualStyleBackColor = true;
    this.checkBoxAllowAutoBaud.CheckedChanged += new EventHandler(this.checkBoxAllowAutoBaud_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxUseMcd, "checkBoxUseMcd");
    this.checkBoxUseMcd.Name = "checkBoxUseMcd";
    this.checkBoxUseMcd.UseVisualStyleBackColor = true;
    this.checkBoxUseMcd.CheckedChanged += new EventHandler(this.checkBoxUseMcd_CheckedChanged);
    this.groupSelectionPanel.Controls.Add((Control) this.tableLayoutPanelProprietaryDevices);
    componentResourceManager.ApplyResources((object) this.groupSelectionPanel, "groupSelectionPanel");
    this.groupSelectionPanel.Name = "groupSelectionPanel";
    this.groupSelectionPanel.TabStop = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelProprietaryDevices, "tableLayoutPanelProprietaryDevices");
    this.tableLayoutPanelProprietaryDevices.Controls.Add((Control) this.selectNoneButton, 1, 2);
    this.tableLayoutPanelProprietaryDevices.Controls.Add((Control) this.selectAllButton, 1, 1);
    this.tableLayoutPanelProprietaryDevices.Controls.Add((Control) this.resetToDefaultsButton, 1, 0);
    this.tableLayoutPanelProprietaryDevices.Controls.Add((Control) this.panelTableLayoutAutoConnect, 0, 0);
    this.tableLayoutPanelProprietaryDevices.Name = "tableLayoutPanelProprietaryDevices";
    componentResourceManager.ApplyResources((object) this.selectNoneButton, "selectNoneButton");
    this.selectNoneButton.Name = "selectNoneButton";
    this.selectNoneButton.UseVisualStyleBackColor = true;
    this.selectNoneButton.Click += new EventHandler(this.selectNoneButton_Click);
    componentResourceManager.ApplyResources((object) this.selectAllButton, "selectAllButton");
    this.selectAllButton.Name = "selectAllButton";
    this.selectAllButton.UseVisualStyleBackColor = true;
    this.selectAllButton.Click += new EventHandler(this.selectAllButton_Click);
    componentResourceManager.ApplyResources((object) this.resetToDefaultsButton, "resetToDefaultsButton");
    this.resetToDefaultsButton.Name = "resetToDefaultsButton";
    this.resetToDefaultsButton.UseVisualStyleBackColor = true;
    this.resetToDefaultsButton.Click += new EventHandler(this.resetToDefaultsButton_Click);
    this.panelTableLayoutAutoConnect.BorderStyle = BorderStyle.FixedSingle;
    this.panelTableLayoutAutoConnect.Controls.Add((Control) this.tableLayoutPanelAutoConnect);
    componentResourceManager.ApplyResources((object) this.panelTableLayoutAutoConnect, "panelTableLayoutAutoConnect");
    this.panelTableLayoutAutoConnect.Name = "panelTableLayoutAutoConnect";
    this.tableLayoutPanelProprietaryDevices.SetRowSpan((Control) this.panelTableLayoutAutoConnect, 4);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAutoConnect, "tableLayoutPanelAutoConnect");
    this.tableLayoutPanelAutoConnect.BackColor = SystemColors.Window;
    this.tableLayoutPanelAutoConnect.Name = "tableLayoutPanelAutoConnect";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.Controls.Add((Control) this.groupSelectionPanel);
    this.Controls.Add((Control) this.tableLayoutPanelVCI);
    this.Controls.Add((Control) this.groupBox1);
    this.Name = nameof (ConnectionOptionsPanel);
    this.Controls.SetChildIndex((Control) this.groupBox1, 0);
    this.Controls.SetChildIndex((Control) this.tableLayoutPanelVCI, 0);
    this.Controls.SetChildIndex((Control) this.groupSelectionPanel, 0);
    this.groupBox1.ResumeLayout(false);
    this.tableLayoutPanelRollCall.ResumeLayout(false);
    this.tableLayoutPanelRollCall.PerformLayout();
    ((ISupportInitialize) this.pictureBoxRollcallSupportWarning).EndInit();
    this.tableLayoutPanelVCI.ResumeLayout(false);
    this.tableLayoutPanelVCI.PerformLayout();
    this.groupSelectionPanel.ResumeLayout(false);
    this.tableLayoutPanelProprietaryDevices.ResumeLayout(false);
    this.tableLayoutPanelProprietaryDevices.PerformLayout();
    this.panelTableLayoutAutoConnect.ResumeLayout(false);
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
