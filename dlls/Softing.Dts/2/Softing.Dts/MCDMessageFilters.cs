using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDMessageFilters : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDMessageFilter> ToList();

	MCDMessageFilter[] ToArray();

	MCDMessageFilter AddByFilterType(MCDMessageFilterType messageFilterType);

	MCDMessageFilter GetItemByIndex(uint index);

	void Remove(MCDMessageFilter messageFilter);

	void RemoveAll();

	void RemoveByIndex(uint index);
}
