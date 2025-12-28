using System;

namespace Softing.Dts;

public interface DtsLinkMapping : DtsObject, MCDObject, IDisposable
{
	string PhysicalVehicleLink { get; set; }

	string PhysicalInterfaceLink { get; set; }
}
