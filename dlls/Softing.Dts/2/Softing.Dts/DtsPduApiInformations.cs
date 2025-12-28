using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsPduApiInformations : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsPduApiInformation GetItemByIndex(uint index);
}
