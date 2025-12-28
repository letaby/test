// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsRawServiceImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsRawServiceImpl : 
  MappedObject,
  DtsRawService,
  DtsDiagService,
  MCDDiagService,
  MCDDataPrimitive,
  MCDDiagComPrimitive,
  MCDObject,
  IDisposable,
  DtsDataPrimitive,
  DtsDiagComPrimitive,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;
  private uint handler_count;
  private uint listener_handle;

  public DtsRawServiceImpl(IntPtr handle)
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

  ~DtsRawServiceImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDValue RawData
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr rawData = CSWrap.CSNIDTS_DtsRawService_getRawData(this.Handle, out returnValue);
      if (rawData != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(rawData);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public void EnterRawData(MCDValue rawData)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsRawService_enterRawData(this.Handle, DTS_ObjectMapper.getHandle(rawData as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public ushort ResultBufferSize
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr resultBufferSize = CSWrap.CSNIDTS_DtsDataPrimitive_getResultBufferSize(this.Handle, out returnValue);
      if (resultBufferSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(resultBufferSize);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDataPrimitive_setResultBufferSize(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDRepetitionState RepetitionState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDRepetitionState returnValue;
      IntPtr repetitionState = CSWrap.CSNIDTS_DtsDataPrimitive_getRepetitionState(this.Handle, out returnValue);
      if (repetitionState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(repetitionState);
      return returnValue;
    }
  }

  public ushort RepetitionTime
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr repetitionTime = CSWrap.CSNIDTS_DtsDataPrimitive_getRepetitionTime(this.Handle, out returnValue);
      if (repetitionTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(repetitionTime);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDataPrimitive_setRepetitionTime(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void StartRepetition()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDataPrimitive_startRepetition(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StopRepetition()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDataPrimitive_stopRepetition(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void UpdateRepetitionParameters()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDataPrimitive_updateRepetitionParameters(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void ExecuteAsync()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDataPrimitive_executeAsync(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDResults FetchResults(int numReq)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDataPrimitive_fetchResults(this.Handle, numReq, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResults) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResults);
  }

  public MCDResultState ResultState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr resultState = CSWrap.CSNIDTS_DtsDataPrimitive_getResultState(this.Handle, out returnValue);
      if (resultState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(resultState);
      return (MCDResultState) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResultState);
    }
  }

  public void StartCyclicSend(uint cyclicTime, int numSendCycles)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDataPrimitive_startCyclicSend(this.Handle, cyclicTime, numSendCycles);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Cancel()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDiagComPrimitive_cancel(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDResult ExecuteSync()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDiagComPrimitive_executeSync(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResult) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResult);
  }

  public MCDDbDiagComPrimitive DbObject
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbObject = CSWrap.CSNIDTS_DtsDiagComPrimitive_getDbObject(this.Handle, out returnValue);
      if (dbObject != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbObject);
      return (MCDDbDiagComPrimitive) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagComPrimitive);
    }
  }

  public MCDErrors Errors
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr errors = CSWrap.CSNIDTS_DtsDiagComPrimitive_getErrors(this.Handle, out returnValue);
      if (errors != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(errors);
      return (MCDErrors) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsErrors);
    }
  }

  public MCDRequest Request
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr request = CSWrap.CSNIDTS_DtsDiagComPrimitive_getRequest(this.Handle, out returnValue);
      if (request != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(request);
      return (MCDRequest) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsRequest);
    }
  }

  public void ResetToDefaultValue(string parameterName)
  {
    GC.KeepAlive((object) this);
    IntPtr defaultValue = CSWrap.CSNIDTS_DtsDiagComPrimitive_resetToDefaultValue(this.Handle, parameterName);
    if (defaultValue != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(defaultValue);
  }

  public void ResetToDefaultValues()
  {
    GC.KeepAlive((object) this);
    IntPtr defaultValues = CSWrap.CSNIDTS_DtsDiagComPrimitive_resetToDefaultValues(this.Handle);
    if (defaultValues != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(defaultValues);
  }

  public MCDResultState ExecuteSyncWithResultState()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDiagComPrimitive_executeSyncWithResultState(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResultState) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResultState);
  }

  public MCDResults ExecuteSyncWithResults()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDiagComPrimitive_executeSyncWithResults(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDResults) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResults);
  }

  public MCDObject Parent
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr parent = CSWrap.CSNIDTS_DtsDiagComPrimitive_getParent(this.Handle, out returnValue);
      if (parent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parent);
      return (MCDObject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject);
    }
  }

  public uint UniqueRuntimeID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr uniqueRuntimeId = CSWrap.CSNIDTS_DtsDiagComPrimitive_getUniqueRuntimeID(this.Handle, out returnValue);
      if (uniqueRuntimeId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(uniqueRuntimeId);
      return returnValue;
    }
  }

  public MCDDiagComPrimitiveState State
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDiagComPrimitiveState returnValue;
      IntPtr state = CSWrap.CSNIDTS_DtsDiagComPrimitive_getState(this.Handle, out returnValue);
      if (state != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(state);
      return returnValue;
    }
  }

  public string InternalShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr internalShortName = CSWrap.CSNIDTS_DtsDiagComPrimitive_getInternalShortName(this.Handle, out returnValue);
      if (internalShortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(internalShortName);
      return returnValue.makeString();
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
