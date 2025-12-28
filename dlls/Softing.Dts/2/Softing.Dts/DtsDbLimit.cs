using System;

namespace Softing.Dts;

public interface DtsDbLimit : DtsObject, MCDObject, IDisposable
{
	bool HasIntervalType { get; }

	MCDValue Value { get; }

	MCDLimitType Type { get; }
}
