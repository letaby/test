using System;

namespace Softing.Dts;

public interface MCDConstraint : MCDObject, IDisposable
{
	MCDInterval Interval { get; }

	MCDScaleConstraints ScaleConstraints { get; }

	bool IsComputed { get; }
}
