using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbVehicleConnectorPins : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbVehicleConnectorPin> ToList();

	MCDDbVehicleConnectorPin[] ToArray();

	MCDDbVehicleConnectorPin GetItemByIndex(uint index);

	MCDDbVehicleConnectorPin GetItemByName(string name);
}
