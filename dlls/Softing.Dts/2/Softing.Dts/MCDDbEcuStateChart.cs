using System;

namespace Softing.Dts;

public interface MCDDbEcuStateChart : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbEcuStates DbEcuStates { get; }

	MCDDbEcuStateTransitions DbEcuStateTransitions { get; }

	MCDDbDiagComPrimitives DbPreConditionStateDiagComPrimitives { get; }

	MCDDbEcuState DbStartState { get; }

	MCDDbDiagComPrimitives DbStateTransitionDiagComPrimitives { get; }

	string Semantic { get; }
}
