using System;

namespace Softing.Dts;

public interface DtsDbProject : MCDDbProject, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	DtsDbODXFiles DbODXFiles { get; }

	string RevisionLabel { get; }

	MCDDbConfigurationDatas DbConfigurationDatas { get; }

	DtsProjectType ProjectType { get; }

	string VehicleModelRange { get; }

	DtsIdentifierInfos IdentifierInfos { get; }

	DtsCanFilters CanFilters { get; }

	MCDDbConfigurationDatas LoadNewConfigurationDatasByFileName(string filename, bool permanent);

	DtsIdentifierInfos CreateIdentifierInfos();
}
