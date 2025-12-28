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
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using mshtml;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class SelectOperationPage : WizardPage
{
	private enum OperationType
	{
		None,
		Replace,
		Update,
		ChangeDataSet
	}

	private IProvideProgrammingData dataProvider;

	private const string ManualConnectionFlag = "ManualConnection";

	private OperationType selectedOperation;

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

	private Channel SelectedChannel => selectedChannel;

	private UnitInformation SelectedUnitPackage => selectedUnitPackage;

	private string SelectedUnitPackageType => selectedUnitPackageType;

	public SelectOperationPage(ReprogrammingView dataProvider, HtmlElement inputPane)
		: base(dataProvider, inputPane)
	{
		this.dataProvider = dataProvider;
		Sapi sapi = SapiManager.GlobalInstance.Sapi;
		sapi.Channels.ConnectCompleteEvent += OnConnectCompleteEvent;
		sapi.Channels.DisconnectCompleteEvent += OnDisconnectCompleteEvent;
		SapiManager.GlobalInstance.ConnectedUnitChanged += SapiManagerChannelChangedEvent;
		SapiManager.GlobalInstance.LogFileChannelsChanged += SapiManagerChannelChangedEvent;
		SapiManager.GlobalInstance.ChannelIdentificationChanged += GlobalInstance_ChannelIdentificationChanged;
		LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadStatusChanged += GlobalInstance_RemoteFileDownloadStatusChanged;
		SetInputPane(inputPane);
	}

	private void GlobalInstance_RemoteFileDownloadStatusChanged(object sender, EventArgs e)
	{
		if (base.Wizard.ActivePage == this)
		{
			ReprogrammingView.UpdateTitle(titlePane, dataProvider.SelectedUnit);
		}
	}

	internal void SetInputPane(HtmlElement inputPane)
	{
		inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("\r\n                <div>\r\n                    <div id='titlePane'>&nbsp;</div>\r\n                    <div id='deviceListPane'>&nbsp;</div>\r\n                    <div id='operationPane' class='hide'>&nbsp;</div>\r\n                    <div id='manualConnectionPane' class='hide'>\r\n                        <span><button id='connectButton' class='button blue' onClick=\"clickButton('connect')\">{0}</button></span>\r\n                        <span><select id='connectionResources'>&nbsp;</select></span>\r\n                        <span><input type='checkbox' id='showAllConnectionResources'>Show all</span>\r\n                    </div>\r\n                    <div id='unitSubPanel' class='hide'>&nbsp;</div>\r\n                    <div id='softwareSubPanel' class='hide'>&nbsp;</div>\r\n                    <div id='datasetSubPanel' class='hide'>&nbsp;</div>\r\n                    <br/><br/><br/><br/>\r\n                    <div id='buttonPanel' class='fixedbottom'>\r\n                        <button id='backButtonSOP' class='button blue' onClick=\"clickButton('back')\">{1}</button>\r\n                        <button id='nextButtonSOP' class='button gray' onClick=\"clickButton('next')\">{2}</button>\r\n                    </div>\r\n                </div>\r\n            ", "Connect", Resources.Wizard_ButtonBack, Resources.Wizard_ButtonNext));
		foreach (HtmlElement item in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList())
		{
			switch (item.Id)
			{
			case "titlePane":
				titlePane = item;
				break;
			case "deviceListPane":
				deviceListPane = item;
				break;
			case "operationPane":
				operationPane = item;
				break;
			case "manualConnectionPane":
				manualConnectionPane = item;
				break;
			case "unitSubPanel":
				selectUnitPanel = new SelectUnitSubPanel(dataProvider, item);
				settingsSubPanelPane = item;
				break;
			case "softwareSubPanel":
				selectSoftwarePanel = new SelectFirmwareSubPanel(dataProvider, item);
				softwareSubPanelPane = item;
				break;
			case "datasetSubPanel":
				selectDataSetPanel = new SelectDataSetSubPanel(dataProvider, item);
				datasetSubPanelPane = item;
				break;
			}
		}
		foreach (HtmlElement item2 in inputPane.GetElementsByTagName("button").OfType<HtmlElement>().ToList())
		{
			switch (item2.Id)
			{
			case "backButtonSOP":
				base.BackButton = item2;
				break;
			case "nextButtonSOP":
				base.NextButton = item2;
				break;
			case "connectButton":
				connectButton = item2;
				break;
			}
		}
		showAllConnectionResourcesCheckbox = manualConnectionPane.GetElementsByTagName("input")[0];
		connectionResourcesCombobox = manualConnectionPane.GetElementsByTagName("select")[0];
		ReprogrammingView.SubscribeInputEvents(manualConnectionPane, "checkbox", "onclick", checkBoxShowAll_CheckedChanged);
		selectUnitPanel.SettingSelectionChanged += SelectUnitPanel_SettingSelectionChanged;
		selectSoftwarePanel.SelectedFirmwareChanged += SelectSoftwarePanel_SelectedFirmwareChanged;
		selectDataSetPanel.SelectedDataOptionChanged += SelectDataSetPanel_SelectedDataOptionChanged;
	}

	internal override void Navigate(string fragment)
	{
		if (fragment == "#button_connect")
		{
			ButtonConnectClick();
		}
	}

	private void SelectUnitPanel_SettingSelectionChanged(object sender, EventArgs e)
	{
		SetWizardButtons();
	}

	private void SelectDataSetPanel_SelectedDataOptionChanged(object sender, EventArgs e)
	{
		SetWizardButtons();
	}

	private void SelectSoftwarePanel_SelectedFirmwareChanged(object sender, EventArgs e)
	{
		SetWizardButtons();
	}

	internal override WizardPage OnWizardNext()
	{
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Invalid comparison between Unknown and I4
		try
		{
			GatherProgrammingData();
			ProgrammingData programmingData = dataProvider.ProgrammingData.First();
			if ((programmingData.Operation == ProgrammingOperation.ChangeDataSet || programmingData.Operation == ProgrammingOperation.UpdateAndChangeDataSet) && !selectDataSetPanel.SelectedDataSetOption.IsCurrent)
			{
				ChargeDialog chargeDialog = new ChargeDialog();
				chargeDialog.ReferenceNumber = chargeText;
				DialogResult dialogResult = chargeDialog.ShowDialog();
				if (dialogResult == DialogResult.Cancel || string.IsNullOrEmpty(chargeDialog.ReferenceNumber))
				{
					return null;
				}
				programmingData.ChargeText = (chargeText = chargeDialog.ReferenceNumber);
				programmingData.ChargeType = "REFNUMBER";
			}
			if (programmingData.Operation == ProgrammingOperation.Replace && !programmingData.ReplaceToSameDevice)
			{
				List<string> list = new List<string>();
				foreach (ProgrammingData programmingDatum in dataProvider.ProgrammingData)
				{
					string engineSerialNumber = SapiManager.GetEngineSerialNumber(programmingDatum.Channel);
					string vehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(programmingDatum.Channel);
					if (engineSerialNumber != null && !string.IsNullOrEmpty(programmingDatum.EngineSerialNumber) && !string.Equals(engineSerialNumber, programmingDatum.EngineSerialNumber))
					{
						list.Add(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatChangeESN, programmingDatum.Channel.Ecu.Name, engineSerialNumber, programmingDatum.EngineSerialNumber));
					}
					if (vehicleIdentificationNumber != null && !string.IsNullOrEmpty(programmingDatum.VehicleIdentificationNumber) && !string.Equals(vehicleIdentificationNumber, programmingDatum.VehicleIdentificationNumber))
					{
						list.Add(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatChangeVIN, programmingDatum.Channel.Ecu.Name, vehicleIdentificationNumber, programmingDatum.VehicleIdentificationNumber));
					}
				}
				if (list.Any())
				{
					StringBuilder message = new StringBuilder();
					message.AppendLine(Resources.SelectOperationPage_DifferentESNOrVINMessage);
					message.AppendLine();
					list.ForEach(delegate(string s)
					{
						message.AppendLine(s);
					});
					message.AppendLine();
					message.AppendLine(Resources.SelectOperationPage_ClickOK);
					DialogResult dialogResult2 = ControlHelpers.ShowMessageBox(message.ToString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
					if (dialogResult2 == DialogResult.Cancel)
					{
						return null;
					}
				}
			}
			if ((int)programmingData.DataSource == 2 && programmingData.EdexFileInformation.ConfigurationInformation.Upgrade && ControlHelpers.ShowMessageBox(Resources.SelectOperationPage_EdexUpgradeMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
			{
				return null;
			}
			return base.OnWizardNext();
		}
		catch (DataException ex)
		{
			ControlHelpers.ShowMessageBox(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			return null;
		}
	}

	internal override void OnSetActive()
	{
		SetWizardButtons();
		ReprogrammingView.UpdateTitle(titlePane, dataProvider.SelectedUnit);
		BuildDeviceListDeferred();
		UpdateDeviceSelection();
		Channel channel = SelectedChannel;
		selectUnitPanel.UpdateSettingsList(channel);
		if (channel == null || SapiExtensions.IsDataSourceDepot(channel.Ecu))
		{
			selectSoftwarePanel.UpdateFirmwareList(channel);
			selectDataSetPanel.UpdateDataSetList(channel);
		}
	}

	private void SetWizardButtons()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		if (base.Wizard.ActivePage != this)
		{
			return;
		}
		WizardButtons val = (WizardButtons)1;
		if (selectedChannel != null || selectedUnitPackage != null)
		{
			if (selectedOperation == OperationType.Replace)
			{
				if (((selectUnitPanel.IsResetToDefaultSelected || selectUnitPanel.SelectedSettings != null) && (int)selectUnitPanel.SelectedUnitDataSource == 1) || (selectUnitPanel.SelectedEdexFile != null && (int)selectUnitPanel.SelectedUnitDataSource == 2))
				{
					val = (WizardButtons)(val | 2);
				}
			}
			else if (selectedOperation == OperationType.Update)
			{
				if (selectSoftwarePanel.SelectedFirmware != null)
				{
					val = (WizardButtons)(val | 2);
				}
			}
			else if (selectedOperation == OperationType.ChangeDataSet)
			{
				if (selectDataSetPanel.SelectedDataSetOption != null)
				{
					val = (WizardButtons)(val | 2);
				}
			}
			else if (SelectedUnitPackage != null)
			{
				val = (WizardButtons)(val | 2);
			}
		}
		base.Wizard.SetWizardButtons(val);
	}

	private void GatherProgrammingData()
	{
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Invalid comparison between Unknown and I4
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Invalid comparison between Unknown and I4
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Invalid comparison between Unknown and I4
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Invalid comparison between Unknown and I4
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Invalid comparison between Unknown and I4
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Invalid comparison between Unknown and I4
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Invalid comparison between Unknown and I4
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Invalid comparison between Unknown and I4
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Expected O, but got Unknown
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Invalid comparison between Unknown and I4
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Expected O, but got Unknown
		if (SelectedUnitPackage != null && !string.IsNullOrEmpty(SelectedUnitPackageType))
		{
			List<ProgrammingData> list = new List<ProgrammingData>();
			switch (SelectedUnitPackageType)
			{
			case "powertrain_current":
			case "powertrain_newest":
				foreach (DeviceInformation deviceInformation2 in SelectedUnitPackage.DeviceInformation.Where((DeviceInformation d) => (int)d.DataSource == 1))
				{
					Channel channel2 = SapiManager.GlobalInstance.ActiveChannels.ToList().FirstOrDefault((Channel c) => c.Ecu.Name.Equals(deviceInformation2.Device, StringComparison.OrdinalIgnoreCase));
					SettingsInformation val2 = SelectedUnitPackage.SettingsInformation.FirstOrDefault((SettingsInformation s) => s.SettingsType.Equals("latest", StringComparison.OrdinalIgnoreCase) && s.Device.Equals(deviceInformation2.Device, StringComparison.OrdinalIgnoreCase)) ?? SelectedUnitPackage.SettingsInformation.FirstOrDefault((SettingsInformation s) => s.SettingsType.Equals("oem", StringComparison.OrdinalIgnoreCase) && s.Device.Equals(deviceInformation2.Device, StringComparison.OrdinalIgnoreCase));
					if (channel2 != null && val2 != null)
					{
						list.Add(new ProgrammingData(channel2, SelectedUnitPackage, val2, packageProgrammingOperation: true, SelectedUnitPackageType == "powertrain_newest" && deviceInformation2.HardwareOptionHasNewestForHardware(SapiManager.GetHardwarePartNumber(channel2))));
					}
				}
				break;
			case "vehicle_chec":
				foreach (DeviceInformation deviceInformation in SelectedUnitPackage.DeviceInformation.Where((DeviceInformation d) => (int)d.DataSource == 2 && d.EdexFiles.Any((EdexFileInformation ef) => ef.ConfigurationInformation != null && (ef.ConfigurationInformation.ChecSettings != null || ef.ConfigurationInformation.ApplicableProposedSettingItems().Any()))))
				{
					Channel channel = SapiManager.GlobalInstance.ActiveChannels.ToList().FirstOrDefault((Channel c) => c.Ecu.Name.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase));
					EdexFileInformation val = deviceInformation.EdexFiles.FirstOrDefault((EdexFileInformation ef) => (int)ef.FileType == 3 && !ef.HasErrors && ConfigurationIsValidForChecPackage(channel, ef.ConfigurationInformation)) ?? deviceInformation.EdexFiles.FirstOrDefault((EdexFileInformation ef) => (int)ef.FileType == 2 && !ef.HasErrors && ConfigurationIsValidForChecPackage(channel, ef.ConfigurationInformation)) ?? deviceInformation.EdexFiles.FirstOrDefault((EdexFileInformation ef) => deviceInformation.IsEdexFactoryConfigurationValidForProgramming && (int)ef.FileType == 1 && !ef.HasErrors && ConfigurationIsValidForChecPackage(channel, ef.ConfigurationInformation));
					if (channel != null && val != null)
					{
						list.Add(new ProgrammingData(channel, SelectedUnitPackage, val));
					}
				}
				break;
			}
			dataProvider.ProgrammingData = list;
		}
		else
		{
			ProgrammingData programmingData = null;
			if (selectedOperation == OperationType.Replace)
			{
				UnitInformation selectedUnit = dataProvider.SelectedUnit;
				Channel channel3 = SelectedChannel;
				if (selectedUnit == null)
				{
					throw new DataException(Resources.SelectOperationPage_ErrorSelectedUnitNull);
				}
				if (channel3 == null)
				{
					throw new DataException(Resources.SelectOperationPage_ErrorSelectedChannelNull);
				}
				DeviceDataSource selectedUnitDataSource = selectUnitPanel.SelectedUnitDataSource;
				if ((int)selectedUnitDataSource != 1)
				{
					if ((int)selectedUnitDataSource != 2)
					{
						throw new DataException(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatErrorSelectedDataSourceUnknown, channel3.Ecu.Name, selectedUnit.IdentityKey));
					}
					programmingData = new ProgrammingData(channel3, selectedUnit, selectUnitPanel.SelectedEdexFile);
				}
				else
				{
					programmingData = new ProgrammingData(channel3, selectedUnit, selectUnitPanel.SelectedSettings, packageProgrammingOperation: false, useNewest: false);
				}
			}
			else if (selectedOperation == OperationType.Update)
			{
				Channel channel4 = SelectedChannel;
				FirmwareInformation firmwareInfo = selectSoftwarePanel.SelectedFirmware;
				DeviceInformation informationForDevice = selectSoftwarePanel.Unit.GetInformationForDevice(selectedChannel.Ecu.Name);
				FirmwareOptionInformation val3 = ((informationForDevice != null) ? informationForDevice.FirmwareOptions.FirstOrDefault((FirmwareOptionInformation x) => x.Version == firmwareInfo.Version) : null);
				programmingData = ((val3 == null) ? new ProgrammingData(channel4, firmwareInfo, null, null, selectSoftwarePanel.Unit, null) : new ProgrammingData(channel4, firmwareInfo, val3.BootLoaderKey, val3.ControlListKey, selectSoftwarePanel.Unit, null));
			}
			else
			{
				if (selectedOperation != OperationType.ChangeDataSet)
				{
					throw new DataException(Resources.SelectOperationPage_ErrorNoOperation);
				}
				programmingData = ProgrammingData.CreateFromRequiredDatasetKey(SelectedChannel, selectDataSetPanel.Unit, selectDataSetPanel.SelectedDataSetOption.Key);
				if (!selectDataSetPanel.SelectedDataSetOption.IsCurrent)
				{
					DeviceInformation informationForDevice2 = selectDataSetPanel.Unit.GetInformationForDevice(SelectedChannel.Ecu.Name);
					if (informationForDevice2 != null && informationForDevice2.FirmwareOptions.Count > 1)
					{
						ControlHelpers.ShowMessageBox(Resources.SelectOperationPage_Message_ChangingTheDataSetWillRequireTheDataForThisDeviceToNeedToBeRedownloadedFromTheServerIfYourIntentIsToSubsequentlyUpgradeTheSoftwareInThisDevicePleaseUpgradeTheSoftwareFirst, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
					}
				}
			}
			dataProvider.ProgrammingData = Enumerable.Repeat(programmingData, 1);
			if ((((int)programmingData.DataSource == 1 && programmingData.Firmware != null) || ((int)programmingData.DataSource == 2 && programmingData.EdexFileInformation.ConfigurationInformation != null)) && SelectedChannel != null && !SapiManager.GlobalInstance.IgnoreSoftwareCompatibilityChecks(SelectedChannel) && dataProvider.RequiresCompatibilityChecks)
			{
				DeviceDataSource selectedUnitDataSource = programmingData.DataSource;
				if ((int)selectedUnitDataSource != 1)
				{
					if ((int)selectedUnitDataSource == 2 && ServerDataManager.GlobalInstance.EdexCompatibilityTable.IsCurrentSoftwareCompatible())
					{
						EdexCompatibilityEcuItem val4 = new EdexCompatibilityEcuItem(programmingData.EdexFileInformation.ConfigurationInformation.DeviceName, programmingData.EdexFileInformation.ConfigurationInformation.HardwarePartNumber, programmingData.EdexFileInformation.ConfigurationInformation.FlashwarePartNumber);
						if (!ServerDataManager.GlobalInstance.EdexCompatibilityTable.IsCompatibleWithCurrentSoftware(val4))
						{
							InformUserOfIncompatibility(val4, programmingData.Unit);
						}
					}
				}
				else if (ServerDataManager.GlobalInstance.CompatibilityTable.IsCurrentSoftwareCompatible())
				{
					Software val5 = new Software(programmingData.Firmware.Device, programmingData.Firmware.Version, SapiManager.GetHardwarePartNumber(SelectedChannel));
					if (!programmingData.Unit.UnitFixedAtTest && !ServerDataManager.GlobalInstance.CompatibilityTable.IsCompatibleWithCurrentSoftware(val5) && !ServerDataManager.GlobalInstance.CompatibilityTable.IsAlwaysCompatible(val5))
					{
						InformUserOfIncompatibility(val5, programmingData.Unit);
					}
				}
			}
		}
		List<string> list2 = new List<string>();
		foreach (ProgrammingData data in dataProvider.ProgrammingData)
		{
			if ((((int)data.DataSource == 1 && data.Firmware != null) || ((int)data.DataSource == 2 && data.EdexFileInformation.ConfigurationInformation != null)) && data.GetFlashMeanings(ProgrammingData.FlashBlock.Firmware) == null && ((int)data.DataSource == 1 || !data.TargetChannelHasSameFirmwareVersion || data.FlashRequiredSameFirmwareVersion))
			{
				if (!string.IsNullOrEmpty(data.Firmware.Reference) && (data.Firmware.RequiresDownload || data.Firmware.Status != "OK"))
				{
					throw new DataException(string.Format(CultureInfo.CurrentCulture, data.Firmware.RequiresDownload ? Resources.SelectOperationPage_FormatFirmwareFileStillToBeDownloaded : Resources.SelectOperationPage_FormatFirmwareFileNotAvailableDueToServerStatus, data.Firmware.Key, data.Firmware.Status));
				}
				if (!File.Exists(Path.Combine(Directories.GetDatabasePathForExtension(Path.GetExtension(data.Firmware.FileName)), data.Firmware.FileName)))
				{
					throw new DataException(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatFirmwareFileMissing, data.Firmware.Version, data.Firmware.FileName));
				}
				if (ProgrammingData.GetDiagnosisSourceForFlashware(data.Firmware.FileName) == data.Channel.Ecu.DiagnosisSource)
				{
					throw new DataException(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPage_FormatMeaningCannotBeProgrammed, data.Firmware.Version));
				}
			}
			Tuple<ProgrammingStep, DiagnosisSource> tuple = data.RequiredDiagnosisSources.FirstOrDefault((Tuple<ProgrammingStep, DiagnosisSource> rds) => rds.Item2 != data.Channel.Ecu.DiagnosisSource);
			if (tuple == null)
			{
				continue;
			}
			ConnectionResource targetConnectionResource = data.GetTargetConnectionResource(tuple.Item2);
			if (targetConnectionResource != null)
			{
				if (targetConnectionResource.Type[0] != SapiExtensions.GetActiveConnectionResource(data.Channel).Type[0])
				{
					list2.Add(string.Format(CultureInfo.CurrentCulture, (targetConnectionResource.Type[0] == 'C') ? Resources.SelectOperationPageFormat_PhysicalCANWillBeNeeded : Resources.SelectOperationPageFormat_PhysicalEthernetWillBeNeeded, data.Channel.Ecu.Name, targetConnectionResource.HardwareName));
				}
				continue;
			}
			if (data.Channel.Ecu == null && tuple.Item2 == DiagnosisSource.McdDatabase)
			{
				throw new DataException(string.Format(CultureInfo.CurrentCulture, "Error acquiring connection resource for target diagnosis source {0}.{1} Verify that the SMR-D exists and is valid.", tuple.Item2, Environment.NewLine));
			}
			throw new DataException("Error acquiring connection resource for target diagnosis source " + tuple.Item2);
		}
		if (list2.Count > 0)
		{
			ControlHelpers.ShowMessageBox(string.Join(Environment.NewLine, list2), MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
		}
	}

	private static void InformUserOfIncompatibility(Software targetSoftware, UnitInformation unit)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		MessageBoxIcon messageBoxIcon = MessageBoxIcon.Asterisk;
		CompatibleSoftwareCollection val = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleList(targetSoftware);
		if (((ReadOnlyCollection<SoftwareCollection>)(object)val).Count > 0)
		{
			CompatibleSoftwareCollection val2 = val.FilterForUnit(unit);
			if (((ReadOnlyCollection<SoftwareCollection>)(object)val2).Count > 0)
			{
				CompatibleSoftwareCollection val3 = val2.FilterForOnlineOptions(targetSoftware.Ecu);
				CompatibilityWarningDialog val4 = new CompatibilityWarningDialog(Resources.SelectOperationPage_SoftwareIsNotCompatible_Information, targetSoftware, Resources.SelectOperationPage_Compat_TargetDevice);
				val4.AddCompatibilityInfo(val3, Resources.ProgramDevicePage_Compat_CompatibleSetFormat);
				((Form)(object)val4).ShowDialog();
			}
			else
			{
				messageBoxIcon = MessageBoxIcon.Exclamation;
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_PowertrainSoftwareIsNotCompatible_Warning, targetSoftware);
				ControlHelpers.ShowMessageBox(stringBuilder.ToString(), MessageBoxButtons.OK, messageBoxIcon, MessageBoxDefaultButton.Button1);
			}
		}
		else
		{
			messageBoxIcon = MessageBoxIcon.Hand;
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_PowertrainSoftwareIsNotCompatible_Stop, targetSoftware);
			ControlHelpers.ShowMessageBox(stringBuilder.ToString(), MessageBoxButtons.OK, messageBoxIcon, MessageBoxDefaultButton.Button1);
		}
	}

	private static void InformUserOfIncompatibility(EdexCompatibilityEcuItem targetEcuItem, UnitInformation unit)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		EdexCompatibilityConfigurationCollection val = ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(targetEcuItem);
		if (((ReadOnlyCollection<EdexCompatibilityConfiguration>)(object)val).Count > 0)
		{
			EdexCompatibilityConfigurationCollection val2 = val.FilterForOnlineDevices(targetEcuItem);
			EdexCompatibilityConfigurationCollection val3 = val2.FilterForUnit(unit);
			if (((IEnumerable<EdexCompatibilityConfiguration>)val3).Any((EdexCompatibilityConfiguration cs) => cs.Ecus.Any()))
			{
				CompatibilityWarningDialog val4 = new CompatibilityWarningDialog(Resources.SelectOperationPage_SoftwareIsNotCompatible_Information, targetEcuItem, Resources.SelectOperationPage_Compat_TargetDevice);
				val4.AddCompatibilityInfo(val3, Resources.ProgramDevicePage_Compat_CompatibleSetFormat);
				((Form)(object)val4).ShowDialog();
			}
			else
			{
				IEnumerable<EdexCompatibilityEcuItem> enumerable = default(IEnumerable<EdexCompatibilityEcuItem>);
				EdexCompatibilityConfigurationCollection source = ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(unit, ref enumerable);
				string text = ((((IEnumerable<EdexCompatibilityConfiguration>)source).Any() || !enumerable.Any()) ? string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_Warning, targetEcuItem) : string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_WarningWithHint, targetEcuItem, string.Join(Environment.NewLine, enumerable)));
				ControlHelpers.ShowMessageBox(text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
		}
		else
		{
			string text2 = string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_Stop, targetEcuItem);
			ControlHelpers.ShowMessageBox(text2, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}
	}

	private void SapiManagerChannelChangedEvent(object sender, EventArgs e)
	{
		if (base.Wizard.ActivePage == this)
		{
			BuildDeviceList();
		}
	}

	private void GlobalInstance_ChannelIdentificationChanged(object sender, ChannelIdentificationChangedEventArgs e)
	{
		if (base.Wizard.ActivePage == this)
		{
			BuildDeviceList();
		}
	}

	private void OnDisconnectCompleteEvent(object sender, EventArgs e)
	{
		if (base.Wizard.ActivePage == this && sender is Channel)
		{
			BuildDeviceList();
		}
	}

	private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender is Channel channel)
		{
			channel.EcuInfos.EcuInfosReadCompleteEvent += c_EcuInfosReadCompleteEvent;
		}
	}

	private void c_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
	{
		if (base.Wizard.ActivePage == this)
		{
			BuildDeviceList();
		}
	}

	private void BuildDeviceList()
	{
		if (!dirty)
		{
			dirty = true;
			((Control)(object)base.Wizard).BeginInvoke((Delegate)(Action)delegate
			{
				dirty = false;
				BuildDeviceListDeferred();
			});
		}
	}

	private void BuildDeviceListDeferred()
	{
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		bool flag2 = false;
		List<string> list = new List<string>();
		Sapi sapi = SapiManager.GlobalInstance.Sapi;
		if (sapi != null && SapiManager.GlobalInstance.ActiveChannels.Any())
		{
			UnitInformation selectedUnit = dataProvider.SelectedUnit;
			flag2 = AddDepotPackageListItems(selectedUnit, list, flag2);
			flag2 = AddEdexPackageListItems(selectedUnit, list, flag2);
			foreach (Channel item in SapiManager.GlobalInstance.ActiveChannels.OrderBy((Channel ch) => ch.Ecu.Priority))
			{
				Ecu channelEcu = null;
				bool flag3 = false;
				if (item.IsRollCall && (channelEcu = SapiExtensions.GetSuppressedOfflineRelatedEcu(item)) != null && !SapiManager.GlobalInstance.ActiveChannels.Any((Channel ac) => ac.Ecu == channelEcu))
				{
					flag3 = true;
				}
				else
				{
					channelEcu = item.Ecu;
					if ((item.DiagnosisVariant.IsBase && SapiExtensions.IsDataSourceDepot(item.Ecu)) || !SapiManager.SupportsReprogramming(item.Ecu))
					{
						continue;
					}
				}
				if ((!ApplicationInformation.CanReprogramEdexUnits && SapiExtensions.IsDataSourceEdex(channelEcu) && !ApplicationInformation.CanReprogramEdexEcu(channelEcu.Name)) || (!SapiExtensions.IsDataSourceDepot(channelEcu) && !SapiExtensions.IsDataSourceEdex(channelEcu)))
				{
					continue;
				}
				if (flag3)
				{
					list.Add(ReprogrammingView.CreateInput("deviceList", "manual_" + channelEcu.Name, channelEcu.Name + " - " + channelEcu.ShortDescription, Resources.SelectOperationPageItem_ManualConnection, channelEcu.Name == manualConnectionEcu, disabled: false));
					if (channelEcu.Name == manualConnectionEcu)
					{
						flag = true;
					}
					continue;
				}
				bool flag4 = false;
				string status = string.Empty;
				UnitInformation ui = dataProvider.SelectedUnit;
				if (ui != null)
				{
					string hardwarePartNumber = SapiManager.GetHardwarePartNumber(item);
					Part part = ((hardwarePartNumber != null) ? new Part(hardwarePartNumber) : null);
					DeviceInformation informationForDevice = ui.GetInformationForDevice(channelEcu.Name);
					if (informationForDevice != null && (ui.HasSettingsForDevice(channelEcu.Name) || ui.GetPresetSettingsForDevice(channelEcu.Name) != null || SapiManager.GetBootModeStatus(item)))
					{
						if (!SapiExtensions.IsDataSourceEdex(channelEcu) && ui.GetInformationForDevice(channelEcu.Name).FirmwareOptions.Any() && !informationForDevice.FirmwareOptionSupportsHardware(part))
						{
							flag4 = true;
							status = Resources.SelectUnitSubPanelItem_NoUnitDataForConnectedHardware;
						}
						else
						{
							status = ui.GetStatusDisplayTextForDevice(channelEcu.Name, part);
						}
					}
					else
					{
						flag4 = true;
						DeviceDataSource dataSource = (DeviceDataSource)(SapiExtensions.IsDataSourceDepot(channelEcu) ? 1 : 2);
						if (informationForDevice != null)
						{
							status = ui.GetStatusDisplayTextForDevice(channelEcu.Name, part);
						}
						else if (!SapiExtensions.IsDataSourceDepot(channelEcu) && !SapiExtensions.IsDataSourceEdex(channelEcu))
						{
							status = Resources.SelectOperationPage_DeviceWarning_NotProgrammable;
						}
						else if (!ui.DeviceInformation.Any((DeviceInformation deviceInfo) => deviceInfo.DataSource == dataSource))
						{
							status = string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPage_DeviceWarning_FormatNoContentForDataSource, dataSource);
						}
						else if (ui.MissingDevices.Contains(channelEcu.Name))
						{
							status = Resources.SelectOperationPage_DeviceWarning_NoServerData;
						}
						else
						{
							DeviceInformation val = (from e in SapiManager.GlobalInstance.Sapi.Ecus
								where e.Identifier == channelEcu.Identifier
								select ui.GetInformationForDevice(e.Name)).FirstOrDefault((DeviceInformation altDeviceInfo) => altDeviceInfo != null);
							if (val != null)
							{
								status = string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ConnectedDeviceNotAMatch, val.Device);
							}
							else
							{
								IEnumerable<ElectronicsFamily> source = ui.PossibleFamilies.Where((ElectronicsFamily pf) => SapiManager.IsNotProgrammableForFamily(channelEcu, pf));
								status = ((!source.Any()) ? Resources.SelectOperationPage_DeviceWarning_NoOptionalServerData : string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_NotProgrammableForFamily, string.Join(", ", source.Select((ElectronicsFamily f) => ((ElectronicsFamily)(ref f)).Name))));
							}
						}
					}
				}
				list.Add(ReprogrammingView.CreateInput("deviceList", channelEcu.Name, channelEcu.Name + " - " + item.Ecu.ShortDescription, status, !flag4 && (item == selectedChannel || channelEcu.Name == manualConnectionEcu), flag4));
				if (!flag4 && (item == selectedChannel || channelEcu.Name == manualConnectionEcu))
				{
					flag = true;
					if (selectedChannel == null)
					{
						selectedChannel = item;
						manualConnectionEcu = null;
					}
				}
			}
		}
		if (list.Count > 0)
		{
			deviceListPane.InnerHtml = ReprogrammingView.CreateRadioList(Resources.SelectOperationPage_SelectDevice, list);
		}
		else
		{
			deviceListPane.InnerHtml = FormattableString.Invariant($"<p><span class='warninginline'>{Resources.SelectOperationPageItem_NoDevicesAreConnected}</span></p>");
		}
		if (!ReprogrammingView.SubscribeInputEvents(deviceListPane, "radio", "onchange", deviceList_onChange))
		{
			if (!flag)
			{
				selectedChannel = null;
				manualConnectionEcu = null;
			}
			if (!flag2)
			{
				selectedUnitPackage = null;
			}
			UpdateDeviceSelection();
		}
	}

	private bool AddDepotPackageListItems(UnitInformation connectedUnit, List<string> listItems, bool foundSelectedPackage)
	{
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Expected O, but got Unknown
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Expected O, but got Unknown
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Expected O, but got Unknown
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Expected O, but got Unknown
		if (connectedUnit != null && (int)connectedUnit.GetStatusForDataSource((DeviceDataSource)1) == 2 && connectedUnit.DeviceInformation.Any((DeviceInformation di) => (int)di.DataSource == 1) && SapiManager.GlobalInstance.ActiveChannels.Any((Channel c) => SapiExtensions.IsDataSourceDepot(c.Ecu)))
		{
			List<string> list = new List<string>();
			bool flag = true;
			bool flag2 = false;
			Dictionary<DeviceInformation, Channel> dictionary = connectedUnit.DeviceInformation.Where((DeviceInformation d) => (int)d.DataSource == 1).ToDictionary((DeviceInformation k) => k, (DeviceInformation v) => SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel c) => c.Ecu.Name == v.Device));
			Collection<Software> collection = new Collection<Software>();
			Collection<Software> collection2 = new Collection<Software>();
			if (dictionary.All((KeyValuePair<DeviceInformation, Channel> kv) => kv.Value != null))
			{
				foreach (DeviceInformation deviceInformation in dictionary.Keys)
				{
					bool flag3 = false;
					Channel channel = dictionary[deviceInformation];
					string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
					if (string.IsNullOrEmpty(hardwarePartNumber))
					{
						list.Add(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_HardwarePartNumberInvalid, deviceInformation.Device));
					}
					else
					{
						Part part = new Part(hardwarePartNumber);
						if (!deviceInformation.FirmwareOptionSupportsHardware(part))
						{
							list.Add(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ConnectedHardwareNotAMatch, deviceInformation.Device, part));
						}
						else if (connectedUnit.SettingsInformation.FirstOrDefault((SettingsInformation s) => s.SettingsType.Equals("latest", StringComparison.OrdinalIgnoreCase) && s.Device.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase)) == null && connectedUnit.SettingsInformation.FirstOrDefault((SettingsInformation s) => s.SettingsType.Equals("oem", StringComparison.OrdinalIgnoreCase) && s.Device.Equals(deviceInformation.Device, StringComparison.OrdinalIgnoreCase)) == null)
						{
							list.Add(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_OEMLatestDataNotAvailable, connectedUnit.VehicleIdentity));
						}
						flag &= deviceInformation.HardwareOptionHasCurrentForHardware(hardwarePartNumber);
						flag3 = deviceInformation.HardwareOptionHasNewestForHardware(hardwarePartNumber);
					}
					if (list.Count == 0)
					{
						if (flag)
						{
							try
							{
								ProgrammingData programmingData = new ProgrammingData(channel, connectedUnit, null, packageProgrammingOperation: true, useNewest: false, previewOnly: true);
								if (programmingData.Firmware != null && programmingData.Firmware.Version != null)
								{
									collection.Add(new Software(deviceInformation.Device, programmingData.Firmware.Version));
								}
							}
							catch (DataException ex)
							{
								StatusLog.Add(new StatusMessage(ex.Message, (StatusMessageType)2, (object)this));
								flag = false;
							}
						}
						if (flag || flag3)
						{
							try
							{
								ProgrammingData programmingData2 = new ProgrammingData(channel, connectedUnit, null, packageProgrammingOperation: true, flag3, previewOnly: true);
								if (programmingData2.Firmware != null && programmingData2.Firmware.Version != null)
								{
									collection2.Add(new Software(deviceInformation.Device, programmingData2.Firmware.Version));
								}
							}
							catch (DataException ex2)
							{
								StatusLog.Add(new StatusMessage(ex2.Message, (StatusMessageType)2, (object)this));
								flag3 = false;
							}
						}
					}
					flag2 = flag2 || flag3;
				}
			}
			else
			{
				list.Add(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_RequiredDeviceNotConnected, string.Join(", ", from kv in dictionary
					where kv.Value == null
					select kv.Key.Device)));
			}
			foundSelectedPackage = ((list.Count == 0 && !flag) ? AddDepotPackageListItem(connectedUnit, listItems, foundSelectedPackage, new List<string> { Resources.SelectOperationPage_CurrentSoftwareNotFoundForHardware }, newest: false) : ((list.Count != 0 || ServerDataManager.GlobalInstance.CompatibilityTable.IsCompatible(new SoftwareCollection(collection))) ? AddDepotPackageListItem(connectedUnit, listItems, foundSelectedPackage, list, newest: false) : AddDepotPackageListItem(connectedUnit, listItems, foundSelectedPackage, new List<string> { Resources.SelectOperationPage_NoCompatibleSoftwareForHardware }, newest: false)));
			if (flag2 && list.Count == 0 && collection2.Count == dictionary.Count && ServerDataManager.GlobalInstance.CompatibilityTable.IsCompatible(new SoftwareCollection(collection2)))
			{
				foundSelectedPackage = AddDepotPackageListItem(connectedUnit, listItems, foundSelectedPackage, list, newest: true);
			}
		}
		return foundSelectedPackage;
	}

	private bool AddDepotPackageListItem(UnitInformation connectedUnit, List<string> listItems, bool foundSelectedPackage, List<string> reasonUnitCantBeAdded, bool newest)
	{
		if (reasonUnitCantBeAdded.Count == 0)
		{
			bool flag = SelectedUnitPackageType == (newest ? "powertrain_newest" : "powertrain_current") && SelectedUnitPackage == connectedUnit;
			listItems.Add(ReprogrammingView.CreateInput("deviceList", newest ? "powertrain_newest" : "powertrain_current", newest ? Resources.SelectOperationPage_NewestConnectedPowertrain : Resources.SelectOperationPage_CurrentConnectedPowertrain, string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_Unit, connectedUnit.IdentityKey), flag, disabled: false));
			if (flag)
			{
				foundSelectedPackage = true;
			}
		}
		else
		{
			listItems.Add(ReprogrammingView.CreateInput("deviceList", newest ? "powertrain_newest" : "powertrain_current", newest ? Resources.SelectOperationPage_NewestConnectedPowertrain : Resources.SelectOperationPage_CurrentConnectedPowertrain, string.Join(" <br/> ", reasonUnitCantBeAdded), selected: false, disabled: true));
		}
		return foundSelectedPackage;
	}

	private bool AddEdexPackageListItems(UnitInformation connectedUnit, List<string> listItems, bool foundSelectedPackage)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		if (connectedUnit != null && (int)connectedUnit.GetStatusForDataSource((DeviceDataSource)2) == 2 && connectedUnit.EdexFileServerErrors.Count == 0 && connectedUnit.DeviceInformation.Any((DeviceInformation di) => (int)di.DataSource == 2))
		{
			Dictionary<DeviceInformation, Channel> dictionary = connectedUnit.DeviceInformation.Where((DeviceInformation di) => (int)di.DataSource == 2 && (di.EdexFiles.FirstOrDefault(delegate(EdexFileInformation ef)
			{
				EdexConfigurationInformation configurationInformation = ef.ConfigurationInformation;
				return ((configurationInformation != null) ? configurationInformation.ChecSettings : null) != null;
			}) != null || di.EdexFiles.FirstOrDefault((EdexFileInformation ef) => ef.ConfigurationInformation != null && ef.ConfigurationInformation.ApplicableProposedSettingItems().Any()) != null)).ToDictionary((DeviceInformation k) => k, (DeviceInformation v) => SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel c) => c.Ecu.Name == v.Device));
			if (dictionary.Count > 0)
			{
				List<string> list = new List<string>();
				if (dictionary.All((KeyValuePair<DeviceInformation, Channel> kv) => kv.Value != null))
				{
					foreach (KeyValuePair<DeviceInformation, Channel> deviceChannel in dictionary)
					{
						EdexFileInformation val = deviceChannel.Key.EdexFiles.FirstOrDefault((EdexFileInformation ef) => (int)ef.FileType == 3 && !ef.HasErrors && ConfigurationIsValidForChecPackage(deviceChannel.Value, ef.ConfigurationInformation)) ?? deviceChannel.Key.EdexFiles.FirstOrDefault((EdexFileInformation ef) => (int)ef.FileType == 2 && !ef.HasErrors && ConfigurationIsValidForChecPackage(deviceChannel.Value, ef.ConfigurationInformation)) ?? deviceChannel.Key.EdexFiles.FirstOrDefault((EdexFileInformation ef) => deviceChannel.Key.IsEdexFactoryConfigurationValidForProgramming && (int)ef.FileType == 1 && !ef.HasErrors && ConfigurationIsValidForChecPackage(deviceChannel.Value, ef.ConfigurationInformation));
						if (val == null)
						{
							list.Add(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_ConfigurationDataNotConsistent, deviceChannel.Value.Ecu.DisplayName));
						}
					}
				}
				else
				{
					list.Add(string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_RequiredDeviceNotConnected, string.Join(", ", from kv in dictionary
						where kv.Value == null
						select kv.Key.Device)));
				}
				if (list.Count == 0 && connectedUnit != null)
				{
					bool flag = SelectedUnitPackageType == "vehicle_chec" && SelectedUnitPackage == connectedUnit;
					listItems.Add(ReprogrammingView.CreateInput("deviceList", "vehicle_chec", "CHEC", string.Format(CultureInfo.CurrentCulture, Resources.SelectOperationPageFormat_SettingsForChecConnectedDevices, connectedUnit.IdentityKey), flag, disabled: false));
					if (flag)
					{
						foundSelectedPackage = true;
					}
				}
				else
				{
					listItems.Add(ReprogrammingView.CreateInput("deviceList", "vehicle_chec", Resources.SelectOperationPage_Chec, string.Join(" <br/> ", list), selected: false, disabled: true));
				}
			}
			else
			{
				StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Unit {0} does not contain CHEC data for package programming", connectedUnit.IdentityKey), (StatusMessageType)2, (object)this));
			}
		}
		return foundSelectedPackage;
	}

	private static bool ConfigurationIsValidForChecPackage(Channel channel, EdexConfigurationInformation configurationInformation)
	{
		if (configurationInformation != null && (configurationInformation.ChecSettings != null || configurationInformation.ApplicableProposedSettingItems().Any()) && PartExtensions.IsEqual(configurationInformation.HardwarePartNumber, SapiManager.GetHardwarePartNumber(channel)))
		{
			return configurationInformation.AllFlashwarePartNumbers.Any((Part fp) => PartExtensions.IsEqual(fp, SapiManager.GetSoftwarePartNumber(channel)));
		}
		return false;
	}

	private void UpdateSubPanel()
	{
		settingsSubPanelPane.SetAttribute("className", (selectedOperation == OperationType.Replace) ? "show" : "hide");
		softwareSubPanelPane.SetAttribute("className", (selectedOperation == OperationType.Update) ? "show" : "hide");
		datasetSubPanelPane.SetAttribute("className", (selectedOperation == OperationType.ChangeDataSet) ? "show" : "hide");
		SetWizardButtons();
	}

	private void UpdateDeviceSelection()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		string innerHtml = "&nbsp;";
		Channel channel = SelectedChannel;
		if (channel != null)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			if ((int)GetDeviceDataSource(channel) == 2)
			{
				flag2 = false;
				flag3 = false;
				selectedOperation = OperationType.Replace;
			}
			else
			{
				if (SapiManager.GetBootModeStatus(channel))
				{
					flag2 = false;
					if (ProgrammingData.GetPreviousUpgradeSettingsFile(channel.Ecu.Name) == null)
					{
						flag3 = false;
					}
				}
				else if (!HasDataSet(channel))
				{
					flag2 = false;
				}
				if (!flag2 && selectedOperation == OperationType.ChangeDataSet)
				{
					selectedOperation = OperationType.Replace;
				}
				if (!flag && selectedOperation == OperationType.Replace)
				{
					selectedOperation = OperationType.Update;
				}
				if (!flag3 && selectedOperation == OperationType.Update)
				{
					selectedOperation = OperationType.Replace;
				}
			}
			List<string> list = new List<string>();
			if (flag)
			{
				list.Add(ReprogrammingView.CreateInput("operation", "Replace", Resources.ProgrammingOperation_ReplaceDeviceSettingsWithServerConfiguration, null, selectedOperation == OperationType.Replace, disabled: false));
			}
			if (flag3)
			{
				list.Add(ReprogrammingView.CreateInput("operation", "Update", Resources.ProgrammingOperation_UpdateDeviceSoftware, null, selectedOperation == OperationType.Update, disabled: false));
			}
			if (flag2)
			{
				list.Add(ReprogrammingView.CreateInput("operation", "ChangeDataSet", Resources.ProgrammingOperation_ChangeDataset, null, selectedOperation == OperationType.ChangeDataSet, disabled: false));
			}
			innerHtml = FormattableString.Invariant(FormattableStringFactory.Create("<p><div>{0}</div>{1}</p>", Resources.SelectOperationPage_SelectOperation, string.Join(" ", list)));
			selectUnitPanel.UpdateSettingsList(channel);
			if (SapiExtensions.IsDataSourceDepot(channel.Ecu))
			{
				selectDataSetPanel.UpdateDataSetList(channel);
				selectSoftwarePanel.UpdateFirmwareList(channel);
			}
			operationPane.SetAttribute("className", "show");
		}
		else
		{
			selectedOperation = OperationType.None;
			operationPane.SetAttribute("className", "hide");
		}
		operationPane.InnerHtml = innerHtml;
		ReprogrammingView.SubscribeInputEvents(operationPane, "radio", "onchange", operation_onChange);
		manualConnectionPane.SetAttribute("className", (manualConnectionEcu != null) ? "show" : "hide");
		if (manualConnectionEcu != null)
		{
			ReprogrammingView.SetButtonState(connectButton, enabled: true, show: true);
			ReprogrammingView.EnableElement(connectionResourcesCombobox, enabled: true);
			PopulateConnectionResources();
		}
		UpdateSubPanel();
		SetWizardButtons();
	}

	private DeviceDataSource GetDeviceDataSource(Channel channel)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		UnitInformation val = null;
		DeviceInformation val2 = null;
		DeviceDataSource result = (DeviceDataSource)0;
		if (channel != null)
		{
			val = dataProvider.SelectedUnit;
			val2 = ((val == null) ? null : (val2 = val.GetInformationForDevice(channel.Ecu.Name)));
			if (val2 != null)
			{
				result = val2.DataSource;
			}
			else if (channel.Ecu != null)
			{
				if (SapiExtensions.IsDataSourceEdex(channel.Ecu))
				{
					result = (DeviceDataSource)2;
				}
				else if (SapiExtensions.IsDataSourceDepot(channel.Ecu))
				{
					result = (DeviceDataSource)1;
				}
			}
		}
		return result;
	}

	private bool HasDataSet(Channel channel)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		UnitInformation val = null;
		DeviceInformation val2 = null;
		if (channel != null)
		{
			val = dataProvider.SelectedUnit;
			if (val != null)
			{
				val2 = val.GetInformationForDevice(channel.Ecu.Name);
				if (val2 != null && val2.HasDataSet && (int)val.GetStatusForDataSource(val2.DataSource) == 2)
				{
					return val2.HasDataSet;
				}
			}
		}
		return false;
	}

	private void operation_onChange(object sender, EventArgs args)
	{
		HtmlElement htmlElement = sender as HtmlElement;
		string value = (htmlElement.DomElement as IHTMLInputElement).value;
		selectedOperation = (OperationType)Enum.Parse(typeof(OperationType), value);
		UpdateSubPanel();
	}

	private void deviceList_onChange(object sender, EventArgs args)
	{
		manualConnectionEcu = null;
		selectedUnitPackage = null;
		selectedChannel = null;
		HtmlElement htmlElement = sender as HtmlElement;
		ReprogrammingView.SetActiveRadioElement(htmlElement);
		string result = (htmlElement.DomElement as IHTMLInputElement).value;
		if (result == "powertrain_current" || result == "powertrain_newest" || result == "vehicle_chec")
		{
			selectedUnitPackage = dataProvider.SelectedUnit;
			selectedUnitPackageType = result;
		}
		else if (result.StartsWith("manual_", StringComparison.Ordinal))
		{
			manualConnectionEcu = result.Substring("manual_".Length);
		}
		else
		{
			selectedChannel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel c) => c.Ecu.Name == result);
		}
		UpdateDeviceSelection();
	}

	private void ButtonConnectClick()
	{
		string selectedConnectionResource = ((IHTMLSelectElement)connectionResourcesCombobox.DomElement).value;
		SapiManager.GlobalInstance.OpenConnection(SapiManager.GetEcuByName(manualConnectionEcu)?.GetConnectionResources().FirstOrDefault((ConnectionResource cr) => cr.ToString() == selectedConnectionResource));
		ReprogrammingView.SetButtonState(connectButton, enabled: false, show: true);
		ReprogrammingView.EnableElement(connectionResourcesCombobox, enabled: false);
	}

	private void PopulateConnectionResources()
	{
		bool flag = (showAllConnectionResourcesCheckbox.DomElement as IHTMLInputElement).@checked;
		List<string> list = new List<string>();
		bool flag2 = false;
		Sapi sapi = SapiManager.GlobalInstance.Sapi;
		if (sapi == null)
		{
			return;
		}
		Ecu ecu = sapi.Ecus[manualConnectionEcu];
		if (ecu != null && sapi.Ecus.GetConnectedCountForIdentifier(ecu.Identifier) == 0)
		{
			ConnectionResourceCollection connectionResources = ecu.GetConnectionResources();
			foreach (ConnectionResource item in connectionResources)
			{
				if (flag || !item.Restricted)
				{
					list.Add(FormattableString.Invariant($"<option value='{item.ToString()}'>{SapiExtensions.ToDisplayString(item)}</option>"));
				}
				else
				{
					flag2 = true;
				}
			}
		}
		if (list.Count == 0)
		{
			if (flag2 && ecu.ViaEcus.Any())
			{
				list.Add(FormattableString.Invariant(FormattableStringFactory.Create("<option>{0}</option>", string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ResourceCannotBeDetermined, string.Join("/", ecu.ViaEcus)))));
			}
			else
			{
				list.Add(FormattableString.Invariant($"<option>{Resources.Messsage_NoConnectionResources}</option>"));
			}
		}
		ReprogrammingView.SetButtonState(connectButton, list.Count > 0, show: true);
		connectionResourcesCombobox.InnerHtml = string.Join(" ", list);
	}

	private void checkBoxShowAll_CheckedChanged(object sender, EventArgs e)
	{
		PopulateConnectionResources();
	}
}
