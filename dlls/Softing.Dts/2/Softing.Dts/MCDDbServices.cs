using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbServices : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbService> ToList();

	MCDDbService[] ToArray();

	MCDDbService GetItemByIndex(uint index);

	MCDDbService GetItemByName(string name);
}
