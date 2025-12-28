using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEcuMems : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbEcuMem> ToList();

	MCDDbEcuMem[] ToArray();

	MCDDbEcuMem GetItemByIndex(uint index);

	MCDDbEcuMem GetItemByName(string name);
}
