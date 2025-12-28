using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEcuBaseVariants : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbEcuBaseVariant> ToList();

	MCDDbEcuBaseVariant[] ToArray();

	MCDDbEcuBaseVariant GetItemByIndex(uint index);

	MCDDbEcuBaseVariant GetItemByName(string name);
}
