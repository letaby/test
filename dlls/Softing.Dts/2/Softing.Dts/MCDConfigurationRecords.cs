using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDConfigurationRecords : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	event OnConfigurationRecordLoaded ConfigurationRecordLoaded;

	List<MCDConfigurationRecord> ToList();

	MCDConfigurationRecord[] ToArray();

	MCDConfigurationRecord GetItemByIndex(uint index);

	MCDConfigurationRecord GetItemByName(string name);

	void Remove(MCDConfigurationRecord ConfigurationRecord);

	void RemoveAll();

	void RemoveByIndex(uint index);

	void RemoveByName(string name);

	MCDConfigurationRecord AddByConfigurationIDAndDbConfigurationData(MCDValue ConfigurationID, MCDDbConfigurationData configurationData);

	MCDConfigurationRecord AddByDbObject(MCDDbConfigurationRecord DbConfigurationRecord);

	MCDConfigurationRecord AddByNameAndDbConfigurationData(string dbConfigurationRecordName, MCDDbConfigurationData dbConfigurationData);
}
