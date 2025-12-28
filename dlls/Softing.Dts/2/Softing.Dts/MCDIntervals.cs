using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDIntervals : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDInterval> ToList();

	MCDInterval[] ToArray();

	MCDInterval GetItemByIndex(uint index);
}
