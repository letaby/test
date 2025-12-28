using System;

namespace Softing.Dts;

public interface MCDDbVehicleConnectorPin : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbVehicleConnector DbVehicleConnector { get; }

	ushort PinNumber { get; }
}
