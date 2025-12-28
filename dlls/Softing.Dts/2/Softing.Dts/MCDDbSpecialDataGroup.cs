using System;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbSpecialDataGroup : MCDDbSpecialData, MCDObject, IDisposable
{
	uint Count { get; }

	MCDDbSpecialDataGroupCaption Caption { get; }

	bool HasCaption { get; }

	List<MCDDbSpecialData> ToList();

	MCDDbSpecialData[] ToArray();

	MCDDbSpecialData GetItemByIndex(uint index);
}
