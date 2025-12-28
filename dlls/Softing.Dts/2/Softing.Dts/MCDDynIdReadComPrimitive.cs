using System;

namespace Softing.Dts;

public interface MCDDynIdReadComPrimitive : MCDDiagService, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	MCDValue DynId { get; set; }

	string DefinitionMode { get; set; }
}
