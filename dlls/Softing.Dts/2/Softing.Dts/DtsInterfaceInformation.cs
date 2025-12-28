using System;

namespace Softing.Dts;

public interface DtsInterfaceInformation : DtsNamedObject, MCDNamedObject, MCDObject, IDisposable, DtsObject
{
	DtsBusSystemInterfaceType BusSystemInterfaceType { get; }

	bool SupportsIpAddress { get; }

	MCDDbInterfaceCables DbInterfaceCables { get; }

	string PDUAPIVersion { get; }

	DtsInterfaceLinkInformations InterfaceLinks { get; }

	string[] VendorModuleNames { get; }

	string DefaultCable { get; }
}
