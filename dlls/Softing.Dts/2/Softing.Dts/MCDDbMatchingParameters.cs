using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbMatchingParameters : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbMatchingParameter> ToList();

	MCDDbMatchingParameter[] ToArray();

	MCDDbMatchingParameter GetItemByIndex(uint index);
}
