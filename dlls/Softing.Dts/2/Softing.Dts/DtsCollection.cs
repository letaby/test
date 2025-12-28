using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsCollection : MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	MCDObject GetObjectItemByIndex(uint index);
}
