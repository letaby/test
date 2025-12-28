using System;

namespace SapiLayer1;

public sealed class SynchronousCheckEventArgs : EventArgs
{
	private object queuedAction;

	private bool cancel;

	public object QueuedAction => queuedAction;

	public bool Cancel
	{
		get
		{
			return cancel;
		}
		set
		{
			cancel |= value;
		}
	}

	internal SynchronousCheckEventArgs(object queuedAction)
	{
		this.queuedAction = queuedAction;
	}
}
