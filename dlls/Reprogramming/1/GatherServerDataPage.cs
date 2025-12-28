// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.GatherServerDataPage
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
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

#nullable disable
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

  internal IEnumerable<UnitInformation> UploadUnits
  {
    get => (IEnumerable<UnitInformation>) this.uploadUnits.ToList<UnitInformation>();
  }

  internal IEnumerable<UnitInformation> PendingRequests
  {
    get => (IEnumerable<UnitInformation>) this.newPendingRequests.Keys.ToList<UnitInformation>();
  }

  public GatherServerDataPage(ReprogrammingView dataProvider, HtmlElement inputPane)
    : base(dataProvider, inputPane)
  {
    ServerDataManager.GlobalInstance.DataUpdated += new EventHandler(this.ServerDataManager_DataUpdated);
    ServerClient.GlobalInstance.InUseChanged += new EventHandler(this.ServerClient_InUseChanged);
    ServerClient.GlobalInstance.DownloadUnzipSuccess += new EventHandler(this.ServerClient_DownloadUnzipSuccess);
    LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadStatusChanged += new EventHandler(this.GlobalInstance_RemoteFileDownloadStatusChanged);
    this.SetInputPane(inputPane);
  }

  private void GlobalInstance_RemoteFileDownloadStatusChanged(object sender, EventArgs e)
  {
    if (this.Wizard.ActivePage == this)
      this.BuildUnitList();
    else
      this.tabsBuilt.Remove("unit");
  }

  internal void SetInputPane(HtmlElement inputPane)
  {
    inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("\r\n                <div>\r\n                    <div class='tab'>\r\n                        <button class='tablinks active' onClick=\"clickTab(event,'unit')\">{0}</button>\r\n                        <button class='tablinks' onClick=\"clickTab(event,'software')\">{1}</button>\r\n                        <button class='tablinks' onClick=\"clickTab(event,'dataset')\">{2}</button>\r\n                        <button class='tablinks' onClick=\"clickTab(event,'diagnosticdescription')\">{3}</button>\r\n                        <button id='switchToConnectedUnitButton' class='tablinks exit hide' onClick=\"clickTab(event,'connectedunit', true)\">&#x279c; {4}</button>\r\n                    </div>\r\n                    <div>\r\n                        <div id='unitListPane'>&nbsp;</div>\r\n                        <div id='unitListbuttonPanel' class='fixedbottom show'>&nbsp;</div>\r\n                    </div>\r\n                    <div>\r\n                        <div id='softwareListPane' class='hide'>&nbsp;</div>\r\n                        <div id='softwareListbuttonPanel' class='fixedbottom hide'>&nbsp;</div>\r\n                    </div>\r\n                    <div id='datasetListPane' class='hide'>&nbsp;</div>\r\n                    <div id='diagnosticDescriptionListPane' class='hide'>&nbsp;</div>\r\n                </div>\r\n            ", (object) Resources.GatherServerDataPage_TabUnit, (object) Resources.GatherServerDataPage_TabSoftware, (object) Resources.GatherServerDataPage_TabDataset, (object) Resources.GatherServerDataPage_TabDiagnosticDescriptions, (object) Resources.GatherServerDataPage_TabConnectedUnit));
    foreach (HtmlElement htmlElement in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList<HtmlElement>())
    {
      switch (htmlElement.Id)
      {
        case "unitListPane":
          this.unitListPane = htmlElement;
          continue;
        case "softwareListPane":
          this.softwareListPane = htmlElement;
          continue;
        case "datasetListPane":
          this.datasetListPane = htmlElement;
          continue;
        case "diagnosticDescriptionListPane":
          this.diagnosticDescriptionListPane = htmlElement;
          continue;
        case "unitListbuttonPanel":
          this.unitListbuttonPanel = htmlElement;
          continue;
        case "softwareListbuttonPanel":
          this.softwareListButtonPanel = htmlElement;
          continue;
        default:
          continue;
      }
    }
  }

  private HtmlElement GetButton(string key)
  {
    return this.unitListPane.GetElementsByTagName("button").OfType<HtmlElement>().FirstOrDefault<HtmlElement>((Func<HtmlElement, bool>) (b => b.Id == key));
  }

  internal override void Navigate(string fragment)
  {
    string[] source = fragment.Split("_".ToCharArray());
    switch (source[0])
    {
      case "#button":
        string s = source[1];
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(s))
        {
          case 993596020:
            if (!(s == "add"))
              return;
            this.AddButtonClick();
            return;
          case 2671260646:
            if (!(s == "item"))
              return;
            if (this.selectedItem != null)
            {
              HtmlElement button = this.GetButton(this.selectedItem);
              if (button != (HtmlElement) null)
                button.SetAttribute("className", "unitButton");
            }
            this.selectedItem = string.Join("_", ((IEnumerable<string>) source).Skip<string>(2));
            HtmlElement button1 = this.GetButton(this.selectedItem);
            if (!(button1 != (HtmlElement) null))
              return;
            button1.SetAttribute("className", "unitButton active");
            return;
          case 2866859257:
            if (!(s == "connect"))
              return;
            this.ConnectButtonClick();
            return;
          case 3137510316:
            if (!(s == "removeAll"))
              return;
            this.RemoveAllButtonClick();
            return;
          case 3572655668:
            if (!(s == "refresh"))
              return;
            if (source.Length > 3)
            {
              this.RefreshButtonClick(source[3], source.Length == 5 ? source[4] : (string) null);
              return;
            }
            this.RefreshButtonClick((string) null, (string) null);
            return;
          case 3623273999:
            if (!(s == "refreshAll"))
              return;
            this.RefreshAllButtonClick();
            return;
          case 3657934577:
            if (!(s == "refreshSoftware"))
              return;
            this.RefreshSoftwareButtonClick();
            return;
          case 3683784189:
            if (!(s == "remove"))
              return;
            if (source.Length > 3)
            {
              this.RemoveButtonClick(source[2], source[3], source.Length == 5 ? source[4] : (string) null);
              return;
            }
            this.RemoveButtonClick(source[2], (string) null, (string) null);
            return;
          case 3685020920:
            if (!(s == "view"))
              return;
            this.ViewPartNumbersButtonClick(source[3], source.Length == 5 ? source[4] : (string) null);
            return;
          default:
            return;
        }
      case "#tab":
        this.TabControlSelectedIndexChanged(source[1]);
        break;
    }
  }

  private void ServerClient_InUseChanged(object sender, EventArgs e)
  {
    ReprogrammingView.SetButtonState(this.unitListbuttonPanel, "connectButton", !ServerClient.GlobalInstance.InUse, true);
    if (!ServerClient.GlobalInstance.InUse)
      this.BuildLists();
    this.Wizard.Busy = ServerClient.GlobalInstance.InUse;
    Application.DoEvents();
  }

  private void ServerDataManager_DataUpdated(object sender, EventArgs e) => this.BuildLists();

  private void ServerClient_DownloadUnzipSuccess(object sender, EventArgs e)
  {
    foreach (KeyValuePair<UnitInformation, DateTime> keyValuePair in this.newPendingRequests.ToList<KeyValuePair<UnitInformation, DateTime>>())
    {
      KeyValuePair<UnitInformation, DateTime> pendingRequest = keyValuePair;
      UnitInformation unitInformation = ServerDataManager.GlobalInstance.UnitInformation.FirstOrDefault<UnitInformation>((Func<UnitInformation, bool>) (u => u.IsSameIdentification(pendingRequest.Key.EngineNumber, pendingRequest.Key.VehicleIdentity)));
      if (unitInformation != null && Sapi.TimeFromString(unitInformation.Time) > pendingRequest.Value)
        this.newPendingRequests.Remove(pendingRequest.Key);
    }
    this.BuildLists();
  }

  internal override void OnSetActive() => this.BuildUnitList();

  private void ViewPartNumbersButtonClick(string vehicleIdentity, string engineNumber)
  {
    UnitInformation uploadUnit = this.uploadUnits.FirstOrDefault<UnitInformation>((Func<UnitInformation, bool>) (ui => ui.IsSameIdentification(engineNumber, vehicleIdentity)));
    UnitInformation downloadUnit = ServerDataManager.GlobalInstance.UnitInformation.FirstOrDefault<UnitInformation>((Func<UnitInformation, bool>) (ui => ui.IsSameIdentification(engineNumber, vehicleIdentity)));
    if (uploadUnit != null || downloadUnit != null)
    {
      int num1 = (int) ((Form) new UnitPartNumberViewDialog(uploadUnit, downloadUnit)).ShowDialog();
    }
    else
    {
      int num2 = (int) ControlHelpers.ShowMessageBox(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GatherServerDataPage_UnitNotFound, (object) engineNumber, (object) vehicleIdentity), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }
  }

  private void RemoveButtonClick(string location, string vehicleIdentity, string engineNumber)
  {
    if (!(location == "pending") && ControlHelpers.ShowMessageBox(Resources.GatherServerDataPage_Message_UnitWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
      return;
    IEnumerable<UnitInformation> source = (IEnumerable<UnitInformation>) null;
    switch (location)
    {
      case "download":
        source = (IEnumerable<UnitInformation>) ServerDataManager.GlobalInstance.UnitInformation;
        break;
      case "upload":
        source = (IEnumerable<UnitInformation>) this.uploadUnits;
        break;
      case "pending":
        source = (IEnumerable<UnitInformation>) this.newPendingRequests.Keys;
        break;
    }
    if (vehicleIdentity == null && engineNumber == null)
    {
      foreach (UnitInformation request in source.ToList<UnitInformation>())
        this.RemoveServerItem(request);
    }
    else
    {
      UnitInformation request = source.FirstOrDefault<UnitInformation>((Func<UnitInformation, bool>) (ui => ui.IsSameIdentification(engineNumber, vehicleIdentity)));
      if (request != null)
      {
        this.RemoveServerItem(request);
      }
      else
      {
        int num = (int) ControlHelpers.ShowMessageBox(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GatherServerDataPage_UnitNotFound, (object) engineNumber, (object) vehicleIdentity), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      }
    }
    ServerDataManager.GlobalInstance.SaveUnitXml();
  }

  private void RemoveAllButtonClick()
  {
    if (ControlHelpers.ShowMessageBox(Resources.GatherServerDataPage_Message_AllWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
      return;
    foreach (UnitInformation request in ServerDataManager.GlobalInstance.UnitInformation.Union<UnitInformation>((IEnumerable<UnitInformation>) this.newPendingRequests.Keys).ToList<UnitInformation>())
      this.RemoveServerItem(request);
    ServerDataManager.GlobalInstance.SaveUnitXml();
  }

  internal void RemoveServerItem(UnitInformation request)
  {
    if (request.Status == 1)
      ServerDataManager.DeleteSettingsFilesForUnit(Directories.DrumrollUploadData, request);
    else if (this.newPendingRequests.ContainsKey(request))
    {
      this.newPendingRequests.Remove(request);
    }
    else
    {
      if (!ServerDataManager.GlobalInstance.UnitInformation.Contains(request))
        return;
      ServerDataManager.GlobalInstance.UnitInformation.Remove(request);
      ServerDataManager.DeleteSettingsFilesForUnit(Directories.DrumrollDownloadData, request);
      ServerDataManager.GlobalInstance.RemoveUnreferencedPendingFirmwareDownloads();
    }
  }

  private void CreateRequestPendingItem(UnitInformation request)
  {
    if (!ServerDataManager.GlobalInstance.UnitInformation.Contains(request) || this.newPendingRequests.ContainsKey(request))
      return;
    this.AddToRequestPendingGroup(request);
  }

  internal void RefreshList(IEnumerable<UnitInformation> requestItems)
  {
    foreach (UnitInformation requestItem in requestItems)
      this.CreateRequestPendingItem(requestItem);
    this.BuildUnitList();
  }

  private void RefreshButtonClick(string vehicleIdentity, string engineNumber)
  {
    if (vehicleIdentity == null && engineNumber == null)
    {
      this.RefreshList((IEnumerable<UnitInformation>) ServerDataManager.GlobalInstance.UnitInformation);
    }
    else
    {
      this.CreateRequestPendingItem(ServerDataManager.GlobalInstance.UnitInformation.FirstOrDefault<UnitInformation>((Func<UnitInformation, bool>) (ui => ui.IsSameIdentification(engineNumber, vehicleIdentity))));
      this.BuildUnitList();
    }
  }

  private void RefreshAllButtonClick()
  {
    this.RefreshList((IEnumerable<UnitInformation>) ServerDataManager.GlobalInstance.UnitInformation);
  }

  private void RefreshSoftwareButtonClick()
  {
    if (ControlHelpers.ShowMessageBox(Resources.GatherServerDataPage_MessageRefreshSoftware, MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
      return;
    ServerDataManager.GlobalInstance.FirmwareInformation.Clear();
    this.BuildFirmwareList();
    this.RefreshList((IEnumerable<UnitInformation>) ServerDataManager.GlobalInstance.UnitInformation);
  }

  private void AddButtonClick()
  {
    AddUnitDialog addUnitDialog = new AddUnitDialog();
    if (addUnitDialog.ShowDialog() == DialogResult.OK)
      this.AddToRequestPendingGroup(new UnitInformation(addUnitDialog.EngineSerialNumber, addUnitDialog.VehicleIdNumber, addUnitDialog.EcuHardwarePartNumbers, addUnitDialog.EcuSoftwareIdentificationItems, (UnitInformation.UnitStatus) 0));
    this.BuildUnitList();
  }

  internal void AddToRequestPendingGroup(UnitInformation request)
  {
    this.newPendingRequests.Add(request, DateTime.Now);
  }

  private void ConnectButtonClick()
  {
    GatherServerDataPage.Connect(CollectionExtensions.DistinctBy<UnitInformation, string>((IEnumerable<UnitInformation>) this.newPendingRequests.Keys, (Func<UnitInformation, string>) (pr => pr.IdentityKey)), this.uploadUnits.Where<UnitInformation>((Func<UnitInformation, bool>) (uu => !this.newPendingRequests.Keys.Any<UnitInformation>((Func<UnitInformation, bool>) (pr => pr.IsSameIdentification(uu.EngineNumber, uu.VehicleIdentity))))));
  }

  internal static void Connect(
    IEnumerable<UnitInformation> requestUnits,
    IEnumerable<UnitInformation> settingsUploadUnits)
  {
    ServerClient.GlobalInstance.Go(new Collection<UnitInformation>((IList<UnitInformation>) requestUnits.ToList<UnitInformation>()), new Collection<UnitInformation>((IList<UnitInformation>) settingsUploadUnits.ToList<UnitInformation>()));
  }

  public void BuildLists()
  {
    this.tabsBuilt.Clear();
    this.BuildUnitList();
    switch (this.selectedTab)
    {
      case "software":
        this.BuildFirmwareList();
        break;
      case "dataset":
        this.BuildDataSetList();
        break;
      case "diagnosticdescription":
        this.BuildCBFList();
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
      flag = this.WriteFirmwareList(writer);
    this.softwareListPane.InnerHtml = stringBuilder.ToString() + "<br/><br/><br/><br/>";
    this.softwareListButtonPanel.InnerHtml = flag ? ReprogrammingView.WriteButton("refreshSoftware", Resources.Wizard_ButtonRefreshAllSoftware) : "&nbsp;";
  }

  private bool WriteFirmwareList(XmlWriter writer)
  {
    IOrderedEnumerable<IGrouping<string, FirmwareInformation>> source1 = ServerDataManager.GlobalInstance.FirmwareInformation.OfType<FirmwareInformation>().GroupBy<FirmwareInformation, string>((Func<FirmwareInformation, string>) (fi => fi.Device)).OrderBy<IGrouping<string, FirmwareInformation>, int?>((Func<IGrouping<string, FirmwareInformation>, int?>) (g => GatherServerDataPage.GetEcuPriority(g.Key)));
    if (source1.Any<IGrouping<string, FirmwareInformation>>())
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
      foreach (IGrouping<string, FirmwareInformation> source2 in (IEnumerable<IGrouping<string, FirmwareInformation>>) source1)
      {
        bool flag = true;
        foreach (FirmwareInformation firmwareInformation in (IEnumerable<FirmwareInformation>) source2)
        {
          writer.WriteStartElement("tr");
          if (flag)
          {
            WebBrowserList.WriteGroupCell(writer, firmwareInformation.Device, source2.Count<FirmwareInformation>());
            flag = false;
          }
          writer.WriteElementString("td", firmwareInformation.Key);
          writer.WriteElementString("td", firmwareInformation.Version);
          Ecu ecuByName = SapiManager.GetEcuByName(firmwareInformation.Device);
          GatherServerDataPage.AddFlashwareInformation(writer, firmwareInformation.FileName, firmwareInformation.Key, firmwareInformation.Description, ecuByName != null && SapiExtensions.FlashingRequiresMvci(ecuByName));
          if (!string.IsNullOrEmpty(firmwareInformation.Reference))
          {
            writer.WriteStartElement("td");
            writer.WriteStartElement("div");
            if (firmwareInformation.Status != "OK" || firmwareInformation.RequiresDownload)
              WebBrowserList.WriteWarningTriangle(writer);
            if (firmwareInformation.RequiresDownload)
              writer.WriteString(Resources.GatherServerDataPage_RemoteFileDownloadPending);
            else
              writer.WriteString(firmwareInformation.Status);
            writer.WriteEndElement();
          }
          else
            writer.WriteElementString("td", "n/a");
          writer.WriteEndElement();
        }
      }
      writer.WriteEndElement();
    }
    else
      writer.WriteElementString("p", Resources.GatherServerDataPage_NoInformation);
    this.tabsBuilt.Add("software");
    return source1.Any<IGrouping<string, FirmwareInformation>>();
  }

  private static void AddFlashwareInformation(
    XmlWriter writer,
    string fileName,
    string key,
    string description,
    bool flashingRequiresMvci)
  {
    FlashFile flashFile = Sapi.GetSapi().FlashFiles.FirstOrDefault<FlashFile>((Func<FlashFile, bool>) (ff => Path.GetFileName(ff.FileName) != null && Path.GetFileName(ff.FileName).Equals(fileName, StringComparison.OrdinalIgnoreCase)));
    FlashMeaning flashMeaning = flashFile != null ? flashFile.FlashAreas.SelectMany<FlashArea, FlashMeaning>((Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).FirstOrDefault<FlashMeaning>((Func<FlashMeaning, bool>) (m => PartExtensions.IsEqual(new Part(key), m.FlashKey))) : (FlashMeaning) null;
    if (flashMeaning != null)
    {
      GatherServerDataPage.AddFlashwareInformation(writer, flashMeaning, description);
    }
    else
    {
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
          writer.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GatherServerDataPage_Format_FlashKeyMismatch, (object) string.Join(", ", flashFile.FlashAreas.SelectMany<FlashArea, FlashMeaning>((Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).Select<FlashMeaning, string>((Func<FlashMeaning, string>) (m => m.FlashKey)))));
        else
          writer.WriteString(Resources.GatherServerDataPage_FileMissing);
      }
      writer.WriteEndElement();
    }
  }

  public void BuildDataSetList()
  {
    StringBuilder stringBuilder = new StringBuilder();
    using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
      this.WriteDatasetList(writer);
    this.datasetListPane.InnerHtml = stringBuilder.ToString();
  }

  private void WriteDatasetList(XmlWriter writer)
  {
    GroupCollection source1 = new GroupCollection();
    foreach (UnitInformation unitInformation in ServerDataManager.GlobalInstance.UnitInformation)
    {
      foreach (DeviceInformation deviceInformation in unitInformation.DeviceInformation)
      {
        bool flag = false;
        if (deviceInformation.DataSource == 1)
        {
          foreach (FirmwareOptionInformation firmwareOption in deviceInformation.FirmwareOptions)
          {
            if (firmwareOption.DataSetOptions.Count > 0)
            {
              flag = true;
              break;
            }
          }
        }
        else if (deviceInformation.DataSource == 2)
        {
          foreach (EdexFileInformation edexFile in deviceInformation.EdexFiles)
          {
            if (!edexFile.HasErrors && edexFile.ConfigurationInformation.DataSetPartNumbers.Any<Part>())
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
          source1.Add(deviceInformation.Device, (object) deviceInformation);
      }
    }
    if (((IEnumerable<Group>) source1).Any<Group>())
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
      foreach (Group group in (IEnumerable<Group>) ((IEnumerable<Group>) source1).OrderBy<Group, int?>((Func<Group, int?>) (g => GatherServerDataPage.GetEcuPriority(g.Name))))
      {
        Ecu ecuByName = SapiManager.GetEcuByName(group.Name);
        DeviceInformation.DeviceDataSource dataSource = ((DeviceInformation) group.Items[0]).DataSource;
        if (dataSource != 1)
        {
          if (dataSource == 2)
          {
            IEnumerable<\u003C\u003Ef__AnonymousType7<EdexFileInformation, Part>> source2 = CollectionExtensions.DistinctBy(group.Items.OfType<DeviceInformation>().SelectMany((Func<DeviceInformation, IEnumerable<EdexFileInformation>>) (device => (IEnumerable<EdexFileInformation>) device.EdexFiles), (device, edexFileInformation) => new
            {
              device = device,
              edexFileInformation = edexFileInformation
            }).Where(_param1 => !_param1.edexFileInformation.HasErrors && _param1.edexFileInformation.ConfigurationInformation.DataSetPartNumbers.Any<Part>()).SelectMany(_param1 => (IEnumerable<Part>) _param1.edexFileInformation.ConfigurationInformation.DataSetPartNumbers, (_param1, dataSetPartNumber) => new
            {
              EdexFileInformation = _param1.edexFileInformation,
              DataSetPartNumber = dataSetPartNumber
            }), n => n.DataSetPartNumber);
            bool flag = true;
            foreach (var data in source2)
            {
              string key = PartExtensions.ToFlashKeyStyleString(data.DataSetPartNumber);
              writer.WriteStartElement("tr");
              if (flag)
              {
                WebBrowserList.WriteGroupCell(writer, group.Name, source2.Count());
                flag = false;
              }
              writer.WriteElementString("td", key);
              FirmwareInformation informationForPart = ServerDataManager.GlobalInstance.GetFirmwareInformationForPart(data.EdexFileInformation.ConfigurationInformation.FlashwarePartNumber);
              if (informationForPart != null)
                writer.WriteElementString("td", informationForPart.Version);
              else
                writer.WriteElementString("td", PartExtensions.ToFlashKeyStyleString(data.EdexFileInformation.ConfigurationInformation.FlashwarePartNumber));
              FlashFile flashFile = Sapi.GetSapi().FlashFiles.FirstOrDefault<FlashFile>((Func<FlashFile, bool>) (ff => ff.FlashAreas.Any<FlashArea>((Func<FlashArea, bool>) (fa => fa.FlashMeanings.Any<FlashMeaning>((Func<FlashMeaning, bool>) (fm => fm.FlashKey.Equals(key)))))));
              FlashMeaning flashMeaning = flashFile != null ? flashFile.FlashAreas.SelectMany<FlashArea, FlashMeaning>((Func<FlashArea, IEnumerable<FlashMeaning>>) (fa => (IEnumerable<FlashMeaning>) fa.FlashMeanings)).FirstOrDefault<FlashMeaning>((Func<FlashMeaning, bool>) (m => PartExtensions.IsEqual(new Part(key), m.FlashKey))) : (FlashMeaning) null;
              if (flashMeaning != null)
                GatherServerDataPage.AddFlashwareInformation(writer, flashMeaning);
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
          }
        }
        else
        {
          IEnumerable<\u003C\u003Ef__AnonymousType5<FirmwareOptionInformation, DataSetOptionInformation>> source3 = CollectionExtensions.DistinctBy(group.Items.OfType<DeviceInformation>().SelectMany((Func<DeviceInformation, IEnumerable<FirmwareOptionInformation>>) (device => (IEnumerable<FirmwareOptionInformation>) device.FirmwareOptions), (device, firmwareOption) => new
          {
            device = device,
            firmwareOption = firmwareOption
          }).SelectMany(_param1 => (IEnumerable<DataSetOptionInformation>) _param1.firmwareOption.DataSetOptions, (_param1, datasetOption) => new
          {
            FirmwareOption = _param1.firmwareOption,
            DataSetOption = datasetOption
          }), n => n.DataSetOption.Key);
          bool flag = true;
          foreach (var data in source3)
          {
            writer.WriteStartElement("tr");
            if (flag)
            {
              WebBrowserList.WriteGroupCell(writer, group.Name, source3.Count());
              flag = false;
            }
            writer.WriteElementString("td", data.DataSetOption.Key);
            writer.WriteElementString("td", data.FirmwareOption.Version);
            GatherServerDataPage.AddFlashwareInformation(writer, data.DataSetOption.FileName, data.DataSetOption.Key, data.DataSetOption.Description, ecuByName != null && SapiExtensions.FlashingRequiresMvci(ecuByName));
            writer.WriteEndElement();
          }
        }
      }
      writer.WriteEndElement();
    }
    else
      writer.WriteElementString("p", Resources.GatherServerDataPage_NoInformation);
    this.tabsBuilt.Add("dataset");
  }

  private static void AddFlashwareInformation(
    XmlWriter writer,
    FlashMeaning flashMeaning,
    string description = null)
  {
    string fileName = Path.GetFileName(flashMeaning.FileName);
    writer.WriteElementString("td", fileName);
    if (!string.IsNullOrEmpty(description))
      writer.WriteElementString("td", description);
    else if (!string.IsNullOrEmpty(flashMeaning.Description))
    {
      writer.WriteElementString("td", flashMeaning.Description);
    }
    else
    {
      if (string.IsNullOrEmpty(flashMeaning.Name))
        return;
      writer.WriteElementString("td", flashMeaning.Name);
    }
  }

  private void BuildCBFList()
  {
    StringBuilder stringBuilder = new StringBuilder();
    using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
      this.WriteDiagnosisDescriptionsList(writer);
    this.diagnosticDescriptionListPane.InnerHtml = stringBuilder.ToString();
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
    foreach (IGrouping<string, Ecu> source in Sapi.GetSapi().Ecus.Where<Ecu>((Func<Ecu, bool>) (e => !e.IsRollCall && !e.IsVirtual)).OrderBy<Ecu, int>((Func<Ecu, int>) (e => e.Priority)).GroupBy<Ecu, string>((Func<Ecu, string>) (e => e.Identifier)))
    {
      bool flag = true;
      foreach (Ecu ecu in (IEnumerable<Ecu>) source)
      {
        writer.WriteStartElement("tr");
        if (flag)
        {
          WebBrowserList.WriteGroupCell(writer, ecu.Identifier, source.Count<Ecu>());
          flag = false;
        }
        writer.WriteElementString("th", ecu.Name);
        writer.WriteElementString("td", ecu.DescriptionDataVersion);
        writer.WriteElementString("td", Path.GetFileName(ecu.DescriptionFileName));
        writer.WriteElementString("td", ecu.ShortDescription);
        Match match = GatherServerDataPage.regexGPD.Match(ecu.GpdVersion);
        if (match.Success)
        {
          bool? nullable = new bool?();
          Version result;
          if (Version.TryParse(match.Groups["ver"].Value, out result))
            nullable = new bool?(result.Major >= 4);
          writer.WriteStartElement("td");
          writer.WriteAttributeString("style", "color:" + (nullable.HasValue ? (nullable.Value ? "green" : "red") : "black"));
          writer.WriteString(nullable.HasValue ? (nullable.Value ? Resources.GatherServerDataPage_Yes : Resources.GatherServerDataPage_No) : Resources.GatherServerDataPage_Unknown);
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }
    }
    writer.WriteEndElement();
    this.tabsBuilt.Add("diagnosticdescription");
  }

  private void BuildUnitList()
  {
    List<string> htmlItems1 = new List<string>();
    foreach (UnitInformation info in (IEnumerable<UnitInformation>) ServerDataManager.GlobalInstance.UnitInformation.Where<UnitInformation>((Func<UnitInformation, bool>) (ui => !this.newPendingRequests.Keys.Any<UnitInformation>((Func<UnitInformation, bool>) (pr => pr.IsSameIdentification(ui.EngineNumber, ui.VehicleIdentity))))).OrderByDescending<UnitInformation, DateTime>((Func<UnitInformation, DateTime>) (ui => Sapi.TimeFromString(ui.Time))))
      htmlItems1.Add(FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", (object) ReprogrammingView.CreateDownloadedUnitHtml(info, false))));
    List<string> htmlItems2 = new List<string>();
    foreach (UnitInformation key in this.newPendingRequests.Keys)
    {
      string str1 = $"{key.VehicleIdentity}_{key.EngineNumber}";
      string str2 = FormattableString.Invariant(FormattableStringFactory.Create("<tr><td><button id='pending_{0}' class='unitButton' onClick=\"clickButton('item_pending_{1}')\"><table>", (object) str1, (object) str1)) + FormattableString.Invariant(FormattableStringFactory.Create("<tr><td/><td><div class='identity'>{0}</div></td>", (object) ReprogrammingView.GetTitleString(key.VehicleIdentity, key.EngineNumber))) + FormattableString.Invariant(FormattableStringFactory.Create("<td>{0}</td>", (object) ReprogrammingView.WriteButton("remove_pending_" + str1, Resources.Wizard_ButtonRemove, "midblue"))) + "</tr>" + "</table></button></td></tr>";
      htmlItems2.Add(str2);
    }
    this.uploadUnits = new Collection<UnitInformation>();
    ServerDataManager.GlobalInstance.GetUploadUnits(this.uploadUnits, (ServerDataManager.UploadType) 0);
    List<string> htmlItems3 = new List<string>();
    foreach (UnitInformation uploadUnit in this.uploadUnits)
    {
      UnitInformation info = uploadUnit;
      bool hasPendingRequest = this.newPendingRequests.Keys.Any<UnitInformation>((Func<UnitInformation, bool>) (pr => pr.IsSameIdentification(info.EngineNumber, info.VehicleIdentity)));
      htmlItems3.Add(FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", (object) ReprogrammingView.CreateUploadUnitHtml(info, hasPendingRequest))));
    }
    string unitListHtml;
    if (htmlItems1.Count + htmlItems2.Count + htmlItems3.Count > 0)
    {
      unitListHtml = "<table class='grey unitlist' style='width:100%'>";
      WriteTableSection(htmlItems2, "pending", Resources.GatherServerDataPage_PendingRequestsSection);
      WriteTableSection(htmlItems1, "download", Resources.GatherServerDataPage_DownloadUnitsSection);
      WriteTableSection(htmlItems3, "upload", Resources.GatherServerDataPage_UploadUnitsSection);
      unitListHtml += "</table>";
    }
    else
      unitListHtml = FormattableString.Invariant(FormattableStringFactory.Create("<p>{0}</p>", (object) Resources.GatherServerDataPage_NoInformation));
    this.unitListPane.InnerHtml = unitListHtml + "<br/><br/><br/>";
    this.unitListbuttonPanel.InnerHtml = ReprogrammingView.WriteButton("add", Resources.Wizard_ButtonAddRequest) + ReprogrammingView.WriteButton("refreshAll", "Refresh All", "midblue") + ReprogrammingView.WriteButton("removeAll", "Remove All", "midblue") + ReprogrammingView.WriteButton("connect", Resources.Wizard_ButtonConnect);
    ReprogrammingView.SetButtonState(this.unitListbuttonPanel, "refreshAllButton", ServerDataManager.GlobalInstance.UnitInformation.Any<UnitInformation>(), true);
    ReprogrammingView.SetButtonState(this.unitListbuttonPanel, "removeAllButton", ServerDataManager.GlobalInstance.UnitInformation.Union<UnitInformation>((IEnumerable<UnitInformation>) this.newPendingRequests.Keys).Any<UnitInformation>(), true);
    ReprogrammingView.SetButtonState(this.unitListbuttonPanel, "connectButton", !ServerClient.GlobalInstance.InUse, true);
    this.tabsBuilt.Add("unit");
    if (this.selectedItem == null)
      return;
    HtmlElement button = this.GetButton(this.selectedItem);
    if (button != (HtmlElement) null)
      button.SetAttribute("className", "unitButton active");
    else
      this.selectedItem = (string) null;

    void WriteTableSection(List<string> htmlItems, string id, string sectionText)
    {
      if (!htmlItems.Any<string>())
        return;
      unitListHtml += FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", (object) ReprogrammingView.CreateTableSection(id, sectionText)));
      unitListHtml += string.Join(" ", (IEnumerable<string>) htmlItems);
    }
  }

  private void TabControlSelectedIndexChanged(string selectedTab)
  {
    this.unitListPane.SetAttribute("className", selectedTab == "unit" ? "show" : "hide");
    this.unitListbuttonPanel.SetAttribute("className", selectedTab == "unit" ? "fixedbottom show" : "fixedbottom hide");
    this.softwareListPane.SetAttribute("className", selectedTab == "software" ? "show" : "hide");
    this.softwareListButtonPanel.SetAttribute("className", selectedTab == "software" ? "fixedbottom show" : "fixedbottom hide");
    this.datasetListPane.SetAttribute("className", selectedTab == "dataset" ? "show" : "hide");
    this.diagnosticDescriptionListPane.SetAttribute("className", selectedTab == "diagnosticdescription" ? "show" : "hide");
    this.selectedTab = selectedTab;
    if (this.tabsBuilt.Contains(this.selectedTab))
      return;
    switch (this.selectedTab)
    {
      case "unit":
        this.BuildUnitList();
        break;
      case "software":
        this.BuildFirmwareList();
        break;
      case "dataset":
        this.BuildDataSetList();
        break;
      case "diagnosticdescription":
        this.BuildCBFList();
        break;
    }
  }
}
