// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EcuInfoValue
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class EcuInfoValue
{
  private object value;
  private DateTime time;

  internal EcuInfoValue(object ecuInfoValue, DateTime time)
  {
    this.value = ecuInfoValue;
    this.time = time;
  }

  public override string ToString() => this.value.ToString();

  public object Value => this.value;

  public DateTime Time => this.time;

  internal XElement GetXElement(DateTime startTime)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    DateTime time = this.Time < startTime ? startTime : this.Time;
    return new XElement(currentFormat[TagName.Value], new object[2]
    {
      (object) Presentation.FormatForLog(this.Value),
      (object) new XAttribute(currentFormat[TagName.Time], (object) Sapi.TimeToString(time))
    });
  }

  internal static EcuInfoValue FromXElement(
    XElement element,
    LogFileFormatTagCollection format,
    EcuInfo parent)
  {
    return new EcuInfoValue(parent.Presentation != null ? parent.Presentation.ParseFromLog(element.Value) : (object) element.Value, Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value));
  }
}
