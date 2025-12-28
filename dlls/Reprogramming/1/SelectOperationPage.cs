// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.SelectOperationPage
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using mshtml;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class SelectOperationPage : WizardPage
{
  private IProvideProgrammingData dataProvider;
  private const string ManualConnectionFlag = "ManualConnection";
  private SelectOperationPage.OperationType selectedOperation;
  private SelectUnitSubPanel selectUnitPanel;
  private SelectFirmwareSubPanel selectSoftwarePanel;
  private SelectDataSetSubPanel selectDataSetPanel;
  private HtmlElement titlePane;
  private HtmlElement deviceListPane;
  private HtmlElement operationPane;
  private HtmlElement settingsSubPanelPane;
  private HtmlElement softwareSubPanelPane;
  private HtmlElement datasetSubPanelPane;
  private HtmlElement manualConnectionPane;
  private HtmlElement showAllConnectionResourcesCheckbox;
  private HtmlElement connectionResourcesCombobox;
  private HtmlElement connectButton;
  private string chargeText = string.Empty;
  private Channel selectedChannel;
  private UnitInformation selectedUnitPackage;
  private string selectedUnitPackageType;
  private bool dirty;
  private string manualConnectionEcu;

  public SelectOperationPage(ReprogrammingView dataProvider, HtmlElement inputPane)
    : base(dataProvider, inputPane)
  {
    this.dataProvider = (IProvideProgrammingData) dataProvider;
    Sapi sapi = SapiManager.GlobalInstance.Sapi;
    sapi.Channels.ConnectCompleteEvent += new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
    sapi.Channels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
    SapiManager.GlobalInstance.ConnectedUnitChanged += new EventHandler(this.SapiManagerChannelChangedEvent);
    SapiManager.GlobalInstance.LogFileChannelsChanged += new EventHandler(this.SapiManagerChannelChangedEvent);
    SapiManager.GlobalInstance.ChannelIdentificationChanged += new EventHandler<ChannelIdentificationChangedEventArgs>(this.GlobalInstance_ChannelIdentificationChanged);
    LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadStatusChanged += new EventHandler(this.GlobalInstance_RemoteFileDownloadStatusChanged);
    this.SetInputPane(inputPane);
  }

  private void GlobalInstance_RemoteFileDownloadStatusChanged(object sender, EventArgs e)
  {
    if (this.Wizard.ActivePage != this)
      return;
    ReprogrammingView.UpdateTitle(this.titlePane, this.dataProvider.SelectedUnit);
  }

  internal void SetInputPane(HtmlElement inputPane)
  {
    inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("\r\n                <div>\r\n                    <div id='titlePane'>&nbsp;</div>\r\n                    <div id='deviceListPane'>&nbsp;</div>\r\n                    <div id='operationPane' class='hide'>&nbsp;</div>\r\n                    <div id='manualConnectionPane' class='hide'>\r\n                        <span><button id='connectButton' class='button blue' onClick=\"clickButton('connect')\">{0}</button></span>\r\n                        <span><select id='connectionResources'>&nbsp;</select></span>\r\n                        <span><input type='checkbox' id='showAllConnectionResources'>Show all</span>\r\n                    </div>\r\n                    <div id='unitSubPanel' class='hide'>&nbsp;</div>\r\n                    <div id='softwareSubPanel' class='hide'>&nbsp;</div>\r\n                    <div id='datasetSubPanel' class='hide'>&nbsp;</div>\r\n                    <br/><br/><br/><br/>\r\n                    <div id='buttonPanel' class='fixedbottom'>\r\n                        <button id='backButtonSOP' class='button blue' onClick=\"clickButton('back')\">{1}</button>\r\n                        <button id='nextButtonSOP' class='button gray' onClick=\"clickButton('next')\">{2}</button>\r\n                    </div>\r\n                </div>\r\n            ", (object) "Connect", (object) Resources.Wizard_ButtonBack, (object) Resources.Wizard_ButtonNext));
    foreach (HtmlElement element in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList<HtmlElement>())
    {
      switch (element.Id)
      {
        case "datasetSubPanel":
          this.selectDataSetPanel = new SelectDataSetSubPanel(this.dataProvider, element);
          this.datasetSubPanelPane = element;
          continue;
        case "deviceListPane":
          this.deviceListPane = element;
          continue;
        case "manualConnectionPane":
          this.manualConnectionPane = element;
          continue;
        case "operationPane":
          this.operationPane = element;
          continue;
        case "softwareSubPanel":
          this.selectSoftwarePanel = new SelectFirmwareSubPanel(this.dataProvider, element);
          this.softwareSubPanelPane = element;
          continue;
        case "titlePane":
          this.titlePane = element;
          continue;
        case "unitSubPanel":
          this.selectUnitPanel = new SelectUnitSubPanel(this.dataProvider, element);
          this.settingsSubPanelPane = element;
          continue;
        default:
          continue;
      }
    }
    foreach (HtmlElement htmlElement in inputPane.GetElementsByTagName("button").OfType<HtmlElement>().ToList<HtmlElement>())
    {
      switch (htmlElement.Id)
      {
        case "backButtonSOP":
          this.BackButton = htmlElement;
          continue;
        case "nextButtonSOP":
          this.NextButton = htmlElement;
          continue;
        case "connectButton":
          this.connectButton = htmlElement;
          continue;
        default:
          continue;
      }
    }
    this.showAllConnectionResourcesCheckbox = this.manualConnectionPane.GetElementsByTagName("input")[0];
    this.connectionResourcesCombobox = this.manualConnectionPane.GetElementsByTagName("select")[0];
    ReprogrammingView.SubscribeInputEvents(this.manualConnectionPane, "checkbox", "onclick", new EventHandler(this.checkBoxShowAll_CheckedChanged));
    this.selectUnitPanel.SettingSelectionChanged += new EventHandler(this.SelectUnitPanel_SettingSelectionChanged);
    this.selectSoftwarePanel.SelectedFirmwareChanged += new EventHandler(this.SelectSoftwarePanel_SelectedFirmwareChanged);
    this.selectDataSetPanel.SelectedDataOptionChanged += new EventHandler(this.SelectDataSetPanel_SelectedDataOptionChanged);
  }

  internal override void Navigate(string fragment)
  {
    if (!(fragment == "#button_connect"))
      return;
    this.ButtonConnectClick();
  }

  private void SelectUnitPanel_SettingSelectionChanged(object sender, EventArgs e)
  {
    this.SetWizardButtons();
  }

  private void SelectDataSetPanel_SelectedDataOptionChanged(object sender, EventArgs e)
  {
    this.SetWizardButtons();
  }

  private void SelectSoftwarePanel_SelectedFirmwareChanged(object sender, EventArgs e)
  {
    this.SetWizardButtons();
  }

  internal override WizardPage OnWizardNext()
  {
    try
    {
      this.GatherProgrammingData();
      ProgrammingData programmingData1 = this.dataProvider.ProgrammingData.First<ProgrammingData>();
      if ((programmingData1.Operation == ProgrammingOperation.ChangeDataSet || programmingData1.Operation == ProgrammingOperation.UpdateAndChangeDataSet) && !this.selectDataSetPanel.SelectedDataSetOption.IsCurrent)
      {
        ChargeDialog chargeDialog = new ChargeDialog();
        chargeDialog.ReferenceNumber = this.chargeText;
        if (chargeDialog.ShowDialog() == DialogResult.Cancel || string.IsNullOrEmpty(chargeDialog.ReferenceNumber))
          return (WizardPage) null;
        programmingData1.ChargeText = this.chargeText = chargeDialog.ReferenceNumber;
        programmingData1.ChargeType = "REFNUMBER";
      }
      if (programmingData1.Operation == ProgrammingOperation.Replace && !programmingData1.ReplaceToSameDevice)
      {
        List<string> source = new List<string>();
        foreach (ProgrammingData programmingData2 in this.dataProvider.ProgrammingData)
        {
          string engineSerialNumber = SapiManager.GetEngineSerialNumber(programmingData2.Channel);
          string identificationNumber = SapiManager.GetVehicleIdentificationNumber(programmingData2.Channel);
          if (engineSerialNumber != null && !string.IsNullOrEmpty(programmingData2.EngineSerialNumber) && !string.Equals(engineSerialNumber, programmingData2.EngineSerialNumber))
            source.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatChangeESN, (object) programmingData2.Channel.Ecu.Name, (object) engineSerialNumber, (object) programmingData2.EngineSerialNumber));
          if (identificationNumber != null && !string.IsNullOrEmpty(programmingData2.VehicleIdentificationNumber) && !string.Equals(identificationNumber, programmingData2.VehicleIdentificationNumber))
            source.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatChangeVIN, (object) programmingData2.Channel.Ecu.Name, (object) identificationNumber, (object) programmingData2.VehicleIdentificationNumber));
        }
        if (source.Any<string>())
        {
          StringBuilder message = new StringBuilder();
          message.AppendLine(Resources.SelectOperationPage_DifferentESNOrVINMessage);
          message.AppendLine();
          source.ForEach((Action<string>) (s => message.AppendLine(s)));
          message.AppendLine();
          message.AppendLine(Resources.SelectOperationPage_ClickOK);
          if (ControlHelpers.ShowMessageBox(message.ToString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
            return (WizardPage) null;
        }
      }
      return programmingData1.DataSource == 2 && programmingData1.EdexFileInformation.ConfigurationInformation.Upgrade && ControlHelpers.ShowMessageBox(Resources.SelectOperationPage_EdexUpgradeMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Cancel ? (WizardPage) null : base.OnWizardNext();
    }
    catch (DataException ex)
    {
      int num = (int) ControlHelpers.ShowMessageBox(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      return (WizardPage) null;
    }
  }

  internal override void OnSetActive()
  {
    this.SetWizardButtons();
    ReprogrammingView.UpdateTitle(this.titlePane, this.dataProvider.SelectedUnit);
    this.BuildDeviceListDeferred();
    this.UpdateDeviceSelection();
    Channel selectedChannel = this.SelectedChannel;
    this.selectUnitPanel.UpdateSettingsList(selectedChannel);
    if (selectedChannel != null && !SapiExtensions.IsDataSourceDepot(selectedChannel.Ecu))
      return;
    this.selectSoftwarePanel.UpdateFirmwareList(selectedChannel);
    this.selectDataSetPanel.UpdateDataSetList(selectedChannel);
  }

  private void SetWizardButtons()
  {
    if (this.Wizard.ActivePage != this)
      return;
    WizardControl.WizardButtons buttons = (WizardControl.WizardButtons) 1;
    if (this.selectedChannel != null || this.selectedUnitPackage != null)
    {
      if (this.selectedOperation == SelectOperationPage.OperationType.Replace)
      {
        if ((this.selectUnitPanel.IsResetToDefaultSelected || this.selectUnitPanel.SelectedSettings != null) && this.selectUnitPanel.SelectedUnitDataSource == 1 || this.selectUnitPanel.SelectedEdexFile != null && this.selectUnitPanel.SelectedUnitDataSource == 2)
          buttons = (WizardControl.WizardButtons) (buttons | 2);
      }
      else if (this.selectedOperation == SelectOperationPage.OperationType.Update)
      {
        if (this.selectSoftwarePanel.SelectedFirmware != null)
          buttons = (WizardControl.WizardButtons) (buttons | 2);
      }
      else if (this.selectedOperation == SelectOperationPage.OperationType.ChangeDataSet)
      {
        if (this.selectDataSetPanel.SelectedDataSetOption != null)
          buttons = (WizardControl.WizardButtons) (buttons | 2);
      }
      else if (this.SelectedUnitPackage != null)
        buttons = (WizardControl.WizardButtons) (buttons | 2);
    }
    this.Wizard.SetWizardButtons(buttons);
  }

  private void GatherProgrammingData()
  {
    if (this.SelectedUnitPackage != null && !string.IsNullOrEmpty(this.SelectedUnitPackageType))
    {
      List<ProgrammingData> programmingDataList = new List<ProgrammingData>();
      switch (this.SelectedUnitPackageType)
      {
        case "powertrain_current":
        case "powertrain_newest":
          using (IEnumerator<DeviceInformation> enumerator = this.SelectedUnitPackage.DeviceInformation.Where<DeviceInformation>((System.Func<DeviceInformation, bool>) (d => d.DataSource == 1)).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              DeviceInformation deviceInformation = enumerator.Current;
              Channel channel = SapiManager.GlobalInstance.ActiveChannels.ToList<Channel>().FirstOrDefault<Channel>((System.Func<Channel, bool>) (c => c.Ecu.Name.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase)));
              SettingsInformation settings = this.SelectedUnitPackage.SettingsInformation.FirstOrDefault<SettingsInformation>((System.Func<SettingsInformation, bool>) (s => s.SettingsType.Equals("latest", StringComparison.OrdinalIgnoreCase) && s.Device.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase))) ?? this.SelectedUnitPackage.SettingsInformation.FirstOrDefault<SettingsInformation>((System.Func<SettingsInformation, bool>) (s => s.SettingsType.Equals("oem", StringComparison.OrdinalIgnoreCase) && s.Device.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase)));
              if (channel != null && settings != null)
                programmingDataList.Add(new ProgrammingData(channel, this.SelectedUnitPackage, settings, true, this.SelectedUnitPackageType == "powertrain_newest" && deviceInformation.HardwareOptionHasNewestForHardware(SapiManager.GetHardwarePartNumber(channel))));
            }
            break;
          }
        case "vehicle_chec":
          using (IEnumerator<DeviceInformation> enumerator = this.SelectedUnitPackage.DeviceInformation.Where<DeviceInformation>((System.Func<DeviceInformation, bool>) (d => d.DataSource == 2 && d.EdexFiles.Any<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef =>
          {
            if (ef.ConfigurationInformation == null)
              return false;
            return ef.ConfigurationInformation.ChecSettings != null || ef.ConfigurationInformation.ApplicableProposedSettingItems().Any<EdexSettingItem>();
          })))).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              DeviceInformation deviceInformation = enumerator.Current;
              Channel channel = SapiManager.GlobalInstance.ActiveChannels.ToList<Channel>().FirstOrDefault<Channel>((System.Func<Channel, bool>) (c => c.Ecu.Name.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase)));
              EdexFileInformation edexFile = deviceInformation.EdexFiles.FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => ef.FileType == 3 && !ef.HasErrors && SelectOperationPage.ConfigurationIsValidForChecPackage(channel, ef.ConfigurationInformation))) ?? deviceInformation.EdexFiles.FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => ef.FileType == 2 && !ef.HasErrors && SelectOperationPage.ConfigurationIsValidForChecPackage(channel, ef.ConfigurationInformation))) ?? deviceInformation.EdexFiles.FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => deviceInformation.IsEdexFactoryConfigurationValidForProgramming && ef.FileType == 1 && !ef.HasErrors && SelectOperationPage.ConfigurationIsValidForChecPackage(channel, ef.ConfigurationInformation)));
              if (channel != null && edexFile != null)
                programmingDataList.Add(new ProgrammingData(channel, this.SelectedUnitPackage, edexFile));
            }
            break;
          }
      }
      this.dataProvider.ProgrammingData = (IEnumerable<ProgrammingData>) programmingDataList;
    }
    else
    {
      ProgrammingData element;
      if (this.selectedOperation == SelectOperationPage.OperationType.Replace)
      {
        UnitInformation selectedUnit = this.dataProvider.SelectedUnit;
        Channel selectedChannel = this.SelectedChannel;
        if (selectedUnit == null)
          throw new DataException(Resources.SelectOperationPage_ErrorSelectedUnitNull);
        if (selectedChannel == null)
          throw new DataException(Resources.SelectOperationPage_ErrorSelectedChannelNull);
        DeviceInformation.DeviceDataSource selectedUnitDataSource = this.selectUnitPanel.SelectedUnitDataSource;
        if (selectedUnitDataSource != 1)
        {
          if (selectedUnitDataSource != 2)
            throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatErrorSelectedDataSourceUnknown, (object) selectedChannel.Ecu.Name, (object) selectedUnit.IdentityKey));
          element = new ProgrammingData(selectedChannel, selectedUnit, this.selectUnitPanel.SelectedEdexFile);
        }
        else
          element = new ProgrammingData(selectedChannel, selectedUnit, this.selectUnitPanel.SelectedSettings, false, false);
      }
      else if (this.selectedOperation == SelectOperationPage.OperationType.Update)
      {
        Channel selectedChannel = this.SelectedChannel;
        FirmwareInformation firmwareInfo = this.selectSoftwarePanel.SelectedFirmware;
        DeviceInformation informationForDevice = this.selectSoftwarePanel.Unit.GetInformationForDevice(this.selectedChannel.Ecu.Name);
        FirmwareOptionInformation optionInformation = informationForDevice != null ? informationForDevice.FirmwareOptions.FirstOrDefault<FirmwareOptionInformation>((System.Func<FirmwareOptionInformation, bool>) (x => x.Version == firmwareInfo.Version)) : (FirmwareOptionInformation) null;
        element = optionInformation == null ? new ProgrammingData(selectedChannel, firmwareInfo, (string) null, (string) null, this.selectSoftwarePanel.Unit, (DataSetOptionInformation) null) : new ProgrammingData(selectedChannel, firmwareInfo, optionInformation.BootLoaderKey, optionInformation.ControlListKey, this.selectSoftwarePanel.Unit, (DataSetOptionInformation) null);
      }
      else
      {
        if (this.selectedOperation != SelectOperationPage.OperationType.ChangeDataSet)
          throw new DataException(Resources.SelectOperationPage_ErrorNoOperation);
        element = ProgrammingData.CreateFromRequiredDatasetKey(this.SelectedChannel, this.selectDataSetPanel.Unit, this.selectDataSetPanel.SelectedDataSetOption.Key);
        if (!this.selectDataSetPanel.SelectedDataSetOption.IsCurrent)
        {
          DeviceInformation informationForDevice = this.selectDataSetPanel.Unit.GetInformationForDevice(this.SelectedChannel.Ecu.Name);
          if (informationForDevice != null && informationForDevice.FirmwareOptions.Count > 1)
          {
            int num = (int) ControlHelpers.ShowMessageBox(Resources.SelectOperationPage_Message_ChangingTheDataSetWillRequireTheDataForThisDeviceToNeedToBeRedownloadedFromTheServerIfYourIntentIsToSubsequentlyUpgradeTheSoftwareInThisDevicePleaseUpgradeTheSoftwareFirst, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
          }
        }
      }
      this.dataProvider.ProgrammingData = Enumerable.Repeat<ProgrammingData>(element, 1);
      if ((element.DataSource == 1 && element.Firmware != null || element.DataSource == 2 && element.EdexFileInformation.ConfigurationInformation != null) && this.SelectedChannel != null && !SapiManager.GlobalInstance.IgnoreSoftwareCompatibilityChecks(this.SelectedChannel) && this.dataProvider.RequiresCompatibilityChecks)
      {
        DeviceInformation.DeviceDataSource dataSource = element.DataSource;
        if (dataSource != 1)
        {
          if (dataSource == 2 && ServerDataManager.GlobalInstance.EdexCompatibilityTable.IsCurrentSoftwareCompatible())
          {
            EdexCompatibilityEcuItem targetEcuItem = new EdexCompatibilityEcuItem(element.EdexFileInformation.ConfigurationInformation.DeviceName, element.EdexFileInformation.ConfigurationInformation.HardwarePartNumber, element.EdexFileInformation.ConfigurationInformation.FlashwarePartNumber);
            if (!ServerDataManager.GlobalInstance.EdexCompatibilityTable.IsCompatibleWithCurrentSoftware(targetEcuItem))
              SelectOperationPage.InformUserOfIncompatibility(targetEcuItem, element.Unit);
          }
        }
        else if (ServerDataManager.GlobalInstance.CompatibilityTable.IsCurrentSoftwareCompatible())
        {
          Software targetSoftware = new Software(element.Firmware.Device, element.Firmware.Version, SapiManager.GetHardwarePartNumber(this.SelectedChannel));
          if (!element.Unit.UnitFixedAtTest && !ServerDataManager.GlobalInstance.CompatibilityTable.IsCompatibleWithCurrentSoftware(targetSoftware) && !ServerDataManager.GlobalInstance.CompatibilityTable.IsAlwaysCompatible(targetSoftware))
            SelectOperationPage.InformUserOfIncompatibility(targetSoftware, element.Unit);
        }
      }
    }
    List<string> values = new List<string>();
    foreach (ProgrammingData programmingData in this.dataProvider.ProgrammingData)
    {
      ProgrammingData data = programmingData;
      if ((data.DataSource == 1 && data.Firmware != null || data.DataSource == 2 && data.EdexFileInformation.ConfigurationInformation != null) && data.GetFlashMeanings(ProgrammingData.FlashBlock.Firmware) == null && (data.DataSource == 1 || !data.TargetChannelHasSameFirmwareVersion || data.FlashRequiredSameFirmwareVersion))
      {
        if (!string.IsNullOrEmpty(data.Firmware.Reference) && (data.Firmware.RequiresDownload || data.Firmware.Status != "OK"))
          throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, data.Firmware.RequiresDownload ? Resources.SelectOperationPage_FormatFirmwareFileStillToBeDownloaded : Resources.SelectOperationPage_FormatFirmwareFileNotAvailableDueToServerStatus, (object) data.Firmware.Key, (object) data.Firmware.Status));
        if (!File.Exists(Path.Combine(Directories.GetDatabasePathForExtension(Path.GetExtension(data.Firmware.FileName)), data.Firmware.FileName)))
          throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatFirmwareFileMissing, (object) data.Firmware.Version, (object) data.Firmware.FileName));
        if (ProgrammingData.GetDiagnosisSourceForFlashware(data.Firmware.FileName) == data.Channel.Ecu.DiagnosisSource)
          throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatMeaningCannotBeProgrammed, (object) data.Firmware.Version));
      }
      Tuple<ProgrammingStep, DiagnosisSource> tuple = data.RequiredDiagnosisSources.FirstOrDefault<Tuple<ProgrammingStep, DiagnosisSource>>((System.Func<Tuple<ProgrammingStep, DiagnosisSource>, bool>) (rds => rds.Item2 != data.Channel.Ecu.DiagnosisSource));
      if (tuple != null)
      {
        ConnectionResource connectionResource = data.GetTargetConnectionResource(tuple.Item2);
        if (connectionResource != null)
        {
          if ((int) connectionResource.Type[0] != (int) SapiExtensions.GetActiveConnectionResource(data.Channel).Type[0])
            values.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, connectionResource.Type[0] == 'C' ? Resources.SelectOperationPageFormat_PhysicalCANWillBeNeeded : Resources.SelectOperationPageFormat_PhysicalEthernetWillBeNeeded, (object) data.Channel.Ecu.Name, (object) connectionResource.HardwareName));
        }
        else
        {
          if (data.Channel.Ecu == null && tuple.Item2 == DiagnosisSource.McdDatabase)
            throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Error acquiring connection resource for target diagnosis source {0}.{1} Verify that the SMR-D exists and is valid.", (object) tuple.Item2, (object) Environment.NewLine));
          throw new DataException("Error acquiring connection resource for target diagnosis source " + (object) tuple.Item2);
        }
      }
    }
    if (values.Count <= 0)
      return;
    int num1 = (int) ControlHelpers.ShowMessageBox(string.Join(Environment.NewLine, (IEnumerable<string>) values), MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
  }

  private static void InformUserOfIncompatibility(Software targetSoftware, UnitInformation unit)
  {
    StringBuilder stringBuilder = new StringBuilder();
    CompatibleSoftwareCollection compatibleList = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleList(targetSoftware);
    if (((ReadOnlyCollection<SoftwareCollection>) compatibleList).Count > 0)
    {
      CompatibleSoftwareCollection softwareCollection1 = compatibleList.FilterForUnit(unit);
      if (((ReadOnlyCollection<SoftwareCollection>) softwareCollection1).Count > 0)
      {
        CompatibleSoftwareCollection softwareCollection2 = softwareCollection1.FilterForOnlineOptions(targetSoftware.Ecu);
        CompatibilityWarningDialog compatibilityWarningDialog = new CompatibilityWarningDialog(Resources.SelectOperationPage_SoftwareIsNotCompatible_Information, targetSoftware, Resources.SelectOperationPage_Compat_TargetDevice);
        compatibilityWarningDialog.AddCompatibilityInfo(softwareCollection2, Resources.ProgramDevicePage_Compat_CompatibleSetFormat);
        int num = (int) ((Form) compatibilityWarningDialog).ShowDialog();
      }
      else
      {
        MessageBoxIcon messageBoxIcon = MessageBoxIcon.Exclamation;
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_PowertrainSoftwareIsNotCompatible_Warning, (object) targetSoftware);
        int num = (int) ControlHelpers.ShowMessageBox(stringBuilder.ToString(), MessageBoxButtons.OK, messageBoxIcon, MessageBoxDefaultButton.Button1);
      }
    }
    else
    {
      MessageBoxIcon messageBoxIcon = MessageBoxIcon.Hand;
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_PowertrainSoftwareIsNotCompatible_Stop, (object) targetSoftware);
      int num = (int) ControlHelpers.ShowMessageBox(stringBuilder.ToString(), MessageBoxButtons.OK, messageBoxIcon, MessageBoxDefaultButton.Button1);
    }
  }

  private static void InformUserOfIncompatibility(
    EdexCompatibilityEcuItem targetEcuItem,
    UnitInformation unit)
  {
    EdexCompatibilityConfigurationCollection compatibleList = ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(targetEcuItem);
    if (((ReadOnlyCollection<EdexCompatibilityConfiguration>) compatibleList).Count > 0)
    {
      EdexCompatibilityConfigurationCollection source = compatibleList.FilterForOnlineDevices(targetEcuItem).FilterForUnit(unit);
      if (((IEnumerable<EdexCompatibilityConfiguration>) source).Any<EdexCompatibilityConfiguration>((System.Func<EdexCompatibilityConfiguration, bool>) (cs => cs.Ecus.Any<EdexCompatibilityEcuItem>())))
      {
        CompatibilityWarningDialog compatibilityWarningDialog = new CompatibilityWarningDialog(Resources.SelectOperationPage_SoftwareIsNotCompatible_Information, targetEcuItem, Resources.SelectOperationPage_Compat_TargetDevice);
        compatibilityWarningDialog.AddCompatibilityInfo(source, Resources.ProgramDevicePage_Compat_CompatibleSetFormat);
        int num = (int) ((Form) compatibilityWarningDialog).ShowDialog();
      }
      else
      {
        IEnumerable<EdexCompatibilityEcuItem> compatibilityEcuItems;
        int num = (int) ControlHelpers.ShowMessageBox(((IEnumerable<EdexCompatibilityConfiguration>) ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(unit, ref compatibilityEcuItems)).Any<EdexCompatibilityConfiguration>() || !compatibilityEcuItems.Any<EdexCompatibilityEcuItem>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_Warning, (object) targetEcuItem) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_WarningWithHint, (object) targetEcuItem, (object) string.Join<EdexCompatibilityEcuItem>(Environment.NewLine, compatibilityEcuItems)), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
      }
    }
    else
    {
      int num1 = (int) ControlHelpers.ShowMessageBox(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_Stop, (object) targetEcuItem), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }
  }

  private void SapiManagerChannelChangedEvent(object sender, EventArgs e)
  {
    if (this.Wizard.ActivePage != this)
      return;
    this.BuildDeviceList();
  }

  private void GlobalInstance_ChannelIdentificationChanged(
    object sender,
    ChannelIdentificationChangedEventArgs e)
  {
    if (this.Wizard.ActivePage != this)
      return;
    this.BuildDeviceList();
  }

  private void OnDisconnectCompleteEvent(object sender, EventArgs e)
  {
    if (this.Wizard.ActivePage != this || !(sender is Channel))
      return;
    this.BuildDeviceList();
  }

  private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender is Channel channel))
      return;
    channel.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.c_EcuInfosReadCompleteEvent);
  }

  private void c_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.Wizard.ActivePage != this)
      return;
    this.BuildDeviceList();
  }

  private Channel SelectedChannel => this.selectedChannel;

  private UnitInformation SelectedUnitPackage => this.selectedUnitPackage;

  private string SelectedUnitPackageType => this.selectedUnitPackageType;

  private void BuildDeviceList()
  {
    if (this.dirty)
      return;
    this.dirty = true;
    ((Control) this.Wizard).BeginInvoke((Delegate) (() =>
    {
      this.dirty = false;
      this.BuildDeviceListDeferred();
    }));
  }

  private void BuildDeviceListDeferred()
  {
    bool flag1 = false;
    bool foundSelectedPackage1 = false;
    List<string> listItems = new List<string>();
    if (SapiManager.GlobalInstance.Sapi != null && SapiManager.GlobalInstance.ActiveChannels.Any<Channel>())
    {
      UnitInformation selectedUnit = this.dataProvider.SelectedUnit;
      bool foundSelectedPackage2 = this.AddDepotPackageListItems(selectedUnit, listItems, foundSelectedPackage1);
      foundSelectedPackage1 = this.AddEdexPackageListItems(selectedUnit, listItems, foundSelectedPackage2);
      foreach (Channel channel in (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels.OrderBy<Channel, int>((System.Func<Channel, int>) (ch => ch.Ecu.Priority)))
      {
        Ecu channelEcu = (Ecu) null;
        bool flag2 = false;
        if (channel.IsRollCall && (channelEcu = SapiExtensions.GetSuppressedOfflineRelatedEcu(channel)) != null && !SapiManager.GlobalInstance.ActiveChannels.Any<Channel>((System.Func<Channel, bool>) (ac => ac.Ecu == channelEcu)))
        {
          flag2 = true;
        }
        else
        {
          channelEcu = channel.Ecu;
          if (channel.DiagnosisVariant.IsBase && SapiExtensions.IsDataSourceDepot(channel.Ecu) || !SapiManager.SupportsReprogramming(channel.Ecu))
            continue;
        }
        if ((ApplicationInformation.CanReprogramEdexUnits || !SapiExtensions.IsDataSourceEdex(channelEcu) || ApplicationInformation.CanReprogramEdexEcu(channelEcu.Name)) && (SapiExtensions.IsDataSourceDepot(channelEcu) || SapiExtensions.IsDataSourceEdex(channelEcu)))
        {
          if (flag2)
          {
            listItems.Add(ReprogrammingView.CreateInput("deviceList", "manual_" + channelEcu.Name, $"{channelEcu.Name} - {channelEcu.ShortDescription}", Resources.SelectOperationPageItem_ManualConnection, channelEcu.Name == this.manualConnectionEcu, false));
            if (channelEcu.Name == this.manualConnectionEcu)
              flag1 = true;
          }
          else
          {
            bool disabled = false;
            string status = string.Empty;
            UnitInformation ui = this.dataProvider.SelectedUnit;
            if (ui != null)
            {
              string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
              Part part = hardwarePartNumber != null ? new Part(hardwarePartNumber) : (Part) null;
              DeviceInformation informationForDevice = ui.GetInformationForDevice(channelEcu.Name);
              if (informationForDevice != null && (ui.HasSettingsForDevice(channelEcu.Name) || ui.GetPresetSettingsForDevice(channelEcu.Name) != null || SapiManager.GetBootModeStatus(channel)))
              {
                if (!SapiExtensions.IsDataSourceEdex(channelEcu) && ui.GetInformationForDevice(channelEcu.Name).FirmwareOptions.Any<FirmwareOptionInformation>() && !informationForDevice.FirmwareOptionSupportsHardware(part))
                {
                  disabled = true;
                  status = Resources.SelectUnitSubPanelItem_NoUnitDataForConnectedHardware;
                }
                else
                  status = ui.GetStatusDisplayTextForDevice(channelEcu.Name, part);
              }
              else
              {
                disabled = true;
                DeviceInformation.DeviceDataSource dataSource = SapiExtensions.IsDataSourceDepot(channelEcu) ? (DeviceInformation.DeviceDataSource) 1 : (DeviceInformation.DeviceDataSource) 2;
                if (informationForDevice != null)
                  status = ui.GetStatusDisplayTextForDevice(channelEcu.Name, part);
                else if (!SapiExtensions.IsDataSourceDepot(channelEcu) && !SapiExtensions.IsDataSourceEdex(channelEcu))
                  status = Resources.SelectOperationPage_DeviceWarning_NotProgrammable;
                else if (!ui.DeviceInformation.Any<DeviceInformation>((System.Func<DeviceInformation, bool>) (deviceInfo => deviceInfo.DataSource == dataSource)))
                  status = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPage_DeviceWarning_FormatNoContentForDataSource, (object) dataSource);
                else if (ui.MissingDevices.Contains<string>(channelEcu.Name))
                {
                  status = Resources.SelectOperationPage_DeviceWarning_NoServerData;
                }
                else
                {
                  DeviceInformation deviceInformation = SapiManager.GlobalInstance.Sapi.Ecus.Where<Ecu>((System.Func<Ecu, bool>) (e => e.Identifier == channelEcu.Identifier)).Select<Ecu, DeviceInformation>((System.Func<Ecu, DeviceInformation>) (e => ui.GetInformationForDevice(e.Name))).FirstOrDefault<DeviceInformation>((System.Func<DeviceInformation, bool>) (altDeviceInfo => altDeviceInfo != null));
                  if (deviceInformation != null)
                  {
                    status = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ConnectedDeviceNotAMatch, (object) deviceInformation.Device);
                  }
                  else
                  {
                    IEnumerable<ElectronicsFamily> source = ui.PossibleFamilies.Where<ElectronicsFamily>((System.Func<ElectronicsFamily, bool>) (pf => SapiManager.IsNotProgrammableForFamily(channelEcu, pf)));
                    status = !source.Any<ElectronicsFamily>() ? Resources.SelectOperationPage_DeviceWarning_NoOptionalServerData : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_NotProgrammableForFamily, (object) string.Join(", ", source.Select<ElectronicsFamily, string>((System.Func<ElectronicsFamily, string>) (f => ((ElectronicsFamily) ref f).Name))));
                  }
                }
              }
            }
            listItems.Add(ReprogrammingView.CreateInput("deviceList", channelEcu.Name, $"{channelEcu.Name} - {channel.Ecu.ShortDescription}", status, !disabled && (channel == this.selectedChannel || channelEcu.Name == this.manualConnectionEcu), disabled));
            if (!disabled && (channel == this.selectedChannel || channelEcu.Name == this.manualConnectionEcu))
            {
              flag1 = true;
              if (this.selectedChannel == null)
              {
                this.selectedChannel = channel;
                this.manualConnectionEcu = (string) null;
              }
            }
          }
        }
      }
    }
    if (listItems.Count > 0)
      this.deviceListPane.InnerHtml = ReprogrammingView.CreateRadioList(Resources.SelectOperationPage_SelectDevice, listItems);
    else
      this.deviceListPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'>{0}</span></p>", (object) Resources.SelectOperationPageItem_NoDevicesAreConnected));
    if (ReprogrammingView.SubscribeInputEvents(this.deviceListPane, "radio", "onchange", new EventHandler(this.deviceList_onChange)))
      return;
    if (!flag1)
    {
      this.selectedChannel = (Channel) null;
      this.manualConnectionEcu = (string) null;
    }
    if (!foundSelectedPackage1)
      this.selectedUnitPackage = (UnitInformation) null;
    this.UpdateDeviceSelection();
  }

  private bool AddDepotPackageListItems(
    UnitInformation connectedUnit,
    List<string> listItems,
    bool foundSelectedPackage)
  {
    if (connectedUnit != null && connectedUnit.GetStatusForDataSource((DeviceInformation.DeviceDataSource) 1) == 2 && connectedUnit.DeviceInformation.Any<DeviceInformation>((System.Func<DeviceInformation, bool>) (di => di.DataSource == 1)) && SapiManager.GlobalInstance.ActiveChannels.Any<Channel>((System.Func<Channel, bool>) (c => SapiExtensions.IsDataSourceDepot(c.Ecu))))
    {
      List<string> reasonUnitCantBeAdded = new List<string>();
      bool flag1 = true;
      bool flag2 = false;
      Dictionary<DeviceInformation, Channel> dictionary = connectedUnit.DeviceInformation.Where<DeviceInformation>((System.Func<DeviceInformation, bool>) (d => d.DataSource == 1)).ToDictionary<DeviceInformation, DeviceInformation, Channel>((System.Func<DeviceInformation, DeviceInformation>) (k => k), (System.Func<DeviceInformation, Channel>) (v => SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((System.Func<Channel, bool>) (c => c.Ecu.Name == v.Device))));
      Collection<Software> collection1 = new Collection<Software>();
      Collection<Software> collection2 = new Collection<Software>();
      if (dictionary.All<KeyValuePair<DeviceInformation, Channel>>((System.Func<KeyValuePair<DeviceInformation, Channel>, bool>) (kv => kv.Value != null)))
      {
        foreach (DeviceInformation key in dictionary.Keys)
        {
          DeviceInformation deviceInformation = key;
          bool useNewest = false;
          Channel channel = dictionary[deviceInformation];
          string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
          if (string.IsNullOrEmpty(hardwarePartNumber))
          {
            reasonUnitCantBeAdded.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_HardwarePartNumberInvalid, (object) deviceInformation.Device));
          }
          else
          {
            Part part = new Part(hardwarePartNumber);
            if (!deviceInformation.FirmwareOptionSupportsHardware(part))
              reasonUnitCantBeAdded.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ConnectedHardwareNotAMatch, (object) deviceInformation.Device, (object) part));
            else if (connectedUnit.SettingsInformation.FirstOrDefault<SettingsInformation>((System.Func<SettingsInformation, bool>) (s => s.SettingsType.Equals("latest", StringComparison.OrdinalIgnoreCase) && s.Device.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase))) == null && connectedUnit.SettingsInformation.FirstOrDefault<SettingsInformation>((System.Func<SettingsInformation, bool>) (s => s.SettingsType.Equals("oem", StringComparison.OrdinalIgnoreCase) && s.Device.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase))) == null)
              reasonUnitCantBeAdded.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_OEMLatestDataNotAvailable, (object) connectedUnit.VehicleIdentity));
            flag1 &= deviceInformation.HardwareOptionHasCurrentForHardware(hardwarePartNumber);
            useNewest = deviceInformation.HardwareOptionHasNewestForHardware(hardwarePartNumber);
          }
          if (reasonUnitCantBeAdded.Count == 0)
          {
            if (flag1)
            {
              try
              {
                ProgrammingData programmingData = new ProgrammingData(channel, connectedUnit, (SettingsInformation) null, true, false, true);
                if (programmingData.Firmware != null)
                {
                  if (programmingData.Firmware.Version != null)
                    collection1.Add(new Software(deviceInformation.Device, programmingData.Firmware.Version));
                }
              }
              catch (DataException ex)
              {
                StatusLog.Add(new StatusMessage(ex.Message, (StatusMessageType) 2, (object) this));
                flag1 = false;
              }
            }
            if (flag1 | useNewest)
            {
              try
              {
                ProgrammingData programmingData = new ProgrammingData(channel, connectedUnit, (SettingsInformation) null, true, useNewest, true);
                if (programmingData.Firmware != null)
                {
                  if (programmingData.Firmware.Version != null)
                    collection2.Add(new Software(deviceInformation.Device, programmingData.Firmware.Version));
                }
              }
              catch (DataException ex)
              {
                StatusLog.Add(new StatusMessage(ex.Message, (StatusMessageType) 2, (object) this));
                useNewest = false;
              }
            }
          }
          flag2 |= useNewest;
        }
      }
      else
        reasonUnitCantBeAdded.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_RequiredDeviceNotConnected, (object) string.Join(", ", dictionary.Where<KeyValuePair<DeviceInformation, Channel>>((System.Func<KeyValuePair<DeviceInformation, Channel>, bool>) (kv => kv.Value == null)).Select<KeyValuePair<DeviceInformation, Channel>, string>((System.Func<KeyValuePair<DeviceInformation, Channel>, string>) (kv => kv.Key.Device)))));
      if (reasonUnitCantBeAdded.Count == 0 && !flag1)
        foundSelectedPackage = this.AddDepotPackageListItem(connectedUnit, listItems, (foundSelectedPackage ? 1 : 0) != 0, new List<string>()
        {
          Resources.SelectOperationPage_CurrentSoftwareNotFoundForHardware
        }, false);
      else if (reasonUnitCantBeAdded.Count == 0 && !ServerDataManager.GlobalInstance.CompatibilityTable.IsCompatible(new SoftwareCollection(collection1)))
        foundSelectedPackage = this.AddDepotPackageListItem(connectedUnit, listItems, (foundSelectedPackage ? 1 : 0) != 0, new List<string>()
        {
          Resources.SelectOperationPage_NoCompatibleSoftwareForHardware
        }, false);
      else
        foundSelectedPackage = this.AddDepotPackageListItem(connectedUnit, listItems, foundSelectedPackage, reasonUnitCantBeAdded, false);
      if (flag2 && reasonUnitCantBeAdded.Count == 0 && collection2.Count == dictionary.Count && ServerDataManager.GlobalInstance.CompatibilityTable.IsCompatible(new SoftwareCollection(collection2)))
        foundSelectedPackage = this.AddDepotPackageListItem(connectedUnit, listItems, foundSelectedPackage, reasonUnitCantBeAdded, true);
    }
    return foundSelectedPackage;
  }

  private bool AddDepotPackageListItem(
    UnitInformation connectedUnit,
    List<string> listItems,
    bool foundSelectedPackage,
    List<string> reasonUnitCantBeAdded,
    bool newest)
  {
    if (reasonUnitCantBeAdded.Count == 0)
    {
      bool selected = this.SelectedUnitPackageType == (newest ? "powertrain_newest" : "powertrain_current") && this.SelectedUnitPackage == connectedUnit;
      listItems.Add(ReprogrammingView.CreateInput("deviceList", newest ? "powertrain_newest" : "powertrain_current", newest ? Resources.SelectOperationPage_NewestConnectedPowertrain : Resources.SelectOperationPage_CurrentConnectedPowertrain, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_Unit, (object) connectedUnit.IdentityKey), selected, false));
      if (selected)
        foundSelectedPackage = true;
    }
    else
      listItems.Add(ReprogrammingView.CreateInput("deviceList", newest ? "powertrain_newest" : "powertrain_current", newest ? Resources.SelectOperationPage_NewestConnectedPowertrain : Resources.SelectOperationPage_CurrentConnectedPowertrain, string.Join(" <br/> ", (IEnumerable<string>) reasonUnitCantBeAdded), false, true));
    return foundSelectedPackage;
  }

  private bool AddEdexPackageListItems(
    UnitInformation connectedUnit,
    List<string> listItems,
    bool foundSelectedPackage)
  {
    if (connectedUnit != null && connectedUnit.GetStatusForDataSource((DeviceInformation.DeviceDataSource) 2) == 2 && connectedUnit.EdexFileServerErrors.Count == 0 && connectedUnit.DeviceInformation.Any<DeviceInformation>((System.Func<DeviceInformation, bool>) (di => di.DataSource == 2)))
    {
      Dictionary<DeviceInformation, Channel> dictionary = connectedUnit.DeviceInformation.Where<DeviceInformation>((System.Func<DeviceInformation, bool>) (di =>
      {
        if (di.DataSource != 2)
          return false;
        return di.EdexFiles.FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => ef.ConfigurationInformation?.ChecSettings != null)) != null || di.EdexFiles.FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => ef.ConfigurationInformation != null && ef.ConfigurationInformation.ApplicableProposedSettingItems().Any<EdexSettingItem>())) != null;
      })).ToDictionary<DeviceInformation, DeviceInformation, Channel>((System.Func<DeviceInformation, DeviceInformation>) (k => k), (System.Func<DeviceInformation, Channel>) (v => SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((System.Func<Channel, bool>) (c => c.Ecu.Name == v.Device))));
      if (dictionary.Count > 0)
      {
        List<string> values = new List<string>();
        if (dictionary.All<KeyValuePair<DeviceInformation, Channel>>((System.Func<KeyValuePair<DeviceInformation, Channel>, bool>) (kv => kv.Value != null)))
        {
          foreach (KeyValuePair<DeviceInformation, Channel> keyValuePair in dictionary)
          {
            KeyValuePair<DeviceInformation, Channel> deviceChannel = keyValuePair;
            if ((deviceChannel.Key.EdexFiles.FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => ef.FileType == 3 && !ef.HasErrors && SelectOperationPage.ConfigurationIsValidForChecPackage(deviceChannel.Value, ef.ConfigurationInformation))) ?? deviceChannel.Key.EdexFiles.FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => ef.FileType == 2 && !ef.HasErrors && SelectOperationPage.ConfigurationIsValidForChecPackage(deviceChannel.Value, ef.ConfigurationInformation))) ?? deviceChannel.Key.EdexFiles.FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => deviceChannel.Key.IsEdexFactoryConfigurationValidForProgramming && ef.FileType == 1 && !ef.HasErrors && SelectOperationPage.ConfigurationIsValidForChecPackage(deviceChannel.Value, ef.ConfigurationInformation)))) == null)
              values.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ConfigurationDataNotConsistent, (object) deviceChannel.Value.Ecu.DisplayName));
          }
        }
        else
          values.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_RequiredDeviceNotConnected, (object) string.Join(", ", dictionary.Where<KeyValuePair<DeviceInformation, Channel>>((System.Func<KeyValuePair<DeviceInformation, Channel>, bool>) (kv => kv.Value == null)).Select<KeyValuePair<DeviceInformation, Channel>, string>((System.Func<KeyValuePair<DeviceInformation, Channel>, string>) (kv => kv.Key.Device)))));
        if (values.Count == 0 && connectedUnit != null)
        {
          bool selected = this.SelectedUnitPackageType == "vehicle_chec" && this.SelectedUnitPackage == connectedUnit;
          listItems.Add(ReprogrammingView.CreateInput("deviceList", "vehicle_chec", "CHEC", string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_SettingsForChecConnectedDevices, (object) connectedUnit.IdentityKey), selected, false));
          if (selected)
            foundSelectedPackage = true;
        }
        else
          listItems.Add(ReprogrammingView.CreateInput("deviceList", "vehicle_chec", Resources.SelectOperationPage_Chec, string.Join(" <br/> ", (IEnumerable<string>) values), false, true));
      }
      else
        StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unit {0} does not contain CHEC data for package programming", (object) connectedUnit.IdentityKey), (StatusMessageType) 2, (object) this));
    }
    return foundSelectedPackage;
  }

  private static bool ConfigurationIsValidForChecPackage(
    Channel channel,
    EdexConfigurationInformation configurationInformation)
  {
    return configurationInformation != null && (configurationInformation.ChecSettings != null || configurationInformation.ApplicableProposedSettingItems().Any<EdexSettingItem>()) && PartExtensions.IsEqual(configurationInformation.HardwarePartNumber, SapiManager.GetHardwarePartNumber(channel)) && configurationInformation.AllFlashwarePartNumbers.Any<Part>((System.Func<Part, bool>) (fp => PartExtensions.IsEqual(fp, SapiManager.GetSoftwarePartNumber(channel))));
  }

  private void UpdateSubPanel()
  {
    this.settingsSubPanelPane.SetAttribute("className", this.selectedOperation == SelectOperationPage.OperationType.Replace ? "show" : "hide");
    this.softwareSubPanelPane.SetAttribute("className", this.selectedOperation == SelectOperationPage.OperationType.Update ? "show" : "hide");
    this.datasetSubPanelPane.SetAttribute("className", this.selectedOperation == SelectOperationPage.OperationType.ChangeDataSet ? "show" : "hide");
    this.SetWizardButtons();
  }

  private void UpdateDeviceSelection()
  {
    string str = "&nbsp;";
    Channel selectedChannel = this.SelectedChannel;
    if (selectedChannel != null)
    {
      bool flag1 = true;
      bool flag2 = true;
      bool flag3 = true;
      if (this.GetDeviceDataSource(selectedChannel) == 2)
      {
        flag2 = false;
        flag3 = false;
        this.selectedOperation = SelectOperationPage.OperationType.Replace;
      }
      else
      {
        if (SapiManager.GetBootModeStatus(selectedChannel))
        {
          flag2 = false;
          if (ProgrammingData.GetPreviousUpgradeSettingsFile(selectedChannel.Ecu.Name) == null)
            flag3 = false;
        }
        else if (!this.HasDataSet(selectedChannel))
          flag2 = false;
        if (!flag2 && this.selectedOperation == SelectOperationPage.OperationType.ChangeDataSet)
          this.selectedOperation = SelectOperationPage.OperationType.Replace;
        if (!flag1 && this.selectedOperation == SelectOperationPage.OperationType.Replace)
          this.selectedOperation = SelectOperationPage.OperationType.Update;
        if (!flag3 && this.selectedOperation == SelectOperationPage.OperationType.Update)
          this.selectedOperation = SelectOperationPage.OperationType.Replace;
      }
      List<string> values = new List<string>();
      if (flag1)
        values.Add(ReprogrammingView.CreateInput("operation", "Replace", Resources.ProgrammingOperation_ReplaceDeviceSettingsWithServerConfiguration, (string) null, this.selectedOperation == SelectOperationPage.OperationType.Replace, false));
      if (flag3)
        values.Add(ReprogrammingView.CreateInput("operation", "Update", Resources.ProgrammingOperation_UpdateDeviceSoftware, (string) null, this.selectedOperation == SelectOperationPage.OperationType.Update, false));
      if (flag2)
        values.Add(ReprogrammingView.CreateInput("operation", "ChangeDataSet", Resources.ProgrammingOperation_ChangeDataset, (string) null, this.selectedOperation == SelectOperationPage.OperationType.ChangeDataSet, false));
      str = FormattableString.Invariant(FormattableStringFactory.Create("<p><div>{0}</div>{1}</p>", (object) Resources.SelectOperationPage_SelectOperation, (object) string.Join(" ", (IEnumerable<string>) values)));
      this.selectUnitPanel.UpdateSettingsList(selectedChannel);
      if (SapiExtensions.IsDataSourceDepot(selectedChannel.Ecu))
      {
        this.selectDataSetPanel.UpdateDataSetList(selectedChannel);
        this.selectSoftwarePanel.UpdateFirmwareList(selectedChannel);
      }
      this.operationPane.SetAttribute("className", "show");
    }
    else
    {
      this.selectedOperation = SelectOperationPage.OperationType.None;
      this.operationPane.SetAttribute("className", "hide");
    }
    this.operationPane.InnerHtml = str;
    ReprogrammingView.SubscribeInputEvents(this.operationPane, "radio", "onchange", new EventHandler(this.operation_onChange));
    this.manualConnectionPane.SetAttribute("className", this.manualConnectionEcu != null ? "show" : "hide");
    if (this.manualConnectionEcu != null)
    {
      ReprogrammingView.SetButtonState(this.connectButton, true, true);
      ReprogrammingView.EnableElement(this.connectionResourcesCombobox, true);
      this.PopulateConnectionResources();
    }
    this.UpdateSubPanel();
    this.SetWizardButtons();
  }

  private DeviceInformation.DeviceDataSource GetDeviceDataSource(Channel channel)
  {
    DeviceInformation deviceInformation1 = (DeviceInformation) null;
    DeviceInformation.DeviceDataSource deviceDataSource = (DeviceInformation.DeviceDataSource) 0;
    if (channel != null)
    {
      UnitInformation selectedUnit = this.dataProvider.SelectedUnit;
      DeviceInformation deviceInformation2;
      if (selectedUnit == null)
        deviceInformation2 = (DeviceInformation) null;
      else
        deviceInformation1 = deviceInformation2 = selectedUnit.GetInformationForDevice(channel.Ecu.Name);
      DeviceInformation deviceInformation3 = deviceInformation2;
      if (deviceInformation3 != null)
        deviceDataSource = deviceInformation3.DataSource;
      else if (channel.Ecu != null)
      {
        if (SapiExtensions.IsDataSourceEdex(channel.Ecu))
          deviceDataSource = (DeviceInformation.DeviceDataSource) 2;
        else if (SapiExtensions.IsDataSourceDepot(channel.Ecu))
          deviceDataSource = (DeviceInformation.DeviceDataSource) 1;
      }
    }
    return deviceDataSource;
  }

  private bool HasDataSet(Channel channel)
  {
    if (channel != null)
    {
      UnitInformation selectedUnit = this.dataProvider.SelectedUnit;
      if (selectedUnit != null)
      {
        DeviceInformation informationForDevice = selectedUnit.GetInformationForDevice(channel.Ecu.Name);
        if (informationForDevice != null && informationForDevice.HasDataSet && selectedUnit.GetStatusForDataSource(informationForDevice.DataSource) == 2)
          return informationForDevice.HasDataSet;
      }
    }
    return false;
  }

  private void operation_onChange(object sender, EventArgs args)
  {
    this.selectedOperation = (SelectOperationPage.OperationType) Enum.Parse(typeof (SelectOperationPage.OperationType), ((sender as HtmlElement).DomElement as IHTMLInputElement).value);
    this.UpdateSubPanel();
  }

  private void deviceList_onChange(object sender, EventArgs args)
  {
    this.manualConnectionEcu = (string) null;
    this.selectedUnitPackage = (UnitInformation) null;
    this.selectedChannel = (Channel) null;
    HtmlElement inputElement = sender as HtmlElement;
    ReprogrammingView.SetActiveRadioElement(inputElement);
    string result = (inputElement.DomElement as IHTMLInputElement).value;
    if (result == "powertrain_current" || result == "powertrain_newest" || result == "vehicle_chec")
    {
      this.selectedUnitPackage = this.dataProvider.SelectedUnit;
      this.selectedUnitPackageType = result;
    }
    else if (result.StartsWith("manual_", StringComparison.Ordinal))
      this.manualConnectionEcu = result.Substring("manual_".Length);
    else
      this.selectedChannel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((System.Func<Channel, bool>) (c => c.Ecu.Name == result));
    this.UpdateDeviceSelection();
  }

  private void ButtonConnectClick()
  {
    string selectedConnectionResource = ((IHTMLSelectElement) this.connectionResourcesCombobox.DomElement).value;
    SapiManager globalInstance = SapiManager.GlobalInstance;
    Ecu ecuByName = SapiManager.GetEcuByName(this.manualConnectionEcu);
    ConnectionResource connectionResource = ecuByName != null ? ecuByName.GetConnectionResources().FirstOrDefault<ConnectionResource>((System.Func<ConnectionResource, bool>) (cr => cr.ToString() == selectedConnectionResource)) : (ConnectionResource) null;
    globalInstance.OpenConnection(connectionResource);
    ReprogrammingView.SetButtonState(this.connectButton, false, true);
    ReprogrammingView.EnableElement(this.connectionResourcesCombobox, false);
  }

  private void PopulateConnectionResources()
  {
    bool flag1 = (this.showAllConnectionResourcesCheckbox.DomElement as IHTMLInputElement).@checked;
    List<string> values = new List<string>();
    bool flag2 = false;
    Sapi sapi = SapiManager.GlobalInstance.Sapi;
    if (sapi == null)
      return;
    Ecu ecu = sapi.Ecus[this.manualConnectionEcu];
    if (ecu != null && sapi.Ecus.GetConnectedCountForIdentifier(ecu.Identifier) == 0)
    {
      foreach (ConnectionResource connectionResource in (ReadOnlyCollection<ConnectionResource>) ecu.GetConnectionResources())
      {
        if (flag1 || !connectionResource.Restricted)
          values.Add(FormattableString.Invariant(FormattableStringFactory.Create("<option value='{0}'>{1}</option>", (object) connectionResource.ToString(), (object) SapiExtensions.ToDisplayString(connectionResource))));
        else
          flag2 = true;
      }
    }
    if (values.Count == 0)
    {
      if (flag2 && ecu.ViaEcus.Any<Ecu>())
        values.Add(FormattableString.Invariant(FormattableStringFactory.Create("<option>{0}</option>", (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ResourceCannotBeDetermined, (object) string.Join<Ecu>("/", (IEnumerable<Ecu>) ecu.ViaEcus)))));
      else
        values.Add(FormattableString.Invariant(FormattableStringFactory.Create("<option>{0}</option>", (object) Resources.Messsage_NoConnectionResources)));
    }
    ReprogrammingView.SetButtonState(this.connectButton, values.Count > 0, true);
    this.connectionResourcesCombobox.InnerHtml = string.Join(" ", (IEnumerable<string>) values);
  }

  private void checkBoxShowAll_CheckedChanged(object sender, EventArgs e)
  {
    this.PopulateConnectionResources();
  }

  private enum OperationType
  {
    None,
    Replace,
    Update,
    ChangeDataSet,
  }
}
