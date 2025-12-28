using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDWriteDiagComPrimitives : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDiagComPrimitive> ToList();

	MCDDiagComPrimitive[] ToArray();

	MCDDiagComPrimitive GetItemByIndex(uint index);
}
