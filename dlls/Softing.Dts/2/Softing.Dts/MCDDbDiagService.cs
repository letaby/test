using System;

namespace Softing.Dts;

public interface MCDDbDiagService : MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDRuntimeMode RuntimeMode { get; }
}
