using System;
using Softing.Dts;

public abstract class MCDException : Exception, MCDObject, IDisposable
{
	public abstract MCDError Error { get; }

	public abstract string SourceFile { get; }

	public abstract uint SourceLine { get; }

	public abstract MCDObjectType ObjectType { get; }

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
	}
}
