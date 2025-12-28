using System;
using System.Collections.Generic;
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
using CaesarAbstraction;
using McdAbstraction;
using Microsoft.VisualBasic.FileIO;

namespace SapiLayer1;

public sealed class Ecu
{
	internal enum ResponseParameterQualifierSource
	{
		None,
		Name,
		Qualifier
	}

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

	public IEnumerable<Ecu> RelatedEcus
	{
		get
		{
			List<Ecu> list = new List<Ecu>();
			if (IsRollCall)
			{
				list.AddRange(Sapi.GetSapi().Ecus.Where((Ecu e) => e.IsRelated(this)));
			}
			list.AddRange(from e in RollCallJ1708.GlobalInstance.Ecus.Union(RollCallJ1939.GlobalInstance.Ecus).Union(RollCallDoIP.GlobalInstance.Ecus)
				where IsRelated(e)
				select e);
			return list;
		}
	}

	public bool HasRelatedAddresses
	{
		get
		{
			if (!rollCallAddressJ1708.Union(rollCallAddressJ1939).Any() && !rollCallFunctionJ1939.Any())
			{
				return rollCallAddressDoIP.Any();
			}
			return true;
		}
	}

	public bool IsPowertrainDevice
	{
		get
		{
			if (rollCallManager != null)
			{
				return rollCallManager.PowertrainAddresses.Any((byte pa) => pa == sourceAddress);
			}
			if (!rollCallAddressJ1708.Intersect(RollCallJ1708.GlobalInstance.PowertrainAddresses).Any() && !rollCallAddressJ1939.Intersect(RollCallJ1939.GlobalInstance.PowertrainAddresses).Any())
			{
				if (sourceAddress.HasValue && IsUds)
				{
					return RollCallJ1939.GlobalInstance.PowertrainAddresses.Contains(SourceAddress.Value);
				}
				return false;
			}
			return true;
		}
	}

	public int Priority
	{
		get
		{
			if (!IsRollCall)
			{
				if (SourceAddress.HasValue)
				{
					return SourceAddress.Value;
				}
				return 509;
			}
			if (!function.HasValue)
			{
				return 510 + SourceAddress.Value;
			}
			return 765 + function.Value;
		}
	}

	public bool IsRollCall => rollCallManager != null;

	public bool IsByteMessaging => isByteMessaging;

	public DiagnosisProtocol DiagnosisProtocol => diagnosisProtocol;

	public bool IsMcd
	{
		get
		{
			if (diagnosisSource != DiagnosisSource.McdDatabase)
			{
				return diagnosisSource == DiagnosisSource.McdApi1;
			}
			return true;
		}
	}

	public DiagnosisSource DiagnosisSource => diagnosisSource;

	internal bool IsRollCallBaseEcu
	{
		get
		{
			if (IsRollCall)
			{
				return rollCallName == null;
			}
			return false;
		}
	}

	public bool HasMultipleByteDiagnosticVersion => hasMultiByteDiagnosticVersion;

	public IDictionary<string, string> SuspectParameters
	{
		get
		{
			if (rollCallManager != null)
			{
				return rollCallManager.GetSuspectParametersForEcu(this);
			}
			return null;
		}
	}

	public CultureInfo OriginalCulture => CultureInfo.GetCultureInfo(Properties["OriginalLanguage"] ?? "en-US");

	internal XmlNode Xml => xml;

	internal List<CompoundEnvironmentData> CompoundEnvironmentDatas => compoundEnvironmentDatas;

	public ListDictionary EcuInfoComParameters => ecuInfoComParameters;

	internal string NameSplit => nameSplit;

	internal Dictionary<string, string> AffectServices { get; private set; }

	internal Dictionary<string, string> PreService { get; private set; }

	internal Dictionary<string, string> AlternateQualifiers { get; private set; }

	internal static object ResourceLock => resourceLock;

	public string ShortDescription
	{
		get
		{
			if (rollCallManager != null)
			{
				if (rollCallDescription != null)
				{
					return rollCallDescription;
				}
				if (function.HasValue)
				{
					return Translate(Sapi.MakeTranslationIdentifier(RollCall.ID.Function.ToNumberString(), function.Value.ToString(CultureInfo.InvariantCulture), "Name"), string.Empty);
				}
				return Translate(Sapi.MakeTranslationIdentifier(sourceAddress.Value.ToString(CultureInfo.InvariantCulture), "Source"), string.Empty);
			}
			return Properties["ShortDescription"] ?? string.Empty;
		}
	}

