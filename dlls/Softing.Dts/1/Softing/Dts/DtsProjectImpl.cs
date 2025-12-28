// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsProjectImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsProjectImpl : 
  MappedObject,
  DtsProject,
  MCDProject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsProjectImpl(IntPtr handle)
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

  ~DtsProjectImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbVehicleInformation SelectDbVehicleInformationByName(string shortName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_selectDbVehicleInformationByName(this.Handle, shortName, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDDbVehicleInformation) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbVehicleInformation);
  }

  public void SelectDbVehicleInformation(MCDDbVehicleInformation vehicleInformation)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_selectDbVehicleInformation(this.Handle, DTS_ObjectMapper.getHandle(vehicleInformation as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDClampState GetClampState(string clampName)
  {
    GC.KeepAlive((object) this);
    MCDClampState returnValue;
    IntPtr clampState = CSWrap.CSNIDTS_DtsProject_getClampState(this.Handle, clampName, out returnValue);
    if (clampState != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(clampState);
    return returnValue;
  }

  public void DeselectVehicleInformation()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_deselectVehicleInformation(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbProject DbProject
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbProject = CSWrap.CSNIDTS_DtsProject_getDbProject(this.Handle, out returnValue);
      if (dbProject != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbProject);
      return (MCDDbProject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProject);
    }
  }

  public MCDDbVehicleInformation ActiveDbVehicleInformation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr vehicleInformation = CSWrap.CSNIDTS_DtsProject_getActiveDbVehicleInformation(this.Handle, out returnValue);
      if (vehicleInformation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vehicleInformation);
      return (MCDDbVehicleInformation) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbVehicleInformation);
    }
  }

  public MCDLogicalLink CreateLogicalLink(MCDDbLogicalLink LogicalLink)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr logicalLink = CSWrap.CSNIDTS_DtsProject_createLogicalLink(this.Handle, DTS_ObjectMapper.getHandle(LogicalLink as MappedObject), out returnValue);
    if (logicalLink != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(logicalLink);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByAccessKey(
    string AccessKeyString,
    string PhysicalVehicleLinkString)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr logicalLinkByAccessKey = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByAccessKey(this.Handle, AccessKeyString, PhysicalVehicleLinkString, out returnValue);
    if (logicalLinkByAccessKey != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(logicalLinkByAccessKey);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByName(string LogicalLinkName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr logicalLinkByName = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByName(this.Handle, LogicalLinkName, out returnValue);
    if (logicalLinkByName != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(logicalLinkByName);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByVariant(
    string BaseVariantLogicalLinkName,
    string VariantName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr logicalLinkByVariant = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByVariant(this.Handle, BaseVariantLogicalLinkName, VariantName, out returnValue);
    if (logicalLinkByVariant != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(logicalLinkByVariant);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public void RemoveLogicalLink(MCDLogicalLink LogicalLink)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_removeLogicalLink(this.Handle, DTS_ObjectMapper.getHandle(LogicalLink as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string[] TraceFilterNames
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr traceFilterNames = CSWrap.CSNIDTS_DtsProject_getTraceFilterNames(this.Handle, out returnValue);
      if (traceFilterNames != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceFilterNames);
      return returnValue.ToStringArray();
    }
  }

  public DtsLogicalLinkMonitor CreateDtsLogicalLinkMonitor(string PilShortName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr logicalLinkMonitor = CSWrap.CSNIDTS_DtsProject_createDtsLogicalLinkMonitor(this.Handle, PilShortName, out returnValue);
    if (logicalLinkMonitor != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(logicalLinkMonitor);
    return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLinkMonitor;
  }

  public string[] ListAddonFiles(
    string strDirectory,
    string strBaseVariant,
    string strVariant,
    string strIdents,
    bool bReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_listAddonFiles(this.Handle, strDirectory, strBaseVariant, strVariant, strIdents, bReload, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue.ToStringArray();
  }

  public void UnlinkDatabaseFiles()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_unlinkDatabaseFiles(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void ReplaceProjectFlashFile(string strOldFile, string strNewFile)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_replaceProjectFlashFile(this.Handle, strOldFile, strNewFile);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public int CustomerVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr customerVersion = CSWrap.CSNIDTS_DtsProject_getCustomerVersion(this.Handle, out returnValue);
      if (customerVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(customerVersion);
      return returnValue;
    }
  }

  public MCDValue CreateValue(MCDDataType dataType)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_createValue(this.Handle, dataType, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public MCDLogicalLink CreateLogicalLinkByAccessKeyAndInterface(
    string accessKeyString,
    string physicalVehicleLinkString,
    MCDInterface interface_)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr accessKeyAndInterface = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByAccessKeyAndInterface(this.Handle, accessKeyString, physicalVehicleLinkString, DTS_ObjectMapper.getHandle(interface_ as MappedObject), out returnValue);
    if (accessKeyAndInterface != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(accessKeyAndInterface);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByAccessKeyAndInterfaceResource(
    string accessKeyString,
    string physicalVehicleLinkString,
    MCDInterfaceResource interfaceResource)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr interfaceResource1 = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByAccessKeyAndInterfaceResource(this.Handle, accessKeyString, physicalVehicleLinkString, DTS_ObjectMapper.getHandle(interfaceResource as MappedObject), out returnValue);
    if (interfaceResource1 != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(interfaceResource1);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByInterface(
    MCDDbLogicalLink logicalLink,
    MCDInterface interface_)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr logicalLinkByInterface = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByInterface(this.Handle, DTS_ObjectMapper.getHandle(logicalLink as MappedObject), DTS_ObjectMapper.getHandle(interface_ as MappedObject), out returnValue);
    if (logicalLinkByInterface != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(logicalLinkByInterface);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByInterfaceResource(
    MCDDbLogicalLink logicalLink,
    MCDInterfaceResource interfaceResource)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr interfaceResource1 = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByInterfaceResource(this.Handle, DTS_ObjectMapper.getHandle(logicalLink as MappedObject), DTS_ObjectMapper.getHandle(interfaceResource as MappedObject), out returnValue);
    if (interfaceResource1 != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(interfaceResource1);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByNameAndInterface(
    string logicalLinkName,
    MCDInterface interface_)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr nameAndInterface = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByNameAndInterface(this.Handle, logicalLinkName, DTS_ObjectMapper.getHandle(interface_ as MappedObject), out returnValue);
    if (nameAndInterface != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(nameAndInterface);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByNameAndInterfaceResource(
    string logicalLinkName,
    MCDInterfaceResource interfaceResource)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr interfaceResource1 = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByNameAndInterfaceResource(this.Handle, logicalLinkName, DTS_ObjectMapper.getHandle(interfaceResource as MappedObject), out returnValue);
    if (interfaceResource1 != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(interfaceResource1);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByVariantAndInterface(
    string shortNameDbLogicalLink,
    string variantName,
    MCDInterface interface_)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr variantAndInterface = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByVariantAndInterface(this.Handle, shortNameDbLogicalLink, variantName, DTS_ObjectMapper.getHandle(interface_ as MappedObject), out returnValue);
    if (variantAndInterface != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(variantAndInterface);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDLogicalLink CreateLogicalLinkByVariantAndInterfaceResource(
    string shortNameDbLogicalLink,
    string variantName,
    MCDInterfaceResource interfaceResource)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr interfaceResource1 = CSWrap.CSNIDTS_DtsProject_createLogicalLinkByVariantAndInterfaceResource(this.Handle, shortNameDbLogicalLink, variantName, DTS_ObjectMapper.getHandle(interfaceResource as MappedObject), out returnValue);
    if (interfaceResource1 != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(interfaceResource1);
    return (MCDLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLink);
  }

  public MCDMonitoringLink CreateMonitoringLink(MCDInterfaceResource ifResource)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr monitoringLink = CSWrap.CSNIDTS_DtsProject_createMonitoringLink(this.Handle, DTS_ObjectMapper.getHandle(ifResource as MappedObject), out returnValue);
    if (monitoringLink != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(monitoringLink);
    return (MCDMonitoringLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitoringLink);
  }

  public MCDValues ExecIOCtrl(
    string IOCtrlName,
    MCDValue inputData,
    uint inputDataItemType,
    uint outputDataSize)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_execIOCtrl(this.Handle, IOCtrlName, DTS_ObjectMapper.getHandle(inputData as MappedObject), inputDataItemType, outputDataSize, out returnValue);
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
      IntPtr ioControlNames = CSWrap.CSNIDTS_DtsProject_getIOControlNames(this.Handle, out returnValue);
      if (ioControlNames != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ioControlNames);
      return returnValue.ToStringArray();
    }
  }

  public void RemoveMonitoringLink(MCDMonitoringLink monLink)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_removeMonitoringLink(this.Handle, DTS_ObjectMapper.getHandle(monLink as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public DtsGlobalProtocolParameterSets GlobalProtocolParameterSets
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr protocolParameterSets = CSWrap.CSNIDTS_DtsProject_getGlobalProtocolParameterSets(this.Handle, out returnValue);
      if (protocolParameterSets != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(protocolParameterSets);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsGlobalProtocolParameterSets;
    }
  }

  public void UnlinkDatabaseFile(string strFile)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_unlinkDatabaseFile(this.Handle, strFile);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbEcuMems LinkDatabaseFile(string strFile)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_linkDatabaseFile(this.Handle, strFile, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDDbEcuMems) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuMems);
  }

  public DtsFileLocations DatabaseFileList
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr databaseFileList = CSWrap.CSNIDTS_DtsProject_getDatabaseFileList(this.Handle, out returnValue);
      if (databaseFileList != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(databaseFileList);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsFileLocations;
    }
  }

  public MCDMonitoringLink CreateDtsMonitoringLink(string PilShortName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr dtsMonitoringLink = CSWrap.CSNIDTS_DtsProject_createDtsMonitoringLink(this.Handle, PilShortName, out returnValue);
    if (dtsMonitoringLink != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(dtsMonitoringLink);
    return (MCDMonitoringLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitoringLink);
  }

  public string ActiveSimFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr activeSimFile = CSWrap.CSNIDTS_DtsProject_getActiveSimFile(this.Handle, out returnValue);
      if (activeSimFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(activeSimFile);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_setActiveSimFile(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string[] SimFiles
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr simFiles = CSWrap.CSNIDTS_DtsProject_getSimFiles(this.Handle, out returnValue);
      if (simFiles != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(simFiles);
      return returnValue.ToStringArray();
    }
  }

  public DtsDoIPMonitorLink CreateDoIPMonitorLink(string NetworkId)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr doIpMonitorLink = CSWrap.CSNIDTS_DtsProject_createDoIPMonitorLink(this.Handle, NetworkId, out returnValue);
    if (doIpMonitorLink != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(doIpMonitorLink);
    return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDoIPMonitorLink;
  }

  public uint Characteristic
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr characteristic = CSWrap.CSNIDTS_DtsProject_getCharacteristic(this.Handle, out returnValue);
      if (characteristic != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(characteristic);
      return returnValue;
    }
  }

  public string[] AglFiles
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr aglFiles = CSWrap.CSNIDTS_DtsProject_getAglFiles(this.Handle, out returnValue);
      if (aglFiles != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(aglFiles);
      return returnValue.ToStringArray();
    }
  }

  public void ClearLinkCache()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProject_clearLinkCache(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string ProjectUid
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr projectUid = CSWrap.CSNIDTS_DtsProject_getProjectUid(this.Handle, out returnValue);
      if (projectUid != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(projectUid);
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
