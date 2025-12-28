using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbVehicleInformations : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbVehicleInformation> ToList();

	MCDDbVehicleInformation[] ToArray();

	MCDDbVehicleInformation GetItemByIndex(uint index);

	MCDDbVehicleInformation GetItemByName(string name);
}
