using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFunctionDiagComConnectors : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFunctionDiagComConnector> ToList();

	MCDDbFunctionDiagComConnector[] ToArray();

	MCDDbFunctionDiagComConnector GetItemByIndex(uint index);

	MCDDbFunctionDiagComConnector GetItemByName(string name);
}
