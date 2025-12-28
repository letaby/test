// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsLogicalLinkMonitorImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsLogicalLinkMonitorImpl : 
  MappedObject,
  DtsLogicalLinkMonitor,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsLogicalLinkMonitorImpl(IntPtr handle)
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

  ~DtsLogicalLinkMonitorImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public void AddAllLogicalLinkForMonitoring()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_addAllLogicalLinkForMonitoring(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveAllLogicalLinkForMonitoring()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_removeAllLogicalLinkForMonitoring(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void AddLogicalLinkForMonitoring(string NewLogicalLinkShortName)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_addLogicalLinkForMonitoring(this.Handle, NewLogicalLinkShortName);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveLogicalLinkFromMonitoring(string RemoveLogicalLinkShortName)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_removeLogicalLinkFromMonitoring(this.Handle, RemoveLogicalLinkShortName);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public uint RingBufferSize
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_setRingBufferSize(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void Start()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_start(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Stop()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_stop(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void OpenFileTrace(string FileName, bool bOverwrite)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_openFileTrace(this.Handle, FileName, bOverwrite);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StartFileTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_startFileTrace(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StopFileTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_stopFileTrace(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void CloseFileTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_closeFileTrace(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDCollection GetLatestEvents(uint uMaxNoOfNewEvents)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr latestEvents = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_getLatestEvents(this.Handle, uMaxNoOfNewEvents, out returnValue);
    if (latestEvents != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(latestEvents);
    return (MCDCollection) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsCollection);
  }

  public void SetFilter(DtsLogicalLinkFilterConfig filterConfig, bool filterView, bool filterTrace)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_setFilter(this.Handle, DTS_ObjectMapper.getHandle(filterConfig as MappedObject), filterView, filterTrace);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void OpenFileTraceInFolder(string outputFolderPath, string FileName)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_openFileTraceInFolder(this.Handle, outputFolderPath, FileName);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StartSnapshotModeTracing(uint timeInterval)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_startSnapshotModeTracing(this.Handle, timeInterval);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void GenerateSnapshotTrace(string outputPath)
  {
    GC.KeepAlive((object) this);
    IntPtr snapshotTrace = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_generateSnapshotTrace(this.Handle, outputPath);
    if (snapshotTrace != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(snapshotTrace);
  }

  public void TakeSnapshotTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr snapshotTrace = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_takeSnapshotTrace(this.Handle);
    if (snapshotTrace != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(snapshotTrace);
  }

  public void StopSnapshotModeTracing()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkMonitor_stopSnapshotModeTracing(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
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
