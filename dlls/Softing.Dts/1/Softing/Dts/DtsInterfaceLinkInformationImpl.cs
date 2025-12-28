// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsInterfaceLinkInformationImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsInterfaceLinkInformationImpl : 
  MappedObject,
  DtsInterfaceLinkInformation,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsInterfaceLinkInformationImpl(IntPtr handle)
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

  ~DtsInterfaceLinkInformationImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbInterfaceConnectorPins ConnectorPins
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr connectorPins = CSWrap.CSNIDTS_DtsInterfaceLinkInformation_getConnectorPins(this.Handle, out returnValue);
      if (connectorPins != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(connectorPins);
      return (MCDDbInterfaceConnectorPins) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbInterfaceConnectorPins);
    }
  }

  public DtsPhysicalLinkOrInterfaceType LinkType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsPhysicalLinkOrInterfaceType returnValue;
      IntPtr linkType = CSWrap.CSNIDTS_DtsInterfaceLinkInformation_getLinkType(this.Handle, out returnValue);
      if (linkType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(linkType);
      return returnValue;
    }
  }

  public DtsPduApiLinkType PduApiLinkType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsPduApiLinkType returnValue;
      IntPtr pduApiLinkType = CSWrap.CSNIDTS_DtsInterfaceLinkInformation_getPduApiLinkType(this.Handle, out returnValue);
      if (pduApiLinkType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(pduApiLinkType);
      return returnValue;
    }
  }

  public uint LocalIndex
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr localIndex = CSWrap.CSNIDTS_DtsInterfaceLinkInformation_getLocalIndex(this.Handle, out returnValue);
      if (localIndex != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(localIndex);
      return returnValue;
    }
  }

  public string String
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceLinkInformation_getString(this.Handle, out returnValue);
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
