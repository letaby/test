using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbProtocolParameters : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsDbProtocolParameter GetItemByIndex(uint index);

	DtsDbProtocolParameter GetItemByName(string name);
}
