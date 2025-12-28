using System;

namespace Softing.Dts;

public interface MCDDbEcuState : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbEcuStateTransitions DbEcuStateTransitions { get; }

	MCDDbPreconditionDefinitions DbRestrictedDiagComPrimitives { get; }
}
