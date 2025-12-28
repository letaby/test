using System;

namespace Softing.Dts;

public interface DtsDelay : DtsControlPrimitive, MCDControlPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable, DtsDiagComPrimitive, DtsObject
{
	uint TimeDelay { get; set; }
}
