using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFlashSessionClasses : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFlashSessionClass> ToList();

	MCDDbFlashSessionClass[] ToArray();

	MCDDbFlashSessionClass GetItemByIndex(uint index);

	MCDDbFlashSessionClass GetItemByName(string name);
}
