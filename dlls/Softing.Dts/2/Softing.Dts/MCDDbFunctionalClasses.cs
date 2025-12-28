using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFunctionalClasses : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFunctionalClass> ToList();

	MCDDbFunctionalClass[] ToArray();

	MCDDbFunctionalClass GetItemByIndex(uint index);

	MCDDbFunctionalClass GetItemByName(string name);
}
