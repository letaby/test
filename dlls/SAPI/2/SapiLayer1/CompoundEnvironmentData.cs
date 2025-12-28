using System.Collections.Specialized;
using System.Xml;

namespace SapiLayer1;

internal class CompoundEnvironmentData
{
	private string qualifier;

	private string name;

	private bool hideComponents;

	private string units;

	private string formatString;

	private StringCollection referenced;

	internal string Name => name;

	internal string Qualifier => qualifier;

	internal string Units => units;

	internal string FormatString => formatString;

	internal StringCollection Referenced => referenced;

	internal bool HideComponents => hideComponents;

	internal CompoundEnvironmentData(XmlNode environmentDataNode)
	{
		referenced = new StringCollection();
		qualifier = environmentDataNode.Attributes.GetNamedItem("Qualifier").InnerText;
		name = environmentDataNode.Attributes.GetNamedItem("Name").InnerText;
		formatString = environmentDataNode.Attributes.GetNamedItem("FormatString").InnerText;
		XmlNode namedItem = environmentDataNode.Attributes.GetNamedItem("Units");
		if (namedItem == null)
		{
			units = string.Empty;
		}
		else
		{
			units = namedItem.InnerText;
		}
		XmlNode namedItem2 = environmentDataNode.Attributes.GetNamedItem("HideComponents");
		if (namedItem2 == null)
		{
			hideComponents = true;
		}
		else if (!bool.TryParse(namedItem2.InnerText, out hideComponents))
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to parse HideComponents attribute");
		}
		XmlNodeList xmlNodeList = environmentDataNode.SelectNodes("Reference");
		for (int i = 0; i < xmlNodeList.Count; i++)
		{
			XmlNode xmlNode = xmlNodeList.Item(i);
			referenced.Add(xmlNode.Attributes.GetNamedItem("Qualifier").InnerText);
		}
	}
}
