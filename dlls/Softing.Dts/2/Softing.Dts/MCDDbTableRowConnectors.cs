using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbTableRowConnectors : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbTableRowConnector> ToList();

	MCDDbTableRowConnector[] ToArray();

	MCDDbTableRowConnector GetItemByIndex(uint index);

	MCDDbTableRowConnector GetItemByName(string name);
}
