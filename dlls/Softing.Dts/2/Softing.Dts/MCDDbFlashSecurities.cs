using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFlashSecurities : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFlashSecurity> ToList();

	MCDDbFlashSecurity[] ToArray();

	MCDDbFlashSecurity GetItemByIndex(uint index);

	MCDDbFlashSecurity GetItemByName(string name);
}
