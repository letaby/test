using System;

namespace Softing.Dts;

public interface MCDDbSubComponentParamConnector : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbDiagComPrimitive DbDiagComPrimitive { get; }

	MCDDbRequestParameters DbSubComponentInParams { get; }

	MCDDbResponseParameters DbSubComponentOutParams { get; }
}
