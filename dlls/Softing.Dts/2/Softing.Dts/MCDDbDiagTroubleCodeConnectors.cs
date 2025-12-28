using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbDiagTroubleCodeConnectors : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbDiagTroubleCodeConnector> ToList();

	MCDDbDiagTroubleCodeConnector[] ToArray();

	MCDDbDiagTroubleCodeConnector GetItemByIndex(uint index);

	MCDDbDiagTroubleCodeConnector GetItemByName(string name);
}
