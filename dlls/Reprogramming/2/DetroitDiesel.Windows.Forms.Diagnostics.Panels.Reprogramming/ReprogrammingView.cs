using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Reflection;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;

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

	public IEnumerable<ProgrammingData> ProgrammingData { get; set; }

	public UnitInformation SelectedUnit { get; set; }

	public bool RequiresCompatibilityChecks
	{
		get
		{
			IEnumerable<ProgrammingData> programmingData = ProgrammingData;
			if (programmingData == null)
			{
				return false;
			}
			return programmingData.Count() == 1;
		}
	}

	private static bool ShowEdexInformation
	{
		get
		{
			if (!ApplicationInformation.CanReprogramEdexUnits)
			{
				return ServerDataManager.GlobalInstance.UnitInformation.Any((UnitInformation ui) => ui.DeviceInformation.Any((DeviceInformation di) => (int)di.DataSource == 2 && ApplicationInformation.CanReprogramEdexEcu(di.Device) && di.EdexFiles.Any((EdexFileInformation ef) => ef.HasErrors)));
			}
			return true;
		}
	}

	internal WizardPage ActivePage => activePage;

	internal IList<WizardPage> Pages => pages.Skip(1).ToList();

	internal bool Busy
	{
		set
		{
			if (webBrowserList != null && !webBrowserList.IsDisposed)
			{
				webBrowserList.Document.InvokeScript("showLoaderAnimation", new object[1] { value });
				webBrowserList.Invalidate();
				webBrowserList.Update();
			}
		}
	}

	internal static string CreateInput(string list, string value, string label, string status, bool selected, bool disabled, string statusIcon = null)
	{
		return string.Format(CultureInfo.InvariantCulture, "<label for='{1}' class='{3}'><input type='radio' name='{0}' id='{1}' value='{1}' {2} {3} />{6}{4} {5}</label><br/>", list, value, selected ? "checked" : "", disabled ? "disabled" : "", label, (!string.IsNullOrEmpty(status)) ? (" - <i>" + status + "</i>") : string.Empty, (statusIcon != null) ? FormattableString.Invariant($"<span class='{statusIcon}'/>") : string.Empty);
	}

	internal static string CreateTextInput(string name, string label, string initialValue, int maxLength, string className)
	{
		return string.Format(CultureInfo.InvariantCulture, "<label for='{0}' >{1}</label><input type='text' name='{0}' id='{0}' value='{2}' maxlength='{3}' class='{4}'/><br/>", name, label, initialValue ?? string.Empty, maxLength, className ?? string.Empty);
	}

	internal static bool SubscribeInputEvents(HtmlElement element, string inputType, string eventName, EventHandler handler, bool autoSelect = true)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		bool flag = false;
		foreach (HtmlElement inputElement in from el in element.GetElementsByTagName("input").OfType<HtmlElement>()
			where el.OuterHtml.IndexOf("type=\"" + inputType + "\"", StringComparison.Ordinal) != -1
			select el)
		{
			inputElement.AttachEventHandler(eventName, delegate(object sender, EventArgs args)
			{
				try
				{
					handler(inputElement, args);
				}
				catch (Exception ex)
				{
					CrashHandler.ReportExceptionAndTerminate(ex);
				}
			});
			if (inputElement.Enabled)
			{
				list.Add(inputElement);
			}
			if (bool.Parse(inputElement.GetAttribute("checked")))
			{
				flag = true;
			}
		}
		if (autoSelect && inputType == "radio" && !flag && list.Count == 1)
		{
			HtmlElement htmlElement = list.First();
			htmlElement.SetAttribute("checked", "True");
			handler(htmlElement, new EventArgs());
			return true;
		}
		return false;
	}

	internal static string CreateRadioList(string title, List<string> listItems)
	{
		return FormattableString.Invariant(FormattableStringFactory.Create("<p><div>{0}</div><div class='radioList'>{1}</div></p>", title, string.Join(" ", listItems)));
	}

	internal static void SetActiveRadioElement(HtmlElement inputElement)
	{
		HtmlElement parent = inputElement.Parent;
		HtmlElement parent2 = parent.Parent;
		foreach (HtmlElement child in parent2.Children)
		{
			string attribute = child.GetAttribute("className");
			child.SetAttribute("className", (child == parent) ? ("active" + attribute) : attribute.Replace("active", string.Empty));
		}
	}

	internal static string GetTitleString(string vehicleIdentity, string engineNumber)
	{
		string text = string.Empty;
		if (!string.IsNullOrEmpty(vehicleIdentity) || !string.IsNullOrEmpty(engineNumber))
		{
			if (!string.IsNullOrEmpty(vehicleIdentity))
			{
				VinInformation vinInfo = VinInformation.GetVinInformation(vehicleIdentity);
				text = string.Join(" - ", new string[3]
				{
					vehicleIdentity,
					engineNumber,
					vinInfo.InformationEntries.Any() ? string.Join(" ", new string[3] { "modelyear", "make", "model" }.Select((string q) => vinInfo.GetInformationValue(q))) : null
				}.Where((string s) => !string.IsNullOrEmpty(s)));
			}
			else
			{
				text = engineNumber;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return Resources.ReprogrammingView_NoTitle;
		}
		return text;
	}

	internal static void SetButtonState(HtmlElement container, string id, bool enabled, bool show)
	{
		HtmlElement htmlElement = container.GetElementsByTagName("button").OfType<HtmlElement>().FirstOrDefault((HtmlElement b) => b.Id == id);
		if (htmlElement != null)
		{
			SetButtonState(htmlElement, enabled, show);
		}
	}

	internal static void SetButtonState(HtmlElement button, bool enabled, bool show)
	{
		button.SetAttribute("className", (enabled ? "button blue" : "button gray") + (show ? " show" : " hide"));
		EnableElement(button, enabled);
	}

	internal static void EnableElement(HtmlElement element, bool enabled)
	{
		element.Document.InvokeScript("enableButton", new object[2] { element.Id, enabled });
	}

	internal static string WriteButton(string id, string text, string color = "blue")
	{
		return string.Format(CultureInfo.InvariantCulture, "<button id='{0}Button' class='button {2}' onClick=\"clickButton('{0}')\">{1}</button>", id, text, color);
	}

	internal static string GetUnitTimeDisplay(UnitInformation unit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		bool flag = (int)unit.Status == 1;
		DateTime dateTime = (flag ? unit.SettingsInformation.Max((SettingsInformation si) => si.Timestamp.Value) : Utility.TimeFromString(unit.Time));
		if (dateTime.Date == DateTime.Now.Date)
		{
			return string.Format(CultureInfo.CurrentCulture, flag ? Resources.ConnectedUnit_FormatUnitDataUpdatedToday : Resources.ConnectedUnit_FormatUnitDataDownloadedToday, dateTime.ToShortTimeString());
		}
		if (dateTime.Date == (DateTime.Now - TimeSpan.FromDays(1.0)).Date)
		{
			return string.Format(CultureInfo.CurrentCulture, flag ? Resources.ConnectedUnit_FormatUnitDataUpdatedYesterday : Resources.ConnectedUnit_FormatUnitDataDownloadedYesterday, dateTime.ToShortTimeString());
		}
		if (dateTime.Date > DateTime.Now - TimeSpan.FromDays(7.0) && dateTime.Date < DateTime.Now)
		{
			return string.Format(CultureInfo.CurrentCulture, flag ? Resources.ConnectedUnit_FormatUnitDataUpdatedOn : Resources.ConnectedUnit_FormatUnitDataDownloadedOn, CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dateTime.DayOfWeek));
		}
		return string.Format(CultureInfo.CurrentCulture, flag ? Resources.ConnectedUnit_FormatUnitDataUpdatedOn : Resources.ConnectedUnit_FormatUnitDataDownloadedOn, dateTime.ToLongDateString());
	}

	private static string CreateStatusHtml(string prefix, IEnumerable<string> serverErrors, IEnumerable<string> deviceErrors, IEnumerable<string> missingDevices, bool hasDevicesForDataSource, bool showErrors)
	{
		string empty = string.Empty;
		IEnumerable<string> enumerable = null;
		string text = string.Format(CultureInfo.CurrentCulture, Resources.GatherServerDataPage_Format_MissingServerDataForDevices, string.Join(", ", missingDevices));
		if (deviceErrors.Any() || serverErrors.Any())
		{
			empty = string.Join(Resources.GatherServerDataPage_ServerStatusSeparator, new string[3]
			{
				serverErrors.Any() ? ServerDataManager.GetServerStatusText(serverErrors.First()) : null,
				deviceErrors.Any() ? string.Format(CultureInfo.CurrentCulture, Resources.GatherServerDataPage_FormatDeviceErrors, deviceErrors.Distinct().Count()) : null,
				missingDevices.Any() ? string.Format(CultureInfo.CurrentCulture, Resources.GatherServerDataPage_FormatMissingDevices, missingDevices.Distinct().Count()) : null
			}.Where((string s) => !string.IsNullOrEmpty(s)));
			enumerable = (missingDevices.Any() ? deviceErrors.Union(Enumerable.Repeat(text, 1)) : deviceErrors).Distinct();
		}
		else
		{
			empty = (missingDevices.Any() ? text : (hasDevicesForDataSource ? Resources.GatherServerDataPage_DataOK : Resources.GatherServerDataPage_DataNotApplicable));
		}
		string text2 = FormattableString.Invariant($"<div class='nowrap'><span>{prefix}</span><span>{empty}</span>");
		if (enumerable != null)
		{
			text2 += FormattableString.Invariant(FormattableStringFactory.Create("<div class='{0}'>{1}</div>", showErrors ? string.Empty : "help", string.Join(" ", enumerable.Select((string ef) => FormattableString.Invariant($"<div class='nowrap indent'>{ef}</div>")))));
		}
		return text2 + "</div>";
	}

	internal static string CreateDownloadedUnitHtml(UnitInformation info, bool expandErrors)
	{
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Invalid comparison between Unknown and I4
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Expected I4, but got Unknown
		IEnumerable<string> enumerable = from deviceInfo in info.DeviceInformation
			where (int)deviceInfo.DataSource == 2
			where GetPartsForDevice(deviceInfo).Any((Part p) => FlashFileRequiresDownload(p))
			select deviceInfo.Device;
		string text = (enumerable.Any() ? string.Format(CultureInfo.CurrentCulture, LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadInProgress ? Resources.Unit_FormatRemoteInProgressStatus : Resources.Unit_FormatRemoteDownloadNeededStatus, string.Join(", ", enumerable)) : string.Empty);
		string text2 = info.VehicleIdentity + "_" + info.EngineNumber;
		string text3 = FormattableString.Invariant($"<button id='download_{text2}' class='unitButton' onClick=\"clickButton('item_download_{text2}')\"><table>");
		text3 += FormattableString.Invariant($"<tr><td><div class='{GetRowClass(info, enumerable.Any())}'/></td><td>");
		text3 += FormattableString.Invariant($"<div><div class='identity'>{GetTitleString(info.VehicleIdentity, info.EngineNumber)}</div>");
		text3 += FormattableString.Invariant($"<div><span>{GetUnitTimeDisplay(info)}</span> <span class='parameter'>{text}</span></div>");
		if ((int)info.Status == 3)
		{
			text3 += FormattableString.Invariant($"<div>{Resources.GatherServerDataPage_UnitStatusExpired}</div>");
		}
		else
		{
			text3 += CreateStatusHtml(Resources.GatherServerDataPage_PowertrainStatus, info.DepotServerErrors.Union(info.DepotServerWarnings), info.DepotDeviceServerErrors.Select((Tuple<string, string> d) => ServerDataManager.GetDeviceStatusText(d.Item1, d.Item2)), from d in info.MissingDevices.Distinct()
				where (int)UnitInformation.GetDataSource(d) == 1
				select d, info.DeviceInformation.Any((DeviceInformation d) => (int)d.DataSource == 1), expandErrors);
			if (ShowEdexInformation)
			{
				text3 += CreateStatusHtml(Resources.GatherServerDataPage_ChassisStatus, info.EdexServerErrors, info.EdexFileServerErrors.Select((string d) => ServerDataManager.GetServerStatusText(d)), from d in info.MissingDevices.Distinct()
					where (int)UnitInformation.GetDataSource(d) == 2
					select d, info.DeviceInformation.Any((DeviceInformation d) => (int)d.DataSource == 2), expandErrors);
			}
		}
		foreach (AutomaticOperation automaticOperation in info.AutomaticOperations)
		{
			string text4 = string.Format(CultureInfo.InvariantCulture, "{0} - {1} - {2} - {3}", automaticOperation.Reason, DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgrammingData.OperationToDisplayString((ProgrammingOperation)automaticOperation.OperationType), automaticOperation.Device, Sapi.TimeFromString(automaticOperation.Time).ToShortDateString());
			string text5 = (automaticOperation.Completed ? Resources.GatherServerDataPage_Status_AutomaticOperationComplete : Resources.GatherServerDataPage_Status_AutomaticOperationRequired);
			text3 += FormattableString.Invariant($"<div class='nowrap indent'>{text4} - <i>{text5}</i></div>");
		}
		text3 += "</td>";
		text3 += FormattableString.Invariant(FormattableStringFactory.Create("<td>{0}</td><td>{1}</td>", WriteButton("refresh_download_" + text2, Resources.Wizard_ButtonRefresh, "midblue"), WriteButton("remove_download_" + text2, Resources.Wizard_ButtonRemove, "midblue")));
		return text3 + "</tr></table></button>";
		static bool FlashFileRequiresDownload(Part part)
		{
			return ServerDataManager.GlobalInstance.FirmwareInformation.OfType<FirmwareInformation>().Any((FirmwareInformation fi) => PartExtensions.IsEqual(part, fi.Key) && fi.RequiresDownload);
		}
		static IEnumerable<Part> GetPartsForDevice(DeviceInformation deviceInfo)
		{
			return deviceInfo.EdexFiles.Where((EdexFileInformation ef) => ef.ConfigurationInformation != null).SelectMany((EdexFileInformation ef) => ef.ConfigurationInformation.AllFlashwarePartNumbers).Distinct();
		}
	}

	internal static string CreateUploadUnitHtml(UnitInformation info, bool hasPendingRequest)
	{
		string text = info.VehicleIdentity + "_" + info.EngineNumber;
		string text2 = FormattableString.Invariant($"<button id='upload_{text}' class='unitButton' onClick=\"clickButton('item_upload_{text}')\"><table>");
		text2 += "<tr><td class='statusUpload'/><td>";
		text2 += FormattableString.Invariant($"<div class='identity'>{GetTitleString(info.VehicleIdentity, info.EngineNumber)}</div>");
		text2 += FormattableString.Invariant($"<div>{GetUnitTimeDisplay(info)}</div>");
		if (info.SettingsInformation.Any((SettingsInformation s) => (int)UnitInformation.GetDataSource(s.Device) == 1))
		{
			text2 += FormattableString.Invariant(FormattableStringFactory.Create("<div>{0}</div>", Resources.GatherServerDataPage_PowertrainStatus + string.Format(CultureInfo.CurrentCulture, hasPendingRequest ? Resources.GatherServerDataPage_FormatUploadPending : Resources.GatherServerDataPage_FormatUploadFor, string.Join(", ", info.SettingsInformation.Select((SettingsInformation si) => si.Device).Distinct()))));
		}
		bool flag = ShowEdexInformation && info.SettingsInformation.Any((SettingsInformation s) => (int)UnitInformation.GetDataSource(s.Device) == 2);
		if (flag)
		{
			text2 += FormattableString.Invariant(FormattableStringFactory.Create("<div>{0}</div>", Resources.GatherServerDataPage_ChassisStatus + string.Format(CultureInfo.CurrentCulture, hasPendingRequest ? Resources.GatherServerDataPage_FormatUploadPending : Resources.GatherServerDataPage_FormatUploadFor, string.Join(", ", info.SettingsInformation.Select((SettingsInformation si) => si.Device).Distinct()))));
		}
		text2 += "</td>";
		text2 += (flag ? FormattableString.Invariant(FormattableStringFactory.Create("<td>{0}</td>", WriteButton("view_upload_" + text, Resources.Wizard_ButtonView, "midblue"))) : "<td/>");
		text2 += FormattableString.Invariant(FormattableStringFactory.Create("<td>{0}</td>", WriteButton("remove_upload_" + text, Resources.Wizard_ButtonRemove, "midblue")));
		return text2 + "</tr></table></button>";
	}

	internal static string CreateUnitToBeDownloadedHtml(string vehicleIdentity, string engineNumber)
	{
		string text = vehicleIdentity + "_" + engineNumber;
		return FormattableString.Invariant($"\r\n                <button id='download_{text}' class='unitButton' onClick=\"clickButton('item_download_{text}')\">\r\n                    <table>\r\n                        <tr>\r\n                            <td class='statusWarning'/>\r\n                            <td>\r\n                                <div class='identity'>{GetTitleString(vehicleIdentity, engineNumber)}</div>\r\n                                <div>{Resources.ConnectedUnit_NoDownload}</div>\r\n                            </td>\r\n                            <td/>\r\n                        </tr>\r\n                    </table>\r\n                </button>\r\n            ");
	}

	private static string GetRowClass(UnitInformation info, bool remoteSoftwareDownloadNeeded)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected I4, but got Unknown
		string result = string.Empty;
		UnitStatus status = info.Status;
		switch (status - 2)
		{
		case 0:
			result = ((!remoteSoftwareDownloadNeeded) ? "statusOk" : (LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadInProgress ? "statusOkRemoteDownloading" : "statusOkRemoteNeeded"));
			break;
		case 1:
			result = "statusError";
			break;
		case 2:
			result = ((info.DeviceInformation.Count == 0) ? "statusError" : "statusWarning");
			break;
		}
		return result;
	}

	internal static string CreateTableSection(string id, string sectionText)
	{
		return FormattableString.Invariant(FormattableStringFactory.Create("\r\n                <button id='{0}' class='unitButton' onClick=\"clickButton('item_{1}')\">\r\n                    <table>\r\n                        <tr>\r\n                            <th/><th><div class='identity'>{2}</div></th>\r\n                            <th>{3}</th><th>{4}</th>\r\n                        </tr>\r\n                    </table>\r\n                </button>\r\n            ", id, id, sectionText, (id == "download") ? WriteButton("refresh_" + id, Resources.Wizard_ButtonRefresh, "midblue") : string.Empty, WriteButton("remove_" + id, Resources.Wizard_ButtonRemove, "midblue")));
	}

	internal static void UpdateTitle(HtmlElement element, UnitInformation unit)
	{
		string text = FormattableString.Invariant($"<table class='grey' style='width:100%'>");
		text += FormattableString.Invariant($"<tr><td>{CreateDownloadedUnitHtml(unit, expandErrors: false)}</td></tr>");
		text += "</td></tr></table>";
		element.InnerHtml = text;
	}

	public ReprogrammingView()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		MenuProxy.GlobalInstance.ReprogrammingView = this;
		InitializeComponent();
		Stream manifestResourceStream = typeof(ProgramDeviceManager).Assembly.GetManifestResourceStream("DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.BlankProgramDeviceList.html");
		using (StreamReader streamReader = new StreamReader(manifestResourceStream))
		{
			webBrowserList.DocumentText = streamReader.ReadToEnd();
		}
		((ContextHelpControl)this).SetLink(LinkSupport.GetViewLink((PanelIdentifier)7));
		SapiManager.GlobalInstance.SapiResetCompleted += GlobalInstance_SapiResetCompleted;
		SapiManager.GlobalInstance.ActiveChannelsListChanged += GlobalInstance_ActiveChannelsListChanged;
		ServerClient.GlobalInstance.InUseChanged += GlobalInstance_InUseChanged;
	}

	private void GlobalInstance_SapiResetCompleted(object sender, EventArgs e)
	{
		CheckForMissingCBFs();
	}

	private void webBrowserList_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
	{
		try
		{
			SetInputPane(webBrowserList.Document.All.GetElementsByName("inputpane")[0]);
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
			if (e.Url.ToString() != "about:blank" || !string.IsNullOrEmpty(e.Url.Fragment))
			{
				e.Cancel = true;
				bool flag = pages.Count > 0 && activePage == pages[1];
				switch (e.Url.Fragment)
				{
				case "#tab_connectedunit":
					switchedFromConnectedUnitView = flag;
					ShowOrHideConnectedUnitPage();
					break;
				case "#tab_allunits":
					switchedFromConnectedUnitView = flag;
					ShowOrHideConnectedUnitPage();
					break;
				case "#button_back":
					MoveBack();
					break;
				case "#button_next":
					MoveNext();
					break;
				default:
					activePage?.Navigate(e.Url.Fragment);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			CrashHandler.ReportExceptionAndTerminate(ex);
		}
	}

	private void WebBrowserListOnPreviewKeyDown(object sender, PreviewKeyDownEventArgs previewKeyDownEventArgs)
	{
		if (previewKeyDownEventArgs.KeyCode == Keys.F5)
		{
			previewKeyDownEventArgs.IsInputKey = true;
		}
	}

	private void SetInputPane(HtmlElement inputPane)
	{
		inputPane.InnerHtml = FormattableString.Invariant($"\r\n                <div>\r\n                    <div id='gatherServerDataPane' class='show'>&nbsp;</div>\r\n                    <div id='wizardPane' class='hide'>\r\n                        <div class='tab'>\r\n                            <button id='connectedUnitTab' class='tablinks' disabled>{Resources.Header_GatherServerData}</button>\r\n                            <button id='selectOperationTab' class='tablinks' disabled>{Resources.Header_SelectOperation}</button>\r\n                            <button id='programDeviceTab' class='tablinks' disabled>{Resources.Header_ProgramDevice}</button>\r\n                            <button id='switchToAllUnitsButton' class='tablinks exit hide' onClick=\"clickTab(event,'allunits', true)\">&#x279c; {Resources.GatherServerDataPage_TabUnitManagement}</button>\r\n                        </div>\r\n                        <div id='connectedUnitPane'>&nbsp;</div>\r\n                        <div id='selectOperationPane' class='hide'>&nbsp;</div>\r\n                        <div id='programDevicePane' class='hide'>&nbsp;</div>\r\n                    </div>\r\n                </div>\r\n            ");
		GatherServerDataPage gatherServerDataPage = null;
		SelectOperationPage selectOperationPage = null;
		ProgramDevicePage programDevicePage = null;
		ConnectedUnitPage connectedUnitPage = null;
		foreach (HtmlElement item in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList())
		{
			switch (item.Id)
			{
			case "wizardPane":
				wizardPane = item;
				break;
			case "gatherServerDataPane":
				gatherServerDataPage = new GatherServerDataPage(this, item);
				break;
			case "connectedUnitPane":
				connectedUnitPage = new ConnectedUnitPage(gatherServerDataPage, this, item);
				break;
			case "selectOperationPane":
				selectOperationPage = new SelectOperationPage(this, item);
				break;
			case "programDevicePane":
				programDevicePage = new ProgramDevicePage(this, item);
				break;
			}
		}
		foreach (HtmlElement item2 in inputPane.GetElementsByTagName("button").OfType<HtmlElement>())
		{
			switch (item2.Id)
			{
			case "switchToConnectedUnitButton":
				switchToConnectedUnitButton = item2;
				break;
			case "switchToAllUnitsButton":
				switchToAllUnitsButton = item2;
				break;
			case "connectedUnitTab":
				connectedUnitPage.Tab = item2;
				break;
			case "selectOperationTab":
				selectOperationPage.Tab = item2;
				break;
			case "programDeviceTab":
				programDevicePage.Tab = item2;
				break;
			}
		}
		pages.Add(gatherServerDataPage);
		pages.Add(connectedUnitPage);
		pages.Add(selectOperationPage);
		pages.Add(programDevicePage);
		if (!advanceToLast)
		{
			SetActivePage(gatherServerDataPage);
			ShowOrHideConnectedUnitPage();
		}
		else
		{
			AdvanceToLastPage();
		}
	}

	private void GlobalInstance_ActiveChannelsListChanged(object sender, EventArgs e)
	{
		if (!ServerClient.GlobalInstance.InUse && webBrowserList != null && !webBrowserList.IsDisposed)
		{
			ShowOrHideConnectedUnitPage();
		}
	}

	private void GlobalInstance_InUseChanged(object sender, EventArgs e)
	{
		if (!ServerClient.GlobalInstance.InUse)
		{
			ShowOrHideConnectedUnitPage();
		}
	}

	private void ShowOrHideConnectedUnitPage()
	{
		if (pages.Count <= 0)
		{
			return;
		}
		if (!switchedFromConnectedUnitView && ConnectedUnitPage.ProgrammableChannels.Any())
		{
			if (activePage == pages[0])
			{
				wizardPane.SetAttribute("className", "show");
				SetActivePage(pages[1]);
			}
			return;
		}
		if (!ConnectedUnitPage.ProgrammableChannels.Any())
		{
			switchedFromConnectedUnitView = false;
		}
		if (activePage != pages.Last())
		{
			wizardPane.SetAttribute("className", "hide");
			SetActivePage(pages[0]);
		}
		switchToConnectedUnitButton.SetAttribute("className", switchedFromConnectedUnitView ? "tablinks exit show" : "tablinks exitdisabled show");
		EnableElement(switchToConnectedUnitButton, switchedFromConnectedUnitView);
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		CheckForMissingCBFs();
		((ScrollableControl)this).OnVisibleChanged(e);
	}

	private void CheckForMissingCBFs()
	{
		if (((Control)this).Visible)
		{
			IEnumerable<CodingFile> source = SapiManager.GlobalInstance.Sapi.CodingFiles.Where((CodingFile f) => !f.Ecus.Any());
			if (source.Any())
			{
				WarningsPanel.GlobalInstance.Add("MissingCBF", MessageBoxIcon.Hand, (string)null, string.Format(CultureInfo.CurrentCulture, Resources.ReprogrammingViewFormat_MissingCBFWarning, string.Join(", ", source.Select((CodingFile cf) => Path.GetFileName(cf.FileName)))), (EventHandler)null);
			}
			else
			{
				WarningsPanel.GlobalInstance.Remove("MissingCBF");
			}
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove("MissingCBF");
		}
	}

	private void SetActivePage(WizardPage page)
	{
		WizardPage wizardPage = activePage;
		activePage = page;
		pages.Except(Enumerable.Repeat(activePage, 1)).ToList().ForEach(delegate(WizardPage p)
		{
			p.Visible = false;
		});
		activePage.Visible = true;
		switchToAllUnitsButton.SetAttribute("className", (activePage == pages[1]) ? "tablinks exit show" : "tablinks exit hide");
		webBrowserList.Document.InvokeScript("alignTabContainers");
		Busy = true;
		wizardPage?.OnSetInactive();
		activePage.OnSetActive();
		Busy = false;
		if (activePage is ConnectedUnitPage)
		{
			MenuProxy.GlobalInstance.CheckAutomaticOperations();
		}
		else if (activePage is ProgramDevicePage)
		{
			WarningsPanel.GlobalInstance.Remove("automaticoperation");
		}
	}

	internal void AdvanceToLastPage()
	{
		if (pages.Count == 0)
		{
			advanceToLast = true;
		}
		else
		{
			SetActivePage(Pages.Last());
		}
	}

	private void MoveBack()
	{
		WizardPage wizardPage = activePage.OnWizardBack();
		if (wizardPage != null)
		{
			SetActivePage(wizardPage);
		}
	}

	private void MoveNext()
	{
		WizardPage wizardPage = activePage.OnWizardNext();
		if (wizardPage != null)
		{
			SetActivePage(wizardPage);
		}
	}

	public void SetWizardButtons(WizardButtons buttons)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (activePage.BackButton != null)
		{
			SetButtonState(activePage.BackButton, (buttons & 1) > 0, show: true);
		}
		if (activePage.NextButton != null)
		{
			SetButtonState(activePage.NextButton, (buttons & 6) > 0, show: true);
			activePage.NextButton.InnerText = (((buttons & 4) != 0) ? Resources.Wizard_ButtonFinish : Resources.Wizard_ButtonNext);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			foreach (WizardPage page in pages)
			{
				if (page is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			pages.Clear();
			SapiManager.GlobalInstance.SapiResetCompleted -= GlobalInstance_SapiResetCompleted;
			SapiManager.GlobalInstance.ActiveChannelsListChanged -= GlobalInstance_ActiveChannelsListChanged;
			ServerClient.GlobalInstance.InUseChanged -= GlobalInstance_InUseChanged;
		}
		((ContextHelpControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ReprogrammingView));
		webBrowserList = new WebBrowser();
		((Control)this).SuspendLayout();
		webBrowserList.AllowWebBrowserDrop = false;
		webBrowserList.IsWebBrowserContextMenuEnabled = false;
		componentResourceManager.ApplyResources(webBrowserList, "webBrowserList");
		webBrowserList.Name = "webBrowserList";
		webBrowserList.DocumentCompleted += webBrowserList_DocumentCompleted;
		webBrowserList.Navigating += webBrowserList_Navigating;
		webBrowserList.PreviewKeyDown += WebBrowserListOnPreviewKeyDown;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add(webBrowserList);
		((Control)this).Name = "ReprogrammingView";
		((Control)this).ResumeLayout(performLayout: false);
	}
}
