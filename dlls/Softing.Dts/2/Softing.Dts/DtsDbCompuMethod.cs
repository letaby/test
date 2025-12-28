using System;

namespace Softing.Dts;

public interface DtsDbCompuMethod : DtsObject, MCDObject, IDisposable
{
	string CategoryName { get; }

	bool IsCompuInternalToPhysValid { get; }

	bool IsCompuPhysToInternalValid { get; }

	DtsDbComputation CompuInternalToPhys { get; }

	DtsDbComputation CompuPhysToInternal { get; }
}
