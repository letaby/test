using System;

namespace SapiLayer1;

public sealed class DebugInfoEventArgs : EventArgs
{
	private string message;

	private DateTime timestamp;

	public string Message => message;

	public DateTime Timestamp => timestamp;

	internal DebugInfoEventArgs(string msg)
	{
		message = msg;
		timestamp = Sapi.Now;
	}
}
