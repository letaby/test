// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsLogicalLinkImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsLogicalLinkImpl : 
  MappedObject,
  DtsLogicalLink,
  MCDLogicalLink,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;
  private uint handler_count;
  private uint listener_handle;

  public DtsLogicalLinkImpl(IntPtr handle)
  {
    this.Handle = handle;
    DTS_ObjectMapper.registerObject(this.Handle, (object) this);
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!(this.Handle != IntPtr.Zero))
      return;
    if (this.listener_handle != 0U)
    {
      IntPtr Handle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, this.listener_handle);
      if (Handle != IntPtr.Zero)
        CSWrap.CSNIDTS_releaseObject(Handle);
      this.listener_handle = 0U;
    }
    DTS_ObjectMapper.unregisterObject(this.Handle);
    this.Handle = IntPtr.Zero;
  }

  ~DtsLogicalLinkImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public void ClearQueue()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_clearQueue(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Close()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_close(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbLogicalLink DbObject
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbObject = CSWrap.CSNIDTS_DtsLogicalLink_getDbObject(this.Handle, out returnValue);
      if (dbObject != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbObject);
      return (MCDDbLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLogicalLink);
    }
  }

  public MCDAccessKeys IdentifiedVariantAccessKeys
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr variantAccessKeys = CSWrap.CSNIDTS_DtsLogicalLink_getIdentifiedVariantAccessKeys(this.Handle, out returnValue);
      if (variantAccessKeys != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(variantAccessKeys);
      return (MCDAccessKeys) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKeys);
    }
  }

  public MCDLockState LockState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLockState returnValue;
      IntPtr lockState = CSWrap.CSNIDTS_DtsLogicalLink_getLockState(this.Handle, out returnValue);
      if (lockState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lockState);
      return returnValue;
    }
  }

  public MCDQueueErrorMode QueueErrorMode
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDQueueErrorMode returnValue;
      IntPtr queueErrorMode = CSWrap.CSNIDTS_DtsLogicalLink_getQueueErrorMode(this.Handle, out returnValue);
      if (queueErrorMode != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(queueErrorMode);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_setQueueErrorMode(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint QueueFillingLevel
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr queueFillingLevel = CSWrap.CSNIDTS_DtsLogicalLink_getQueueFillingLevel(this.Handle, out returnValue);
      if (queueFillingLevel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(queueFillingLevel);
      return returnValue;
    }
  }

  public bool AutoSyncWithInternalState
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr withInternalState = CSWrap.CSNIDTS_DtsLogicalLink_getAutoSyncWithInternalState(this.Handle, out returnValue);
      if (withInternalState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(withInternalState);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_setAutoSyncWithInternalState(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDActivityState QueueState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDActivityState returnValue;
      IntPtr queueState = CSWrap.CSNIDTS_DtsLogicalLink_getQueueState(this.Handle, out returnValue);
      if (queueState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(queueState);
      return returnValue;
    }
  }

  public MCDAccessKeys SelectedVariantAccessKeys
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr variantAccessKeys = CSWrap.CSNIDTS_DtsLogicalLink_getSelectedVariantAccessKeys(this.Handle, out returnValue);
      if (variantAccessKeys != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(variantAccessKeys);
      return (MCDAccessKeys) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKeys);
    }
  }

  public MCDLogicalLinkState State
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLogicalLinkState returnValue;
      IntPtr state = CSWrap.CSNIDTS_DtsLogicalLink_getState(this.Handle, out returnValue);
      if (state != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(state);
      return returnValue;
    }
  }

  public MCDLogicalLinkType Type
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLogicalLinkType returnValue;
      IntPtr type = CSWrap.CSNIDTS_DtsLogicalLink_getType(this.Handle, out returnValue);
      if (type != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(type);
      return returnValue;
    }
  }

  public bool HasDetectedVariant
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_hasDetectedVariant(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public void LockLink()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_lockLink(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Open()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_open(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Resume()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_resume(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDProtocolParameterSet ProtocolParameters
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_setProtocolParameters(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool SupportsTimeStamp()
  {
    GC.KeepAlive((object) this);
    bool returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_supportsTimeStamp(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public void Suspend()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_suspend(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void UnlockLink()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_unlockLink(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDiagComPrimitive CreateDiagComPrimitiveByDbObject(MCDDbDiagComPrimitive dbComPrimitive)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr primitiveByDbObject = CSWrap.CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByDbObject(this.Handle, DTS_ObjectMapper.getHandle(dbComPrimitive as MappedObject), out returnValue);
    if (primitiveByDbObject != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(primitiveByDbObject);
    return (MCDDiagComPrimitive) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive);
  }

  public MCDDiagComPrimitive CreateDiagComPrimitiveByName(string shortName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr comPrimitiveByName = CSWrap.CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByName(this.Handle, shortName, out returnValue);
    if (comPrimitiveByName != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(comPrimitiveByName);
    return (MCDDiagComPrimitive) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive);
  }

  public MCDDiagComPrimitive CreateDiagComPrimitiveBySemanticAttribute(string semanticAttribute)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr semanticAttribute1 = CSWrap.CSNIDTS_DtsLogicalLink_createDiagComPrimitiveBySemanticAttribute(this.Handle, semanticAttribute, out returnValue);
    if (semanticAttribute1 != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(semanticAttribute1);
    return (MCDDiagComPrimitive) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive);
  }

  public MCDDiagComPrimitive CreateDiagComPrimitiveByType(MCDObjectType diagComPrimitiveType)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr comPrimitiveByType = CSWrap.CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByType(this.Handle, diagComPrimitiveType, out returnValue);
    if (comPrimitiveByType != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(comPrimitiveByType);
    return (MCDDiagComPrimitive) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive);
  }

  public MCDService CreateDVServiceByRelationType(string relationType)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr serviceByRelationType = CSWrap.CSNIDTS_DtsLogicalLink_createDVServiceByRelationType(this.Handle, relationType, out returnValue);
    if (serviceByRelationType != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(serviceByRelationType);
    return (MCDService) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsService);
  }

  public void DisableReducedResults()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_disableReducedResults(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDValues DefinableDynIds
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr definableDynIds = CSWrap.CSNIDTS_DtsLogicalLink_getDefinableDynIds(this.Handle, out returnValue);
      if (definableDynIds != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(definableDynIds);
      return (MCDValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues);
    }
  }

  public void GotoOnline()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_gotoOnline(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public bool IsUnsupportedComParametersAccepted
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_isUnsupportedComParametersAccepted(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public void Reset()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_reset(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void SetUnitGroup(string shortName)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_setUnitGroup(this.Handle, shortName);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void UnsupportedComParametersAccepted(bool accept)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_unsupportedComParametersAccepted(this.Handle, accept);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void EnableReducedResults()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_enableReducedResults(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveDiagComPrimitive(MCDDiagComPrimitive diagComPrimitive)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_removeDiagComPrimitive(this.Handle, DTS_ObjectMapper.getHandle(diagComPrimitive as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void GotoOffline()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_gotoOffline(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbUnitGroup GetUnitGroup()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr unitGroup = CSWrap.CSNIDTS_DtsLogicalLink_getUnitGroup(this.Handle, out returnValue);
    if (unitGroup != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(unitGroup);
    return (MCDDbUnitGroup) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbUnitGroup);
  }

  public MCDDiagService CreateDynIdComPrimitiveByTypeAndDefinitionMode(
    MCDObjectType type,
    string definitionMode)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr andDefinitionMode = CSWrap.CSNIDTS_DtsLogicalLink_createDynIdComPrimitiveByTypeAndDefinitionMode(this.Handle, type, definitionMode, out returnValue);
    if (andDefinitionMode != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(andDefinitionMode);
    return (MCDDiagService) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagService);
  }

  public byte[] ExecuteIoCtl(uint uIoCtlCommandId, byte[] pInputData)
  {
    GC.KeepAlive((object) this);
    ByteField_Struct returnValue = new ByteField_Struct();
    ByteField_Struct _pInputData = new ByteField_Struct(pInputData);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_executeIoCtl(this.Handle, uIoCtlCommandId, ref _pInputData, out returnValue);
    _pInputData.FreeMemory();
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue.ToByteArray();
  }

  public DtsPhysicalInterfaceLink PhysicalInterfaceLink
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr physicalInterfaceLink = CSWrap.CSNIDTS_DtsLogicalLink_getPhysicalInterfaceLink(this.Handle, out returnValue);
      if (physicalInterfaceLink != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalInterfaceLink);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsPhysicalInterfaceLink;
    }
  }

  public MCDConfigurationRecords ConfigurationRecords
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr configurationRecords = CSWrap.CSNIDTS_DtsLogicalLink_getConfigurationRecords(this.Handle, out returnValue);
      if (configurationRecords != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationRecords);
      return (MCDConfigurationRecords) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecords);
    }
  }

  public uint UniqueRuntimeID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr uniqueRuntimeId = CSWrap.CSNIDTS_DtsLogicalLink_getUniqueRuntimeID(this.Handle, out returnValue);
      if (uniqueRuntimeId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uniqueRuntimeId);
      return returnValue;
    }
  }

  public MCDValues ExecIOCtrl(
    string IOCtrlName,
    MCDValue inputData,
    uint inputDataItemType,
    uint outputDataSize)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_execIOCtrl(this.Handle, IOCtrlName, DTS_ObjectMapper.getHandle(inputData as MappedObject), inputDataItemType, outputDataSize, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues);
  }

  public MCDInterfaceResource InterfaceResource
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interfaceResource = CSWrap.CSNIDTS_DtsLogicalLink_getInterfaceResource(this.Handle, out returnValue);
      if (interfaceResource != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceResource);
      return (MCDInterfaceResource) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceResource);
    }
  }

  public void SendBreak()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_sendBreak(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbMatchingPattern MatchedDbEcuVariantPattern
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr ecuVariantPattern = CSWrap.CSNIDTS_DtsLogicalLink_getMatchedDbEcuVariantPattern(this.Handle, out returnValue);
      if (ecuVariantPattern != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ecuVariantPattern);
      return (MCDDbMatchingPattern) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbMatchingPattern);
    }
  }

  public bool ChannelMonitoring
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr channelMonitoring = CSWrap.CSNIDTS_DtsLogicalLink_getChannelMonitoring(this.Handle, out returnValue);
      if (channelMonitoring != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(channelMonitoring);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_setChannelMonitoring(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDAccessKey CreationAccessKey
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr creationAccessKey = CSWrap.CSNIDTS_DtsLogicalLink_getCreationAccessKey(this.Handle, out returnValue);
      if (creationAccessKey != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(creationAccessKey);
      return (MCDAccessKey) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKey);
    }
  }

  public MCDLogicalLinkState InternalState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLogicalLinkState returnValue;
      IntPtr internalState = CSWrap.CSNIDTS_DtsLogicalLink_getInternalState(this.Handle, out returnValue);
      if (internalState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(internalState);
      return returnValue;
    }
  }

  public void GotoOnlineWithTimeout(uint timeout)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_gotoOnlineWithTimeout(this.Handle, timeout);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void OpenCached(bool useVariant)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_openCached(this.Handle, useVariant);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public uint QueueSize
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr queueSize = CSWrap.CSNIDTS_DtsLogicalLink_getQueueSize(this.Handle, out returnValue);
      if (queueSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(queueSize);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLink_setQueueSize(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint OpenCounter
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr openCounter = CSWrap.CSNIDTS_DtsLogicalLink_getOpenCounter(this.Handle, out returnValue);
      if (openCounter != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(openCounter);
      return returnValue;
    }
  }

  public uint OnlineCounter
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr onlineCounter = CSWrap.CSNIDTS_DtsLogicalLink_getOnlineCounter(this.Handle, out returnValue);
      if (onlineCounter != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(onlineCounter);
      return returnValue;
    }
  }

  public uint StartCommCounter
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr startCommCounter = CSWrap.CSNIDTS_DtsLogicalLink_getStartCommCounter(this.Handle, out returnValue);
      if (startCommCounter != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(startCommCounter);
      return returnValue;
    }
  }

  public uint LockedCounter
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr lockedCounter = CSWrap.CSNIDTS_DtsLogicalLink_getLockedCounter(this.Handle, out returnValue);
      if (lockedCounter != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lockedCounter);
      return returnValue;
    }
  }

  public MCDObjectType ObjectType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDObjectType returnValue;
      IntPtr objectType = CSWrap.CSNIDTS_DtsObject_getObjectType(this.Handle, out returnValue);
      if (objectType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectType);
      return returnValue;
    }
  }

  public uint ObjectID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr objectId = CSWrap.CSNIDTS_DtsObject_getObjectID(this.Handle, out returnValue);
      if (objectId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectId);
      return returnValue;
    }
  }

  internal event OnDefinableDynIdListChanged __DefinableDynIdListChanged;

  internal bool _onDefinableDynIdListChanged(MCDValues dynIdList, MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__DefinableDynIdListChanged != null)
      {
        this.__DefinableDynIdListChanged((object) this, new DefinableDynIdListChangedArgs(dynIdList, link));
        return true;
      }
    }
    return false;
  }

  public event OnDefinableDynIdListChanged DefinableDynIdListChanged
  {
    add
    {
      lock (this)
      {
        this.__DefinableDynIdListChanged += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__DefinableDynIdListChanged -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkActivityStateIdle __LinkActivityStateIdle;

  internal bool _onLinkActivityStateIdle(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkActivityStateIdle != null)
      {
        this.__LinkActivityStateIdle((object) this, new LinkActivityStateIdleArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkActivityStateIdle LinkActivityStateIdle
  {
    add
    {
      lock (this)
      {
        this.__LinkActivityStateIdle += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkActivityStateIdle -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkActivityStateRunning __LinkActivityStateRunning;

  internal bool _onLinkActivityStateRunning(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkActivityStateRunning != null)
      {
        this.__LinkActivityStateRunning((object) this, new LinkActivityStateRunningArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkActivityStateRunning LinkActivityStateRunning
  {
    add
    {
      lock (this)
      {
        this.__LinkActivityStateRunning += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkActivityStateRunning -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkActivityStateSuspended __LinkActivityStateSuspended;

  internal bool _onLinkActivityStateSuspended(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkActivityStateSuspended != null)
      {
        this.__LinkActivityStateSuspended((object) this, new LinkActivityStateSuspendedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkActivityStateSuspended LinkActivityStateSuspended
  {
    add
    {
      lock (this)
      {
        this.__LinkActivityStateSuspended += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkActivityStateSuspended -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkError __LinkError;

  internal bool _onLinkError(MCDLogicalLink link, MCDError error)
  {
    lock (this)
    {
      if (this.__LinkError != null)
      {
        this.__LinkError((object) this, new LinkErrorArgs(link, error));
        return true;
      }
    }
    return false;
  }

  public event OnLinkError LinkError
  {
    add
    {
      lock (this)
      {
        this.__LinkError += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkError -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkLocked __LinkLocked;

  internal bool _onLinkLocked(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkLocked != null)
      {
        this.__LinkLocked((object) this, new LinkLockedArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkLocked LinkLocked
  {
    add
    {
      lock (this)
      {
        this.__LinkLocked += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkLocked -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkOneVariantIdentified __LinkOneVariantIdentified;

  internal bool _onLinkOneVariantIdentified(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkOneVariantIdentified != null)
      {
        this.__LinkOneVariantIdentified((object) this, new LinkOneVariantIdentifiedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkOneVariantIdentified LinkOneVariantIdentified
  {
    add
    {
      lock (this)
      {
        this.__LinkOneVariantIdentified += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkOneVariantIdentified -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkOneVariantSelected __LinkOneVariantSelected;

  internal bool _onLinkOneVariantSelected(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkOneVariantSelected != null)
      {
        this.__LinkOneVariantSelected((object) this, new LinkOneVariantSelectedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkOneVariantSelected LinkOneVariantSelected
  {
    add
    {
      lock (this)
      {
        this.__LinkOneVariantSelected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkOneVariantSelected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkQueueCleared __LinkQueueCleared;

  internal bool _onLinkQueueCleared(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkQueueCleared != null)
      {
        this.__LinkQueueCleared((object) this, new LinkQueueClearedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkQueueCleared LinkQueueCleared
  {
    add
    {
      lock (this)
      {
        this.__LinkQueueCleared += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkQueueCleared -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkStateCommunication __LinkStateCommunication;

  internal bool _onLinkStateCommunication(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkStateCommunication != null)
      {
        this.__LinkStateCommunication((object) this, new LinkStateCommunicationArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkStateCommunication LinkStateCommunication
  {
    add
    {
      lock (this)
      {
        this.__LinkStateCommunication += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkStateCommunication -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkStateCreated __LinkStateCreated;

  internal bool _onLinkStateCreated(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkStateCreated != null)
      {
        this.__LinkStateCreated((object) this, new LinkStateCreatedArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkStateCreated LinkStateCreated
  {
    add
    {
      lock (this)
      {
        this.__LinkStateCreated += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkStateCreated -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkStateOffline __LinkStateOffline;

  internal bool _onLinkStateOffline(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkStateOffline != null)
      {
        this.__LinkStateOffline((object) this, new LinkStateOfflineArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkStateOffline LinkStateOffline
  {
    add
    {
      lock (this)
      {
        this.__LinkStateOffline += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkStateOffline -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkStateOnline __LinkStateOnline;

  internal bool _onLinkStateOnline(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkStateOnline != null)
      {
        this.__LinkStateOnline((object) this, new LinkStateOnlineArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkStateOnline LinkStateOnline
  {
    add
    {
      lock (this)
      {
        this.__LinkStateOnline += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkStateOnline -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkUnlocked __LinkUnlocked;

  internal bool _onLinkUnlocked(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkUnlocked != null)
      {
        this.__LinkUnlocked((object) this, new LinkUnlockedArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkUnlocked LinkUnlocked
  {
    add
    {
      lock (this)
      {
        this.__LinkUnlocked += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkUnlocked -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkVariantIdentified __LinkVariantIdentified;

  internal bool _onLinkVariantIdentified(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkVariantIdentified != null)
      {
        this.__LinkVariantIdentified((object) this, new LinkVariantIdentifiedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkVariantIdentified LinkVariantIdentified
  {
    add
    {
      lock (this)
      {
        this.__LinkVariantIdentified += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkVariantIdentified -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkVariantSelected __LinkVariantSelected;

  internal bool _onLinkVariantSelected(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkVariantSelected != null)
      {
        this.__LinkVariantSelected((object) this, new LinkVariantSelectedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkVariantSelected LinkVariantSelected
  {
    add
    {
      lock (this)
      {
        this.__LinkVariantSelected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkVariantSelected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkVariantSet __LinkVariantSet;

  internal bool _onLinkVariantSet(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkVariantSet != null)
      {
        this.__LinkVariantSet((object) this, new LinkVariantSetArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkVariantSet LinkVariantSet
  {
    add
    {
      lock (this)
      {
        this.__LinkVariantSet += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkVariantSet -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveBufferOverflow __PrimitiveBufferOverflow;

  internal bool _onPrimitiveBufferOverflow(MCDDiagComPrimitive primitive, MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__PrimitiveBufferOverflow != null)
      {
        this.__PrimitiveBufferOverflow((object) this, new PrimitiveBufferOverflowArgs(primitive, link));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveBufferOverflow PrimitiveBufferOverflow
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveBufferOverflow += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveBufferOverflow -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveCanceledDuringExecution __PrimitiveCanceledDuringExecution;

  internal bool _onPrimitiveCanceledDuringExecution(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__PrimitiveCanceledDuringExecution != null)
      {
        this.__PrimitiveCanceledDuringExecution((object) this, new PrimitiveCanceledDuringExecutionArgs(primitive, link));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveCanceledDuringExecution PrimitiveCanceledDuringExecution
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveCanceledDuringExecution += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveCanceledDuringExecution -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveCanceledFromQueue __PrimitiveCanceledFromQueue;

  internal bool _onPrimitiveCanceledFromQueue(MCDDiagComPrimitive primitive, MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__PrimitiveCanceledFromQueue != null)
      {
        this.__PrimitiveCanceledFromQueue((object) this, new PrimitiveCanceledFromQueueArgs(primitive, link));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveCanceledFromQueue PrimitiveCanceledFromQueue
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveCanceledFromQueue += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveCanceledFromQueue -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveError __PrimitiveError;

  internal bool _onPrimitiveError(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDError error)
  {
    lock (this)
    {
      if (this.__PrimitiveError != null)
      {
        this.__PrimitiveError((object) this, new PrimitiveErrorArgs(primitive, link, error));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveError PrimitiveError
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveError += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveError -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveHasIntermediateResult __PrimitiveHasIntermediateResult;

  internal bool _onPrimitiveHasIntermediateResult(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDResultState resultstate)
  {
    lock (this)
    {
      if (this.__PrimitiveHasIntermediateResult != null)
      {
        this.__PrimitiveHasIntermediateResult((object) this, new PrimitiveHasIntermediateResultArgs(primitive, link, resultstate));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveHasIntermediateResult PrimitiveHasIntermediateResult
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveHasIntermediateResult += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveHasIntermediateResult -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveHasResult __PrimitiveHasResult;

  internal bool _onPrimitiveHasResult(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDResultState resultstate)
  {
    lock (this)
    {
      if (this.__PrimitiveHasResult != null)
      {
        this.__PrimitiveHasResult((object) this, new PrimitiveHasResultArgs(primitive, link, resultstate));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveHasResult PrimitiveHasResult
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveHasResult += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveHasResult -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveJobInfo __PrimitiveJobInfo;

  internal bool _onPrimitiveJobInfo(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    string info)
  {
    lock (this)
    {
      if (this.__PrimitiveJobInfo != null)
      {
        this.__PrimitiveJobInfo((object) this, new PrimitiveJobInfoArgs(primitive, link, info));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveJobInfo PrimitiveJobInfo
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveJobInfo += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveJobInfo -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveProgressInfo __PrimitiveProgressInfo;

  internal bool _onPrimitiveProgressInfo(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    byte progress)
  {
    lock (this)
    {
      if (this.__PrimitiveProgressInfo != null)
      {
        this.__PrimitiveProgressInfo((object) this, new PrimitiveProgressInfoArgs(primitive, link, progress));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveProgressInfo PrimitiveProgressInfo
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveProgressInfo += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveProgressInfo -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveRepetitionStopped __PrimitiveRepetitionStopped;

  internal bool _onPrimitiveRepetitionStopped(MCDDiagComPrimitive primitive, MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__PrimitiveRepetitionStopped != null)
      {
        this.__PrimitiveRepetitionStopped((object) this, new PrimitiveRepetitionStoppedArgs(primitive, link));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveRepetitionStopped PrimitiveRepetitionStopped
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveRepetitionStopped += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveRepetitionStopped -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveTerminated __PrimitiveTerminated;

  internal bool _onPrimitiveTerminated(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDResultState resultstate)
  {
    lock (this)
    {
      if (this.__PrimitiveTerminated != null)
      {
        this.__PrimitiveTerminated((object) this, new PrimitiveTerminatedArgs(primitive, link, resultstate));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveTerminated PrimitiveTerminated
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveTerminated += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveTerminated -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }
}
