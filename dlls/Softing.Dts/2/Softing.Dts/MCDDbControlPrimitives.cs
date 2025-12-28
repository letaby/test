using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbControlPrimitives : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbControlPrimitive> ToList();

	MCDDbControlPrimitive[] ToArray();

	MCDDbControlPrimitive GetItemByIndex(uint index);

	MCDDbControlPrimitive GetItemByName(string name);

	MCDDbControlPrimitive GetItemByType(MCDObjectType type);
}
