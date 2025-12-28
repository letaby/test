using System;
using System.Collections.Generic;

namespace McdAbstraction;

public class McdByteMessageEventArgs : EventArgs
{
	public IEnumerable<byte> Message { get; private set; }

	public bool IsSend { get; private set; }

	public McdByteMessageEventArgs(IEnumerable<byte> byteMessage, bool isSend)
	{
		Message = byteMessage;
		IsSend = isSend;
	}
}
