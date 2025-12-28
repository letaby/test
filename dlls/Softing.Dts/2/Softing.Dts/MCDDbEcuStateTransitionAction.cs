using System;

namespace Softing.Dts;

public interface MCDDbEcuStateTransitionAction : MCDObject, IDisposable
{
	MCDDbDiagComPrimitive DbDiagComPrimitive { get; }

	MCDDbRequestParameter DbRequestParameter { get; }

	MCDValue Value { get; }
}
