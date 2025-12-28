using System;

namespace Softing.Dts;

public interface MCDDbPhysicalVehicleLinkOrInterface : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string Type { get; }

	MCDDbVehicleConnectorPins DbVehicleConnectorPins { get; }

	MCDDbInterfaceConnectorPins DbInterfaceConnectorPins { get; }
}
