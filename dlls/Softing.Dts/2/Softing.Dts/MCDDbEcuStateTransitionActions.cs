using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEcuStateTransitionActions : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbEcuStateTransitionAction> ToList();

	MCDDbEcuStateTransitionAction[] ToArray();

	MCDDbEcuStateTransitionAction GetItemByIndex(uint index);
}
