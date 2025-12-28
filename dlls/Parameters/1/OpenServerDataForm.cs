// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.OpenServerDataForm
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public sealed class OpenServerDataForm : Form
{
  private string ecuName = string.Empty;
  private Tuple<UnitInformation, bool> unit;
  private OpenServerDataForm.SettingsPair settings;
  private IEnumerable<Tuple<UnitInformation, bool>> availableUnits;
  private IContainer components;
  private Button cancelButton;
  private Button okButton;
  private System.Windows.Forms.Label labelUnit;
  private System.Windows.Forms.Label labelSettings;
  private TableLayoutPanel tableLayoutPanelBody;
  private System.Windows.Forms.Label labelShowingContentFor;
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

  public OpenServerDataForm()
    : this((string) null)
  {
  }

  public OpenServerDataForm(string ecuName)
  {
    this.ecuName = ecuName;
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.availableUnits = (IEnumerable<Tuple<UnitInformation, bool>>) ServerDataManager.GlobalInstance.UnitInformation.Where<UnitInformation>((Func<UnitInformation, bool>) (unit => unit.Status != 3 && unit.DeviceInformation.Any<DeviceInformation>((Func<DeviceInformation, bool>) (di =>
    {
      if (!string.IsNullOrEmpty(this.ecuName) && !(di.Device == this.ecuName))
        return false;
      return unit.SettingsInformation.Any<SettingsInformation>((Func<SettingsInformation, bool>) (si => si.Device == di.Device)) || di.EdexFiles.Any<EdexFileInformation>((Func<EdexFileInformation, bool>) (fi => !fi.HasErrors));
    })))).Select<UnitInformation, Tuple<UnitInformation, bool>>((Func<UnitInformation, Tuple<UnitInformation, bool>>) (unit => Tuple.Create<UnitInformation, bool>(unit, false))).ToList<Tuple<UnitInformation, bool>>();
    Collection<UnitInformation> source = new Collection<UnitInformation>();
    ServerDataManager.GlobalInstance.GetUploadUnits(source, (ServerDataManager.UploadType) 0);
    if (source.Any<UnitInformation>())
      this.availableUnits = this.availableUnits.Union<Tuple<UnitInformation, bool>>(source.Where<UnitInformation>((Func<UnitInformation, bool>) (uploadUnit => uploadUnit.SettingsInformation.Any<SettingsInformation>((Func<SettingsInformation, bool>) (si => string.IsNullOrEmpty(this.ecuName) || si.Device == this.ecuName)))).Select<UnitInformation, Tuple<UnitInformation, bool>>((Func<UnitInformation, Tuple<UnitInformation, bool>>) (u => Tuple.Create<UnitInformation, bool>(u, true))));
    if (!string.IsNullOrEmpty(this.ecuName))
      this.labelShowingContentFor.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.labelShowingContentFor.Text, (object) this.ecuName);
    else
      this.labelShowingContentFor.Visible = this.pictureBoxWarning.Visible = false;
  }

  public UnitInformation Unit => this.unit.Item1;

  public bool UnitIsUpload => this.unit.Item2;

  public SettingsInformation Settings
  {
    get => this.settings == null ? (SettingsInformation) null : this.settings.settingsInformation;
  }

  public SettingsInformation PresetSettings
  {
    get => this.settings == null ? (SettingsInformation) null : this.settings.presetInformation;
  }

  public EdexFileInformation EdexSettings
  {
    get => this.settings == null ? (EdexFileInformation) null : this.settings.edexInformation;
  }

  public bool IncludeExtraSettings => this.settings.includeExtraSettings;

  public IEnumerable<UnitInformation> AvailableUnits
  {
    get
    {
      return this.availableUnits.Select<Tuple<UnitInformation, bool>, UnitInformation>((Func<Tuple<UnitInformation, bool>, UnitInformation>) (au => au.Item1));
    }
  }

  protected override void OnLoad(EventArgs e)
  {
    foreach (Tuple<UnitInformation, bool> availableUnit in this.availableUnits)
    {
      ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(availableUnit.Item1.VehicleIdentity);
      ((ListViewItem) listViewExGroupItem).SubItems.Add(availableUnit.Item1.EngineNumber);
      ((ListViewItem) listViewExGroupItem).SubItems.Add(availableUnit.Item2 ? Resources.OpenServerDataForm_SourceUpload : Resources.OpenServerDataForm_SourceDownload);
      ((ListViewItem) listViewExGroupItem).Tag = (object) availableUnit;
      ((ListView) this.listViewExUnit).Items.Add((ListViewItem) listViewExGroupItem);
    }
    base.OnLoad(e);
  }

  private static int GetPriority(string device)
  {
    Ecu ecuByName = SapiManager.GetEcuByName(device);
    return ecuByName == null ? int.MaxValue : ecuByName.Priority;
  }

  private void listViewExUnit_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.listViewExSettings.BeginUpdate();
    ((ListView) this.listViewExSettings).Items.Clear();
    if (((ListView) this.listViewExUnit).SelectedItems.Count > 0)
    {
      this.unit = ((ListView) this.listViewExUnit).SelectedItems[0].Tag as Tuple<UnitInformation, bool>;
      UnitInformation unitInformation = this.unit.Item1;
      if (!this.unit.Item2)
      {
        foreach (DeviceInformation deviceInformation in (IEnumerable<DeviceInformation>) unitInformation.DeviceInformation.Where<DeviceInformation>((Func<DeviceInformation, bool>) (di => string.IsNullOrEmpty(this.ecuName) || di.Device == this.ecuName)).OrderBy<DeviceInformation, int>((Func<DeviceInformation, int>) (di => OpenServerDataForm.GetPriority(di.Device))))
        {
          DeviceInformation device = deviceInformation;
          foreach (SettingsInformation settings in unitInformation.SettingsInformation.Where<SettingsInformation>((Func<SettingsInformation, bool>) (si => si.Device == device.Device)))
          {
            if (!settings.Preset)
            {
              SettingsInformation settingsForDevice = unitInformation.GetPresetSettingsForDevice(settings.Device);
              if (settingsForDevice != null)
                this.AddSettingsItem(new OpenServerDataForm.SettingsPair(settings, settingsForDevice));
            }
            if (ApplicationInformation.CanViewSeparatedPresetSettings)
              this.AddSettingsItem(new OpenServerDataForm.SettingsPair(settings, (SettingsInformation) null));
          }
          foreach (EdexFileInformation edexFile in device.EdexFiles.Where<EdexFileInformation>((Func<EdexFileInformation, bool>) (efi => !efi.HasErrors)))
          {
            if (ApplicationInformation.CanViewSeparatedPresetSettings && (edexFile.ConfigurationInformation.ApplicableProposedSettingItems().Any<EdexSettingItem>() || edexFile.ConfigurationInformation.ChecSettings != null))
              this.AddSettingsItem(new OpenServerDataForm.SettingsPair(edexFile, false));
            this.AddSettingsItem(new OpenServerDataForm.SettingsPair(edexFile, true));
          }
        }
      }
      else
      {
        foreach (SettingsInformation settings in unitInformation.SettingsInformation)
        {
          FileNameInformation fni = FileNameInformation.FromName(settings.FileName, (FileNameInformation.FileType) 1);
          if (fni.SettingsFileFormatType == 3)
          {
            DeviceInformation informationForDevice = this.availableUnits.Where<Tuple<UnitInformation, bool>>((Func<Tuple<UnitInformation, bool>, bool>) (au => !au.Item2)).Select<Tuple<UnitInformation, bool>, UnitInformation>((Func<Tuple<UnitInformation, bool>, UnitInformation>) (au => au.Item1)).FirstOrDefault<UnitInformation>((Func<UnitInformation, bool>) (u => u.IsSameIdentification(fni.EngineSerialNumber, fni.VehicleIdentity)))?.GetInformationForDevice(fni.Device);
            this.AddSettingsItem(new OpenServerDataForm.SettingsPair(EdexFileInformation.FromUploadData(settings.FileName, informationForDevice), false));
          }
          else
            this.AddSettingsItem(new OpenServerDataForm.SettingsPair(settings, (SettingsInformation) null));
        }
      }
    }
    ((Control) this.listViewExSettings).Enabled = ((ListView) this.listViewExSettings).Items.Count > 0;
    this.okButton.Enabled = ((ListView) this.listViewExSettings).SelectedItems.Count > 0;
    this.listViewExSettings.EndUpdate();
  }

  private void AddSettingsItem(OpenServerDataForm.SettingsPair item)
  {
    ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(item.Device);
    ((ListViewItem) listViewExGroupItem).SubItems.Add(item.Settings);
    ((ListViewItem) listViewExGroupItem).SubItems.Add(item.Hardware);
    ((ListViewItem) listViewExGroupItem).SubItems.Add(item.Timestamp);
    ((ListViewItem) listViewExGroupItem).Tag = (object) item;
    ((ListView) this.listViewExSettings).Items.Add((ListViewItem) listViewExGroupItem);
  }

  private void listViewExSettings_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.settings = ((ListView) this.listViewExSettings).SelectedItems.Count <= 0 ? (OpenServerDataForm.SettingsPair) null : ((ListView) this.listViewExSettings).SelectedItems[0].Tag as OpenServerDataForm.SettingsPair;
    this.okButton.Enabled = this.settings != null;
  }

  private void OpenServerDataForm_SizeChanged(object sender, EventArgs e)
  {
    this.columnHeaderTimestamp.Width = this.columnHeaderDevice.Width = this.columnHeaderHardware.Width = this.columnHeaderSettings.Width = (((Control) this.listViewExSettings).Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.BorderSize.Width * 4) / 4;
    this.columnHeaderVehicle.Width = this.columnHeaderEngine.Width = (((Control) this.listViewExUnit).Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.BorderSize.Width * 4) / 3;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (OpenServerDataForm));
    this.cancelButton = new Button();
    this.okButton = new Button();
    this.labelUnit = new System.Windows.Forms.Label();
    this.labelSettings = new System.Windows.Forms.Label();
    this.tableLayoutPanelBody = new TableLayoutPanel();
    this.listViewExUnit = new ListViewEx();
    this.columnHeaderVehicle = new ColumnHeader();
    this.columnHeaderEngine = new ColumnHeader();
    this.columnHeaderSource = new ColumnHeader();
    this.listViewExSettings = new ListViewEx();
    this.columnHeaderDevice = new ColumnHeader();
    this.columnHeaderSettings = new ColumnHeader();
    this.columnHeaderHardware = new ColumnHeader();
    this.columnHeaderTimestamp = new ColumnHeader();
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.pictureBoxWarning = new PictureBox();
    this.labelShowingContentFor = new System.Windows.Forms.Label();
    this.tableLayoutPanelBody.SuspendLayout();
    ((ISupportInitialize) this.listViewExUnit).BeginInit();
    ((ISupportInitialize) this.listViewExSettings).BeginInit();
    this.flowLayoutPanel1.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxWarning).BeginInit();
    this.SuspendLayout();
    this.cancelButton.DialogResult = DialogResult.Cancel;
    componentResourceManager.ApplyResources((object) this.cancelButton, "cancelButton");
    this.cancelButton.Name = "cancelButton";
    this.cancelButton.UseVisualStyleBackColor = true;
    this.okButton.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.okButton, "okButton");
    this.okButton.Name = "okButton";
    this.okButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelUnit, "labelUnit");
    this.tableLayoutPanelBody.SetColumnSpan((Control) this.labelUnit, 4);
    this.labelUnit.Name = "labelUnit";
    componentResourceManager.ApplyResources((object) this.labelSettings, "labelSettings");
    this.tableLayoutPanelBody.SetColumnSpan((Control) this.labelSettings, 4);
    this.labelSettings.Name = "labelSettings";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelBody, "tableLayoutPanelBody");
    this.tableLayoutPanelBody.Controls.Add((Control) this.listViewExUnit, 0, 1);
    this.tableLayoutPanelBody.Controls.Add((Control) this.cancelButton, 3, 4);
    this.tableLayoutPanelBody.Controls.Add((Control) this.labelSettings, 0, 2);
    this.tableLayoutPanelBody.Controls.Add((Control) this.okButton, 2, 4);
    this.tableLayoutPanelBody.Controls.Add((Control) this.listViewExSettings, 0, 3);
    this.tableLayoutPanelBody.Controls.Add((Control) this.labelUnit, 0, 0);
    this.tableLayoutPanelBody.Controls.Add((Control) this.flowLayoutPanel1, 2, 1);
    this.tableLayoutPanelBody.Name = "tableLayoutPanelBody";
    this.listViewExUnit.CanDelete = false;
    ((ListView) this.listViewExUnit).Columns.AddRange(new ColumnHeader[3]
    {
      this.columnHeaderVehicle,
      this.columnHeaderEngine,
      this.columnHeaderSource
    });
    this.tableLayoutPanelBody.SetColumnSpan((Control) this.listViewExUnit, 2);
    componentResourceManager.ApplyResources((object) this.listViewExUnit, "listViewExUnit");
    this.listViewExUnit.EditableColumn = -1;
    this.listViewExUnit.GridLines = true;
    ((Control) this.listViewExUnit).Name = "listViewExUnit";
    this.listViewExUnit.ShowGlyphs = (GlyphBehavior) 1;
    this.listViewExUnit.ShowItemImages = (ImageBehavior) 1;
    this.listViewExUnit.ShowStateImages = (ImageBehavior) 1;
    ((ListView) this.listViewExUnit).UseCompatibleStateImageBehavior = false;
    ((ListView) this.listViewExUnit).SelectedIndexChanged += new EventHandler(this.listViewExUnit_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.columnHeaderVehicle, "columnHeaderVehicle");
    componentResourceManager.ApplyResources((object) this.columnHeaderEngine, "columnHeaderEngine");
    componentResourceManager.ApplyResources((object) this.columnHeaderSource, "columnHeaderSource");
    this.listViewExSettings.CanDelete = false;
    ((ListView) this.listViewExSettings).Columns.AddRange(new ColumnHeader[4]
    {
      this.columnHeaderDevice,
      this.columnHeaderSettings,
      this.columnHeaderHardware,
      this.columnHeaderTimestamp
    });
    this.tableLayoutPanelBody.SetColumnSpan((Control) this.listViewExSettings, 4);
    componentResourceManager.ApplyResources((object) this.listViewExSettings, "listViewExSettings");
    this.listViewExSettings.EditableColumn = -1;
    this.listViewExSettings.GridLines = true;
    ((Control) this.listViewExSettings).Name = "listViewExSettings";
    this.listViewExSettings.ShowGlyphs = (GlyphBehavior) 1;
    this.listViewExSettings.ShowItemImages = (ImageBehavior) 1;
    this.listViewExSettings.ShowStateImages = (ImageBehavior) 1;
    ((ListView) this.listViewExSettings).UseCompatibleStateImageBehavior = false;
    ((ListView) this.listViewExSettings).SelectedIndexChanged += new EventHandler(this.listViewExSettings_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.columnHeaderDevice, "columnHeaderDevice");
    componentResourceManager.ApplyResources((object) this.columnHeaderSettings, "columnHeaderSettings");
    componentResourceManager.ApplyResources((object) this.columnHeaderHardware, "columnHeaderHardware");
    componentResourceManager.ApplyResources((object) this.columnHeaderTimestamp, "columnHeaderTimestamp");
    this.tableLayoutPanelBody.SetColumnSpan((Control) this.flowLayoutPanel1, 2);
    this.flowLayoutPanel1.Controls.Add((Control) this.pictureBoxWarning);
    this.flowLayoutPanel1.Controls.Add((Control) this.labelShowingContentFor);
    componentResourceManager.ApplyResources((object) this.flowLayoutPanel1, "flowLayoutPanel1");
    this.flowLayoutPanel1.Name = "flowLayoutPanel1";
    this.pictureBoxWarning.Image = (Image) Resources.warning;
    componentResourceManager.ApplyResources((object) this.pictureBoxWarning, "pictureBoxWarning");
    this.pictureBoxWarning.Name = "pictureBoxWarning";
    this.pictureBoxWarning.TabStop = false;
    componentResourceManager.ApplyResources((object) this.labelShowingContentFor, "labelShowingContentFor");
    this.labelShowingContentFor.Name = "labelShowingContentFor";
    this.AcceptButton = (IButtonControl) this.okButton;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.cancelButton;
    this.Controls.Add((Control) this.tableLayoutPanelBody);
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (OpenServerDataForm);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.SizeChanged += new EventHandler(this.OpenServerDataForm_SizeChanged);
    this.tableLayoutPanelBody.ResumeLayout(false);
    this.tableLayoutPanelBody.PerformLayout();
    ((ISupportInitialize) this.listViewExUnit).EndInit();
    ((ISupportInitialize) this.listViewExSettings).EndInit();
    this.flowLayoutPanel1.ResumeLayout(false);
    this.flowLayoutPanel1.PerformLayout();
    ((ISupportInitialize) this.pictureBoxWarning).EndInit();
    this.ResumeLayout(false);
  }

  private class SettingsPair
  {
    public SettingsInformation settingsInformation;
    public SettingsInformation presetInformation;
    public EdexFileInformation edexInformation;
    public bool includeExtraSettings;

    public SettingsPair(SettingsInformation settings, SettingsInformation presets)
    {
      this.settingsInformation = settings;
      this.presetInformation = presets;
    }

    public SettingsPair(EdexFileInformation edexFile, bool includeExtraSettings)
    {
      this.edexInformation = edexFile;
      this.includeExtraSettings = includeExtraSettings;
    }

    public string Device
    {
      get
      {
        return this.settingsInformation == null ? this.edexInformation.ConfigurationInformation.DeviceName : this.settingsInformation.Device;
      }
    }

    public string Hardware
    {
      get
      {
        return this.edexInformation == null ? (string) null : PartExtensions.ToHardwarePartNumberString(this.edexInformation.ConfigurationInformation.HardwarePartNumber, this.Device, true);
      }
    }

    public string Settings
    {
      get
      {
        if (this.settingsInformation != null)
        {
          string settingsType = this.settingsInformation.SettingsType;
          if (this.presetInformation != null)
            settingsType += "+preset";
          return settingsType;
        }
        return !this.includeExtraSettings ? this.edexInformation.FileType.ToString() : this.edexInformation.CompleteFileType;
      }
    }

    public string Timestamp
    {
      get
      {
        if (this.settingsInformation != null && this.settingsInformation.Timestamp.HasValue)
          return this.settingsInformation.Timestamp.ToString();
        EdexFileInformation edexInformation = this.edexInformation;
        return (edexInformation != null ? (((DateTime?) edexInformation.ConfigurationInformation?.DiagnosticLinkSettingsTimestamp).HasValue ? 1 : 0) : 0) != 0 ? this.edexInformation.ConfigurationInformation.DiagnosticLinkSettingsTimestamp.ToString() : (string) null;
      }
    }
  }
}
