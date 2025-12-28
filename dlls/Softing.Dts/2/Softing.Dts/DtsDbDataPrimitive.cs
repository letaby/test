using System;

namespace Softing.Dts;

public interface DtsDbDataPrimitive : MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbDiagComPrimitive, DtsDbObject, DtsNamedObject, DtsObject
{
	uint RepetitionTime { get; }
}
