using System;

namespace Softing.Dts;

public interface DtsInterfaceLinkConfig : DtsObject, MCDObject, IDisposable
{
	DtsPhysicalLinkOrInterfaceType LinkType { get; set; }

	DtsPduApiLinkType PduApiLinkType { get; set; }

	int GlobalIndex { get; set; }

	int LocalIndex { get; set; }

	uint PinCount { get; }

	string String { get; }

	void Assign(DtsInterfaceLinkInformation linkInformation);

	MCDConnectorPinType GetPinType(uint index);

	uint GetVehiclePin(uint index);
}
