using System;

namespace Softing.Dts;

public interface MCDScaleConstraint : MCDObject, IDisposable
{
	string Description { get; }

	string DescriptionID { get; }

	MCDInterval Interval { get; }

	MCDRangeInfo RangeInfo { get; }

	string ShortLabel { get; }

	string ShortLabelID { get; }

	bool IsComputed { get; }
}
