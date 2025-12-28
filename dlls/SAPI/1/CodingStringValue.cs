// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingStringValue
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CodingStringValue
{
  private Dump value;
  private DateTime time;

  internal CodingStringValue(Dump codingStringValue, DateTime time)
  {
    this.value = codingStringValue;
    this.time = time;
  }

  public override string ToString() => this.value.ToString();

  public Dump Value => this.value;

  public DateTime Time => this.time;

  internal void WriteXmlTo(DateTime startTime, XmlWriter writer)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    DateTime time = this.Time < startTime ? startTime : this.Time;
    writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.Time].LocalName, Sapi.TimeToString(time));
    writer.WriteValue(this.Value.ToString());
    writer.WriteEndElement();
  }

  internal static CodingStringValue FromXElement(
    XElement element,
    LogFileFormatTagCollection format)
  {
    return new CodingStringValue(new Dump(element.Value), Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value));
  }
}
