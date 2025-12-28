using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsGlobalProtocolParameters : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsGlobalProtocolParameter GetItemByName(string name);

	DtsGlobalProtocolParameter GetItemByIndex(uint index);
}
