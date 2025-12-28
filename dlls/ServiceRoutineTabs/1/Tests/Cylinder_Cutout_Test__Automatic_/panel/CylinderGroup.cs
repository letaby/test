// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel.CylinderGroup
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

#nullable disable
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

  public List<int> Cylinders => this.cylinders;

  public DateTime TimeGroupWasTurnedOff
  {
    get => this.start;
    set => this.start = value;
  }

  public DateTime TimeGroupWasTurnedOn
  {
    get => this.end;
    set => this.end = value;
  }

  public double? Result { get; private set; }

  public string RawUnits { get; private set; }

  public string ResultAsString => ((Control) this.label).Text;

  public void SetResult(double? result, string rawUnits)
  {
    this.Result = result;
    this.RawUnits = rawUnits;
    if (!result.HasValue)
    {
      ((Control) this.label).Text = Resources.Message_NoValue;
      ((Control) this.unitsLabel).Text = string.Empty;
    }
    else
    {
      double num = result.Value;
      string str = rawUnits;
      if (!string.IsNullOrEmpty(rawUnits))
      {
        Conversion conversion = Converter.GlobalInstance.GetConversion(rawUnits);
        if (conversion != null)
        {
          num = conversion.Convert(num);
          str = conversion.OutputUnit;
        }
      }
      ((Control) this.label).Text = string.Format(Converter.GetFormatString(1), (object) num);
      ((Control) this.unitsLabel).Text = str;
    }
  }

  private void OnConversionChanged(object sender, EventArgs e)
  {
    this.SetResult(this.Result, this.RawUnits);
  }

  public DateTime EndOfWaitTimeAfterGroupWasTurnedOff
  {
    get => this.TimeGroupWasTurnedOff + CylinderGroup.defaultOffDuration;
  }

  public DateTime EndOfWaitTimeAfterGroupWasTurnedOn
  {
    get => this.TimeGroupWasTurnedOn + CylinderGroup.defaultOnDuration;
  }

  public CylinderGroup(IEnumerable<int> cylinders, ScalingLabel label, Label unitsLabel)
  {
    this.label = label;
    this.unitsLabel = unitsLabel;
    this.cylinders = new List<int>(cylinders);
    Converter.GlobalInstance.UnitsSelectionChanged += new EventHandler(this.OnConversionChanged);
  }

  public string GetServiceExecuteList()
  {
    if (string.IsNullOrEmpty(this.serviceExecuteList))
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (int cylinder in this.cylinders)
      {
        stringBuilder.AppendFormat("RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder({0},0);", (object) cylinder);
        this.serviceExecuteList = stringBuilder.ToString();
      }
    }
    return this.serviceExecuteList;
  }

  public string GetCuttingStatusText()
  {
    if (string.IsNullOrEmpty(this.cylinderCutoutStatusText))
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.cylinders.Count; ++index)
      {
        if (index == this.cylinders.Count - 1)
        {
          stringBuilder.Append(Resources.Message_And);
          stringBuilder.Append(this.cylinders[index].ToString());
          break;
        }
        if (index != 0)
          stringBuilder.Append(", ");
        stringBuilder.Append(this.Cylinders[index].ToString());
      }
      this.cylinderCutoutStatusText = stringBuilder.ToString();
    }
    return this.cylinderCutoutStatusText;
  }
}
