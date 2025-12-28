namespace SapiLayer1;

public sealed class LogFileChannelCollection : ChannelBaseCollection
{
	private LogFile parent;

	public LogFile LogFile => parent;

	internal new object SyncRoot => base.Items;

	internal LogFileChannelCollection(LogFile parent)
	{
		this.parent = parent;
	}

	internal void Add(Channel c)
	{
		base.Items.Add(c);
		RaiseConnectCompleteEvent(c, null);
	}

	internal bool ChannelExists(Channel c)
	{
		return base.Items.IndexOf(c) != -1;
	}
}
