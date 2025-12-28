using System;

namespace Softing.Dts;

public interface DtsRawService : DtsDiagService, MCDDiagService, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsDataPrimitive, DtsDiagComPrimitive, DtsObject
{
	MCDValue RawData { get; }

	void EnterRawData(MCDValue rawData);
}
