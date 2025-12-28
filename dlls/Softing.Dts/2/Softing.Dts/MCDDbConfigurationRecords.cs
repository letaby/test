using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbConfigurationRecords : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbConfigurationRecord> ToList();

	MCDDbConfigurationRecord[] ToArray();

	MCDDbConfigurationRecord GetItemByConfigurationID(MCDValue configurationID);

	MCDDbConfigurationRecord GetItemByIndex(uint index);

	MCDDbConfigurationRecord GetItemByName(string name);
}
