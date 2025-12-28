using System;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter.panel;

internal class FaultInstrument
{
	private readonly Label titleLabel;

	private readonly ScalingLabel valueLabel;

	private string NoFault = Resources.Message_NoFault;

	internal readonly string faultCodeId;

	public FaultInstrument(Label titleLabel, ScalingLabel valueLabel, string faultCodeId)
	{
		this.titleLabel = titleLabel;
		this.valueLabel = valueLabel;
		this.faultCodeId = faultCodeId;
	}

	internal void DisplayFaultTitle(FaultCode fault)
	{
		if (fault != null)
		{
			((Control)(object)titleLabel).Text = $"{fault.Text}: ({fault.Number}/{fault.Mode})";
		}
		else
		{
			((Control)(object)titleLabel).Text = string.Empty;
		}
	}

	internal void DisplayFaultText(FaultCodeIncidentCollection faultIncidents)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ValueState representedState = (ValueState)0;
		if (faultIncidents != null && faultIncidents.Current != null)
		{
			FaultCodeIncident current = faultIncidents.Current;
			((Control)(object)valueLabel).Text = SapiExtensions.ToStatusString(current);
			if (SapiManager.IsFaultActionable(current))
			{
				representedState = (ValueState)3;
			}
		}
		else
		{
			((Control)(object)valueLabel).Text = NoFault;
			representedState = (ValueState)1;
		}
		valueLabel.RepresentedState = representedState;
	}

	internal void OnFaultCodeIncidentUpdateHandler(object sender, EventArgs e)
	{
		FaultCodeIncidentCollection faultCodeIncidentCollection = sender as FaultCodeIncidentCollection;
		if (faultCodeIncidentCollection != null)
		{
			DisplayFaultTitle(faultCodeIncidentCollection.FaultCode);
		}
		DisplayFaultText(faultCodeIncidentCollection);
	}
}
