using System;

namespace Softing.Dts;

public interface MCDDbDiagComPrimitive : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbRequest DbRequest { get; }

	MCDDbResponses DbResponses { get; }

	MCDDbFunctionalClasses DbFunctionalClasses { get; }

	string Semantic { get; }

	MCDTransmissionMode TransmissionMode { get; }

	bool IsApiExecutable { get; }

	bool IsNoOperation { get; }

	MCDDbResponses GetDbResponsesByType(MCDResponseType type);

	MCDDbEcuStateTransitions GetDbEcuStateTransitionsByDbObject(MCDDbEcuStateChart chart);

	MCDDbEcuStateTransitions GetDbEcuStateTransitionsBySemantic(string semantic);

	MCDDbEcuStates GetDbPreConditionStatesByDbObject(MCDDbEcuStateChart chart);

	MCDDbEcuStates GetDbPreConditionStatesBySemantic(string semantic);
}
