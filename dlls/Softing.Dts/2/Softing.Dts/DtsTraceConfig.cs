using System;

namespace Softing.Dts;

public interface DtsTraceConfig : DtsObject, MCDObject, IDisposable
{
	bool ClientCallTrace { get; set; }

	bool ClientCallTraceThreadContext { get; set; }

	bool ClientCallTraceFunctionCalls { get; set; }

	bool ClientCallTraceFunctionParameters { get; set; }

	bool ClientCallTraceExceptions { get; set; }

	bool ClientCallTraceExtendedObjectInfo { get; set; }

	bool ClientCallTraceObjectLifetime { get; set; }

	bool ClientCallTraceFunctionTimings { get; set; }

	bool ClientCallTraceSuppressPointerAddress { get; set; }

	uint TraceMaxFileSize { get; set; }

	bool UseSubDirectory { get; set; }

	bool PduApiCallTrace { get; set; }

	bool PduApiCallTraceLogFunctionParameters { get; set; }

	bool PduApiCallTraceLogVersionInfo { get; set; }

	bool PduApiCallTraceLogComParameterCalls { get; set; }

	bool PduApiCallTraceLogDetails { get; set; }

	bool DebugTrace { get; set; }

	bool LimitNumberOfTraceFiles { get; set; }

	bool LimitNumberOfTraceSessions { get; set; }

	uint MaxNumberOfTraceFiles { get; set; }

	uint MaxNumberOfTraceSessions { get; set; }

	bool UseTracePathForWritingSimFile { get; set; }

	bool AppendSimFileTrace { get; set; }

	bool ClientCallTraceSuppressTimestamps { get; set; }

	bool WriteMicroseconds { get; set; }

	bool UseSystemTracePath { get; set; }

	uint PduApiCallTraceMaxPduSize { get; set; }

	bool ClientCallTraceFunctionReturns { get; set; }

	bool ClientCallTraceSuppressThreadChanges { get; set; }

	bool PduApiCallTraceMergeIntoApiTrace { get; set; }

	bool DebugTraceToDebugOut { get; set; }

	bool DebugTraceToApiTrace { get; set; }

	DtsDebugTraceLevel DebugTraceLevel { get; set; }

	bool JobLogging { get; set; }

	bool RemoteLogging { get; set; }

	string RemoteLoggingAddress { get; set; }

	bool PduApiCallTraceLogEvents { get; set; }
}
