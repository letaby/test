using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDSystemItems : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDSystemItem> ToList();

	MCDSystemItem[] ToArray();

	MCDSystemItem GetItemByIndex(uint index);

	MCDSystemItem GetItemByName(string name);
}
