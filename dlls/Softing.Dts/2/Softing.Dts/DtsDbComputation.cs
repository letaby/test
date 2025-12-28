using System;

namespace Softing.Dts;

public interface DtsDbComputation : DtsObject, MCDObject, IDisposable
{
	bool IsCompuDefaultValueValid { get; }

	MCDValue CompuDefaultValue { get; }

	DtsDbCompuScales DbCompuScales { get; }
}
