// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsMonitorLinkImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsMonitorLinkImpl : MappedObject, DtsMonitorLink, DtsObject, MCDObject, IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsMonitorLinkImpl(IntPtr handle)
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

  ~DtsMonitorLinkImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public void Open()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_open(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Close()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_close(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void OpenTraceFile(string TraceFileName, bool bOverwriteIfFileExists)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_openTraceFile(this.Handle, TraceFileName, bOverwriteIfFileExists);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StartTraceFile()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_startTraceFile(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StopTraceFile()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_stopTraceFile(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void CloseTraceFile()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_closeTraceFile(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public uint TraceFileLimit
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr traceFileLimit = CSWrap.CSNIDTS_DtsMonitorLink_getTraceFileLimit(this.Handle, out returnValue);
      if (traceFileLimit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceFileLimit);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_setTraceFileLimit(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public byte[] GetLastItems(
    uint uNoOfItems,
    uint uLastTotalNoOfItems,
    ref uint puNoOfDeliveredItems,
    ref uint puTotalNoOfItems)
  {
    GC.KeepAlive((object) this);
    ByteField_Struct returnValue = new ByteField_Struct();
    IntPtr lastItems = CSWrap.CSNIDTS_DtsMonitorLink_getLastItems(this.Handle, uNoOfItems, uLastTotalNoOfItems, ref puNoOfDeliveredItems, ref puTotalNoOfItems, out returnValue);
    if (lastItems != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(lastItems);
    return returnValue.ToByteArray();
  }

  public bool FilterForDisplayAndFileFlag
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr displayAndFileFlag = CSWrap.CSNIDTS_DtsMonitorLink_getFilterForDisplayAndFileFlag(this.Handle, out returnValue);
      if (displayAndFileFlag != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(displayAndFileFlag);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_setFilterForDisplayAndFileFlag(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool FilterForDisplayFlag
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr filterForDisplayFlag = CSWrap.CSNIDTS_DtsMonitorLink_getFilterForDisplayFlag(this.Handle, out returnValue);
      if (filterForDisplayFlag != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(filterForDisplayFlag);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_setFilterForDisplayFlag(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool BusloadFlag
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr busloadFlag = CSWrap.CSNIDTS_DtsMonitorLink_getBusloadFlag(this.Handle, out returnValue);
      if (busloadFlag != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(busloadFlag);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_setBusloadFlag(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public double CurrentBusLoad
  {
    get
    {
      GC.KeepAlive((object) this);
      double returnValue;
      IntPtr currentBusLoad = CSWrap.CSNIDTS_DtsMonitorLink_getCurrentBusLoad(this.Handle, out returnValue);
      if (currentBusLoad != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(currentBusLoad);
      return returnValue;
    }
  }

  public string CanFilter
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsMonitorLink_setCanFilter(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
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
