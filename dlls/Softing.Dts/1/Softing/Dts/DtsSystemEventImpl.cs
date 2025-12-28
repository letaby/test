// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsSystemEventImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsSystemEventImpl : 
  MappedObject,
  DtsSystemEvent,
  DtsEvent,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsSystemEventImpl(IntPtr handle)
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

  ~DtsSystemEventImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public DtsEventType EventType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsEventType returnValue;
      IntPtr eventType = CSWrap.CSNIDTS_DtsEvent_getEventType(this.Handle, out returnValue);
      if (eventType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(eventType);
      return returnValue;
    }
  }

  public DtsEventId EventId
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsEventId returnValue;
      IntPtr eventId = CSWrap.CSNIDTS_DtsEvent_getEventId(this.Handle, out returnValue);
      if (eventId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(eventId);
      return returnValue;
    }
  }

  public MCDError Error
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr error = CSWrap.CSNIDTS_DtsEvent_getError(this.Handle, out returnValue);
      if (error != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(error);
      return (MCDError) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsError);
    }
  }

  public MCDDiagComPrimitive DiagComPrimitive
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr diagComPrimitive = CSWrap.CSNIDTS_DtsEvent_getDiagComPrimitive(this.Handle, out returnValue);
      if (diagComPrimitive != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(diagComPrimitive);
      return (MCDDiagComPrimitive) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDiagComPrimitive);
    }
  }

  public string JobInfo
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr jobInfo = CSWrap.CSNIDTS_DtsEvent_getJobInfo(this.Handle, out returnValue);
      if (jobInfo != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(jobInfo);
      return returnValue.makeString();
    }
  }

  public byte Progress
  {
    get
    {
      GC.KeepAlive((object) this);
      byte returnValue;
      IntPtr progress = CSWrap.CSNIDTS_DtsEvent_getProgress(this.Handle, out returnValue);
      if (progress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(progress);
      return returnValue;
    }
  }

  public MCDResultState ResultState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr resultState = CSWrap.CSNIDTS_DtsEvent_getResultState(this.Handle, out returnValue);
      if (resultState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(resultState);
      return (MCDResultState) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResultState);
    }
  }

  public MCDLockState LockState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLockState returnValue;
      IntPtr lockState = CSWrap.CSNIDTS_DtsEvent_getLockState(this.Handle, out returnValue);
      if (lockState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lockState);
      return returnValue;
    }
  }

  public MCDLogicalLink LogicalLink
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr logicalLink = CSWrap.CSNIDTS_DtsEvent_getLogicalLink(this.Handle, out returnValue);
      if (logicalLink != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalLink);
      return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
    }
  }

  public MCDLogicalLinkState LogicalLinkState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLogicalLinkState returnValue;
      IntPtr logicalLinkState = CSWrap.CSNIDTS_DtsEvent_getLogicalLinkState(this.Handle, out returnValue);
      if (logicalLinkState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalLinkState);
      return returnValue;
    }
  }

  public string Clamp
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr clamp = CSWrap.CSNIDTS_DtsEvent_getClamp(this.Handle, out returnValue);
      if (clamp != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(clamp);
      return returnValue.makeString();
    }
  }

  public MCDClampState ClampState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDClampState returnValue;
      IntPtr clampState = CSWrap.CSNIDTS_DtsEvent_getClampState(this.Handle, out returnValue);
      if (clampState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(clampState);
      return returnValue;
    }
  }

  public MCDMonitoringLink MonitoringLink
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr monitoringLink = CSWrap.CSNIDTS_DtsEvent_getMonitoringLink(this.Handle, out returnValue);
      if (monitoringLink != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(monitoringLink);
      return (MCDMonitoringLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitoringLink);
    }
  }

  public MCDInterface Interface
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsEvent_getInterface(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return (MCDInterface) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterface);
    }
  }

  public MCDInterfaceStatus InterfaceStatus
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDInterfaceStatus returnValue;
      IntPtr interfaceStatus = CSWrap.CSNIDTS_DtsEvent_getInterfaceStatus(this.Handle, out returnValue);
      if (interfaceStatus != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceStatus);
      return returnValue;
    }
  }

  public MCDConfigurationRecord ConfigurationRecord
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr configurationRecord = CSWrap.CSNIDTS_DtsEvent_getConfigurationRecord(this.Handle, out returnValue);
      if (configurationRecord != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationRecord);
      return (MCDConfigurationRecord) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConfigurationRecord);
    }
  }

  public MCDValues DynIdList
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dynIdList = CSWrap.CSNIDTS_DtsEvent_getDynIdList(this.Handle, out returnValue);
      if (dynIdList != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dynIdList);
      return (MCDValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues);
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
