// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CommunicationsStateValue
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CommunicationsStateValue
{
  private Channel channel;
  private CommunicationsState value;
  private DateTime time;
  private string additional;

  internal CommunicationsStateValue(
    Channel channel,
    CommunicationsState state,
    DateTime time,
    string additional)
  {
    this.channel = channel;
    this.value = state;
    this.time = time;
    this.additional = additional;
  }

  public override string ToString() => this.value.ToString();

  public Channel Channel => this.channel;

  public CommunicationsState Value => this.value;

  public DateTime Time => this.time;

  public string Additional => this.additional;

  internal XElement GetXElement(DateTime startTime)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    DateTime time = this.Time < startTime ? startTime : this.Time;
    XElement xelement = new XElement(currentFormat[TagName.CommunicationsState], new object[2]
    {
      (object) this.ToString(),
      (object) new XAttribute(currentFormat[TagName.Time], (object) Sapi.TimeToString(time))
    });
    if (!string.IsNullOrEmpty(this.Additional))
      xelement.Add((object) new XAttribute(currentFormat[TagName.Additional], (object) this.Additional));
    return xelement;
  }

  internal static CommunicationsStateValue FromXElement(
    XElement element,
    LogFileFormatTagCollection format,
    Channel channel)
  {
    CommunicationsState state = CommunicationsState.Unknown;
    try
    {
      state = (CommunicationsState) Enum.Parse(typeof (CommunicationsState), element.Value);
    }
    catch (ArgumentException ex)
    {
    }
    DateTime time = Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value);
    XAttribute xattribute = element.Attribute(format[TagName.Additional]);
    string additional = xattribute != null ? xattribute.Value : string.Empty;
    return new CommunicationsStateValue(channel, state, time, additional);
  }
}
