// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Net.AddUnitDialog
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Net;

public class AddUnitDialog : Form
{
  private const int ecuNameColumnIndex = 0;
  private const int hardwarePartColumnIndex = 1;
  private List<AddUnitEcuHardwarePartNumber> ecuHardwarePartNumbers;
  private BindingList<AddUnitEcuHardwarePartNumber> ecuHardwarePartsBindingList;
  private System.Windows.Forms.Label engineSerialNumberLabel;
  private TextBox engineSerialNumberTextBox;
  private TextBox vehicleIdNumberTextBox;
  private System.Windows.Forms.Label mainInfoLabel;
  private Button okButton;
  private Button cancelButton;
  private DataGridView dataGridViewEcuHardware;
  private TableLayoutPanel rootTableLayoutPanel;
  private Panel formButtonsPanel;
  private System.Windows.Forms.Label ecuInfoLabel;
  private Button clearFormButton;
  private TableLayoutPanel ecuTableLayoutPanel;
  private ToolStrip dataGridToolStrip;
  private ToolStripButton addEcuItemButton;
  private ToolStripButton removeEcuItemButton;
  private DataGridViewComboBoxColumn ecu;
  private DataGridViewTextBoxColumn ecuPartNumber;
  private ToolTip helpToolTip;
  private TableLayoutPanel tableLayoutPanel1;
  private System.Windows.Forms.Label vehicleIdNumberLabel;
  private RadioButton radioButtonVIN;
  private RadioButton radioButtonPIN;
  private IContainer components;

  public string EngineSerialNumber
  {
    get => this.engineSerialNumberTextBox.Text;
    set => this.engineSerialNumberTextBox.Text = value ?? string.Empty;
  }

  public string VehicleIdNumber
  {
    get => this.vehicleIdNumberTextBox.Text;
    set => this.vehicleIdNumberTextBox.Text = value ?? string.Empty;
  }

  public IEnumerable<EcuHardwarePart> EcuHardwarePartNumbers
  {
    get
    {
      return this.ecuHardwarePartNumbers.Where<AddUnitEcuHardwarePartNumber>((Func<AddUnitEcuHardwarePartNumber, bool>) (x => !string.IsNullOrEmpty(x.EcuName) && !string.IsNullOrEmpty(x.EcuPartNumberDisplayValue) && !SapiManager.ProgramDeviceUsesSoftwareIdentification(x.EcuName))).Select<AddUnitEcuHardwarePartNumber, EcuHardwarePart>((Func<AddUnitEcuHardwarePartNumber, EcuHardwarePart>) (addUnitEcuHardwarePartNumber => new EcuHardwarePart(addUnitEcuHardwarePartNumber.EcuName, addUnitEcuHardwarePartNumber.EcuPartNumber)));
    }
  }

  public IEnumerable<EcuSoftwareIdentification> EcuSoftwareIdentificationItems
  {
    get
    {
      return this.ecuHardwarePartNumbers.Where<AddUnitEcuHardwarePartNumber>((Func<AddUnitEcuHardwarePartNumber, bool>) (x => !string.IsNullOrEmpty(x.EcuName) && !string.IsNullOrEmpty(x.EcuPartNumberDisplayValue) && SapiManager.ProgramDeviceUsesSoftwareIdentification(x.EcuName))).Select<AddUnitEcuHardwarePartNumber, EcuSoftwareIdentification>((Func<AddUnitEcuHardwarePartNumber, EcuSoftwareIdentification>) (addUnitEcuHardwarePartNumber => new EcuSoftwareIdentification(addUnitEcuHardwarePartNumber.EcuName, addUnitEcuHardwarePartNumber.EcuPartNumberDisplayValue)));
    }
  }

