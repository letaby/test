using System;

namespace SapiLayer1;

public class ConnectCompleteEventArgs : ResultEventArgs
{
	public bool AutoConnect { get; }

	public ChannelOptions ChannelOptions { get; }

	internal ConnectCompleteEventArgs(BackgroundConnect backgroundConnect, Exception exception)
		: base(exception)
	{
		AutoConnect = backgroundConnect?.AutoConnect ?? false;
		ChannelOptions = backgroundConnect?.ChannelOptions ?? ChannelOptions.All;
	}
}
