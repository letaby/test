// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ConnectedUnitPage
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using mshtml;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
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

  public ConnectedUnitPage(
    GatherServerDataPage parent,
    ReprogrammingView wizard,
    HtmlElement element)
    : base(wizard, element)
  {
    this.parent = parent;
    this.dataProvider = (IProvideProgrammingData) wizard;
    this.SetInputPane(element);
    SapiManager.GlobalInstance.ChannelIdentificationChanged += new EventHandler<ChannelIdentificationChangedEventArgs>(this.GlobalInstance_ChannelIdentificationChanged);
    SapiManager.GlobalInstance.LogFileChannelsChanged += new EventHandler(this.GlobalInstance_LogFileChannelsChanged);
    ServerClient.GlobalInstance.InUseChanged += new EventHandler(this.GlobalInstance_InUseChanged);
    LargeFileDownloadManager.GlobalInstance.RemoteFileDownloadStatusChanged += new EventHandler(this.GlobalInstance_RemoteFileDownloadStatusChanged);
  }

  private void GlobalInstance_RemoteFileDownloadStatusChanged(object sender, EventArgs e)
  {
    if (this.Wizard.ActivePage != this)
      return;
    this.UpdateTitle();
  }

  internal void SetInputPane(HtmlElement inputPane)
  {
    this.inputPane = inputPane;
    this.inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("\r\n                    <div>\r\n                        <div id='connectedUnit_titlePane'>&nbsp;</div>\r\n                        <div id='connectedUnit_warningPane'>&nbsp;</div>\r\n                        <div id='connectedUnit_unitSelectionPane'>&nbsp;</div>\r\n                        <br/><br/><br/><br/>\r\n                        <div id='connectedUnit_buttonPanel' class='fixedbottom'>\r\n                            &nbsp;\r\n                        </div>\r\n                    </div>\r\n            "));
    foreach (HtmlElement htmlElement in inputPane.GetElementsByTagName("div").OfType<HtmlElement>().ToList<HtmlElement>())
    {
      switch (htmlElement.Id)
      {
        case "connectedUnit_titlePane":
          this.titlePane = htmlElement;
          continue;
        case "connectedUnit_warningPane":
          this.warningPane = htmlElement;
          continue;
        case "connectedUnit_unitSelectionPane":
          this.unitSelectionPane = htmlElement;
          continue;
        case "connectedUnit_buttonPanel":
          this.buttonPanel = htmlElement;
          continue;
        default:
          continue;
      }
    }
    this.WriteConnectedUnitPage();
  }

  private void GlobalInstance_ChannelIdentificationChanged(
    object sender,
    ChannelIdentificationChangedEventArgs e)
  {
    if (ServerClient.GlobalInstance.InUse || this.Wizard.ActivePage != this)
      return;
    this.WriteConnectedUnitPage();
  }

  private void GlobalInstance_LogFileChannelsChanged(object sender, EventArgs e)
  {
    if (ServerClient.GlobalInstance.InUse || this.Wizard.ActivePage != this)
      return;
    this.WriteConnectedUnitPage();
  }

  private void GlobalInstance_InUseChanged(object sender, EventArgs e)
  {
    if (ServerClient.GlobalInstance.InUse || this.Wizard.ActivePage != this)
      return;
    this.UpdateTitle();
    this.UpdateIdentificationWarning();
    this.UpdateUnitSelection();
    this.UpdateButtonPanel();
  }

  internal override void OnSetActive() => this.WriteConnectedUnitPage();

  private void UpdateConnectedIdentification()
  {
    if (SapiManager.GlobalInstance.ActiveChannels.Count > 1)
    {
      this.currentVehicleIdentification = SapiManager.GlobalInstance.CurrentVehicleIdentification;
      this.currentEngineSerial = SapiManager.GlobalInstance.CurrentEngineSerialNumber;
      if (this.selectedVehicleIdentification == null || SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.Contains<string>(this.selectedVehicleIdentification))
        return;
      this.selectedVehicleIdentification = this.selectedEngineSerial = (string) null;
    }
    else
    {
      Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>();
      if (channel != null)
      {
        ProgrammingData programmingData = new ProgrammingData(channel);
        this.currentEngineSerial = programmingData.EngineSerialNumber;
        this.currentVehicleIdentification = programmingData.VehicleIdentificationNumber;
      }
      else
      {
        this.selectedEngineSerial = this.selectedVehicleIdentification = this.currentEngineSerial = this.currentVehicleIdentification = (string) null;
        this.manualEngineSerial = this.manualVehicleIdentification = string.Empty;
        this.manualEntry = false;
      }
    }
  }

  internal override void Navigate(string urlFragment)
  {
    switch (urlFragment)
    {
      case "#button_remove":
        this.parent.RemoveServerItem(this.GetSelectedOrCurrentUnit());
        break;
      case "#button_download":
        List<UnitInformation> requests = new List<UnitInformation>();
        IEnumerable<EcuHardwarePart> ecuHardwareParts = AddUnitDialog.ConnectedHardwareParts.Where<AddUnitEcuHardwarePartNumber>((System.Func<AddUnitEcuHardwarePartNumber, bool>) (p => !SapiManager.ProgramDeviceUsesSoftwareIdentification(p.EcuName))).Select<AddUnitEcuHardwarePartNumber, EcuHardwarePart>((System.Func<AddUnitEcuHardwarePartNumber, EcuHardwarePart>) (p => new EcuHardwarePart(p.EcuName, p.EcuPartNumber)));
        IEnumerable<EcuSoftwareIdentification> softwareIdentifications = AddUnitDialog.ConnectedHardwareParts.Where<AddUnitEcuHardwarePartNumber>((System.Func<AddUnitEcuHardwarePartNumber, bool>) (p => SapiManager.ProgramDeviceUsesSoftwareIdentification(p.EcuName))).Select<AddUnitEcuHardwarePartNumber, EcuSoftwareIdentification>((System.Func<AddUnitEcuHardwarePartNumber, EcuSoftwareIdentification>) (p => new EcuSoftwareIdentification(p.EcuName, p.EcuPartNumberDisplayValue)));
        UnitInformation selectedOrCurrentUnit = this.GetSelectedOrCurrentUnit();
        if (selectedOrCurrentUnit != null)
        {
          selectedOrCurrentUnit.AddConnectedUnitHardwareForRequest(ecuHardwareParts, softwareIdentifications);
          this.parent.RefreshList(Enumerable.Repeat<UnitInformation>(selectedOrCurrentUnit, 1));
          requests.Add(selectedOrCurrentUnit);
        }
        else
        {
          bool flag = this.selectedVehicleIdentification != null || this.selectedEngineSerial != null;
          string str1 = flag ? this.selectedEngineSerial : this.currentEngineSerial;
          string str2 = flag ? this.selectedVehicleIdentification : this.currentVehicleIdentification;
          if (!string.IsNullOrEmpty(str1))
            requests.Add(new UnitInformation(str1, string.Empty, ecuHardwareParts, softwareIdentifications, (UnitInformation.UnitStatus) 0));
          if (!string.IsNullOrEmpty(str2))
            requests.Add(new UnitInformation(string.Empty, str2, ecuHardwareParts, softwareIdentifications, (UnitInformation.UnitStatus) 0));
          requests.ForEach((Action<UnitInformation>) (r => this.parent.AddToRequestPendingGroup(r)));
        }
        GatherServerDataPage.Connect((IEnumerable<UnitInformation>) requests, this.parent.UploadUnits.Where<UnitInformation>((System.Func<UnitInformation, bool>) (uu => !requests.Any<UnitInformation>((System.Func<UnitInformation, bool>) (r => r.IsSameIdentification(uu.EngineNumber, uu.VehicleIdentity))))));
        break;
    }
    this.UpdateUnitSelection();
    this.UpdateButtonPanel();
  }

  private static string CreateIdentityKey(string vehicleIdentification, string engineSerial)
  {
    return UnitInformation.CreateIdentityKey(vehicleIdentification, engineSerial, (UnitInformation.IdentityTypes) ((!string.IsNullOrEmpty(vehicleIdentification) ? 2 : 0) | (!string.IsNullOrEmpty(engineSerial) ? 1 : 0)));
  }

  private static UnitInformation GetUnit(
    IEnumerable<UnitInformation> units,
    string vehicleIdentity,
    string engineNumber)
  {
    List<UnitInformation> list = units.Where<UnitInformation>((System.Func<UnitInformation, bool>) (u => u.IsSameIdentification(string.Empty, vehicleIdentity) || u.IsSameIdentification(engineNumber, string.Empty))).ToList<UnitInformation>();
    return list.FirstOrDefault<UnitInformation>((System.Func<UnitInformation, bool>) (u => !string.IsNullOrEmpty(vehicleIdentity) && !string.IsNullOrEmpty(engineNumber) && u.IsSameIdentification(engineNumber, vehicleIdentity) && u.IdentityKeyContent == 3)) ?? list.OrderByDescending<UnitInformation, int>((System.Func<UnitInformation, int>) (u =>
    {
      if (u.Status != null)
        return (u.DeviceInformation.Any<DeviceInformation>((System.Func<DeviceInformation, bool>) (d => d.DataSource == 2)) ? 1 : 0) + (u.DeviceInformation.Any<DeviceInformation>((System.Func<DeviceInformation, bool>) (d => d.DataSource == 1)) ? 1 : 0);
      return (u.IdentityKeyContent & 2) == null ? 0 : 1;
    })).FirstOrDefault<UnitInformation>();
  }

  internal UnitInformation GetSelectedOrCurrentUnit()
  {
    bool flag = this.selectedVehicleIdentification != null || this.selectedEngineSerial != null;
    return ConnectedUnitPage.GetUnit((IEnumerable<UnitInformation>) ServerDataManager.GlobalInstance.UnitInformation, flag ? this.selectedVehicleIdentification : this.currentVehicleIdentification, flag ? this.selectedEngineSerial : this.currentEngineSerial) ?? ConnectedUnitPage.GetUnit(this.parent.PendingRequests, flag ? this.selectedVehicleIdentification : this.currentVehicleIdentification, flag ? this.selectedEngineSerial : this.currentEngineSerial);
  }

  private void WriteConnectedUnitPage()
  {
    if (this.dirty)
      return;
    this.dirty = true;
    ((Control) this.Wizard).BeginInvoke((Delegate) (() =>
    {
      this.dirty = false;
      this.WriteConnectedUnitPageDeferred();
    }));
  }

  private void WriteConnectedUnitPageDeferred()
  {
    this.UpdateConnectedIdentification();
    this.UpdateTitle();
    this.UpdateIdentificationWarning();
    this.UpdateUnitSelection();
    this.UpdateButtonPanel();
  }

  private void UpdateTitle()
  {
    if (!(this.titlePane != (HtmlElement) null))
      return;
    UnitInformation selectedOrCurrentUnit = this.GetSelectedOrCurrentUnit();
    string vehicleIdentity = selectedOrCurrentUnit != null ? selectedOrCurrentUnit.VehicleIdentity : this.selectedVehicleIdentification ?? this.currentVehicleIdentification;
    string engineNumber = selectedOrCurrentUnit != null ? selectedOrCurrentUnit.EngineNumber : this.selectedEngineSerial ?? this.currentEngineSerial;
    string str1 = FormattableString.Invariant(FormattableStringFactory.Create("<table class='grey' style='width:100%'>"));
    string str2;
    if (selectedOrCurrentUnit != null && ServerDataManager.GlobalInstance.UnitInformation.Contains(selectedOrCurrentUnit))
      str2 = str1 + FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", (object) ReprogrammingView.CreateTableSection("download", Resources.ConnectedUnit_DownloadedUnitData))) + FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", (object) ReprogrammingView.CreateDownloadedUnitHtml(selectedOrCurrentUnit, true)));
    else
      str2 = str1 + FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", (object) ReprogrammingView.CreateTableSection("download", Resources.GatherServerDataPage_TabConnectedUnit))) + FormattableString.Invariant(FormattableStringFactory.Create("<tr><td>{0}</td></tr>", (object) ReprogrammingView.CreateUnitToBeDownloadedHtml(vehicleIdentity, engineNumber)));
    this.titlePane.InnerHtml = str2 + "</table>";
  }

  private static bool IsValidIdentification(string vinOrPin)
  {
    return VehicleIdentification.GetClassification(vinOrPin) == (SapiManager.GlobalInstance.IsOffHighwayConnection ? 2 : 1);
  }

  private static Ecu GetProgrammableEcu(Channel channel)
  {
    Ecu programmableEcu;
    if (!channel.IsRollCall || (programmableEcu = SapiExtensions.GetSuppressedOfflineRelatedEcu(channel)) == null)
      programmableEcu = channel.Ecu;
    return programmableEcu;
  }

  internal static IEnumerable<Channel> ProgrammableChannels
  {
    get
    {
      return SapiManager.GlobalInstance.ActiveChannels.Select(channel => new
      {
        channel = channel,
        channelEcu = ConnectedUnitPage.GetProgrammableEcu(channel)
      }).Where(_param1 => (SapiExtensions.IsDataSourceDepot(_param1.channelEcu) || SapiExtensions.IsDataSourceEdex(_param1.channelEcu)) && !SapiManager.GlobalInstance.IsNotProgrammableForConnectedEquipment(_param1.channelEcu)).Select(_param1 => _param1.channel);
    }
  }

  private void UpdateIdentificationWarning()
  {
    List<string> source = new List<string>();
    List<Channel> list = ConnectedUnitPage.ProgrammableChannels.ToList<Channel>();
    int num = list.Where<Channel>((System.Func<Channel, bool>) (c => SapiManager.GetVehicleIdentificationNumber(c) == SapiManager.GlobalInstance.CurrentVehicleIdentification)).Count<Channel>();
    if (SapiManager.GlobalInstance.VehicleIdentificationInconsistency || SapiManager.GlobalInstance.EngineSerialInconsistency || num < 2 || !ConnectedUnitPage.IsValidIdentification(this.currentVehicleIdentification))
    {
      string str = !SapiManager.GlobalInstance.VehicleIdentificationInconsistency ? (!SapiManager.GlobalInstance.EngineSerialInconsistency ? (num >= 2 ? Resources.ConnectedUnit_VehicleIdentificationInvalid : Resources.ConnectedUnit_NotEnoughDevicesForCorrectIdentification) : Resources.ConnectedUnit_EngineSerialNumberInconsistency) : (num < 2 || SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.Count<string>() > 1 ? Resources.ConnectedUnit_VehicleIdentificationInconsitencyNotEnoughDevicesForCorrectIdentification : string.Format((IFormatProvider) CultureInfo.CurrentCulture, SapiManager.GlobalInstance.EngineSerialInconsistency ? Resources.ConnectedUnit_FormatVehicleIdentificationAndEngineSerialNumberInconsistency : Resources.ConnectedUnit_FormatVehicleIdentificationInconsistency, (object) string.Join(", ", list.Where<Channel>((System.Func<Channel, bool>) (c => SapiManager.GetVehicleIdentificationNumber(c) != (this.selectedVehicleIdentification ?? this.currentVehicleIdentification))).Select<Channel, string>((System.Func<Channel, string>) (c => c.Ecu.Name)))));
      source.Add(str);
    }
    UnitInformation selectedOrCurrentUnit = this.GetSelectedOrCurrentUnit();
    if (selectedOrCurrentUnit != null && ServerDataManager.GlobalInstance.UnitInformation.Contains(selectedOrCurrentUnit) && selectedOrCurrentUnit.VehicleIdentity == SapiManager.GlobalInstance.CurrentVehicleIdentification && !string.IsNullOrEmpty(SapiManager.GlobalInstance.CurrentEngineSerialNumber) && selectedOrCurrentUnit.EngineNumber != SapiManager.GlobalInstance.CurrentEngineSerialNumber)
      source.Add(Resources.ConnectedUnit_EngineSerialNumberInconsistencyWithVehicleIdentification);
    if (source.Count > 0)
    {
      this.warningPane.InnerHtml = string.Join(" ", source.Select<string, string>((System.Func<string, string>) (warning => FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'>{0}</span></p>", (object) warning)))));
      this.warningPane.SetAttribute("className", "show");
    }
    else
    {
      this.warningPane.InnerHtml = "&nbsp;";
      this.warningPane.SetAttribute("className", "hide");
    }
  }

  private static bool UnitCanBeProgrammed(
    UnitInformation unit,
    IEnumerable<Channel> programmableChannels)
  {
    bool flag = false;
    foreach (Channel programmableChannel1 in programmableChannels)
    {
      Channel programmableChannel = programmableChannel1;
      if (unit.GetInformationForDevice(ConnectedUnitPage.GetProgrammableEcu(programmableChannel).Name) != null)
        flag = true;
      else if (SapiManager.GlobalInstance.Sapi.Ecus.Where<Ecu>((System.Func<Ecu, bool>) (e => e.Identifier == programmableChannel.Identifier)).Any<Ecu>((System.Func<Ecu, bool>) (e => unit.GetInformationForDevice(e.Name) != null)))
        return false;
    }
    return flag;
  }

  private void UpdateUnitSelection()
  {
    if (!(this.unitSelectionPane != (HtmlElement) null))
      return;
    List<Channel> programmableChannels = ConnectedUnitPage.ProgrammableChannels.ToList<Channel>();
    int num = programmableChannels.Where<Channel>((System.Func<Channel, bool>) (c => SapiManager.GetVehicleIdentificationNumber(c) == SapiManager.GlobalInstance.CurrentVehicleIdentification)).Count<Channel>();
    if (!ServerClient.GlobalInstance.InUse && programmableChannels.Any<Channel>() && (SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.Count<string>() > 1 || SapiManager.GlobalInstance.EngineSerialInconsistency || num < 2))
    {
      string str = FormattableString.Invariant(FormattableStringFactory.Create("<div>{0}</div>", (object) Resources.ConnectedUnit_VerifyUnitIdentity));
      UnitInformation currentUnit = ConnectedUnitPage.GetUnit((IEnumerable<UnitInformation>) ServerDataManager.GlobalInstance.UnitInformation, this.currentVehicleIdentification, this.currentEngineSerial);
      if (currentUnit != null)
        str += ReprogrammingView.CreateInput("unitList", $"{currentUnit.VehicleIdentity}-{currentUnit.EngineNumber}", ReprogrammingView.GetTitleString(currentUnit.VehicleIdentity, currentUnit.EngineNumber), $"{Resources.ConnectedUnit_PrimaryConnectedIdentityDescription} - {ReprogrammingView.GetUnitTimeDisplay(currentUnit)}", true, false);
      else if (!string.IsNullOrEmpty(this.currentEngineSerial) || ConnectedUnitPage.IsValidIdentification(this.currentVehicleIdentification))
        str += ReprogrammingView.CreateInput("unitList", $"{this.currentVehicleIdentification}-{this.currentEngineSerial}", ReprogrammingView.GetTitleString(this.currentVehicleIdentification, this.currentEngineSerial), Resources.ConnectedUnit_PrimaryConnectedIdentityDescription, true, false);
      if (num < 2 || SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.All<string>((System.Func<string, bool>) (vin => !ConnectedUnitPage.IsValidIdentification(vin))))
      {
        string identityKey = ConnectedUnitPage.CreateIdentityKey(this.selectedVehicleIdentification, this.selectedEngineSerial);
        foreach (UnitInformation unit in ServerDataManager.GlobalInstance.UnitInformation.Where<UnitInformation>((System.Func<UnitInformation, bool>) (u => u != currentUnit && ConnectedUnitPage.UnitCanBeProgrammed(u, (IEnumerable<Channel>) programmableChannels))))
          str += ReprogrammingView.CreateInput("unitList", $"{unit.VehicleIdentity}-{unit.EngineNumber}", ReprogrammingView.GetTitleString(unit.VehicleIdentity, unit.EngineNumber), ReprogrammingView.GetUnitTimeDisplay(unit), unit.IdentityKey == identityKey, false);
        str = str + ReprogrammingView.CreateInput("unitList", "manual", Resources.ConnectedUnit_EnterIdentity, Resources.ConnectedUnit_EnterIdentityDescription, this.manualEntry, false) + FormattableString.Invariant(FormattableStringFactory.Create("\r\n                            <div name='manual' class='{0}'>\r\n                                <div class='indent'>{1}</div>\r\n                                <div class='indent'>{2}</div>\r\n                                <div id='esnonlywarning' class='indent hide'><span class='warninginline'>{3}</span></div>\r\n                            </div>\r\n                        ", this.manualEntry ? (object) "show" : (object) "hide", (object) ReprogrammingView.CreateTextInput("vin", Resources.ProgrammingDataItem_VehicleIdentificationNumber, this.manualVehicleIdentification, 17, !string.IsNullOrEmpty(this.manualVehicleIdentification) ? (ConnectedUnitPage.IsValidIdentification(this.manualVehicleIdentification) ? "valid" : "invalid") : (string) null), (object) ReprogrammingView.CreateTextInput("esn", Resources.ProgrammingDataItem_EngineSerialNumber, this.manualEngineSerial, 14, (string) null), (object) Resources.ConnectedUnit_VinIsNeededToContinueDataDownload));
      }
      else
      {
        foreach (string vehicleIdentity in SapiManager.GlobalInstance.CurrentLogFileInformation.PossibleVehicleIdentifications.Where<string>((System.Func<string, bool>) (pi => pi != this.currentVehicleIdentification)))
        {
          UnitInformation unit = ConnectedUnitPage.GetUnit((IEnumerable<UnitInformation>) ServerDataManager.GlobalInstance.UnitInformation, vehicleIdentity, string.Empty);
          str = unit == null ? str + ReprogrammingView.CreateInput("unitList", vehicleIdentity, ReprogrammingView.GetTitleString(vehicleIdentity, (string) null), Resources.ConnectedUnit_OtherConnectedIdentityDescription, this.selectedVehicleIdentification == vehicleIdentity, false) : str + ReprogrammingView.CreateInput("unitList", $"{unit.VehicleIdentity}-{unit.EngineNumber}", ReprogrammingView.GetTitleString(unit.VehicleIdentity, unit.EngineNumber), $"{Resources.ConnectedUnit_OtherConnectedIdentityDescription} - {ReprogrammingView.GetUnitTimeDisplay(unit)}", this.selectedVehicleIdentification == unit.VehicleIdentity, false);
        }
      }
      this.unitSelectionPane.InnerHtml = str;
      this.unitSelectionPane.SetAttribute("className", "show");
      this.manualInputPane = this.unitSelectionPane.GetElementsByTagName("div").GetElementsByName("manual").OfType<HtmlElement>().FirstOrDefault<HtmlElement>();
      ReprogrammingView.SubscribeInputEvents(this.inputPane, "radio", "onchange", new EventHandler(this.connectedUnitList_onChange), false);
      ReprogrammingView.SubscribeInputEvents(this.inputPane, "text", "oninput", new EventHandler(this.textArea_onInput));
    }
    else
    {
      this.unitSelectionPane.InnerHtml = "&nbsp;";
      this.unitSelectionPane.SetAttribute("className", "hide");
      this.manualInputPane = (HtmlElement) null;
    }
  }

  private void UpdateButtonPanel()
  {
    if (!ServerClient.GlobalInstance.InUse)
    {
      UnitInformation selectedOrCurrentUnit = this.GetSelectedOrCurrentUnit();
      if (selectedOrCurrentUnit != null && ServerDataManager.GlobalInstance.UnitInformation.Contains(selectedOrCurrentUnit))
      {
        string empty = string.Empty;
        string str;
        if (selectedOrCurrentUnit.Status == 2 || selectedOrCurrentUnit.Status == 4 && selectedOrCurrentUnit.DeviceInformation.Count > 0)
        {
          str = empty + ReprogrammingView.WriteButton("next", Resources.Wizard_ButtonNext) + ReprogrammingView.WriteButton("download", Resources.Wizard_ButtonRefreshData);
        }
        else
        {
          if (selectedOrCurrentUnit.Status == 4)
            empty += ReprogrammingView.WriteButton("remove", Resources.Wizard_ButtonRemoveData);
          str = empty + ReprogrammingView.WriteButton("download", Resources.Wizard_ButtonRefreshData);
        }
        this.buttonPanel.InnerHtml = str;
      }
      else
      {
        this.buttonPanel.InnerHtml = ReprogrammingView.WriteButton("download", Resources.Wizard_ButtonDownloadData);
        if (!string.IsNullOrEmpty(this.selectedEngineSerial ?? this.currentEngineSerial) || ConnectedUnitPage.IsValidIdentification(this.selectedVehicleIdentification ?? this.currentVehicleIdentification))
          return;
        ReprogrammingView.SetButtonState(this.buttonPanel.GetElementsByTagName("button")[0], false, true);
      }
    }
    else
      this.buttonPanel.InnerHtml = "&nbsp;";
  }

  private void connectedUnitList_onChange(object sender, EventArgs e)
  {
    HtmlElement htmlElement1 = sender as HtmlElement;
    if (htmlElement1.Id == "manual")
    {
      this.manualInputPane.SetAttribute("className", "show");
      if (string.IsNullOrEmpty(this.manualEngineSerial) && string.IsNullOrEmpty(this.manualVehicleIdentification))
      {
        this.manualVehicleIdentification = this.currentVehicleIdentification;
        this.manualEngineSerial = this.currentEngineSerial;
        foreach (HtmlElement htmlElement2 in this.manualInputPane.GetElementsByTagName("input"))
        {
          switch (htmlElement2.Name)
          {
            case "vin":
              if (!string.IsNullOrEmpty(this.manualVehicleIdentification))
              {
                (htmlElement2.DomElement as IHTMLInputTextElement).value = this.manualVehicleIdentification;
                htmlElement2.SetAttribute("className", ConnectedUnitPage.IsValidIdentification(this.manualVehicleIdentification) ? "valid" : "invalid");
                continue;
              }
              continue;
            case "esn":
              if (!string.IsNullOrEmpty(this.manualEngineSerial))
              {
                (htmlElement2.DomElement as IHTMLInputTextElement).value = this.manualEngineSerial;
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      this.selectedEngineSerial = this.manualEngineSerial;
      this.selectedVehicleIdentification = this.manualVehicleIdentification;
      this.manualEntry = true;
    }
    else
    {
      this.manualInputPane?.SetAttribute("className", "hide");
      string[] strArray = (htmlElement1.DomElement as IHTMLInputElement).value.Split("-".ToCharArray());
      this.selectedVehicleIdentification = strArray[0];
      this.selectedEngineSerial = strArray.Length > 1 ? strArray[1] : string.Empty;
      this.manualEntry = false;
    }
    this.UpdateTitle();
    this.UpdateIdentificationWarning();
    this.UpdateButtonPanel();
  }

  private void textArea_onInput(object sender, EventArgs e)
  {
    HtmlElement htmlElement = sender as HtmlElement;
    string str = string.Empty;
    try
    {
      str = (htmlElement.DomElement as IHTMLInputTextElement).value.ToUpperInvariant();
    }
    catch (NullReferenceException ex)
    {
    }
    (htmlElement.DomElement as IHTMLInputTextElement).value = str;
    switch (htmlElement.Id)
    {
      case "vin":
        this.selectedVehicleIdentification = this.manualVehicleIdentification = str;
        htmlElement.SetAttribute("className", ConnectedUnitPage.IsValidIdentification(this.manualVehicleIdentification) ? "valid" : "invalid");
        this.manualInputPane.GetElementsByTagName("div").OfType<HtmlElement>().FirstOrDefault<HtmlElement>((System.Func<HtmlElement, bool>) (el => el.Id == "esnonlywarning")).SetAttribute("className", "indent " + (str.Length == 0 ? "show" : "hide"));
        break;
      case "esn":
        this.selectedEngineSerial = this.manualEngineSerial = str;
        break;
    }
    this.UpdateTitle();
    this.UpdateIdentificationWarning();
    this.UpdateButtonPanel();
  }

  internal override WizardPage OnWizardNext()
  {
    try
    {
      ProgrammingData forConnectedUnit = ProgrammingData.CreateFromAutomaticOperationForConnectedUnit();
      if (forConnectedUnit != null)
      {
        this.dataProvider.SelectedUnit = forConnectedUnit.Unit;
        this.dataProvider.ProgrammingData = Enumerable.Repeat<ProgrammingData>(forConnectedUnit, 1);
        return this.Wizard.Pages.Last<WizardPage>();
      }
    }
    catch (DataException ex)
    {
      int num = (int) ControlHelpers.ShowMessageBox(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      return (WizardPage) null;
    }
    this.dataProvider.SelectedUnit = this.GetSelectedOrCurrentUnit();
    return base.OnWizardNext();
  }
}
