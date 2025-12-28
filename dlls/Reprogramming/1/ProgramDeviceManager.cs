// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgramDeviceManager
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Net;
using DetroitDiesel.Security;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public sealed class ProgramDeviceManager : IDisposable
{
  private ProgrammingData data;
  private Dictionary<string, string> eventInfos;
  private DateTime startTime;
  private static ProgrammingStep[] UpdateFirmwareSteps = new ProgrammingStep[20]
  {
    ProgrammingStep.Connect,
    ProgrammingStep.ReadExistingSettings,
    ProgrammingStep.FlashBootLoader,
    ProgrammingStep.UnlockVeDocFirmware,
    ProgrammingStep.FlashFirmware,
    ProgrammingStep.FlashControlList,
    ProgrammingStep.ExecuteVersionSpecificInitialization,
    ProgrammingStep.UnlockBackdoor,
    ProgrammingStep.ResetToDefault,
    ProgrammingStep.UnlockBackdoor,
    ProgrammingStep.LoadExistingSettings,
    ProgrammingStep.LoadPresetSettings,
    ProgrammingStep.WriteSettings,
    ProgrammingStep.WriteEngineSerialNumber,
    ProgrammingStep.WriteVehicleIdentificationNumber,
    ProgrammingStep.CommitToNonvolatile,
    ProgrammingStep.Reconnect,
    ProgrammingStep.ExecutePostProgrammingActions,
    ProgrammingStep.VerifySettings,
    ProgrammingStep.UpdateUsageCount
  };
  private static ProgrammingStep[] UpdateFirmwareStepsWithDataSet = new ProgrammingStep[22]
  {
    ProgrammingStep.Connect,
    ProgrammingStep.ReadExistingSettings,
    ProgrammingStep.FlashBootLoader,
    ProgrammingStep.UnlockVeDocFirmware,
    ProgrammingStep.FlashFirmware,
    ProgrammingStep.UnlockVeDocDataSet,
    ProgrammingStep.FlashDataSet,
    ProgrammingStep.FlashControlList,
    ProgrammingStep.ExecuteVersionSpecificInitialization,
    ProgrammingStep.UnlockBackdoor,
    ProgrammingStep.ResetToDefault,
    ProgrammingStep.UnlockBackdoor,
    ProgrammingStep.LoadExistingSettings,
    ProgrammingStep.LoadPresetSettings,
    ProgrammingStep.WriteSettings,
    ProgrammingStep.WriteEngineSerialNumber,
    ProgrammingStep.WriteVehicleIdentificationNumber,
    ProgrammingStep.CommitToNonvolatile,
    ProgrammingStep.Reconnect,
    ProgrammingStep.ExecutePostProgrammingActions,
    ProgrammingStep.VerifySettings,
    ProgrammingStep.UpdateUsageCount
  };
  private static ProgrammingStep[] ReplaceFirmwareSteps = new ProgrammingStep[18]
  {
    ProgrammingStep.Connect,
    ProgrammingStep.FlashBootLoader,
    ProgrammingStep.UnlockVeDocFirmware,
    ProgrammingStep.FlashFirmware,
    ProgrammingStep.FlashControlList,
    ProgrammingStep.ExecuteVersionSpecificInitialization,
    ProgrammingStep.UnlockBackdoorAndClearPasswords,
    ProgrammingStep.ResetToDefault,
    ProgrammingStep.LoadServerSettings,
    ProgrammingStep.LoadPresetSettings,
    ProgrammingStep.WriteSettings,
    ProgrammingStep.WriteEngineSerialNumber,
    ProgrammingStep.WriteVehicleIdentificationNumber,
    ProgrammingStep.CommitToNonvolatile,
    ProgrammingStep.Reconnect,
    ProgrammingStep.ExecutePostProgrammingActions,
    ProgrammingStep.VerifySettings,
    ProgrammingStep.UpdateUsageCount
  };
  private static ProgrammingStep[] ReplaceFirmwareStepsWithDataSet = new ProgrammingStep[21]
  {
    ProgrammingStep.Connect,
    ProgrammingStep.FlashBootLoader,
    ProgrammingStep.UnlockVeDocFirmware,
    ProgrammingStep.FlashFirmware,
    ProgrammingStep.UnlockVeDocDataSet,
    ProgrammingStep.FlashDataSet,
    ProgrammingStep.FlashControlList,
    ProgrammingStep.ExecuteVersionSpecificInitialization,
    ProgrammingStep.UnlockBackdoorAndClearPasswords,
    ProgrammingStep.ResetToDefault,
    ProgrammingStep.LoadServerSettings,
    ProgrammingStep.LoadDataSetSettings,
    ProgrammingStep.LoadPresetSettings,
    ProgrammingStep.WriteSettings,
    ProgrammingStep.WriteEngineSerialNumber,
    ProgrammingStep.WriteVehicleIdentificationNumber,
    ProgrammingStep.CommitToNonvolatile,
    ProgrammingStep.Reconnect,
    ProgrammingStep.ExecutePostProgrammingActions,
    ProgrammingStep.VerifySettings,
    ProgrammingStep.UpdateUsageCount
  };
  private static ProgrammingStep[] ChangeDataSetSteps = new ProgrammingStep[12]
  {
    ProgrammingStep.Connect,
    ProgrammingStep.UnlockVeDocDataSet,
    ProgrammingStep.FlashDataSet,
    ProgrammingStep.UnlockBackdoor,
    ProgrammingStep.LoadDataSetSettings,
    ProgrammingStep.LoadPresetSettings,
    ProgrammingStep.WriteSettings,
    ProgrammingStep.CommitToNonvolatile,
    ProgrammingStep.Reconnect,
    ProgrammingStep.ExecutePostProgrammingActions,
    ProgrammingStep.VerifySettings,
    ProgrammingStep.UpdateUsageCount
  };
  private ProgrammingStep[] steps;
  private int stepIndex = -1;
  private bool waitingForTransitionToOnline;
  private List<Tuple<string, object, Exception>> parameterErrors;
  private Dictionary<string, object> parameterTargetValues;
  private Dictionary<string, CaesarException> partNumberExceptions;
  private ProgramDevicePage programDevicePage;
  private Queue<FlashMeaning> meaningsToFlash;
  private int totalMeaningsToFlashCount;
  private int indexOfMeaningCurrentlyBeingFlashed;
  private List<string> remainingPostProgrammingActions = new List<string>();
  private bool disposedValue;

  public ProgrammingStep CurrentStep
  {
    get
    {
      ProgrammingStep currentStep = ProgrammingStep.None;
      if (this.steps != null && this.stepIndex >= 0 && this.stepIndex < this.steps.Length)
        currentStep = this.steps[this.stepIndex];
      return currentStep;
    }
  }

  public static string GetProgrammingStepDescription(ProgrammingStep step)
  {
    return ((DescriptionAttribute) typeof (ProgrammingStep).GetField(Enum.GetName(typeof (ProgrammingStep), (object) step)).GetCustomAttributes(typeof (DescriptionAttribute), false)[0]).Description;
  }

  public ProgramDeviceManager(ProgramDevicePage parent)
  {
    this.eventInfos = new Dictionary<string, string>();
    this.programDevicePage = parent;
    ChannelCollection channels = SapiManager.GlobalInstance.Sapi.Channels;
    channels.ConnectCompleteEvent += new ConnectCompleteEventHandler(this.channels_ConnectCompleteEvent);
    channels.ConnectProgressEvent += new ConnectProgressEventHandler(this.channels_ConnectProgressEvent);
    SapiManager.GlobalInstance.ChannelInitializingEvent += new EventHandler<ChannelInitializingEventArgs>(this.GlobalInstance_ChannelInitializingEvent);
  }

  private void SubscribeEvents(Channel channel)
  {
    channel.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
    channel.Parameters.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.Parameters_ParameterUpdateEvent);
    channel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
    channel.FlashAreas.FlashProgressUpdateEvent += new FlashProgressUpdateEventHandler(this.FlashAreas_FlashProgressUpdateEvent);
    channel.FlashAreas.FlashCompleteEvent += new FlashCompleteEventHandler(this.FlashAreas_FlashCompleteEvent);
    channel.InitCompleteEvent += new InitCompleteEventHandler(this.channel_InitCompleteEvent);
  }

  private void UnsubscribeEvents(Channel channel)
  {
    channel.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
    channel.Parameters.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.Parameters_ParameterUpdateEvent);
    channel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
    channel.FlashAreas.FlashProgressUpdateEvent -= new FlashProgressUpdateEventHandler(this.FlashAreas_FlashProgressUpdateEvent);
    channel.FlashAreas.FlashCompleteEvent -= new FlashCompleteEventHandler(this.FlashAreas_FlashCompleteEvent);
    channel.InitCompleteEvent -= new InitCompleteEventHandler(this.channel_InitCompleteEvent);
  }

  public void Start(ProgrammingData programmingData)
  {
    this.data = programmingData;
    this.waitingForTransitionToOnline = false;
    this.parameterErrors = new List<Tuple<string, object, Exception>>();
    this.partNumberExceptions = (Dictionary<string, CaesarException>) null;
    this.parameterTargetValues = new Dictionary<string, object>();
    this.data.Channel.Ecu.Properties["Programming"] = "true";
    ServerDataManager.GlobalInstance.Programming = true;
    this.SubscribeEvents(this.data.Channel);
    this.eventInfos.Clear();
    this.eventInfos.Add("compatibilitywarningatstart", ServerDataManager.GlobalInstance.CompatibilityTable.IsCurrentSoftwareCompatible() ? "0" : "1");
    this.startTime = DateTime.Now;
    this.eventInfos.Add("faultcountatstart", this.data.Channel.FaultCodes.Current.Where<FaultCode>((System.Func<FaultCode, bool>) (x => x.FaultCodeIncidents.Current != null && SapiManager.IsFaultActionable(x.FaultCodeIncidents.Current))).Count<FaultCode>().ToString((IFormatProvider) CultureInfo.InvariantCulture));
    ProgrammingStep[] source = (ProgrammingStep[]) null;
    switch (this.data.Operation)
    {
      case ProgrammingOperation.Replace:
        source = this.data.HasDataSet ? ProgramDeviceManager.ReplaceFirmwareStepsWithDataSet : ProgramDeviceManager.ReplaceFirmwareSteps;
        break;
      case ProgrammingOperation.Update:
        source = this.data.HasDataSet ? ProgramDeviceManager.UpdateFirmwareStepsWithDataSet : ProgramDeviceManager.UpdateFirmwareSteps;
        break;
      case ProgrammingOperation.ChangeDataSet:
        Trace.Assert(this.data.HasDataSet);
        source = ProgramDeviceManager.ChangeDataSetSteps;
        break;
      case ProgrammingOperation.UpdateAndChangeDataSet:
        source = ProgramDeviceManager.UpdateFirmwareStepsWithDataSet;
        break;
    }
    IEnumerable<Tuple<ProgrammingStep, DiagnosisSource>> requiredDiagnosisSources = this.data.RequiredDiagnosisSources;
    List<ProgrammingStep> list = ((IEnumerable<ProgrammingStep>) source).Where<ProgrammingStep>((System.Func<ProgrammingStep, bool>) (x => this.IsAvailable(x, requiredDiagnosisSources.Select<Tuple<ProgrammingStep, DiagnosisSource>, ProgrammingStep>((System.Func<Tuple<ProgrammingStep, DiagnosisSource>, ProgrammingStep>) (rds => rds.Item1))))).ToList<ProgrammingStep>();
    if (list.Contains(ProgrammingStep.WriteVehicleIdentificationNumber) && this.data.Channel.Ecu.Properties.ContainsKey("WriteVINBeforeParameterization") && Convert.ToBoolean(this.data.Channel.Ecu.Properties["WriteVINBeforeParameterization"], (IFormatProvider) CultureInfo.InvariantCulture))
    {
      list.Remove(ProgrammingStep.WriteVehicleIdentificationNumber);
      list.Insert(list.IndexOf(ProgrammingStep.WriteSettings), ProgrammingStep.WriteVehicleIdentificationNumber);
    }
    if (!requiredDiagnosisSources.All<Tuple<ProgrammingStep, DiagnosisSource>>((System.Func<Tuple<ProgrammingStep, DiagnosisSource>, bool>) (rds => rds.Item2 == this.data.Channel.Ecu.DiagnosisSource)))
      this.InsertDiagnosisSourceSwitchSteps(list, requiredDiagnosisSources);
    this.steps = list.ToArray();
    this.stepIndex = -1;
    SapiManager.GlobalInstance.HoldLogFilesOpen = true;
    this.ExecuteNextStep();
  }

  private void InsertDiagnosisSourceSwitchSteps(
    List<ProgrammingStep> tempSteps,
    IEnumerable<Tuple<ProgrammingStep, DiagnosisSource>> requiredDiagnosisSources)
  {
    DiagnosisSource diagnosisSource = this.data.Channel.Ecu.DiagnosisSource;
    for (int i = 0; i < tempSteps.Count; i++)
    {
      Tuple<ProgrammingStep, DiagnosisSource> tuple = requiredDiagnosisSources.FirstOrDefault<Tuple<ProgrammingStep, DiagnosisSource>>((System.Func<Tuple<ProgrammingStep, DiagnosisSource>, bool>) (rds => rds.Item1 == tempSteps[i]));
      if (tuple != null && diagnosisSource != tuple.Item2)
      {
        tempSteps.Insert(i, tuple.Item2 == DiagnosisSource.CaesarDatabase ? ProgrammingStep.ConnectCaesar : ProgrammingStep.ConnectMvci);
        diagnosisSource = tuple.Item2;
        i++;
      }
    }
    if (tempSteps[0] != ProgrammingStep.Connect || tempSteps[1] != ProgrammingStep.ConnectCaesar && tempSteps[1] != ProgrammingStep.ConnectMvci)
      return;
    tempSteps.RemoveAt(0);
  }

  private void UpdateStep(string text, bool moveToNext, double? progress = null)
  {
    this.programDevicePage.UpdateStepText(this.data.Channel.Ecu, text, this.CurrentStep);
    if (progress.HasValue)
      this.programDevicePage.UpdateEcuProgress(this.data.Channel.Ecu, (double) this.stepIndex / (double) this.steps.Length * 100.0 + progress.Value / 100.0);
    if (!moveToNext)
      return;
    this.ExecuteNextStep();
  }

  private void Complete(bool success, string message) => this.Complete(success, message, true);

  private void Complete(bool success, string message, bool canRetry)
  {
    this.UnsubscribeEvents(this.data.Channel);
    this.data.Channel.Ecu.Properties["Programming"] = "false";
    SapiManager.GlobalInstance.HoldLogFilesOpen = false;
    this.waitingForTransitionToOnline = false;
    string str1 = Resources.ProgramDeviceManager_Status_OK;
    if (!success)
    {
      str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1}", (object) this.CurrentStep.ToString(), (object) message);
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Program Device operation '{0}' for {1} {2} failed because '{3}'", (object) this.data.Operation.ToString(), (object) this.data.EngineSerialNumber, (object) this.data.VehicleIdentificationNumber, (object) str1), (StatusMessageType) 1, (object) this));
    }
    else
    {
      switch (this.data.Operation)
      {
        case ProgrammingOperation.Update:
        case ProgrammingOperation.UpdateAndChangeDataSet:
          File.Delete(Path.Combine(Directories.DrumrollDownloadData, FileEncryptionProvider.EncryptFileName(this.data.Settings.FileName)));
          break;
      }
      File.Delete(this.data.AttemptInfoPath);
      if (this.data.AutomaticOperation != null)
        ServerDataManager.GlobalInstance.MarkAutomaticOperationComplete(this.data.AutomaticOperation);
      if (this.data.Operation == ProgrammingOperation.ChangeDataSet || this.data.Operation == ProgrammingOperation.UpdateAndChangeDataSet)
        this.data.Unit.GetInformationForDevice(this.data.Channel.Ecu.Name).UpdateCurrentDataSet(this.data.DataSet);
    }
    this.eventInfos.Add("totalreprogrammingtime", (DateTime.Now - this.startTime).ToString());
    if (success && this.data.DataSource == 2 && this.data.EdexFileInformation != null && this.data.EdexFileInformation.ConfigurationInformation.ChecSettings != null)
    {
      this.eventInfos.Add("checsettingsfilename", this.data.EdexFileInformation.ConfigurationInformation.ChecSettings.FileName);
      this.eventInfos.Add("checsettingstimestamp", this.data.EdexFileInformation.ConfigurationInformation.ChecSettings.Timestamp.ToString());
    }
    this.eventInfos.Add("packageProgrammingOperation", this.data.PackageProgrammingOperation.ToString());
    string str2 = this.data.Operation != ProgrammingOperation.Replace ? this.data.Operation.ToString() : (this.data.Settings == null ? (this.data.EdexFileInformation == null ? this.data.Operation.ToString() + "defaults" : this.data.Operation.ToString() + (object) this.data.EdexFileInformation.FileType) : this.data.Operation.ToString() + this.data.Settings.SettingsType);
    ServerDataManager.UpdateEventsFile(this.data.Channel, (IDictionary<string, string>) this.eventInfos, str2, this.data.EngineSerialNumber, this.data.VehicleIdentificationNumber, str1, this.data.ChargeType, this.data.ChargeText, false);
    SapiExtensions.LabelLogWithPrefix(SapiManager.GlobalInstance.Sapi.LogFiles, this.GetType().Name, $"{str2}: {str1}");
    ProgrammingStep currentStep = this.CurrentStep;
    this.stepIndex = -1;
    ServerDataManager.GlobalInstance.Programming = false;
    List<string> list = this.parameterErrors.Select(error => new
    {
      error = error,
      parameter = this.data.Channel.Parameters[error.Item1]
    }).Where(_param1 =>
    {
      if (_param1.parameter == null)
        return true;
      return _param1.error.Item2 != null && !_param1.error.Item2.Equals(_param1.parameter.Value);
    }).Select(_param1 => string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CouldNotSetParameterError, (object) _param1.error.Item1, _param1.error.Item2, (object) _param1.error.Item3.Message)).ToList<string>();
    this.programDevicePage.Complete(this.data, success, currentStep, message, list, this.partNumberExceptions, canRetry);
    if (success || !this.data.Channel.Online)
      return;
    ConnectionResource connectionResource = this.data.Channel.ConnectionResource;
    if (connectionResource == null)
      return;
    this.data.Channel.Disconnect();
    SapiManager.GlobalInstance.Sapi.Channels.Connect(connectionResource, false);
  }

  private void ExecuteNextStep()
  {
    string message = (string) null;
    ++this.stepIndex;
    this.programDevicePage.UpdateEcuProgress(this.data.Channel.Ecu, (double) this.stepIndex / (double) this.steps.Length * 100.0);
    if (this.data.Channel.Online || this.CurrentStep == ProgrammingStep.Connect || this.CurrentStep == ProgrammingStep.ConnectMvci || this.CurrentStep == ProgrammingStep.ConnectCaesar)
    {
      try
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", (object) this.stepIndex, (object) this.CurrentStep);
        StatusLog.Add(new StatusMessage(str, (StatusMessageType) 2, (object) this));
        SapiExtensions.LabelLogWithPrefix(SapiManager.GlobalInstance.Sapi.LogFiles, this.GetType().Name, str);
        switch (this.CurrentStep)
        {
          case ProgrammingStep.Connect:
            this.ConnectDevice(false, new DiagnosisSource?());
            break;
          case ProgrammingStep.ConnectCaesar:
            this.ConnectDevice(false, new DiagnosisSource?(DiagnosisSource.CaesarDatabase));
            break;
          case ProgrammingStep.ConnectMvci:
            this.ConnectDevice(false, new DiagnosisSource?(DiagnosisSource.McdDatabase));
            break;
          case ProgrammingStep.WriteEngineSerialNumber:
            if (!string.IsNullOrEmpty(this.data.EngineSerialNumber))
            {
              this.ServiceWrite("WriteEngineSerialNumberService", this.data.EngineSerialNumber);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.WriteVehicleIdentificationNumber:
            if (!string.IsNullOrEmpty(this.data.VehicleIdentificationNumber))
            {
              this.ServiceWrite("WriteVINService", this.data.VehicleIdentificationNumber);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.ReadExistingSettings:
            if (!File.Exists(Path.Combine(Directories.DrumrollDownloadData, FileEncryptionProvider.EncryptFileName(this.data.Settings.FileName))))
            {
              this.ReadParameters();
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_UsingSettingsFromPreviouslyFailedUpgrade, true);
            break;
          case ProgrammingStep.LoadExistingSettings:
          case ProgrammingStep.LoadServerSettings:
            if (this.data.DataSource == 1 && this.data.Settings != null)
            {
              this.LoadParameters(this.data.Settings.FileName, this.CurrentStep == ProgrammingStep.LoadExistingSettings ? ParameterFileFormat.VerFile : ParameterFileFormat.ParFile);
              break;
            }
            if (this.data.DataSource == 2 && this.data.EdexFileInformation != null && !this.data.EdexFileInformation.HasErrors)
            {
              this.LoadParameters(this.data.EdexFileInformation.ConfigurationInformation);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NAAllParametersResetToDefault, true);
            break;
          case ProgrammingStep.UnlockVeDocFirmware:
            if (this.data.Firmware != null && (!this.data.TargetChannelHasSameFirmwareVersion || this.data.FlashRequiredSameFirmwareVersion || SapiManager.GetBootModeStatus(this.data.Channel)))
            {
              if (ProgramDeviceManager.RequiresVeDocUnlock(this.data))
              {
                this.UnlockVeDoc();
                break;
              }
              this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.UnlockVeDocDataSet:
            if (this.data.HasDataSet && this.data.TargetChannelNeededDataSetVersions.Any<Part>())
            {
              if (ProgramDeviceManager.RequiresVeDocUnlock(this.data))
              {
                this.UnlockVeDoc();
                break;
              }
              this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.FlashBootLoader:
            if (this.data.TargetChannelRequiresBootLoaderFlash)
            {
              if (this.data.TargetChannelIsValidForFirmware)
              {
                this.Flash(ProgrammingData.FlashBlock.BootLoader);
                break;
              }
              this.Complete(false, Resources.ProgramDeviceManager_Complete_FirmwareIncompatibleWithHardware);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NAUsingExistingBootFirmware, true);
            break;
          case ProgrammingStep.FlashFirmware:
            if (this.data.Firmware != null && (!this.data.TargetChannelHasSameFirmwareVersion || this.data.FlashRequiredSameFirmwareVersion || SapiManager.GetBootModeStatus(this.data.Channel)))
            {
              if (this.data.TargetChannelIsValidForFirmware)
              {
                this.Flash(ProgrammingData.FlashBlock.Firmware);
                break;
              }
              this.Complete(false, Resources.ProgramDeviceManager_Complete_FirmwareIncompatibleWithHardware);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NAUsingExistingFirmware, true);
            break;
          case ProgrammingStep.FlashDataSet:
            if (this.data.HasDataSet)
            {
              if (this.data.TargetChannelNeededDataSetVersions.Any<Part>())
              {
                this.Flash(ProgrammingData.FlashBlock.DataSet);
                break;
              }
              this.UpdateStep(Resources.ProgramDeviceManager_Step_NAUsingExistingDataset, true);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.FlashControlList:
            if (this.data.HasControlList)
            {
              if (this.data.TargetChannelRequiresControlListFlash)
              {
                this.Flash(ProgrammingData.FlashBlock.ControlList);
                break;
              }
              this.UpdateStep(Resources.ProgramDeviceManager_Step_NAUsingExistingControlListFirmware, true);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.LoadDataSetSettings:
            if (this.data.DataSet != null)
            {
              if (this.data.DataSet.SettingsFileName != null && this.data.DataSet.SettingsFileName.Length > 0)
              {
                this.LoadParameters(this.data.DataSet.SettingsFileName, ParameterFileFormat.ParFile);
                break;
              }
              this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.LoadPresetSettings:
            SettingsInformation settingsForDevice = this.data.Unit.GetPresetSettingsForDevice(this.data.Channel.Ecu.Name);
            if (settingsForDevice != null && !string.IsNullOrEmpty(settingsForDevice.FileName))
            {
              this.LoadParameters(settingsForDevice.FileName, ParameterFileFormat.ParFile);
              break;
            }
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.UnlockBackdoor:
            this.UnlockBackdoor(false);
            break;
          case ProgrammingStep.UnlockBackdoorAndClearPasswords:
            this.UnlockBackdoor(true);
            break;
          case ProgrammingStep.ResetToDefault:
            this.ResetToDefault();
            break;
          case ProgrammingStep.ExecuteVersionSpecificInitialization:
            this.ExecuteVersionSpecificInitialization();
            break;
          case ProgrammingStep.WriteSettings:
            this.WriteParameters();
            break;
          case ProgrammingStep.CommitToNonvolatile:
            this.CommitToNonvolatile();
            break;
          case ProgrammingStep.Reconnect:
            this.ConnectDevice(true, new DiagnosisSource?());
            break;
          case ProgrammingStep.ExecutePostProgrammingActions:
            this.ExecutePostProgrammingActions();
            break;
          case ProgrammingStep.VerifySettings:
            if (this.data.Channel.Parameters.Count > 0)
            {
              this.data.Channel.WriteAllParametersToSummaryFiles = true;
              this.ReadParameters();
              break;
            }
            this.SaveHistory((ParameterCollection) null);
            this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
            break;
          case ProgrammingStep.UpdateUsageCount:
            this.UpdateDataUsageCount();
            break;
        }
      }
      catch (InvalidChecksumException ex)
      {
        message = ((Exception) ex).Message;
      }
      catch (DataException ex)
      {
        message = ex.Message;
      }
      catch (InvalidOperationException ex)
      {
        message = ex.Message;
      }
      catch (IOException ex)
      {
        message = ex.Message;
      }
      catch (CaesarException ex)
      {
        message = ex.Message;
      }
      catch (ArgumentException ex)
      {
        message = ex.Message;
      }
      if (message == null)
        return;
      this.Complete(false, message);
    }
    else
      this.Complete(false, Resources.ProgramDeviceManager_Complete_ChannelUnexpectedlyDisconnected);
  }

  private string GetServiceName(string serviceReference)
  {
    string serviceName = string.Empty;
    if (this.data.Channel.Ecu.Properties.ContainsKey(serviceReference))
      serviceName = this.data.Channel.Ecu.Properties[serviceReference];
    return serviceName;
  }

  private void UnlockBackdoor(bool clearPasswords)
  {
    bool flag = false;
    if (PasswordManager.HasPasswords(this.data.Channel))
    {
      PasswordManager passwordManager = PasswordManager.Create(this.data.Channel);
      if (passwordManager.Valid)
      {
        flag = true;
        this.data.Channel.Extension.Invoke("Unlock", (object[]) null);
        if (clearPasswords)
        {
          for (int index = 0; index < passwordManager.ProtectionListCount; ++index)
            passwordManager.ClearPasswordForList(index);
        }
      }
    }
    if (!flag)
      this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
  }

  public static bool RequiresVeDocUnlock(ProgrammingData data)
  {
    if (!data.Channel.Ecu.Properties.ContainsKey("HasVeDocProtection") || !Convert.ToBoolean(data.Channel.Ecu.Properties["HasVeDocProtection"], (IFormatProvider) CultureInfo.InvariantCulture))
      return false;
    return Convert.ToBoolean(data.Channel.Extension.Invoke("GetRequiresVeDocUnlock", (object[]) new string[1]
    {
      data.EngineSerialNumber
    }), (IFormatProvider) CultureInfo.InvariantCulture);
  }

  private void UnlockVeDoc()
  {
    if (NetworkSettings.GlobalInstance.UseManualUnlockDialog)
    {
      ActionsMenuProxy.GlobalInstance.ShowDialog("Unlock ECU for Reprogramming", (string) null, (object) null, true);
    }
    else
    {
      string property = this.data.Channel.Ecu.Properties["ServerUnlockForProgrammingAction"];
      if (!string.IsNullOrEmpty(property))
      {
        string str = property + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0},{1})", (object) this.data.VehicleIdentificationNumber, (object) this.data.EngineSerialNumber);
        SharedProcedureBase availableProcedure = SharedProcedureBase.AvailableProcedures[str];
        if (availableProcedure != null)
        {
          if (availableProcedure.CanStart)
          {
            availableProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StartComplete);
            availableProcedure.Start();
            return;
          }
          StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure was found but it could not be started: {0}", (object) availableProcedure.Name), (StatusMessageType) 2, (object) this));
        }
        else
          StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure was not found: {0}", (object) str), (StatusMessageType) 2, (object) this));
      }
      else
        StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} reports that it requires server based unlocking but does not define a procedure.", (object) this.data.Channel.Ecu.Name), (StatusMessageType) 2, (object) this));
    }
    this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
  }

  private void unlockSharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StartComplete);
    if (((ResultEventArgs) e).Succeeded && e.Result == 1)
    {
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} unlock via the server was initiated using procedure {1}", (object) this.data.Channel.Ecu.Name, (object) sharedProcedureBase.Name), (StatusMessageType) 2, (object) this));
      sharedProcedureBase.StopComplete += new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StopComplete);
    }
    else
    {
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed at start. {1}", (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty), (StatusMessageType) 2, (object) this));
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
    }
  }

  private void unlockSharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StopComplete);
    if (!((ResultEventArgs) e).Succeeded || e.Result == null)
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed. {1}", (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty), (StatusMessageType) 2, (object) this));
    else
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} was unlocked via the server using procedure {1}", (object) this.data.Channel.Ecu.Name, (object) sharedProcedureBase.Name), (StatusMessageType) 2, (object) this));
    this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
  }

  private void ResetToDefault()
  {
    int num = 0;
    if (this.data.Operation == ProgrammingOperation.Replace && !this.data.ReplaceToSameDevice)
      num = this.ExecuteService("ResetAllToDefaultNewDeviceService");
    if (num == 0)
      num = this.ExecuteService("ResetCalibrationsToDefaultService");
    if (num == 0)
      num = this.ExecuteService("ResetAllToDefaultService");
    if (num != 0)
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Resetting, false);
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
  }

  private void ExecuteVersionSpecificInitialization()
  {
    bool flag = this.data.Operation == ProgrammingOperation.Replace && !this.data.ReplaceToSameDevice;
    int num = 0;
    string text = string.Empty;
    StringBuilder stringBuilder = new StringBuilder();
    string softwareVersion = SapiManager.GetSoftwareVersion(this.data.Channel);
    if (!string.IsNullOrEmpty(softwareVersion))
    {
      foreach (DictionaryEntry applicableProperty in SapiExtensions.GetVariantApplicableProperties(this.data.Channel))
      {
        string key = applicableProperty.Key as string;
        if (flag)
        {
          if (key.StartsWith("ProgramNewDeviceService", StringComparison.OrdinalIgnoreCase))
          {
            string[] strArray = key.Split("|".ToCharArray());
            if (strArray.Length == 2 && softwareVersion.StartsWith(strArray[1], StringComparison.OrdinalIgnoreCase))
              stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0};", applicableProperty.Value);
          }
        }
        else if (key.StartsWith("ProgramExistingDeviceService", StringComparison.OrdinalIgnoreCase))
        {
          string[] strArray = key.Split("|".ToCharArray());
          if (strArray.Length == 3 && this.data.PreviousSoftwareVersion.StartsWith(strArray[1], StringComparison.OrdinalIgnoreCase) && softwareVersion.StartsWith(strArray[2], StringComparison.OrdinalIgnoreCase))
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0};", applicableProperty.Value);
        }
      }
    }
    if (stringBuilder.Length > 0)
    {
      string serviceList = stringBuilder.ToString().TrimEnd(";".ToCharArray());
      text = !flag ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatUpgradeFrom0To1Executing, (object) this.data.PreviousSoftwareVersion, (object) softwareVersion) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatReplaceTo0Executing, (object) softwareVersion);
      num = this.ExecuteServiceList(serviceList);
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} ({2} successfully queued)", (object) text, (object) serviceList, (object) num), (StatusMessageType) 2, (object) this));
    }
    if (num != 0)
      this.UpdateStep(text, false);
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
  }

  private void CommitToNonvolatile()
  {
    if (this.data.Channel.Ecu.Properties.ContainsKey("CommitToPermanentMemoryRequiresIgnitionCycle") && bool.Parse(this.data.Channel.Ecu.Properties["CommitToPermanentMemoryRequiresIgnitionCycle"]))
    {
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Committing, false);
      CycleIgnition cycleIgnition = new CycleIgnition(this.data.Channel);
      ((Control) this.programDevicePage.Wizard).BeginInvoke((Delegate) (() =>
      {
        cycleIgnition.ShowDialog();
        if (cycleIgnition.Succeeded)
        {
          this.SubscribeEvents(cycleIgnition.Channel);
          this.data.Channel = cycleIgnition.Channel;
          this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
        }
        else
          this.Complete(false, Resources.ProgramDeviceManager_Complete_CycleIgnitionFailed);
      }));
    }
    else if (this.ExecuteService("CommitToPermanentMemoryService") != 0)
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Committing, false);
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
  }

  private void ServiceWrite(string abstractedServiceName, string dataToWrite)
  {
    Service service = this.data.Channel.Services[this.GetServiceName(abstractedServiceName)];
    if (service != (Service) null)
    {
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Writing, false);
      service.InputValues[0].Value = (object) dataToWrite;
      this.ExecuteService(service);
    }
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
  }

  private int ExecuteService(string abstractedServiceName)
  {
    string serviceName = this.GetServiceName(abstractedServiceName);
    return !string.IsNullOrEmpty(serviceName) ? this.ExecuteServiceList(serviceName) : 0;
  }

  private int ExecuteServiceList(string serviceList)
  {
    this.data.Channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
    int num = this.data.Channel.Services.Execute(serviceList, false);
    if (num == 0)
      this.data.Channel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
    return num;
  }

  private void ExecuteService(Service service)
  {
    this.data.Channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
    service.Execute(false);
  }

  private void SaveHistory(ParameterCollection parameters)
  {
    if (this.CurrentStep == ProgrammingStep.ReadExistingSettings && parameters != null)
    {
      bool flag = !parameters.Channel.Ecu.Properties.ContainsKey("ResetCalibrationsToDefaultService") && parameters.Channel.Ecu.Properties.ContainsKey("ResetAllToDefaultService");
      ServerDataManager.SaveSettings(parameters, Directories.DrumrollDownloadData, this.data.Settings.FileName, ParameterFileFormat.VerFile, flag);
      ServerDataManager.GlobalInstance.AutoSaveSettings(this.data.Channel, (ServerDataManager.AutoSaveDestination) 1, "Pre" + this.data.Operation.ToString());
    }
    else
    {
      if (this.CurrentStep != ProgrammingStep.VerifySettings)
        return;
      ServerDataManager.GlobalInstance.AutoSaveSettings(this.data.Channel, (ServerDataManager.AutoSaveDestination) 1, "Post" + this.data.Operation.ToString());
      if (this.data.DataSource != 2 || !NetworkSettings.GlobalInstance.SaveUploadContent)
        return;
      if (this.data.Channel.Parameters.Count > 0)
        ServerDataManager.GlobalInstance.AutoSaveSettings(this.data.Channel, (ServerDataManager.AutoSaveDestination) 0, this.data.Operation.ToString());
      else
        ServerDataManager.GlobalInstance.GenerateEdexSettingsForUpload(this.data.Channel, this.data.EdexFileInformation.ConfigurationInformation, this.data.Operation.ToString());
    }
  }

  private void ReadParameters()
  {
    if (this.data.Channel.Parameters.Count > 0)
    {
      this.data.Channel.Parameters.Read(false);
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Reading, false);
    }
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
  }

  private void WriteParameters()
  {
    foreach (Parameter parameter in this.data.Channel.Parameters.Where<Parameter>((System.Func<Parameter, bool>) (p => p.Marked)))
      this.parameterTargetValues[string.Join(".", parameter.GroupQualifier, parameter.Qualifier)] = parameter.Value;
    this.data.Channel.Parameters.Write(false);
  }

  private void LoadParameters(string fileName, ParameterFileFormat format)
  {
    this.UpdateStep(Resources.ProgramDeviceManager_Step_LoadingParameters, false);
    StringDictionary stringDictionary = new StringDictionary();
    ServerDataManager.LoadSettings(Directories.DrumrollDownloadData, fileName, (EncryptionType) 1, format, this.data.Channel.Parameters, stringDictionary);
    foreach (DictionaryEntry dictionaryEntry in stringDictionary)
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not set unknown parameter '{0}' to value '{1}'", dictionaryEntry.Key, dictionaryEntry.Value), (StatusMessageType) 2, (object) this));
    this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
  }

  private void LoadParameters(EdexConfigurationInformation edexConfiguration)
  {
    this.UpdateStep(Resources.ProgramDeviceManager_Step_LoadingParameters, false);
    this.partNumberExceptions = new Dictionary<string, CaesarException>();
    foreach (object obj in edexConfiguration.LoadSettingsToChannel(this.data.Channel, true, (IDictionary<string, CaesarException>) this.partNumberExceptions))
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not set unknown parameter '{0}'", obj), (StatusMessageType) 2, (object) this));
    if (this.partNumberExceptions.Any<KeyValuePair<string, CaesarException>>())
      throw this.partNumberExceptions.First<KeyValuePair<string, CaesarException>>().Value;
    this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
  }

  private void Flash(ProgrammingData.FlashBlock which)
  {
    IEnumerable<FlashMeaning> flashMeanings = (IEnumerable<FlashMeaning>) this.data.GetFlashMeanings(which);
    if (flashMeanings != null)
    {
      this.meaningsToFlash = new Queue<FlashMeaning>(flashMeanings);
      this.totalMeaningsToFlashCount = 0;
      this.indexOfMeaningCurrentlyBeingFlashed = 0;
      if (this.meaningsToFlash != null && this.meaningsToFlash.Any<FlashMeaning>())
      {
        this.totalMeaningsToFlashCount = this.meaningsToFlash.Count;
        this.FlashNextMeaning();
      }
      else
        this.Complete(false, Resources.ProgramDeviceManager_Complete_CouldNotLocateFlashKey);
    }
    else
      this.Complete(false, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Complete_FlashBlock_not_found_for_connected_hardware, (object) which));
  }

  private void FlashNextMeaning()
  {
    FlashMeaning flashMeaning = this.meaningsToFlash.Dequeue();
    ++this.indexOfMeaningCurrentlyBeingFlashed;
    foreach (FlashArea flashArea in (ReadOnlyCollection<FlashArea>) this.data.Channel.FlashAreas)
      flashArea.Marked = flashMeaning.FlashArea == flashArea ? flashMeaning : (FlashMeaning) null;
    this.UpdateStep(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_Starting_Count_Format, (object) this.indexOfMeaningCurrentlyBeingFlashed, (object) this.totalMeaningsToFlashCount), false);
    this.data.Channel.Services.SetListVariable("ESN", this.data.EngineSerialNumber ?? string.Empty);
    this.data.Channel.Services.SetListVariable("VIN", this.data.VehicleIdentificationNumber ?? string.Empty);
    this.data.Channel.FlashAreas.Flash(false);
  }

  private void UpdateDataUsageCount()
  {
    DeviceInformation informationForDevice = this.data.Unit.GetInformationForDevice(this.data.Channel.Ecu.Name);
    if (informationForDevice != null)
    {
      ++informationForDevice.UsageCount;
      this.data.Unit.CheckAndUpdateExpiredStatus();
      ServerDataManager.GlobalInstance.SaveUnitXml();
    }
    this.Complete(true, Resources.ProgramDeviceManager_Step_Complete);
  }

  private void ConnectDevice(bool reconnect, DiagnosisSource? targetDiagnosisSource)
  {
    if (this.data.Channel.Online)
    {
      if (reconnect || targetDiagnosisSource.HasValue && targetDiagnosisSource.Value != this.data.Channel.Ecu.DiagnosisSource)
      {
        this.data.Channel.Disconnect();
      }
      else
      {
        this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
        return;
      }
    }
    ConnectionResource connectionResource = this.data.Channel.ConnectionResource;
    if (targetDiagnosisSource.HasValue)
    {
      DiagnosisSource? nullable = targetDiagnosisSource;
      DiagnosisSource diagnosisSource = connectionResource.Ecu.DiagnosisSource;
      if ((nullable.GetValueOrDefault() == diagnosisSource ? (!nullable.HasValue ? 1 : 0) : 1) != 0)
      {
        connectionResource = this.data.GetTargetConnectionResource(targetDiagnosisSource.Value);
        if (connectionResource != null)
        {
          if ((int) connectionResource.Type[0] != (int) this.data.Channel.ConnectionResource.Type[0])
          {
            int num = (int) ControlHelpers.ShowMessageBox(string.Format((IFormatProvider) CultureInfo.CurrentCulture, connectionResource.Type[0] == 'C' ? Resources.ProgramDeviceManager_MessagFormatConnectToCAN : Resources.ProgramDeviceManager_MessagFormatConnectToEthernet, (object) this.data.Channel.Ecu.Name), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
          }
        }
        else
        {
          this.Complete(false, Resources.ProgramDeviceManager_Complete_ConnectionResourceForTargetPlatformNotFound);
          return;
        }
      }
    }
    if (connectionResource != null)
    {
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Connecting, false);
      SapiManager.GlobalInstance.Sapi.Channels.Connect(connectionResource, false);
    }
    else
      this.Complete(false, Resources.ProgramDeviceManager_Complete_ConnectionResourceIsNotValid);
  }

  private void channels_ConnectProgressEvent(object sender, ProgressEventArgs e)
  {
    if (this.CurrentStep != ProgrammingStep.Reconnect)
      return;
    this.UpdateStep(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_FormatPercentComplete, (object) e.PercentComplete), false, new double?(e.PercentComplete));
  }

  private void channels_ConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.CurrentStep == ProgrammingStep.CommitToNonvolatile)
      return;
    if (!e.Succeeded)
    {
      if (this.CurrentStep == ProgrammingStep.None || sender is ConnectionResource connectionResource && connectionResource.Ecu.Name != this.data.Channel.Ecu.Name)
        return;
      this.Complete(false, e.Exception.Message);
    }
    else
    {
      Channel channel = sender as Channel;
      if (this.CurrentStep == ProgrammingStep.None || channel.Ecu.Name != this.data.Channel.Ecu.Name)
        return;
      switch (this.CurrentStep)
      {
        case ProgrammingStep.Connect:
        case ProgrammingStep.ConnectCaesar:
        case ProgrammingStep.ConnectMvci:
        case ProgrammingStep.FlashBootLoader:
        case ProgrammingStep.FlashFirmware:
        case ProgrammingStep.FlashDataSet:
        case ProgrammingStep.FlashControlList:
        case ProgrammingStep.Reconnect:
          this.SubscribeEvents(channel);
          this.data.Channel = channel;
          if (channel.CommunicationsState == CommunicationsState.OnlineButNotInitialized || channel.CommunicationsState == CommunicationsState.ReadEcuInfo)
          {
            this.waitingForTransitionToOnline = true;
            this.UpdateStep(Resources.ProgramDeviceManager_Step_WaitingForOnlineStatus, false);
            break;
          }
          this.CheckBootModeAndMoveOn();
          break;
        default:
          this.Complete(false, Resources.ProgramDeviceManager_Complete_UnexpectedSequenceInConnectComplete);
          break;
      }
    }
  }

  private void GlobalInstance_ChannelInitializingEvent(
    object sender,
    ChannelInitializingEventArgs e)
  {
    if (this.CurrentStep == ProgrammingStep.None || e.Channel.Ecu.Name != this.data.Channel.Ecu.Name)
      return;
    switch (this.CurrentStep)
    {
      case ProgrammingStep.Connect:
      case ProgrammingStep.ConnectCaesar:
      case ProgrammingStep.ConnectMvci:
      case ProgrammingStep.FlashBootLoader:
      case ProgrammingStep.FlashFirmware:
      case ProgrammingStep.FlashDataSet:
      case ProgrammingStep.FlashControlList:
        e.AutoRead = false;
        using (IEnumerator<EcuInfo> enumerator = e.Channel.EcuInfos.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            EcuInfo current = enumerator.Current;
            if (!current.Qualifier.StartsWith("CO", StringComparison.Ordinal))
              current.Marked = false;
          }
          break;
        }
    }
    e.Channel.Parameters.AutoReadSummaryParameters = false;
  }

  private void Parameters_ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    Parameter parameter = sender as Parameter;
    if (this.CurrentStep == ProgrammingStep.ReadExistingSettings || this.CurrentStep == ProgrammingStep.VerifySettings || this.CurrentStep == ProgrammingStep.WriteSettings)
      this.UpdateStep(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_FormatPercentComplete, (object) parameter.Channel.Parameters.Progress), false, new double?((double) parameter.Channel.Parameters.Progress));
    if (e.Succeeded)
      return;
    StatusLog.Add(new StatusMessage(e.Exception.Message, (StatusMessageType) 1, (object) parameter));
  }

  private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    ParameterCollection parameters = sender as ParameterCollection;
    if (parameters.Channel != this.data.Channel || this.CurrentStep != ProgrammingStep.ReadExistingSettings && this.CurrentStep != ProgrammingStep.VerifySettings)
      return;
    if (e.Succeeded)
    {
      this.SaveHistory(parameters);
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
    }
    else
      this.Complete(false, e.Exception.Message);
  }

  private void FlashAreas_FlashProgressUpdateEvent(object sender, ProgressEventArgs e)
  {
    if (this.totalMeaningsToFlashCount > 1)
      this.UpdateStep(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_FormatPercentCompleteMultiple, (object) e.PercentComplete, (object) this.indexOfMeaningCurrentlyBeingFlashed, (object) this.totalMeaningsToFlashCount), false, new double?(e.PercentComplete));
    else
      this.UpdateStep(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_FormatPercentComplete, (object) e.PercentComplete), false, new double?(e.PercentComplete));
  }

  private void FlashAreas_FlashCompleteEvent(object sender, ResultEventArgs e)
  {
    if (e.Succeeded && this.meaningsToFlash != null)
    {
      if (!this.meaningsToFlash.Any<FlashMeaning>())
      {
        this.meaningsToFlash = (Queue<FlashMeaning>) null;
        this.totalMeaningsToFlashCount = 0;
        this.UpdateStep(Resources.ProgramDeviceManager_Step_Reconnecting, false);
        if (sender is FlashAreaCollection flashAreaCollection)
        {
          ConnectionResource connectionResource = flashAreaCollection.Channel.ConnectionResource;
          if (connectionResource != null)
          {
            flashAreaCollection.Channel.Disconnect();
            this.UnsubscribeEvents(flashAreaCollection.Channel);
            SapiManager.GlobalInstance.Sapi.Channels.Connect(connectionResource, false);
          }
          else
            this.Complete(false, Resources.ProgramDeviceManager_Complete_ConnectionResourceNotAvailable);
        }
        else
          this.Complete(false, Resources.ProgramDeviceManager_Complete_FlashAreaCollectionNotAvailable);
      }
      else
        this.FlashNextMeaning();
    }
    else
    {
      if (this.meaningsToFlash != null)
      {
        this.meaningsToFlash.Clear();
        this.meaningsToFlash = (Queue<FlashMeaning>) null;
        this.totalMeaningsToFlashCount = 0;
      }
      this.Complete(false, e.Exception.Message);
    }
  }

  private void AppendParameterError(string qualifier, object intendedValue, Exception exception)
  {
    this.parameterErrors.Add(Tuple.Create<string, object, Exception>(qualifier, intendedValue, exception));
  }

  private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    if (e.Succeeded)
    {
      foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) (sender as ParameterCollection))
      {
        if (parameter.Marked && parameter.Exception != null)
        {
          string str = string.Join(".", parameter.GroupQualifier, parameter.Qualifier);
          object parameterTargetValue = this.parameterTargetValues.ContainsKey(str) ? this.parameterTargetValues[str] : (object) null;
          this.AppendParameterError(str, parameterTargetValue, parameter.Exception);
        }
      }
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
    }
    else
      this.Complete(false, e.Exception.Message);
  }

  private void Services_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.data.Channel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
    }
    else
    {
      Service service = sender as Service;
      if (service != (Service) null && service.ServiceTypes == ServiceTypes.Download && service.InputValues.Count > 0)
      {
        if (this.CurrentStep != ProgrammingStep.WriteVehicleIdentificationNumber || SapiManager.GetVehicleIdentificationNumber(this.data.Channel) != this.data.VehicleIdentificationNumber)
          this.AppendParameterError(service.Qualifier, service.InputValues[0].Value, e.Exception);
        this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
      }
      else
        this.Complete(false, e.Exception.Message);
    }
  }

  private void channel_InitCompleteEvent(object sender, EventArgs e)
  {
    if (sender != this.data.Channel || !this.waitingForTransitionToOnline)
      return;
    this.waitingForTransitionToOnline = false;
    this.CheckBootModeAndMoveOn();
  }

  private void CheckBootModeAndMoveOn()
  {
    bool flag = this.CurrentStep == ProgrammingStep.FlashDataSet || this.CurrentStep == ProgrammingStep.FlashFirmware || this.CurrentStep == ProgrammingStep.FlashControlList;
    if (flag && this.data.DeferBootModeCheck)
    {
      switch (this.CurrentStep)
      {
        case ProgrammingStep.FlashFirmware:
          flag = !this.data.HasDataSet && !this.data.HasControlList;
          break;
        case ProgrammingStep.FlashDataSet:
          flag = !this.data.HasControlList && (this.meaningsToFlash == null || !this.meaningsToFlash.Any<FlashMeaning>());
          break;
      }
    }
    if (flag)
    {
      if (SapiManager.GetBootModeStatus(this.data.Channel))
        this.Complete(false, Resources.ProgramDeviceManager_Complete_DataImplausibleReconnectedInBootMode);
      else if (this.data.Channel.DiagnosisVariant.IsBase)
        this.Complete(false, Resources.ProgramDeviceManager_Complete_EcuSoftwareVersionTooNew, false);
      else
        this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
    }
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
  }

  private void ExecutePostProgrammingActions()
  {
    this.remainingPostProgrammingActions.Clear();
    if (this.data.Channel.Ecu.Properties.ContainsKey("PostProgrammingActions"))
    {
      this.remainingPostProgrammingActions.AddRange((IEnumerable<string>) this.data.Channel.Ecu.Properties["PostProgrammingActions"].Split(";".ToCharArray()));
      this.ExecuteNextPostProgrammingAction();
    }
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, true);
  }

  private void ExecuteNextPostProgrammingAction()
  {
    if (this.remainingPostProgrammingActions.Count > 0)
    {
      string programmingAction = this.remainingPostProgrammingActions[0];
      SharedProcedureBase availableProcedure = SharedProcedureBase.AvailableProcedures[programmingAction];
      this.remainingPostProgrammingActions.RemoveAt(0);
      if (availableProcedure != null)
      {
        if (availableProcedure.CanStart)
        {
          availableProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.sharedProcedure_StartComplete);
          availableProcedure.Start();
        }
        else
        {
          StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure was found but it could not be started: {0}", (object) availableProcedure.Name), (StatusMessageType) 2, (object) this));
          this.ExecuteNextPostProgrammingAction();
        }
      }
      else
      {
        StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure was not found: {0}", (object) programmingAction), (StatusMessageType) 2, (object) this));
        this.ExecuteNextPostProgrammingAction();
      }
    }
    else
      this.UpdateStep(Resources.ProgramDeviceManager_Step_Complete, true);
  }

  private void sharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.sharedProcedure_StartComplete);
    if (((ResultEventArgs) e).Succeeded && e.Result == 1)
    {
      sharedProcedureBase.StopComplete += new EventHandler<PassFailResultEventArgs>(this.sharedProcedure_StopComplete);
      this.UpdateStep(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_FormatProcessing, (object) sharedProcedureBase.Name), false);
    }
    else
    {
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed at start. {1}", (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty), (StatusMessageType) 2, (object) this));
      this.ExecuteNextPostProgrammingAction();
    }
  }

  private void sharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.sharedProcedure_StopComplete);
    if (!((ResultEventArgs) e).Succeeded || e.Result == null)
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed. {1}", (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty), (StatusMessageType) 2, (object) this));
    this.ExecuteNextPostProgrammingAction();
  }

  private bool IsAvailable(
    ProgrammingStep step,
    IEnumerable<ProgrammingStep> requiredFlashStepsForData)
  {
    bool flag = true;
    switch (step)
    {
      case ProgrammingStep.WriteEngineSerialNumber:
        if (string.IsNullOrEmpty(this.data.EngineSerialNumber))
        {
          flag = false;
          break;
        }
        if (string.IsNullOrEmpty(this.GetServiceName("WriteEngineSerialNumberService")))
        {
          flag = false;
          break;
        }
        break;
      case ProgrammingStep.WriteVehicleIdentificationNumber:
        if (string.IsNullOrEmpty(this.data.VehicleIdentificationNumber))
        {
          flag = false;
          break;
        }
        if (string.IsNullOrEmpty(this.GetServiceName("WriteVINService")))
        {
          flag = false;
          break;
        }
        break;
      case ProgrammingStep.UnlockVeDocFirmware:
        if (!this.data.Channel.Ecu.Properties.ContainsKey("HasVeDocProtection") || !requiredFlashStepsForData.Contains<ProgrammingStep>(ProgrammingStep.FlashFirmware))
        {
          flag = false;
          break;
        }
        break;
      case ProgrammingStep.UnlockVeDocDataSet:
        if (!this.data.Channel.Ecu.Properties.ContainsKey("HasVeDocProtection") || !requiredFlashStepsForData.Contains<ProgrammingStep>(ProgrammingStep.FlashDataSet))
        {
          flag = false;
          break;
        }
        break;
      case ProgrammingStep.FlashBootLoader:
      case ProgrammingStep.FlashFirmware:
      case ProgrammingStep.FlashDataSet:
      case ProgrammingStep.FlashControlList:
        if (!requiredFlashStepsForData.Contains<ProgrammingStep>(step))
        {
          flag = false;
          break;
        }
        break;
      case ProgrammingStep.LoadDataSetSettings:
        if (this.data.DataSet == null || string.IsNullOrEmpty(this.data.DataSet.SettingsFileName))
        {
          flag = false;
          break;
        }
        break;
      case ProgrammingStep.LoadPresetSettings:
        SettingsInformation settingsForDevice = this.data.Unit.GetPresetSettingsForDevice(this.data.Channel.Ecu.Name);
        if (settingsForDevice == null || string.IsNullOrEmpty(settingsForDevice.FileName))
        {
          flag = false;
          break;
        }
        break;
      case ProgrammingStep.UnlockBackdoor:
      case ProgrammingStep.UnlockBackdoorAndClearPasswords:
        if (!PasswordManager.HasPasswords(this.data.Channel))
        {
          flag = false;
          break;
        }
        break;
      case ProgrammingStep.ExecuteVersionSpecificInitialization:
        flag = this.data.Channel.Ecu.Properties.OfType<DictionaryEntry>().Any<DictionaryEntry>((System.Func<DictionaryEntry, bool>) (p => ((string) p.Key).StartsWith("ProgramNewDeviceService", StringComparison.OrdinalIgnoreCase) || ((string) p.Key).StartsWith("ProgramExistingDeviceService", StringComparison.OrdinalIgnoreCase)));
        break;
      case ProgrammingStep.ExecutePostProgrammingActions:
        if (!this.data.Channel.Ecu.Properties.ContainsKey("PostProgrammingActions"))
        {
          flag = false;
          break;
        }
        break;
    }
    return flag;
  }

  private void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      ChannelCollection channels = SapiManager.GlobalInstance.Sapi.Channels;
      channels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.channels_ConnectCompleteEvent);
      channels.ConnectProgressEvent -= new ConnectProgressEventHandler(this.channels_ConnectProgressEvent);
      SapiManager.GlobalInstance.ChannelInitializingEvent -= new EventHandler<ChannelInitializingEventArgs>(this.GlobalInstance_ChannelInitializingEvent);
    }
    this.data = (ProgrammingData) null;
    this.programDevicePage = (ProgramDevicePage) null;
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
