using System;

namespace Softing.Dts;

public interface DtsResponseParameter : MCDResponseParameter, MCDParameter, MCDNamedObject, MCDObject, IDisposable, DtsParameter, DtsNamedObject, DtsObject
{
	MCDResponseParameters ResponseParameters { get; }
}
