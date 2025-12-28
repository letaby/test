using System;
using Softing.Dts;

namespace McdAbstraction;

public class McdMonitoringLink : IDisposable
{
	private MCDMonitoringLink link;

	private bool isLoggingEthernet;

	private bool disposedValue = false;

	internal McdMonitoringLink(MCDMonitoringLink link, string path)
	{
		if (path != null)
		{
			isLoggingEthernet = true;
			((DtsDoIPMonitorLink)link).OpenFileTrace(path);
		}
		this.link = link;
	}

	public void Start()
	{
		try
		{
			if (isLoggingEthernet)
			{
				((DtsDoIPMonitorLink)link).StartFileTrace();
			}
			link.Start();
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "Start");
		}
	}

	public void Stop()
	{
		link.Stop();
		if (isLoggingEthernet)
		{
			((DtsDoIPMonitorLink)link).StopFileTrace();
		}
	}

	public string[] FetchMonitoringFrames(int numberRequired)
	{
		try
		{
			return link.FetchMonitoringFrames((uint)numberRequired);
		}
		catch (MCDException ex)
		{
			throw new McdException(ex, "FetchMonitoringFrame");
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing && link != null)
		{
			if (isLoggingEthernet)
			{
				((DtsDoIPMonitorLink)link).CloseFileTrace();
			}
			McdRoot.RemoveMonitoringLink(link);
			link.Dispose();
			link = null;
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
