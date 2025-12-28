using System;

namespace Softing.Dts;

public interface MCDMessageFilter : MCDObject, IDisposable
{
	uint FilterCompareSize { get; set; }

	uint FilterId { get; }

	MCDMessageFilterValues FilterMasks { get; }

	MCDMessageFilterValues FilterPatterns { get; }

	MCDMessageFilterType FilterType { get; set; }

	bool IsMessageFilterEnabled { get; }

	void EnableMessageFilter(bool enableMessageFilter);
}
