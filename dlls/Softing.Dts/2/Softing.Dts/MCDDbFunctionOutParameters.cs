using System;
using System.Collections;

namespace Softing.Dts;

public interface MCDDbFunctionOutParameters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	MCDDbLocations GetDbLocationsForItemByIndex(uint index);

	MCDDbLocations GetDbLocationsForItemByName(string name);

	MCDDbFunctionOutParameter GetItemByIndex(uint index, MCDDbLocation locationContext);

	MCDDbFunctionOutParameter GetItemByName(string name, MCDDbLocation locationContext);
}
