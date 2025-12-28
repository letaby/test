// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel.StartEngineStep
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

internal class StartEngineStep
{
  private BarInstrument barInstrumentEngineSpeed;
  private Action<string> DisplayDirections;
  private Action<TestResults> NotifyComplete;

  public StartEngineStep(
    BarInstrument barInstrumentEngineSpeed,
    Action<string> displayDirections,
    Action<TestResults> notifyComplete)
  {
    this.barInstrumentEngineSpeed = barInstrumentEngineSpeed;
    this.DisplayDirections = displayDirections;
    this.NotifyComplete = notifyComplete;
  }

  public void AskTheTechToStartTheEngine()
  {
    this.barInstrumentEngineSpeed.RepresentedStateChanged += new EventHandler(this.EngineSpeedStateChanged);
    this.DisplayDirections(Resources.DirectionsStartEngine);
  }

  public void Dispose(bool disposing)
  {
    if (!disposing || this.barInstrumentEngineSpeed == null)
      return;
    this.barInstrumentEngineSpeed.RepresentedStateChanged -= new EventHandler(this.EngineSpeedStateChanged);
  }

  public void Stop()
  {
    if (this.barInstrumentEngineSpeed != null)
      this.barInstrumentEngineSpeed.RepresentedStateChanged -= new EventHandler(this.EngineSpeedStateChanged);
    this.NotifyComplete(TestResults.StopTest);
  }

  private void EngineSpeedStateChanged(object sender, EventArgs e)
  {
    if (this.barInstrumentEngineSpeed == null || this.barInstrumentEngineSpeed.RepresentedState != 1)
      return;
    this.barInstrumentEngineSpeed.RepresentedStateChanged -= new EventHandler(this.EngineSpeedStateChanged);
    this.NotifyComplete(TestResults.Success);
  }
}
