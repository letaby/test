using System;

namespace Softing.Dts;

public interface DtsLogicalLinkMonitor : DtsObject, MCDObject, IDisposable
{
	uint RingBufferSize { set; }

	void AddAllLogicalLinkForMonitoring();

	void RemoveAllLogicalLinkForMonitoring();

	void AddLogicalLinkForMonitoring(string NewLogicalLinkShortName);

	void RemoveLogicalLinkFromMonitoring(string RemoveLogicalLinkShortName);

	void Start();

	void Stop();

	void OpenFileTrace(string FileName, bool bOverwrite);

	void StartFileTrace();

	void StopFileTrace();

	void CloseFileTrace();

	MCDCollection GetLatestEvents(uint uMaxNoOfNewEvents);

	void SetFilter(DtsLogicalLinkFilterConfig filterConfig, bool filterView, bool filterTrace);

	void OpenFileTraceInFolder(string outputFolderPath, string FileName);

	void StartSnapshotModeTracing(uint timeInterval);

	void GenerateSnapshotTrace(string outputPath);

	void TakeSnapshotTrace();

	void StopSnapshotModeTracing();
}
