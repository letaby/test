using System;

namespace Softing.Dts;

public interface MCDDbVehicleInformation : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbLogicalLinks DbLogicalLinks { get; }

	MCDDbPhysicalVehicleLinkOrInterfaces DbPhysicalVehicleLinkOrInterfaces { get; }

	MCDDbVehicleConnectors DbVehicleConnectors { get; }
}
