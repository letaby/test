// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbIdentDescriptionImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbIdentDescriptionImpl : 
  MappedObject,
  DtsDbIdentDescription,
  MCDDbIdentDescription,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbIdentDescriptionImpl(IntPtr handle)
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

  ~DtsDbIdentDescriptionImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbDataPrimitive DbDataPrimitive
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDataPrimitive = CSWrap.CSNIDTS_DtsDbIdentDescription_getDbDataPrimitive(this.Handle, out returnValue);
      if (dbDataPrimitive != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDataPrimitive);
      return (MCDDbDataPrimitive) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataPrimitive);
    }
  }

  public MCDDbResponseParameter DbResponseParameter
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr responseParameter = CSWrap.CSNIDTS_DtsDbIdentDescription_getDbResponseParameter(this.Handle, out returnValue);
      if (responseParameter != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(responseParameter);
      return (MCDDbResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbResponseParameter);
    }
  }

  public MCDDbDataPrimitive GetDbDataPrimitiveByLocation(MCDDbLocation pDtsDbLocation)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr primitiveByLocation = CSWrap.CSNIDTS_DtsDbIdentDescription_getDbDataPrimitiveByLocation(this.Handle, DTS_ObjectMapper.getHandle(pDtsDbLocation as MappedObject), out returnValue);
    if (primitiveByLocation != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(primitiveByLocation);
    return (MCDDbDataPrimitive) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataPrimitive);
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
