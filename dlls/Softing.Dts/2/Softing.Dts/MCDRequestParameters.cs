using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDRequestParameters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDRequestParameter> ToList();

	MCDRequestParameter[] ToArray();

	MCDRequestParameter GetItemByIndex(uint index);

	MCDRequestParameter GetItemByName(string name);
}
