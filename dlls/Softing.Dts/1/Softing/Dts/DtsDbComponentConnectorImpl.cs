// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbComponentConnectorImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbComponentConnectorImpl : 
  MappedObject,
  DtsDbComponentConnector,
  MCDDbComponentConnector,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbComponentConnectorImpl(IntPtr handle)
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

  ~DtsDbComponentConnectorImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbEcuBaseVariant DbEcuBaseVariant
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbEcuBaseVariant = CSWrap.CSNIDTS_DtsDbComponentConnector_getDbEcuBaseVariant(this.Handle, out returnValue);
      if (dbEcuBaseVariant != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbEcuBaseVariant);
      return (MCDDbEcuBaseVariant) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuBaseVariant);
    }
  }

  public MCDDbEcuVariants DbEcuVariants
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbEcuVariants = CSWrap.CSNIDTS_DtsDbComponentConnector_getDbEcuVariants(this.Handle, out returnValue);
      if (dbEcuVariants != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbEcuVariants);
      return (MCDDbEcuVariants) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuVariants);
    }
  }

  public MCDDbDiagObjectConnector GetDbDiagObjectConnector(MCDDbLocation locationContext)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr diagObjectConnector = CSWrap.CSNIDTS_DtsDbComponentConnector_getDbDiagObjectConnector(this.Handle, DTS_ObjectMapper.getHandle(locationContext as MappedObject), out returnValue);
    if (diagObjectConnector != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(diagObjectConnector);
    return (MCDDbDiagObjectConnector) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagObjectConnector);
  }

  public MCDDbLocations DbLocationsForDiagObjectConnector
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr diagObjectConnector = CSWrap.CSNIDTS_DtsDbComponentConnector_getDbLocationsForDiagObjectConnector(this.Handle, out returnValue);
      if (diagObjectConnector != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(diagObjectConnector);
      return (MCDDbLocations) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLocations);
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
