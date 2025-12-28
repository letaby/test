using System;
using Softing.Dts;

namespace McdAbstraction;

public class McdInterfaceResource : IDisposable
{
	private MCDInterfaceResource theInterfaceResource;

	private bool disposedValue = false;

	public bool IsPassThru { get; }

	public bool IsEthernet { get; }

	public string PhysicalInterfaceLinkType { get; private set; }

	public string ProtocolType { get; private set; }

	public string Qualifier { get; private set; }

	public string Name { get; private set; }

	internal MCDInterfaceResource Handle => theInterfaceResource;

	internal McdInterfaceResource(MCDInterfaceResource theInterfaceResource)
	{
		this.theInterfaceResource = theInterfaceResource;
		ProtocolType = theInterfaceResource.ProtocolType;
		Name = theInterfaceResource.LongName;
		Qualifier = theInterfaceResource.ShortName;
		IsPassThru = theInterfaceResource.Interface.PDUApiSoftwareName == "PDUAPI_DTNA_SID";
		switch (((DtsDbPhysicalVehicleLinkOrInterface)theInterfaceResource.DbPhysicalInterfaceLink).PILType)
		{
		case DtsPhysicalLinkOrInterfaceType.eCAN:
			PhysicalInterfaceLinkType = "CAN";
			break;
		case DtsPhysicalLinkOrInterfaceType.eCANHS:
			PhysicalInterfaceLinkType = "CANHS";
			break;
		case DtsPhysicalLinkOrInterfaceType.eCANLS:
			PhysicalInterfaceLinkType = "CANLS";
			break;
		case DtsPhysicalLinkOrInterfaceType.eETHERNET:
			PhysicalInterfaceLinkType = "ETHERNET";
			IsEthernet = true;
			break;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing && theInterfaceResource != null)
			{
				theInterfaceResource.Dispose();
				theInterfaceResource = null;
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
