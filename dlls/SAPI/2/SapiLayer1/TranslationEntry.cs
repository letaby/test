using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SapiLayer1;

public struct TranslationEntry
{
	public string Qualifier { get; private set; }

	public string Original { get; private set; }

	public string Translation { get; private set; }

	public TranslationEntry(string qualifier, string original)
	{
		this = default(TranslationEntry);
		Qualifier = qualifier;
		Original = original;
	}

	public TranslationEntry(string qualifier, string original, string translation)
	{
		this = default(TranslationEntry);
		Qualifier = qualifier;
		Original = original;
		Translation = translation;
	}

	public static bool operator ==(TranslationEntry left, TranslationEntry right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TranslationEntry left, TranslationEntry right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	internal static string GetTranslationFileName(string ecuName, CultureInfo culture)
	{
		return Path.Combine(Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value, string.Format(CultureInfo.InvariantCulture, "{0}.{1}.xml", ecuName, (culture != null) ? culture.Neutralize().Name : "*"));
	}

	internal static IEnumerable<TranslationEntry> ReadTranslationFile(string ecuName, CultureInfo culture)
	{
		XElement xElement = XDocument.Load(GetTranslationFileName(ecuName, culture)).Element("Translations");
		if (xElement.Attribute("Ecu").Value == ecuName)
		{
			if (xElement.Attribute("Code").Value == culture.Neutralize().Name)
			{
				return from element in xElement.Elements("Translation").Elements("Text")
					select new TranslationEntry(element.Attribute("Qualifier").Value, (element.Element("Original") != null) ? element.Element("Original").Value : string.Empty, element.Element("Translation").Value);
			}
			throw new NotSupportedException("Language name in translation file does not match");
		}
		throw new NotSupportedException("Ecu name in translation file does not match");
	}

	internal static void WriteTranslationFile(string ecuName, CultureInfo culture, string descriptionDataVersion, IEnumerable<TranslationEntry> translations, bool emitEmptyTranslations)
	{
		string translationFileName = GetTranslationFileName(ecuName, culture);
		XDocument xDocument;
		XElement xElement;
		if (File.Exists(translationFileName))
		{
			xDocument = XDocument.Load(translationFileName);
			xElement = xDocument.Element("Translations");
		}
		else
		{
			xDocument = new XDocument();
			xElement = new XElement("Translations", new XAttribute("Ecu", ecuName), new XAttribute("Code", culture.Neutralize().Name));
			xDocument.Add(xElement);
		}
		XElement xElement2 = new XElement("Translation", new XAttribute("Date", Sapi.TimeToString(Sapi.Now)), new XAttribute("Version", descriptionDataVersion));
		xElement.Add(xElement2);
		foreach (TranslationEntry translation in translations)
		{
			if (emitEmptyTranslations || (!string.IsNullOrEmpty(translation.Translation) && translation.Translation.Reduce() != translation.Original.Reduce()))
			{
				xElement2.Add(new XElement("Text", new XAttribute("Qualifier", translation.Qualifier), new XElement("Original", translation.Original), new XElement("Translation", translation.Translation)));
			}
		}
		xDocument.Save(translationFileName);
	}
}
