using System;

namespace Softing.Dts;

public interface MCDSystem : MCDNamedObject, MCDObject, IDisposable
{
	MCDProject ActiveProject { get; }

	MCDVersion ASAMMCDVersion { get; }

	MCDDbProjectDescriptions DbProjectDescriptions { get; }

	MCDLockState LockState { get; }

	MCDServerType ServerType { get; }

	MCDSystemState State { get; }

	MCDVersion Version { get; }

	MCDDbProjectConfiguration DbProjectConfiguration { get; }

	uint InterfaceNumber { get; }

	bool IsUnsupportedComParametersAccepted { get; }

	MCDInterfaces ConnectedInterfaces { get; }

	MCDInterfaces CurrentInterfaces { get; }

	string[] PropertyNames { get; }

	MCDEnumValue EnumValue { get; }

	event OnDefinableDynIdListChanged DefinableDynIdListChanged;

	event OnLinkActivityStateIdle LinkActivityStateIdle;

	event OnLinkActivityStateRunning LinkActivityStateRunning;

	event OnLinkActivityStateSuspended LinkActivityStateSuspended;

	event OnLinkError LinkError;

	event OnLinkLocked LinkLocked;

	event OnLinkOneVariantIdentified LinkOneVariantIdentified;

	event OnLinkOneVariantSelected LinkOneVariantSelected;

	event OnLinkQueueCleared LinkQueueCleared;

	event OnLinkStateCommunication LinkStateCommunication;

	event OnLinkStateCreated LinkStateCreated;

	event OnLinkStateOffline LinkStateOffline;

	event OnLinkStateOnline LinkStateOnline;

	event OnLinkUnlocked LinkUnlocked;

	event OnLinkVariantIdentified LinkVariantIdentified;

	event OnLinkVariantSelected LinkVariantSelected;

	event OnLinkVariantSet LinkVariantSet;

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

	event OnSystemClampStateChanged SystemClampStateChanged;

	event OnSystemConfigurationClosed SystemConfigurationClosed;

	event OnSystemConfigurationOpened SystemConfigurationOpened;

	event OnSystemError SystemError;

	event OnSystemLocked SystemLocked;

	event OnSystemLogicallyConnected SystemLogicallyConnected;

	event OnSystemLogicallyDisconnected SystemLogicallyDisconnected;

	event OnSystemProjectDeselected SystemProjectDeselected;

	event OnSystemProjectSelected SystemProjectSelected;

	event OnSystemUnlocked SystemUnlocked;

	event OnSystemVehicleInfoDeselected SystemVehicleInfoDeselected;

	event OnSystemVehicleInfoSelected SystemVehicleInfoSelected;

	event OnConfigurationRecordLoaded ConfigurationRecordLoaded;

	event OnStaticInterfaceError StaticInterfaceError;

	event OnInterfaceStatusChanged InterfaceStatusChanged;

	event OnMonitoringFramesReady MonitoringFramesReady;

	event OnInterfaceError InterfaceError;

	event OnInterfacesModified InterfacesModified;

	event OnDetectionFinished DetectionFinished;

	event OnInterfaceDetected InterfaceDetected;

	void DeselectProject();

	void PrepareInterface();

	MCDProject SelectProject(MCDDbProjectDescription project);

	MCDProject SelectProjectByName(string project);

	void UnprepareInterface();

	MCDValue GetSystemParameter(string value);

	void LockServer();

	void UnlockServer();

	void UnsupportedComParametersAccepted(bool accept);

	MCDValue GetProperty(string name);

	void SetProperty(string name, MCDValue value);

	void ResetProperty(string name);

	void DetectInterfaces(string optionString);

	void PrepareVciAccessLayer();

	void UnprepareVciAccessLayer();
}
