using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class ConnectionDialog : Form
{
	private class DisplayedConnectionResource
	{
		private bool includeProtocol;

		private ConnectionResource connectionResource;

		public ConnectionResource ConnectionResource => connectionResource;

		public DisplayedConnectionResource(ConnectionResource resource, bool includeProtocol)
		{
			connectionResource = resource;
			this.includeProtocol = includeProtocol;
		}

		public override string ToString()
		{
			return SapiExtensions.ToDisplayString(ConnectionResource, includeProtocol);
		}
	}

	private class DisplayedFamily
	{
		public ElectronicsFamily Family { get; private set; }

		public DisplayedFamily(ElectronicsFamily family)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Family = family;
		}

		public override string ToString()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = Family;
			return ((ElectronicsFamily)(ref family)).Name;
		}
	}

	private IContainer components;

	private Label labelConnection;

	private TableLayoutPanel tableLayoutPanelMain;

	private Button buttonConnect;

	private TableLayoutPanel tableLayoutPanelFamily;

	private ComboBox comboBoxVehicle;

	private ComboBox comboBoxConnectionResource;

	private ListViewEx listViewExEcus;

	private ColumnHeader columnHeaderIdentifier;

	private ColumnHeader columnHeaderName;

	private ColumnHeader columnHeaderDescription;

	private ColumnHeader columnHeaderCategory;

	private RadioButton radioButtonPowertrain;

	private RadioButton radioButtonVehicle;

	private CheckBox checkBoxShowAll;

	private PictureBox pictureBoxLogo;

	private Button buttonCommunicationParameters;

	private Button buttonChangeDiagnosticDescription;

	private TextBox textBoxDiagnosticDescription;

	private Button buttonCancel;

	private CheckBox checkBoxExecuteStartComm;

	private Label labelTargetVariant;

	private ComboBox comboBoxTargetVariant;

	private Label labelDiagnosticDescription;

	private CheckBox checkBoxAdvanced;

	private CheckBox checkBoxCyclicRead;

	private TableLayoutPanel tableLayoutPanelAdvanced;

	private FlowLayoutPanel flowLayoutPanelVariant;

	private CheckBox checkBoxAutoExecConfiguredServices;

	private SapiManager sapi;

	private Ecu selectedEcu;

	private string selectedCategory;

	private ListViewColumnSorter lvwColumnSorter;

	internal static ChannelOptions ChannelOptionsCyclicServices => (ChannelOptions)12;

	internal static ChannelOptions ChannelOptionsAutoExecuteConfiguredServices => (ChannelOptions)240;

	protected override void Dispose(bool disposing)
	{
		sapi.ActiveChannelsListChanged -= OnSapiActiveChannelsListChanged;
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.ConnectionDialog));
		this.labelConnection = new System.Windows.Forms.Label();
		this.tableLayoutPanelFamily = new System.Windows.Forms.TableLayoutPanel();
		this.radioButtonPowertrain = new System.Windows.Forms.RadioButton();
		this.comboBoxVehicle = new System.Windows.Forms.ComboBox();
		this.radioButtonVehicle = new System.Windows.Forms.RadioButton();
		this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
		this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
		this.comboBoxConnectionResource = new System.Windows.Forms.ComboBox();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonConnect = new System.Windows.Forms.Button();
		this.listViewExEcus = new ListViewEx();
		this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderDescription = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderCategory = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderIdentifier = new System.Windows.Forms.ColumnHeader();
		this.checkBoxShowAll = new System.Windows.Forms.CheckBox();
		this.buttonCommunicationParameters = new System.Windows.Forms.Button();
		this.labelDiagnosticDescription = new System.Windows.Forms.Label();
		this.buttonChangeDiagnosticDescription = new System.Windows.Forms.Button();
		this.textBoxDiagnosticDescription = new System.Windows.Forms.TextBox();
		this.checkBoxAdvanced = new System.Windows.Forms.CheckBox();
		this.tableLayoutPanelAdvanced = new System.Windows.Forms.TableLayoutPanel();
		this.checkBoxExecuteStartComm = new System.Windows.Forms.CheckBox();
		this.checkBoxAutoExecConfiguredServices = new System.Windows.Forms.CheckBox();
		this.checkBoxCyclicRead = new System.Windows.Forms.CheckBox();
		this.flowLayoutPanelVariant = new System.Windows.Forms.FlowLayoutPanel();
		this.labelTargetVariant = new System.Windows.Forms.Label();
		this.comboBoxTargetVariant = new System.Windows.Forms.ComboBox();
		this.tableLayoutPanelFamily.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxLogo).BeginInit();
		this.tableLayoutPanelMain.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.listViewExEcus).BeginInit();
		this.tableLayoutPanelAdvanced.SuspendLayout();
		this.flowLayoutPanelVariant.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.labelConnection, "labelConnection");
		this.labelConnection.Name = "labelConnection";
		resources.ApplyResources(this.tableLayoutPanelFamily, "tableLayoutPanelFamily");
		this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelFamily, 10);
		this.tableLayoutPanelFamily.Controls.Add(this.radioButtonPowertrain, 0, 1);
		this.tableLayoutPanelFamily.Controls.Add(this.comboBoxVehicle, 1, 0);
		this.tableLayoutPanelFamily.Controls.Add(this.radioButtonVehicle, 0, 0);
		this.tableLayoutPanelFamily.Controls.Add(this.pictureBoxLogo, 4, 0);
		this.tableLayoutPanelFamily.Name = "tableLayoutPanelFamily";
		resources.ApplyResources(this.radioButtonPowertrain, "radioButtonPowertrain");
		this.radioButtonPowertrain.Name = "radioButtonPowertrain";
		this.radioButtonPowertrain.TabStop = true;
		this.radioButtonPowertrain.UseVisualStyleBackColor = true;
		this.radioButtonPowertrain.CheckedChanged += new System.EventHandler(radioButtonPowertrain_CheckedChanged);
		this.comboBoxVehicle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxVehicle.FormattingEnabled = true;
		resources.ApplyResources(this.comboBoxVehicle, "comboBoxVehicle");
		this.comboBoxVehicle.Name = "comboBoxVehicle";
		this.comboBoxVehicle.SelectedIndexChanged += new System.EventHandler(comboBoxVehicle_SelectedIndexChanged);
		resources.ApplyResources(this.radioButtonVehicle, "radioButtonVehicle");
		this.radioButtonVehicle.Name = "radioButtonVehicle";
		this.radioButtonVehicle.TabStop = true;
		this.radioButtonVehicle.UseVisualStyleBackColor = true;
		this.radioButtonVehicle.CheckedChanged += new System.EventHandler(radioButtonVehicle_CheckedChanged);
		resources.ApplyResources(this.pictureBoxLogo, "pictureBoxLogo");
		this.pictureBoxLogo.Name = "pictureBoxLogo";
		this.tableLayoutPanelFamily.SetRowSpan(this.pictureBoxLogo, 2);
		this.pictureBoxLogo.TabStop = false;
		resources.ApplyResources(this.tableLayoutPanelMain, "tableLayoutPanelMain");
		this.tableLayoutPanelMain.Controls.Add(this.comboBoxConnectionResource, 1, 2);
		this.tableLayoutPanelMain.Controls.Add(this.labelConnection, 0, 2);
		this.tableLayoutPanelMain.Controls.Add(this.buttonCancel, 8, 4);
		this.tableLayoutPanelMain.Controls.Add(this.buttonConnect, 7, 4);
		this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelFamily, 0, 0);
		this.tableLayoutPanelMain.Controls.Add((System.Windows.Forms.Control)(object)this.listViewExEcus, 0, 1);
		this.tableLayoutPanelMain.Controls.Add(this.checkBoxShowAll, 8, 2);
		this.tableLayoutPanelMain.Controls.Add(this.buttonCommunicationParameters, 3, 4);
		this.tableLayoutPanelMain.Controls.Add(this.labelDiagnosticDescription, 0, 3);
		this.tableLayoutPanelMain.Controls.Add(this.buttonChangeDiagnosticDescription, 9, 3);
		this.tableLayoutPanelMain.Controls.Add(this.textBoxDiagnosticDescription, 3, 3);
		this.tableLayoutPanelMain.Controls.Add(this.checkBoxAdvanced, 0, 4);
		this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelAdvanced, 0, 6);
		this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
		this.tableLayoutPanelMain.SetColumnSpan(this.comboBoxConnectionResource, 7);
		resources.ApplyResources(this.comboBoxConnectionResource, "comboBoxConnectionResource");
		this.comboBoxConnectionResource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxConnectionResource.FormattingEnabled = true;
		this.comboBoxConnectionResource.Name = "comboBoxConnectionResource";
		this.comboBoxConnectionResource.SelectedIndexChanged += new System.EventHandler(comboBoxConnectionResource_SelectedIndexChanged);
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.tableLayoutPanelMain.SetColumnSpan(this.buttonCancel, 2);
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		resources.ApplyResources(this.buttonConnect, "buttonConnect");
		this.buttonConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonConnect.Name = "buttonConnect";
		this.buttonConnect.Click += new System.EventHandler(OnConnectButtonClick);
		this.listViewExEcus.CanDelete = false;
		((System.Windows.Forms.ListView)(object)this.listViewExEcus).Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] { this.columnHeaderName, this.columnHeaderDescription, this.columnHeaderCategory, this.columnHeaderIdentifier });
		this.tableLayoutPanelMain.SetColumnSpan((System.Windows.Forms.Control)(object)this.listViewExEcus, 10);
		resources.ApplyResources(this.listViewExEcus, "listViewExEcus");
		this.listViewExEcus.EditableColumn = -1;
		this.listViewExEcus.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
		((System.Windows.Forms.Control)(object)this.listViewExEcus).Name = "listViewExEcus";
		this.listViewExEcus.ShowGlyphs = (GlyphBehavior)1;
		((System.Windows.Forms.ListView)(object)this.listViewExEcus).ShowGroups = false;
		this.listViewExEcus.ShowItemImages = (ImageBehavior)1;
		this.listViewExEcus.ShowStateImages = (ImageBehavior)1;
		((System.Windows.Forms.ListView)(object)this.listViewExEcus).UseCompatibleStateImageBehavior = false;
		((System.Windows.Forms.ListView)(object)this.listViewExEcus).ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(listViewExEcus_ColumnClick);
		((System.Windows.Forms.ListView)(object)this.listViewExEcus).SelectedIndexChanged += new System.EventHandler(listViewExEcus_SelectedIndexChanged);
		((System.Windows.Forms.Control)(object)this.listViewExEcus).DoubleClick += new System.EventHandler(listViewExEcus_DoubleClick_1);
		((System.Windows.Forms.Control)(object)this.listViewExEcus).KeyPress += new System.Windows.Forms.KeyPressEventHandler(listViewExEcus_KeyPress);
		resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
		resources.ApplyResources(this.columnHeaderDescription, "columnHeaderDescription");
		resources.ApplyResources(this.columnHeaderCategory, "columnHeaderCategory");
		resources.ApplyResources(this.columnHeaderIdentifier, "columnHeaderIdentifier");
		resources.ApplyResources(this.checkBoxShowAll, "checkBoxShowAll");
		this.tableLayoutPanelMain.SetColumnSpan(this.checkBoxShowAll, 2);
		this.checkBoxShowAll.Name = "checkBoxShowAll";
		this.checkBoxShowAll.UseVisualStyleBackColor = true;
		this.checkBoxShowAll.CheckedChanged += new System.EventHandler(checkBoxShowAll_CheckedChanged_1);
		this.tableLayoutPanelMain.SetColumnSpan(this.buttonCommunicationParameters, 4);
		resources.ApplyResources(this.buttonCommunicationParameters, "buttonCommunicationParameters");
		this.buttonCommunicationParameters.Name = "buttonCommunicationParameters";
		this.buttonCommunicationParameters.UseVisualStyleBackColor = true;
		this.buttonCommunicationParameters.Click += new System.EventHandler(buttonCommunicationParameters_Click);
		resources.ApplyResources(this.labelDiagnosticDescription, "labelDiagnosticDescription");
		this.tableLayoutPanelMain.SetColumnSpan(this.labelDiagnosticDescription, 3);
		this.labelDiagnosticDescription.Name = "labelDiagnosticDescription";
		resources.ApplyResources(this.buttonChangeDiagnosticDescription, "buttonChangeDiagnosticDescription");
		this.buttonChangeDiagnosticDescription.Name = "buttonChangeDiagnosticDescription";
		this.buttonChangeDiagnosticDescription.Click += new System.EventHandler(buttonChangeDiagnosticDescription_Click);
		this.tableLayoutPanelMain.SetColumnSpan(this.textBoxDiagnosticDescription, 6);
		resources.ApplyResources(this.textBoxDiagnosticDescription, "textBoxDiagnosticDescription");
		this.textBoxDiagnosticDescription.Name = "textBoxDiagnosticDescription";
		resources.ApplyResources(this.checkBoxAdvanced, "checkBoxAdvanced");
		this.tableLayoutPanelMain.SetColumnSpan(this.checkBoxAdvanced, 3);
		this.checkBoxAdvanced.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.navigate_down;
		this.checkBoxAdvanced.Name = "checkBoxAdvanced";
		this.checkBoxAdvanced.UseVisualStyleBackColor = true;
		this.checkBoxAdvanced.CheckedChanged += new System.EventHandler(checkBoxAdvanced_CheckedChanged);
		resources.ApplyResources(this.tableLayoutPanelAdvanced, "tableLayoutPanelAdvanced");
		this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelAdvanced, 10);
		this.tableLayoutPanelAdvanced.Controls.Add(this.checkBoxExecuteStartComm, 0, 1);
		this.tableLayoutPanelAdvanced.Controls.Add(this.checkBoxAutoExecConfiguredServices, 0, 2);
		this.tableLayoutPanelAdvanced.Controls.Add(this.checkBoxCyclicRead, 1, 1);
		this.tableLayoutPanelAdvanced.Controls.Add(this.flowLayoutPanelVariant, 0, 0);
		this.tableLayoutPanelAdvanced.Name = "tableLayoutPanelAdvanced";
		resources.ApplyResources(this.checkBoxExecuteStartComm, "checkBoxExecuteStartComm");
		this.checkBoxExecuteStartComm.Name = "checkBoxExecuteStartComm";
		this.checkBoxExecuteStartComm.UseVisualStyleBackColor = true;
		this.checkBoxExecuteStartComm.CheckedChanged += new System.EventHandler(checkBoxExecuteStartComm_CheckedChanged);
		resources.ApplyResources(this.checkBoxAutoExecConfiguredServices, "checkBoxAutoExecConfiguredServices");
		this.tableLayoutPanelAdvanced.SetColumnSpan(this.checkBoxAutoExecConfiguredServices, 2);
		this.checkBoxAutoExecConfiguredServices.Name = "checkBoxAutoExecConfiguredServices";
		this.checkBoxAutoExecConfiguredServices.UseVisualStyleBackColor = true;
		this.checkBoxAutoExecConfiguredServices.CheckedChanged += new System.EventHandler(checkBoxAutoExecConfiguredServices_CheckedChanged);
		resources.ApplyResources(this.checkBoxCyclicRead, "checkBoxCyclicRead");
		this.checkBoxCyclicRead.Name = "checkBoxCyclicRead";
		this.checkBoxCyclicRead.UseVisualStyleBackColor = true;
		this.checkBoxCyclicRead.CheckedChanged += new System.EventHandler(checkBoxCyclicRead_CheckedChanged);
		resources.ApplyResources(this.flowLayoutPanelVariant, "flowLayoutPanelVariant");
		this.tableLayoutPanelAdvanced.SetColumnSpan(this.flowLayoutPanelVariant, 2);
		this.flowLayoutPanelVariant.Controls.Add(this.labelTargetVariant);
		this.flowLayoutPanelVariant.Controls.Add(this.comboBoxTargetVariant);
		this.flowLayoutPanelVariant.Name = "flowLayoutPanelVariant";
		resources.ApplyResources(this.labelTargetVariant, "labelTargetVariant");
		this.labelTargetVariant.Name = "labelTargetVariant";
		this.comboBoxTargetVariant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxTargetVariant.FormattingEnabled = true;
		resources.ApplyResources(this.comboBoxTargetVariant, "comboBoxTargetVariant");
		this.comboBoxTargetVariant.Name = "comboBoxTargetVariant";
		this.comboBoxTargetVariant.SelectedIndexChanged += new System.EventHandler(comboBoxTargetVariant_SelectedIndexChanged);
		base.AcceptButton = this.buttonConnect;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(this.tableLayoutPanelMain);
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ConnectionDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(OnHelpButtonClicked);
		this.tableLayoutPanelFamily.ResumeLayout(false);
		this.tableLayoutPanelFamily.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxLogo).EndInit();
		this.tableLayoutPanelMain.ResumeLayout(false);
		this.tableLayoutPanelMain.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.listViewExEcus).EndInit();
		this.tableLayoutPanelAdvanced.ResumeLayout(false);
		this.tableLayoutPanelAdvanced.PerformLayout();
		this.flowLayoutPanelVariant.ResumeLayout(false);
		this.flowLayoutPanelVariant.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public ConnectionDialog()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		lvwColumnSorter = new ListViewColumnSorter();
		lvwColumnSorter.ColumnToSort = columnHeaderIdentifier.Index;
		lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
		lvwColumnSorter.SortType = ListViewColumnSorter.SortBy.Tag;
		((ListView)(object)listViewExEcus).ListViewItemSorter = lvwColumnSorter;
		sapi = SapiManager.GlobalInstance;
		sapi.ActiveChannelsListChanged += OnSapiActiveChannelsListChanged;
		pictureBoxLogo.Image = ApplicationInformation.Branding.Logo;
		tableLayoutPanelFamily.BackColor = (tableLayoutPanelMain.BackColor = (BackColor = ApplicationInformation.Branding.LogoBackColor));
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		base.OnLoad(e);
		StringSetting value = SettingsManager.GlobalInstance.GetValue<StringSetting>("Family", "ConnectionDialog", new StringSetting("Vehicle/All"));
		string[] array = value.Value.Split("/".ToCharArray());
		string text = array[0];
		if (!(text == "Vehicle"))
		{
			if (text == "Engine")
			{
				radioButtonPowertrain.Checked = true;
			}
		}
		else
		{
			radioButtonVehicle.Checked = true;
		}
		PopulateFamilyCombo(array[1]);
		BuildList();
		((Control)(object)listViewExEcus).Select();
		checkBoxShowAll.Checked = SettingsManager.GlobalInstance.GetValue<bool>("ShowAllResources", "ConnectionDialog", checkBoxShowAll.Checked);
		int num = ((Control)(object)listViewExEcus).ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
		columnHeaderName.Width = num * 4 / 20;
		columnHeaderDescription.Width = num * 10 / 20;
		columnHeaderCategory.Width = num * 3 / 20;
		columnHeaderIdentifier.Width = num * 3 / 20;
	}

	private void OnSapiActiveChannelsListChanged(object sender, EventArgs e)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ListViewExGroupItem item = ((((ListView)(object)listViewExEcus).SelectedItems.Count <= 0) ? ((ListViewExGroupItem)null) : ((ListViewExGroupItem)((ListView)(object)listViewExEcus).SelectedItems[0]));
		bool enabled = buttonConnect.Enabled;
		buttonConnect.Enabled = false;
		BuildList();
		if (((ListView)(object)listViewExEcus).SelectedItems.Contains((ListViewItem)(object)item))
		{
			buttonConnect.Enabled = enabled;
		}
	}

	private void BuildList()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		listViewExEcus.BeginUpdate();
		listViewExEcus.LockSorting();
		ListViewExGroupItem val = ((((ListView)(object)listViewExEcus).SelectedItems.Count <= 0) ? ((ListViewExGroupItem)null) : ((ListViewExGroupItem)((ListView)(object)listViewExEcus).SelectedItems[0]));
		bool flag = ((ListViewItem)(object)val)?.Bounds.IntersectsWith(((Control)(object)listViewExEcus).ClientRectangle) ?? false;
		bool showIfNoCategoryMatch = radioButtonVehicle.Checked;
		DisplayedFamily requiredFamily = comboBoxVehicle.SelectedItem as DisplayedFamily;
		List<Ecu> list = (from ecu in CollectionExtensions.DistinctBy<Ecu, string>(((IEnumerable<Ecu>)sapi.Sapi.Ecus).Where((Ecu ecu) => !ecu.IsRollCall && !ecu.OfflineSupportOnly && sapi.Sapi.Ecus.GetConnectedCountForIdentifier(ecu.Identifier) == 0 && IsAllowedForSelectedCategoryFamily(selectedCategory, requiredFamily, ecu, showIfNoCategoryMatch)), (Func<Ecu, string>)((Ecu ecu) => ecu.Name))
			orderby ecu.Priority
			select ecu).ToList();
		foreach (ListViewExGroupItem item2 in ((ListView)(object)listViewExEcus).Items.OfType<ListViewExGroupItem>().ToList())
		{
			object tag = ((ListViewItem)(object)item2).Tag;
			Ecu item = (Ecu)((tag is Ecu) ? tag : null);
			if (!list.Contains(item))
			{
				((ListView)(object)listViewExEcus).Items.Remove((ListViewItem)(object)item2);
			}
			else
			{
				list.Remove(item);
			}
		}
		foreach (Ecu item3 in list)
		{
			ListViewExGroupItem val2 = new ListViewExGroupItem(item3.Name);
			((ListViewItem)(object)val2).SubItems.Add(item3.ShortDescription);
			((ListViewItem)(object)val2).SubItems.Add(SapiExtensions.EquipmentCategory(item3));
			ListViewItem.ListViewSubItem listViewSubItem = ((ListViewItem)(object)val2).SubItems.Add(item3.Identifier);
			listViewSubItem.Tag = item3.Priority;
			((ListViewItem)(object)val2).Tag = item3;
			((ListView)(object)listViewExEcus).Items.Add((ListViewItem)(object)val2);
		}
		if (((ListView)(object)listViewExEcus).SelectedItems.Count == 0 && ((ListView)(object)listViewExEcus).Items.Count > 0)
		{
			((ListView)(object)listViewExEcus).Items[0].Selected = true;
		}
		listViewExEcus.UnlockSorting();
		listViewExEcus.EndUpdate();
		if (flag && ((ListView)(object)listViewExEcus).SelectedItems.Contains((ListViewItem)(object)val))
		{
			((ListView)(object)listViewExEcus).EnsureVisible(((ListViewItem)(object)val).Index);
		}
	}

	private void PopulateFamilyCombo(string selectedFamily = null)
	{
		comboBoxVehicle.Items.Clear();
		comboBoxVehicle.Items.Add(Resources.ConnectionDialog_All);
		comboBoxVehicle.Items.AddRange((from ef in ElectronicsFamily.AvailableFamilies()
			where ((ElectronicsFamily)(ref ef)).Category == selectedCategory
			select new DisplayedFamily(ef)).ToArray());
		if (selectedFamily != null)
		{
			comboBoxVehicle.SelectedIndex = comboBoxVehicle.FindStringExact(selectedFamily, -1);
		}
		if (comboBoxVehicle.SelectedItem == null)
		{
			comboBoxVehicle.SelectedItem = Resources.ConnectionDialog_All;
		}
	}

	private static bool IsAllowedForSelectedCategoryFamily(string selectedCategory, DisplayedFamily selectedFamily, Ecu ecu, bool defaultIfNoCategoryMatch)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (SapiExtensions.EquipmentCategory(ecu) == selectedCategory)
		{
			if (selectedFamily != null)
			{
				return ElectronicsFamily.FromDevice(ecu).Contains(selectedFamily.Family);
			}
			return true;
		}
		IEnumerable<string> enumerable = default(IEnumerable<string>);
		if (SapiExtensions.TryGetSubcategoryFamilies(ecu, selectedCategory, ref enumerable))
		{
			if (selectedFamily != null)
			{
				IEnumerable<string> source = enumerable;
				ElectronicsFamily family = selectedFamily.Family;
				return source.Contains(((ElectronicsFamily)(ref family)).Name);
			}
			return true;
		}
		return defaultIfNoCategoryMatch;
	}

	private ChannelOptions GetSelectedChannelOptions()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected I4, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		ChannelOptions val = (ChannelOptions)0;
		if (comboBoxConnectionResource.SelectedItem is DisplayedConnectionResource displayedConnectionResource)
		{
			if (checkBoxExecuteStartComm.Checked)
			{
				val = (ChannelOptions)(val | 1);
			}
			if (checkBoxCyclicRead.Checked)
			{
				val |= ChannelOptionsCyclicServices;
			}
			if (checkBoxAutoExecConfiguredServices.Checked)
			{
				val |= ChannelOptionsAutoExecuteConfiguredServices;
			}
			val &= displayedConnectionResource.ConnectionResource.Ecu.AvailableChannelOptions;
			SettingsManager.GlobalInstance.SetValue<int>("ChannelOptions_" + displayedConnectionResource.ConnectionResource.Ecu.Name + "_" + displayedConnectionResource.ConnectionResource.Ecu.DiagnosisSource, "ConnectionDialog", (int)val, false);
		}
		return val;
	}

	private void OnConnectButtonClick(object sender, EventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		DisplayedConnectionResource displayedConnectionResource = comboBoxConnectionResource.SelectedItem as DisplayedConnectionResource;
		ChannelOptions val = (ChannelOptions)253;
		SettingsManager.GlobalInstance.SetValue<StringSetting>("SelectedResource_" + displayedConnectionResource.ConnectionResource.Ecu.Name, "ConnectionDialog", new StringSetting(((object)displayedConnectionResource.ConnectionResource).ToString()), false);
		sapi.OpenConnection(displayedConnectionResource.ConnectionResource, val);
		buttonConnect.Enabled = false;
		((Control)(object)listViewExEcus).Enabled = false;
		comboBoxConnectionResource.Enabled = false;
		Close();
	}

	private void listViewExEcus_SelectedIndexChanged(object sender, EventArgs e)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ListViewExGroupItem val = ((((ListView)(object)listViewExEcus).SelectedItems.Count <= 0) ? ((ListViewExGroupItem)null) : ((ListViewExGroupItem)((ListView)(object)listViewExEcus).SelectedItems[0]));
		selectedEcu = (Ecu)((val != null) ? /*isinst with value type is only supported in some contexts*/: null);
		textBoxDiagnosticDescription.Text = string.Empty;
		buttonChangeDiagnosticDescription.Enabled = false;
		PopulateConnectionResources();
		PopulateTargetVariant();
		UpdateChannelOptions();
	}

	private void PopulateConnectionResources()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		bool flag = checkBoxShowAll.Checked;
		comboBoxConnectionResource.Items.Clear();
		bool flag2 = false;
		if (selectedEcu != null)
		{
			List<Ecu> list = new List<Ecu>();
			List<ConnectionResource> cs = new List<ConnectionResource>();
			List<ConnectionResource> list2 = new List<ConnectionResource>();
			string text = SettingsManager.GlobalInstance.GetValue<StringSetting>("SelectedResource_" + selectedEcu.Name, "ConnectionDialog", new StringSetting()).Value;
			foreach (Ecu item in ((IEnumerable<Ecu>)sapi.Sapi.Ecus).Where((Ecu e) => e.Name == selectedEcu.Name))
			{
				if (sapi.Sapi.Ecus.GetConnectedCountForIdentifier(item.Identifier) == 0)
				{
					cs.AddRange((IEnumerable<ConnectionResource>)item.GetConnectionResources());
					if (!string.IsNullOrEmpty(text))
					{
						list2.Add(ConnectionResource.FromString(item, text));
					}
				}
			}
			if (!list2.Any((ConnectionResource settingsConnectionResource) => settingsConnectionResource != null && cs.Any((ConnectionResource val2) => val2.HardwareName == settingsConnectionResource.HardwareName)))
			{
				text = null;
			}
			foreach (ConnectionResource cr in cs)
			{
				if (flag || !cr.Restricted)
				{
					int selectedIndex = comboBoxConnectionResource.Items.Add(new DisplayedConnectionResource(cr, flag && cs.Any((ConnectionResource ocr) => SapiExtensions.IsEquivalentExceptProtocol(cr, ocr))));
					if (comboBoxConnectionResource.SelectedIndex != -1)
					{
						continue;
					}
					if (!string.IsNullOrEmpty(text))
					{
						if (text == ((object)cr).ToString())
						{
							comboBoxConnectionResource.SelectedIndex = selectedIndex;
						}
					}
					else if (!cr.Restricted)
					{
						comboBoxConnectionResource.SelectedIndex = selectedIndex;
					}
				}
				else
				{
					flag2 = true;
					list.AddRange(cr.Ecu.ViaEcus);
				}
			}
			if (comboBoxConnectionResource.Items.Count == 0)
			{
				if (flag2 && list.Any())
				{
					Ecu val = list.FirstOrDefault((Ecu ve) => ve.ConnectedChannelCount > 0);
					if (val != null)
					{
						comboBoxConnectionResource.Items.Add(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ResourceCannotBeDeterminedViaEcuPresent, val.Name));
					}
					else
					{
						comboBoxConnectionResource.Items.Add(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ResourceCannotBeDetermined, string.Join("/", list.Distinct())));
					}
				}
				else
				{
					comboBoxConnectionResource.Items.Add(Resources.Messsage_NoConnectionResources);
				}
				comboBoxConnectionResource.SelectedIndex = 0;
			}
		}
		comboBoxConnectionResource.Enabled = comboBoxConnectionResource.Items.OfType<DisplayedConnectionResource>().Any();
		DisplayedConnectionResource displayedConnectionResource = comboBoxConnectionResource.SelectedItem as DisplayedConnectionResource;
		buttonConnect.Enabled = displayedConnectionResource != null;
		textBoxDiagnosticDescription.Text = ((displayedConnectionResource != null) ? (Path.GetFileName(displayedConnectionResource.ConnectionResource.Ecu.DescriptionFileName) + " (" + displayedConnectionResource.ConnectionResource.Ecu.DescriptionDataVersion + ")") : string.Empty);
		buttonChangeDiagnosticDescription.Enabled = displayedConnectionResource != null && string.Equals(Path.GetExtension(displayedConnectionResource.ConnectionResource.Ecu.DescriptionFileName), ".cbf", StringComparison.OrdinalIgnoreCase);
	}

	private void PopulateTargetVariant()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		comboBoxTargetVariant.Items.Clear();
		DisplayedConnectionResource displayedConnectionResource = comboBoxConnectionResource.SelectedItem as DisplayedConnectionResource;
		comboBoxTargetVariant.Enabled = displayedConnectionResource != null;
		if (displayedConnectionResource == null)
		{
			return;
		}
		StringSetting value = SettingsManager.GlobalInstance.GetValue<StringSetting>("FixedVariant_" + displayedConnectionResource.ConnectionResource.Ecu.Name + "_" + displayedConnectionResource.ConnectionResource.Ecu.DiagnosisSource, "ConnectionDialog", new StringSetting(string.Empty));
		string value2 = value.Value;
		int selectedIndex = comboBoxTargetVariant.Items.Add(Resources.ConnectionDialog_UseDetectedVariant);
		foreach (DiagnosisVariant item in (ReadOnlyCollection<DiagnosisVariant>)(object)displayedConnectionResource.ConnectionResource.Ecu.DiagnosisVariants)
		{
			int num = comboBoxTargetVariant.Items.Add(item);
			if (item.Name == value2)
			{
				selectedIndex = num;
			}
		}
		comboBoxTargetVariant.SelectedIndex = selectedIndex;
	}

	private void UpdateChannelOptions()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected I4, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Invalid comparison between Unknown and I4
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Invalid comparison between Unknown and I4
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Invalid comparison between Unknown and I4
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Invalid comparison between Unknown and I4
		DisplayedConnectionResource displayedConnectionResource = comboBoxConnectionResource.SelectedItem as DisplayedConnectionResource;
		ChannelOptions val = (ChannelOptions)((displayedConnectionResource != null) ? ((int)displayedConnectionResource.ConnectionResource.Ecu.AvailableChannelOptions) : 0);
		ChannelOptions val2 = (ChannelOptions)((displayedConnectionResource != null) ? SettingsManager.GlobalInstance.GetValue<int>("ChannelOptions_" + displayedConnectionResource.ConnectionResource.Ecu.Name + "_" + displayedConnectionResource.ConnectionResource.Ecu.DiagnosisSource, "ConnectionDialog", (int)val) : 0);
		checkBoxExecuteStartComm.Enabled = (val & 1) > 0;
		checkBoxCyclicRead.Enabled = (val & ChannelOptionsCyclicServices) > 0;
		checkBoxAutoExecConfiguredServices.Enabled = (val & ChannelOptionsAutoExecuteConfiguredServices) > 0;
		checkBoxExecuteStartComm.Checked = (val2 & 1) > 0;
		checkBoxCyclicRead.Checked = (val2 & ChannelOptionsCyclicServices) > 0;
		checkBoxAutoExecConfiguredServices.Checked = (val2 & ChannelOptionsAutoExecuteConfiguredServices) > 0;
	}

	private static void UpdateDeviceComponentInformation(Ecu ecu)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ecu != null)
		{
			ComponentInformationGroups.UpdateItem(Components.GroupSupportedDevices, ecu.Name + "_" + ecu.DiagnosisSource, ecu.ConfigurationFileVersion.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0} ({1})", ecu.DescriptionDataVersion, ecu.ConfigurationFileVersion) : ecu.DescriptionDataVersion, true);
		}
	}

	private void OnHelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		e.Cancel = true;
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_ConnectionDialog"));
	}

	private void checkBoxShowAll_CheckedChanged_1(object sender, EventArgs e)
	{
		PopulateConnectionResources();
		SettingsManager.GlobalInstance.SetValue<bool>("ShowAllResources", "ConnectionDialog", checkBoxShowAll.Checked, false);
	}

	private void comboBoxVehicle_SelectedIndexChanged(object sender, EventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		BuildList();
		DisplayedFamily displayedFamily = comboBoxVehicle.SelectedItem as DisplayedFamily;
		SettingsManager globalInstance = SettingsManager.GlobalInstance;
		string obj = selectedCategory;
		string obj2;
		if (displayedFamily == null)
		{
			obj2 = Resources.ConnectionDialog_All;
		}
		else
		{
			ElectronicsFamily family = displayedFamily.Family;
			obj2 = ((ElectronicsFamily)(ref family)).Name;
		}
		globalInstance.SetValue<StringSetting>("Family", "ConnectionDialog", new StringSetting(obj + "/" + obj2), false);
	}

	private void radioButtonVehicle_CheckedChanged(object sender, EventArgs e)
	{
		selectedCategory = "Vehicle";
		tableLayoutPanelFamily.SetCellPosition(comboBoxVehicle, new TableLayoutPanelCellPosition(1, 0));
		PopulateFamilyCombo();
	}

	private void radioButtonPowertrain_CheckedChanged(object sender, EventArgs e)
	{
		selectedCategory = "Engine";
		tableLayoutPanelFamily.SetCellPosition(comboBoxVehicle, new TableLayoutPanelCellPosition(1, 1));
		PopulateFamilyCombo();
	}

	private void comboBoxConnectionResource_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayedConnectionResource displayedConnectionResource = comboBoxConnectionResource.SelectedItem as DisplayedConnectionResource;
		buttonConnect.Enabled = displayedConnectionResource != null;
		if (displayedConnectionResource != null)
		{
			textBoxDiagnosticDescription.Text = Path.GetFileName(displayedConnectionResource.ConnectionResource.Ecu.DescriptionFileName) + " (" + displayedConnectionResource.ConnectionResource.Ecu.DescriptionDataVersion + ")";
		}
		buttonChangeDiagnosticDescription.Enabled = displayedConnectionResource != null && string.Equals(Path.GetExtension(displayedConnectionResource.ConnectionResource.Ecu.DescriptionFileName), ".cbf", StringComparison.OrdinalIgnoreCase);
		PopulateTargetVariant();
		UpdateChannelOptions();
	}

	private void listViewExEcus_DoubleClick_1(object sender, EventArgs e)
	{
		if (buttonConnect.Enabled)
		{
			buttonConnect.PerformClick();
		}
	}

	private void listViewExEcus_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\r' || e.KeyChar == '\r')
		{
			e.Handled = true;
			buttonConnect.PerformClick();
		}
	}

	private void listViewExEcus_ColumnClick(object sender, ColumnClickEventArgs e)
	{
		if (e.Column == lvwColumnSorter.ColumnToSort)
		{
			lvwColumnSorter.OrderOfSort = ((lvwColumnSorter.OrderOfSort != SortOrder.Ascending) ? SortOrder.Ascending : SortOrder.Descending);
		}
		else
		{
			lvwColumnSorter.ColumnToSort = e.Column;
			lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
		}
		lvwColumnSorter.SortType = ((e.Column == columnHeaderIdentifier.Index) ? ListViewColumnSorter.SortBy.Tag : ListViewColumnSorter.SortBy.Text);
		((ListView)(object)listViewExEcus).Sort();
	}

	private void buttonCommunicationParameters_Click(object sender, EventArgs e)
	{
		object obj = selectedEcu;
		DisplayedConnectionResource displayedConnectionResource = comboBoxConnectionResource.SelectedItem as DisplayedConnectionResource;
		if (displayedConnectionResource != null)
		{
			obj = displayedConnectionResource.ConnectionResource.Interface;
		}
		ActionsMenuProxy.GlobalInstance.ShowDialog("Communication Parameters", (string)null, obj, false);
		PopulateConnectionResources();
		if (displayedConnectionResource != null)
		{
			comboBoxConnectionResource.SelectedItem = comboBoxConnectionResource.Items.OfType<DisplayedConnectionResource>().FirstOrDefault((DisplayedConnectionResource dcr) => ((object)dcr.ConnectionResource).ToString() == ((object)displayedConnectionResource.ConnectionResource).ToString());
		}
	}

	private void buttonChangeDiagnosticDescription_Click(object sender, EventArgs e)
	{
	}

	private void checkBoxAdvanced_CheckedChanged(object sender, EventArgs e)
	{
		Size preferredSize = tableLayoutPanelAdvanced.GetPreferredSize(new Size(base.Size.Width, tableLayoutPanelAdvanced.Height));
		if (checkBoxAdvanced.Checked)
		{
			tableLayoutPanelMain.SuspendLayout();
			base.Size = new Size(base.Size.Width, base.Size.Height + preferredSize.Height);
			BeginInvoke((Action)delegate
			{
				tableLayoutPanelAdvanced.Visible = true;
				tableLayoutPanelMain.ResumeLayout();
			});
		}
		else
		{
			tableLayoutPanelMain.SuspendLayout();
			base.Size = new Size(base.Size.Width, base.Size.Height - preferredSize.Height);
			tableLayoutPanelAdvanced.Visible = false;
			tableLayoutPanelMain.ResumeLayout();
		}
		checkBoxAdvanced.Image = (checkBoxAdvanced.Checked ? Resources.navigate_up : Resources.navigate_down);
	}

	private void checkBoxCyclicRead_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GetSelectedChannelOptions();
	}

	private void checkBoxAutoExecConfiguredServices_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GetSelectedChannelOptions();
	}

	private void checkBoxExecuteStartComm_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GetSelectedChannelOptions();
	}

	private void comboBoxTargetVariant_SelectedIndexChanged(object sender, EventArgs e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		string text = string.Empty;
		if (comboBoxTargetVariant.SelectedItem is DiagnosisVariant)
		{
			text = ((DiagnosisVariant)comboBoxTargetVariant.SelectedItem).Name;
		}
		if (comboBoxConnectionResource.SelectedItem is DisplayedConnectionResource displayedConnectionResource)
		{
			SettingsManager.GlobalInstance.SetValue<StringSetting>("FixedVariant_" + displayedConnectionResource.ConnectionResource.Ecu.Name + "_" + displayedConnectionResource.ConnectionResource.Ecu.DiagnosisSource, "ConnectionDialog", new StringSetting(text), false);
		}
	}
}
