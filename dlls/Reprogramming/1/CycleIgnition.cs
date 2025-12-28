// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.CycleIgnition
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

internal class CycleIgnition
{
  private Channel channel;
  private CycleIgnition.State state;

  public CycleIgnition(Channel channel) => this.channel = channel;

  public void ShowDialog()
  {
    ConnectionManager.GlobalInstance.UpdateIgnitionStatus();
    this.channel.Instruments.AutoRead = true;
    int num = (int) new LongOperationForm(Resources.CycleIgnition_Title, new Action<BackgroundWorker, DoWorkEventArgs>(this.DoWorkAction), (object) null, true, false).ShowDialog();
  }

  public Channel Channel => this.channel;

  public bool Succeeded => this.state == CycleIgnition.State.Complete;

  private void DoWorkAction(BackgroundWorker worker, DoWorkEventArgs args)
  {
    this.state = CycleIgnition.State.WaitingForIgnitionOff;
    TimeSpan timeSpan = TimeSpan.FromSeconds(10.0);
    DateTime dateTime = DateTime.MinValue;
    while (this.state != CycleIgnition.State.Complete && !worker.CancellationPending)
    {
      Thread.Sleep(500);
      switch (this.state)
      {
        case CycleIgnition.State.WaitingForIgnitionOff:
          if (ConnectionManager.GlobalInstance.IgnitionStatus == 1)
          {
            worker.ReportProgress(20, (object) Resources.CycleIgnition_WaitingForDisconnection);
            if (this.channel.CommunicationsState == CommunicationsState.Offline)
            {
              dateTime = DateTime.Now;
              this.state = CycleIgnition.State.WaitingWhileIgnitionOff;
              continue;
            }
            continue;
          }
          worker.ReportProgress(0, (object) Resources.CycleIgnition_TurnIgnitionOff);
          continue;
        case CycleIgnition.State.WaitingWhileIgnitionOff:
          if (ConnectionManager.GlobalInstance.IgnitionStatus == 1)
          {
            int totalSeconds = (int) (timeSpan - (DateTime.Now - dateTime)).TotalSeconds;
            if (totalSeconds <= 0)
            {
              this.state = CycleIgnition.State.WaitingForIgnitionOn;
              continue;
            }
            worker.ReportProgress(80 /*0x50*/ - 40 / timeSpan.Seconds * totalSeconds, (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CycleIgnition_FormatWaitingForRunoff, (object) totalSeconds));
            continue;
          }
          this.state = CycleIgnition.State.WaitingForIgnitionOff;
          continue;
        case CycleIgnition.State.WaitingForIgnitionOn:
          if (ConnectionManager.GlobalInstance.IgnitionStatus == null)
          {
            if (this.channel.CommunicationsState == CommunicationsState.Online)
            {
              this.state = CycleIgnition.State.Complete;
              continue;
            }
            continue;
          }
          if (this.channel.CommunicationsState == CommunicationsState.Offline)
          {
            worker.ReportProgress(80 /*0x50*/, (object) Resources.CycleIgnition_TurnIgnitionOn);
            try
            {
              Channel channel = SapiManager.GlobalInstance.Sapi.Channels.Connect(this.channel.ConnectionResource, true);
              if (channel != null)
              {
                this.channel = channel;
                continue;
              }
              continue;
            }
            catch (CaesarException ex)
            {
              continue;
            }
          }
          else
          {
            worker.ReportProgress(90, (object) Resources.CycleIgnition_WaitingForReconnection);
            continue;
          }
        default:
          continue;
      }
    }
  }

  internal enum State
  {
    NotRunning,
    WaitingForIgnitionOff,
    WaitingWhileIgnitionOff,
    WaitingForIgnitionOn,
    Complete,
  }
}
