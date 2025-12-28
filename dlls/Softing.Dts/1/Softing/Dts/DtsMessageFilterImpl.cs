// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsMessageFilterImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsMessageFilterImpl : 
  MappedObject,
  DtsMessageFilter,
  MCDMessageFilter,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsMessageFilterImpl(IntPtr handle)
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

  ~DtsMessageFilterImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public void EnableMessageFilter(bool enableMessageFilter)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsMessageFilter_enableMessageFilter(this.Handle, enableMessageFilter);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public uint FilterCompareSize
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr filterCompareSize = CSWrap.CSNIDTS_DtsMessageFilter_getFilterCompareSize(this.Handle, out returnValue);
      if (filterCompareSize != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(filterCompareSize);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsMessageFilter_setFilterCompareSize(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint FilterId
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr filterId = CSWrap.CSNIDTS_DtsMessageFilter_getFilterId(this.Handle, out returnValue);
      if (filterId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(filterId);
      return returnValue;
    }
  }

  public MCDMessageFilterValues FilterMasks
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr filterMasks = CSWrap.CSNIDTS_DtsMessageFilter_getFilterMasks(this.Handle, out returnValue);
      if (filterMasks != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(filterMasks);
      return (MCDMessageFilterValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMessageFilterValues);
    }
  }

  public MCDMessageFilterValues FilterPatterns
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr filterPatterns = CSWrap.CSNIDTS_DtsMessageFilter_getFilterPatterns(this.Handle, out returnValue);
      if (filterPatterns != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(filterPatterns);
      return (MCDMessageFilterValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMessageFilterValues);
    }
  }

  public MCDMessageFilterType FilterType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDMessageFilterType returnValue;
      IntPtr filterType = CSWrap.CSNIDTS_DtsMessageFilter_getFilterType(this.Handle, out returnValue);
      if (filterType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(filterType);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsMessageFilter_setFilterType(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool IsMessageFilterEnabled
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsMessageFilter_isMessageFilterEnabled(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
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
