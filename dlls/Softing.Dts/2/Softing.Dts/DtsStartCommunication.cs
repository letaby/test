using System;

namespace Softing.Dts;

public interface DtsStartCommunication : MCDStartCommunication, MCDControlPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsControlPrimitive, DtsDiagComPrimitive, DtsObject
{
}
