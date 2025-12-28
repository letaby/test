using System;
using System.Collections;

namespace Softing.Dts;

public interface MCDMessageFilterValues : MCDValues, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	void Remove(MCDValue filterValue);

	void RemoveAll();

	void RemoveByIndex(uint index);

	void Add(MCDValue filterValue);
}
