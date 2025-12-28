using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsLogicalLinkFilterConfigs : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsLogicalLinkFilterConfig GetItemByIndex(uint index);

	DtsLogicalLinkFilterConfig CreateItem(string filterName);

	void RemoveItem(DtsLogicalLinkFilterConfig filterConfig);

	DtsLogicalLinkFilterConfig GetItemByName(string name);
}
