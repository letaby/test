using System;

namespace Softing.Dts;

public interface MCDDbFunctionDiagComConnector : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbDiagComPrimitive DbDiagComPrimitive { get; }

	MCDDbLogicalLink DbLogicalLink { get; }
}
