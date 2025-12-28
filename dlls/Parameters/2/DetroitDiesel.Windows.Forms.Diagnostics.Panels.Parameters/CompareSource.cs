using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public sealed class CompareSource
{
	private bool owner;

	private string title;

	private Color color;

	private ParameterCollection parameters;

	private VariantMethod variantMethod;

	private EdexConfigurationInformation edexConfigurationInformation;

	private StringDictionary unknownList;

	private StringDictionary identificationList;

	private static DiagnosisVariant sourceVariant;

	public bool Loaded => parameters != null;

	public string Title => title;

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
		}
	}

	public ParameterCollection Parameters => parameters;

	public VariantMethod Method => variantMethod;

	private string VariantSuffix
	{
		get
		{
			string text = string.Empty;
			switch (variantMethod)
			{
			case VariantMethod.Assumed:
				text += Resources.VariantMethodSuffix_Assumed;
				break;
			case VariantMethod.Preset:
				text += Resources.VariantMethodSuffix_Preset;
				break;
			case VariantMethod.PreviousSource:
				text += Resources.VariantMethodSuffix_PreviousSource;
				break;
			}
			return text;
		}
	}

	public EdexConfigurationInformation EdexConfigurationInformation => edexConfigurationInformation;

	public StringDictionary UnknownList => unknownList;

	public StringDictionary IdentificationList => identificationList;

	public event EventHandler SourceChanged;

	public event EventHandler SourceContentChanged;

	internal CompareSource()
	{
		identificationList = new StringDictionary();
		unknownList = new StringDictionary();
	}

	private void SetParameters(ParameterCollection parameters, VariantMethod variantMethod, bool owner)
	{
		if (this.parameters != null)
		{
			if (this.parameters.Channel.ConnectionResource != null)
			{
				this.parameters.ParametersReadCompleteEvent -= Parameters_ReadWriteCompleteEvent;
				this.parameters.ParametersWriteCompleteEvent -= Parameters_ReadWriteCompleteEvent;
				this.parameters.Channel.InitCompleteEvent -= Channel_InitCompleteEvent;
				this.parameters.ParameterUpdateEvent -= Parameters_ParameterUpdateEvent;
			}
			if (this.owner)
			{
				this.parameters.Channel.Disconnect();
			}
		}
		this.parameters = parameters;
		this.variantMethod = variantMethod;
		this.owner = owner;
		if (this.parameters != null && this.parameters.Channel.ConnectionResource != null)
		{
			this.parameters.ParametersReadCompleteEvent += Parameters_ReadWriteCompleteEvent;
			this.parameters.ParametersWriteCompleteEvent += Parameters_ReadWriteCompleteEvent;
			this.parameters.ParameterUpdateEvent += Parameters_ParameterUpdateEvent;
			this.parameters.Channel.InitCompleteEvent += Channel_InitCompleteEvent;
			if (this.parameters.Channel.DiagnosisVariant != null)
			{
				sourceVariant = this.parameters.Channel.DiagnosisVariant;
			}
		}
	}

	private void Parameters_ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		OnSourceContentChanged();
	}

	private void Channel_InitCompleteEvent(object sender, EventArgs e)
	{
		if (parameters != null && parameters.Channel != null && parameters.Channel.ConnectionResource != null)
		{
			LoadFromChannel(parameters.Channel);
		}
	}

	private void OnSourceChanged()
	{
		if (this.SourceChanged != null)
		{
			this.SourceChanged(this, new EventArgs());
		}
	}

	private void OnSourceContentChanged()
	{
		if (this.SourceContentChanged != null)
		{
			this.SourceContentChanged(this, new EventArgs());
		}
	}

	private void Parameters_ReadWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender is ParameterCollection parameterCollection)
		{
			if (parameterCollection == parameters && identificationList.ContainsKey("time"))
			{
				identificationList["time"] = DateTime.Now.ToString(CultureInfo.CurrentCulture);
			}
			OnSourceContentChanged();
		}
	}

	private static StreamReader Decrypt(string file)
	{
		StreamReader result = null;
		if (!string.IsNullOrEmpty(file))
		{
			byte[] array = null;
			try
			{
				array = FileEncryptionProvider.ReadEncryptedFile(file, true);
			}
			catch (InvalidChecksumException)
			{
			}
			if (array != null && array.Length != 0)
			{
				MemoryStream stream = new MemoryStream(array);
				result = new StreamReader(stream);
			}
			else
			{
				string text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatWarningCorruptFile, Path.GetFileName(file));
				MessageBox.Show(text, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : ((MessageBoxOptions)0));
			}
		}
		return result;
	}

	private static DiagnosisVariant GetPreviousSourceVariant(string device)
	{
		if (sourceVariant == null || !(sourceVariant.Ecu.Name == device))
		{
			return null;
		}
		return sourceVariant;
	}

	private static ParameterCollection LoadEncryptedFile(string file1, string file2, ParameterFileFormat format, StringDictionary unknownList, out VariantMethod variantMethod)
	{
		ParameterCollection result = null;
		variantMethod = VariantMethod.None;
		using (StreamReader streamReader = Decrypt(file1))
		{
			using StreamReader streamReader2 = Decrypt(file2);
			if (streamReader != null)
			{
				TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(streamReader, format);
				Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[targetEcuDetails.Ecu];
				DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails.DiagnosisVariant];
				if (diagnosisVariant != null)
				{
					sourceVariant = diagnosisVariant;
					variantMethod = VariantMethod.Defined;
				}
				else
				{
					DiagnosisVariant previousSourceVariant = GetPreviousSourceVariant(ecu.Name);
					if (previousSourceVariant != null)
					{
						diagnosisVariant = previousSourceVariant;
						variantMethod = VariantMethod.PreviousSource;
					}
					else
					{
						diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails.AssumedDiagnosisVariant];
						variantMethod = VariantMethod.Assumed;
					}
					if (streamReader2 != null)
					{
						TargetEcuDetails targetEcuDetails2 = ParameterCollection.GetTargetEcuDetails(streamReader2, format);
						DiagnosisVariant diagnosisVariant2 = ecu.DiagnosisVariants[targetEcuDetails2.DiagnosisVariant];
						if (diagnosisVariant2 != null)
						{
							diagnosisVariant = diagnosisVariant2;
							variantMethod = VariantMethod.Preset;
							sourceVariant = diagnosisVariant2;
						}
						else if (previousSourceVariant != null)
						{
							diagnosisVariant = previousSourceVariant;
							variantMethod = VariantMethod.PreviousSource;
						}
						else if (targetEcuDetails2.AssumedUnknownCount < targetEcuDetails.AssumedUnknownCount)
						{
							diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails2.AssumedDiagnosisVariant];
							variantMethod = VariantMethod.Assumed;
						}
					}
				}
				if (diagnosisVariant != null)
				{
					Channel channel = SapiManager.GlobalInstance.Sapi.Channels.OpenOffline(diagnosisVariant);
					channel.Parameters.Load(streamReader, format, unknownList, respectAccessLevels: false);
					if (streamReader2 != null)
					{
						channel.Parameters.Load(streamReader2, format, unknownList, respectAccessLevels: false);
					}
					result = channel.Parameters;
				}
			}
		}
		return result;
	}

	private void Reset()
	{
		identificationList.Clear();
		unknownList.Clear();
		SetParameters(null, VariantMethod.None, owner: false);
		title = string.Empty;
		edexConfigurationInformation = null;
	}

	public void Clear()
	{
		Reset();
		OnSourceChanged();
	}

	public bool LoadFromParameterFile(string file)
	{
		Reset();
		identificationList.Add("File", file);
		TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(file, ParameterFileFormat.ParFile);
		Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[targetEcuDetails.Ecu];
		if (ecu != null)
		{
			identificationList.Add(Resources.ColumnHeaderDevice, targetEcuDetails.Ecu);
			DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails.DiagnosisVariant];
			VariantMethod variantMethod;
			if (diagnosisVariant != null)
			{
				variantMethod = VariantMethod.Defined;
			}
			else
			{
				DiagnosisVariant previousSourceVariant = GetPreviousSourceVariant(ecu.Name);
				if (previousSourceVariant != null)
				{
					diagnosisVariant = previousSourceVariant;
					variantMethod = VariantMethod.PreviousSource;
				}
				else
				{
					diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails.AssumedDiagnosisVariant];
					variantMethod = VariantMethod.Assumed;
				}
			}
			Channel channel = SapiManager.GlobalInstance.Sapi.Channels.OpenOffline(diagnosisVariant);
			if (channel != null)
			{
				using (StreamReader inputStream = new StreamReader(file))
				{
					channel.Parameters.Load(inputStream, ParameterFileFormat.ParFile, unknownList, respectAccessLevels: false);
				}
				SetParameters(channel.Parameters, variantMethod, owner: true);
				identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, parameters.Channel.DiagnosisVariant.Name + VariantSuffix);
			}
		}
		else
		{
			unknownList.Add(Resources.ColumnHeaderDevice, targetEcuDetails.Ecu);
		}
		title = file;
		OnSourceChanged();
		return parameters != null;
	}

	public bool LoadFromServerData(UnitInformation unit, SettingsInformation settings, SettingsInformation presetSettings)
	{
		return LoadFromServerData(unit, unitIsUpload: false, settings, presetSettings);
	}

	public bool LoadFromServerData(UnitInformation unit, bool unitIsUpload, SettingsInformation settings, SettingsInformation presetSettings)
	{
		Reset();
		if (unit != null && settings != null)
		{
			string file = FileEncryptionProvider.EncryptFileName(Path.Combine(unitIsUpload ? Directories.DrumrollUploadData : Directories.DrumrollDownloadData, settings.FileName));
			string file2 = string.Empty;
			identificationList.Add(Resources.ColumnHeaderEquipmentSerialNumber, unit.EngineNumber);
			identificationList.Add(Resources.ColumnHeaderVIN, unit.VehicleIdentity);
			if (presetSettings != null)
			{
				file2 = FileEncryptionProvider.EncryptFileName(Path.Combine(Directories.DrumrollDownloadData, presetSettings.FileName));
				identificationList.Add(Resources.ColumnHeaderSettings, string.Format(CultureInfo.CurrentCulture, Resources.SuffixFormat_HasPresets, settings.SettingsType));
			}
			else
			{
				identificationList.Add(Resources.ColumnHeaderSettings, unitIsUpload ? Resources.OpenServerDataForm_SourceUpload : settings.SettingsType);
			}
			if (settings.Timestamp.HasValue)
			{
				identificationList.Add(Resources.ColumnHeaderTime, settings.Timestamp.ToString());
			}
			SetParameters(LoadEncryptedFile(file, file2, unitIsUpload ? ParameterFileFormat.VerFile : ParameterFileFormat.ParFile, unknownList, out var variantMethod), variantMethod, owner: true);
			if (parameters != null)
			{
				identificationList.Add(Resources.ColumnHeaderDevice, parameters.Channel.Ecu.Name);
				identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, parameters.Channel.DiagnosisVariant.Name + VariantSuffix);
			}
			title = string.Format(CultureInfo.CurrentCulture, "{0} {1}", unit.EngineNumber, settings.SettingsType);
			if (settings.Timestamp.HasValue)
			{
				title = title + " " + settings.Timestamp.ToString();
			}
		}
		OnSourceChanged();
		return parameters != null;
	}

	public bool LoadFromEncryptedParameterFile(string file, StringDictionary information)
	{
		Reset();
		if (!string.IsNullOrEmpty(file))
		{
			SetParameters(LoadEncryptedFile(file, string.Empty, ParameterFileFormat.VerFile, unknownList, out var variantMethod), variantMethod, owner: true);
			if (information != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (DictionaryEntry item in information)
				{
					identificationList.Add(item.Key.ToString(), item.Value.ToString());
					stringBuilder.Append(item.Value.ToString());
					stringBuilder.Append(" ");
				}
				title = stringBuilder.ToString();
			}
			else if (parameters != null)
			{
				identificationList.Add(Resources.ColumnHeaderDevice, parameters.Channel.Ecu.Name);
			}
			if (parameters != null)
			{
				identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, parameters.Channel.DiagnosisVariant.Name + VariantSuffix);
			}
		}
		OnSourceChanged();
		return parameters != null;
	}

	public void LoadFromChannel(Channel channel)
	{
		Reset();
		if (channel != null)
		{
			SetParameters(channel.Parameters, VariantMethod.Defined, owner: false);
			identificationList.Add(Resources.ColumnHeaderEquipmentSerialNumber, SapiManager.GetEngineSerialNumber(parameters.Channel));
			identificationList.Add(Resources.ColumnHeaderVIN, SapiManager.GetVehicleIdentificationNumber(parameters.Channel));
			identificationList.Add(Resources.ColumnHeaderDevice, parameters.Channel.Ecu.Name);
			identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, parameters.Channel.DiagnosisVariant.Name);
			identificationList.Add(Resources.ColumnHeaderTime, DateTime.Now.ToString(CultureInfo.CurrentCulture));
			if (SapiExtensions.IsDataSourceEdex(channel.Ecu))
			{
				string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
				if (!string.IsNullOrEmpty(hardwarePartNumber))
				{
					identificationList.Add(Resources.Identification_HardwarePartNumber, PartExtensions.ToHardwarePartNumberString(new Part(hardwarePartNumber), channel.Ecu, true));
				}
				string softwarePartNumber = SapiManager.GetSoftwarePartNumber(channel);
				if (!string.IsNullOrEmpty(softwarePartNumber))
				{
					identificationList.Add(Resources.Identification_SoftwarePartNumber, PartExtensions.ToFlashKeyStyleString(new Part(softwarePartNumber)));
				}
				string ecuModel = SapiManager.GetEcuModel(channel);
				if (!string.IsNullOrEmpty(ecuModel))
				{
					identificationList.Add(Resources.Identification_EcuModel, ecuModel);
				}
			}
			if (channel.Online)
			{
				title = string.Format(CultureInfo.CurrentCulture, Resources.EcuStatusFormat_Online, parameters.Channel.Ecu.Name);
			}
			else
			{
				title = string.Format(CultureInfo.CurrentCulture, Resources.EcuStatusFormat_Offline, parameters.Channel.Ecu.Name);
			}
		}
		OnSourceChanged();
	}

	public bool LoadFromEdexServerData(UnitInformation unit, EdexFileInformation settings, bool includeProposedSettings)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Reset();
		if (unit != null && settings != null)
		{
			EdexConfigurationInformation configurationInformation = settings.ConfigurationInformation;
			if (configurationInformation != null && !settings.HasErrors)
			{
				Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[configurationInformation.DeviceName];
				if (ecu != null)
				{
					string text = (includeProposedSettings ? settings.CompleteFileType : ((object)settings.FileType/*cast due to .constrained prefix*/).ToString());
					SetParameters(LoadEdexData(ecu, configurationInformation, includeProposedSettings, unknownList, out var variantMethod), variantMethod, owner: true);
					identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, parameters.Channel.DiagnosisVariant.ToString() + VariantSuffix);
					identificationList.Add(Resources.ColumnHeaderVIN, configurationInformation.VehicleIdentificationNumber);
					identificationList.Add(Resources.ColumnHeaderDevice, ecu.Name);
					identificationList.Add(Resources.ColumnHeaderSettings, text);
					if (configurationInformation.DiagnosticLinkSettingsTimestamp.HasValue)
					{
						identificationList.Add(Resources.ColumnHeaderTime, configurationInformation.DiagnosticLinkSettingsTimestamp.ToString());
					}
					if (configurationInformation.HardwarePartNumber != null)
					{
						identificationList.Add(Resources.Identification_HardwarePartNumber, PartExtensions.ToHardwarePartNumberString(configurationInformation.HardwarePartNumber, ecu, true));
					}
					else
					{
						identificationList.Add(Resources.Identification_HardwareRevision, configurationInformation.HardwareRevision);
					}
					identificationList.Add(Resources.Identification_SoftwarePartNumber, PartExtensions.ToFlashKeyStyleString(configurationInformation.FlashwarePartNumber));
					string ecuModel = configurationInformation.EcuModel;
					if (ecuModel != null)
					{
						identificationList.Add(Resources.Identification_EcuModel, ecuModel);
					}
					title = string.Format(CultureInfo.CurrentCulture, "{0} {1}", unit.IdentityKey, text);
					if (configurationInformation.DiagnosticLinkSettingsTimestamp.HasValue)
					{
						title = title + " " + configurationInformation.DiagnosticLinkSettingsTimestamp.ToString();
					}
					edexConfigurationInformation = configurationInformation;
				}
			}
		}
		OnSourceChanged();
		return parameters != null;
	}

	private static ParameterCollection LoadEdexData(Ecu ecu, EdexConfigurationInformation configurationInformation, bool includeProposedSettings, StringDictionary unknownList, out VariantMethod variantMethod)
	{
		DiagnosisVariant targetDiagnosticVariant = configurationInformation.GetTargetDiagnosticVariant(ecu);
		IEnumerable<DiagnosisVariant> enumerable;
		if (targetDiagnosticVariant == null)
		{
			IEnumerable<DiagnosisVariant> diagnosisVariants = ecu.DiagnosisVariants;
			enumerable = diagnosisVariants;
		}
		else
		{
			enumerable = Enumerable.Repeat(targetDiagnosticVariant, 1).Concat(ecu.DiagnosisVariants);
		}
		IEnumerable<DiagnosisVariant> enumerable2 = enumerable;
		List<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>> list = new List<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>>();
		foreach (DiagnosisVariant item in enumerable2)
		{
			Dictionary<string, CaesarException> dictionary = new Dictionary<string, CaesarException>();
			Channel channel = SapiManager.GlobalInstance.Sapi.Channels.OpenOffline(item);
			IEnumerable<string> source = configurationInformation.LoadSettingsToChannel(channel, includeProposedSettings, (IDictionary<string, CaesarException>)dictionary);
			list.Add(new Tuple<Channel, Dictionary<string, CaesarException>, List<string>>(channel, dictionary, source.ToList()));
			if (!dictionary.Any() && !source.Any())
			{
				break;
			}
		}
		Tuple<Channel, Dictionary<string, CaesarException>, List<string>> tuple = (from r in list
			orderby r.Item2.Count, r.Item3.Count()
			select r).First();
		tuple.Item3.ForEach(delegate(string s)
		{
			unknownList.Add(s, s);
		});
		foreach (KeyValuePair<string, CaesarException> item2 in tuple.Item2)
		{
			unknownList.Add(item2.Key, item2.Value.Message);
		}
		variantMethod = ((targetDiagnosticVariant != null) ? VariantMethod.Defined : VariantMethod.Assumed);
		foreach (Channel item3 in list.Select((Tuple<Channel, Dictionary<string, CaesarException>, List<string>> r) => r.Item1).Except(Enumerable.Repeat(tuple.Item1, 1)))
		{
			item3.Disconnect();
		}
		return tuple.Item1.Parameters;
	}
}
