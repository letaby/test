// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.ConnectionStatusButton
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal sealed class ConnectionStatusButton : ToolStripButton
{
  private ChannelBaseCollection activeChannels;
  private Image tick;
  private Image cross;

  public ConnectionStatusButton()
  {
    SapiManager.GlobalInstance.ActiveChannelsChanged += new EventHandler(this.sapiManager_ActiveChannelsChanged);
    this.UpdateActiveChannels();
    Icon icon1 = new Icon(this.GetType(), "small icon connected.ico");
    Icon icon2 = new Icon(this.GetType(), "small icon stop.ico");
    this.tick = (Image) icon1.ToBitmap();
    this.cross = (Image) icon2.ToBitmap();
  }

  private void sapiManager_ActiveChannelsChanged(object sender, EventArgs e)
  {
    this.UpdateActiveChannels();
  }

  private void UpdateActiveChannels()
  {
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
      foreach (Channel activeChannel in this.activeChannels)
        activeChannel.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    }
    this.activeChannels = SapiManager.GlobalInstance.ActiveChannels;
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent += new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
      foreach (Channel activeChannel in this.activeChannels)
        activeChannel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    }
    this.UpdateButtonState();
  }

  private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    if (sender is Channel channel)
      channel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    this.UpdateButtonState();
  }

  private void OnDisconnectCompleteEvent(object sender, EventArgs e)
  {
    if (sender is Channel channel)
      channel.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    this.UpdateButtonState();
  }

  private void OnFaultCodeCollectionUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateButtonState();
  }

  private void UpdateButtonState()
  {
    if (this.activeChannels != null && ((ReadOnlyCollection<Channel>) this.activeChannels).Count > 0)
    {
      int num = 0;
      foreach (Channel activeChannel in this.activeChannels)
      {
        foreach (FaultCode faultCode in activeChannel.FaultCodes)
        {
          if (faultCode.FaultCodeIncidents.Current != null && faultCode.FaultCodeIncidents.Current.Active == 1)
            ++num;
        }
      }
      this.ForeColor = num > 0 ? Color.Red : SystemColors.ControlText;
      this.Image = num > 0 ? this.cross : this.tick;
      this.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatStatusCombine, ((ReadOnlyCollection<Channel>) this.activeChannels).Count != 1 ? (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatConnectionStatusPlural, (object) ((ReadOnlyCollection<Channel>) this.activeChannels).Count) : (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatConnectionStatusSingular, (object) ((ReadOnlyCollection<Channel>) this.activeChannels).Count), num != 1 ? (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatActiveFaultStatusPlural, (object) num) : (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatActiveFaultStatusSingular, (object) num));
    }
    else
    {
      this.ForeColor = SystemColors.ControlText;
      this.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatStatusCombine, (object) Resources.MessageConnectionStatusNone, (object) Resources.MessageActiveFaultStatusNone);
    }
  }
}
