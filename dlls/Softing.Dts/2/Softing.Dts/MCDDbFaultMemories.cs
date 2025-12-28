using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFaultMemories : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFaultMemory> ToList();

	MCDDbFaultMemory[] ToArray();

	MCDDbFaultMemory GetItemByIndex(uint index);

	MCDDbFaultMemory GetItemByName(string name);
}
