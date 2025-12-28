// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.PartNumberErrorDialog
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal sealed class PartNumberErrorDialog : WebBrowserDialog
{
  private Channel channel;
  private Dictionary<string, CaesarException> exceptions;

  public PartNumberErrorDialog(Channel channel, Dictionary<string, CaesarException> exceptions)
    : base(Resources.PartNumberErrorDialog_Title, (Action<XmlWriter>) null)
  {
    this.channel = channel;
    this.exceptions = exceptions;
  }

  protected virtual void UpdateContent(XmlWriter writer)
  {
    writer.WriteStartElement("h1");
    writer.WriteStartElement("span");
    writer.WriteStartAttribute("class");
    writer.WriteString("warning");
    writer.WriteEndAttribute();
    writer.WriteEndElement();
    writer.WriteString(this.channel.Ecu.Name);
    writer.WriteEndElement();
    PartNumberErrorDialog.WriteHighlightedContent(writer, new string[3][]
    {
      new string[2]
      {
        Resources.PartNumberErrorDialog_SoftwareVersion,
        SapiManager.GetSoftwareVersion(this.channel)
      },
      new string[2]
      {
        Resources.PartNumberErrorDialog_PartNumber,
        PartExtensions.ToFlashKeyStyleString(new Part(SapiManager.GetSoftwarePartNumber(this.channel)))
      },
      new string[2]
      {
        Resources.PartNumberErrorDialog_DiagnosticVariant,
        this.channel.DiagnosisVariant.Name
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
    foreach (KeyValuePair<string, CaesarException> exception in this.exceptions)
    {
      CodingChoice codingForPart = this.channel.CodingParameterGroups.GetCodingForPart(exception.Key);
      writer.WriteStartElement("tr");
      writer.WriteStartElement("td");
      writer.WriteString(exception.Key);
      writer.WriteEndElement();
      writer.WriteStartElement("td");
      writer.WriteString(codingForPart != null ? codingForPart.Meaning : Resources.PartNumberErrorDialog_NotApplicable);
      writer.WriteEndElement();
      writer.WriteStartElement("td");
      writer.WriteString(codingForPart != null ? codingForPart.ParameterGroup.Qualifier : Resources.PartNumberErrorDialog_NotApplicable);
      writer.WriteEndElement();
      writer.WriteStartElement("td");
      writer.WriteString(codingForPart == null || codingForPart.Parameter == null ? Resources.PartNumberErrorDialog_NotApplicable : codingForPart.Parameter.Name);
      writer.WriteEndElement();
      writer.WriteStartElement("td");
      writer.WriteString(exception.Value.Message);
      writer.WriteEndElement();
      writer.WriteStartElement("td");
      this.GetNotes(writer, codingForPart, exception.Value);
      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    writer.WriteEndElement();
    CodingFile codingFile = SapiManager.GlobalInstance.Sapi.CodingFiles.FirstOrDefault<CodingFile>((Func<CodingFile, bool>) (cf => cf.Ecus.Contains(this.channel.Ecu)));
    if (codingFile == null)
      return;
    PartNumberErrorDialog.WriteHighlightedContent(writer, new string[3][]
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

  private void GetNotes(XmlWriter writer, CodingChoice codingChoice, CaesarException exception)
  {
    if (codingChoice != null)
    {
      switch (exception.ErrorNumber)
      {
        case 6119:
          if (codingChoice.ParameterGroup.DiagnosisVariants.Contains(this.channel.DiagnosisVariant))
          {
            if (this.channel.Parameters.GetItemFirstInGroup(codingChoice.ParameterGroup.Qualifier) != null)
            {
              if (codingChoice.Parameter != null && !this.channel.Parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.GroupQualifier == codingChoice.ParameterGroup.Qualifier)).Any<Parameter>((Func<Parameter, bool>) (f => f.Name == codingChoice.Parameter.Name || f.Qualifier == PartNumberErrorDialog.CreateQualifierFromName(codingChoice.Parameter.Name))))
              {
                writer.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatFragmentNotExist, (object) this.channel.DiagnosisVariant));
                return;
              }
              break;
            }
            writer.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatDomainNotExist, (object) this.channel.DiagnosisVariant));
            return;
          }
          writer.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatVariantNotInPermittedList, (object) this.channel.DiagnosisVariant));
          return;
        case 6127:
          CodingParameterGroup group = codingChoice.ParameterGroup;
          IEnumerable<Parameter> source = this.channel.Parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.GroupQualifier == group.Qualifier && p.Value != null));
          if (source.Any<Parameter>())
          {
            writer.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatFragmentBeforeDefaultString, (object) string.Join(", ", source.Select<Parameter, string>((Func<Parameter, string>) (p => p.Name)))));
            return;
          }
          break;
        case 6130:
          Parameter itemFirstInGroup = this.channel.Parameters.GetItemFirstInGroup(codingChoice.ParameterGroup.Qualifier);
          if (itemFirstInGroup != null)
          {
            int count = new Dump(codingChoice.RawValue).Data.Count;
            writer.WriteStartElement("li");
            writer.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatDefaultStringLength, (object) count));
            writer.WriteEndElement();
            int? groupLength = itemFirstInGroup.GroupLength;
            writer.WriteStartElement("li");
            writer.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PartNumberErrorDialog_FormatDomainLength, (object) this.channel.DiagnosisVariant, groupLength.HasValue ? (object) groupLength.Value.ToString((IFormatProvider) CultureInfo.CurrentCulture) : (object) "unknown"));
            writer.WriteEndElement();
            return;
          }
          break;
      }
    }
    writer.WriteString(Resources.PartNumberErrorDialog_NotApplicable);
  }

  private static void WriteHighlightedContent(XmlWriter writer, string[][] content)
  {
    writer.WriteStartElement("p");
    foreach (string[] strArray in content)
    {
      writer.WriteString(strArray[0] + ": ");
      writer.WriteStartElement("b");
      writer.WriteString(strArray[1] + " ");
      writer.WriteEndElement();
    }
    writer.WriteEndElement();
  }

  private static string CreateQualifierFromName(string name)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < name.Length; ++index)
    {
      char c = name[index];
      if (char.IsLetterOrDigit(c))
        stringBuilder.Append(c);
      else
        stringBuilder.Append("_");
    }
    return stringBuilder.ToString();
  }
}
