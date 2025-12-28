// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.UnitPartNumberViewDialog
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal sealed class UnitPartNumberViewDialog : WebBrowserDialog
{
  private UnitInformation downloadUnit;
  private UnitInformation uploadUnit;

  public UnitPartNumberViewDialog(UnitInformation uploadUnit, UnitInformation downloadUnit)
    : base(Resources.UnitPartNumberViewDialog_Title, (Action<XmlWriter>) null)
  {
    this.uploadUnit = uploadUnit;
    this.downloadUnit = downloadUnit;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    Rectangle bounds = Screen.FromControl((Control) this).Bounds;
    ((Form) this).Size = new Size(bounds.Width * 8 / 10, bounds.Height * 8 / 10);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((Form) this).OnLoad(e));
  }

  private static void WriteDeviceContent(
    XmlWriter writer,
    List<Tuple<EdexConfigurationInformation, bool, string>> sources)
  {
    Ecu ecu = SapiManager.GetEcuByName(sources[0].Item1.DeviceName);
    CodingFile codingFile = ecu != null ? SapiManager.GlobalInstance.Sapi.CodingFiles.Where<CodingFile>((Func<CodingFile, bool>) (cf => cf.Ecus.Contains(ecu))).FirstOrDefault<CodingFile>() : (CodingFile) null;
    string[] sourceBackgroundColors = new string[5]
    {
      "#EBF5FB",
      "#FEF9E7",
      "#E8F6F3",
      "#EAFAF1",
      "#F4ECF7"
    };
    WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", ecu.DisplayName, (Action) (() =>
    {
      if (codingFile != null)
      {
        List<IList<EdexSettingItem>> list1 = sources.Select<Tuple<EdexConfigurationInformation, bool, string>, IList<EdexSettingItem>>((Func<Tuple<EdexConfigurationInformation, bool, string>, IList<EdexSettingItem>>) (source => source.Item1.GetSettingsAndProposedSettings(codingFile.CodingParameterGroups, true))).ToList<IList<EdexSettingItem>>();
        if (list1.Any<IList<EdexSettingItem>>((Func<IList<EdexSettingItem>, bool>) (relevantSettingsForSource => relevantSettingsForSource.Any<EdexSettingItem>())))
        {
          List<IEnumerable<CodingChoice>> list2 = list1.Select<IList<EdexSettingItem>, IEnumerable<CodingChoice>>((Func<IList<EdexSettingItem>, IEnumerable<CodingChoice>>) (rs => rs.Select<EdexSettingItem, CodingChoice>((Func<EdexSettingItem, CodingChoice>) (si => codingFile.CodingParameterGroups.GetCodingForPart(si.PartNumber.ToString()))).Where<CodingChoice>((Func<CodingChoice, bool>) (cc => cc != null)))).ToList<IEnumerable<CodingChoice>>();
          writer.WriteStartElement("table");
          writer.WriteAttributeString("style", "margin-top:5px;margin-bottom:5px;");
          writer.WriteStartElement("tr");
          writer.WriteStartElement("th");
          writer.WriteAttributeString("rowspan", "2");
          writer.WriteAttributeString("colspan", "2");
          writer.WriteRaw("&nbsp;");
          writer.WriteEndElement();
          foreach (IGrouping<bool, Tuple<EdexConfigurationInformation, bool, string>> source in sources.GroupBy<Tuple<EdexConfigurationInformation, bool, string>, bool>((Func<Tuple<EdexConfigurationInformation, bool, string>, bool>) (source => source.Item2)))
          {
            writer.WriteStartElement("th");
            writer.WriteAttributeString("colspan", (2 * source.Count<Tuple<EdexConfigurationInformation, bool, string>>()).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            writer.WriteString(source.Key ? Resources.UnitPartNumberViewDialog_UploadData : Resources.UnitPartNumberViewDialog_DownloadData);
            writer.WriteEndElement();
          }
          writer.WriteEndElement();
          writer.WriteStartElement("tr");
          foreach (Tuple<EdexConfigurationInformation, bool, string> source1 in sources)
          {
            writer.WriteStartElement("th");
            writer.WriteAttributeString("colspan", "2");
            XmlWriter xmlWriter = writer;
            string[] source2 = new string[4]
            {
              source1.Item3,
              source1.Item1.FlashwarePartNumber.ToString(),
              PartExtensions.ToHardwarePartNumberString(source1.Item1.HardwarePartNumber, ecu, true),
              null
            };
            DateTime? settingsTimestamp = source1.Item1.DiagnosticLinkSettingsTimestamp;
            string str;
            if (!settingsTimestamp.HasValue)
            {
              str = (string) null;
            }
            else
            {
              settingsTimestamp = source1.Item1.DiagnosticLinkSettingsTimestamp;
              str = settingsTimestamp.Value.ToString((IFormatProvider) CultureInfo.CurrentCulture);
            }
            source2[3] = str;
            string text = string.Join(" - ", ((IEnumerable<string>) source2).Where<string>((Func<string, bool>) (s => s != null)));
            xmlWriter.WriteString(text);
            writer.WriteEndElement();
          }
          writer.WriteEndElement();
          writer.WriteStartElement("tr");
          writer.WriteElementString("th", Resources.PartNumberErrorDialog_Domain);
          writer.WriteElementString("th", Resources.PartNumberErrorDialog_Fragment);
          sources.ForEach((Action<Tuple<EdexConfigurationInformation, bool, string>>) (source =>
          {
            writer.WriteElementString("th", Resources.PartNumberErrorDialog_PartNumber);
            writer.WriteElementString("th", Resources.PartNumberErrorDialog_Meaning);
          }));
          writer.WriteEndElement();
          foreach (CodingParameterGroup codingParameterGroup in (IEnumerable<CodingParameterGroup>) codingFile.CodingParameterGroups.OrderBy<CodingParameterGroup, string>((Func<CodingParameterGroup, string>) (cg => cg.Qualifier)))
          {
            CodingParameterGroup codingGroup = codingParameterGroup;
            List<List<CodingChoice>> list3 = list2.Select<IEnumerable<CodingChoice>, List<CodingChoice>>((Func<IEnumerable<CodingChoice>, List<CodingChoice>>) (rcc => rcc.Where<CodingChoice>((Func<CodingChoice, bool>) (cc => cc.ParameterGroup.Qualifier == codingGroup.Qualifier)).ToList<CodingChoice>())).ToList<List<CodingChoice>>();
            List<CodingParameter> list4 = list3.SelectMany<List<CodingChoice>, CodingParameter>((Func<List<CodingChoice>, IEnumerable<CodingParameter>>) (rcfg => rcfg.Select<CodingChoice, CodingParameter>((Func<CodingChoice, CodingParameter>) (cc => cc.Parameter)))).Distinct<CodingParameter>().ToList<CodingParameter>();
            bool flag = true;
            foreach (CodingParameter codingParameter1 in list4)
            {
              CodingParameter codingParameter = codingParameter1;
              List<List<CodingChoice>> list5 = list3.Select<List<CodingChoice>, List<CodingChoice>>((Func<List<CodingChoice>, List<CodingChoice>>) (rcfg => rcfg.Where<CodingChoice>((Func<CodingChoice, bool>) (cc => cc.Parameter == codingParameter)).OrderBy<CodingChoice, string>((Func<CodingChoice, string>) (cc => cc.Part.ToString())).ToList<CodingChoice>())).ToList<List<CodingChoice>>();
              writer.WriteStartElement("tr");
              writer.WriteAttributeString("style", "background:white");
              if (flag)
              {
                writer.WriteStartElement("td");
                writer.WriteAttributeString("rowspan", list4.Count<CodingParameter>().ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteString(codingGroup.Qualifier);
                writer.WriteEndElement();
                flag = false;
              }
              writer.WriteStartElement("td");
              if (list5.Any<List<CodingChoice>>((Func<List<CodingChoice>, bool>) (sourceCodingChoicesForParameter => sourceCodingChoicesForParameter.GroupBy<CodingChoice, string>((Func<CodingChoice, string>) (cc => cc.RawValue)).Count<IGrouping<string, CodingChoice>>() > 1)))
                writer.WriteAttributeString("class", "warning");
              writer.WriteStartElement("span");
              writer.WriteAttributeString("style", "color: " + (codingParameter == null ? "gray" : "black"));
              writer.WriteString(codingParameter == null ? Resources.UnitPartNumberViewDialog_DefaultString : codingParameter.Name);
              writer.WriteEndElement();
              writer.WriteEndElement();
              int index = 0;
              string str = list5.Select<List<CodingChoice>, string>((Func<List<CodingChoice>, string>) (codingChoices => string.Join(" ", codingChoices.Select<CodingChoice, string>((Func<CodingChoice, string>) (cc => cc.Part.ToString()))))).Distinct<string>().Count<string>() == 1 ? "green" : "orangered";
              foreach (List<CodingChoice> codingChoiceList in list5)
              {
                writer.WriteStartElement("td");
                writer.WriteAttributeString("style", "background:" + sourceBackgroundColors[index]);
                if (codingParameter != null && codingChoiceList.Count == 0 && list3[index].Any<CodingChoice>((Func<CodingChoice, bool>) (cc => cc.Parameter == null)))
                {
                  writer.WriteStartElement("div");
                  writer.WriteAttributeString("style", "color: " + str);
                  writer.WriteString(Resources.UnitPartNumberViewDialog_FromParent);
                  writer.WriteEndElement();
                }
                else
                {
                  foreach (CodingChoice codingChoice in codingChoiceList)
                  {
                    writer.WriteStartElement("div");
                    writer.WriteAttributeString("style", "color: " + str);
                    writer.WriteString(codingChoice.Part.ToString());
                    writer.WriteEndElement();
                  }
                }
                writer.WriteEndElement();
                writer.WriteStartElement("td");
                writer.WriteAttributeString("style", "background:" + sourceBackgroundColors[index]);
                foreach (CodingChoice codingChoice in codingChoiceList)
                {
                  writer.WriteStartElement("div");
                  writer.WriteAttributeString("style", "color: " + str);
                  writer.WriteString(codingChoice != null ? codingChoice.Meaning : string.Empty);
                  writer.WriteEndElement();
                }
                writer.WriteEndElement();
                ++index;
              }
              writer.WriteEndElement();
            }
          }
          writer.WriteEndElement();
        }
        else
          writer.WriteElementString("p", string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnitPartNumberViewDialog_FormatNoSettings, (object) sources[0].Item1.DeviceName));
      }
      else
        writer.WriteElementString("p", string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnitPartNumberViewDialog_FormatCodingFileNotFound, (object) sources[0].Item1.DeviceName));
    }));
  }

  protected virtual void UpdateContent(XmlWriter writer)
  {
    writer.WriteStartElement("h1");
    writer.WriteAttributeString("class", "heading1");
    writer.WriteString(ReprogrammingView.GetTitleString(this.uploadUnit != null ? this.uploadUnit.VehicleIdentity : this.downloadUnit.VehicleIdentity, this.uploadUnit != null ? this.uploadUnit.EngineNumber : this.downloadUnit.EngineNumber));
    writer.WriteEndElement();
    if (this.uploadUnit != null)
    {
      foreach (SettingsInformation settingsInformation in (IEnumerable<SettingsInformation>) this.uploadUnit.SettingsInformation.Where<SettingsInformation>((Func<SettingsInformation, bool>) (s => UnitInformation.GetDataSource(s.Device) == 2)).OrderBy<SettingsInformation, int>((Func<SettingsInformation, int>) (s => GetPriority(s.Device))))
      {
        if (FileNameInformation.FromName(settingsInformation.FileName, (FileNameInformation.FileType) 1).SettingsFileFormatType == 3)
        {
          ArrayList arrayList = new ArrayList();
          List<Tuple<EdexConfigurationInformation, bool, string>> sources = new List<Tuple<EdexConfigurationInformation, bool, string>>();
          EdexConfigurationInformation uei = EdexConfigurationXmlDocument.Deserialize((EdexSettingsType) 4, Path.Combine(Directories.DrumrollUploadData, FileEncryptionProvider.EncryptFileName(settingsInformation.FileName)), (DeviceInformation) null, arrayList);
          sources.Add(Tuple.Create<EdexConfigurationInformation, bool, string>(uei, true, (string) null));
          if (this.downloadUnit != null)
          {
            DeviceInformation informationForDevice = this.downloadUnit.GetInformationForDevice(uei.DeviceName);
            if (informationForDevice != null)
            {
              foreach (EdexFileInformation edexFileInformation in informationForDevice.EdexFiles.Where<EdexFileInformation>((Func<EdexFileInformation, bool>) (ef => !ef.HasErrors && ef.HasFlashwarePartNumber && ef.ConfigurationInformation != null && ef.ConfigurationInformation.FlashwarePartNumber.Equals((object) uei.FlashwarePartNumber))))
                sources.Add(Tuple.Create<EdexConfigurationInformation, bool, string>(edexFileInformation.ConfigurationInformation, false, edexFileInformation.CompleteFileType));
            }
          }
          UnitPartNumberViewDialog.WriteDeviceContent(writer, sources);
        }
      }
    }
    else
    {
      foreach (DeviceInformation deviceInformation in (IEnumerable<DeviceInformation>) this.downloadUnit.DeviceInformation.OrderBy<DeviceInformation, int>((Func<DeviceInformation, int>) (di => GetPriority(di.Device))))
      {
        List<Tuple<EdexConfigurationInformation, bool, string>> list = deviceInformation.EdexFiles.Where<EdexFileInformation>((Func<EdexFileInformation, bool>) (efi => !efi.HasErrors)).Select<EdexFileInformation, Tuple<EdexConfigurationInformation, bool, string>>((Func<EdexFileInformation, Tuple<EdexConfigurationInformation, bool, string>>) (edexFile => Tuple.Create<EdexConfigurationInformation, bool, string>(edexFile.ConfigurationInformation, false, edexFile.CompleteFileType))).ToList<Tuple<EdexConfigurationInformation, bool, string>>();
        if (list.Any<Tuple<EdexConfigurationInformation, bool, string>>())
          UnitPartNumberViewDialog.WriteDeviceContent(writer, list);
      }
    }

    static int GetPriority(string device)
    {
      Ecu ecuByName = SapiManager.GetEcuByName(device);
      return ecuByName == null ? int.MaxValue : ecuByName.Priority;
    }
  }
}
