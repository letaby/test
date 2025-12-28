using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbItemValues : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbItemValue> ToList();

	MCDDbItemValue[] ToArray();

	MCDDbItemValue GetItemByIndex(uint index);

	MCDDbItemValue GetItemByKey(string key);

	MCDDbItemValue GetItemByPhysicalConstantValue(MCDValue physicalConstantValue);
}
