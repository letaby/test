using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDOptionItems : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDOptionItem> ToList();

	MCDOptionItem[] ToArray();

	MCDOptionItem GetItemByIndex(uint index);

	MCDOptionItem GetItemByName(string name);
}
