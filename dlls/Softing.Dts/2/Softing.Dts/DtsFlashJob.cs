using System;

namespace Softing.Dts;

public interface DtsFlashJob : MCDFlashJob, MCDJob, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsJob, DtsDataPrimitive, DtsDiagComPrimitive, DtsObject
{
}
