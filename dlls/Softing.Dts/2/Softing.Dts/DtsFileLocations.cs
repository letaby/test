using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsFileLocations : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsFileLocation GetItemByIndex(uint index);
}
