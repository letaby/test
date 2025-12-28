using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbDiagVariables : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsDbDiagVariable GetItemByIndex(uint index);

	DtsDbDiagVariable GetItemByName(string name);
}
