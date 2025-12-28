using System;

namespace Softing.Dts;

public interface DtsHexService : MCDHexService, MCDDiagService, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsDiagService, DtsDataPrimitive, DtsDiagComPrimitive, DtsObject
{
	uint ServiceID { get; set; }
}
