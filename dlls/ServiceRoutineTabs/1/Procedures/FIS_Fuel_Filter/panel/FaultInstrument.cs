// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter.panel.FaultInstrument
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter.panel;

internal class FaultInstrument
{
  private readonly DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label titleLabel;
  private readonly ScalingLabel valueLabel;
  private string NoFault = Resources.Message_NoFault;
  internal readonly string faultCodeId;

  public FaultInstrument(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label titleLabel, ScalingLabel valueLabel, string faultCodeId)
  {
    this.titleLabel = titleLabel;
    this.valueLabel = valueLabel;
    this.faultCodeId = faultCodeId;
  }

  internal void DisplayFaultTitle(FaultCode fault)
  {
    if (fault != null)
      ((Control) this.titleLabel).Text = $"{fault.Text}: ({fault.Number}/{fault.Mode})";
    else
      ((Control) this.titleLabel).Text = string.Empty;
  }

  internal void DisplayFaultText(FaultCodeIncidentCollection faultIncidents)
  {
    ValueState valueState = (ValueState) 0;
    if (faultIncidents != null && faultIncidents.Current != null)
    {
      FaultCodeIncident current = faultIncidents.Current;
      ((Control) this.valueLabel).Text = SapiExtensions.ToStatusString(current);
      if (SapiManager.IsFaultActionable(current))
        valueState = (ValueState) 3;
    }
    else
    {
      ((Control) this.valueLabel).Text = this.NoFault;
      valueState = (ValueState) 1;
    }
    this.valueLabel.RepresentedState = valueState;
  }

  internal void OnFaultCodeIncidentUpdateHandler(object sender, EventArgs e)
  {
    if (sender is FaultCodeIncidentCollection faultIncidents)
      this.DisplayFaultTitle(faultIncidents.FaultCode);
    this.DisplayFaultText(faultIncidents);
  }
}
