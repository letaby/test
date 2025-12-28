// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Ecu
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class Ecu
{
  private static Dictionary<string, Dictionary<string, TranslationEntry>> translations = new Dictionary<string, Dictionary<string, TranslationEntry>>();
  private Dictionary<string, IEnumerable<KeyValuePair<string, string>>> rollCallIdValues;
  private Dictionary<string, bool> useRelatedAddresses;
  private List<byte> rollCallAddressJ1708;
  private List<byte> rollCallAddressJ1939;
  private List<int> rollCallFunctionJ1939;
  private List<int> rollCallAddressDoIP;
  private bool isByteMessaging;
  private readonly DiagnosisSource diagnosisSource;
  private bool hasMultiByteDiagnosticVersion;
  private Dictionary<string, TranslationEntry> translation;
  private static string DefaultNameSplit = ": ";
  private static object resourceLock = new object();
  private bool faultCodeCanBeDuplicate;
  private bool? faultCodeIsEncodedSpnFmi;
  private bool faultNumberIsFromEnvironmentData;
  private bool faultCodeNumberAndModeFromEngineeringNotes;
  private Type extensionType;
  private bool haveAttemptedExtensionTypeLoad;
  private uint? requestId;
  private uint? responseId;
  private uint? logicalEcuAddress;
  private uint? logicalTesterAddress;
  private string baseName;
  private string description;
  private string cBFVersion;
  private string preamble;
  private string gPDVersion;
  private string protocolName;
  private string descriptionDataVersion;
  private string descriptionFileName;
  private DiagnosisVariantCollection variants;
  private EcuInterfaceCollection interfaces;
  private bool markedForAutoConnect;
  private int connectedChannelCount;
  private XmlNode xml;
  private StringDictionary properties;
  private List<CompoundEnvironmentData> compoundEnvironmentDatas;
  private string extensionSource;
  private bool configurationLoadedFromFile;
  private int? configurationFileVersion;
  private string checksum;
  private ListDictionary ecuInfoComParameters;
  private List<Tuple<Ecu, string>> viaEcus;
  private int? restrictedPortIndex;
  private string restrictedInterface;
  private string restrictedPort;
  private string restrictedInterfaceForViaEcu;
  private int? restrictedPortIndexForViaEcu;
  private string identifier;
  private readonly List<Regex> makeStoredQualifiers;
  private readonly List<Regex> makeInstrumentQualifiers;
  private readonly List<Regex> ignoredQualifiers;
  private readonly List<Regex> defaultActionQualifiers;
  private readonly List<Regex> forceRequestQualifiers;
  private readonly List<Regex> summaryQualifiers;
  private readonly Dictionary<string, int> cacheTimeQualifiers;
  private readonly Dictionary<Tuple<string, string>, IEnumerable<KeyValuePair<string, string>>> ecuInfoAttributes;
  private string nameSplit;
  private int? sourceAddress;
  private int? function;
  private RollCall rollCallManager;
  private DiagnosisProtocol diagnosisProtocol;
  private string rollCallName;
  private string rollCallDescription;

  internal static Ecu CreateFromRollCallLog(string name)
  {
    string[] strArray = name.Split("-".ToCharArray());
    byte sourceAddress = byte.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
    DiagnosisProtocol diagnosisProtocol = Sapi.GetSapi().DiagnosisProtocols[strArray[0]];
    if (diagnosisProtocol != null)
      return new Ecu(name, sourceAddress, diagnosisProtocol);
    int protocolId = (int) Enum.Parse(typeof (Protocol), strArray[0]);
    int? function = new int?();
    if (strArray.Length > 2)
      function = new int?(int.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture));
    return RollCall.GetManager((Protocol) protocolId).GetEcu((int) sourceAddress, function);
  }

  internal Ecu(int sourceAddress, int? function, RollCall rollCallManager)
    : this(new int?(sourceAddress), function, (string) null, DiagnosisSource.RollCallDynamic, rollCallManager, (string) null, (string) null, (string) null, (string) null, new byte?())
  {
  }

  internal Ecu(string identifier, byte sourceAddress, DiagnosisProtocol protocol)
    : this(identifier, (string) null, protocol.IsMcd ? DiagnosisSource.McdApi1 : DiagnosisSource.CaesarApi1, (RollCall) null, (string) null, protocol)
  {
    this.isByteMessaging = true;
    this.sourceAddress = new int?((int) sourceAddress);
    this.protocolName = protocol.Name;
    this.identifier = identifier;
    this.rollCallAddressJ1708 = new List<byte>();
    this.rollCallAddressJ1939 = new List<byte>();
    this.rollCallFunctionJ1939 = new List<int>();
    this.rollCallAddressDoIP = new List<int>();
    this.rollCallIdValues = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
    this.variants.Add(new DiagnosisVariant(this, "BYTE", string.Empty, (IEnumerable<Tuple<RollCall.ID, string>>) null, (IEnumerable<string>) new List<string>()));
    this.properties.Add("SupportsFaultRead", "false");
  }

  internal Ecu(
    int? sourceAddress,
    int? function,
    string ecuName,
    DiagnosisSource source,
    RollCall rollCallManager,
    string shortDescription,
    string category,
    string family,
    string supportedEquipment,
    byte? otherProtocolAddress)
    : this(rollCallManager.Protocol.ToString(), (string) null, source, rollCallManager, ecuName)
  {
    this.rollCallDescription = shortDescription;
    this.protocolName = rollCallManager.Protocol.ToString();
    if (rollCallManager.Protocol == Protocol.J1939)
      this.faultCodeIsEncodedSpnFmi = new bool?(true);
    this.sourceAddress = sourceAddress;
    this.function = function;
    if (supportedEquipment != null)
      this.properties.Add("SupportedEquipment", supportedEquipment);
    if (family != null)
      this.properties.Add("Family", family);
    if (category != null)
      this.properties.Add("Category", category);
    this.rollCallFunctionJ1939 = new List<int>();
    this.rollCallIdValues = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
    this.rollCallIdValues.Add("J1708", (IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>());
    this.rollCallIdValues.Add("J1939", (IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>());
    this.rollCallIdValues.Add("DoIP", (IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>());
    List<byte> byteList1;
    if (!otherProtocolAddress.HasValue || rollCallManager.Protocol != Protocol.J1708)
      byteList1 = new List<byte>();
    else
      byteList1 = new List<byte>()
      {
        otherProtocolAddress.Value
      };
    this.rollCallAddressJ1939 = byteList1;
    List<byte> byteList2;
    if (!otherProtocolAddress.HasValue || rollCallManager.Protocol != Protocol.J1939)
      byteList2 = new List<byte>();
    else
      byteList2 = new List<byte>()
      {
        otherProtocolAddress.Value
      };
    this.rollCallAddressJ1708 = byteList2;
    this.rollCallAddressDoIP = new List<int>();
  }

  internal Ecu(string baseName, string description, DiagnosisSource source)
    : this(baseName, description, source, (RollCall) null, (string) null)
  {
  }

  private Ecu(
    string baseName,
    string description,
    DiagnosisSource source,
    RollCall rollCallManager,
    string rollCallName,
    DiagnosisProtocol diagnosisProtocol = null)
  {
    this.diagnosisSource = source;
    this.rollCallName = rollCallName;
    this.diagnosisProtocol = diagnosisProtocol;
    this.rollCallManager = rollCallManager;
    this.baseName = baseName;
    this.description = description;
    this.cBFVersion = string.Empty;
    this.preamble = string.Empty;
    this.gPDVersion = string.Empty;
    this.protocolName = string.Empty;
    this.descriptionFileName = string.Empty;
    this.nameSplit = Ecu.DefaultNameSplit;
    this.descriptionDataVersion = string.Empty;
    int ver = 0;
    DateTime dt = DateTime.MinValue;
    this.variants = new DiagnosisVariantCollection(this);
    this.interfaces = new EcuInterfaceCollection(this);
    this.ecuInfoComParameters = new ListDictionary();
    this.properties = new StringDictionary();
    this.compoundEnvironmentDatas = new List<CompoundEnvironmentData>();
    this.makeStoredQualifiers = new List<Regex>();
    this.makeInstrumentQualifiers = new List<Regex>();
    this.defaultActionQualifiers = new List<Regex>();
    this.forceRequestQualifiers = new List<Regex>();
    this.ignoredQualifiers = new List<Regex>();
    this.summaryQualifiers = new List<Regex>();
    this.cacheTimeQualifiers = new Dictionary<string, int>();
    this.AffectServices = new Dictionary<string, string>();
    this.PreService = new Dictionary<string, string>();
    this.AlternateQualifiers = new Dictionary<string, string>();
    this.viaEcus = new List<Tuple<Ecu, string>>();
    Sapi sapi = Sapi.GetSapi();
    if (this.rollCallManager == null && this.diagnosisProtocol == null || this.rollCallName != null)
    {
      Stream inStream = (Stream) null;
      string path = Path.Combine(this.IsMcd ? McdRoot.DatabaseLocation : sapi.ConfigurationItems["CBFFiles"].Value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.EcuInfo", (object) (this.rollCallName ?? this.baseName)));
      if (sapi.GetConfiguration != null)
      {
        inStream = sapi.GetConfiguration(this, path);
        if (this.rollCallManager == null || inStream != null)
        {
          XmlDocument doc = new XmlDocument();
          doc.Load(inStream);
          this.xml = Sapi.ReadSapiXmlFile(doc, nameof (Ecu), out ver, out dt);
          this.configurationLoadedFromFile = inStream is FileStream;
        }
      }
      else if (this.rollCallManager == null || File.Exists(path))
      {
        this.xml = Sapi.ReadSapiXmlFile(path, nameof (Ecu), out ver, out dt);
        this.configurationLoadedFromFile = true;
      }
      if (this.xml != null && ver > 1)
      {
        XmlNodeList xmlNodeList1 = this.xml.SelectNodes("Ecu/Properties/Property");
        if (xmlNodeList1 != null)
        {
          for (int i = 0; i < xmlNodeList1.Count; ++i)
          {
            XmlNode xmlNode = xmlNodeList1[i];
            this.properties.Add(xmlNode.Attributes.GetNamedItem(nameof (Name)).InnerText, xmlNode.InnerText);
          }
        }
        if (ver > 8)
        {
          XmlNode xmlNode = this.xml.SelectSingleNode("Ecu/Extension");
          if (xmlNode != null)
            this.extensionSource = Sapi.Decrypt(new Dump(xmlNode.Attributes.GetNamedItem("Source").InnerText));
        }
        if (this.rollCallManager == null)
        {
          XmlNodeList xmlNodeList2 = this.xml.SelectNodes("Ecu/EnvironmentDatas/EnvironmentData");
          if (xmlNodeList2 != null)
          {
            for (int i = 0; i < xmlNodeList2.Count; ++i)
              this.compoundEnvironmentDatas.Add(new CompoundEnvironmentData(xmlNodeList2[i]));
          }
          if (ver > 5)
          {
            XmlNode node1 = this.xml.SelectSingleNode("Ecu/Services");
            if (node1 != null)
            {
              IEnumerable<XElement> source1 = node1.ToXElement().Elements((XName) "Service");
              foreach (XElement xelement in source1.Where<XElement>((Func<XElement, bool>) (xe => xe.Attribute((XName) "Qualifier") != null && xe.Attribute((XName) "Affects") != null)))
                this.AffectServices.Add(xelement.Attribute((XName) "Qualifier").Value, xelement.Attribute((XName) "Affects").Value);
              if (ver > 15)
              {
                foreach (XElement xelement in source1.Where<XElement>((Func<XElement, bool>) (xe => xe.Attribute((XName) "Qualifier") != null && xe.Attribute((XName) nameof (PreService)) != null)))
                  this.PreService.Add(xelement.Attribute((XName) "Qualifier").Value, xelement.Attribute((XName) nameof (PreService)).Value);
              }
            }
            if (ver > 9)
            {
              XmlNode node2 = this.xml.SelectSingleNode("Ecu/DiagServices");
              if (node2 != null)
              {
                XElement xelement = node2.ToXElement();
                IEnumerable<XElement> source2 = xelement.Elements((XName) "DiagService");
                this.ecuInfoAttributes = source2.Select(element => new
                {
                  element = element,
                  qualifier = element.Attribute((XName) "Qualifier").Value
                }).Select(_param1 => new
                {
                  \u003C\u003Eh__TransparentIdentifier0 = _param1,
                  variantAttr = _param1.element.Attribute((XName) "Variants")
                }).SelectMany(_param1 => _param1.\u003C\u003Eh__TransparentIdentifier0.element.Attributes(), (_param1, propertyAttr) => new
                {
                  \u003C\u003Eh__TransparentIdentifier1 = _param1,
                  propertyAttr = propertyAttr
                }).Where(_param1 => _param1.propertyAttr.Name.LocalName != "Qualifier" && _param1.propertyAttr.Name.LocalName != "Variants").SelectMany(_param1 => _param1.\u003C\u003Eh__TransparentIdentifier1.variantAttr == null ? (IEnumerable<string>) new string[1] : (IEnumerable<string>) _param1.\u003C\u003Eh__TransparentIdentifier1.variantAttr.Value.Split(";".ToCharArray()), (_param1, variant) => new
                {
                  \u003C\u003Eh__TransparentIdentifier2 = _param1,
                  variant = variant
                }).OrderByDescending(_param1 => _param1.\u003C\u003Eh__TransparentIdentifier2.\u003C\u003Eh__TransparentIdentifier1.\u003C\u003Eh__TransparentIdentifier0.qualifier.Length).GroupBy(_param1 => Tuple.Create<string, string>(_param1.\u003C\u003Eh__TransparentIdentifier2.propertyAttr.Name.LocalName, _param1.variant), _param1 => new KeyValuePair<string, string>(_param1.\u003C\u003Eh__TransparentIdentifier2.\u003C\u003Eh__TransparentIdentifier1.\u003C\u003Eh__TransparentIdentifier0.qualifier, _param1.\u003C\u003Eh__TransparentIdentifier2.propertyAttr.Value)).ToDictionary<IGrouping<Tuple<string, string>, KeyValuePair<string, string>>, Tuple<string, string>, IEnumerable<KeyValuePair<string, string>>>((Func<IGrouping<Tuple<string, string>, KeyValuePair<string, string>>, Tuple<string, string>>) (group => group.Key), (Func<IGrouping<Tuple<string, string>, KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>>) (group => group.ToList<KeyValuePair<string, string>>().AsEnumerable<KeyValuePair<string, string>>()));
                foreach (IGrouping<string, XElement> source3 in source2.Where<XElement>((Func<XElement, bool>) (xe => xe.Attribute((XName) "Action") != null)).GroupBy<XElement, string>((Func<XElement, string>) (xe => xe.Attribute((XName) "Action").Value)))
                {
                  List<Regex> list = source3.Select<XElement, Regex>((Func<XElement, Regex>) (xe => new Regex(xe.Attribute((XName) "Qualifier").Value, RegexOptions.Compiled))).ToList<Regex>();
                  switch (source3.Key)
                  {
                    case "ignore":
                      this.ignoredQualifiers = list;
                      continue;
                    case "makestored":
                      this.makeStoredQualifiers = list;
                      continue;
                    case "makeinstrument":
                      this.makeInstrumentQualifiers = list;
                      continue;
                    case "default":
                      this.defaultActionQualifiers = list;
                      continue;
                    case "forcerequest":
                      this.forceRequestQualifiers = list;
                      continue;
                    default:
                      continue;
                  }
                }
                if (ver > 10)
                {
                  if (xelement.Attribute((XName) nameof (NameSplit)) != null)
                    this.nameSplit = xelement.Attribute((XName) nameof (NameSplit)).Value;
                  this.cacheTimeQualifiers = source2.Where<XElement>((Func<XElement, bool>) (xe => xe.Attribute((XName) "CacheTime") != null)).ToDictionary<XElement, string, int>((Func<XElement, string>) (k => k.Attribute((XName) "Qualifier").Value), (Func<XElement, int>) (v => Convert.ToInt32(v.Attribute((XName) "CacheTime").Value, (IFormatProvider) CultureInfo.InvariantCulture)));
                  if (ver > 11)
                  {
                    this.summaryQualifiers = source2.Where<XElement>((Func<XElement, bool>) (xe => xe.Attribute((XName) "Summary") != null && xe.Attribute((XName) "Summary").Value == "true")).Select<XElement, Regex>((Func<XElement, Regex>) (xe => new Regex(xe.Attribute((XName) "Qualifier").Value, RegexOptions.Compiled))).ToList<Regex>();
                    if (ver > 12)
                      this.AlternateQualifiers = source2.Where<XElement>((Func<XElement, bool>) (xe => xe.Attribute((XName) "AlternateQualifier") != null)).ToDictionary<XElement, string, string>((Func<XElement, string>) (k => k.Attribute((XName) "AlternateQualifier").Value), (Func<XElement, string>) (v => v.Attribute((XName) "Qualifier").Value));
                  }
                }
              }
            }
          }
        }
        if (ver > 13)
        {
          XmlNode xmlNode = this.xml.SelectSingleNode(nameof (Ecu));
          if (xmlNode != null)
          {
            this.configurationFileVersion = new int?(Convert.ToInt32(xmlNode.Attributes.GetNamedItem("ConfigurationVersion").InnerText, (IFormatProvider) CultureInfo.InvariantCulture));
            this.checksum = xmlNode.Attributes.GetNamedItem("Checksum").InnerText;
            XmlNode namedItem = xmlNode.Attributes.GetNamedItem("TargetApplication");
            if (namedItem != null)
            {
              string strB = Assembly.GetEntryAssembly().GetName().Name.ToString();
              if (string.Compare(namedItem.InnerText, strB, StringComparison.OrdinalIgnoreCase) != 0)
                Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WARNING: {0}.EcuInfo Target Application {1} does not match actual application {2}", (object) this.Name, (object) namedItem.InnerText, (object) strB));
            }
          }
        }
        XmlNodeList xmlNodeList3 = this.xml.SelectNodes("Ecu/ComParameters/ComParameter");
        if (xmlNodeList3 != null)
        {
          for (int i = 0; i < xmlNodeList3.Count; ++i)
          {
            XmlNode xmlNode = xmlNodeList3[i];
            if (xmlNode != null)
              this.ecuInfoComParameters.Add((object) xmlNode.Attributes.GetNamedItem(nameof (Name)).InnerText, (object) xmlNode.InnerText);
          }
        }
      }
      if (this.rollCallManager == null)
      {
        if (this.properties.ContainsKey("RestrictedPortIndex"))
        {
          int result = 0;
          if (int.TryParse(this.properties["RestrictedPortIndex"], out result))
            this.restrictedPortIndex = new int?(result);
        }
        if (this.properties.ContainsKey("RestrictedInterface"))
          this.restrictedInterface = this.properties["RestrictedInterface"];
        if (this.properties.ContainsKey("RestrictedPort"))
          this.restrictedPort = this.properties["RestrictedPort"];
        if (this.properties.ContainsKey("RestrictedInterfaceForViaEcu"))
          this.restrictedInterfaceForViaEcu = this.properties["RestrictedInterfaceForViaEcu"];
        int result1;
        if (this.properties.ContainsKey("RestrictedPortIndexForViaEcu") && int.TryParse(this.properties["RestrictedPortIndexForViaEcu"], out result1))
          this.restrictedPortIndexForViaEcu = new int?(result1);
        if (this.properties.ContainsKey(nameof (OfflineSupportOnly)))
          this.OfflineSupportOnly = Convert.ToBoolean(this.properties[nameof (OfflineSupportOnly)], (IFormatProvider) CultureInfo.InvariantCulture);
        if (this.properties.ContainsKey(nameof (SignalNotAvailableValue)))
          this.SignalNotAvailableValue = new Dump(this.properties[nameof (SignalNotAvailableValue)]);
        if (this.properties.ContainsKey(nameof (ProhibitAutoConnection)))
          this.ProhibitAutoConnection = Convert.ToBoolean(this.properties[nameof (ProhibitAutoConnection)], (IFormatProvider) CultureInfo.InvariantCulture);
        if (this.DiagnosisSource == DiagnosisSource.McdDatabase)
        {
          this.CaesarEquivalentResponseParameterQualifierSource = !this.properties.ContainsKey(nameof (CaesarEquivalentResponseParameterQualifierSource)) ? Ecu.ResponseParameterQualifierSource.Name : (Ecu.ResponseParameterQualifierSource) Enum.Parse(typeof (Ecu.ResponseParameterQualifierSource), this.properties[nameof (CaesarEquivalentResponseParameterQualifierSource)]);
          this.SupportsDoublePrecisionVariantCoding = true;
        }
        else if (this.properties.ContainsKey(nameof (SupportsDoublePrecisionVariantCoding)))
          this.SupportsDoublePrecisionVariantCoding = Convert.ToBoolean(this.properties[nameof (SupportsDoublePrecisionVariantCoding)], (IFormatProvider) CultureInfo.InvariantCulture);
        this.rollCallAddressJ1708 = this.ParseAddressesFromProperty("J1708SourceAddress").Select<int, byte>((Func<int, byte>) (a => (byte) a)).ToList<byte>();
        this.rollCallAddressJ1939 = this.ParseAddressesFromProperty("J1939SourceAddress").Select<int, byte>((Func<int, byte>) (a => (byte) a)).ToList<byte>();
        this.rollCallFunctionJ1939 = this.ParseAddressesFromProperty("J1939Function").ToList<int>();
        this.useRelatedAddresses = new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (string key in this.properties.Keys.OfType<string>().Where<string>((Func<string, bool>) (k => k.StartsWith("UseRelatedAddresses_", StringComparison.OrdinalIgnoreCase))))
          this.useRelatedAddresses.Add(key.Substring("UseRelatedAddresses_".Length), bool.Parse(this.properties[key]));
        this.rollCallAddressDoIP = this.ParseAddressesFromProperty("DoIPSourceAddress").ToList<int>();
        this.rollCallIdValues = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
        this.rollCallIdValues.Add("J1708", this.ParseIdValuesFromProperty("J1708IdValues"));
        this.rollCallIdValues.Add("J1939", this.ParseIdValuesFromProperty("J1939IdValues"));
        this.rollCallIdValues.Add("DoIP", this.ParseIdValuesFromProperty("DoIPIdValues"));
      }
      if (this.xml != null && Sapi.GetSapi().ValidateConfigurationFileChecksums && this.GetConfigurationChecksum() != this.checksum)
        throw new InvalidOperationException(this.Name + ".EcuInfo checksum is invalid. Check your installation.");
      if (inStream != null)
        sapi.ReleaseConfiguration(this, inStream, this.xml);
    }
    CultureInfo culture = Sapi.GetSapi().PresentationCulture;
    string translationFileName = this.GetTranslationFileName(culture);
    if (Ecu.translations.TryGetValue(translationFileName, out this.translation))
      return;
    if (this.rollCallManager != null)
    {
      Ecu.translations[translationFileName] = this.rollCallManager.Translations;
    }
    else
    {
      if (this.IsTranslationNecessary(culture) && !this.IsTranslationFilePresent(culture))
        culture = this.OriginalCulture;
      if (!this.IsTranslationFilePresent(culture))
        return;
      this.translation = this.ReadTranslationFile(culture).Reverse<TranslationEntry>().DistinctBy<TranslationEntry, string>((Func<TranslationEntry, string>) (e => e.Qualifier)).ToDictionary<TranslationEntry, string>((Func<TranslationEntry, string>) (item => item.Qualifier));
      Ecu.translations[translationFileName] = this.translation;
    }
  }

  private IEnumerable<int> ParseAddressesFromProperty(string property)
  {
    if (this.properties.ContainsKey(property))
    {
      string[] strArray = this.properties[property].Split(",".ToCharArray());
      for (int index = 0; index < strArray.Length; ++index)
      {
        int sourceAddress;
        if (strArray[index].TryParseSourceAddress(out sourceAddress))
          yield return sourceAddress;
      }
      strArray = (string[]) null;
    }
  }

  private IEnumerable<KeyValuePair<string, string>> ParseIdValuesFromProperty(string property)
  {
    if (this.properties.ContainsKey(property))
    {
      string[] strArray = this.properties[property].Split(";".ToCharArray());
      for (int index = 0; index < strArray.Length; ++index)
      {
        string[] strArray1 = strArray[index].Split("=".ToCharArray());
        if (strArray1.Length == 2)
          yield return new KeyValuePair<string, string>(strArray1[0], strArray1[1]);
      }
      strArray = (string[]) null;
    }
  }

  private bool IsRelated(Ecu rollCallEcu)
  {
    switch (rollCallEcu.protocolName)
    {
      case "J1708":
        return this.rollCallAddressJ1708.Any<byte>((Func<byte, bool>) (address =>
        {
          int num = (int) address;
          byte? sourceAddress = rollCallEcu.SourceAddress;
          int? nullable = sourceAddress.HasValue ? new int?((int) sourceAddress.GetValueOrDefault()) : new int?();
          int valueOrDefault = nullable.GetValueOrDefault();
          return num == valueOrDefault && nullable.HasValue;
        }));
      case "J1939":
        return this.rollCallFunctionJ1939.Any<int>((Func<int, bool>) (function => rollCallEcu.Function.HasValue && function == rollCallEcu.Function.Value)) || this.rollCallAddressJ1939.Any<byte>((Func<byte, bool>) (address =>
        {
          int num = (int) address;
          byte? sourceAddress = rollCallEcu.SourceAddress;
          int? nullable = sourceAddress.HasValue ? new int?((int) sourceAddress.GetValueOrDefault()) : new int?();
          int valueOrDefault = nullable.GetValueOrDefault();
          return num == valueOrDefault && nullable.HasValue;
        }));
      case "DoIP":
        return this.rollCallAddressDoIP.Any<int>((Func<int, bool>) (address =>
        {
          int num = address;
          int? sourceAddress = rollCallEcu.sourceAddress;
          int valueOrDefault = sourceAddress.GetValueOrDefault();
          return num == valueOrDefault && sourceAddress.HasValue;
        }));
      default:
        return false;
    }
  }

  internal bool IsRelated(Protocol protocol)
  {
    switch (protocol)
    {
      case Protocol.DoIP:
        return this.rollCallAddressDoIP.Any<int>();
      case Protocol.J1708:
        return this.rollCallAddressJ1708.Any<byte>();
      case Protocol.J1939:
        return this.rollCallAddressJ1939.Any<byte>() || this.rollCallFunctionJ1939.Any<int>();
      default:
        return false;
    }
  }

  internal bool IsRelated(RollCall manager, int sourceAddress, Ecu allowedByViaEcu)
  {
    bool flag = false;
    switch (manager.Protocol)
    {
      case Protocol.DoIP:
        flag = this.rollCallAddressDoIP.Any<int>((Func<int, bool>) (address => address == sourceAddress));
        break;
      case Protocol.J1708:
        flag = this.rollCallAddressJ1708.Any<byte>((Func<byte, bool>) (address => (int) address == sourceAddress));
        break;
      case Protocol.J1939:
        flag = this.rollCallFunctionJ1939.Any<int>((Func<int, bool>) (function => Ecu.IdValueMatch(manager, sourceAddress, new KeyValuePair<string, string>(RollCall.ID.Function.ToNumberString(), function.ToString((IFormatProvider) CultureInfo.InvariantCulture))))) || this.rollCallAddressJ1939.Any<byte>((Func<byte, bool>) (address => (int) address == sourceAddress));
        break;
    }
    if (!flag)
      return false;
    IEnumerable<KeyValuePair<string, string>> rollCallIdValues = this.GetRollCallIdValues(manager.Protocol, allowedByViaEcu);
    return !rollCallIdValues.Any<KeyValuePair<string, string>>() || rollCallIdValues.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (idValue => Ecu.IdValueMatch(manager, sourceAddress, idValue)));
  }

  internal bool IsRelated(Channel rollCallChannel, Ecu allowedByViaEcu)
  {
    bool flag = false;
    switch (rollCallChannel.Ecu.RollCallManager.Protocol)
    {
      case Protocol.DoIP:
        flag = this.rollCallAddressDoIP.Any<int>((Func<int, bool>) (address => address == rollCallChannel.SourceAddressLong.Value));
        break;
      case Protocol.J1708:
        flag = this.rollCallAddressJ1708.Any<byte>((Func<byte, bool>) (address => (int) address == (int) rollCallChannel.SourceAddress.Value));
        break;
      case Protocol.J1939:
        flag = this.rollCallFunctionJ1939.Any<int>((Func<int, bool>) (function => Ecu.IdValueMatch(rollCallChannel, new KeyValuePair<string, string>(RollCall.ID.Function.ToNumberString(), function.ToString((IFormatProvider) CultureInfo.InvariantCulture))))) || this.rollCallFunctionJ1939.Any<int>((Func<int, bool>) (function => rollCallChannel.Ecu.Function.HasValue && function == rollCallChannel.Ecu.Function.Value)) || this.rollCallAddressJ1939.Any<byte>((Func<byte, bool>) (address => rollCallChannel.SourceAddress.HasValue && (int) address == (int) rollCallChannel.SourceAddress.Value));
        break;
    }
    if (!flag)
      return false;
    IEnumerable<KeyValuePair<string, string>> rollCallIdValues = this.GetRollCallIdValues(rollCallChannel.Ecu.RollCallManager.Protocol, allowedByViaEcu);
    return !rollCallIdValues.Any<KeyValuePair<string, string>>() || rollCallIdValues.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (idValue => Ecu.IdValueMatch(rollCallChannel, idValue)));
  }

  private IEnumerable<KeyValuePair<string, string>> GetRollCallIdValues(
    Protocol protocol,
    Ecu allowedByViaEcu)
  {
    string key = protocol.ToString();
    return allowedByViaEcu != null && this.rollCallIdValues.ContainsKey(key + allowedByViaEcu.Name) ? this.rollCallIdValues[key + allowedByViaEcu.Name] : this.rollCallIdValues[key];
  }

  private static bool IdValueMatch(
    RollCall manager,
    int sourceAddress,
    KeyValuePair<string, string> idValue)
  {
    object identificationValue = manager.GetIdentificationValue(sourceAddress, idValue.Key);
    return identificationValue != null && identificationValue.ToString() == idValue.Value;
  }

  private static bool IdValueMatch(Channel rollCallChannel, KeyValuePair<string, string> idValue)
  {
    EcuInfo ecuInfo = rollCallChannel.EcuInfos[idValue.Key];
    if (ecuInfo != null && ecuInfo.EcuInfoValues.Current != null)
    {
      object obj = ecuInfo.EcuInfoValues.Current.Value;
      Choice choice = obj as Choice;
      if (choice != (object) null && choice.RawValue.ToString() == idValue.Value || obj != null && obj.ToString() == idValue.Value)
        return true;
    }
    return false;
  }

  public IEnumerable<Ecu> RelatedEcus
  {
    get
    {
      List<Ecu> relatedEcus = new List<Ecu>();
      if (this.IsRollCall)
        relatedEcus.AddRange(Sapi.GetSapi().Ecus.Where<Ecu>((Func<Ecu, bool>) (e => e.IsRelated(this))));
      relatedEcus.AddRange(RollCallJ1708.GlobalInstance.Ecus.Union<Ecu>(RollCallJ1939.GlobalInstance.Ecus).Union<Ecu>(RollCallDoIP.GlobalInstance.Ecus).Where<Ecu>((Func<Ecu, bool>) (e => this.IsRelated(e))));
      return (IEnumerable<Ecu>) relatedEcus;
    }
  }

  public bool HasRelatedAddresses
  {
    get
    {
      return this.rollCallAddressJ1708.Union<byte>((IEnumerable<byte>) this.rollCallAddressJ1939).Any<byte>() || this.rollCallFunctionJ1939.Any<int>() || this.rollCallAddressDoIP.Any<int>();
    }
  }

  public bool HasRelatedAddressesWhenViaChannel(Channel viaChannel)
  {
    if (viaChannel == null || this.useRelatedAddresses == null)
      return this.HasRelatedAddresses;
    foreach (string key in this.useRelatedAddresses.Keys.Where<string>((Func<string, bool>) (k => k.StartsWith(viaChannel.Ecu.Name, StringComparison.OrdinalIgnoreCase) && k.Contains<char>(','))))
    {
      string str = key.Split(",".ToCharArray())[1];
      if (viaChannel.DiagnosisVariant.Name.StartsWith(str, StringComparison.OrdinalIgnoreCase) && !this.useRelatedAddresses[key])
        return false;
    }
    return (!this.useRelatedAddresses.ContainsKey(viaChannel.Ecu.Name) || this.useRelatedAddresses[viaChannel.Ecu.Name]) && this.HasRelatedAddressesWhenViaChannel(viaChannel.ActualViaChannel);
  }

  public bool IsPowertrainDevice
  {
    get
    {
      if (this.rollCallManager != null)
        return this.rollCallManager.PowertrainAddresses.Any<byte>((Func<byte, bool>) (pa =>
        {
          int num = (int) pa;
          int? sourceAddress = this.sourceAddress;
          int valueOrDefault = sourceAddress.GetValueOrDefault();
          return num == valueOrDefault && sourceAddress.HasValue;
        }));
      if (this.rollCallAddressJ1708.Intersect<byte>(RollCallJ1708.GlobalInstance.PowertrainAddresses).Any<byte>() || this.rollCallAddressJ1939.Intersect<byte>(RollCallJ1939.GlobalInstance.PowertrainAddresses).Any<byte>())
        return true;
      return this.sourceAddress.HasValue && this.IsUds && RollCallJ1939.GlobalInstance.PowertrainAddresses.Contains<byte>(this.SourceAddress.Value);
    }
  }

  public int Priority
  {
    get
    {
      return !this.IsRollCall ? (this.SourceAddress.HasValue ? (int) this.SourceAddress.Value : 509) : (!this.function.HasValue ? 510 + (int) this.SourceAddress.Value : 765 + this.function.Value);
    }
  }

  public bool IsRollCall => this.rollCallManager != null;

  public bool IsByteMessaging => this.isByteMessaging;

  public DiagnosisProtocol DiagnosisProtocol => this.diagnosisProtocol;

  public bool IsMcd
  {
    get
    {
      return this.diagnosisSource == DiagnosisSource.McdDatabase || this.diagnosisSource == DiagnosisSource.McdApi1;
    }
  }

  public DiagnosisSource DiagnosisSource => this.diagnosisSource;

  public bool IsSameEcuOnDifferentDiagnosisSource(Ecu compareEcu)
  {
    return compareEcu.baseName == this.baseName && compareEcu.DiagnosisSource != this.DiagnosisSource;
  }

  internal void AcquireFromRollCall(
    string descriptionVersion,
    IEnumerable<DiagnosisVariant> variants)
  {
    this.descriptionDataVersion = descriptionVersion ?? string.Empty;
    if (variants != null)
    {
      foreach (DiagnosisVariant variant in variants)
        this.variants.Add(variant);
      this.properties.Add("CurrentDiagnosisVariant", this.variants.Last<DiagnosisVariant>().Name);
    }
    if (this.IsRollCallBaseEcu && !this.variants.Any<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v => v.IsBase)))
      this.variants.Add(new DiagnosisVariant(this, "ROLLCALL", string.Empty, (IEnumerable<Tuple<RollCall.ID, string>>) null, (IEnumerable<string>) new List<string>()));
    string str = this.variants.GroupBy<DiagnosisVariant, string>((Func<DiagnosisVariant, string>) (e => e.Name)).Where<IGrouping<string, DiagnosisVariant>>((Func<IGrouping<string, DiagnosisVariant>, bool>) (g => g.Count<DiagnosisVariant>() > 1)).Select<IGrouping<string, DiagnosisVariant>, string>((Func<IGrouping<string, DiagnosisVariant>, string>) (g => g.Key)).FirstOrDefault<string>();
    if (str != null)
      throw new InvalidOperationException($"Variant names must be unique. The Ecu '{this.Name}' has a duplicate variant '{str}'.");
  }

  internal bool IsRollCallBaseEcu => this.IsRollCall && this.rollCallName == null;

  internal void AcquireInfoMCD()
  {
    McdDBEcuBaseVariant mcdHandle = this.GetMcdHandle();
    this.preamble = mcdHandle.Preamble;
    this.description = mcdHandle.Description;
    this.descriptionFileName = mcdHandle.DatabaseFile;
    this.descriptionDataVersion = McdRoot.GetDatabaseFileVersion(this.descriptionFileName);
    this.ValidateDescriptionDataVersion();
    this.identifier = this.Name;
    this.interfaces.AcquireList();
    this.protocolName = this.interfaces.Where<EcuInterface>((Func<EcuInterface, bool>) (i => !McdRoot.LocationRestricted.Contains(i.ProtocolName))).FirstOrDefault<EcuInterface>().ProtocolName;
    this.diagnosisProtocol = Sapi.GetSapi().DiagnosisProtocols[this.protocolName];
    EcuInterface ecuInterface1 = this.interfaces.FirstOrDefault<EcuInterface>((Func<EcuInterface, bool>) (i => i.ProtocolType == "ISO_15765_3_on_ISO_15765_2"));
    if (ecuInterface1 != null)
    {
      this.requestId = new uint?(Convert.ToUInt32(ecuInterface1.PrioritizedComParameterValue("CP_CanPhysReqId"), (IFormatProvider) CultureInfo.InvariantCulture));
      this.responseId = new uint?(Convert.ToUInt32(ecuInterface1.PrioritizedComParameterValue("CP_CanRespUSDTId"), (IFormatProvider) CultureInfo.InvariantCulture));
      this.sourceAddress = new int?((int) BitConverter.GetBytes(this.responseId.Value)[0]);
    }
    EcuInterface ecuInterface2 = this.interfaces.FirstOrDefault<EcuInterface>((Func<EcuInterface, bool>) (i => i.ProtocolType == "ISO_14229_5_on_ISO_13400_2"));
    if (ecuInterface2 != null)
    {
      this.logicalTesterAddress = new uint?(Convert.ToUInt32(ecuInterface2.PrioritizedComParameterValue("CP_DoIPLogicalTesterAddress"), (IFormatProvider) CultureInfo.InvariantCulture));
      this.logicalEcuAddress = new uint?(Convert.ToUInt32(ecuInterface2.PrioritizedComParameterValue("CP_DoIPLogicalEcuAddress"), (IFormatProvider) CultureInfo.InvariantCulture));
      if (!this.sourceAddress.HasValue || this.sourceAddress.Value == (int) BitConverter.GetBytes(this.logicalEcuAddress.Value)[0])
        this.sourceAddress = new int?((int) this.logicalEcuAddress.Value);
    }
    this.AcquireVariants(mcdHandle);
    this.SetGenericProperties();
  }

  internal McdDBEcuBaseVariant GetMcdHandle() => McdRoot.GetDBEcuBaseVariant(this.baseName);

  internal void AcquireInfo()
  {
    using (CaesarEcu ecu = this.OpenEcuHandle())
    {
      if (ecu != null)
      {
        this.description = ecu.Description;
        this.cBFVersion = ecu.CbfVersion;
        this.preamble = ecu.Preamble;
        this.gPDVersion = ecu.GpdVersion;
        this.descriptionDataVersion = ecu.DescriptionDataVersion;
        this.ValidateDescriptionDataVersion();
        this.protocolName = ecu.ProtocolName;
        this.diagnosisProtocol = Sapi.GetSapi().DiagnosisProtocols[this.protocolName];
        this.descriptionFileName = ecu.FileName;
        this.AcquireVariants(ecu);
      }
    }
    this.interfaces.AcquireList();
    this.identifier = this.Name;
    if (this.interfaces.Count > 0)
    {
      EcuInterface ecuInterface = this.interfaces[0];
      ListDictionary comParameters = ecuInterface.ComParameters;
      if (this.IsUds)
      {
        if (comParameters.Contains((object) "CP_RESPONSE_CANIDENTIFIER"))
        {
          this.requestId = new uint?(Convert.ToUInt32(ecuInterface.PrioritizedComParameterValue("CP_REQUEST_CANIDENTIFIER"), (IFormatProvider) CultureInfo.InvariantCulture));
          this.responseId = new uint?(Convert.ToUInt32(ecuInterface.PrioritizedComParameterValue("CP_RESPONSE_CANIDENTIFIER"), (IFormatProvider) CultureInfo.InvariantCulture));
          this.sourceAddress = new int?((int) BitConverter.GetBytes(this.responseId.Value)[0]);
        }
      }
      else if (string.Equals(this.protocolName, "E1708", StringComparison.Ordinal))
        this.sourceAddress = new int?((int) Convert.ToByte(comParameters[(object) "CP_RESPSOURCEMID"], (IFormatProvider) CultureInfo.InvariantCulture));
      else if (string.Equals(this.protocolName, "E1939", StringComparison.Ordinal) && comParameters.Contains((object) "CP_DESTADDRESS"))
        this.sourceAddress = new int?((int) Convert.ToByte(comParameters[(object) "CP_DESTADDRESS"], (IFormatProvider) CultureInfo.InvariantCulture));
    }
    if (this.Properties.ContainsKey("HasFaultCodeNumberAndMode") && !bool.TryParse(this.Properties["HasFaultCodeNumberAndMode"], out this.faultNumberIsFromEnvironmentData))
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to parse HasFaultCodeNumberAndMode property");
    if (!this.FaultCodeIsEncodedSpnFmi)
    {
      this.faultCodeNumberAndModeFromEngineeringNotes = this.properties.GetValue<bool>("FaultCodeNumberAndModeFromEngineeringNotes", false);
      if (!this.faultCodeNumberAndModeFromEngineeringNotes && (string.Equals(this.ProtocolName, "E1708") || string.Equals(this.ProtocolName, "E1939")))
        this.faultCodeCanBeDuplicate = true;
    }
    if (this.Properties.ContainsKey("HasMultiByteDiagnosticVersion") && !bool.TryParse(this.Properties["HasMultiByteDiagnosticVersion"], out this.hasMultiByteDiagnosticVersion))
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to parse HasMultiByteDiagnosticVersion property");
    this.SetGenericProperties();
  }

  private void SetGenericProperties()
  {
    if (!this.IsUds || !this.sourceAddress.HasValue)
      return;
    if (!this.Properties.ContainsKey("ShortDescription") && RollCallJ1939.GlobalInstance.Translations != null)
    {
      TranslationEntry translationEntry;
      if (RollCallJ1939.GlobalInstance.Translations.TryGetValue(Sapi.MakeTranslationIdentifier(this.sourceAddress.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture), "Source"), out translationEntry))
        this.Properties["ShortDescription"] = translationEntry.Translation;
    }
    if (this.Properties.ContainsKey("Category"))
      return;
    this.Properties["Category"] = this.IsPowertrainDevice ? (this.sourceAddress.Value == 3 ? "Transmission" : "Engine") : "Vehicle";
  }

  private void ValidateDescriptionDataVersion()
  {
    Version result1;
    Version result2;
    if (this.properties.ContainsKey("MinimumDescriptionDataVersion") && Version.TryParse(this.properties["MinimumDescriptionDataVersion"], out result1) && Version.TryParse(this.descriptionDataVersion, out result2) && result2 < result1)
      throw new InvalidOperationException($"Diagnosis description version for {this.Name} is too low");
  }

  public bool HasMultipleByteDiagnosticVersion => this.hasMultiByteDiagnosticVersion;

  internal string Translate(string qualifier, string original)
  {
    TranslationEntry translationEntry;
    return this.translation != null && this.translation.TryGetValue(qualifier, out translationEntry) ? translationEntry.Translation : original;
  }

  public IDictionary<string, string> SuspectParameters
  {
    get
    {
      return this.rollCallManager != null ? (IDictionary<string, string>) this.rollCallManager.GetSuspectParametersForEcu(this) : (IDictionary<string, string>) null;
    }
  }

  public CultureInfo OriginalCulture
  {
    get => CultureInfo.GetCultureInfo(this.Properties["OriginalLanguage"] ?? "en-US");
  }

  internal void IncrementConnectedChannelCount()
  {
    Interlocked.Increment(ref this.connectedChannelCount);
  }

  internal void DecrementConnectedChannelCount()
  {
    Interlocked.Decrement(ref this.connectedChannelCount);
  }

  internal XmlNode Xml => this.xml;

  internal List<CompoundEnvironmentData> CompoundEnvironmentDatas => this.compoundEnvironmentDatas;

  internal CaesarEcu OpenEcuHandle() => Ecu.OpenEcuHandle(this.Name);

  internal static CaesarEcu OpenEcuHandle(string name)
  {
    CaesarEcu caesarEcu = (CaesarEcu) null;
    uint interfaceTypeCount = CaesarRoot.GetAvailableInterfaceTypeCount(name);
    for (uint index = 0; index < interfaceTypeCount; ++index)
    {
      if (caesarEcu == null)
      {
        try
        {
          using (CaesarEcuInterface interfaceByIndex = CaesarRoot.GetInterfaceByIndex(name, index))
          {
            if (interfaceByIndex != null)
              caesarEcu = interfaceByIndex.OpenEcu(name);
          }
        }
        catch (CaesarErrorException ex)
        {
        }
      }
      else
        break;
    }
    return caesarEcu;
  }

  internal DiagnosisVariant GetDiagnosisVariantFromIDBlock(CaesarIdBlock idBlock)
  {
    DiagnosisVariant variantFromIdBlock = (DiagnosisVariant) null;
    using (CaesarEcu caesarEcu = this.OpenEcuHandle())
    {
      if (caesarEcu != null)
      {
        if (caesarEcu.SetVariantByIdBlock(idBlock))
        {
          string variantName = caesarEcu.VariantName;
          if (!string.IsNullOrEmpty(variantName))
            variantFromIdBlock = this.variants[variantName];
        }
      }
    }
    return variantFromIdBlock;
  }

  internal void AddViaEcu(Ecu viaEcu, string variantPrefix)
  {
    this.viaEcus.Add(new Tuple<Ecu, string>(viaEcu, variantPrefix));
    if (this.rollCallIdValues == null)
      return;
    string[] strArray = new string[3]
    {
      "J1708",
      "J1939",
      "DoIP"
    };
    foreach (string str in strArray)
    {
      IEnumerable<KeyValuePair<string, string>> valuesFromProperty = this.ParseIdValuesFromProperty($"{str}IdValues_{viaEcu.Name}");
      if (valuesFromProperty.Any<KeyValuePair<string, string>>())
        this.rollCallIdValues.Add(str + viaEcu.Name, valuesFromProperty);
    }
  }

  internal bool PassesConnectionResourceFilter(ConnectionResource resource)
  {
    if (this.restrictedPortIndex.HasValue && !string.Equals(resource.Type, "ETHERNET") && resource.PortIndex != this.restrictedPortIndex.Value || this.restrictedInterface != null && !string.Equals(resource.Interface.Qualifier, this.restrictedInterface, StringComparison.Ordinal))
      return false;
    if (!(this.restrictedPort == "ViaEcu"))
      return true;
    Channel currentViaEcuChannel = this.GetCurrentViaEcuChannel(out bool _);
    if (currentViaEcuChannel == null)
      return false;
    if (!currentViaEcuChannel.Ecu.restrictedPortIndexForViaEcu.HasValue && currentViaEcuChannel.Ecu.restrictedInterfaceForViaEcu == null)
      return currentViaEcuChannel.ConnectionResource.IsEquivalent(resource);
    return (!currentViaEcuChannel.Ecu.restrictedPortIndexForViaEcu.HasValue || string.Equals(resource.Type, "ETHERNET") || resource.PortIndex == currentViaEcuChannel.Ecu.restrictedPortIndexForViaEcu.Value) && (currentViaEcuChannel.Ecu.restrictedInterfaceForViaEcu == null || string.Equals(resource.Interface.Qualifier, currentViaEcuChannel.Ecu.restrictedInterfaceForViaEcu, StringComparison.Ordinal));
  }

  internal Channel GetCurrentViaEcuChannel(out bool validatedByExtension)
  {
    validatedByExtension = false;
    foreach (Tuple<Ecu, string> viaEcu1 in this.viaEcus)
    {
      Tuple<Ecu, string> viaEcu = viaEcu1;
      Channel currentViaEcuChannel = Sapi.GetSapi().Channels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu == viaEcu.Item1 && c.ConnectionResource != null));
      if (currentViaEcuChannel != null)
      {
        string[] strArray;
        if (viaEcu.Item2 == null)
          strArray = new string[0];
        else
          strArray = viaEcu.Item2.Split('+');
        foreach (string str in strArray)
        {
          if (str.StartsWith("extension:", StringComparison.Ordinal))
          {
            string[] source = str.Substring(10).Split(new char[3]
            {
              '(',
              ',',
              ')'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (!(bool) currentViaEcuChannel.Extension.Invoke(source[0], (object[]) ((IEnumerable<string>) source).Skip<string>(1).ToArray<string>()))
              return (Channel) null;
            validatedByExtension = true;
          }
          else if (!currentViaEcuChannel.DiagnosisVariant.Name.StartsWith(str, StringComparison.Ordinal))
            return (Channel) null;
        }
        return currentViaEcuChannel;
      }
    }
    return (Channel) null;
  }

  public ListDictionary EcuInfoComParameters => this.ecuInfoComParameters;

  internal int GetComParameter(string name, int defaultValue)
  {
    return !this.EcuInfoComParameters.Contains((object) name) ? defaultValue : Convert.ToInt32(this.EcuInfoComParameters[(object) name], (IFormatProvider) CultureInfo.InvariantCulture);
  }

  internal bool IgnoreQualifier(string qualifier)
  {
    return Ecu.IsQualifierInList(this.ignoredQualifiers, qualifier) && !Ecu.IsQualifierInList(this.defaultActionQualifiers, qualifier);
  }

  internal bool MakeStoredQualifier(string qualifier)
  {
    return Ecu.IsQualifierInList(this.makeStoredQualifiers, qualifier) && !Ecu.IsQualifierInList(this.defaultActionQualifiers, qualifier);
  }

  internal bool MakeInstrumentQualifier(string qualifier)
  {
    return Ecu.IsQualifierInList(this.makeInstrumentQualifiers, qualifier) && !Ecu.IsQualifierInList(this.defaultActionQualifiers, qualifier);
  }

  internal bool ForceRequestQualifier(string qualifier)
  {
    return Ecu.IsQualifierInList(this.forceRequestQualifiers, qualifier);
  }

  internal object CacheTimeQualifier(string qualifier)
  {
    foreach (KeyValuePair<string, int> cacheTimeQualifier in this.cacheTimeQualifiers)
    {
      if (qualifier.StartsWith(cacheTimeQualifier.Key, StringComparison.Ordinal))
        return (object) cacheTimeQualifier.Value;
    }
    return (object) null;
  }

  internal T GetEcuInfoAttribute<T>(string attribute, string qualifier, string variantName)
  {
    string str1 = (string) null;
    if (this.ecuInfoAttributes != null)
    {
      IEnumerable<KeyValuePair<string, string>> source;
      if (this.ecuInfoAttributes.TryGetValue(Tuple.Create<string, string>(attribute, variantName), out source))
        str1 = source.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kv => qualifier.StartsWith(kv.Key, StringComparison.Ordinal))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => kv.Value)).FirstOrDefault<string>();
      if (str1 == null && this.ecuInfoAttributes.TryGetValue(Tuple.Create<string, string>(attribute, (string) null), out source))
        str1 = source.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kv => qualifier.StartsWith(kv.Key, StringComparison.Ordinal))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => kv.Value)).FirstOrDefault<string>();
    }
    Type nullableType = typeof (T);
    if (str1 == null)
      return default (T);
    string str2 = str1;
    Type conversionType = Nullable.GetUnderlyingType(nullableType);
    if ((object) conversionType == null)
      conversionType = nullableType;
    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
    return (T) Convert.ChangeType((object) str2, conversionType, (IFormatProvider) invariantCulture);
  }

  internal bool SummaryQualifier(string qualifier)
  {
    return Ecu.IsQualifierInList(this.summaryQualifiers, qualifier);
  }

  internal string NameSplit => this.nameSplit;

  internal string ShortName(string name)
  {
    int num = name.IndexOf(this.NameSplit, StringComparison.Ordinal);
    return num != -1 ? name.Substring(num + this.NameSplit.Length) : name;
  }

  internal Dictionary<string, string> AffectServices { private set; get; }

  internal Dictionary<string, string> PreService { private set; get; }

  internal Dictionary<string, string> AlternateQualifiers { private set; get; }

  internal static object ResourceLock => Ecu.resourceLock;

  public ConnectionResourceCollection GetConnectionResources()
  {
    return this.GetConnectionResources(new byte?());
  }

  internal ConnectionResourceCollection GetConnectionResources(byte? sourceAddress)
  {
    switch (Sapi.GetSapi().InitState)
    {
      case InitState.NotInitialized:
        throw new InvalidOperationException("Sapi not initialized");
      case InitState.Online:
        ConnectionResourceCollection connectionResources = new ConnectionResourceCollection();
        if (this.IsMcd)
          this.PopulateMcdConnectionResources(connectionResources);
        else
          this.PopulateCaesarConnectionResources(connectionResources, sourceAddress);
        return connectionResources;
      case InitState.Offline:
        throw new InvalidOperationException("Resource cannot be acquired in offline operation mode");
      default:
        return (ConnectionResourceCollection) null;
    }
  }

  private void PopulateCaesarConnectionResources(
    ConnectionResourceCollection connectionResources,
    byte? sourceAddress)
  {
    if (this.interfaces.Count <= 0)
      return;
    lock (Ecu.resourceLock)
    {
      try
      {
        CaesarRoot.LockResources();
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        throw new CaesarException(ex, negativeResponseCode);
      }
      uint ecuResourceCount;
      try
      {
        ecuResourceCount = CaesarRoot.GetAvailableEcuResourceCount(this.baseName);
      }
      catch (CaesarErrorException ex)
      {
        CaesarRoot.UnlockResources();
        byte? negativeResponseCode = new byte?();
        throw new CaesarException(ex, negativeResponseCode);
      }
      for (ushort index = 0; (uint) index < ecuResourceCount; ++index)
      {
        CaesarResource availableEcuResource;
        try
        {
          availableEcuResource = CaesarRoot.GetAvailableEcuResource(this.baseName, index);
        }
        catch (CaesarErrorException ex)
        {
          CaesarRoot.UnlockResources();
          byte? negativeResponseCode = new byte?();
          throw new CaesarException(ex, negativeResponseCode);
        }
        ConnectionResource connectionResource = new ConnectionResource(this, availableEcuResource, sourceAddress);
        ConnectionResource equivalent = connectionResources.GetEquivalent(connectionResource);
        if (equivalent == null)
          connectionResources.Add(connectionResource);
        else if (equivalent.Restricted && !connectionResource.Restricted)
          equivalent.Restricted = false;
      }
      CaesarRoot.UnlockResources();
    }
  }

  private void PopulateMcdConnectionResources(ConnectionResourceCollection connectionResources)
  {
    foreach (McdInterface currentInterface in McdRoot.CurrentInterfaces)
    {
      int portIndex = 1;
      IEnumerable<McdInterfaceResource> resources;
      if (currentInterface.Resources.All<McdInterfaceResource>((Func<McdInterfaceResource, bool>) (r => r.PhysicalInterfaceLinkType == "ETHERNET")))
      {
        resources = currentInterface.Resources.Take<McdInterfaceResource>(1);
        portIndex = 0;
      }
      else
        resources = currentInterface.Resources;
      foreach (McdInterfaceResource interfaceResource in resources)
      {
        McdInterfaceResource theInterfaceResource = interfaceResource;
        IEnumerable<EcuInterface> source = this.interfaces.Where<EcuInterface>((Func<EcuInterface, bool>) (i => i.ProtocolType == theInterfaceResource.ProtocolType));
        if (source.Any<EcuInterface>())
        {
          bool flag = false;
          foreach (EcuInterface ecuInterface in source)
          {
            ConnectionResource connectionResource = new ConnectionResource(this, ecuInterface, currentInterface, theInterfaceResource, portIndex);
            connectionResources.Add(connectionResource);
            if (!connectionResource.Restricted)
            {
              if (!flag)
                flag = true;
              else
                connectionResource.Restricted = true;
            }
          }
          ++portIndex;
        }
      }
    }
  }

  private string GetConfigurationChecksum()
  {
    MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
    cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(this.xml.SelectSingleNode(nameof (Ecu)).InnerXml));
    return BitConverter.ToString(cryptoServiceProvider.Hash);
  }

  public override string ToString() => this.Name;

  public string ShortDescription
  {
    get
    {
      if (this.rollCallManager == null)
        return this.Properties[nameof (ShortDescription)] ?? string.Empty;
      if (this.rollCallDescription != null)
        return this.rollCallDescription;
      return this.function.HasValue ? this.Translate(Sapi.MakeTranslationIdentifier(RollCall.ID.Function.ToNumberString(), this.function.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture), "Name"), string.Empty) : this.Translate(Sapi.MakeTranslationIdentifier(this.sourceAddress.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture), "Source"), string.Empty);
    }
  }

  public string Name
  {
    get
    {
      if (this.rollCallManager == null)
        return this.baseName;
      if (this.rollCallName != null)
        return this.rollCallName;
      return this.function.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}-{2}", (object) this.protocolName, (object) this.SourceAddress, (object) this.function.Value) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}", (object) this.protocolName, (object) this.SourceAddress);
    }
  }

  internal RollCall RollCallManager => this.rollCallManager;

  public string DisplayName
  {
    get
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} - {1}", (object) this.Name, (object) this.ShortDescription);
    }
  }

  internal byte? SourceAddress
  {
    get => !this.sourceAddress.HasValue ? new byte?() : new byte?((byte) this.sourceAddress.Value);
  }

  internal int? SourceAddressLong => this.sourceAddress;

  public bool IsVirtual
  {
    get
    {
      return this.IsRollCall ? this.sourceAddress.HasValue && this.rollCallManager.IsVirtual(this.sourceAddress.Value) : this.diagnosisSource != DiagnosisSource.CaesarApi1 && this.diagnosisSource != DiagnosisSource.McdApi1 && this.IsUds && !this.Interfaces.Any<EcuInterface>((Func<EcuInterface, bool>) (ei => ei.ComParameters.Contains((object) "CP_RESPONSE_CANIDENTIFIER") || ei.ComParameters.Contains((object) "CP_CanRespUSDTId") || ei.ComParameters.Contains((object) "CP_DoIPLogicalEcuAddress")));
    }
  }

  public bool IsUds => this.ProtocolName.StartsWith("UDS", StringComparison.Ordinal);

  internal int? Function => this.function;

  public string Description => this.description;

  public string CbfVersion => this.cBFVersion;

  public string Preamble => this.preamble;

  public string GpdVersion => this.gPDVersion;

  public string ProtocolName => this.protocolName;

  public string DescriptionDataVersion => this.descriptionDataVersion;

  public string DescriptionFileName => this.descriptionFileName;

  internal string ExtensionSource => this.extensionSource;

  internal bool ConfigurationLoadedFromFile => this.configurationLoadedFromFile;

  public int? ConfigurationFileVersion => this.configurationFileVersion;

  public string Identifier
  {
    get
    {
      if (this.SourceAddress.HasValue)
      {
        string str = this.protocolName;
        if (this.IsMcd)
          str = new string(this.protocolName.TakeWhile<char>((Func<char, bool>) (c => c != '_')).ToArray<char>());
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}", (object) str, (object) this.SourceAddress);
      }
      return this.function.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-F{1}", (object) this.protocolName, (object) this.function) : this.identifier;
    }
  }

  public IList<Ecu> ViaEcus
  {
    get
    {
      return (IList<Ecu>) this.viaEcus.Select<Tuple<Ecu, string>, Ecu>((Func<Tuple<Ecu, string>, Ecu>) (vep => vep.Item1)).ToList<Ecu>().AsReadOnly();
    }
  }

  public DiagnosisVariantCollection DiagnosisVariants => this.variants;

  public EcuInterfaceCollection Interfaces => this.interfaces;

  public bool MarkedForAutoConnect
  {
    get => !this.OfflineSupportOnly && this.markedForAutoConnect;
    set => this.markedForAutoConnect = value;
  }

  public bool OfflineSupportOnly { get; private set; }

  public bool ProhibitAutoConnection { get; private set; }

  public Dump SignalNotAvailableValue { get; private set; }

  public bool SupportsDoublePrecisionVariantCoding { get; private set; }

  internal Ecu.ResponseParameterQualifierSource CaesarEquivalentResponseParameterQualifierSource { get; private set; }

  public int ConnectedChannelCount => this.connectedChannelCount;

  public StringDictionary Properties => this.properties;

  internal bool FaultCodeCanBeDuplicate => this.faultCodeCanBeDuplicate;

  internal bool FaultCodeIsEncodedSpnFmi
  {
    get
    {
      if (!this.faultCodeIsEncodedSpnFmi.HasValue)
      {
        bool flag1 = false;
        bool flag2 = false;
        foreach (CompoundEnvironmentData compoundEnvironmentData in this.CompoundEnvironmentDatas)
        {
          if (compoundEnvironmentData.Referenced.Count == 1)
          {
            if (string.Equals(compoundEnvironmentData.Qualifier, "FaultCodeNumber", StringComparison.Ordinal))
              flag1 = string.Equals(compoundEnvironmentData.Referenced[0], "UDSCODESPN", StringComparison.Ordinal);
            if (string.Equals(compoundEnvironmentData.Qualifier, "FaultCodeMode", StringComparison.Ordinal))
              flag2 = string.Equals(compoundEnvironmentData.Referenced[0], "UDSCODEFMI", StringComparison.Ordinal);
          }
        }
        this.faultCodeIsEncodedSpnFmi = new bool?(flag1 & flag2);
      }
      return this.faultCodeIsEncodedSpnFmi.Value;
    }
  }

  internal bool FaultNumberIsFromEnvironmentData => this.faultNumberIsFromEnvironmentData;

  internal bool FaultCodeNumberAndModeFromEngineeringNotes
  {
    get => this.faultCodeNumberAndModeFromEngineeringNotes;
  }

  public ListDictionary ProtocolComParameters
  {
    get
    {
      return this.diagnosisProtocol == null ? (ListDictionary) null : this.diagnosisProtocol.ComParameters;
    }
  }

  internal Type ExtensionType
  {
    get
    {
      if (!this.haveAttemptedExtensionTypeLoad)
      {
        Assembly assembly = this.ConfigurationLoadedFromFile ? Sapi.GetSapi().SapiFileSystemExtensionAssembly : Sapi.GetSapi().SapiExtensionAssembly;
        if (assembly != (Assembly) null)
        {
          this.extensionType = assembly.GetType(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SapiExtension.{0}.{1}", (object) this.DiagnosisSource, (object) this.Name));
          if (this.extensionType == (Type) null)
            this.extensionType = assembly.GetType(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SapiExtension.{0}", (object) this.Name));
        }
        else
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "No extension");
        this.haveAttemptedExtensionTypeLoad = true;
      }
      return this.extensionType;
    }
  }

  public Dictionary<string, string> GetTranslatedStringsForTranslation()
  {
    return this.GetTranslatedStringsForTranslation(new int?(), (string[]) null);
  }

  public Dictionary<string, string> GetTranslatedStringsForTranslation(
    int? maxAccessLevel,
    string[] forbiddenQualifierPrefixes)
  {
    return this.GetStringsForTranslation(maxAccessLevel, forbiddenQualifierPrefixes).Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (c => new KeyValuePair<string, string>(c.Key, this.Translate(c.Key, c.Value)))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value));
  }

  public Dictionary<string, string> GetStringsForTranslation()
  {
    return this.GetStringsForTranslation(new int?(), (string[]) null);
  }

  public Dictionary<string, string> GetStringsForTranslation(
    int? maxAccessLevel,
    string[] forbiddenQualifierPrefixes)
  {
    this.translation = (Dictionary<string, TranslationEntry>) null;
    Dictionary<string, string> table = new Dictionary<string, string>();
    foreach (DiagnosisVariant diagnosisVariant in (ReadOnlyCollection<DiagnosisVariant>) this.DiagnosisVariants)
    {
      using (Channel channel = Sapi.GetSapi().Channels.OpenOffline(diagnosisVariant))
      {
        foreach (Instrument instrument1 in (ReadOnlyCollection<Instrument>) channel.Instruments)
        {
          Instrument instrument = instrument1;
          if ((!maxAccessLevel.HasValue || instrument.AccessLevel <= maxAccessLevel.Value) && (forbiddenQualifierPrefixes == null || ((IEnumerable<string>) forbiddenQualifierPrefixes).All<string>((Func<string, bool>) (p => !instrument.Qualifier.StartsWith(p, StringComparison.Ordinal)))))
            instrument.AddStringsForTranslation(table);
        }
        foreach (EcuInfo ecuInfo1 in (ReadOnlyCollection<EcuInfo>) channel.EcuInfos)
        {
          EcuInfo ecuInfo = ecuInfo1;
          if ((!maxAccessLevel.HasValue || ecuInfo.AccessLevel <= maxAccessLevel.Value) && (forbiddenQualifierPrefixes == null || ((IEnumerable<string>) forbiddenQualifierPrefixes).All<string>((Func<string, bool>) (p => !ecuInfo.Qualifier.StartsWith(p, StringComparison.Ordinal)))))
            ecuInfo.AddStringsForTranslation(table);
        }
        foreach (Service service1 in (ReadOnlyCollection<Service>) channel.Services)
        {
          Service service = service1;
          if ((!maxAccessLevel.HasValue || service.Access <= maxAccessLevel.Value) && (forbiddenQualifierPrefixes == null || ((IEnumerable<string>) forbiddenQualifierPrefixes).All<string>((Func<string, bool>) (p => !service.Qualifier.StartsWith(p, StringComparison.Ordinal)))))
            service.AddStringsForTranslation(table);
        }
        foreach (FaultCode faultCode in channel.FaultCodes)
          faultCode.AddStringsForTranslation(table, maxAccessLevel);
        foreach (Service environmentDataDescription in (ReadOnlyCollection<Service>) channel.FaultCodes.EnvironmentDataDescriptions)
          environmentDataDescription.AddStringsForTranslation(table);
        foreach (Parameter parameter1 in (ReadOnlyCollection<Parameter>) channel.Parameters)
        {
          Parameter parameter = parameter1;
          if ((!maxAccessLevel.HasValue || parameter.ReadAccess <= maxAccessLevel.Value) && (forbiddenQualifierPrefixes == null || ((IEnumerable<string>) forbiddenQualifierPrefixes).All<string>((Func<string, bool>) (p => !parameter.Qualifier.StartsWith(p, StringComparison.Ordinal) && !parameter.CombinedQualifier.StartsWith(p, StringComparison.Ordinal)))))
            parameter.AddStringsForTranslation(table);
        }
        foreach (CodingParameterGroup codingParameterGroup in (ReadOnlyCollection<CodingParameterGroup>) channel.CodingParameterGroups)
        {
          foreach (CodingChoice choice in (ReadOnlyCollection<CodingChoice>) codingParameterGroup.Choices)
            choice.AddStringsForTranslation(table);
          foreach (CodingParameter parameter in (ReadOnlyCollection<CodingParameter>) codingParameterGroup.Parameters)
          {
            foreach (CodingChoice choice in (ReadOnlyCollection<CodingChoice>) parameter.Choices)
              choice.AddStringsForTranslation(table);
          }
        }
        channel.Disconnect();
      }
    }
    string translationFileName = this.GetTranslationFileName(Sapi.GetSapi().PresentationCulture);
    if (Ecu.translations.ContainsKey(translationFileName))
      this.translation = Ecu.translations[translationFileName];
    return table;
  }

  public bool IsTranslationFilePresent(CultureInfo culture)
  {
    return File.Exists(this.GetTranslationFileName(culture));
  }

  public bool IsTranslationNecessary(CultureInfo culture)
  {
    return !this.OriginalCulture.Neutralize().Name.Equals(culture.Neutralize().Name);
  }

  private string GetTranslationFileName(CultureInfo culture)
  {
    return TranslationEntry.GetTranslationFileName(this.baseName, culture);
  }

  public IEnumerable<CultureInfo> SupportedCultures
  {
    get
    {
      CultureInfo originalCulture = this.OriginalCulture.Neutralize();
      yield return originalCulture;
      string translationFileName = this.GetTranslationFileName((CultureInfo) null);
      string[] strArray = Directory.GetFiles(Path.GetDirectoryName(translationFileName), Path.GetFileName(translationFileName));
      for (int index = 0; index < strArray.Length; ++index)
      {
        CultureInfo supportedCulture = CultureInfo.GetCultureInfo(strArray[index].Split(".".ToCharArray())[1]).Neutralize();
        if (supportedCulture != originalCulture)
          yield return supportedCulture;
      }
      strArray = (string[]) null;
    }
  }

  public IEnumerable<TranslationEntry> ReadTranslationFile(CultureInfo culture)
  {
    return TranslationEntry.ReadTranslationFile(this.baseName, culture);
  }

  public void WriteTranslationFile(
    CultureInfo culture,
    IEnumerable<TranslationEntry> translations,
    bool emitEmptyTranslations)
  {
    TranslationEntry.WriteTranslationFile(this.baseName, culture, this.DescriptionDataVersion, translations, emitEmptyTranslations);
  }

  private void AcquireVariants(CaesarEcu ecu)
  {
    StringCollection variants = ecu.Variants;
    for (int index = 0; index < variants.Count; ++index)
    {
      string name = variants[index];
      if (ecu.SetVariant(name))
      {
        Part partNumber1 = (Part) null;
        string partNumber2 = ecu.PartNumber;
        if (partNumber2 != null)
        {
          object partVersion = ecu.PartVersion;
          partNumber1 = partVersion == null ? new Part(partNumber2) : new Part(partNumber2, partVersion);
        }
        this.variants.Add(new DiagnosisVariant(this, name, ecu.VariantDescription, partNumber1, ecu.GetVariantIdBlocks(name)));
      }
    }
  }

  private void AcquireVariants(McdDBEcuBaseVariant ecu)
  {
    List<string> list1 = this.interfaces.Select<EcuInterface, string>((Func<EcuInterface, string>) (i => i.ProtocolName)).ToList<string>();
    List<Tuple<string, string, IEnumerable<string>, IEnumerable<McdDBLocation>, bool>> tupleList = new List<Tuple<string, string, IEnumerable<string>, IEnumerable<McdDBLocation>, bool>>();
    tupleList.Add(Tuple.Create<string, string, IEnumerable<string>, IEnumerable<McdDBLocation>, bool>("_base_", string.Empty, ecu.DBLocationNames, list1.Select<string, McdDBLocation>((Func<string, McdDBLocation>) (l => ecu.GetDBLocationForProtocol(l))), true));
    foreach (string dbEcuVariantName in ecu.DBEcuVariantNames)
    {
      McdDBEcuVariant variant = ecu.GetDBEcuVariant(dbEcuVariantName);
      tupleList.Add(Tuple.Create<string, string, IEnumerable<string>, IEnumerable<McdDBLocation>, bool>(dbEcuVariantName, variant.Description, variant.DBLocationNames, list1.Select<string, McdDBLocation>((Func<string, McdDBLocation>) (l => variant.GetDBLocationForProtocol(l))), false));
    }
    foreach (Tuple<string, string, IEnumerable<string>, IEnumerable<McdDBLocation>, bool> tuple in tupleList)
    {
      List<McdDBLocation> list2 = tuple.Item4.Where<McdDBLocation>((Func<McdDBLocation, bool>) (l => l != null)).ToList<McdDBLocation>();
      string number = list2.Select<McdDBLocation, string>((Func<McdDBLocation, string>) (l => l.PartNumber)).FirstOrDefault<string>((Func<string, bool>) (pn => !string.IsNullOrEmpty(pn)));
      McdDBLocation mcdDbLocation = list2.FirstOrDefault<McdDBLocation>((Func<McdDBLocation, bool>) (l => l.VariantAttributes != null && l.VariantAttributes.Any<KeyValuePair<string, string>>()));
      this.variants.Add(new DiagnosisVariant(this, tuple.Item1, tuple.Item2, !string.IsNullOrEmpty(number) ? new Part(number) : (Part) null, mcdDbLocation?.VariantAttributes, list2.SelectMany<McdDBLocation, McdDBMatchingPattern>((Func<McdDBLocation, IEnumerable<McdDBMatchingPattern>>) (l => l.DBVariantPatterns)), tuple.Item5)
      {
        Locations = tuple.Item3
      });
    }
  }

  private static bool IsQualifierInList(List<Regex> qualifiers, string qualifier)
  {
    foreach (Regex qualifier1 in qualifiers)
    {
      if (qualifier1.Match(qualifier).Success)
        return true;
    }
    return false;
  }

  internal TextFieldParser GetDefParser()
  {
    string name = this.Properties.GetValue<string>("VcpDefPath", (string) null);
    if (name != null)
    {
      string path = Environment.ExpandEnvironmentVariables(name);
      if (File.Exists(path))
      {
        TextFieldParser defParser = new TextFieldParser(path);
        defParser.SetDelimiters(",");
        defParser.HasFieldsEnclosedInQuotes = true;
        return defParser;
      }
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Missing VCP .DEF file " + path);
    }
    return (TextFieldParser) null;
  }

  internal uint? RequestId => this.requestId;

  internal uint? ResponseId => this.responseId;

  internal uint? LogicalEcuAddress => this.logicalEcuAddress;

  internal uint? LogicalTesterAddress => this.logicalTesterAddress;

  public bool RemoveDescriptionFile()
  {
    if (this.diagnosisSource != DiagnosisSource.CaesarDatabase || Sapi.GetSapi().Ecus.GetConnectedCountForIdentifier(this.Identifier) != 0)
      return false;
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Removing CBF: {0}", (object) this.DescriptionFileName));
    try
    {
      CaesarRoot.RemoveCbfFile(this.DescriptionFileName);
      Sapi.GetSapi().Ecus.Remove(this);
      return true;
    }
    catch (CaesarErrorException ex)
    {
      byte? negativeResponseCode = new byte?();
      throw new CaesarException(ex, negativeResponseCode);
    }
  }

  public ChannelOptions AvailableChannelOptions
  {
    get
    {
      return (ChannelOptions) (5 | (this.properties.ContainsKey("InitializeService") ? 64 /*0x40*/ : 0) | (this.properties.ContainsKey("CommitToPermanentMemoryService") || this.properties.ContainsKey("ParameterWriteInitializeService") ? 128 /*0x80*/ : 0) | (this.properties.ContainsKey("MaintainSession") ? 8 : 0) | (this.PreService.Any<KeyValuePair<string, string>>() ? 16 /*0x10*/ : 0) | (this.AffectServices.Any<KeyValuePair<string, string>>() ? 32 /*0x20*/ : 0));
    }
  }

  internal enum ResponseParameterQualifierSource
  {
    None,
    Name,
    Qualifier,
  }
}
