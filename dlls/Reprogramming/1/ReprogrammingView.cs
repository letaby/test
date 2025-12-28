// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ReprogrammingView
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Reflection;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class ReprogrammingView : ContextHelpControl, IProvideProgrammingData
{
  public const string AutomaticWarningPanelItemName = "automaticoperation";
  private WizardPage activePage;
  private List<WizardPage> pages = new List<WizardPage>();
  private bool switchedFromConnectedUnitView;
  private HtmlElement switchToConnectedUnitButton;
  private HtmlElement switchToAllUnitsButton;
  private HtmlElement wizardPane;
  private bool advanceToLast;
  private WebBrowser webBrowserList;
  private IContainer components;

  public IEnumerable<DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgrammingData> ProgrammingData { set; get; }

  public UnitInformation SelectedUnit { get; set; }

  public bool RequiresCompatibilityChecks
  {
    get
    {
      IEnumerable<DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgrammingData> programmingData = this.ProgrammingData;
      return programmingData != null && programmingData.Count<DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgrammingData>() == 1;
    }
  }

  internal static string CreateInput(
    string list,
    string value,
    string label,
    string status,
    bool selected,
    bool disabled,
    string statusIcon = null)
  {
    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
    object[] objArray = new object[7]
    {
      (object) list,
      (object) value,
      selected ? (object) "checked" : (object) "",
      disabled ? (object) nameof (disabled) : (object) "",
      (object) label,
      !string.IsNullOrEmpty(status) ? (object) $" - <i>{status}</i>" : (object) string.Empty,
      null
    };
    string str;
    if (statusIcon == null)
      str = string.Empty;
    else
      str = FormattableString.Invariant(FormattableStringFactory.Create("<span class='{0}'/>", (object) statusIcon));
    objArray[6] = (object) str;
    return string.Format((IFormatProvider) invariantCulture, "<label for='{1}' class='{3}'><input type='radio' name='{0}' id='{1}' value='{1}' {2} {3} />{6}{4} {5}</label><br/>", objArray);
  }

  internal static string CreateTextInput(
    string name,
    string label,
    string initialValue,
    int maxLength,
    string className)
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<label for='{0}' >{1}</label><input type='text' name='{0}' id='{0}' value='{2}' maxlength='{3}' class='{4}'/><br/>", (object) name, (object) label, (object) (initialValue ?? string.Empty), (object) maxLength, (object) (className ?? string.Empty));
  }

  internal static bool SubscribeInputEvents(
    HtmlElement element,
    string inputType,
    string eventName,
    EventHandler handler,
    bool autoSelect = true)
  {
    List<HtmlElement> source = new List<HtmlElement>();
    bool flag = false;
    foreach (HtmlElement htmlElement in element.GetElementsByTagName("input").OfType<HtmlElement>().Where<HtmlElement>((Func<HtmlElement, bool>) (el => el.OuterHtml.IndexOf($"type=\"{inputType}\"", StringComparison.Ordinal) != -1)))
    {
      HtmlElement inputElement = htmlElement;
      inputElement.AttachEventHandler(eventName, (EventHandler) ((sender, args) =>
      {
        try
        {
          handler((object) inputElement, args);
        }
        catch (Exception ex)
        {
          CrashHandler.ReportExceptionAndTerminate(ex);
        }
      }));
      if (inputElement.Enabled)
        source.Add(inputElement);
      if (bool.Parse(inputElement.GetAttribute("checked")))
        flag = true;
    }
    if (!autoSelect || !(inputType == "radio") || flag || source.Count != 1)
      return false;
    HtmlElement sender1 = source.First<HtmlElement>();
    sender1.SetAttribute("checked", "True");
    handler((object) sender1, new EventArgs());
    return true;
  }

  internal static string CreateRadioList(string title, List<string> listItems)
  {
    return FormattableString.Invariant(FormattableStringFactory.Create("<p><div>{0}</div><div class='radioList'>{1}</div></p>", (object) title, (object) string.Join(" ", (IEnumerable<string>) listItems)));
  }

  internal static void SetActiveRadioElement(HtmlElement inputElement)
  {
    HtmlElement parent = inputElement.Parent;
    foreach (HtmlElement child in parent.Parent.Children)
    {
      string attribute = child.GetAttribute("className");
      child.SetAttribute("className", child == parent ? "active" + attribute : attribute.Replace("active", string.Empty));
    }
  }

  internal static string GetTitleString(string vehicleIdentity, string engineNumber)
  {
    string str1 = string.Empty;
    if (!string.IsNullOrEmpty(vehicleIdentity) || !string.IsNullOrEmpty(engineNumber))
    {
      if (!string.IsNullOrEmpty(vehicleIdentity))
      {
        VinInformation vinInfo = VinInformation.GetVinInformation(vehicleIdentity);
        string[] source = new string[3]
        {
          vehicleIdentity,
          engineNumber,
          null
        };
        string str2;
        if (!vinInfo.InformationEntries.Any<KeyValuePair<string, string>>())
          str2 = (string) null;
        else
          str2 = string.Join(" ", ((IEnumerable<string>) new string[3]
          {
            "modelyear",
            "make",
            "model"
          }).Select<string, string>((Func<string, string>) (q => vinInfo.GetInformationValue(q))));
        source[2] = str2;
        str1 = string.Join(" - ", ((IEnumerable<string>) source).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))));
      }
      else
        str1 = engineNumber;
    }
    return string.IsNullOrEmpty(str1) ? Resources.ReprogrammingView_NoTitle : str1;
  }

  internal static void SetButtonState(HtmlElement container, string id, bool enabled, bool show)
  {
    HtmlElement button = container.GetElementsByTagName("button").OfType<HtmlElement>().FirstOrDefault<HtmlElement>((Func<HtmlElement, bool>) (b => b.Id == id));
    if (!(button != (HtmlElement) null))
      return;
    ReprogrammingView.SetButtonState(button, enabled, show);
  }

  internal static void SetButtonState(HtmlElement button, bool enabled, bool show)
  {
    button.SetAttribute("className", (enabled ? "button blue" : "button gray") + (show ? " show" : " hide"));
    ReprogrammingView.EnableElement(button, enabled);
  }

  internal static void EnableElement(HtmlElement element, bool enabled)
  {
    element.Document.InvokeScript("enableButton", new object[2]
    {
      (object) element.Id,
      (object) enabled
    });
  }

  internal static string WriteButton(string id, string text, string color = "blue")
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<button id='{0}Button' class='button {2}' onClick=\"clickButton('{0}')\">{1}</button>", (object) id, (object) text, (object) color);
  }

  internal static string GetUnitTimeDisplay(UnitInformation unit)
  {
    bool flag = unit.Status == 1;
    DateTime dateTime = flag ? unit.SettingsInformation.Max<SettingsInformation, DateTime>((Func<SettingsInformation, DateTime>) (si => si.Timestamp.Value)) : Utility.TimeFromString(unit.Time);
    if (dateTime.Date == DateTime.Now.Date)
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, flag ? Resources.ConnectedUnit_FormatUnitDataUpdatedToday : Resources.ConnectedUnit_FormatUnitDataDownloadedToday, (object) dateTime.ToShortTimeString());
    if (dateTime.Date == (DateTime.Now - TimeSpan.FromDays(1.0)).Date)
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, flag ? Resources.ConnectedUnit_FormatUnitDataUpdatedYesterday : Resources.ConnectedUnit_FormatUnitDataDownloadedYesterday, (object) dateTime.ToShortTimeString());
    return dateTime.Date > DateTime.Now - TimeSpan.FromDays(7.0) && dateTime.Date < DateTime.Now ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, flag ? Resources.ConnectedUnit_FormatUnitDataUpdatedOn : Resources.ConnectedUnit_FormatUnitDataDownloadedOn, (object) CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dateTime.DayOfWeek)) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, flag ? Resources.ConnectedUnit_FormatUnitDataUpdatedOn : Resources.ConnectedUnit_FormatUnitDataDownloadedOn, (object) dateTime.ToLongDateString());
  }

  private static string CreateStatusHtml(
    string prefix,
    IEnumerable<string> serverErrors,
    IEnumerable<string> deviceErrors,
    IEnumerable<string> missingDevices,
    bool hasDevicesForDataSource,
    bool showErrors)
  {
    string empty = string.Empty;
    IEnumerable<string> source = (IEnumerable<string>) null;
    string element = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GatherServerDataPage_Format_MissingServerDataForDevices, (object) string.Join(", ", missingDevices));
    string str1;
    if (deviceErrors.Any<string>() || serverErrors.Any<string>())
    {
      str1 = string.Join(Resources.GatherServerDataPage_ServerStatusSeparator, ((IEnumerable<string>) new string[3]
      {
        serverErrors.Any<string>() ? ServerDataManager.GetServerStatusText(serverErrors.First<string>()) : (string) null,
        deviceErrors.Any<string>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GatherServerDataPage_FormatDeviceErrors, (object) deviceErrors.Distinct<string>().Count<string>()) : (string) null,
        missingDevices.Any<string>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GatherServerDataPage_FormatMissingDevices, (object) missingDevices.Distinct<string>().Count<string>()) : (string) null
      }).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))));
      source = (missingDevices.Any<string>() ? deviceErrors.Union<string>(Enumerable.Repeat<string>(element, 1)) : deviceErrors).Distinct<string>();
    }
    else
      str1 = missingDevices.Any<string>() ? element : (hasDevicesForDataSource ? Resources.GatherServerDataPage_DataOK : Resources.GatherServerDataPage_DataNotApplicable);
    string str2 = FormattableString.Invariant(FormattableStringFactory.Create("<div class='nowrap'><span>{0}</span><span>{1}</span>", (object) prefix, (object) str1));
    if (source != null)
      str2 += FormattableString.Invariant(FormattableStringFactory.Create("<div class='{0}'>{1}</div>", showErrors ? (object) string.Empty : (object) "help", (object) string.Join(" ", source.Select<string, string>((Func<string, string>) (ef => FormattableString.Invariant(FormattableStringFactory.Create("<div class='nowrap indent'>{0}</div>", (object) ef)))))));
    return str2 + "</div>";
  }

  internal static string CreateDownloadedUnitHtml(UnitInformation info, bool expandErrors)
  {
    IEnumerable<string> strings = info.DeviceInformation.Where<DeviceInformation>((Func<DeviceInformation, bool>) (d => d.DataSource == 2)).Where<DeviceInformation>((Func<DeviceInformation, bool>) (deviceInfo => GetPartsForDevice(deviceInfo).Any<Part>((Func<Part, bool>) (p => FlashFileRequiresDownload(p))))).Select<DeviceInformation, string>((Func<DeviceInformation, string>) (deviceInfo => deviceInfo.Device));
    string str1 = strings.Any<string>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadInProgress ? Resources.Unit_FormatRemoteInProgressStatus : Resources.Unit_FormatRemoteDownloadNeededStatus, (object) string.Join(", ", strings)) : string.Empty;
    string str2 = $"{info.VehicleIdentity}_{info.EngineNumber}";
    string str3 = FormattableString.Invariant(FormattableStringFactory.Create("<button id='download_{0}' class='unitButton' onClick=\"clickButton('item_download_{1}')\"><table>", (object) str2, (object) str2)) + FormattableString.Invariant(FormattableStringFactory.Create("<tr><td><div class='{0}'/></td><td>", (object) ReprogrammingView.GetRowClass(info, strings.Any<string>()))) + FormattableString.Invariant(FormattableStringFactory.Create("<div><div class='identity'>{0}</div>", (object) ReprogrammingView.GetTitleString(info.VehicleIdentity, info.EngineNumber))) + FormattableString.Invariant(FormattableStringFactory.Create("<div><span>{0}</span> <span class='parameter'>{1}</span></div>", (object) ReprogrammingView.GetUnitTimeDisplay(info), (object) str1));
    string str4;
    if (info.Status == 3)
    {
      str4 = str3 + FormattableString.Invariant(FormattableStringFactory.Create("<div>{0}</div>", (object) Resources.GatherServerDataPage_UnitStatusExpired));
    }
    else
    {
      str4 = str3 + ReprogrammingView.CreateStatusHtml(Resources.GatherServerDataPage_PowertrainStatus, info.DepotServerErrors.Union<string>((IEnumerable<string>) info.DepotServerWarnings), info.DepotDeviceServerErrors.Select<Tuple<string, string>, string>((Func<Tuple<string, string>, string>) (d => ServerDataManager.GetDeviceStatusText(d.Item1, d.Item2))), info.MissingDevices.Distinct<string>().Where<string>((Func<string, bool>) (d => UnitInformation.GetDataSource(d) == 1)), info.DeviceInformation.Any<DeviceInformation>((Func<DeviceInformation, bool>) (d => d.DataSource == 1)), expandErrors);
      if (ReprogrammingView.ShowEdexInformation)
        str4 += ReprogrammingView.CreateStatusHtml(Resources.GatherServerDataPage_ChassisStatus, (IEnumerable<string>) info.EdexServerErrors, info.EdexFileServerErrors.Select<string, string>((Func<string, string>) (d => ServerDataManager.GetServerStatusText(d))), info.MissingDevices.Distinct<string>().Where<string>((Func<string, bool>) (d => UnitInformation.GetDataSource(d) == 2)), info.DeviceInformation.Any<DeviceInformation>((Func<DeviceInformation, bool>) (d => d.DataSource == 2)), expandErrors);
    }
    foreach (AutomaticOperation automaticOperation in info.AutomaticOperations)
    {
      string str5 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} - {1} - {2} - {3}", (object) automaticOperation.Reason, (object) DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgrammingData.OperationToDisplayString((ProgrammingOperation) automaticOperation.OperationType), (object) automaticOperation.Device, (object) Sapi.TimeFromString(automaticOperation.Time).ToShortDateString());
      string str6 = automaticOperation.Completed ? Resources.GatherServerDataPage_Status_AutomaticOperationComplete : Resources.GatherServerDataPage_Status_AutomaticOperationRequired;
      str4 += FormattableString.Invariant(FormattableStringFactory.Create("<div class='nowrap indent'>{0} - <i>{1}</i></div>", (object) str5, (object) str6));
    }
    return str4 + "</td>" + FormattableString.Invariant(FormattableStringFactory.Create("<td>{0}</td><td>{1}</td>", (object) ReprogrammingView.WriteButton("refresh_download_" + str2, Resources.Wizard_ButtonRefresh, "midblue"), (object) ReprogrammingView.WriteButton("remove_download_" + str2, Resources.Wizard_ButtonRemove, "midblue"))) + "</tr></table></button>";

    static bool FlashFileRequiresDownload(Part part)
    {
      return ServerDataManager.GlobalInstance.FirmwareInformation.OfType<FirmwareInformation>().Any<FirmwareInformation>((Func<FirmwareInformation, bool>) (fi => PartExtensions.IsEqual(part, fi.Key) && fi.RequiresDownload));
    }

    static IEnumerable<Part> GetPartsForDevice(DeviceInformation deviceInfo)
    {
      return deviceInfo.EdexFiles.Where<EdexFileInformation>((Func<EdexFileInformation, bool>) (ef => ef.ConfigurationInformation != null)).SelectMany<EdexFileInformation, Part>((Func<EdexFileInformation, IEnumerable<Part>>) (ef => ef.ConfigurationInformation.AllFlashwarePartNumbers)).Distinct<Part>();
    }
  }

  internal static string CreateUploadUnitHtml(UnitInformation info, bool hasPendingRequest)
  {
    string str1 = $"{info.VehicleIdentity}_{info.EngineNumber}";
    string str2 = FormattableString.Invariant(FormattableStringFactory.Create("<button id='upload_{0}' class='unitButton' onClick=\"clickButton('item_upload_{1}')\"><table>", (object) str1, (object) str1)) + "<tr><td class='statusUpload'/><td>" + FormattableString.Invariant(FormattableStringFactory.Create("<div class='identity'>{0}</div>", (object) ReprogrammingView.GetTitleString(info.VehicleIdentity, info.EngineNumber))) + FormattableString.Invariant(FormattableStringFactory.Create("<div>{0}</div>", (object) ReprogrammingView.GetUnitTimeDisplay(info)));
    if (info.SettingsInformation.Any<SettingsInformation>((Func<SettingsInformation, bool>) (s => UnitInformation.GetDataSource(s.Device) == 1)))
      str2 += FormattableString.Invariant(FormattableStringFactory.Create("<div>{0}</div>", (object) (Resources.GatherServerDataPage_PowertrainStatus + string.Format((IFormatProvider) CultureInfo.CurrentCulture, hasPendingRequest ? Resources.GatherServerDataPage_FormatUploadPending : Resources.GatherServerDataPage_FormatUploadFor, (object) string.Join(", ", info.SettingsInformation.Select<SettingsInformation, string>((Func<SettingsInformation, string>) (si => si.Device)).Distinct<string>())))));
    bool flag = ReprogrammingView.ShowEdexInformation && info.SettingsInformation.Any<SettingsInformation>((Func<SettingsInformation, bool>) (s => UnitInformation.GetDataSource(s.Device) == 2));
    if (flag)
      str2 += FormattableString.Invariant(FormattableStringFactory.Create("<div>{0}</div>", (object) (Resources.GatherServerDataPage_ChassisStatus + string.Format((IFormatProvider) CultureInfo.CurrentCulture, hasPendingRequest ? Resources.GatherServerDataPage_FormatUploadPending : Resources.GatherServerDataPage_FormatUploadFor, (object) string.Join(", ", info.SettingsInformation.Select<SettingsInformation, string>((Func<SettingsInformation, string>) (si => si.Device)).Distinct<string>())))));
    string str3 = str2 + "</td>";
    string str4;
    if (!flag)
      str4 = "<td/>";
    else
      str4 = FormattableString.Invariant(FormattableStringFactory.Create("<td>{0}</td>", (object) ReprogrammingView.WriteButton("view_upload_" + str1, Resources.Wizard_ButtonView, "midblue")));
    return str3 + str4 + FormattableString.Invariant(FormattableStringFactory.Create("<td>{0}</td>", (object) ReprogrammingView.WriteButton("remove_upload_" + str1, Resources.Wizard_ButtonRemove, "midblue"))) + "</tr></table></button>";
  }

  internal static string CreateUnitToBeDownloadedHtml(string vehicleIdentity, string engineNumber)
  {
    string str = $"{vehicleIdentity}_{engineNumber}";
    return FormattableString.Invariant(FormattableStringFactory.Create("\r\n                <button id='download_{0}' class='unitButton' onClick=\"clickButton('item_download_{1}')\">\r\n                    <table>\r\n                        <tr>\r\n                            <td class='statusWarning'/>\r\n                            <td>\r\n                                <div class='identity'>{2}</div>\r\n                                <div>{3}</div>\r\n                            </td>\r\n                            <td/>\r\n                        </tr>\r\n                    </table>\r\n                </button>\r\n            ", (object) str, (object) str, (object) ReprogrammingView.GetTitleString(vehicleIdentity, engineNumber), (object) Resources.ConnectedUnit_NoDownload));
  }

  private static string GetRowClass(UnitInformation info, bool remoteSoftwareDownloadNeeded)
  {
    string rowClass = string.Empty;
    switch (info.Status - 2)
    {
      case 0:
        rowClass = remoteSoftwareDownloadNeeded ? (LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadInProgress ? "statusOkRemoteDownloading" : "statusOkRemoteNeeded") : "statusOk";
        break;
      case 1:
        rowClass = "statusError";
        break;
      case 2:
        rowClass = info.DeviceInformation.Count == 0 ? "statusError" : "statusWarning";
        break;
    }
    return rowClass;
  }

  private static bool ShowEdexInformation
  {
    get
    {
      return ApplicationInformation.CanReprogramEdexUnits || ServerDataManager.GlobalInstance.UnitInformation.Any<UnitInformation>((Func<UnitInformation, bool>) (ui => ui.DeviceInformation.Any<DeviceInformation>((Func<DeviceInformation, bool>) (di => di.DataSource == 2 && ApplicationInformation.CanReprogramEdexEcu(di.Device) && di.EdexFiles.Any<EdexFileInformation>((Func<EdexFileInformation, bool>) (ef => ef.HasErrors))))));
    }
  }

  internal static string CreateTableSection(string id, string sectionText)
  {
    return FormattableString.Invariant(FormattableStringFactory.Create("\r\n                <button id='{0}' class='unitButton' onClick=\"clickButton('item_{1}')\">\r\n                    <table>\r\n                        <tr>\r\n                            <th/><th><div class='identity'>{2}</div></th>\r\n                            <th>{3}</th><th>{4}</th>\r\n                        </tr>\r\n                    </table>\r\n                </button>\r\n            ", (object) id, (object) id, (object) sectionText, id == "download" ? (object) ReprogrammingView.WriteButton("refresh_" + id, Resources.Wizard_ButtonRefresh, "midblue") : (object) string.Empty, (object) ReprogrammingView.WriteButton("remove_" + id, Resources.Wizard_ButtonRemove, "midblue")));
  }

  internal static void UpdateTitle(HtmlElement element, UnitInformation unit)
  {
    string str = FormattableString.Invariant(FormattableStringFactory.Create("<table class='grey' style='width:100%'>")) + FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", (object) ReprogrammingView.CreateDownloadedUnitHtml(unit, false))) + "</td></tr></table>";
    element.InnerHtml = str;
  }

  public ReprogrammingView()
  {
    MenuProxy.GlobalInstance.ReprogrammingView = this;
    this.InitializeComponent();
    using (StreamReader streamReader = new StreamReader(typeof (ProgramDeviceManager).Assembly.GetManifestResourceStream("DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.BlankProgramDeviceList.html")))
      this.webBrowserList.DocumentText = streamReader.ReadToEnd();
    this.SetLink(LinkSupport.GetViewLink((PanelIdentifier) 7));
    SapiManager.GlobalInstance.SapiResetCompleted += new EventHandler(this.GlobalInstance_SapiResetCompleted);
    SapiManager.GlobalInstance.ActiveChannelsListChanged += new EventHandler(this.GlobalInstance_ActiveChannelsListChanged);
    ServerClient.GlobalInstance.InUseChanged += new EventHandler(this.GlobalInstance_InUseChanged);
  }

  private void GlobalInstance_SapiResetCompleted(object sender, EventArgs e)
  {
    this.CheckForMissingCBFs();
  }

  private void webBrowserList_DocumentCompleted(
    object sender,
    WebBrowserDocumentCompletedEventArgs e)
  {
    try
    {
      this.SetInputPane(this.webBrowserList.Document.All.GetElementsByName("inputpane")[0]);
    }
    catch (Exception ex)
    {
      CrashHandler.ReportExceptionAndTerminate(ex);
    }
  }

  private void webBrowserList_Navigating(object sender, WebBrowserNavigatingEventArgs e)
  {
    try
    {
      if (!(e.Url.ToString() != "about:blank") && string.IsNullOrEmpty(e.Url.Fragment))
        return;
      e.Cancel = true;
      bool flag = this.pages.Count > 0 && this.activePage == this.pages[1];
      switch (e.Url.Fragment)
      {
        case "#tab_connectedunit":
          this.switchedFromConnectedUnitView = flag;
          this.ShowOrHideConnectedUnitPage();
          break;
        case "#tab_allunits":
          this.switchedFromConnectedUnitView = flag;
          this.ShowOrHideConnectedUnitPage();
          break;
        case "#button_back":
          this.MoveBack();
          break;
        case "#button_next":
          this.MoveNext();
          break;
        default:
          this.activePage?.Navigate(e.Url.Fragment);
          break;
      }
    }
    catch (Exception ex)
    {
      CrashHandler.ReportExceptionAndTerminate(ex);
    }
  }

  private void WebBrowserListOnPreviewKeyDown(
    object sender,
    PreviewKeyDownEventArgs previewKeyDownEventArgs)
  {
    if (previewKeyDownEventArgs.KeyCode != Keys.F5)
      return;
    previewKeyDownEventArgs.IsInputKey = true;
  }

  private void SetInputPane(HtmlElement inputPane)
  {
    inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("\r\n                <div>\r\n                    <div id='gatherServerDataPane' class='show'>&nbsp;</div>\r\n                    <div id='wizardPane' class='hide'>\r\n                        <div class='tab'>\r\n                            <button id='connectedUnitTab' class='tablinks' disabled>{0}</button>\r\n                            <button id='selectOperationTab' class='tablinks' disabled>{1}</button>\r\n                            <button id='programDeviceTab' class='tablinks' disabled>{2}</button>\r\n                            <button id='switchToAllUnitsButton' class='tablinks exit hide' onClick=\"clickTab(event,'allunits', true)\">&#x279c; {3}</button>\r\n                        </div>\r\n                        <div id='connectedUnitPane'>&nbsp;</div>\r\n                        <div id='selectOperationPane' class='hide'>&nbsp;</div>\r\n                        <div id='programDevicePane' class='hide'>&nbsp;</div>\r\n                    </div>\r\n                </div>\r\n            ", (object) Resources.Header_GatherServerData, (object) Resources.Header_SelectOperation, (object) Resources.Header_ProgramDevice, (object) Resources.GatherServerDataPage_TabUnitManagement));
    GatherServerDataPage gatherServerDataPage = (GatherServerDataPage) null;
    SelectOperationPage selectOperationPage = (SelectOperationPage) null;
    ProgramDevicePage programDevicePage = (ProgramDevicePage) null;
    ConnectedUnitPage connectedUnitPage = (ConnectedUnitPage) null;
    foreach (HtmlElement htmlElement in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList<HtmlElement>())
    {
      switch (htmlElement.Id)
      {
        case "wizardPane":
          this.wizardPane = htmlElement;
          continue;
        case "gatherServerDataPane":
          gatherServerDataPage = new GatherServerDataPage(this, htmlElement);
          continue;
        case "connectedUnitPane":
          connectedUnitPage = new ConnectedUnitPage(gatherServerDataPage, this, htmlElement);
          continue;
        case "selectOperationPane":
          selectOperationPage = new SelectOperationPage(this, htmlElement);
          continue;
        case "programDevicePane":
          programDevicePage = new ProgramDevicePage(this, htmlElement);
          continue;
        default:
          continue;
      }
    }
    foreach (HtmlElement htmlElement in inputPane.GetElementsByTagName("button").OfType<HtmlElement>())
    {
      switch (htmlElement.Id)
      {
        case "switchToConnectedUnitButton":
          this.switchToConnectedUnitButton = htmlElement;
          continue;
        case "switchToAllUnitsButton":
          this.switchToAllUnitsButton = htmlElement;
          continue;
        case "connectedUnitTab":
          connectedUnitPage.Tab = htmlElement;
          continue;
        case "selectOperationTab":
          selectOperationPage.Tab = htmlElement;
          continue;
        case "programDeviceTab":
          programDevicePage.Tab = htmlElement;
          continue;
        default:
          continue;
      }
    }
    this.pages.Add((WizardPage) gatherServerDataPage);
    this.pages.Add((WizardPage) connectedUnitPage);
    this.pages.Add((WizardPage) selectOperationPage);
    this.pages.Add((WizardPage) programDevicePage);
    if (!this.advanceToLast)
    {
      this.SetActivePage((WizardPage) gatherServerDataPage);
      this.ShowOrHideConnectedUnitPage();
    }
    else
      this.AdvanceToLastPage();
  }

  private void GlobalInstance_ActiveChannelsListChanged(object sender, EventArgs e)
  {
    if (ServerClient.GlobalInstance.InUse || this.webBrowserList == null || this.webBrowserList.IsDisposed)
      return;
    this.ShowOrHideConnectedUnitPage();
  }

  private void GlobalInstance_InUseChanged(object sender, EventArgs e)
  {
    if (ServerClient.GlobalInstance.InUse)
      return;
    this.ShowOrHideConnectedUnitPage();
  }

  private void ShowOrHideConnectedUnitPage()
  {
    if (this.pages.Count <= 0)
      return;
    if (!this.switchedFromConnectedUnitView && ConnectedUnitPage.ProgrammableChannels.Any<Channel>())
    {
      if (this.activePage != this.pages[0])
        return;
      this.wizardPane.SetAttribute("className", "show");
      this.SetActivePage(this.pages[1]);
    }
    else
    {
      if (!ConnectedUnitPage.ProgrammableChannels.Any<Channel>())
        this.switchedFromConnectedUnitView = false;
      if (this.activePage != this.pages.Last<WizardPage>())
      {
        this.wizardPane.SetAttribute("className", "hide");
        this.SetActivePage(this.pages[0]);
      }
      this.switchToConnectedUnitButton.SetAttribute("className", this.switchedFromConnectedUnitView ? "tablinks exit show" : "tablinks exitdisabled show");
      ReprogrammingView.EnableElement(this.switchToConnectedUnitButton, this.switchedFromConnectedUnitView);
    }
  }

  protected virtual void OnVisibleChanged(EventArgs e)
  {
    this.CheckForMissingCBFs();
    // ISSUE: explicit non-virtual call
    __nonvirtual (((ScrollableControl) this).OnVisibleChanged(e));
  }

  private void CheckForMissingCBFs()
  {
    if (((Control) this).Visible)
    {
      IEnumerable<CodingFile> source = SapiManager.GlobalInstance.Sapi.CodingFiles.Where<CodingFile>((Func<CodingFile, bool>) (f => !f.Ecus.Any<Ecu>()));
      if (source.Any<CodingFile>())
        WarningsPanel.GlobalInstance.Add("MissingCBF", MessageBoxIcon.Hand, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReprogrammingViewFormat_MissingCBFWarning, (object) string.Join(", ", source.Select<CodingFile, string>((Func<CodingFile, string>) (cf => Path.GetFileName(cf.FileName))))), (EventHandler) null);
      else
        WarningsPanel.GlobalInstance.Remove("MissingCBF");
    }
    else
      WarningsPanel.GlobalInstance.Remove("MissingCBF");
  }

  private void SetActivePage(WizardPage page)
  {
    WizardPage activePage = this.activePage;
    this.activePage = page;
    this.pages.Except<WizardPage>(Enumerable.Repeat<WizardPage>(this.activePage, 1)).ToList<WizardPage>().ForEach((Action<WizardPage>) (p => p.Visible = false));
    this.activePage.Visible = true;
    this.switchToAllUnitsButton.SetAttribute("className", this.activePage == this.pages[1] ? "tablinks exit show" : "tablinks exit hide");
    this.webBrowserList.Document.InvokeScript("alignTabContainers");
    this.Busy = true;
    activePage?.OnSetInactive();
    this.activePage.OnSetActive();
    this.Busy = false;
    if (this.activePage is ConnectedUnitPage)
    {
      MenuProxy.GlobalInstance.CheckAutomaticOperations();
    }
    else
    {
      if (!(this.activePage is ProgramDevicePage))
        return;
      WarningsPanel.GlobalInstance.Remove("automaticoperation");
    }
  }

  internal void AdvanceToLastPage()
  {
    if (this.pages.Count == 0)
      this.advanceToLast = true;
    else
      this.SetActivePage(this.Pages.Last<WizardPage>());
  }

  internal WizardPage ActivePage => this.activePage;

  internal IList<WizardPage> Pages
  {
    get => (IList<WizardPage>) this.pages.Skip<WizardPage>(1).ToList<WizardPage>();
  }

  private void MoveBack()
  {
    WizardPage page = this.activePage.OnWizardBack();
    if (page == null)
      return;
    this.SetActivePage(page);
  }

  private void MoveNext()
  {
    WizardPage page = this.activePage.OnWizardNext();
    if (page == null)
      return;
    this.SetActivePage(page);
  }

  internal bool Busy
  {
    set
    {
      if (this.webBrowserList == null || this.webBrowserList.IsDisposed)
        return;
      this.webBrowserList.Document.InvokeScript("showLoaderAnimation", new object[1]
      {
        (object) value
      });
      this.webBrowserList.Invalidate();
      this.webBrowserList.Update();
    }
  }

  public void SetWizardButtons(WizardControl.WizardButtons buttons)
  {
    if (this.activePage.BackButton != (HtmlElement) null)
      ReprogrammingView.SetButtonState(this.activePage.BackButton, (buttons & 1) > 0, true);
    if (!(this.activePage.NextButton != (HtmlElement) null))
      return;
    ReprogrammingView.SetButtonState(this.activePage.NextButton, (buttons & 6) > 0, true);
    this.activePage.NextButton.InnerText = (buttons & 4) != null ? Resources.Wizard_ButtonFinish : Resources.Wizard_ButtonNext;
  }

  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      if (this.components != null)
        this.components.Dispose();
      foreach (WizardPage page in this.pages)
      {
        if (page is IDisposable disposable)
          disposable.Dispose();
      }
      this.pages.Clear();
      SapiManager.GlobalInstance.SapiResetCompleted -= new EventHandler(this.GlobalInstance_SapiResetCompleted);
      SapiManager.GlobalInstance.ActiveChannelsListChanged -= new EventHandler(this.GlobalInstance_ActiveChannelsListChanged);
      ServerClient.GlobalInstance.InUseChanged -= new EventHandler(this.GlobalInstance_InUseChanged);
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ReprogrammingView));
    this.webBrowserList = new WebBrowser();
    ((Control) this).SuspendLayout();
    this.webBrowserList.AllowWebBrowserDrop = false;
    this.webBrowserList.IsWebBrowserContextMenuEnabled = false;
    componentResourceManager.ApplyResources((object) this.webBrowserList, "webBrowserList");
    this.webBrowserList.Name = "webBrowserList";
    this.webBrowserList.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowserList_DocumentCompleted);
    this.webBrowserList.Navigating += new WebBrowserNavigatingEventHandler(this.webBrowserList_Navigating);
    this.webBrowserList.PreviewKeyDown += new PreviewKeyDownEventHandler(this.WebBrowserListOnPreviewKeyDown);
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.webBrowserList);
    ((Control) this).Name = nameof (ReprogrammingView);
    ((Control) this).ResumeLayout(false);
  }
}
