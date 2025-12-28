using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsInterfaceLinkInformations : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsInterfaceLinkInformation GetItemByIndex(uint index);
}
