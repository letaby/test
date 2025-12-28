// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgramDevicePage
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Xml;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class ProgramDevicePage : WizardPage, IDisposable
{
  private HtmlElement inputPane;
  private readonly Dictionary<Ecu, List<string>> parameterErrors = new Dictionary<Ecu, List<string>>();
  private ProgramDevicePage.PageActionState pageState = ProgramDevicePage.PageActionState.Unknown;
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

  public ProgramDevicePage(ReprogrammingView dataProvider, HtmlElement inputPane)
    : base(dataProvider, inputPane)
  {
    this.programmingDataProvider = (IProvideProgrammingData) dataProvider;
    this.programDeviceManager = new ProgramDeviceManager(this);
    LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadStatusChanged += new EventHandler(this.GlobalInstance_RemoteFileDownloadStatusChanged);
    this.SetInputPane(inputPane);
    this.programmingPreconditions = PreconditionManager.GlobalInstance.Preconditions.Where<Precondition>((Func<Precondition, bool>) (p => p.PreconditionType == 1 || p.PreconditionType == 3 || p.PreconditionType == 2)).ToList<Precondition>();
  }

  private void GlobalInstance_RemoteFileDownloadStatusChanged(object sender, EventArgs e)
  {
    if (this.Wizard.ActivePage != this)
      return;
    ReprogrammingView.UpdateTitle(this.titlePane, this.programmingDataProvider.SelectedUnit);
  }

  private void UpdateUi(ProgramDevicePage.PageActionState state)
  {
    this.pageState = state;
    switch (this.pageState)
    {
      case ProgramDevicePage.PageActionState.ReadyToStart:
      case ProgramDevicePage.PageActionState.ReadyToRetry:
        ProgramDevicePage.SetUIEnabled(true);
        ReprogrammingView.SetButtonState(this.startButton, !SapiManager.GlobalInstance.LogFileIsOpen && !this.FailedPreconditions.Any<Precondition>(), true);
        this.Wizard.SetWizardButtons((WizardControl.WizardButtons) 1);
        break;
      case ProgramDevicePage.PageActionState.Busy:
        ProgramDevicePage.SetUIEnabled(false);
        ReprogrammingView.SetButtonState(this.startButton, false, true);
        this.Wizard.SetWizardButtons((WizardControl.WizardButtons) 0);
        break;
      case ProgramDevicePage.PageActionState.Finished:
        ProgramDevicePage.SetUIEnabled(true);
        ReprogrammingView.SetButtonState(this.startButton, false, false);
        this.Wizard.SetWizardButtons((WizardControl.WizardButtons) 4);
        break;
      case ProgramDevicePage.PageActionState.FinishedAutomaticOperation:
        ProgramDevicePage.SetUIEnabled(true);
        ReprogrammingView.SetButtonState(this.startButton, false, false);
        this.Wizard.SetWizardButtons((WizardControl.WizardButtons) 2);
        break;
      case ProgramDevicePage.PageActionState.FinishedFailed:
      case ProgramDevicePage.PageActionState.Error:
        ProgramDevicePage.SetUIEnabled(true);
        ReprogrammingView.SetButtonState(this.startButton, false, true);
        this.Wizard.SetWizardButtons((WizardControl.WizardButtons) 1);
        break;
      case ProgramDevicePage.PageActionState.Unknown:
        ProgramDevicePage.SetUIEnabled(true);
        ReprogrammingView.SetButtonState(this.startButton, false, true);
        this.Wizard.SetWizardButtons((WizardControl.WizardButtons) 1);
        break;
    }
  }

  internal void SetInputPane(HtmlElement inputPane)
  {
    inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("\r\n                    <div>\r\n                        <div id='titlePane'>&nbsp;</div>\r\n                        <div id='programDevicePane'>&nbsp;</div>\r\n                        <br/><br/><br/><br/>\r\n                        <div id='bottomPanel' class='fixedbottom litetransparency'>\r\n                            <div id='preconditionsPane'>&nbsp;</div>\r\n                            <div>\r\n                                <button id='backButtonPDP' class='button blue show' onClick=\"clickButton('back')\">{0}</button>\r\n                                <button id='nextButtonPDP' class='button gray hide' onClick=\"clickButton('next')\">{1}</button>\r\n                                <button id='startButton' class='button gray show' disabled onClick=\"clickButton('start')\">{2}</button>\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n                ", (object) Resources.Wizard_ButtonBack, (object) Resources.Wizard_ButtonNext, (object) Resources.Wizard_ButtonStart));
    foreach (HtmlElement htmlElement in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList<HtmlElement>())
    {
      switch (htmlElement.Id)
      {
        case "titlePane":
          this.titlePane = htmlElement;
          continue;
        case "programDevicePane":
          this.inputPane = htmlElement;
          continue;
        case "preconditionsPane":
          this.preconditionsPane = htmlElement;
          continue;
        default:
          continue;
      }
    }
    foreach (HtmlElement htmlElement in inputPane.GetElementsByTagName("button").OfType<HtmlElement>().ToList<HtmlElement>())
    {
      switch (htmlElement.Id)
      {
        case "backButtonPDP":
          this.BackButton = htmlElement;
          continue;
        case "nextButtonPDP":
          this.NextButton = htmlElement;
          continue;
        case "startButton":
          this.startButton = htmlElement;
          continue;
        default:
          continue;
      }
    }
  }

  private void ShowContent()
  {
    if (!(this.inputPane != (HtmlElement) null))
      return;
    ReprogrammingView.UpdateTitle(this.titlePane, this.programmingDataProvider.SelectedUnit);
    this.inputPane.InnerHtml = this.BuildContent();
  }

  private HtmlElement FindHtmlElementById(string id) => this.inputPane.Document.GetElementById(id);

  private void UpdateHtmlElement(string id, string text)
  {
    HtmlElement htmlElementById = this.FindHtmlElementById(id);
    if (!(htmlElementById != (HtmlElement) null))
      return;
    htmlElementById.InnerText = text;
  }

  private void UpdateHtmlElement(Ecu ecu, string id, string text, string updatedClass = null)
  {
    HtmlElement htmlElementById = this.FindHtmlElementById(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) ecu.Name, (object) id));
    if (!(htmlElementById != (HtmlElement) null))
      return;
    htmlElementById.InnerText = text;
    if (updatedClass == null)
      return;
    htmlElementById.SetAttribute("className", updatedClass);
  }

  private void UpdateHtmlElementClass(string id, string className)
  {
    HtmlElement htmlElementById = this.FindHtmlElementById(id);
    if (!(htmlElementById != (HtmlElement) null))
      return;
    htmlElementById.SetAttribute(nameof (className), className);
  }

  private static void WriteAttribute(XmlWriter xmlWriter, string name, string value)
  {
    xmlWriter.WriteStartAttribute(name);
    xmlWriter.WriteString(value);
    xmlWriter.WriteEndAttribute();
  }

  private void UpdateOverallStatus(string header, string content)
  {
    this.UpdateHtmlElement("statusHeader", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) string.Join(": ", header, content)));
  }

  private void UpdateOverallProgress(double value)
  {
    HtmlElement htmlElementById = this.FindHtmlElementById("progressBar");
    if (!(htmlElementById != (HtmlElement) null))
      return;
    htmlElementById.SetAttribute(nameof (value), value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  private void HideOverallProgress(bool hidden)
  {
    if (!hidden)
      this.UpdateHtmlElementClass("progressBar", "determinateProgress");
    else
      this.UpdateHtmlElementClass("progressBar", "determinateProgressHidden");
  }

  internal void UpdateEcuProgress(Ecu ecu, double value)
  {
    HtmlElement htmlElementById = this.FindHtmlElementById(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_ecuProgress", (object) ecu.Name));
    if (htmlElementById != (HtmlElement) null)
      htmlElementById.SetAttribute(nameof (value), value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    this.UpdateOverallProgress(((double) this.programmingIndex + value / 100.0) / (double) this.programmingDataProvider.ProgrammingData.Count<ProgrammingData>() * 100.0);
  }

  private void UpdateEcuProgressBarClass(Ecu ecu, string className)
  {
    this.UpdateHtmlElementClass(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_ecuProgress", (object) ecu.Name), className);
  }

  private void UpdateEcuProcessStatus(Ecu ecu, ProgramDevicePage.ProcessStatus status)
  {
    switch (status)
    {
      case ProgramDevicePage.ProcessStatus.Pass:
        this.UpdateEcuProgress(ecu, 100.0);
        this.UpdateEcuProgressBarClass(ecu, "determinateProgressPass");
        this.UpdateHtmlElement(ecu, "ecuStatus", Resources.ProgramDevicePage_Success);
        this.UpdateHtmlElement(ecu, "ecuStep", string.Empty);
        break;
      case ProgramDevicePage.ProcessStatus.Fail:
        this.UpdateEcuProgress(ecu, 100.0);
        this.UpdateEcuProgressBarClass(ecu, "determinateProgressFail");
        this.UpdateHtmlElement(ecu, "ecuStatus", Resources.ProgramDevicePage_Failed);
        break;
      case ProgramDevicePage.ProcessStatus.Processing:
        this.UpdateEcuProgressBarClass(ecu, "determinateProgress");
        this.UpdateHtmlElement(ecu, "ecuStatus", Resources.ProgramDevicePage_Processing);
        break;
    }
  }

  private void UpdateEcuConfiguration(ProgrammingData data)
  {
    if (data?.Channel == null)
      return;
    this.UpdateHtmlElement(data.Channel.Ecu, "currentBootSoftware", ProgramDevicePage.FormatPartNumberAndDescription(data.Channel.EcuInfos["CO_BootSoftwarePartNumber"], (EcuInfo) null));
    if (SapiManager.ProgramDeviceUsesSoftwareIdentification(data.Channel.Ecu))
      this.UpdateHtmlElement(data.Channel.Ecu, "currentSoftware", SapiManager.GetSoftwareIdentification(data.Channel));
    else
      this.UpdateHtmlElement(data.Channel.Ecu, "currentSoftware", ProgramDevicePage.FormatPartNumberAndDescription(data.Channel.EcuInfos["CO_SoftwarePartNumber"], data.Channel.EcuInfos["CO_SoftwareVersion"]));
    foreach (Tuple<int, EcuInfo, EcuInfo> fuelmapEcuInfo in ProgramDevicePage.GetFuelmapEcuInfos(data.Channel))
      this.UpdateHtmlElement(data.Channel.Ecu, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "currentFuelmap{0}", (object) fuelmapEcuInfo.Item1), ProgramDevicePage.FormatPartNumberAndDescription(fuelmapEcuInfo.Item2, fuelmapEcuInfo.Item3));
    string text1 = data.Channel.EcuInfos["CO_VIN"]?.Value;
    if (text1 != null)
      this.UpdateHtmlElement(data.Channel.Ecu, "vin", text1, text1 != data.VehicleIdentificationNumber ? "warning" : "standard");
    string text2 = data.Channel.EcuInfos["CO_ESN"]?.Value;
    if (text2 == null)
      return;
    this.UpdateHtmlElement(data.Channel.Ecu, "esn", text2, text2 != data.EngineSerialNumber ? "warning" : "standard");
  }

  internal void UpdateStepText(Ecu ecu, string text, ProgrammingStep step, bool failure = false)
  {
    this.UpdateHtmlElement(ecu, "ecuStep", string.Join(": ", ProgramDeviceManager.GetProgrammingStepDescription(step), text), failure ? nameof (failure) : "standard");
  }

  private string BuildContent()
  {
    StringBuilder stringBuilder = new StringBuilder();
    using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
    {
      writer.WriteStartElement("div");
      writer.WriteStartElement("div");
      ProgramDevicePage.WriteAttribute(writer, "id", "statusDiv");
      ProgramDevicePage.WriteAttribute(writer, "class", "statusDiv");
      writer.WriteStartElement("table");
      ProgramDevicePage.WriteAttribute(writer, "width", "100%");
      writer.WriteStartElement("th");
      writer.WriteStartElement("td");
      ProgramDevicePage.WriteAttribute(writer, "valign", "left");
      ProgramDevicePage.WriteAttribute(writer, "id", "statusHeader");
      ProgramDevicePage.WriteAttribute(writer, "class", "ecu");
      writer.WriteString(SapiManager.GlobalInstance.LogFileIsOpen ? Resources.ProgramDevicePage_ViewingLogFile : Resources.ProgramDevicePageOutputInfoLabel_IfThisIsCorrectClickTheStartButton);
      writer.WriteFullEndElement();
      writer.WriteStartElement("td");
      ProgramDevicePage.WriteAttribute(writer, "style", "text-align: right");
      ProgramDevicePage.WriteAttribute(writer, "valign", "center");
      writer.WriteStartElement("progress");
      ProgramDevicePage.WriteAttribute(writer, "id", "progressBar");
      ProgramDevicePage.WriteAttribute(writer, "class", "determinateProgress");
      ProgramDevicePage.WriteAttribute(writer, "value", "0");
      ProgramDevicePage.WriteAttribute(writer, "max", "100");
      writer.WriteFullEndElement();
      writer.WriteFullEndElement();
      writer.WriteFullEndElement();
      writer.WriteFullEndElement();
      writer.WriteFullEndElement();
      bool flag = this.programmingDataProvider.ProgrammingData.Count<ProgrammingData>() > 1;
      foreach (ProgrammingData programmingData in this.programmingDataProvider.ProgrammingData.ToList<ProgrammingData>())
      {
        writer.WriteStartElement("div");
        ProgramDevicePage.WriteAttribute(writer, "id", programmingData.Channel.Ecu.Name);
        ProgramDevicePage.WriteAttribute(writer, "class", "ecuItemDivHighlight");
        ProgramDevicePage.WriteAttribute(writer, "name", programmingData.Channel.Ecu.Name);
        writer.WriteStartElement("table");
        ProgramDevicePage.WriteAttribute(writer, "width", "100%");
        writer.WriteStartElement("th");
        writer.WriteStartElement("td");
        ProgramDevicePage.WriteAttribute(writer, "valign", "center");
        writer.WriteStartElement("a");
        ProgramDevicePage.WriteAttribute(writer, "onclick", "ExpandCollapse(this);");
        ProgramDevicePage.WriteAttribute(writer, "href", "javascript:void(0)");
        ProgramDevicePage.WriteAttribute(writer, "class", "link_collapse");
        if (!flag)
          writer.WriteEntityRef("ndash");
        else
          writer.WriteString("+");
        writer.WriteFullEndElement();
        writer.WriteFullEndElement();
        writer.WriteStartElement("td");
        ProgramDevicePage.WriteAttribute(writer, "valign", "center");
        ProgramDevicePage.WriteAttribute(writer, "class", "ecu");
        writer.WriteString(ProgramDevicePage.GetChannelString(programmingData.Channel));
        writer.WriteFullEndElement();
        writer.WriteStartElement("td");
        ProgramDevicePage.WriteAttribute(writer, "style", "text-align: right");
        ProgramDevicePage.WriteAttribute(writer, "valign", "center");
        ProgramDevicePage.WriteAttribute(writer, "class", "ecu");
        ProgramDevicePage.WriteAttribute(writer, "id", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_ecuStatus", (object) programmingData.Channel.Ecu.Name));
        writer.WriteFullEndElement();
        writer.WriteStartElement("td");
        ProgramDevicePage.WriteAttribute(writer, "style", "text-align: right");
        ProgramDevicePage.WriteAttribute(writer, "valign", "center");
        writer.WriteStartElement("progress");
        ProgramDevicePage.WriteAttribute(writer, "id", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_ecuProgress", (object) programmingData.Channel.Ecu.Name));
        ProgramDevicePage.WriteAttribute(writer, "class", "determinateProgress");
        ProgramDevicePage.WriteAttribute(writer, "value", "0");
        ProgramDevicePage.WriteAttribute(writer, "max", "100");
        writer.WriteFullEndElement();
        writer.WriteFullEndElement();
        writer.WriteFullEndElement();
        writer.WriteStartElement("tr");
        writer.WriteElementString("td", string.Empty);
        writer.WriteStartElement("td");
        ProgramDevicePage.WriteAttribute(writer, "valign", "center");
        ProgramDevicePage.WriteAttribute(writer, "class", "standard");
        ProgramDevicePage.WriteAttribute(writer, "colspan", "3");
        ProgramDevicePage.WriteAttribute(writer, "id", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_ecuStep", (object) programmingData.Channel.Ecu.Name));
        writer.WriteFullEndElement();
        writer.WriteFullEndElement();
        writer.WriteFullEndElement();
        writer.WriteStartElement("div");
        ProgramDevicePage.WriteAttribute(writer, "class", !flag ? "collapsable_content content_expanded" : "collapsable_content content_collapsed");
        writer.WriteStartElement("table");
        ProgramDevicePage.BuildOperationXml(writer, programmingData);
        ProgramDevicePage.BuildConfigurationXml(writer, programmingData);
        ProgramDevicePage.BuildIdentificationXml(writer, programmingData);
        ProgramDevicePage.BuildHardwareXml(writer, programmingData);
        writer.WriteFullEndElement();
        writer.WriteStartElement("table");
        ProgramDevicePage.WriteAttribute(writer, "id", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_ecuParameterWarningsTable", (object) programmingData.Channel.Ecu.Name));
        writer.WriteFullEndElement();
        writer.WriteFullEndElement();
        writer.WriteFullEndElement();
      }
      writer.WriteFullEndElement();
    }
    return stringBuilder.ToString();
  }

  private static void BuildOperationXml(XmlWriter xmlWriter, ProgrammingData programmingData)
  {
    NameValueCollection nameValueCollection = programmingData.NameValueCollection;
    ProgramDevicePage.BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_OperationInformation, false);
    string name = nameValueCollection[Resources.ProgrammingDataItem_AutomaticOperation] != null ? Resources.ProgrammingDataItem_AutomaticOperation : Resources.ProgrammingDataItem_Operation;
    ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "operationType", name, nameValueCollection[name], string.Empty);
    if (nameValueCollection[Resources.ProgrammingDataItem_Settings] == null)
      return;
    ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "settingsType", Resources.ProgrammingDataItem_Settings, nameValueCollection[Resources.ProgrammingDataItem_Settings], string.Empty);
  }

  private static string FormatPartNumberAndDescription(
    EcuInfo partNumberEcuInfo,
    EcuInfo descriptionEcuInfo)
  {
    return ProgramDevicePage.FormatPartNumberAndDescription(partNumberEcuInfo == null || partNumberEcuInfo.Value == null ? (Part) null : new Part(partNumberEcuInfo.Value), descriptionEcuInfo?.Value);
  }

  private static string FormatPartNumberAndDescription(Part partNumber, string description)
  {
    string str = partNumber != null ? partNumber.ToString() : string.Empty;
    if (!string.IsNullOrEmpty(description))
      str = $"{str} ({description})";
    return str;
  }

  private static IEnumerable<Tuple<int, EcuInfo, EcuInfo>> GetFuelmapEcuInfos(Channel channel)
  {
    EcuInfo[] fuelMapPartNumbers = channel.EcuInfos.Where<EcuInfo>((Func<EcuInfo, bool>) (ei => ei.Qualifier.StartsWith("CO_FuelmapPartNumber", StringComparison.OrdinalIgnoreCase))).ToArray<EcuInfo>();
    EcuInfo[] fuelMapDescriptions = channel.EcuInfos.Where<EcuInfo>((Func<EcuInfo, bool>) (ei => ei.Qualifier.StartsWith("CO_FuelmapDescription", StringComparison.OrdinalIgnoreCase))).ToArray<EcuInfo>();
    for (int i = 0; i < fuelMapPartNumbers.Length; ++i)
      yield return Tuple.Create<int, EcuInfo, EcuInfo>(i, fuelMapPartNumbers[i], i < fuelMapDescriptions.Length ? fuelMapDescriptions[i] : (EcuInfo) null);
  }

  private static void BuildConfigurationXml(XmlWriter xmlWriter, ProgrammingData programmingData)
  {
    List<FlashMeaning> list1 = SapiManager.GlobalInstance.Sapi.FlashFiles.Where<FlashFile>((Func<FlashFile, bool>) (ff => ff.Ecus.Contains(programmingData.Channel.Ecu.Name))).SelectMany<FlashFile, FlashArea>((Func<FlashFile, IEnumerable<FlashArea>>) (ff => ff.FlashAreas)).SelectMany<FlashArea, FlashMeaning>((Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).ToList<FlashMeaning>();
    Part targetBootLoaderPart = programmingData.EdexFileInformation?.ConfigurationInformation?.BootLoaderPartNumber != null ? programmingData.EdexFileInformation.ConfigurationInformation.BootLoaderPartNumber : (programmingData.Bootcode != null ? new Part(programmingData.Bootcode.Key) : (Part) null);
    if (targetBootLoaderPart == null && programmingData.Firmware == null && !programmingData.HasDataSet && !programmingData.HasControlList)
      return;
    ProgramDevicePage.BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_Configuration, true);
    if (targetBootLoaderPart != null || programmingData.Bootcode != null)
    {
      EcuInfo ecuInfo = programmingData.Channel.EcuInfos["CO_BootSoftwarePartNumber"];
      FlashMeaning flashMeaning = list1.FirstOrDefault<FlashMeaning>((Func<FlashMeaning, bool>) (m => PartExtensions.IsEqual(targetBootLoaderPart, m.FlashKey)));
      ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "currentBootSoftware", Resources.ProgrammingDataItem_BootSoftware, ecuInfo == null || ecuInfo.Value == null ? string.Empty : new Part(ecuInfo.Value).ToString(), ProgramDevicePage.FormatPartNumberAndDescription(targetBootLoaderPart, flashMeaning?.Name), PartExtensions.IsEqual(targetBootLoaderPart, ecuInfo?.Value) ? "standard" : "info");
    }
    if (programmingData.Firmware != null)
    {
      Part partNumber = new Part(programmingData.Firmware.Key);
      if (SapiManager.ProgramDeviceUsesSoftwareIdentification(programmingData.Channel.Ecu))
      {
        string softwareIdentification1 = SapiManager.GetSoftwareIdentification(programmingData.Channel);
        string softwareIdentification2 = programmingData.EdexFileInformation.ConfigurationInformation.SoftwareIdentification;
        ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "currentSoftware", Resources.ProgrammingDataItem_Software, softwareIdentification1, ProgramDevicePage.FormatPartNumberAndDescription(partNumber, softwareIdentification2), softwareIdentification1.Equals(softwareIdentification2) ? "standard" : "info");
      }
      else
      {
        EcuInfo ecuInfo = programmingData.Channel.EcuInfos["CO_SoftwarePartNumber"];
        ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "currentSoftware", Resources.ProgrammingDataItem_Software, ProgramDevicePage.FormatPartNumberAndDescription(ecuInfo, programmingData.Channel.EcuInfos["CO_SoftwareVersion"]), ProgramDevicePage.FormatPartNumberAndDescription(partNumber, programmingData.Firmware.Version), PartExtensions.IsEqual(partNumber, ecuInfo?.Value) ? "standard" : "info");
      }
    }
    if (programmingData.HasDataSet)
    {
      List<Tuple<int, EcuInfo, EcuInfo>> list2 = ProgramDevicePage.GetFuelmapEcuInfos(programmingData.Channel).ToList<Tuple<int, EcuInfo, EcuInfo>>();
      Part[] matchedParts = programmingData.GetMatchedDataSetParts(list2.Select<Tuple<int, EcuInfo, EcuInfo>, EcuInfo>((Func<Tuple<int, EcuInfo, EcuInfo>, EcuInfo>) (fi => fi.Item2)).ToArray<EcuInfo>(), (IEnumerable<FlashMeaning>) list1);
      foreach (Tuple<int, EcuInfo, EcuInfo, Part> tuple in (IEnumerable<Tuple<int, EcuInfo, EcuInfo, Part>>) list2.Select<Tuple<int, EcuInfo, EcuInfo>, Tuple<int, EcuInfo, EcuInfo, Part>>((Func<Tuple<int, EcuInfo, EcuInfo>, Tuple<int, EcuInfo, EcuInfo, Part>>) (fi => Tuple.Create<int, EcuInfo, EcuInfo, Part>(fi.Item1, fi.Item2, fi.Item3, matchedParts[fi.Item1]))).OrderByDescending<Tuple<int, EcuInfo, EcuInfo, Part>, bool>((Func<Tuple<int, EcuInfo, EcuInfo, Part>, bool>) (cs => cs.Item4 != null)))
      {
        Tuple<int, EcuInfo, EcuInfo, Part> datasetInfo = tuple;
        FlashMeaning flashMeaning = list1.FirstOrDefault<FlashMeaning>((Func<FlashMeaning, bool>) (fm => PartExtensions.IsEqual(datasetInfo.Item4, fm.FlashKey)));
        ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "currentFuelmap{0}", (object) datasetInfo.Item1), datasetInfo.Item2.Name, ProgramDevicePage.FormatPartNumberAndDescription(datasetInfo.Item2, datasetInfo.Item3), ProgramDevicePage.FormatPartNumberAndDescription(datasetInfo.Item4, flashMeaning?.Name), PartExtensions.IsEqual(datasetInfo.Item4, datasetInfo.Item2?.Value) ? "standard" : "info");
      }
      foreach (Part part in (IEnumerable<Part>) programmingData.DataSetVersions.Except<Part>((IEnumerable<Part>) matchedParts).OrderBy<Part, string>((Func<Part, string>) (p => p.Number)))
      {
        Part targetPart = part;
        FlashMeaning flashMeaning = list1.FirstOrDefault<FlashMeaning>((Func<FlashMeaning, bool>) (fm => PartExtensions.IsEqual(targetPart, fm.FlashKey)));
        ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, string.Empty, Resources.ProgrammingDataItem_Dataset, string.Empty, ProgramDevicePage.FormatPartNumberAndDescription(targetPart, flashMeaning?.Name));
      }
    }
    if (!programmingData.HasControlList)
      return;
    Part targetControlListPartNumber = new Part(programmingData.ControlList.Key);
    EcuInfo ecuInfo1 = programmingData.Channel.EcuInfos["CO_ControlListSoftwarePartNumber"];
    FlashMeaning flashMeaning1 = list1.FirstOrDefault<FlashMeaning>((Func<FlashMeaning, bool>) (m => PartExtensions.IsEqual(targetControlListPartNumber, m.FlashKey)));
    ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "currentControlListSoftware", "Control List Software", ecuInfo1 == null || ecuInfo1.Value == null ? string.Empty : new Part(ecuInfo1.Value).ToString(), ProgramDevicePage.FormatPartNumberAndDescription(targetControlListPartNumber, flashMeaning1?.Name), PartExtensions.IsEqual(targetControlListPartNumber, ecuInfo1?.Value) ? "standard" : "info");
  }

  private static void BuildIdentificationXml(XmlWriter xmlWriter, ProgrammingData programmingData)
  {
    ProgramDevicePage.BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_Identification, true);
    if (!string.IsNullOrEmpty(programmingData.PreviousVehicleIdentificationNumber) || !string.IsNullOrEmpty(programmingData.VehicleIdentificationNumber))
      ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "vin", Resources.ProgrammingDataItem_VehicleIdentificationNumber, programmingData.PreviousVehicleIdentificationNumber, programmingData.VehicleIdentificationNumber, string.IsNullOrEmpty(programmingData.PreviousVehicleIdentificationNumber) || programmingData.PreviousVehicleIdentificationNumber == programmingData.VehicleIdentificationNumber ? "standard" : "warning");
    if (string.IsNullOrEmpty(programmingData.PreviousEngineSerialNumber) && string.IsNullOrEmpty(programmingData.EngineSerialNumber))
      return;
    ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "esn", Resources.ProgrammingDataItem_EngineSerialNumber, programmingData.PreviousEngineSerialNumber, programmingData.EngineSerialNumber, string.IsNullOrEmpty(programmingData.PreviousEngineSerialNumber) || programmingData.PreviousEngineSerialNumber == programmingData.EngineSerialNumber ? "standard" : "warning");
  }

  private static void BuildHardwareXml(XmlWriter xmlWriter, ProgrammingData programmingData)
  {
    string current = programmingData.Channel.EcuInfos["CO_EcuSerialNumber"]?.Value;
    string number = programmingData.Channel.EcuInfos["CO_HardwarePartNumber"]?.Value;
    string hardwareRevision = programmingData.PreviousHardwareRevision;
    if (current == null && number == null && hardwareRevision == null)
      return;
    ProgramDevicePage.BuildXmlTableGroupHeader(xmlWriter, Resources.ProgramDevicePage_Hardware, false);
    if (current != null)
      ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "ecuSerialNumber", Resources.ProgramDevicePage_ECUSerialNumber, current, string.Empty);
    if (number != null)
      ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "hardwarePartNumber", Resources.ProgrammingDataItem_HardwarePartNumber, PartExtensions.ToHardwarePartNumberString(new Part(number), programmingData.Channel.Ecu, true), string.Empty);
    if (string.IsNullOrEmpty(hardwareRevision))
      return;
    ProgramDevicePage.BuildXmlTableRow(xmlWriter, programmingData.Channel.Ecu, "hardwareRevision", Resources.ProgrammingDataItem_HardwareRevision, hardwareRevision, string.Empty);
  }

  private static void BuildXmlTableGroupHeader(
    XmlWriter xmlWriter,
    string groupName,
    bool hasCurrentAndTargetColumns)
  {
    xmlWriter.WriteStartElement("tr");
    xmlWriter.WriteStartElement("td");
    ProgramDevicePage.WriteAttribute(xmlWriter, "class", "group");
    xmlWriter.WriteString(groupName);
    xmlWriter.WriteFullEndElement();
    if (hasCurrentAndTargetColumns)
    {
      xmlWriter.WriteStartElement("td");
      ProgramDevicePage.WriteAttribute(xmlWriter, "class", "group");
      xmlWriter.WriteString("Current");
      xmlWriter.WriteFullEndElement();
      xmlWriter.WriteStartElement("td");
      ProgramDevicePage.WriteAttribute(xmlWriter, "class", "group");
      xmlWriter.WriteString("Target");
      xmlWriter.WriteFullEndElement();
    }
    xmlWriter.WriteFullEndElement();
  }

  private static void BuildXmlTableRow(
    XmlWriter xmlWriter,
    Ecu ecu,
    string id,
    string name,
    string current,
    string target,
    string currentClass = "standard",
    string targetClass = "standard")
  {
    xmlWriter.WriteStartElement("tr");
    xmlWriter.WriteStartElement("td");
    ProgramDevicePage.WriteAttribute(xmlWriter, "class", "standard");
    xmlWriter.WriteString(name);
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteStartElement("td");
    ProgramDevicePage.WriteAttribute(xmlWriter, "class", currentClass);
    if (string.IsNullOrEmpty(target))
      ProgramDevicePage.WriteAttribute(xmlWriter, "colspan", "2");
    ProgramDevicePage.WriteAttribute(xmlWriter, nameof (id), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) ecu.Name, (object) id));
    xmlWriter.WriteString(current);
    xmlWriter.WriteFullEndElement();
    if (!string.IsNullOrEmpty(target))
    {
      xmlWriter.WriteStartElement("td");
      ProgramDevicePage.WriteAttribute(xmlWriter, "class", targetClass);
      xmlWriter.WriteString(target);
      xmlWriter.WriteFullEndElement();
    }
    xmlWriter.WriteFullEndElement();
  }

  private void PopulateParameterWarnings(Ecu ecu, List<string> warnings)
  {
    HtmlElement htmlElementById = this.FindHtmlElementById(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_ecuParameterWarningsTable", (object) ecu.Name));
    if (!(htmlElementById != (HtmlElement) null))
      return;
    StringBuilder stringBuilder = new StringBuilder();
    using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
    {
      writer.WriteElementString("tr", string.Empty);
      ProgramDevicePage.BuildXmlTableGroupHeader(writer, Resources.ProgramDevicePage_ParameterWarnings, false);
      foreach (string warning in warnings)
      {
        writer.WriteStartElement("tr");
        writer.WriteStartElement("td");
        writer.WriteAttributeString("class", "parameter warning");
        writer.WriteString(warning);
        writer.WriteFullEndElement();
        writer.WriteFullEndElement();
      }
    }
    htmlElementById.InnerHtml = stringBuilder.ToString();
  }

  private static string GetChannelString(Channel channel)
  {
    string channelString = channel.Ecu.Name;
    if (!string.IsNullOrEmpty(channel.Ecu.ShortDescription))
      channelString = $"{channelString} - {channel.Ecu.ShortDescription}";
    return channelString;
  }

  internal override WizardPage OnWizardNext() => this.Wizard.Pages[0];

  internal override WizardPage OnWizardBack()
  {
    return this.programmingDataProvider.ProgrammingData.FirstOrDefault<ProgrammingData>() != null && this.programmingDataProvider.ProgrammingData.First<ProgrammingData>().AutomaticOperation != null ? this.Wizard.Pages[0] : base.OnWizardBack();
  }

  internal override void OnSetActive()
  {
    foreach (Precondition programmingPrecondition in this.programmingPreconditions)
      programmingPrecondition.StateChanged += new EventHandler(this.Precondition_ValueStateChanged);
    this.ShowContent();
    this.UpdatePreconditions();
    this.UpdateUi(ProgramDevicePage.PageActionState.ReadyToStart);
  }

  internal override void OnSetInactive()
  {
    foreach (Precondition programmingPrecondition in this.programmingPreconditions)
      programmingPrecondition.StateChanged -= new EventHandler(this.Precondition_ValueStateChanged);
  }

  private void Precondition_ValueStateChanged(object sender, EventArgs e)
  {
    if (this.Wizard.ActivePage != this || this.pageState != ProgramDevicePage.PageActionState.ReadyToStart && this.pageState != ProgramDevicePage.PageActionState.ReadyToRetry)
      return;
    this.UpdatePreconditions();
    this.UpdateUi(this.pageState);
  }

  private IEnumerable<Precondition> FailedPreconditions
  {
    get
    {
      return this.programmingPreconditions.Where<Precondition>((Func<Precondition, bool>) (precondition => precondition.State == 2));
    }
  }

  private void UpdatePreconditions()
  {
    if (!this.FailedPreconditions.Any<Precondition>())
    {
      this.preconditionsPane.InnerHtml = "&nbsp;";
      this.preconditionsPane.SetAttribute("className", "hide");
    }
    else
    {
      string empty = string.Empty;
      foreach (Precondition failedPrecondition in this.FailedPreconditions)
      {
        string str = string.Empty;
        if (failedPrecondition.DiagnosticPanelName != null)
          str = FormattableString.Invariant(FormattableStringFactory.Create("<a href='#precondition_{0}'>{1}</a>", (object) failedPrecondition.PreconditionType, (object) ActionsMenuProxy.GlobalInstance.GetDialogLocalizedDisplayName(failedPrecondition.DiagnosticPanelName, (string) null, false)));
        empty += FormattableString.Invariant(FormattableStringFactory.Create("<div><span class='warninginline'>{0}</span>&nbsp;{1}</div>", (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDevicePage_FormatPreconditionNotMet, (object) failedPrecondition.Text), (object) str));
      }
      this.preconditionsPane.InnerHtml = empty;
      this.preconditionsPane.SetAttribute("className", "show");
    }
  }

  private static void SetUIEnabled(bool enable)
  {
    Form openForm = Application.OpenForms[0];
    if (openForm == null)
      return;
    MenuProxy.GlobalInstance.ContainerApplication.DisableForProgramming(!enable);
    foreach (Control control in (ArrangedElementCollection) openForm.Controls)
    {
      if (control.GetType().Name != "TabbedView")
        control.Enabled = enable;
    }
  }

  internal override void Navigate(string fragment)
  {
    if (fragment == "#button_start")
    {
      if (this.pageState == ProgramDevicePage.PageActionState.ReadyToStart)
        this.StartProgramming();
      else if (this.pageState == ProgramDevicePage.PageActionState.ReadyToRetry)
        this.ProcessNextEcu(true);
    }
    if (!fragment.StartsWith("#precondition_", StringComparison.OrdinalIgnoreCase))
      return;
    string preconditionType = fragment.Split("_".ToCharArray())[1];
    ActionsMenuProxy.GlobalInstance.ShowDialog(this.programmingPreconditions.First<Precondition>((Func<Precondition, bool>) (pc => pc.PreconditionType.ToString() == preconditionType)).DiagnosticPanelName, (string) null, (object) null, false);
  }

  private void StartProgramming()
  {
    if (this.programmingDataProvider.RequiresCompatibilityChecks && !this.CheckCompatibility(this.programmingDataProvider.ProgrammingData.Single<ProgrammingData>()))
      return;
    this.programmingIndex = -1;
    this.UpdateOverallStatus(Resources.ProgramDeviceManager_Step_Starting, Resources.ProgramDevicePage_PleaseWait);
    ((Control) this.Wizard).Invalidate();
    this.UpdateUi(ProgramDevicePage.PageActionState.Busy);
    Application.DoEvents();
    this.DisconnectChannels(true);
    this.ProcessNextEcu(false);
  }

  private void ProcessNextEcu(bool isRetry)
  {
    if (!isRetry)
      ++this.programmingIndex;
    if (this.programmingDataProvider.ProgrammingData.Count<ProgrammingData>() > this.programmingIndex)
    {
      this.UpdateUi(ProgramDevicePage.PageActionState.Busy);
      if (this.programDeviceManager != null)
        this.programDeviceManager.Dispose();
      this.programDeviceManager = new ProgramDeviceManager(this);
      ProgrammingData programmingData = this.programmingDataProvider.ProgrammingData.Skip<ProgrammingData>(this.programmingIndex).First<ProgrammingData>();
      this.startingVariant = new Tuple<string, string>(programmingData.Channel.Ecu.Name, programmingData.Channel.DiagnosisVariant.ToString());
      this.DisconnectChannels(false);
      this.HideOverallProgress(false);
      this.programDeviceManager.Start(programmingData);
      this.UpdateEcuProcessStatus(programmingData.Channel.Ecu, ProgramDevicePage.ProcessStatus.Processing);
      this.UpdateOverallStatus(Resources.ProgramDevicePage_Processing, ProgramDevicePage.GetChannelString(programmingData.Channel));
    }
    else
      this.AllDone();
  }

  internal void Complete(
    ProgrammingData programmingData,
    bool success,
    ProgrammingStep terminalStep,
    string message,
    List<string> parameterErrorsForEcu,
    Dictionary<string, CaesarException> partNumberExceptionsForEcu,
    bool canRetry)
  {
    if (parameterErrorsForEcu.Any<string>())
      this.parameterErrors[programmingData.Channel.Ecu] = parameterErrorsForEcu;
    this.UpdateEcuConfiguration(programmingData);
    if (success)
    {
      this.UpdateEcuProcessStatus(programmingData.Channel.Ecu, ProgramDevicePage.ProcessStatus.Pass);
      this.ProcessNextEcu(false);
    }
    else
    {
      this.UpdateEcuProcessStatus(programmingData.Channel.Ecu, ProgramDevicePage.ProcessStatus.Fail);
      this.UpdateStepText(programmingData.Channel.Ecu, message, terminalStep, true);
      if (canRetry)
      {
        this.UpdatePreconditions();
        this.UpdateUi(ProgramDevicePage.PageActionState.ReadyToRetry);
        this.UpdateOverallStatus(Resources.ProgramDevicePage_Error, Resources.ProgramDevicePageOutputInfoLabel_TheProgrammingOperationFailedToRetryClickTheStartButton);
      }
      else
      {
        this.UpdateUi(ProgramDevicePage.PageActionState.FinishedFailed);
        this.UpdateOverallStatus(Resources.ProgramDevicePage_Error, Resources.ProgramDevicePageOutputInfoLabel_ProgrammingFailedUnableToContinue);
      }
      if (partNumberExceptionsForEcu == null || !partNumberExceptionsForEcu.Any<KeyValuePair<string, CaesarException>>())
        return;
      Application.DoEvents();
      int num = (int) ((Form) new PartNumberErrorDialog(programmingData.Channel, partNumberExceptionsForEcu)).ShowDialog();
    }
  }

  private void DisconnectChannels(bool saveConnectedList)
  {
    ChannelCollection channels = SapiManager.GlobalInstance.Sapi.Channels;
    channels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.channels_ConnectCompleteEvent);
    if (saveConnectedList)
    {
      this.devicesToClearFaultsFor.Clear();
      this.manualConnections.Clear();
    }
    SapiManager.GlobalInstance.SuspendAutoConnect();
    int num;
    for (int index = 0; index < channels.Count; index = num + 1)
    {
      Channel channel = channels[index];
      channel.Disconnect();
      num = index - 1;
      if (saveConnectedList)
      {
        this.devicesToClearFaultsFor.Add(channel.Ecu);
        if (!channel.Ecu.IsRollCall && !channel.Ecu.MarkedForAutoConnect)
          this.manualConnections.Add(channel.ConnectionResource);
      }
    }
  }

  private void ReconnectChannels()
  {
    SapiManager.GlobalInstance.Sapi.Channels.ConnectCompleteEvent += new ConnectCompleteEventHandler(this.channels_ConnectCompleteEvent);
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
      this.ResetFaultsIfRequired(channel);
    SapiManager.GlobalInstance.ResumeAutoConnect();
    foreach (ConnectionResource manualConnection in this.manualConnections)
      SapiManager.GlobalInstance.Sapi.Channels.Connect(manualConnection, false);
    this.manualConnections.Clear();
  }

  private void ResetFaultsIfRequired(Channel channel)
  {
    if (!this.devicesToClearFaultsFor.Contains(channel.Ecu) || !channel.Online)
      return;
    channel.FaultCodes.Reset(false);
    this.devicesToClearFaultsFor.Remove(channel.Ecu);
  }

  private void channels_ConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!e.Succeeded || !(sender is Channel channel))
      return;
    this.ResetFaultsIfRequired(channel);
  }

  private void AllDone()
  {
    this.HideOverallProgress(true);
    if (this.parameterErrors.Any<KeyValuePair<Ecu, List<string>>>())
    {
      this.UpdateOverallStatus(Resources.ProgramDevicePage_Success, Resources.ProgramDevicePageOutputInfoLabel_TheDeviceWasSuccessfullyProgrammedButMayHaveConfigurationErrors);
      foreach (Ecu key in this.parameterErrors.Keys)
      {
        Ecu ecu = key;
        if (this.programmingDataProvider.ProgrammingData.FirstOrDefault<ProgrammingData>((Func<ProgrammingData, bool>) (d => d.Channel.Ecu == ecu)) != null)
          this.PopulateParameterWarnings(ecu, this.parameterErrors[ecu]);
      }
    }
    else
      this.UpdateOverallStatus(Resources.ProgramDevicePage_Success, Resources.ProgramDevicePageOutputInfoLabel_TheDeviceWasSuccessfullyProgrammed);
    this.ReconnectChannels();
    if (this.informUserOfOfflineDepotCompatibility)
      ProgramDevicePage.ShowOfflineCompatibilityForm(this.offlineSoftwareTarget, this.offlineCompatibilitySoftwareCollection);
    if (this.informUserOfOfflineEdexCompatibility)
      ProgramDevicePage.ShowOfflineCompatibilityForm(this.offlineCompatibilityTarget, this.offlineCompatiblityConfigurationCollection);
    List<KeyValuePair<Ecu, string>> list1 = this.programmingDataProvider.ProgrammingData.First<ProgrammingData>().Unit.DeviceInformation.Select(deviceInfo => new
    {
      deviceInfo = deviceInfo,
      ecu = SapiManager.GetEcuByName(deviceInfo.Device)
    }).Where(_param1 => _param1.ecu != null && _param1.ecu.Properties.ContainsKey("PromptForCheckAfterOtherEcuVariantChange")).Select(_param1 => new
    {
      \u003C\u003Eh__TransparentIdentifier0 = _param1,
      feature = _param1.ecu.Properties["PromptForCheckAfterOtherEcuVariantChange"]
    }).Select(_param1 => new KeyValuePair<Ecu, string>(_param1.\u003C\u003Eh__TransparentIdentifier0.ecu, PanelBase.GetLocalizedDisplayName(_param1.feature) ?? _param1.feature)).ToList<KeyValuePair<Ecu, string>>();
    Channel channel = this.programmingDataProvider.ProgrammingData.FirstOrDefault<ProgrammingData>((Func<ProgrammingData, bool>) (c => c.Channel.Ecu.Name == this.startingVariant.Item1))?.Channel;
    if (channel != null && channel.DiagnosisVariant.ToString() != this.startingVariant.Item2)
    {
      List<KeyValuePair<Ecu, string>> list2 = list1.Where<KeyValuePair<Ecu, string>>((Func<KeyValuePair<Ecu, string>, bool>) (d => this.programmingDataProvider.ProgrammingData.All<ProgrammingData>((Func<ProgrammingData, bool>) (x => x.Channel.Ecu != d.Key)))).ToList<KeyValuePair<Ecu, string>>();
      if (list2.Any<KeyValuePair<Ecu, string>>())
      {
        int num = (int) ControlHelpers.ShowMessageBox(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDevicePage_CheckDependentDeviceFeaturesAfterProgramming, (object) string.Join(Environment.NewLine, list2.Select<KeyValuePair<Ecu, string>, string>((Func<KeyValuePair<Ecu, string>, string>) (e => $"{e.Key.DisplayName}: {e.Value}")))), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
      }
    }
    if (NetworkSettings.GlobalInstance.SaveUploadContent && ApplicationInformation.MustConnectAfterTroubleshootingOrReprogramming)
    {
      Collection<UnitInformation> collection = new Collection<UnitInformation>();
      ServerDataManager.GlobalInstance.GetUploadUnits(collection, (ServerDataManager.UploadType) 0);
      ServerClient.GlobalInstance.Upload(collection, Application.OpenForms.OfType<IProgressBar>().First<IProgressBar>());
    }
    if (this.pageState != ProgramDevicePage.PageActionState.Busy)
      return;
    this.UpdateUi(this.programmingDataProvider.ProgrammingData.FirstOrDefault<ProgrammingData>() != null && this.programmingDataProvider.ProgrammingData.First<ProgrammingData>().AutomaticOperation != null ? ProgramDevicePage.PageActionState.FinishedAutomaticOperation : ProgramDevicePage.PageActionState.Finished);
  }

  private static void ShowOfflineCompatibilityForm(
    Software targetSoftware,
    CompatibleSoftwareCollection compatibleSoftwareCollection)
  {
    if (targetSoftware == null || compatibleSoftwareCollection == null || ((ReadOnlyCollection<SoftwareCollection>) compatibleSoftwareCollection).Count <= 0)
      return;
    CompatibilityWarningDialog compatibilityWarningDialog = new CompatibilityWarningDialog(Resources.ProgramDevicePage_OfflineDeviceCompatibility_Information, targetSoftware, Resources.ProgramDevicePage_Compat_TargetDevice);
    compatibilityWarningDialog.AddCompatibilityInfo(compatibleSoftwareCollection, Resources.ProgramDevicePage_Compat_CompatibleSetFormat);
    int num = (int) ((Form) compatibilityWarningDialog).ShowDialog();
  }

  private static void ShowOfflineCompatibilityForm(
    EdexCompatibilityEcuItem targetEcuItem,
    EdexCompatibilityConfigurationCollection compatibilityConfigurationCollection)
  {
    if (targetEcuItem == null || compatibilityConfigurationCollection == null || ((ReadOnlyCollection<EdexCompatibilityConfiguration>) compatibilityConfigurationCollection).Count <= 0)
      return;
    CompatibilityWarningDialog compatibilityWarningDialog = new CompatibilityWarningDialog(Resources.ProgramDevicePage_OfflineDeviceCompatibility_Information, targetEcuItem, Resources.ProgramDevicePage_Compat_TargetDevice);
    compatibilityWarningDialog.AddCompatibilityInfo(compatibilityConfigurationCollection, Resources.ProgramDevicePage_Compat_CompatibleSetFormat);
    int num = (int) ((Form) compatibilityWarningDialog).ShowDialog();
  }

  private bool CheckCompatibility(ProgrammingData data)
  {
    DeviceInformation.DeviceDataSource dataSource = data.DataSource;
    if (dataSource != 1)
    {
      if (dataSource == 2 && !data.Unit.InAutomaticOperation && !SapiManager.GlobalInstance.IgnoreSoftwareCompatibilityChecks(data.Channel) && data.EdexFileInformation?.ConfigurationInformation != null)
      {
        EdexConfigurationInformation configurationInformation = data.EdexFileInformation.ConfigurationInformation;
        this.CheckOfflineCompatibility(new EdexCompatibilityEcuItem(configurationInformation.DeviceName, configurationInformation.HardwarePartNumber, configurationInformation.FlashwarePartNumber), data.Unit);
      }
    }
    else if (data.Firmware != null)
    {
      FirmwareInformation firmware = data.Firmware;
      Software targetSoftware = new Software(firmware.Device, firmware.Version, SapiManager.GetHardwarePartNumber(data.Channel));
      if (!ServerDataManager.GlobalInstance.CompatibilityTable.IsHardwareCompatibleWithSoftware(targetSoftware))
      {
        IList<Part> compatibleHardwareList = (IList<Part>) ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleHardwareList(targetSoftware);
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProgramDevicePage_FormatIncompatibleHardware, (object) firmware.Device);
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
        foreach (Part part in (IEnumerable<Part>) compatibleHardwareList)
          stringBuilder.AppendLine(PartExtensions.ToHardwarePartNumberString(part, firmware.Device, true));
        int num = (int) CustomMessageBox.Show((IWin32Window) null, stringBuilder.ToString(), Resources.ProgramDevicePage_IncompatibleHardwareDialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, (CustomMessageBoxOptions) 3);
        this.UpdateOverallStatus(Resources.ProgramDevicePage_Failed, Resources.ProgramDevicePageOutputInfoLabel_ProgrammingFailedUnableToContinue);
        this.UpdateUi(ProgramDevicePage.PageActionState.Error);
        return false;
      }
      if (!data.Unit.InAutomaticOperation && !data.Unit.UnitFixedAtTest && !SapiManager.GlobalInstance.IgnoreSoftwareCompatibilityChecks(data.Channel))
        this.CheckOfflineCompatibility(targetSoftware, data.Unit);
    }
    return true;
  }

  private void CheckOfflineCompatibility(Software targetSoftware, UnitInformation unit)
  {
    this.offlineSoftwareTarget = targetSoftware;
    this.offlineCompatibilitySoftwareCollection = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleList(targetSoftware).FilterForUnit(unit).FilterForOfflineDevices();
    this.informUserOfOfflineDepotCompatibility = ((ReadOnlyCollection<SoftwareCollection>) this.offlineCompatibilitySoftwareCollection).Count > 0;
  }

  private void CheckOfflineCompatibility(
    EdexCompatibilityEcuItem targetEcuItem,
    UnitInformation unit)
  {
    this.offlineCompatibilityTarget = targetEcuItem;
    this.offlineCompatiblityConfigurationCollection = ServerDataManager.GlobalInstance.EdexCompatibilityTable.CreateCompatibleList(targetEcuItem).FilterForUnit(unit).FilterForOfflineDevices();
    this.informUserOfOfflineEdexCompatibility = ((ReadOnlyCollection<EdexCompatibilityConfiguration>) this.offlineCompatiblityConfigurationCollection).Count > 0;
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    int num = disposing ? 1 : 0;
    if (this.programDeviceManager != null)
    {
      this.programDeviceManager.Dispose();
      this.programDeviceManager = (ProgramDeviceManager) null;
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private enum PageActionState
  {
    ReadyToStart,
    Busy,
    ReadyToRetry,
    Finished,
    FinishedAutomaticOperation,
    FinishedFailed,
    Error,
    Unknown,
  }

  internal enum ProcessStatus
  {
    Pass,
    Fail,
    Processing,
  }
}
