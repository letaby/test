using System;

namespace Softing.Dts;

public interface MCDDbInterfaceCable : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbInterfaceConnectorPins DbInterfaceConnectorPins { get; }

	string InterfaceConnectorType { get; }
}
