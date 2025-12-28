// Decompiled with JetBrains decompiler
// Type: SapiLayer1.McdCaesarEquivalence
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace SapiLayer1;

internal class McdCaesarEquivalence
{
  private static readonly Regex DomainPattern = new Regex("(?<domain>.*)(_Read|_Write)(_\\d)*", RegexOptions.Compiled);

  internal static string GetCaesarEquivalentName(
    McdDBDiagComPrimitive diagService,
    bool forServiceList = false)
  {
    string caesarEquivalentName = diagService.Name;
    string[] strArray;
    if (diagService.Semantic == "SESSION")
      strArray = new string[1]{ "Start" };
    else if (forServiceList && (diagService.Semantic == "VARIANTCODINGWRITE" || diagService.Semantic == "VARIANTCODINGREAD" || diagService.Semantic == "VARCODING"))
      strArray = new string[0];
    else
      strArray = new string[5]
      {
        "Read",
        "Write",
        "Lesen",
        "Read - Analog Signal",
        "Read - Discrete Signal"
      };
    foreach (string str in strArray)
    {
      if (caesarEquivalentName.EndsWith(" " + str, StringComparison.Ordinal))
      {
        caesarEquivalentName = caesarEquivalentName.Substring(0, caesarEquivalentName.Length - (str.Length + 1));
        break;
      }
    }
    return caesarEquivalentName;
  }

  internal static string GetRetainedSuffix(string serviceName)
  {
    string[] strArray = new string[5]
    {
      "Start",
      "Stop",
      "Request Results",
      "Send",
      "Request"
    };
    foreach (string retainedSuffix in strArray)
    {
      if (serviceName.EndsWith(" " + retainedSuffix, StringComparison.Ordinal))
        return retainedSuffix;
    }
    return (string) null;
  }

  internal static string GetSingleServiceEquivalentName(
    string serviceName,
    string[] responseParameterNames)
  {
    string retainedSuffix = McdCaesarEquivalence.GetRetainedSuffix(serviceName);
    string serviceEquivalentName = retainedSuffix == null || serviceName.EndsWith(": " + retainedSuffix, StringComparison.OrdinalIgnoreCase) ? serviceName : serviceName.Insert(serviceName.Length - retainedSuffix.Length - 1, ":");
    if (responseParameterNames != null)
      serviceEquivalentName = serviceEquivalentName + (retainedSuffix != null ? " " : ": ") + string.Join(": ", responseParameterNames);
    return serviceEquivalentName;
  }

  public string Qualifier { get; private set; }

  public string Name { get; private set; }

  public McdDBDiagComPrimitive Service { get; private set; }

  public ServiceTypes ServiceTypes { get; private set; }

  private McdCaesarEquivalence(McdDBJob service, Dictionary<string, int> existingSet)
  {
    this.Service = (McdDBDiagComPrimitive) service;
    this.ServiceTypes = ServiceTypes.DiagJob;
    this.Name = McdCaesarEquivalence.GetCaesarEquivalentName(this.Service, true);
    this.Qualifier = McdCaesarEquivalence.MakeQualifier("DJ_" + this.Name, existingSet);
  }

