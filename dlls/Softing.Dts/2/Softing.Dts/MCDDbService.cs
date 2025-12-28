using System;

namespace Softing.Dts;

public interface MCDDbService : MCDDbDiagService, MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDAddressingMode AddressingMode { get; }
}
