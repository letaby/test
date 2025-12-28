using System;

namespace Softing.Dts;

public interface DtsDiagComPrimitive : MCDDiagComPrimitive, MCDObject, IDisposable, DtsObject
{
	MCDResultState ResultState { get; }

	string InternalShortName { get; }

	MCDResultState ExecuteSyncWithResultState();

	MCDResults ExecuteSyncWithResults();

	void ExecuteAsync();

	MCDResults FetchResults(int numReq);
}
