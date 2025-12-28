using System;

namespace McdAbstraction;

public class McdDebugInfoEventArgs : EventArgs
{
	public string Message { get; private set; }

	public McdDebugInfoEventArgs(string message)
	{
		Message = message;
	}
}
