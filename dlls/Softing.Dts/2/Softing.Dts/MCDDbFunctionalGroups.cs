using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFunctionalGroups : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFunctionalGroup> ToList();

	MCDDbFunctionalGroup[] ToArray();

	MCDDbFunctionalGroup GetItemByIndex(uint index);

	MCDDbFunctionalGroup GetItemByName(string name);
}
