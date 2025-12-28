using System;

namespace SapiLayer1;

public sealed class FatalEventArgs : EventArgs
{
	private int param1;

	private int param2;

	private string message;

	private string fileName;

	private int line;

	private DateTime timestamp;

	public int Param1 => param1;

	public int Param2 => param2;

	public string Message => message;

	public string FileName => fileName;

	public int Line => line;

	public DateTime Timestamp => timestamp;

	internal FatalEventArgs(int param1, int param2, string message, string fileName, int line)
	{
		this.param1 = param1;
		this.param2 = param2;
		this.message = message;
		this.fileName = fileName;
		this.line = line;
		timestamp = Sapi.Now;
	}
}
