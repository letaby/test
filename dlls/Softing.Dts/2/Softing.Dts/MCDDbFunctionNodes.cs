using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFunctionNodes : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFunctionNode> ToList();

	MCDDbFunctionNode[] ToArray();

	MCDDbFunctionNode GetItemByIndex(uint index);

	MCDDbFunctionNode GetItemByName(string name);
}
