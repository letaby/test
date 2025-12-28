using System;

namespace Softing.Dts;

public interface DtsDbLogicalLink : MCDDbLogicalLink, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	MCDDbLogicalLink GatewayDbLogicalLink { get; }

	bool IsGateway { get; }

	uint SourceIdentifier { get; }

	uint TargetIdentifier { get; }

	DtsPhysicalLinkOrInterfaceType CANType { get; }

	MCDDbRequestParameters CommunicationParameters { get; }

	bool ViaGateway();
}
