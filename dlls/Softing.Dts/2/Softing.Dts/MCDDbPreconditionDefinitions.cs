using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbPreconditionDefinitions : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbPreconditionDefinition> ToList();

	MCDDbPreconditionDefinition[] ToArray();

	MCDDbPreconditionDefinition GetItemByIndex(uint index);
}
