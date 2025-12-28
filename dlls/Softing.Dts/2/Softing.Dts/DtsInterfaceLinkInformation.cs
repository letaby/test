using System;

namespace Softing.Dts;

public interface DtsInterfaceLinkInformation : DtsObject, MCDObject, IDisposable
{
	MCDDbInterfaceConnectorPins ConnectorPins { get; }

	DtsPhysicalLinkOrInterfaceType LinkType { get; }

	DtsPduApiLinkType PduApiLinkType { get; }

	uint LocalIndex { get; }

	string String { get; }
}
