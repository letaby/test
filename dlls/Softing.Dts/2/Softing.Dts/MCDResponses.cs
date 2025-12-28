using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDResponses : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	MCDObject Parent { get; }

	List<MCDResponse> ToList();

	MCDResponse[] ToArray();

	MCDResponse GetItemByIndex(uint index);

	MCDResponse Add(MCDDbLocation pDbLocation, bool isPositive);

	void RemoveByIndex(uint index);

	MCDResponse GetItemByName(string name);
}
