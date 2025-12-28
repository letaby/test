using System;

namespace Softing.Dts;

public interface DtsMonitorLink : DtsObject, MCDObject, IDisposable
{
	uint TraceFileLimit { get; set; }

	bool FilterForDisplayAndFileFlag { get; set; }

	bool FilterForDisplayFlag { get; set; }

	bool BusloadFlag { get; set; }

	double CurrentBusLoad { get; }

	string CanFilter { set; }

	void Open();

	void Close();

	void OpenTraceFile(string TraceFileName, bool bOverwriteIfFileExists);

	void StartTraceFile();

	void StopTraceFile();

	void CloseTraceFile();

	byte[] GetLastItems(uint uNoOfItems, uint uLastTotalNoOfItems, ref uint puNoOfDeliveredItems, ref uint puTotalNoOfItems);
}
