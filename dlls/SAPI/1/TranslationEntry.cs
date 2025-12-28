// Decompiled with JetBrains decompiler
// Type: SapiLayer1.TranslationEntry
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public struct TranslationEntry
{
  public TranslationEntry(string qualifier, string original)
    : this()
  {
    this.Qualifier = qualifier;
    this.Original = original;
  }

  public TranslationEntry(string qualifier, string original, string translation)
    : this()
  {
    this.Qualifier = qualifier;
    this.Original = original;
    this.Translation = translation;
  }

  public string Qualifier { private set; get; }

  public string Original { private set; get; }

  public string Translation { private set; get; }

  public static bool operator ==(TranslationEntry left, TranslationEntry right)
  {
    return left.Equals((object) right);
  }

  public static bool operator !=(TranslationEntry left, TranslationEntry right)
  {
    return !left.Equals((object) right);
  }

  public override bool Equals(object obj) => base.Equals(obj);

  public override int GetHashCode() => base.GetHashCode();

  internal static string GetTranslationFileName(string ecuName, CultureInfo culture)
  {
    return Path.Combine(Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.xml", (object) ecuName, culture != null ? (object) culture.Neutralize().Name : (object) "*"));
  }

  internal static IEnumerable<TranslationEntry> ReadTranslationFile(
    string ecuName,
    CultureInfo culture)
  {
    XElement xelement = XDocument.Load(TranslationEntry.GetTranslationFileName(ecuName, culture)).Element((XName) "Translations");
    if (!(xelement.Attribute((XName) "Ecu").Value == ecuName))
      throw new NotSupportedException("Ecu name in translation file does not match");
    if (xelement.Attribute((XName) "Code").Value == culture.Neutralize().Name)
      return xelement.Elements((XName) "Translation").Elements<XElement>((XName) "Text").Select<XElement, TranslationEntry>((Func<XElement, TranslationEntry>) (element => new TranslationEntry(element.Attribute((XName) "Qualifier").Value, element.Element((XName) "Original") != null ? element.Element((XName) "Original").Value : string.Empty, element.Element((XName) "Translation").Value)));
    throw new NotSupportedException("Language name in translation file does not match");
  }

  internal static void WriteTranslationFile(
    string ecuName,
    CultureInfo culture,
    string descriptionDataVersion,
    IEnumerable<TranslationEntry> translations,
    bool emitEmptyTranslations)
  {
    string translationFileName = TranslationEntry.GetTranslationFileName(ecuName, culture);
    XDocument xdocument;
    XElement content1;
    if (File.Exists(translationFileName))
    {
      xdocument = XDocument.Load(translationFileName);
      content1 = xdocument.Element((XName) "Translations");
    }
    else
    {
      xdocument = new XDocument();
      content1 = new XElement((XName) "Translations", new object[2]
      {
        (object) new XAttribute((XName) "Ecu", (object) ecuName),
        (object) new XAttribute((XName) "Code", (object) culture.Neutralize().Name)
      });
      xdocument.Add((object) content1);
    }
    XElement content2 = new XElement((XName) "Translation", new object[2]
    {
      (object) new XAttribute((XName) "Date", (object) Sapi.TimeToString(Sapi.Now)),
      (object) new XAttribute((XName) "Version", (object) descriptionDataVersion)
    });
    content1.Add((object) content2);
    foreach (TranslationEntry translation in translations)
    {
      if (emitEmptyTranslations || !string.IsNullOrEmpty(translation.Translation) && translation.Translation.Reduce() != translation.Original.Reduce())
        content2.Add((object) new XElement((XName) "Text", new object[3]
        {
          (object) new XAttribute((XName) "Qualifier", (object) translation.Qualifier),
          (object) new XElement((XName) "Original", (object) translation.Original),
          (object) new XElement((XName) "Translation", (object) translation.Translation)
        }));
    }
    xdocument.Save(translationFileName);
  }
}
