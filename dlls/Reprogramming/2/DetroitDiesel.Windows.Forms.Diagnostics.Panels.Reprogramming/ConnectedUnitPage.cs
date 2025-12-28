using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using mshtml;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal class ConnectedUnitPage : WizardPage
{
	private IProvideProgrammingData dataProvider;

	private string currentVehicleIdentification;

	private string currentEngineSerial;

	private string selectedVehicleIdentification;

	private string selectedEngineSerial;

	private string manualVehicleIdentification = string.Empty;

	private string manualEngineSerial = string.Empty;

	private bool manualEntry;

	private HtmlElement inputPane;

	private HtmlElement titlePane;

	private HtmlElement warningPane;

	private HtmlElement unitSelectionPane;

	private HtmlElement buttonPanel;

	private HtmlElement manualInputPane;

	private GatherServerDataPage parent;

	private bool dirty;

	internal static IEnumerable<Channel> ProgrammableChannels => from channel in SapiManager.GlobalInstance.ActiveChannels
		let channelEcu = GetProgrammableEcu(channel)
		where (SapiExtensions.IsDataSourceDepot(channelEcu) || SapiExtensions.IsDataSourceEdex(channelEcu)) && !SapiManager.GlobalInstance.IsNotProgrammableForConnectedEquipment(channelEcu)
		select channel;

	public ConnectedUnitPage(GatherServerDataPage parent, ReprogrammingView wizard, HtmlElement element)
		: base(wizard, element)
	{
		this.parent = parent;
		dataProvider = wizard;
		SetInputPane(element);
		SapiManager.GlobalInstance.ChannelIdentificationChanged += GlobalInstance_ChannelIdentificationChanged;
		SapiManager.GlobalInstance.LogFileChannelsChanged += GlobalInstance_LogFileChannelsChanged;
		ServerClient.GlobalInstance.InUseChanged += GlobalInstance_InUseChanged;
		LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadStatusChanged += GlobalInstance_RemoteFileDownloadStatusChanged;
	}

	private void GlobalInstance_RemoteFileDownloadStatusChanged(object sender, EventArgs e)
	{
		if (base.Wizard.ActivePage == this)
		{
			UpdateTitle();
		}
	}

	internal void SetInputPane(HtmlElement inputPane)
	{
		this.inputPane = inputPane;
		this.inputPane.InnerHtml = FormattableString.Invariant($"\r\n                    <div>\r\n                        <div id='connectedUnit_titlePane'>&nbsp;</div>\r\n                        <div id='connectedUnit_warningPane'>&nbsp;</div>\r\n                        <div id='connectedUnit_unitSelectionPane'>&nbsp;</div>\r\n                        <br/><br/><br/><br/>\r\n                        <div id='connectedUnit_buttonPanel' class='fixedbottom'>\r\n                            &nbsp;\r\n                        </div>\r\n                    </div>\r\n            ");
		foreach (HtmlElement item in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList())
		{
			switch (item.Id)
			{
			case "connectedUnit_titlePane":
				titlePane = item;
				break;
			case "connectedUnit_warningPane":
				warningPane = item;
				break;
			case "connectedUnit_unitSelectionPane":
				unitSelectionPane = item;
				break;
			case "connectedUnit_buttonPanel":
				buttonPanel = item;
				break;
			}
		}
		WriteConnectedUnitPage();
	}

	private void GlobalInstance_ChannelIdentificationChanged(object sender, ChannelIdentificationChangedEventArgs e)
	{
		if (!ServerClient.GlobalInstance.InUse && base.Wizard.ActivePage == this)
		{
			WriteConnectedUnitPage();
		}
	}

	private void GlobalInstance_LogFileChannelsChanged(object sender, EventArgs e)
	{
		if (!ServerClient.GlobalInstance.InUse && base.Wizard.ActivePage == this)
		{
			WriteConnectedUnitPage();
		}
	}

	private void GlobalInstance_InUseChanged(object sender, EventArgs e)
	{
		if (!ServerClient.GlobalInstance.InUse && base.Wizard.ActivePage == this)
		{
			UpdateTitle();
			UpdateIdentificationWarning();
			UpdateUnitSelection();
			UpdateButtonPanel();
		}
	}

	internal override void OnSetActive()
	{
		WriteConnectedUnitPage();
	}

	private void UpdateConnectedIdentification()
	{
		if (SapiManager.GlobalInstance.ActiveChannels.Count > 1)
		{
			currentVehicleIdentification = SapiManager.GlobalInstance.CurrentVehicleIdentification;
			currentEngineSerial = SapiManager.GlobalInstance.CurrentEngineSerialNumber;
			if (selectedVehicleIdentification != null && !SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.Contains(selectedVehicleIdentification))
			{
				selectedVehicleIdentification = (selectedEngineSerial = null);
			}
			return;
		}
		Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault();
		if (channel != null)
		{
			ProgrammingData programmingData = new ProgrammingData(channel);
			currentEngineSerial = programmingData.EngineSerialNumber;
			currentVehicleIdentification = programmingData.VehicleIdentificationNumber;
		}
		else
		{
			selectedEngineSerial = (selectedVehicleIdentification = (currentEngineSerial = (currentVehicleIdentification = null)));
			manualEngineSerial = (manualVehicleIdentification = string.Empty);
			manualEntry = false;
		}
	}

	internal override void Navigate(string urlFragment)
	{
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		if (!(urlFragment == "#button_remove"))
		{
			if (urlFragment == "#button_download")
			{
				List<UnitInformation> requests = new List<UnitInformation>();
				IEnumerable<EcuHardwarePart> enumerable = AddUnitDialog.ConnectedHardwareParts.Where((AddUnitEcuHardwarePartNumber p) => !SapiManager.ProgramDeviceUsesSoftwareIdentification(p.EcuName)).Select((Func<AddUnitEcuHardwarePartNumber, EcuHardwarePart>)((AddUnitEcuHardwarePartNumber p) => new EcuHardwarePart(p.EcuName, p.EcuPartNumber)));
				IEnumerable<EcuSoftwareIdentification> enumerable2 = AddUnitDialog.ConnectedHardwareParts.Where((AddUnitEcuHardwarePartNumber p) => SapiManager.ProgramDeviceUsesSoftwareIdentification(p.EcuName)).Select((Func<AddUnitEcuHardwarePartNumber, EcuSoftwareIdentification>)((AddUnitEcuHardwarePartNumber p) => new EcuSoftwareIdentification(p.EcuName, p.EcuPartNumberDisplayValue)));
				UnitInformation selectedOrCurrentUnit = GetSelectedOrCurrentUnit();
				if (selectedOrCurrentUnit != null)
				{
					selectedOrCurrentUnit.AddConnectedUnitHardwareForRequest(enumerable, enumerable2);
					parent.RefreshList(Enumerable.Repeat<UnitInformation>(selectedOrCurrentUnit, 1));
					requests.Add(selectedOrCurrentUnit);
				}
				else
				{
					bool flag = selectedVehicleIdentification != null || selectedEngineSerial != null;
					string text = (flag ? selectedEngineSerial : currentEngineSerial);
					string text2 = (flag ? selectedVehicleIdentification : currentVehicleIdentification);
					if (!string.IsNullOrEmpty(text))
					{
						requests.Add(new UnitInformation(text, string.Empty, enumerable, enumerable2, (UnitStatus)0));
					}
					if (!string.IsNullOrEmpty(text2))
					{
						requests.Add(new UnitInformation(string.Empty, text2, enumerable, enumerable2, (UnitStatus)0));
					}
					requests.ForEach(delegate(UnitInformation r)
					{
						parent.AddToRequestPendingGroup(r);
					});
				}
				GatherServerDataPage.Connect(requests, parent.UploadUnits.Where((UnitInformation uu) => !requests.Any((UnitInformation r) => r.IsSameIdentification(uu.EngineNumber, uu.VehicleIdentity))));
			}
		}
		else
		{
			UnitInformation selectedOrCurrentUnit2 = GetSelectedOrCurrentUnit();
			parent.RemoveServerItem(selectedOrCurrentUnit2);
		}
		UpdateUnitSelection();
		UpdateButtonPanel();
	}

	private static string CreateIdentityKey(string vehicleIdentification, string engineSerial)
	{
		return UnitInformation.CreateIdentityKey(vehicleIdentification, engineSerial, (IdentityTypes)(((!string.IsNullOrEmpty(vehicleIdentification)) ? 2 : 0) | ((!string.IsNullOrEmpty(engineSerial)) ? 1 : 0)));
	}

	private static UnitInformation GetUnit(IEnumerable<UnitInformation> units, string vehicleIdentity, string engineNumber)
	{
		List<UnitInformation> source = units.Where((UnitInformation u) => u.IsSameIdentification(string.Empty, vehicleIdentity) || u.IsSameIdentification(engineNumber, string.Empty)).ToList();
		UnitInformation val = source.FirstOrDefault((UnitInformation u) => !string.IsNullOrEmpty(vehicleIdentity) && !string.IsNullOrEmpty(engineNumber) && u.IsSameIdentification(engineNumber, vehicleIdentity) && (int)u.IdentityKeyContent == 3);
		if (val != null)
		{
			return val;
		}
		return source.OrderByDescending(delegate(UnitInformation u)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if ((int)u.Status != 0)
			{
				return (u.DeviceInformation.Any((DeviceInformation d) => (int)d.DataSource == 2) ? 1 : 0) + (u.DeviceInformation.Any((DeviceInformation d) => (int)d.DataSource == 1) ? 1 : 0);
			}
			return ((u.IdentityKeyContent & 2) != 0) ? 1 : 0;
		}).FirstOrDefault();
	}

	internal UnitInformation GetSelectedOrCurrentUnit()
	{
		bool flag = selectedVehicleIdentification != null || selectedEngineSerial != null;
		UnitInformation unit = GetUnit(ServerDataManager.GlobalInstance.UnitInformation, flag ? selectedVehicleIdentification : currentVehicleIdentification, flag ? selectedEngineSerial : currentEngineSerial);
		if (unit == null)
		{
			unit = GetUnit(parent.PendingRequests, flag ? selectedVehicleIdentification : currentVehicleIdentification, flag ? selectedEngineSerial : currentEngineSerial);
		}
		return unit;
	}

	private void WriteConnectedUnitPage()
	{
		if (!dirty)
		{
			dirty = true;
			((Control)(object)base.Wizard).BeginInvoke((Delegate)(Action)delegate
			{
				dirty = false;
				WriteConnectedUnitPageDeferred();
			});
		}
	}

	private void WriteConnectedUnitPageDeferred()
	{
		UpdateConnectedIdentification();
		UpdateTitle();
		UpdateIdentificationWarning();
		UpdateUnitSelection();
		UpdateButtonPanel();
	}

	private void UpdateTitle()
	{
		if (titlePane != null)
		{
			UnitInformation selectedOrCurrentUnit = GetSelectedOrCurrentUnit();
			string vehicleIdentity = ((selectedOrCurrentUnit != null) ? selectedOrCurrentUnit.VehicleIdentity : (selectedVehicleIdentification ?? currentVehicleIdentification));
			string engineNumber = ((selectedOrCurrentUnit != null) ? selectedOrCurrentUnit.EngineNumber : (selectedEngineSerial ?? currentEngineSerial));
			string text = FormattableString.Invariant($"<table class='grey' style='width:100%'>");
			if (selectedOrCurrentUnit != null && ServerDataManager.GlobalInstance.UnitInformation.Contains(selectedOrCurrentUnit))
			{
				text += FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", ReprogrammingView.CreateTableSection("download", Resources.ConnectedUnit_DownloadedUnitData)));
				text += FormattableString.Invariant($"<tr><td>{ReprogrammingView.CreateDownloadedUnitHtml(selectedOrCurrentUnit, expandErrors: true)}</td></tr>");
			}
			else
			{
				text += FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", ReprogrammingView.CreateTableSection("download", Resources.GatherServerDataPage_TabConnectedUnit)));
				text += FormattableString.Invariant($"<tr><td>{ReprogrammingView.CreateUnitToBeDownloadedHtml(vehicleIdentity, engineNumber)}</td></tr>");
			}
			text += "</table>";
			titlePane.InnerHtml = text;
		}
	}

	private static bool IsValidIdentification(string vinOrPin)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		return (int)VehicleIdentification.GetClassification(vinOrPin) == ((!SapiManager.GlobalInstance.IsOffHighwayConnection) ? 1 : 2);
	}

	private static Ecu GetProgrammableEcu(Channel channel)
	{
		Ecu result;
		if (!channel.IsRollCall || (result = SapiExtensions.GetSuppressedOfflineRelatedEcu(channel)) == null)
		{
			result = channel.Ecu;
		}
		return result;
	}

	private void UpdateIdentificationWarning()
	{
		List<string> list = new List<string>();
		List<Channel> source = ProgrammableChannels.ToList();
		int num = source.Where((Channel c) => SapiManager.GetVehicleIdentificationNumber(c) == SapiManager.GlobalInstance.CurrentVehicleIdentification).Count();
		if (SapiManager.GlobalInstance.VehicleIdentificationInconsistency || SapiManager.GlobalInstance.EngineSerialInconsistency || num < 2 || !IsValidIdentification(currentVehicleIdentification))
		{
			string item = (SapiManager.GlobalInstance.VehicleIdentificationInconsistency ? ((num >= 2 && SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.Count() <= 1) ? string.Format(CultureInfo.CurrentCulture, SapiManager.GlobalInstance.EngineSerialInconsistency ? Resources.ConnectedUnit_FormatVehicleIdentificationAndEngineSerialNumberInconsistency : Resources.ConnectedUnit_FormatVehicleIdentificationInconsistency, string.Join(", ", from c in source
				where SapiManager.GetVehicleIdentificationNumber(c) != (selectedVehicleIdentification ?? currentVehicleIdentification)
				select c.Ecu.Name)) : Resources.ConnectedUnit_VehicleIdentificationInconsitencyNotEnoughDevicesForCorrectIdentification) : (SapiManager.GlobalInstance.EngineSerialInconsistency ? Resources.ConnectedUnit_EngineSerialNumberInconsistency : ((num >= 2) ? Resources.ConnectedUnit_VehicleIdentificationInvalid : Resources.ConnectedUnit_NotEnoughDevicesForCorrectIdentification)));
			list.Add(item);
		}
		UnitInformation selectedOrCurrentUnit = GetSelectedOrCurrentUnit();
		if (selectedOrCurrentUnit != null && ServerDataManager.GlobalInstance.UnitInformation.Contains(selectedOrCurrentUnit) && selectedOrCurrentUnit.VehicleIdentity == SapiManager.GlobalInstance.CurrentVehicleIdentification && !string.IsNullOrEmpty(SapiManager.GlobalInstance.CurrentEngineSerialNumber) && selectedOrCurrentUnit.EngineNumber != SapiManager.GlobalInstance.CurrentEngineSerialNumber)
		{
			list.Add(Resources.ConnectedUnit_EngineSerialNumberInconsistencyWithVehicleIdentification);
		}
		if (list.Count > 0)
		{
			warningPane.InnerHtml = string.Join(" ", list.Select((string warning) => FormattableString.Invariant($"<p><span class='warninginline'>{warning}</span></p>")));
			warningPane.SetAttribute("className", "show");
		}
		else
		{
			warningPane.InnerHtml = "&nbsp;";
			warningPane.SetAttribute("className", "hide");
		}
	}

	private static bool UnitCanBeProgrammed(UnitInformation unit, IEnumerable<Channel> programmableChannels)
	{
		bool result = false;
		foreach (Channel programmableChannel in programmableChannels)
		{
			if (unit.GetInformationForDevice(GetProgrammableEcu(programmableChannel).Name) != null)
			{
				result = true;
			}
			else if (SapiManager.GlobalInstance.Sapi.Ecus.Where((Ecu e) => e.Identifier == programmableChannel.Identifier).Any((Ecu e) => unit.GetInformationForDevice(e.Name) != null))
			{
				return false;
			}
		}
		return result;
	}

	private void UpdateUnitSelection()
	{
		if (!(unitSelectionPane != null))
		{
			return;
		}
		List<Channel> programmableChannels = ProgrammableChannels.ToList();
		int num = programmableChannels.Where((Channel c) => SapiManager.GetVehicleIdentificationNumber(c) == SapiManager.GlobalInstance.CurrentVehicleIdentification).Count();
		if (!ServerClient.GlobalInstance.InUse && programmableChannels.Any() && (SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.Count() > 1 || SapiManager.GlobalInstance.EngineSerialInconsistency || num < 2))
		{
			string text = FormattableString.Invariant($"<div>{Resources.ConnectedUnit_VerifyUnitIdentity}</div>");
			UnitInformation currentUnit = GetUnit(ServerDataManager.GlobalInstance.UnitInformation, currentVehicleIdentification, currentEngineSerial);
			if (currentUnit != null)
			{
				text += ReprogrammingView.CreateInput("unitList", currentUnit.VehicleIdentity + "-" + currentUnit.EngineNumber, ReprogrammingView.GetTitleString(currentUnit.VehicleIdentity, currentUnit.EngineNumber), Resources.ConnectedUnit_PrimaryConnectedIdentityDescription + " - " + ReprogrammingView.GetUnitTimeDisplay(currentUnit), selected: true, disabled: false);
			}
			else if (!string.IsNullOrEmpty(currentEngineSerial) || IsValidIdentification(currentVehicleIdentification))
			{
				text += ReprogrammingView.CreateInput("unitList", currentVehicleIdentification + "-" + currentEngineSerial, ReprogrammingView.GetTitleString(currentVehicleIdentification, currentEngineSerial), Resources.ConnectedUnit_PrimaryConnectedIdentityDescription, selected: true, disabled: false);
			}
			if (num < 2 || SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.All((string vin) => !IsValidIdentification(vin)))
			{
				string text2 = CreateIdentityKey(selectedVehicleIdentification, selectedEngineSerial);
				foreach (UnitInformation item in ServerDataManager.GlobalInstance.UnitInformation.Where((UnitInformation u) => u != currentUnit && UnitCanBeProgrammed(u, programmableChannels)))
				{
					text += ReprogrammingView.CreateInput("unitList", item.VehicleIdentity + "-" + item.EngineNumber, ReprogrammingView.GetTitleString(item.VehicleIdentity, item.EngineNumber), ReprogrammingView.GetUnitTimeDisplay(item), item.IdentityKey == text2, disabled: false);
				}
				text += ReprogrammingView.CreateInput("unitList", "manual", Resources.ConnectedUnit_EnterIdentity, Resources.ConnectedUnit_EnterIdentityDescription, manualEntry, disabled: false);
				text += FormattableString.Invariant(FormattableStringFactory.Create("\r\n                            <div name='manual' class='{0}'>\r\n                                <div class='indent'>{1}</div>\r\n                                <div class='indent'>{2}</div>\r\n                                <div id='esnonlywarning' class='indent hide'><span class='warninginline'>{3}</span></div>\r\n                            </div>\r\n                        ", manualEntry ? "show" : "hide", ReprogrammingView.CreateTextInput("vin", Resources.ProgrammingDataItem_VehicleIdentificationNumber, manualVehicleIdentification, 17, string.IsNullOrEmpty(manualVehicleIdentification) ? null : (IsValidIdentification(manualVehicleIdentification) ? "valid" : "invalid")), ReprogrammingView.CreateTextInput("esn", Resources.ProgrammingDataItem_EngineSerialNumber, manualEngineSerial, 14, null), Resources.ConnectedUnit_VinIsNeededToContinueDataDownload));
			}
			else
			{
				foreach (string item2 in SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.Where((string pi) => pi != currentVehicleIdentification))
				{
					UnitInformation unit = GetUnit(ServerDataManager.GlobalInstance.UnitInformation, item2, string.Empty);
					text = ((unit == null) ? (text + ReprogrammingView.CreateInput("unitList", item2, ReprogrammingView.GetTitleString(item2, null), Resources.ConnectedUnit_OtherConnectedIdentityDescription, selectedVehicleIdentification == item2, disabled: false)) : (text + ReprogrammingView.CreateInput("unitList", unit.VehicleIdentity + "-" + unit.EngineNumber, ReprogrammingView.GetTitleString(unit.VehicleIdentity, unit.EngineNumber), Resources.ConnectedUnit_OtherConnectedIdentityDescription + " - " + ReprogrammingView.GetUnitTimeDisplay(unit), selectedVehicleIdentification == unit.VehicleIdentity, disabled: false)));
				}
			}
			unitSelectionPane.InnerHtml = text;
			unitSelectionPane.SetAttribute("className", "show");
			manualInputPane = unitSelectionPane.GetElementsByTagName("div").GetElementsByName("manual").OfType<HtmlElement>()
				.FirstOrDefault();
			ReprogrammingView.SubscribeInputEvents(inputPane, "radio", "onchange", connectedUnitList_onChange, autoSelect: false);
			ReprogrammingView.SubscribeInputEvents(inputPane, "text", "oninput", textArea_onInput);
		}
		else
		{
			unitSelectionPane.InnerHtml = "&nbsp;";
			unitSelectionPane.SetAttribute("className", "hide");
			manualInputPane = null;
		}
	}

	private void UpdateButtonPanel()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		if (!ServerClient.GlobalInstance.InUse)
		{
			UnitInformation selectedOrCurrentUnit = GetSelectedOrCurrentUnit();
			if (selectedOrCurrentUnit != null && ServerDataManager.GlobalInstance.UnitInformation.Contains(selectedOrCurrentUnit))
			{
				string text = string.Empty;
				if ((int)selectedOrCurrentUnit.Status == 2 || ((int)selectedOrCurrentUnit.Status == 4 && selectedOrCurrentUnit.DeviceInformation.Count > 0))
				{
					text = text + ReprogrammingView.WriteButton("next", Resources.Wizard_ButtonNext) + ReprogrammingView.WriteButton("download", Resources.Wizard_ButtonRefreshData);
				}
				else
				{
					if ((int)selectedOrCurrentUnit.Status == 4)
					{
						text += ReprogrammingView.WriteButton("remove", Resources.Wizard_ButtonRemoveData);
					}
					text += ReprogrammingView.WriteButton("download", Resources.Wizard_ButtonRefreshData);
				}
				buttonPanel.InnerHtml = text;
			}
			else
			{
				buttonPanel.InnerHtml = ReprogrammingView.WriteButton("download", Resources.Wizard_ButtonDownloadData);
				if (string.IsNullOrEmpty(selectedEngineSerial ?? currentEngineSerial) && !IsValidIdentification(selectedVehicleIdentification ?? currentVehicleIdentification))
				{
					ReprogrammingView.SetButtonState(buttonPanel.GetElementsByTagName("button")[0], enabled: false, show: true);
				}
			}
		}
		else
		{
			buttonPanel.InnerHtml = "&nbsp;";
		}
	}

	private void connectedUnitList_onChange(object sender, EventArgs e)
	{
		HtmlElement htmlElement = sender as HtmlElement;
		string id = htmlElement.Id;
		if (id == "manual")
		{
			manualInputPane.SetAttribute("className", "show");
			if (string.IsNullOrEmpty(manualEngineSerial) && string.IsNullOrEmpty(manualVehicleIdentification))
			{
				manualVehicleIdentification = currentVehicleIdentification;
				manualEngineSerial = currentEngineSerial;
				foreach (HtmlElement item in manualInputPane.GetElementsByTagName("input"))
				{
					id = item.Name;
					if (!(id == "vin"))
					{
						if (id == "esn" && !string.IsNullOrEmpty(manualEngineSerial))
						{
							(item.DomElement as IHTMLInputTextElement).value = manualEngineSerial;
						}
					}
					else if (!string.IsNullOrEmpty(manualVehicleIdentification))
					{
						(item.DomElement as IHTMLInputTextElement).value = manualVehicleIdentification;
						item.SetAttribute("className", IsValidIdentification(manualVehicleIdentification) ? "valid" : "invalid");
					}
				}
			}
			selectedEngineSerial = manualEngineSerial;
			selectedVehicleIdentification = manualVehicleIdentification;
			manualEntry = true;
		}
		else
		{
			manualInputPane?.SetAttribute("className", "hide");
			string[] array = (htmlElement.DomElement as IHTMLInputElement).value.Split("-".ToCharArray());
			selectedVehicleIdentification = array[0];
			selectedEngineSerial = ((array.Length > 1) ? array[1] : string.Empty);
			manualEntry = false;
		}
		UpdateTitle();
		UpdateIdentificationWarning();
		UpdateButtonPanel();
	}

	private void textArea_onInput(object sender, EventArgs e)
	{
		HtmlElement htmlElement = sender as HtmlElement;
		string text = string.Empty;
		try
		{
			text = (htmlElement.DomElement as IHTMLInputTextElement).value.ToUpperInvariant();
		}
		catch (NullReferenceException)
		{
		}
		(htmlElement.DomElement as IHTMLInputTextElement).value = text;
		string id = htmlElement.Id;
		if (!(id == "vin"))
		{
			if (id == "esn")
			{
				selectedEngineSerial = (manualEngineSerial = text);
			}
		}
		else
		{
			selectedVehicleIdentification = (manualVehicleIdentification = text);
			htmlElement.SetAttribute("className", IsValidIdentification(manualVehicleIdentification) ? "valid" : "invalid");
			HtmlElement htmlElement2 = manualInputPane.GetElementsByTagName("div").OfType<HtmlElement>().FirstOrDefault((HtmlElement el) => el.Id == "esnonlywarning");
			htmlElement2.SetAttribute("className", "indent " + ((text.Length == 0) ? "show" : "hide"));
		}
		UpdateTitle();
		UpdateIdentificationWarning();
		UpdateButtonPanel();
	}

	internal override WizardPage OnWizardNext()
	{
		try
		{
			ProgrammingData programmingData = ProgrammingData.CreateFromAutomaticOperationForConnectedUnit();
			if (programmingData != null)
			{
				dataProvider.SelectedUnit = programmingData.Unit;
				dataProvider.ProgrammingData = Enumerable.Repeat(programmingData, 1);
				return base.Wizard.Pages.Last();
			}
		}
		catch (DataException ex)
		{
			ControlHelpers.ShowMessageBox(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			return null;
		}
		dataProvider.SelectedUnit = GetSelectedOrCurrentUnit();
		return base.OnWizardNext();
	}
}