  private McdCaesarEquivalence(McdDBService service, Dictionary<string, int> existingSet)
  {
    this.Service = (McdDBDiagComPrimitive) service;
    ServiceTypes serviceTypes = ServiceTypes.None;
    string str = string.Empty;
    switch (this.Service.Semantic)
    {
      case "COMMUNICATION":
        serviceTypes = ServiceTypes.Routine;
        str = "RT_";
        break;
      case "CONTROL":
        serviceTypes = ServiceTypes.IOControl;
        str = "IOC_";
        break;
      case "CURRENTDATA":
      case "DATA":
        serviceTypes = ServiceTypes.Data;
        str = "DT_";
        break;
      case "FUNCTION":
        serviceTypes = ServiceTypes.Function;
        str = "FN_";
        break;
      case "IDENTIFICATION":
      case "STOREDDATA":
      case "STOREDDATAREAD":
        if (this.Service.AllRequestParameters.Any<McdDBRequestParameter>((Func<McdDBRequestParameter, bool>) (rp => !rp.IsConst)))
        {
          serviceTypes = ServiceTypes.Download;
          str = this.Service.Semantic == "IDENTIFICATION" ? "DL_ID_" : "DL_";
          break;
        }
        serviceTypes = ServiceTypes.StoredData;
        str = this.Service.Semantic == "IDENTIFICATION" ? "DT_STO_ID_" : "DT_STO_";
        break;
      case "ROUTINE":
        serviceTypes = ServiceTypes.Routine;
        str = "RT_";
        break;
      case "SECURITY":
        serviceTypes = ServiceTypes.Security;
        str = "DNU_";
        break;
      case "SESSION":
        serviceTypes = ServiceTypes.Session;
        str = "SES_";
        break;
      case "STOREDDATAWRITE":
        serviceTypes = ServiceTypes.Download;
        str = "DL_";
        break;
      case "VARIANTCODINGREAD":
        serviceTypes = ServiceTypes.ReadVarCode;
        str = "RVC_";
        break;
      case "VARIANTCODINGWRITE":
        serviceTypes = ServiceTypes.WriteVarCode;
        str = "WVC_";
        break;
    }
    this.ServiceTypes = serviceTypes;
    this.Name = McdCaesarEquivalence.GetCaesarEquivalentName(this.Service, true);
    this.Qualifier = McdCaesarEquivalence.MakeQualifier(str + this.Name, existingSet);
  }

  private McdCaesarEquivalence(McdDBControlPrimitive controlPrimitive, IEnumerable<string> jobNames)
  {
    this.Service = (McdDBDiagComPrimitive) controlPrimitive;
    this.Qualifier = controlPrimitive.Qualifier;
    this.ServiceTypes = jobNames.Contains<string>(controlPrimitive.InternalShortName) ? ServiceTypes.DiagJob : ServiceTypes.None;
    switch (controlPrimitive.ObjectType)
    {
      case McdObjectType.DBStartCommunication:
      case McdObjectType.DBStopCommunication:
        this.Name = $"{controlPrimitive.Name} ({controlPrimitive.Qualifier})";
        break;
      default:
        this.Name = controlPrimitive.Name;
        break;
    }
  }

