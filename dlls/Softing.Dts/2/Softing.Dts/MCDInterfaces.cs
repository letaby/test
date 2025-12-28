using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDInterfaces : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDInterface> ToList();

	MCDInterface[] ToArray();

	MCDInterface GetItemByIndex(uint index);

	MCDInterface GetItemByName(string name);
}
