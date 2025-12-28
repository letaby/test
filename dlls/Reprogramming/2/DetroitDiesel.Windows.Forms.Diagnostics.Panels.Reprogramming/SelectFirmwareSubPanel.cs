using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using mshtml;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class SelectFirmwareSubPanel
{
	private IProvideProgrammingData dataProvider;

	private HtmlElement inputPane;

	private UnitInformation unit;

	private FirmwareInformation selectedFirmware;

	public UnitInformation Unit => unit;

	public FirmwareInformation SelectedFirmware => selectedFirmware;

	public event EventHandler SelectedFirmwareChanged;

	public SelectFirmwareSubPanel(IProvideProgrammingData dataProvider, HtmlElement element)
	{
		this.dataProvider = dataProvider;
		inputPane = element;
	}

	public void UpdateFirmwareList(Channel channel)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Invalid comparison between Unknown and I4
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		bool flag = false;
		unit = null;
		DeviceInformation val = null;
		string innerHtml = "&nbsp;";
		if (channel != null)
		{
			unit = dataProvider.SelectedUnit;
			if (unit != null)
			{
				if (unit != ProgrammingData.UnitInformation(channel))
				{
					selectedFirmware = null;
					inputPane.InnerHtml = FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectFirmwareSubPanelItem_CannotUpdateSoftwareForDeviceWithDifferentIdentity}</p>");
					return;
				}
				val = unit.GetInformationForDevice(channel.Ecu.Name);
				if (val == null)
				{
					selectedFirmware = null;
					inputPane.InnerHtml = FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectFirmwareSubPanelItem_UnitFileDoesNotContainInformationForThisDevice}</p>");
					return;
				}
			}
			if (unit == null || (val != null && (int)unit.GetStatusForDataSource(val.DataSource) != 2))
			{
				selectedFirmware = null;
				inputPane.InnerHtml = FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectFirmwareSubPanelItem_MustHaveUnitFileToUpdateSoftwareForThisDevice}</p>");
				return;
			}
			List<string> list = new List<string>();
			Part part = new Part(SapiManager.GetHardwarePartNumber(channel));
			foreach (FirmwareInformation item in ServerDataManager.GlobalInstance.FirmwareInformation)
			{
				FirmwareInformation val2 = item;
				if (!(val2.Device == channel.Ecu.Name))
				{
					continue;
				}
				bool flag2 = true;
				if (!val.FirmwareOptionAvailableForHardware(val2, part))
				{
					flag2 = false;
				}
				else if (val.HasDataSet)
				{
					DataSetOptionInformation compatibleDataSetOption = val.GetCompatibleDataSetOption(val2.Version, PartExtensions.ToFlashKeyStyleString(part));
					if (compatibleDataSetOption == null)
					{
						flag2 = false;
					}
				}
				bool flag3 = false;
				if (flag2)
				{
					Software val3 = new Software(val2.Device, val2.Version, part);
					if (!Unit.UnitFixedAtTest && !ServerDataManager.GlobalInstance.CompatibilityTable.IsCompatibleWithCurrentSoftware(val3))
					{
						CompatibleSoftwareCollection val4 = ServerDataManager.GlobalInstance.CompatibilityTable.CreateCompatibleList(val3);
						if (((ReadOnlyCollection<SoftwareCollection>)(object)val4).Count > 0)
						{
							CompatibleSoftwareCollection val5 = val4.FilterForUnit(unit);
							if (((ReadOnlyCollection<SoftwareCollection>)(object)val5).Count > 0)
							{
								flag3 = true;
							}
							else
							{
								flag2 = false;
							}
						}
						else
						{
							flag2 = false;
						}
					}
				}
				if (flag2)
				{
					list.Add(ReprogrammingView.CreateInput("softwareList", val2.Key, string.Join(" - ", val2.Version, val2.Key, val2.Description), flag3 ? Resources.SelectFirmwareSubPanelToolTip_AdditionalReprogrammingIsRequiredToUseThisSoftware : "", val2 == selectedFirmware, disabled: false, flag3 ? "warninginline" : null));
					if (val2 == selectedFirmware)
					{
						flag = true;
					}
				}
			}
			innerHtml = ((list.Count <= 0) ? FormattableString.Invariant($"<p><span class='warninginline'/>{Resources.SelectFirmwareSubPanelItem_NoSoftwareUpdatesAvailableForSelectedDevice}</p>") : ReprogrammingView.CreateRadioList(Resources.SelectFirmwareSubPanel_SelectFirmware, list));
		}
		inputPane.InnerHtml = innerHtml;
		if (!ReprogrammingView.SubscribeInputEvents(inputPane, "radio", "onchange", softwareList_onChange) && !flag)
		{
			selectedFirmware = null;
			this.SelectedFirmwareChanged?.Invoke(this, new EventArgs());
		}
	}

	private void softwareList_onChange(object sender, EventArgs args)
	{
		HtmlElement htmlElement = sender as HtmlElement;
		ReprogrammingView.SetActiveRadioElement(htmlElement);
		string value = (htmlElement.DomElement as IHTMLInputElement).value;
		selectedFirmware = ServerDataManager.GlobalInstance.GetFirmwareInformationForKey(value);
		this.SelectedFirmwareChanged?.Invoke(this, new EventArgs());
	}
}
