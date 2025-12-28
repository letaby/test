using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbResponses : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbResponse> ToList();

	MCDDbResponse[] ToArray();

	MCDDbResponse GetItemByIndex(uint index);

	MCDDbResponse GetItemByName(string name);
}
