using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

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
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		base.MinAccessLevel = 1;
		InitializeComponent();
		base.HeaderImage = new Bitmap(GetType(), "option_support.png");
		pictureFileHistory.Image = new Bitmap(GetType(), "option_support_history.png");
		pictureBoxPanels.Image = new Bitmap(GetType(), "option_support_panels.png");
		foreach (object value in Enum.GetValues(typeof(ExpirationAge)))
		{
			comboBoxLogExpirationAge.Items.Add(value);
			comboBoxTraceExpirationAge.Items.Add(value);
		}
		foreach (object value2 in Enum.GetValues(typeof(LargeSummaryFileOption)))
		{
			comboBoxSummaryFileSizeAction.Items.Add(value2);
		}
		comboBoxLevel.Items.Add((object)new AccessLevel("Engineering", 3));
		comboBoxLevel.Items.Add((object)new AccessLevel("Dealer/Distributor", 2));
		comboBoxLevel.Items.Add((object)new AccessLevel("Customer", 1));
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		checkBoxByte.Checked = !TraceLogManager.IsFiltered((StatusMessageType)0);
		checkBoxDebug.Checked = !TraceLogManager.IsFiltered((StatusMessageType)2);
		checkBoxException.Checked = !TraceLogManager.IsFiltered((StatusMessageType)1);
		comboBoxLogExpirationAge.SelectedItem = SapiManager.LogFileExpirationAge;
		comboBoxTraceExpirationAge.SelectedItem = TraceLogManager.TraceLogExpirationAge;
		comboBoxSummaryFileSizeAction.SelectedItem = SapiManager.GlobalInstance.LargeSummaryOptions;
		nudMaxSummaryFileSize.Value = SapiManager.GlobalInstance.SummaryFileMaxSize;
		string text = (labelAdrAdditionalSaveLocation.Text = Reporter.AdditionalSaveLocation);
		adrAdditionalSaveLocation = text;
		checkBoxAdrAdditionalSaveLocation.Checked = !string.IsNullOrEmpty(labelAdrAdditionalSaveLocation.Text);
		buttonBrowse.Enabled = checkBoxAdrAdditionalSaveLocation.Checked;
		checkBoxAlwaysLoad.Visible = false;
		useNameValuePairsToParameterize.Visible = false;
		checkBoxIgnoreLastServicedCheck.Visible = false;
		checkBoxAllowChecEdit.Visible = false;
		comboBoxLevel.Visible = false;
		labelFilterLists.Visible = false;
		pictureBoxPanels.Visible = false;
		labelPanels.Visible = false;
		base.OnLoad(e);
	}

	private void OnCheckByte(object sender, EventArgs e)
	{
		if (checkBoxByte.Checked == TraceLogManager.IsFiltered((StatusMessageType)0))
		{
			MarkDirty();
		}
	}

	private void OnCheckDebug(object sender, EventArgs e)
	{
		if (checkBoxDebug.Checked == TraceLogManager.IsFiltered((StatusMessageType)2))
		{
			MarkDirty();
		}
	}

	private void OnCheckException(object sender, EventArgs e)
	{
		if (checkBoxException.Checked == TraceLogManager.IsFiltered((StatusMessageType)1))
		{
			MarkDirty();
		}
	}

	public override bool ApplySettings()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		if (base.IsDirty)
		{
			TraceLogManager.FilterMessageType((StatusMessageType)0, !checkBoxByte.Checked);
			TraceLogManager.FilterMessageType((StatusMessageType)2, !checkBoxDebug.Checked);
			TraceLogManager.FilterMessageType((StatusMessageType)1, !checkBoxException.Checked);
			TraceLogManager.TraceLogExpirationAge = (ExpirationAge)comboBoxTraceExpirationAge.SelectedItem;
			SapiManager.LogFileExpirationAge = (ExpirationAge)comboBoxLogExpirationAge.SelectedItem;
			if (comboBoxLevel.SelectedItem != null)
			{
				AccessLevel val = (AccessLevel)comboBoxLevel.SelectedItem;
				SapiManager.GlobalInstance.AccessLevelFilter = val.Level;
			}
			PanelBase.AlwaysLoadPanels = checkBoxAlwaysLoad.Checked;
			Reporter.AdditionalSaveLocation = labelAdrAdditionalSaveLocation.Text;
			SapiManager.GlobalInstance.UseNameValuePairsToParameterize = useNameValuePairsToParameterize.Checked;
			SapiManager.GlobalInstance.IgnoreLastServicedCheck = checkBoxIgnoreLastServicedCheck.Checked;
			SapiManager.GlobalInstance.AllowChecParameterEdit = checkBoxAllowChecEdit.Checked;
			SapiManager.GlobalInstance.LargeSummaryOptions = (LargeSummaryFileOption)comboBoxSummaryFileSizeAction.SelectedItem;
			SapiManager.GlobalInstance.SummaryFileMaxSize = (int)nudMaxSummaryFileSize.Value;
		}
		return base.ApplySettings();
	}

	private void OnTraceExpirationCommitted(object sender, EventArgs e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((ExpirationAge)comboBoxTraceExpirationAge.SelectedItem != TraceLogManager.TraceLogExpirationAge)
		{
			MarkDirty();
		}
	}

	private void OnLogExpirationCommitted(object sender, EventArgs e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((ExpirationAge)comboBoxLogExpirationAge.SelectedItem != SapiManager.LogFileExpirationAge)
		{
			MarkDirty();
		}
	}

	private void OnCheckAlwaysLoadPanels(object sender, EventArgs e)
	{
		if (checkBoxAlwaysLoad.Checked != PanelBase.AlwaysLoadPanels)
		{
			MarkDirty();
		}
	}

	private void OnSelectedLevelChanged(object sender, EventArgs e)
	{
		if (comboBoxLevel.SelectedIndex != filterListsItemIndex)
		{
			filterListsItemIndex = comboBoxLevel.SelectedIndex;
			MarkDirty();
		}
	}

	private void nudMaxSummaryFileSize_ValueChanged(object sender, EventArgs e)
	{
		if ((int)nudMaxSummaryFileSize.Value != SapiManager.GlobalInstance.SummaryFileMaxSize)
		{
			MarkDirty();
		}
	}

	private void comboBoxSummaryFileSizeAction_SelectionChangeCommitted(object sender, EventArgs e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if ((LargeSummaryFileOption)comboBoxSummaryFileSizeAction.SelectedItem != SapiManager.GlobalInstance.LargeSummaryOptions)
		{
			MarkDirty();
		}
	}

	private void useNameValuePairsToParameterize_CheckedChanged(object sender, EventArgs e)
	{
		if (useNameValuePairsToParameterize.Checked != SapiManager.GlobalInstance.UseNameValuePairsToParameterize)
		{
			MarkDirty();
		}
	}

	private void checkBoxIgnoreLastServicedCheck_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxIgnoreLastServicedCheck.Checked != SapiManager.GlobalInstance.IgnoreLastServicedCheck)
		{
			MarkDirty();
		}
	}

	private void buttonBrowse_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.SelectedPath = labelAdrAdditionalSaveLocation.Text;
		folderBrowserDialog.ShowNewFolderButton = false;
		folderBrowserDialog.Description = Resources.AdrAdditionSaveLocationDialogMessage;
		if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		try
		{
			labelAdrAdditionalSaveLocation.Text = (adrAdditionalSaveLocation = folderBrowserDialog.SelectedPath);
			if (labelAdrAdditionalSaveLocation.Text != Reporter.AdditionalSaveLocation)
			{
				MarkDirty();
			}
		}
		catch (PathTooLongException)
		{
			ControlHelpers.ShowMessageBox((Control)this, Resources.MessageFormat_PathToLong, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
		}
	}

	private void checkBoxAdrAdditionalSaveLocation_Click(object sender, EventArgs e)
	{
		buttonBrowse.Enabled = checkBoxAdrAdditionalSaveLocation.Checked;
		labelAdrAdditionalSaveLocation.Text = (checkBoxAdrAdditionalSaveLocation.Checked ? adrAdditionalSaveLocation : string.Empty);
		MarkDirty();
	}

	private void checkBoxAllowChecEdit_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxAllowChecEdit.Checked != SapiManager.GlobalInstance.AllowChecParameterEdit)
		{
			MarkDirty();
		}
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.SupportOptionsPanel));
		this.checkBoxByte = new System.Windows.Forms.CheckBox();
		this.checkBoxDebug = new System.Windows.Forms.CheckBox();
		this.checkBoxException = new System.Windows.Forms.CheckBox();
		this.useNameValuePairsToParameterize = new System.Windows.Forms.CheckBox();
		this.checkBoxIgnoreLastServicedCheck = new System.Windows.Forms.CheckBox();
		this.pictureFileHistory = new System.Windows.Forms.PictureBox();
		this.labelFileManagement = new System.Windows.Forms.Label();
		this.comboBoxLogExpirationAge = new System.Windows.Forms.ComboBox();
		this.comboBoxTraceExpirationAge = new System.Windows.Forms.ComboBox();
		this.labelPanels = new System.Windows.Forms.Label();
		this.pictureBoxPanels = new System.Windows.Forms.PictureBox();
		this.checkBoxAlwaysLoad = new System.Windows.Forms.CheckBox();
		this.nudMaxSummaryFileSize = new System.Windows.Forms.NumericUpDown();
		this.comboBoxSummaryFileSizeAction = new System.Windows.Forms.ComboBox();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.panel1 = new System.Windows.Forms.Panel();
		this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
		this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
		this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
		this.checkBoxAllowChecEdit = new System.Windows.Forms.CheckBox();
		this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
		this.comboBoxLevel = new System.Windows.Forms.ComboBox();
		this.labelFilterLists = new System.Windows.Forms.Label();
		this.tableLayoutPanelADRReports = new System.Windows.Forms.TableLayoutPanel();
		this.checkBoxAdrAdditionalSaveLocation = new System.Windows.Forms.CheckBox();
		this.labelAdrAdditionalSaveLocation = new System.Windows.Forms.Label();
		this.buttonBrowse = new System.Windows.Forms.Button();
		this.tableLayoutPanelTrace = new System.Windows.Forms.TableLayoutPanel();
		System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip(this.components);
		System.Windows.Forms.Label label = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label4 = new System.Windows.Forms.Label();
		System.Windows.Forms.Label label5 = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.pictureFileHistory).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxPanels).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudMaxSummaryFileSize).BeginInit();
		this.tableLayoutPanel1.SuspendLayout();
		this.panel1.SuspendLayout();
		this.tableLayoutPanel2.SuspendLayout();
		this.tableLayoutPanel3.SuspendLayout();
		this.flowLayoutPanel1.SuspendLayout();
		this.tableLayoutPanel4.SuspendLayout();
		this.tableLayoutPanelADRReports.SuspendLayout();
		this.tableLayoutPanelTrace.SuspendLayout();
		base.SuspendLayout();
		toolTip.AutoPopDelay = 7500;
		toolTip.InitialDelay = 500;
		toolTip.IsBalloon = true;
		toolTip.ReshowDelay = 100;
		toolTip.ShowAlways = true;
		toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
		toolTip.ToolTipTitle = "Help";
		resources.ApplyResources(this.checkBoxByte, "checkBoxByte");
		this.checkBoxByte.Name = "checkBoxByte";
		toolTip.SetToolTip(this.checkBoxByte, resources.GetString("checkBoxByte.ToolTip"));
		this.checkBoxByte.UseVisualStyleBackColor = true;
		this.checkBoxByte.CheckedChanged += new System.EventHandler(OnCheckByte);
		resources.ApplyResources(this.checkBoxDebug, "checkBoxDebug");
		this.checkBoxDebug.Name = "checkBoxDebug";
		toolTip.SetToolTip(this.checkBoxDebug, resources.GetString("checkBoxDebug.ToolTip"));
		this.checkBoxDebug.UseVisualStyleBackColor = true;
		this.checkBoxDebug.CheckedChanged += new System.EventHandler(OnCheckDebug);
		resources.ApplyResources(this.checkBoxException, "checkBoxException");
		this.checkBoxException.Name = "checkBoxException";
		toolTip.SetToolTip(this.checkBoxException, resources.GetString("checkBoxException.ToolTip"));
		this.checkBoxException.UseVisualStyleBackColor = true;
		this.checkBoxException.CheckedChanged += new System.EventHandler(OnCheckException);
		resources.ApplyResources(label, "labelSummaryLogFileSize");
		label.Name = "labelSummaryLogFileSize";
		toolTip.SetToolTip(label, resources.GetString("labelSummaryLogFileSize.ToolTip"));
		resources.ApplyResources(label2, "label1");
		label2.Name = "label1";
		toolTip.SetToolTip(label2, resources.GetString("label1.ToolTip"));
		resources.ApplyResources(label3, "labelLogExpirationAge");
		label3.Name = "labelLogExpirationAge";
		resources.ApplyResources(label4, "labelTraceExpirationAge");
		label4.Name = "labelTraceExpirationAge";
		resources.ApplyResources(label5, "labelMegaBytes");
		label5.Name = "labelMegaBytes";
		resources.ApplyResources(this.useNameValuePairsToParameterize, "useNameValuePairsToParameterize");
		this.useNameValuePairsToParameterize.Name = "useNameValuePairsToParameterize";
		this.useNameValuePairsToParameterize.UseVisualStyleBackColor = true;
		this.useNameValuePairsToParameterize.CheckedChanged += new System.EventHandler(useNameValuePairsToParameterize_CheckedChanged);
		resources.ApplyResources(this.checkBoxIgnoreLastServicedCheck, "checkBoxIgnoreLastServicedCheck");
		this.checkBoxIgnoreLastServicedCheck.Name = "checkBoxIgnoreLastServicedCheck";
		this.checkBoxIgnoreLastServicedCheck.UseVisualStyleBackColor = true;
		this.checkBoxIgnoreLastServicedCheck.CheckedChanged += new System.EventHandler(checkBoxIgnoreLastServicedCheck_CheckedChanged);
		resources.ApplyResources(this.pictureFileHistory, "pictureFileHistory");
		this.pictureFileHistory.Name = "pictureFileHistory";
		this.pictureFileHistory.TabStop = false;
		resources.ApplyResources(this.labelFileManagement, "labelFileManagement");
		this.labelFileManagement.Name = "labelFileManagement";
		resources.ApplyResources(this.comboBoxLogExpirationAge, "comboBoxLogExpirationAge");
		this.comboBoxLogExpirationAge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxLogExpirationAge.FormattingEnabled = true;
		this.comboBoxLogExpirationAge.Name = "comboBoxLogExpirationAge";
		this.comboBoxLogExpirationAge.SelectionChangeCommitted += new System.EventHandler(OnLogExpirationCommitted);
		resources.ApplyResources(this.comboBoxTraceExpirationAge, "comboBoxTraceExpirationAge");
		this.comboBoxTraceExpirationAge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxTraceExpirationAge.FormattingEnabled = true;
		this.comboBoxTraceExpirationAge.Name = "comboBoxTraceExpirationAge";
		this.comboBoxTraceExpirationAge.SelectionChangeCommitted += new System.EventHandler(OnTraceExpirationCommitted);
		resources.ApplyResources(this.labelPanels, "labelPanels");
		this.labelPanels.Name = "labelPanels";
		resources.ApplyResources(this.pictureBoxPanels, "pictureBoxPanels");
		this.pictureBoxPanels.Name = "pictureBoxPanels";
		this.pictureBoxPanels.TabStop = false;
		resources.ApplyResources(this.checkBoxAlwaysLoad, "checkBoxAlwaysLoad");
		this.checkBoxAlwaysLoad.Name = "checkBoxAlwaysLoad";
		this.checkBoxAlwaysLoad.UseVisualStyleBackColor = true;
		this.checkBoxAlwaysLoad.CheckedChanged += new System.EventHandler(OnCheckAlwaysLoadPanels);
		resources.ApplyResources(this.nudMaxSummaryFileSize, "nudMaxSummaryFileSize");
		this.nudMaxSummaryFileSize.Maximum = new decimal(new int[4] { 1024, 0, 0, 0 });
		this.nudMaxSummaryFileSize.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.nudMaxSummaryFileSize.Name = "nudMaxSummaryFileSize";
		this.nudMaxSummaryFileSize.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		this.nudMaxSummaryFileSize.ValueChanged += new System.EventHandler(nudMaxSummaryFileSize_ValueChanged);
		resources.ApplyResources(this.comboBoxSummaryFileSizeAction, "comboBoxSummaryFileSizeAction");
		this.comboBoxSummaryFileSizeAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxSummaryFileSizeAction.FormattingEnabled = true;
		this.comboBoxSummaryFileSizeAction.Name = "comboBoxSummaryFileSizeAction";
		this.comboBoxSummaryFileSizeAction.SelectionChangeCommitted += new System.EventHandler(comboBoxSummaryFileSizeAction_SelectionChangeCommitted);
		resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
		this.tableLayoutPanel1.Controls.Add(this.comboBoxSummaryFileSizeAction, 3, 3);
		this.tableLayoutPanel1.Controls.Add(label3, 0, 2);
		this.tableLayoutPanel1.Controls.Add(label2, 2, 3);
		this.tableLayoutPanel1.Controls.Add(this.comboBoxLogExpirationAge, 1, 2);
		this.tableLayoutPanel1.Controls.Add(label4, 0, 3);
		this.tableLayoutPanel1.Controls.Add(this.comboBoxTraceExpirationAge, 1, 3);
		this.tableLayoutPanel1.Controls.Add(label, 2, 2);
		this.tableLayoutPanel1.Controls.Add(this.panel1, 3, 2);
		this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 5);
		this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 6);
		this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 7);
		this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelADRReports, 0, 4);
		this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelTrace, 0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		resources.ApplyResources(this.panel1, "panel1");
		this.panel1.Controls.Add(this.nudMaxSummaryFileSize);
		this.panel1.Controls.Add(label5);
		this.panel1.Name = "panel1";
		resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
		this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 4);
		this.tableLayoutPanel2.Controls.Add(this.pictureFileHistory, 0, 0);
		this.tableLayoutPanel2.Controls.Add(this.labelFileManagement, 1, 0);
		this.tableLayoutPanel2.Name = "tableLayoutPanel2";
		resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
		this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel3, 4);
		this.tableLayoutPanel3.Controls.Add(this.pictureBoxPanels, 0, 0);
		this.tableLayoutPanel3.Controls.Add(this.labelPanels, 1, 0);
		this.tableLayoutPanel3.Name = "tableLayoutPanel3";
		resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
		this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 4);
		this.flowLayoutPanel1.Controls.Add(this.checkBoxAlwaysLoad);
		this.flowLayoutPanel1.Controls.Add(this.useNameValuePairsToParameterize);
		this.flowLayoutPanel1.Controls.Add(this.checkBoxIgnoreLastServicedCheck);
		this.flowLayoutPanel1.Controls.Add(this.checkBoxAllowChecEdit);
		this.flowLayoutPanel1.Name = "flowLayoutPanel1";
		resources.ApplyResources(this.checkBoxAllowChecEdit, "checkBoxAllowChecEdit");
		this.checkBoxAllowChecEdit.Name = "checkBoxAllowChecEdit";
		this.checkBoxAllowChecEdit.UseVisualStyleBackColor = true;
		this.checkBoxAllowChecEdit.CheckedChanged += new System.EventHandler(checkBoxAllowChecEdit_CheckedChanged);
		resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
		this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel4, 4);
		this.tableLayoutPanel4.Controls.Add(this.comboBoxLevel, 1, 0);
		this.tableLayoutPanel4.Controls.Add(this.labelFilterLists, 0, 0);
		this.tableLayoutPanel4.Name = "tableLayoutPanel4";
		this.comboBoxLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxLevel.FormattingEnabled = true;
		resources.ApplyResources(this.comboBoxLevel, "comboBoxLevel");
		this.comboBoxLevel.Name = "comboBoxLevel";
		this.comboBoxLevel.SelectedIndexChanged += new System.EventHandler(OnSelectedLevelChanged);
		resources.ApplyResources(this.labelFilterLists, "labelFilterLists");
		this.labelFilterLists.Name = "labelFilterLists";
		resources.ApplyResources(this.tableLayoutPanelADRReports, "tableLayoutPanelADRReports");
		this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelADRReports, 4);
		this.tableLayoutPanelADRReports.Controls.Add(this.checkBoxAdrAdditionalSaveLocation, 0, 0);
		this.tableLayoutPanelADRReports.Controls.Add(this.labelAdrAdditionalSaveLocation, 1, 0);
		this.tableLayoutPanelADRReports.Controls.Add(this.buttonBrowse, 2, 0);
		this.tableLayoutPanelADRReports.Name = "tableLayoutPanelADRReports";
		resources.ApplyResources(this.checkBoxAdrAdditionalSaveLocation, "checkBoxAdrAdditionalSaveLocation");
		this.checkBoxAdrAdditionalSaveLocation.Name = "checkBoxAdrAdditionalSaveLocation";
		this.checkBoxAdrAdditionalSaveLocation.UseVisualStyleBackColor = true;
		this.checkBoxAdrAdditionalSaveLocation.Click += new System.EventHandler(checkBoxAdrAdditionalSaveLocation_Click);
		this.labelAdrAdditionalSaveLocation.AutoEllipsis = true;
		this.labelAdrAdditionalSaveLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		resources.ApplyResources(this.labelAdrAdditionalSaveLocation, "labelAdrAdditionalSaveLocation");
		this.labelAdrAdditionalSaveLocation.Name = "labelAdrAdditionalSaveLocation";
		this.labelAdrAdditionalSaveLocation.UseMnemonic = false;
		resources.ApplyResources(this.buttonBrowse, "buttonBrowse");
		this.buttonBrowse.Name = "buttonBrowse";
		this.buttonBrowse.UseVisualStyleBackColor = true;
		this.buttonBrowse.Click += new System.EventHandler(buttonBrowse_Click);
		resources.ApplyResources(this.tableLayoutPanelTrace, "tableLayoutPanelTrace");
		this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelTrace, 4);
		this.tableLayoutPanelTrace.Controls.Add(this.checkBoxByte, 0, 0);
		this.tableLayoutPanelTrace.Controls.Add(this.checkBoxException, 2, 0);
		this.tableLayoutPanelTrace.Controls.Add(this.checkBoxDebug, 1, 0);
		this.tableLayoutPanelTrace.Name = "tableLayoutPanelTrace";
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "SupportOptionsPanel";
		base.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
		((System.ComponentModel.ISupportInitialize)this.pictureFileHistory).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxPanels).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudMaxSummaryFileSize).EndInit();
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
		base.ResumeLayout(false);
	}
}
