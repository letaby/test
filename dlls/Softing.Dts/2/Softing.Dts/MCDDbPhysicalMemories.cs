using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbPhysicalMemories : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbPhysicalMemory> ToList();

	MCDDbPhysicalMemory[] ToArray();

	MCDDbPhysicalMemory GetItemByIndex(uint index);

	MCDDbPhysicalMemory GetItemByName(string name);
}
