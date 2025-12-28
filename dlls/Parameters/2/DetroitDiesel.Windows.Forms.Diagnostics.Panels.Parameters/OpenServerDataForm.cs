using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public sealed class OpenServerDataForm : Form
{
	private class SettingsPair
	{
		public SettingsInformation settingsInformation;

		public SettingsInformation presetInformation;

		public EdexFileInformation edexInformation;

		public bool includeExtraSettings;

		public string Device
		{
			get
			{
				if (settingsInformation == null)
				{
					return edexInformation.ConfigurationInformation.DeviceName;
				}
				return settingsInformation.Device;
			}
		}

		public string Hardware
		{
			get
			{
				if (edexInformation == null)
				{
					return null;
				}
				return PartExtensions.ToHardwarePartNumberString(edexInformation.ConfigurationInformation.HardwarePartNumber, Device, true);
			}
		}

		public string Settings
		{
			get
			{
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				if (settingsInformation != null)
				{
					string text = settingsInformation.SettingsType;
					if (presetInformation != null)
					{
						text += "+preset";
					}
					return text;
				}
				if (!includeExtraSettings)
				{
					return ((object)edexInformation.FileType/*cast due to .constrained prefix*/).ToString();
				}
				return edexInformation.CompleteFileType;
			}
		}

		public string Timestamp
		{
			get
			{
				if (settingsInformation != null && settingsInformation.Timestamp.HasValue)
				{
					return settingsInformation.Timestamp.ToString();
				}
				EdexFileInformation obj = edexInformation;
				if (obj != null)
				{
					EdexConfigurationInformation configurationInformation = obj.ConfigurationInformation;
					if (((configurationInformation != null) ? configurationInformation.DiagnosticLinkSettingsTimestamp : ((DateTime?)null)).HasValue)
					{
						return edexInformation.ConfigurationInformation.DiagnosticLinkSettingsTimestamp.ToString();
					}
				}
				return null;
			}
		}

		public SettingsPair(SettingsInformation settings, SettingsInformation presets)
		{
			settingsInformation = settings;
			presetInformation = presets;
		}

		public SettingsPair(EdexFileInformation edexFile, bool includeExtraSettings)
		{
			edexInformation = edexFile;
			this.includeExtraSettings = includeExtraSettings;
		}
	}

	private string ecuName = string.Empty;

	private Tuple<UnitInformation, bool> unit;

	private SettingsPair settings;

	private IEnumerable<Tuple<UnitInformation, bool>> availableUnits;

	private IContainer components;

	private Button cancelButton;

	private Button okButton;

	private Label labelUnit;

	private Label labelSettings;

	private TableLayoutPanel tableLayoutPanelBody;

	private Label labelShowingContentFor;

	private ListViewEx listViewExSettings;

	private ColumnHeader columnHeaderDevice;

	private ColumnHeader columnHeaderSettings;

	private ColumnHeader columnHeaderHardware;

	private PictureBox pictureBoxWarning;

	private ColumnHeader columnHeaderTimestamp;

	private ListViewEx listViewExUnit;

	private ColumnHeader columnHeaderVehicle;

	private ColumnHeader columnHeaderEngine;

	private FlowLayoutPanel flowLayoutPanel1;

	private ColumnHeader columnHeaderSource;

	public UnitInformation Unit => unit.Item1;

	public bool UnitIsUpload => unit.Item2;

	public SettingsInformation Settings
	{
		get
		{
			if (settings == null)
			{
				return null;
			}
			return settings.settingsInformation;
		}
	}

	public SettingsInformation PresetSettings
	{
		get
		{
			if (settings == null)
			{
				return null;
			}
			return settings.presetInformation;
		}
	}

	public EdexFileInformation EdexSettings
	{
		get
		{
			if (settings == null)
			{
				return null;
			}
			return settings.edexInformation;
		}
	}

	public bool IncludeExtraSettings => settings.includeExtraSettings;

	public IEnumerable<UnitInformation> AvailableUnits => availableUnits.Select((Tuple<UnitInformation, bool> au) => au.Item1);

	public OpenServerDataForm()
		: this(null)
	{
	}

	public OpenServerDataForm(string ecuName)
	{
		this.ecuName = ecuName;
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		availableUnits = (from unit in ServerDataManager.GlobalInstance.UnitInformation
			where (int)unit.Status != 3 && unit.DeviceInformation.Any((DeviceInformation di) => (string.IsNullOrEmpty(this.ecuName) || di.Device == this.ecuName) && (unit.SettingsInformation.Any((SettingsInformation si) => si.Device == di.Device) || di.EdexFiles.Any((EdexFileInformation fi) => !fi.HasErrors)))
			select Tuple.Create<UnitInformation, bool>(unit, item2: false)).ToList();
		Collection<UnitInformation> collection = new Collection<UnitInformation>();
		ServerDataManager.GlobalInstance.GetUploadUnits(collection, (UploadType)0);
		if (collection.Any())
		{
			availableUnits = availableUnits.Union(from u in collection
				where u.SettingsInformation.Any((SettingsInformation si) => string.IsNullOrEmpty(this.ecuName) || si.Device == this.ecuName)
				select Tuple.Create<UnitInformation, bool>(u, item2: true));
		}
		if (!string.IsNullOrEmpty(this.ecuName))
		{
			labelShowingContentFor.Text = string.Format(CultureInfo.CurrentCulture, labelShowingContentFor.Text, this.ecuName);
		}
		else
		{
			labelShowingContentFor.Visible = (pictureBoxWarning.Visible = false);
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		foreach (Tuple<UnitInformation, bool> availableUnit in availableUnits)
		{
			ListViewExGroupItem val = new ListViewExGroupItem(availableUnit.Item1.VehicleIdentity);
			((ListViewItem)(object)val).SubItems.Add(availableUnit.Item1.EngineNumber);
			((ListViewItem)(object)val).SubItems.Add(availableUnit.Item2 ? Resources.OpenServerDataForm_SourceUpload : Resources.OpenServerDataForm_SourceDownload);
			((ListViewItem)(object)val).Tag = availableUnit;
			((ListView)(object)listViewExUnit).Items.Add((ListViewItem)(object)val);
		}
		base.OnLoad(e);
	}

	private static int GetPriority(string device)
	{
		return SapiManager.GetEcuByName(device)?.Priority ?? int.MaxValue;
	}

	private void listViewExUnit_SelectedIndexChanged(object sender, EventArgs e)
	{
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Invalid comparison between Unknown and I4
		listViewExSettings.BeginUpdate();
		((ListView)(object)listViewExSettings).Items.Clear();
		if (((ListView)(object)listViewExUnit).SelectedItems.Count > 0)
		{
			unit = ((ListView)(object)listViewExUnit).SelectedItems[0].Tag as Tuple<UnitInformation, bool>;
			UnitInformation item = unit.Item1;
			if (!unit.Item2)
			{
				foreach (DeviceInformation device in from di in item.DeviceInformation
					where string.IsNullOrEmpty(ecuName) || di.Device == ecuName
					orderby GetPriority(di.Device)
					select di)
				{
					foreach (SettingsInformation item2 in item.SettingsInformation.Where((SettingsInformation si) => si.Device == device.Device))
					{
						if (!item2.Preset)
						{
							SettingsInformation presetSettingsForDevice = item.GetPresetSettingsForDevice(item2.Device);
							if (presetSettingsForDevice != null)
							{
								AddSettingsItem(new SettingsPair(item2, presetSettingsForDevice));
							}
						}
						if (ApplicationInformation.CanViewSeparatedPresetSettings)
						{
							AddSettingsItem(new SettingsPair(item2, null));
						}
					}
					foreach (EdexFileInformation item3 in device.EdexFiles.Where((EdexFileInformation efi) => !efi.HasErrors))
					{
						if (ApplicationInformation.CanViewSeparatedPresetSettings && (item3.ConfigurationInformation.ApplicableProposedSettingItems().Any() || item3.ConfigurationInformation.ChecSettings != null))
						{
							AddSettingsItem(new SettingsPair(item3, includeExtraSettings: false));
						}
						AddSettingsItem(new SettingsPair(item3, includeExtraSettings: true));
					}
				}
			}
			else
			{
				foreach (SettingsInformation item4 in item.SettingsInformation)
				{
					FileNameInformation fni = FileNameInformation.FromName(item4.FileName, (FileType)1);
					if ((int)fni.SettingsFileFormatType == 3)
					{
						UnitInformation val = (from au in availableUnits
							where !au.Item2
							select au.Item1).FirstOrDefault((UnitInformation u) => u.IsSameIdentification(fni.EngineSerialNumber, fni.VehicleIdentity));
						DeviceInformation val2 = ((val != null) ? val.GetInformationForDevice(fni.Device) : null);
						AddSettingsItem(new SettingsPair(EdexFileInformation.FromUploadData(item4.FileName, val2), includeExtraSettings: false));
					}
					else
					{
						AddSettingsItem(new SettingsPair(item4, null));
					}
				}
			}
		}
		((Control)(object)listViewExSettings).Enabled = ((ListView)(object)listViewExSettings).Items.Count > 0;
		okButton.Enabled = ((ListView)(object)listViewExSettings).SelectedItems.Count > 0;
		listViewExSettings.EndUpdate();
	}

	private void AddSettingsItem(SettingsPair item)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		ListViewExGroupItem val = new ListViewExGroupItem(item.Device);
		((ListViewItem)(object)val).SubItems.Add(item.Settings);
		((ListViewItem)(object)val).SubItems.Add(item.Hardware);
		((ListViewItem)(object)val).SubItems.Add(item.Timestamp);
		((ListViewItem)(object)val).Tag = item;
		((ListView)(object)listViewExSettings).Items.Add((ListViewItem)(object)val);
	}

	private void listViewExSettings_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (((ListView)(object)listViewExSettings).SelectedItems.Count > 0)
		{
			settings = ((ListView)(object)listViewExSettings).SelectedItems[0].Tag as SettingsPair;
		}
		else
		{
			settings = null;
		}
		okButton.Enabled = settings != null;
	}

	private void OpenServerDataForm_SizeChanged(object sender, EventArgs e)
	{
		int num = ((Control)(object)listViewExSettings).Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.BorderSize.Width * 4;
		ColumnHeader columnHeader = columnHeaderTimestamp;
		ColumnHeader columnHeader2 = columnHeaderDevice;
		ColumnHeader columnHeader3 = columnHeaderHardware;
		int num2 = (columnHeaderSettings.Width = num / 4);
		int num4 = (columnHeader3.Width = num2);
		int num6 = (columnHeader2.Width = num4);
		columnHeader.Width = num6;
		int num8 = ((Control)(object)listViewExUnit).Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.BorderSize.Width * 4;
		ColumnHeader columnHeader4 = columnHeaderVehicle;
		num6 = (columnHeaderEngine.Width = num8 / 3);
		columnHeader4.Width = num6;
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
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.OpenServerDataForm));
		this.cancelButton = new System.Windows.Forms.Button();
		this.okButton = new System.Windows.Forms.Button();
		this.labelUnit = new System.Windows.Forms.Label();
		this.labelSettings = new System.Windows.Forms.Label();
		this.tableLayoutPanelBody = new System.Windows.Forms.TableLayoutPanel();
		this.listViewExUnit = new ListViewEx();
		this.columnHeaderVehicle = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderEngine = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderSource = new System.Windows.Forms.ColumnHeader();
		this.listViewExSettings = new ListViewEx();
		this.columnHeaderDevice = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderSettings = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderHardware = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderTimestamp = new System.Windows.Forms.ColumnHeader();
		this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
		this.pictureBoxWarning = new System.Windows.Forms.PictureBox();
		this.labelShowingContentFor = new System.Windows.Forms.Label();
		this.tableLayoutPanelBody.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.listViewExUnit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.listViewExSettings).BeginInit();
		this.flowLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxWarning).BeginInit();
		base.SuspendLayout();
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		resources.ApplyResources(this.cancelButton, "cancelButton");
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.UseVisualStyleBackColor = true;
		this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		resources.ApplyResources(this.okButton, "okButton");
		this.okButton.Name = "okButton";
		this.okButton.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.labelUnit, "labelUnit");
		this.tableLayoutPanelBody.SetColumnSpan(this.labelUnit, 4);
		this.labelUnit.Name = "labelUnit";
		resources.ApplyResources(this.labelSettings, "labelSettings");
		this.tableLayoutPanelBody.SetColumnSpan(this.labelSettings, 4);
		this.labelSettings.Name = "labelSettings";
		resources.ApplyResources(this.tableLayoutPanelBody, "tableLayoutPanelBody");
		this.tableLayoutPanelBody.Controls.Add((System.Windows.Forms.Control)(object)this.listViewExUnit, 0, 1);
		this.tableLayoutPanelBody.Controls.Add(this.cancelButton, 3, 4);
		this.tableLayoutPanelBody.Controls.Add(this.labelSettings, 0, 2);
		this.tableLayoutPanelBody.Controls.Add(this.okButton, 2, 4);
		this.tableLayoutPanelBody.Controls.Add((System.Windows.Forms.Control)(object)this.listViewExSettings, 0, 3);
		this.tableLayoutPanelBody.Controls.Add(this.labelUnit, 0, 0);
		this.tableLayoutPanelBody.Controls.Add(this.flowLayoutPanel1, 2, 1);
		this.tableLayoutPanelBody.Name = "tableLayoutPanelBody";
		this.listViewExUnit.CanDelete = false;
		((System.Windows.Forms.ListView)(object)this.listViewExUnit).Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.columnHeaderVehicle, this.columnHeaderEngine, this.columnHeaderSource });
		this.tableLayoutPanelBody.SetColumnSpan((System.Windows.Forms.Control)(object)this.listViewExUnit, 2);
		resources.ApplyResources(this.listViewExUnit, "listViewExUnit");
		this.listViewExUnit.EditableColumn = -1;
		this.listViewExUnit.GridLines = true;
		((System.Windows.Forms.Control)(object)this.listViewExUnit).Name = "listViewExUnit";
		this.listViewExUnit.ShowGlyphs = (GlyphBehavior)1;
		this.listViewExUnit.ShowItemImages = (ImageBehavior)1;
		this.listViewExUnit.ShowStateImages = (ImageBehavior)1;
		((System.Windows.Forms.ListView)(object)this.listViewExUnit).UseCompatibleStateImageBehavior = false;
		((System.Windows.Forms.ListView)(object)this.listViewExUnit).SelectedIndexChanged += new System.EventHandler(listViewExUnit_SelectedIndexChanged);
		resources.ApplyResources(this.columnHeaderVehicle, "columnHeaderVehicle");
		resources.ApplyResources(this.columnHeaderEngine, "columnHeaderEngine");
		resources.ApplyResources(this.columnHeaderSource, "columnHeaderSource");
		this.listViewExSettings.CanDelete = false;
		((System.Windows.Forms.ListView)(object)this.listViewExSettings).Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] { this.columnHeaderDevice, this.columnHeaderSettings, this.columnHeaderHardware, this.columnHeaderTimestamp });
		this.tableLayoutPanelBody.SetColumnSpan((System.Windows.Forms.Control)(object)this.listViewExSettings, 4);
		resources.ApplyResources(this.listViewExSettings, "listViewExSettings");
		this.listViewExSettings.EditableColumn = -1;
		this.listViewExSettings.GridLines = true;
		((System.Windows.Forms.Control)(object)this.listViewExSettings).Name = "listViewExSettings";
		this.listViewExSettings.ShowGlyphs = (GlyphBehavior)1;
		this.listViewExSettings.ShowItemImages = (ImageBehavior)1;
		this.listViewExSettings.ShowStateImages = (ImageBehavior)1;
		((System.Windows.Forms.ListView)(object)this.listViewExSettings).UseCompatibleStateImageBehavior = false;
		((System.Windows.Forms.ListView)(object)this.listViewExSettings).SelectedIndexChanged += new System.EventHandler(listViewExSettings_SelectedIndexChanged);
		resources.ApplyResources(this.columnHeaderDevice, "columnHeaderDevice");
		resources.ApplyResources(this.columnHeaderSettings, "columnHeaderSettings");
		resources.ApplyResources(this.columnHeaderHardware, "columnHeaderHardware");
		resources.ApplyResources(this.columnHeaderTimestamp, "columnHeaderTimestamp");
		this.tableLayoutPanelBody.SetColumnSpan(this.flowLayoutPanel1, 2);
		this.flowLayoutPanel1.Controls.Add(this.pictureBoxWarning);
		this.flowLayoutPanel1.Controls.Add(this.labelShowingContentFor);
		resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
		this.flowLayoutPanel1.Name = "flowLayoutPanel1";
		this.pictureBoxWarning.Image = DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties.Resources.warning;
		resources.ApplyResources(this.pictureBoxWarning, "pictureBoxWarning");
		this.pictureBoxWarning.Name = "pictureBoxWarning";
		this.pictureBoxWarning.TabStop = false;
		resources.ApplyResources(this.labelShowingContentFor, "labelShowingContentFor");
		this.labelShowingContentFor.Name = "labelShowingContentFor";
		base.AcceptButton = this.okButton;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.cancelButton;
		base.Controls.Add(this.tableLayoutPanelBody);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "OpenServerDataForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.SizeChanged += new System.EventHandler(OpenServerDataForm_SizeChanged);
		this.tableLayoutPanelBody.ResumeLayout(false);
		this.tableLayoutPanelBody.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.listViewExUnit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.listViewExSettings).EndInit();
		this.flowLayoutPanel1.ResumeLayout(false);
		this.flowLayoutPanel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxWarning).EndInit();
		base.ResumeLayout(false);
	}
}
