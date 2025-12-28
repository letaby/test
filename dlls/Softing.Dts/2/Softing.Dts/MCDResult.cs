using System;

namespace Softing.Dts;

public interface MCDResult : MCDObject, IDisposable
{
	MCDResultType Type { get; }

	bool HasError { get; }

	MCDError Error { get; set; }

	MCDResponses Responses { get; }

	string ServiceShortName { get; }

	MCDResultState ResultState { get; }

	ulong RequestEndTime { get; }
}
