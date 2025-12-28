using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbParameters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbParameter> ToList();

	MCDDbParameter[] ToArray();

	MCDDbParameter GetItemByIndex(uint index);

	MCDDbParameter GetItemByName(string name);
}
