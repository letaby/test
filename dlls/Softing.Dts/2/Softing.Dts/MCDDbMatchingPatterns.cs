using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbMatchingPatterns : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbMatchingPattern> ToList();

	MCDDbMatchingPattern[] ToArray();

	MCDDbMatchingPattern GetItemByIndex(uint index);
}
