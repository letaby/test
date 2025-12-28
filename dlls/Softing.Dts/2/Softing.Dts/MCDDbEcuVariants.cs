using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEcuVariants : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbEcuVariant> ToList();

	MCDDbEcuVariant[] ToArray();

	MCDDbEcuVariant GetItemByIndex(uint index);

	MCDDbEcuVariant GetItemByName(string name);
}
