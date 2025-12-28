using System;

namespace SapiLayer1;

public sealed class CommunicationsStateEventArgs : EventArgs
{
	private CommunicationsState communicationsState;

	public CommunicationsState CommunicationsState => communicationsState;

	internal CommunicationsStateEventArgs(CommunicationsState cs)
	{
		communicationsState = cs;
	}
}
