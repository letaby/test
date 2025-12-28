using System;

namespace Softing.Dts;

public interface DtsDbDiagVariable : DtsDbObject, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	MCDDiagVarType ValueType { get; }

	bool IsReadBeforeWrite { get; }

	string[] DbRelationTypes { get; }
}
