using System;

namespace Softing.Dts;

public interface DtsResult : MCDResult, MCDObject, IDisposable, DtsObject
{
	MCDValue RequestMessage { get; }

	MCDRequestParameters RequestParameters { get; }

	uint RequestTime { get; }

	uint CANIdentifier { get; }

	string ServiceLongName { get; }

	bool HasRequestMessage { get; }
}
