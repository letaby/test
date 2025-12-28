// Decompiled with JetBrains decompiler
// Type: SapiLayer1.InstrumentValue
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class InstrumentValue
{
  private object value;
  private DateTime firstSampleTime;
  private DateTime lastSampleTime;
  private int index;
  private int sampleCount;
  private bool contiguousSegmentStart;

  internal InstrumentValue(object o, DateTime sampleTime, int index, bool contiguousSegmentStart)
  {
    this.value = o;
    this.index = index;
    this.firstSampleTime = sampleTime;
    this.lastSampleTime = sampleTime;
    this.sampleCount = 1;
    this.contiguousSegmentStart = contiguousSegmentStart;
  }

  private InstrumentValue(
    object o,
    DateTime firstSampleTime,
    DateTime lastSampleTime,
    int sampleCount,
    int index,
    bool contiguousSegmentStart)
  {
    this.value = o;
    this.index = index;
    this.firstSampleTime = firstSampleTime;
    this.lastSampleTime = lastSampleTime;
    this.sampleCount = sampleCount;
    this.contiguousSegmentStart = contiguousSegmentStart;
  }

  internal void SetLastSampleTime(DateTime time)
  {
    this.lastSampleTime = time;
    ++this.sampleCount;
  }

  internal void WriteXmlTo(DateTime startTime, DateTime endTime, XmlWriter writer)
  {
    if (this.LastSampleTime < startTime || this.FirstSampleTime > endTime)
      return;
    DateTime time1 = this.FirstSampleTime > startTime ? this.FirstSampleTime : startTime;
    DateTime time2 = this.LastSampleTime < endTime ? this.LastSampleTime : endTime;
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.Time].LocalName, Sapi.TimeToString(time1));
    if (time2 != time1)
      writer.WriteAttributeString(currentFormat[TagName.LastSampleTime].LocalName, Sapi.TimeToString(time2));
    if (this.ItemSampleCount > 1)
    {
      TimeSpan timeSpan1 = this.LastSampleTime - this.FirstSampleTime;
      TimeSpan timeSpan2 = time2 - time1;
      int num = this.ItemSampleCount;
      if (timeSpan1 != timeSpan2)
        num = (int) Math.Ceiling((double) this.ItemSampleCount * timeSpan2.TotalSeconds / timeSpan1.TotalSeconds);
      writer.WriteAttributeString(currentFormat[TagName.SampleCount].LocalName, num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
    if (this.ContiguousSegmentStart)
      writer.WriteAttributeString(currentFormat[TagName.ContiguousSegmentStart].LocalName, "1");
    writer.WriteValue(Presentation.FormatForLog(this.Value));
    writer.WriteEndElement();
  }

  internal static InstrumentValue FromXElement(
    XElement element,
    LogFileFormatTagCollection format,
    Instrument instrument)
  {
    DateTime firstSampleTime = Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value);
    int sampleCount = 1;
    bool contiguousSegmentStart = false;
    XAttribute xattribute1 = element.Attribute(format[TagName.LastSampleTime]);
    DateTime lastSampleTime = xattribute1 == null ? firstSampleTime : Sapi.TimeFromString(xattribute1.Value);
    XAttribute xattribute2 = element.Attribute(format[TagName.SampleCount]);
    if (xattribute2 != null)
      sampleCount = Convert.ToInt32(xattribute2.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    XAttribute xattribute3 = element.Attribute(format[TagName.ContiguousSegmentStart]);
    if (xattribute3 != null)
      contiguousSegmentStart = string.Equals(xattribute3.Value, "1", StringComparison.Ordinal);
    return new InstrumentValue(instrument.ParseFromLog(element.Value), firstSampleTime, lastSampleTime, sampleCount, instrument.InstrumentValues.Count, contiguousSegmentStart);
  }

  public override string ToString() => this.value == null ? string.Empty : this.value.ToString();

  public bool ActiveAtTime(DateTime time)
  {
    return time >= this.firstSampleTime && time <= this.lastSampleTime;
  }

  public object Value => this.value;

  public DateTime FirstSampleTime => this.firstSampleTime;

  public DateTime LastSampleTime => this.lastSampleTime;

  public int ItemSampleCount => this.sampleCount;

  public int ItemIndex => this.index;

  public bool ContiguousSegmentStart => this.contiguousSegmentStart;

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("SampleCount is deprecated due to non-CLS compliance, please use ItemSampleCount instead.")]
  public uint SampleCount => (uint) this.sampleCount;

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("Index is deprecated due to non-CLS compliance, please use ItemIndex instead.")]
  public uint Index => (uint) this.index;
}
