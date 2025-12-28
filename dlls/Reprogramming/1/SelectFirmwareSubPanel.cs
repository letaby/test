// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.SelectFirmwareSubPanel
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
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class SelectFirmwareSubPanel
{
  private IProvideProgrammingData dataProvider;
  private HtmlElement inputPane;
  private UnitInformation unit;
  private FirmwareInformation selectedFirmware;

  public SelectFirmwareSubPanel(IProvideProgrammingData dataProvider, HtmlElement element)
  {
    this.dataProvider = dataProvider;
    this.inputPane = element;
  }

  public void UpdateFirmwareList(Channel channel)
  {
    bool flag1 = false;
    this.unit = (UnitInformation) null;
    DeviceInformation deviceInformation = (DeviceInformation) null;
    string str = "&nbsp;";
    if (channel != null)
    {
      this.unit = this.dataProvider.SelectedUnit;
      if (this.unit != null)
      {
        if (this.unit != ProgrammingData.UnitInformation(channel))
        {
          this.selectedFirmware = (FirmwareInformation) null;
          this.inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectFirmwareSubPanelItem_CannotUpdateSoftwareForDeviceWithDifferentIdentity));
          return;
        }
        deviceInformation = this.unit.GetInformationForDevice(channel.Ecu.Name);
        if (deviceInformation == null)
        {
          this.selectedFirmware = (FirmwareInformation) null;
          this.inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectFirmwareSubPanelItem_UnitFileDoesNotContainInformationForThisDevice));
          return;
        }
      }
      if (this.unit == null || deviceInformation != null && this.unit.GetStatusForDataSource(deviceInformation.DataSource) != 2)
      {
        this.selectedFirmware = (FirmwareInformation) null;
        this.inputPane.InnerHtml = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectFirmwareSubPanelItem_MustHaveUnitFileToUpdateSoftwareForThisDevice));
        return;
      }
      List<string> listItems = new List<string>();
      Part part = new Part(SapiManager.GetHardwarePartNumber(channel));
      foreach (FirmwareInformation firmwareInformation in ServerDataManager.GlobalInstance.FirmwareInformation)
      {
        if (firmwareInformation.Device == channel.Ecu.Name)
        {
          bool flag2 = true;
          if (!deviceInformation.FirmwareOptionAvailableForHardware(firmwareInformation, part))
            flag2 = false;
          else if (deviceInformation.HasDataSet && deviceInformation.GetCompatibleDataSetOption(firmwareInformation.Version, PartExtensions.ToFlashKeyStyleString(part)) == null)
            flag2 = false;
          bool flag3 = false;
          if (flag2)
          {
            Software software = new Software(firmwareInformation.Device, firmwareInformation.Version, part);
            if (!this.Unit.UnitFixedAtTest && !ServerDataManager.GlobalInstance.CompatibilityTable.IsCompatibleWithCurrentSoftware(software))
            {
              CompatibleSoftwareCollection compatibleList = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleList(software);
              if (((ReadOnlyCollection<SoftwareCollection>) compatibleList).Count > 0)
              {
                if (((ReadOnlyCollection<SoftwareCollection>) compatibleList.FilterForUnit(this.unit)).Count > 0)
                  flag3 = true;
                else
                  flag2 = false;
              }
              else
                flag2 = false;
            }
          }
          if (flag2)
          {
            listItems.Add(ReprogrammingView.CreateInput("softwareList", firmwareInformation.Key, string.Join(" - ", firmwareInformation.Version, firmwareInformation.Key, firmwareInformation.Description), flag3 ? Resources.SelectFirmwareSubPanelToolTip_AdditionalReprogrammingIsRequiredToUseThisSoftware : "", (firmwareInformation == this.selectedFirmware ? 1 : 0) != 0, false, flag3 ? "warninginline" : (string) null));
            if (firmwareInformation == this.selectedFirmware)
              flag1 = true;
          }
        }
      }
      if (listItems.Count > 0)
        str = ReprogrammingView.CreateRadioList(Resources.SelectFirmwareSubPanel_SelectFirmware, listItems);
      else
        str = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectFirmwareSubPanelItem_NoSoftwareUpdatesAvailableForSelectedDevice));
    }
    this.inputPane.InnerHtml = str;
    if (ReprogrammingView.SubscribeInputEvents(this.inputPane, "radio", "onchange", new EventHandler(this.softwareList_onChange)) || flag1)
      return;
    this.selectedFirmware = (FirmwareInformation) null;
    EventHandler selectedFirmwareChanged = this.SelectedFirmwareChanged;
    if (selectedFirmwareChanged == null)
      return;
    selectedFirmwareChanged((object) this, new EventArgs());
  }

  public UnitInformation Unit => this.unit;

  public FirmwareInformation SelectedFirmware => this.selectedFirmware;

  public event EventHandler SelectedFirmwareChanged;

  private void softwareList_onChange(object sender, EventArgs args)
  {
    HtmlElement inputElement = sender as HtmlElement;
    ReprogrammingView.SetActiveRadioElement(inputElement);
    this.selectedFirmware = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey((inputElement.DomElement as IHTMLInputElement).value);
    EventHandler selectedFirmwareChanged = this.SelectedFirmwareChanged;
    if (selectedFirmwareChanged == null)
      return;
    selectedFirmwareChanged((object) this, new EventArgs());
  }
}
