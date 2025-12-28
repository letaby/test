// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ParameterValue
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ParameterValue
{
  private object value;
  private DateTime time;

  internal ParameterValue(object ParameterValue, DateTime time)
  {
    this.value = ParameterValue;
    this.time = time;
  }

  public override string ToString() => this.value.ToString();

  public object Value => this.value;

  public DateTime Time => this.time;

  internal void WriteXmlTo(DateTime startTime, Parameter parent, XmlWriter writer)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    DateTime time = this.Time < startTime ? startTime : this.Time;
    writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.Time].LocalName, Sapi.TimeToString(time));
    writer.WriteValue(parent.FormatValue(this.Value));
    writer.WriteEndElement();
  }

  internal static ParameterValue FromXElement(
    XElement element,
    LogFileFormatTagCollection format,
    Parameter parent)
  {
    return new ParameterValue(parent.ParseValue(element.Value), Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value));
  }
}
