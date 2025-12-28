using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbOptionItems : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbOptionItem> ToList();

	MCDDbOptionItem[] ToArray();

	MCDDbOptionItem GetItemByIndex(uint index);

	MCDDbOptionItem GetItemByName(string name);
}
