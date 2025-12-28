using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbSubComponents : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbSubComponent> ToList();

	MCDDbSubComponent[] ToArray();

	MCDDbSubComponent GetItemByIndex(uint index);

	MCDDbSubComponent GetItemByName(string name);

	MCDDbSubComponents GetItemsBySemanticAttribute(string semantic);
}
