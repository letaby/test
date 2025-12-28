using System;

namespace Softing.Dts;

public interface MCDDbDiagObjectConnector : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbDiagTroubleCodeConnectors DbDiagTroubleCodeConnectors { get; }

	MCDDbEnvDataConnectors DbEnvDataConnectors { get; }

	MCDDbFunctionDiagComConnectors DbFunctionDiagComConnectors { get; }

	MCDDbTableRowConnectors DbTableRowConnectors { get; }
}
