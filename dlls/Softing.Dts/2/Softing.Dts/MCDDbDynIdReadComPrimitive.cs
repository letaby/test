using System;

namespace Softing.Dts;

public interface MCDDbDynIdReadComPrimitive : MCDDbDiagService, MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string[] DefinitionModes { get; }
}
