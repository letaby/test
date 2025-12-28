// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.CompareSource
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
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

#nullable disable
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

  internal CompareSource()
  {
    this.identificationList = new StringDictionary();
    this.unknownList = new StringDictionary();
  }

  public bool Loaded => this.parameters != null;

  public string Title => this.title;

  public Color Color
  {
    get => this.color;
    set => this.color = value;
  }

  public ParameterCollection Parameters => this.parameters;

  public VariantMethod Method => this.variantMethod;

  private string VariantSuffix
  {
    get
    {
      string empty = string.Empty;
      switch (this.variantMethod)
      {
        case VariantMethod.Assumed:
          empty += Resources.VariantMethodSuffix_Assumed;
          break;
        case VariantMethod.PreviousSource:
          empty += Resources.VariantMethodSuffix_PreviousSource;
          break;
        case VariantMethod.Preset:
          empty += Resources.VariantMethodSuffix_Preset;
          break;
      }
      return empty;
    }
  }

  public EdexConfigurationInformation EdexConfigurationInformation
  {
    get => this.edexConfigurationInformation;
  }

  private void SetParameters(
    ParameterCollection parameters,
    VariantMethod variantMethod,
    bool owner)
  {
    if (this.parameters != null)
    {
      if (this.parameters.Channel.ConnectionResource != null)
      {
        this.parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ReadWriteCompleteEvent);
        this.parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ReadWriteCompleteEvent);
        this.parameters.Channel.InitCompleteEvent -= new InitCompleteEventHandler(this.Channel_InitCompleteEvent);
        this.parameters.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.Parameters_ParameterUpdateEvent);
      }
      if (this.owner)
        this.parameters.Channel.Disconnect();
    }
    this.parameters = parameters;
    this.variantMethod = variantMethod;
    this.owner = owner;
    if (this.parameters == null || this.parameters.Channel.ConnectionResource == null)
      return;
    this.parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ReadWriteCompleteEvent);
    this.parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ReadWriteCompleteEvent);
    this.parameters.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.Parameters_ParameterUpdateEvent);
    this.parameters.Channel.InitCompleteEvent += new InitCompleteEventHandler(this.Channel_InitCompleteEvent);
    if (this.parameters.Channel.DiagnosisVariant == null)
      return;
    CompareSource.sourceVariant = this.parameters.Channel.DiagnosisVariant;
  }

  private void Parameters_ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    this.OnSourceContentChanged();
  }

  private void Channel_InitCompleteEvent(object sender, EventArgs e)
  {
    if (this.parameters == null || this.parameters.Channel == null || this.parameters.Channel.ConnectionResource == null)
      return;
    this.LoadFromChannel(this.parameters.Channel);
  }

  public event EventHandler SourceChanged;

  private void OnSourceChanged()
  {
    if (this.SourceChanged == null)
      return;
    this.SourceChanged((object) this, new EventArgs());
  }

  public event EventHandler SourceContentChanged;

  private void OnSourceContentChanged()
  {
    if (this.SourceContentChanged == null)
      return;
    this.SourceContentChanged((object) this, new EventArgs());
  }

  public StringDictionary UnknownList => this.unknownList;

  public StringDictionary IdentificationList => this.identificationList;

  private void Parameters_ReadWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender is ParameterCollection parameterCollection))
      return;
    if (parameterCollection == this.parameters && this.identificationList.ContainsKey("time"))
      this.identificationList["time"] = DateTime.Now.ToString((IFormatProvider) CultureInfo.CurrentCulture);
    this.OnSourceContentChanged();
  }

  private static StreamReader Decrypt(string file)
  {
    StreamReader streamReader = (StreamReader) null;
    if (!string.IsNullOrEmpty(file))
    {
      byte[] buffer = (byte[]) null;
      try
      {
        buffer = FileEncryptionProvider.ReadEncryptedFile(file, true);
      }
      catch (InvalidChecksumException ex)
      {
      }
      if (buffer != null && buffer.Length != 0)
      {
        streamReader = new StreamReader((Stream) new MemoryStream(buffer));
      }
      else
      {
        int num = (int) MessageBox.Show(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatWarningCorruptFile, (object) Path.GetFileName(file)), ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : (MessageBoxOptions) 0);
      }
    }
    return streamReader;
  }

  private static DiagnosisVariant GetPreviousSourceVariant(string device)
  {
    return CompareSource.sourceVariant == null || !(CompareSource.sourceVariant.Ecu.Name == device) ? (DiagnosisVariant) null : CompareSource.sourceVariant;
  }

  private static ParameterCollection LoadEncryptedFile(
    string file1,
    string file2,
    ParameterFileFormat format,
    StringDictionary unknownList,
    out VariantMethod variantMethod)
  {
    ParameterCollection parameterCollection = (ParameterCollection) null;
    variantMethod = VariantMethod.None;
    using (StreamReader streamReader1 = CompareSource.Decrypt(file1))
    {
      using (StreamReader streamReader2 = CompareSource.Decrypt(file2))
      {
        if (streamReader1 != null)
        {
          TargetEcuDetails targetEcuDetails1 = ParameterCollection.GetTargetEcuDetails(streamReader1, format);
          Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[targetEcuDetails1.Ecu];
          DiagnosisVariant diagnosisVariant1 = ecu.DiagnosisVariants[targetEcuDetails1.DiagnosisVariant];
          if (diagnosisVariant1 != null)
          {
            CompareSource.sourceVariant = diagnosisVariant1;
            variantMethod = VariantMethod.Defined;
          }
          else
          {
            DiagnosisVariant previousSourceVariant = CompareSource.GetPreviousSourceVariant(ecu.Name);
            if (previousSourceVariant != null)
            {
              diagnosisVariant1 = previousSourceVariant;
              variantMethod = VariantMethod.PreviousSource;
            }
            else
            {
              diagnosisVariant1 = ecu.DiagnosisVariants[targetEcuDetails1.AssumedDiagnosisVariant];
              variantMethod = VariantMethod.Assumed;
            }
            if (streamReader2 != null)
            {
              TargetEcuDetails targetEcuDetails2 = ParameterCollection.GetTargetEcuDetails(streamReader2, format);
              DiagnosisVariant diagnosisVariant2 = ecu.DiagnosisVariants[targetEcuDetails2.DiagnosisVariant];
              if (diagnosisVariant2 != null)
              {
                diagnosisVariant1 = diagnosisVariant2;
                variantMethod = VariantMethod.Preset;
                CompareSource.sourceVariant = diagnosisVariant2;
              }
              else if (previousSourceVariant != null)
              {
                diagnosisVariant1 = previousSourceVariant;
                variantMethod = VariantMethod.PreviousSource;
              }
              else if (targetEcuDetails2.AssumedUnknownCount < targetEcuDetails1.AssumedUnknownCount)
              {
                diagnosisVariant1 = ecu.DiagnosisVariants[targetEcuDetails2.AssumedDiagnosisVariant];
                variantMethod = VariantMethod.Assumed;
              }
            }
          }
          if (diagnosisVariant1 != null)
          {
            Channel channel = SapiManager.GlobalInstance.Sapi.Channels.OpenOffline(diagnosisVariant1);
            channel.Parameters.Load(streamReader1, format, unknownList, false);
            if (streamReader2 != null)
              channel.Parameters.Load(streamReader2, format, unknownList, false);
            parameterCollection = channel.Parameters;
          }
        }
      }
    }
    return parameterCollection;
  }

  private void Reset()
  {
    this.identificationList.Clear();
    this.unknownList.Clear();
    this.SetParameters((ParameterCollection) null, VariantMethod.None, false);
    this.title = string.Empty;
    this.edexConfigurationInformation = (EdexConfigurationInformation) null;
  }

  public void Clear()
  {
    this.Reset();
    this.OnSourceChanged();
  }

  public bool LoadFromParameterFile(string file)
  {
    this.Reset();
    this.identificationList.Add("File", file);
    TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(file, ParameterFileFormat.ParFile);
    Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[targetEcuDetails.Ecu];
    if (ecu != null)
    {
      this.identificationList.Add(Resources.ColumnHeaderDevice, targetEcuDetails.Ecu);
      DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails.DiagnosisVariant];
      VariantMethod variantMethod;
      if (diagnosisVariant != null)
      {
        variantMethod = VariantMethod.Defined;
      }
      else
      {
        DiagnosisVariant previousSourceVariant = CompareSource.GetPreviousSourceVariant(ecu.Name);
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
          channel.Parameters.Load(inputStream, ParameterFileFormat.ParFile, this.unknownList, false);
        this.SetParameters(channel.Parameters, variantMethod, true);
        this.identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, this.parameters.Channel.DiagnosisVariant.Name + this.VariantSuffix);
      }
    }
    else
      this.unknownList.Add(Resources.ColumnHeaderDevice, targetEcuDetails.Ecu);
    this.title = file;
    this.OnSourceChanged();
    return this.parameters != null;
  }

  public bool LoadFromServerData(
    UnitInformation unit,
    SettingsInformation settings,
    SettingsInformation presetSettings)
  {
    return this.LoadFromServerData(unit, false, settings, presetSettings);
  }

  public bool LoadFromServerData(
    UnitInformation unit,
    bool unitIsUpload,
    SettingsInformation settings,
    SettingsInformation presetSettings)
  {
    this.Reset();
    if (unit != null && settings != null)
    {
      string file1 = FileEncryptionProvider.EncryptFileName(Path.Combine(unitIsUpload ? Directories.DrumrollUploadData : Directories.DrumrollDownloadData, settings.FileName));
      string file2 = string.Empty;
      this.identificationList.Add(Resources.ColumnHeaderEquipmentSerialNumber, unit.EngineNumber);
      this.identificationList.Add(Resources.ColumnHeaderVIN, unit.VehicleIdentity);
      if (presetSettings != null)
      {
        file2 = FileEncryptionProvider.EncryptFileName(Path.Combine(Directories.DrumrollDownloadData, presetSettings.FileName));
        this.identificationList.Add(Resources.ColumnHeaderSettings, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SuffixFormat_HasPresets, (object) settings.SettingsType));
      }
      else
        this.identificationList.Add(Resources.ColumnHeaderSettings, unitIsUpload ? Resources.OpenServerDataForm_SourceUpload : settings.SettingsType);
      DateTime? timestamp = settings.Timestamp;
      if (timestamp.HasValue)
      {
        StringDictionary identificationList = this.identificationList;
        string columnHeaderTime = Resources.ColumnHeaderTime;
        timestamp = settings.Timestamp;
        string str = timestamp.ToString();
        identificationList.Add(columnHeaderTime, str);
      }
      VariantMethod variantMethod;
      this.SetParameters(CompareSource.LoadEncryptedFile(file1, file2, unitIsUpload ? ParameterFileFormat.VerFile : ParameterFileFormat.ParFile, this.unknownList, out variantMethod), variantMethod, true);
      if (this.parameters != null)
      {
        this.identificationList.Add(Resources.ColumnHeaderDevice, this.parameters.Channel.Ecu.Name);
        this.identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, this.parameters.Channel.DiagnosisVariant.Name + this.VariantSuffix);
      }
      this.title = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) unit.EngineNumber, (object) settings.SettingsType);
      timestamp = settings.Timestamp;
      if (timestamp.HasValue)
      {
        string title = this.title;
        timestamp = settings.Timestamp;
        string str = timestamp.ToString();
        this.title = $"{title} {str}";
      }
    }
    this.OnSourceChanged();
    return this.parameters != null;
  }

  public bool LoadFromEncryptedParameterFile(string file, StringDictionary information)
  {
    this.Reset();
    if (!string.IsNullOrEmpty(file))
    {
      VariantMethod variantMethod;
      this.SetParameters(CompareSource.LoadEncryptedFile(file, string.Empty, ParameterFileFormat.VerFile, this.unknownList, out variantMethod), variantMethod, true);
      if (information != null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (DictionaryEntry dictionaryEntry in information)
        {
          this.identificationList.Add(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
          stringBuilder.Append(dictionaryEntry.Value.ToString());
          stringBuilder.Append(" ");
        }
        this.title = stringBuilder.ToString();
      }
      else if (this.parameters != null)
        this.identificationList.Add(Resources.ColumnHeaderDevice, this.parameters.Channel.Ecu.Name);
      if (this.parameters != null)
        this.identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, this.parameters.Channel.DiagnosisVariant.Name + this.VariantSuffix);
    }
    this.OnSourceChanged();
    return this.parameters != null;
  }

  public void LoadFromChannel(Channel channel)
  {
    this.Reset();
    if (channel != null)
    {
      this.SetParameters(channel.Parameters, VariantMethod.Defined, false);
      this.identificationList.Add(Resources.ColumnHeaderEquipmentSerialNumber, SapiManager.GetEngineSerialNumber(this.parameters.Channel));
      this.identificationList.Add(Resources.ColumnHeaderVIN, SapiManager.GetVehicleIdentificationNumber(this.parameters.Channel));
      this.identificationList.Add(Resources.ColumnHeaderDevice, this.parameters.Channel.Ecu.Name);
      this.identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, this.parameters.Channel.DiagnosisVariant.Name);
      this.identificationList.Add(Resources.ColumnHeaderTime, DateTime.Now.ToString((IFormatProvider) CultureInfo.CurrentCulture));
      if (SapiExtensions.IsDataSourceEdex(channel.Ecu))
      {
        string hardwarePartNumber = SapiManager.GetHardwarePartNumber(channel);
        if (!string.IsNullOrEmpty(hardwarePartNumber))
          this.identificationList.Add(Resources.Identification_HardwarePartNumber, PartExtensions.ToHardwarePartNumberString(new Part(hardwarePartNumber), channel.Ecu, true));
        string softwarePartNumber = SapiManager.GetSoftwarePartNumber(channel);
        if (!string.IsNullOrEmpty(softwarePartNumber))
          this.identificationList.Add(Resources.Identification_SoftwarePartNumber, PartExtensions.ToFlashKeyStyleString(new Part(softwarePartNumber)));
        string ecuModel = SapiManager.GetEcuModel(channel);
        if (!string.IsNullOrEmpty(ecuModel))
          this.identificationList.Add(Resources.Identification_EcuModel, ecuModel);
      }
      this.title = !channel.Online ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EcuStatusFormat_Offline, (object) this.parameters.Channel.Ecu.Name) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EcuStatusFormat_Online, (object) this.parameters.Channel.Ecu.Name);
    }
    this.OnSourceChanged();
  }

  public bool LoadFromEdexServerData(
    UnitInformation unit,
    EdexFileInformation settings,
    bool includeProposedSettings)
  {
    this.Reset();
    if (unit != null && settings != null)
    {
      EdexConfigurationInformation configurationInformation = settings.ConfigurationInformation;
      if (configurationInformation != null && !settings.HasErrors)
      {
        Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[configurationInformation.DeviceName];
        if (ecu != null)
        {
          string str = includeProposedSettings ? settings.CompleteFileType : settings.FileType.ToString();
          VariantMethod variantMethod;
          this.SetParameters(CompareSource.LoadEdexData(ecu, configurationInformation, includeProposedSettings, this.unknownList, out variantMethod), variantMethod, true);
          this.identificationList.Add(Resources.ColumnHeaderDiagnosticVariant, this.parameters.Channel.DiagnosisVariant.ToString() + this.VariantSuffix);
          this.identificationList.Add(Resources.ColumnHeaderVIN, configurationInformation.VehicleIdentificationNumber);
          this.identificationList.Add(Resources.ColumnHeaderDevice, ecu.Name);
          this.identificationList.Add(Resources.ColumnHeaderSettings, str);
          if (configurationInformation.DiagnosticLinkSettingsTimestamp.HasValue)
            this.identificationList.Add(Resources.ColumnHeaderTime, configurationInformation.DiagnosticLinkSettingsTimestamp.ToString());
          if (configurationInformation.HardwarePartNumber != null)
            this.identificationList.Add(Resources.Identification_HardwarePartNumber, PartExtensions.ToHardwarePartNumberString(configurationInformation.HardwarePartNumber, ecu, true));
          else
            this.identificationList.Add(Resources.Identification_HardwareRevision, configurationInformation.HardwareRevision);
          this.identificationList.Add(Resources.Identification_SoftwarePartNumber, PartExtensions.ToFlashKeyStyleString(configurationInformation.FlashwarePartNumber));
          string ecuModel = configurationInformation.EcuModel;
          if (ecuModel != null)
            this.identificationList.Add(Resources.Identification_EcuModel, ecuModel);
          this.title = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) unit.IdentityKey, (object) str);
          if (configurationInformation.DiagnosticLinkSettingsTimestamp.HasValue)
            this.title = $"{this.title} {configurationInformation.DiagnosticLinkSettingsTimestamp.ToString()}";
          this.edexConfigurationInformation = configurationInformation;
        }
      }
    }
    this.OnSourceChanged();
    return this.parameters != null;
  }

  private static ParameterCollection LoadEdexData(
    Ecu ecu,
    EdexConfigurationInformation configurationInformation,
    bool includeProposedSettings,
    StringDictionary unknownList,
    out VariantMethod variantMethod)
  {
    DiagnosisVariant diagnosticVariant = configurationInformation.GetTargetDiagnosticVariant(ecu);
    IEnumerable<DiagnosisVariant> diagnosisVariants = diagnosticVariant != null ? Enumerable.Repeat<DiagnosisVariant>(diagnosticVariant, 1).Concat<DiagnosisVariant>((IEnumerable<DiagnosisVariant>) ecu.DiagnosisVariants) : (IEnumerable<DiagnosisVariant>) ecu.DiagnosisVariants;
    List<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>> source1 = new List<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>>();
    foreach (DiagnosisVariant diagnosisVariant in diagnosisVariants)
    {
      Dictionary<string, CaesarException> source2 = new Dictionary<string, CaesarException>();
      Channel channel1 = SapiManager.GlobalInstance.Sapi.Channels.OpenOffline(diagnosisVariant);
      IEnumerable<string> channel2 = configurationInformation.LoadSettingsToChannel(channel1, includeProposedSettings, (IDictionary<string, CaesarException>) source2);
      source1.Add(new Tuple<Channel, Dictionary<string, CaesarException>, List<string>>(channel1, source2, channel2.ToList<string>()));
      if (!source2.Any<KeyValuePair<string, CaesarException>>())
      {
        if (!channel2.Any<string>())
          break;
      }
    }
    Tuple<Channel, Dictionary<string, CaesarException>, List<string>> tuple = source1.OrderBy<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>, int>((Func<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>, int>) (r => r.Item2.Count)).ThenBy<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>, int>((Func<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>, int>) (r => r.Item3.Count<string>())).First<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>>();
    tuple.Item3.ForEach((Action<string>) (s => unknownList.Add(s, s)));
    foreach (KeyValuePair<string, CaesarException> keyValuePair in tuple.Item2)
      unknownList.Add(keyValuePair.Key, keyValuePair.Value.Message);
    variantMethod = diagnosticVariant != null ? VariantMethod.Defined : VariantMethod.Assumed;
    foreach (Channel channel in source1.Select<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>, Channel>((Func<Tuple<Channel, Dictionary<string, CaesarException>, List<string>>, Channel>) (r => r.Item1)).Except<Channel>(Enumerable.Repeat<Channel>(tuple.Item1, 1)))
      channel.Disconnect();
    return tuple.Item1.Parameters;
  }
}
