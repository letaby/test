using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFunctionDictionaries : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFunctionDictionary> ToList();

	MCDDbFunctionDictionary[] ToArray();

	MCDDbFunctionDictionary GetItemByIndex(uint index);

	MCDDbFunctionDictionary GetItemByName(string name);
}
