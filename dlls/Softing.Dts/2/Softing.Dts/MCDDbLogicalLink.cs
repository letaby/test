using System;

namespace Softing.Dts;

public interface MCDDbLogicalLink : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbLocation DbLocation { get; }

	MCDDbPhysicalVehicleLinkOrInterface DbPhysicalVehicleLinkOrInterface { get; }

	bool IsAccessedViaGateway { get; }

	MCDGatewayMode GatewayMode { get; }

	MCDDbLogicalLinks DbLogicalLinksOfGateways { get; }

	MCDDbPhysicalVehicleLink DbPhysicalVehicleLink { get; }

	string ProtocolType { get; }
}
