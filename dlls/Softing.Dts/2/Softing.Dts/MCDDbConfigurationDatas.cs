using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbConfigurationDatas : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbConfigurationData> ToList();

	MCDDbConfigurationData[] ToArray();

	MCDDbConfigurationData GetItemByIndex(uint index);

	MCDDbConfigurationData GetItemByName(string name);
}
