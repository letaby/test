using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbVehicleConnectors : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbVehicleConnector> ToList();

	MCDDbVehicleConnector[] ToArray();

	MCDDbVehicleConnector GetItemByIndex(uint index);

	MCDDbVehicleConnector GetItemByName(string name);
}
