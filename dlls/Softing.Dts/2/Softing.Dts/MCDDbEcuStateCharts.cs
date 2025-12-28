using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbEcuStateCharts : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	string[] Semantics { get; }

	List<MCDDbEcuStateChart> ToList();

	MCDDbEcuStateChart[] ToArray();

	MCDDbEcuStateChart GetItemByIndex(uint index);

	MCDDbEcuStateChart GetItemByName(string name);

	MCDDbEcuStateChart GetItemBySemanticAttribute(string semantic);
}
