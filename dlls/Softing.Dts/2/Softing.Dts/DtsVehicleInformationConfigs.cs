using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsVehicleInformationConfigs : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsVehicleInformationConfig GetItemByIndex(uint index);

	void RemoveItem(DtsVehicleInformationConfig vehicleInfoConfig);
}
