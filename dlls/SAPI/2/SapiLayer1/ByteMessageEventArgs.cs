using System;

namespace SapiLayer1;

public sealed class ByteMessageEventArgs : EventArgs
{
	private ByteMessageDirection direction;

	private Dump data;

	private DateTime timestamp;

	public ByteMessageDirection Direction => direction;

	public Dump Data => data;

	public DateTime Timestamp => timestamp;

	internal ByteMessageEventArgs(ByteMessageDirection direction, Dump data)
	{
		this.direction = direction;
		this.data = data;
		timestamp = Sapi.Now;
	}
}
