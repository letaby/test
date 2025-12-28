using System;

namespace Softing.Dts;

public interface MCDDbDynIdClearComPrimitive : MCDDbDiagService, MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string[] DefinitionModes { get; }
}
