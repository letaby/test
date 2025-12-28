// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsInterfaceLinkConfigImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsInterfaceLinkConfigImpl : 
  MappedObject,
  DtsInterfaceLinkConfig,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsInterfaceLinkConfigImpl(IntPtr handle)
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

  ~DtsInterfaceLinkConfigImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public DtsPhysicalLinkOrInterfaceType LinkType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsPhysicalLinkOrInterfaceType returnValue;
      IntPtr linkType = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getLinkType(this.Handle, out returnValue);
      if (linkType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(linkType);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_setLinkType(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public DtsPduApiLinkType PduApiLinkType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsPduApiLinkType returnValue;
      IntPtr pduApiLinkType = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getPduApiLinkType(this.Handle, out returnValue);
      if (pduApiLinkType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(pduApiLinkType);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_setPduApiLinkType(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public int GlobalIndex
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr globalIndex = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getGlobalIndex(this.Handle, out returnValue);
      if (globalIndex != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(globalIndex);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_setGlobalIndex(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public int LocalIndex
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr localIndex = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getLocalIndex(this.Handle, out returnValue);
      if (localIndex != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(localIndex);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_setLocalIndex(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void Assign(DtsInterfaceLinkInformation linkInformation)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_assign(this.Handle, DTS_ObjectMapper.getHandle(linkInformation as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDConnectorPinType GetPinType(uint index)
  {
    GC.KeepAlive((object) this);
    MCDConnectorPinType returnValue;
    IntPtr pinType = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getPinType(this.Handle, index, out returnValue);
    if (pinType != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(pinType);
    return returnValue;
  }

  public uint GetVehiclePin(uint index)
  {
    GC.KeepAlive((object) this);
    uint returnValue;
    IntPtr vehiclePin = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getVehiclePin(this.Handle, index, out returnValue);
    if (vehiclePin != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(vehiclePin);
    return returnValue;
  }

  public uint PinCount
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr pinCount = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getPinCount(this.Handle, out returnValue);
      if (pinCount != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(pinCount);
      return returnValue;
    }
  }

  public string String
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceLinkConfig_getString(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue.makeString();
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
