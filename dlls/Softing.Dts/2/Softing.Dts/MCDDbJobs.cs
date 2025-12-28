using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbJobs : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbJob> ToList();

	MCDDbJob[] ToArray();

	MCDDbJob GetItemByIndex(uint index);

	MCDDbJob GetItemByName(string name);
}
