using System;

namespace Softing.Dts;

public interface DtsVehicleInformationConfig : DtsNamedObjectConfig, DtsObject, MCDObject, IDisposable
{
	DtsLinkMappings LinkMappings { get; }

	DtsLogicalLinkConfigs LogicalLinkConfigs { get; }
}
