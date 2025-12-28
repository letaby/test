// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgrammingData
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class ProgrammingData
{
  private const string ExistingSettingsFileNamePrefix = "preupgrade";
  private const string ExistingSettingsFileSearchPath = "HF4FK60H*.*";
  private const string AttemptInfoFileName = "ddrsattempt.dat";
  private const string AutomaticChargeType = "AUTOMATIC";
  private Channel channel;
  private ProgrammingOperation operation;
  private DetroitDiesel.Net.UnitInformation unit;
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
  private DeviceInformation.DeviceDataSource dataSource;

  public Channel Channel
  {
    get => this.channel;
    set => this.channel = value;
  }

  public ProgrammingOperation Operation => this.operation;

  public DetroitDiesel.Net.UnitInformation Unit => this.unit;

  public SettingsInformation Settings => this.settings;

  public EdexFileInformation EdexFileInformation => this.edexFileInformation;

  public FirmwareInformation Bootcode => this.bootcode;

  public FirmwareInformation ControlList => this.controlList;

  public FirmwareInformation Firmware => this.firmware;

  public DataSetOptionInformation DataSet => this.dataSet;

  public bool HasDataSet
  {
    get
    {
      if (this.DataSource == 1 && this.DataSet != null)
        return true;
      if (this.DataSource != 2)
        return false;
      return this.EdexFileInformation.ConfigurationInformation.DataSetPartNumbers.Any<Part>() || !string.IsNullOrEmpty(this.EdexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile);
    }
  }

  public bool DeferBootModeCheck
  {
    get
    {
      return this.Channel.Ecu.Properties.ContainsKey(nameof (DeferBootModeCheck)) && Convert.ToBoolean(this.Channel.Ecu.Properties[nameof (DeferBootModeCheck)], (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }

  public bool FlashRequiredSameFirmwareVersion
  {
    get
    {
      return this.Channel.Ecu.Properties.ContainsKey(nameof (FlashRequiredSameFirmwareVersion)) && Convert.ToBoolean(this.Channel.Ecu.Properties[nameof (FlashRequiredSameFirmwareVersion)], (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }

  public string EngineSerialNumber => this.engineSerialNumber;

  public string VehicleIdentificationNumber => this.vehicleIdentificationNumber;

  public string ChargeType
  {
    get => this.AutomaticOperation != null ? "AUTOMATIC" : this.chargeType;
    set => this.chargeType = value;
  }

  public string ChargeText
  {
    get => this.AutomaticOperation != null ? this.AutomaticOperation.Time : this.chargeText;
    set => this.chargeText = value;
  }

  public string PreviousEngineSerialNumber => this.previousEngineSerialNumber;

  public string PreviousVehicleIdentificationNumber => this.previousVehicleIdentificationNumber;

  public string PreviousSoftwareVersion => this.previousSoftwareVersion;

  public string PreviousHardwareRevision => this.previousHardwareRevision;

  public string PreviousDiagnosticVariant => this.previousDiagnosticVariant;

  public bool ReplaceToSameDevice => this.replaceToSameDevice;

  public string AttemptInfoPath
  {
    get => Path.Combine(Path.GetTempPath(), this.channel.Ecu.Name + "ddrsattempt.dat");
  }

  public bool PackageProgrammingOperation => this.packageProgrammingOperation;

  public DeviceInformation.DeviceDataSource DataSource => this.dataSource;

  public AutomaticOperation AutomaticOperation { private set; get; }

  public bool TargetChannelHasSameFirmwareVersion
  {
    get
    {
      if (this.Channel == null)
        throw new InvalidOperationException("The Channel property must not be null.");
      DeviceInformation.DeviceDataSource dataSource = this.DataSource;
      if (dataSource != 1)
      {
        if (dataSource != 2)
          throw new InvalidOperationException("The DataSource type is unknown or unsupported.");
        if (this.EdexFileInformation == null)
          throw new InvalidOperationException("The EdexFileInformation property must not be null.");
        return SapiManager.ProgramDeviceUsesSoftwareIdentification(this.Channel.Ecu) ? this.EdexFileInformation.ConfigurationInformation.SoftwareIdentification.Equals(SapiManager.GetSoftwareIdentification(this.Channel), StringComparison.OrdinalIgnoreCase) : PartExtensions.IsEqual(this.EdexFileInformation.ConfigurationInformation.FlashwarePartNumber, SapiManager.GetSoftwarePartNumber(this.Channel));
      }
      if (this.Firmware == null)
        throw new InvalidOperationException("The Firmware property must not be null.");
      return string.Equals(this.Firmware.Version, SapiManager.GetSoftwareVersion(this.Channel), StringComparison.OrdinalIgnoreCase);
    }
  }

  public IEnumerable<Part> DataSetVersions
  {
    get
    {
      if (this.channel == null)
        throw new InvalidOperationException("The Channel property must not be null.");
      if (!this.HasDataSet)
        throw new InvalidOperationException("The dataset information must not be null.");
      List<Part> dataSetVersions = new List<Part>();
      if (this.DataSource == 2)
      {
        if (!string.IsNullOrEmpty(this.EdexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile))
          return Enumerable.Repeat<Part>(new Part(SapiManager.GetLateBoundDataSetHexFileCff(this.channel).Split(',')[1]), 1);
        dataSetVersions.AddRange((IEnumerable<Part>) this.EdexFileInformation.ConfigurationInformation.DataSetPartNumbers);
      }
      else if (this.DataSource == 1)
        dataSetVersions.Add(new Part(this.DataSet.Key));
      return (IEnumerable<Part>) dataSetVersions;
    }
  }

  public IEnumerable<Part> TargetChannelNeededDataSetVersions
  {
    get
    {
      return this.DataSource == 2 && !string.IsNullOrEmpty(this.EdexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile) || this.Firmware != null && (!this.TargetChannelHasSameFirmwareVersion || this.FlashRequiredSameFirmwareVersion || SapiManager.GetBootModeStatus(this.Channel)) ? this.DataSetVersions : this.DataSetVersions.Where<Part>((System.Func<Part, bool>) (dsFlash => !SapiManager.GetDataSetPartNumbers(this.Channel).Any<string>(new System.Func<string, bool>(((PartExtensions) dsFlash).IsEqual))));
    }
  }

  internal Part[] GetMatchedDataSetParts(
    EcuInfo[] fuelmapEcuInfos,
    IEnumerable<FlashMeaning> allFlashMeanings)
  {
    Part[] matchedDataSetParts = new Part[fuelmapEcuInfos.Length];
    List<Part> targetParts = this.DataSetVersions.ToList<Part>();
    if (fuelmapEcuInfos.Length == 1)
    {
      matchedDataSetParts[0] = targetParts.FirstOrDefault<Part>();
    }
    else
    {
      for (int index = 0; index < fuelmapEcuInfos.Length; ++index)
      {
        EcuInfo fuelmapEcuInfo = fuelmapEcuInfos[index];
        if (!string.IsNullOrEmpty(fuelmapEcuInfo.Description))
        {
          FlashMeaning meaning = allFlashMeanings.FirstOrDefault<FlashMeaning>((System.Func<FlashMeaning, bool>) (m => m.FlashJobName == fuelmapEcuInfo.Description && targetParts.Any<Part>((System.Func<Part, bool>) (tp => PartExtensions.IsEqual(tp, m.FlashKey)))));
          if (meaning != null)
            matchedDataSetParts[index] = targetParts.First<Part>((System.Func<Part, bool>) (tp => PartExtensions.IsEqual(tp, meaning.FlashKey)));
        }
        if (matchedDataSetParts[index] == null)
          matchedDataSetParts[index] = targetParts.FirstOrDefault<Part>((System.Func<Part, bool>) (tp => PartExtensions.IsEqual(tp, fuelmapEcuInfo?.Value)));
      }
    }
    return matchedDataSetParts;
  }

  public bool TargetChannelIsValidForFirmware
  {
    get
    {
      DeviceInformation informationForDevice = this.Unit.GetInformationForDevice(this.Channel.Ecu.Name);
      string hardwarePartNumber = SapiManager.GetHardwarePartNumber(this.Channel);
      string hardwareRevision = SapiManager.GetHardwareRevision(this.Channel);
      Part part = !string.IsNullOrEmpty(hardwarePartNumber) ? new Part(hardwarePartNumber) : (Part) null;
      return part == null && string.IsNullOrEmpty(hardwareRevision) || informationForDevice.FirmwareOptionAvailableForHardware(this.Firmware, part, hardwareRevision);
    }
  }

  public static string OperationToDisplayString(ProgrammingOperation operation)
  {
    string displayString = string.Empty;
    switch (operation)
    {
      case ProgrammingOperation.Replace:
        displayString = Resources.ProgrammingOperation_ReplaceDeviceSettingsWithServerConfiguration;
        break;
      case ProgrammingOperation.Update:
        displayString = Resources.ProgrammingOperation_UpdateDeviceSoftware;
        break;
      case ProgrammingOperation.ChangeDataSet:
        displayString = Resources.ProgrammingOperation_ChangeDataset;
        break;
      case ProgrammingOperation.UpdateAndChangeDataSet:
        displayString = Resources.ProgrammingOperation_UpdateDeviceSoftwareAndChangeDataset;
        break;
    }
    return displayString;
  }

  public static string OperationToDisplayString(ProgrammingOperation operation, bool sameDevice)
  {
    return operation == ProgrammingOperation.Replace ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, sameDevice ? Resources.Format_ProrgrammingOperationSameDevice : Resources.Format_ProrgrammingOperationNewDevice, (object) ProgrammingData.OperationToDisplayString(operation)) : ProgrammingData.OperationToDisplayString(operation);
  }

  public NameValueCollection NameValueCollection
  {
    get
    {
      NameValueCollection nameValueCollection = new NameValueCollection();
      if (this.AutomaticOperation != null)
        nameValueCollection.Add(Resources.ProgrammingDataItem_AutomaticOperation, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} - {1} - {2}", (object) this.AutomaticOperation.Reason, (object) Sapi.TimeFromString(this.AutomaticOperation.Time).ToShortDateString(), (object) ProgrammingData.OperationToDisplayString(this.operation, this.replaceToSameDevice)));
      else
        nameValueCollection.Add(Resources.ProgrammingDataItem_Operation, ProgrammingData.OperationToDisplayString(this.operation, this.replaceToSameDevice));
      nameValueCollection.Add(Resources.ProgrammingDataItem_Device, this.channel.Ecu.Name);
      if (this.unit != null)
        nameValueCollection.Add(Resources.ProgrammingDataItem_Unit, this.unit.IdentityKey);
      if (this.firmware != null)
      {
        if (this.DataSource == 2 && this.edexFileInformation != null)
        {
          Part part = this.edexFileInformation.ConfigurationInformation.BootLoaderPartNumber;
          if (part != null)
          {
            FlashMeaning flashMeaning = this.channel.FlashAreas.SelectMany<FlashArea, FlashMeaning>((System.Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).FirstOrDefault<FlashMeaning>((System.Func<FlashMeaning, bool>) (m => PartExtensions.IsEqual(part, m.FlashKey)));
            nameValueCollection.Add(Resources.ProgrammingDataItem_BootSoftware, flashMeaning != null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) part, (object) flashMeaning.Name) : part.ToString());
          }
          string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) new Part(this.firmware.Key).ToString(), (object) this.firmware.Version);
          nameValueCollection.Add(Resources.ProgrammingDataItem_Software, str);
        }
        else
          nameValueCollection.Add(Resources.ProgrammingDataItem_Software, this.firmware.Version);
      }
      if (this.dataSet != null && !string.IsNullOrEmpty(this.dataSet.Key) && !string.IsNullOrEmpty(this.dataSet.Description))
        nameValueCollection.Add(Resources.ProgrammingDataItem_Dataset, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) new Part(this.dataSet.Key).ToString(), (object) this.dataSet.Description));
      if (this.operation != ProgrammingOperation.ChangeDataSet)
      {
        string str = this.settings == null ? (this.edexFileInformation == null ? (this.operation != ProgrammingOperation.Update ? Resources.ProgrammingDataItemWarning_ResetToDefaultNotRecommended : Resources.ProgrammingDataItemWarning_NoExistingSettingsResetToDefault) : this.edexFileInformation.CompleteFileType) : this.settings.SettingsType;
        nameValueCollection.Add(Resources.ProgrammingDataItem_Settings, str);
      }
      if (this.engineSerialNumber != null && this.engineSerialNumber.Length > 0)
        nameValueCollection.Add(Resources.ProgrammingDataItem_EngineSerialNumber, this.engineSerialNumber);
      if (this.vehicleIdentificationNumber != null && this.vehicleIdentificationNumber.Length > 0)
        nameValueCollection.Add(Resources.ProgrammingDataItem_VehicleIdentificationNumber, this.vehicleIdentificationNumber);
      if (this.DataSource == 2 && this.edexFileInformation != null && !this.edexFileInformation.HasErrors)
      {
        if (this.edexFileInformation.ConfigurationInformation.HardwarePartNumber != null)
          nameValueCollection.Add(Resources.ProgrammingDataItem_HardwarePartNumber, PartExtensions.ToHardwarePartNumberString(this.edexFileInformation.ConfigurationInformation.HardwarePartNumber, this.edexFileInformation.ConfigurationInformation.DeviceName, true));
        if (this.edexFileInformation.ConfigurationInformation.DataSetPartNumbers != null && this.edexFileInformation.ConfigurationInformation.DataSetPartNumbers.Any<Part>())
        {
          for (int index = 0; index < this.edexFileInformation.ConfigurationInformation.DataSetPartNumbers.Count; ++index)
          {
            Part part = this.edexFileInformation.ConfigurationInformation.DataSetPartNumbers[index];
            FlashMeaning flashMeaning = this.channel.FlashAreas.SelectMany<FlashArea, FlashMeaning>((System.Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).FirstOrDefault<FlashMeaning>((System.Func<FlashMeaning, bool>) (m => PartExtensions.IsEqual(part, m.FlashKey)));
            string str = flashMeaning != null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) part, (object) flashMeaning.Name) : part.ToString();
            nameValueCollection.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgrammingDataItem_DataSet_PartNumberFormat, this.edexFileInformation.ConfigurationInformation.DataSetPartNumbers.Count > 1 ? (object) (index + 1).ToString((IFormatProvider) CultureInfo.CurrentCulture) : (object) string.Empty), str);
          }
        }
        if (this.edexFileInformation.ConfigurationInformation.HardwareRevision != null)
          nameValueCollection.Add(Resources.ProgrammingDataItem_HardwareRevision, this.edexFileInformation.ConfigurationInformation.HardwareRevision);
        if (!string.IsNullOrEmpty(this.edexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile))
          nameValueCollection.Add(Resources.ProgrammingDataItem_LateBoundConfigurationHexFile, Path.GetFileNameWithoutExtension(this.edexFileInformation.ConfigurationInformation.LateBoundDataSetHexFile));
      }
      return nameValueCollection;
    }
  }

  private void ReadAttemptInfo()
  {
    if (!SapiManager.GetBootModeStatus(this.channel))
    {
      this.engineSerialNumber = this.previousEngineSerialNumber = SapiManager.GetEngineSerialNumber(this.channel);
      this.vehicleIdentificationNumber = this.previousVehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(this.channel);
      this.previousSoftwareVersion = SapiManager.GetSoftwareVersion(this.channel);
      this.previousDiagnosticVariant = this.channel.DiagnosisVariant.ToString();
      this.previousHardwareRevision = SapiManager.GetHardwareRevision(this.channel) ?? string.Empty;
    }
    else
    {
      try
      {
        string[] source = File.ReadAllText(this.AttemptInfoPath).Split(",".ToCharArray());
        this.engineSerialNumber = this.previousEngineSerialNumber = source[0];
        this.vehicleIdentificationNumber = this.previousVehicleIdentificationNumber = source[1];
        this.previousSoftwareVersion = source[2];
        if (((IEnumerable<string>) source).Count<string>() <= 3)
          return;
        this.previousDiagnosticVariant = source[3];
        this.previousHardwareRevision = source[4];
      }
      catch (FileNotFoundException ex)
      {
      }
      catch (IOException ex)
      {
      }
    }
  }

  private void ReadWriteAttemptInfo()
  {
    this.ReadAttemptInfo();
    if (SapiManager.GetBootModeStatus(this.channel))
      return;
    try
    {
      File.WriteAllText(this.AttemptInfoPath, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", (object) this.previousEngineSerialNumber, (object) this.previousVehicleIdentificationNumber, (object) this.previousSoftwareVersion, (object) this.previousDiagnosticVariant, (object) this.previousHardwareRevision));
    }
    catch (IOException ex)
    {
    }
  }

  public ProgrammingData(Channel channel, DetroitDiesel.Net.UnitInformation unit, EdexFileInformation edexFile)
  {
    this.channel = channel;
    this.operation = ProgrammingOperation.Replace;
    this.settings = (SettingsInformation) null;
    this.unit = unit;
    this.dataSource = (DeviceInformation.DeviceDataSource) 2;
    this.edexFileInformation = edexFile;
    EdexConfigurationInformation configurationInformation = edexFile.ConfigurationInformation;
    if (configurationInformation != null)
    {
      this.firmware = ServerDataManager.GlobalInstance.GetFirmwareInformationForPart(configurationInformation.FlashwarePartNumber);
      if (this.firmware == null && (!this.TargetChannelHasSameFirmwareVersion || this.FlashRequiredSameFirmwareVersion))
        throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgrammingData_FormatCouldNotLocateFirmwareInfo, (object) configurationInformation.FlashwarePartNumber));
      if (!string.IsNullOrEmpty(configurationInformation.LateBoundDataSetHexFile))
      {
        string path1 = FileEncryptionProvider.EncryptFileName(Path.Combine(Directories.DrumrollDownloadData, configurationInformation.LateBoundDataSetHexFile));
        if (!File.Exists(path1))
          throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgrammingData_FormatCouldNotLocateFile, (object) configurationInformation.LateBoundDataSetHexFile));
        string path2 = Path.Combine(Sapi.GetSapi().ConfigurationItems["CFFFiles"].Value, SapiManager.GetLateBoundDataSetHexFileTargetName(channel));
        FileManagement.EnsureWritePossible(path2);
        File.WriteAllBytes(path2, FileEncryptionProvider.ReadEncryptedFile(path1, (EncryptionType) 2));
      }
    }
    this.ReadWriteAttemptInfo();
    this.replaceToSameDevice = this.unit.IsSameIdentification(this.engineSerialNumber, this.vehicleIdentificationNumber);
    this.engineSerialNumber = this.unit.EngineNumber;
    this.vehicleIdentificationNumber = this.unit.VehicleIdentity;
  }

  public ProgrammingData(
    Channel channel,
    DetroitDiesel.Net.UnitInformation unit,
    SettingsInformation settings,
    bool packageProgrammingOperation,
    bool useNewest)
    : this(channel, unit, settings, packageProgrammingOperation, useNewest, false)
  {
  }

  public ProgrammingData(
    Channel channel,
    DetroitDiesel.Net.UnitInformation unit,
    SettingsInformation settings,
    bool packageProgrammingOperation,
    bool useNewest,
    bool previewOnly)
  {
    this.channel = channel;
    this.operation = ProgrammingOperation.Replace;
    this.settings = settings;
    this.edexFileInformation = (EdexFileInformation) null;
    this.unit = unit;
    this.packageProgrammingOperation = packageProgrammingOperation;
    DeviceInformation informationForDevice = this.unit.GetInformationForDevice(this.channel.Ecu.Name);
    this.dataSource = informationForDevice != null ? informationForDevice.DataSource : throw new DataException("Device information was not specified (old server data format)");
    string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
    if (informationForDevice.HasDataSet)
    {
      string str = useNewest ? informationForDevice.GetNewestDataSetKey(hardwarePartNumber) : informationForDevice.GetCurrentDataSetKey(hardwarePartNumber);
      if (string.IsNullOrEmpty(str))
        throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "There is no dataset available that matches your current hardware {0}.", (object) hardwarePartNumber));
      FirmwareOptionInformation relatedFirmwareOption = informationForDevice.GetRelatedFirmwareOption(str, hardwarePartNumber);
      if (relatedFirmwareOption == null)
        throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "There is no firmware available that matches your target dataset {0}.", (object) str));
      this.firmware = ServerDataManager.GlobalInstance.GetFirmwareInformationForVersion(informationForDevice.Device, relatedFirmwareOption.Version);
      if (this.firmware == null && (SapiManager.GetSoftwareVersion(this.channel) != relatedFirmwareOption.Version || SapiManager.GetBootModeStatus(this.channel)))
        throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find associated firmware {0} for dataset.", (object) relatedFirmwareOption.Version));
      if (relatedFirmwareOption.BootLoaderKey != null)
      {
        this.bootcode = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(relatedFirmwareOption.BootLoaderKey);
        if (this.bootcode == null && !PartExtensions.IsEqual(new Part(SapiManager.GetBootSoftwarePartNumber(channel)), relatedFirmwareOption.BootLoaderKey))
          throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find associated bootcode {0} for firmware.", (object) relatedFirmwareOption.BootLoaderKey));
      }
      if (relatedFirmwareOption.ControlListKey != null)
      {
        this.controlList = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(relatedFirmwareOption.ControlListKey);
        if (this.controlList == null && !PartExtensions.IsEqual(new Part(SapiManager.GetControlListSoftwarePartNumber(channel)), relatedFirmwareOption.ControlListKey))
          throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find associated control list {0} for firmware.", (object) relatedFirmwareOption.ControlListKey));
      }
      this.dataSet = relatedFirmwareOption.GetDataSetOption(str);
      if (this.dataSet == null)
        throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find compatible dataset option for version {0}.", (object) relatedFirmwareOption.Version));
    }
    else
    {
      string str = useNewest ? informationForDevice.GetNewestFirmwareVersion(hardwarePartNumber) : informationForDevice.GetCurrentFirmwareVersion(hardwarePartNumber);
      if (string.IsNullOrEmpty(str))
        throw new DataException("The device information provided did not specify the target firmware version.");
      this.firmware = ServerDataManager.GlobalInstance.GetFirmwareInformationForVersion(informationForDevice.Device, str);
      if (this.firmware == null && (SapiManager.GetSoftwareVersion(this.channel) != str || SapiManager.GetBootModeStatus(this.channel)))
        throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The target firmware version {0} is not available.", (object) str));
    }
    if (!previewOnly)
      this.ReadWriteAttemptInfo();
    this.replaceToSameDevice = this.unit.IsSameIdentification(this.engineSerialNumber, this.vehicleIdentificationNumber);
    this.engineSerialNumber = this.unit.EngineNumber;
    this.vehicleIdentificationNumber = this.unit.VehicleIdentity;
  }

  public ProgrammingData(
    Channel channel,
    FirmwareInformation firmware,
    string bootLoaderKey,
    string controlListKey,
    DetroitDiesel.Net.UnitInformation unit,
    DataSetOptionInformation dataSet)
  {
    this.channel = channel;
    this.operation = dataSet != null ? ProgrammingOperation.UpdateAndChangeDataSet : ProgrammingOperation.Update;
    this.firmware = firmware;
    this.unit = unit;
    this.engineSerialNumber = this.unit.EngineNumber;
    this.vehicleIdentificationNumber = this.unit.VehicleIdentity;
    this.dataSet = dataSet;
    this.settings = !SapiManager.GetBootModeStatus(this.channel) ? new SettingsInformation(this.channel.Ecu.Name, "Existing Settings", Utility.GetSettingsFileName(this.channel, "preupgrade", (FileNameInformation.SettingsFileFormat) 2)) : ProgrammingData.GetPreviousUpgradeSettingsFile(this.channel.Ecu.Name);
    if (this.settings == null)
      throw new DataException("Update software requires previous settings, but they are not available");
    if (bootLoaderKey != null)
    {
      this.bootcode = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(bootLoaderKey);
      if (this.bootcode == null && !PartExtensions.IsEqual(new Part(SapiManager.GetBootSoftwarePartNumber(channel)), bootLoaderKey))
        throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find associated bootcode {0} for firmware.", (object) bootLoaderKey));
    }
    if (controlListKey != null)
    {
      this.controlList = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(controlListKey);
      if (this.controlList == null && !PartExtensions.IsEqual(new Part(SapiManager.GetControlListSoftwarePartNumber(channel)), controlListKey))
        throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find associated control list {0} for firmware.", (object) controlListKey));
    }
    this.ReadWriteAttemptInfo();
    DetroitDiesel.Net.UnitInformation unitInformation = string.IsNullOrEmpty(this.engineSerialNumber) ? ServerDataManager.GlobalInstance.GetUnitInformationByVehicleIdentity(this.vehicleIdentificationNumber) : ServerDataManager.GlobalInstance.GetUnitInformationByEngineSerialNumber(this.engineSerialNumber);
    if (unitInformation == null)
      return;
    DeviceInformation informationForDevice = unitInformation.GetInformationForDevice(this.channel.Ecu.Name);
    if (informationForDevice == null)
      return;
    this.dataSource = informationForDevice.DataSource;
    if (!informationForDevice.HasDataSet || this.dataSet != null)
      return;
    string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
    this.dataSet = informationForDevice.GetCompatibleDataSetOption(this.firmware.Version, hardwarePartNumber);
    if (this.dataSet == null)
      throw new DataException("Could not find associated DataSet reference");
  }

  internal ProgrammingData(Channel channel)
  {
    this.channel = channel;
    this.dataSource = (DeviceInformation.DeviceDataSource) 0;
    this.ReadAttemptInfo();
  }

  public static ProgrammingData CreateFromAutomaticOperationForConnectedUnit()
  {
    DetroitDiesel.Net.UnitInformation connectedUnitInformation = ProgrammingData.ConnectedUnitInformation;
    if (connectedUnitInformation == null || !connectedUnitInformation.InAutomaticOperation)
      return (ProgrammingData) null;
    AutomaticOperation automaticOperation = connectedUnitInformation.CurrentAutomaticOperation;
    return ProgrammingData.CreateFromAutomaticOperation(automaticOperation.Channel ?? throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The automatic operation '{0}' is defined for the connected unit '{1}'. You must connect the device {2} to proceed.", (object) automaticOperation.Reason, (object) automaticOperation.Unit.IdentityKey, (object) automaticOperation.Device)), automaticOperation);
  }

  public static ProgrammingData CreateFromAutomaticOperation(
    Channel channel,
    AutomaticOperation automaticOperation)
  {
    if (automaticOperation.Unit.Status == 3)
      throw new DataException("The unit data has expired. You must reconnect to the server and refresh the data in order to proceed");
    ProgrammingData programmingData = (ProgrammingData) null;
    switch ((int) automaticOperation.OperationType)
    {
      case 0:
        try
        {
          SettingsInformation settings = automaticOperation.Unit.SettingsInformation.Where<SettingsInformation>((System.Func<SettingsInformation, bool>) (x => x.Device.Equals(automaticOperation.Device))).Single<SettingsInformation>((System.Func<SettingsInformation, bool>) (x => x.SettingsType.Equals(automaticOperation.Detail)));
          programmingData = new ProgrammingData(channel, automaticOperation.Unit, settings, false, false);
          break;
        }
        catch (InvalidOperationException ex)
        {
          throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Automatic data for Replace Device specified settings that could not be found: {0}", (object) automaticOperation.Detail));
        }
      case 1:
        programmingData = new ProgrammingData(channel, ServerDataManager.GlobalInstance.GetFirmwareInformationForVersion(automaticOperation.Device, automaticOperation.Detail) ?? throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Automatic data for Update Device Software specified a firmware version that could not be found: {0}", (object) automaticOperation.Detail)), (string) null, (string) null, automaticOperation.Unit, (DataSetOptionInformation) null);
        break;
      case 2:
        programmingData = ProgrammingData.CreateFromRequiredDatasetKey(channel, automaticOperation.Unit, automaticOperation.Detail);
        break;
    }
    programmingData.AutomaticOperation = automaticOperation;
    return programmingData.Firmware == null || programmingData.GetFlashMeanings(ProgrammingData.FlashBlock.Firmware) != null ? programmingData : throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The required firmware ({0}) is present, but cannot be programmed into the connected device.", (object) programmingData.Firmware.Version));
  }

  internal static ProgrammingData CreateFromRequiredDatasetKey(
    Channel channel,
    DetroitDiesel.Net.UnitInformation unit,
    string key)
  {
    DeviceInformation deviceInfo = unit.GetInformationForDevice(channel.Ecu.Name);
    string previousSoftwareVersion = new ProgrammingData(channel).PreviousSoftwareVersion;
    if (string.IsNullOrEmpty(previousSoftwareVersion))
      throw new DataException("Unable to determine current firmware version for Change Data Set");
    try
    {
      Part channelHardwarePartNumber = new Part(SapiManager.GetHardwarePartNumber(channel));
      FirmwareOptionInformation optionInformation = deviceInfo.FirmwareOptions.Single<FirmwareOptionInformation>((System.Func<FirmwareOptionInformation, bool>) (x => x.DataSetOptions.Any<DataSetOptionInformation>((System.Func<DataSetOptionInformation, bool>) (y => y.Key.Equals(key))) && (x.HardwarePartNumber == null || x.HardwarePartNumber.Equals((object) channelHardwarePartNumber)) && ServerDataManager.GlobalInstance.CompatibilityTable.IsHardwareCompatibleWithSoftware(new Software(deviceInfo.Device, x.Version, channelHardwarePartNumber))));
      if (optionInformation == null)
        throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Change Dataset specified a dataset that has no compatible firmware available for the currently connected hardware"));
      DataSetOptionInformation dataSet = optionInformation.DataSetOptions.Single<DataSetOptionInformation>((System.Func<DataSetOptionInformation, bool>) (y => y.Key.Equals(key)));
      if (optionInformation.Version.Equals(previousSoftwareVersion))
        return new ProgrammingData(channel, dataSet, unit);
      return new ProgrammingData(channel, ServerDataManager.GlobalInstance.GetFirmwareInformationForVersion(deviceInfo.Device, optionInformation.Version) ?? throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Change Dataset referenced a firmware version that could not be found: {0}", (object) optionInformation.Version)), optionInformation.BootLoaderKey, optionInformation.ControlListKey, unit, dataSet);
    }
    catch (InvalidOperationException ex)
    {
      throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Change Dataset specified a dataset option {0} that could not be found or isn't unique. ", (object) key, (object) ex.Message));
    }
  }

  public static DetroitDiesel.Net.UnitInformation UnitInformation(Channel channel)
  {
    if (ServerDataManager.GlobalInstance.UnitInformation.Count > 0)
    {
      ProgrammingData programmingData = new ProgrammingData(channel);
      if (!string.IsNullOrEmpty(programmingData.EngineSerialNumber))
        return ServerDataManager.GlobalInstance.GetUnitInformationByEngineSerialNumber(programmingData.EngineSerialNumber);
      if (!string.IsNullOrEmpty(programmingData.VehicleIdentificationNumber))
        return ServerDataManager.GlobalInstance.GetUnitInformationByVehicleIdentity(programmingData.VehicleIdentificationNumber);
    }
    return (DetroitDiesel.Net.UnitInformation) null;
  }

  public static DetroitDiesel.Net.UnitInformation ConnectedUnitInformation
  {
    get
    {
      if (ServerDataManager.GlobalInstance == null || ServerDataManager.GlobalInstance.UnitInformation == null || ServerDataManager.GlobalInstance.UnitInformation.Count <= 0 || SapiManager.GlobalInstance == null || SapiManager.GlobalInstance.Sapi == null)
        return (DetroitDiesel.Net.UnitInformation) null;
      List<IGrouping<DetroitDiesel.Net.UnitInformation, Channel>> list = SapiManager.GlobalInstance.ActiveChannels.Where<Channel>((System.Func<Channel, bool>) (c => !c.IsRollCall)).GroupBy<Channel, DetroitDiesel.Net.UnitInformation>((System.Func<Channel, DetroitDiesel.Net.UnitInformation>) (x => ProgrammingData.UnitInformation(x))).ToList<IGrouping<DetroitDiesel.Net.UnitInformation, Channel>>();
      return list.Count != 1 ? (DetroitDiesel.Net.UnitInformation) null : list[0].Key;
    }
  }

  public static SettingsInformation GetPreviousUpgradeSettingsFile(string ecu)
  {
    foreach (string file in Directory.GetFiles(Directories.DrumrollDownloadData, "HF4FK60H*.*"))
    {
      string str = FileEncryptionProvider.DecryptFileName(Path.GetFileName(file));
      string[] strArray = str.Split("_.".ToCharArray());
      if (strArray.Length == 5 && strArray[3] == ecu)
        return new SettingsInformation(ecu, "Existing Settings (from previous attempt)", str);
    }
    return (SettingsInformation) null;
  }

  public ProgrammingData(Channel channel, DataSetOptionInformation dataSet, DetroitDiesel.Net.UnitInformation unit)
  {
    this.channel = channel;
    this.operation = ProgrammingOperation.ChangeDataSet;
    this.dataSet = dataSet;
    this.unit = unit;
    this.dataSource = (DeviceInformation.DeviceDataSource) 1;
    this.ReadWriteAttemptInfo();
  }

  internal Collection<FlashMeaning> GetFlashMeanings(ProgrammingData.FlashBlock which)
  {
    if (this.DataSource == 2 && which == ProgrammingData.FlashBlock.DataSet && this.EdexFileInformation.ConfigurationInformation.DataSetPartNumbers.Any<Part>())
    {
      Collection<FlashMeaning> flashMeanings = new Collection<FlashMeaning>();
      foreach (Part neededDataSetVersion in this.TargetChannelNeededDataSetVersions)
      {
        Part part = neededDataSetVersion;
        FlashMeaning flashMeaning = this.channel.FlashAreas.SelectMany<FlashArea, FlashMeaning>((System.Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).FirstOrDefault<FlashMeaning>((System.Func<FlashMeaning, bool>) (m => PartExtensions.IsEqual(part, m.FlashKey)));
        if (flashMeaning != null)
        {
          flashMeanings.Add(flashMeaning);
        }
        else
        {
          StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Flash data desired key {0} not found", (object) PartExtensions.ToFlashKeyStyleString(part)), (StatusMessageType) 2, (object) this));
          return (Collection<FlashMeaning>) null;
        }
      }
      return flashMeanings;
    }
    if (this.DataSource == 2 && which == ProgrammingData.FlashBlock.BootLoader)
    {
      Part part = this.EdexFileInformation?.ConfigurationInformation?.BootLoaderPartNumber;
      if (part == null)
        return (Collection<FlashMeaning>) null;
      FlashMeaning flashMeaning = this.channel.FlashAreas.SelectMany<FlashArea, FlashMeaning>((System.Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).FirstOrDefault<FlashMeaning>((System.Func<FlashMeaning, bool>) (m => PartExtensions.IsEqual(part, m.FlashKey)));
      if (flashMeaning == null)
        return (Collection<FlashMeaning>) null;
      return new Collection<FlashMeaning>((IList<FlashMeaning>) new FlashMeaning[1]
      {
        flashMeaning
      });
    }
    FlashMeaning flashMeaning1 = this.GetFlashMeaning(which);
    if (flashMeaning1 == null)
      return (Collection<FlashMeaning>) null;
    return new Collection<FlashMeaning>((IList<FlashMeaning>) new FlashMeaning[1]
    {
      flashMeaning1
    });
  }

  private FlashMeaning GetFlashMeaning(ProgrammingData.FlashBlock which)
  {
    string str = string.Empty;
    string b1 = string.Empty;
    switch (which)
    {
      case ProgrammingData.FlashBlock.BootLoader:
        if (this.Bootcode == null || !string.IsNullOrEmpty(this.Bootcode.Reference) && (this.Bootcode.RequiresDownload || this.Bootcode.Status != "OK"))
          return (FlashMeaning) null;
        str = this.Bootcode.FileName;
        b1 = this.Bootcode.Key;
        break;
      case ProgrammingData.FlashBlock.Firmware:
        if (this.firmware == null || !string.IsNullOrEmpty(this.firmware.Reference) && (this.firmware.RequiresDownload || this.firmware.Status != "OK"))
          return (FlashMeaning) null;
        str = this.firmware.FileName;
        b1 = this.firmware.Key;
        break;
      case ProgrammingData.FlashBlock.DataSet:
        if (this.HasDataSet)
        {
          string[] source = SapiManager.GetLateBoundDataSetHexFileCff(this.channel).Split(',');
          if (((IEnumerable<string>) source).Count<string>() >= 2)
          {
            str = source[0];
            b1 = source[1];
            break;
          }
          str = this.dataSet.FileName;
          b1 = this.dataSet.Key;
          break;
        }
        break;
      case ProgrammingData.FlashBlock.ControlList:
        if (!this.HasControlList)
          return (FlashMeaning) null;
        str = this.ControlList.FileName;
        b1 = this.ControlList.Key;
        break;
    }
    FlashMeaning flashMeaning1 = (FlashMeaning) null;
    FlashMeaning flashMeaning2 = (FlashMeaning) null;
    string b2 = Path.Combine(Directories.GetDatabasePathForExtension(Path.GetExtension(str)), str);
    foreach (FlashArea flashArea in (ReadOnlyCollection<FlashArea>) this.channel.FlashAreas)
    {
      foreach (FlashMeaning flashMeaning3 in (ReadOnlyCollection<FlashMeaning>) flashArea.FlashMeanings)
      {
        if (string.Equals(flashMeaning3.FileName, b2, StringComparison.OrdinalIgnoreCase))
        {
          flashMeaning2 = flashMeaning3;
          if (string.Equals(flashMeaning3.FlashKey, b1))
          {
            flashMeaning1 = flashMeaning3;
            break;
          }
        }
      }
    }
    if (flashMeaning1 == null && flashMeaning2 != null)
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Flash data was found by filename ({0}), but the desired key {1} did not match the key in the file ({2})", (object) flashMeaning2.FileName, (object) b1, (object) flashMeaning2.FlashKey), (StatusMessageType) 2, (object) this));
    return flashMeaning1 ?? flashMeaning2;
  }

  public static DiagnosisSource GetDiagnosisSourceForFlashware(string fileName)
  {
    return Directories.GetDiagnosisSourceForExtension(Path.GetExtension(fileName));
  }

  public bool TargetChannelRequiresBootLoaderFlash
  {
    get
    {
      Part part = (Part) null;
      if (this.DataSource == 2)
        part = this.EdexFileInformation?.ConfigurationInformation?.BootLoaderPartNumber;
      else if (this.DataSource == 1 && this.Bootcode?.Key != null)
        part = new Part(this.Bootcode.Key);
      return part != null && !PartExtensions.IsEqual(part, SapiManager.GetBootSoftwarePartNumber(this.Channel));
    }
  }

  public bool TargetChannelRequiresControlListFlash
  {
    get
    {
      return this.HasControlList && this.ControlList?.Key != null && !PartExtensions.IsEqual(new Part(this.ControlList.Key), SapiManager.GetControlListSoftwarePartNumber(this.Channel));
    }
  }

  public bool HasControlList => this.DataSource == 1 && this.ControlList != null;

  internal IEnumerable<Tuple<ProgrammingStep, DiagnosisSource>> RequiredDiagnosisSources
  {
    get
    {
      List<Tuple<ProgrammingStep, DiagnosisSource>> source = new List<Tuple<ProgrammingStep, DiagnosisSource>>();
      if (this.Firmware != null && (!this.TargetChannelHasSameFirmwareVersion || this.FlashRequiredSameFirmwareVersion || SapiManager.GetBootModeStatus(this.Channel)))
      {
        source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashFirmware, ProgrammingData.GetDiagnosisSourceForFlashware(this.Firmware.FileName)));
        if (this.TargetChannelRequiresBootLoaderFlash)
        {
          Part bootPartNumber = this.DataSource == 2 ? this.EdexFileInformation.ConfigurationInformation.BootLoaderPartNumber : new Part(this.Bootcode.Key);
          source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashBootLoader, ProgrammingData.GetDiagnosisSourceForFlashware((SapiManager.GlobalInstance.Sapi.FlashFiles.FirstOrDefault<FlashFile>((System.Func<FlashFile, bool>) (ff => ff.FlashAreas.Any<FlashArea>((System.Func<FlashArea, bool>) (fa => fa.FlashMeanings.Any<FlashMeaning>((System.Func<FlashMeaning, bool>) (fm => PartExtensions.IsEqual(bootPartNumber, fm.FlashKey))))))) ?? throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingBootFirmwareFlashKey, (object) bootPartNumber))).FileName)));
        }
      }
      if (this.TargetChannelRequiresControlListFlash)
      {
        Part controlListPart = this.DataSource == 2 ? (Part) null : new Part(this.ControlList.Key);
        source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashControlList, ProgrammingData.GetDiagnosisSourceForFlashware((SapiManager.GlobalInstance.Sapi.FlashFiles.FirstOrDefault<FlashFile>((System.Func<FlashFile, bool>) (ff => ff.FlashAreas.Any<FlashArea>((System.Func<FlashArea, bool>) (fa => fa.FlashMeanings.Any<FlashMeaning>((System.Func<FlashMeaning, bool>) (fm => PartExtensions.IsEqual(controlListPart, fm.FlashKey))))))) ?? throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingBootFirmwareFlashKey, (object) controlListPart))).FileName)));
      }
      if (this.HasDataSet)
      {
        foreach (Part neededDataSetVersion in this.TargetChannelNeededDataSetVersions)
        {
          Part part = neededDataSetVersion;
          FlashFile flashFile = SapiManager.GlobalInstance.Sapi.FlashFiles.FirstOrDefault<FlashFile>((System.Func<FlashFile, bool>) (ff => ff.FlashAreas.Any<FlashArea>((System.Func<FlashArea, bool>) (fa => fa.FlashMeanings.Any<FlashMeaning>((System.Func<FlashMeaning, bool>) (fm => PartExtensions.IsEqual(part, fm.FlashKey)))))));
          if (flashFile != null)
          {
            source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashDataSet, ProgrammingData.GetDiagnosisSourceForFlashware(flashFile.FileName)));
          }
          else
          {
            if (this.DataSource != 1 || this.DataSet == null)
              throw new DataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectUnitSubPanelItemFormat_MissingDatasetFlashKey, (object) part));
            source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.FlashDataSet, ProgrammingData.GetDiagnosisSourceForFlashware(this.DataSet.FileName)));
          }
        }
      }
      if (this.DataSource == 2 && (this.EdexFileInformation.ConfigurationInformation.SettingItems.Any<EdexSettingItem>() || this.EdexFileInformation.ConfigurationInformation.ApplicableProposedSettingItems().Any<EdexSettingItem>() || this.edexFileInformation.ConfigurationInformation.ChecSettings != null) || this.DataSource == 1 && this.settings != null)
      {
        DiagnosisSource diagnosisSource = this.Channel.Ecu.DiagnosisSource;
        if (this.Channel.Ecu.IsMcd && this.DataSource == 2 && this.Channel.CodingParameterGroups.Count == 0)
        {
          Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus.FirstOrDefault<Ecu>((System.Func<Ecu, bool>) (e => e.Name.Equals(this.Channel.Ecu.Name) && !e.IsMcd && SapiExtensions.FlashingRequiresMvci(e)));
          if (ecu != null)
            diagnosisSource = ecu.DiagnosisSource;
        }
        source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.UnlockBackdoorAndClearPasswords, diagnosisSource));
        source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.UnlockBackdoor, diagnosisSource));
        source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.ResetToDefault, diagnosisSource));
        source.Add(new Tuple<ProgrammingStep, DiagnosisSource>(ProgrammingStep.WriteSettings, diagnosisSource));
      }
      return source.Distinct<Tuple<ProgrammingStep, DiagnosisSource>>();
    }
  }

  internal ConnectionResource GetTargetConnectionResource(DiagnosisSource targetDiagnosisSource)
  {
    Ecu ecu1 = SapiManager.GlobalInstance.Sapi.Ecus.FirstOrDefault<Ecu>((System.Func<Ecu, bool>) (ecu => ecu.Name == this.Channel.Ecu.Name && ecu.DiagnosisSource == targetDiagnosisSource));
    return ecu1 != null ? ecu1.GetConnectionResources().FirstOrDefault<ConnectionResource>((System.Func<ConnectionResource, bool>) (cr => !cr.Restricted)) : (ConnectionResource) null;
  }

  public enum FlashBlock
  {
    BootLoader,
    Firmware,
    DataSet,
    ControlList,
  }
}
