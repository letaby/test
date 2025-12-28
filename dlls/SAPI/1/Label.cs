// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Label
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class Label
{
  private string name;
  private DateTime time;
  private Ecu ecu;
  private Channel channel;

  internal Label(string name, DateTime time, Ecu ecu, Channel channel = null)
  {
    this.name = name;
    this.time = time;
    this.ecu = ecu;
    this.channel = channel;
  }

  public override string ToString() => this.name;

  public string Name => this.name;

  public DateTime Time => this.time;

  public Ecu Ecu => this.ecu;

  public Channel Channel => this.channel;

  internal XElement XElement
  {
    get
    {
      LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
      XElement xelement = new XElement(currentFormat[TagName.Label], new object[2]
      {
        (object) this.Name,
        (object) new XAttribute(currentFormat[TagName.Time], (object) Sapi.TimeToString(this.Time))
      });
      if (this.Ecu != null)
        xelement.Add((object) new XAttribute(currentFormat[TagName.EcuName], (object) this.Ecu.Name));
      return xelement;
    }
  }

  internal static Label FromXElement(
    XElement element,
    LogFileFormatTagCollection format,
    ChannelBaseCollection allChannels)
  {
    XAttribute xattribute = element.Attribute(format[TagName.EcuName]);
    Ecu ecu = xattribute != null ? Sapi.GetSapi().Ecus[xattribute.Value] : (Ecu) null;
    DateTime labelTime = Sapi.TimeFromString(element.GetAttribute(format[TagName.Time].LocalName));
    Channel channel = allChannels == null || xattribute == null ? (Channel) null : allChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu == ecu && c.ActiveAtTime(labelTime)));
    return new Label(element.Value, labelTime, ecu, channel);
  }

  internal static LogMetadataItem ExtractMetadata(XmlReader xmlReader)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    string attribute1 = xmlReader.GetAttribute(currentFormat[TagName.EcuName].LocalName);
    string attribute2 = xmlReader.GetAttribute(currentFormat[TagName.Time].LocalName);
    string content = xmlReader.ReadElementContentAsString();
    return new LogMetadataItem(LogMetadataType.Label, attribute1, content, attribute2);
  }
}
