using System;

namespace Softing.Dts;

public interface MCDDbSubComponent : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbDiagTroubleCodeConnectors DbDiagTroubleCodeConnectors { get; }

	MCDDbEnvDataConnectors DbEnvDataConnectors { get; }

	MCDDbSubComponentParamConnectors DbSubComponentParamConnectors { get; }

	MCDDbMatchingPatterns DbSubComponentPatterns { get; }

	MCDDbTableRowConnectors DbTableRowConnectors { get; }

	string Semantic { get; }
}
