using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbODXFiles : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsDbODXFile GetItemByIndex(uint index);
}
