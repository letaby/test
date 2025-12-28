using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDInterfaceResources : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDInterfaceResource> ToList();

	MCDInterfaceResource[] ToArray();

	MCDInterfaceResource GetItemByIndex(uint index);

	MCDInterfaceResource GetItemByName(string name);
}