	public string Name
	{
		get
		{
			if (rollCallManager == null)
			{
				return baseName;
			}
			if (rollCallName != null)
			{
				return rollCallName;
			}
			if (function.HasValue)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2}", protocolName, SourceAddress, function.Value);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", protocolName, SourceAddress);
		}
	}

	internal RollCall RollCallManager => rollCallManager;

	public string DisplayName => string.Format(CultureInfo.InvariantCulture, "{0} - {1}", Name, ShortDescription);

	internal byte? SourceAddress
	{
		get
		{
			if (!sourceAddress.HasValue)
			{
				return null;
			}
			return (byte)sourceAddress.Value;
		}
	}

	internal int? SourceAddressLong => sourceAddress;

	public bool IsVirtual
	{
		get
		{
			if (IsRollCall)
			{
				if (!sourceAddress.HasValue)
				{
					return false;
				}
				return rollCallManager.IsVirtual(sourceAddress.Value);
			}
			if (diagnosisSource == DiagnosisSource.CaesarApi1 || diagnosisSource == DiagnosisSource.McdApi1)
			{
				return false;
			}
			if (IsUds)
			{
				return !Interfaces.Any((EcuInterface ei) => ei.ComParameters.Contains("CP_RESPONSE_CANIDENTIFIER") || ei.ComParameters.Contains("CP_CanRespUSDTId") || ei.ComParameters.Contains("CP_DoIPLogicalEcuAddress"));
			}
			return false;
		}
	}

	public bool IsUds => ProtocolName.StartsWith("UDS", StringComparison.Ordinal);

	internal int? Function => function;

	public string Description => description;

	public string CbfVersion => cBFVersion;

	public string Preamble => preamble;

	public string GpdVersion => gPDVersion;

	public string ProtocolName => protocolName;

	public string DescriptionDataVersion => descriptionDataVersion;

	public string DescriptionFileName => descriptionFileName;

	internal string ExtensionSource => extensionSource;

	internal bool ConfigurationLoadedFromFile => configurationLoadedFromFile;

	public int? ConfigurationFileVersion => configurationFileVersion;

	public string Identifier
	{
		get
		{
			if (SourceAddress.HasValue)
			{
				string arg = protocolName;
				if (IsMcd)
				{
					arg = new string(protocolName.TakeWhile((char c) => c != '_').ToArray());
				}
				return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", arg, SourceAddress);
			}
			if (function.HasValue)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}-F{1}", protocolName, function);
			}
			return identifier;
		}
	}

	public IList<Ecu> ViaEcus => viaEcus.Select((Tuple<Ecu, string> vep) => vep.Item1).ToList().AsReadOnly();

	public DiagnosisVariantCollection DiagnosisVariants => variants;

	public EcuInterfaceCollection Interfaces => interfaces;

	public bool MarkedForAutoConnect
	{
		get
		{
			if (!OfflineSupportOnly)
			{
				return markedForAutoConnect;
			}
			return false;
		}
		set
		{
			markedForAutoConnect = value;
		}
	}

	public bool OfflineSupportOnly { get; private set; }

	public bool ProhibitAutoConnection { get; private set; }

	public Dump SignalNotAvailableValue { get; private set; }

	public bool SupportsDoublePrecisionVariantCoding { get; private set; }

	internal ResponseParameterQualifierSource CaesarEquivalentResponseParameterQualifierSource { get; private set; }

	public int ConnectedChannelCount => connectedChannelCount;

	public StringDictionary Properties => properties;

	internal bool FaultCodeCanBeDuplicate => faultCodeCanBeDuplicate;

	internal bool FaultCodeIsEncodedSpnFmi
	{
		get
		{
			if (!faultCodeIsEncodedSpnFmi.HasValue)
			{
				bool flag = false;
				bool flag2 = false;
				foreach (CompoundEnvironmentData compoundEnvironmentData in CompoundEnvironmentDatas)
				{
					if (compoundEnvironmentData.Referenced.Count == 1)
					{
						if (string.Equals(compoundEnvironmentData.Qualifier, "FaultCodeNumber", StringComparison.Ordinal))
						{
							flag = string.Equals(compoundEnvironmentData.Referenced[0], "UDSCODESPN", StringComparison.Ordinal);
						}
						if (string.Equals(compoundEnvironmentData.Qualifier, "FaultCodeMode", StringComparison.Ordinal))
						{
							flag2 = string.Equals(compoundEnvironmentData.Referenced[0], "UDSCODEFMI", StringComparison.Ordinal);
						}
					}
				}
				faultCodeIsEncodedSpnFmi = flag && flag2;
			}
			return faultCodeIsEncodedSpnFmi.Value;
		}
	}

	internal bool FaultNumberIsFromEnvironmentData => faultNumberIsFromEnvironmentData;

	internal bool FaultCodeNumberAndModeFromEngineeringNotes => faultCodeNumberAndModeFromEngineeringNotes;

	public ListDictionary ProtocolComParameters
	{
		get
		{
			if (diagnosisProtocol == null)
			{
				return null;
			}
			return diagnosisProtocol.ComParameters;
		}
	}

	internal Type ExtensionType
	{
		get
		{
			if (!haveAttemptedExtensionTypeLoad)
			{
				Assembly assembly = (ConfigurationLoadedFromFile ? Sapi.GetSapi().SapiFileSystemExtensionAssembly : Sapi.GetSapi().SapiExtensionAssembly);
				if (assembly != null)
				{
					extensionType = assembly.GetType(string.Format(CultureInfo.InvariantCulture, "SapiExtension.{0}.{1}", DiagnosisSource, Name));
					if (extensionType == null)
					{
						extensionType = assembly.GetType(string.Format(CultureInfo.InvariantCulture, "SapiExtension.{0}", Name));
					}
				}
				else
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, "No extension");
				}
				haveAttemptedExtensionTypeLoad = true;
			}
			return extensionType;
		}
	}

	public IEnumerable<CultureInfo> SupportedCultures
	{
		get
		{
			CultureInfo originalCulture = OriginalCulture.Neutralize();
			yield return originalCulture;
			string translationFileName = GetTranslationFileName(null);
			string[] files = Directory.GetFiles(Path.GetDirectoryName(translationFileName), Path.GetFileName(translationFileName));
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(array[i].Split(".".ToCharArray())[1]).Neutralize();
				if (cultureInfo != originalCulture)
				{
					yield return cultureInfo;
				}
			}
		}
	}

	internal uint? RequestId => requestId;

	internal uint? ResponseId => responseId;

	internal uint? LogicalEcuAddress => logicalEcuAddress;

	internal uint? LogicalTesterAddress => logicalTesterAddress;

	public ChannelOptions AvailableChannelOptions => (ChannelOptions)(5 | (properties.ContainsKey("InitializeService") ? 64 : 0) | ((properties.ContainsKey("CommitToPermanentMemoryService") || properties.ContainsKey("ParameterWriteInitializeService")) ? 128 : 0) | (properties.ContainsKey("MaintainSession") ? 8 : 0) | (PreService.Any() ? 16 : 0) | (AffectServices.Any() ? 32 : 0));

	internal static Ecu CreateFromRollCallLog(string name)
	{
		string[] array = name.Split("-".ToCharArray());
		byte b = byte.Parse(array[1], CultureInfo.InvariantCulture);
		DiagnosisProtocol diagnosisProtocol = Sapi.GetSapi().DiagnosisProtocols[array[0]];
		if (diagnosisProtocol != null)
		{
			return new Ecu(name, b, diagnosisProtocol);
		}
		Protocol protocolId = (Protocol)Enum.Parse(typeof(Protocol), array[0]);
		int? num = null;
		if (array.Length > 2)
		{
			num = int.Parse(array[2], CultureInfo.InvariantCulture);
		}
		return RollCall.GetManager(protocolId).GetEcu(b, num);
	}

	internal Ecu(int sourceAddress, int? function, RollCall rollCallManager)
		: this(sourceAddress, function, null, DiagnosisSource.RollCallDynamic, rollCallManager, null, null, null, null, null)
	{
	}

	internal Ecu(string identifier, byte sourceAddress, DiagnosisProtocol protocol)
		: this(identifier, null, (!protocol.IsMcd) ? DiagnosisSource.CaesarApi1 : DiagnosisSource.McdApi1, null, null, protocol)
	{
		isByteMessaging = true;
		this.sourceAddress = sourceAddress;
		protocolName = protocol.Name;
		this.identifier = identifier;
		rollCallAddressJ1708 = new List<byte>();
		rollCallAddressJ1939 = new List<byte>();
		rollCallFunctionJ1939 = new List<int>();
		rollCallAddressDoIP = new List<int>();
		rollCallIdValues = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
		variants.Add(new DiagnosisVariant(this, "BYTE", string.Empty, null, new List<string>()));
		properties.Add("SupportsFaultRead", "false");
	}

	internal Ecu(int? sourceAddress, int? function, string ecuName, DiagnosisSource source, RollCall rollCallManager, string shortDescription, string category, string family, string supportedEquipment, byte? otherProtocolAddress)
		: this(rollCallManager.Protocol.ToString(), null, source, rollCallManager, ecuName)
	{
		rollCallDescription = shortDescription;
		protocolName = rollCallManager.Protocol.ToString();
		if (rollCallManager.Protocol == Protocol.J1939)
		{
			faultCodeIsEncodedSpnFmi = true;
		}
		this.sourceAddress = sourceAddress;
		this.function = function;
		if (supportedEquipment != null)
		{
			properties.Add("SupportedEquipment", supportedEquipment);
		}
		if (family != null)
		{
			properties.Add("Family", family);
		}
		if (category != null)
		{
			properties.Add("Category", category);
		}
		rollCallFunctionJ1939 = new List<int>();
		rollCallIdValues = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
		rollCallIdValues.Add("J1708", new List<KeyValuePair<string, string>>());
		rollCallIdValues.Add("J1939", new List<KeyValuePair<string, string>>());
		rollCallIdValues.Add("DoIP", new List<KeyValuePair<string, string>>());
		rollCallAddressJ1939 = ((otherProtocolAddress.HasValue && rollCallManager.Protocol == Protocol.J1708) ? new List<byte> { otherProtocolAddress.Value } : new List<byte>());
		rollCallAddressJ1708 = ((otherProtocolAddress.HasValue && rollCallManager.Protocol == Protocol.J1939) ? new List<byte> { otherProtocolAddress.Value } : new List<byte>());
		rollCallAddressDoIP = new List<int>();
	}

	internal Ecu(string baseName, string description, DiagnosisSource source)
		: this(baseName, description, source, null, null)
	{
	}

	private Ecu(string baseName, string description, DiagnosisSource source, RollCall rollCallManager, string rollCallName, DiagnosisProtocol diagnosisProtocol = null)
	{
		diagnosisSource = source;
		this.rollCallName = rollCallName;
		this.diagnosisProtocol = diagnosisProtocol;
		this.rollCallManager = rollCallManager;
		this.baseName = baseName;
		this.description = description;
		cBFVersion = string.Empty;
		preamble = string.Empty;
		gPDVersion = string.Empty;
		protocolName = string.Empty;
		descriptionFileName = string.Empty;
		nameSplit = DefaultNameSplit;
		descriptionDataVersion = string.Empty;
		int ver = 0;
		DateTime dt = DateTime.MinValue;
		variants = new DiagnosisVariantCollection(this);
		interfaces = new EcuInterfaceCollection(this);
		ecuInfoComParameters = new ListDictionary();
		properties = new StringDictionary();
		compoundEnvironmentDatas = new List<CompoundEnvironmentData>();
		makeStoredQualifiers = new List<Regex>();
		makeInstrumentQualifiers = new List<Regex>();
		defaultActionQualifiers = new List<Regex>();
		forceRequestQualifiers = new List<Regex>();
		ignoredQualifiers = new List<Regex>();
		summaryQualifiers = new List<Regex>();
		cacheTimeQualifiers = new Dictionary<string, int>();
		AffectServices = new Dictionary<string, string>();
		PreService = new Dictionary<string, string>();
		AlternateQualifiers = new Dictionary<string, string>();
		viaEcus = new List<Tuple<Ecu, string>>();
		Sapi sapi = Sapi.GetSapi();
		if ((this.rollCallManager == null && this.diagnosisProtocol == null) || this.rollCallName != null)
		{
			Stream stream = null;
			string text = Path.Combine(IsMcd ? McdRoot.DatabaseLocation : sapi.ConfigurationItems["CBFFiles"].Value, string.Format(CultureInfo.InvariantCulture, "{0}.EcuInfo", this.rollCallName ?? this.baseName));
			if (sapi.GetConfiguration != null)
			{
				stream = sapi.GetConfiguration(this, text);
				if (this.rollCallManager == null || stream != null)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(stream);
					xml = Sapi.ReadSapiXmlFile(xmlDocument, "Ecu", out ver, out dt);
					configurationLoadedFromFile = stream is FileStream;
				}
			}
			else if (this.rollCallManager == null || File.Exists(text))
			{
				xml = Sapi.ReadSapiXmlFile(text, "Ecu", out ver, out dt);
				configurationLoadedFromFile = true;
			}
			if (xml != null && ver > 1)
			{
				XmlNodeList xmlNodeList = xml.SelectNodes("Ecu/Properties/Property");
				if (xmlNodeList != null)
				{
					for (int i = 0; i < xmlNodeList.Count; i++)
					{
						XmlNode xmlNode = xmlNodeList[i];
						XmlNode namedItem = xmlNode.Attributes.GetNamedItem("Name");
						properties.Add(namedItem.InnerText, xmlNode.InnerText);
					}
				}
				if (ver > 8)
				{
					XmlNode xmlNode2 = xml.SelectSingleNode("Ecu/Extension");
					if (xmlNode2 != null)
					{
						XmlNode namedItem2 = xmlNode2.Attributes.GetNamedItem("Source");
						extensionSource = Sapi.Decrypt(new Dump(namedItem2.InnerText));
					}
				}
				if (this.rollCallManager == null)
				{
					XmlNodeList xmlNodeList2 = xml.SelectNodes("Ecu/EnvironmentDatas/EnvironmentData");
					if (xmlNodeList2 != null)
					{
						for (int j = 0; j < xmlNodeList2.Count; j++)
						{
							XmlNode environmentDataNode = xmlNodeList2[j];
							compoundEnvironmentDatas.Add(new CompoundEnvironmentData(environmentDataNode));
						}
					}
					if (ver > 5)
					{
						XmlNode xmlNode3 = xml.SelectSingleNode("Ecu/Services");
						if (xmlNode3 != null)
						{
							IEnumerable<XElement> source2 = xmlNode3.ToXElement().Elements("Service");
							foreach (XElement item in source2.Where((XElement xe) => xe.Attribute("Qualifier") != null && xe.Attribute("Affects") != null))
							{
								AffectServices.Add(item.Attribute("Qualifier").Value, item.Attribute("Affects").Value);
							}
							if (ver > 15)
							{
								foreach (XElement item2 in source2.Where((XElement xe) => xe.Attribute("Qualifier") != null && xe.Attribute("PreService") != null))
								{
									PreService.Add(item2.Attribute("Qualifier").Value, item2.Attribute("PreService").Value);
								}
							}
						}
						if (ver > 9)
						{
							XmlNode xmlNode4 = xml.SelectSingleNode("Ecu/DiagServices");
							if (xmlNode4 != null)
							{
								XElement xElement = xmlNode4.ToXElement();
								IEnumerable<XElement> source3 = xElement.Elements("DiagService");
								ecuInfoAttributes = (from element in source3
									let qualifier = element.Attribute("Qualifier").Value
									let variantAttr = element.Attribute("Variants")
									from propertyAttr in element.Attributes()
									where propertyAttr.Name.LocalName != "Qualifier" && propertyAttr.Name.LocalName != "Variants"
									from variant in (variantAttr == null) ? new string[1] : variantAttr.Value.Split(";".ToCharArray())
									orderby qualifier.Length descending
									group new KeyValuePair<string, string>(qualifier, propertyAttr.Value) by Tuple.Create(propertyAttr.Name.LocalName, variant)).ToDictionary((IGrouping<Tuple<string, string>, KeyValuePair<string, string>> group) => group.Key, (IGrouping<Tuple<string, string>, KeyValuePair<string, string>> group) => group.ToList().AsEnumerable());
								foreach (IGrouping<string, XElement> item3 in from xe in source3
									where xe.Attribute("Action") != null
									group xe by xe.Attribute("Action").Value)
								{
									List<Regex> list = item3.Select((XElement xe) => new Regex(xe.Attribute("Qualifier").Value, RegexOptions.Compiled)).ToList();
									switch (item3.Key)
									{
									case "ignore":
										ignoredQualifiers = list;
										break;
									case "makestored":
										makeStoredQualifiers = list;
										break;
									case "makeinstrument":
										makeInstrumentQualifiers = list;
										break;
									case "default":
										defaultActionQualifiers = list;
										break;
									case "forcerequest":
										forceRequestQualifiers = list;
										break;
									}
								}
								if (ver > 10)
								{
									if (xElement.Attribute("NameSplit") != null)
									{
										nameSplit = xElement.Attribute("NameSplit").Value;
									}
									cacheTimeQualifiers = source3.Where((XElement xe) => xe.Attribute("CacheTime") != null).ToDictionary((XElement k) => k.Attribute("Qualifier").Value, (XElement v) => Convert.ToInt32(v.Attribute("CacheTime").Value, CultureInfo.InvariantCulture));
									if (ver > 11)
									{
										summaryQualifiers = (from xe in source3
											where xe.Attribute("Summary") != null && xe.Attribute("Summary").Value == "true"
											select new Regex(xe.Attribute("Qualifier").Value, RegexOptions.Compiled)).ToList();
										if (ver > 12)
										{
											AlternateQualifiers = source3.Where((XElement xe) => xe.Attribute("AlternateQualifier") != null).ToDictionary((XElement k) => k.Attribute("AlternateQualifier").Value, (XElement v) => v.Attribute("Qualifier").Value);
										}
									}
								}
							}
						}
					}
				}
				if (ver > 13)
				{
					XmlNode xmlNode5 = xml.SelectSingleNode("Ecu");
					if (xmlNode5 != null)
					{
						XmlNode namedItem3 = xmlNode5.Attributes.GetNamedItem("ConfigurationVersion");
						configurationFileVersion = Convert.ToInt32(namedItem3.InnerText, CultureInfo.InvariantCulture);
						XmlNode namedItem4 = xmlNode5.Attributes.GetNamedItem("Checksum");
						checksum = namedItem4.InnerText;
						XmlNode namedItem5 = xmlNode5.Attributes.GetNamedItem("TargetApplication");
						if (namedItem5 != null)
						{
							string text2 = Assembly.GetEntryAssembly().GetName().Name.ToString();
							if (string.Compare(namedItem5.InnerText, text2, StringComparison.OrdinalIgnoreCase) != 0)
							{
								Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "WARNING: {0}.EcuInfo Target Application {1} does not match actual application {2}", Name, namedItem5.InnerText, text2));
							}
						}
					}
				}
				XmlNodeList xmlNodeList3 = xml.SelectNodes("Ecu/ComParameters/ComParameter");
				if (xmlNodeList3 != null)
				{
					for (int num = 0; num < xmlNodeList3.Count; num++)
					{
						XmlNode xmlNode6 = xmlNodeList3[num];
						if (xmlNode6 != null)
						{
							XmlNode namedItem6 = xmlNode6.Attributes.GetNamedItem("Name");
							ecuInfoComParameters.Add(namedItem6.InnerText, xmlNode6.InnerText);
						}
					}
				}
			}
			if (this.rollCallManager == null)
			{
				if (properties.ContainsKey("RestrictedPortIndex"))
				{
					int result = 0;
					if (int.TryParse(properties["RestrictedPortIndex"], out result))
					{
						restrictedPortIndex = result;
					}
				}
				if (properties.ContainsKey("RestrictedInterface"))
				{
					restrictedInterface = properties["RestrictedInterface"];
				}
				if (properties.ContainsKey("RestrictedPort"))
				{
					restrictedPort = properties["RestrictedPort"];
				}
				if (properties.ContainsKey("RestrictedInterfaceForViaEcu"))
				{
					restrictedInterfaceForViaEcu = properties["RestrictedInterfaceForViaEcu"];
				}
				if (properties.ContainsKey("RestrictedPortIndexForViaEcu") && int.TryParse(properties["RestrictedPortIndexForViaEcu"], out var result2))
				{
					restrictedPortIndexForViaEcu = result2;
				}
				if (properties.ContainsKey("OfflineSupportOnly"))
				{
					OfflineSupportOnly = Convert.ToBoolean(properties["OfflineSupportOnly"], CultureInfo.InvariantCulture);
				}
				if (properties.ContainsKey("SignalNotAvailableValue"))
				{
					SignalNotAvailableValue = new Dump(properties["SignalNotAvailableValue"]);
				}
				if (properties.ContainsKey("ProhibitAutoConnection"))
				{
					ProhibitAutoConnection = Convert.ToBoolean(properties["ProhibitAutoConnection"], CultureInfo.InvariantCulture);
				}
				if (DiagnosisSource == DiagnosisSource.McdDatabase)
				{
					if (properties.ContainsKey("CaesarEquivalentResponseParameterQualifierSource"))
					{
						CaesarEquivalentResponseParameterQualifierSource = (ResponseParameterQualifierSource)Enum.Parse(typeof(ResponseParameterQualifierSource), properties["CaesarEquivalentResponseParameterQualifierSource"]);
					}
					else
					{
						CaesarEquivalentResponseParameterQualifierSource = ResponseParameterQualifierSource.Name;
					}
					SupportsDoublePrecisionVariantCoding = true;
				}
				else if (properties.ContainsKey("SupportsDoublePrecisionVariantCoding"))
				{
					SupportsDoublePrecisionVariantCoding = Convert.ToBoolean(properties["SupportsDoublePrecisionVariantCoding"], CultureInfo.InvariantCulture);
				}
				rollCallAddressJ1708 = (from a in ParseAddressesFromProperty("J1708SourceAddress")
					select (byte)a).ToList();
				rollCallAddressJ1939 = (from a in ParseAddressesFromProperty("J1939SourceAddress")
					select (byte)a).ToList();
				rollCallFunctionJ1939 = ParseAddressesFromProperty("J1939Function").ToList();
				useRelatedAddresses = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
				foreach (string item4 in from k in properties.Keys.OfType<string>()
					where k.StartsWith("UseRelatedAddresses_", StringComparison.OrdinalIgnoreCase)
					select k)
				{
					useRelatedAddresses.Add(item4.Substring("UseRelatedAddresses_".Length), bool.Parse(properties[item4]));
				}
				rollCallAddressDoIP = ParseAddressesFromProperty("DoIPSourceAddress").ToList();
				rollCallIdValues = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
				rollCallIdValues.Add("J1708", ParseIdValuesFromProperty("J1708IdValues"));
				rollCallIdValues.Add("J1939", ParseIdValuesFromProperty("J1939IdValues"));
				rollCallIdValues.Add("DoIP", ParseIdValuesFromProperty("DoIPIdValues"));
			}
			if (xml != null && Sapi.GetSapi().ValidateConfigurationFileChecksums && GetConfigurationChecksum() != checksum)
			{
				throw new InvalidOperationException(Name + ".EcuInfo checksum is invalid. Check your installation.");
			}
			if (stream != null)
			{
				sapi.ReleaseConfiguration(this, stream, xml);
			}
		}
		CultureInfo culture = Sapi.GetSapi().PresentationCulture;
		string translationFileName = GetTranslationFileName(culture);
		if (translations.TryGetValue(translationFileName, out translation))
		{
			return;
		}
		if (this.rollCallManager != null)
		{
			translations[translationFileName] = this.rollCallManager.Translations;
			return;
		}
		if (IsTranslationNecessary(culture) && !IsTranslationFilePresent(culture))
		{
			culture = OriginalCulture;
		}
		if (IsTranslationFilePresent(culture))
		{
			translation = ReadTranslationFile(culture).Reverse().DistinctBy((TranslationEntry e) => e.Qualifier).ToDictionary((TranslationEntry item) => item.Qualifier);
			translations[translationFileName] = translation;
		}
	}

	private IEnumerable<int> ParseAddressesFromProperty(string property)
	{
		if (!properties.ContainsKey(property))
		{
			yield break;
		}
		string[] array = properties[property].Split(",".ToCharArray());
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i].TryParseSourceAddress(out var num))
			{
				yield return num;
			}
		}
	}

	private IEnumerable<KeyValuePair<string, string>> ParseIdValuesFromProperty(string property)
	{
		if (!properties.ContainsKey(property))
		{
			yield break;
		}
		string[] array = properties[property].Split(";".ToCharArray());
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split("=".ToCharArray());
			if (array2.Length == 2)
			{
				yield return new KeyValuePair<string, string>(array2[0], array2[1]);
			}
		}
	}

	private bool IsRelated(Ecu rollCallEcu)
	{
		switch (rollCallEcu.protocolName)
		{
		case "J1708":
			return rollCallAddressJ1708.Any((byte address) => address == rollCallEcu.SourceAddress);
		case "J1939":
			if (!rollCallFunctionJ1939.Any((int function) => rollCallEcu.Function.HasValue && function == rollCallEcu.Function.Value))
			{
				return rollCallAddressJ1939.Any((byte address) => address == rollCallEcu.SourceAddress);
			}
			return true;
		case "DoIP":
			return rollCallAddressDoIP.Any((int address) => address == rollCallEcu.sourceAddress);
		default:
			return false;
		}
	}

	internal bool IsRelated(Protocol protocol)
	{
		switch (protocol)
		{
		case Protocol.J1708:
			return rollCallAddressJ1708.Any();
		case Protocol.J1939:
			if (!rollCallAddressJ1939.Any())
			{
				return rollCallFunctionJ1939.Any();
			}
			return true;
		case Protocol.DoIP:
			return rollCallAddressDoIP.Any();
		default:
			return false;
		}
	}

	internal bool IsRelated(RollCall manager, int sourceAddress, Ecu allowedByViaEcu)
	{
		bool flag = false;
		switch (manager.Protocol)
		{
		case Protocol.J1708:
			flag = rollCallAddressJ1708.Any((byte address) => address == sourceAddress);
			break;
		case Protocol.J1939:
			flag = rollCallFunctionJ1939.Any((int function) => IdValueMatch(manager, sourceAddress, new KeyValuePair<string, string>(RollCall.ID.Function.ToNumberString(), function.ToString(CultureInfo.InvariantCulture)))) || rollCallAddressJ1939.Any((byte address) => address == sourceAddress);
			break;
		case Protocol.DoIP:
			flag = rollCallAddressDoIP.Any((int address) => address == sourceAddress);
			break;
		}
		if (flag)
		{
			IEnumerable<KeyValuePair<string, string>> source = GetRollCallIdValues(manager.Protocol, allowedByViaEcu);
			if (!source.Any())
			{
				return true;
			}
			return source.Any((KeyValuePair<string, string> idValue) => IdValueMatch(manager, sourceAddress, idValue));
		}
		return false;
	}

	internal bool IsRelated(Channel rollCallChannel, Ecu allowedByViaEcu)
	{
		bool flag = false;
		switch (rollCallChannel.Ecu.RollCallManager.Protocol)
		{
		case Protocol.J1708:
			flag = rollCallAddressJ1708.Any((byte address) => address == rollCallChannel.SourceAddress.Value);
			break;
		case Protocol.J1939:
			flag = rollCallFunctionJ1939.Any((int function) => IdValueMatch(rollCallChannel, new KeyValuePair<string, string>(RollCall.ID.Function.ToNumberString(), function.ToString(CultureInfo.InvariantCulture)))) || rollCallFunctionJ1939.Any((int function) => rollCallChannel.Ecu.Function.HasValue && function == rollCallChannel.Ecu.Function.Value) || rollCallAddressJ1939.Any((byte address) => rollCallChannel.SourceAddress.HasValue && address == rollCallChannel.SourceAddress.Value);
			break;
		case Protocol.DoIP:
			flag = rollCallAddressDoIP.Any((int address) => address == rollCallChannel.SourceAddressLong.Value);
			break;
		}
		if (flag)
		{
			IEnumerable<KeyValuePair<string, string>> source = GetRollCallIdValues(rollCallChannel.Ecu.RollCallManager.Protocol, allowedByViaEcu);
			if (!source.Any())
			{
				return true;
			}
			return source.Any((KeyValuePair<string, string> idValue) => IdValueMatch(rollCallChannel, idValue));
		}
		return false;
	}

	private IEnumerable<KeyValuePair<string, string>> GetRollCallIdValues(Protocol protocol, Ecu allowedByViaEcu)
	{
		string text = protocol.ToString();
		if (allowedByViaEcu != null && rollCallIdValues.ContainsKey(text + allowedByViaEcu.Name))
		{
			return rollCallIdValues[text + allowedByViaEcu.Name];
		}
		return rollCallIdValues[text];
	}

	private static bool IdValueMatch(RollCall manager, int sourceAddress, KeyValuePair<string, string> idValue)
	{
		object identificationValue = manager.GetIdentificationValue(sourceAddress, idValue.Key);
		if (identificationValue != null && identificationValue.ToString() == idValue.Value)
		{
			return true;
		}
		return false;
	}

	private static bool IdValueMatch(Channel rollCallChannel, KeyValuePair<string, string> idValue)
	{
		EcuInfo ecuInfo = rollCallChannel.EcuInfos[idValue.Key];
		if (ecuInfo != null && ecuInfo.EcuInfoValues.Current != null)
		{
			object value = ecuInfo.EcuInfoValues.Current.Value;
			Choice choice = value as Choice;
			if ((choice != null && choice.RawValue.ToString() == idValue.Value) || (value != null && value.ToString() == idValue.Value))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasRelatedAddressesWhenViaChannel(Channel viaChannel)
	{
		if (viaChannel != null && useRelatedAddresses != null)
		{
			foreach (string item in useRelatedAddresses.Keys.Where((string k) => k.StartsWith(viaChannel.Ecu.Name, StringComparison.OrdinalIgnoreCase) && k.Contains(',')))
			{
				string value = item.Split(",".ToCharArray())[1];
				if (viaChannel.DiagnosisVariant.Name.StartsWith(value, StringComparison.OrdinalIgnoreCase) && !useRelatedAddresses[item])
				{
					return false;
				}
			}
			if (useRelatedAddresses.ContainsKey(viaChannel.Ecu.Name) && !useRelatedAddresses[viaChannel.Ecu.Name])
			{
				return false;
			}
			return HasRelatedAddressesWhenViaChannel(viaChannel.ActualViaChannel);
		}
		return HasRelatedAddresses;
	}

	public bool IsSameEcuOnDifferentDiagnosisSource(Ecu compareEcu)
	{
		if (compareEcu.baseName == baseName)
		{
			return compareEcu.DiagnosisSource != DiagnosisSource;
		}
		return false;
	}

	internal void AcquireFromRollCall(string descriptionVersion, IEnumerable<DiagnosisVariant> variants)
	{
		descriptionDataVersion = descriptionVersion ?? string.Empty;
		if (variants != null)
		{
			foreach (DiagnosisVariant variant in variants)
			{
				this.variants.Add(variant);
			}
			properties.Add("CurrentDiagnosisVariant", this.variants.Last().Name);
		}
		if (IsRollCallBaseEcu && !this.variants.Any((DiagnosisVariant v) => v.IsBase))
		{
			this.variants.Add(new DiagnosisVariant(this, "ROLLCALL", string.Empty, null, new List<string>()));
		}
		string text = (from e in this.variants
			group e by e.Name into g
			where g.Count() > 1
			select g.Key).FirstOrDefault();
		if (text != null)
		{
			throw new InvalidOperationException("Variant names must be unique. The Ecu '" + Name + "' has a duplicate variant '" + text + "'.");
		}
	}

	internal void AcquireInfoMCD()
	{
		McdDBEcuBaseVariant mcdHandle = GetMcdHandle();
		preamble = mcdHandle.Preamble;
		description = mcdHandle.Description;
		descriptionFileName = mcdHandle.DatabaseFile;
		descriptionDataVersion = McdRoot.GetDatabaseFileVersion(descriptionFileName);
		ValidateDescriptionDataVersion();
		identifier = Name;
		interfaces.AcquireList();
		EcuInterface ecuInterface = interfaces.Where((EcuInterface i) => !McdRoot.LocationRestricted.Contains(i.ProtocolName)).FirstOrDefault();
		protocolName = ecuInterface.ProtocolName;
		diagnosisProtocol = Sapi.GetSapi().DiagnosisProtocols[protocolName];
		EcuInterface ecuInterface2 = interfaces.FirstOrDefault((EcuInterface i) => i.ProtocolType == "ISO_15765_3_on_ISO_15765_2");
		if (ecuInterface2 != null)
		{
			requestId = Convert.ToUInt32(ecuInterface2.PrioritizedComParameterValue("CP_CanPhysReqId"), CultureInfo.InvariantCulture);
			responseId = Convert.ToUInt32(ecuInterface2.PrioritizedComParameterValue("CP_CanRespUSDTId"), CultureInfo.InvariantCulture);
			sourceAddress = BitConverter.GetBytes(responseId.Value)[0];
		}
		ecuInterface2 = interfaces.FirstOrDefault((EcuInterface i) => i.ProtocolType == "ISO_14229_5_on_ISO_13400_2");
		if (ecuInterface2 != null)
		{
			logicalTesterAddress = Convert.ToUInt32(ecuInterface2.PrioritizedComParameterValue("CP_DoIPLogicalTesterAddress"), CultureInfo.InvariantCulture);
			logicalEcuAddress = Convert.ToUInt32(ecuInterface2.PrioritizedComParameterValue("CP_DoIPLogicalEcuAddress"), CultureInfo.InvariantCulture);
			if (!sourceAddress.HasValue || sourceAddress.Value == BitConverter.GetBytes(logicalEcuAddress.Value)[0])
			{
				sourceAddress = (int)logicalEcuAddress.Value;
			}
		}
		AcquireVariants(mcdHandle);
		SetGenericProperties();
	}

	internal McdDBEcuBaseVariant GetMcdHandle()
	{
		return McdRoot.GetDBEcuBaseVariant(baseName);
	}

	internal void AcquireInfo()
	{
		CaesarEcu val = OpenEcuHandle();
		try
		{
			if (val != null)
			{
				description = val.Description;
				cBFVersion = val.CbfVersion;
				preamble = val.Preamble;
				gPDVersion = val.GpdVersion;
				descriptionDataVersion = val.DescriptionDataVersion;
				ValidateDescriptionDataVersion();
				protocolName = val.ProtocolName;
				diagnosisProtocol = Sapi.GetSapi().DiagnosisProtocols[protocolName];
				descriptionFileName = val.FileName;
				AcquireVariants(val);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		interfaces.AcquireList();
		identifier = Name;
		if (interfaces.Count > 0)
		{
			EcuInterface ecuInterface = interfaces[0];
			ListDictionary comParameters = ecuInterface.ComParameters;
			if (IsUds)
			{
				if (comParameters.Contains("CP_RESPONSE_CANIDENTIFIER"))
				{
					requestId = Convert.ToUInt32(ecuInterface.PrioritizedComParameterValue("CP_REQUEST_CANIDENTIFIER"), CultureInfo.InvariantCulture);
					responseId = Convert.ToUInt32(ecuInterface.PrioritizedComParameterValue("CP_RESPONSE_CANIDENTIFIER"), CultureInfo.InvariantCulture);
					sourceAddress = BitConverter.GetBytes(responseId.Value)[0];
				}
			}
			else if (string.Equals(protocolName, "E1708", StringComparison.Ordinal))
			{
				sourceAddress = Convert.ToByte(comParameters["CP_RESPSOURCEMID"], CultureInfo.InvariantCulture);
			}
			else if (string.Equals(protocolName, "E1939", StringComparison.Ordinal) && comParameters.Contains("CP_DESTADDRESS"))
			{
				sourceAddress = Convert.ToByte(comParameters["CP_DESTADDRESS"], CultureInfo.InvariantCulture);
			}
		}
		if (Properties.ContainsKey("HasFaultCodeNumberAndMode") && !bool.TryParse(Properties["HasFaultCodeNumberAndMode"], out faultNumberIsFromEnvironmentData))
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to parse HasFaultCodeNumberAndMode property");
		}
		if (!FaultCodeIsEncodedSpnFmi)
		{
			faultCodeNumberAndModeFromEngineeringNotes = properties.GetValue("FaultCodeNumberAndModeFromEngineeringNotes", defaultIfNotSet: false);
			if (!faultCodeNumberAndModeFromEngineeringNotes && (string.Equals(ProtocolName, "E1708") || string.Equals(ProtocolName, "E1939")))
			{
				faultCodeCanBeDuplicate = true;
			}
		}
		if (Properties.ContainsKey("HasMultiByteDiagnosticVersion") && !bool.TryParse(Properties["HasMultiByteDiagnosticVersion"], out hasMultiByteDiagnosticVersion))
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to parse HasMultiByteDiagnosticVersion property");
		}
		SetGenericProperties();
	}

	private void SetGenericProperties()
	{
		if (IsUds && sourceAddress.HasValue)
		{
			if (!Properties.ContainsKey("ShortDescription") && RollCallJ1939.GlobalInstance.Translations != null && RollCallJ1939.GlobalInstance.Translations.TryGetValue(Sapi.MakeTranslationIdentifier(sourceAddress.Value.ToString(CultureInfo.InvariantCulture), "Source"), out var value))
			{
				Properties["ShortDescription"] = value.Translation;
			}
			if (!Properties.ContainsKey("Category"))
			{
				Properties["Category"] = ((!IsPowertrainDevice) ? "Vehicle" : ((sourceAddress.Value == 3) ? "Transmission" : "Engine"));
			}
		}
	}

	private void ValidateDescriptionDataVersion()
	{
		if (properties.ContainsKey("MinimumDescriptionDataVersion") && Version.TryParse(properties["MinimumDescriptionDataVersion"], out var result) && Version.TryParse(descriptionDataVersion, out var result2) && result2 < result)
		{
			throw new InvalidOperationException("Diagnosis description version for " + Name + " is too low");
		}
	}

	internal string Translate(string qualifier, string original)
	{
		if (translation != null && translation.TryGetValue(qualifier, out var value))
		{
			return value.Translation;
		}
		return original;
	}

	internal void IncrementConnectedChannelCount()
	{
		Interlocked.Increment(ref connectedChannelCount);
	}

	internal void DecrementConnectedChannelCount()
	{
		Interlocked.Decrement(ref connectedChannelCount);
	}

	internal CaesarEcu OpenEcuHandle()
	{
		return OpenEcuHandle(Name);
	}

	internal static CaesarEcu OpenEcuHandle(string name)
	{
		CaesarEcu val = null;
		uint availableInterfaceTypeCount = CaesarRoot.GetAvailableInterfaceTypeCount(name);
		for (uint num = 0u; num < availableInterfaceTypeCount; num++)
		{
			if (val != null)
			{
				break;
			}
			try
			{
				CaesarEcuInterface interfaceByIndex = CaesarRoot.GetInterfaceByIndex(name, num);
				try
				{
					if (interfaceByIndex != null)
					{
						val = interfaceByIndex.OpenEcu(name);
					}
				}
				finally
				{
					((IDisposable)interfaceByIndex)?.Dispose();
				}
			}
			catch (CaesarErrorException)
			{
			}
		}
		return val;
	}

	internal DiagnosisVariant GetDiagnosisVariantFromIDBlock(CaesarIdBlock idBlock)
	{
		DiagnosisVariant result = null;
		CaesarEcu val = OpenEcuHandle();
		try
		{
			if (val != null && val.SetVariantByIdBlock(idBlock))
			{
				string variantName = val.VariantName;
				if (!string.IsNullOrEmpty(variantName))
				{
					result = variants[variantName];
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return result;
	}

	internal void AddViaEcu(Ecu viaEcu, string variantPrefix)
	{
		viaEcus.Add(new Tuple<Ecu, string>(viaEcu, variantPrefix));
		if (rollCallIdValues == null)
		{
			return;
		}
		string[] array = new string[3] { "J1708", "J1939", "DoIP" };
		foreach (string text in array)
		{
			IEnumerable<KeyValuePair<string, string>> enumerable = ParseIdValuesFromProperty(text + "IdValues_" + viaEcu.Name);
			if (enumerable.Any())
			{
				rollCallIdValues.Add(text + viaEcu.Name, enumerable);
			}
		}
	}

	internal bool PassesConnectionResourceFilter(ConnectionResource resource)
	{
		if (restrictedPortIndex.HasValue && !string.Equals(resource.Type, "ETHERNET") && resource.PortIndex != restrictedPortIndex.Value)
		{
			return false;
		}
		if (restrictedInterface != null && !string.Equals(resource.Interface.Qualifier, restrictedInterface, StringComparison.Ordinal))
		{
			return false;
		}
		if (restrictedPort == "ViaEcu")
		{
			bool validatedByExtension;
			Channel currentViaEcuChannel = GetCurrentViaEcuChannel(out validatedByExtension);
			if (currentViaEcuChannel != null)
			{
				if (currentViaEcuChannel.Ecu.restrictedPortIndexForViaEcu.HasValue || currentViaEcuChannel.Ecu.restrictedInterfaceForViaEcu != null)
				{
					if (currentViaEcuChannel.Ecu.restrictedPortIndexForViaEcu.HasValue && !string.Equals(resource.Type, "ETHERNET") && resource.PortIndex != currentViaEcuChannel.Ecu.restrictedPortIndexForViaEcu.Value)
					{
						return false;
					}
					if (currentViaEcuChannel.Ecu.restrictedInterfaceForViaEcu != null && !string.Equals(resource.Interface.Qualifier, currentViaEcuChannel.Ecu.restrictedInterfaceForViaEcu, StringComparison.Ordinal))
					{
						return false;
					}
					return true;
				}
				return currentViaEcuChannel.ConnectionResource.IsEquivalent(resource);
			}
			return false;
		}
		return true;
	}

	internal Channel GetCurrentViaEcuChannel(out bool validatedByExtension)
	{
		validatedByExtension = false;
		foreach (Tuple<Ecu, string> viaEcu in viaEcus)
		{
			Channel channel = Sapi.GetSapi().Channels.FirstOrDefault((Channel c) => c.Ecu == viaEcu.Item1 && c.ConnectionResource != null);
			if (channel == null)
			{
				continue;
			}
			string[] array = ((viaEcu.Item2 != null) ? viaEcu.Item2.Split('+') : new string[0]);
			foreach (string text in array)
			{
				if (text.StartsWith("extension:", StringComparison.Ordinal))
				{
					string[] array2 = text.Substring(10).Split(new char[3] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
					if (!(bool)channel.Extension.Invoke(array2[0], array2.Skip(1).ToArray()))
					{
						return null;
					}
					validatedByExtension = true;
				}
				else if (!channel.DiagnosisVariant.Name.StartsWith(text, StringComparison.Ordinal))
				{
					return null;
				}
			}
			return channel;
		}
		return null;
	}

	internal int GetComParameter(string name, int defaultValue)
	{
		if (!EcuInfoComParameters.Contains(name))
		{
			return defaultValue;
		}
		return Convert.ToInt32(EcuInfoComParameters[name], CultureInfo.InvariantCulture);
	}

	internal bool IgnoreQualifier(string qualifier)
	{
		if (IsQualifierInList(ignoredQualifiers, qualifier))
		{
			return !IsQualifierInList(defaultActionQualifiers, qualifier);
		}
		return false;
	}

	internal bool MakeStoredQualifier(string qualifier)
	{
		if (IsQualifierInList(makeStoredQualifiers, qualifier))
		{
			return !IsQualifierInList(defaultActionQualifiers, qualifier);
		}
		return false;
	}

	internal bool MakeInstrumentQualifier(string qualifier)
	{
		if (IsQualifierInList(makeInstrumentQualifiers, qualifier))
		{
			return !IsQualifierInList(defaultActionQualifiers, qualifier);
		}
		return false;
	}

	internal bool ForceRequestQualifier(string qualifier)
	{
		return IsQualifierInList(forceRequestQualifiers, qualifier);
	}

	internal object CacheTimeQualifier(string qualifier)
	{
		foreach (KeyValuePair<string, int> cacheTimeQualifier in cacheTimeQualifiers)
		{
			if (qualifier.StartsWith(cacheTimeQualifier.Key, StringComparison.Ordinal))
			{
				return cacheTimeQualifier.Value;
			}
		}
		return null;
	}

	internal T GetEcuInfoAttribute<T>(string attribute, string qualifier, string variantName)
	{
		string text = null;
		if (ecuInfoAttributes != null)
		{
			if (ecuInfoAttributes.TryGetValue(Tuple.Create(attribute, variantName), out var value))
			{
				text = (from kv in value
					where qualifier.StartsWith(kv.Key, StringComparison.Ordinal)
					select kv.Value).FirstOrDefault();
			}
			if (text == null && ecuInfoAttributes.TryGetValue(Tuple.Create<string, string>(attribute, null), out value))
			{
				text = (from kv in value
					where qualifier.StartsWith(kv.Key, StringComparison.Ordinal)
					select kv.Value).FirstOrDefault();
			}
		}
		Type typeFromHandle = typeof(T);
		if (text == null)
		{
			return default(T);
		}
		return (T)Convert.ChangeType(text, Nullable.GetUnderlyingType(typeFromHandle) ?? typeFromHandle, CultureInfo.InvariantCulture);
	}

	internal bool SummaryQualifier(string qualifier)
	{
		return IsQualifierInList(summaryQualifiers, qualifier);
	}

	internal string ShortName(string name)
	{
		int num = name.IndexOf(NameSplit, StringComparison.Ordinal);
		if (num != -1)
		{
			return name.Substring(num + NameSplit.Length);
		}
		return name;
	}

	public ConnectionResourceCollection GetConnectionResources()
	{
		return GetConnectionResources(null);
	}

	internal ConnectionResourceCollection GetConnectionResources(byte? sourceAddress)
	{
		switch (Sapi.GetSapi().InitState)
		{
		case InitState.Online:
		{
			ConnectionResourceCollection connectionResourceCollection = new ConnectionResourceCollection();
			if (IsMcd)
			{
				PopulateMcdConnectionResources(connectionResourceCollection);
			}
			else
			{
				PopulateCaesarConnectionResources(connectionResourceCollection, sourceAddress);
			}
			return connectionResourceCollection;
		}
		case InitState.Offline:
			throw new InvalidOperationException("Resource cannot be acquired in offline operation mode");
		case InitState.NotInitialized:
			throw new InvalidOperationException("Sapi not initialized");
		default:
			return null;
		}
	}

	private void PopulateCaesarConnectionResources(ConnectionResourceCollection connectionResources, byte? sourceAddress)
	{
		//IL_0039: Expected O, but got Unknown
		//IL_005e: Expected O, but got Unknown
		//IL_0088: Expected O, but got Unknown
		if (interfaces.Count <= 0)
		{
			return;
		}
		lock (resourceLock)
		{
			try
			{
				CaesarRoot.LockResources();
			}
			catch (CaesarErrorException ex)
			{
				throw new CaesarException(ex, null, null);
			}
			uint availableEcuResourceCount;
			try
			{
				availableEcuResourceCount = CaesarRoot.GetAvailableEcuResourceCount(baseName);
			}
			catch (CaesarErrorException ex2)
			{
				CaesarRoot.UnlockResources();
				throw new CaesarException(ex2, null, null);
			}
			for (ushort num = 0; num < availableEcuResourceCount; num++)
			{
				CaesarResource availableEcuResource;
				try
				{
					availableEcuResource = CaesarRoot.GetAvailableEcuResource(baseName, num);
				}
				catch (CaesarErrorException ex3)
				{
					CaesarRoot.UnlockResources();
					throw new CaesarException(ex3, null, null);
				}
				ConnectionResource connectionResource = new ConnectionResource(this, availableEcuResource, sourceAddress);
				ConnectionResource equivalent = connectionResources.GetEquivalent(connectionResource);
				if (equivalent == null)
				{
					connectionResources.Add(connectionResource);
				}
				else if (equivalent.Restricted && !connectionResource.Restricted)
				{
					equivalent.Restricted = false;
				}
			}
			CaesarRoot.UnlockResources();
		}
	}

	private void PopulateMcdConnectionResources(ConnectionResourceCollection connectionResources)
	{
		foreach (McdInterface currentInterface in McdRoot.CurrentInterfaces)
		{
			int num = 1;
			IEnumerable<McdInterfaceResource> enumerable;
			if (currentInterface.Resources.All((McdInterfaceResource r) => r.PhysicalInterfaceLinkType == "ETHERNET"))
			{
				enumerable = currentInterface.Resources.Take(1);
				num = 0;
			}
			else
			{
				enumerable = currentInterface.Resources;
			}
			foreach (McdInterfaceResource theInterfaceResource in enumerable)
			{
				IEnumerable<EcuInterface> enumerable2 = interfaces.Where((EcuInterface i) => i.ProtocolType == theInterfaceResource.ProtocolType);
				if (!enumerable2.Any())
				{
					continue;
				}
				bool flag = false;
				foreach (EcuInterface item in enumerable2)
				{
					ConnectionResource connectionResource = new ConnectionResource(this, item, currentInterface, theInterfaceResource, num);
					connectionResources.Add(connectionResource);
					if (!connectionResource.Restricted)
					{
						if (!flag)
						{
							flag = true;
						}
						else
						{
							connectionResource.Restricted = true;
						}
					}
				}
				num++;
			}
		}
	}

	private string GetConfigurationChecksum()
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(xml.SelectSingleNode("Ecu").InnerXml));
		return BitConverter.ToString(mD5CryptoServiceProvider.Hash);
	}

	public override string ToString()
	{
		return Name;
	}

	public Dictionary<string, string> GetTranslatedStringsForTranslation()
	{
		return GetTranslatedStringsForTranslation(null, null);
	}

	public Dictionary<string, string> GetTranslatedStringsForTranslation(int? maxAccessLevel, string[] forbiddenQualifierPrefixes)
	{
		return (from c in GetStringsForTranslation(maxAccessLevel, forbiddenQualifierPrefixes)
			select new KeyValuePair<string, string>(c.Key, Translate(c.Key, c.Value))).ToDictionary((KeyValuePair<string, string> k) => k.Key, (KeyValuePair<string, string> v) => v.Value);
	}

	public Dictionary<string, string> GetStringsForTranslation()
	{
		return GetStringsForTranslation(null, null);
	}

	public Dictionary<string, string> GetStringsForTranslation(int? maxAccessLevel, string[] forbiddenQualifierPrefixes)
	{
		translation = null;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (DiagnosisVariant diagnosisVariant in DiagnosisVariants)
		{
			using Channel channel = Sapi.GetSapi().Channels.OpenOffline(diagnosisVariant);
			foreach (Instrument instrument in channel.Instruments)
			{
				if ((!maxAccessLevel.HasValue || instrument.AccessLevel <= maxAccessLevel.Value) && (forbiddenQualifierPrefixes == null || forbiddenQualifierPrefixes.All((string p) => !instrument.Qualifier.StartsWith(p, StringComparison.Ordinal))))
				{
					instrument.AddStringsForTranslation(dictionary);
				}
			}
			foreach (EcuInfo ecuInfo in channel.EcuInfos)
			{
				if ((!maxAccessLevel.HasValue || ecuInfo.AccessLevel <= maxAccessLevel.Value) && (forbiddenQualifierPrefixes == null || forbiddenQualifierPrefixes.All((string p) => !ecuInfo.Qualifier.StartsWith(p, StringComparison.Ordinal))))
				{
					ecuInfo.AddStringsForTranslation(dictionary);
				}
			}
			foreach (Service service in channel.Services)
			{
				if ((!maxAccessLevel.HasValue || service.Access <= maxAccessLevel.Value) && (forbiddenQualifierPrefixes == null || forbiddenQualifierPrefixes.All((string p) => !service.Qualifier.StartsWith(p, StringComparison.Ordinal))))
				{
					service.AddStringsForTranslation(dictionary);
				}
			}
			foreach (FaultCode faultCode in channel.FaultCodes)
			{
				faultCode.AddStringsForTranslation(dictionary, maxAccessLevel);
			}
			foreach (Service environmentDataDescription in channel.FaultCodes.EnvironmentDataDescriptions)
			{
				environmentDataDescription.AddStringsForTranslation(dictionary);
			}
			foreach (Parameter parameter in channel.Parameters)
			{
				if ((!maxAccessLevel.HasValue || parameter.ReadAccess <= maxAccessLevel.Value) && (forbiddenQualifierPrefixes == null || forbiddenQualifierPrefixes.All((string p) => !parameter.Qualifier.StartsWith(p, StringComparison.Ordinal) && !parameter.CombinedQualifier.StartsWith(p, StringComparison.Ordinal))))
				{
					parameter.AddStringsForTranslation(dictionary);
				}
			}
			foreach (CodingParameterGroup codingParameterGroup in channel.CodingParameterGroups)
			{
				foreach (CodingChoice choice in codingParameterGroup.Choices)
				{
					choice.AddStringsForTranslation(dictionary);
				}
				foreach (CodingParameter parameter2 in codingParameterGroup.Parameters)
				{
					foreach (CodingChoice choice2 in parameter2.Choices)
					{
						choice2.AddStringsForTranslation(dictionary);
					}
				}
			}
			channel.Disconnect();
		}
		string translationFileName = GetTranslationFileName(Sapi.GetSapi().PresentationCulture);
		if (translations.ContainsKey(translationFileName))
		{
			translation = translations[translationFileName];
		}
		return dictionary;
	}

	public bool IsTranslationFilePresent(CultureInfo culture)
	{
		return File.Exists(GetTranslationFileName(culture));
	}

	public bool IsTranslationNecessary(CultureInfo culture)
	{
		return !OriginalCulture.Neutralize().Name.Equals(culture.Neutralize().Name);
	}

	private string GetTranslationFileName(CultureInfo culture)
	{
		return TranslationEntry.GetTranslationFileName(baseName, culture);
	}

	public IEnumerable<TranslationEntry> ReadTranslationFile(CultureInfo culture)
	{
		return TranslationEntry.ReadTranslationFile(baseName, culture);
	}

	public void WriteTranslationFile(CultureInfo culture, IEnumerable<TranslationEntry> translations, bool emitEmptyTranslations)
	{
		TranslationEntry.WriteTranslationFile(baseName, culture, DescriptionDataVersion, translations, emitEmptyTranslations);
	}

	private void AcquireVariants(CaesarEcu ecu)
	{
		StringCollection stringCollection = ecu.Variants;
		for (int i = 0; i < stringCollection.Count; i++)
		{
			string text = stringCollection[i];
			if (ecu.SetVariant(text))
			{
				Part partNumber = null;
				string partNumber2 = ecu.PartNumber;
				if (partNumber2 != null)
				{
					object partVersion = ecu.PartVersion;
					partNumber = ((partVersion == null) ? new Part(partNumber2) : new Part(partNumber2, partVersion));
				}
				DiagnosisVariant diagnosisVariant = new DiagnosisVariant(this, text, ecu.VariantDescription, partNumber, ecu.GetVariantIdBlocks(text));
				variants.Add(diagnosisVariant);
			}
		}
	}

	private void AcquireVariants(McdDBEcuBaseVariant ecu)
	{
		List<string> source = interfaces.Select((EcuInterface i) => i.ProtocolName).ToList();
		List<Tuple<string, string, IEnumerable<string>, IEnumerable<McdDBLocation>, bool>> list = new List<Tuple<string, string, IEnumerable<string>, IEnumerable<McdDBLocation>, bool>>();
		list.Add(Tuple.Create("_base_", string.Empty, ecu.DBLocationNames, source.Select((string l) => ecu.GetDBLocationForProtocol(l)), item5: true));
		foreach (string dBEcuVariantName in ecu.DBEcuVariantNames)
		{
			McdDBEcuVariant variant = ecu.GetDBEcuVariant(dBEcuVariantName);
			list.Add(Tuple.Create(dBEcuVariantName, variant.Description, variant.DBLocationNames, source.Select((string l) => variant.GetDBLocationForProtocol(l)), item5: false));
		}
		foreach (Tuple<string, string, IEnumerable<string>, IEnumerable<McdDBLocation>, bool> item in list)
		{
			List<McdDBLocation> source2 = item.Item4.Where((McdDBLocation l) => l != null).ToList();
			string text = source2.Select((McdDBLocation l) => l.PartNumber).FirstOrDefault((string pn) => !string.IsNullOrEmpty(pn));
			McdDBLocation mcdDBLocation = source2.FirstOrDefault((McdDBLocation l) => l.VariantAttributes != null && l.VariantAttributes.Any());
			DiagnosisVariant diagnosisVariant = new DiagnosisVariant(this, item.Item1, item.Item2, (!string.IsNullOrEmpty(text)) ? new Part(text) : null, mcdDBLocation?.VariantAttributes, source2.SelectMany((McdDBLocation l) => l.DBVariantPatterns), item.Item5);
			diagnosisVariant.Locations = item.Item3;
			variants.Add(diagnosisVariant);
		}
	}

	private static bool IsQualifierInList(List<Regex> qualifiers, string qualifier)
	{
		foreach (Regex qualifier2 in qualifiers)
		{
			if (qualifier2.Match(qualifier).Success)
			{
				return true;
			}
		}
		return false;
	}

	internal TextFieldParser GetDefParser()
	{
		string value = Properties.GetValue<string>("VcpDefPath", null);
		if (value != null)
		{
			value = Environment.ExpandEnvironmentVariables(value);
			if (File.Exists(value))
			{
				TextFieldParser textFieldParser = new TextFieldParser(value);
				textFieldParser.SetDelimiters(",");
				textFieldParser.HasFieldsEnclosedInQuotes = true;
				return textFieldParser;
			}
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Missing VCP .DEF file " + value);
		}
		return null;
	}

	public bool RemoveDescriptionFile()
	{
		//IL_006d: Expected O, but got Unknown
		if (diagnosisSource == DiagnosisSource.CaesarDatabase && Sapi.GetSapi().Ecus.GetConnectedCountForIdentifier(Identifier) == 0)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Removing CBF: {0}", DescriptionFileName));
			try
			{
				CaesarRoot.RemoveCbfFile(DescriptionFileName);
				Sapi.GetSapi().Ecus.Remove(this);
				return true;
			}
			catch (CaesarErrorException ex)
			{
				throw new CaesarException(ex, null, null);
			}
		}
		return false;
	}
}
