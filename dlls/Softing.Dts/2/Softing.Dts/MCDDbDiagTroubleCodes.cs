using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbDiagTroubleCodes : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbDiagTroubleCode> ToList();

	MCDDbDiagTroubleCode[] ToArray();

	MCDDbDiagTroubleCode GetItemByIndex(uint index);

	MCDDbDiagTroubleCode GetItemByName(string name);
}
