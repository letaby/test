using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbDiagServices : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbDiagService> ToList();

	MCDDbDiagService[] ToArray();

	MCDDbDiagService GetItemByIndex(uint index);

	MCDDbDiagService GetItemByName(string name);
}
