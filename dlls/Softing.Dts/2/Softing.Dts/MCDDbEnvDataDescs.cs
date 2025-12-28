using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEnvDataDescs : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbEnvDataDesc> ToList();

	MCDDbEnvDataDesc[] ToArray();

	MCDDbEnvDataDesc GetItemByIndex(uint index);

	MCDDbEnvDataDesc GetItemByName(string name);
}
