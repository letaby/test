// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsIntervalImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsIntervalImpl : 
  MappedObject,
  DtsInterval,
  MCDInterval,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsIntervalImpl(IntPtr handle)
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

  ~DtsIntervalImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDValue LowerLimit
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr lowerLimit = CSWrap.CSNIDTS_DtsInterval_getLowerLimit(this.Handle, out returnValue);
      if (lowerLimit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lowerLimit);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public MCDLimitType LowerLimitIntervalType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLimitType returnValue;
      IntPtr limitIntervalType = CSWrap.CSNIDTS_DtsInterval_getLowerLimitIntervalType(this.Handle, out returnValue);
      if (limitIntervalType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(limitIntervalType);
      return returnValue;
    }
  }

  public MCDValue UpperLimit
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr upperLimit = CSWrap.CSNIDTS_DtsInterval_getUpperLimit(this.Handle, out returnValue);
      if (upperLimit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(upperLimit);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public MCDLimitType UpperLimitIntervalType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLimitType returnValue;
      IntPtr limitIntervalType = CSWrap.CSNIDTS_DtsInterval_getUpperLimitIntervalType(this.Handle, out returnValue);
      if (limitIntervalType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(limitIntervalType);
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
