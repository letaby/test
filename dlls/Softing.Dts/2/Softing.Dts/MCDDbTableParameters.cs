using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbTableParameters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbTableParameter> ToList();

	MCDDbTableParameter[] ToArray();

	MCDDbTableParameter GetItemByIndex(uint index);

	MCDDbTableParameter GetItemByKey(MCDValue key);

	MCDDbTableParameter GetItemByName(string name);
}
