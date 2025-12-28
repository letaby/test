// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsVersionImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsVersionImpl : 
  MappedObject,
  DtsVersion,
  MCDVersion,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsVersionImpl(IntPtr handle)
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

  ~DtsVersionImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public uint Major
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr major = CSWrap.CSNIDTS_DtsVersion_getMajor(this.Handle, out returnValue);
      if (major != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(major);
      return returnValue;
    }
  }

  public uint Minor
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr minor = CSWrap.CSNIDTS_DtsVersion_getMinor(this.Handle, out returnValue);
      if (minor != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(minor);
      return returnValue;
    }
  }

  public uint Revision
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr revision = CSWrap.CSNIDTS_DtsVersion_getRevision(this.Handle, out returnValue);
      if (revision != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(revision);
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
