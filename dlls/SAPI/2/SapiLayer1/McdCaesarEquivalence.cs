using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using McdAbstraction;

namespace SapiLayer1;

internal class McdCaesarEquivalence
{
	private static readonly Regex DomainPattern = new Regex("(?<domain>.*)(_Read|_Write)(_\\d)*", RegexOptions.Compiled);

	public string Qualifier { get; private set; }

	public string Name { get; private set; }

	public McdDBDiagComPrimitive Service { get; private set; }

	public ServiceTypes ServiceTypes { get; private set; }

	internal static string GetCaesarEquivalentName(McdDBDiagComPrimitive diagService, bool forServiceList = false)
	{
		string text = diagService.Name;
		string[] array = ((diagService.Semantic == "SESSION") ? new string[1] { "Start" } : ((!forServiceList || (!(diagService.Semantic == "VARIANTCODINGWRITE") && !(diagService.Semantic == "VARIANTCODINGREAD") && !(diagService.Semantic == "VARCODING"))) ? new string[5] { "Read", "Write", "Lesen", "Read - Analog Signal", "Read - Discrete Signal" } : new string[0]));
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (text.EndsWith(" " + text2, StringComparison.Ordinal))
			{
				text = text.Substring(0, text.Length - (text2.Length + 1));
				break;
			}
		}
		return text;
	}

	internal static string GetRetainedSuffix(string serviceName)
	{
		string[] array = new string[5] { "Start", "Stop", "Request Results", "Send", "Request" };
		foreach (string text in array)
		{
			if (serviceName.EndsWith(" " + text, StringComparison.Ordinal))
			{
				return text;
			}
		}
		return null;
	}

	internal static string GetSingleServiceEquivalentName(string serviceName, string[] responseParameterNames)
	{
		string retainedSuffix = GetRetainedSuffix(serviceName);
		string text = ((retainedSuffix != null && !serviceName.EndsWith(": " + retainedSuffix, StringComparison.OrdinalIgnoreCase)) ? serviceName.Insert(serviceName.Length - retainedSuffix.Length - 1, ":") : serviceName);
		if (responseParameterNames != null)
		{
			text = text + ((retainedSuffix != null) ? " " : ": ") + string.Join(": ", responseParameterNames);
		}
		return text;
	}

	private McdCaesarEquivalence(McdDBJob service, Dictionary<string, int> existingSet)
	{
		Service = service;
		ServiceTypes = ServiceTypes.DiagJob;
		Name = GetCaesarEquivalentName(Service, forServiceList: true);
		Qualifier = MakeQualifier("DJ_" + Name, existingSet);
	}

	private McdCaesarEquivalence(McdDBService service, Dictionary<string, int> existingSet)
	{
		Service = service;
		ServiceTypes serviceTypes = ServiceTypes.None;
		string text = string.Empty;
		switch (Service.Semantic)
		{
		case "CURRENTDATA":
		case "DATA":
			serviceTypes = ServiceTypes.Data;
			text = "DT_";
			break;
		case "STOREDDATAREAD":
		case "STOREDDATA":
		case "IDENTIFICATION":
			if (Service.AllRequestParameters.Any((McdDBRequestParameter rp) => !rp.IsConst))
			{
				serviceTypes = ServiceTypes.Download;
				text = ((Service.Semantic == "IDENTIFICATION") ? "DL_ID_" : "DL_");
			}
			else
			{
				serviceTypes = ServiceTypes.StoredData;
				text = ((Service.Semantic == "IDENTIFICATION") ? "DT_STO_ID_" : "DT_STO_");
			}
			break;
		case "STOREDDATAWRITE":
			serviceTypes = ServiceTypes.Download;
			text = "DL_";
			break;
		case "SECURITY":
			serviceTypes = ServiceTypes.Security;
			text = "DNU_";
			break;
		case "COMMUNICATION":
			serviceTypes = ServiceTypes.Routine;
			text = "RT_";
			break;
		case "CONTROL":
			serviceTypes = ServiceTypes.IOControl;
			text = "IOC_";
			break;
		case "ROUTINE":
			serviceTypes = ServiceTypes.Routine;
			text = "RT_";
			break;
		case "SESSION":
			serviceTypes = ServiceTypes.Session;
			text = "SES_";
			break;
		case "FUNCTION":
			serviceTypes = ServiceTypes.Function;
			text = "FN_";
			break;
		case "VARIANTCODINGWRITE":
			serviceTypes = ServiceTypes.WriteVarCode;
			text = "WVC_";
			break;
		case "VARIANTCODINGREAD":
			serviceTypes = ServiceTypes.ReadVarCode;
			text = "RVC_";
			break;
		}
		ServiceTypes = serviceTypes;
		Name = GetCaesarEquivalentName(Service, forServiceList: true);
		Qualifier = MakeQualifier(text + Name, existingSet);
	}

	private McdCaesarEquivalence(McdDBControlPrimitive controlPrimitive, IEnumerable<string> jobNames)
	{
		Service = controlPrimitive;
		Qualifier = controlPrimitive.Qualifier;
		ServiceTypes = (jobNames.Contains(controlPrimitive.InternalShortName) ? ServiceTypes.DiagJob : ServiceTypes.None);
		McdObjectType objectType = controlPrimitive.ObjectType;
		if ((uint)(objectType - 1180) <= 1u)
		{
			Name = controlPrimitive.Name + " (" + controlPrimitive.Qualifier + ")";
		}
		else
		{
			Name = controlPrimitive.Name;
		}
	}

	internal static string MakeQualifier(string name, Dictionary<string, int> existingSet, bool isFragmentName = false)
	{
		string text = MakeQualifier(name, isDOPName: false, isFragmentName);
		if (existingSet != null)
		{
			if (existingSet.ContainsKey(text))
			{
				string text2;
				do
				{
					existingSet[text]++;
					text2 = text + "_" + existingSet[text];
				}
				while (existingSet.ContainsKey(text2));
				existingSet.Add(text2, 0);
				return text2;
			}
			existingSet.Add(text, 0);
		}
		return text;
	}

	internal static string MakeQualifier(string name, bool isDOPName = false, bool isFragmentName = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (char c in name)
		{
			if (char.IsLetterOrDigit(c))
			{
				flag = false;
				switch (c)
				{
				case 'ä':
					stringBuilder.Append("ae");
					break;
				case 'ö':
					stringBuilder.Append("oe");
					break;
				case 'ü':
					stringBuilder.Append("ue");
					break;
				case 'Ä':
					stringBuilder.Append("Ae");
					break;
				case 'Ö':
					stringBuilder.Append("Oe");
					break;
				case 'Ü':
					stringBuilder.Append("Ue");
					break;
				case 'ß':
					stringBuilder.Append("ss");
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
			else if (!isFragmentName || (c != '[' && c != ']'))
			{
				if (isDOPName && c == '.')
				{
					stringBuilder.Append("p");
				}
				else if (!flag)
				{
					stringBuilder.Append("_");
					flag = true;
				}
			}
		}
		string text = stringBuilder.ToString();
		if (text.EndsWith("_", StringComparison.Ordinal))
		{
			text = text.Substring(0, text.Length - 1);
		}
		return text;
	}

	internal static IEnumerable<McdCaesarEquivalence> FromDBLocation(McdDBLocation location)
	{
		Dictionary<string, int> caesarEquivalentQualifiers = new Dictionary<string, int>();
		return location.DBServices.Select((McdDBService mcdService) => new McdCaesarEquivalence(mcdService, caesarEquivalentQualifiers)).Union(location.DBJobs.Select((McdDBJob mcdJob) => new McdCaesarEquivalence(mcdJob, caesarEquivalentQualifiers))).Union(from mcdControlPrimitive in location.DBControlPrimitives
			where mcdControlPrimitive.ObjectType == McdObjectType.DBStartCommunication || mcdControlPrimitive.ObjectType == McdObjectType.DBStopCommunication || mcdControlPrimitive.ObjectType == McdObjectType.DBProtocolParameterSet || mcdControlPrimitive.ObjectType == McdObjectType.DBVariantIdentification
			select new McdCaesarEquivalence(mcdControlPrimitive, location.DBJobs.Select((McdDBJob j) => j.Qualifier)))
			.ToList();
	}

	internal static string GetDomainQualifier(string longName)
	{
		string text = MakeQualifier(longName);
		Match match = DomainPattern.Match(text);
		return "VCD_" + (match.Success ? match.Groups["domain"].Value : text);
	}

	internal static bool TryGetIndexLengthIgnoreUnderscores(string qualifier, string partialQualifier, out int position, out int length)
	{
		position = -1;
		length = 0;
		int i = 0;
		int j;
		for (j = 0; j < partialQualifier.Length; j++)
		{
			char c = partialQualifier[j];
			if (c == '_')
			{
				continue;
			}
			for (; i < qualifier.Length; i++)
			{
				char c2 = qualifier[i];
				if (c2 == c)
				{
					length++;
					if (position == -1)
					{
						position = i;
					}
					i++;
					break;
				}
				if (c2 == '_')
				{
					if (position != -1)
					{
						length++;
					}
				}
				else if (position != -1)
				{
					i -= length - 1;
					length = 0;
					j = (position = -1);
					break;
				}
			}
		}
		if (position != -1 && length > 0)
		{
			return j == partialQualifier.Length;
		}
		return false;
	}

	internal static string GetResponsePart(string complete, string servicePart, bool isName)
	{
		int i;
		for (i = servicePart.Length; i < complete.Length && IsSeparator(complete[i]); i++)
		{
		}
		if (i >= complete.Length)
		{
			return string.Empty;
		}
		return complete.Substring(i);
		bool IsSeparator(char value)
		{
			if (!isName)
			{
				return value == '_';
			}
			if (value != ' ')
			{
				return value == ':';
			}
			return true;
		}
	}

	private static bool AdjustServiceQualifierName(string originalDiagServiceQualifier, string responseQualifier, ref string serviceQualifier, ref string serviceName)
	{
		if (originalDiagServiceQualifier.EndsWith("_" + responseQualifier, StringComparison.OrdinalIgnoreCase))
		{
			int num = serviceQualifier.IndexOf("_" + responseQualifier, StringComparison.Ordinal);
			if (num != -1)
			{
				serviceQualifier = serviceQualifier.Substring(0, num);
				serviceName = serviceName.Substring(0, originalDiagServiceQualifier.Length - responseQualifier.Length - 1);
				return true;
			}
		}
		return false;
	}

	internal static bool AdjustServiceQualifierName(McdDBDiagComPrimitive diagService, McdDBResponseParameter response, ref string serviceQualifier, ref string serviceName)
	{
		string originalDiagServiceQualifier = MakeQualifier(diagService.Name, null);
		if (AdjustServiceQualifierName(originalDiagServiceQualifier, MakeQualifier(response.Name, null), ref serviceQualifier, ref serviceName))
		{
			return true;
		}
		if (AdjustServiceQualifierName(originalDiagServiceQualifier, response.Qualifier, ref serviceQualifier, ref serviceName))
		{
			return true;
		}
		return false;
	}
}