  internal static string MakeQualifier(
    string name,
    Dictionary<string, int> existingSet,
    bool isFragmentName = false)
  {
    string key1 = McdCaesarEquivalence.MakeQualifier(name, isFragmentName: isFragmentName);
    if (existingSet != null)
    {
      if (existingSet.ContainsKey(key1))
      {
        string key2;
        do
        {
          existingSet[key1]++;
          key2 = $"{key1}_{(object) existingSet[key1]}";
        }
        while (existingSet.ContainsKey(key2));
        existingSet.Add(key2, 0);
        return key2;
      }
      existingSet.Add(key1, 0);
    }
    return key1;
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
          case 'Ä':
            stringBuilder.Append("Ae");
            continue;
          case 'Ö':
            stringBuilder.Append("Oe");
            continue;
          case 'Ü':
            stringBuilder.Append("Ue");
            continue;
          case 'ß':
            stringBuilder.Append("ss");
            continue;
          case 'ä':
            stringBuilder.Append("ae");
            continue;
          case 'ö':
            stringBuilder.Append("oe");
            continue;
          case 'ü':
            stringBuilder.Append("ue");
            continue;
          default:
            stringBuilder.Append(c);
            continue;
        }
      }
      else if (!isFragmentName || c != '[' && c != ']')
      {
        if (isDOPName && c == '.')
          stringBuilder.Append("p");
        else if (!flag)
        {
          stringBuilder.Append("_");
          flag = true;
        }
      }
    }
    string str = stringBuilder.ToString();
    if (str.EndsWith("_", StringComparison.Ordinal))
      str = str.Substring(0, str.Length - 1);
    return str;
  }

  internal static IEnumerable<McdCaesarEquivalence> FromDBLocation(McdDBLocation location)
  {
    Dictionary<string, int> caesarEquivalentQualifiers = new Dictionary<string, int>();
    return (IEnumerable<McdCaesarEquivalence>) location.DBServices.Select<McdDBService, McdCaesarEquivalence>((Func<McdDBService, McdCaesarEquivalence>) (mcdService => new McdCaesarEquivalence(mcdService, caesarEquivalentQualifiers))).Union<McdCaesarEquivalence>(location.DBJobs.Select<McdDBJob, McdCaesarEquivalence>((Func<McdDBJob, McdCaesarEquivalence>) (mcdJob => new McdCaesarEquivalence(mcdJob, caesarEquivalentQualifiers)))).Union<McdCaesarEquivalence>(location.DBControlPrimitives.Where<McdDBControlPrimitive>((Func<McdDBControlPrimitive, bool>) (p => p.ObjectType == McdObjectType.DBStartCommunication || p.ObjectType == McdObjectType.DBStopCommunication || p.ObjectType == McdObjectType.DBProtocolParameterSet || p.ObjectType == McdObjectType.DBVariantIdentification)).Select<McdDBControlPrimitive, McdCaesarEquivalence>((Func<McdDBControlPrimitive, McdCaesarEquivalence>) (mcdControlPrimitive => new McdCaesarEquivalence(mcdControlPrimitive, location.DBJobs.Select<McdDBJob, string>((Func<McdDBJob, string>) (j => j.Qualifier)))))).ToList<McdCaesarEquivalence>();
  }

  internal static string GetDomainQualifier(string longName)
  {
    string input = McdCaesarEquivalence.MakeQualifier(longName);
    Match match = McdCaesarEquivalence.DomainPattern.Match(input);
    return "VCD_" + (match.Success ? match.Groups["domain"].Value : input);
  }

  internal static bool TryGetIndexLengthIgnoreUnderscores(
    string qualifier,
    string partialQualifier,
    out int position,
    out int length)
  {
    position = -1;
    length = 0;
    int index1 = 0;
    int index2;
    for (index2 = 0; index2 < partialQualifier.Length; ++index2)
    {
      char ch1 = partialQualifier[index2];
      if (ch1 != '_')
      {
        for (; index1 < qualifier.Length; ++index1)
        {
          char ch2 = qualifier[index1];
          if ((int) ch2 == (int) ch1)
          {
            ++length;
            if (position == -1)
              position = index1;
            ++index1;
            break;
          }
          if (ch2 == '_')
          {
            if (position != -1)
              ++length;
          }
          else if (position != -1)
          {
            index1 -= length - 1;
            length = 0;
            index2 = position = -1;
            break;
          }
        }
      }
    }
    return position != -1 && length > 0 && index2 == partialQualifier.Length;
  }

  internal static string GetResponsePart(string complete, string servicePart, bool isName)
  {
    int length = servicePart.Length;
    while (length < complete.Length && IsSeparator(complete[length]))
      ++length;
    return length >= complete.Length ? string.Empty : complete.Substring(length);

    bool IsSeparator(char value)
    {
      if (!isName)
        return value == '_';
      return value == ' ' || value == ':';
    }
  }

  private static bool AdjustServiceQualifierName(
    string originalDiagServiceQualifier,
    string responseQualifier,
    ref string serviceQualifier,
    ref string serviceName)
  {
    if (originalDiagServiceQualifier.EndsWith("_" + responseQualifier, StringComparison.OrdinalIgnoreCase))
    {
      int length = serviceQualifier.IndexOf("_" + responseQualifier, StringComparison.Ordinal);
      if (length != -1)
      {
        serviceQualifier = serviceQualifier.Substring(0, length);
        serviceName = serviceName.Substring(0, originalDiagServiceQualifier.Length - responseQualifier.Length - 1);
        return true;
      }
    }
    return false;
  }

  internal static bool AdjustServiceQualifierName(
    McdDBDiagComPrimitive diagService,
    McdDBResponseParameter response,
    ref string serviceQualifier,
    ref string serviceName)
  {
    string originalDiagServiceQualifier = McdCaesarEquivalence.MakeQualifier(diagService.Name, (Dictionary<string, int>) null);
    return McdCaesarEquivalence.AdjustServiceQualifierName(originalDiagServiceQualifier, McdCaesarEquivalence.MakeQualifier(response.Name, (Dictionary<string, int>) null), ref serviceQualifier, ref serviceName) || McdCaesarEquivalence.AdjustServiceQualifierName(originalDiagServiceQualifier, response.Qualifier, ref serviceQualifier, ref serviceName);
  }
}
