using System;

namespace Softing.Dts;

public interface MCDDbPhysicalVehicleLink : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbVehicleConnectorPins DbVehicleConnectorPins { get; }

	string Type { get; }
}
