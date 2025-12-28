// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDoIPMonitorLinkImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDoIPMonitorLinkImpl : 
  MappedObject,
  DtsDoIPMonitorLink,
  DtsMonitoringLink,
  MCDMonitoringLink,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDoIPMonitorLinkImpl(IntPtr handle)
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

  ~DtsDoIPMonitorLinkImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string[] FetchMonitoringFrames(uint numReq)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitoringLink_fetchMonitoringFrames(this.Handle, numReq, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue.ToStringArray();
  }

  public MCDInterfaceResource InterfaceResource
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interfaceResource = CSWrap.CSNIDTS_DtsMonitoringLink_getInterfaceResource(this.Handle, out returnValue);
      if (interfaceResource != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceResource);
      return (MCDInterfaceResource) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceResource);
    }
  }

  public MCDMessageFilters MessageFilters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr messageFilters = CSWrap.CSNIDTS_DtsMonitoringLink_getMessageFilters(this.Handle, out returnValue);
      if (messageFilters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(messageFilters);
      return (MCDMessageFilters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMessageFilters);
    }
  }

  public ushort NoOfSampleToFireEvent
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr fireEvent = CSWrap.CSNIDTS_DtsMonitoringLink_setNoOfSampleToFireEvent(this.Handle, value);
      if (fireEvent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(fireEvent);
    }
  }

  public ushort TimeToFireEvent
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr fireEvent = CSWrap.CSNIDTS_DtsMonitoringLink_setTimeToFireEvent(this.Handle, value);
      if (fireEvent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(fireEvent);
    }
  }

  public void Start()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitoringLink_start(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Stop()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitoringLink_stop(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StartFileTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitoringLink_startFileTrace(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StopFileTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitoringLink_stopFileTrace(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void OpenFileTrace(string FileName)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitoringLink_openFileTrace(this.Handle, FileName);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void CloseFileTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitoringLink_closeFileTrace(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public byte[] FetchDtsMonitorFrames(uint numReq)
  {
    GC.KeepAlive((object) this);
    ByteField_Struct returnValue = new ByteField_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitoringLink_fetchDtsMonitorFrames(this.Handle, numReq, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue.ToByteArray();
  }

  public string Description
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr description = CSWrap.CSNIDTS_DtsNamedObject_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
  }

  public string ShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortName = CSWrap.CSNIDTS_DtsNamedObject_getShortName(this.Handle, out returnValue);
      if (shortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortName);
      return returnValue.makeString();
    }
  }

  public string LongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longName = CSWrap.CSNIDTS_DtsNamedObject_getLongName(this.Handle, out returnValue);
      if (longName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longName);
      return returnValue.makeString();
    }
  }

  public uint StringID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr stringId = CSWrap.CSNIDTS_DtsNamedObject_getStringID(this.Handle, out returnValue);
      if (stringId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(stringId);
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
}
