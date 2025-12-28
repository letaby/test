using System;

namespace Softing.Dts;

public interface DtsSingleEcuJob : MCDSingleEcuJob, MCDJob, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsJob, DtsDataPrimitive, DtsDiagComPrimitive, DtsObject
{
}
