// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ParameterGroup
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ParameterGroup
{
  private Dump codingStringMask;
  private string qualifier;
  private CodingStringValueCollection codingStringValues;
  private List<Parameter> parameters;
  private int? groupLength;

  internal ParameterGroup(string qualifier, List<Parameter> parameters)
  {
    this.codingStringValues = new CodingStringValueCollection();
    this.qualifier = qualifier;
    this.parameters = parameters;
    this.groupLength = (int?) this.parameters.FirstOrDefault<Parameter>()?.GroupLength;
  }

  public string Qualifier => this.qualifier;

  public bool CommunicatedViaJob
  {
    get
    {
      Service writeService = this.Parameters.FirstOrDefault<Parameter>()?.WriteService;
      if (!(writeService != (Service) null))
        return false;
      return !writeService.Channel.Ecu.IsMcd ? writeService.Arguments != null : VarcodeMcd.IsSplittedParameterGroup(writeService.SpecialData);
    }
  }

  public Dump CodingStringMask
  {
    get
    {
      if (this.codingStringMask == null && this.GroupLength.HasValue)
        this.codingStringMask = Parameter.CreateCodingStringMask(this.GroupLength.Value, this.Parameters, true);
      return this.codingStringMask;
    }
  }

  public bool ParametersCoverGroup
  {
    get
    {
      return this.GroupLength.HasValue && this.GroupLength.Value > 0 && this.CodingStringMask.Data.All<byte>((Func<byte, bool>) (b => b == byte.MaxValue));
    }
  }

  public IEnumerable<Parameter> Parameters => (IEnumerable<Parameter>) this.parameters.AsReadOnly();

  public int? GroupLength => this.groupLength;

  public bool ServiceAsParameter => this.parameters.First<Parameter>().ServiceAsParameter;

  public CodingStringValueCollection CodingStringValues => this.codingStringValues;

  internal static bool LoadFromLog(
    XElement element,
    string groupQualifier,
    LogFileFormatTagCollection format,
    Channel channel,
    List<string> missingQualifierList,
    object missingInfoLock)
  {
    ParameterGroup parameterGroup = channel.ParameterGroups[groupQualifier];
    if (parameterGroup != null)
    {
      bool flag = false;
      foreach (XElement element1 in element.Elements(format[TagName.Value]))
      {
        try
        {
          parameterGroup.codingStringValues.Add(CodingStringValue.FromXElement(element1, format), false);
          flag = true;
        }
        catch (ArgumentOutOfRangeException ex)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ArgumentOutOfRangeException while loading {0} value '{1}' from log file", (object) groupQualifier, (object) element1.Value));
        }
        catch (FormatException ex)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FormatException while loading {0} value '{1}' from log file", (object) groupQualifier, (object) element1.Value));
        }
      }
      foreach (XElement element2 in element.Elements(format[TagName.Parameter]))
        flag |= Parameter.LoadFromLog(element2, groupQualifier, format, channel, missingQualifierList, missingInfoLock);
      return flag;
    }
    if (!channel.Ecu.IgnoreQualifier(groupQualifier))
    {
      lock (missingInfoLock)
        missingQualifierList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) channel.Ecu.Name, (object) groupQualifier));
    }
    return false;
  }

  internal void WriteXmlTo(DateTime startTime, DateTime endTime, XmlWriter writer, bool all)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    if (!this.Parameters.Any<Parameter>((Func<Parameter, bool>) (p =>
    {
      if (p.ParameterValues.Count <= 0)
        return false;
      return all || p.Summary;
    })) && !(this.CodingStringValues.Any<CodingStringValue>() & all))
      return;
    writer.WriteStartElement(currentFormat[TagName.Group].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, this.Qualifier);
    if (all)
    {
      foreach (CodingStringValue codingStringValue in this.CodingStringValues)
        codingStringValue.WriteXmlTo(startTime, writer);
    }
    foreach (Parameter parameter in this.Parameters)
    {
      if (parameter.ParameterValues.Count > 0 && (all || parameter.Summary))
        parameter.WriteXmlTo(startTime, endTime, writer);
    }
    writer.WriteEndElement();
  }
}
