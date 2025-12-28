using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbTables : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbTable> ToList();

	MCDDbTable[] ToArray();

	MCDDbTable GetItemByIndex(uint index);

	MCDDbTable GetItemByName(string name);
}
