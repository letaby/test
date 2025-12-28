using System;

namespace Softing.Dts;

public interface MCDResultState : MCDObject, IDisposable
{
	ushort NoOfResults { get; }

	MCDExecutionState ExecutionState { get; }

	MCDRepetitionState RepetitionState { get; }

	MCDLogicalLinkState LogicalLinkState { get; }

	MCDLockState LogicalLinkLockState { get; }
}
