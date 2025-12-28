using System;

namespace Softing.Dts;

public interface DtsDbPhysicalVehicleLinkOrInterface : MCDDbPhysicalVehicleLinkOrInterface, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	DtsPhysicalLinkOrInterfaceType PILType { get; }

	MCDDbRequestParameters CommunicationParameters { get; }

	uint PhysicalLinkId { get; }

	string PhysicalLinkKey { get; }
}
