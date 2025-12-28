using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdInterface : IDisposable
{
	private MCDInterface theInterface;

	private IEnumerable<McdInterfaceResource> resources;

	private bool disposedValue = false;

	public string Qualifier => theInterface.ShortName;

	public string Name => theInterface.LongName;

	public string HardwareName => theInterface.HardwareName;

	public IEnumerable<McdInterfaceResource> Resources
	{
		get
		{
			if (resources == null && theInterface != null)
			{
				resources = (from r in theInterface.InterfaceResources.OfType<MCDInterfaceResource>()
					select new McdInterfaceResource(r)).ToList();
			}
			return resources;
		}
	}

	internal MCDInterface Handle => theInterface;

	internal McdInterface(MCDInterface theInterface)
	{
		this.theInterface = theInterface;
		if (this.theInterface.Status == MCDInterfaceStatus.eAVAILABLE)
		{
			this.theInterface.Connect();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing && theInterface != null)
		{
			if (theInterface.Status == MCDInterfaceStatus.eREADY)
			{
				theInterface.Disconnect();
			}
			theInterface.Dispose();
			theInterface = null;
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
