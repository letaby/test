using System;

namespace Softing.Dts;

public interface MCDDbPreconditionDefinition : MCDObject, IDisposable
{
	MCDDbDiagComPrimitive DbDiagComPrimitive { get; }

	MCDDbRequestParameter DbRequestParameter { get; }

	MCDValue Value { get; }
}
