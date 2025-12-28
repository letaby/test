using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbPhysicalVehicleLinkOrInterfaces : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbPhysicalVehicleLinkOrInterface> ToList();

	MCDDbPhysicalVehicleLinkOrInterface[] ToArray();

	MCDDbPhysicalVehicleLinkOrInterface GetItemByIndex(uint index);

	MCDDbPhysicalVehicleLinkOrInterface GetItemByName(string name);
}
