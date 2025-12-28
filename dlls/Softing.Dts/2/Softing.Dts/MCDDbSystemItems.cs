using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbSystemItems : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbSystemItem> ToList();

	MCDDbSystemItem[] ToArray();

	MCDDbSystemItem GetItemByIndex(uint index);

	MCDDbSystemItem GetItemByName(string name);
}
