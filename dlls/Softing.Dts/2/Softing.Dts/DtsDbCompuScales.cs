using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbCompuScales : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsDbCompuScale GetItemByIndex(uint index);
}
