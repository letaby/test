#define TRACE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
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
			ProgrammingStep result = ProgrammingStep.None;
			if (steps != null && stepIndex >= 0 && stepIndex < steps.Length)
			{
				result = steps[stepIndex];
			}
			return result;
		}
	}

	public static string GetProgrammingStepDescription(ProgrammingStep step)
	{
		FieldInfo field = typeof(ProgrammingStep).GetField(Enum.GetName(typeof(ProgrammingStep), step));
		DescriptionAttribute descriptionAttribute = (DescriptionAttribute)field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0];
		return descriptionAttribute.Description;
	}

	public ProgramDeviceManager(ProgramDevicePage parent)
	{
		eventInfos = new Dictionary<string, string>();
		programDevicePage = parent;
		ChannelCollection channels = SapiManager.GlobalInstance.Sapi.Channels;
		channels.ConnectCompleteEvent += channels_ConnectCompleteEvent;
		channels.ConnectProgressEvent += channels_ConnectProgressEvent;
		SapiManager.GlobalInstance.ChannelInitializingEvent += GlobalInstance_ChannelInitializingEvent;
	}

	private void SubscribeEvents(Channel channel)
	{
		channel.Parameters.ParametersWriteCompleteEvent += Parameters_ParametersWriteCompleteEvent;
		channel.Parameters.ParameterUpdateEvent += Parameters_ParameterUpdateEvent;
		channel.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
		channel.FlashAreas.FlashProgressUpdateEvent += FlashAreas_FlashProgressUpdateEvent;
		channel.FlashAreas.FlashCompleteEvent += FlashAreas_FlashCompleteEvent;
		channel.InitCompleteEvent += channel_InitCompleteEvent;
	}

	private void UnsubscribeEvents(Channel channel)
	{
		channel.Parameters.ParametersWriteCompleteEvent -= Parameters_ParametersWriteCompleteEvent;
		channel.Parameters.ParameterUpdateEvent -= Parameters_ParameterUpdateEvent;
		channel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
		channel.FlashAreas.FlashProgressUpdateEvent -= FlashAreas_FlashProgressUpdateEvent;
		channel.FlashAreas.FlashCompleteEvent -= FlashAreas_FlashCompleteEvent;
		channel.InitCompleteEvent -= channel_InitCompleteEvent;
	}

	public void Start(ProgrammingData programmingData)
	{
		data = programmingData;
		waitingForTransitionToOnline = false;
		parameterErrors = new List<Tuple<string, object, Exception>>();
		partNumberExceptions = null;
		parameterTargetValues = new Dictionary<string, object>();
		data.Channel.Ecu.Properties["Programming"] = "true";
		ServerDataManager.GlobalInstance.Programming = true;
		SubscribeEvents(data.Channel);
		eventInfos.Clear();
		eventInfos.Add("compatibilitywarningatstart", ServerDataManager.GlobalInstance.CompatibilityTable.IsCurrentSoftwareCompatible() ? "0" : "1");
		startTime = DateTime.Now;
		int num = data.Channel.FaultCodes.Current.Where((FaultCode x) => x.FaultCodeIncidents.Current != null && SapiManager.IsFaultActionable(x.FaultCodeIncidents.Current)).Count();
		eventInfos.Add("faultcountatstart", num.ToString(CultureInfo.InvariantCulture));
		ProgrammingStep[] source = null;
		switch (data.Operation)
		{
		case ProgrammingOperation.Update:
			source = (data.HasDataSet ? UpdateFirmwareStepsWithDataSet : UpdateFirmwareSteps);
			break;
		case ProgrammingOperation.Replace:
			source = (data.HasDataSet ? ReplaceFirmwareStepsWithDataSet : ReplaceFirmwareSteps);
			break;
		case ProgrammingOperation.ChangeDataSet:
			Trace.Assert(data.HasDataSet);
			source = ChangeDataSetSteps;
			break;
		case ProgrammingOperation.UpdateAndChangeDataSet:
			source = UpdateFirmwareStepsWithDataSet;
			break;
		}
		IEnumerable<Tuple<ProgrammingStep, DiagnosisSource>> requiredDiagnosisSources = data.RequiredDiagnosisSources;
		List<ProgrammingStep> list = source.Where((ProgrammingStep x) => IsAvailable(x, requiredDiagnosisSources.Select((Tuple<ProgrammingStep, DiagnosisSource> rds) => rds.Item1))).ToList();
		if (list.Contains(ProgrammingStep.WriteVehicleIdentificationNumber) && data.Channel.Ecu.Properties.ContainsKey("WriteVINBeforeParameterization") && Convert.ToBoolean(data.Channel.Ecu.Properties["WriteVINBeforeParameterization"], CultureInfo.InvariantCulture))
		{
			list.Remove(ProgrammingStep.WriteVehicleIdentificationNumber);
			list.Insert(list.IndexOf(ProgrammingStep.WriteSettings), ProgrammingStep.WriteVehicleIdentificationNumber);
		}
		if (!requiredDiagnosisSources.All((Tuple<ProgrammingStep, DiagnosisSource> rds) => rds.Item2 == data.Channel.Ecu.DiagnosisSource))
		{
			InsertDiagnosisSourceSwitchSteps(list, requiredDiagnosisSources);
		}
		steps = list.ToArray();
		stepIndex = -1;
		SapiManager.GlobalInstance.HoldLogFilesOpen = true;
		ExecuteNextStep();
	}

	private void InsertDiagnosisSourceSwitchSteps(List<ProgrammingStep> tempSteps, IEnumerable<Tuple<ProgrammingStep, DiagnosisSource>> requiredDiagnosisSources)
	{
		DiagnosisSource diagnosisSource = data.Channel.Ecu.DiagnosisSource;
		int i;
		for (i = 0; i < tempSteps.Count; i++)
		{
			Tuple<ProgrammingStep, DiagnosisSource> tuple = requiredDiagnosisSources.FirstOrDefault((Tuple<ProgrammingStep, DiagnosisSource> rds) => rds.Item1 == tempSteps[i]);
			if (tuple != null && diagnosisSource != tuple.Item2)
			{
				tempSteps.Insert(i, (tuple.Item2 == DiagnosisSource.CaesarDatabase) ? ProgrammingStep.ConnectCaesar : ProgrammingStep.ConnectMvci);
				diagnosisSource = tuple.Item2;
				i++;
			}
		}
		if (tempSteps[0] == ProgrammingStep.Connect && (tempSteps[1] == ProgrammingStep.ConnectCaesar || tempSteps[1] == ProgrammingStep.ConnectMvci))
		{
			tempSteps.RemoveAt(0);
		}
	}

	private void UpdateStep(string text, bool moveToNext, double? progress = null)
	{
		programDevicePage.UpdateStepText(data.Channel.Ecu, text, CurrentStep);
		if (progress.HasValue)
		{
			programDevicePage.UpdateEcuProgress(data.Channel.Ecu, (double)stepIndex / (double)steps.Length * 100.0 + progress.Value / 100.0);
		}
		if (moveToNext)
		{
			ExecuteNextStep();
		}
	}

	private void Complete(bool success, string message)
	{
		Complete(success, message, canRetry: true);
	}

	private void Complete(bool success, string message, bool canRetry)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Invalid comparison between Unknown and I4
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		UnsubscribeEvents(data.Channel);
		data.Channel.Ecu.Properties["Programming"] = "false";
		SapiManager.GlobalInstance.HoldLogFilesOpen = false;
		waitingForTransitionToOnline = false;
		string text = Resources.ProgramDeviceManager_Status_OK;
		if (!success)
		{
			text = string.Format(CultureInfo.CurrentCulture, "{0}:{1}", CurrentStep.ToString(), message);
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Program Device operation '{0}' for {1} {2} failed because '{3}'", data.Operation.ToString(), data.EngineSerialNumber, data.VehicleIdentificationNumber, text), (StatusMessageType)1, (object)this));
		}
		else
		{
			ProgrammingOperation operation = data.Operation;
			if (operation == ProgrammingOperation.Update || operation == ProgrammingOperation.UpdateAndChangeDataSet)
			{
				File.Delete(Path.Combine(Directories.DrumrollDownloadData, FileEncryptionProvider.EncryptFileName(data.Settings.FileName)));
			}
			File.Delete(data.AttemptInfoPath);
			if (data.AutomaticOperation != null)
			{
				ServerDataManager.GlobalInstance.MarkAutomaticOperationComplete(data.AutomaticOperation);
			}
			if (data.Operation == ProgrammingOperation.ChangeDataSet || data.Operation == ProgrammingOperation.UpdateAndChangeDataSet)
			{
				data.Unit.GetInformationForDevice(data.Channel.Ecu.Name).UpdateCurrentDataSet(data.DataSet);
			}
		}
		eventInfos.Add("totalreprogrammingtime", (DateTime.Now - startTime).ToString());
		if (success && (int)data.DataSource == 2 && data.EdexFileInformation != null && data.EdexFileInformation.ConfigurationInformation.ChecSettings != null)
		{
			eventInfos.Add("checsettingsfilename", data.EdexFileInformation.ConfigurationInformation.ChecSettings.FileName);
			eventInfos.Add("checsettingstimestamp", data.EdexFileInformation.ConfigurationInformation.ChecSettings.Timestamp.ToString());
		}
		eventInfos.Add("packageProgrammingOperation", data.PackageProgrammingOperation.ToString());
		string text2 = null;
		text2 = ((data.Operation != ProgrammingOperation.Replace) ? data.Operation.ToString() : ((data.Settings != null) ? (data.Operation.ToString() + data.Settings.SettingsType) : ((data.EdexFileInformation == null) ? (data.Operation.ToString() + "defaults") : (data.Operation.ToString() + data.EdexFileInformation.FileType))));
		ServerDataManager.UpdateEventsFile(data.Channel, (IDictionary<string, string>)eventInfos, text2, data.EngineSerialNumber, data.VehicleIdentificationNumber, text, data.ChargeType, data.ChargeText, false);
		SapiExtensions.LabelLogWithPrefix(SapiManager.GlobalInstance.Sapi.LogFiles, GetType().Name, text2 + ": " + text);
		ProgrammingStep currentStep = CurrentStep;
		stepIndex = -1;
		ServerDataManager.GlobalInstance.Programming = false;
		List<string> parameterErrorsForEcu = (from error in parameterErrors
			let parameter = data.Channel.Parameters[error.Item1]
			where parameter == null || (error.Item2 != null && !error.Item2.Equals(parameter.Value))
			select string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CouldNotSetParameterError, error.Item1, error.Item2, error.Item3.Message)).ToList();
		programDevicePage.Complete(data, success, currentStep, message, parameterErrorsForEcu, partNumberExceptions, canRetry);
		if (!success && data.Channel.Online)
		{
			ConnectionResource connectionResource = data.Channel.ConnectionResource;
			if (connectionResource != null)
			{
				data.Channel.Disconnect();
				SapiManager.GlobalInstance.Sapi.Channels.Connect(connectionResource, synchronous: false);
			}
		}
	}

	private void ExecuteNextStep()
	{
		//IL_071b: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Invalid comparison between Unknown and I4
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Invalid comparison between Unknown and I4
		string text = null;
		stepIndex++;
		programDevicePage.UpdateEcuProgress(data.Channel.Ecu, (double)stepIndex / (double)steps.Length * 100.0);
		if (data.Channel.Online || CurrentStep == ProgrammingStep.Connect || CurrentStep == ProgrammingStep.ConnectMvci || CurrentStep == ProgrammingStep.ConnectCaesar)
		{
			try
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", stepIndex, CurrentStep);
				StatusLog.Add(new StatusMessage(text2, (StatusMessageType)2, (object)this));
				SapiExtensions.LabelLogWithPrefix(SapiManager.GlobalInstance.Sapi.LogFiles, GetType().Name, text2);
				switch (CurrentStep)
				{
				case ProgrammingStep.Connect:
					ConnectDevice(reconnect: false, null);
					break;
				case ProgrammingStep.ConnectCaesar:
					ConnectDevice(reconnect: false, DiagnosisSource.CaesarDatabase);
					break;
				case ProgrammingStep.ConnectMvci:
					ConnectDevice(reconnect: false, DiagnosisSource.McdDatabase);
					break;
				case ProgrammingStep.WriteEngineSerialNumber:
					if (!string.IsNullOrEmpty(data.EngineSerialNumber))
					{
						ServiceWrite("WriteEngineSerialNumberService", data.EngineSerialNumber);
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				case ProgrammingStep.WriteVehicleIdentificationNumber:
					if (!string.IsNullOrEmpty(data.VehicleIdentificationNumber))
					{
						ServiceWrite("WriteVINService", data.VehicleIdentificationNumber);
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				case ProgrammingStep.ReadExistingSettings:
					if (!File.Exists(Path.Combine(Directories.DrumrollDownloadData, FileEncryptionProvider.EncryptFileName(data.Settings.FileName))))
					{
						ReadParameters();
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_UsingSettingsFromPreviouslyFailedUpgrade, moveToNext: true);
					}
					break;
				case ProgrammingStep.ExecutePostProgrammingActions:
					ExecutePostProgrammingActions();
					break;
				case ProgrammingStep.VerifySettings:
					if (data.Channel.Parameters.Count > 0)
					{
						data.Channel.WriteAllParametersToSummaryFiles = true;
						ReadParameters();
					}
					else
					{
						SaveHistory(null);
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				case ProgrammingStep.LoadExistingSettings:
				case ProgrammingStep.LoadServerSettings:
					if ((int)data.DataSource == 1 && data.Settings != null)
					{
						LoadParameters(data.Settings.FileName, (CurrentStep == ProgrammingStep.LoadExistingSettings) ? ParameterFileFormat.VerFile : ParameterFileFormat.ParFile);
					}
					else if ((int)data.DataSource == 2 && data.EdexFileInformation != null && !data.EdexFileInformation.HasErrors)
					{
						LoadParameters(data.EdexFileInformation.ConfigurationInformation);
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NAAllParametersResetToDefault, moveToNext: true);
					}
					break;
				case ProgrammingStep.UnlockVeDocFirmware:
					if (data.Firmware != null && (!data.TargetChannelHasSameFirmwareVersion || data.FlashRequiredSameFirmwareVersion || SapiManager.GetBootModeStatus(data.Channel)))
					{
						if (RequiresVeDocUnlock(data))
						{
							UnlockVeDoc();
						}
						else
						{
							UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
						}
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				case ProgrammingStep.FlashFirmware:
					if (data.Firmware != null && (!data.TargetChannelHasSameFirmwareVersion || data.FlashRequiredSameFirmwareVersion || SapiManager.GetBootModeStatus(data.Channel)))
					{
						if (data.TargetChannelIsValidForFirmware)
						{
							Flash(ProgrammingData.FlashBlock.Firmware);
						}
						else
						{
							Complete(success: false, Resources.ProgramDeviceManager_Complete_FirmwareIncompatibleWithHardware);
						}
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NAUsingExistingFirmware, moveToNext: true);
					}
					break;
				case ProgrammingStep.FlashBootLoader:
					if (data.TargetChannelRequiresBootLoaderFlash)
					{
						if (data.TargetChannelIsValidForFirmware)
						{
							Flash(ProgrammingData.FlashBlock.BootLoader);
						}
						else
						{
							Complete(success: false, Resources.ProgramDeviceManager_Complete_FirmwareIncompatibleWithHardware);
						}
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NAUsingExistingBootFirmware, moveToNext: true);
					}
					break;
				case ProgrammingStep.UnlockVeDocDataSet:
					if (data.HasDataSet && data.TargetChannelNeededDataSetVersions.Any())
					{
						if (RequiresVeDocUnlock(data))
						{
							UnlockVeDoc();
						}
						else
						{
							UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
						}
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				case ProgrammingStep.FlashDataSet:
					if (data.HasDataSet)
					{
						if (data.TargetChannelNeededDataSetVersions.Any())
						{
							Flash(ProgrammingData.FlashBlock.DataSet);
						}
						else
						{
							UpdateStep(Resources.ProgramDeviceManager_Step_NAUsingExistingDataset, moveToNext: true);
						}
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				case ProgrammingStep.FlashControlList:
					if (data.HasControlList)
					{
						if (data.TargetChannelRequiresControlListFlash)
						{
							Flash(ProgrammingData.FlashBlock.ControlList);
						}
						else
						{
							UpdateStep(Resources.ProgramDeviceManager_Step_NAUsingExistingControlListFirmware, moveToNext: true);
						}
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				case ProgrammingStep.LoadDataSetSettings:
					if (data.DataSet != null)
					{
						if (data.DataSet.SettingsFileName != null && data.DataSet.SettingsFileName.Length > 0)
						{
							LoadParameters(data.DataSet.SettingsFileName, ParameterFileFormat.ParFile);
						}
						else
						{
							UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
						}
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				case ProgrammingStep.LoadPresetSettings:
				{
					SettingsInformation presetSettingsForDevice = data.Unit.GetPresetSettingsForDevice(data.Channel.Ecu.Name);
					if (presetSettingsForDevice != null && !string.IsNullOrEmpty(presetSettingsForDevice.FileName))
					{
						LoadParameters(presetSettingsForDevice.FileName, ParameterFileFormat.ParFile);
					}
					else
					{
						UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
					}
					break;
				}
				case ProgrammingStep.UnlockBackdoor:
					UnlockBackdoor(clearPasswords: false);
					break;
				case ProgrammingStep.UnlockBackdoorAndClearPasswords:
					UnlockBackdoor(clearPasswords: true);
					break;
				case ProgrammingStep.ResetToDefault:
					ResetToDefault();
					break;
				case ProgrammingStep.ExecuteVersionSpecificInitialization:
					ExecuteVersionSpecificInitialization();
					break;
				case ProgrammingStep.CommitToNonvolatile:
					CommitToNonvolatile();
					break;
				case ProgrammingStep.WriteSettings:
					WriteParameters();
					break;
				case ProgrammingStep.Reconnect:
					ConnectDevice(reconnect: true, null);
					break;
				case ProgrammingStep.UpdateUsageCount:
					UpdateDataUsageCount();
					break;
				}
			}
			catch (InvalidChecksumException ex)
			{
				InvalidChecksumException ex2 = ex;
				text = ((Exception)(object)ex2).Message;
			}
			catch (DataException ex3)
			{
				text = ex3.Message;
			}
			catch (InvalidOperationException ex4)
			{
				text = ex4.Message;
			}
			catch (IOException ex5)
			{
				text = ex5.Message;
			}
			catch (CaesarException ex6)
			{
				text = ex6.Message;
			}
			catch (ArgumentException ex7)
			{
				text = ex7.Message;
			}
			if (text != null)
			{
				Complete(success: false, text);
			}
		}
		else
		{
			Complete(success: false, Resources.ProgramDeviceManager_Complete_ChannelUnexpectedlyDisconnected);
		}
	}

	private string GetServiceName(string serviceReference)
	{
		string result = string.Empty;
		if (data.Channel.Ecu.Properties.ContainsKey(serviceReference))
		{
			result = data.Channel.Ecu.Properties[serviceReference];
		}
		return result;
	}

	private void UnlockBackdoor(bool clearPasswords)
	{
		bool flag = false;
		if (PasswordManager.HasPasswords(data.Channel))
		{
			PasswordManager val = PasswordManager.Create(data.Channel);
			if (val.Valid)
			{
				flag = true;
				data.Channel.Extension.Invoke("Unlock", null);
				if (clearPasswords)
				{
					for (int i = 0; i < val.ProtectionListCount; i++)
					{
						val.ClearPasswordForList(i);
					}
				}
			}
		}
		if (!flag)
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
		}
	}

	public static bool RequiresVeDocUnlock(ProgrammingData data)
	{
		if (data.Channel.Ecu.Properties.ContainsKey("HasVeDocProtection") && Convert.ToBoolean(data.Channel.Ecu.Properties["HasVeDocProtection"], CultureInfo.InvariantCulture))
		{
			return Convert.ToBoolean(data.Channel.Extension.Invoke("GetRequiresVeDocUnlock", new string[1] { data.EngineSerialNumber }), CultureInfo.InvariantCulture);
		}
		return false;
	}

	private void UnlockVeDoc()
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		if (NetworkSettings.GlobalInstance.UseManualUnlockDialog)
		{
			ActionsMenuProxy.GlobalInstance.ShowDialog("Unlock ECU for Reprogramming", (string)null, (object)null, true);
		}
		else
		{
			string text = data.Channel.Ecu.Properties["ServerUnlockForProgrammingAction"];
			if (!string.IsNullOrEmpty(text))
			{
				text += string.Format(CultureInfo.InvariantCulture, "({0},{1})", data.VehicleIdentificationNumber, data.EngineSerialNumber);
				SharedProcedureBase val = SharedProcedureBase.AvailableProcedures[text];
				if (val != null)
				{
					if (val.CanStart)
					{
						val.StartComplete += unlockSharedProcedure_StartComplete;
						val.Start();
						return;
					}
					StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure was found but it could not be started: {0}", val.Name), (StatusMessageType)2, (object)this));
				}
				else
				{
					StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure was not found: {0}", text), (StatusMessageType)2, (object)this));
				}
			}
			else
			{
				StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "{0} reports that it requires server based unlocking but does not define a procedure.", data.Channel.Ecu.Name), (StatusMessageType)2, (object)this));
			}
		}
		UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
	}

	private void unlockSharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StartComplete -= unlockSharedProcedure_StartComplete;
		if (((ResultEventArgs)(object)e).Succeeded && (int)e.Result == 1)
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "{0} unlock via the server was initiated using procedure {1}", data.Channel.Ecu.Name, val.Name), (StatusMessageType)2, (object)this));
			val.StopComplete += unlockSharedProcedure_StopComplete;
		}
		else
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed at start. {1}", val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty), (StatusMessageType)2, (object)this));
			UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
		}
	}

	private void unlockSharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StopComplete -= unlockSharedProcedure_StopComplete;
		if (!((ResultEventArgs)(object)e).Succeeded || (int)e.Result == 0)
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed. {1}", val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty), (StatusMessageType)2, (object)this));
		}
		else
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "{0} was unlocked via the server using procedure {1}", data.Channel.Ecu.Name, val.Name), (StatusMessageType)2, (object)this));
		}
		UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
	}

	private void ResetToDefault()
	{
		int num = 0;
		if (data.Operation == ProgrammingOperation.Replace && !data.ReplaceToSameDevice)
		{
			num = ExecuteService("ResetAllToDefaultNewDeviceService");
		}
		if (num == 0)
		{
			num = ExecuteService("ResetCalibrationsToDefaultService");
		}
		if (num == 0)
		{
			num = ExecuteService("ResetAllToDefaultService");
		}
		if (num != 0)
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Resetting, moveToNext: false);
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
		}
	}

	private void ExecuteVersionSpecificInitialization()
	{
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		bool flag = data.Operation == ProgrammingOperation.Replace && !data.ReplaceToSameDevice;
		int num = 0;
		string text = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		string softwareVersion = SapiManager.GetSoftwareVersion(data.Channel);
		if (!string.IsNullOrEmpty(softwareVersion))
		{
			foreach (DictionaryEntry variantApplicableProperty in SapiExtensions.GetVariantApplicableProperties(data.Channel))
			{
				string text2 = variantApplicableProperty.Key as string;
				if (flag)
				{
					if (text2.StartsWith("ProgramNewDeviceService", StringComparison.OrdinalIgnoreCase))
					{
						string[] array = text2.Split("|".ToCharArray());
						if (array.Length == 2 && softwareVersion.StartsWith(array[1], StringComparison.OrdinalIgnoreCase))
						{
							stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0};", variantApplicableProperty.Value);
						}
					}
				}
				else if (text2.StartsWith("ProgramExistingDeviceService", StringComparison.OrdinalIgnoreCase))
				{
					string[] array2 = text2.Split("|".ToCharArray());
					if (array2.Length == 3 && data.PreviousSoftwareVersion.StartsWith(array2[1], StringComparison.OrdinalIgnoreCase) && softwareVersion.StartsWith(array2[2], StringComparison.OrdinalIgnoreCase))
					{
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0};", variantApplicableProperty.Value);
					}
				}
			}
		}
		if (stringBuilder.Length > 0)
		{
			string text3 = stringBuilder.ToString().TrimEnd(";".ToCharArray());
			text = ((!flag) ? string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatUpgradeFrom0To1Executing, data.PreviousSoftwareVersion, softwareVersion) : string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatReplaceTo0Executing, softwareVersion));
			num = ExecuteServiceList(text3);
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2} successfully queued)", text, text3, num), (StatusMessageType)2, (object)this));
		}
		if (num != 0)
		{
			UpdateStep(text, moveToNext: false);
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
		}
	}

	private void CommitToNonvolatile()
	{
		if (data.Channel.Ecu.Properties.ContainsKey("CommitToPermanentMemoryRequiresIgnitionCycle") && bool.Parse(data.Channel.Ecu.Properties["CommitToPermanentMemoryRequiresIgnitionCycle"]))
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Committing, moveToNext: false);
			CycleIgnition cycleIgnition = new CycleIgnition(data.Channel);
			((Control)(object)programDevicePage.Wizard).BeginInvoke((Delegate)(Action)delegate
			{
				cycleIgnition.ShowDialog();
				if (cycleIgnition.Succeeded)
				{
					SubscribeEvents(cycleIgnition.Channel);
					data.Channel = cycleIgnition.Channel;
					UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
				}
				else
				{
					Complete(success: false, Resources.ProgramDeviceManager_Complete_CycleIgnitionFailed);
				}
			});
		}
		else if (ExecuteService("CommitToPermanentMemoryService") != 0)
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Committing, moveToNext: false);
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
		}
	}

	private void ServiceWrite(string abstractedServiceName, string dataToWrite)
	{
		Service service = data.Channel.Services[GetServiceName(abstractedServiceName)];
		if (service != null)
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Writing, moveToNext: false);
			service.InputValues[0].Value = dataToWrite;
			ExecuteService(service);
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
		}
	}

	private int ExecuteService(string abstractedServiceName)
	{
		string serviceName = GetServiceName(abstractedServiceName);
		if (!string.IsNullOrEmpty(serviceName))
		{
			return ExecuteServiceList(serviceName);
		}
		return 0;
	}

	private int ExecuteServiceList(string serviceList)
	{
		data.Channel.Services.ServiceCompleteEvent += Services_ServiceCompleteEvent;
		int num = data.Channel.Services.Execute(serviceList, synchronous: false);
		if (num == 0)
		{
			data.Channel.Services.ServiceCompleteEvent -= Services_ServiceCompleteEvent;
		}
		return num;
	}

	private void ExecuteService(Service service)
	{
		data.Channel.Services.ServiceCompleteEvent += Services_ServiceCompleteEvent;
		service.Execute(synchronous: false);
	}

	private void SaveHistory(ParameterCollection parameters)
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Invalid comparison between Unknown and I4
		if (CurrentStep == ProgrammingStep.ReadExistingSettings && parameters != null)
		{
			bool flag = !parameters.Channel.Ecu.Properties.ContainsKey("ResetCalibrationsToDefaultService") && parameters.Channel.Ecu.Properties.ContainsKey("ResetAllToDefaultService");
			ServerDataManager.SaveSettings(parameters, Directories.DrumrollDownloadData, data.Settings.FileName, ParameterFileFormat.VerFile, flag);
			ServerDataManager.GlobalInstance.AutoSaveSettings(data.Channel, (AutoSaveDestination)1, "Pre" + data.Operation);
		}
		else
		{
			if (CurrentStep != ProgrammingStep.VerifySettings)
			{
				return;
			}
			ServerDataManager.GlobalInstance.AutoSaveSettings(data.Channel, (AutoSaveDestination)1, "Post" + data.Operation);
			if ((int)data.DataSource == 2 && NetworkSettings.GlobalInstance.SaveUploadContent)
			{
				if (data.Channel.Parameters.Count > 0)
				{
					ServerDataManager.GlobalInstance.AutoSaveSettings(data.Channel, (AutoSaveDestination)0, data.Operation.ToString());
				}
				else
				{
					ServerDataManager.GlobalInstance.GenerateEdexSettingsForUpload(data.Channel, data.EdexFileInformation.ConfigurationInformation, data.Operation.ToString());
				}
			}
		}
	}

	private void ReadParameters()
	{
		if (data.Channel.Parameters.Count > 0)
		{
			data.Channel.Parameters.Read(synchronous: false);
			UpdateStep(Resources.ProgramDeviceManager_Step_Reading, moveToNext: false);
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
		}
	}

	private void WriteParameters()
	{
		foreach (Parameter item in data.Channel.Parameters.Where((Parameter p) => p.Marked))
		{
			parameterTargetValues[string.Join(".", item.GroupQualifier, item.Qualifier)] = item.Value;
		}
		data.Channel.Parameters.Write(synchronous: false);
	}

	private void LoadParameters(string fileName, ParameterFileFormat format)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		UpdateStep(Resources.ProgramDeviceManager_Step_LoadingParameters, moveToNext: false);
		StringDictionary stringDictionary = new StringDictionary();
		ServerDataManager.LoadSettings(Directories.DrumrollDownloadData, fileName, (EncryptionType)1, format, data.Channel.Parameters, stringDictionary);
		foreach (DictionaryEntry item in stringDictionary)
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Could not set unknown parameter '{0}' to value '{1}'", item.Key, item.Value), (StatusMessageType)2, (object)this));
		}
		UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
	}

	private void LoadParameters(EdexConfigurationInformation edexConfiguration)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		UpdateStep(Resources.ProgramDeviceManager_Step_LoadingParameters, moveToNext: false);
		partNumberExceptions = new Dictionary<string, CaesarException>();
		IEnumerable<string> enumerable = edexConfiguration.LoadSettingsToChannel(data.Channel, true, (IDictionary<string, CaesarException>)partNumberExceptions);
		foreach (string item in enumerable)
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Could not set unknown parameter '{0}'", item), (StatusMessageType)2, (object)this));
		}
		if (partNumberExceptions.Any())
		{
			throw partNumberExceptions.First().Value;
		}
		UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
	}

	private void Flash(ProgrammingData.FlashBlock which)
	{
		IEnumerable<FlashMeaning> flashMeanings = data.GetFlashMeanings(which);
		if (flashMeanings != null)
		{
			meaningsToFlash = new Queue<FlashMeaning>(flashMeanings);
			totalMeaningsToFlashCount = 0;
			indexOfMeaningCurrentlyBeingFlashed = 0;
			if (meaningsToFlash != null && meaningsToFlash.Any())
			{
				totalMeaningsToFlashCount = meaningsToFlash.Count;
				FlashNextMeaning();
			}
			else
			{
				Complete(success: false, Resources.ProgramDeviceManager_Complete_CouldNotLocateFlashKey);
			}
		}
		else
		{
			Complete(success: false, string.Format(CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Complete_FlashBlock_not_found_for_connected_hardware, which));
		}
	}

	private void FlashNextMeaning()
	{
		FlashMeaning flashMeaning = meaningsToFlash.Dequeue();
		indexOfMeaningCurrentlyBeingFlashed++;
		foreach (FlashArea flashArea in data.Channel.FlashAreas)
		{
			flashArea.Marked = ((flashMeaning.FlashArea == flashArea) ? flashMeaning : null);
		}
		UpdateStep(string.Format(CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_Starting_Count_Format, indexOfMeaningCurrentlyBeingFlashed, totalMeaningsToFlashCount), moveToNext: false);
		data.Channel.Services.SetListVariable("ESN", data.EngineSerialNumber ?? string.Empty);
		data.Channel.Services.SetListVariable("VIN", data.VehicleIdentificationNumber ?? string.Empty);
		data.Channel.FlashAreas.Flash(synchronous: false);
	}

	private void UpdateDataUsageCount()
	{
		DeviceInformation informationForDevice = data.Unit.GetInformationForDevice(data.Channel.Ecu.Name);
		if (informationForDevice != null)
		{
			int usageCount = informationForDevice.UsageCount;
			informationForDevice.UsageCount = usageCount + 1;
			data.Unit.CheckAndUpdateExpiredStatus();
			ServerDataManager.GlobalInstance.SaveUnitXml();
		}
		Complete(success: true, Resources.ProgramDeviceManager_Step_Complete);
	}

	private void ConnectDevice(bool reconnect, DiagnosisSource? targetDiagnosisSource)
	{
		if (data.Channel.Online)
		{
			if (!reconnect && (!targetDiagnosisSource.HasValue || targetDiagnosisSource.Value == data.Channel.Ecu.DiagnosisSource))
			{
				UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
				return;
			}
			data.Channel.Disconnect();
		}
		ConnectionResource connectionResource = data.Channel.ConnectionResource;
		if (targetDiagnosisSource.HasValue && targetDiagnosisSource != connectionResource.Ecu.DiagnosisSource)
		{
			connectionResource = data.GetTargetConnectionResource(targetDiagnosisSource.Value);
			if (connectionResource == null)
			{
				Complete(success: false, Resources.ProgramDeviceManager_Complete_ConnectionResourceForTargetPlatformNotFound);
				return;
			}
			if (connectionResource.Type[0] != data.Channel.ConnectionResource.Type[0])
			{
				ControlHelpers.ShowMessageBox(string.Format(CultureInfo.CurrentCulture, (connectionResource.Type[0] == 'C') ? Resources.ProgramDeviceManager_MessagFormatConnectToCAN : Resources.ProgramDeviceManager_MessagFormatConnectToEthernet, data.Channel.Ecu.Name), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
		if (connectionResource != null)
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Connecting, moveToNext: false);
			SapiManager.GlobalInstance.Sapi.Channels.Connect(connectionResource, synchronous: false);
		}
		else
		{
			Complete(success: false, Resources.ProgramDeviceManager_Complete_ConnectionResourceIsNotValid);
		}
	}

	private void channels_ConnectProgressEvent(object sender, ProgressEventArgs e)
	{
		if (CurrentStep == ProgrammingStep.Reconnect)
		{
			UpdateStep(string.Format(CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_FormatPercentComplete, e.PercentComplete), moveToNext: false, e.PercentComplete);
		}
	}

	private void channels_ConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		if (CurrentStep == ProgrammingStep.CommitToNonvolatile)
		{
			return;
		}
		if (!e.Succeeded)
		{
			if (CurrentStep != ProgrammingStep.None && (!(sender is ConnectionResource connectionResource) || !(connectionResource.Ecu.Name != data.Channel.Ecu.Name)))
			{
				Complete(success: false, e.Exception.Message);
			}
			return;
		}
		Channel channel = sender as Channel;
		if (CurrentStep == ProgrammingStep.None || channel.Ecu.Name != data.Channel.Ecu.Name)
		{
			return;
		}
		ProgrammingStep currentStep = CurrentStep;
		if ((uint)(currentStep - 1) <= 2u || (uint)(currentStep - 11) <= 3u || currentStep == ProgrammingStep.Reconnect)
		{
			SubscribeEvents(channel);
			data.Channel = channel;
			if (channel.CommunicationsState == CommunicationsState.OnlineButNotInitialized || channel.CommunicationsState == CommunicationsState.ReadEcuInfo)
			{
				waitingForTransitionToOnline = true;
				UpdateStep(Resources.ProgramDeviceManager_Step_WaitingForOnlineStatus, moveToNext: false);
			}
			else
			{
				CheckBootModeAndMoveOn();
			}
		}
		else
		{
			Complete(success: false, Resources.ProgramDeviceManager_Complete_UnexpectedSequenceInConnectComplete);
		}
	}

	private void GlobalInstance_ChannelInitializingEvent(object sender, ChannelInitializingEventArgs e)
	{
		if (CurrentStep == ProgrammingStep.None || e.Channel.Ecu.Name != data.Channel.Ecu.Name)
		{
			return;
		}
		ProgrammingStep currentStep = CurrentStep;
		if ((uint)(currentStep - 1) <= 2u || (uint)(currentStep - 11) <= 3u)
		{
			e.AutoRead = false;
			foreach (EcuInfo ecuInfo in e.Channel.EcuInfos)
			{
				if (!ecuInfo.Qualifier.StartsWith("CO", StringComparison.Ordinal))
				{
					ecuInfo.Marked = false;
				}
			}
		}
		e.Channel.Parameters.AutoReadSummaryParameters = false;
	}

	private void Parameters_ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		Parameter parameter = sender as Parameter;
		if (CurrentStep == ProgrammingStep.ReadExistingSettings || CurrentStep == ProgrammingStep.VerifySettings || CurrentStep == ProgrammingStep.WriteSettings)
		{
			UpdateStep(string.Format(CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_FormatPercentComplete, parameter.Channel.Parameters.Progress), moveToNext: false, parameter.Channel.Parameters.Progress);
		}
		if (!e.Succeeded)
		{
			StatusLog.Add(new StatusMessage(e.Exception.Message, (StatusMessageType)1, (object)parameter));
		}
	}

	private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		ParameterCollection parameterCollection = sender as ParameterCollection;
		if (parameterCollection.Channel == data.Channel && (CurrentStep == ProgrammingStep.ReadExistingSettings || CurrentStep == ProgrammingStep.VerifySettings))
		{
			if (e.Succeeded)
			{
				SaveHistory(parameterCollection);
				UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
			}
			else
			{
				Complete(success: false, e.Exception.Message);
			}
		}
	}

	private void FlashAreas_FlashProgressUpdateEvent(object sender, ProgressEventArgs e)
	{
		if (totalMeaningsToFlashCount > 1)
		{
			UpdateStep(string.Format(CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_FormatPercentCompleteMultiple, e.PercentComplete, indexOfMeaningCurrentlyBeingFlashed, totalMeaningsToFlashCount), moveToNext: false, e.PercentComplete);
		}
		else
		{
			UpdateStep(string.Format(CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_Step_FormatPercentComplete, e.PercentComplete), moveToNext: false, e.PercentComplete);
		}
	}

	private void FlashAreas_FlashCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded && meaningsToFlash != null)
		{
			if (!meaningsToFlash.Any())
			{
				meaningsToFlash = null;
				totalMeaningsToFlashCount = 0;
				UpdateStep(Resources.ProgramDeviceManager_Step_Reconnecting, moveToNext: false);
				if (sender is FlashAreaCollection flashAreaCollection)
				{
					ConnectionResource connectionResource = flashAreaCollection.Channel.ConnectionResource;
					if (connectionResource != null)
					{
						flashAreaCollection.Channel.Disconnect();
						UnsubscribeEvents(flashAreaCollection.Channel);
						SapiManager.GlobalInstance.Sapi.Channels.Connect(connectionResource, synchronous: false);
					}
					else
					{
						Complete(success: false, Resources.ProgramDeviceManager_Complete_ConnectionResourceNotAvailable);
					}
				}
				else
				{
					Complete(success: false, Resources.ProgramDeviceManager_Complete_FlashAreaCollectionNotAvailable);
				}
			}
			else
			{
				FlashNextMeaning();
			}
		}
		else
		{
			if (meaningsToFlash != null)
			{
				meaningsToFlash.Clear();
				meaningsToFlash = null;
				totalMeaningsToFlashCount = 0;
			}
			Complete(success: false, e.Exception.Message);
		}
	}

	private void AppendParameterError(string qualifier, object intendedValue, Exception exception)
	{
		parameterErrors.Add(Tuple.Create(qualifier, intendedValue, exception));
	}

	private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			foreach (Parameter item in sender as ParameterCollection)
			{
				if (item.Marked && item.Exception != null)
				{
					string text = string.Join(".", item.GroupQualifier, item.Qualifier);
					object intendedValue = (parameterTargetValues.ContainsKey(text) ? parameterTargetValues[text] : null);
					AppendParameterError(text, intendedValue, item.Exception);
				}
			}
			UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
		}
		else
		{
			Complete(success: false, e.Exception.Message);
		}
	}

	private void Services_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		data.Channel.Services.ServiceCompleteEvent -= Services_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
			return;
		}
		Service service = sender as Service;
		if (service != null && service.ServiceTypes == ServiceTypes.Download && service.InputValues.Count > 0)
		{
			if (CurrentStep != ProgrammingStep.WriteVehicleIdentificationNumber || SapiManager.GetVehicleIdentificationNumber(data.Channel) != data.VehicleIdentificationNumber)
			{
				AppendParameterError(service.Qualifier, service.InputValues[0].Value, e.Exception);
			}
			UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
		}
		else
		{
			Complete(success: false, e.Exception.Message);
		}
	}

	private void channel_InitCompleteEvent(object sender, EventArgs e)
	{
		if (sender == data.Channel && waitingForTransitionToOnline)
		{
			waitingForTransitionToOnline = false;
			CheckBootModeAndMoveOn();
		}
	}

	private void CheckBootModeAndMoveOn()
	{
		bool flag = CurrentStep == ProgrammingStep.FlashDataSet || CurrentStep == ProgrammingStep.FlashFirmware || CurrentStep == ProgrammingStep.FlashControlList;
		if (flag && data.DeferBootModeCheck)
		{
			switch (CurrentStep)
			{
			case ProgrammingStep.FlashFirmware:
				flag = !data.HasDataSet && !data.HasControlList;
				break;
			case ProgrammingStep.FlashDataSet:
				flag = !data.HasControlList && (meaningsToFlash == null || !meaningsToFlash.Any());
				break;
			}
		}
		if (flag)
		{
			if (SapiManager.GetBootModeStatus(data.Channel))
			{
				Complete(success: false, Resources.ProgramDeviceManager_Complete_DataImplausibleReconnectedInBootMode);
			}
			else if (data.Channel.DiagnosisVariant.IsBase)
			{
				Complete(success: false, Resources.ProgramDeviceManager_Complete_EcuSoftwareVersionTooNew, canRetry: false);
			}
			else
			{
				UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
			}
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
		}
	}

	private void ExecutePostProgrammingActions()
	{
		remainingPostProgrammingActions.Clear();
		if (data.Channel.Ecu.Properties.ContainsKey("PostProgrammingActions"))
		{
			string[] collection = data.Channel.Ecu.Properties["PostProgrammingActions"].Split(";".ToCharArray());
			remainingPostProgrammingActions.AddRange(collection);
			ExecuteNextPostProgrammingAction();
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_NotApplicable, moveToNext: true);
		}
	}

	private void ExecuteNextPostProgrammingAction()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		if (remainingPostProgrammingActions.Count > 0)
		{
			string text = remainingPostProgrammingActions[0];
			SharedProcedureBase val = SharedProcedureBase.AvailableProcedures[text];
			remainingPostProgrammingActions.RemoveAt(0);
			if (val != null)
			{
				if (val.CanStart)
				{
					val.StartComplete += sharedProcedure_StartComplete;
					val.Start();
				}
				else
				{
					StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure was found but it could not be started: {0}", val.Name), (StatusMessageType)2, (object)this));
					ExecuteNextPostProgrammingAction();
				}
			}
			else
			{
				StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure was not found: {0}", text), (StatusMessageType)2, (object)this));
				ExecuteNextPostProgrammingAction();
			}
		}
		else
		{
			UpdateStep(Resources.ProgramDeviceManager_Step_Complete, moveToNext: true);
		}
	}

	private void sharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StartComplete -= sharedProcedure_StartComplete;
		if (((ResultEventArgs)(object)e).Succeeded && (int)e.Result == 1)
		{
			val.StopComplete += sharedProcedure_StopComplete;
			UpdateStep(string.Format(CultureInfo.CurrentCulture, Resources.ProgramDeviceManager_FormatProcessing, val.Name), moveToNext: false);
		}
		else
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed at start. {1}", val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty), (StatusMessageType)2, (object)this));
			ExecuteNextPostProgrammingAction();
		}
	}

	private void sharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		SharedProcedureBase val = (SharedProcedureBase)((sender is SharedProcedureBase) ? sender : null);
		val.StopComplete -= sharedProcedure_StopComplete;
		if (!((ResultEventArgs)(object)e).Succeeded || (int)e.Result == 0)
		{
			StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "Referenced shared procedure {0} failed. {1}", val.Name, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty), (StatusMessageType)2, (object)this));
		}
		ExecuteNextPostProgrammingAction();
	}

	private bool IsAvailable(ProgrammingStep step, IEnumerable<ProgrammingStep> requiredFlashStepsForData)
	{
		bool result = true;
		switch (step)
		{
		case ProgrammingStep.WriteVehicleIdentificationNumber:
			if (string.IsNullOrEmpty(data.VehicleIdentificationNumber))
			{
				result = false;
			}
			else if (string.IsNullOrEmpty(GetServiceName("WriteVINService")))
			{
				result = false;
			}
			break;
		case ProgrammingStep.WriteEngineSerialNumber:
			if (string.IsNullOrEmpty(data.EngineSerialNumber))
			{
				result = false;
			}
			else if (string.IsNullOrEmpty(GetServiceName("WriteEngineSerialNumberService")))
			{
				result = false;
			}
			break;
		case ProgrammingStep.ExecutePostProgrammingActions:
			if (!data.Channel.Ecu.Properties.ContainsKey("PostProgrammingActions"))
			{
				result = false;
			}
			break;
		case ProgrammingStep.UnlockVeDocFirmware:
			if (!data.Channel.Ecu.Properties.ContainsKey("HasVeDocProtection") || !requiredFlashStepsForData.Contains(ProgrammingStep.FlashFirmware))
			{
				result = false;
			}
			break;
		case ProgrammingStep.UnlockVeDocDataSet:
			if (!data.Channel.Ecu.Properties.ContainsKey("HasVeDocProtection") || !requiredFlashStepsForData.Contains(ProgrammingStep.FlashDataSet))
			{
				result = false;
			}
			break;
		case ProgrammingStep.FlashBootLoader:
		case ProgrammingStep.FlashFirmware:
		case ProgrammingStep.FlashDataSet:
		case ProgrammingStep.FlashControlList:
			if (!requiredFlashStepsForData.Contains(step))
			{
				result = false;
			}
			break;
		case ProgrammingStep.ExecuteVersionSpecificInitialization:
			result = data.Channel.Ecu.Properties.OfType<DictionaryEntry>().Any((DictionaryEntry p) => ((string)p.Key).StartsWith("ProgramNewDeviceService", StringComparison.OrdinalIgnoreCase) || ((string)p.Key).StartsWith("ProgramExistingDeviceService", StringComparison.OrdinalIgnoreCase));
			break;
		case ProgrammingStep.LoadDataSetSettings:
			if (data.DataSet == null || string.IsNullOrEmpty(data.DataSet.SettingsFileName))
			{
				result = false;
			}
			break;
		case ProgrammingStep.LoadPresetSettings:
		{
			SettingsInformation presetSettingsForDevice = data.Unit.GetPresetSettingsForDevice(data.Channel.Ecu.Name);
			if (presetSettingsForDevice == null || string.IsNullOrEmpty(presetSettingsForDevice.FileName))
			{
				result = false;
			}
			break;
		}
		case ProgrammingStep.UnlockBackdoor:
		case ProgrammingStep.UnlockBackdoorAndClearPasswords:
			if (!PasswordManager.HasPasswords(data.Channel))
			{
				result = false;
			}
			break;
		}
		return result;
	}

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				ChannelCollection channels = SapiManager.GlobalInstance.Sapi.Channels;
				channels.ConnectCompleteEvent -= channels_ConnectCompleteEvent;
				channels.ConnectProgressEvent -= channels_ConnectProgressEvent;
				SapiManager.GlobalInstance.ChannelInitializingEvent -= GlobalInstance_ChannelInitializingEvent;
			}
			data = null;
			programDevicePage = null;
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
