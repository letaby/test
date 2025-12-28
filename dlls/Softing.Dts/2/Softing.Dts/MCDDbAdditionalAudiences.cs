using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbAdditionalAudiences : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbAdditionalAudience> ToList();

	MCDDbAdditionalAudience[] ToArray();

	MCDDbAdditionalAudience GetItemByIndex(uint uIndex);

	MCDDbAdditionalAudience GetItemByName(string name);
}
