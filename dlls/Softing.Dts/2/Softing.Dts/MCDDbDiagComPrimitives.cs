using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbDiagComPrimitives : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbDiagComPrimitive> ToList();

	MCDDbDiagComPrimitive[] ToArray();

	MCDDbDiagComPrimitive GetItemByIndex(uint index);

	MCDDbDiagComPrimitive GetItemByName(string name);

	MCDDbDiagComPrimitive GetItemByType(MCDObjectType type);
}
