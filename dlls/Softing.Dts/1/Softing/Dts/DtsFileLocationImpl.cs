// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsFileLocationImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsFileLocationImpl : MappedObject, DtsFileLocation, DtsObject, MCDObject, IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsFileLocationImpl(IntPtr handle)
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

  ~DtsFileLocationImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string FilePath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr filePath = CSWrap.CSNIDTS_DtsFileLocation_getFilePath(this.Handle, out returnValue);
      if (filePath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(filePath);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsFileLocation_setFilePath(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string ShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortName = CSWrap.CSNIDTS_DtsFileLocation_getShortName(this.Handle, out returnValue);
      if (shortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortName);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsFileLocation_setShortName(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Version
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr version = CSWrap.CSNIDTS_DtsFileLocation_getVersion(this.Handle, out returnValue);
      if (version != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(version);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsFileLocation_setVersion(this.Handle, value);
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
