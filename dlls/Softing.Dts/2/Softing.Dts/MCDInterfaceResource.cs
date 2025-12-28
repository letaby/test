using System;

namespace Softing.Dts;

public interface MCDInterfaceResource : MCDNamedObject, MCDObject, IDisposable
{
	MCDDbPhysicalVehicleLinkOrInterface DbPhysicalInterfaceLink { get; }

	MCDInterface Interface { get; }

	string ProtocolType { get; }

	bool IsAvailable { get; }

	bool IsInUse { get; }
}
