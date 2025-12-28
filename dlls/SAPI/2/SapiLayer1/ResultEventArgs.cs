using System;

namespace SapiLayer1;

public class ResultEventArgs : EventArgs
{
	private Exception exception;

	private DateTime timestamp;

	public Exception Exception => exception;

	public bool Succeeded => exception == null;

	public DateTime Timestamp => timestamp;

	public ResultEventArgs(Exception exception)
	{
		this.exception = exception;
		timestamp = Sapi.Now;
	}
}
