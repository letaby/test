using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbProjectDescriptions : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbProjectDescription> ToList();

	MCDDbProjectDescription[] ToArray();

	MCDDbProjectDescription GetItemByIndex(uint index);

	MCDDbProjectDescription GetItemByName(string name);
}
