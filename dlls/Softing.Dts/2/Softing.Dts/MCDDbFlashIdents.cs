using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFlashIdents : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFlashIdent> ToList();

	MCDDbFlashIdent[] ToArray();

	MCDDbFlashIdent GetItemByIndex(uint index);

	MCDDbFlashIdent GetItemByName(string name);
}
