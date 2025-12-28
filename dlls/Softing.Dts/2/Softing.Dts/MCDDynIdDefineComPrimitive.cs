using System;

namespace Softing.Dts;

public interface MCDDynIdDefineComPrimitive : MCDDiagService, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	string[] DynIdParams { set; }

	MCDValue DynId { get; set; }

	string DefinitionMode { get; set; }
}
