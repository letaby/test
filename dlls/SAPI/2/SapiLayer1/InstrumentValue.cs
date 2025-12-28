using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace SapiLayer1;

public sealed class InstrumentValue
{
	private object value;

	private DateTime firstSampleTime;

	private DateTime lastSampleTime;

	private int index;

	private int sampleCount;

	private bool contiguousSegmentStart;

	public object Value => value;

	public DateTime FirstSampleTime => firstSampleTime;

	public DateTime LastSampleTime => lastSampleTime;

	public int ItemSampleCount => sampleCount;

	public int ItemIndex => index;

	public bool ContiguousSegmentStart => contiguousSegmentStart;

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SampleCount is deprecated due to non-CLS compliance, please use ItemSampleCount instead.")]
	public uint SampleCount => (uint)sampleCount;

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Index is deprecated due to non-CLS compliance, please use ItemIndex instead.")]
	public uint Index => (uint)index;

	internal InstrumentValue(object o, DateTime sampleTime, int index, bool contiguousSegmentStart)
	{
		value = o;
		this.index = index;
		firstSampleTime = sampleTime;
		lastSampleTime = sampleTime;
		sampleCount = 1;
		this.contiguousSegmentStart = contiguousSegmentStart;
	}

	private InstrumentValue(object o, DateTime firstSampleTime, DateTime lastSampleTime, int sampleCount, int index, bool contiguousSegmentStart)
	{
		value = o;
		this.index = index;
		this.firstSampleTime = firstSampleTime;
		this.lastSampleTime = lastSampleTime;
		this.sampleCount = sampleCount;
		this.contiguousSegmentStart = contiguousSegmentStart;
	}

	internal void SetLastSampleTime(DateTime time)
	{
		lastSampleTime = time;
		sampleCount++;
	}

	internal void WriteXmlTo(DateTime startTime, DateTime endTime, XmlWriter writer)
	{
		if (LastSampleTime < startTime || FirstSampleTime > endTime)
		{
			return;
		}
		DateTime dateTime = ((FirstSampleTime > startTime) ? FirstSampleTime : startTime);
		DateTime dateTime2 = ((LastSampleTime < endTime) ? LastSampleTime : endTime);
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.Time].LocalName, Sapi.TimeToString(dateTime));
		if (dateTime2 != dateTime)
		{
			writer.WriteAttributeString(currentFormat[TagName.LastSampleTime].LocalName, Sapi.TimeToString(dateTime2));
		}
		if (ItemSampleCount > 1)
		{
			TimeSpan timeSpan = LastSampleTime - FirstSampleTime;
			TimeSpan timeSpan2 = dateTime2 - dateTime;
			int num = ItemSampleCount;
			if (timeSpan != timeSpan2)
			{
				num = (int)Math.Ceiling((double)ItemSampleCount * timeSpan2.TotalSeconds / timeSpan.TotalSeconds);
			}
			writer.WriteAttributeString(currentFormat[TagName.SampleCount].LocalName, num.ToString(CultureInfo.InvariantCulture));
		}
		if (ContiguousSegmentStart)
		{
			writer.WriteAttributeString(currentFormat[TagName.ContiguousSegmentStart].LocalName, "1");
		}
		writer.WriteValue(Presentation.FormatForLog(Value));
		writer.WriteEndElement();
	}

	internal static InstrumentValue FromXElement(XElement element, LogFileFormatTagCollection format, Instrument instrument)
	{
		DateTime dateTime = Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value);
		int num = 1;
		bool flag = false;
		XAttribute xAttribute = element.Attribute(format[TagName.LastSampleTime]);
		DateTime dateTime2 = ((xAttribute == null) ? dateTime : Sapi.TimeFromString(xAttribute.Value));
		XAttribute xAttribute2 = element.Attribute(format[TagName.SampleCount]);
		if (xAttribute2 != null)
		{
			num = Convert.ToInt32(xAttribute2.Value, CultureInfo.InvariantCulture);
		}
		XAttribute xAttribute3 = element.Attribute(format[TagName.ContiguousSegmentStart]);
		if (xAttribute3 != null)
		{
			flag = string.Equals(xAttribute3.Value, "1", StringComparison.Ordinal);
		}
		return new InstrumentValue(instrument.ParseFromLog(element.Value), dateTime, dateTime2, num, instrument.InstrumentValues.Count, flag);
	}

	public override string ToString()
	{
		if (value == null)
		{
			return string.Empty;
		}
		return value.ToString();
	}

	public bool ActiveAtTime(DateTime time)
	{
		if (time >= firstSampleTime)
		{
			return time <= lastSampleTime;
		}
		return false;
	}
}
