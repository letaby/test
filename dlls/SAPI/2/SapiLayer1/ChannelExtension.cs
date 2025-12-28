using System;

namespace SapiLayer1;

public abstract class ChannelExtension : IDisposable
{
	private Channel channel;

	protected Channel Channel => channel;

	~ChannelExtension()
	{
		Dispose(disposing: false);
	}

	public abstract object Invoke(string method, object[] inputs);

	public virtual void PrepareVcp()
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected ChannelExtension(Channel channel)
	{
		this.channel = channel;
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	protected void RaiseExceptionEvent(Exception exception)
	{
		Sapi.GetSapi().RaiseExceptionEvent(this, exception);
	}

	protected void RaiseDebugInfoEvent(string message)
	{
		Sapi.GetSapi().RaiseDebugInfoEvent(this, message);
	}

	protected static void RaiseDebugInfoEvent(object sender, string message)
	{
		Sapi.GetSapi().RaiseDebugInfoEvent(sender, message);
	}
}
