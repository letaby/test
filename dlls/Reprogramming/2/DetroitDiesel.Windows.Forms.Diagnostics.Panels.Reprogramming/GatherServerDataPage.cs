using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class GatherServerDataPage : WizardPage
{
	private List<string> tabsBuilt = new List<string>();

	private Dictionary<UnitInformation, DateTime> newPendingRequests = new Dictionary<UnitInformation, DateTime>();

	private Collection<UnitInformation> uploadUnits = new Collection<UnitInformation>();

	private string selectedTab = "unit";

	private HtmlElement unitListPane;

	private HtmlElement softwareListPane;

	private HtmlElement datasetListPane;

	private HtmlElement diagnosticDescriptionListPane;

	private HtmlElement unitListbuttonPanel;

	private HtmlElement softwareListButtonPanel;

	private string selectedItem;

	private static Regex regexGPD = new Regex("\\(\\(VERSION (?<ver>\\d+.\\d+.\\d+), (?<date>\\d+.\\d+.\\d+),", RegexOptions.Compiled);

	internal IEnumerable<UnitInformation> UploadUnits => uploadUnits.ToList();

	internal IEnumerable<UnitInformation> PendingRequests => newPendingRequests.Keys.ToList();

	public GatherServerDataPage(ReprogrammingView dataProvider, HtmlElement inputPane)
		: base(dataProvider, inputPane)
	{
		ServerDataManager.GlobalInstance.DataUpdated += ServerDataManager_DataUpdated;
		ServerClient.GlobalInstance.InUseChanged += ServerClient_InUseChanged;
		ServerClient.GlobalInstance.DownloadUnzipSuccess += ServerClient_DownloadUnzipSuccess;
		LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadStatusChanged += GlobalInstance_RemoteFileDownloadStatusChanged;
		SetInputPane(inputPane);
	}

	private void GlobalInstance_RemoteFileDownloadStatusChanged(object sender, EventArgs e)
	{
		if (base.Wizard.ActivePage == this)
		{
			BuildUnitList();
		}
		else
		{
			tabsBuilt.Remove("unit");
		}
	}

	internal void SetInputPane(HtmlElement inputPane)
	{
		inputPane.InnerHtml = FormattableString.Invariant($"\r\n                <div>\r\n                    <div class='tab'>\r\n                        <button class='tablinks active' onClick=\"clickTab(event,'unit')\">{Resources.GatherServerDataPage_TabUnit}</button>\r\n                        <button class='tablinks' onClick=\"clickTab(event,'software')\">{Resources.GatherServerDataPage_TabSoftware}</button>\r\n                        <button class='tablinks' onClick=\"clickTab(event,'dataset')\">{Resources.GatherServerDataPage_TabDataset}</button>\r\n                        <button class='tablinks' onClick=\"clickTab(event,'diagnosticdescription')\">{Resources.GatherServerDataPage_TabDiagnosticDescriptions}</button>\r\n                        <button id='switchToConnectedUnitButton' class='tablinks exit hide' onClick=\"clickTab(event,'connectedunit', true)\">&#x279c; {Resources.GatherServerDataPage_TabConnectedUnit}</button>\r\n                    </div>\r\n                    <div>\r\n                        <div id='unitListPane'>&nbsp;</div>\r\n                        <div id='unitListbuttonPanel' class='fixedbottom show'>&nbsp;</div>\r\n                    </div>\r\n                    <div>\r\n                        <div id='softwareListPane' class='hide'>&nbsp;</div>\r\n                        <div id='softwareListbuttonPanel' class='fixedbottom hide'>&nbsp;</div>\r\n                    </div>\r\n                    <div id='datasetListPane' class='hide'>&nbsp;</div>\r\n                    <div id='diagnosticDescriptionListPane' class='hide'>&nbsp;</div>\r\n                </div>\r\n            ");
		foreach (HtmlElement item in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList())
		{
			switch (item.Id)
			{
			case "unitListPane":
				unitListPane = item;
				break;
			case "softwareListPane":
				softwareListPane = item;
				break;
			case "datasetListPane":
				datasetListPane = item;
				break;
			case "diagnosticDescriptionListPane":
				diagnosticDescriptionListPane = item;
				break;
			case "unitListbuttonPanel":
				unitListbuttonPanel = item;
				break;
			case "softwareListbuttonPanel":
				softwareListButtonPanel = item;
				break;
			}
		}
	}

	private HtmlElement GetButton(string key)
	{
		return unitListPane.GetElementsByTagName("button").OfType<HtmlElement>().FirstOrDefault((HtmlElement b) => b.Id == key);
	}

	internal override void Navigate(string fragment)
	{
		string[] array = fragment.Split("_".ToCharArray());
		string text = array[0];
		if (!(text == "#button"))
		{
			if (text == "#tab")
			{
				TabControlSelectedIndexChanged(array[1]);
			}
			return;
		}
		switch (array[1])
		{
		case "add":
			AddButtonClick();
			break;
		case "refresh":
			if (array.Length > 3)
			{
				RefreshButtonClick(array[3], (array.Length == 5) ? array[4] : null);
			}
			else
			{
				RefreshButtonClick(null, null);
			}
			break;
		case "refreshAll":
			RefreshAllButtonClick();
			break;
		case "remove":
			if (array.Length > 3)
			{
				RemoveButtonClick(array[2], array[3], (array.Length == 5) ? array[4] : null);
			}
			else
			{
				RemoveButtonClick(array[2], null, null);
			}
			break;
		case "removeAll":
			RemoveAllButtonClick();
			break;
		case "view":
			ViewPartNumbersButtonClick(array[3], (array.Length == 5) ? array[4] : null);
			break;
		case "item":
		{
			if (selectedItem != null)
			{
				HtmlElement button = GetButton(selectedItem);
				if (button != null)
				{
					button.SetAttribute("className", "unitButton");
				}
			}
			selectedItem = string.Join("_", array.Skip(2));
			HtmlElement button2 = GetButton(selectedItem);
			if (button2 != null)
			{
				button2.SetAttribute("className", "unitButton active");
			}
			break;
		}
		case "connect":
			ConnectButtonClick();
			break;
		case "refreshSoftware":
			RefreshSoftwareButtonClick();
			break;
		}
	}

	private void ServerClient_InUseChanged(object sender, EventArgs e)
	{
		ReprogrammingView.SetButtonState(unitListbuttonPanel, "connectButton", !ServerClient.GlobalInstance.InUse, show: true);
		if (!ServerClient.GlobalInstance.InUse)
		{
			BuildLists();
		}
		base.Wizard.Busy = ServerClient.GlobalInstance.InUse;
		Application.DoEvents();
	}

	private void ServerDataManager_DataUpdated(object sender, EventArgs e)
	{
		BuildLists();
	}

	private void ServerClient_DownloadUnzipSuccess(object sender, EventArgs e)
	{
		foreach (KeyValuePair<UnitInformation, DateTime> pendingRequest in newPendingRequests.ToList())
		{
			UnitInformation val = ServerDataManager.GlobalInstance.UnitInformation.FirstOrDefault((UnitInformation u) => u.IsSameIdentification(pendingRequest.Key.EngineNumber, pendingRequest.Key.VehicleIdentity));
			if (val != null && Sapi.TimeFromString(val.Time) > pendingRequest.Value)
			{
				newPendingRequests.Remove(pendingRequest.Key);
			}
		}
		BuildLists();
	}

	internal override void OnSetActive()
	{
		BuildUnitList();
	}

	private void ViewPartNumbersButtonClick(string vehicleIdentity, string engineNumber)
	{
		UnitInformation val = uploadUnits.FirstOrDefault((UnitInformation ui) => ui.IsSameIdentification(engineNumber, vehicleIdentity));
		UnitInformation val2 = ServerDataManager.GlobalInstance.UnitInformation.FirstOrDefault((UnitInformation ui) => ui.IsSameIdentification(engineNumber, vehicleIdentity));
		if (val != null || val2 != null)
		{
			((Form)(object)new UnitPartNumberViewDialog(val, val2)).ShowDialog();
		}
		else
		{
			ControlHelpers.ShowMessageBox(string.Format(CultureInfo.CurrentCulture, Resources.GatherServerDataPage_UnitNotFound, engineNumber, vehicleIdentity), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}
	}

	private void RemoveButtonClick(string location, string vehicleIdentity, string engineNumber)
	{
		if (!(location == "pending") && ControlHelpers.ShowMessageBox(Resources.GatherServerDataPage_Message_UnitWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
		{
			return;
		}
		IEnumerable<UnitInformation> source = null;
		switch (location)
		{
		case "download":
			source = ServerDataManager.GlobalInstance.UnitInformation;
			break;
		case "upload":
			source = uploadUnits;
			break;
		case "pending":
			source = newPendingRequests.Keys;
			break;
		}
		if (vehicleIdentity == null && engineNumber == null)
		{
			foreach (UnitInformation item in source.ToList())
			{
				RemoveServerItem(item);
			}
		}
		else
		{
			UnitInformation val = source.FirstOrDefault((UnitInformation ui) => ui.IsSameIdentification(engineNumber, vehicleIdentity));
			if (val != null)
			{
				RemoveServerItem(val);
			}
			else
			{
				ControlHelpers.ShowMessageBox(string.Format(CultureInfo.CurrentCulture, Resources.GatherServerDataPage_UnitNotFound, engineNumber, vehicleIdentity), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
		ServerDataManager.GlobalInstance.SaveUnitXml();
	}

	private void RemoveAllButtonClick()
	{
		if (ControlHelpers.ShowMessageBox(Resources.GatherServerDataPage_Message_AllWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
		{
			return;
		}
		foreach (UnitInformation item in ServerDataManager.GlobalInstance.UnitInformation.Union(newPendingRequests.Keys).ToList())
		{
			RemoveServerItem(item);
		}
		ServerDataManager.GlobalInstance.SaveUnitXml();
	}

	internal void RemoveServerItem(UnitInformation request)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)request.Status == 1)
		{
			ServerDataManager.DeleteSettingsFilesForUnit(Directories.DrumrollUploadData, request);
		}
		else if (newPendingRequests.ContainsKey(request))
		{
			newPendingRequests.Remove(request);
		}
		else if (ServerDataManager.GlobalInstance.UnitInformation.Contains(request))
		{
			ServerDataManager.GlobalInstance.UnitInformation.Remove(request);
			ServerDataManager.DeleteSettingsFilesForUnit(Directories.DrumrollDownloadData, request);
			ServerDataManager.GlobalInstance.RemoveUnreferencedPendingFirmwareDownloads();
		}
	}

	private void CreateRequestPendingItem(UnitInformation request)
	{
		if (ServerDataManager.GlobalInstance.UnitInformation.Contains(request) && !newPendingRequests.ContainsKey(request))
		{
			AddToRequestPendingGroup(request);
		}
	}

	internal void RefreshList(IEnumerable<UnitInformation> requestItems)
	{
		foreach (UnitInformation requestItem in requestItems)
		{
			CreateRequestPendingItem(requestItem);
		}
		BuildUnitList();
	}

	private void RefreshButtonClick(string vehicleIdentity, string engineNumber)
	{
		if (vehicleIdentity == null && engineNumber == null)
		{
			RefreshList(ServerDataManager.GlobalInstance.UnitInformation);
			return;
		}
		CreateRequestPendingItem(ServerDataManager.GlobalInstance.UnitInformation.FirstOrDefault((UnitInformation ui) => ui.IsSameIdentification(engineNumber, vehicleIdentity)));
		BuildUnitList();
	}

	private void RefreshAllButtonClick()
	{
		RefreshList(ServerDataManager.GlobalInstance.UnitInformation);
	}

	private void RefreshSoftwareButtonClick()
	{
		if (ControlHelpers.ShowMessageBox(Resources.GatherServerDataPage_MessageRefreshSoftware, MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
		{
			ServerDataManager.GlobalInstance.FirmwareInformation.Clear();
			BuildFirmwareList();
			RefreshList(ServerDataManager.GlobalInstance.UnitInformation);
		}
	}

	private void AddButtonClick()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		AddUnitDialog addUnitDialog = new AddUnitDialog();
		if (addUnitDialog.ShowDialog() == DialogResult.OK)
		{
			UnitInformation request = new UnitInformation(addUnitDialog.EngineSerialNumber, addUnitDialog.VehicleIdNumber, addUnitDialog.EcuHardwarePartNumbers, addUnitDialog.EcuSoftwareIdentificationItems, (UnitStatus)0);
			AddToRequestPendingGroup(request);
		}
		BuildUnitList();
	}

	internal void AddToRequestPendingGroup(UnitInformation request)
	{
		newPendingRequests.Add(request, DateTime.Now);
	}

	private void ConnectButtonClick()
	{
		Connect(CollectionExtensions.DistinctBy<UnitInformation, string>((IEnumerable<UnitInformation>)newPendingRequests.Keys, (Func<UnitInformation, string>)((UnitInformation pr) => pr.IdentityKey)), uploadUnits.Where((UnitInformation uu) => !newPendingRequests.Keys.Any((UnitInformation pr) => pr.IsSameIdentification(uu.EngineNumber, uu.VehicleIdentity))));
	}

	internal static void Connect(IEnumerable<UnitInformation> requestUnits, IEnumerable<UnitInformation> settingsUploadUnits)
	{
		ServerClient.GlobalInstance.Go(new Collection<UnitInformation>(requestUnits.ToList()), new Collection<UnitInformation>(settingsUploadUnits.ToList()));
	}

	public void BuildLists()
	{
		tabsBuilt.Clear();
		BuildUnitList();
		switch (selectedTab)
		{
		case "software":
			BuildFirmwareList();
			break;
		case "dataset":
			BuildDataSetList();
			break;
		case "diagnosticdescription":
			BuildCBFList();
			break;
		}
	}

	private static int? GetEcuPriority(string device)
	{
		return SapiManager.GlobalInstance.Sapi.Ecus[device]?.Priority;
	}

	public void BuildFirmwareList()
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
		{
			flag = WriteFirmwareList(writer);
		}
		softwareListPane.InnerHtml = stringBuilder.ToString() + "<br/><br/><br/><br/>";
		softwareListButtonPanel.InnerHtml = (flag ? ReprogrammingView.WriteButton("refreshSoftware", Resources.Wizard_ButtonRefreshAllSoftware) : "&nbsp;");
	}

	private bool WriteFirmwareList(XmlWriter writer)
	{
		IOrderedEnumerable<IGrouping<string, FirmwareInformation>> orderedEnumerable = from fi in ServerDataManager.GlobalInstance.FirmwareInformation.OfType<FirmwareInformation>()
			group fi by fi.Device into g
			orderby GetEcuPriority(g.Key)
			select g;
		if (orderedEnumerable.Any())
		{
			writer.WriteStartElement("table");
			writer.WriteAttributeString("class", "grey");
			WebBrowserList.WriteTableHeader(writer, new string[6]
			{
				Resources.GatherServerDataPage_Device,
				Resources.GatherServerDataPage_PartNumber,
				Resources.GatherServerDataPage_Version,
				Resources.GatherServerDataPage_Filename,
				Resources.GatherServerDataPage_Description,
				Resources.GatherServerDataPage_RemoteFileStatus
			});
			foreach (IGrouping<string, FirmwareInformation> item in orderedEnumerable)
			{
				bool flag = true;
				foreach (FirmwareInformation item2 in item)
				{
					writer.WriteStartElement("tr");
					if (flag)
					{
						WebBrowserList.WriteGroupCell(writer, item2.Device, item.Count());
						flag = false;
					}
					writer.WriteElementString("td", item2.Key);
					writer.WriteElementString("td", item2.Version);
					Ecu ecuByName = SapiManager.GetEcuByName(item2.Device);
					AddFlashwareInformation(writer, item2.FileName, item2.Key, item2.Description, ecuByName != null && SapiExtensions.FlashingRequiresMvci(ecuByName));
					if (!string.IsNullOrEmpty(item2.Reference))
					{
						writer.WriteStartElement("td");
						writer.WriteStartElement("div");
						if (item2.Status != "OK" || item2.RequiresDownload)
						{
							WebBrowserList.WriteWarningTriangle(writer);
						}
						if (item2.RequiresDownload)
						{
							writer.WriteString(Resources.GatherServerDataPage_RemoteFileDownloadPending);
						}
						else
						{
							writer.WriteString(item2.Status);
						}
						writer.WriteEndElement();
					}
					else
					{
						writer.WriteElementString("td", "n/a");
					}
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}
		else
		{
			writer.WriteElementString("p", Resources.GatherServerDataPage_NoInformation);
		}
		tabsBuilt.Add("software");
		return orderedEnumerable.Any();
	}

	private static void AddFlashwareInformation(XmlWriter writer, string fileName, string key, string description, bool flashingRequiresMvci)
	{
		FlashFile flashFile = Sapi.GetSapi().FlashFiles.FirstOrDefault((FlashFile ff) => Path.GetFileName(ff.FileName) != null && Path.GetFileName(ff.FileName).Equals(fileName, StringComparison.OrdinalIgnoreCase));
		FlashMeaning flashMeaning = flashFile?.FlashAreas.SelectMany((FlashArea fa) => fa.FlashMeanings).FirstOrDefault((FlashMeaning m) => PartExtensions.IsEqual(new Part(key), m.FlashKey));
		if (flashMeaning != null)
		{
			AddFlashwareInformation(writer, flashMeaning, description);
			return;
		}
		writer.WriteElementString("td", fileName);
		writer.WriteStartElement("td");
		if (flashingRequiresMvci && !SapiManager.GlobalInstance.UseMcd && File.Exists(Path.Combine(Directories.McdSystemDatabase, fileName)))
		{
			writer.WriteString(fileName);
		}
		else
		{
			WebBrowserList.WriteWarningTriangle(writer);
			if (flashFile != null)
			{
				writer.WriteString(string.Format(CultureInfo.CurrentCulture, Resources.GatherServerDataPage_Format_FlashKeyMismatch, string.Join(", ", from m in flashFile.FlashAreas.SelectMany((FlashArea fa) => fa.FlashMeanings)
					select m.FlashKey)));
			}
			else
			{
				writer.WriteString(Resources.GatherServerDataPage_FileMissing);
			}
		}
		writer.WriteEndElement();
	}

	public void BuildDataSetList()
	{
		StringBuilder stringBuilder = new StringBuilder();
		using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
		{
			WriteDatasetList(writer);
		}
		datasetListPane.InnerHtml = stringBuilder.ToString();
	}

	private void WriteDatasetList(XmlWriter writer)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Invalid comparison between Unknown and I4
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Invalid comparison between Unknown and I4
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Invalid comparison between Unknown and I4
		GroupCollection val = new GroupCollection();
		foreach (UnitInformation item in ServerDataManager.GlobalInstance.UnitInformation)
		{
			foreach (DeviceInformation item2 in item.DeviceInformation)
			{
				bool flag = false;
				if ((int)item2.DataSource == 1)
				{
					foreach (FirmwareOptionInformation firmwareOption in item2.FirmwareOptions)
					{
						if (firmwareOption.DataSetOptions.Count > 0)
						{
							flag = true;
							break;
						}
					}
				}
				else if ((int)item2.DataSource == 2)
				{
					foreach (EdexFileInformation edexFile in item2.EdexFiles)
					{
						if (!edexFile.HasErrors && edexFile.ConfigurationInformation.DataSetPartNumbers.Any())
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					val.Add(item2.Device, (object)item2);
				}
			}
		}
		if (((IEnumerable<Group>)val).Any())
		{
			writer.WriteStartElement("table");
			writer.WriteAttributeString("class", "grey");
			WebBrowserList.WriteTableHeader(writer, new string[5]
			{
				Resources.GatherServerDataPage_Device,
				Resources.GatherServerDataPage_PartNumber,
				Resources.GatherServerDataPage_ForSoftwareVersion,
				Resources.GatherServerDataPage_Filename,
				Resources.GatherServerDataPage_Description
			});
			foreach (Group item3 in ((IEnumerable<Group>)val).OrderBy((Group g) => GetEcuPriority(g.Name)))
			{
				Ecu ecuByName = SapiManager.GetEcuByName(item3.Name);
				DeviceDataSource dataSource = ((DeviceInformation)item3.Items[0]).DataSource;
				if ((int)dataSource != 1)
				{
					if ((int)dataSource != 2)
					{
						continue;
					}
					var enumerable = CollectionExtensions.DistinctBy(from device in item3.Items.OfType<DeviceInformation>()
						from edexFileInformation in device.EdexFiles
						where !edexFileInformation.HasErrors && edexFileInformation.ConfigurationInformation.DataSetPartNumbers.Any()
						from dataSetPartNumber in edexFileInformation.ConfigurationInformation.DataSetPartNumbers
						select new
						{
							EdexFileInformation = edexFileInformation,
							DataSetPartNumber = dataSetPartNumber
						}, n => n.DataSetPartNumber);
					bool flag2 = true;
					foreach (var item4 in enumerable)
					{
						string key = PartExtensions.ToFlashKeyStyleString(item4.DataSetPartNumber);
						writer.WriteStartElement("tr");
						if (flag2)
						{
							WebBrowserList.WriteGroupCell(writer, item3.Name, enumerable.Count());
							flag2 = false;
						}
						writer.WriteElementString("td", key);
						FirmwareInformation firmwareInformationForPart = ServerDataManager.GlobalInstance.GetFirmwareInformationForPart(item4.EdexFileInformation.ConfigurationInformation.FlashwarePartNumber);
						if (firmwareInformationForPart != null)
						{
							writer.WriteElementString("td", firmwareInformationForPart.Version);
						}
						else
						{
							writer.WriteElementString("td", PartExtensions.ToFlashKeyStyleString(item4.EdexFileInformation.ConfigurationInformation.FlashwarePartNumber));
						}
						FlashMeaning flashMeaning = Sapi.GetSapi().FlashFiles.FirstOrDefault((FlashFile ff) => ff.FlashAreas.Any((FlashArea fa) => fa.FlashMeanings.Any((FlashMeaning fm) => fm.FlashKey.Equals(key))))?.FlashAreas.SelectMany((FlashArea fa) => fa.FlashMeanings).FirstOrDefault((FlashMeaning m) => PartExtensions.IsEqual(new Part(key), m.FlashKey));
						if (flashMeaning != null)
						{
							AddFlashwareInformation(writer, flashMeaning);
						}
						else if (ecuByName != null && SapiExtensions.FlashingRequiresMvci(ecuByName) && !SapiManager.GlobalInstance.UseMcd)
						{
							writer.WriteStartElement("td");
							writer.WriteAttributeString("colspan", "2");
							WebBrowserList.WriteWarningTriangle(writer);
							writer.WriteString(Resources.GatherServerDataPage_MVCIServerMustBeEnabledToLoadFile);
						}
						else
						{
							writer.WriteStartElement("td");
							writer.WriteAttributeString("colspan", "2");
							WebBrowserList.WriteWarningTriangle(writer);
							writer.WriteString(Resources.GatherServerDataPage_FlashKeyNotFound);
							writer.WriteEndElement();
						}
						writer.WriteEndElement();
					}
					continue;
				}
				var enumerable2 = CollectionExtensions.DistinctBy(from device in item3.Items.OfType<DeviceInformation>()
					from firmwareOption in device.FirmwareOptions
					from datasetOption in firmwareOption.DataSetOptions
					select new
					{
						FirmwareOption = firmwareOption,
						DataSetOption = datasetOption
					}, n => n.DataSetOption.Key);
				bool flag3 = true;
				foreach (var item5 in enumerable2)
				{
					writer.WriteStartElement("tr");
					if (flag3)
					{
						WebBrowserList.WriteGroupCell(writer, item3.Name, enumerable2.Count());
						flag3 = false;
					}
					writer.WriteElementString("td", item5.DataSetOption.Key);
					writer.WriteElementString("td", item5.FirmwareOption.Version);
					AddFlashwareInformation(writer, item5.DataSetOption.FileName, item5.DataSetOption.Key, item5.DataSetOption.Description, ecuByName != null && SapiExtensions.FlashingRequiresMvci(ecuByName));
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}
		else
		{
			writer.WriteElementString("p", Resources.GatherServerDataPage_NoInformation);
		}
		tabsBuilt.Add("dataset");
	}

	private static void AddFlashwareInformation(XmlWriter writer, FlashMeaning flashMeaning, string description = null)
	{
		string fileName = Path.GetFileName(flashMeaning.FileName);
		writer.WriteElementString("td", fileName);
		if (!string.IsNullOrEmpty(description))
		{
			writer.WriteElementString("td", description);
		}
		else if (!string.IsNullOrEmpty(flashMeaning.Description))
		{
			writer.WriteElementString("td", flashMeaning.Description);
		}
		else if (!string.IsNullOrEmpty(flashMeaning.Name))
		{
			writer.WriteElementString("td", flashMeaning.Name);
		}
	}

	private void BuildCBFList()
	{
		StringBuilder stringBuilder = new StringBuilder();
		using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
		{
			WriteDiagnosisDescriptionsList(writer);
		}
		diagnosticDescriptionListPane.InnerHtml = stringBuilder.ToString();
	}

	private void WriteDiagnosisDescriptionsList(XmlWriter writer)
	{
		writer.WriteStartElement("table");
		writer.WriteAttributeString("class", "grey");
		WebBrowserList.WriteTableHeader(writer, new string[6]
		{
			Resources.GatherServerDataPage_Identifier,
			Resources.GatherServerDataPage_Device,
			Resources.GatherServerDataPage_Version,
			Resources.GatherServerDataPage_Filename,
			Resources.GatherServerDataPage_Description,
			Resources.GatherServerDataPage_Is64BitCompatible
		});
		foreach (IGrouping<string, Ecu> item in from e in Sapi.GetSapi().Ecus
			where !e.IsRollCall && !e.IsVirtual
			orderby e.Priority
			group e by e.Identifier)
		{
			bool flag = true;
			foreach (Ecu item2 in item)
			{
				writer.WriteStartElement("tr");
				if (flag)
				{
					WebBrowserList.WriteGroupCell(writer, item2.Identifier, item.Count());
					flag = false;
				}
				writer.WriteElementString("th", item2.Name);
				writer.WriteElementString("td", item2.DescriptionDataVersion);
				writer.WriteElementString("td", Path.GetFileName(item2.DescriptionFileName));
				writer.WriteElementString("td", item2.ShortDescription);
				Match match = regexGPD.Match(item2.GpdVersion);
				if (match.Success)
				{
					bool? flag2 = null;
					if (Version.TryParse(match.Groups["ver"].Value, out var result))
					{
						flag2 = result.Major >= 4;
					}
					writer.WriteStartElement("td");
					writer.WriteAttributeString("style", "color:" + ((!flag2.HasValue) ? "black" : (flag2.Value ? "green" : "red")));
					writer.WriteString((!flag2.HasValue) ? Resources.GatherServerDataPage_Unknown : (flag2.Value ? Resources.GatherServerDataPage_Yes : Resources.GatherServerDataPage_No));
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
		}
		writer.WriteEndElement();
		tabsBuilt.Add("diagnosticdescription");
	}

	private void BuildUnitList()
	{
		List<string> list = new List<string>();
		foreach (UnitInformation item in from ui in ServerDataManager.GlobalInstance.UnitInformation
			where !newPendingRequests.Keys.Any((UnitInformation pr) => pr.IsSameIdentification(ui.EngineNumber, ui.VehicleIdentity))
			orderby Sapi.TimeFromString(ui.Time) descending
			select ui)
		{
			list.Add(FormattableString.Invariant($"<tr><td>{ReprogrammingView.CreateDownloadedUnitHtml(item, expandErrors: false)}</td></tr>"));
		}
		List<string> list2 = new List<string>();
		foreach (UnitInformation key in newPendingRequests.Keys)
		{
			string text = key.VehicleIdentity + "_" + key.EngineNumber;
			string text2 = FormattableString.Invariant($"<tr><td><button id='pending_{text}' class='unitButton' onClick=\"clickButton('item_pending_{text}')\"><table>");
			text2 += FormattableString.Invariant($"<tr><td/><td><div class='identity'>{ReprogrammingView.GetTitleString(key.VehicleIdentity, key.EngineNumber)}</div></td>");
			text2 += FormattableString.Invariant(FormattableStringFactory.Create("<td>{0}</td>", ReprogrammingView.WriteButton("remove_pending_" + text, Resources.Wizard_ButtonRemove, "midblue")));
			text2 += "</tr>";
			text2 += "</table></button></td></tr>";
			list2.Add(text2);
		}
		uploadUnits = new Collection<UnitInformation>();
		ServerDataManager.GlobalInstance.GetUploadUnits(uploadUnits, (UploadType)0);
		List<string> list3 = new List<string>();
		foreach (UnitInformation info in uploadUnits)
		{
			bool hasPendingRequest = newPendingRequests.Keys.Any((UnitInformation pr) => pr.IsSameIdentification(info.EngineNumber, info.VehicleIdentity));
			list3.Add(FormattableString.Invariant($"<tr><td>{ReprogrammingView.CreateUploadUnitHtml(info, hasPendingRequest)}</td></tr>"));
		}
		string unitListHtml;
		if (list.Count + list2.Count + list3.Count > 0)
		{
			unitListHtml = "<table class='grey unitlist' style='width:100%'>";
			WriteTableSection(list2, "pending", Resources.GatherServerDataPage_PendingRequestsSection);
			WriteTableSection(list, "download", Resources.GatherServerDataPage_DownloadUnitsSection);
			WriteTableSection(list3, "upload", Resources.GatherServerDataPage_UploadUnitsSection);
			unitListHtml += "</table>";
		}
		else
		{
			unitListHtml = FormattableString.Invariant($"<p>{Resources.GatherServerDataPage_NoInformation}</p>");
		}
		unitListPane.InnerHtml = unitListHtml + "<br/><br/><br/>";
		unitListbuttonPanel.InnerHtml = ReprogrammingView.WriteButton("add", Resources.Wizard_ButtonAddRequest) + ReprogrammingView.WriteButton("refreshAll", "Refresh All", "midblue") + ReprogrammingView.WriteButton("removeAll", "Remove All", "midblue") + ReprogrammingView.WriteButton("connect", Resources.Wizard_ButtonConnect);
		ReprogrammingView.SetButtonState(unitListbuttonPanel, "refreshAllButton", ServerDataManager.GlobalInstance.UnitInformation.Any(), show: true);
		ReprogrammingView.SetButtonState(unitListbuttonPanel, "removeAllButton", ServerDataManager.GlobalInstance.UnitInformation.Union(newPendingRequests.Keys).Any(), show: true);
		ReprogrammingView.SetButtonState(unitListbuttonPanel, "connectButton", !ServerClient.GlobalInstance.InUse, show: true);
		tabsBuilt.Add("unit");
		if (selectedItem != null)
		{
			HtmlElement button = GetButton(selectedItem);
			if (button != null)
			{
				button.SetAttribute("className", "unitButton active");
			}
			else
			{
				selectedItem = null;
			}
		}
		void WriteTableSection(List<string> htmlItems, string id, string sectionText)
		{
			if (htmlItems.Any())
			{
				unitListHtml += FormattableString.Invariant($"<tr><td>{ReprogrammingView.CreateTableSection(id, sectionText)}</td></tr>");
				unitListHtml += string.Join(" ", htmlItems);
			}
		}
	}

	private void TabControlSelectedIndexChanged(string selectedTab)
	{
		unitListPane.SetAttribute("className", (selectedTab == "unit") ? "show" : "hide");
		unitListbuttonPanel.SetAttribute("className", (selectedTab == "unit") ? "fixedbottom show" : "fixedbottom hide");
		softwareListPane.SetAttribute("className", (selectedTab == "software") ? "show" : "hide");
		softwareListButtonPanel.SetAttribute("className", (selectedTab == "software") ? "fixedbottom show" : "fixedbottom hide");
		datasetListPane.SetAttribute("className", (selectedTab == "dataset") ? "show" : "hide");
		diagnosticDescriptionListPane.SetAttribute("className", (selectedTab == "diagnosticdescription") ? "show" : "hide");
		this.selectedTab = selectedTab;
		if (!tabsBuilt.Contains(this.selectedTab))
		{
			switch (this.selectedTab)
			{
			case "unit":
				BuildUnitList();
				break;
			case "software":
				BuildFirmwareList();
				break;
			case "dataset":
				BuildDataSetList();
				break;
			case "diagnosticdescription":
				BuildCBFList();
				break;
			}
		}
	}
}
