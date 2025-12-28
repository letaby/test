using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbRequestParameters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbRequestParameter> ToList();

	MCDDbRequestParameter[] ToArray();

	MCDDbRequestParameter GetItemByIndex(uint index);

	MCDDbRequestParameter GetItemByName(string name);
}
