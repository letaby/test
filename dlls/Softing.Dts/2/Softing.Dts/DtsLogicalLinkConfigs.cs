using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsLogicalLinkConfigs : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsLogicalLinkConfig GetItemByIndex(uint index);
}
