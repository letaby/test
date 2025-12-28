// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ConfigurationItemCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml;

#nullable disable
namespace SapiLayer1;

public sealed class ConfigurationItemCollection : ReadOnlyCollection<ConfigurationItem>
{
  private XmlNode xml;

  internal ConfigurationItemCollection()
    : base((IList<ConfigurationItem>) new List<ConfigurationItem>())
  {
    this.Items.Add(new ConfigurationItem("CBFFiles", string.Empty));
    this.Items.Add(new ConfigurationItem("CFFFiles", string.Empty));
    this.Items.Add(new ConfigurationItem("CCFFiles", string.Empty));
    this.Items.Add(new ConfigurationItem("CTFFiles", string.Empty));
    ConfigurationItem configurationItem1 = new ConfigurationItem("HardwareType", "A+J+Y");
    this.Items.Add(configurationItem1);
    configurationItem1.Choices.Add(new Choice("A+J+Y", (object) "A+J+Y"));
    configurationItem1.Choices.Add(new Choice("AE+J+Y", (object) "AE+J+Y"));
    configurationItem1.Choices.Add(new Choice("A", (object) "A"));
    configurationItem1.Choices.Add(new Choice("AE", (object) "AE"));
    configurationItem1.Choices.Add(new Choice("C", (object) "C"));
    configurationItem1.Choices.Add(new Choice("D", (object) "D"));
    configurationItem1.Choices.Add(new Choice("J", (object) "J"));
    configurationItem1.Choices.Add(new Choice("PDU-API", (object) "P"));
    configurationItem1.Choices.Add(new Choice("SDconnect", (object) "W"));
    configurationItem1.Choices.Add(new Choice("X", (object) "X"));
    configurationItem1.Choices.Add(new Choice("Y", (object) "Y"));
    configurationItem1.Choices.Add(new Choice("Z", (object) "Z"));
    ConfigurationItem configurationItem2 = new ConfigurationItem("PinMapping", "No");
    this.Items.Add(configurationItem2);
    configurationItem2.Choices.Add(new Choice("Yes", (object) 1));
    configurationItem2.Choices.Add(new Choice("No", (object) 0));
    ConfigurationItem configurationItem3 = new ConfigurationItem("FlashBoot", "No");
    this.Items.Add(configurationItem3);
    configurationItem3.Choices.Add(new Choice("Yes", (object) 1));
    configurationItem3.Choices.Add(new Choice("No", (object) 0));
    ConfigurationItem configurationItem4 = new ConfigurationItem("GPDFlashCaching", "No");
    this.Items.Add(configurationItem4);
    configurationItem4.Choices.Add(new Choice("Yes", (object) 1));
    configurationItem4.Choices.Add(new Choice("No", (object) 0));
    ConfigurationItem configurationItem5 = new ConfigurationItem("TLSlave", "Yes");
    this.Items.Add(configurationItem5);
    configurationItem5.Choices.Add(new Choice("Yes", (object) 1));
    configurationItem5.Choices.Add(new Choice("No", (object) 0));
    ConfigurationItem configurationItem6 = new ConfigurationItem("Units", "US");
    this.Items.Add(configurationItem6);
    configurationItem6.Choices.Add(new Choice("Metric", (object) 0));
    configurationItem6.Choices.Add(new Choice("US", (object) 1));
    ConfigurationItem configurationItem7 = new ConfigurationItem("DebugLevel", "0");
    this.Items.Add(configurationItem7);
    configurationItem7.Choices.Add(new Choice("0", (object) 0));
    configurationItem7.Choices.Add(new Choice("1", (object) 1));
    configurationItem7.Choices.Add(new Choice("2", (object) 2));
    configurationItem7.Choices.Add(new Choice("3", (object) 3));
    this.Items.Add(new ConfigurationItem("PartXIPAddress", "192.168.0.1"));
    this.Items.Add(new ConfigurationItem("PartXPort", "4000"));
    this.Items.Add(new ConfigurationItem("PassThruDevice", "1"));
    this.Items.Add(new ConfigurationItem("PassThruHardwareNamePrefix", "SID"));
    this.Items.Add(new ConfigurationItem("RollCallDebugLevel", "0"));
    this.Items.Add(new ConfigurationItem("RollCallRestrictJ1939Address", "no"));
    this.Items.Add(new ConfigurationItem("RollCallRestrictJ1708Address", "no"));
    this.Items.Add(new ConfigurationItem("McdDtsPath", ""));
    this.Items.Add(new ConfigurationItem("McdDtsPath64", ""));
    this.Items.Add(new ConfigurationItem("McdRootDescriptionFile", ""));
    this.Items.Add(new ConfigurationItem("McdSessionProjectPath", ""));
    this.Items.Add(new ConfigurationItem("McdProjectName", "SAPI"));
    this.Items.Add(new ConfigurationItem("McdVehicleInformationTable", "DefaultVIT"));
    this.Items.Add(new ConfigurationItem("McdEthernetDetectionString", "PDU_IOCTL='PDU_IOCTL_VEHICLE_ID_REQUEST' CombinationMode='DoIP-Collection' VehicleDiscoveryTime='2000'"));
    this.Items.Add(new ConfigurationItem("PassThru_P2_Offset_CAN", "2000"));
    this.Items.Add(new ConfigurationItem("PassThru_P2_Offset_KLine", "2000"));
  }

  public void Load(string path)
  {
    int ver = 0;
    DateTime dt = DateTime.MinValue;
    this.xml = Sapi.ReadSapiXmlFile(path, "ConfigurationItems", out ver, out dt);
    if (ver <= 0)
      throw new InvalidOperationException("Configuration items not initialized - version did not match");
    XmlNode xmlNode1 = this.xml.SelectSingleNode("ConfigurationItems");
    foreach (ConfigurationItem configurationItem in (ReadOnlyCollection<ConfigurationItem>) this)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ConfigurationItem[@Name = \"{0}\"]", (object) configurationItem.Name);
      XmlNode xmlNode2 = xmlNode1.SelectSingleNode(xpath);
      if (xmlNode2 != null)
        configurationItem.Value = xmlNode2.InnerText;
    }
  }

  public void Save(string path)
  {
    if (this.xml != null)
      this.xml.RemoveChild(this.xml.SelectSingleNode("ConfigurationItems"));
    else
      this.xml = Sapi.GetSapi().InitSapiXmlFile("ConfigurationItems", 1, Sapi.Now, false);
    XmlDocument ownerDocument = this.xml.OwnerDocument;
    XmlNode node1 = ownerDocument.CreateNode(XmlNodeType.Element, "ConfigurationItems", string.Empty);
    foreach (ConfigurationItem configurationItem in (ReadOnlyCollection<ConfigurationItem>) this)
    {
      XmlNode node2 = ownerDocument.CreateNode(XmlNodeType.Element, "ConfigurationItem", string.Empty);
      XmlAttribute attribute = ownerDocument.CreateAttribute("Name");
      attribute.InnerText = configurationItem.Name;
      node2.InnerText = configurationItem.Value;
      node2.Attributes.Append(attribute);
      node1.AppendChild(node2);
    }
    this.xml.InsertAfter(node1, (XmlNode) null);
    ownerDocument.Save(path);
  }

  public ConfigurationItem this[string name]
  {
    get
    {
      if (Environment.Is64BitProcess && name == "McdDtsPath")
        name = "McdDtsPath64";
      return this.FirstOrDefault<ConfigurationItem>((Func<ConfigurationItem, bool>) (item => string.Equals(item.Name, name, StringComparison.Ordinal)));
    }
  }
}
