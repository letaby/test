using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbLocations : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbLocation> ToList();

	MCDDbLocation[] ToArray();

	MCDDbLocation GetItemByIndex(uint index);

	MCDDbLocation GetItemByName(string name);
}
