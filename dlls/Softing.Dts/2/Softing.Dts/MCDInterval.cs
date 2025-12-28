using System;

namespace Softing.Dts;

public interface MCDInterval : MCDObject, IDisposable
{
	MCDValue LowerLimit { get; }

	MCDLimitType LowerLimitIntervalType { get; }

	MCDValue UpperLimit { get; }

	MCDLimitType UpperLimitIntervalType { get; }
}
