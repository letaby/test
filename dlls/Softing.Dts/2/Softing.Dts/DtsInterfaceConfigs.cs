using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsInterfaceConfigs : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsInterfaceConfig GetItemByIndex(uint index);

	DtsInterfaceConfig CreateInterface();

	void Remove(DtsInterfaceConfig interfaceConfig);
}
