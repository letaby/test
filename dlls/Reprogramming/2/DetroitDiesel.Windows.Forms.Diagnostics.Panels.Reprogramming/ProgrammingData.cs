using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class ProgrammingData
{
	public enum FlashBlock
	{
		BootLoader,
		Firmware,
		DataSet,
		ControlList
	}

	private const string ExistingSettingsFileNamePrefix = "preupgrade";

	private const string ExistingSettingsFileSearchPath = "HF4FK60H*.*";

	private const string AttemptInfoFileName = "ddrsattempt.dat";

	private const string AutomaticChargeType = "AUTOMATIC";

	private Channel channel;

	private ProgrammingOperation operation;

	private UnitInformation unit;

	private SettingsInformation settings;

	private EdexFileInformation edexFileInformation;

	private FirmwareInformation bootcode;

	private FirmwareInformation controlList;

	private FirmwareInformation firmware;

	private DataSetOptionInformation dataSet;

	private string engineSerialNumber;

	private string vehicleIdentificationNumber;

	private string chargeType;

	private string chargeText;

	private string previousEngineSerialNumber = string.Empty;

	private string previousVehicleIdentificationNumber = string.Empty;

	private string previousSoftwareVersion = string.Empty;

	private string previousHardwareRevision = string.Empty;

	private string previousDiagnosticVariant = string.Empty;

	private bool replaceToSameDevice;

	private bool packageProgrammingOperation;

	private DeviceDataSource dataSource;

	public Channel Channel
	{
		get
		{
			return channel;
		}
		set
		{
			channel = value;
		}
	}

	public ProgrammingOperation Operation => operation;

	public UnitInformation Unit => unit;

	public SettingsInformation Settings => settings;

	public EdexFileInformation EdexFileInformation => edexFileInformation;

	public FirmwareInformation Bootcode => bootcode;

	public FirmwareInformation ControlList => controlList;

	public FirmwareInformation Firmware => firmware;

	public DataSetOptionInformation DataSet => dataSet;

	public bool HasDataSet
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			if ((int)DataSource != 1 || DataSet == null)
			{
				if ((int)DataSource == 2)
				{
					if (!EdexFileInformation.ConfigurationInformation.DataSetPartNumbers.Any())
					{
						return !string.IsNullOrEmpty(EdexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile);
					}
					return true;
				}
				return false;
			}
			return true;
		}
	}

	public bool DeferBootModeCheck
	{
		get
		{
			if (Channel.Ecu.Properties.ContainsKey("DeferBootModeCheck"))
			{
				return Convert.ToBoolean(Channel.Ecu.Properties["DeferBootModeCheck"], CultureInfo.InvariantCulture);
			}
			return false;
		}
	}

	public bool FlashRequiredSameFirmwareVersion
	{
		get
		{
			if (Channel.Ecu.Properties.ContainsKey("FlashRequiredSameFirmwareVersion"))
			{
				return Convert.ToBoolean(Channel.Ecu.Properties["FlashRequiredSameFirmwareVersion"], CultureInfo.InvariantCulture);
			}
			return false;
		}
	}

	public string EngineSerialNumber => engineSerialNumber;

	public string VehicleIdentificationNumber => vehicleIdentificationNumber;

	public string ChargeType
	{
		get
		{
			if (AutomaticOperation != null)
			{
				return "AUTOMATIC";
			}
			return chargeType;
		}
		set
		{
			chargeType = value;
		}
	}

	public string ChargeText
	{
		get
		{
			if (AutomaticOperation != null)
			{
				return AutomaticOperation.Time;
			}
			return chargeText;
		}
		set
		{
			chargeText = value;
		}
	}

	public string PreviousEngineSerialNumber => previousEngineSerialNumber;

	public string PreviousVehicleIdentificationNumber => previousVehicleIdentificationNumber;

	public string PreviousSoftwareVersion => previousSoftwareVersion;

	public string PreviousHardwareRevision => previousHardwareRevision;

	public string PreviousDiagnosticVariant => previousDiagnosticVariant;

	public bool ReplaceToSameDevice => replaceToSameDevice;

	public string AttemptInfoPath => Path.Combine(Path.GetTempPath(), channel.Ecu.Name + "ddrsattempt.dat");

	public bool PackageProgrammingOperation => packageProgrammingOperation;

	public DeviceDataSource DataSource => dataSource;

	public AutomaticOperation AutomaticOperation { get; private set; }

	public bool TargetChannelHasSameFirmwareVersion
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I4
			if (Channel == null)
			{
				throw new InvalidOperationException("The Channel property must not be null.");
			}
			DeviceDataSource val = DataSource;
			if ((int)val != 1)
			{
				if ((int)val == 2)
				{
					if (EdexFileInformation == null)
					{
						throw new InvalidOperationException("The EdexFileInformation property must not be null.");
					}
					if (SapiManager.ProgramDeviceUsesSoftwareIdentification(Channel.Ecu))
					{
						return EdexFileInformation.ConfigurationInformation.SoftwareIdentification.Equals(SapiManager.GetSoftwareIdentification(Channel), StringComparison.OrdinalIgnoreCase);
					}
					return PartExtensions.IsEqual(EdexFileInformation.ConfigurationInformation.FlashwarePartNumber, SapiManager.GetSoftwarePartNumber(Channel));
				}
				throw new InvalidOperationException("The DataSource type is unknown or unsupported.");
			}
			if (Firmware == null)
			{
				throw new InvalidOperationException("The Firmware property must not be null.");
			}
			return string.Equals(Firmware.Version, SapiManager.GetSoftwareVersion(Channel), StringComparison.OrdinalIgnoreCase);
		}
	}

	public IEnumerable<Part> DataSetVersions
	{
		get
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Invalid comparison between Unknown and I4
			if (channel == null)
			{
				throw new InvalidOperationException("The Channel property must not be null.");
			}
			if (!HasDataSet)
			{
				throw new InvalidOperationException("The dataset information must not be null.");
			}
			List<Part> list = new List<Part>();
			if ((int)DataSource == 2)
			{
				if (!string.IsNullOrEmpty(EdexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile))
				{
					return Enumerable.Repeat(new Part(SapiManager.GetLateBoundDataSetHexFileCff(channel).Split(',')[1]), 1);
				}
				list.AddRange(EdexFileInformation.ConfigurationInformation.DataSetPartNumbers);
			}
			else if ((int)DataSource == 1)
			{
				list.Add(new Part(DataSet.Key));
			}
			return list;
		}
	}

	public unsafe IEnumerable<Part> TargetChannelNeededDataSetVersions
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if (((int)DataSource == 2 && !string.IsNullOrEmpty(EdexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile)) || (Firmware != null && (!TargetChannelHasSameFirmwareVersion || FlashRequiredSameFirmwareVersion || SapiManager.GetBootModeStatus(Channel))))
			{
				return DataSetVersions;
			}
			return DataSetVersions.Where((Part dsFlash) => !SapiManager.GetDataSetPartNumbers(Channel).Any(new Func<string, bool>(dsFlash, (nint)(delegate*<Part, string, bool>)(&PartExtensions.IsEqual))));
		}
	}

	public bool TargetChannelIsValidForFirmware
	{
		get
		{
			DeviceInformation informationForDevice = Unit.GetInformationForDevice(Channel.Ecu.Name);
			string hardwarePartNumber = SapiManager.GetHardwarePartNumber(Channel);
			string hardwareRevision = SapiManager.GetHardwareRevision(Channel);
			Part part = ((!string.IsNullOrEmpty(hardwarePartNumber)) ? new Part(hardwarePartNumber) : null);
			if (part != null || !string.IsNullOrEmpty(hardwareRevision))
			{
				return informationForDevice.FirmwareOptionAvailableForHardware(Firmware, part, hardwareRevision);
			}
			return true;
		}
	}

	public NameValueCollection NameValueCollection
	{
		get
		{
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Invalid comparison between Unknown and I4
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Invalid comparison between Unknown and I4
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (AutomaticOperation != null)
			{
				nameValueCollection.Add(Resources.ProgrammingDataItem_AutomaticOperation, string.Format(CultureInfo.InvariantCulture, "{0} - {1} - {2}", AutomaticOperation.Reason, Sapi.TimeFromString(AutomaticOperation.Time).ToShortDateString(), OperationToDisplayString(operation, replaceToSameDevice)));
			}
			else
			{
				nameValueCollection.Add(Resources.ProgrammingDataItem_Operation, OperationToDisplayString(operation, replaceToSameDevice));
			}
			nameValueCollection.Add(Resources.ProgrammingDataItem_Device, channel.Ecu.Name);
			if (unit != null)
			{
				nameValueCollection.Add(Resources.ProgrammingDataItem_Unit, unit.IdentityKey);
			}
			if (firmware != null)
			{
				if ((int)DataSource == 2 && edexFileInformation != null)
				{
					Part part = edexFileInformation.ConfigurationInformation.BootLoaderPartNumber;
					if (part != null)
					{
						FlashMeaning flashMeaning = channel.FlashAreas.SelectMany((FlashArea fa) => fa.FlashMeanings).FirstOrDefault((FlashMeaning m) => PartExtensions.IsEqual(part, m.FlashKey));
						nameValueCollection.Add(Resources.ProgrammingDataItem_BootSoftware, (flashMeaning != null) ? string.Format(CultureInfo.CurrentCulture, "{0} ({1})", part, flashMeaning.Name) : part.ToString());
					}
					string value = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", new Part(firmware.Key).ToString(), firmware.Version);
					nameValueCollection.Add(Resources.ProgrammingDataItem_Software, value);
				}
				else
				{
					nameValueCollection.Add(Resources.ProgrammingDataItem_Software, firmware.Version);
				}
			}
			if (dataSet != null && !string.IsNullOrEmpty(dataSet.Key) && !string.IsNullOrEmpty(dataSet.Description))
			{
				nameValueCollection.Add(Resources.ProgrammingDataItem_Dataset, string.Format(CultureInfo.CurrentCulture, "{0} ({1})", new Part(dataSet.Key).ToString(), dataSet.Description));
			}
			if (operation != ProgrammingOperation.ChangeDataSet)
			{
				nameValueCollection.Add(value: (settings != null) ? settings.SettingsType : ((edexFileInformation != null) ? edexFileInformation.CompleteFileType : ((operation != ProgrammingOperation.Update) ? Resources.ProgrammingDataItemWarning_ResetToDefaultNotRecommended : Resources.ProgrammingDataItemWarning_NoExistingSettingsResetToDefault)), name: Resources.ProgrammingDataItem_Settings);
			}
			if (engineSerialNumber != null && engineSerialNumber.Length > 0)
			{
				nameValueCollection.Add(Resources.ProgrammingDataItem_EngineSerialNumber, engineSerialNumber);
			}
			if (vehicleIdentificationNumber != null && vehicleIdentificationNumber.Length > 0)
			{
				nameValueCollection.Add(Resources.ProgrammingDataItem_VehicleIdentificationNumber, vehicleIdentificationNumber);
			}
			if ((int)DataSource == 2 && edexFileInformation != null && !edexFileInformation.HasErrors)
			{
				if (edexFileInformation.ConfigurationInformation.HardwarePartNumber != null)
				{
					nameValueCollection.Add(Resources.ProgrammingDataItem_HardwarePartNumber, PartExtensions.ToHardwarePartNumberString(edexFileInformation.ConfigurationInformation.HardwarePartNumber, edexFileInformation.ConfigurationInformation.DeviceName, true));
				}
				if (edexFileInformation.ConfigurationInformation.DataSetPartNumbers != null && edexFileInformation.ConfigurationInformation.DataSetPartNumbers.Any())
				{
					for (int num = 0; num < edexFileInformation.ConfigurationInformation.DataSetPartNumbers.Count; num++)
					{
						Part part2 = edexFileInformation.ConfigurationInformation.DataSetPartNumbers[num];
						FlashMeaning flashMeaning2 = channel.FlashAreas.SelectMany((FlashArea fa) => fa.FlashMeanings).FirstOrDefault((FlashMeaning m) => PartExtensions.IsEqual(part2, m.FlashKey));
						string value2 = ((flashMeaning2 != null) ? string.Format(CultureInfo.CurrentCulture, "{0} ({1})", part2, flashMeaning2.Name) : part2.ToString());
						nameValueCollection.Add(string.Format(CultureInfo.CurrentCulture, Resources.ProgrammingDataItem_DataSet_PartNumberFormat, (edexFileInformation.ConfigurationInformation.DataSetPartNumbers.Count > 1) ? (num + 1).ToString(CultureInfo.CurrentCulture) : string.Empty), value2);
					}
				}
				if (edexFileInformation.ConfigurationInformation.HardwareRevision != null)
				{
					nameValueCollection.Add(Resources.ProgrammingDataItem_HardwareRevision, edexFileInformation.ConfigurationInformation.HardwareRevision);
				}
				if (!string.IsNullOrEmpty(edexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile))
				{
					nameValueCollection.Add(Resources.ProgrammingDataItem_LateBoundConfigurationHexFile, Path.GetFileNameWithoutExtension(edexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile));
				}
			}
			return nameValueCollection;
		}
	}

	public static UnitInformation ConnectedUnitInformation
	{
		get
		{
			if (ServerDataManager.GlobalInstance != null && ServerDataManager.GlobalInstance.UnitInformation != null && ServerDataManager.GlobalInstance.UnitInformation.Count > 0 && SapiManager.GlobalInstance != null && SapiManager.GlobalInstance.Sapi != null)
			{
				List<IGrouping<UnitInformation, Channel>> list = (from x in SapiManager.GlobalInstance.ActiveChannels
					where !x.IsRollCall
					group x by UnitInformation(x)).ToList();
				if (list.Count != 1)
				{
					return null;
				}
				return list[0].Key;
			}
			return null;
		}
	}

	public bool TargetChannelRequiresBootLoaderFlash
	{
		get
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			Part part = null;
			if ((int)DataSource == 2)
			{
				EdexFileInformation obj = EdexFileInformation;
				object obj2;
				if (obj == null)
				{
					obj2 = null;
				}
				else
				{
					EdexConfigurationInformation configurationInformation = obj.ConfigurationInformation;
					obj2 = ((configurationInformation != null) ? configurationInformation.BootLoaderPartNumber : null);
				}
				part = (Part)obj2;
			}
			else if ((int)DataSource == 1)
			{
				FirmwareInformation obj3 = Bootcode;
				if (((obj3 != null) ? obj3.Key : null) != null)
				{
					part = new Part(Bootcode.Key);
				}
			}
			if (part != null)
			{
				return !PartExtensions.IsEqual(part, SapiManager.GetBootSoftwarePartNumber(Channel));
			}
			return false;
		}
	}

	public bool TargetChannelRequiresControlListFlash
	{
		get
		{
			if (HasControlList)
			{
				FirmwareInformation obj = ControlList;
				if (((obj != null) ? obj.Key : null) != null)
				{
					return !PartExtensions.IsEqual(new Part(ControlList.Key), SapiManager.GetControlListSoftwarePartNumber(Channel));
				}
			}
			return false;
		}
	}

	public bool HasControlList
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)DataSource == 1)
			{
				return ControlList != null;
			}
			return false;
		}
	}

	internal IEnumerable<Tuple<ProgrammingStep, DiagnosisSource>> RequiredDiagnosisSources
	{
		get
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Invalid comparison between Unknown and I4
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Invalid comparison between Unknown and I4
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Invalid comparison between Unknown and I4
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Invalid comparison between Unknown and I4
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Invalid comparison between Unknown and I4
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Invalid comparison between Unknown and I4
			List<Tuple<ProgrammingStep, DiagnosisSource>> list = new List<Tuple<ProgrammingStep, DiagnosisSource>>();
			if (Firmware != null && (!TargetChannelHasSameFirmwareVersion || FlashRequiredSameFirmwareVersion || SapiManager.GetBootModeStatus(Channel)))
			{
				list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashFirmware, GetDiagnosisSourceForFlashware(Firmware.FileName)));
				if (TargetChannelRequiresBootLoaderFlash)
				{
					Part bootPartNumber = (((int)DataSource == 2) ? EdexFileInformation.ConfigurationInformation.BootLoaderPartNumber : new Part(Bootcode.Key));
					FlashFile flashFile = SapiManager.GlobalInstance.Sapi.FlashFiles.FirstOrDefault((FlashFile ff) => ff.FlashAreas.Any((FlashArea fa) => fa.FlashMeanings.Any((FlashMeaning fm) => PartExtensions.IsEqual(bootPartNumber, fm.FlashKey))));
					if (flashFile == null)
					{
						throw new DataException(string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingBootFirmwareFlashKey, bootPartNumber));
					}
					list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashBootLoader, GetDiagnosisSourceForFlashware(flashFile.FileName)));
				}
			}
			if (TargetChannelRequiresControlListFlash)
			{
				Part controlListPart = (((int)DataSource == 2) ? null : new Part(ControlList.Key));
				FlashFile flashFile2 = SapiManager.GlobalInstance.Sapi.FlashFiles.FirstOrDefault((FlashFile ff) => ff.FlashAreas.Any((FlashArea fa) => fa.FlashMeanings.Any((FlashMeaning fm) => PartExtensions.IsEqual(controlListPart, fm.FlashKey))));
				if (flashFile2 == null)
				{
					throw new DataException(string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingBootFirmwareFlashKey, controlListPart));
				}
				list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashControlList, GetDiagnosisSourceForFlashware(flashFile2.FileName)));
			}
			if (HasDataSet)
			{
				foreach (Part part in TargetChannelNeededDataSetVersions)
				{
					FlashFile flashFile3 = SapiManager.GlobalInstance.Sapi.FlashFiles.FirstOrDefault((FlashFile ff) => ff.FlashAreas.Any((FlashArea fa) => fa.FlashMeanings.Any((FlashMeaning fm) => PartExtensions.IsEqual(part, fm.FlashKey))));
					if (flashFile3 != null)
					{
						list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashDataSet, GetDiagnosisSourceForFlashware(flashFile3.FileName)));
						continue;
					}
					if ((int)DataSource == 1 && DataSet != null)
					{
						list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashDataSet, GetDiagnosisSourceForFlashware(DataSet.FileName)));
						continue;
					}
					throw new DataException(string.Format(CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingDatasetFlashKey, part));
				}
			}
			if (((int)DataSource == 2 && (EdexFileInformation.ConfigurationInformation.SettingItems.Any() || EdexFileInformation.ConfigurationInformation.ApplicableProposedSettingItems().Any() || edexFileInformation.ConfigurationInformation.ChecSettings != null)) || ((int)DataSource == 1 && settings != null))
			{
				DiagnosisSource diagnosisSource = Channel.Ecu.DiagnosisSource;
				if (Channel.Ecu.IsMcd && (int)DataSource == 2 && Channel.CodingParameterGroups.Count == 0)
				{
					Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus.FirstOrDefault((Ecu e) => e.Name.Equals(Channel.Ecu.Name) && !e.IsMcd && SapiExtensions.FlashingRequiresMvci(e));
					if (ecu != null)
					{
						diagnosisSource = ecu.DiagnosisSource;
					}
				}
				list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.UnlockBackdoorAndClearPasswords, diagnosisSource));
				list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.UnlockBackdoor, diagnosisSource));
				list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.ResetToDefault, diagnosisSource));
				list.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.WriteSettings, diagnosisSource));
			}
			return list.Distinct();
		}
	}

	internal Part[] GetMatchedDataSetParts(EcuInfo[] fuelmapEcuInfos, IEnumerable<FlashMeaning> allFlashMeanings)
	{
		Part[] array = new Part[fuelmapEcuInfos.Length];
		List<Part> targetParts = DataSetVersions.ToList();
		if (fuelmapEcuInfos.Length == 1)
		{
			array[0] = targetParts.FirstOrDefault();
		}
		else
		{
			for (int i = 0; i < fuelmapEcuInfos.Length; i++)
			{
				EcuInfo fuelmapEcuInfo = fuelmapEcuInfos[i];
				if (!string.IsNullOrEmpty(fuelmapEcuInfo.Description))
				{
					FlashMeaning meaning = allFlashMeanings.FirstOrDefault((FlashMeaning m) => m.FlashJobName == fuelmapEcuInfo.Description && targetParts.Any((Part tp) => PartExtensions.IsEqual(tp, m.FlashKey)));
					if (meaning != null)
					{
						array[i] = targetParts.First((Part tp) => PartExtensions.IsEqual(tp, meaning.FlashKey));
					}
				}
				if (array[i] == null)
				{
					array[i] = targetParts.FirstOrDefault((Part tp) => PartExtensions.IsEqual(tp, fuelmapEcuInfo?.Value));
				}
			}
		}
		return array;
	}

	public static string OperationToDisplayString(ProgrammingOperation operation)
	{
		string result = string.Empty;
		switch (operation)
		{
		case ProgrammingOperation.Replace:
			result = Resources.ProgrammingOperation_ReplaceDeviceSettingsWithServerConfiguration;
			break;
		case ProgrammingOperation.Update:
			result = Resources.ProgrammingOperation_UpdateDeviceSoftware;
			break;
		case ProgrammingOperation.ChangeDataSet:
			result = Resources.ProgrammingOperation_ChangeDataset;
			break;
		case ProgrammingOperation.UpdateAndChangeDataSet:
			result = Resources.ProgrammingOperation_UpdateDeviceSoftwareAndChangeDataset;
			break;
		}
		return result;
	}

	public static string OperationToDisplayString(ProgrammingOperation operation, bool sameDevice)
	{
		if (operation == ProgrammingOperation.Replace)
		{
			return string.Format(CultureInfo.CurrentCulture, sameDevice ? Resources.Format_ProrgrammingOperationSameDevice : Resources.Format_ProrgrammingOperationNewDevice, OperationToDisplayString(operation));
		}
		return OperationToDisplayString(operation);
	}

	private void ReadAttemptInfo()
	{
		if (!SapiManager.GetBootModeStatus(channel))
		{
			engineSerialNumber = (previousEngineSerialNumber = SapiManager.GetEngineSerialNumber(channel));
			vehicleIdentificationNumber = (previousVehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(channel));
			previousSoftwareVersion = SapiManager.GetSoftwareVersion(channel);
			previousDiagnosticVariant = channel.DiagnosisVariant.ToString();
			previousHardwareRevision = SapiManager.GetHardwareRevision(channel) ?? string.Empty;
			return;
		}
		try
		{
			string[] array = File.ReadAllText(AttemptInfoPath).Split(",".ToCharArray());
			engineSerialNumber = (previousEngineSerialNumber = array[0]);
			vehicleIdentificationNumber = (previousVehicleIdentificationNumber = array[1]);
			previousSoftwareVersion = array[2];
			if (array.Count() > 3)
			{
				previousDiagnosticVariant = array[3];
				previousHardwareRevision = array[4];
			}
		}
		catch (FileNotFoundException)
		{
		}
		catch (IOException)
		{
		}
	}

	private void ReadWriteAttemptInfo()
	{
		ReadAttemptInfo();
		if (!SapiManager.GetBootModeStatus(channel))
		{
			try
			{
				File.WriteAllText(AttemptInfoPath, string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", previousEngineSerialNumber, previousVehicleIdentificationNumber, previousSoftwareVersion, previousDiagnosticVariant, previousHardwareRevision));
			}
			catch (IOException)
			{
			}
		}
	}

	public ProgrammingData(Channel channel, UnitInformation unit, EdexFileInformation edexFile)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		this.channel = channel;
		operation = ProgrammingOperation.Replace;
		settings = null;
		this.unit = unit;
		dataSource = (DeviceDataSource)2;
		edexFileInformation = edexFile;
		EdexConfigurationInformation configurationInformation = edexFile.ConfigurationInformation;
		if (configurationInformation != null)
		{
			firmware = ServerDataManager.GlobalInstance.GetFirmwareInformationForPart(configurationInformation.FlashwarePartNumber);
			if (firmware == null && (!TargetChannelHasSameFirmwareVersion || FlashRequiredSameFirmwareVersion))
			{
				throw new DataException(string.Format(CultureInfo.CurrentCulture, Resources.ProgrammingData_FormatCouldNotLocateFirmwareInfo, configurationInformation.FlashwarePartNumber));
			}
			if (!string.IsNullOrEmpty(configurationInformation.LateBoundDataSetHexFile))
			{
				string text = FileEncryptionProvider.EncryptFileName(Path.Combine(Directories.DrumrollDownloadData, configurationInformation.LateBoundDataSetHexFile));
				if (!File.Exists(text))
				{
					throw new DataException(string.Format(CultureInfo.CurrentCulture, Resources.ProgrammingData_FormatCouldNotLocateFile, configurationInformation.LateBoundDataSetHexFile));
				}
				string text2 = Path.Combine(Sapi.GetSapi().ConfigurationItems["CFFFiles"].Value, SapiManager.GetLateBoundDataSetHexFileTargetName(channel));
				FileManagement.EnsureWritePossible(text2);
				File.WriteAllBytes(text2, FileEncryptionProvider.ReadEncryptedFile(text, (EncryptionType)2));
			}
		}
		ReadWriteAttemptInfo();
		replaceToSameDevice = this.unit.IsSameIdentification(engineSerialNumber, vehicleIdentificationNumber);
		engineSerialNumber = this.unit.EngineNumber;
		vehicleIdentificationNumber = this.unit.VehicleIdentity;
	}

	public ProgrammingData(Channel channel, UnitInformation unit, SettingsInformation settings, bool packageProgrammingOperation, bool useNewest)
		: this(channel, unit, settings, packageProgrammingOperation, useNewest, previewOnly: false)
	{
	}

	public ProgrammingData(Channel channel, UnitInformation unit, SettingsInformation settings, bool packageProgrammingOperation, bool useNewest, bool previewOnly)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		this.channel = channel;
		operation = ProgrammingOperation.Replace;
		this.settings = settings;
		edexFileInformation = null;
		this.unit = unit;
		this.packageProgrammingOperation = packageProgrammingOperation;
		DeviceInformation informationForDevice = this.unit.GetInformationForDevice(this.channel.Ecu.Name);
		if (informationForDevice != null)
		{
			dataSource = informationForDevice.DataSource;
			string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
			if (informationForDevice.HasDataSet)
			{
				string text = (useNewest ? informationForDevice.GetNewestDataSetKey(hardwarePartNumber) : informationForDevice.GetCurrentDataSetKey(hardwarePartNumber));
				if (string.IsNullOrEmpty(text))
				{
					throw new DataException(string.Format(CultureInfo.InvariantCulture, "There is no dataset available that matches your current hardware {0}.", hardwarePartNumber));
				}
				FirmwareOptionInformation relatedFirmwareOption = informationForDevice.GetRelatedFirmwareOption(text, hardwarePartNumber);
				if (relatedFirmwareOption == null)
				{
					throw new DataException(string.Format(CultureInfo.InvariantCulture, "There is no firmware available that matches your target dataset {0}.", text));
				}
				firmware = ServerDataManager.GlobalInstance.GetFirmwareInformationForVersion(informationForDevice.Device, relatedFirmwareOption.Version);
				if (firmware == null && (SapiManager.GetSoftwareVersion(this.channel) != relatedFirmwareOption.Version || SapiManager.GetBootModeStatus(this.channel)))
				{
					throw new DataException(string.Format(CultureInfo.InvariantCulture, "Could not find associated firmware {0} for dataset.", relatedFirmwareOption.Version));
				}
				if (relatedFirmwareOption.BootLoaderKey != null)
				{
					bootcode = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(relatedFirmwareOption.BootLoaderKey);
					if (bootcode == null && !PartExtensions.IsEqual(new Part(SapiManager.GetBootSoftwarePartNumber(channel)), relatedFirmwareOption.BootLoaderKey))
					{
						throw new DataException(string.Format(CultureInfo.InvariantCulture, "Could not find associated bootcode {0} for firmware.", relatedFirmwareOption.BootLoaderKey));
					}
				}
				if (relatedFirmwareOption.ControlListKey != null)
				{
					controlList = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(relatedFirmwareOption.ControlListKey);
					if (controlList == null && !PartExtensions.IsEqual(new Part(SapiManager.GetControlListSoftwarePartNumber(channel)), relatedFirmwareOption.ControlListKey))
					{
						throw new DataException(string.Format(CultureInfo.InvariantCulture, "Could not find associated control list {0} for firmware.", relatedFirmwareOption.ControlListKey));
					}
				}
				dataSet = relatedFirmwareOption.GetDataSetOption(text);
				if (dataSet == null)
				{
					throw new DataException(string.Format(CultureInfo.InvariantCulture, "Could not find compatible dataset option for version {0}.", relatedFirmwareOption.Version));
				}
			}
			else
			{
				string text2 = (useNewest ? informationForDevice.GetNewestFirmwareVersion(hardwarePartNumber) : informationForDevice.GetCurrentFirmwareVersion(hardwarePartNumber));
				if (string.IsNullOrEmpty(text2))
				{
					throw new DataException("The device information provided did not specify the target firmware version.");
				}
				firmware = ServerDataManager.GlobalInstance.GetFirmwareInformationForVersion(informationForDevice.Device, text2);
				if (firmware == null && (SapiManager.GetSoftwareVersion(this.channel) != text2 || SapiManager.GetBootModeStatus(this.channel)))
				{
					throw new DataException(string.Format(CultureInfo.InvariantCulture, "The target firmware version {0} is not available.", text2));
				}
			}
			if (!previewOnly)
			{
				ReadWriteAttemptInfo();
			}
			replaceToSameDevice = this.unit.IsSameIdentification(engineSerialNumber, vehicleIdentificationNumber);
			engineSerialNumber = this.unit.EngineNumber;
			vehicleIdentificationNumber = this.unit.VehicleIdentity;
			return;
		}
		throw new DataException("Device information was not specified (old server data format)");
	}

	public ProgrammingData(Channel channel, FirmwareInformation firmware, string bootLoaderKey, string controlListKey, UnitInformation unit, DataSetOptionInformation dataSet)
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		this.channel = channel;
		operation = ((dataSet == null) ? ProgrammingOperation.Update : ProgrammingOperation.UpdateAndChangeDataSet);
		this.firmware = firmware;
		this.unit = unit;
		engineSerialNumber = this.unit.EngineNumber;
		vehicleIdentificationNumber = this.unit.VehicleIdentity;
		this.dataSet = dataSet;
		if (SapiManager.GetBootModeStatus(this.channel))
		{
			settings = GetPreviousUpgradeSettingsFile(this.channel.Ecu.Name);
		}
		else
		{
			string settingsFileName = Utility.GetSettingsFileName(this.channel, "preupgrade", (SettingsFileFormat)2);
			settings = new SettingsInformation(this.channel.Ecu.Name, "Existing Settings", settingsFileName);
		}
		if (settings == null)
		{
			throw new DataException("Update software requires previous settings, but they are not available");
		}
		if (bootLoaderKey != null)
		{
			bootcode = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(bootLoaderKey);
			if (bootcode == null && !PartExtensions.IsEqual(new Part(SapiManager.GetBootSoftwarePartNumber(channel)), bootLoaderKey))
			{
				throw new DataException(string.Format(CultureInfo.InvariantCulture, "Could not find associated bootcode {0} for firmware.", bootLoaderKey));
			}
		}
		if (controlListKey != null)
		{
			controlList = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(controlListKey);
			if (controlList == null && !PartExtensions.IsEqual(new Part(SapiManager.GetControlListSoftwarePartNumber(channel)), controlListKey))
			{
				throw new DataException(string.Format(CultureInfo.InvariantCulture, "Could not find associated control list {0} for firmware.", controlListKey));
			}
		}
		ReadWriteAttemptInfo();
		UnitInformation val = (string.IsNullOrEmpty(engineSerialNumber) ? ServerDataManager.GlobalInstance.GetUnitInformationByVehicleIdentity(vehicleIdentificationNumber) : ServerDataManager.GlobalInstance.GetUnitInformationByEngineSerialNumber(engineSerialNumber));
		if (val == null)
		{
			return;
		}
		DeviceInformation informationForDevice = val.GetInformationForDevice(this.channel.Ecu.Name);
		if (informationForDevice == null)
		{
			return;
		}
		dataSource = informationForDevice.DataSource;
		if (informationForDevice.HasDataSet && this.dataSet == null)
		{
			string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
			this.dataSet = informationForDevice.GetCompatibleDataSetOption(this.firmware.Version, hardwarePartNumber);
			if (this.dataSet == null)
			{
				throw new DataException("Could not find associated DataSet reference");
			}
		}
	}

	internal ProgrammingData(Channel channel)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		this.channel = channel;
		dataSource = (DeviceDataSource)0;
		ReadAttemptInfo();
	}

	public static ProgrammingData CreateFromAutomaticOperationForConnectedUnit()
	{
		UnitInformation connectedUnitInformation = ConnectedUnitInformation;
		if (connectedUnitInformation != null && connectedUnitInformation.InAutomaticOperation)
		{
			AutomaticOperation currentAutomaticOperation = connectedUnitInformation.CurrentAutomaticOperation;
			Channel channel = currentAutomaticOperation.Channel;
			if (channel != null)
			{
				return CreateFromAutomaticOperation(channel, currentAutomaticOperation);
			}
			throw new DataException(string.Format(CultureInfo.InvariantCulture, "The automatic operation '{0}' is defined for the connected unit '{1}'. You must connect the device {2} to proceed.", currentAutomaticOperation.Reason, currentAutomaticOperation.Unit.IdentityKey, currentAutomaticOperation.Device));
		}
		return null;
	}

	public static ProgrammingData CreateFromAutomaticOperation(Channel channel, AutomaticOperation automaticOperation)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected I4, but got Unknown
		if ((int)automaticOperation.Unit.Status == 3)
		{
			throw new DataException("The unit data has expired. You must reconnect to the server and refresh the data in order to proceed");
		}
		ProgrammingData programmingData = null;
		AutomaticOperationType operationType = automaticOperation.OperationType;
		switch ((int)operationType)
		{
		case 0:
			try
			{
				SettingsInformation val = automaticOperation.Unit.SettingsInformation.Where((SettingsInformation x) => x.Device.Equals(automaticOperation.Device)).Single((SettingsInformation x) => x.SettingsType.Equals(automaticOperation.Detail));
				programmingData = new ProgrammingData(channel, automaticOperation.Unit, val, packageProgrammingOperation: false, useNewest: false);
			}
			catch (InvalidOperationException)
			{
				throw new DataException(string.Format(CultureInfo.InvariantCulture, "Automatic data for Replace Device specified settings that could not be found: {0}", automaticOperation.Detail));
			}
			break;
		case 1:
		{
			FirmwareInformation firmwareInformationForVersion = ServerDataManager.GlobalInstance.GetFirmwareInformationForVersion(automaticOperation.Device, automaticOperation.Detail);
			if (firmwareInformationForVersion != null)
			{
				programmingData = new ProgrammingData(channel, firmwareInformationForVersion, null, null, automaticOperation.Unit, null);
				break;
			}
			throw new DataException(string.Format(CultureInfo.InvariantCulture, "Automatic data for Update Device Software specified a firmware version that could not be found: {0}", automaticOperation.Detail));
		}
		case 2:
			programmingData = CreateFromRequiredDatasetKey(channel, automaticOperation.Unit, automaticOperation.Detail);
			break;
		}
		programmingData.AutomaticOperation = automaticOperation;
		if (programmingData.Firmware != null && programmingData.GetFlashMeanings(FlashBlock.Firmware) == null)
		{
			throw new DataException(string.Format(CultureInfo.InvariantCulture, "The required firmware ({0}) is present, but cannot be programmed into the connected device.", programmingData.Firmware.Version));
		}
		return programmingData;
	}

	internal static ProgrammingData CreateFromRequiredDatasetKey(Channel channel, UnitInformation unit, string key)
	{
		ProgrammingData programmingData = null;
		DeviceInformation deviceInfo = unit.GetInformationForDevice(channel.Ecu.Name);
		string value = new ProgrammingData(channel).PreviousSoftwareVersion;
		if (!string.IsNullOrEmpty(value))
		{
			try
			{
				Part channelHardwarePartNumber = new Part(SapiManager.GetHardwarePartNumber(channel));
				FirmwareOptionInformation val = deviceInfo.FirmwareOptions.Single((FirmwareOptionInformation x) => x.DataSetOptions.Any((DataSetOptionInformation y) => y.Key.Equals(key)) && (x.HardwarePartNumber == null || x.HardwarePartNumber.Equals(channelHardwarePartNumber)) && ServerDataManager.GlobalInstance.CompatibilityTable.IsHardwareCompatibleWithSoftware(new Software(deviceInfo.Device, x.Version, channelHardwarePartNumber)));
				if (val == null)
				{
					throw new DataException(string.Format(CultureInfo.InvariantCulture, "Change Dataset specified a dataset that has no compatible firmware available for the currently connected hardware"));
				}
				DataSetOptionInformation val2 = val.DataSetOptions.Single((DataSetOptionInformation y) => y.Key.Equals(key));
				if (val.Version.Equals(value))
				{
					return new ProgrammingData(channel, val2, unit);
				}
				FirmwareInformation firmwareInformationForVersion = ServerDataManager.GlobalInstance.GetFirmwareInformationForVersion(deviceInfo.Device, val.Version);
				if (firmwareInformationForVersion == null)
				{
					throw new DataException(string.Format(CultureInfo.InvariantCulture, "Change Dataset referenced a firmware version that could not be found: {0}", val.Version));
				}
				return new ProgrammingData(channel, firmwareInformationForVersion, val.BootLoaderKey, val.ControlListKey, unit, val2);
			}
			catch (InvalidOperationException ex)
			{
				throw new DataException(string.Format(CultureInfo.InvariantCulture, "Change Dataset specified a dataset option {0} that could not be found or isn't unique. ", key, ex.Message));
			}
		}
		throw new DataException("Unable to determine current firmware version for Change Data Set");
	}

	public static UnitInformation UnitInformation(Channel channel)
	{
		if (ServerDataManager.GlobalInstance.UnitInformation.Count > 0)
		{
			ProgrammingData programmingData = new ProgrammingData(channel);
			if (!string.IsNullOrEmpty(programmingData.EngineSerialNumber))
			{
				return ServerDataManager.GlobalInstance.GetUnitInformationByEngineSerialNumber(programmingData.EngineSerialNumber);
			}
			if (!string.IsNullOrEmpty(programmingData.VehicleIdentificationNumber))
			{
				return ServerDataManager.GlobalInstance.GetUnitInformationByVehicleIdentity(programmingData.VehicleIdentificationNumber);
			}
		}
		return null;
	}

	public static SettingsInformation GetPreviousUpgradeSettingsFile(string ecu)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		string[] files = Directory.GetFiles(Directories.DrumrollDownloadData, "HF4FK60H*.*");
		string[] array = files;
		foreach (string path in array)
		{
			string text = FileEncryptionProvider.DecryptFileName(Path.GetFileName(path));
			string[] array2 = text.Split("_.".ToCharArray());
			if (array2.Length == 5 && array2[3] == ecu)
			{
				return new SettingsInformation(ecu, "Existing Settings (from previous attempt)", text);
			}
		}
		return null;
	}

	public ProgrammingData(Channel channel, DataSetOptionInformation dataSet, UnitInformation unit)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		this.channel = channel;
		operation = ProgrammingOperation.ChangeDataSet;
		this.dataSet = dataSet;
		this.unit = unit;
		dataSource = (DeviceDataSource)1;
		ReadWriteAttemptInfo();
	}

	internal Collection<FlashMeaning> GetFlashMeanings(FlashBlock which)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Invalid comparison between Unknown and I4
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		if ((int)DataSource == 2 && which == FlashBlock.DataSet && EdexFileInformation.ConfigurationInformation.DataSetPartNumbers.Any())
		{
			Collection<FlashMeaning> collection = new Collection<FlashMeaning>();
			{
				foreach (Part part in TargetChannelNeededDataSetVersions)
				{
					FlashMeaning flashMeaning = channel.FlashAreas.SelectMany((FlashArea fa) => fa.FlashMeanings).FirstOrDefault((FlashMeaning m) => PartExtensions.IsEqual(part, m.FlashKey));
					if (flashMeaning != null)
					{
						collection.Add(flashMeaning);
						continue;
					}
					StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Flash data desired key {0} not found", PartExtensions.ToFlashKeyStyleString(part)), (StatusMessageType)2, (object)this));
					return null;
				}
				return collection;
			}
		}
		if ((int)DataSource == 2 && which == FlashBlock.BootLoader)
		{
			EdexFileInformation obj = EdexFileInformation;
			object obj2;
			if (obj == null)
			{
				obj2 = null;
			}
			else
			{
				EdexConfigurationInformation configurationInformation = obj.ConfigurationInformation;
				obj2 = ((configurationInformation != null) ? configurationInformation.BootLoaderPartNumber : null);
			}
			Part part2 = (Part)obj2;
			if (part2 != null)
			{
				FlashMeaning flashMeaning2 = channel.FlashAreas.SelectMany((FlashArea fa) => fa.FlashMeanings).FirstOrDefault((FlashMeaning m) => PartExtensions.IsEqual(part2, m.FlashKey));
				if (flashMeaning2 == null)
				{
					return null;
				}
				return new Collection<FlashMeaning>(new FlashMeaning[1] { flashMeaning2 });
			}
			return null;
		}
		FlashMeaning flashMeaning3 = GetFlashMeaning(which);
		if (flashMeaning3 == null)
		{
			return null;
		}
		return new Collection<FlashMeaning>(new FlashMeaning[1] { flashMeaning3 });
	}

	private FlashMeaning GetFlashMeaning(FlashBlock which)
	{
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		string text = string.Empty;
		string text2 = string.Empty;
		switch (which)
		{
		case FlashBlock.Firmware:
			if (firmware == null || (!string.IsNullOrEmpty(firmware.Reference) && (firmware.RequiresDownload || firmware.Status != "OK")))
			{
				return null;
			}
			text = firmware.FileName;
			text2 = firmware.Key;
			break;
		case FlashBlock.DataSet:
			if (HasDataSet)
			{
				string[] array = SapiManager.GetLateBoundDataSetHexFileCff(channel).Split(',');
				if (array.Count() >= 2)
				{
					text = array[0];
					text2 = array[1];
				}
				else
				{
					text = dataSet.FileName;
					text2 = dataSet.Key;
				}
			}
			break;
		case FlashBlock.BootLoader:
			if (Bootcode == null || (!string.IsNullOrEmpty(Bootcode.Reference) && (Bootcode.RequiresDownload || Bootcode.Status != "OK")))
			{
				return null;
			}
			text = Bootcode.FileName;
			text2 = Bootcode.Key;
			break;
		case FlashBlock.ControlList:
			if (!HasControlList)
			{
				return null;
			}
			text = ControlList.FileName;
			text2 = ControlList.Key;
			break;
		}
		FlashMeaning flashMeaning = null;
		FlashMeaning flashMeaning2 = null;
		string b = Path.Combine(Directories.GetDatabasePathForExtension(Path.GetExtension(text)), text);
		foreach (FlashArea flashArea in channel.FlashAreas)
		{
			foreach (FlashMeaning flashMeaning3 in flashArea.FlashMeanings)
			{
				if (string.Equals(flashMeaning3.FileName, b, StringComparison.OrdinalIgnoreCase))
				{
					flashMeaning2 = flashMeaning3;
					if (string.Equals(flashMeaning3.FlashKey, text2))
					{
						flashMeaning = flashMeaning3;
						break;
					}
				}
			}
		}
		if (flashMeaning == null && flashMeaning2 != null)
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Flash data was found by filename ({0}), but the desired key {1} did not match the key in the file ({2})", flashMeaning2.FileName, text2, flashMeaning2.FlashKey), (StatusMessageType)2, (object)this));
		}
		if (flashMeaning == null)
		{
			return flashMeaning2;
		}
		return flashMeaning;
	}

	public static DiagnosisSource GetDiagnosisSourceForFlashware(string fileName)
	{
		return Directories.GetDiagnosisSourceForExtension(Path.GetExtension(fileName));
	}

	internal ConnectionResource GetTargetConnectionResource(DiagnosisSource targetDiagnosisSource)
	{
		return SapiManager.GlobalInstance.Sapi.Ecus.FirstOrDefault((Ecu ecu) => ecu.Name == Channel.Ecu.Name && ecu.DiagnosisSource == targetDiagnosisSource)?.GetConnectionResources().FirstOrDefault((ConnectionResource cr) => !cr.Restricted);
	}
}
