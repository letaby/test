using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal sealed class UnitPartNumberViewDialog : WebBrowserDialog
{
	private UnitInformation downloadUnit;

	private UnitInformation uploadUnit;

	public UnitPartNumberViewDialog(UnitInformation uploadUnit, UnitInformation downloadUnit)
		: base(Resources.UnitPartNumberViewDialog_Title, (Action<XmlWriter>)null)
	{
		this.uploadUnit = uploadUnit;
		this.downloadUnit = downloadUnit;
	}

	protected override void OnLoad(EventArgs e)
	{
		Rectangle bounds = Screen.FromControl((Control)(object)this).Bounds;
		((Form)this).Size = new Size(bounds.Width * 8 / 10, bounds.Height * 8 / 10);
		((Form)this).OnLoad(e);
	}

	private static void WriteDeviceContent(XmlWriter writer, List<Tuple<EdexConfigurationInformation, bool, string>> sources)
	{
		Ecu ecu = SapiManager.GetEcuByName(sources[0].Item1.DeviceName);
		CodingFile codingFile = ((ecu != null) ? SapiManager.GlobalInstance.Sapi.CodingFiles.Where((CodingFile cf) => cf.Ecus.Contains(ecu)).FirstOrDefault() : null);
		string[] sourceBackgroundColors = new string[5] { "#EBF5FB", "#FEF9E7", "#E8F6F3", "#EAFAF1", "#F4ECF7" };
		WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", ecu.DisplayName, (Action)delegate
		{
			if (codingFile != null)
			{
				List<IList<EdexSettingItem>> source = sources.Select((Tuple<EdexConfigurationInformation, bool, string> tuple) => tuple.Item1.GetSettingsAndProposedSettings(codingFile.CodingParameterGroups, true)).ToList();
				if (source.Any((IList<EdexSettingItem> relevantSettingsForSource) => relevantSettingsForSource.Any()))
				{
					List<IEnumerable<CodingChoice>> source2 = source.Select((IList<EdexSettingItem> rs) => from si in rs
						select codingFile.CodingParameterGroups.GetCodingForPart(si.PartNumber.ToString()) into cc
						where cc != null
						select cc).ToList();
					writer.WriteStartElement("table");
					writer.WriteAttributeString("style", "margin-top:5px;margin-bottom:5px;");
					writer.WriteStartElement("tr");
					writer.WriteStartElement("th");
					writer.WriteAttributeString("rowspan", "2");
					writer.WriteAttributeString("colspan", "2");
					writer.WriteRaw("&nbsp;");
					writer.WriteEndElement();
					foreach (IGrouping<bool, Tuple<EdexConfigurationInformation, bool, string>> item in from tuple in sources
						group tuple by tuple.Item2)
					{
						writer.WriteStartElement("th");
						writer.WriteAttributeString("colspan", (2 * item.Count()).ToString(CultureInfo.InvariantCulture));
						writer.WriteString(item.Key ? Resources.UnitPartNumberViewDialog_UploadData : Resources.UnitPartNumberViewDialog_DownloadData);
						writer.WriteEndElement();
					}
					writer.WriteEndElement();
					writer.WriteStartElement("tr");
					foreach (Tuple<EdexConfigurationInformation, bool, string> source3 in sources)
					{
						writer.WriteStartElement("th");
						writer.WriteAttributeString("colspan", "2");
						writer.WriteString(string.Join(" - ", new string[4]
						{
							source3.Item3,
							source3.Item1.FlashwarePartNumber.ToString(),
							PartExtensions.ToHardwarePartNumberString(source3.Item1.HardwarePartNumber, ecu, true),
							source3.Item1.DiagnosticLinkSettingsTimestamp.HasValue ? source3.Item1.DiagnosticLinkSettingsTimestamp.Value.ToString(CultureInfo.CurrentCulture) : null
						}.Where((string s) => s != null)));
						writer.WriteEndElement();
					}
					writer.WriteEndElement();
					writer.WriteStartElement("tr");
					writer.WriteElementString("th", Resources.PartNumberErrorDialog_Domain);
					writer.WriteElementString("th", Resources.PartNumberErrorDialog_Fragment);
					sources.ForEach(delegate
					{
						writer.WriteElementString("th", Resources.PartNumberErrorDialog_PartNumber);
						writer.WriteElementString("th", Resources.PartNumberErrorDialog_Meaning);
					});
					writer.WriteEndElement();
					foreach (CodingParameterGroup codingGroup in codingFile.CodingParameterGroups.OrderBy((CodingParameterGroup cg) => cg.Qualifier))
					{
						List<List<CodingChoice>> list = source2.Select((IEnumerable<CodingChoice> rcc) => rcc.Where((CodingChoice cc) => cc.ParameterGroup.Qualifier == codingGroup.Qualifier).ToList()).ToList();
						List<CodingParameter> list2 = list.SelectMany((List<CodingChoice> rcfg) => rcfg.Select((CodingChoice cc) => cc.Parameter)).Distinct().ToList();
						bool flag = true;
						foreach (CodingParameter codingParameter in list2)
						{
							List<List<CodingChoice>> list3 = list.Select((List<CodingChoice> rcfg) => (from cc in rcfg
								where cc.Parameter == codingParameter
								orderby cc.Part.ToString()
								select cc).ToList()).ToList();
							writer.WriteStartElement("tr");
							writer.WriteAttributeString("style", "background:white");
							if (flag)
							{
								writer.WriteStartElement("td");
								writer.WriteAttributeString("rowspan", list2.Count().ToString(CultureInfo.InvariantCulture));
								writer.WriteString(codingGroup.Qualifier);
								writer.WriteEndElement();
								flag = false;
							}
							writer.WriteStartElement("td");
							if (list3.Any((List<CodingChoice> sourceCodingChoicesForParameter) => (from cc in sourceCodingChoicesForParameter
								group cc by cc.RawValue).Count() > 1))
							{
								writer.WriteAttributeString("class", "warning");
							}
							writer.WriteStartElement("span");
							writer.WriteAttributeString("style", "color: " + ((codingParameter == null) ? "gray" : "black"));
							writer.WriteString((codingParameter == null) ? Resources.UnitPartNumberViewDialog_DefaultString : codingParameter.Name);
							writer.WriteEndElement();
							writer.WriteEndElement();
							int num = 0;
							string text = ((list3.Select((List<CodingChoice> codingChoices) => string.Join(" ", codingChoices.Select((CodingChoice cc) => cc.Part.ToString()))).Distinct().Count() == 1) ? "green" : "orangered");
							foreach (List<CodingChoice> item2 in list3)
							{
								writer.WriteStartElement("td");
								writer.WriteAttributeString("style", "background:" + sourceBackgroundColors[num]);
								if (codingParameter != null && item2.Count == 0 && list[num].Any((CodingChoice cc) => cc.Parameter == null))
								{
									writer.WriteStartElement("div");
									writer.WriteAttributeString("style", "color: " + text);
									writer.WriteString(Resources.UnitPartNumberViewDialog_FromParent);
									writer.WriteEndElement();
								}
								else
								{
									foreach (CodingChoice item3 in item2)
									{
										writer.WriteStartElement("div");
										writer.WriteAttributeString("style", "color: " + text);
										writer.WriteString(item3.Part.ToString());
										writer.WriteEndElement();
									}
								}
								writer.WriteEndElement();
								writer.WriteStartElement("td");
								writer.WriteAttributeString("style", "background:" + sourceBackgroundColors[num]);
								foreach (CodingChoice item4 in item2)
								{
									writer.WriteStartElement("div");
									writer.WriteAttributeString("style", "color: " + text);
									writer.WriteString((item4 != null) ? item4.Meaning : string.Empty);
									writer.WriteEndElement();
								}
								writer.WriteEndElement();
								num++;
							}
							writer.WriteEndElement();
						}
					}
					writer.WriteEndElement();
				}
				else
				{
					writer.WriteElementString("p", string.Format(CultureInfo.CurrentCulture, Resources.UnitPartNumberViewDialog_FormatNoSettings, sources[0].Item1.DeviceName));
				}
			}
			else
			{
				writer.WriteElementString("p", string.Format(CultureInfo.CurrentCulture, Resources.UnitPartNumberViewDialog_FormatCodingFileNotFound, sources[0].Item1.DeviceName));
			}
		});
	}

	protected override void UpdateContent(XmlWriter writer)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Invalid comparison between Unknown and I4
		writer.WriteStartElement("h1");
		writer.WriteAttributeString("class", "heading1");
		writer.WriteString(ReprogrammingView.GetTitleString((uploadUnit != null) ? uploadUnit.VehicleIdentity : downloadUnit.VehicleIdentity, (uploadUnit != null) ? uploadUnit.EngineNumber : downloadUnit.EngineNumber));
		writer.WriteEndElement();
		if (uploadUnit != null)
		{
			foreach (SettingsInformation item in from s in uploadUnit.SettingsInformation
				where (int)UnitInformation.GetDataSource(s.Device) == 2
				orderby GetPriority(s.Device)
				select s)
			{
				FileNameInformation val = FileNameInformation.FromName(item.FileName, (FileType)1);
				if ((int)val.SettingsFileFormatType == 3)
				{
					ArrayList arrayList = new ArrayList();
					List<Tuple<EdexConfigurationInformation, bool, string>> list = new List<Tuple<EdexConfigurationInformation, bool, string>>();
					EdexConfigurationInformation uei = EdexConfigurationXmlDocument.Deserialize((EdexSettingsType)4, Path.Combine(Directories.DrumrollUploadData, FileEncryptionProvider.EncryptFileName(item.FileName)), (DeviceInformation)null, arrayList);
					list.Add(Tuple.Create<EdexConfigurationInformation, bool, string>(uei, item2: true, null));
					if (downloadUnit != null)
					{
						DeviceInformation informationForDevice = downloadUnit.GetInformationForDevice(uei.DeviceName);
						if (informationForDevice != null)
						{
							foreach (EdexFileInformation item2 in informationForDevice.EdexFiles.Where((EdexFileInformation ef) => !ef.HasErrors && ef.HasFlashwarePartNumber && ef.ConfigurationInformation != null && ef.ConfigurationInformation.FlashwarePartNumber.Equals(uei.FlashwarePartNumber)))
							{
								list.Add(Tuple.Create<EdexConfigurationInformation, bool, string>(item2.ConfigurationInformation, item2: false, item2.CompleteFileType));
							}
						}
					}
					WriteDeviceContent(writer, list);
				}
			}
			return;
		}
		foreach (DeviceInformation item3 in downloadUnit.DeviceInformation.OrderBy((DeviceInformation di) => GetPriority(di.Device)))
		{
			List<Tuple<EdexConfigurationInformation, bool, string>> list2 = (from edexFile in item3.EdexFiles
				where !edexFile.HasErrors
				select Tuple.Create<EdexConfigurationInformation, bool, string>(edexFile.ConfigurationInformation, item2: false, edexFile.CompleteFileType)).ToList();
			if (list2.Any())
			{
				WriteDeviceContent(writer, list2);
			}
		}
		static int GetPriority(string device)
		{
			return SapiManager.GetEcuByName(device)?.Priority ?? int.MaxValue;
		}
	}
}
