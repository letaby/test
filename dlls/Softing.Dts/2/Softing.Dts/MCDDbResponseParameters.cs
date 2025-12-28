using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbResponseParameters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbResponseParameter> ToList();

	MCDDbResponseParameter[] ToArray();

	MCDDbResponseParameter GetItemByIndex(uint index);

	MCDDbResponseParameter GetItemByName(string name);
}
