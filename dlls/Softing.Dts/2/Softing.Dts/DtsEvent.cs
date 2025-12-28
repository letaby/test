using System;

namespace Softing.Dts;

public interface DtsEvent : DtsObject, MCDObject, IDisposable
{
	DtsEventType EventType { get; }

	DtsEventId EventId { get; }

	MCDError Error { get; }

	MCDDiagComPrimitive DiagComPrimitive { get; }

	string JobInfo { get; }

	byte Progress { get; }

	MCDResultState ResultState { get; }

	MCDLockState LockState { get; }

	MCDLogicalLink LogicalLink { get; }

	MCDLogicalLinkState LogicalLinkState { get; }

	string Clamp { get; }

	MCDClampState ClampState { get; }

	MCDMonitoringLink MonitoringLink { get; }

	MCDInterface Interface { get; }

	MCDInterfaceStatus InterfaceStatus { get; }

	MCDConfigurationRecord ConfigurationRecord { get; }

	MCDValues DynIdList { get; }
}
