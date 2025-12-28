using System;

namespace Softing.Dts;

public interface MCDDiagComPrimitive : MCDObject, IDisposable
{
	MCDDbDiagComPrimitive DbObject { get; }

	MCDErrors Errors { get; }

	MCDRequest Request { get; }

	MCDObject Parent { get; }

	uint UniqueRuntimeID { get; }

	MCDDiagComPrimitiveState State { get; }

	event OnPrimitiveBufferOverflow PrimitiveBufferOverflow;

	event OnPrimitiveCanceledDuringExecution PrimitiveCanceledDuringExecution;

	event OnPrimitiveCanceledFromQueue PrimitiveCanceledFromQueue;

	event OnPrimitiveError PrimitiveError;

	event OnPrimitiveHasIntermediateResult PrimitiveHasIntermediateResult;

	event OnPrimitiveHasResult PrimitiveHasResult;

	event OnPrimitiveJobInfo PrimitiveJobInfo;

	event OnPrimitiveProgressInfo PrimitiveProgressInfo;

	event OnPrimitiveRepetitionStopped PrimitiveRepetitionStopped;

	event OnPrimitiveTerminated PrimitiveTerminated;

	void Cancel();

	MCDResult ExecuteSync();

	void ResetToDefaultValue(string parameterName);

	void ResetToDefaultValues();
}
