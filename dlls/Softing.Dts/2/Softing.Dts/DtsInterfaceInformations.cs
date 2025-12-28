using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsInterfaceInformations : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsInterfaceInformation GetItemByIndex(uint index);
}
