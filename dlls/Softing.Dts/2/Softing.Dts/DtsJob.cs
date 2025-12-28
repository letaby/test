using System;

namespace Softing.Dts;

public interface DtsJob : MCDJob, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsDataPrimitive, DtsDiagComPrimitive, DtsObject
{
}
