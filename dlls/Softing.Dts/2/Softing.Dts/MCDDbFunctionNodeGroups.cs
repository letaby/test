using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFunctionNodeGroups : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFunctionNodeGroup> ToList();

	MCDDbFunctionNodeGroup[] ToArray();

	MCDDbFunctionNodeGroup GetItemByIndex(uint index);

	MCDDbFunctionNodeGroup GetItemByName(string name);
}
