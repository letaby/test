using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal class CycleIgnition
{
	internal enum State
	{
		NotRunning,
		WaitingForIgnitionOff,
		WaitingWhileIgnitionOff,
		WaitingForIgnitionOn,
		Complete
	}

	private Channel channel;

	private State state;

	public Channel Channel => channel;

	public bool Succeeded => state == State.Complete;

	public CycleIgnition(Channel channel)
	{
		this.channel = channel;
	}

	public void ShowDialog()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		ConnectionManager.GlobalInstance.UpdateIgnitionStatus();
		channel.Instruments.AutoRead = true;
		LongOperationForm val = new LongOperationForm(Resources.CycleIgnition_Title, (Action<BackgroundWorker, DoWorkEventArgs>)DoWorkAction, (object)null, true, false);
		val.ShowDialog();
	}

	private void DoWorkAction(BackgroundWorker worker, DoWorkEventArgs args)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Invalid comparison between Unknown and I4
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		state = State.WaitingForIgnitionOff;
		TimeSpan timeSpan = TimeSpan.FromSeconds(10.0);
		DateTime dateTime = DateTime.MinValue;
		while (state != State.Complete && !worker.CancellationPending)
		{
			Thread.Sleep(500);
			switch (state)
			{
			case State.WaitingForIgnitionOff:
				if ((int)ConnectionManager.GlobalInstance.IgnitionStatus == 1)
				{
					worker.ReportProgress(20, Resources.CycleIgnition_WaitingForDisconnection);
					if (this.channel.CommunicationsState == CommunicationsState.Offline)
					{
						dateTime = DateTime.Now;
						state = State.WaitingWhileIgnitionOff;
					}
				}
				else
				{
					worker.ReportProgress(0, Resources.CycleIgnition_TurnIgnitionOff);
				}
				break;
			case State.WaitingWhileIgnitionOff:
				if ((int)ConnectionManager.GlobalInstance.IgnitionStatus == 1)
				{
					int num = (int)(timeSpan - (DateTime.Now - dateTime)).TotalSeconds;
					if (num <= 0)
					{
						state = State.WaitingForIgnitionOn;
					}
					else
					{
						worker.ReportProgress(80 - 40 / timeSpan.Seconds * num, string.Format(CultureInfo.CurrentCulture, Resources.CycleIgnition_FormatWaitingForRunoff, num));
					}
				}
				else
				{
					state = State.WaitingForIgnitionOff;
				}
				break;
			case State.WaitingForIgnitionOn:
				if ((int)ConnectionManager.GlobalInstance.IgnitionStatus == 0)
				{
					if (this.channel.CommunicationsState == CommunicationsState.Online)
					{
						state = State.Complete;
					}
				}
				else if (this.channel.CommunicationsState == CommunicationsState.Offline)
				{
					worker.ReportProgress(80, Resources.CycleIgnition_TurnIgnitionOn);
					try
					{
						Channel channel = SapiManager.GlobalInstance.Sapi.Channels.Connect(this.channel.ConnectionResource, synchronous: true);
						if (channel != null)
						{
							this.channel = channel;
						}
					}
					catch (CaesarException)
					{
					}
				}
				else
				{
					worker.ReportProgress(90, Resources.CycleIgnition_WaitingForReconnection);
				}
				break;
			}
		}
	}
}
