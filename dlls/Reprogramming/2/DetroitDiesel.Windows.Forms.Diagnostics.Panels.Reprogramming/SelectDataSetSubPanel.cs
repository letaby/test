using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using mshtml;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class SelectDataSetSubPanel
{
	private HtmlElement inputPane;

	private IProvideProgrammingData dataProvider;

	private DataSetOptionInformation selectedDataSetOption;

	private UnitInformation unit;

	public DataSetOptionInformation SelectedDataSetOption => selectedDataSetOption;

	public UnitInformation Unit => unit;

	public event EventHandler SelectedDataOptionChanged;

	public SelectDataSetSubPanel(IProvideProgrammingData dataProvider, HtmlElement element)
	{
		this.dataProvider = dataProvider;
		inputPane = element;
	}

	public void UpdateDataSetList(Channel channel)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Invalid comparison between Unknown and I4
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		bool flag = false;
		unit = null;
		string innerHtml = "&nbsp;";
		if (channel != null)
		{
			unit = dataProvider.SelectedUnit;
			if (unit != null)
			{
				if (unit == ProgrammingData.UnitInformation(channel))
				{
					List<string> list = new List<string>();
					foreach (DeviceInformation item in unit.DeviceInformation)
					{
						if (!(item.Device == channel.Ecu.Name))
						{
							continue;
						}
						if ((int)item.DataSource == 2)
						{
							throw new InvalidOperationException("Change dataset operation not permitted for Edex based ecu " + channel.Ecu.Name + ".");
						}
						if (!item.HasDataSet || (int)unit.GetStatusForDataSource(item.DataSource) != 2)
						{
							continue;
						}
						string softwareVersion = SapiManager.GetSoftwareVersion(channel);
						Part obj = new Part(SapiManager.GetHardwarePartNumber(channel));
						foreach (FirmwareOptionInformation firmwareOption in item.FirmwareOptions)
						{
							if (!(firmwareOption.Version == softwareVersion) || (firmwareOption.HardwarePartNumber != null && !firmwareOption.HardwarePartNumber.Equals(obj)))
							{
								continue;
							}
							foreach (DataSetOptionInformation dataSetOption in firmwareOption.DataSetOptions)
							{
								list.Add(ReprogrammingView.CreateInput("datasetList", channel.Ecu.Name + "." + dataSetOption.Key, dataSetOption.Description + " - " + dataSetOption.Key, dataSetOption.IsCurrent ? Resources.SelectDataSetPanel_Message_ThisIsTheCurrentDatasetForTheDevice : "", dataSetOption == selectedDataSetOption, disabled: false));
								if (dataSetOption == selectedDataSetOption)
								{
									flag = true;
								}
							}
						}
					}
					innerHtml = ((list.Count <= 0) ? FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectDataSetSubPanelItem_NoOptionsForThisUnitSoftware}</p>") : ReprogrammingView.CreateRadioList(Resources.SelectDataSetSubPanel_SelectDataSet, list));
				}
				else
				{
					innerHtml = FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectDataSetSubPanelItem_CannotChangeDataSetForDeviceWithDifferentIdentity}</p>");
				}
			}
			else
			{
				innerHtml = FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectDataSetSubPanelItem_NoDataForThisUnit}</p>");
			}
		}
		inputPane.InnerHtml = innerHtml;
		if (!ReprogrammingView.SubscribeInputEvents(inputPane, "radio", "onchange", dataSetList_onChange) && !flag)
		{
			selectedDataSetOption = null;
			this.SelectedDataOptionChanged?.Invoke(this, new EventArgs());
		}
	}

	private void dataSetList_onChange(object sender, EventArgs args)
	{
		HtmlElement htmlElement = sender as HtmlElement;
		ReprogrammingView.SetActiveRadioElement(htmlElement);
		string[] result = (htmlElement.DomElement as IHTMLInputElement).value.Split(".".ToCharArray());
		UnitInformation obj = unit;
		DeviceInformation val = ((obj != null) ? obj.GetInformationForDevice(result[0]) : null);
		selectedDataSetOption = ((val != null) ? val.FirmwareOptions.OfType<FirmwareOptionInformation>().SelectMany((FirmwareOptionInformation foi) => foi.DataSetOptions).FirstOrDefault((DataSetOptionInformation doi) => doi.Key == result[1]) : null);
		this.SelectedDataOptionChanged?.Invoke(this, new EventArgs());
	}
}
