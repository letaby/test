using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEnvDataConnectors : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbEnvDataConnector> ToList();

	MCDDbEnvDataConnector[] ToArray();

	MCDDbEnvDataConnector GetItemByIndex(uint index);

	MCDDbEnvDataConnector GetItemByName(string name);
}
