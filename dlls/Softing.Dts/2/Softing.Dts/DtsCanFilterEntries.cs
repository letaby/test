using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsCanFilterEntries : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsCanFilterEntry GetItemByIndex(uint index);

	DtsCanFilterEntry CreateItem();

	void RemoveItem(DtsCanFilterEntry entry);
}
