using System;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

internal class StartEngineStep
{
	private BarInstrument barInstrumentEngineSpeed;

	private Action<string> DisplayDirections;

	private Action<TestResults> NotifyComplete;

	public StartEngineStep(BarInstrument barInstrumentEngineSpeed, Action<string> displayDirections, Action<TestResults> notifyComplete)
	{
		this.barInstrumentEngineSpeed = barInstrumentEngineSpeed;
		DisplayDirections = displayDirections;
		NotifyComplete = notifyComplete;
	}

	public void AskTheTechToStartTheEngine()
	{
		barInstrumentEngineSpeed.RepresentedStateChanged += EngineSpeedStateChanged;
		DisplayDirections(Resources.DirectionsStartEngine);
	}

	public void Dispose(bool disposing)
	{
		if (disposing && barInstrumentEngineSpeed != null)
		{
			barInstrumentEngineSpeed.RepresentedStateChanged -= EngineSpeedStateChanged;
		}
	}

	public void Stop()
	{
		if (barInstrumentEngineSpeed != null)
		{
			barInstrumentEngineSpeed.RepresentedStateChanged -= EngineSpeedStateChanged;
		}
		NotifyComplete(TestResults.StopTest);
	}

	private void EngineSpeedStateChanged(object sender, EventArgs e)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		if (barInstrumentEngineSpeed != null && (int)barInstrumentEngineSpeed.RepresentedState == 1)
		{
			barInstrumentEngineSpeed.RepresentedStateChanged -= EngineSpeedStateChanged;
			NotifyComplete(TestResults.Success);
		}
	}
}
