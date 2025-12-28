using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbSpecialDataGroups : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbSpecialDataGroup> ToList();

	MCDDbSpecialDataGroup[] ToArray();

	MCDDbSpecialDataGroup GetItemByIndex(uint index);
}
