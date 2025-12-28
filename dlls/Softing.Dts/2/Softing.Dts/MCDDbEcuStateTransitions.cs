using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEcuStateTransitions : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbEcuStateTransition> ToList();

	MCDDbEcuStateTransition[] ToArray();

	MCDDbEcuStateTransition GetItemByIndex(uint index);

	MCDDbEcuStateTransition GetItemByName(string name);
}
