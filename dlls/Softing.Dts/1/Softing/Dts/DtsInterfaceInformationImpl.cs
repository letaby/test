// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsInterfaceInformationImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsInterfaceInformationImpl : 
  MappedObject,
  DtsInterfaceInformation,
  DtsNamedObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsInterfaceInformationImpl(IntPtr handle)
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

  ~DtsInterfaceInformationImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public DtsBusSystemInterfaceType BusSystemInterfaceType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsBusSystemInterfaceType returnValue;
      IntPtr systemInterfaceType = CSWrap.CSNIDTS_DtsInterfaceInformation_getBusSystemInterfaceType(this.Handle, out returnValue);
      if (systemInterfaceType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(systemInterfaceType);
      return returnValue;
    }
  }

  public bool SupportsIpAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr supportsIpAddress = CSWrap.CSNIDTS_DtsInterfaceInformation_getSupportsIpAddress(this.Handle, out returnValue);
      if (supportsIpAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(supportsIpAddress);
      return returnValue;
    }
  }

  public MCDDbInterfaceCables DbInterfaceCables
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbInterfaceCables = CSWrap.CSNIDTS_DtsInterfaceInformation_getDbInterfaceCables(this.Handle, out returnValue);
      if (dbInterfaceCables != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbInterfaceCables);
      return (MCDDbInterfaceCables) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbInterfaceCables);
    }
  }

  public string PDUAPIVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr pduapiVersion = CSWrap.CSNIDTS_DtsInterfaceInformation_getPDUAPIVersion(this.Handle, out returnValue);
      if (pduapiVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(pduapiVersion);
      return returnValue.makeString();
    }
  }

  public DtsInterfaceLinkInformations InterfaceLinks
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interfaceLinks = CSWrap.CSNIDTS_DtsInterfaceInformation_getInterfaceLinks(this.Handle, out returnValue);
      if (interfaceLinks != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceLinks);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceLinkInformations;
    }
  }

  public string[] VendorModuleNames
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr vendorModuleNames = CSWrap.CSNIDTS_DtsInterfaceInformation_getVendorModuleNames(this.Handle, out returnValue);
      if (vendorModuleNames != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vendorModuleNames);
      return returnValue.ToStringArray();
    }
  }

  public string DefaultCable
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr defaultCable = CSWrap.CSNIDTS_DtsInterfaceInformation_getDefaultCable(this.Handle, out returnValue);
      if (defaultCable != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(defaultCable);
      return returnValue.makeString();
    }
  }

  public string Description
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr description = CSWrap.CSNIDTS_DtsNamedObject_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
  }

  public string ShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortName = CSWrap.CSNIDTS_DtsNamedObject_getShortName(this.Handle, out returnValue);
      if (shortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortName);
      return returnValue.makeString();
    }
  }

  public string LongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longName = CSWrap.CSNIDTS_DtsNamedObject_getLongName(this.Handle, out returnValue);
      if (longName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longName);
      return returnValue.makeString();
    }
  }

  public uint StringID
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr stringId = CSWrap.CSNIDTS_DtsNamedObject_getStringID(this.Handle, out returnValue);
      if (stringId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(stringId);
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
