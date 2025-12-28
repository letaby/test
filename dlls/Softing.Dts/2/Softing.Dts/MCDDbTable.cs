using System;

namespace Softing.Dts;

public interface MCDDbTable : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbDiagComPrimitives DbDiagComPrimitives { get; }

	MCDValues Keys { get; }

	string Semantic { get; }

	MCDDbTableParameters DbTableRows { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	MCDDbDiagComPrimitive GetDbDiagComPrimitiveByConnectorSemantic(string semantic);
}
