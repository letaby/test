using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDErrors : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDError> ToList();

	MCDError[] ToArray();

	MCDError GetItemByIndex(uint index);
}
