using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal sealed class PartNumberErrorDialog : WebBrowserDialog
{
	private Channel channel;

	private Dictionary<string, CaesarException> exceptions;

	public PartNumberErrorDialog(Channel channel, Dictionary<string, CaesarException> exceptions)
		: base(Resources.PartNumberErrorDialog_Title, (Action<XmlWriter>)null)
	{
		this.channel = channel;
		this.exceptions = exceptions;
	}

	protected override void UpdateContent(XmlWriter writer)
	{
		writer.WriteStartElement("h1");
		writer.WriteStartElement("span");
		writer.WriteStartAttribute("class");
		writer.WriteString("warning");
		writer.WriteEndAttribute();
		writer.WriteEndElement();
		writer.WriteString(channel.Ecu.Name);
		writer.WriteEndElement();
		WriteHighlightedContent(writer, new string[3][]
		{
			new string[2]
			{
				Resources.PartNumberErrorDialog_SoftwareVersion,
				SapiManager.GetSoftwareVersion(channel)
			},
			new string[2]
			{
				Resources.PartNumberErrorDialog_PartNumber,
				PartExtensions.ToFlashKeyStyleString(new Part(SapiManager.GetSoftwarePartNumber(channel)))
			},
			new string[2]
			{
				Resources.PartNumberErrorDialog_DiagnosticVariant,
				channel.DiagnosisVariant.Name
			}
		});
		writer.WriteStartElement("p");
		writer.WriteString(Resources.PartNumberErrorDialog_PrefixText);
		writer.WriteEndElement();
		writer.WriteStartElement("table");
		writer.WriteStartElement("tr");
		writer.WriteStartElement("th");
		writer.WriteString(Resources.PartNumberErrorDialog_PartNumber);
		writer.WriteEndElement();
		writer.WriteStartElement("th");
		writer.WriteString(Resources.PartNumberErrorDialog_Meaning);
		writer.WriteEndElement();
		writer.WriteStartElement("th");
		writer.WriteString(Resources.PartNumberErrorDialog_Domain);
		writer.WriteEndElement();
		writer.WriteStartElement("th");
		writer.WriteString(Resources.PartNumberErrorDialog_Fragment);
		writer.WriteEndElement();
		writer.WriteStartElement("th");
		writer.WriteString(Resources.PartNumberErrorDialog_Error);
		writer.WriteEndElement();
		writer.WriteStartElement("th");
		writer.WriteString(Resources.PartNumberErrorDialog_Notes);
		writer.WriteEndElement();
		writer.WriteEndElement();
		foreach (KeyValuePair<string, CaesarException> exception in exceptions)
		{
			CodingChoice codingForPart = channel.CodingParameterGroups.GetCodingForPart(exception.Key);
			writer.WriteStartElement("tr");
			writer.WriteStartElement("td");
			writer.WriteString(exception.Key);
			writer.WriteEndElement();
			writer.WriteStartElement("td");
			writer.WriteString((codingForPart != null) ? codingForPart.Meaning : Resources.PartNumberErrorDialog_NotApplicable);
			writer.WriteEndElement();
			writer.WriteStartElement("td");
			writer.WriteString((codingForPart != null) ? codingForPart.ParameterGroup.Qualifier : Resources.PartNumberErrorDialog_NotApplicable);
			writer.WriteEndElement();
			writer.WriteStartElement("td");
			writer.WriteString((codingForPart != null && codingForPart.Parameter != null) ? codingForPart.Parameter.Name : Resources.PartNumberErrorDialog_NotApplicable);
			writer.WriteEndElement();
			writer.WriteStartElement("td");
			writer.WriteString(exception.Value.Message);
			writer.WriteEndElement();
			writer.WriteStartElement("td");
			GetNotes(writer, codingForPart, exception.Value);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
		CodingFile codingFile = SapiManager.GlobalInstance.Sapi.CodingFiles.FirstOrDefault((CodingFile cf) => cf.Ecus.Contains(channel.Ecu));
		if (codingFile != null)
		{
			WriteHighlightedContent(writer, new string[3][]
			{
				new string[2]
				{
					Resources.PartNumberErrorDialog_CodingFileName,
					Path.GetFileName(codingFile.FileName)
				},
				new string[2]
				{
					Resources.PartNumberErrorDialog_CodingFileVersion,
					codingFile.Version
				},
				new string[2]
				{
					Resources.PartNumberErrorDialog_CodingFileDate,
					codingFile.Date
				}
			});
			writer.WriteStartElement("p");
			writer.WriteString(codingFile.Preamble);
			writer.WriteEndElement();
		}
	}

	private void GetNotes(XmlWriter writer, CodingChoice codingChoice, CaesarException exception)
	{
		if (codingChoice != null)
		{
			switch (exception.ErrorNumber)
			{
			case 6130L:
			{
				Parameter itemFirstInGroup2 = channel.Parameters.GetItemFirstInGroup(codingChoice.ParameterGroup.Qualifier);
				if (itemFirstInGroup2 != null)
				{
					int count = new Dump(codingChoice.RawValue).Data.Count;
					writer.WriteStartElement("li");
					writer.WriteString(string.Format(CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatDefaultStringLength, count));
					writer.WriteEndElement();
					int? groupLength = itemFirstInGroup2.GroupLength;
					writer.WriteStartElement("li");
					writer.WriteString(string.Format(CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatDomainLength, channel.DiagnosisVariant, groupLength.HasValue ? groupLength.Value.ToString(CultureInfo.CurrentCulture) : "unknown"));
					writer.WriteEndElement();
					return;
				}
				break;
			}
			case 6119L:
				if (codingChoice.ParameterGroup.DiagnosisVariants.Contains(channel.DiagnosisVariant))
				{
					Parameter itemFirstInGroup = channel.Parameters.GetItemFirstInGroup(codingChoice.ParameterGroup.Qualifier);
					if (itemFirstInGroup != null)
					{
						if (codingChoice.Parameter != null)
						{
							IEnumerable<Parameter> source2 = channel.Parameters.Where((Parameter p) => p.GroupQualifier == codingChoice.ParameterGroup.Qualifier);
							if (!source2.Any((Parameter f) => f.Name == codingChoice.Parameter.Name || f.Qualifier == CreateQualifierFromName(codingChoice.Parameter.Name)))
							{
								writer.WriteString(string.Format(CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatFragmentNotExist, channel.DiagnosisVariant));
								return;
							}
						}
						break;
					}
					writer.WriteString(string.Format(CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatDomainNotExist, channel.DiagnosisVariant));
					return;
				}
				writer.WriteString(string.Format(CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatVariantNotInPermittedList, channel.DiagnosisVariant));
				return;
			case 6127L:
			{
				CodingParameterGroup group = codingChoice.ParameterGroup;
				IEnumerable<Parameter> source = channel.Parameters.Where((Parameter p) => p.GroupQualifier == group.Qualifier && p.Value != null);
				if (source.Any())
				{
					writer.WriteString(string.Format(CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatFragmentBeforeDefaultString, string.Join(", ", source.Select((Parameter p) => p.Name))));
					return;
				}
				break;
			}
			}
		}
		writer.WriteString(Resources.PartNumberErrorDialog_NotApplicable);
	}

	private static void WriteHighlightedContent(XmlWriter writer, string[][] content)
	{
		writer.WriteStartElement("p");
		foreach (string[] array in content)
		{
			writer.WriteString(array[0] + ": ");
			writer.WriteStartElement("b");
			writer.WriteString(array[1] + " ");
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
	}

	private static string CreateQualifierFromName(string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in name)
		{
			if (char.IsLetterOrDigit(c))
			{
				stringBuilder.Append(c);
			}
			else
			{
				stringBuilder.Append("_");
			}
		}
		return stringBuilder.ToString();
	}
}
