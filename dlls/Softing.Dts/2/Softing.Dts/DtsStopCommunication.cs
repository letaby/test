using System;

namespace Softing.Dts;

public interface DtsStopCommunication : MCDStopCommunication, MCDControlPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsControlPrimitive, DtsDiagComPrimitive, DtsObject
{
}
