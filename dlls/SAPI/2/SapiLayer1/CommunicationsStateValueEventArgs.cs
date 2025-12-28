using System;

namespace SapiLayer1;

public sealed class CommunicationsStateValueEventArgs : EventArgs
{
	private CommunicationsStateValue communicationsStateValue;

	public CommunicationsStateValue CommunicationsStateValue => communicationsStateValue;

	internal CommunicationsStateValueEventArgs(CommunicationsStateValue cs)
	{
		communicationsStateValue = cs;
	}
}
