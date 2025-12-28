using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace SapiLayer1;

public sealed class ConfigurationItemCollection : ReadOnlyCollection<ConfigurationItem>
{
	private XmlNode xml;

	public ConfigurationItem this[string name]
	{
		get
		{
			if (Environment.Is64BitProcess)
			{
				string text = name;
				if (text == "McdDtsPath")
				{
					name = "McdDtsPath64";
				}
			}
			return this.FirstOrDefault((ConfigurationItem item) => string.Equals(item.Name, name, StringComparison.Ordinal));
		}
	}

	internal ConfigurationItemCollection()
		: base((IList<ConfigurationItem>)new List<ConfigurationItem>())
	{
		base.Items.Add(new ConfigurationItem("CBFFiles", string.Empty));
		base.Items.Add(new ConfigurationItem("CFFFiles", string.Empty));
		base.Items.Add(new ConfigurationItem("CCFFiles", string.Empty));
		base.Items.Add(new ConfigurationItem("CTFFiles", string.Empty));
		ConfigurationItem configurationItem = new ConfigurationItem("HardwareType", "A+J+Y");
		base.Items.Add(configurationItem);
		configurationItem.Choices.Add(new Choice("A+J+Y", "A+J+Y"));
		configurationItem.Choices.Add(new Choice("AE+J+Y", "AE+J+Y"));
		configurationItem.Choices.Add(new Choice("A", "A"));
		configurationItem.Choices.Add(new Choice("AE", "AE"));
		configurationItem.Choices.Add(new Choice("C", "C"));
		configurationItem.Choices.Add(new Choice("D", "D"));
		configurationItem.Choices.Add(new Choice("J", "J"));
		configurationItem.Choices.Add(new Choice("PDU-API", "P"));
		configurationItem.Choices.Add(new Choice("SDconnect", "W"));
		configurationItem.Choices.Add(new Choice("X", "X"));
		configurationItem.Choices.Add(new Choice("Y", "Y"));
		configurationItem.Choices.Add(new Choice("Z", "Z"));
		ConfigurationItem configurationItem2 = new ConfigurationItem("PinMapping", "No");
		base.Items.Add(configurationItem2);
		configurationItem2.Choices.Add(new Choice("Yes", 1));
		configurationItem2.Choices.Add(new Choice("No", 0));
		ConfigurationItem configurationItem3 = new ConfigurationItem("FlashBoot", "No");
		base.Items.Add(configurationItem3);
		configurationItem3.Choices.Add(new Choice("Yes", 1));
		configurationItem3.Choices.Add(new Choice("No", 0));
		ConfigurationItem configurationItem4 = new ConfigurationItem("GPDFlashCaching", "No");
		base.Items.Add(configurationItem4);
		configurationItem4.Choices.Add(new Choice("Yes", 1));
		configurationItem4.Choices.Add(new Choice("No", 0));
		ConfigurationItem configurationItem5 = new ConfigurationItem("TLSlave", "Yes");
		base.Items.Add(configurationItem5);
		configurationItem5.Choices.Add(new Choice("Yes", 1));
		configurationItem5.Choices.Add(new Choice("No", 0));
		ConfigurationItem configurationItem6 = new ConfigurationItem("Units", "US");
		base.Items.Add(configurationItem6);
		configurationItem6.Choices.Add(new Choice("Metric", 0));
		configurationItem6.Choices.Add(new Choice("US", 1));
		ConfigurationItem configurationItem7 = new ConfigurationItem("DebugLevel", "0");
		base.Items.Add(configurationItem7);
		configurationItem7.Choices.Add(new Choice("0", 0));
		configurationItem7.Choices.Add(new Choice("1", 1));
		configurationItem7.Choices.Add(new Choice("2", 2));
		configurationItem7.Choices.Add(new Choice("3", 3));
		base.Items.Add(new ConfigurationItem("PartXIPAddress", "192.168.0.1"));
		base.Items.Add(new ConfigurationItem("PartXPort", "4000"));
		base.Items.Add(new ConfigurationItem("PassThruDevice", "1"));
		base.Items.Add(new ConfigurationItem("PassThruHardwareNamePrefix", "SID"));
		ConfigurationItem item = new ConfigurationItem("RollCallDebugLevel", "0");
		base.Items.Add(item);
		ConfigurationItem item2 = new ConfigurationItem("RollCallRestrictJ1939Address", "no");
		base.Items.Add(item2);
		ConfigurationItem item3 = new ConfigurationItem("RollCallRestrictJ1708Address", "no");
		base.Items.Add(item3);
		base.Items.Add(new ConfigurationItem("McdDtsPath", ""));
		base.Items.Add(new ConfigurationItem("McdDtsPath64", ""));
		base.Items.Add(new ConfigurationItem("McdRootDescriptionFile", ""));
		base.Items.Add(new ConfigurationItem("McdSessionProjectPath", ""));
		base.Items.Add(new ConfigurationItem("McdProjectName", "SAPI"));
		base.Items.Add(new ConfigurationItem("McdVehicleInformationTable", "DefaultVIT"));
		base.Items.Add(new ConfigurationItem("McdEthernetDetectionString", "PDU_IOCTL='PDU_IOCTL_VEHICLE_ID_REQUEST' CombinationMode='DoIP-Collection' VehicleDiscoveryTime='2000'"));
		base.Items.Add(new ConfigurationItem("PassThru_P2_Offset_CAN", "2000"));
		base.Items.Add(new ConfigurationItem("PassThru_P2_Offset_KLine", "2000"));
	}

	public void Load(string path)
	{
		int ver = 0;
		DateTime dt = DateTime.MinValue;
		xml = Sapi.ReadSapiXmlFile(path, "ConfigurationItems", out ver, out dt);
		if (ver > 0)
		{
			XmlNode xmlNode = xml.SelectSingleNode("ConfigurationItems");
			using IEnumerator<ConfigurationItem> enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				ConfigurationItem current = enumerator.Current;
				string xpath = string.Format(CultureInfo.InvariantCulture, "ConfigurationItem[@Name = \"{0}\"]", current.Name);
				XmlNode xmlNode2 = xmlNode.SelectSingleNode(xpath);
				if (xmlNode2 != null)
				{
					current.Value = xmlNode2.InnerText;
				}
			}
			return;
		}
		throw new InvalidOperationException("Configuration items not initialized - version did not match");
	}

	public void Save(string path)
	{
		if (xml != null)
		{
			XmlNode oldChild = xml.SelectSingleNode("ConfigurationItems");
			xml.RemoveChild(oldChild);
		}
		else
		{
			xml = Sapi.GetSapi().InitSapiXmlFile("ConfigurationItems", 1, Sapi.Now, addCBFVersions: false);
		}
		XmlDocument ownerDocument = xml.OwnerDocument;
		XmlNode xmlNode = ownerDocument.CreateNode(XmlNodeType.Element, "ConfigurationItems", string.Empty);
		using (IEnumerator<ConfigurationItem> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ConfigurationItem current = enumerator.Current;
				XmlNode xmlNode2 = ownerDocument.CreateNode(XmlNodeType.Element, "ConfigurationItem", string.Empty);
				XmlAttribute xmlAttribute = ownerDocument.CreateAttribute("Name");
				xmlAttribute.InnerText = current.Name;
				xmlNode2.InnerText = current.Value;
				xmlNode2.Attributes.Append(xmlAttribute);
				xmlNode.AppendChild(xmlNode2);
			}
		}
		xml.InsertAfter(xmlNode, null);
		ownerDocument.Save(path);
	}
}
