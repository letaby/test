// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.ConnectionDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class ConnectionDialog : Form
{
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

  protected override void Dispose(bool disposing)
  {
    this.sapi.ActiveChannelsListChanged -= new EventHandler(this.OnSapiActiveChannelsListChanged);
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ConnectionDialog));
    this.labelConnection = new Label();
    this.tableLayoutPanelFamily = new TableLayoutPanel();
    this.radioButtonPowertrain = new RadioButton();
    this.comboBoxVehicle = new ComboBox();
    this.radioButtonVehicle = new RadioButton();
    this.pictureBoxLogo = new PictureBox();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.comboBoxConnectionResource = new ComboBox();
    this.buttonCancel = new Button();
    this.buttonConnect = new Button();
    this.listViewExEcus = new ListViewEx();
    this.columnHeaderName = new ColumnHeader();
    this.columnHeaderDescription = new ColumnHeader();
    this.columnHeaderCategory = new ColumnHeader();
    this.columnHeaderIdentifier = new ColumnHeader();
    this.checkBoxShowAll = new CheckBox();
    this.buttonCommunicationParameters = new Button();
    this.labelDiagnosticDescription = new Label();
    this.buttonChangeDiagnosticDescription = new Button();
    this.textBoxDiagnosticDescription = new TextBox();
    this.checkBoxAdvanced = new CheckBox();
    this.tableLayoutPanelAdvanced = new TableLayoutPanel();
    this.checkBoxExecuteStartComm = new CheckBox();
    this.checkBoxAutoExecConfiguredServices = new CheckBox();
    this.checkBoxCyclicRead = new CheckBox();
    this.flowLayoutPanelVariant = new FlowLayoutPanel();
    this.labelTargetVariant = new Label();
    this.comboBoxTargetVariant = new ComboBox();
    this.tableLayoutPanelFamily.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxLogo).BeginInit();
    this.tableLayoutPanelMain.SuspendLayout();
    ((ISupportInitialize) this.listViewExEcus).BeginInit();
    this.tableLayoutPanelAdvanced.SuspendLayout();
    this.flowLayoutPanelVariant.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.labelConnection, "labelConnection");
    this.labelConnection.Name = "labelConnection";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFamily, "tableLayoutPanelFamily");
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.tableLayoutPanelFamily, 10);
    this.tableLayoutPanelFamily.Controls.Add((Control) this.radioButtonPowertrain, 0, 1);
    this.tableLayoutPanelFamily.Controls.Add((Control) this.comboBoxVehicle, 1, 0);
    this.tableLayoutPanelFamily.Controls.Add((Control) this.radioButtonVehicle, 0, 0);
    this.tableLayoutPanelFamily.Controls.Add((Control) this.pictureBoxLogo, 4, 0);
    this.tableLayoutPanelFamily.Name = "tableLayoutPanelFamily";
    componentResourceManager.ApplyResources((object) this.radioButtonPowertrain, "radioButtonPowertrain");
    this.radioButtonPowertrain.Name = "radioButtonPowertrain";
    this.radioButtonPowertrain.TabStop = true;
    this.radioButtonPowertrain.UseVisualStyleBackColor = true;
    this.radioButtonPowertrain.CheckedChanged += new EventHandler(this.radioButtonPowertrain_CheckedChanged);
    this.comboBoxVehicle.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxVehicle.FormattingEnabled = true;
    componentResourceManager.ApplyResources((object) this.comboBoxVehicle, "comboBoxVehicle");
    this.comboBoxVehicle.Name = "comboBoxVehicle";
    this.comboBoxVehicle.SelectedIndexChanged += new EventHandler(this.comboBoxVehicle_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.radioButtonVehicle, "radioButtonVehicle");
    this.radioButtonVehicle.Name = "radioButtonVehicle";
    this.radioButtonVehicle.TabStop = true;
    this.radioButtonVehicle.UseVisualStyleBackColor = true;
    this.radioButtonVehicle.CheckedChanged += new EventHandler(this.radioButtonVehicle_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.pictureBoxLogo, "pictureBoxLogo");
    this.pictureBoxLogo.Name = "pictureBoxLogo";
    this.tableLayoutPanelFamily.SetRowSpan((Control) this.pictureBoxLogo, 2);
    this.pictureBoxLogo.TabStop = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    this.tableLayoutPanelMain.Controls.Add((Control) this.comboBoxConnectionResource, 1, 2);
    this.tableLayoutPanelMain.Controls.Add((Control) this.labelConnection, 0, 2);
    this.tableLayoutPanelMain.Controls.Add((Control) this.buttonCancel, 8, 4);
    this.tableLayoutPanelMain.Controls.Add((Control) this.buttonConnect, 7, 4);
    this.tableLayoutPanelMain.Controls.Add((Control) this.tableLayoutPanelFamily, 0, 0);
    this.tableLayoutPanelMain.Controls.Add((Control) this.listViewExEcus, 0, 1);
    this.tableLayoutPanelMain.Controls.Add((Control) this.checkBoxShowAll, 8, 2);
    this.tableLayoutPanelMain.Controls.Add((Control) this.buttonCommunicationParameters, 3, 4);
    this.tableLayoutPanelMain.Controls.Add((Control) this.labelDiagnosticDescription, 0, 3);
    this.tableLayoutPanelMain.Controls.Add((Control) this.buttonChangeDiagnosticDescription, 9, 3);
    this.tableLayoutPanelMain.Controls.Add((Control) this.textBoxDiagnosticDescription, 3, 3);
    this.tableLayoutPanelMain.Controls.Add((Control) this.checkBoxAdvanced, 0, 4);
    this.tableLayoutPanelMain.Controls.Add((Control) this.tableLayoutPanelAdvanced, 0, 6);
    this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.comboBoxConnectionResource, 7);
    componentResourceManager.ApplyResources((object) this.comboBoxConnectionResource, "comboBoxConnectionResource");
    this.comboBoxConnectionResource.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxConnectionResource.FormattingEnabled = true;
    this.comboBoxConnectionResource.Name = "comboBoxConnectionResource";
    this.comboBoxConnectionResource.SelectedIndexChanged += new EventHandler(this.comboBoxConnectionResource_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.buttonCancel, 2);
    this.buttonCancel.DialogResult = DialogResult.Cancel;
    this.buttonCancel.Name = "buttonCancel";
    componentResourceManager.ApplyResources((object) this.buttonConnect, "buttonConnect");
    this.buttonConnect.DialogResult = DialogResult.OK;
    this.buttonConnect.Name = "buttonConnect";
    this.buttonConnect.Click += new EventHandler(this.OnConnectButtonClick);
    this.listViewExEcus.CanDelete = false;
    ((ListView) this.listViewExEcus).Columns.AddRange(new ColumnHeader[4]
    {
      this.columnHeaderName,
      this.columnHeaderDescription,
      this.columnHeaderCategory,
      this.columnHeaderIdentifier
    });
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.listViewExEcus, 10);
    componentResourceManager.ApplyResources((object) this.listViewExEcus, "listViewExEcus");
    this.listViewExEcus.EditableColumn = -1;
    this.listViewExEcus.HeaderStyle = ColumnHeaderStyle.Clickable;
    ((Control) this.listViewExEcus).Name = "listViewExEcus";
    this.listViewExEcus.ShowGlyphs = (GlyphBehavior) 1;
    ((ListView) this.listViewExEcus).ShowGroups = false;
    this.listViewExEcus.ShowItemImages = (ImageBehavior) 1;
    this.listViewExEcus.ShowStateImages = (ImageBehavior) 1;
    ((ListView) this.listViewExEcus).UseCompatibleStateImageBehavior = false;
    ((ListView) this.listViewExEcus).ColumnClick += new ColumnClickEventHandler(this.listViewExEcus_ColumnClick);
    ((ListView) this.listViewExEcus).SelectedIndexChanged += new EventHandler(this.listViewExEcus_SelectedIndexChanged);
    ((Control) this.listViewExEcus).DoubleClick += new EventHandler(this.listViewExEcus_DoubleClick_1);
    ((Control) this.listViewExEcus).KeyPress += new KeyPressEventHandler(this.listViewExEcus_KeyPress);
    componentResourceManager.ApplyResources((object) this.columnHeaderName, "columnHeaderName");
    componentResourceManager.ApplyResources((object) this.columnHeaderDescription, "columnHeaderDescription");
    componentResourceManager.ApplyResources((object) this.columnHeaderCategory, "columnHeaderCategory");
    componentResourceManager.ApplyResources((object) this.columnHeaderIdentifier, "columnHeaderIdentifier");
    componentResourceManager.ApplyResources((object) this.checkBoxShowAll, "checkBoxShowAll");
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.checkBoxShowAll, 2);
    this.checkBoxShowAll.Name = "checkBoxShowAll";
    this.checkBoxShowAll.UseVisualStyleBackColor = true;
    this.checkBoxShowAll.CheckedChanged += new EventHandler(this.checkBoxShowAll_CheckedChanged_1);
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.buttonCommunicationParameters, 4);
    componentResourceManager.ApplyResources((object) this.buttonCommunicationParameters, "buttonCommunicationParameters");
    this.buttonCommunicationParameters.Name = "buttonCommunicationParameters";
    this.buttonCommunicationParameters.UseVisualStyleBackColor = true;
    this.buttonCommunicationParameters.Click += new EventHandler(this.buttonCommunicationParameters_Click);
    componentResourceManager.ApplyResources((object) this.labelDiagnosticDescription, "labelDiagnosticDescription");
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.labelDiagnosticDescription, 3);
    this.labelDiagnosticDescription.Name = "labelDiagnosticDescription";
    componentResourceManager.ApplyResources((object) this.buttonChangeDiagnosticDescription, "buttonChangeDiagnosticDescription");
    this.buttonChangeDiagnosticDescription.Name = "buttonChangeDiagnosticDescription";
    this.buttonChangeDiagnosticDescription.Click += new EventHandler(this.buttonChangeDiagnosticDescription_Click);
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.textBoxDiagnosticDescription, 6);
    componentResourceManager.ApplyResources((object) this.textBoxDiagnosticDescription, "textBoxDiagnosticDescription");
    this.textBoxDiagnosticDescription.Name = "textBoxDiagnosticDescription";
    componentResourceManager.ApplyResources((object) this.checkBoxAdvanced, "checkBoxAdvanced");
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.checkBoxAdvanced, 3);
    this.checkBoxAdvanced.Image = (Image) Resources.navigate_down;
    this.checkBoxAdvanced.Name = "checkBoxAdvanced";
    this.checkBoxAdvanced.UseVisualStyleBackColor = true;
    this.checkBoxAdvanced.CheckedChanged += new EventHandler(this.checkBoxAdvanced_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAdvanced, "tableLayoutPanelAdvanced");
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.tableLayoutPanelAdvanced, 10);
    this.tableLayoutPanelAdvanced.Controls.Add((Control) this.checkBoxExecuteStartComm, 0, 1);
    this.tableLayoutPanelAdvanced.Controls.Add((Control) this.checkBoxAutoExecConfiguredServices, 0, 2);
    this.tableLayoutPanelAdvanced.Controls.Add((Control) this.checkBoxCyclicRead, 1, 1);
    this.tableLayoutPanelAdvanced.Controls.Add((Control) this.flowLayoutPanelVariant, 0, 0);
    this.tableLayoutPanelAdvanced.Name = "tableLayoutPanelAdvanced";
    componentResourceManager.ApplyResources((object) this.checkBoxExecuteStartComm, "checkBoxExecuteStartComm");
    this.checkBoxExecuteStartComm.Name = "checkBoxExecuteStartComm";
    this.checkBoxExecuteStartComm.UseVisualStyleBackColor = true;
    this.checkBoxExecuteStartComm.CheckedChanged += new EventHandler(this.checkBoxExecuteStartComm_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxAutoExecConfiguredServices, "checkBoxAutoExecConfiguredServices");
    this.tableLayoutPanelAdvanced.SetColumnSpan((Control) this.checkBoxAutoExecConfiguredServices, 2);
    this.checkBoxAutoExecConfiguredServices.Name = "checkBoxAutoExecConfiguredServices";
    this.checkBoxAutoExecConfiguredServices.UseVisualStyleBackColor = true;
    this.checkBoxAutoExecConfiguredServices.CheckedChanged += new EventHandler(this.checkBoxAutoExecConfiguredServices_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxCyclicRead, "checkBoxCyclicRead");
    this.checkBoxCyclicRead.Name = "checkBoxCyclicRead";
    this.checkBoxCyclicRead.UseVisualStyleBackColor = true;
    this.checkBoxCyclicRead.CheckedChanged += new EventHandler(this.checkBoxCyclicRead_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.flowLayoutPanelVariant, "flowLayoutPanelVariant");
    this.tableLayoutPanelAdvanced.SetColumnSpan((Control) this.flowLayoutPanelVariant, 2);
    this.flowLayoutPanelVariant.Controls.Add((Control) this.labelTargetVariant);
    this.flowLayoutPanelVariant.Controls.Add((Control) this.comboBoxTargetVariant);
    this.flowLayoutPanelVariant.Name = "flowLayoutPanelVariant";
    componentResourceManager.ApplyResources((object) this.labelTargetVariant, "labelTargetVariant");
    this.labelTargetVariant.Name = "labelTargetVariant";
    this.comboBoxTargetVariant.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxTargetVariant.FormattingEnabled = true;
    componentResourceManager.ApplyResources((object) this.comboBoxTargetVariant, "comboBoxTargetVariant");
    this.comboBoxTargetVariant.Name = "comboBoxTargetVariant";
    this.comboBoxTargetVariant.SelectedIndexChanged += new EventHandler(this.comboBoxTargetVariant_SelectedIndexChanged);
    this.AcceptButton = (IButtonControl) this.buttonConnect;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.buttonCancel;
    this.Controls.Add((Control) this.tableLayoutPanelMain);
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (ConnectionDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.HelpButtonClicked += new CancelEventHandler(this.OnHelpButtonClicked);
    this.tableLayoutPanelFamily.ResumeLayout(false);
    this.tableLayoutPanelFamily.PerformLayout();
    ((ISupportInitialize) this.pictureBoxLogo).EndInit();
    this.tableLayoutPanelMain.ResumeLayout(false);
    this.tableLayoutPanelMain.PerformLayout();
    ((ISupportInitialize) this.listViewExEcus).EndInit();
    this.tableLayoutPanelAdvanced.ResumeLayout(false);
    this.tableLayoutPanelAdvanced.PerformLayout();
    this.flowLayoutPanelVariant.ResumeLayout(false);
    this.flowLayoutPanelVariant.PerformLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  public ConnectionDialog()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.lvwColumnSorter = new ListViewColumnSorter();
    this.lvwColumnSorter.ColumnToSort = this.columnHeaderIdentifier.Index;
    this.lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
    this.lvwColumnSorter.SortType = ListViewColumnSorter.SortBy.Tag;
    ((ListView) this.listViewExEcus).ListViewItemSorter = (IComparer) this.lvwColumnSorter;
    this.sapi = SapiManager.GlobalInstance;
    this.sapi.ActiveChannelsListChanged += new EventHandler(this.OnSapiActiveChannelsListChanged);
    this.pictureBoxLogo.Image = ApplicationInformation.Branding.Logo;
    this.tableLayoutPanelFamily.BackColor = this.tableLayoutPanelMain.BackColor = this.BackColor = ApplicationInformation.Branding.LogoBackColor;
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    string[] strArray = SettingsManager.GlobalInstance.GetValue<StringSetting>("Family", nameof (ConnectionDialog), new StringSetting("Vehicle/All")).Value.Split("/".ToCharArray());
    switch (strArray[0])
    {
      case "Vehicle":
        this.radioButtonVehicle.Checked = true;
        break;
      case "Engine":
        this.radioButtonPowertrain.Checked = true;
        break;
    }
    this.PopulateFamilyCombo(strArray[1]);
    this.BuildList();
    ((Control) this.listViewExEcus).Select();
    this.checkBoxShowAll.Checked = SettingsManager.GlobalInstance.GetValue<bool>("ShowAllResources", nameof (ConnectionDialog), this.checkBoxShowAll.Checked);
    int num = ((Control) this.listViewExEcus).ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
    this.columnHeaderName.Width = num * 4 / 20;
    this.columnHeaderDescription.Width = num * 10 / 20;
    this.columnHeaderCategory.Width = num * 3 / 20;
    this.columnHeaderIdentifier.Width = num * 3 / 20;
  }

  private void OnSapiActiveChannelsListChanged(object sender, EventArgs e)
  {
    ListViewExGroupItem selectedItem = ((ListView) this.listViewExEcus).SelectedItems.Count > 0 ? (ListViewExGroupItem) ((ListView) this.listViewExEcus).SelectedItems[0] : (ListViewExGroupItem) null;
    bool enabled = this.buttonConnect.Enabled;
    this.buttonConnect.Enabled = false;
    this.BuildList();
    if (!((ListView) this.listViewExEcus).SelectedItems.Contains((ListViewItem) selectedItem))
      return;
    this.buttonConnect.Enabled = enabled;
  }

  private void BuildList()
  {
    this.listViewExEcus.BeginUpdate();
    this.listViewExEcus.LockSorting();
    ListViewExGroupItem selectedItem = ((ListView) this.listViewExEcus).SelectedItems.Count > 0 ? (ListViewExGroupItem) ((ListView) this.listViewExEcus).SelectedItems[0] : (ListViewExGroupItem) null;
    bool flag = selectedItem != null && ((ListViewItem) selectedItem).Bounds.IntersectsWith(((Control) this.listViewExEcus).ClientRectangle);
    bool showIfNoCategoryMatch = this.radioButtonVehicle.Checked;
    ConnectionDialog.DisplayedFamily requiredFamily = this.comboBoxVehicle.SelectedItem as ConnectionDialog.DisplayedFamily;
    List<Ecu> list = CollectionExtensions.DistinctBy<Ecu, string>(((IEnumerable<Ecu>) this.sapi.Sapi.Ecus).Where<Ecu>((Func<Ecu, bool>) (ecu => !ecu.IsRollCall && !ecu.OfflineSupportOnly && this.sapi.Sapi.Ecus.GetConnectedCountForIdentifier(ecu.Identifier) == 0 && ConnectionDialog.IsAllowedForSelectedCategoryFamily(this.selectedCategory, requiredFamily, ecu, showIfNoCategoryMatch))), (Func<Ecu, string>) (ecu => ecu.Name)).OrderBy<Ecu, int>((Func<Ecu, int>) (ecu => ecu.Priority)).ToList<Ecu>();
    foreach (ListViewExGroupItem listViewExGroupItem in ((ListView) this.listViewExEcus).Items.OfType<ListViewExGroupItem>().ToList<ListViewExGroupItem>())
    {
      Ecu tag = ((ListViewItem) listViewExGroupItem).Tag as Ecu;
      if (!list.Contains(tag))
        ((ListView) this.listViewExEcus).Items.Remove((ListViewItem) listViewExGroupItem);
      else
        list.Remove(tag);
    }
    foreach (Ecu ecu in list)
    {
      ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(ecu.Name);
      ((ListViewItem) listViewExGroupItem).SubItems.Add(ecu.ShortDescription);
      ((ListViewItem) listViewExGroupItem).SubItems.Add(SapiExtensions.EquipmentCategory(ecu));
      ((ListViewItem) listViewExGroupItem).SubItems.Add(ecu.Identifier).Tag = (object) ecu.Priority;
      ((ListViewItem) listViewExGroupItem).Tag = (object) ecu;
      ((ListView) this.listViewExEcus).Items.Add((ListViewItem) listViewExGroupItem);
    }
    if (((ListView) this.listViewExEcus).SelectedItems.Count == 0 && ((ListView) this.listViewExEcus).Items.Count > 0)
      ((ListView) this.listViewExEcus).Items[0].Selected = true;
    this.listViewExEcus.UnlockSorting();
    this.listViewExEcus.EndUpdate();
    if (!flag || !((ListView) this.listViewExEcus).SelectedItems.Contains((ListViewItem) selectedItem))
      return;
    ((ListView) this.listViewExEcus).EnsureVisible(((ListViewItem) selectedItem).Index);
  }

  private void PopulateFamilyCombo(string selectedFamily = null)
  {
    this.comboBoxVehicle.Items.Clear();
    this.comboBoxVehicle.Items.Add((object) Resources.ConnectionDialog_All);
    this.comboBoxVehicle.Items.AddRange((object[]) ElectronicsFamily.AvailableFamilies().Where<ElectronicsFamily>((Func<ElectronicsFamily, bool>) (ef => ((ElectronicsFamily) ref ef).Category == this.selectedCategory)).Select<ElectronicsFamily, ConnectionDialog.DisplayedFamily>((Func<ElectronicsFamily, ConnectionDialog.DisplayedFamily>) (ef => new ConnectionDialog.DisplayedFamily(ef))).ToArray<ConnectionDialog.DisplayedFamily>());
    if (selectedFamily != null)
      this.comboBoxVehicle.SelectedIndex = this.comboBoxVehicle.FindStringExact(selectedFamily, -1);
    if (this.comboBoxVehicle.SelectedItem != null)
      return;
    this.comboBoxVehicle.SelectedItem = (object) Resources.ConnectionDialog_All;
  }

  private static bool IsAllowedForSelectedCategoryFamily(
    string selectedCategory,
    ConnectionDialog.DisplayedFamily selectedFamily,
    Ecu ecu,
    bool defaultIfNoCategoryMatch)
  {
    if (SapiExtensions.EquipmentCategory(ecu) == selectedCategory)
      return selectedFamily == null || ElectronicsFamily.FromDevice(ecu).Contains<ElectronicsFamily>(selectedFamily.Family);
    IEnumerable<string> strings;
    if (!SapiExtensions.TryGetSubcategoryFamilies(ecu, selectedCategory, ref strings))
      return defaultIfNoCategoryMatch;
    if (selectedFamily == null)
      return true;
    IEnumerable<string> source = strings;
    ElectronicsFamily family = selectedFamily.Family;
    string name = ((ElectronicsFamily) ref family).Name;
    return source.Contains<string>(name);
  }

  internal static ChannelOptions ChannelOptionsCyclicServices => (ChannelOptions) 12;

  internal static ChannelOptions ChannelOptionsAutoExecuteConfiguredServices
  {
    get => (ChannelOptions) 240 /*0xF0*/;
  }

  private ChannelOptions GetSelectedChannelOptions()
  {
    ChannelOptions selectedChannelOptions = (ChannelOptions) 0;
    if (this.comboBoxConnectionResource.SelectedItem is ConnectionDialog.DisplayedConnectionResource selectedItem)
    {
      if (this.checkBoxExecuteStartComm.Checked)
        selectedChannelOptions = (ChannelOptions) (selectedChannelOptions | 1);
      if (this.checkBoxCyclicRead.Checked)
        selectedChannelOptions = selectedChannelOptions | ConnectionDialog.ChannelOptionsCyclicServices;
      if (this.checkBoxAutoExecConfiguredServices.Checked)
        selectedChannelOptions = selectedChannelOptions | ConnectionDialog.ChannelOptionsAutoExecuteConfiguredServices;
      selectedChannelOptions = selectedChannelOptions & selectedItem.ConnectionResource.Ecu.AvailableChannelOptions;
      SettingsManager.GlobalInstance.SetValue<int>($"ChannelOptions_{selectedItem.ConnectionResource.Ecu.Name}_{(object) selectedItem.ConnectionResource.Ecu.DiagnosisSource}", nameof (ConnectionDialog), (int) selectedChannelOptions, false);
    }
    return selectedChannelOptions;
  }

  private void OnConnectButtonClick(object sender, EventArgs e)
  {
    ConnectionDialog.DisplayedConnectionResource selectedItem = this.comboBoxConnectionResource.SelectedItem as ConnectionDialog.DisplayedConnectionResource;
    ChannelOptions channelOptions = (ChannelOptions) 253;
    SettingsManager.GlobalInstance.SetValue<StringSetting>("SelectedResource_" + selectedItem.ConnectionResource.Ecu.Name, nameof (ConnectionDialog), new StringSetting(selectedItem.ConnectionResource.ToString()), false);
    this.sapi.OpenConnection(selectedItem.ConnectionResource, channelOptions);
    this.buttonConnect.Enabled = false;
    ((Control) this.listViewExEcus).Enabled = false;
    this.comboBoxConnectionResource.Enabled = false;
    this.Close();
  }

  private void listViewExEcus_SelectedIndexChanged(object sender, EventArgs e)
  {
    ListViewExGroupItem selectedItem = ((ListView) this.listViewExEcus).SelectedItems.Count > 0 ? (ListViewExGroupItem) ((ListView) this.listViewExEcus).SelectedItems[0] : (ListViewExGroupItem) null;
    this.selectedEcu = selectedItem != null ? ((ListViewItem) selectedItem).Tag as Ecu : (Ecu) null;
    this.textBoxDiagnosticDescription.Text = string.Empty;
    this.buttonChangeDiagnosticDescription.Enabled = false;
    this.PopulateConnectionResources();
    this.PopulateTargetVariant();
    this.UpdateChannelOptions();
  }

  private void PopulateConnectionResources()
  {
    bool flag1 = this.checkBoxShowAll.Checked;
    this.comboBoxConnectionResource.Items.Clear();
    bool flag2 = false;
    if (this.selectedEcu != null)
    {
      List<Ecu> source1 = new List<Ecu>();
      List<ConnectionResource> cs = new List<ConnectionResource>();
      List<ConnectionResource> source2 = new List<ConnectionResource>();
      string str = SettingsManager.GlobalInstance.GetValue<StringSetting>("SelectedResource_" + this.selectedEcu.Name, nameof (ConnectionDialog), new StringSetting()).Value;
      foreach (Ecu ecu in ((IEnumerable<Ecu>) this.sapi.Sapi.Ecus).Where<Ecu>((Func<Ecu, bool>) (e => e.Name == this.selectedEcu.Name)))
      {
        if (this.sapi.Sapi.Ecus.GetConnectedCountForIdentifier(ecu.Identifier) == 0)
        {
          cs.AddRange((IEnumerable<ConnectionResource>) ecu.GetConnectionResources());
          if (!string.IsNullOrEmpty(str))
            source2.Add(ConnectionResource.FromString(ecu, str));
        }
      }
      if (!source2.Any<ConnectionResource>((Func<ConnectionResource, bool>) (settingsConnectionResource => settingsConnectionResource != null && cs.Any<ConnectionResource>((Func<ConnectionResource, bool>) (cr => cr.HardwareName == settingsConnectionResource.HardwareName)))))
        str = (string) null;
      foreach (ConnectionResource connectionResource in cs)
      {
        ConnectionResource cr = connectionResource;
        if (flag1 || !cr.Restricted)
        {
          int num = this.comboBoxConnectionResource.Items.Add((object) new ConnectionDialog.DisplayedConnectionResource(cr, flag1 && cs.Any<ConnectionResource>((Func<ConnectionResource, bool>) (ocr => SapiExtensions.IsEquivalentExceptProtocol(cr, ocr)))));
          if (this.comboBoxConnectionResource.SelectedIndex == -1)
          {
            if (!string.IsNullOrEmpty(str))
            {
              if (str == cr.ToString())
                this.comboBoxConnectionResource.SelectedIndex = num;
            }
            else if (!cr.Restricted)
              this.comboBoxConnectionResource.SelectedIndex = num;
          }
        }
        else
        {
          flag2 = true;
          source1.AddRange((IEnumerable<Ecu>) cr.Ecu.ViaEcus);
        }
      }
      if (this.comboBoxConnectionResource.Items.Count == 0)
      {
        if (flag2 && source1.Any<Ecu>())
        {
          Ecu ecu = source1.FirstOrDefault<Ecu>((Func<Ecu, bool>) (ve => ve.ConnectedChannelCount > 0));
          if (ecu != null)
            this.comboBoxConnectionResource.Items.Add((object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ResourceCannotBeDeterminedViaEcuPresent, (object) ecu.Name));
          else
            this.comboBoxConnectionResource.Items.Add((object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ResourceCannotBeDetermined, (object) string.Join<Ecu>("/", source1.Distinct<Ecu>())));
        }
        else
          this.comboBoxConnectionResource.Items.Add((object) Resources.Messsage_NoConnectionResources);
        this.comboBoxConnectionResource.SelectedIndex = 0;
      }
    }
    this.comboBoxConnectionResource.Enabled = this.comboBoxConnectionResource.Items.OfType<ConnectionDialog.DisplayedConnectionResource>().Any<ConnectionDialog.DisplayedConnectionResource>();
    ConnectionDialog.DisplayedConnectionResource selectedItem = this.comboBoxConnectionResource.SelectedItem as ConnectionDialog.DisplayedConnectionResource;
    this.buttonConnect.Enabled = selectedItem != null;
    this.textBoxDiagnosticDescription.Text = selectedItem != null ? $"{Path.GetFileName(selectedItem.ConnectionResource.Ecu.DescriptionFileName)} ({selectedItem.ConnectionResource.Ecu.DescriptionDataVersion})" : string.Empty;
    this.buttonChangeDiagnosticDescription.Enabled = selectedItem != null && string.Equals(Path.GetExtension(selectedItem.ConnectionResource.Ecu.DescriptionFileName), ".cbf", StringComparison.OrdinalIgnoreCase);
  }

  private void PopulateTargetVariant()
  {
    this.comboBoxTargetVariant.Items.Clear();
    ConnectionDialog.DisplayedConnectionResource selectedItem = this.comboBoxConnectionResource.SelectedItem as ConnectionDialog.DisplayedConnectionResource;
    this.comboBoxTargetVariant.Enabled = selectedItem != null;
    if (selectedItem == null)
      return;
    string str = SettingsManager.GlobalInstance.GetValue<StringSetting>($"FixedVariant_{selectedItem.ConnectionResource.Ecu.Name}_{(object) selectedItem.ConnectionResource.Ecu.DiagnosisSource}", nameof (ConnectionDialog), new StringSetting(string.Empty)).Value;
    int num1 = this.comboBoxTargetVariant.Items.Add((object) Resources.ConnectionDialog_UseDetectedVariant);
    foreach (DiagnosisVariant diagnosisVariant in (ReadOnlyCollection<DiagnosisVariant>) selectedItem.ConnectionResource.Ecu.DiagnosisVariants)
    {
      int num2 = this.comboBoxTargetVariant.Items.Add((object) diagnosisVariant);
      if (diagnosisVariant.Name == str)
        num1 = num2;
    }
    this.comboBoxTargetVariant.SelectedIndex = num1;
  }

  private void UpdateChannelOptions()
  {
    ChannelOptions availableChannelOptions = this.comboBoxConnectionResource.SelectedItem is ConnectionDialog.DisplayedConnectionResource selectedItem ? selectedItem.ConnectionResource.Ecu.AvailableChannelOptions : (ChannelOptions) (object) 0;
    int num;
    if (selectedItem == null)
      num = 0;
    else
      num = SettingsManager.GlobalInstance.GetValue<int>($"ChannelOptions_{selectedItem.ConnectionResource.Ecu.Name}_{(object) selectedItem.ConnectionResource.Ecu.DiagnosisSource}", nameof (ConnectionDialog), (int) availableChannelOptions);
    ChannelOptions channelOptions = (ChannelOptions) num;
    this.checkBoxExecuteStartComm.Enabled = (availableChannelOptions & 1) > 0;
    this.checkBoxCyclicRead.Enabled = (availableChannelOptions & ConnectionDialog.ChannelOptionsCyclicServices) > 0;
    this.checkBoxAutoExecConfiguredServices.Enabled = (availableChannelOptions & ConnectionDialog.ChannelOptionsAutoExecuteConfiguredServices) > 0;
    this.checkBoxExecuteStartComm.Checked = (channelOptions & 1) > 0;
    this.checkBoxCyclicRead.Checked = (channelOptions & ConnectionDialog.ChannelOptionsCyclicServices) > 0;
    this.checkBoxAutoExecConfiguredServices.Checked = (channelOptions & ConnectionDialog.ChannelOptionsAutoExecuteConfiguredServices) > 0;
  }

  private static void UpdateDeviceComponentInformation(Ecu ecu)
  {
    if (ecu == null)
      return;
    ComponentInformationGroups.UpdateItem(Components.GroupSupportedDevices, $"{ecu.Name}_{(object) ecu.DiagnosisSource}", ecu.ConfigurationFileVersion.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) ecu.DescriptionDataVersion, (object) ecu.ConfigurationFileVersion) : ecu.DescriptionDataVersion, true);
  }

  private void OnHelpButtonClicked(object sender, CancelEventArgs e)
  {
    e.Cancel = true;
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_ConnectionDialog"));
  }

  private void checkBoxShowAll_CheckedChanged_1(object sender, EventArgs e)
  {
    this.PopulateConnectionResources();
    SettingsManager.GlobalInstance.SetValue<bool>("ShowAllResources", nameof (ConnectionDialog), this.checkBoxShowAll.Checked, false);
  }

  private void comboBoxVehicle_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.BuildList();
    ConnectionDialog.DisplayedFamily selectedItem = this.comboBoxVehicle.SelectedItem as ConnectionDialog.DisplayedFamily;
    SettingsManager globalInstance = SettingsManager.GlobalInstance;
    string selectedCategory = this.selectedCategory;
    string str;
    if (selectedItem == null)
    {
      str = Resources.ConnectionDialog_All;
    }
    else
    {
      ElectronicsFamily family = selectedItem.Family;
      str = ((ElectronicsFamily) ref family).Name;
    }
    StringSetting stringSetting = new StringSetting($"{selectedCategory}/{str}");
    globalInstance.SetValue<StringSetting>("Family", nameof (ConnectionDialog), stringSetting, false);
  }

  private void radioButtonVehicle_CheckedChanged(object sender, EventArgs e)
  {
    this.selectedCategory = "Vehicle";
    this.tableLayoutPanelFamily.SetCellPosition((Control) this.comboBoxVehicle, new TableLayoutPanelCellPosition(1, 0));
    this.PopulateFamilyCombo();
  }

  private void radioButtonPowertrain_CheckedChanged(object sender, EventArgs e)
  {
    this.selectedCategory = "Engine";
    this.tableLayoutPanelFamily.SetCellPosition((Control) this.comboBoxVehicle, new TableLayoutPanelCellPosition(1, 1));
    this.PopulateFamilyCombo();
  }

  private void comboBoxConnectionResource_SelectedIndexChanged(object sender, EventArgs e)
  {
    ConnectionDialog.DisplayedConnectionResource selectedItem = this.comboBoxConnectionResource.SelectedItem as ConnectionDialog.DisplayedConnectionResource;
    this.buttonConnect.Enabled = selectedItem != null;
    if (selectedItem != null)
      this.textBoxDiagnosticDescription.Text = $"{Path.GetFileName(selectedItem.ConnectionResource.Ecu.DescriptionFileName)} ({selectedItem.ConnectionResource.Ecu.DescriptionDataVersion})";
    this.buttonChangeDiagnosticDescription.Enabled = selectedItem != null && string.Equals(Path.GetExtension(selectedItem.ConnectionResource.Ecu.DescriptionFileName), ".cbf", StringComparison.OrdinalIgnoreCase);
    this.PopulateTargetVariant();
    this.UpdateChannelOptions();
  }

  private void listViewExEcus_DoubleClick_1(object sender, EventArgs e)
  {
    if (!this.buttonConnect.Enabled)
      return;
    this.buttonConnect.PerformClick();
  }

  private void listViewExEcus_KeyPress(object sender, KeyPressEventArgs e)
  {
    if (e.KeyChar != '\r' && e.KeyChar != '\r')
      return;
    e.Handled = true;
    this.buttonConnect.PerformClick();
  }

  private void listViewExEcus_ColumnClick(object sender, ColumnClickEventArgs e)
  {
    if (e.Column == this.lvwColumnSorter.ColumnToSort)
    {
      this.lvwColumnSorter.OrderOfSort = this.lvwColumnSorter.OrderOfSort == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
    }
    else
    {
      this.lvwColumnSorter.ColumnToSort = e.Column;
      this.lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
    }
    this.lvwColumnSorter.SortType = e.Column == this.columnHeaderIdentifier.Index ? ListViewColumnSorter.SortBy.Tag : ListViewColumnSorter.SortBy.Text;
    ((ListView) this.listViewExEcus).Sort();
  }

  private void buttonCommunicationParameters_Click(object sender, EventArgs e)
  {
    object selectedEcu = (object) this.selectedEcu;
    ConnectionDialog.DisplayedConnectionResource displayedConnectionResource = this.comboBoxConnectionResource.SelectedItem as ConnectionDialog.DisplayedConnectionResource;
    if (displayedConnectionResource != null)
      selectedEcu = (object) displayedConnectionResource.ConnectionResource.Interface;
    ActionsMenuProxy.GlobalInstance.ShowDialog("Communication Parameters", (string) null, selectedEcu, false);
    this.PopulateConnectionResources();
    if (displayedConnectionResource == null)
      return;
    this.comboBoxConnectionResource.SelectedItem = (object) this.comboBoxConnectionResource.Items.OfType<ConnectionDialog.DisplayedConnectionResource>().FirstOrDefault<ConnectionDialog.DisplayedConnectionResource>((Func<ConnectionDialog.DisplayedConnectionResource, bool>) (dcr => dcr.ConnectionResource.ToString() == displayedConnectionResource.ConnectionResource.ToString()));
  }

  private void buttonChangeDiagnosticDescription_Click(object sender, EventArgs e)
  {
  }

  private void checkBoxAdvanced_CheckedChanged(object sender, EventArgs e)
  {
    Size preferredSize = this.tableLayoutPanelAdvanced.GetPreferredSize(new Size(this.Size.Width, this.tableLayoutPanelAdvanced.Height));
    if (this.checkBoxAdvanced.Checked)
    {
      this.tableLayoutPanelMain.SuspendLayout();
      this.Size = new Size(this.Size.Width, this.Size.Height + preferredSize.Height);
      this.BeginInvoke((Delegate) (() =>
      {
        this.tableLayoutPanelAdvanced.Visible = true;
        this.tableLayoutPanelMain.ResumeLayout();
      }));
    }
    else
    {
      this.tableLayoutPanelMain.SuspendLayout();
      this.Size = new Size(this.Size.Width, this.Size.Height - preferredSize.Height);
      this.tableLayoutPanelAdvanced.Visible = false;
      this.tableLayoutPanelMain.ResumeLayout();
    }
    this.checkBoxAdvanced.Image = this.checkBoxAdvanced.Checked ? (Image) Resources.navigate_up : (Image) Resources.navigate_down;
  }

  private void checkBoxCyclicRead_CheckedChanged(object sender, EventArgs e)
  {
    this.GetSelectedChannelOptions();
  }

  private void checkBoxAutoExecConfiguredServices_CheckedChanged(object sender, EventArgs e)
  {
    this.GetSelectedChannelOptions();
  }

  private void checkBoxExecuteStartComm_CheckedChanged(object sender, EventArgs e)
  {
    this.GetSelectedChannelOptions();
  }

  private void comboBoxTargetVariant_SelectedIndexChanged(object sender, EventArgs e)
  {
    string str = string.Empty;
    if (this.comboBoxTargetVariant.SelectedItem is DiagnosisVariant)
      str = ((DiagnosisVariant) this.comboBoxTargetVariant.SelectedItem).Name;
    if (!(this.comboBoxConnectionResource.SelectedItem is ConnectionDialog.DisplayedConnectionResource selectedItem))
      return;
    SettingsManager.GlobalInstance.SetValue<StringSetting>($"FixedVariant_{selectedItem.ConnectionResource.Ecu.Name}_{(object) selectedItem.ConnectionResource.Ecu.DiagnosisSource}", nameof (ConnectionDialog), new StringSetting(str), false);
  }

  private class DisplayedConnectionResource
  {
    private bool includeProtocol;
    private ConnectionResource connectionResource;

    public ConnectionResource ConnectionResource => this.connectionResource;

    public DisplayedConnectionResource(ConnectionResource resource, bool includeProtocol)
    {
      this.connectionResource = resource;
      this.includeProtocol = includeProtocol;
    }

    public override string ToString()
    {
      return SapiExtensions.ToDisplayString(this.ConnectionResource, this.includeProtocol);
    }
  }

  private class DisplayedFamily
  {
    public ElectronicsFamily Family { private set; get; }

    public DisplayedFamily(ElectronicsFamily family) => this.Family = family;

    public override string ToString()
    {
      ElectronicsFamily family = this.Family;
      return ((ElectronicsFamily) ref family).Name;
    }
  }
}
