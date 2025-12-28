using System;

namespace Softing.Dts;

public interface MCDDbJob : MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDVersion Version { get; }

	bool IsReducedResultEnabled { get; }

	MCDDbCodeInformations DbCodeInformations { get; }
}
