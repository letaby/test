// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbProjectConfigurationImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbProjectConfigurationImpl : 
  MappedObject,
  DtsDbProjectConfiguration,
  MCDDbProjectConfiguration,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbProjectConfigurationImpl(IntPtr handle)
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

  ~DtsDbProjectConfigurationImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbProject ActiveDbProject
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr activeDbProject = CSWrap.CSNIDTS_DtsDbProjectConfiguration_getActiveDbProject(this.Handle, out returnValue);
      if (activeDbProject != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(activeDbProject);
      return (MCDDbProject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProject);
    }
  }

  public void Close()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbProjectConfiguration_close(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbProject Load(string name)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbProjectConfiguration_load(this.Handle, name, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDDbProject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProject);
  }

  public string[] AdditionalECUMEMNames
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr additionalEcumemNames = CSWrap.CSNIDTS_DtsDbProjectConfiguration_getAdditionalECUMEMNames(this.Handle, out returnValue);
      if (additionalEcumemNames != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalEcumemNames);
      return returnValue.ToStringArray();
    }
  }

  public void UnloadConfigurationProject()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbProjectConfiguration_unloadConfigurationProject(this.Handle);
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
