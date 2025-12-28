// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsSystemImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsSystemImpl : 
  MappedObject,
  DtsSystem,
  MCDSystem,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;
  private uint handler_count;
  private uint listener_handle;

  public DtsSystemImpl(IntPtr handle)
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
    if (this.listener_handle != 0U)
    {
      IntPtr Handle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, this.listener_handle);
      if (Handle != IntPtr.Zero)
        CSWrap.CSNIDTS_releaseObject(Handle);
      this.listener_handle = 0U;
    }
    DTS_ObjectMapper.unregisterObject(this.Handle);
    this.Handle = IntPtr.Zero;
  }

  ~DtsSystemImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public void CloseByteTrace(string PhysicalInterfaceLink)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_closeByteTrace(this.Handle, PhysicalInterfaceLink);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void DeselectProject()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_deselectProject(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void DisableApiTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_disableApiTrace(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void EnableApiTrace()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_enableApiTrace(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDProject ActiveProject
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr activeProject = CSWrap.CSNIDTS_DtsSystem_getActiveProject(this.Handle, out returnValue);
      if (activeProject != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(activeProject);
      return (MCDProject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProject);
    }
  }

  public MCDVersion ASAMMCDVersion
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr asammcdVersion = CSWrap.CSNIDTS_DtsSystem_getASAMMCDVersion(this.Handle, out returnValue);
      if (asammcdVersion != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(asammcdVersion);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public DtsClassFactory ClassFactory
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr classFactory = CSWrap.CSNIDTS_DtsSystem_getClassFactory(this.Handle, out returnValue);
      if (classFactory != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(classFactory);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsClassFactory;
    }
  }

  public string CurrentTracePath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr currentTracePath = CSWrap.CSNIDTS_DtsSystem_getCurrentTracePath(this.Handle, out returnValue);
      if (currentTracePath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(currentTracePath);
      return returnValue.makeString();
    }
  }

  public MCDDbProjectDescriptions DbProjectDescriptions
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr projectDescriptions = CSWrap.CSNIDTS_DtsSystem_getDbProjectDescriptions(this.Handle, out returnValue);
      if (projectDescriptions != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(projectDescriptions);
      return (MCDDbProjectDescriptions) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProjectDescriptions);
    }
  }

  public string InstallationPath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr installationPath = CSWrap.CSNIDTS_DtsSystem_getInstallationPath(this.Handle, out returnValue);
      if (installationPath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(installationPath);
      return returnValue.makeString();
    }
  }

  public MCDLockState LockState
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLockState returnValue;
      IntPtr lockState = CSWrap.CSNIDTS_DtsSystem_getLockState(this.Handle, out returnValue);
      if (lockState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lockState);
      return returnValue;
    }
  }

  public uint MaxNoOfClients
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr maxNoOfClients = CSWrap.CSNIDTS_DtsSystem_getMaxNoOfClients(this.Handle, out returnValue);
      if (maxNoOfClients != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(maxNoOfClients);
      return returnValue;
    }
  }

  public string ProjectPath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr projectPath = CSWrap.CSNIDTS_DtsSystem_getProjectPath(this.Handle, out returnValue);
      if (projectPath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(projectPath);
      return returnValue.makeString();
    }
  }

  public MCDServerType ServerType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDServerType returnValue;
      IntPtr serverType = CSWrap.CSNIDTS_DtsSystem_getServerType(this.Handle, out returnValue);
      if (serverType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(serverType);
      return returnValue;
    }
  }

  public MCDSystemState State
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDSystemState returnValue;
      IntPtr state = CSWrap.CSNIDTS_DtsSystem_getState(this.Handle, out returnValue);
      if (state != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(state);
      return returnValue;
    }
  }

  public string GetStringFromEnum(uint eConst)
  {
    GC.KeepAlive((object) this);
    String_Struct returnValue = new String_Struct();
    IntPtr stringFromEnum = CSWrap.CSNIDTS_DtsSystem_getStringFromEnum(this.Handle, eConst, out returnValue);
    if (stringFromEnum != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(stringFromEnum);
    return returnValue.makeString();
  }

  public string GetStringFromErrorCode(MCDErrorCodes errorCode)
  {
    GC.KeepAlive((object) this);
    String_Struct returnValue = new String_Struct();
    IntPtr stringFromErrorCode = CSWrap.CSNIDTS_DtsSystem_getStringFromErrorCode(this.Handle, errorCode, out returnValue);
    if (stringFromErrorCode != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(stringFromErrorCode);
    return returnValue.makeString();
  }

  public MCDVersion Version
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr version = CSWrap.CSNIDTS_DtsSystem_getVersion(this.Handle, out returnValue);
      if (version != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(version);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public void Initialize()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_initialize(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public bool IsApiTraceEnabled
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_isApiTraceEnabled(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public void OpenByteTrace(string PhysicalInterfaceLink, string FileName)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_openByteTrace(this.Handle, PhysicalInterfaceLink, FileName);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void PrepareInterface()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_prepareInterface(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDProject SelectProject(MCDDbProjectDescription project)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_selectProject(this.Handle, DTS_ObjectMapper.getHandle(project as MappedObject), out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDProject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProject);
  }

  public MCDProject SelectProjectByName(string project)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_selectProjectByName(this.Handle, project, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDProject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProject);
  }

  public uint ApiTraceLevel
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_setApiTraceLevel(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void StartByteTrace(string PhysicalInterfaceLink)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_startByteTrace(this.Handle, PhysicalInterfaceLink);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void StopByteTrace(string PhysicalInterfaceLink)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_stopByteTrace(this.Handle, PhysicalInterfaceLink);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Uninitialize()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_uninitialize(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void UnprepareInterface()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_unprepareInterface(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string TracePath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr tracePath = CSWrap.CSNIDTS_DtsSystem_getTracePath(this.Handle, out returnValue);
      if (tracePath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(tracePath);
      return returnValue.makeString();
    }
  }

  public MCDDbProjectConfiguration DbProjectConfiguration
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr projectConfiguration = CSWrap.CSNIDTS_DtsSystem_getDbProjectConfiguration(this.Handle, out returnValue);
      if (projectConfiguration != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(projectConfiguration);
      return (MCDDbProjectConfiguration) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProjectConfiguration);
    }
  }

  public MCDDbPhysicalVehicleLinkOrInterfaces DbPhysicalInterfaceLinks
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr physicalInterfaceLinks = CSWrap.CSNIDTS_DtsSystem_getDbPhysicalInterfaceLinks(this.Handle, out returnValue);
      if (physicalInterfaceLinks != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalInterfaceLinks);
      return (MCDDbPhysicalVehicleLinkOrInterfaces) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbPhysicalVehicleLinkOrInterfaces);
    }
  }

  public DtsMonitorLink CreateMonitorLinkByName(string PhysicalInterfaceLinkName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr monitorLinkByName = CSWrap.CSNIDTS_DtsSystem_createMonitorLinkByName(this.Handle, PhysicalInterfaceLinkName, out returnValue);
    if (monitorLinkByName != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(monitorLinkByName);
    return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitorLink;
  }

  public DtsMonitorLink CreateMonitorLink(
    MCDDbPhysicalVehicleLinkOrInterface PhysicalInterfaceLink)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr monitorLink = CSWrap.CSNIDTS_DtsSystem_createMonitorLink(this.Handle, DTS_ObjectMapper.getHandle(PhysicalInterfaceLink as MappedObject), out returnValue);
    if (monitorLink != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(monitorLink);
    return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMonitorLink;
  }

  public uint RegisterApp(uint appID, uint reqItem)
  {
    GC.KeepAlive((object) this);
    uint returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_registerApp(this.Handle, appID, reqItem, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public uint GetRequiredItem(uint ulID, uint reqItem)
  {
    GC.KeepAlive((object) this);
    uint returnValue;
    IntPtr requiredItem = CSWrap.CSNIDTS_DtsSystem_getRequiredItem(this.Handle, ulID, reqItem, out returnValue);
    if (requiredItem != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(requiredItem);
    return returnValue;
  }

  public uint ClientNo
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr clientNo = CSWrap.CSNIDTS_DtsSystem_getClientNo(this.Handle, out returnValue);
      if (clientNo != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(clientNo);
      return returnValue;
    }
  }

  public uint InterfaceNumber
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr interfaceNumber = CSWrap.CSNIDTS_DtsSystem_getInterfaceNumber(this.Handle, out returnValue);
      if (interfaceNumber != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceNumber);
      return returnValue;
    }
  }

  public MCDValue GetSystemParameter(string value)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr systemParameter = CSWrap.CSNIDTS_DtsSystem_getSystemParameter(this.Handle, value, out returnValue);
    if (systemParameter != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(systemParameter);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public bool IsUnsupportedComParametersAccepted
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_isUnsupportedComParametersAccepted(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public void LockServer()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_lockServer(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void UnlockServer()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_unlockServer(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void UnsupportedComParametersAccepted(bool accept)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_unsupportedComParametersAccepted(this.Handle, accept);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void EnableInterface(string shortName, bool bEnable)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_enableInterface(this.Handle, shortName, bEnable);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string FlashDataRoot
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_setFlashDataRoot(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDValue GetProperty(string name)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr property = CSWrap.CSNIDTS_DtsSystem_getProperty(this.Handle, name, out returnValue);
    if (property != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(property);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public void SetProperty(string name, MCDValue value)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_setProperty(this.Handle, name, DTS_ObjectMapper.getHandle(value as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string JavaLocation
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_setJavaLocation(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDInterfaces ConnectedInterfaces
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr connectedInterfaces = CSWrap.CSNIDTS_DtsSystem_getConnectedInterfaces(this.Handle, out returnValue);
      if (connectedInterfaces != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(connectedInterfaces);
      return (MCDInterfaces) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaces);
    }
  }

  public MCDInterfaces CurrentInterfaces
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr currentInterfaces = CSWrap.CSNIDTS_DtsSystem_getCurrentInterfaces(this.Handle, out returnValue);
      if (currentInterfaces != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(currentInterfaces);
      return (MCDInterfaces) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaces);
    }
  }

  public string[] PropertyNames
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr propertyNames = CSWrap.CSNIDTS_DtsSystem_getPropertyNames(this.Handle, out returnValue);
      if (propertyNames != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(propertyNames);
      return returnValue.ToStringArray();
    }
  }

  public void ResetProperty(string name)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_resetProperty(this.Handle, name);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDEnumValue EnumValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr enumValue = CSWrap.CSNIDTS_DtsSystem_getEnumValue(this.Handle, out returnValue);
      if (enumValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(enumValue);
      return (MCDEnumValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsEnumValue);
    }
  }

  public byte[] GetSeed(uint procedureId, DtsAppID appId)
  {
    GC.KeepAlive((object) this);
    ByteField_Struct returnValue = new ByteField_Struct();
    IntPtr seed = CSWrap.CSNIDTS_DtsSystem_getSeed(this.Handle, procedureId, appId, out returnValue);
    if (seed != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(seed);
    return returnValue.ToByteArray();
  }

  public void SendKey(byte[] key)
  {
    GC.KeepAlive((object) this);
    ByteField_Struct _key = new ByteField_Struct(key);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_sendKey(this.Handle, ref _key);
    _key.FreeMemory();
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void DetectInterfaces(string optionString)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_detectInterfaces(this.Handle, optionString);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void PrepareVciAccessLayer()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_prepareVciAccessLayer(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void UnprepareVciAccessLayer()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_unprepareVciAccessLayer(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string ConfigurationPath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr configurationPath = CSWrap.CSNIDTS_DtsSystem_getConfigurationPath(this.Handle, out returnValue);
      if (configurationPath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationPath);
      return returnValue.makeString();
    }
  }

  public void DumpRunningObjects(string outputFile, bool singleObjects)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_dumpRunningObjects(this.Handle, outputFile, singleObjects);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public DtsSystemConfig Configuration
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr configuration = CSWrap.CSNIDTS_DtsSystem_getConfiguration(this.Handle, out returnValue);
      if (configuration != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configuration);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsSystemConfig;
    }
  }

  public MCDDbProject LoadViewerProject(string[] databaseFiles)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    StringArray_Struct _databaseFiles = new StringArray_Struct(databaseFiles);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_loadViewerProject(this.Handle, ref _databaseFiles, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDDbProject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProject);
  }

  public void UnloadViewerProject(MCDDbProject project)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_unloadViewerProject(this.Handle, DTS_ObjectMapper.getHandle(project as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string SessionProjectPath
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_setSessionProjectPath(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void ReloadConfiguration()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_reloadConfiguration(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void DumpMemoryUsage(string outputFile, int flags, bool append)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_dumpMemoryUsage(this.Handle, outputFile, flags, append);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void WriteTraceEntry(string message)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_writeTraceEntry(this.Handle, message);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string DebugTracePath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr debugTracePath = CSWrap.CSNIDTS_DtsSystem_getDebugTracePath(this.Handle, out returnValue);
      if (debugTracePath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(debugTracePath);
      return returnValue.makeString();
    }
  }

  public void StartSnapshotModeTracing(string PhysicalInterfaceLink, uint timeInterval)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_startSnapshotModeTracing(this.Handle, PhysicalInterfaceLink, timeInterval);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void TakeSnapshotByteTrace(string PhysicalInterfaceLink)
  {
    GC.KeepAlive((object) this);
    IntPtr snapshotByteTrace = CSWrap.CSNIDTS_DtsSystem_takeSnapshotByteTrace(this.Handle, PhysicalInterfaceLink);
    if (snapshotByteTrace != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(snapshotByteTrace);
  }

  public void GenerateSnapshotByteTrace(string PhysicalInterfaceLink, string outputPath)
  {
    GC.KeepAlive((object) this);
    IntPtr snapshotByteTrace = CSWrap.CSNIDTS_DtsSystem_generateSnapshotByteTrace(this.Handle, PhysicalInterfaceLink, outputPath);
    if (snapshotByteTrace != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(snapshotByteTrace);
  }

  public void StopSnapshotModeTracing(string PhysicalInterfaceLink)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_stopSnapshotModeTracing(this.Handle, PhysicalInterfaceLink);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void WriteExternTraceEntry(string prefix, string message)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_writeExternTraceEntry(this.Handle, prefix, message);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDProject SelectDynamicProject(string name, string[] files)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_selectDynamicProject(this.Handle, name, ref _files, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDProject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProject);
  }

  public MCDValue GetOptionalProperty(string name)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr optionalProperty = CSWrap.CSNIDTS_DtsSystem_getOptionalProperty(this.Handle, name, out returnValue);
    if (optionalProperty != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(optionalProperty);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public void StartInterfaceDetection()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystem_startInterfaceDetection(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void CreateJVM()
  {
    GC.KeepAlive((object) this);
    IntPtr jvm = CSWrap.CSNIDTS_DtsSystem_createJVM(this.Handle);
    if (jvm != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(jvm);
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

  internal event OnDefinableDynIdListChanged __DefinableDynIdListChanged;

  internal bool _onDefinableDynIdListChanged(MCDValues dynIdList, MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__DefinableDynIdListChanged != null)
      {
        this.__DefinableDynIdListChanged((object) this, new DefinableDynIdListChangedArgs(dynIdList, link));
        return true;
      }
    }
    return false;
  }

  public event OnDefinableDynIdListChanged DefinableDynIdListChanged
  {
    add
    {
      lock (this)
      {
        this.__DefinableDynIdListChanged += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__DefinableDynIdListChanged -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkActivityStateIdle __LinkActivityStateIdle;

  internal bool _onLinkActivityStateIdle(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkActivityStateIdle != null)
      {
        this.__LinkActivityStateIdle((object) this, new LinkActivityStateIdleArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkActivityStateIdle LinkActivityStateIdle
  {
    add
    {
      lock (this)
      {
        this.__LinkActivityStateIdle += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkActivityStateIdle -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkActivityStateRunning __LinkActivityStateRunning;

  internal bool _onLinkActivityStateRunning(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkActivityStateRunning != null)
      {
        this.__LinkActivityStateRunning((object) this, new LinkActivityStateRunningArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkActivityStateRunning LinkActivityStateRunning
  {
    add
    {
      lock (this)
      {
        this.__LinkActivityStateRunning += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkActivityStateRunning -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkActivityStateSuspended __LinkActivityStateSuspended;

  internal bool _onLinkActivityStateSuspended(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkActivityStateSuspended != null)
      {
        this.__LinkActivityStateSuspended((object) this, new LinkActivityStateSuspendedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkActivityStateSuspended LinkActivityStateSuspended
  {
    add
    {
      lock (this)
      {
        this.__LinkActivityStateSuspended += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkActivityStateSuspended -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkError __LinkError;

  internal bool _onLinkError(MCDLogicalLink link, MCDError error)
  {
    lock (this)
    {
      if (this.__LinkError != null)
      {
        this.__LinkError((object) this, new LinkErrorArgs(link, error));
        return true;
      }
    }
    return false;
  }

  public event OnLinkError LinkError
  {
    add
    {
      lock (this)
      {
        this.__LinkError += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkError -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkLocked __LinkLocked;

  internal bool _onLinkLocked(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkLocked != null)
      {
        this.__LinkLocked((object) this, new LinkLockedArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkLocked LinkLocked
  {
    add
    {
      lock (this)
      {
        this.__LinkLocked += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkLocked -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkOneVariantIdentified __LinkOneVariantIdentified;

  internal bool _onLinkOneVariantIdentified(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkOneVariantIdentified != null)
      {
        this.__LinkOneVariantIdentified((object) this, new LinkOneVariantIdentifiedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkOneVariantIdentified LinkOneVariantIdentified
  {
    add
    {
      lock (this)
      {
        this.__LinkOneVariantIdentified += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkOneVariantIdentified -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkOneVariantSelected __LinkOneVariantSelected;

  internal bool _onLinkOneVariantSelected(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkOneVariantSelected != null)
      {
        this.__LinkOneVariantSelected((object) this, new LinkOneVariantSelectedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkOneVariantSelected LinkOneVariantSelected
  {
    add
    {
      lock (this)
      {
        this.__LinkOneVariantSelected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkOneVariantSelected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkQueueCleared __LinkQueueCleared;

  internal bool _onLinkQueueCleared(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkQueueCleared != null)
      {
        this.__LinkQueueCleared((object) this, new LinkQueueClearedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkQueueCleared LinkQueueCleared
  {
    add
    {
      lock (this)
      {
        this.__LinkQueueCleared += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkQueueCleared -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkStateCommunication __LinkStateCommunication;

  internal bool _onLinkStateCommunication(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkStateCommunication != null)
      {
        this.__LinkStateCommunication((object) this, new LinkStateCommunicationArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkStateCommunication LinkStateCommunication
  {
    add
    {
      lock (this)
      {
        this.__LinkStateCommunication += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkStateCommunication -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkStateCreated __LinkStateCreated;

  internal bool _onLinkStateCreated(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkStateCreated != null)
      {
        this.__LinkStateCreated((object) this, new LinkStateCreatedArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkStateCreated LinkStateCreated
  {
    add
    {
      lock (this)
      {
        this.__LinkStateCreated += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkStateCreated -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkStateOffline __LinkStateOffline;

  internal bool _onLinkStateOffline(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkStateOffline != null)
      {
        this.__LinkStateOffline((object) this, new LinkStateOfflineArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkStateOffline LinkStateOffline
  {
    add
    {
      lock (this)
      {
        this.__LinkStateOffline += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkStateOffline -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkStateOnline __LinkStateOnline;

  internal bool _onLinkStateOnline(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkStateOnline != null)
      {
        this.__LinkStateOnline((object) this, new LinkStateOnlineArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkStateOnline LinkStateOnline
  {
    add
    {
      lock (this)
      {
        this.__LinkStateOnline += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkStateOnline -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkUnlocked __LinkUnlocked;

  internal bool _onLinkUnlocked(MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__LinkUnlocked != null)
      {
        this.__LinkUnlocked((object) this, new LinkUnlockedArgs(link));
        return true;
      }
    }
    return false;
  }

  public event OnLinkUnlocked LinkUnlocked
  {
    add
    {
      lock (this)
      {
        this.__LinkUnlocked += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkUnlocked -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkVariantIdentified __LinkVariantIdentified;

  internal bool _onLinkVariantIdentified(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkVariantIdentified != null)
      {
        this.__LinkVariantIdentified((object) this, new LinkVariantIdentifiedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkVariantIdentified LinkVariantIdentified
  {
    add
    {
      lock (this)
      {
        this.__LinkVariantIdentified += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkVariantIdentified -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkVariantSelected __LinkVariantSelected;

  internal bool _onLinkVariantSelected(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkVariantSelected != null)
      {
        this.__LinkVariantSelected((object) this, new LinkVariantSelectedArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkVariantSelected LinkVariantSelected
  {
    add
    {
      lock (this)
      {
        this.__LinkVariantSelected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkVariantSelected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnLinkVariantSet __LinkVariantSet;

  internal bool _onLinkVariantSet(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    lock (this)
    {
      if (this.__LinkVariantSet != null)
      {
        this.__LinkVariantSet((object) this, new LinkVariantSetArgs(link, linkstate));
        return true;
      }
    }
    return false;
  }

  public event OnLinkVariantSet LinkVariantSet
  {
    add
    {
      lock (this)
      {
        this.__LinkVariantSet += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__LinkVariantSet -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveBufferOverflow __PrimitiveBufferOverflow;

  internal bool _onPrimitiveBufferOverflow(MCDDiagComPrimitive primitive, MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__PrimitiveBufferOverflow != null)
      {
        this.__PrimitiveBufferOverflow((object) this, new PrimitiveBufferOverflowArgs(primitive, link));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveBufferOverflow PrimitiveBufferOverflow
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveBufferOverflow += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveBufferOverflow -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveCanceledDuringExecution __PrimitiveCanceledDuringExecution;

  internal bool _onPrimitiveCanceledDuringExecution(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__PrimitiveCanceledDuringExecution != null)
      {
        this.__PrimitiveCanceledDuringExecution((object) this, new PrimitiveCanceledDuringExecutionArgs(primitive, link));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveCanceledDuringExecution PrimitiveCanceledDuringExecution
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveCanceledDuringExecution += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveCanceledDuringExecution -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveCanceledFromQueue __PrimitiveCanceledFromQueue;

  internal bool _onPrimitiveCanceledFromQueue(MCDDiagComPrimitive primitive, MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__PrimitiveCanceledFromQueue != null)
      {
        this.__PrimitiveCanceledFromQueue((object) this, new PrimitiveCanceledFromQueueArgs(primitive, link));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveCanceledFromQueue PrimitiveCanceledFromQueue
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveCanceledFromQueue += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveCanceledFromQueue -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveError __PrimitiveError;

  internal bool _onPrimitiveError(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDError error)
  {
    lock (this)
    {
      if (this.__PrimitiveError != null)
      {
        this.__PrimitiveError((object) this, new PrimitiveErrorArgs(primitive, link, error));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveError PrimitiveError
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveError += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveError -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveHasIntermediateResult __PrimitiveHasIntermediateResult;

  internal bool _onPrimitiveHasIntermediateResult(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDResultState resultstate)
  {
    lock (this)
    {
      if (this.__PrimitiveHasIntermediateResult != null)
      {
        this.__PrimitiveHasIntermediateResult((object) this, new PrimitiveHasIntermediateResultArgs(primitive, link, resultstate));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveHasIntermediateResult PrimitiveHasIntermediateResult
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveHasIntermediateResult += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveHasIntermediateResult -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveHasResult __PrimitiveHasResult;

  internal bool _onPrimitiveHasResult(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDResultState resultstate)
  {
    lock (this)
    {
      if (this.__PrimitiveHasResult != null)
      {
        this.__PrimitiveHasResult((object) this, new PrimitiveHasResultArgs(primitive, link, resultstate));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveHasResult PrimitiveHasResult
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveHasResult += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveHasResult -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveJobInfo __PrimitiveJobInfo;

  internal bool _onPrimitiveJobInfo(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    string info)
  {
    lock (this)
    {
      if (this.__PrimitiveJobInfo != null)
      {
        this.__PrimitiveJobInfo((object) this, new PrimitiveJobInfoArgs(primitive, link, info));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveJobInfo PrimitiveJobInfo
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveJobInfo += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveJobInfo -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveProgressInfo __PrimitiveProgressInfo;

  internal bool _onPrimitiveProgressInfo(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    byte progress)
  {
    lock (this)
    {
      if (this.__PrimitiveProgressInfo != null)
      {
        this.__PrimitiveProgressInfo((object) this, new PrimitiveProgressInfoArgs(primitive, link, progress));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveProgressInfo PrimitiveProgressInfo
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveProgressInfo += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveProgressInfo -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveRepetitionStopped __PrimitiveRepetitionStopped;

  internal bool _onPrimitiveRepetitionStopped(MCDDiagComPrimitive primitive, MCDLogicalLink link)
  {
    lock (this)
    {
      if (this.__PrimitiveRepetitionStopped != null)
      {
        this.__PrimitiveRepetitionStopped((object) this, new PrimitiveRepetitionStoppedArgs(primitive, link));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveRepetitionStopped PrimitiveRepetitionStopped
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveRepetitionStopped += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveRepetitionStopped -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnPrimitiveTerminated __PrimitiveTerminated;

  internal bool _onPrimitiveTerminated(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDResultState resultstate)
  {
    lock (this)
    {
      if (this.__PrimitiveTerminated != null)
      {
        this.__PrimitiveTerminated((object) this, new PrimitiveTerminatedArgs(primitive, link, resultstate));
        return true;
      }
    }
    return false;
  }

  public event OnPrimitiveTerminated PrimitiveTerminated
  {
    add
    {
      lock (this)
      {
        this.__PrimitiveTerminated += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__PrimitiveTerminated -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemClampStateChanged __SystemClampStateChanged;

  internal bool _onSystemClampStateChanged(string clamp, MCDClampState clampState)
  {
    lock (this)
    {
      if (this.__SystemClampStateChanged != null)
      {
        this.__SystemClampStateChanged((object) this, new SystemClampStateChangedArgs(clamp, clampState));
        return true;
      }
    }
    return false;
  }

  public event OnSystemClampStateChanged SystemClampStateChanged
  {
    add
    {
      lock (this)
      {
        this.__SystemClampStateChanged += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemClampStateChanged -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemConfigurationClosed __SystemConfigurationClosed;

  internal bool _onSystemConfigurationClosed()
  {
    lock (this)
    {
      if (this.__SystemConfigurationClosed != null)
      {
        this.__SystemConfigurationClosed((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemConfigurationClosed SystemConfigurationClosed
  {
    add
    {
      lock (this)
      {
        this.__SystemConfigurationClosed += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemConfigurationClosed -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemConfigurationOpened __SystemConfigurationOpened;

  internal bool _onSystemConfigurationOpened()
  {
    lock (this)
    {
      if (this.__SystemConfigurationOpened != null)
      {
        this.__SystemConfigurationOpened((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemConfigurationOpened SystemConfigurationOpened
  {
    add
    {
      lock (this)
      {
        this.__SystemConfigurationOpened += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemConfigurationOpened -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemError __SystemError;

  internal bool _onSystemError(MCDError error)
  {
    lock (this)
    {
      if (this.__SystemError != null)
      {
        this.__SystemError((object) this, new SystemErrorArgs(error));
        return true;
      }
    }
    return false;
  }

  public event OnSystemError SystemError
  {
    add
    {
      lock (this)
      {
        this.__SystemError += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemError -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemLocked __SystemLocked;

  internal bool _onSystemLocked()
  {
    lock (this)
    {
      if (this.__SystemLocked != null)
      {
        this.__SystemLocked((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemLocked SystemLocked
  {
    add
    {
      lock (this)
      {
        this.__SystemLocked += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemLocked -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemLogicallyConnected __SystemLogicallyConnected;

  internal bool _onSystemLogicallyConnected()
  {
    lock (this)
    {
      if (this.__SystemLogicallyConnected != null)
      {
        this.__SystemLogicallyConnected((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemLogicallyConnected SystemLogicallyConnected
  {
    add
    {
      lock (this)
      {
        this.__SystemLogicallyConnected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemLogicallyConnected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemLogicallyDisconnected __SystemLogicallyDisconnected;

  internal bool _onSystemLogicallyDisconnected()
  {
    lock (this)
    {
      if (this.__SystemLogicallyDisconnected != null)
      {
        this.__SystemLogicallyDisconnected((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemLogicallyDisconnected SystemLogicallyDisconnected
  {
    add
    {
      lock (this)
      {
        this.__SystemLogicallyDisconnected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemLogicallyDisconnected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemProjectDeselected __SystemProjectDeselected;

  internal bool _onSystemProjectDeselected()
  {
    lock (this)
    {
      if (this.__SystemProjectDeselected != null)
      {
        this.__SystemProjectDeselected((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemProjectDeselected SystemProjectDeselected
  {
    add
    {
      lock (this)
      {
        this.__SystemProjectDeselected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemProjectDeselected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemProjectSelected __SystemProjectSelected;

  internal bool _onSystemProjectSelected()
  {
    lock (this)
    {
      if (this.__SystemProjectSelected != null)
      {
        this.__SystemProjectSelected((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemProjectSelected SystemProjectSelected
  {
    add
    {
      lock (this)
      {
        this.__SystemProjectSelected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemProjectSelected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemUnlocked __SystemUnlocked;

  internal bool _onSystemUnlocked()
  {
    lock (this)
    {
      if (this.__SystemUnlocked != null)
      {
        this.__SystemUnlocked((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemUnlocked SystemUnlocked
  {
    add
    {
      lock (this)
      {
        this.__SystemUnlocked += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemUnlocked -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemVehicleInfoDeselected __SystemVehicleInfoDeselected;

  internal bool _onSystemVehicleInfoDeselected()
  {
    lock (this)
    {
      if (this.__SystemVehicleInfoDeselected != null)
      {
        this.__SystemVehicleInfoDeselected((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemVehicleInfoDeselected SystemVehicleInfoDeselected
  {
    add
    {
      lock (this)
      {
        this.__SystemVehicleInfoDeselected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemVehicleInfoDeselected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnSystemVehicleInfoSelected __SystemVehicleInfoSelected;

  internal bool _onSystemVehicleInfoSelected()
  {
    lock (this)
    {
      if (this.__SystemVehicleInfoSelected != null)
      {
        this.__SystemVehicleInfoSelected((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnSystemVehicleInfoSelected SystemVehicleInfoSelected
  {
    add
    {
      lock (this)
      {
        this.__SystemVehicleInfoSelected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__SystemVehicleInfoSelected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnConfigurationRecordLoaded __ConfigurationRecordLoaded;

  internal bool _onConfigurationRecordLoaded(MCDConfigurationRecord configurationRecord)
  {
    lock (this)
    {
      if (this.__ConfigurationRecordLoaded != null)
      {
        this.__ConfigurationRecordLoaded((object) this, new ConfigurationRecordLoadedArgs(configurationRecord));
        return true;
      }
    }
    return false;
  }

  public event OnConfigurationRecordLoaded ConfigurationRecordLoaded
  {
    add
    {
      lock (this)
      {
        this.__ConfigurationRecordLoaded += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__ConfigurationRecordLoaded -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnStaticInterfaceError __StaticInterfaceError;

  internal bool _onStaticInterfaceError(MCDError error)
  {
    lock (this)
    {
      if (this.__StaticInterfaceError != null)
      {
        this.__StaticInterfaceError((object) this, new StaticInterfaceErrorArgs(error));
        return true;
      }
    }
    return false;
  }

  public event OnStaticInterfaceError StaticInterfaceError
  {
    add
    {
      lock (this)
      {
        this.__StaticInterfaceError += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__StaticInterfaceError -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnInterfaceStatusChanged __InterfaceStatusChanged;

  internal bool _onInterfaceStatusChanged(MCDInterface interface_, MCDInterfaceStatus status)
  {
    lock (this)
    {
      if (this.__InterfaceStatusChanged != null)
      {
        this.__InterfaceStatusChanged((object) this, new InterfaceStatusChangedArgs(interface_, status));
        return true;
      }
    }
    return false;
  }

  public event OnInterfaceStatusChanged InterfaceStatusChanged
  {
    add
    {
      lock (this)
      {
        this.__InterfaceStatusChanged += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__InterfaceStatusChanged -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnMonitoringFramesReady __MonitoringFramesReady;

  internal bool _onMonitoringFramesReady(MCDMonitoringLink monLink)
  {
    lock (this)
    {
      if (this.__MonitoringFramesReady != null)
      {
        this.__MonitoringFramesReady((object) this, new MonitoringFramesReadyArgs(monLink));
        return true;
      }
    }
    return false;
  }

  public event OnMonitoringFramesReady MonitoringFramesReady
  {
    add
    {
      lock (this)
      {
        this.__MonitoringFramesReady += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__MonitoringFramesReady -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnInterfaceError __InterfaceError;

  internal bool _onInterfaceError(MCDInterface interface_, MCDError error)
  {
    lock (this)
    {
      if (this.__InterfaceError != null)
      {
        this.__InterfaceError((object) this, new InterfaceErrorArgs(interface_, error));
        return true;
      }
    }
    return false;
  }

  public event OnInterfaceError InterfaceError
  {
    add
    {
      lock (this)
      {
        this.__InterfaceError += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__InterfaceError -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnInterfacesModified __InterfacesModified;

  internal bool _onInterfacesModified()
  {
    lock (this)
    {
      if (this.__InterfacesModified != null)
      {
        this.__InterfacesModified((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnInterfacesModified InterfacesModified
  {
    add
    {
      lock (this)
      {
        this.__InterfacesModified += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__InterfacesModified -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnDetectionFinished __DetectionFinished;

  internal bool _onDetectionFinished()
  {
    lock (this)
    {
      if (this.__DetectionFinished != null)
      {
        this.__DetectionFinished((object) this, new EventArgs());
        return true;
      }
    }
    return false;
  }

  public event OnDetectionFinished DetectionFinished
  {
    add
    {
      lock (this)
      {
        this.__DetectionFinished += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__DetectionFinished -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  internal event OnInterfaceDetected __InterfaceDetected;

  internal bool _onInterfaceDetected(MCDInterface interface_)
  {
    lock (this)
    {
      if (this.__InterfaceDetected != null)
      {
        this.__InterfaceDetected((object) this, new InterfaceDetectedArgs(interface_));
        return true;
      }
    }
    return false;
  }

  public event OnInterfaceDetected InterfaceDetected
  {
    add
    {
      lock (this)
      {
        this.__InterfaceDetected += value;
        if (this.handler_count == 0U)
        {
          IntPtr objectHandle = CSWrap.CSNIDTS_setEventListener(this.Handle, out this.listener_handle);
          if (objectHandle != IntPtr.Zero)
            throw DTS_ObjectMapper.createException(objectHandle);
        }
        ++this.handler_count;
      }
    }
    remove
    {
      uint? nullable = new uint?();
      lock (this)
      {
        this.__InterfaceDetected -= value;
        if (this.handler_count > 0U)
        {
          --this.handler_count;
          if (this.handler_count == 0U)
          {
            nullable = new uint?(this.listener_handle);
            this.listener_handle = 0U;
          }
        }
      }
      if (!nullable.HasValue)
        return;
      IntPtr objectHandle = CSWrap.CSNIDTS_releaseEventListener(this.Handle, nullable.Value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }
}