  public AddUnitDialog()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.helpToolTip.SetToolTip((Control) this.ecuInfoLabel, Resources.HelpToolTip_AddEditEcuHardwarePartNumbers_Overview);
    this.helpToolTip.SetToolTip((Control) this.dataGridToolStrip, Resources.HelpToolTip_AddEditEcuHardwarePartNumbers_Overview);
    this.dataGridViewEcuHardware.DataError += new DataGridViewDataErrorEventHandler(this.DataGridViewEcuHardware_DataError);
    this.InitializeFormData();
  }

  private void InitializeFormData()
  {
    this.EngineSerialNumber = SapiManager.GlobalInstance.CurrentEngineSerialNumber ?? string.Empty;
    this.VehicleIdNumber = SapiManager.GlobalInstance.CurrentVehicleIdentification ?? string.Empty;
    this.radioButtonVIN.Checked = true;
    this.ecu.DataSource = (object) AddUnitEcuHardwarePartNumber.EcuNamesAvailable;
    this.ecuHardwarePartNumbers = AddUnitDialog.ConnectedHardwareParts.ToList<AddUnitEcuHardwarePartNumber>();
    this.ecuHardwarePartsBindingList = new BindingList<AddUnitEcuHardwarePartNumber>((IList<AddUnitEcuHardwarePartNumber>) this.ecuHardwarePartNumbers);
    this.dataGridViewEcuHardware.DataSource = (object) this.ecuHardwarePartsBindingList;
    this.UpdateDataGridControls();
    this.UpdateUserInterface();
  }

  public static IEnumerable<AddUnitEcuHardwarePartNumber> ConnectedHardwareParts
  {
    get
    {
      List<AddUnitEcuHardwarePartNumber> collection = new List<AddUnitEcuHardwarePartNumber>(SapiManager.GlobalInstance.ActiveChannels.Where<Channel>((Func<Channel, bool>) (c => c.IsRollCall)).Select(channel => new
      {
        channel = channel,
        relatedEcu = SapiExtensions.GetSuppressedOfflineRelatedEcu(channel)
      }).Where(_param1 => _param1.relatedEcu != null && SapiManager.ProgramDeviceUsesSoftwareIdentification(_param1.relatedEcu)).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        partNumber = SapiManager.GetSoftwareIdentification(_param1.channel)
      }).Where(_param1 => !string.IsNullOrEmpty(_param1.partNumber)).Select(_param1 => new AddUnitEcuHardwarePartNumber(_param1.\u003C\u003Eh__TransparentIdentifier0.relatedEcu.Name, _param1.partNumber, false)));
      List<AddUnitEcuHardwarePartNumber> connectedHardwareParts = new List<AddUnitEcuHardwarePartNumber>(((IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels.Where<Channel>((Func<Channel, bool>) (x =>
      {
        if (x.IsRollCall && !SapiManager.SupportsRollCallParameterization(x.Ecu))
          return false;
        return SapiExtensions.IsDataSourceDepot(x.Ecu) || SapiExtensions.IsDataSourceEdex(x.Ecu);
      })).OrderBy<Channel, int>((Func<Channel, int>) (x => x.Ecu.Priority)).ToArray<Channel>()).Select(channel => new
      {
        channel = channel,
        usesSoftwareIdent = SapiManager.ProgramDeviceUsesSoftwareIdentification(channel.Ecu)
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        partNumber = _param1.usesSoftwareIdent ? SapiManager.GetSoftwareIdentification(_param1.channel) : SapiManager.GetHardwarePartNumber(_param1.channel)
      }).Where(_param1 => !string.IsNullOrEmpty(_param1.partNumber)).Select(_param1 => new AddUnitEcuHardwarePartNumber(_param1.\u003C\u003Eh__TransparentIdentifier0.channel.Ecu.Name, _param1.partNumber, !_param1.\u003C\u003Eh__TransparentIdentifier0.usesSoftwareIdent)));
      connectedHardwareParts.AddRange((IEnumerable<AddUnitEcuHardwarePartNumber>) collection);
      return (IEnumerable<AddUnitEcuHardwarePartNumber>) connectedHardwareParts;
    }
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
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (AddUnitDialog));
    this.engineSerialNumberLabel = new System.Windows.Forms.Label();
    this.engineSerialNumberTextBox = new TextBox();
    this.vehicleIdNumberTextBox = new TextBox();
    this.mainInfoLabel = new System.Windows.Forms.Label();
    this.okButton = new Button();
    this.cancelButton = new Button();
    this.dataGridViewEcuHardware = new DataGridView();
    this.ecu = new DataGridViewComboBoxColumn();
    this.ecuPartNumber = new DataGridViewTextBoxColumn();
    this.rootTableLayoutPanel = new TableLayoutPanel();
    this.formButtonsPanel = new Panel();
    this.clearFormButton = new Button();
    this.ecuInfoLabel = new System.Windows.Forms.Label();
    this.ecuTableLayoutPanel = new TableLayoutPanel();
    this.dataGridToolStrip = new ToolStrip();
    this.addEcuItemButton = new ToolStripButton();
    this.removeEcuItemButton = new ToolStripButton();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.vehicleIdNumberLabel = new System.Windows.Forms.Label();
    this.radioButtonVIN = new RadioButton();
    this.radioButtonPIN = new RadioButton();
    this.helpToolTip = new ToolTip(this.components);
    ((ISupportInitialize) this.dataGridViewEcuHardware).BeginInit();
    this.rootTableLayoutPanel.SuspendLayout();
    this.formButtonsPanel.SuspendLayout();
    this.ecuTableLayoutPanel.SuspendLayout();
    this.dataGridToolStrip.SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.engineSerialNumberLabel, "engineSerialNumberLabel");
    this.engineSerialNumberLabel.Name = "engineSerialNumberLabel";
    this.engineSerialNumberTextBox.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.engineSerialNumberTextBox, "engineSerialNumberTextBox");
    this.engineSerialNumberTextBox.Name = "engineSerialNumberTextBox";
    this.engineSerialNumberTextBox.TextChanged += new EventHandler(this.EngineSerialNumber_TextChanged);
    this.engineSerialNumberTextBox.KeyPress += new KeyPressEventHandler(this.EngineSerialNumber_KeyPress);
    this.vehicleIdNumberTextBox.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.vehicleIdNumberTextBox, "vehicleIdNumberTextBox");
    this.vehicleIdNumberTextBox.Name = "vehicleIdNumberTextBox";
    this.vehicleIdNumberTextBox.TextChanged += new EventHandler(this.VehicleIdNumber_TextChanged);
    this.vehicleIdNumberTextBox.KeyPress += new KeyPressEventHandler(this.VehicleIdNumber_KeyPress);
    componentResourceManager.ApplyResources((object) this.mainInfoLabel, "mainInfoLabel");
    this.mainInfoLabel.Name = "mainInfoLabel";
    componentResourceManager.ApplyResources((object) this.okButton, "okButton");
    this.okButton.DialogResult = DialogResult.OK;
    this.okButton.Name = "okButton";
    this.okButton.Click += new EventHandler(this.okButton_Click);
    componentResourceManager.ApplyResources((object) this.cancelButton, "cancelButton");
    this.cancelButton.DialogResult = DialogResult.Cancel;
    this.cancelButton.Name = "cancelButton";
    this.dataGridViewEcuHardware.AllowUserToAddRows = false;
    this.dataGridViewEcuHardware.AllowUserToDeleteRows = false;
    this.dataGridViewEcuHardware.AllowUserToResizeColumns = false;
    this.dataGridViewEcuHardware.AllowUserToResizeRows = false;
    this.dataGridViewEcuHardware.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    this.dataGridViewEcuHardware.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
    this.dataGridViewEcuHardware.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    this.dataGridViewEcuHardware.Columns.AddRange((DataGridViewColumn) this.ecu, (DataGridViewColumn) this.ecuPartNumber);
    componentResourceManager.ApplyResources((object) this.dataGridViewEcuHardware, "dataGridViewEcuHardware");
    this.dataGridViewEcuHardware.MultiSelect = false;
    this.dataGridViewEcuHardware.Name = "dataGridViewEcuHardware";
    this.dataGridViewEcuHardware.CellValueChanged += new DataGridViewCellEventHandler(this.DataGrid_CellValueChanged);
    this.dataGridViewEcuHardware.RowsAdded += new DataGridViewRowsAddedEventHandler(this.DataGrid_RowsAdded);
    this.dataGridViewEcuHardware.RowsRemoved += new DataGridViewRowsRemovedEventHandler(this.DataGrid_RowsRemoved);
    this.dataGridViewEcuHardware.RowValidating += new DataGridViewCellCancelEventHandler(this.DataGrid_RowValidating);
    this.ecu.DataPropertyName = "EcuName";
    this.ecu.FillWeight = 35f;
    this.ecu.FlatStyle = FlatStyle.Flat;
    componentResourceManager.ApplyResources((object) this.ecu, "ecu");
    this.ecu.Name = "ecu";
    this.ecuPartNumber.DataPropertyName = "EcuPartNumberDisplayValue";
    this.ecuPartNumber.FillWeight = 65f;
    componentResourceManager.ApplyResources((object) this.ecuPartNumber, "ecuPartNumber");
    this.ecuPartNumber.MaxInputLength = 20;
    this.ecuPartNumber.Name = "ecuPartNumber";
    componentResourceManager.ApplyResources((object) this.rootTableLayoutPanel, "rootTableLayoutPanel");
    this.rootTableLayoutPanel.Controls.Add((Control) this.mainInfoLabel, 0, 0);
    this.rootTableLayoutPanel.Controls.Add((Control) this.vehicleIdNumberTextBox, 0, 2);
    this.rootTableLayoutPanel.Controls.Add((Control) this.engineSerialNumberLabel, 0, 3);
    this.rootTableLayoutPanel.Controls.Add((Control) this.engineSerialNumberTextBox, 0, 4);
    this.rootTableLayoutPanel.Controls.Add((Control) this.formButtonsPanel, 0, 7);
    this.rootTableLayoutPanel.Controls.Add((Control) this.ecuInfoLabel, 0, 5);
    this.rootTableLayoutPanel.Controls.Add((Control) this.ecuTableLayoutPanel, 0, 6);
    this.rootTableLayoutPanel.Controls.Add((Control) this.tableLayoutPanel1, 0, 1);
    this.rootTableLayoutPanel.Name = "rootTableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.formButtonsPanel, "formButtonsPanel");
    this.formButtonsPanel.Controls.Add((Control) this.clearFormButton);
    this.formButtonsPanel.Controls.Add((Control) this.okButton);
    this.formButtonsPanel.Controls.Add((Control) this.cancelButton);
    this.formButtonsPanel.Name = "formButtonsPanel";
    componentResourceManager.ApplyResources((object) this.clearFormButton, "clearFormButton");
    this.clearFormButton.Name = "clearFormButton";
    this.clearFormButton.UseVisualStyleBackColor = true;
    this.clearFormButton.Click += new EventHandler(this.ClearButton_Click);
    componentResourceManager.ApplyResources((object) this.ecuInfoLabel, "ecuInfoLabel");
    this.ecuInfoLabel.Name = "ecuInfoLabel";
    componentResourceManager.ApplyResources((object) this.ecuTableLayoutPanel, "ecuTableLayoutPanel");
    this.ecuTableLayoutPanel.Controls.Add((Control) this.dataGridToolStrip, 0, 0);
    this.ecuTableLayoutPanel.Controls.Add((Control) this.dataGridViewEcuHardware, 0, 1);
    this.ecuTableLayoutPanel.Name = "ecuTableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.dataGridToolStrip, "dataGridToolStrip");
    this.dataGridToolStrip.GripStyle = ToolStripGripStyle.Hidden;
    this.dataGridToolStrip.Items.AddRange(new ToolStripItem[2]
    {
      (ToolStripItem) this.addEcuItemButton,
      (ToolStripItem) this.removeEcuItemButton
    });
    this.dataGridToolStrip.Name = "dataGridToolStrip";
    this.dataGridToolStrip.TabStop = true;
    this.addEcuItemButton.Image = (Image) Resources.add;
    componentResourceManager.ApplyResources((object) this.addEcuItemButton, "addEcuItemButton");
    this.addEcuItemButton.Name = "addEcuItemButton";
    this.addEcuItemButton.Click += new EventHandler(this.AddEcuItemButton_Click);
    this.removeEcuItemButton.Image = (Image) Resources.remove;
    componentResourceManager.ApplyResources((object) this.removeEcuItemButton, "removeEcuItemButton");
    this.removeEcuItemButton.Name = "removeEcuItemButton";
    this.removeEcuItemButton.Click += new EventHandler(this.RemoveEcuItemButton_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.vehicleIdNumberLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.radioButtonVIN, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.radioButtonPIN, 2, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.vehicleIdNumberLabel, "vehicleIdNumberLabel");
    this.vehicleIdNumberLabel.Name = "vehicleIdNumberLabel";
    componentResourceManager.ApplyResources((object) this.radioButtonVIN, "radioButtonVIN");
    this.radioButtonVIN.Name = "radioButtonVIN";
    this.radioButtonVIN.TabStop = true;
    this.radioButtonVIN.UseVisualStyleBackColor = true;
    this.radioButtonVIN.Click += new EventHandler(this.radioButtonVIN_Click);
    componentResourceManager.ApplyResources((object) this.radioButtonPIN, "radioButtonPIN");
    this.radioButtonPIN.Name = "radioButtonPIN";
    this.radioButtonPIN.TabStop = true;
    this.radioButtonPIN.UseVisualStyleBackColor = true;
    this.radioButtonPIN.Click += new EventHandler(this.radioButtonPIN_Click);
    this.helpToolTip.AutoPopDelay = 15000;
    this.helpToolTip.InitialDelay = 500;
    this.helpToolTip.IsBalloon = true;
    this.helpToolTip.ReshowDelay = 100;
    this.helpToolTip.ToolTipIcon = ToolTipIcon.Info;
    this.helpToolTip.ToolTipTitle = "Help";
    this.AcceptButton = (IButtonControl) this.okButton;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.cancelButton;
    this.ControlBox = false;
    this.Controls.Add((Control) this.rootTableLayoutPanel);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (AddUnitDialog);
    this.ShowInTaskbar = false;
    this.SizeGripStyle = SizeGripStyle.Hide;
    ((ISupportInitialize) this.dataGridViewEcuHardware).EndInit();
    this.rootTableLayoutPanel.ResumeLayout(false);
    this.rootTableLayoutPanel.PerformLayout();
    this.formButtonsPanel.ResumeLayout(false);
    this.ecuTableLayoutPanel.ResumeLayout(false);
    this.ecuTableLayoutPanel.PerformLayout();
    this.dataGridToolStrip.ResumeLayout(false);
    this.dataGridToolStrip.PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    this.ResumeLayout(false);
  }

  private void okButton_Click(object sender, EventArgs e)
  {
    this.engineSerialNumberTextBox.Text = this.engineSerialNumberTextBox.Text.Trim();
    this.vehicleIdNumberTextBox.Text = this.vehicleIdNumberTextBox.Text.Trim();
    if (this.engineSerialNumberTextBox.Text.Length == 0 && this.vehicleIdNumberTextBox.Text.Length == 0)
      this.ReportValidationError(Resources.MessageMustEnterAValidVehicleNumberOrEngineSerialNumber, (Control) this.engineSerialNumberTextBox);
    else if (this.vehicleIdNumberTextBox.Text.Length > 0)
    {
      if (this.radioButtonPIN.Checked)
      {
        if (AddUnitDialog.IsSupportedPIN(this.vehicleIdNumberTextBox.Text))
          return;
        this.ReportValidationError(Resources.MessageMustEnterAValidProductIdentificationNumber, (Control) this.vehicleIdNumberTextBox);
      }
      else
      {
        if (AddUnitDialog.IsSupportedVIN(this.vehicleIdNumberTextBox.Text))
          return;
        this.ReportValidationError(Resources.MessageMustEnterAValidVehicleIdentificationNumber, (Control) this.vehicleIdNumberTextBox);
      }
    }
    else
    {
      if (this.vehicleIdNumberTextBox.Text.Length != 0 || this.dataGridViewEcuHardware.Rows.Count <= 0 || this.AskUserYesNoQuestion(Resources.WarningUserYesNoQuestion_VinIsNeededToContinueDataDownload))
        return;
      this.DialogResult = DialogResult.None;
      this.vehicleIdNumberTextBox.Focus();
    }
  }

  private void ReportValidationError(string errorMessage, Control failedOn)
  {
    int num = (int) ControlHelpers.ShowMessageBox((Control) this, errorMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    this.DialogResult = DialogResult.None;
    failedOn.Focus();
  }

  private bool AskUserYesNoQuestion(string question)
  {
    return ControlHelpers.ShowMessageBox((Control) this, question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
  }

  private void EngineSerialNumber_TextChanged(object sender, EventArgs e)
  {
    if (this.engineSerialNumberTextBox.Text.Length <= 0 || !this.engineSerialNumberTextBox.Text.Contains(" "))
      return;
    this.engineSerialNumberTextBox.Text = this.engineSerialNumberTextBox.Text.Replace(" ", "");
  }

  private void EngineSerialNumber_KeyPress(object sender, KeyPressEventArgs e)
  {
    if (e.KeyChar != ' ')
      return;
    e.Handled = true;
  }

  private void radioButtonVIN_Click(object sender, EventArgs e) => this.UpdateUserInterface();

  private void radioButtonPIN_Click(object sender, EventArgs e) => this.UpdateUserInterface();

  private void VehicleIdNumber_TextChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void VehicleIdNumber_KeyPress(object sender, KeyPressEventArgs e)
  {
    e.Handled = this.radioButtonPIN.Checked ? !VehicleIdentification.IsValidPinCharacter(e.KeyChar) : !VehicleIdentification.IsValidVinCharacter(e.KeyChar);
  }

  private void UpdateUserInterface()
  {
    if (this.vehicleIdNumberTextBox.ReadOnly)
      this.vehicleIdNumberTextBox.BackColor = SystemColors.Control;
    else
      this.vehicleIdNumberTextBox.BackColor = (this.radioButtonPIN.Checked ? VehicleIdentification.IsValidPin(this.vehicleIdNumberTextBox.Text) && !VehicleIdentification.IsValidVin(this.vehicleIdNumberTextBox.Text) : VehicleIdentification.IsValidVin(this.vehicleIdNumberTextBox.Text)) ? Color.PaleGreen : Color.LightPink;
  }

  private void RemoveEcuItemButton_Click(object sender, EventArgs e)
  {
    if (this.dataGridViewEcuHardware.CurrentRow == null)
      return;
    this.ecuHardwarePartsBindingList.RemoveAt(this.dataGridViewEcuHardware.CurrentRow.Index);
  }

  private void AddEcuItemButton_Click(object sender, EventArgs e)
  {
    this.ecuHardwarePartsBindingList.AddNew();
  }

  private void ClearButton_Click(object sender, EventArgs e)
  {
    this.VehicleIdNumber = string.Empty;
    this.EngineSerialNumber = string.Empty;
    this.ecuHardwarePartsBindingList.Clear();
  }

  private void UpdateDataGridControls()
  {
    if (this.ecuHardwarePartNumbers == null)
      return;
    this.removeEcuItemButton.Enabled = this.ecuHardwarePartNumbers.Count > 0;
    this.addEcuItemButton.Enabled = this.ecuHardwarePartNumbers.Count == 0 || this.ecuHardwarePartNumbers.Last<AddUnitEcuHardwarePartNumber>().EcuPartNumber != null && !string.IsNullOrEmpty(this.ecuHardwarePartNumbers.Last<AddUnitEcuHardwarePartNumber>().EcuName) || SapiManager.ProgramDeviceUsesSoftwareIdentification(this.ecuHardwarePartNumbers.Last<AddUnitEcuHardwarePartNumber>().EcuName) && !string.IsNullOrEmpty(this.ecuHardwarePartNumbers.Last<AddUnitEcuHardwarePartNumber>().EcuPartNumberDisplayValue);
  }

  private void DataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
  {
    this.UpdateDataGridControls();
  }

  private void DataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
  {
    this.UpdateDataGridControls();
  }

  private void DataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
  {
    if (e.RowIndex > -1 && e.ColumnIndex == 0)
    {
      string str = this.dataGridViewEcuHardware.Rows[e.RowIndex].Cells[0].Value as string;
      this.ecuHardwarePartNumbers[e.RowIndex].FormatPartNumber = !SapiManager.ProgramDeviceUsesSoftwareIdentification(str);
    }
    this.UpdateDataGridControls();
  }

  private void DataGridViewEcuHardware_DataError(object sender, DataGridViewDataErrorEventArgs e)
  {
    this.dataGridViewEcuHardware.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = e.Exception.Message;
  }

  private static bool IsSupportedVIN(string value)
  {
    return VehicleIdentification.GetClassification(value) == 1;
  }

  private static bool IsSupportedPIN(string value)
  {
    return VehicleIdentification.GetClassification(value) == 2;
  }

  private static bool IsSupportedVehicleIdentifier(string value, bool isPIN)
  {
    return VehicleIdentification.GetClassification(value) == (isPIN ? 2 : 1);
  }

  private void DataGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
  {
    DataGridViewRow row = this.dataGridViewEcuHardware.Rows[e.RowIndex];
    string ecuName = row.Cells[0].Value as string;
    string str = row.Cells[1].Value as string;
    row.Cells[0].ErrorText = string.IsNullOrEmpty(ecuName) ? Resources.MustSelectAnEcu : string.Empty;
    try
    {
      Ecu ecuByName = SapiManager.GetEcuByName(ecuName);
      if (ecuByName != null && !SapiExtensions.IsDataSourceEdex(ecuByName) && this.ecuHardwarePartNumbers.FindAll((Predicate<AddUnitEcuHardwarePartNumber>) (x => x.EcuName.Equals(ecuName) && x.EcuPartNumber != null)).Count<AddUnitEcuHardwarePartNumber>() > 1)
      {
        row.Cells[1].ErrorText = Resources.DuplicatePowertrainEcusNotAllowed;
        row.Cells[1].Value = (object) string.Empty;
      }
      if (new AddUnitEcuHardwarePartNumber(ecuName ?? string.Empty, str ?? string.Empty, !SapiManager.ProgramDeviceUsesSoftwareIdentification(ecuName)).EcuPartNumberDisplayValue.Length == 0)
      {
        row.Cells[1].ErrorText = Resources.HardwarePartNumberIsRequired;
        row.Cells[1].Value = (object) string.Empty;
      }
      else
        row.Cells[1].ErrorText = string.Empty;
    }
    catch (ArgumentException ex)
    {
      row.Cells[1].ErrorText = Resources.HardwarePartNumberIsRequired;
    }
  }
}
