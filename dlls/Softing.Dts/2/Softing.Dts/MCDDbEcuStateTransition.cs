using System;

namespace Softing.Dts;

public interface MCDDbEcuStateTransition : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbEcuStateTransitionActions DbEcuStateTransitionActions { get; }

	MCDDbExternalAccessMethod DbExternalAccessMethod { get; }

	MCDDbEcuState DbSourceState { get; }

	MCDDbEcuState DbTargetState { get; }

	bool HasDbExternalAccessMethod { get; }
}
