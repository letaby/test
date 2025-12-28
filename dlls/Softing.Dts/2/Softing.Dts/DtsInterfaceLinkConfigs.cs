using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsInterfaceLinkConfigs : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsInterfaceLinkConfig GetItemByIndex(uint index);

	DtsInterfaceLinkConfig CreateInterfaceLink();

	void Remove(DtsInterfaceLinkConfig link);
}
