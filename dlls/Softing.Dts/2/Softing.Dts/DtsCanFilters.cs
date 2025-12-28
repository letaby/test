using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsCanFilters : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsCanFilter GetItemByIndex(uint index);

	DtsCanFilter GetItemByName(string name);

	DtsCanFilter CreateItem(string name);

	void RemoveItem(DtsCanFilter item);
}
