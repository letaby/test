// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.SelectDataSetSubPanel
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class SelectDataSetSubPanel
{
  private HtmlElement inputPane;
  private IProvideProgrammingData dataProvider;
  private DataSetOptionInformation selectedDataSetOption;
  private UnitInformation unit;

  public SelectDataSetSubPanel(IProvideProgrammingData dataProvider, HtmlElement element)
  {
    this.dataProvider = dataProvider;
    this.inputPane = element;
  }

  public void UpdateDataSetList(Channel channel)
  {
    bool flag = false;
    this.unit = (UnitInformation) null;
    string str = "&nbsp;";
    if (channel != null)
    {
      this.unit = this.dataProvider.SelectedUnit;
      if (this.unit != null)
      {
        if (this.unit == ProgrammingData.UnitInformation(channel))
        {
          List<string> listItems = new List<string>();
          foreach (DeviceInformation deviceInformation in this.unit.DeviceInformation)
          {
            if (deviceInformation.Device == channel.Ecu.Name)
            {
              if (deviceInformation.DataSource == 2)
                throw new InvalidOperationException($"Change dataset operation not permitted for Edex based ecu {channel.Ecu.Name}.");
              if (deviceInformation.HasDataSet && this.unit.GetStatusForDataSource(deviceInformation.DataSource) == 2)
              {
                string softwareVersion = SapiManager.GetSoftwareVersion(channel);
                Part part = new Part(SapiManager.GetHardwarePartNumber(channel));
                foreach (FirmwareOptionInformation firmwareOption in deviceInformation.FirmwareOptions)
                {
                  if (firmwareOption.Version == softwareVersion && (firmwareOption.HardwarePartNumber == null || firmwareOption.HardwarePartNumber.Equals((object) part)))
                  {
                    foreach (DataSetOptionInformation dataSetOption in firmwareOption.DataSetOptions)
                    {
                      listItems.Add(ReprogrammingView.CreateInput("datasetList", $"{channel.Ecu.Name}.{dataSetOption.Key}", $"{dataSetOption.Description} - {dataSetOption.Key}", dataSetOption.IsCurrent ? Resources.SelectDataSetPanel_Message_ThisIsTheCurrentDatasetForTheDevice : "", dataSetOption == this.selectedDataSetOption, false));
                      if (dataSetOption == this.selectedDataSetOption)
                        flag = true;
                    }
                  }
                }
              }
            }
          }
          if (listItems.Count > 0)
            str = ReprogrammingView.CreateRadioList(Resources.SelectDataSetSubPanel_SelectDataSet, listItems);
          else
            str = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectDataSetSubPanelItem_NoOptionsForThisUnitSoftware));
        }
        else
          str = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectDataSetSubPanelItem_CannotChangeDataSetForDeviceWithDifferentIdentity));
      }
      else
        str = FormattableString.Invariant(FormattableStringFactory.Create("<p><span class='warninginline'/>{0}</p>", (object) Resources.SelectDataSetSubPanelItem_NoDataForThisUnit));
    }
    this.inputPane.InnerHtml = str;
    if (ReprogrammingView.SubscribeInputEvents(this.inputPane, "radio", "onchange", new EventHandler(this.dataSetList_onChange)) || flag)
      return;
    this.selectedDataSetOption = (DataSetOptionInformation) null;
    EventHandler dataOptionChanged = this.SelectedDataOptionChanged;
    if (dataOptionChanged == null)
      return;
    dataOptionChanged((object) this, new EventArgs());
  }

  public DataSetOptionInformation SelectedDataSetOption => this.selectedDataSetOption;

  public UnitInformation Unit => this.unit;

  public event EventHandler SelectedDataOptionChanged;

  private void dataSetList_onChange(object sender, EventArgs args)
  {
    HtmlElement inputElement = sender as HtmlElement;
    ReprogrammingView.SetActiveRadioElement(inputElement);
    string[] result = (inputElement.DomElement as IHTMLInputElement).value.Split(".".ToCharArray());
    DeviceInformation informationForDevice = this.unit?.GetInformationForDevice(result[0]);
    this.selectedDataSetOption = informationForDevice != null ? informationForDevice.FirmwareOptions.OfType<FirmwareOptionInformation>().SelectMany<FirmwareOptionInformation, DataSetOptionInformation>((Func<FirmwareOptionInformation, IEnumerable<DataSetOptionInformation>>) (foi => (IEnumerable<DataSetOptionInformation>) foi.DataSetOptions)).FirstOrDefault<DataSetOptionInformation>((Func<DataSetOptionInformation, bool>) (doi => doi.Key == result[1])) : (DataSetOptionInformation) null;
    EventHandler dataOptionChanged = this.SelectedDataOptionChanged;
    if (dataOptionChanged == null)
      return;
    dataOptionChanged((object) this, new EventArgs());
  }
}
