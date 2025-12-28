using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDTextTableElements : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDTextTableElement> ToList();

	MCDTextTableElement[] ToArray();

	MCDTextTableElement GetItemByIndex(uint index);
}
