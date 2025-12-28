// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsSystemConfigImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsSystemConfigImpl : MappedObject, DtsSystemConfig, DtsObject, MCDObject, IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsSystemConfigImpl(IntPtr handle)
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

  ~DtsSystemConfigImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string ProjectPath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr projectPath = CSWrap.CSNIDTS_DtsSystemConfig_getProjectPath(this.Handle, out returnValue);
      if (projectPath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(projectPath);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_setProjectPath(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string DatabaseCachesPath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr databaseCachesPath = CSWrap.CSNIDTS_DtsSystemConfig_getDatabaseCachesPath(this.Handle, out returnValue);
      if (databaseCachesPath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(databaseCachesPath);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_setDatabaseCachesPath(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void UpdateLicenseInfo()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_updateLicenseInfo(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public DtsSystemProperties SystemProperties
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr systemProperties = CSWrap.CSNIDTS_DtsSystemConfig_getSystemProperties(this.Handle, out returnValue);
      if (systemProperties != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(systemProperties);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsSystemProperties;
    }
  }

  public string FinasReportPath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr finasReportPath = CSWrap.CSNIDTS_DtsSystemConfig_getFinasReportPath(this.Handle, out returnValue);
      if (finasReportPath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(finasReportPath);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_setFinasReportPath(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Odx201EditorPath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr odx201EditorPath = CSWrap.CSNIDTS_DtsSystemConfig_getOdx201EditorPath(this.Handle, out returnValue);
      if (odx201EditorPath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(odx201EditorPath);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_setOdx201EditorPath(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string[] LicensedProducts
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr licensedProducts = CSWrap.CSNIDTS_DtsSystemConfig_getLicensedProducts(this.Handle, out returnValue);
      if (licensedProducts != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(licensedProducts);
      return returnValue.ToStringArray();
    }
  }

  public void EnableWriteAccess()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_enableWriteAccess(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void ReleaseWriteAccess()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_releaseWriteAccess(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void Save()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_save(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public DtsProjectConfigs ProjectConfigs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr projectConfigs = CSWrap.CSNIDTS_DtsSystemConfig_getProjectConfigs(this.Handle, out returnValue);
      if (projectConfigs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(projectConfigs);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsProjectConfigs;
    }
  }

  public DtsTraceConfig TraceConfig
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr traceConfig = CSWrap.CSNIDTS_DtsSystemConfig_getTraceConfig(this.Handle, out returnValue);
      if (traceConfig != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(traceConfig);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsTraceConfig;
    }
  }

  public DtsJavaConfig JavaConfig
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr javaConfig = CSWrap.CSNIDTS_DtsSystemConfig_getJavaConfig(this.Handle, out returnValue);
      if (javaConfig != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(javaConfig);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsJavaConfig;
    }
  }

  public DtsInterfaceInformations SupportedInterfaces
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr supportedInterfaces = CSWrap.CSNIDTS_DtsSystemConfig_getSupportedInterfaces(this.Handle, out returnValue);
      if (supportedInterfaces != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(supportedInterfaces);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceInformations;
    }
  }

  public DtsInterfaceConfigs InterfaceConfigs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interfaceConfigs = CSWrap.CSNIDTS_DtsSystemConfig_getInterfaceConfigs(this.Handle, out returnValue);
      if (interfaceConfigs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceConfigs);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterfaceConfigs;
    }
  }

  public uint UserInterfaceLanguage
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr interfaceLanguage = CSWrap.CSNIDTS_DtsSystemConfig_getUserInterfaceLanguage(this.Handle, out returnValue);
      if (interfaceLanguage != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interfaceLanguage);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_setUserInterfaceLanguage(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string LicenseFile
  {
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_setLicenseFile(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public DtsLicenseInfos LicenseInfos
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr licenseInfos = CSWrap.CSNIDTS_DtsSystemConfig_getLicenseInfos(this.Handle, out returnValue);
      if (licenseInfos != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(licenseInfos);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLicenseInfos;
    }
  }

  public bool HasChanges
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_hasChanges(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public string RootDescriptionFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr rootDescriptionFile = CSWrap.CSNIDTS_DtsSystemConfig_getRootDescriptionFile(this.Handle, out returnValue);
      if (rootDescriptionFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(rootDescriptionFile);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_setRootDescriptionFile(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string TracePath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr tracePath = CSWrap.CSNIDTS_DtsSystemConfig_getTracePath(this.Handle, out returnValue);
      if (tracePath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(tracePath);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsSystemConfig_setTracePath(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string DefaultConfigPath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr defaultConfigPath = CSWrap.CSNIDTS_DtsSystemConfig_getDefaultConfigPath(this.Handle, out returnValue);
      if (defaultConfigPath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(defaultConfigPath);
      return returnValue.makeString();
    }
  }

  public DtsPduApiInformations SupportedPduApis
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr supportedPduApis = CSWrap.CSNIDTS_DtsSystemConfig_getSupportedPduApis(this.Handle, out returnValue);
      if (supportedPduApis != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(supportedPduApis);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsPduApiInformations;
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
