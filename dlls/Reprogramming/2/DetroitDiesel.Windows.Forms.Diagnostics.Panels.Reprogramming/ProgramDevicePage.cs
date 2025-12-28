using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class ProgramDevicePage : WizardPage, IDisposable
{
	private enum PageActionState
	{
		ReadyToStart,
		Busy,
		ReadyToRetry,
		Finished,
		FinishedAutomaticOperation,
		FinishedFailed,
		Error,
		Unknown
	}

	internal enum ProcessStatus
	{
		Pass,
		Fail,
		Processing
	}

	private HtmlElement inputPane;

	private readonly Dictionary<Ecu, List<string>> parameterErrors = new Dictionary<Ecu, List<string>>();

	private PageActionState pageState = PageActionState.Unknown;

	private IProvideProgrammingData programmingDataProvider;

	private int programmingIndex;

	private ProgramDeviceManager programDeviceManager;

	private bool informUserOfOfflineEdexCompatibility;

	private EdexCompatibilityConfigurationCollection offlineCompatiblityConfigurationCollection;

	private EdexCompatibilityEcuItem offlineCompatibilityTarget;

	private bool informUserOfOfflineDepotCompatibility;

	private Software offlineSoftwareTarget;

	private CompatibleSoftwareCollection offlineCompatibilitySoftwareCollection;

	private Tuple<string, string> startingVariant;

	private List<Precondition> programmingPreconditions;

	private HtmlElement startButton;

	private HtmlElement titlePane;

	private HtmlElement preconditionsPane;

	private readonly List<Ecu> devicesToClearFaultsFor = new List<Ecu>();

	private readonly List<ConnectionResource> manualConnections = new List<ConnectionResource>();

	private bool disposedValue;

	private IEnumerable<Precondition> FailedPreconditions => programmingPreconditions.Where((Precondition precondition) => (int)precondition.State == 2);

	public ProgramDevicePage(ReprogrammingView dataProvider, HtmlElement inputPane)
		: base(dataProvider, inputPane)
	{
		programmingDataProvider = dataProvider;
		programDeviceManager = new ProgramDeviceManager(this);
		LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadStatusChanged += GlobalInstance_RemoteFileDownloadStatusChanged;
		SetInputPane(inputPane);
		programmingPreconditions = PreconditionManager.GlobalInstance.Preconditions.Where((Precondition p) => (int)p.PreconditionType == 1 || (int)p.PreconditionType == 3 || (int)p.PreconditionType == 2).ToList();
	}

	private void GlobalInstance_RemoteFileDownloadStatusChanged(object sender, EventArgs e)
	{
		if (base.Wizard.ActivePage == this)
		{
			ReprogrammingView.UpdateTitle(titlePane, programmingDataProvider.SelectedUnit);
		}
	}

	private void UpdateUi(PageActionState state)
	{
		pageState = state;
		switch (pageState)
		{
		case PageActionState.ReadyToStart:
		case PageActionState.ReadyToRetry:
			SetUIEnabled(enable: true);
			ReprogrammingView.SetButtonState(startButton, !SapiManager.GlobalInstance.LogFileIsOpen && !FailedPreconditions.Any(), show: true);
			base.Wizard.SetWizardButtons((WizardButtons)1);
			break;
		case PageActionState.Busy:
			SetUIEnabled(enable: false);
			ReprogrammingView.SetButtonState(startButton, enabled: false, show: true);
			base.Wizard.SetWizardButtons((WizardButtons)0);
			break;
		case PageActionState.FinishedFailed:
		case PageActionState.Error:
			SetUIEnabled(enable: true);
			ReprogrammingView.SetButtonState(startButton, enabled: false, show: true);
			base.Wizard.SetWizardButtons((WizardButtons)1);
			break;
		case PageActionState.Finished:
			SetUIEnabled(enable: true);
			ReprogrammingView.SetButtonState(startButton, enabled: false, show: false);
			base.Wizard.SetWizardButtons((WizardButtons)4);
			break;
		case PageActionState.FinishedAutomaticOperation:
			SetUIEnabled(enable: true);
			ReprogrammingView.SetButtonState(startButton, enabled: false, show: false);
			base.Wizard.SetWizardButtons((WizardButtons)2);
			break;
		case PageActionState.Unknown:
			SetUIEnabled(enable: true);
			ReprogrammingView.SetButtonState(startButton, enabled: false, show: true);
			base.Wizard.SetWizardButtons((WizardButtons)1);
			break;
		}
	}

	internal void SetInputPane(HtmlElement inputPane)
	{
		inputPane.InnerHtml = FormattableString.Invariant($"\r\n                    <div>\r\n                        <div id='titlePane'>&nbsp;</div>\r\n                        <div id='programDevicePane'>&nbsp;</div>\r\n                        <br/><br/><br/><br/>\r\n                        <div id='bottomPanel' class='fixedbottom litetransparency'>\r\n                            <div id='preconditionsPane'>&nbsp;</div>\r\n                            <div>\r\n                                <button id='backButtonPDP' class='button blue show' onClick=\"clickButton('back')\">{Resources.Wizard_ButtonBack}</button>\r\n                                <button id='nextButtonPDP' class='button gray hide' onClick=\"clickButton('next')\">{Resources.Wizard_ButtonNext}</button>\r\n                                <button id='startButton' class='button gray show' disabled onClick=\"clickButton('start')\">{Resources.Wizard_ButtonStart}</button>\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n                ");
		foreach (HtmlElement item in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList())
		{
			switch (item.Id)
			{
			case "titlePane":
				titlePane = item;
				break;
			case "programDevicePane":
				this.inputPane = item;
				break;
			case "preconditionsPane":
				preconditionsPane = item;
				break;
			}
		}
		foreach (HtmlElement item2 in inputPane.GetElementsByTagName("button").OfType<HtmlElement>().ToList())
		{
			switch (item2.Id)
			{
			case "backButtonPDP":
				base.BackButton = item2;
				break;
			case "nextButtonPDP":
				base.NextButton = item2;
				break;
			case "startButton":
				startButton = item2;
				break;
			}
		}
	}

	private void ShowContent()
	{
		if (inputPane != null)
		{
			ReprogrammingView.UpdateTitle(titlePane, programmingDataProvider.SelectedUnit);
			inputPane.InnerHtml = BuildContent();
		}
	}

	private HtmlElement FindHtmlElementById(string id)
	{
		return inputPane.Document.GetElementById(id);
	}

	private void UpdateHtmlElement(string id, string text)
	{
		HtmlElement htmlElement = FindHtmlElementById(id);
		if (htmlElement != null)
		{
			htmlElement.InnerText = text;
		}
	}

	private void UpdateHtmlElement(Ecu ecu, string id, string text, string updatedClass = null)
	{
		HtmlElement htmlElement = FindHtmlElementById(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", ecu.Name, id));
		if (htmlElement != null)
		{
			htmlElement.InnerText = text;
			if (updatedClass != null)
			{
				htmlElement.SetAttribute("className", updatedClass);
			}
		}
	}

	private void UpdateHtmlElementClass(string id, string className)
	{
		HtmlElement htmlElement = FindHtmlElementById(id);
		if (htmlElement != null)
		{
			htmlElement.SetAttribute("className", className);
		}
	}

	private static void WriteAttribute(XmlWriter xmlWriter, string name, string value)
	{
		xmlWriter.WriteStartAttribute(name);
		xmlWriter.WriteString(value);
		xmlWriter.WriteEndAttribute();
	}

	private void UpdateOverallStatus(string header, string content)
	{
		UpdateHtmlElement("statusHeader", string.Format(CultureInfo.InvariantCulture, "{0}", string.Join(": ", header, content)));
	}

	private void UpdateOverallProgress(double value)
	{
		HtmlElement htmlElement = FindHtmlElementById("progressBar");
		if (htmlElement != null)
		{
			htmlElement.SetAttribute("value", value.ToString(CultureInfo.InvariantCulture));
		}
	}

	private void HideOverallProgress(bool hidden)
	{
		if (!hidden)
		{
			UpdateHtmlElementClass("progressBar", "determinateProgress");
		}
		else
		{
			UpdateHtmlElementClass("progressBar", "determinateProgressHidden");
		}
	}

	internal void UpdateEcuProgress(Ecu ecu, double value)
	{
		HtmlElement htmlElement = FindHtmlElementById(string.Format(CultureInfo.InvariantCulture, "{0}_ecuProgress", ecu.Name));
		if (htmlElement != null)
		{
			htmlElement.SetAttribute("value", value.ToString(CultureInfo.InvariantCulture));
		}
		UpdateOverallProgress(((double)programmingIndex + value / 100.0) / (double)programmingDataProvider.ProgrammingData.Count() * 100.0);
	}

	private void UpdateEcuProgressBarClass(Ecu ecu, string className)
	{
		UpdateHtmlElementClass(string.Format(CultureInfo.InvariantCulture, "{0}_ecuProgress", ecu.Name), className);
	}

	private void UpdateEcuProcessStatus(Ecu ecu, ProcessStatus status)
	{
		switch (status)
		{
		case ProcessStatus.Pass:
			UpdateEcuProgress(ecu, 100.0);
			UpdateEcuProgressBarClass(ecu, "determinateProgressPass");
			UpdateHtmlElement(ecu, "ecuStatus", Resources.ProgramDevicePage_Success);
			UpdateHtmlElement(ecu, "ecuStep", string.Empty);
			break;
		case ProcessStatus.Fail:
			UpdateEcuProgress(ecu, 100.0);
			UpdateEcuProgressBarClass(ecu, "determinateProgressFail");
			UpdateHtmlElement(ecu, "ecuStatus", Resources.ProgramDevicePage_Failed);
			break;
		case ProcessStatus.Processing:
			UpdateEcuProgressBarClass(ecu, "determinateProgress");
			UpdateHtmlElement(ecu, "ecuStatus", Resources.ProgramDevicePage_Processing);
			break;
		}
	}

	private void UpdateEcuConfiguration(ProgrammingData data)
	{
		if (data?.Channel == null)
		{
			return;
		}
		UpdateHtmlElement(data.Channel.Ecu, "currentBootSoftware", FormatPartNumberAndDescription(data.Channel.EcuInfos["CO_BootSoftwarePartNumber"], null));
		if (SapiManager.ProgramDeviceUsesSoftwareIdentification(data.Channel.Ecu))
		{
			UpdateHtmlElement(data.Channel.Ecu, "currentSoftware", SapiManager.GetSoftwareIdentification(data.Channel));
		}
		else
		{
			UpdateHtmlElement(data.Channel.Ecu, "currentSoftware", FormatPartNumberAndDescription(data.Channel.EcuInfos["CO_SoftwarePartNumber"], data.Channel.EcuInfos["CO_SoftwareVersion"]));
		}
		foreach (Tuple<int, EcuInfo, EcuInfo> fuelmapEcuInfo in GetFuelmapEcuInfos(data.Channel))
		{
			UpdateHtmlElement(data.Channel.Ecu, string.Format(CultureInfo.InvariantCulture, "currentFuelmap{0}", fuelmapEcuInfo.Item1), FormatPartNumberAndDescription(fuelmapEcuInfo.Item2, fuelmapEcuInfo.Item3));
		}
		string text = data.Channel.EcuInfos["CO_VIN"]?.Value;
		if (text != null)
		{
			UpdateHtmlElement(data.Channel.Ecu, "vin", text, (text != data.VehicleIdentificationNumber) ? "warning" : "standard");
		}
		string text2 = data.Channel.EcuInfos["CO_ESN"]?.Value;
		if (text2 != null)
		{
			UpdateHtmlElement(data.Channel.Ecu, "esn", text2, (text2 != data.EngineSerialNumber) ? "warning" : "standard");
		}
	}

	internal void UpdateStepText(Ecu ecu, string text, ProgrammingStep step, bool failure = false)
	{
		UpdateHtmlElement(ecu, "ecuStep", string.Join(": ", ProgramDeviceManager.GetProgrammingStepDescription(step), text), failure ? "failure" : "standard");
	}

	private string BuildContent()
	{
		StringBuilder stringBuilder = new StringBuilder();
		using (XmlWriter xmlWriter = PrintHelper.CreateWriter(stringBuilder))
		{
			xmlWriter.WriteStartElement("div");
			xmlWriter.WriteStartElement("div");
			WriteAttribute(xmlWriter, "id", "statusDiv");
			WriteAttribute(xmlWriter, "class", "statusDiv");
			xmlWriter.WriteStartElement("table");
			WriteAttribute(xmlWriter, "width", "100%");
			xmlWriter.WriteStartElement("th");
			xmlWriter.WriteStartElement("td");
			WriteAttribute(xmlWriter, "valign", "left");
			WriteAttribute(xmlWriter, "id", "statusHeader");
			WriteAttribute(xmlWriter, "class", "ecu");
			xmlWriter.WriteString(SapiManager.GlobalInstance.LogFileIsOpen ? Resources.ProgramDevicePage_ViewingLogFile : Resources.ProgramDevicePageOutputInfoLabel_IfThisIsCorrectClickTheStartButton);
			xmlWriter.WriteFullEndElement();
			xmlWriter.WriteStartElement("td");
			WriteAttribute(xmlWriter, "style", "text-align: right");
			WriteAttribute(xmlWriter, "valign", "center");
			xmlWriter.WriteStartElement("progress");
			WriteAttribute(xmlWriter, "id", "progressBar");
			WriteAttribute(xmlWriter, "class", "determinateProgress");
			WriteAttribute(xmlWriter, "value", "0");
			WriteAttribute(xmlWriter, "max", "100");
			xmlWriter.WriteFullEndElement();
			xmlWriter.WriteFullEndElement();
			xmlWriter.WriteFullEndElement();
			xmlWriter.WriteFullEndElement();
			xmlWriter.WriteFullEndElement();
			bool flag = programmingDataProvider.ProgrammingData.Count() > 1;
			foreach (ProgrammingData item in programmingDataProvider.ProgrammingData.ToList())
			{
				xmlWriter.WriteStartElement("div");
				WriteAttribute(xmlWriter, "id", item.Channel.Ecu.Name);
				WriteAttribute(xmlWriter, "class", "ecuItemDivHighlight");
				WriteAttribute(xmlWriter, "name", item.Channel.Ecu.Name);
				xmlWriter.WriteStartElement("table");
				WriteAttribute(xmlWriter, "width", "100%");
				xmlWriter.WriteStartElement("th");
				xmlWriter.WriteStartElement("td");
				WriteAttribute(xmlWriter, "valign", "center");
				xmlWriter.WriteStartElement("a");
				WriteAttribute(xmlWriter, "onclick", "ExpandCollapse(this);");
				WriteAttribute(xmlWriter, "href", "javascript:void(0)");
				WriteAttribute(xmlWriter, "class", "link_collapse");
				if (!flag)
				{
					xmlWriter.WriteEntityRef("ndash");
				}
				else
				{
					xmlWriter.WriteString("+");
				}
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("td");
				WriteAttribute(xmlWriter, "valign", "center");
				WriteAttribute(xmlWriter, "class", "ecu");
				xmlWriter.WriteString(GetChannelString(item.Channel));
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("td");
				WriteAttribute(xmlWriter, "style", "text-align: right");
				WriteAttribute(xmlWriter, "valign", "center");
				WriteAttribute(xmlWriter, "class", "ecu");
				WriteAttribute(xmlWriter, "id", string.Format(CultureInfo.InvariantCulture, "{0}_ecuStatus", item.Channel.Ecu.Name));
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("td");
				WriteAttribute(xmlWriter, "style", "text-align: right");
				WriteAttribute(xmlWriter, "valign", "center");
				xmlWriter.WriteStartElement("progress");
				WriteAttribute(xmlWriter, "id", string.Format(CultureInfo.InvariantCulture, "{0}_ecuProgress", item.Channel.Ecu.Name));
				WriteAttribute(xmlWriter, "class", "determinateProgress");
				WriteAttribute(xmlWriter, "value", "0");
				WriteAttribute(xmlWriter, "max", "100");
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("tr");
				xmlWriter.WriteElementString("td", string.Empty);
				xmlWriter.WriteStartElement("td");
				WriteAttribute(xmlWriter, "valign", "center");
				WriteAttribute(xmlWriter, "class", "standard");
				WriteAttribute(xmlWriter, "colspan", "3");
				WriteAttribute(xmlWriter, "id", string.Format(CultureInfo.InvariantCulture, "{0}_ecuStep", item.Channel.Ecu.Name));
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("div");
				WriteAttribute(xmlWriter, "class", (!flag) ? "collapsable_content content_expanded" : "collapsable_content content_collapsed");
				xmlWriter.WriteStartElement("table");
				BuildOperationXml(xmlWriter, item);
				BuildConfigurationXml(xmlWriter, item);
				BuildIdentificationXml(xmlWriter, item);
				BuildHardwareXml(xmlWriter, item);
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("table");
				WriteAttribute(xmlWriter, "id", string.Format(CultureInfo.InvariantCulture, "{0}_ecuParameterWarningsTable", item.Channel.Ecu.Name));
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
			}
			xmlWriter.WriteFullEndElement();
		}
		return stringBuilder.ToString();
	}

	private static void BuildOperationXml(XmlWriter xmlWriter, ProgrammingData programmingData)
	{
		NameValueCollection nameValueCollection = programmingData.NameValueCollection;
		BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_OperationInformation, hasCurrentAndTargetColumns: false);
		string name = ((nameValueCollection[Resources.ProgrammingDataItem_AutomaticOperation] != null) ? Resources.ProgrammingDataItem_AutomaticOperation : Resources.ProgrammingDataItem_Operation);
		BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "operationType", name, nameValueCollection[name], string.Empty);
		if (nameValueCollection[Resources.ProgrammingDataItem_Settings] != null)
		{
			BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "settingsType", Resources.ProgrammingDataItem_Settings, nameValueCollection[Resources.ProgrammingDataItem_Settings], string.Empty);
		}
	}

	private static string FormatPartNumberAndDescription(EcuInfo partNumberEcuInfo, EcuInfo descriptionEcuInfo)
	{
		return FormatPartNumberAndDescription((partNumberEcuInfo != null && partNumberEcuInfo.Value != null) ? new Part(partNumberEcuInfo.Value) : null, descriptionEcuInfo?.Value);
	}

	private static string FormatPartNumberAndDescription(Part partNumber, string description)
	{
		string text = ((partNumber != null) ? partNumber.ToString() : string.Empty);
		if (!string.IsNullOrEmpty(description))
		{
			text = text + " (" + description + ")";
		}
		return text;
	}

	private static IEnumerable<Tuple<int, EcuInfo, EcuInfo>> GetFuelmapEcuInfos(Channel channel)
	{
		EcuInfo[] fuelMapPartNumbers = channel.EcuInfos.Where((EcuInfo ei) => ei.Qualifier.StartsWith("CO_FuelmapPartNumber", StringComparison.OrdinalIgnoreCase)).ToArray();
		EcuInfo[] fuelMapDescriptions = channel.EcuInfos.Where((EcuInfo ei) => ei.Qualifier.StartsWith("CO_FuelmapDescription", StringComparison.OrdinalIgnoreCase)).ToArray();
		for (int i = 0; i < fuelMapPartNumbers.Length; i++)
		{
			yield return Tuple.Create(i, fuelMapPartNumbers[i], (i < fuelMapDescriptions.Length) ? fuelMapDescriptions[i] : null);
		}
	}

	private static void BuildConfigurationXml(XmlWriter xmlWriter, ProgrammingData programmingData)
	{
		List<FlashMeaning> list = SapiManager.GlobalInstance.Sapi.FlashFiles.Where((FlashFile ff) => ff.Ecus.Contains(programmingData.Channel.Ecu.Name)).SelectMany((FlashFile ff) => ff.FlashAreas).SelectMany((FlashArea fa) => fa.FlashMeanings)
			.ToList();
		EdexFileInformation edexFileInformation = programmingData.EdexFileInformation;
		object obj;
		if (edexFileInformation == null)
		{
			obj = null;
		}
		else
		{
			EdexConfigurationInformation configurationInformation = edexFileInformation.ConfigurationInformation;
			obj = ((configurationInformation != null) ? configurationInformation.BootLoaderPartNumber : null);
		}
		Part targetBootLoaderPart = ((obj != null) ? programmingData.EdexFileInformation.ConfigurationInformation.BootLoaderPartNumber : ((programmingData.Bootcode != null) ? new Part(programmingData.Bootcode.Key) : null));
		if (targetBootLoaderPart == null && programmingData.Firmware == null && !programmingData.HasDataSet && !programmingData.HasControlList)
		{
			return;
		}
		BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_Configuration, hasCurrentAndTargetColumns: true);
		if (targetBootLoaderPart != null || programmingData.Bootcode != null)
		{
			EcuInfo ecuInfo = programmingData.Channel.EcuInfos["CO_BootSoftwarePartNumber"];
			FlashMeaning flashMeaning = list.FirstOrDefault((FlashMeaning m) => PartExtensions.IsEqual(targetBootLoaderPart, m.FlashKey));
			BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "currentBootSoftware", Resources.ProgrammingDataItem_BootSoftware, (ecuInfo != null && ecuInfo.Value != null) ? new Part(ecuInfo.Value).ToString() : string.Empty, FormatPartNumberAndDescription(targetBootLoaderPart, flashMeaning?.Name), PartExtensions.IsEqual(targetBootLoaderPart, ecuInfo?.Value) ? "standard" : "info");
		}
		if (programmingData.Firmware != null)
		{
			Part part = new Part(programmingData.Firmware.Key);
			if (SapiManager.ProgramDeviceUsesSoftwareIdentification(programmingData.Channel.Ecu))
			{
				string softwareIdentification = SapiManager.GetSoftwareIdentification(programmingData.Channel);
				string softwareIdentification2 = programmingData.EdexFileInformation.ConfigurationInformation.SoftwareIdentification;
				BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "currentSoftware", Resources.ProgrammingDataItem_Software, softwareIdentification, FormatPartNumberAndDescription(part, softwareIdentification2), softwareIdentification.Equals(softwareIdentification2) ? "standard" : "info");
			}
			else
			{
				EcuInfo ecuInfo2 = programmingData.Channel.EcuInfos["CO_SoftwarePartNumber"];
				BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "currentSoftware", Resources.ProgrammingDataItem_Software, FormatPartNumberAndDescription(ecuInfo2, programmingData.Channel.EcuInfos["CO_SoftwareVersion"]), FormatPartNumberAndDescription(part, programmingData.Firmware.Version), PartExtensions.IsEqual(part, ecuInfo2?.Value) ? "standard" : "info");
			}
		}
		if (programmingData.HasDataSet)
		{
			List<Tuple<int, EcuInfo, EcuInfo>> source = GetFuelmapEcuInfos(programmingData.Channel).ToList();
			Part[] matchedParts = programmingData.GetMatchedDataSetParts(source.Select((Tuple<int, EcuInfo, EcuInfo> fi) => fi.Item2).ToArray(), list);
			foreach (Tuple<int, EcuInfo, EcuInfo, Part> datasetInfo in from fi in source
				select Tuple.Create(fi.Item1, fi.Item2, fi.Item3, matchedParts[fi.Item1]) into cs
				orderby cs.Item4 != null descending
				select cs)
			{
				FlashMeaning flashMeaning2 = list.FirstOrDefault((FlashMeaning fm) => PartExtensions.IsEqual(datasetInfo.Item4, fm.FlashKey));
				BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, string.Format(CultureInfo.InvariantCulture, "currentFuelmap{0}", datasetInfo.Item1), datasetInfo.Item2.Name, FormatPartNumberAndDescription(datasetInfo.Item2, datasetInfo.Item3), FormatPartNumberAndDescription(datasetInfo.Item4, flashMeaning2?.Name), PartExtensions.IsEqual(datasetInfo.Item4, datasetInfo.Item2?.Value) ? "standard" : "info");
			}
			foreach (Part targetPart in from p in programmingData.DataSetVersions.Except(matchedParts)
				orderby p.Number
				select p)
			{
				FlashMeaning flashMeaning3 = list.FirstOrDefault((FlashMeaning fm) => PartExtensions.IsEqual(targetPart, fm.FlashKey));
				BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, string.Empty, Resources.ProgrammingDataItem_Dataset, string.Empty, FormatPartNumberAndDescription(targetPart, flashMeaning3?.Name));
			}
		}
		if (programmingData.HasControlList)
		{
			Part targetControlListPartNumber = new Part(programmingData.ControlList.Key);
			EcuInfo ecuInfo3 = programmingData.Channel.EcuInfos["CO_ControlListSoftwarePartNumber"];
			FlashMeaning flashMeaning4 = list.FirstOrDefault((FlashMeaning m) => PartExtensions.IsEqual(targetControlListPartNumber, m.FlashKey));
			BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "currentControlListSoftware", "Control List Software", (ecuInfo3 != null && ecuInfo3.Value != null) ? new Part(ecuInfo3.Value).ToString() : string.Empty, FormatPartNumberAndDescription(targetControlListPartNumber, flashMeaning4?.Name), PartExtensions.IsEqual(targetControlListPartNumber, ecuInfo3?.Value) ? "standard" : "info");
		}
	}

	private static void BuildIdentificationXml(XmlWriter xmlWriter, ProgrammingData programmingData)
	{
		BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_Identification, hasCurrentAndTargetColumns: true);
		if (!string.IsNullOrEmpty(programmingData.PreviousVehicleIdentificationNumber) || !string.IsNullOrEmpty(programmingData.VehicleIdentificationNumber))
		{
			BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "vin", Resources.ProgrammingDataItem_VehicleIdentificationNumber, programmingData.PreviousVehicleIdentificationNumber, programmingData.VehicleIdentificationNumber, (string.IsNullOrEmpty(programmingData.PreviousVehicleIdentificationNumber) || programmingData.PreviousVehicleIdentificationNumber == programmingData.VehicleIdentificationNumber) ? "standard" : "warning");
		}
		if (!string.IsNullOrEmpty(programmingData.PreviousEngineSerialNumber) || !string.IsNullOrEmpty(programmingData.EngineSerialNumber))
		{
			BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "esn", Resources.ProgrammingDataItem_EngineSerialNumber, programmingData.PreviousEngineSerialNumber, programmingData.EngineSerialNumber, (string.IsNullOrEmpty(programmingData.PreviousEngineSerialNumber) || programmingData.PreviousEngineSerialNumber == programmingData.EngineSerialNumber) ? "standard" : "warning");
		}
	}

	private static void BuildHardwareXml(XmlWriter xmlWriter, ProgrammingData programmingData)
	{
		string text = programmingData.Channel.EcuInfos["CO_EcuSerialNumber"]?.Value;
		string text2 = programmingData.Channel.EcuInfos["CO_HardwarePartNumber"]?.Value;
		string previousHardwareRevision = programmingData.PreviousHardwareRevision;
		if (text != null || text2 != null || previousHardwareRevision != null)
		{
			BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_Hardware, hasCurrentAndTargetColumns: false);
			if (text != null)
			{
				BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "ecuSerialNumber", Resources.ProgramDevicePage_ECUSerialNumber, text, string.Empty);
			}
			if (text2 != null)
			{
				BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "hardwarePartNumber", Resources.ProgrammingDataItem_HardwarePartNumber, PartExtensions.ToHardwarePartNumberString(new Part(text2), programmingData.Channel.Ecu, true), string.Empty);
			}
			if (!string.IsNullOrEmpty(previousHardwareRevision))
			{
				BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "hardwareRevision", Resources.ProgrammingDataItem_HardwareRevision, previousHardwareRevision, string.Empty);
			}
		}
	}

	private static void BuildXmlTableGroupHeader(XmlWriter xmlWriter, string groupName, bool hasCurrentAndTargetColumns)
	{
		xmlWriter.WriteStartElement("tr");
		xmlWriter.WriteStartElement("td");
		WriteAttribute(xmlWriter, "class", "group");
		xmlWriter.WriteString(groupName);
		xmlWriter.WriteFullEndElement();
		if (hasCurrentAndTargetColumns)
		{
			xmlWriter.WriteStartElement("td");
			WriteAttribute(xmlWriter, "class", "group");
			xmlWriter.WriteString("Current");
			xmlWriter.WriteFullEndElement();
			xmlWriter.WriteStartElement("td");
			WriteAttribute(xmlWriter, "class", "group");
			xmlWriter.WriteString("Target");
			xmlWriter.WriteFullEndElement();
		}
		xmlWriter.WriteFullEndElement();
	}

	private static void BuildXmlTableRow(XmlWriter xmlWriter, Ecu ecu, string id, string name, string current, string target, string currentClass = "standard", string targetClass = "standard")
	{
		xmlWriter.WriteStartElement("tr");
		xmlWriter.WriteStartElement("td");
		WriteAttribute(xmlWriter, "class", "standard");
		xmlWriter.WriteString(name);
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteStartElement("td");
		WriteAttribute(xmlWriter, "class", currentClass);
		if (string.IsNullOrEmpty(target))
		{
			WriteAttribute(xmlWriter, "colspan", "2");
		}
		WriteAttribute(xmlWriter, "id", string.Format(CultureInfo.InvariantCulture, "{0}_{1}", ecu.Name, id));
		xmlWriter.WriteString(current);
		xmlWriter.WriteFullEndElement();
		if (!string.IsNullOrEmpty(target))
		{
			xmlWriter.WriteStartElement("td");
			WriteAttribute(xmlWriter, "class", targetClass);
			xmlWriter.WriteString(target);
			xmlWriter.WriteFullEndElement();
		}
		xmlWriter.WriteFullEndElement();
	}

	private void PopulateParameterWarnings(Ecu ecu, List<string> warnings)
	{
		HtmlElement htmlElement = FindHtmlElementById(string.Format(CultureInfo.InvariantCulture, "{0}_ecuParameterWarningsTable", ecu.Name));
		if (!(htmlElement != null))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		using (XmlWriter xmlWriter = PrintHelper.CreateWriter(stringBuilder))
		{
			xmlWriter.WriteElementString("tr", string.Empty);
			BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_ParameterWarnings, hasCurrentAndTargetColumns: false);
			foreach (string warning in warnings)
			{
				xmlWriter.WriteStartElement("tr");
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteAttributeString("class", "parameter warning");
				xmlWriter.WriteString(warning);
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
			}
		}
		htmlElement.InnerHtml = stringBuilder.ToString();
	}

	private static string GetChannelString(Channel channel)
	{
		string text = channel.Ecu.Name;
		if (!string.IsNullOrEmpty(channel.Ecu.ShortDescription))
		{
			text = text + " - " + channel.Ecu.ShortDescription;
		}
		return text;
	}

	internal override WizardPage OnWizardNext()
	{
		return base.Wizard.Pages[0];
	}

	internal override WizardPage OnWizardBack()
	{
		if (programmingDataProvider.ProgrammingData.FirstOrDefault() != null && programmingDataProvider.ProgrammingData.First().AutomaticOperation != null)
		{
			return base.Wizard.Pages[0];
		}
		return base.OnWizardBack();
	}

	internal override void OnSetActive()
	{
		foreach (Precondition programmingPrecondition in programmingPreconditions)
		{
			programmingPrecondition.StateChanged += Precondition_ValueStateChanged;
		}
		ShowContent();
		UpdatePreconditions();
		UpdateUi(PageActionState.ReadyToStart);
	}

	internal override void OnSetInactive()
	{
		foreach (Precondition programmingPrecondition in programmingPreconditions)
		{
			programmingPrecondition.StateChanged -= Precondition_ValueStateChanged;
		}
	}

	private void Precondition_ValueStateChanged(object sender, EventArgs e)
	{
		if (base.Wizard.ActivePage == this && (pageState == PageActionState.ReadyToStart || pageState == PageActionState.ReadyToRetry))
		{
			UpdatePreconditions();
			UpdateUi(pageState);
		}
	}

	private void UpdatePreconditions()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!FailedPreconditions.Any())
		{
			preconditionsPane.InnerHtml = "&nbsp;";
			preconditionsPane.SetAttribute("className", "hide");
			return;
		}
		string text = string.Empty;
		foreach (Precondition failedPrecondition in FailedPreconditions)
		{
			string text2 = string.Empty;
			if (failedPrecondition.DiagnosticPanelName != null)
			{
				text2 = FormattableString.Invariant($"<a href='#precondition_{failedPrecondition.PreconditionType}'>{ActionsMenuProxy.GlobalInstance.GetDialogLocalizedDisplayName(failedPrecondition.DiagnosticPanelName, (string)null, false)}</a>");
			}
			text += FormattableString.Invariant($"<div><span class='warninginline'>{string.Format(CultureInfo.CurrentCulture, Resources.ProgramDevicePage_FormatPreconditionNotMet, failedPrecondition.Text)}</span>&nbsp;{text2}</div>");
		}
		preconditionsPane.InnerHtml = text;
		preconditionsPane.SetAttribute("className", "show");
	}

	private static void SetUIEnabled(bool enable)
	{
		Form form = Application.OpenForms[0];
		if (form == null)
		{
			return;
		}
		MenuProxy.GlobalInstance.ContainerApplication.DisableForProgramming(!enable);
		foreach (Control control in form.Controls)
		{
			if (control.GetType().Name != "TabbedView")
			{
				control.Enabled = enable;
			}
		}
	}

	internal override void Navigate(string fragment)
	{
		if (fragment == "#button_start")
		{
			if (pageState == PageActionState.ReadyToStart)
			{
				StartProgramming();
			}
			else if (pageState == PageActionState.ReadyToRetry)
			{
				ProcessNextEcu(isRetry: true);
			}
		}
		if (fragment.StartsWith("#precondition_", StringComparison.OrdinalIgnoreCase))
		{
			string preconditionType = fragment.Split("_".ToCharArray())[1];
			Precondition val = programmingPreconditions.First((Precondition pc) => ((object)pc.PreconditionType/*cast due to .constrained prefix*/).ToString() == preconditionType);
			ActionsMenuProxy.GlobalInstance.ShowDialog(val.DiagnosticPanelName, (string)null, (object)null, false);
		}
	}

	private void StartProgramming()
	{
		if (!programmingDataProvider.RequiresCompatibilityChecks || CheckCompatibility(programmingDataProvider.ProgrammingData.Single()))
		{
			programmingIndex = -1;
			UpdateOverallStatus(Resources.ProgramDeviceManager_Step_Starting, Resources.ProgramDevicePage_PleaseWait);
			((Control)(object)base.Wizard).Invalidate();
			UpdateUi(PageActionState.Busy);
			Application.DoEvents();
			DisconnectChannels(saveConnectedList: true);
			ProcessNextEcu(isRetry: false);
		}
	}

	private void ProcessNextEcu(bool isRetry)
	{
		if (!isRetry)
		{
			programmingIndex++;
		}
		if (programmingDataProvider.ProgrammingData.Count() > programmingIndex)
		{
			UpdateUi(PageActionState.Busy);
			if (programDeviceManager != null)
			{
				programDeviceManager.Dispose();
			}
			programDeviceManager = new ProgramDeviceManager(this);
			ProgrammingData programmingData = programmingDataProvider.ProgrammingData.Skip(programmingIndex).First();
			startingVariant = new Tuple<string, string>(programmingData.Channel.Ecu.Name, programmingData.Channel.DiagnosisVariant.ToString());
			DisconnectChannels(saveConnectedList: false);
			HideOverallProgress(hidden: false);
			programDeviceManager.Start(programmingData);
			UpdateEcuProcessStatus(programmingData.Channel.Ecu, ProcessStatus.Processing);
			UpdateOverallStatus(Resources.ProgramDevicePage_Processing, GetChannelString(programmingData.Channel));
		}
		else
		{
			AllDone();
		}
	}

	internal void Complete(ProgrammingData programmingData, bool success, ProgrammingStep terminalStep, string message, List<string> parameterErrorsForEcu, Dictionary<string, CaesarException> partNumberExceptionsForEcu, bool canRetry)
	{
		if (parameterErrorsForEcu.Any())
		{
			parameterErrors[programmingData.Channel.Ecu] = parameterErrorsForEcu;
		}
		UpdateEcuConfiguration(programmingData);
		if (success)
		{
			UpdateEcuProcessStatus(programmingData.Channel.Ecu, ProcessStatus.Pass);
			ProcessNextEcu(isRetry: false);
			return;
		}
		UpdateEcuProcessStatus(programmingData.Channel.Ecu, ProcessStatus.Fail);
		UpdateStepText(programmingData.Channel.Ecu, message, terminalStep, failure: true);
		if (canRetry)
		{
			UpdatePreconditions();
			UpdateUi(PageActionState.ReadyToRetry);
			UpdateOverallStatus(Resources.ProgramDevicePage_Error, Resources.ProgramDevicePageOutputInfoLabel_TheProgrammingOperationFailedToRetryClickTheStartButton);
		}
		else
		{
			UpdateUi(PageActionState.FinishedFailed);
			UpdateOverallStatus(Resources.ProgramDevicePage_Error, Resources.ProgramDevicePageOutputInfoLabel_ProgrammingFailedUnableToContinue);
		}
		if (partNumberExceptionsForEcu != null && partNumberExceptionsForEcu.Any())
		{
			Application.DoEvents();
			PartNumberErrorDialog partNumberErrorDialog = new PartNumberErrorDialog(programmingData.Channel, partNumberExceptionsForEcu);
			((Form)(object)partNumberErrorDialog).ShowDialog();
		}
	}

	private void DisconnectChannels(bool saveConnectedList)
	{
		ChannelCollection channels = SapiManager.GlobalInstance.Sapi.Channels;
		channels.ConnectCompleteEvent -= channels_ConnectCompleteEvent;
		if (saveConnectedList)
		{
			devicesToClearFaultsFor.Clear();
			manualConnections.Clear();
		}
		SapiManager.GlobalInstance.SuspendAutoConnect();
		int num;
		for (num = 0; num < channels.Count; num++)
		{
			Channel channel = channels[num];
			channel.Disconnect();
			num--;
			if (saveConnectedList)
			{
				devicesToClearFaultsFor.Add(channel.Ecu);
				if (!channel.Ecu.IsRollCall && !channel.Ecu.MarkedForAutoConnect)
				{
					manualConnections.Add(channel.ConnectionResource);
				}
			}
		}
	}

	private void ReconnectChannels()
	{
		SapiManager.GlobalInstance.Sapi.Channels.ConnectCompleteEvent += channels_ConnectCompleteEvent;
		foreach (Channel channel in SapiManager.GlobalInstance.Sapi.Channels)
		{
			ResetFaultsIfRequired(channel);
		}
		SapiManager.GlobalInstance.ResumeAutoConnect();
		foreach (ConnectionResource manualConnection in manualConnections)
		{
			SapiManager.GlobalInstance.Sapi.Channels.Connect(manualConnection, synchronous: false);
		}
		manualConnections.Clear();
	}

	private void ResetFaultsIfRequired(Channel channel)
	{
		if (devicesToClearFaultsFor.Contains(channel.Ecu) && channel.Online)
		{
			channel.FaultCodes.Reset(synchronous: false);
			devicesToClearFaultsFor.Remove(channel.Ecu);
		}
	}

	private void channels_ConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded && sender is Channel channel)
		{
			ResetFaultsIfRequired(channel);
		}
	}

	private void AllDone()
	{
		HideOverallProgress(hidden: true);
		if (parameterErrors.Any())
		{
			UpdateOverallStatus(Resources.ProgramDevicePage_Success, Resources.ProgramDevicePageOutputInfoLabel_TheDeviceWasSuccessfullyProgrammedButMayHaveConfigurationErrors);
			foreach (Ecu ecu in parameterErrors.Keys)
			{
				ProgrammingData programmingData = programmingDataProvider.ProgrammingData.FirstOrDefault((ProgrammingData d) => d.Channel.Ecu == ecu);
				if (programmingData != null)
				{
					PopulateParameterWarnings(ecu, parameterErrors[ecu]);
				}
			}
		}
		else
		{
			UpdateOverallStatus(Resources.ProgramDevicePage_Success, Resources.ProgramDevicePageOutputInfoLabel_TheDeviceWasSuccessfullyProgrammed);
		}
		ReconnectChannels();
		if (informUserOfOfflineDepotCompatibility)
		{
			ShowOfflineCompatibilityForm(offlineSoftwareTarget, offlineCompatibilitySoftwareCollection);
		}
		if (informUserOfOfflineEdexCompatibility)
		{
			ShowOfflineCompatibilityForm(offlineCompatibilityTarget, offlineCompatiblityConfigurationCollection);
		}
		List<KeyValuePair<Ecu, string>> source = (from deviceInfo in programmingDataProvider.ProgrammingData.First().Unit.DeviceInformation
			let ecu = SapiManager.GetEcuByName(deviceInfo.Device)
			where ecu != null && ecu.Properties.ContainsKey("PromptForCheckAfterOtherEcuVariantChange")
			let feature = ecu.Properties["PromptForCheckAfterOtherEcuVariantChange"]
			select new KeyValuePair<Ecu, string>(ecu, PanelBase.GetLocalizedDisplayName(feature) ?? feature)).ToList();
		Channel channel = programmingDataProvider.ProgrammingData.FirstOrDefault((ProgrammingData c) => c.Channel.Ecu.Name == startingVariant.Item1)?.Channel;
		if (channel != null && channel.DiagnosisVariant.ToString() != startingVariant.Item2)
		{
			List<KeyValuePair<Ecu, string>> source2 = source.Where((KeyValuePair<Ecu, string> d) => programmingDataProvider.ProgrammingData.All((ProgrammingData x) => x.Channel.Ecu != d.Key)).ToList();
			if (source2.Any())
			{
				string text = string.Format(CultureInfo.CurrentCulture, Resources.ProgramDevicePage_CheckDependentDeviceFeaturesAfterProgramming, string.Join(Environment.NewLine, source2.Select((KeyValuePair<Ecu, string> e) => e.Key.DisplayName + ": " + e.Value)));
				ControlHelpers.ShowMessageBox(text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
		}
		if (NetworkSettings.GlobalInstance.SaveUploadContent && ApplicationInformation.MustConnectAfterTroubleshootingOrReprogramming)
		{
			Collection<UnitInformation> collection = new Collection<UnitInformation>();
			ServerDataManager.GlobalInstance.GetUploadUnits(collection, (UploadType)0);
			ServerClient.GlobalInstance.Upload(collection, Application.OpenForms.OfType<IProgressBar>().First());
		}
		if (pageState == PageActionState.Busy)
		{
			bool flag = programmingDataProvider.ProgrammingData.FirstOrDefault() != null && programmingDataProvider.ProgrammingData.First().AutomaticOperation != null;
			UpdateUi(flag ? PageActionState.FinishedAutomaticOperation : PageActionState.Finished);
		}
	}

	private static void ShowOfflineCompatibilityForm(Software targetSoftware, CompatibleSoftwareCollection compatibleSoftwareCollection)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (targetSoftware != null && compatibleSoftwareCollection != null && ((ReadOnlyCollection<SoftwareCollection>)(object)compatibleSoftwareCollection).Count > 0)
		{
			CompatibilityWarningDialog val = new CompatibilityWarningDialog(Resources.ProgramDevicePage_OfflineDeviceCompatibility_Information, targetSoftware, Resources.ProgramDevicePage_Compat_TargetDevice);
			val.AddCompatibilityInfo(compatibleSoftwareCollection, Resources.ProgramDevicePage_Compat_CompatibleSetFormat);
			((Form)(object)val).ShowDialog();
		}
	}

	private static void ShowOfflineCompatibilityForm(EdexCompatibilityEcuItem targetEcuItem, EdexCompatibilityConfigurationCollection compatibilityConfigurationCollection)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (targetEcuItem != null && compatibilityConfigurationCollection != null && ((ReadOnlyCollection<EdexCompatibilityConfiguration>)(object)compatibilityConfigurationCollection).Count > 0)
		{
			CompatibilityWarningDialog val = new CompatibilityWarningDialog(Resources.ProgramDevicePage_OfflineDeviceCompatibility_Information, targetEcuItem, Resources.ProgramDevicePage_Compat_TargetDevice);
			val.AddCompatibilityInfo(compatibilityConfigurationCollection, Resources.ProgramDevicePage_Compat_CompatibleSetFormat);
			((Form)(object)val).ShowDialog();
		}
	}

	private bool CheckCompatibility(ProgrammingData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		DeviceDataSource dataSource = data.DataSource;
		if ((int)dataSource != 1)
		{
			if ((int)dataSource == 2 && !data.Unit.InAutomaticOperation && !SapiManager.GlobalInstance.IgnoreSoftwareCompatibilityChecks(data.Channel))
			{
				EdexFileInformation edexFileInformation = data.EdexFileInformation;
				if (((edexFileInformation != null) ? edexFileInformation.ConfigurationInformation : null) != null)
				{
					EdexConfigurationInformation configurationInformation = data.EdexFileInformation.ConfigurationInformation;
					EdexCompatibilityEcuItem targetEcuItem = new EdexCompatibilityEcuItem(configurationInformation.DeviceName, configurationInformation.HardwarePartNumber, configurationInformation.FlashwarePartNumber);
					CheckOfflineCompatibility(targetEcuItem, data.Unit);
				}
			}
		}
		else if (data.Firmware != null)
		{
			FirmwareInformation firmware = data.Firmware;
			Software val = new Software(firmware.Device, firmware.Version, SapiManager.GetHardwarePartNumber(data.Channel));
			if (!ServerDataManager.GlobalInstance.CompatibilityTable.IsHardwareCompatibleWithSoftware(val))
			{
				IList<Part> list = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleHardwareList(val);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.ProgramDevicePage_FormatIncompatibleHardware, firmware.Device);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				foreach (Part item in list)
				{
					stringBuilder.AppendLine(PartExtensions.ToHardwarePartNumberString(item, firmware.Device, true));
				}
				CustomMessageBox.Show((IWin32Window)null, stringBuilder.ToString(), Resources.ProgramDevicePage_IncompatibleHardwareDialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, (CustomMessageBoxOptions)3);
				UpdateOverallStatus(Resources.ProgramDevicePage_Failed, Resources.ProgramDevicePageOutputInfoLabel_ProgrammingFailedUnableToContinue);
				UpdateUi(PageActionState.Error);
				return false;
			}
			if (!data.Unit.InAutomaticOperation && !data.Unit.UnitFixedAtTest && !SapiManager.GlobalInstance.IgnoreSoftwareCompatibilityChecks(data.Channel))
			{
				CheckOfflineCompatibility(val, data.Unit);
			}
		}
		return true;
	}

	private void CheckOfflineCompatibility(Software targetSoftware, UnitInformation unit)
	{
		offlineSoftwareTarget = targetSoftware;
		CompatibleSoftwareCollection val = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleList(targetSoftware);
		CompatibleSoftwareCollection val2 = val.FilterForUnit(unit);
		offlineCompatibilitySoftwareCollection = val2.FilterForOfflineDevices();
		informUserOfOfflineDepotCompatibility = ((ReadOnlyCollection<SoftwareCollection>)(object)offlineCompatibilitySoftwareCollection).Count > 0;
	}

	private void CheckOfflineCompatibility(EdexCompatibilityEcuItem targetEcuItem, UnitInformation unit)
	{
		offlineCompatibilityTarget = targetEcuItem;
		EdexCompatibilityConfigurationCollection val = ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(targetEcuItem);
		EdexCompatibilityConfigurationCollection val2 = val.FilterForUnit(unit);
		offlineCompatiblityConfigurationCollection = val2.FilterForOfflineDevices();
		informUserOfOfflineEdexCompatibility = ((ReadOnlyCollection<EdexCompatibilityConfiguration>)(object)offlineCompatiblityConfigurationCollection).Count > 0;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (programDeviceManager != null)
			{
				programDeviceManager.Dispose();
				programDeviceManager = null;
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
