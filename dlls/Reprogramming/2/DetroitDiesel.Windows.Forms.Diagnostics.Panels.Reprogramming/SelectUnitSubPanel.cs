using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using mshtml;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal class SelectUnitSubPanel
{
	private IProvideProgrammingData dataProvider;

	private HtmlElement settingsListPane;

	private SettingsInformation selectedSettings;

	private EdexFileInformation selectedEdexFile;

	private DeviceDataSource selectedDataSource;

	private bool isResetToDefaultSelected;

	public SettingsInformation SelectedSettings => selectedSettings;

	public EdexFileInformation SelectedEdexFile => selectedEdexFile;

	public DeviceDataSource SelectedUnitDataSource => selectedDataSource;

	public bool IsResetToDefaultSelected => isResetToDefaultSelected;

	public event EventHandler SettingSelectionChanged;

	public SelectUnitSubPanel(IProvideProgrammingData dataProvider, HtmlElement element)
	{
		this.dataProvider = dataProvider;
		settingsListPane = element;
	}

	public void UpdateSettingsList(Channel channel)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected I4, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		bool foundSelectedItem = false;
		string innerHtml = "&nbsp;";
		if (channel != null)
		{
			UnitInformation selectedUnit = dataProvider.SelectedUnit;
			if (selectedUnit != null)
			{
				DeviceInformation val = selectedUnit.DeviceInformation.FirstOrDefault((DeviceInformation deviceInformation) => deviceInformation.Device.Equals(channel.Ecu.Name));
				if (val != null)
				{
					if (ApplicationInformation.Branding.MaximumDeviceReprogrammings > 0 && val.UsageCount >= ApplicationInformation.Branding.MaximumDeviceReprogrammings)
					{
						settingsListPane.InnerHtml = FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectUnitSubPanelItem_MaxProgrammingUsage}</p>");
						return;
					}
					selectedDataSource = val.DataSource;
				}
				else
				{
					selectedDataSource = (DeviceDataSource)0;
				}
				DeviceDataSource val2 = selectedDataSource;
				innerHtml = (int)val2 switch
				{
					2 => CreateEdexSettingsList(channel, val, ref foundSelectedItem), 
					1 => CreateDepotSettingsList(channel, selectedUnit, val, ref foundSelectedItem), 
					_ => throw new DataException("Unknown unit data source, can not update unit settings."), 
				};
			}
		}
		settingsListPane.InnerHtml = innerHtml;
		if (!ReprogrammingView.SubscribeInputEvents(settingsListPane, "radio", "onchange", settingsList_onChange))
		{
			if (!foundSelectedItem)
			{
				isResetToDefaultSelected = false;
				selectedSettings = null;
				selectedEdexFile = null;
			}
			this.SettingSelectionChanged?.Invoke(this, new EventArgs());
		}
	}

	private string CreateEdexSettingsList(Channel channel, DeviceInformation di, ref bool foundSelectedItem)
	{
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Invalid comparison between Unknown and I4
		string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
		string hardwareRevision = SapiManager.GetHardwareRevision(channel);
		if (!string.IsNullOrEmpty(hardwarePartNumber) || !string.IsNullOrEmpty(hardwareRevision))
		{
			Part part = ((hardwarePartNumber != null) ? new Part(hardwarePartNumber) : null);
			IEnumerable<Part> channelDataSetParts = from p in SapiManager.GetDataSetPartNumbers(channel)
				select new Part(p);
			IEnumerable<Part> flashFileKeys = from fm in Sapi.GetSapi().FlashFiles.SelectMany((FlashFile ff) => ff.FlashAreas).SelectMany((FlashArea fa) => fa.FlashMeanings)
				select new Part(fm.FlashKey);
			List<string> list = new List<string>();
			foreach (EdexFileInformation item in CollectionExtensions.DistinctBy<EdexFileInformation, string>(di.EdexFiles.Where((EdexFileInformation efi) => !string.IsNullOrEmpty(efi.FileName)), (Func<EdexFileInformation, string>)((EdexFileInformation ef) => ef.FileName)))
			{
				string statusText = item.StatusText;
				bool flag = false;
				if (!item.HasErrors)
				{
					if ((int)item.FileType == 1 && !di.IsEdexFactoryConfigurationValidForProgramming)
					{
						statusText = string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_FactoryCannotBeProgrammed, item.ConfigurationInformation.FlashwarePartNumber);
						flag = true;
					}
					else if (!item.ConfigurationInformation.IsApplicableTo(part, hardwareRevision))
					{
						statusText = ((item.ConfigurationInformation.HardwarePartNumber != null) ? string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_HardwarePartNumberMismatch, PartExtensions.ToHardwarePartNumberString(item.ConfigurationInformation.HardwarePartNumber, item.ConfigurationInformation.DeviceName, true)) : string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_HardwarePartNumberMismatch, item.ConfigurationInformation.HardwareRevision));
						flag = true;
					}
					else if (!AreCodingParametersAvailable(item.ConfigurationInformation, channel, ref statusText))
					{
						flag = true;
					}
					else if (!AreNeededFlashFilesAvailable(item.ConfigurationInformation, channelDataSetParts, flashFileKeys, channel, ref statusText))
					{
						flag = true;
						if (item.ConfigurationInformation.FlashingRequiresMvci && !SapiManager.GlobalInstance.UseMcd)
						{
							statusText = (SapiManager.GlobalInstance.McdAvailable ? Resources.SelectUnitSubPanelItem_MVCIServerRequiredButNotEnabled : Resources.SelectUnitSubPanelItem_MVCIServerRequiredButNotAvailable);
						}
					}
				}
				DateTime? dateTime = ((item.ConfigurationInformation != null) ? item.ConfigurationInformation.DiagnosticLinkSettingsTimestamp : ((DateTime?)null));
				list.Add(ReprogrammingView.CreateInput("settingsList", "edex_" + item.FileName, string.Join(" - ", new string[4]
				{
					item.CompleteFileType,
					(!string.IsNullOrEmpty(item.Comments)) ? ("(" + item.Comments + ")") : string.Empty,
					dateTime.HasValue ? Resources.SelectUnitSubPanelItem_LatestDescription : null,
					dateTime.HasValue ? dateTime.ToString() : null
				}.Where((string s) => !string.IsNullOrEmpty(s))), statusText, item == selectedEdexFile, flag || item.HasErrors, (flag || item.HasErrors) ? "warninginline" : null));
				if (item == selectedEdexFile)
				{
					foundSelectedItem = true;
				}
			}
			return ReprogrammingView.CreateRadioList(Resources.SelectSettingsSubPanel_SelectConfiguration, list);
		}
		return FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectUnitSubPanelItem_CouldNotReadHardwarePartNumber}</p>");
	}

	private string CreateDepotSettingsList(Channel channel, UnitInformation ui, DeviceInformation di, ref bool foundSelectedItem)
	{
		if (di.FirmwareOptionSupportsHardware(new Part(SapiManager.GetHardwarePartNumber(channel))))
		{
			List<string> list = new List<string>();
			foreach (SettingsInformation item in ui.SettingsInformation)
			{
				if (item.Device == channel.Ecu.Name && !item.Preset)
				{
					string text = Resources.SelectUnitSubPanelItemStatus_Default;
					switch (item.SettingsType)
					{
					case "latest":
						text = Resources.SelectUnitSubPanelItem_LatestDescription;
						break;
					case "history":
						text = Resources.SelectUnitSubPanelItem_HistoryDescription;
						break;
					case "ota":
						text = Resources.SelectUnitSubPanelItem_OverTheAirDescription;
						break;
					case "factory":
						text = Resources.SelectUnitSubPanelItem_FactoryDescription;
						break;
					case "oem":
						text = Resources.SelectUnitSubPanelItem_VehiclePlantDescription;
						break;
					}
					list.Add(ReprogrammingView.CreateInput("settingsList", "depot_" + item.FileName, string.Join(" - ", new string[4]
					{
						item.SettingsType,
						text,
						item.Timestamp.HasValue ? item.Timestamp.ToString() : string.Empty,
						(item.Description != "DiagnosticLink") ? item.Description : string.Empty
					}.Where((string s) => !string.IsNullOrEmpty(s))), null, item == selectedSettings, disabled: false));
					if (item == selectedSettings)
					{
						foundSelectedItem = true;
					}
				}
			}
			if (!ui.HasSettingsForDevice(channel.Ecu.Name))
			{
				list.Add(ReprogrammingView.CreateInput("settingsList", "depot_reset", Resources.SelectUnitSubPanelItemName_Defaults, Resources.SelectUnitSubPanelItem_ResetConfigurationToDefaultNotRecommended, isResetToDefaultSelected, disabled: false, "warninginline"));
				if (isResetToDefaultSelected)
				{
					foundSelectedItem = true;
				}
			}
			return ReprogrammingView.CreateRadioList(Resources.SelectSettingsSubPanel_SelectSettings, list);
		}
		string[] array = (from part in (from firmware in di.FirmwareOptions
				from part in ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleHardwareList(new Software(di.Device, firmware.Version))
				select part).Distinct()
			select PartExtensions.ToHardwarePartNumberString(part, channel.Ecu, true)).ToArray();
		string text2 = (array.Any() ? string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_HardwarePartNumberMismatch, string.Join(", ", array)) : Resources.SelectUnitSubPanelItemFormat_HardwarePartNumberNoMatchesAvailable);
		return FormattableString.Invariant($"<p><span class='warninginline'/>{text2}</p>");
	}

	private static bool AreCodingParametersAvailable(EdexConfigurationInformation configInfo, Channel channel, ref string statusText)
	{
		List<CodingFile> source = SapiManager.GlobalInstance.Sapi.CodingFiles.Where((CodingFile f) => f.Ecus.Any((Ecu e) => e.Name == channel.Ecu.Name)).ToList();
		foreach (EdexSettingItem setting in configInfo.SettingItems.Where((EdexSettingItem si) => (int)si.MasterType == 0 || (int)si.MasterType == 1))
		{
			if (!source.Any((CodingFile f) => f.CodingParameterGroups.GetCodingForPart(setting.PartNumber.ToString()) != null))
			{
				statusText = string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingCodingParameter, setting.PartNumber);
				return false;
			}
		}
		foreach (EdexSettingItem setting2 in configInfo.ApplicableProposedSettingItems())
		{
			if (!source.Any((CodingFile f) => f.CodingParameterGroups.GetCodingForPart(setting2.PartNumber.ToString()) != null))
			{
				statusText = string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingCodingParameter, setting2.PartNumber);
				return false;
			}
		}
		return true;
	}

	private static bool AreNeededFlashFilesAvailable(EdexConfigurationInformation configInfo, IEnumerable<Part> channelDataSetParts, IEnumerable<Part> flashFileKeys, Channel channel, ref string statusText)
	{
		Part part = configInfo.DataSetPartNumbers.FirstOrDefault((Part dsp) => !channelDataSetParts.Any((Part cdsp) => dsp.Equals(cdsp)) && !flashFileKeys.Any((Part k) => k.Equals(dsp)));
		if (part != null)
		{
			statusText = string.Format(CultureInfo.CurrentCulture, FlashFileRequiresDownload(part) ? Resources.SelectUnitSubPanelItemFormat_DatasetFlashKeyRequiresDownload : Resources.SelectUnitSubPanelItemFormat_MissingDatasetFlashKey, part);
			return false;
		}
		if (!SapiManager.ProgramDeviceUsesSoftwareIdentification(channel.Ecu) && configInfo.FlashwarePartNumber != null && !PartExtensions.IsEqual(configInfo.FlashwarePartNumber, SapiManager.GetSoftwarePartNumber(channel)))
		{
			if (!flashFileKeys.Any((Part k) => k.Equals(configInfo.FlashwarePartNumber)))
			{
				statusText = string.Format(CultureInfo.CurrentCulture, FlashFileRequiresDownload(configInfo.FlashwarePartNumber) ? Resources.SelectUnitSubPanelItemFormat_FirmwareFlashKeyRequiresDownload : Resources.SelectUnitSubPanelItemFormat_MissingFirmwareFlashKey, configInfo.FlashwarePartNumber);
				return false;
			}
			if (configInfo.BootLoaderPartNumber != null && !PartExtensions.IsEqual(configInfo.BootLoaderPartNumber, SapiManager.GetBootSoftwarePartNumber(channel)) && !flashFileKeys.Any((Part k) => k.Equals(configInfo.BootLoaderPartNumber)))
			{
				statusText = string.Format(CultureInfo.CurrentCulture, FlashFileRequiresDownload(configInfo.BootLoaderPartNumber) ? Resources.SelectUnitSubPanelItemFormat_BootFirmwareFlashKeyRequiresDownload : Resources.SelectUnitSubPanelItemFormat_MissingBootFirmwareFlashKey, configInfo.BootLoaderPartNumber);
				return false;
			}
		}
		return true;
		static bool FlashFileRequiresDownload(Part part2)
		{
			return ServerDataManager.GlobalInstance.FirmwareInformation.OfType<FirmwareInformation>().Any((FirmwareInformation fi) => PartExtensions.IsEqual(part2, fi.Key) && fi.RequiresDownload);
		}
	}

	private void settingsList_onChange(object sender, EventArgs args)
	{
		HtmlElement htmlElement = sender as HtmlElement;
		ReprogrammingView.SetActiveRadioElement(htmlElement);
		string value = (htmlElement.DomElement as IHTMLInputElement).value;
		if (value.StartsWith("edex_", StringComparison.Ordinal))
		{
			string file = value.Substring("edex_".Length);
			UnitInformation selectedUnit = dataProvider.SelectedUnit;
			selectedEdexFile = ((selectedUnit != null) ? selectedUnit.DeviceInformation.SelectMany((DeviceInformation di) => di.EdexFiles).FirstOrDefault((EdexFileInformation ef) => ef.FileName == file) : null);
		}
		else if (value.StartsWith("depot_", StringComparison.Ordinal))
		{
			if (value == "depot_reset")
			{
				isResetToDefaultSelected = true;
			}
			else
			{
				string file2 = value.Substring("depot_".Length);
				UnitInformation selectedUnit2 = dataProvider.SelectedUnit;
				selectedSettings = ((selectedUnit2 != null) ? selectedUnit2.SettingsInformation.FirstOrDefault((SettingsInformation si) => si.FileName == file2) : null);
			}
		}
		this.SettingSelectionChanged?.Invoke(this, new EventArgs());
	}
}
