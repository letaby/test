using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsLinkMappings : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsLinkMapping GetItemByIndex(uint index);

	void RemoveItem(DtsLinkMapping item);

	DtsLinkMapping CreateItem();
}
