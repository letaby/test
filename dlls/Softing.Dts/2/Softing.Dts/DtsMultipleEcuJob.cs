using System;

namespace Softing.Dts;

public interface DtsMultipleEcuJob : MCDMultipleEcuJob, MCDJob, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsJob, DtsDataPrimitive, DtsDiagComPrimitive, DtsObject
{
}
