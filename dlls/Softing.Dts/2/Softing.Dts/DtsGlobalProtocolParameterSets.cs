using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsGlobalProtocolParameterSets : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsGlobalProtocolParameterSet GetItemByName(string name);

	DtsGlobalProtocolParameterSet GetItemByIndex(uint index);
}
