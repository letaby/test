using System;
using System.Collections;

namespace Softing.Dts;

public interface MCDDbFunctionInParameters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	MCDDbLocations GetDbLocationsForItemByIndex(uint index);

	MCDDbLocations GetDbLocationsForItemByName(string name);

	MCDDbFunctionInParameter GetItemByIndex(uint index, MCDDbLocation locationContext);

	MCDDbFunctionInParameter GetItemByName(string name, MCDDbLocation locationContext);
}
