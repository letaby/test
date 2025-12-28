using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FaultCode : IDiogenesDataItem
{
	private const string FaultCodeNumber = "FaultCodeNumber";

	private const string FaultCodeMode = "FaultCodeMode";

	internal static string[] isoAreas = new string[4] { "P", "C", "B", "U" };

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

	internal uint LongNumber
	{
		get
		{
			return longNumber;
		}
		set
		{
			longNumber = value;
		}
	}

	public Channel Channel => channel;

	public string Code => code;

	public string Text
	{
		get
		{
			string result;
			if (channel.Ecu.RollCallManager != null)
			{
				result = channel.Ecu.RollCallManager.GetFaultText(channel, Number, Mode);
			}
			else
			{
				result = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(Code, "Text"), text);
				if (channel.Ecu.FaultCodeCanBeDuplicate)
				{
					if (modeText == null)
					{
						FaultCodeIncident faultCodeIncident = faultCodeIncidents.Where((FaultCodeIncident fci) => fci.EnvironmentDatas.Count > 0).FirstOrDefault();
						if (faultCodeIncident != null)
						{
							EnvironmentData environmentData = faultCodeIncident.EnvironmentDatas.FirstOrDefault((EnvironmentData ed) => ed.Value.ToString().StartsWith("FMI", StringComparison.Ordinal));
							if (environmentData != null)
							{
								modeText = environmentData.Value.ToString().Split(":".ToCharArray())[1];
							}
						}
					}
					if (modeText != null)
					{
						result = text + modeText;
					}
				}
			}
			return result;
		}
	}

	public FaultCodeIncidentCollection FaultCodeIncidents => faultCodeIncidents;

	public ServiceCollection EnvironmentDataDescriptions => environmentDataDescriptions;

	public FaultCodeIncidentCollection Snapshots => snapshots;

	public string Number
	{
		get
		{
			if (string.IsNullOrEmpty(number))
			{
				number = GetNumberOrMode(mode: false);
			}
			return number;
		}
	}

	public string Mode
	{
		get
		{
			if (string.IsNullOrEmpty(mode))
			{
				mode = GetNumberOrMode(mode: true);
			}
			return mode;
		}
	}

	public FaultCodeIncident ManipulatedValue
	{
		get
		{
			return manipulatedValue;
		}
		private set
		{
			manipulatedValue = value;
			channel.SetManipulatedState(Code, manipulatedValue != null);
		}
	}

	public string Name
	{
		get
		{
			if (!string.IsNullOrEmpty(Mode))
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} ({1}/{2})", Text, Number, Mode);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", Text, Code);
		}
	}

	public string ShortName => Name;

	public string Qualifier => Code;

	public string Description => null;

	public IDictionary<string, string> Descriptions
	{
		get
		{
			if (descriptions != null)
			{
				return descriptions.ToDictionary((KeyValuePair<string, string> k) => k.Key, (KeyValuePair<string, string> v) => channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(Code, v.Key, "Description"), v.Value));
			}
			return null;
		}
	}

	public string GroupName => string.Empty;

	public string GroupQualifier => string.Empty;

	public string Units => null;

	public object Precision => null;

	public ChoiceCollection Choices => Channel.FaultCodeStatusChoices;

	public bool Visible => true;

	public Service CombinedService => null;

	internal FaultCode(Channel ch, string code, string caesarcode = null)
	{
		caesarCode = caesarcode;
		this.code = code;
		text = string.Empty;
		channel = ch;
		faultCodeIncidents = new FaultCodeIncidentCollection(this, ReadFunctions.NonPermanent);
		snapshots = new FaultCodeIncidentCollection(this, ReadFunctions.Snapshot);
	}

	internal void InternalReset()
	{
		if (channel.ChannelHandle != null)
		{
			channel.ChannelHandle.ClearSingleError((caesarCode != null) ? caesarCode : code);
		}
		if (channel.IsChannelErrorSet)
		{
			channel.SyncDone(new CaesarException(channel.ChannelHandle));
		}
		else
		{
			channel.SyncDone(null);
		}
	}

	internal void AcquireText()
	{
		string text = ((caesarCode != null) ? caesarCode : code);
		if (channel.Ecu.FaultCodeCanBeDuplicate)
		{
			text = code.Split(":".ToCharArray())[0];
		}
		if (channel.EcuHandle != null)
		{
			this.text = channel.EcuHandle.GetErrorText(text);
			if (!Channel.FaultCodes.HasFaultDescriptions)
			{
				return;
			}
			string errorDescription = channel.EcuHandle.GetErrorDescription(text);
			if (!string.IsNullOrWhiteSpace(errorDescription))
			{
				descriptions = (from item in errorDescription.Split(new string[1] { "; \r\n" }, StringSplitOptions.RemoveEmptyEntries)
					let separatorposition = item.IndexOf(": ", StringComparison.Ordinal)
					where separatorposition > -1
					let qualifier = item.Substring(0, separatorposition)
					let value = item.Substring(separatorposition + 2)
					select new KeyValuePair<string, string>(qualifier, value)).ToDictionary((KeyValuePair<string, string> k) => k.Key, (KeyValuePair<string, string> v) => v.Value);
			}
		}
		else if (channel.McdEcuHandle != null)
		{
			McdDBDiagTroubleCode mcdDBDiagTroubleCode = channel.McdEcuHandle.DBDiagTroubleCodes.FirstOrDefault((McdDBDiagTroubleCode f) => f.DisplayTroubleCode == code);
			if (mcdDBDiagTroubleCode != null)
			{
				longNumber = (uint)mcdDBDiagTroubleCode.TroubleCode;
				this.text = mcdDBDiagTroubleCode.Text;
				environmentDataDescriptions = new ServiceCollection(this);
			}
		}
	}

	internal void AddStringsForTranslation(Dictionary<string, string> table, int? maxAccessLevel)
	{
		table[Sapi.MakeTranslationIdentifier(Code, "Text")] = text;
		if (descriptions == null || (maxAccessLevel.HasValue && maxAccessLevel.Value <= 2))
		{
			return;
		}
		foreach (KeyValuePair<string, string> description in descriptions)
		{
			table[Sapi.MakeTranslationIdentifier(Code, description.Key, "Description")] = description.Value;
		}
	}

	internal static string ConvertIsoCodeToUdsCode(string isoCode)
	{
		if (isoCode.Length == 7)
		{
			try
			{
				byte b = 0;
				bool flag = false;
				byte b2 = 0;
				while (b2 < isoAreas.Length && !flag)
				{
					if (isoCode.StartsWith(isoAreas[b2].ToString(), StringComparison.Ordinal))
					{
						b = (byte)(b2 << 6);
						flag = true;
					}
					b2++;
				}
				if (flag)
				{
					byte[] array = new Dump(isoCode.Substring(1)).Data.ToArray();
					array[0] |= b;
					return new Dump(array).ToString();
				}
			}
			catch (FormatException)
			{
			}
		}
		return null;
	}

	internal static string ConvertUdsCodeToIsoCode(string nonIsoCode)
	{
		if (nonIsoCode.Length == 6)
		{
			try
			{
				byte[] array = new Dump(nonIsoCode).Data.ToArray();
				byte b = (byte)(array[0] >> 6);
				array[0] = (byte)(array[0] & 0x3F);
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}", isoAreas[b], new Dump(array));
			}
			catch (FormatException)
			{
			}
		}
		return null;
	}

	public void Manipulate(FaultCodeStatus status)
	{
		ManipulatedValue = new FaultCodeIncident(this, Sapi.Now, status);
		if (status != FaultCodeStatus.None)
		{
			FaultCodeIncidents.Add(ManipulatedValue, readEnvironmentIfNew: false);
		}
	}

	public void ClearManipulation()
	{
		ManipulatedValue = null;
	}

	public void Reset(bool synchronous)
	{
		channel.QueueAction(this, synchronous);
	}

	private static bool TryDecodeSpnFmi(Ecu ecu, string originalCode, bool isFailureMode, out string result)
	{
		result = originalCode;
		if (ecu.FaultCodeIsEncodedSpnFmi)
		{
			string text = ((originalCode.Length == 7) ? ConvertIsoCodeToUdsCode(originalCode) : originalCode);
			if (text.Length == 6)
			{
				try
				{
					IList<byte> data = new Dump(text).Data;
					if (!isFailureMode)
					{
						result = ((uint)(data[0] + (data[1] << 8) + ((data[2] & 0xE0) << 11))).ToString(CultureInfo.InvariantCulture);
						return true;
					}
					result = (data[2] & 0x1F).ToString(CultureInfo.InvariantCulture);
					return true;
				}
				catch (FormatException)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(text, ".EcuInfo defined a reference to J1939SPN/FMI from the UDS code, but the UDS code was not in the correct format.");
				}
			}
		}
		else if (ecu.FaultCodeCanBeDuplicate || ecu.RollCallManager != null)
		{
			string[] array = originalCode.Split(":".ToCharArray());
			if (!isFailureMode)
			{
				result = array[0];
				return true;
			}
			if (array.Length > 1 && array[1].StartsWith("FMI", StringComparison.Ordinal))
			{
				result = array[1].Substring(3);
				return true;
			}
		}
		return false;
	}

	private string GetNumberOrMode(bool mode)
	{
		if (TryDecodeSpnFmi(Channel.Ecu, code, mode, out var result))
		{
			return result;
		}
		if (Channel.Ecu.FaultCodeNumberAndModeFromEngineeringNotes)
		{
			if (descriptions != null && descriptions.TryGetValue("ENGINEERING_NOTES", out var value))
			{
				Match match = engineeringNotesSpnFmiRegex.Match(value);
				if (match.Success)
				{
					return match.Groups[mode ? "fmi" : "spn"].Value;
				}
			}
		}
		else if (channel.Ecu.FaultNumberIsFromEnvironmentData)
		{
			foreach (FaultCodeIncident faultCodeIncident in faultCodeIncidents)
			{
				EnvironmentData environmentData = faultCodeIncident.EnvironmentDatas[mode ? "FaultCodeMode" : "FaultCodeNumber"];
				if (environmentData != null)
				{
					return environmentData.Value.ToString();
				}
			}
		}
		else if (!mode)
		{
			return code;
		}
		return string.Empty;
	}

	internal static LogMetadataItem ExtractMetadata(XmlReader xmlReader, string ecuName, string variantName)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		string text = xmlReader.GetAttribute(currentFormat[TagName.Code].LocalName);
		string attribute = xmlReader.GetAttribute(currentFormat[TagName.StartTime].LocalName);
		string result = null;
		string result2 = null;
		Ecu ecu = Sapi.GetSapi().Ecus[ecuName];
		if (ecu == null && variantName == "ROLLCALL")
		{
			ecu = Ecu.CreateFromRollCallLog(ecuName);
		}
		if (ecu != null && (!TryDecodeSpnFmi(ecu, text, isFailureMode: false, out result) || !TryDecodeSpnFmi(ecu, text, isFailureMode: true, out result2)) && ecu.FaultNumberIsFromEnvironmentData)
		{
			xmlReader.ReadToDescendant(currentFormat[TagName.EnvironmentDatas].LocalName);
			if (xmlReader.ReadToDescendant(currentFormat[TagName.EnvironmentData].LocalName))
			{
				do
				{
					if (result == null || result2 == null)
					{
						KeyValuePair<string, string> keyValuePair = EnvironmentData.ExtractMetadata(xmlReader);
						string key = keyValuePair.Key;
						if (!(key == "FaultCodeNumber"))
						{
							if (key == "FaultCodeMode")
							{
								result2 = keyValuePair.Value;
							}
						}
						else
						{
							result = keyValuePair.Value;
						}
					}
					else
					{
						xmlReader.Skip();
					}
				}
				while (xmlReader.NodeType == XmlNodeType.Element);
			}
			xmlReader.Skip();
		}
		xmlReader.Skip();
		if (result != null && result2 != null)
		{
			text = result + "/" + result2;
		}
		return new LogMetadataItem(LogMetadataType.FaultCode, ecuName, text, attribute);
	}

	public bool IsRelated(FaultCode faultCode)
	{
		if (faultCode != null)
		{
			if (faultCode.Number == Number && faultCode.Mode == Mode)
			{
				return true;
			}
			if (channel.IsRollCall)
			{
				return CompareJ1587InfoToRollCall(faultCode, this);
			}
			return CompareJ1587InfoToRollCall(this, faultCode);
		}
		return false;
	}

	private static bool CompareJ1587InfoToRollCall(FaultCode udsFault, FaultCode j1587Fault)
	{
		if (j1587Fault.channel.IsRollCall && j1587Fault.Channel.Ecu.ProtocolName == "J1708" && udsFault.channel.Ecu.FaultNumberIsFromEnvironmentData)
		{
			FaultCodeIncident faultCodeIncident = udsFault.faultCodeIncidents.Where((FaultCodeIncident fci) => fci.EnvironmentDatas.Count > 0).FirstOrDefault();
			if (faultCodeIncident != null)
			{
				EnvironmentData environmentData = faultCodeIncident.EnvironmentDatas["J1587Info"];
				if (environmentData != null && environmentData.Value != null && TryParseJ1587Info(environmentData.Value.ToString(), out var text, out var text2))
				{
					if (text == j1587Fault.Number)
					{
						return text2 == j1587Fault.Mode;
					}
					return false;
				}
			}
		}
		return false;
	}

	private static bool TryParseJ1587Info(string value, out string number, out string mode)
	{
		number = null;
		mode = null;
		int num = value.IndexOf("FMI", StringComparison.Ordinal);
		int num2 = value.IndexOf("ID", StringComparison.Ordinal);
		if (num > 0 && num2 > 0 && num < value.Length - 3)
		{
			mode = value.Substring(num + 3).Trim();
			number = value.Substring(0, 1) + value.Substring(num2 + 2, num - (num2 + 2)).Trim();
			return true;
		}
		return false;
	}
}
