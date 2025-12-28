using System;
using System.Xml.Linq;

namespace SapiLayer1;

public sealed class CommunicationsStateValue
{
	private Channel channel;

	private CommunicationsState value;

	private DateTime time;

	private string additional;

	public Channel Channel => channel;

	public CommunicationsState Value => value;

	public DateTime Time => time;

	public string Additional => additional;

	internal CommunicationsStateValue(Channel channel, CommunicationsState state, DateTime time, string additional)
	{
		this.channel = channel;
		value = state;
		this.time = time;
		this.additional = additional;
	}

	public override string ToString()
	{
		return value.ToString();
	}

	internal XElement GetXElement(DateTime startTime)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		DateTime dateTime = ((Time < startTime) ? startTime : Time);
		XElement xElement = new XElement(currentFormat[TagName.CommunicationsState], ToString(), new XAttribute(currentFormat[TagName.Time], Sapi.TimeToString(dateTime)));
		if (!string.IsNullOrEmpty(Additional))
		{
			xElement.Add(new XAttribute(currentFormat[TagName.Additional], Additional));
		}
		return xElement;
	}

	internal static CommunicationsStateValue FromXElement(XElement element, LogFileFormatTagCollection format, Channel channel)
	{
		CommunicationsState state = CommunicationsState.Unknown;
		try
		{
			state = (CommunicationsState)Enum.Parse(typeof(CommunicationsState), element.Value);
		}
		catch (ArgumentException)
		{
		}
		DateTime dateTime = Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value);
		XAttribute xAttribute = element.Attribute(format[TagName.Additional]);
		string text = ((xAttribute != null) ? xAttribute.Value : string.Empty);
		return new CommunicationsStateValue(channel, state, dateTime, text);
	}
}
