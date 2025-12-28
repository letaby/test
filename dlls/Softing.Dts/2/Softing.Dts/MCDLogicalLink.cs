using System;

namespace Softing.Dts;

public interface MCDLogicalLink : MCDObject, IDisposable
{
	MCDDbLogicalLink DbObject { get; }

	MCDAccessKeys IdentifiedVariantAccessKeys { get; }

	MCDLockState LockState { get; }

	MCDQueueErrorMode QueueErrorMode { get; set; }

	uint QueueFillingLevel { get; }

	MCDActivityState QueueState { get; }

	MCDAccessKeys SelectedVariantAccessKeys { get; }

	MCDLogicalLinkState State { get; }

	MCDLogicalLinkType Type { get; }

	MCDValues DefinableDynIds { get; }

	bool IsUnsupportedComParametersAccepted { get; }

	MCDConfigurationRecords ConfigurationRecords { get; }

	uint UniqueRuntimeID { get; }

	MCDInterfaceResource InterfaceResource { get; }

	MCDDbMatchingPattern MatchedDbEcuVariantPattern { get; }

	uint QueueSize { get; set; }

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

	void ClearQueue();

	void Close();

	void Open();

	void Resume();

	void Suspend();

	MCDDiagComPrimitive CreateDiagComPrimitiveByDbObject(MCDDbDiagComPrimitive dbComPrimitive);

	MCDDiagComPrimitive CreateDiagComPrimitiveByName(string shortName);

	MCDDiagComPrimitive CreateDiagComPrimitiveBySemanticAttribute(string semanticAttribute);

	MCDDiagComPrimitive CreateDiagComPrimitiveByType(MCDObjectType diagComPrimitiveType);

	void DisableReducedResults();

	void GotoOnline();

	void Reset();

	void SetUnitGroup(string shortName);

	void UnsupportedComParametersAccepted(bool accept);

	void EnableReducedResults();

	void RemoveDiagComPrimitive(MCDDiagComPrimitive diagComPrimitive);

	MCDDbUnitGroup GetUnitGroup();

	MCDDiagService CreateDynIdComPrimitiveByTypeAndDefinitionMode(MCDObjectType type, string definitionMode);

	MCDValues ExecIOCtrl(string IOCtrlName, MCDValue inputData, uint inputDataItemType, uint outputDataSize);

	void SendBreak();
}
