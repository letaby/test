// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDLogicalLink
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDLogicalLink : MCDObject, IDisposable
{
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

  MCDDbLogicalLink DbObject { get; }

  MCDAccessKeys IdentifiedVariantAccessKeys { get; }

  MCDLockState LockState { get; }

  MCDQueueErrorMode QueueErrorMode { get; set; }

  uint QueueFillingLevel { get; }

  MCDActivityState QueueState { get; }

  MCDAccessKeys SelectedVariantAccessKeys { get; }

  MCDLogicalLinkState State { get; }

  MCDLogicalLinkType Type { get; }

  void Open();

  void Resume();

  void Suspend();

  MCDDiagComPrimitive CreateDiagComPrimitiveByDbObject(MCDDbDiagComPrimitive dbComPrimitive);

  MCDDiagComPrimitive CreateDiagComPrimitiveByName(string shortName);

  MCDDiagComPrimitive CreateDiagComPrimitiveBySemanticAttribute(string semanticAttribute);

  MCDDiagComPrimitive CreateDiagComPrimitiveByType(MCDObjectType diagComPrimitiveType);

  void DisableReducedResults();

  MCDValues DefinableDynIds { get; }

  void GotoOnline();

  bool IsUnsupportedComParametersAccepted { get; }

  void Reset();

  void SetUnitGroup(string shortName);

  void UnsupportedComParametersAccepted(bool accept);

  void EnableReducedResults();

  void RemoveDiagComPrimitive(MCDDiagComPrimitive diagComPrimitive);

  MCDDbUnitGroup GetUnitGroup();

  MCDDiagService CreateDynIdComPrimitiveByTypeAndDefinitionMode(
    MCDObjectType type,
    string definitionMode);

  MCDConfigurationRecords ConfigurationRecords { get; }

  uint UniqueRuntimeID { get; }

  MCDValues ExecIOCtrl(
    string IOCtrlName,
    MCDValue inputData,
    uint inputDataItemType,
    uint outputDataSize);

  MCDInterfaceResource InterfaceResource { get; }

  void SendBreak();

  MCDDbMatchingPattern MatchedDbEcuVariantPattern { get; }

  uint QueueSize { get; set; }
}
