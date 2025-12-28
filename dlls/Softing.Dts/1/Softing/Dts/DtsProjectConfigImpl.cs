// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsProjectConfigImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsProjectConfigImpl : 
  MappedObject,
  DtsProjectConfig,
  DtsNamedObjectConfig,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsProjectConfigImpl(IntPtr handle)
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

  ~DtsProjectConfigImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public DtsVehicleInformationConfigs VehicleInformations
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr vehicleInformations = CSWrap.CSNIDTS_DtsProjectConfig_getVehicleInformations(this.Handle, out returnValue);
      if (vehicleInformations != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vehicleInformations);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVehicleInformationConfigs;
    }
  }

  public string[] GetModularDatabaseFiles(bool bForceReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr modularDatabaseFiles = CSWrap.CSNIDTS_DtsProjectConfig_getModularDatabaseFiles(this.Handle, bForceReload, out returnValue);
    if (modularDatabaseFiles != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(modularDatabaseFiles);
    return returnValue.ToStringArray();
  }

  public void AddModularDatabaseFile(string file, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addModularDatabaseFile(this.Handle, file, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string Database
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr database = CSWrap.CSNIDTS_DtsProjectConfig_getDatabase(this.Handle, out returnValue);
      if (database != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(database);
      return returnValue.makeString();
    }
  }

  public string[] GetCxfDatabaseFiles(bool bForceReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr cxfDatabaseFiles = CSWrap.CSNIDTS_DtsProjectConfig_getCxfDatabaseFiles(this.Handle, bForceReload, out returnValue);
    if (cxfDatabaseFiles != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(cxfDatabaseFiles);
    return returnValue.ToStringArray();
  }

  public void AddCxfDatabaseFile(string file, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addCxfDatabaseFile(this.Handle, file, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string VehicleModelRange
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr vehicleModelRange = CSWrap.CSNIDTS_DtsProjectConfig_getVehicleModelRange(this.Handle, out returnValue);
      if (vehicleModelRange != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vehicleModelRange);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setVehicleModelRange(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void SetDatabaseFile(string databaseFile, bool allowMove, bool overwriteExisting)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setDatabaseFile(this.Handle, databaseFile, allowMove, overwriteExisting);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveCxfDatabaseFiles(string[] files)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeCxfDatabaseFiles(this.Handle, ref _files);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveVariantCodingConfigFiles(string[] files)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeVariantCodingConfigFiles(this.Handle, ref _files);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void AddCANFilterFile(string file, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addCANFilterFile(this.Handle, file, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string[] ExternalFlashFiles
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr externalFlashFiles = CSWrap.CSNIDTS_DtsProjectConfig_getExternalFlashFiles(this.Handle, out returnValue);
      if (externalFlashFiles != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(externalFlashFiles);
      return returnValue.ToStringArray();
    }
  }

  public void RemoveCANFilterFiles(string[] files)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeCANFilterFiles(this.Handle, ref _files);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public bool UseOptimizedDatabase
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr optimizedDatabase = CSWrap.CSNIDTS_DtsProjectConfig_getUseOptimizedDatabase(this.Handle, out returnValue);
      if (optimizedDatabase != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(optimizedDatabase);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setUseOptimizedDatabase(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void SetOptimizedDatabaseFile(string databaseFile, bool allowMove, bool overwriteExisting)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setOptimizedDatabaseFile(this.Handle, databaseFile, allowMove, overwriteExisting);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string OptimizedDatabase
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr optimizedDatabase = CSWrap.CSNIDTS_DtsProjectConfig_getOptimizedDatabase(this.Handle, out returnValue);
      if (optimizedDatabase != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(optimizedDatabase);
      return returnValue.makeString();
    }
  }

  public DtsProjectType ProjectType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsProjectType returnValue;
      IntPtr projectType = CSWrap.CSNIDTS_DtsProjectConfig_getProjectType(this.Handle, out returnValue);
      if (projectType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(projectType);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setProjectType(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool IsDatabaseODX201Legacy
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_isDatabaseODX201Legacy(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool SynchronizeVitWithDatabase
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr synchronizeVitWithDatabase = CSWrap.CSNIDTS_DtsProjectConfig_getSynchronizeVitWithDatabase(this.Handle, out returnValue);
      if (synchronizeVitWithDatabase != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(synchronizeVitWithDatabase);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setSynchronizeVitWithDatabase(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool CreateDefaultVit
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr createDefaultVit = CSWrap.CSNIDTS_DtsProjectConfig_getCreateDefaultVit(this.Handle, out returnValue);
      if (createDefaultVit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(createDefaultVit);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr defaultVit = CSWrap.CSNIDTS_DtsProjectConfig_setCreateDefaultVit(this.Handle, value);
      if (defaultVit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(defaultVit);
    }
  }

  public void AddExternalFlashFile(string file, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addExternalFlashFile(this.Handle, file, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string[] GetCANFilterFiles(bool bForceReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr canFilterFiles = CSWrap.CSNIDTS_DtsProjectConfig_getCANFilterFiles(this.Handle, bForceReload, out returnValue);
    if (canFilterFiles != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(canFilterFiles);
    return returnValue.ToStringArray();
  }

  public void RemoveExternalFlashFiles(string[] files)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeExternalFlashFiles(this.Handle, ref _files);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void UpdateVehicleInformationTables()
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_updateVehicleInformationTables(this.Handle);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void AddOTXProject(string otxProject, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addOTXProject(this.Handle, otxProject, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string[] GetOTXProjects(bool bForceReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr otxProjects = CSWrap.CSNIDTS_DtsProjectConfig_getOTXProjects(this.Handle, bForceReload, out returnValue);
    if (otxProjects != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(otxProjects);
    return returnValue.ToStringArray();
  }

  public void RemoveOTXProjects(string[] otxProjects)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _otxProjects = new StringArray_Struct(otxProjects);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeOTXProjects(this.Handle, ref _otxProjects);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string DCDIFirmware
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr dcdiFirmware = CSWrap.CSNIDTS_DtsProjectConfig_getDCDIFirmware(this.Handle, out returnValue);
      if (dcdiFirmware != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dcdiFirmware);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setDCDIFirmware(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void GenerateOptimizedDatabase(uint encryption)
  {
    GC.KeepAlive((object) this);
    IntPtr optimizedDatabase = CSWrap.CSNIDTS_DtsProjectConfig_generateOptimizedDatabase(this.Handle, encryption);
    if (optimizedDatabase != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(optimizedDatabase);
  }

  public uint PreferredDbEncryption
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr preferredDbEncryption = CSWrap.CSNIDTS_DtsProjectConfig_getPreferredDbEncryption(this.Handle, out returnValue);
      if (preferredDbEncryption != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(preferredDbEncryption);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setPreferredDbEncryption(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public int CheckDatabaseConsistency(bool bRunCheck)
  {
    GC.KeepAlive((object) this);
    int returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_checkDatabaseConsistency(this.Handle, bRunCheck, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public string[] GetVariantCodingConfigFiles(bool bForceReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr codingConfigFiles = CSWrap.CSNIDTS_DtsProjectConfig_getVariantCodingConfigFiles(this.Handle, bForceReload, out returnValue);
    if (codingConfigFiles != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(codingConfigFiles);
    return returnValue.ToStringArray();
  }

  public void AddVariantCodingConfigFile(string file, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addVariantCodingConfigFile(this.Handle, file, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void RemoveModularDatabaseFiles(string[] files)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeModularDatabaseFiles(this.Handle, ref _files);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public bool IsOptimizedDatabaseODX201Legacy
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_isOptimizedDatabaseODX201Legacy(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public int CheckOptimizedDatabaseConsistency()
  {
    GC.KeepAlive((object) this);
    int returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_checkOptimizedDatabaseConsistency(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public bool JavaJob201Legacy
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr javaJob201Legacy = CSWrap.CSNIDTS_DtsProjectConfig_getJavaJob201Legacy(this.Handle, out returnValue);
      if (javaJob201Legacy != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(javaJob201Legacy);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setJavaJob201Legacy(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void AddAdditionalDatabaseFiles(string file, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addAdditionalDatabaseFiles(this.Handle, file, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string[] GetAdditionalDatabaseFiles(bool bForceReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr additionalDatabaseFiles = CSWrap.CSNIDTS_DtsProjectConfig_getAdditionalDatabaseFiles(this.Handle, bForceReload, out returnValue);
    if (additionalDatabaseFiles != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(additionalDatabaseFiles);
    return returnValue.ToStringArray();
  }

  public void RemoveAdditionalDatabaseFiles(string[] files)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeAdditionalDatabaseFiles(this.Handle, ref _files);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public void AddLogicalLinkFilterFile(string file, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addLogicalLinkFilterFile(this.Handle, file, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string[] GetLogicalLinkFilterFiles(bool bForceReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr logicalLinkFilterFiles = CSWrap.CSNIDTS_DtsProjectConfig_getLogicalLinkFilterFiles(this.Handle, bForceReload, out returnValue);
    if (logicalLinkFilterFiles != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(logicalLinkFilterFiles);
    return returnValue.ToStringArray();
  }

  public void RemoveLogicalLinkFilterFiles(string[] files)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeLogicalLinkFilterFiles(this.Handle, ref _files);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public DtsLogicalLinkFilterConfigs LogicalLinkFilters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr logicalLinkFilters = CSWrap.CSNIDTS_DtsProjectConfig_getLogicalLinkFilters(this.Handle, out returnValue);
      if (logicalLinkFilters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalLinkFilters);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsLogicalLinkFilterConfigs;
    }
  }

  public string PreferredOptionSet
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr preferredOptionSet = CSWrap.CSNIDTS_DtsProjectConfig_getPreferredOptionSet(this.Handle, out returnValue);
      if (preferredOptionSet != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(preferredOptionSet);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setPreferredOptionSet(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool IsReferencing
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_isReferencing(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool WriteSimFile
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr writeSimFile = CSWrap.CSNIDTS_DtsProjectConfig_getWriteSimFile(this.Handle, out returnValue);
      if (writeSimFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(writeSimFile);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setWriteSimFile(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool SimFileAppend
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr simFileAppend = CSWrap.CSNIDTS_DtsProjectConfig_getSimFileAppend(this.Handle, out returnValue);
      if (simFileAppend != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(simFileAppend);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setSimFileAppend(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public void AddSimFile(string file, bool allowMove)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_addSimFile(this.Handle, file, allowMove);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string[] GetSimFiles(bool bForceReload)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct returnValue = new StringArray_Struct();
    IntPtr simFiles = CSWrap.CSNIDTS_DtsProjectConfig_getSimFiles(this.Handle, bForceReload, out returnValue);
    if (simFiles != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(simFiles);
    return returnValue.ToStringArray();
  }

  public void RemoveSimFiles(string[] files)
  {
    GC.KeepAlive((object) this);
    StringArray_Struct _files = new StringArray_Struct(files);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_removeSimFiles(this.Handle, ref _files);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public string ActiveSimFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr activeSimFile = CSWrap.CSNIDTS_DtsProjectConfig_getActiveSimFile(this.Handle, out returnValue);
      if (activeSimFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(activeSimFile);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setActiveSimFile(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public uint Characteristic
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr characteristic = CSWrap.CSNIDTS_DtsProjectConfig_getCharacteristic(this.Handle, out returnValue);
      if (characteristic != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(characteristic);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsProjectConfig_setCharacteristic(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
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
