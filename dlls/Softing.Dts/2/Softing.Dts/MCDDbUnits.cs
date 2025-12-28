using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbUnits : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbUnit> ToList();

	MCDDbUnit[] ToArray();

	MCDDbUnit GetItemByIndex(uint index);

	MCDDbUnit GetItemByName(string name);
}
