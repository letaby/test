// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsInterfaceConfigImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsInterfaceConfigImpl : 
  MappedObject,
  DtsInterfaceConfig,
  DtsNamedObjectConfig,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsInterfaceConfigImpl(IntPtr handle)
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

  ~DtsInterfaceConfigImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string ModuleType
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr moduleType = CSWrap.CSNIDTS_DtsInterfaceConfig_getModuleType(this.Handle, out returnValue);
      if (moduleType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(moduleType);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setModuleType(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string PDUAPIVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr pduapiVersion = CSWrap.CSNIDTS_DtsInterfaceConfig_getPDUAPIVersion(this.Handle, out returnValue);
      if (pduapiVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(pduapiVersion);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setPDUAPIVersion(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool Enabled
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr enabled = CSWrap.CSNIDTS_DtsInterfaceConfig_getEnabled(this.Handle, out returnValue);
      if (enabled != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(enabled);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setEnabled(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public DtsBusSystemInterfaceType BusSystemInterfaceType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsBusSystemInterfaceType returnValue;
      IntPtr systemInterfaceType = CSWrap.CSNIDTS_DtsInterfaceConfig_getBusSystemInterfaceType(this.Handle, out returnValue);
      if (systemInterfaceType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(systemInterfaceType);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setBusSystemInterfaceType(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public DtsInterfaceInformation InterfaceInformation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interfaceInformation = CSWrap.CSNIDTS_DtsInterfaceConfig_getInterfaceInformation(this.Handle, out returnValue);
      if (interfaceInformation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceInformation);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceInformation;
    }
  }

  public string SerialNumber
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr serialNumber = CSWrap.CSNIDTS_DtsInterfaceConfig_getSerialNumber(this.Handle, out returnValue);
      if (serialNumber != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(serialNumber);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setSerialNumber(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string IpAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr ipAddress = CSWrap.CSNIDTS_DtsInterfaceConfig_getIpAddress(this.Handle, out returnValue);
      if (ipAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ipAddress);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setIpAddress(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Cable
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr cable = CSWrap.CSNIDTS_DtsInterfaceConfig_getCable(this.Handle, out returnValue);
      if (cable != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(cable);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setCable(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public DtsInterfaceLinkInformations InterfaceLinkInformations
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr linkInformations = CSWrap.CSNIDTS_DtsInterfaceConfig_getInterfaceLinkInformations(this.Handle, out returnValue);
      if (linkInformations != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(linkInformations);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceLinkInformations;
    }
  }

  public DtsInterfaceLinkConfigs InterfaceLinkConfigs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interfaceLinkConfigs = CSWrap.CSNIDTS_DtsInterfaceConfig_getInterfaceLinkConfigs(this.Handle, out returnValue);
      if (interfaceLinkConfigs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceLinkConfigs);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceLinkConfigs;
    }
  }

  public string VendorModuleName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr vendorModuleName = CSWrap.CSNIDTS_DtsInterfaceConfig_getVendorModuleName(this.Handle, out returnValue);
      if (vendorModuleName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vendorModuleName);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setVendorModuleName(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool UseForLicensing
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr useForLicensing = CSWrap.CSNIDTS_DtsInterfaceConfig_getUseForLicensing(this.Handle, out returnValue);
      if (useForLicensing != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(useForLicensing);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterfaceConfig_setUseForLicensing(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool GetConnectedStatus(bool doDetection)
  {
    GC.KeepAlive((object) this);
    bool returnValue;
    IntPtr connectedStatus = CSWrap.CSNIDTS_DtsInterfaceConfig_getConnectedStatus(this.Handle, doDetection, out returnValue);
    if (connectedStatus != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(connectedStatus);
    return returnValue;
  }

  public string DetectedSerialNumber
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr detectedSerialNumber = CSWrap.CSNIDTS_DtsInterfaceConfig_getDetectedSerialNumber(this.Handle, out returnValue);
      if (detectedSerialNumber != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(detectedSerialNumber);
      return returnValue.makeString();
    }
  }

  public string ShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortName = CSWrap.CSNIDTS_DtsNamedObjectConfig_getShortName(this.Handle, out returnValue);
      if (shortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortName);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsNamedObjectConfig_setShortName(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string LongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longName = CSWrap.CSNIDTS_DtsNamedObjectConfig_getLongName(this.Handle, out returnValue);
      if (longName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longName);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsNamedObjectConfig_setLongName(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Description
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr description = CSWrap.CSNIDTS_DtsNamedObjectConfig_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsNamedObjectConfig_setDescription(this.Handle, value);
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
