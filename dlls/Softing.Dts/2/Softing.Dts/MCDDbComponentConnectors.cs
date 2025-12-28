using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbComponentConnectors : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbComponentConnector> ToList();

	MCDDbComponentConnector[] ToArray();

	MCDDbComponentConnector GetItemByIndex(uint index);
}
