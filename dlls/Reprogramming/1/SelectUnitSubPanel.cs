// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.SelectUnitSubPanel
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using mshtml;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal class SelectUnitSubPanel
{
  private IProvideProgrammingData dataProvider;
  private HtmlElement settingsListPane;
  private SettingsInformation selectedSettings;
  private EdexFileInformation selectedEdexFile;
  private DeviceInformation.DeviceDataSource selectedDataSource;
  private bool isResetToDefaultSelected;

  public SelectUnitSubPanel(IProvideProgrammingData dataProvider, HtmlElement element)
  {
    this.dataProvider = dataProvider;
    this.settingsListPane = element;
  }

  public void UpdateSettingsList(Channel channel)
  {
    bool foundSelectedItem = false;
    string str = "&nbsp;";
    if (channel != null)
    {
      UnitInformation selectedUnit = this.dataProvider.SelectedUnit;
      if (selectedUnit != null)
      {
        DeviceInformation di = selectedUnit.DeviceInformation.FirstOrDefault<DeviceInformation>((System.Func<DeviceInformation, bool>) (deviceInformation => deviceInformation.Device.Equals(channel.Ecu.Name)));
        if (di != null)
        {
          if (ApplicationInformation.Branding.MaximumDeviceReprogrammings > 0 && di.UsageCount >= ApplicationInformation.Branding.MaximumDeviceReprogrammings)
          {
            this.settingsListPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectUnitSubPanelItem_MaxProgrammingUsage));
            return;
          }
          this.selectedDataSource = di.DataSource;
        }
        else
          this.selectedDataSource = (DeviceInformation.DeviceDataSource) 0;
        switch ((int) this.selectedDataSource)
        {
          case 1:
            str = this.CreateDepotSettingsList(channel, selectedUnit, di, ref foundSelectedItem);
            break;
          case 2:
            str = this.CreateEdexSettingsList(channel, di, ref foundSelectedItem);
            break;
          default:
            throw new DataException("Unknown unit data source, can not update unit settings.");
        }
      }
    }
    this.settingsListPane.InnerHtml = str;
    if (ReprogrammingView.SubscribeInputEvents(this.settingsListPane, "radio", "onchange", new EventHandler(this.settingsList_onChange)))
      return;
    if (!foundSelectedItem)
    {
      this.isResetToDefaultSelected = false;
      this.selectedSettings = (SettingsInformation) null;
      this.selectedEdexFile = (EdexFileInformation) null;
    }
    EventHandler selectionChanged = this.SettingSelectionChanged;
    if (selectionChanged == null)
      return;
    selectionChanged((object) this, new EventArgs());
  }

  private string CreateEdexSettingsList(
    Channel channel,
    DeviceInformation di,
    ref bool foundSelectedItem)
  {
    string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
    string hardwareRevision = SapiManager.GetHardwareRevision(channel);
    if (!string.IsNullOrEmpty(hardwarePartNumber) || !string.IsNullOrEmpty(hardwareRevision))
    {
      Part part = hardwarePartNumber != null ? new Part(hardwarePartNumber) : (Part) null;
      IEnumerable<Part> channelDataSetParts = SapiManager.GetDataSetPartNumbers(channel).Select<string, Part>((System.Func<string, Part>) (p => new Part(p)));
      IEnumerable<Part> flashFileKeys = Sapi.GetSapi().FlashFiles.SelectMany<FlashFile, FlashArea>((System.Func<FlashFile, IEnumerable<FlashArea>>) (ff => ff.FlashAreas)).SelectMany<FlashArea, FlashMeaning>((System.Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).Select<FlashMeaning, Part>((System.Func<FlashMeaning, Part>) (fm => new Part(fm.FlashKey)));
      List<string> listItems = new List<string>();
      foreach (EdexFileInformation edexFileInformation in CollectionExtensions.DistinctBy<EdexFileInformation, string>(di.EdexFiles.Where<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (efi => !string.IsNullOrEmpty(efi.FileName))), (System.Func<EdexFileInformation, string>) (ef => ef.FileName)))
      {
        string statusText = edexFileInformation.StatusText;
        bool flag = false;
        if (!edexFileInformation.HasErrors)
        {
          if (edexFileInformation.FileType == 1 && !di.IsEdexFactoryConfigurationValidForProgramming)
          {
            statusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_FactoryCannotBeProgrammed, (object) edexFileInformation.ConfigurationInformation.FlashwarePartNumber);
            flag = true;
          }
          else if (!edexFileInformation.ConfigurationInformation.IsApplicableTo(part, hardwareRevision))
          {
            statusText = edexFileInformation.ConfigurationInformation.HardwarePartNumber != null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_HardwarePartNumberMismatch, (object) PartExtensions.ToHardwarePartNumberString(edexFileInformation.ConfigurationInformation.HardwarePartNumber, edexFileInformation.ConfigurationInformation.DeviceName, true)) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_HardwarePartNumberMismatch, (object) edexFileInformation.ConfigurationInformation.HardwareRevision);
            flag = true;
          }
          else if (!SelectUnitSubPanel.AreCodingParametersAvailable(edexFileInformation.ConfigurationInformation, channel, ref statusText))
            flag = true;
          else if (!SelectUnitSubPanel.AreNeededFlashFilesAvailable(edexFileInformation.ConfigurationInformation, channelDataSetParts, flashFileKeys, channel, ref statusText))
          {
            flag = true;
            if (edexFileInformation.ConfigurationInformation.FlashingRequiresMvci && !SapiManager.GlobalInstance.UseMcd)
              statusText = SapiManager.GlobalInstance.McdAvailable ? Resources.SelectUnitSubPanelItem_MVCIServerRequiredButNotEnabled : Resources.SelectUnitSubPanelItem_MVCIServerRequiredButNotAvailable;
          }
        }
        DateTime? nullable = edexFileInformation.ConfigurationInformation != null ? edexFileInformation.ConfigurationInformation.DiagnosticLinkSettingsTimestamp : new DateTime?();
        listItems.Add(ReprogrammingView.CreateInput("settingsList", "edex_" + edexFileInformation.FileName, string.Join(" - ", ((IEnumerable<string>) new string[4]
        {
          edexFileInformation.CompleteFileType,
          !string.IsNullOrEmpty(edexFileInformation.Comments) ? $"({edexFileInformation.Comments})" : string.Empty,
          nullable.HasValue ? Resources.SelectUnitSubPanelItem_LatestDescription : (string) null,
          nullable.HasValue ? nullable.ToString() : (string) null
        }).Where<string>((System.Func<string, bool>) (s => !string.IsNullOrEmpty(s)))), statusText, (edexFileInformation == this.selectedEdexFile ? 1 : 0) != 0, (flag ? 1 : (edexFileInformation.HasErrors ? 1 : 0)) != 0, flag || edexFileInformation.HasErrors ? "warninginline" : (string) null));
        if (edexFileInformation == this.selectedEdexFile)
          foundSelectedItem = true;
      }
      return ReprogrammingView.CreateRadioList(Resources.SelectSettingsSubPanel_SelectConfiguration, listItems);
    }
    return FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectUnitSubPanelItem_CouldNotReadHardwarePartNumber));
  }

  private string CreateDepotSettingsList(
    Channel channel,
    UnitInformation ui,
    DeviceInformation di,
    ref bool foundSelectedItem)
  {
    if (di.FirmwareOptionSupportsHardware(new Part(SapiManager.GetHardwarePartNumber(channel))))
    {
      List<string> listItems = new List<string>();
      foreach (SettingsInformation settingsInformation in ui.SettingsInformation)
      {
        if (settingsInformation.Device == channel.Ecu.Name && !settingsInformation.Preset)
        {
          string str = Resources.SelectUnitSubPanelItemStatus_Default;
          switch (settingsInformation.SettingsType)
          {
            case "latest":
              str = Resources.SelectUnitSubPanelItem_LatestDescription;
              break;
            case "history":
              str = Resources.SelectUnitSubPanelItem_HistoryDescription;
              break;
            case "ota":
              str = Resources.SelectUnitSubPanelItem_OverTheAirDescription;
              break;
            case "factory":
              str = Resources.SelectUnitSubPanelItem_FactoryDescription;
              break;
            case "oem":
              str = Resources.SelectUnitSubPanelItem_VehiclePlantDescription;
              break;
          }
          listItems.Add(ReprogrammingView.CreateInput("settingsList", "depot_" + settingsInformation.FileName, string.Join(" - ", ((IEnumerable<string>) new string[4]
          {
            settingsInformation.SettingsType,
            str,
            settingsInformation.Timestamp.HasValue ? settingsInformation.Timestamp.ToString() : string.Empty,
            settingsInformation.Description != "DiagnosticLink" ? settingsInformation.Description : string.Empty
          }).Where<string>((System.Func<string, bool>) (s => !string.IsNullOrEmpty(s)))), (string) null, (settingsInformation == this.selectedSettings ? 1 : 0) != 0, false));
          if (settingsInformation == this.selectedSettings)
            foundSelectedItem = true;
        }
      }
      if (!ui.HasSettingsForDevice(channel.Ecu.Name))
      {
        listItems.Add(ReprogrammingView.CreateInput("settingsList", "depot_reset", Resources.SelectUnitSubPanelItemName_Defaults, Resources.SelectUnitSubPanelItem_ResetConfigurationToDefaultNotRecommended, this.isResetToDefaultSelected, false, "warninginline"));
        if (this.isResetToDefaultSelected)
          foundSelectedItem = true;
      }
      return ReprogrammingView.CreateRadioList(Resources.SelectSettingsSubPanel_SelectSettings, listItems);
    }
    string[] array = di.FirmwareOptions.SelectMany<FirmwareOptionInformation, Part, Part>((System.Func<FirmwareOptionInformation, IEnumerable<Part>>) (firmware => (IEnumerable<Part>) ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleHardwareList(new Software(di.Device, firmware.Version))), (Func<FirmwareOptionInformation, Part, Part>) ((firmware, part) => part)).Distinct<Part>().Select<Part, string>((System.Func<Part, string>) (part => PartExtensions.ToHardwarePartNumberString(part, channel.Ecu, true))).ToArray<string>();
    return FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) (((IEnumerable<string>) array).Any<string>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_HardwarePartNumberMismatch, (object) string.Join(", ", array)) : Resources.SelectUnitSubPanelItemFormat_HardwarePartNumberNoMatchesAvailable)));
  }

  private static bool AreCodingParametersAvailable(
    EdexConfigurationInformation configInfo,
    Channel channel,
    ref string statusText)
  {
    List<CodingFile> list = SapiManager.GlobalInstance.Sapi.CodingFiles.Where<CodingFile>((System.Func<CodingFile, bool>) (f => f.Ecus.Any<Ecu>((System.Func<Ecu, bool>) (e => e.Name == channel.Ecu.Name)))).ToList<CodingFile>();
    foreach (EdexSettingItem edexSettingItem in configInfo.SettingItems.Where<EdexSettingItem>((System.Func<EdexSettingItem, bool>) (si => si.MasterType == null || si.MasterType == 1)))
    {
      EdexSettingItem setting = edexSettingItem;
      if (!list.Any<CodingFile>((System.Func<CodingFile, bool>) (f => f.CodingParameterGroups.GetCodingForPart(setting.PartNumber.ToString()) != null)))
      {
        statusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingCodingParameter, (object) setting.PartNumber);
        return false;
      }
    }
    foreach (EdexSettingItem proposedSettingItem in configInfo.ApplicableProposedSettingItems())
    {
      EdexSettingItem setting = proposedSettingItem;
      if (!list.Any<CodingFile>((System.Func<CodingFile, bool>) (f => f.CodingParameterGroups.GetCodingForPart(setting.PartNumber.ToString()) != null)))
      {
        statusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingCodingParameter, (object) setting.PartNumber);
        return false;
      }
    }
    return true;
  }

  private static bool AreNeededFlashFilesAvailable(
    EdexConfigurationInformation configInfo,
    IEnumerable<Part> channelDataSetParts,
    IEnumerable<Part> flashFileKeys,
    Channel channel,
    ref string statusText)
  {
    Part part = configInfo.DataSetPartNumbers.FirstOrDefault<Part>((System.Func<Part, bool>) (dsp => !channelDataSetParts.Any<Part>((System.Func<Part, bool>) (cdsp => dsp.Equals((object) cdsp))) && !flashFileKeys.Any<Part>((System.Func<Part, bool>) (k => k.Equals((object) dsp)))));
    if (part != null)
    {
      statusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, FlashFileRequiresDownload(part) ? Resources.SelectUnitSubPanelItemFormat_DatasetFlashKeyRequiresDownload : Resources.SelectUnitSubPanelItemFormat_MissingDatasetFlashKey, (object) part);
      return false;
    }
    if (!SapiManager.ProgramDeviceUsesSoftwareIdentification(channel.Ecu) && configInfo.FlashwarePartNumber != null && !PartExtensions.IsEqual(configInfo.FlashwarePartNumber, SapiManager.GetSoftwarePartNumber(channel)))
    {
      if (!flashFileKeys.Any<Part>((System.Func<Part, bool>) (k => k.Equals((object) configInfo.FlashwarePartNumber))))
      {
        statusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, FlashFileRequiresDownload(configInfo.FlashwarePartNumber) ? Resources.SelectUnitSubPanelItemFormat_FirmwareFlashKeyRequiresDownload : Resources.SelectUnitSubPanelItemFormat_MissingFirmwareFlashKey, (object) configInfo.FlashwarePartNumber);
        return false;
      }
      if (configInfo.BootLoaderPartNumber != null && !PartExtensions.IsEqual(configInfo.BootLoaderPartNumber, SapiManager.GetBootSoftwarePartNumber(channel)) && !flashFileKeys.Any<Part>((System.Func<Part, bool>) (k => k.Equals((object) configInfo.BootLoaderPartNumber))))
      {
        statusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, FlashFileRequiresDownload(configInfo.BootLoaderPartNumber) ? Resources.SelectUnitSubPanelItemFormat_BootFirmwareFlashKeyRequiresDownload : Resources.SelectUnitSubPanelItemFormat_MissingBootFirmwareFlashKey, (object) configInfo.BootLoaderPartNumber);
        return false;
      }
    }
    return true;

    static bool FlashFileRequiresDownload(Part part)
    {
      return ServerDataManager.GlobalInstance.FirmwareInformation.OfType<FirmwareInformation>().Any<FirmwareInformation>((System.Func<FirmwareInformation, bool>) (fi => PartExtensions.IsEqual(part, fi.Key) && fi.RequiresDownload));
    }
  }

  public SettingsInformation SelectedSettings => this.selectedSettings;

  public EdexFileInformation SelectedEdexFile => this.selectedEdexFile;

  public DeviceInformation.DeviceDataSource SelectedUnitDataSource => this.selectedDataSource;

  public bool IsResetToDefaultSelected => this.isResetToDefaultSelected;

  public event EventHandler SettingSelectionChanged;

  private void settingsList_onChange(object sender, EventArgs args)
  {
    HtmlElement inputElement = sender as HtmlElement;
    ReprogrammingView.SetActiveRadioElement(inputElement);
    string str = (inputElement.DomElement as IHTMLInputElement).value;
    if (str.StartsWith("edex_", StringComparison.Ordinal))
    {
      string file = str.Substring("edex_".Length);
      UnitInformation selectedUnit = this.dataProvider.SelectedUnit;
      this.selectedEdexFile = selectedUnit != null ? selectedUnit.DeviceInformation.SelectMany<DeviceInformation, EdexFileInformation>((System.Func<DeviceInformation, IEnumerable<EdexFileInformation>>) (di => (IEnumerable<EdexFileInformation>) di.EdexFiles)).FirstOrDefault<EdexFileInformation>((System.Func<EdexFileInformation, bool>) (ef => ef.FileName == file)) : (EdexFileInformation) null;
    }
    else if (str.StartsWith("depot_", StringComparison.Ordinal))
    {
      if (str == "depot_reset")
      {
        this.isResetToDefaultSelected = true;
      }
      else
      {
        string file = str.Substring("depot_".Length);
        UnitInformation selectedUnit = this.dataProvider.SelectedUnit;
        this.selectedSettings = selectedUnit != null ? selectedUnit.SettingsInformation.FirstOrDefault<SettingsInformation>((System.Func<SettingsInformation, bool>) (si => si.FileName == file)) : (SettingsInformation) null;
      }
    }
    EventHandler selectionChanged = this.SettingSelectionChanged;
    if (selectionChanged == null)
      return;
    selectionChanged((object) this, new EventArgs());
  }
}
