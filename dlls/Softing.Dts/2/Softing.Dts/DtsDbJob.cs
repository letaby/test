using System;

namespace Softing.Dts;

public interface DtsDbJob : MCDDbJob, MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbDataPrimitive, DtsDbDiagComPrimitive, DtsDbObject, DtsNamedObject, DtsObject
{
	string ParentName { get; }

	string SourceFilePath { get; }
}
