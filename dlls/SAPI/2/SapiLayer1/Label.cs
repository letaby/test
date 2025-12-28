using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SapiLayer1;

public sealed class Label
{
	private string name;

	private DateTime time;

	private Ecu ecu;

	private Channel channel;

	public string Name => name;

	public DateTime Time => time;

	public Ecu Ecu => ecu;

	public Channel Channel => channel;

	internal XElement XElement
	{
		get
		{
			LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
			XElement xElement = new XElement(currentFormat[TagName.Label], Name, new XAttribute(currentFormat[TagName.Time], Sapi.TimeToString(Time)));
			if (Ecu != null)
			{
				xElement.Add(new XAttribute(currentFormat[TagName.EcuName], Ecu.Name));
			}
			return xElement;
		}
	}

	internal Label(string name, DateTime time, Ecu ecu, Channel channel = null)
	{
		this.name = name;
		this.time = time;
		this.ecu = ecu;
		this.channel = channel;
	}

	public override string ToString()
	{
		return name;
	}

	internal static Label FromXElement(XElement element, LogFileFormatTagCollection format, ChannelBaseCollection allChannels)
	{
		XAttribute xAttribute = element.Attribute(format[TagName.EcuName]);
		Ecu ecu = ((xAttribute != null) ? Sapi.GetSapi().Ecus[xAttribute.Value] : null);
		DateTime labelTime = Sapi.TimeFromString(element.GetAttribute(format[TagName.Time].LocalName));
		Channel channel = ((allChannels != null && xAttribute != null) ? allChannels.FirstOrDefault((Channel c) => c.Ecu == ecu && c.ActiveAtTime(labelTime)) : null);
		return new Label(element.Value, labelTime, ecu, channel);
	}

	internal static LogMetadataItem ExtractMetadata(XmlReader xmlReader)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		string attribute = xmlReader.GetAttribute(currentFormat[TagName.EcuName].LocalName);
		string attribute2 = xmlReader.GetAttribute(currentFormat[TagName.Time].LocalName);
		string content = xmlReader.ReadElementContentAsString();
		return new LogMetadataItem(LogMetadataType.Label, attribute, content, attribute2);
	}
}
