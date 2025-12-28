using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbDataRecords : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbDataRecord> ToList();

	MCDDbDataRecord[] ToArray();

	MCDDbDataRecord GetItemByIndex(uint index);

	MCDDbDataRecord GetItemByName(string name);

	MCDDbDataRecord GetItemByDataID(MCDValue dataID);

	MCDDbDataRecord GetItemByKey(string key);
}
