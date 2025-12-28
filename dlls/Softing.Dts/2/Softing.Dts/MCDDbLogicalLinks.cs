using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbLogicalLinks : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbLogicalLink> ToList();

	MCDDbLogicalLink[] ToArray();

	MCDDbLogicalLink GetItemByIndex(uint index);

	MCDDbLogicalLink GetItemByName(string name);
}
