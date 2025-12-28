using System;

namespace Softing.Dts;

public interface MCDDbInterfaceConnectorPin : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	uint PinNumber { get; }

	uint PinNumberOnVCI { get; }

	MCDConnectorPinType PinType { get; }
}
