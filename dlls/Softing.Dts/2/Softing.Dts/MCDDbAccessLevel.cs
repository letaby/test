using System;

namespace Softing.Dts;

public interface MCDDbAccessLevel : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbDiagComPrimitive AccessDiagComPrimitive { get; }

	uint AccessLevelValue { get; }

	string ExternalAccessMethod { get; }

	bool HasExternalAccessMethod { get; }
}
