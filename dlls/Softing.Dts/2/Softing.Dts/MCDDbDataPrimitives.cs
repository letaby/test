using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbDataPrimitives : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbDataPrimitive> ToList();

	MCDDbDataPrimitive[] ToArray();

	MCDDbDataPrimitive GetItemByIndex(uint index);

	MCDDbDataPrimitive GetItemByName(string name);
}
