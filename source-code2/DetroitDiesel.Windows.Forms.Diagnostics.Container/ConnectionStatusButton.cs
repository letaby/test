using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal sealed class ConnectionStatusButton : ToolStripButton
{
	private ChannelBaseCollection activeChannels;

	private Image tick;

	private Image cross;

	public ConnectionStatusButton()
	{
		SapiManager.GlobalInstance.ActiveChannelsChanged += sapiManager_ActiveChannelsChanged;
		UpdateActiveChannels();
		Icon icon = new Icon(GetType(), "small icon connected.ico");
		Icon icon2 = new Icon(GetType(), "small icon stop.ico");
		tick = icon.ToBitmap();
		cross = icon2.ToBitmap();
	}

	private void sapiManager_ActiveChannelsChanged(object sender, EventArgs e)
	{
		UpdateActiveChannels();
	}

	private void UpdateActiveChannels()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(OnConnectCompleteEvent);
			activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(OnDisconnectCompleteEvent);
			foreach (Channel activeChannel in activeChannels)
			{
				activeChannel.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(OnFaultCodeCollectionUpdateEvent);
			}
		}
		activeChannels = SapiManager.GlobalInstance.ActiveChannels;
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent += new ConnectCompleteEventHandler(OnConnectCompleteEvent);
			activeChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(OnDisconnectCompleteEvent);
			foreach (Channel activeChannel2 in activeChannels)
			{
				activeChannel2.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(OnFaultCodeCollectionUpdateEvent);
			}
		}
		UpdateButtonState();
	}

	private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		Channel val = (Channel)((sender is Channel) ? sender : null);
		if (val != null)
		{
			val.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(OnFaultCodeCollectionUpdateEvent);
		}
		UpdateButtonState();
	}

	private void OnDisconnectCompleteEvent(object sender, EventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		Channel val = (Channel)((sender is Channel) ? sender : null);
		if (val != null)
		{
			val.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(OnFaultCodeCollectionUpdateEvent);
		}
		UpdateButtonState();
	}

	private void OnFaultCodeCollectionUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateButtonState();
	}

	private void UpdateButtonState()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Invalid comparison between Unknown and I4
		if (activeChannels != null && ((ReadOnlyCollection<Channel>)(object)activeChannels).Count > 0)
		{
			int num = 0;
			foreach (Channel activeChannel in activeChannels)
			{
				foreach (FaultCode faultCode in activeChannel.FaultCodes)
				{
					if (faultCode.FaultCodeIncidents.Current != null && (int)faultCode.FaultCodeIncidents.Current.Active == 1)
					{
						num++;
					}
				}
			}
			ForeColor = ((num > 0) ? Color.Red : SystemColors.ControlText);
			Image = ((num > 0) ? cross : tick);
			Text = string.Format(arg0: (((ReadOnlyCollection<Channel>)(object)activeChannels).Count != 1) ? string.Format(CultureInfo.CurrentCulture, Resources.FormatConnectionStatusPlural, ((ReadOnlyCollection<Channel>)(object)activeChannels).Count) : string.Format(CultureInfo.CurrentCulture, Resources.FormatConnectionStatusSingular, ((ReadOnlyCollection<Channel>)(object)activeChannels).Count), arg1: (num != 1) ? string.Format(CultureInfo.CurrentCulture, Resources.FormatActiveFaultStatusPlural, num) : string.Format(CultureInfo.CurrentCulture, Resources.FormatActiveFaultStatusSingular, num), provider: CultureInfo.CurrentCulture, format: Resources.FormatStatusCombine);
		}
		else
		{
			ForeColor = SystemColors.ControlText;
			Text = string.Format(CultureInfo.CurrentCulture, Resources.FormatStatusCombine, Resources.MessageConnectionStatusNone, Resources.MessageActiveFaultStatusNone);
		}
	}
}
