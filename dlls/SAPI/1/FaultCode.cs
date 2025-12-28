// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FaultCode
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

#nullable disable
namespace SapiLayer1;

public sealed class FaultCode : IDiogenesDataItem
{
  private const string FaultCodeNumber = "FaultCodeNumber";
  private const string FaultCodeMode = "FaultCodeMode";
  internal static string[] isoAreas = new string[4]
  {
    "P",
    "C",
    "B",
    "U"
  };
  private FaultCodeIncident manipulatedValue;
  private static Regex engineeringNotesSpnFmiRegex = new Regex(".*?SPN = (?<spn>\\d*).*?FMI = (?<fmi>\\d*.).*?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
  private Channel channel;
  private FaultCodeIncidentCollection faultCodeIncidents;
  private FaultCodeIncidentCollection snapshots;
  private string caesarCode;
  private string code;
  private string text;
  private string modeText;
  private Dictionary<string, string> descriptions;
  private string number;
  private string mode;
  private uint longNumber;
  private ServiceCollection environmentDataDescriptions;

  internal FaultCode(Channel ch, string code, string caesarcode = null)
  {
    this.caesarCode = caesarcode;
    this.code = code;
    this.text = string.Empty;
    this.channel = ch;
    this.faultCodeIncidents = new FaultCodeIncidentCollection(this, ReadFunctions.NonPermanent);
    this.snapshots = new FaultCodeIncidentCollection(this, ReadFunctions.Snapshot);
  }

  internal void InternalReset()
  {
    if (this.channel.ChannelHandle != null)
      this.channel.ChannelHandle.ClearSingleError(this.caesarCode != null ? this.caesarCode : this.code);
    if (this.channel.IsChannelErrorSet)
      this.channel.SyncDone((Exception) new CaesarException(this.channel.ChannelHandle));
    else
      this.channel.SyncDone((Exception) null);
  }

  internal void AcquireText()
  {
    string str = this.caesarCode != null ? this.caesarCode : this.code;
    if (this.channel.Ecu.FaultCodeCanBeDuplicate)
      str = this.code.Split(":".ToCharArray())[0];
    if (this.channel.EcuHandle != null)
    {
      this.text = this.channel.EcuHandle.GetErrorText(str);
      if (!this.Channel.FaultCodes.HasFaultDescriptions)
        return;
      string errorDescription = this.channel.EcuHandle.GetErrorDescription(str);
      if (string.IsNullOrWhiteSpace(errorDescription))
        return;
      this.descriptions = ((IEnumerable<string>) errorDescription.Split(new string[1]
      {
        "; \r\n"
      }, StringSplitOptions.RemoveEmptyEntries)).Select(item => new
      {
        item = item,
        separatorposition = item.IndexOf(": ", StringComparison.Ordinal)
      }).Where(_param1 => _param1.separatorposition > -1).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        qualifier = _param1.item.Substring(0, _param1.separatorposition)
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier1 = _param1,
        value = _param1.\u003C\u003Eh__TransparentIdentifier0.item.Substring(_param1.\u003C\u003Eh__TransparentIdentifier0.separatorposition + 2)
      }).Select(_param1 => new KeyValuePair<string, string>(_param1.\u003C\u003Eh__TransparentIdentifier1.qualifier, _param1.value)).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value));
    }
    else
    {
      if (this.channel.McdEcuHandle == null)
        return;
      McdDBDiagTroubleCode dbDiagTroubleCode = this.channel.McdEcuHandle.DBDiagTroubleCodes.FirstOrDefault<McdDBDiagTroubleCode>((Func<McdDBDiagTroubleCode, bool>) (f => f.DisplayTroubleCode == this.code));
      if (dbDiagTroubleCode == null)
        return;
      this.longNumber = (uint) dbDiagTroubleCode.TroubleCode;
      this.text = dbDiagTroubleCode.Text;
      this.environmentDataDescriptions = new ServiceCollection(this);
    }
  }

  internal void AddStringsForTranslation(Dictionary<string, string> table, int? maxAccessLevel)
  {
    table[Sapi.MakeTranslationIdentifier(this.Code, "Text")] = this.text;
    if (this.descriptions == null || maxAccessLevel.HasValue && maxAccessLevel.Value <= 2)
      return;
    foreach (KeyValuePair<string, string> description in this.descriptions)
      table[Sapi.MakeTranslationIdentifier(this.Code, description.Key, "Description")] = description.Value;
  }

  internal static string ConvertIsoCodeToUdsCode(string isoCode)
  {
    if (isoCode.Length == 7)
    {
      try
      {
        byte num = 0;
        bool flag = false;
        for (byte index = 0; (int) index < FaultCode.isoAreas.Length && !flag; ++index)
        {
          if (isoCode.StartsWith(FaultCode.isoAreas[(int) index].ToString(), StringComparison.Ordinal))
          {
            num = (byte) ((uint) index << 6);
            flag = true;
          }
        }
        if (flag)
        {
          byte[] array = new Dump(isoCode.Substring(1)).Data.ToArray<byte>();
          array[0] |= num;
          return new Dump((IEnumerable<byte>) array).ToString();
        }
      }
      catch (FormatException ex)
      {
      }
    }
    return (string) null;
  }

  internal static string ConvertUdsCodeToIsoCode(string nonIsoCode)
  {
    if (nonIsoCode.Length == 6)
    {
      try
      {
        byte[] array = new Dump(nonIsoCode).Data.ToArray<byte>();
        byte index = (byte) ((uint) array[0] >> 6);
        array[0] = (byte) ((uint) array[0] & 63U /*0x3F*/);
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) FaultCode.isoAreas[(int) index], (object) new Dump((IEnumerable<byte>) array));
      }
      catch (FormatException ex)
      {
      }
    }
    return (string) null;
  }

  internal uint LongNumber
  {
    get => this.longNumber;
    set => this.longNumber = value;
  }

  public Channel Channel => this.channel;

  public string Code => this.code;

  public string Text
  {
    get
    {
      string text;
      if (this.channel.Ecu.RollCallManager != null)
      {
        text = this.channel.Ecu.RollCallManager.GetFaultText(this.channel, this.Number, this.Mode);
      }
      else
      {
        text = this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.Code, nameof (Text)), this.text);
        if (this.channel.Ecu.FaultCodeCanBeDuplicate)
        {
          if (this.modeText == null)
          {
            FaultCodeIncident faultCodeIncident = this.faultCodeIncidents.Where<FaultCodeIncident>((Func<FaultCodeIncident, bool>) (fci => fci.EnvironmentDatas.Count > 0)).FirstOrDefault<FaultCodeIncident>();
            if (faultCodeIncident != null)
            {
              EnvironmentData environmentData = faultCodeIncident.EnvironmentDatas.FirstOrDefault<EnvironmentData>((Func<EnvironmentData, bool>) (ed => ed.Value.ToString().StartsWith("FMI", StringComparison.Ordinal)));
              if (environmentData != null)
                this.modeText = environmentData.Value.ToString().Split(":".ToCharArray())[1];
            }
          }
          if (this.modeText != null)
            text = this.text + this.modeText;
        }
      }
      return text;
    }
  }

  public FaultCodeIncidentCollection FaultCodeIncidents => this.faultCodeIncidents;

  public ServiceCollection EnvironmentDataDescriptions => this.environmentDataDescriptions;

  public FaultCodeIncidentCollection Snapshots => this.snapshots;

  public string Number
  {
    get
    {
      if (string.IsNullOrEmpty(this.number))
        this.number = this.GetNumberOrMode(false);
      return this.number;
    }
  }

  public string Mode
  {
    get
    {
      if (string.IsNullOrEmpty(this.mode))
        this.mode = this.GetNumberOrMode(true);
      return this.mode;
    }
  }

  public FaultCodeIncident ManipulatedValue
  {
    get => this.manipulatedValue;
    private set
    {
      this.manipulatedValue = value;
      this.channel.SetManipulatedState(this.Code, this.manipulatedValue != null);
    }
  }

  public void Manipulate(FaultCodeStatus status)
  {
    this.ManipulatedValue = new FaultCodeIncident(this, Sapi.Now, status);
    if (status == FaultCodeStatus.None)
      return;
    this.FaultCodeIncidents.Add(this.ManipulatedValue, false);
  }

  public void ClearManipulation() => this.ManipulatedValue = (FaultCodeIncident) null;

  public void Reset(bool synchronous) => this.channel.QueueAction((object) this, synchronous);

  private static bool TryDecodeSpnFmi(
    Ecu ecu,
    string originalCode,
    bool isFailureMode,
    out string result)
  {
    result = originalCode;
    if (ecu.FaultCodeIsEncodedSpnFmi)
    {
      string str = originalCode.Length == 7 ? FaultCode.ConvertIsoCodeToUdsCode(originalCode) : originalCode;
      if (str.Length == 6)
      {
        try
        {
          IList<byte> data = new Dump(str).Data;
          if (!isFailureMode)
          {
            uint num = (uint) ((int) data[0] + ((int) data[1] << 8) + (((int) data[2] & 224 /*0xE0*/) << 11));
            result = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            return true;
          }
          int num1 = (int) data[2] & 31 /*0x1F*/;
          result = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          return true;
        }
        catch (FormatException ex)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) str, ".EcuInfo defined a reference to J1939SPN/FMI from the UDS code, but the UDS code was not in the correct format.");
        }
      }
    }
    else if (ecu.FaultCodeCanBeDuplicate || ecu.RollCallManager != null)
    {
      string[] strArray = originalCode.Split(":".ToCharArray());
      if (!isFailureMode)
      {
        result = strArray[0];
        return true;
      }
      if (strArray.Length > 1 && strArray[1].StartsWith("FMI", StringComparison.Ordinal))
      {
        result = strArray[1].Substring(3);
        return true;
      }
    }
    return false;
  }

  private string GetNumberOrMode(bool mode)
  {
    string result;
    if (FaultCode.TryDecodeSpnFmi(this.Channel.Ecu, this.code, mode, out result))
      return result;
    if (this.Channel.Ecu.FaultCodeNumberAndModeFromEngineeringNotes)
    {
      string input;
      if (this.descriptions != null && this.descriptions.TryGetValue("ENGINEERING_NOTES", out input))
      {
        Match match = FaultCode.engineeringNotesSpnFmiRegex.Match(input);
        if (match.Success)
          return match.Groups[mode ? "fmi" : "spn"].Value;
      }
    }
    else if (this.channel.Ecu.FaultNumberIsFromEnvironmentData)
    {
      foreach (FaultCodeIncident faultCodeIncident in this.faultCodeIncidents)
      {
        EnvironmentData environmentData = faultCodeIncident.EnvironmentDatas[mode ? "FaultCodeMode" : "FaultCodeNumber"];
        if (environmentData != null)
          return environmentData.Value.ToString();
      }
    }
    else if (!mode)
      return this.code;
    return string.Empty;
  }

  internal static LogMetadataItem ExtractMetadata(
    XmlReader xmlReader,
    string ecuName,
    string variantName)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    string str = xmlReader.GetAttribute(currentFormat[TagName.Code].LocalName);
    string attribute = xmlReader.GetAttribute(currentFormat[TagName.StartTime].LocalName);
    string result1 = (string) null;
    string result2 = (string) null;
    Ecu ecu = Sapi.GetSapi().Ecus[ecuName];
    if (ecu == null && variantName == "ROLLCALL")
      ecu = Ecu.CreateFromRollCallLog(ecuName);
    if (ecu != null && (!FaultCode.TryDecodeSpnFmi(ecu, str, false, out result1) || !FaultCode.TryDecodeSpnFmi(ecu, str, true, out result2)) && ecu.FaultNumberIsFromEnvironmentData)
    {
      xmlReader.ReadToDescendant(currentFormat[TagName.EnvironmentDatas].LocalName);
      if (xmlReader.ReadToDescendant(currentFormat[TagName.EnvironmentData].LocalName))
      {
        do
        {
          if (result1 == null || result2 == null)
          {
            KeyValuePair<string, string> metadata = EnvironmentData.ExtractMetadata(xmlReader);
            switch (metadata.Key)
            {
              case "FaultCodeNumber":
                result1 = metadata.Value;
                break;
              case "FaultCodeMode":
                result2 = metadata.Value;
                break;
            }
          }
          else
            xmlReader.Skip();
        }
        while (xmlReader.NodeType == XmlNodeType.Element);
      }
      xmlReader.Skip();
    }
    xmlReader.Skip();
    if (result1 != null && result2 != null)
      str = $"{result1}/{result2}";
    return new LogMetadataItem(LogMetadataType.FaultCode, ecuName, str, attribute);
  }

  public bool IsRelated(FaultCode faultCode)
  {
    if (faultCode == null)
      return false;
    if (faultCode.Number == this.Number && faultCode.Mode == this.Mode)
      return true;
    return this.channel.IsRollCall ? FaultCode.CompareJ1587InfoToRollCall(faultCode, this) : FaultCode.CompareJ1587InfoToRollCall(this, faultCode);
  }

  private static bool CompareJ1587InfoToRollCall(FaultCode udsFault, FaultCode j1587Fault)
  {
    if (j1587Fault.channel.IsRollCall && j1587Fault.Channel.Ecu.ProtocolName == "J1708" && udsFault.channel.Ecu.FaultNumberIsFromEnvironmentData)
    {
      FaultCodeIncident faultCodeIncident = udsFault.faultCodeIncidents.Where<FaultCodeIncident>((Func<FaultCodeIncident, bool>) (fci => fci.EnvironmentDatas.Count > 0)).FirstOrDefault<FaultCodeIncident>();
      if (faultCodeIncident != null)
      {
        EnvironmentData environmentData = faultCodeIncident.EnvironmentDatas["J1587Info"];
        string number;
        string mode;
        if (environmentData != null && environmentData.Value != null && FaultCode.TryParseJ1587Info(environmentData.Value.ToString(), out number, out mode) && number == j1587Fault.Number)
          return mode == j1587Fault.Mode;
      }
    }
    return false;
  }

  private static bool TryParseJ1587Info(string value, out string number, out string mode)
  {
    number = (string) null;
    mode = (string) null;
    int num1 = value.IndexOf("FMI", StringComparison.Ordinal);
    int num2 = value.IndexOf("ID", StringComparison.Ordinal);
    if (num1 <= 0 || num2 <= 0 || num1 >= value.Length - 3)
      return false;
    mode = value.Substring(num1 + 3).Trim();
    number = value.Substring(0, 1) + value.Substring(num2 + 2, num1 - (num2 + 2)).Trim();
    return true;
  }

  public string Name
  {
    get
    {
      return !string.IsNullOrEmpty(this.Mode) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1}/{2})", (object) this.Text, (object) this.Number, (object) this.Mode) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) this.Text, (object) this.Code);
    }
  }

  public string ShortName => this.Name;

  public string Qualifier => this.Code;

  public string Description => (string) null;

  public IDictionary<string, string> Descriptions
  {
    get
    {
      return this.descriptions != null ? (IDictionary<string, string>) this.descriptions.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.Code, v.Key, "Description"), v.Value))) : (IDictionary<string, string>) null;
    }
  }

  public string GroupName => string.Empty;

  public string GroupQualifier => string.Empty;

  public string Units => (string) null;

  public object Precision => (object) null;

  public ChoiceCollection Choices => this.Channel.FaultCodeStatusChoices;

  public bool Visible => true;

  public Service CombinedService => (Service) null;
}
