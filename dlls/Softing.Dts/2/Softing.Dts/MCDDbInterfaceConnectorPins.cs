using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbInterfaceConnectorPins : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbInterfaceConnectorPin> ToList();

	MCDDbInterfaceConnectorPin[] ToArray();

	MCDDbInterfaceConnectorPin GetItemByIndex(uint index);

	MCDDbInterfaceConnectorPin GetItemByName(string name);
}
