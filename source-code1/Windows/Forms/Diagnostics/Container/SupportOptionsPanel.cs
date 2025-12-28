// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.SupportOptionsPanel
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class SupportOptionsPanel : OptionsPanel
{
  private int filterListsItemIndex = -1;
  private string adrAdditionalSaveLocation = string.Empty;
  private IContainer components;
  private CheckBox checkBoxByte;
  private CheckBox checkBoxDebug;
  private CheckBox checkBoxException;
  private Label labelFileManagement;
  private PictureBox pictureFileHistory;
  private ComboBox comboBoxLogExpirationAge;
  private ComboBox comboBoxTraceExpirationAge;
  private Label labelPanels;
  private PictureBox pictureBoxPanels;
  private CheckBox checkBoxAlwaysLoad;
  private NumericUpDown nudMaxSummaryFileSize;
  private ComboBox comboBoxSummaryFileSizeAction;
  private CheckBox useNameValuePairsToParameterize;
  private CheckBox checkBoxIgnoreLastServicedCheck;
  private TableLayoutPanel tableLayoutPanel1;
  private Panel panel1;
  private TableLayoutPanel tableLayoutPanel2;
  private TableLayoutPanel tableLayoutPanel3;
  private FlowLayoutPanel flowLayoutPanel1;
  private TableLayoutPanel tableLayoutPanel4;
  private ComboBox comboBoxLevel;
  private Label labelFilterLists;
  private TableLayoutPanel tableLayoutPanelADRReports;
  private CheckBox checkBoxAdrAdditionalSaveLocation;
  private Label labelAdrAdditionalSaveLocation;
  private Button buttonBrowse;
  private TableLayoutPanel tableLayoutPanelTrace;
  private CheckBox checkBoxAllowChecEdit;

  public SupportOptionsPanel()
  {
    this.MinAccessLevel = 1;
    this.InitializeComponent();
    this.HeaderImage = (Image) new Bitmap(this.GetType(), "option_support.png");
    this.pictureFileHistory.Image = (Image) new Bitmap(this.GetType(), "option_support_history.png");
    this.pictureBoxPanels.Image = (Image) new Bitmap(this.GetType(), "option_support_panels.png");
    foreach (object obj in Enum.GetValues(typeof (ExpirationAge)))
    {
      this.comboBoxLogExpirationAge.Items.Add(obj);
      this.comboBoxTraceExpirationAge.Items.Add(obj);
    }
    foreach (object obj in Enum.GetValues(typeof (LargeSummaryFileOption)))
      this.comboBoxSummaryFileSizeAction.Items.Add(obj);
    this.comboBoxLevel.Items.Add((object) new AccessLevel("Engineering", 3));
    this.comboBoxLevel.Items.Add((object) new AccessLevel("Dealer/Distributor", 2));
    this.comboBoxLevel.Items.Add((object) new AccessLevel("Customer", 1));
  }

  protected override void OnLoad(EventArgs e)
  {
    this.checkBoxByte.Checked = !TraceLogManager.IsFiltered((StatusMessageType) 0);
    this.checkBoxDebug.Checked = !TraceLogManager.IsFiltered((StatusMessageType) 2);
    this.checkBoxException.Checked = !TraceLogManager.IsFiltered((StatusMessageType) 1);
    this.comboBoxLogExpirationAge.SelectedItem = (object) SapiManager.LogFileExpirationAge;
    this.comboBoxTraceExpirationAge.SelectedItem = (object) TraceLogManager.TraceLogExpirationAge;
    this.comboBoxSummaryFileSizeAction.SelectedItem = (object) SapiManager.GlobalInstance.LargeSummaryOptions;
    this.nudMaxSummaryFileSize.Value = (Decimal) SapiManager.GlobalInstance.SummaryFileMaxSize;
    this.adrAdditionalSaveLocation = this.labelAdrAdditionalSaveLocation.Text = Reporter.AdditionalSaveLocation;
    this.checkBoxAdrAdditionalSaveLocation.Checked = !string.IsNullOrEmpty(this.labelAdrAdditionalSaveLocation.Text);
    this.buttonBrowse.Enabled = this.checkBoxAdrAdditionalSaveLocation.Checked;
    this.checkBoxAlwaysLoad.Visible = false;
    this.useNameValuePairsToParameterize.Visible = false;
    this.checkBoxIgnoreLastServicedCheck.Visible = false;
    this.checkBoxAllowChecEdit.Visible = false;
    this.comboBoxLevel.Visible = false;
    this.labelFilterLists.Visible = false;
    this.pictureBoxPanels.Visible = false;
    this.labelPanels.Visible = false;
    base.OnLoad(e);
  }

  private void OnCheckByte(object sender, EventArgs e)
  {
    if (this.checkBoxByte.Checked != TraceLogManager.IsFiltered((StatusMessageType) 0))
      return;
    this.MarkDirty();
  }

  private void OnCheckDebug(object sender, EventArgs e)
  {
    if (this.checkBoxDebug.Checked != TraceLogManager.IsFiltered((StatusMessageType) 2))
      return;
    this.MarkDirty();
  }

  private void OnCheckException(object sender, EventArgs e)
  {
    if (this.checkBoxException.Checked != TraceLogManager.IsFiltered((StatusMessageType) 1))
      return;
    this.MarkDirty();
  }

  public override bool ApplySettings()
  {
    if (this.IsDirty)
    {
      TraceLogManager.FilterMessageType((StatusMessageType) 0, !this.checkBoxByte.Checked);
      TraceLogManager.FilterMessageType((StatusMessageType) 2, !this.checkBoxDebug.Checked);
      TraceLogManager.FilterMessageType((StatusMessageType) 1, !this.checkBoxException.Checked);
      TraceLogManager.TraceLogExpirationAge = (ExpirationAge) this.comboBoxTraceExpirationAge.SelectedItem;
      SapiManager.LogFileExpirationAge = (ExpirationAge) this.comboBoxLogExpirationAge.SelectedItem;
      if (this.comboBoxLevel.SelectedItem != null)
        SapiManager.GlobalInstance.AccessLevelFilter = new int?(((AccessLevel) this.comboBoxLevel.SelectedItem).Level);
      PanelBase.AlwaysLoadPanels = this.checkBoxAlwaysLoad.Checked;
      Reporter.AdditionalSaveLocation = this.labelAdrAdditionalSaveLocation.Text;
      SapiManager.GlobalInstance.UseNameValuePairsToParameterize = this.useNameValuePairsToParameterize.Checked;
      SapiManager.GlobalInstance.IgnoreLastServicedCheck = this.checkBoxIgnoreLastServicedCheck.Checked;
      SapiManager.GlobalInstance.AllowChecParameterEdit = this.checkBoxAllowChecEdit.Checked;
      SapiManager.GlobalInstance.LargeSummaryOptions = (LargeSummaryFileOption) this.comboBoxSummaryFileSizeAction.SelectedItem;
      SapiManager.GlobalInstance.SummaryFileMaxSize = (int) this.nudMaxSummaryFileSize.Value;
    }
    return base.ApplySettings();
  }

  private void OnTraceExpirationCommitted(object sender, EventArgs e)
  {
    if ((ExpirationAge) this.comboBoxTraceExpirationAge.SelectedItem == TraceLogManager.TraceLogExpirationAge)
      return;
    this.MarkDirty();
  }

  private void OnLogExpirationCommitted(object sender, EventArgs e)
  {
    if ((ExpirationAge) this.comboBoxLogExpirationAge.SelectedItem == SapiManager.LogFileExpirationAge)
      return;
    this.MarkDirty();
  }

  private void OnCheckAlwaysLoadPanels(object sender, EventArgs e)
  {
    if (this.checkBoxAlwaysLoad.Checked == PanelBase.AlwaysLoadPanels)
      return;
    this.MarkDirty();
  }

  private void OnSelectedLevelChanged(object sender, EventArgs e)
  {
    if (this.comboBoxLevel.SelectedIndex == this.filterListsItemIndex)
      return;
    this.filterListsItemIndex = this.comboBoxLevel.SelectedIndex;
    this.MarkDirty();
  }

  private void nudMaxSummaryFileSize_ValueChanged(object sender, EventArgs e)
  {
    if ((int) this.nudMaxSummaryFileSize.Value == SapiManager.GlobalInstance.SummaryFileMaxSize)
      return;
    this.MarkDirty();
  }

  private void comboBoxSummaryFileSizeAction_SelectionChangeCommitted(object sender, EventArgs e)
  {
    if ((LargeSummaryFileOption) this.comboBoxSummaryFileSizeAction.SelectedItem == SapiManager.GlobalInstance.LargeSummaryOptions)
      return;
    this.MarkDirty();
  }

  private void useNameValuePairsToParameterize_CheckedChanged(object sender, EventArgs e)
  {
    if (this.useNameValuePairsToParameterize.Checked == SapiManager.GlobalInstance.UseNameValuePairsToParameterize)
      return;
    this.MarkDirty();
  }

  private void checkBoxIgnoreLastServicedCheck_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxIgnoreLastServicedCheck.Checked == SapiManager.GlobalInstance.IgnoreLastServicedCheck)
      return;
    this.MarkDirty();
  }

  private void buttonBrowse_Click(object sender, EventArgs e)
  {
    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
    folderBrowserDialog.SelectedPath = this.labelAdrAdditionalSaveLocation.Text;
    folderBrowserDialog.ShowNewFolderButton = false;
    folderBrowserDialog.Description = Resources.AdrAdditionSaveLocationDialogMessage;
    if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
      return;
    try
    {
      this.labelAdrAdditionalSaveLocation.Text = this.adrAdditionalSaveLocation = folderBrowserDialog.SelectedPath;
      if (!(this.labelAdrAdditionalSaveLocation.Text != Reporter.AdditionalSaveLocation))
        return;
      this.MarkDirty();
    }
    catch (PathTooLongException ex)
    {
      int num = (int) ControlHelpers.ShowMessageBox((Control) this, Resources.MessageFormat_PathToLong, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
    }
  }

  private void checkBoxAdrAdditionalSaveLocation_Click(object sender, EventArgs e)
  {
    this.buttonBrowse.Enabled = this.checkBoxAdrAdditionalSaveLocation.Checked;
    this.labelAdrAdditionalSaveLocation.Text = this.checkBoxAdrAdditionalSaveLocation.Checked ? this.adrAdditionalSaveLocation : string.Empty;
    this.MarkDirty();
  }

  private void checkBoxAllowChecEdit_CheckedChanged(object sender, EventArgs e)
  {
    if (this.checkBoxAllowChecEdit.Checked == SapiManager.GlobalInstance.AllowChecParameterEdit)
      return;
    this.MarkDirty();
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
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SupportOptionsPanel));
    this.checkBoxByte = new CheckBox();
    this.checkBoxDebug = new CheckBox();
    this.checkBoxException = new CheckBox();
    this.useNameValuePairsToParameterize = new CheckBox();
    this.checkBoxIgnoreLastServicedCheck = new CheckBox();
    this.pictureFileHistory = new PictureBox();
    this.labelFileManagement = new Label();
    this.comboBoxLogExpirationAge = new ComboBox();
    this.comboBoxTraceExpirationAge = new ComboBox();
    this.labelPanels = new Label();
    this.pictureBoxPanels = new PictureBox();
    this.checkBoxAlwaysLoad = new CheckBox();
    this.nudMaxSummaryFileSize = new NumericUpDown();
    this.comboBoxSummaryFileSizeAction = new ComboBox();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.panel1 = new Panel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.checkBoxAllowChecEdit = new CheckBox();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.comboBoxLevel = new ComboBox();
    this.labelFilterLists = new Label();
    this.tableLayoutPanelADRReports = new TableLayoutPanel();
    this.checkBoxAdrAdditionalSaveLocation = new CheckBox();
    this.labelAdrAdditionalSaveLocation = new Label();
    this.buttonBrowse = new Button();
    this.tableLayoutPanelTrace = new TableLayoutPanel();
    ToolTip toolTip = new ToolTip(this.components);
    Label label1 = new Label();
    Label label2 = new Label();
    Label label3 = new Label();
    Label label4 = new Label();
    Label label5 = new Label();
    ((ISupportInitialize) this.pictureFileHistory).BeginInit();
    ((ISupportInitialize) this.pictureBoxPanels).BeginInit();
    this.nudMaxSummaryFileSize.BeginInit();
    this.tableLayoutPanel1.SuspendLayout();
    this.panel1.SuspendLayout();
    this.tableLayoutPanel2.SuspendLayout();
    this.tableLayoutPanel3.SuspendLayout();
    this.flowLayoutPanel1.SuspendLayout();
    this.tableLayoutPanel4.SuspendLayout();
    this.tableLayoutPanelADRReports.SuspendLayout();
    this.tableLayoutPanelTrace.SuspendLayout();
    this.SuspendLayout();
    toolTip.AutoPopDelay = 7500;
    toolTip.InitialDelay = 500;
    toolTip.IsBalloon = true;
    toolTip.ReshowDelay = 100;
    toolTip.ShowAlways = true;
    toolTip.ToolTipIcon = ToolTipIcon.Info;
    toolTip.ToolTipTitle = "Help";
    componentResourceManager.ApplyResources((object) this.checkBoxByte, "checkBoxByte");
    this.checkBoxByte.Name = "checkBoxByte";
    toolTip.SetToolTip((Control) this.checkBoxByte, componentResourceManager.GetString("checkBoxByte.ToolTip"));
    this.checkBoxByte.UseVisualStyleBackColor = true;
    this.checkBoxByte.CheckedChanged += new EventHandler(this.OnCheckByte);
    componentResourceManager.ApplyResources((object) this.checkBoxDebug, "checkBoxDebug");
    this.checkBoxDebug.Name = "checkBoxDebug";
    toolTip.SetToolTip((Control) this.checkBoxDebug, componentResourceManager.GetString("checkBoxDebug.ToolTip"));
    this.checkBoxDebug.UseVisualStyleBackColor = true;
    this.checkBoxDebug.CheckedChanged += new EventHandler(this.OnCheckDebug);
    componentResourceManager.ApplyResources((object) this.checkBoxException, "checkBoxException");
    this.checkBoxException.Name = "checkBoxException";
    toolTip.SetToolTip((Control) this.checkBoxException, componentResourceManager.GetString("checkBoxException.ToolTip"));
    this.checkBoxException.UseVisualStyleBackColor = true;
    this.checkBoxException.CheckedChanged += new EventHandler(this.OnCheckException);
    componentResourceManager.ApplyResources((object) label1, "labelSummaryLogFileSize");
    label1.Name = "labelSummaryLogFileSize";
    toolTip.SetToolTip((Control) label1, componentResourceManager.GetString("labelSummaryLogFileSize.ToolTip"));
    componentResourceManager.ApplyResources((object) label2, "label1");
    label2.Name = "label1";
    toolTip.SetToolTip((Control) label2, componentResourceManager.GetString("label1.ToolTip"));
    componentResourceManager.ApplyResources((object) label3, "labelLogExpirationAge");
    label3.Name = "labelLogExpirationAge";
    componentResourceManager.ApplyResources((object) label4, "labelTraceExpirationAge");
    label4.Name = "labelTraceExpirationAge";
    componentResourceManager.ApplyResources((object) label5, "labelMegaBytes");
    label5.Name = "labelMegaBytes";
    componentResourceManager.ApplyResources((object) this.useNameValuePairsToParameterize, "useNameValuePairsToParameterize");
    this.useNameValuePairsToParameterize.Name = "useNameValuePairsToParameterize";
    this.useNameValuePairsToParameterize.UseVisualStyleBackColor = true;
    this.useNameValuePairsToParameterize.CheckedChanged += new EventHandler(this.useNameValuePairsToParameterize_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxIgnoreLastServicedCheck, "checkBoxIgnoreLastServicedCheck");
    this.checkBoxIgnoreLastServicedCheck.Name = "checkBoxIgnoreLastServicedCheck";
    this.checkBoxIgnoreLastServicedCheck.UseVisualStyleBackColor = true;
    this.checkBoxIgnoreLastServicedCheck.CheckedChanged += new EventHandler(this.checkBoxIgnoreLastServicedCheck_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.pictureFileHistory, "pictureFileHistory");
    this.pictureFileHistory.Name = "pictureFileHistory";
    this.pictureFileHistory.TabStop = false;
    componentResourceManager.ApplyResources((object) this.labelFileManagement, "labelFileManagement");
    this.labelFileManagement.Name = "labelFileManagement";
    componentResourceManager.ApplyResources((object) this.comboBoxLogExpirationAge, "comboBoxLogExpirationAge");
    this.comboBoxLogExpirationAge.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxLogExpirationAge.FormattingEnabled = true;
    this.comboBoxLogExpirationAge.Name = "comboBoxLogExpirationAge";
    this.comboBoxLogExpirationAge.SelectionChangeCommitted += new EventHandler(this.OnLogExpirationCommitted);
    componentResourceManager.ApplyResources((object) this.comboBoxTraceExpirationAge, "comboBoxTraceExpirationAge");
    this.comboBoxTraceExpirationAge.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxTraceExpirationAge.FormattingEnabled = true;
    this.comboBoxTraceExpirationAge.Name = "comboBoxTraceExpirationAge";
    this.comboBoxTraceExpirationAge.SelectionChangeCommitted += new EventHandler(this.OnTraceExpirationCommitted);
    componentResourceManager.ApplyResources((object) this.labelPanels, "labelPanels");
    this.labelPanels.Name = "labelPanels";
    componentResourceManager.ApplyResources((object) this.pictureBoxPanels, "pictureBoxPanels");
    this.pictureBoxPanels.Name = "pictureBoxPanels";
    this.pictureBoxPanels.TabStop = false;
    componentResourceManager.ApplyResources((object) this.checkBoxAlwaysLoad, "checkBoxAlwaysLoad");
    this.checkBoxAlwaysLoad.Name = "checkBoxAlwaysLoad";
    this.checkBoxAlwaysLoad.UseVisualStyleBackColor = true;
    this.checkBoxAlwaysLoad.CheckedChanged += new EventHandler(this.OnCheckAlwaysLoadPanels);
    componentResourceManager.ApplyResources((object) this.nudMaxSummaryFileSize, "nudMaxSummaryFileSize");
    this.nudMaxSummaryFileSize.Maximum = new Decimal(new int[4]
    {
      1024 /*0x0400*/,
      0,
      0,
      0
    });
    this.nudMaxSummaryFileSize.Minimum = new Decimal(new int[4]
    {
      1,
      0,
      0,
      0
    });
    this.nudMaxSummaryFileSize.Name = "nudMaxSummaryFileSize";
    this.nudMaxSummaryFileSize.Value = new Decimal(new int[4]
    {
      1,
      0,
      0,
      0
    });
    this.nudMaxSummaryFileSize.ValueChanged += new EventHandler(this.nudMaxSummaryFileSize_ValueChanged);
    componentResourceManager.ApplyResources((object) this.comboBoxSummaryFileSizeAction, "comboBoxSummaryFileSizeAction");
    this.comboBoxSummaryFileSizeAction.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxSummaryFileSizeAction.FormattingEnabled = true;
    this.comboBoxSummaryFileSizeAction.Name = "comboBoxSummaryFileSizeAction";
    this.comboBoxSummaryFileSizeAction.SelectionChangeCommitted += new EventHandler(this.comboBoxSummaryFileSizeAction_SelectionChangeCommitted);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    this.tableLayoutPanel1.Controls.Add((Control) this.comboBoxSummaryFileSizeAction, 3, 3);
    this.tableLayoutPanel1.Controls.Add((Control) label3, 0, 2);
    this.tableLayoutPanel1.Controls.Add((Control) label2, 2, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.comboBoxLogExpirationAge, 1, 2);
    this.tableLayoutPanel1.Controls.Add((Control) label4, 0, 3);
    this.tableLayoutPanel1.Controls.Add((Control) this.comboBoxTraceExpirationAge, 1, 3);
    this.tableLayoutPanel1.Controls.Add((Control) label1, 2, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.panel1, 3, 2);
    this.tableLayoutPanel1.Controls.Add((Control) this.tableLayoutPanel2, 0, 1);
    this.tableLayoutPanel1.Controls.Add((Control) this.tableLayoutPanel3, 0, 5);
    this.tableLayoutPanel1.Controls.Add((Control) this.flowLayoutPanel1, 0, 6);
    this.tableLayoutPanel1.Controls.Add((Control) this.tableLayoutPanel4, 0, 7);
    this.tableLayoutPanel1.Controls.Add((Control) this.tableLayoutPanelADRReports, 0, 4);
    this.tableLayoutPanel1.Controls.Add((Control) this.tableLayoutPanelTrace, 0, 0);
    this.tableLayoutPanel1.Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.panel1, "panel1");
    this.panel1.Controls.Add((Control) this.nudMaxSummaryFileSize);
    this.panel1.Controls.Add((Control) label5);
    this.panel1.Name = "panel1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.tableLayoutPanel2, 4);
    this.tableLayoutPanel2.Controls.Add((Control) this.pictureFileHistory, 0, 0);
    this.tableLayoutPanel2.Controls.Add((Control) this.labelFileManagement, 1, 0);
    this.tableLayoutPanel2.Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.tableLayoutPanel3, 4);
    this.tableLayoutPanel3.Controls.Add((Control) this.pictureBoxPanels, 0, 0);
    this.tableLayoutPanel3.Controls.Add((Control) this.labelPanels, 1, 0);
    this.tableLayoutPanel3.Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.flowLayoutPanel1, "flowLayoutPanel1");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.flowLayoutPanel1, 4);
    this.flowLayoutPanel1.Controls.Add((Control) this.checkBoxAlwaysLoad);
    this.flowLayoutPanel1.Controls.Add((Control) this.useNameValuePairsToParameterize);
    this.flowLayoutPanel1.Controls.Add((Control) this.checkBoxIgnoreLastServicedCheck);
    this.flowLayoutPanel1.Controls.Add((Control) this.checkBoxAllowChecEdit);
    this.flowLayoutPanel1.Name = "flowLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.checkBoxAllowChecEdit, "checkBoxAllowChecEdit");
    this.checkBoxAllowChecEdit.Name = "checkBoxAllowChecEdit";
    this.checkBoxAllowChecEdit.UseVisualStyleBackColor = true;
    this.checkBoxAllowChecEdit.CheckedChanged += new EventHandler(this.checkBoxAllowChecEdit_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.tableLayoutPanel4, 4);
    this.tableLayoutPanel4.Controls.Add((Control) this.comboBoxLevel, 1, 0);
    this.tableLayoutPanel4.Controls.Add((Control) this.labelFilterLists, 0, 0);
    this.tableLayoutPanel4.Name = "tableLayoutPanel4";
    this.comboBoxLevel.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxLevel.FormattingEnabled = true;
    componentResourceManager.ApplyResources((object) this.comboBoxLevel, "comboBoxLevel");
    this.comboBoxLevel.Name = "comboBoxLevel";
    this.comboBoxLevel.SelectedIndexChanged += new EventHandler(this.OnSelectedLevelChanged);
    componentResourceManager.ApplyResources((object) this.labelFilterLists, "labelFilterLists");
    this.labelFilterLists.Name = "labelFilterLists";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelADRReports, "tableLayoutPanelADRReports");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.tableLayoutPanelADRReports, 4);
    this.tableLayoutPanelADRReports.Controls.Add((Control) this.checkBoxAdrAdditionalSaveLocation, 0, 0);
    this.tableLayoutPanelADRReports.Controls.Add((Control) this.labelAdrAdditionalSaveLocation, 1, 0);
    this.tableLayoutPanelADRReports.Controls.Add((Control) this.buttonBrowse, 2, 0);
    this.tableLayoutPanelADRReports.Name = "tableLayoutPanelADRReports";
    componentResourceManager.ApplyResources((object) this.checkBoxAdrAdditionalSaveLocation, "checkBoxAdrAdditionalSaveLocation");
    this.checkBoxAdrAdditionalSaveLocation.Name = "checkBoxAdrAdditionalSaveLocation";
    this.checkBoxAdrAdditionalSaveLocation.UseVisualStyleBackColor = true;
    this.checkBoxAdrAdditionalSaveLocation.Click += new EventHandler(this.checkBoxAdrAdditionalSaveLocation_Click);
    this.labelAdrAdditionalSaveLocation.AutoEllipsis = true;
    this.labelAdrAdditionalSaveLocation.BorderStyle = BorderStyle.Fixed3D;
    componentResourceManager.ApplyResources((object) this.labelAdrAdditionalSaveLocation, "labelAdrAdditionalSaveLocation");
    this.labelAdrAdditionalSaveLocation.Name = "labelAdrAdditionalSaveLocation";
    this.labelAdrAdditionalSaveLocation.UseMnemonic = false;
    componentResourceManager.ApplyResources((object) this.buttonBrowse, "buttonBrowse");
    this.buttonBrowse.Name = "buttonBrowse";
    this.buttonBrowse.UseVisualStyleBackColor = true;
    this.buttonBrowse.Click += new EventHandler(this.buttonBrowse_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTrace, "tableLayoutPanelTrace");
    this.tableLayoutPanel1.SetColumnSpan((Control) this.tableLayoutPanelTrace, 4);
    this.tableLayoutPanelTrace.Controls.Add((Control) this.checkBoxByte, 0, 0);
    this.tableLayoutPanelTrace.Controls.Add((Control) this.checkBoxException, 2, 0);
    this.tableLayoutPanelTrace.Controls.Add((Control) this.checkBoxDebug, 1, 0);
    this.tableLayoutPanelTrace.Name = "tableLayoutPanelTrace";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.Controls.Add((Control) this.tableLayoutPanel1);
    this.Name = nameof (SupportOptionsPanel);
    this.Controls.SetChildIndex((Control) this.tableLayoutPanel1, 0);
    ((ISupportInitialize) this.pictureFileHistory).EndInit();
    ((ISupportInitialize) this.pictureBoxPanels).EndInit();
    this.nudMaxSummaryFileSize.EndInit();
    this.tableLayoutPanel1.ResumeLayout(false);
    this.tableLayoutPanel1.PerformLayout();
    this.panel1.ResumeLayout(false);
    this.panel1.PerformLayout();
    this.tableLayoutPanel2.ResumeLayout(false);
    this.tableLayoutPanel3.ResumeLayout(false);
    this.flowLayoutPanel1.ResumeLayout(false);
    this.flowLayoutPanel1.PerformLayout();
    this.tableLayoutPanel4.ResumeLayout(false);
    this.tableLayoutPanel4.PerformLayout();
    this.tableLayoutPanelADRReports.ResumeLayout(false);
    this.tableLayoutPanelADRReports.PerformLayout();
    this.tableLayoutPanelTrace.ResumeLayout(false);
    this.tableLayoutPanelTrace.PerformLayout();
    this.ResumeLayout(false);
  }
}
