using System;

namespace Softing.Dts;

public interface MCDDbVehicleConnector : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbVehicleConnectorPins DbVehicleConnectorPins { get; }
}
