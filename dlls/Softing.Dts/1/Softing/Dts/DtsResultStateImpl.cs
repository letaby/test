// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsResultStateImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsResultStateImpl : 
  MappedObject,
  DtsResultState,
  MCDResultState,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsResultStateImpl(IntPtr handle)
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
    DTS_ObjectMapper.unregisterObject(this.Handle);
    this.Handle = IntPtr.Zero;
  }

  ~DtsResultStateImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public ushort NoOfResults
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr noOfResults = CSWrap.CSNIDTS_DtsResultState_getNoOfResults(this.Handle, out returnValue);
      if (noOfResults != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(noOfResults);
      return returnValue;
    }
  }

  public MCDExecutionState ExecutionState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDExecutionState returnValue;
      IntPtr executionState = CSWrap.CSNIDTS_DtsResultState_getExecutionState(this.Handle, out returnValue);
      if (executionState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(executionState);
      return returnValue;
    }
  }

  public MCDRepetitionState RepetitionState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDRepetitionState returnValue;
      IntPtr repetitionState = CSWrap.CSNIDTS_DtsResultState_getRepetitionState(this.Handle, out returnValue);
      if (repetitionState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(repetitionState);
      return returnValue;
    }
  }

  public MCDLogicalLinkState LogicalLinkState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLogicalLinkState returnValue;
      IntPtr logicalLinkState = CSWrap.CSNIDTS_DtsResultState_getLogicalLinkState(this.Handle, out returnValue);
      if (logicalLinkState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalLinkState);
      return returnValue;
    }
  }

  public MCDLockState LogicalLinkLockState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLockState returnValue;
      IntPtr logicalLinkLockState = CSWrap.CSNIDTS_DtsResultState_getLogicalLinkLockState(this.Handle, out returnValue);
      if (logicalLinkLockState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalLinkLockState);
      return returnValue;
    }
  }

  public bool HasError
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResultState_hasError(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDError Error
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr error = CSWrap.CSNIDTS_DtsResultState_getError(this.Handle, out returnValue);
      if (error != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(error);
      return (MCDError) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsError);
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
}
