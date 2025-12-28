using System;

namespace SapiLayer1;

public sealed class StateChangedEventArgs : EventArgs
{
	private ConnectionState state;

	public ConnectionState State => state;

	internal StateChangedEventArgs(ConnectionState state)
	{
		this.state = state;
	}
}
