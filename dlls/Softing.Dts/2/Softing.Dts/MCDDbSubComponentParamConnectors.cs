using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbSubComponentParamConnectors : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbSubComponentParamConnector> ToList();

	MCDDbSubComponentParamConnector[] ToArray();

	MCDDbSubComponentParamConnector GetItemByIndex(uint index);

	MCDDbSubComponentParamConnector GetItemByName(string name);
}
