using System;

namespace Softing.Dts;

public interface DtsDataPrimitive : MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsDiagComPrimitive, DtsObject
{
	void StartCyclicSend(uint cyclicTime, int numSendCycles);
}
