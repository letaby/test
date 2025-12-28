using System;

namespace Softing.Dts;

public interface DtsService : MCDService, MCDDiagService, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsDiagService, DtsDataPrimitive, DtsDiagComPrimitive, DtsObject
{
	[Obsolete("Function is marked as deprecated!")]
	MCDRangeInfo GetParameterValueRangeInfo(string shortName);

	void SetParameterValueByRangeInfo(string shortName, MCDRangeInfo rangeInfo);

	MCDResponse GetResponse(MCDDbResponse pDbResponse);
}
