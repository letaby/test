using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDValues : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDValue> ToList();

	MCDValue[] ToArray();

	MCDValue GetItemByIndex(uint index);
}
