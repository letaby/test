using System;

namespace Softing.Dts;

public interface DtsProtocolParameterSet : MCDProtocolParameterSet, MCDControlPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsControlPrimitive, DtsDiagComPrimitive, DtsObject
{
	DtsProtocolParameterLevel GetParameterLevel(string shortName);
}
