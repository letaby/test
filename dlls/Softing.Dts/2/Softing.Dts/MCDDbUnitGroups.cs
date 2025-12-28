using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbUnitGroups : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbUnitGroup> ToList();

	MCDDbUnitGroup[] ToArray();

	MCDDbUnitGroup GetItemByIndex(uint index);

	MCDDbUnitGroup GetItemByName(string name);
}
