using System;

namespace Softing.Dts;

public interface DtsMonitoringLink : MCDMonitoringLink, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	void StartFileTrace();

	void StopFileTrace();

	void OpenFileTrace(string FileName);

	void CloseFileTrace();

	byte[] FetchDtsMonitorFrames(uint numReq);
}
