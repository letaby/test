using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFlashFilters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFlashFilter> ToList();

	MCDDbFlashFilter[] ToArray();

	MCDDbFlashFilter GetItemByIndex(uint index);

	MCDDbFlashFilter GetItemByName(string name);
}
