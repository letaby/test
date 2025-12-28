// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CompoundEnvironmentData
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System.Collections.Specialized;
using System.Xml;

#nullable disable
namespace SapiLayer1;

internal class CompoundEnvironmentData
{
  private string qualifier;
  private string name;
  private bool hideComponents;
  private string units;
  private string formatString;
  private StringCollection referenced;

  internal CompoundEnvironmentData(XmlNode environmentDataNode)
  {
    this.referenced = new StringCollection();
    this.qualifier = environmentDataNode.Attributes.GetNamedItem(nameof (Qualifier)).InnerText;
    this.name = environmentDataNode.Attributes.GetNamedItem(nameof (Name)).InnerText;
    this.formatString = environmentDataNode.Attributes.GetNamedItem(nameof (FormatString)).InnerText;
    XmlNode namedItem1 = environmentDataNode.Attributes.GetNamedItem(nameof (Units));
    this.units = namedItem1 != null ? namedItem1.InnerText : string.Empty;
    XmlNode namedItem2 = environmentDataNode.Attributes.GetNamedItem(nameof (HideComponents));
    if (namedItem2 == null)
      this.hideComponents = true;
    else if (!bool.TryParse(namedItem2.InnerText, out this.hideComponents))
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to parse HideComponents attribute");
    XmlNodeList xmlNodeList = environmentDataNode.SelectNodes("Reference");
    for (int index = 0; index < xmlNodeList.Count; ++index)
      this.referenced.Add(xmlNodeList.Item(index).Attributes.GetNamedItem(nameof (Qualifier)).InnerText);
  }

  internal string Name => this.name;

  internal string Qualifier => this.qualifier;

  internal string Units => this.units;

  internal string FormatString => this.formatString;

  internal StringCollection Referenced => this.referenced;

  internal bool HideComponents => this.hideComponents;
}
