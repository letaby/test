using System;

namespace Softing.Dts;

public interface MCDDbFaultMemory : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbDiagTroubleCode GetDbDiagTroubleCodeByTroubleCode(uint troublecode);

	MCDDbDiagTroubleCodes GetDbDiagTroubleCodes(ushort levelLowerLimit, ushort levelUpperLimit);
}
