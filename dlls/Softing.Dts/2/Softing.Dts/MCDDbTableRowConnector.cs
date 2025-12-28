using System;

namespace Softing.Dts;

public interface MCDDbTableRowConnector : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbTable DbTable { get; }

	MCDDbTableParameter DbTableRow { get; }
}
