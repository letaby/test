using System;

namespace Softing.Dts;

public interface DtsLogicalLinkConfig : DtsNamedObjectConfig, DtsObject, MCDObject, IDisposable
{
	string AccessKey { get; }

	string PhysicalVehicleLink { get; set; }

	bool Manual { get; set; }

	bool IsGateway { get; set; }

	string Gateway { get; set; }
}
