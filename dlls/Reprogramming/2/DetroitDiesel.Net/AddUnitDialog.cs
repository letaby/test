using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;

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
		get
		{
			return engineSerialNumberTextBox.Text;
		}
		set
		{
			engineSerialNumberTextBox.Text = value ?? string.Empty;
		}
	}

	public string VehicleIdNumber
	{
		get
		{
			return vehicleIdNumberTextBox.Text;
		}
		set
		{
			vehicleIdNumberTextBox.Text = value ?? string.Empty;
		}
	}

	public IEnumerable<EcuHardwarePart> EcuHardwarePartNumbers => ecuHardwarePartNumbers.Where((AddUnitEcuHardwarePartNumber x) => !string.IsNullOrEmpty(x.EcuName) && !string.IsNullOrEmpty(x.EcuPartNumberDisplayValue) && !SapiManager.ProgramDeviceUsesSoftwareIdentification(x.EcuName)).Select((Func<AddUnitEcuHardwarePartNumber, EcuHardwarePart>)((AddUnitEcuHardwarePartNumber addUnitEcuHardwarePartNumber) => new EcuHardwarePart(addUnitEcuHardwarePartNumber.EcuName, addUnitEcuHardwarePartNumber.EcuPartNumber)));

	public IEnumerable<EcuSoftwareIdentification> EcuSoftwareIdentificationItems => ecuHardwarePartNumbers.Where((AddUnitEcuHardwarePartNumber x) => !string.IsNullOrEmpty(x.EcuName) && !string.IsNullOrEmpty(x.EcuPartNumberDisplayValue) && SapiManager.ProgramDeviceUsesSoftwareIdentification(x.EcuName)).Select((Func<AddUnitEcuHardwarePartNumber, EcuSoftwareIdentification>)((AddUnitEcuHardwarePartNumber addUnitEcuHardwarePartNumber) => new EcuSoftwareIdentification(addUnitEcuHardwarePartNumber.EcuName, addUnitEcuHardwarePartNumber.EcuPartNumberDisplayValue)));

	public static IEnumerable<AddUnitEcuHardwarePartNumber> ConnectedHardwareParts
	{
		get
		{
			List<AddUnitEcuHardwarePartNumber> collection = new List<AddUnitEcuHardwarePartNumber>(from channel in SapiManager.GlobalInstance.ActiveChannels
				where channel.IsRollCall
				let relatedEcu = SapiExtensions.GetSuppressedOfflineRelatedEcu(channel)
				where relatedEcu != null && SapiManager.ProgramDeviceUsesSoftwareIdentification(relatedEcu)
				let partNumber = SapiManager.GetSoftwareIdentification(channel)
				where !string.IsNullOrEmpty(partNumber)
				select new AddUnitEcuHardwarePartNumber(relatedEcu.Name, partNumber, formatPartNumber: false));
			Channel[] source = (from x in SapiManager.GlobalInstance.ActiveChannels
				where (!x.IsRollCall || SapiManager.SupportsRollCallParameterization(x.Ecu)) && (SapiExtensions.IsDataSourceDepot(x.Ecu) || SapiExtensions.IsDataSourceEdex(x.Ecu))
				orderby x.Ecu.Priority
				select x).ToArray();
			List<AddUnitEcuHardwarePartNumber> list = new List<AddUnitEcuHardwarePartNumber>(from channel in source
				let usesSoftwareIdent = SapiManager.ProgramDeviceUsesSoftwareIdentification(channel.Ecu)
				let partNumber = usesSoftwareIdent ? SapiManager.GetSoftwareIdentification(channel) : SapiManager.GetHardwarePartNumber(channel)
				where !string.IsNullOrEmpty(partNumber)
				select new AddUnitEcuHardwarePartNumber(channel.Ecu.Name, partNumber, !usesSoftwareIdent));
			list.AddRange(collection);
			return list;
		}
	}

	public AddUnitDialog()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		helpToolTip.SetToolTip(ecuInfoLabel, Resources.HelpToolTip_AddEditEcuHardwarePartNumbers_Overview);
		helpToolTip.SetToolTip(dataGridToolStrip, Resources.HelpToolTip_AddEditEcuHardwarePartNumbers_Overview);
		dataGridViewEcuHardware.DataError += DataGridViewEcuHardware_DataError;
		InitializeFormData();
	}

	private void InitializeFormData()
	{
		EngineSerialNumber = SapiManager.GlobalInstance.CurrentEngineSerialNumber ?? string.Empty;
		VehicleIdNumber = SapiManager.GlobalInstance.CurrentVehicleIdentification ?? string.Empty;
		radioButtonVIN.Checked = true;
		ecu.DataSource = AddUnitEcuHardwarePartNumber.EcuNamesAvailable;
		ecuHardwarePartNumbers = ConnectedHardwareParts.ToList();
		ecuHardwarePartsBindingList = new BindingList<AddUnitEcuHardwarePartNumber>(ecuHardwarePartNumbers);
		dataGridViewEcuHardware.DataSource = ecuHardwarePartsBindingList;
		UpdateDataGridControls();
		UpdateUserInterface();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Net.AddUnitDialog));
		this.engineSerialNumberLabel = new System.Windows.Forms.Label();
		this.engineSerialNumberTextBox = new System.Windows.Forms.TextBox();
		this.vehicleIdNumberTextBox = new System.Windows.Forms.TextBox();
		this.mainInfoLabel = new System.Windows.Forms.Label();
		this.okButton = new System.Windows.Forms.Button();
		this.cancelButton = new System.Windows.Forms.Button();
		this.dataGridViewEcuHardware = new System.Windows.Forms.DataGridView();
		this.ecu = new System.Windows.Forms.DataGridViewComboBoxColumn();
		this.ecuPartNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.rootTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.formButtonsPanel = new System.Windows.Forms.Panel();
		this.clearFormButton = new System.Windows.Forms.Button();
		this.ecuInfoLabel = new System.Windows.Forms.Label();
		this.ecuTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.dataGridToolStrip = new System.Windows.Forms.ToolStrip();
		this.addEcuItemButton = new System.Windows.Forms.ToolStripButton();
		this.removeEcuItemButton = new System.Windows.Forms.ToolStripButton();
		this.tableLayoutPanel1 = new TableLayoutPanel();
		this.vehicleIdNumberLabel = new System.Windows.Forms.Label();
		this.radioButtonVIN = new System.Windows.Forms.RadioButton();
		this.radioButtonPIN = new System.Windows.Forms.RadioButton();
		this.helpToolTip = new System.Windows.Forms.ToolTip(this.components);
		((System.ComponentModel.ISupportInitialize)this.dataGridViewEcuHardware).BeginInit();
		this.rootTableLayoutPanel.SuspendLayout();
		this.formButtonsPanel.SuspendLayout();
		this.ecuTableLayoutPanel.SuspendLayout();
		this.dataGridToolStrip.SuspendLayout();
		((System.Windows.Forms.Control)(object)this.tableLayoutPanel1).SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.engineSerialNumberLabel, "engineSerialNumberLabel");
		this.engineSerialNumberLabel.Name = "engineSerialNumberLabel";
		this.engineSerialNumberTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		resources.ApplyResources(this.engineSerialNumberTextBox, "engineSerialNumberTextBox");
		this.engineSerialNumberTextBox.Name = "engineSerialNumberTextBox";
		this.engineSerialNumberTextBox.TextChanged += new System.EventHandler(EngineSerialNumber_TextChanged);
		this.engineSerialNumberTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(EngineSerialNumber_KeyPress);
		this.vehicleIdNumberTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
		resources.ApplyResources(this.vehicleIdNumberTextBox, "vehicleIdNumberTextBox");
		this.vehicleIdNumberTextBox.Name = "vehicleIdNumberTextBox";
		this.vehicleIdNumberTextBox.TextChanged += new System.EventHandler(VehicleIdNumber_TextChanged);
		this.vehicleIdNumberTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(VehicleIdNumber_KeyPress);
		resources.ApplyResources(this.mainInfoLabel, "mainInfoLabel");
		this.mainInfoLabel.Name = "mainInfoLabel";
		resources.ApplyResources(this.okButton, "okButton");
		this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.okButton.Name = "okButton";
		this.okButton.Click += new System.EventHandler(okButton_Click);
		resources.ApplyResources(this.cancelButton, "cancelButton");
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelButton.Name = "cancelButton";
		this.dataGridViewEcuHardware.AllowUserToAddRows = false;
		this.dataGridViewEcuHardware.AllowUserToDeleteRows = false;
		this.dataGridViewEcuHardware.AllowUserToResizeColumns = false;
		this.dataGridViewEcuHardware.AllowUserToResizeRows = false;
		this.dataGridViewEcuHardware.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dataGridViewEcuHardware.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
		this.dataGridViewEcuHardware.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridViewEcuHardware.Columns.AddRange(this.ecu, this.ecuPartNumber);
		resources.ApplyResources(this.dataGridViewEcuHardware, "dataGridViewEcuHardware");
		this.dataGridViewEcuHardware.MultiSelect = false;
		this.dataGridViewEcuHardware.Name = "dataGridViewEcuHardware";
		this.dataGridViewEcuHardware.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(DataGrid_CellValueChanged);
		this.dataGridViewEcuHardware.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(DataGrid_RowsAdded);
		this.dataGridViewEcuHardware.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(DataGrid_RowsRemoved);
		this.dataGridViewEcuHardware.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(DataGrid_RowValidating);
		this.ecu.DataPropertyName = "EcuName";
		this.ecu.FillWeight = 35f;
		this.ecu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		resources.ApplyResources(this.ecu, "ecu");
		this.ecu.Name = "ecu";
		this.ecuPartNumber.DataPropertyName = "EcuPartNumberDisplayValue";
		this.ecuPartNumber.FillWeight = 65f;
		resources.ApplyResources(this.ecuPartNumber, "ecuPartNumber");
		this.ecuPartNumber.MaxInputLength = 20;
		this.ecuPartNumber.Name = "ecuPartNumber";
		resources.ApplyResources(this.rootTableLayoutPanel, "rootTableLayoutPanel");
		this.rootTableLayoutPanel.Controls.Add(this.mainInfoLabel, 0, 0);
		this.rootTableLayoutPanel.Controls.Add(this.vehicleIdNumberTextBox, 0, 2);
		this.rootTableLayoutPanel.Controls.Add(this.engineSerialNumberLabel, 0, 3);
		this.rootTableLayoutPanel.Controls.Add(this.engineSerialNumberTextBox, 0, 4);
		this.rootTableLayoutPanel.Controls.Add(this.formButtonsPanel, 0, 7);
		this.rootTableLayoutPanel.Controls.Add(this.ecuInfoLabel, 0, 5);
		this.rootTableLayoutPanel.Controls.Add(this.ecuTableLayoutPanel, 0, 6);
		this.rootTableLayoutPanel.Controls.Add((System.Windows.Forms.Control)(object)this.tableLayoutPanel1, 0, 1);
		this.rootTableLayoutPanel.Name = "rootTableLayoutPanel";
		resources.ApplyResources(this.formButtonsPanel, "formButtonsPanel");
		this.formButtonsPanel.Controls.Add(this.clearFormButton);
		this.formButtonsPanel.Controls.Add(this.okButton);
		this.formButtonsPanel.Controls.Add(this.cancelButton);
		this.formButtonsPanel.Name = "formButtonsPanel";
		resources.ApplyResources(this.clearFormButton, "clearFormButton");
		this.clearFormButton.Name = "clearFormButton";
		this.clearFormButton.UseVisualStyleBackColor = true;
		this.clearFormButton.Click += new System.EventHandler(ClearButton_Click);
		resources.ApplyResources(this.ecuInfoLabel, "ecuInfoLabel");
		this.ecuInfoLabel.Name = "ecuInfoLabel";
		resources.ApplyResources(this.ecuTableLayoutPanel, "ecuTableLayoutPanel");
		this.ecuTableLayoutPanel.Controls.Add(this.dataGridToolStrip, 0, 0);
		this.ecuTableLayoutPanel.Controls.Add(this.dataGridViewEcuHardware, 0, 1);
		this.ecuTableLayoutPanel.Name = "ecuTableLayoutPanel";
		resources.ApplyResources(this.dataGridToolStrip, "dataGridToolStrip");
		this.dataGridToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.dataGridToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addEcuItemButton, this.removeEcuItemButton });
		this.dataGridToolStrip.Name = "dataGridToolStrip";
		this.dataGridToolStrip.TabStop = true;
		this.addEcuItemButton.Image = DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties.Resources.add;
		resources.ApplyResources(this.addEcuItemButton, "addEcuItemButton");
		this.addEcuItemButton.Name = "addEcuItemButton";
		this.addEcuItemButton.Click += new System.EventHandler(AddEcuItemButton_Click);
		this.removeEcuItemButton.Image = DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties.Resources.remove;
		resources.ApplyResources(this.removeEcuItemButton, "removeEcuItemButton");
		this.removeEcuItemButton.Name = "removeEcuItemButton";
		this.removeEcuItemButton.Click += new System.EventHandler(RemoveEcuItemButton_Click);
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel1).Controls.Add(this.vehicleIdNumberLabel, 0, 0);
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel1).Controls.Add(this.radioButtonVIN, 1, 0);
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel1).Controls.Add(this.radioButtonPIN, 2, 0);
		((System.Windows.Forms.Control)(object)this.tableLayoutPanel1).Name = "tableLayoutPanel1";
		resources.ApplyResources(this.vehicleIdNumberLabel, "vehicleIdNumberLabel");
		this.vehicleIdNumberLabel.Name = "vehicleIdNumberLabel";
		resources.ApplyResources(this.radioButtonVIN, "radioButtonVIN");
		this.radioButtonVIN.Name = "radioButtonVIN";
		this.radioButtonVIN.TabStop = true;
		this.radioButtonVIN.UseVisualStyleBackColor = true;
		this.radioButtonVIN.Click += new System.EventHandler(radioButtonVIN_Click);
		resources.ApplyResources(this.radioButtonPIN, "radioButtonPIN");
		this.radioButtonPIN.Name = "radioButtonPIN";
		this.radioButtonPIN.TabStop = true;
		this.radioButtonPIN.UseVisualStyleBackColor = true;
		this.radioButtonPIN.Click += new System.EventHandler(radioButtonPIN_Click);
		this.helpToolTip.AutoPopDelay = 15000;
		this.helpToolTip.InitialDelay = 500;
		this.helpToolTip.IsBalloon = true;
		this.helpToolTip.ReshowDelay = 100;
		this.helpToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
		this.helpToolTip.ToolTipTitle = "Help";
		base.AcceptButton = this.okButton;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.cancelButton;
		base.ControlBox = false;
		base.Controls.Add(this.rootTableLayoutPanel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AddUnitDialog";
		base.ShowInTaskbar = false;
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		((System.ComponentModel.ISupportInitialize)this.dataGridViewEcuHardware).EndInit();
		this.rootTableLayoutPanel.ResumeLayout(false);
		this.rootTableLayoutPanel.PerformLayout();
		this.formButtonsPanel.ResumeLayout(false);
		this.ecuTableLayoutPanel.ResumeLayout(false);
		this.ecuTableLayoutPanel.PerformLayout();
		this.dataGridToolStrip.ResumeLayout(false);
		this.dataGridToolStrip.PerformLayout();
		((System.Windows.Forms.Control)(object)this.tableLayoutPanel1).ResumeLayout(false);
		((System.Windows.Forms.Control)(object)this.tableLayoutPanel1).PerformLayout();
		base.ResumeLayout(false);
	}

	private void okButton_Click(object sender, EventArgs e)
	{
		engineSerialNumberTextBox.Text = engineSerialNumberTextBox.Text.Trim();
		vehicleIdNumberTextBox.Text = vehicleIdNumberTextBox.Text.Trim();
		if (engineSerialNumberTextBox.Text.Length == 0 && vehicleIdNumberTextBox.Text.Length == 0)
		{
			ReportValidationError(Resources.MessageMustEnterAValidVehicleNumberOrEngineSerialNumber, engineSerialNumberTextBox);
		}
		else if (vehicleIdNumberTextBox.Text.Length > 0)
		{
			if (radioButtonPIN.Checked)
			{
				if (!IsSupportedPIN(vehicleIdNumberTextBox.Text))
				{
					ReportValidationError(Resources.MessageMustEnterAValidProductIdentificationNumber, vehicleIdNumberTextBox);
				}
			}
			else if (!IsSupportedVIN(vehicleIdNumberTextBox.Text))
			{
				ReportValidationError(Resources.MessageMustEnterAValidVehicleIdentificationNumber, vehicleIdNumberTextBox);
			}
		}
		else if (vehicleIdNumberTextBox.Text.Length == 0 && dataGridViewEcuHardware.Rows.Count > 0 && !AskUserYesNoQuestion(Resources.WarningUserYesNoQuestion_VinIsNeededToContinueDataDownload))
		{
			base.DialogResult = DialogResult.None;
			vehicleIdNumberTextBox.Focus();
		}
	}

	private void ReportValidationError(string errorMessage, Control failedOn)
	{
		ControlHelpers.ShowMessageBox((Control)this, errorMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		base.DialogResult = DialogResult.None;
		failedOn.Focus();
	}

	private bool AskUserYesNoQuestion(string question)
	{
		DialogResult dialogResult = ControlHelpers.ShowMessageBox((Control)this, question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
		return dialogResult == DialogResult.Yes;
	}

	private void EngineSerialNumber_TextChanged(object sender, EventArgs e)
	{
		if (engineSerialNumberTextBox.Text.Length > 0 && engineSerialNumberTextBox.Text.Contains(" "))
		{
			engineSerialNumberTextBox.Text = engineSerialNumberTextBox.Text.Replace(" ", "");
		}
	}

	private void EngineSerialNumber_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == ' ')
		{
			e.Handled = true;
		}
	}

	private void radioButtonVIN_Click(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void radioButtonPIN_Click(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void VehicleIdNumber_TextChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void VehicleIdNumber_KeyPress(object sender, KeyPressEventArgs e)
	{
		e.Handled = (radioButtonPIN.Checked ? (!VehicleIdentification.IsValidPinCharacter(e.KeyChar)) : (!VehicleIdentification.IsValidVinCharacter(e.KeyChar)));
	}

	private void UpdateUserInterface()
	{
		if (vehicleIdNumberTextBox.ReadOnly)
		{
			vehicleIdNumberTextBox.BackColor = SystemColors.Control;
			return;
		}
		bool flag = ((!radioButtonPIN.Checked) ? VehicleIdentification.IsValidVin(vehicleIdNumberTextBox.Text) : (VehicleIdentification.IsValidPin(vehicleIdNumberTextBox.Text) && !VehicleIdentification.IsValidVin(vehicleIdNumberTextBox.Text)));
		vehicleIdNumberTextBox.BackColor = (flag ? Color.PaleGreen : Color.LightPink);
	}

	private void RemoveEcuItemButton_Click(object sender, EventArgs e)
	{
		if (dataGridViewEcuHardware.CurrentRow != null)
		{
			ecuHardwarePartsBindingList.RemoveAt(dataGridViewEcuHardware.CurrentRow.Index);
		}
	}

	private void AddEcuItemButton_Click(object sender, EventArgs e)
	{
		ecuHardwarePartsBindingList.AddNew();
	}

	private void ClearButton_Click(object sender, EventArgs e)
	{
		VehicleIdNumber = string.Empty;
		EngineSerialNumber = string.Empty;
		ecuHardwarePartsBindingList.Clear();
	}

	private void UpdateDataGridControls()
	{
		if (ecuHardwarePartNumbers != null)
		{
			removeEcuItemButton.Enabled = ecuHardwarePartNumbers.Count > 0;
			addEcuItemButton.Enabled = ecuHardwarePartNumbers.Count == 0 || (ecuHardwarePartNumbers.Last().EcuPartNumber != null && !string.IsNullOrEmpty(ecuHardwarePartNumbers.Last().EcuName)) || (SapiManager.ProgramDeviceUsesSoftwareIdentification(ecuHardwarePartNumbers.Last().EcuName) && !string.IsNullOrEmpty(ecuHardwarePartNumbers.Last().EcuPartNumberDisplayValue));
		}
	}

	private void DataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
	{
		UpdateDataGridControls();
	}

	private void DataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
	{
		UpdateDataGridControls();
	}

	private void DataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
	{
		if (e.RowIndex > -1 && e.ColumnIndex == 0)
		{
			string text = dataGridViewEcuHardware.Rows[e.RowIndex].Cells[0].Value as string;
			ecuHardwarePartNumbers[e.RowIndex].FormatPartNumber = !SapiManager.ProgramDeviceUsesSoftwareIdentification(text);
		}
		UpdateDataGridControls();
	}

	private void DataGridViewEcuHardware_DataError(object sender, DataGridViewDataErrorEventArgs e)
	{
		dataGridViewEcuHardware.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = e.Exception.Message;
	}

	private static bool IsSupportedVIN(string value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		return (int)VehicleIdentification.GetClassification(value) == 1;
	}

	private static bool IsSupportedPIN(string value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		return (int)VehicleIdentification.GetClassification(value) == 2;
	}

	private static bool IsSupportedVehicleIdentifier(string value, bool isPIN)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		VehicleIdClassification classification = VehicleIdentification.GetClassification(value);
		return (int)classification == ((!isPIN) ? 1 : 2);
	}

	private void DataGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
	{
		DataGridViewRow dataGridViewRow = dataGridViewEcuHardware.Rows[e.RowIndex];
		string ecuName = dataGridViewRow.Cells[0].Value as string;
		string text = dataGridViewRow.Cells[1].Value as string;
		dataGridViewRow.Cells[0].ErrorText = (string.IsNullOrEmpty(ecuName) ? Resources.MustSelectAnEcu : string.Empty);
		try
		{
			Ecu ecuByName = SapiManager.GetEcuByName(ecuName);
			if (ecuByName != null && !SapiExtensions.IsDataSourceEdex(ecuByName) && ecuHardwarePartNumbers.FindAll((AddUnitEcuHardwarePartNumber x) => x.EcuName.Equals(ecuName) && x.EcuPartNumber != null).Count() > 1)
			{
				dataGridViewRow.Cells[1].ErrorText = Resources.DuplicatePowertrainEcusNotAllowed;
				dataGridViewRow.Cells[1].Value = string.Empty;
			}
			AddUnitEcuHardwarePartNumber addUnitEcuHardwarePartNumber = new AddUnitEcuHardwarePartNumber(ecuName ?? string.Empty, text ?? string.Empty, !SapiManager.ProgramDeviceUsesSoftwareIdentification(ecuName));
			if (addUnitEcuHardwarePartNumber.EcuPartNumberDisplayValue.Length == 0)
			{
				dataGridViewRow.Cells[1].ErrorText = Resources.HardwarePartNumberIsRequired;
				dataGridViewRow.Cells[1].Value = string.Empty;
			}
			else
			{
				dataGridViewRow.Cells[1].ErrorText = string.Empty;
			}
		}
		catch (ArgumentException)
		{
			dataGridViewRow.Cells[1].ErrorText = Resources.HardwarePartNumberIsRequired;
		}
	}
}
