using System;

namespace Softing.Dts;

public interface DtsControlPrimitive : MCDControlPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsDiagComPrimitive, DtsObject
{
}
