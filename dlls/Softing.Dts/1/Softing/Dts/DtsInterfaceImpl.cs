// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsInterfaceImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsInterfaceImpl : 
  MappedObject,
  DtsInterface,
  MCDInterface,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsInterfaceImpl(IntPtr handle)
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

  ~DtsInterfaceImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDValue GetClampState(uint pinOnInterfaceConnector, string clampName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr clampState = CSWrap.CSNIDTS_DtsInterface_getClampState(this.Handle, pinOnInterfaceConnector, clampName, out returnValue);
    if (clampState != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(clampState);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public string PDUApiSoftwareName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr pduApiSoftwareName = CSWrap.CSNIDTS_DtsInterface_getPDUApiSoftwareName(this.Handle, out returnValue);
      if (pduApiSoftwareName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(pduApiSoftwareName);
      return returnValue.makeString();
    }
  }

  public MCDVersion PDUApiSoftwareVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr apiSoftwareVersion = CSWrap.CSNIDTS_DtsInterface_getPDUApiSoftwareVersion(this.Handle, out returnValue);
      if (apiSoftwareVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(apiSoftwareVersion);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public string VendorName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr vendorName = CSWrap.CSNIDTS_DtsInterface_getVendorName(this.Handle, out returnValue);
      if (vendorName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vendorName);
      return returnValue.makeString();
    }
  }

  public bool EthernetActivation
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr ethernetActivation = CSWrap.CSNIDTS_DtsInterface_getEthernetActivation(this.Handle, out returnValue);
      if (ethernetActivation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ethernetActivation);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_setEthernetActivation(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool ExecuteBroadcast()
  {
    GC.KeepAlive((object) this);
    bool returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_executeBroadcast(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public void Connect()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_connect(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Disconnect()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_disconnect(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public uint HardwareSerialNumber
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr hardwareSerialNumber = CSWrap.CSNIDTS_DtsInterface_getHardwareSerialNumber(this.Handle, out returnValue);
      if (hardwareSerialNumber != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(hardwareSerialNumber);
      return returnValue;
    }
  }

  public MCDValues ExecIOCtrl(
    string IOCtrlName,
    MCDValue inputData,
    uint inputDataItemType,
    uint outputDataSize)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_execIOCtrl(this.Handle, IOCtrlName, DTS_ObjectMapper.getHandle(inputData as MappedObject), inputDataItemType, outputDataSize, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues);
  }

  public string[] IOControlNames
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr ioControlNames = CSWrap.CSNIDTS_DtsInterface_getIOControlNames(this.Handle, out returnValue);
      if (ioControlNames != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ioControlNames);
      return returnValue.ToStringArray();
    }
  }

  public void DetectInterfaces(string optionString)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_detectInterfaces(this.Handle, optionString);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbInterfaceCable CurrentDbInterfaceCable
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbInterfaceCable = CSWrap.CSNIDTS_DtsInterface_getCurrentDbInterfaceCable(this.Handle, out returnValue);
      if (dbInterfaceCable != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbInterfaceCable);
      return (MCDDbInterfaceCable) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbInterfaceCable);
    }
  }

  public ulong CurrentTime
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr currentTime = CSWrap.CSNIDTS_DtsInterface_getCurrentTime(this.Handle, out returnValue);
      if (currentTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(currentTime);
      return returnValue;
    }
  }

  public MCDDbInterfaceCables DbInterfaceCables
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbInterfaceCables = CSWrap.CSNIDTS_DtsInterface_getDbInterfaceCables(this.Handle, out returnValue);
      if (dbInterfaceCables != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbInterfaceCables);
      return (MCDDbInterfaceCables) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbInterfaceCables);
    }
  }

  public string FirmwareName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr firmwareName = CSWrap.CSNIDTS_DtsInterface_getFirmwareName(this.Handle, out returnValue);
      if (firmwareName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(firmwareName);
      return returnValue.makeString();
    }
  }

  public MCDVersion FirmwareVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr firmwareVersion = CSWrap.CSNIDTS_DtsInterface_getFirmwareVersion(this.Handle, out returnValue);
      if (firmwareVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(firmwareVersion);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public string HardwareName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr hardwareName = CSWrap.CSNIDTS_DtsInterface_getHardwareName(this.Handle, out returnValue);
      if (hardwareName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(hardwareName);
      return returnValue.makeString();
    }
  }

  public MCDVersion HardwareVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr hardwareVersion = CSWrap.CSNIDTS_DtsInterface_getHardwareVersion(this.Handle, out returnValue);
      if (hardwareVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(hardwareVersion);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public MCDInterfaceResources InterfaceResources
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interfaceResources = CSWrap.CSNIDTS_DtsInterface_getInterfaceResources(this.Handle, out returnValue);
      if (interfaceResources != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceResources);
      return (MCDInterfaceResources) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceResources);
    }
  }

  public MCDVersion MVCIVersionPart1StandardVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr part1StandardVersion = CSWrap.CSNIDTS_DtsInterface_getMVCIVersionPart1StandardVersion(this.Handle, out returnValue);
      if (part1StandardVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(part1StandardVersion);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public MCDVersion MVCIVersionPart2StandardVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr part2StandardVersion = CSWrap.CSNIDTS_DtsInterface_getMVCIVersionPart2StandardVersion(this.Handle, out returnValue);
      if (part2StandardVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(part2StandardVersion);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public MCDInterfaceStatus Status
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDInterfaceStatus returnValue;
      IntPtr status = CSWrap.CSNIDTS_DtsInterface_getStatus(this.Handle, out returnValue);
      if (status != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(status);
      return returnValue;
    }
  }

  public void Reset()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_reset(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public double GetProgrammingVoltage(uint pinOnInterfaceConnector)
  {
    GC.KeepAlive((object) this);
    double returnValue;
    IntPtr programmingVoltage = CSWrap.CSNIDTS_DtsInterface_getProgrammingVoltage(this.Handle, pinOnInterfaceConnector, out returnValue);
    if (programmingVoltage != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(programmingVoltage);
    return returnValue;
  }

  public double BatteryVoltage
  {
    get
    {
      GC.KeepAlive((object) this);
      double returnValue;
      IntPtr batteryVoltage = CSWrap.CSNIDTS_DtsInterface_getBatteryVoltage(this.Handle, out returnValue);
      if (batteryVoltage != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(batteryVoltage);
      return returnValue;
    }
  }

  public void SetProgrammingVoltage(uint pinOnInterfaceConnector, double voltage)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_setProgrammingVoltage(this.Handle, pinOnInterfaceConnector, voltage);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void SetEthernetPinState(bool State, uint Number)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_setEthernetPinState(this.Handle, State, Number);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public uint GetEthernetPinState(uint Number)
  {
    GC.KeepAlive((object) this);
    uint returnValue;
    IntPtr ethernetPinState = CSWrap.CSNIDTS_DtsInterface_getEthernetPinState(this.Handle, Number, out returnValue);
    if (ethernetPinState != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(ethernetPinState);
    return returnValue;
  }

  public string VendorModuleName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr vendorModuleName = CSWrap.CSNIDTS_DtsInterface_getVendorModuleName(this.Handle, out returnValue);
      if (vendorModuleName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vendorModuleName);
      return returnValue.makeString();
    }
  }

  public bool SetPhysicalLinkId(string keyLink, uint id)
  {
    GC.KeepAlive((object) this);
    bool returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsInterface_setPhysicalLinkId(this.Handle, keyLink, id, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public DtsWLanSignalData WLanSignalData
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr wlanSignalData = CSWrap.CSNIDTS_DtsInterface_getWLanSignalData(this.Handle, out returnValue);
      if (wlanSignalData != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(wlanSignalData);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsWLanSignalData;
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
