using System;

namespace Softing.Dts;

public interface MCDDbDynIdDefineComPrimitive : MCDDbDiagService, MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string[] DefinitionModes { get; }
}
