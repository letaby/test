using System;

namespace Softing.Dts;

public interface MCDDynIdClearComPrimitive : MCDDiagService, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	MCDValue DynId { get; set; }

	string DefinitionMode { get; set; }
}
