using System;
using Softing.Dts;

namespace McdAbstraction;

public class McdFlashJob : IDisposable
{
	private MCDFlashJob job;

	private MCDLogicalLink link;

	private bool disposedValue = false;

	public byte Progress => job.Progress;

	public bool Running => job.State == MCDDiagComPrimitiveState.ePENDING;

	internal McdFlashJob(MCDLogicalLink link, MCDFlashJob flashJob)
	{
		job = flashJob;
		this.link = link;
	}

	public void Execute()
	{
		try
		{
			job.ExecuteAsync();
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "Execute");
		}
	}

	public void FetchResults()
	{
		using MCDResult mCDResult = job.FetchResults(0).GetItemByIndex(0u);
		if (mCDResult.HasError)
		{
			throw new McdException(mCDResult.Error, "FetchResults");
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				link.RemoveDiagComPrimitive(job);
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
