// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsJavaConfigImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsJavaConfigImpl : MappedObject, DtsJavaConfig, DtsObject, MCDObject, IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsJavaConfigImpl(IntPtr handle)
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

  ~DtsJavaConfigImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public DtsFileLocations JvmLocations
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr jvmLocations = CSWrap.CSNIDTS_DtsJavaConfig_getJvmLocations(this.Handle, out returnValue);
      if (jvmLocations != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(jvmLocations);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsFileLocations;
    }
  }

  public DtsFileLocations CompilerLocations
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr compilerLocations = CSWrap.CSNIDTS_DtsJavaConfig_getCompilerLocations(this.Handle, out returnValue);
      if (compilerLocations != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compilerLocations);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsFileLocations;
    }
  }

  public DtsFileLocation CurrentJvmLocation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr currentJvmLocation = CSWrap.CSNIDTS_DtsJavaConfig_getCurrentJvmLocation(this.Handle, out returnValue);
      if (currentJvmLocation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(currentJvmLocation);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsFileLocation;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsJavaConfig_setCurrentJvmLocation(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public DtsFileLocation CurrentCompilerLocation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr compilerLocation = CSWrap.CSNIDTS_DtsJavaConfig_getCurrentCompilerLocation(this.Handle, out returnValue);
      if (compilerLocation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compilerLocation);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsFileLocation;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsJavaConfig_setCurrentCompilerLocation(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool JobDebugging
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr jobDebugging = CSWrap.CSNIDTS_DtsJavaConfig_getJobDebugging(this.Handle, out returnValue);
      if (jobDebugging != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(jobDebugging);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsJavaConfig_setJobDebugging(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string CompilerOptions
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr compilerOptions = CSWrap.CSNIDTS_DtsJavaConfig_getCompilerOptions(this.Handle, out returnValue);
      if (compilerOptions != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compilerOptions);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsJavaConfig_setCompilerOptions(this.Handle, value);
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
