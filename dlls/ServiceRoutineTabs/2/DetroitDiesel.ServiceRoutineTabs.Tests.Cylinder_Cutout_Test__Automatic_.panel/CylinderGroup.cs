using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel;

public class CylinderGroup
{
	private List<int> cylinders;

	private readonly ScalingLabel label;

	private readonly Label unitsLabel;

	private DateTime start;

	private DateTime end;

	private static readonly TimeSpan defaultOffDuration = new TimeSpan(0, 0, 15);

	private static readonly TimeSpan defaultOnDuration = new TimeSpan(0, 0, 5);

	private string serviceExecuteList = string.Empty;

	private string cylinderCutoutStatusText = string.Empty;

	public List<int> Cylinders => cylinders;

	public DateTime TimeGroupWasTurnedOff
	{
		get
		{
			return start;
		}
		set
		{
			start = value;
		}
	}

	public DateTime TimeGroupWasTurnedOn
	{
		get
		{
			return end;
		}
		set
		{
			end = value;
		}
	}

	public double? Result { get; private set; }

	public string RawUnits { get; private set; }

	public string ResultAsString => ((Control)(object)label).Text;

	public DateTime EndOfWaitTimeAfterGroupWasTurnedOff => TimeGroupWasTurnedOff + defaultOffDuration;

	public DateTime EndOfWaitTimeAfterGroupWasTurnedOn => TimeGroupWasTurnedOn + defaultOnDuration;

	public void SetResult(double? result, string rawUnits)
	{
		Result = result;
		RawUnits = rawUnits;
		if (!result.HasValue)
		{
			((Control)(object)label).Text = Resources.Message_NoValue;
			((Control)(object)unitsLabel).Text = string.Empty;
			return;
		}
		double num = result.Value;
		string text = rawUnits;
		if (!string.IsNullOrEmpty(rawUnits))
		{
			Conversion conversion = Converter.GlobalInstance.GetConversion(rawUnits);
			if (conversion != null)
			{
				num = conversion.Convert(num);
				text = conversion.OutputUnit;
			}
		}
		((Control)(object)label).Text = string.Format(Converter.GetFormatString(1), num);
		((Control)(object)unitsLabel).Text = text;
	}

	private void OnConversionChanged(object sender, EventArgs e)
	{
		SetResult(Result, RawUnits);
	}

	public CylinderGroup(IEnumerable<int> cylinders, ScalingLabel label, Label unitsLabel)
	{
		this.label = label;
		this.unitsLabel = unitsLabel;
		this.cylinders = new List<int>(cylinders);
		Converter.GlobalInstance.UnitsSelectionChanged += OnConversionChanged;
	}

	public string GetServiceExecuteList()
	{
		if (string.IsNullOrEmpty(serviceExecuteList))
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int cylinder in cylinders)
			{
				stringBuilder.AppendFormat("RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder({0},0);", cylinder);
				serviceExecuteList = stringBuilder.ToString();
			}
		}
		return serviceExecuteList;
	}

	public string GetCuttingStatusText()
	{
		if (string.IsNullOrEmpty(cylinderCutoutStatusText))
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < cylinders.Count; i++)
			{
				if (i == cylinders.Count - 1)
				{
					stringBuilder.Append(Resources.Message_And);
					stringBuilder.Append(cylinders[i].ToString());
					break;
				}
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(Cylinders[i].ToString());
			}
			cylinderCutoutStatusText = stringBuilder.ToString();
		}
		return cylinderCutoutStatusText;
	}
}
