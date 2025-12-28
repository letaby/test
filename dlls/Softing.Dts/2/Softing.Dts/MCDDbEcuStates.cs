using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEcuStates : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbEcuState> ToList();

	MCDDbEcuState[] ToArray();

	MCDDbEcuState GetItemByIndex(uint index);

	MCDDbEcuState GetItemByName(string name);
}
