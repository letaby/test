using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbInterfaceCables : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbInterfaceCable> ToList();

	MCDDbInterfaceCable[] ToArray();

	MCDDbInterfaceCable GetItemByIndex(uint index);

	MCDDbInterfaceCable GetItemByName(string name);
}
