// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbLocationImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbLocationImpl : 
  MappedObject,
  DtsDbLocation,
  MCDDbLocation,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbLocationImpl(IntPtr handle)
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

  ~DtsDbLocationImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDAccessKey AccessKey
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr accessKey = CSWrap.CSNIDTS_DtsDbLocation_getAccessKey(this.Handle, out returnValue);
      if (accessKey != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(accessKey);
      return (MCDAccessKey) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKey);
    }
  }

  public MCDDbEcu DbECU
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbEcu = CSWrap.CSNIDTS_DtsDbLocation_getDbECU(this.Handle, out returnValue);
      if (dbEcu != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbEcu);
      return (MCDDbEcu) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcu);
    }
  }

  public MCDDbLocation ProtocolLocation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr protocolLocation = CSWrap.CSNIDTS_DtsDbLocation_getProtocolLocation(this.Handle, out returnValue);
      if (protocolLocation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(protocolLocation);
      return (MCDDbLocation) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLocation);
    }
  }

  public MCDLocationType Type
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLocationType returnValue;
      IntPtr type = CSWrap.CSNIDTS_DtsDbLocation_getType(this.Handle, out returnValue);
      if (type != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(type);
      return returnValue;
    }
  }

  public MCDDbServices DbServices
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbServices = CSWrap.CSNIDTS_DtsDbLocation_getDbServices(this.Handle, out returnValue);
      if (dbServices != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbServices);
      return (MCDDbServices) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbServices);
    }
  }

  public MCDDbFunctionalClasses DbFunctionalClasses
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr functionalClasses = CSWrap.CSNIDTS_DtsDbLocation_getDbFunctionalClasses(this.Handle, out returnValue);
      if (functionalClasses != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(functionalClasses);
      return (MCDDbFunctionalClasses) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFunctionalClasses);
    }
  }

  public MCDDbFlashSessionClasses DbFlashSessionClasses
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr flashSessionClasses = CSWrap.CSNIDTS_DtsDbLocation_getDbFlashSessionClasses(this.Handle, out returnValue);
      if (flashSessionClasses != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(flashSessionClasses);
      return (MCDDbFlashSessionClasses) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashSessionClasses);
    }
  }

  public MCDDbFlashSessions DbFlashSessions
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbFlashSessions = CSWrap.CSNIDTS_DtsDbLocation_getDbFlashSessions(this.Handle, out returnValue);
      if (dbFlashSessions != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbFlashSessions);
      return (MCDDbFlashSessions) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashSessions);
    }
  }

  public bool HasDbVariantCodingDomains
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbLocation_hasDbVariantCodingDomains(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public DtsDbVariantCodingDomains DbVariantCodingDomains
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr variantCodingDomains = CSWrap.CSNIDTS_DtsDbLocation_getDbVariantCodingDomains(this.Handle, out returnValue);
      if (variantCodingDomains != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(variantCodingDomains);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbVariantCodingDomains;
    }
  }

  public DtsOfflineVariantCoding CreateOfflineVariantCoding()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr offlineVariantCoding = CSWrap.CSNIDTS_DtsDbLocation_createOfflineVariantCoding(this.Handle, out returnValue);
    if (offlineVariantCoding != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(offlineVariantCoding);
    return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsOfflineVariantCoding;
  }

  public MCDDbPhysicalMemories DbPhysicalMemories
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr physicalMemories = CSWrap.CSNIDTS_DtsDbLocation_getDbPhysicalMemories(this.Handle, out returnValue);
      if (physicalMemories != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalMemories);
      return (MCDDbPhysicalMemories) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbPhysicalMemories);
    }
  }

  public MCDDbControlPrimitives DbControlPrimitives
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr controlPrimitives = CSWrap.CSNIDTS_DtsDbLocation_getDbControlPrimitives(this.Handle, out returnValue);
      if (controlPrimitives != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(controlPrimitives);
      return (MCDDbControlPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbControlPrimitives);
    }
  }

  public MCDDbDataPrimitives DbDataPrimitives
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDataPrimitives = CSWrap.CSNIDTS_DtsDbLocation_getDbDataPrimitives(this.Handle, out returnValue);
      if (dbDataPrimitives != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDataPrimitives);
      return (MCDDbDataPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataPrimitives);
    }
  }

  public MCDDbDiagComPrimitives DbDiagComPrimitives
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr diagComPrimitives = CSWrap.CSNIDTS_DtsDbLocation_getDbDiagComPrimitives(this.Handle, out returnValue);
      if (diagComPrimitives != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(diagComPrimitives);
      return (MCDDbDiagComPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagComPrimitives);
    }
  }

  public MCDDbDiagServices DbDiagServices
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDiagServices = CSWrap.CSNIDTS_DtsDbLocation_getDbDiagServices(this.Handle, out returnValue);
      if (dbDiagServices != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDiagServices);
      return (MCDDbDiagServices) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagServices);
    }
  }

  public DtsDbDiagVariables DbDiagVariables
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDiagVariables = CSWrap.CSNIDTS_DtsDbLocation_getDbDiagVariables(this.Handle, out returnValue);
      if (dbDiagVariables != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDiagVariables);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagVariables;
    }
  }

  public MCDDbDiagTroubleCodes GetDbDTCs(ushort levelLowLimit, ushort levelUpLimit)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr dbDtCs = CSWrap.CSNIDTS_DtsDbLocation_getDbDTCs(this.Handle, levelLowLimit, levelUpLimit, out returnValue);
    if (dbDtCs != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(dbDtCs);
    return (MCDDbDiagTroubleCodes) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagTroubleCodes);
  }

  public string[] DbDynDefinedSpecTables
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr definedSpecTables = CSWrap.CSNIDTS_DtsDbLocation_getDbDynDefinedSpecTables(this.Handle, out returnValue);
      if (definedSpecTables != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(definedSpecTables);
      return returnValue.ToStringArray();
    }
  }

  public MCDDbResponseParameter GetDbDynDefinedSpecTableByName(string shortName)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr definedSpecTableByName = CSWrap.CSNIDTS_DtsDbLocation_getDbDynDefinedSpecTableByName(this.Handle, shortName, out returnValue);
    if (definedSpecTableByName != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(definedSpecTableByName);
    return (MCDDbResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbResponseParameter);
  }

  public string GetDbDynDefinedSpecTableByDefinitionMode(string definitionMode)
  {
    GC.KeepAlive((object) this);
    String_Struct returnValue = new String_Struct();
    IntPtr byDefinitionMode = CSWrap.CSNIDTS_DtsDbLocation_getDbDynDefinedSpecTableByDefinitionMode(this.Handle, definitionMode, out returnValue);
    if (byDefinitionMode != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(byDefinitionMode);
    return returnValue.makeString();
  }

  public MCDDbResponseParameters GetDbEnvDataByTroubleCode(uint troubleCode)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr dataByTroubleCode = CSWrap.CSNIDTS_DtsDbLocation_getDbEnvDataByTroubleCode(this.Handle, troubleCode, out returnValue);
    if (dataByTroubleCode != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(dataByTroubleCode);
    return (MCDDbResponseParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbResponseParameters);
  }

  public MCDDbJobs DbJobs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbJobs = CSWrap.CSNIDTS_DtsDbLocation_getDbJobs(this.Handle, out returnValue);
      if (dbJobs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbJobs);
      return (MCDDbJobs) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbJobs);
    }
  }

  public MCDDbServices GetDbServicesBySemanticAttribute(string semantic)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr semanticAttribute = CSWrap.CSNIDTS_DtsDbLocation_getDbServicesBySemanticAttribute(this.Handle, semantic, out returnValue);
    if (semanticAttribute != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(semanticAttribute);
    return (MCDDbServices) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbServices);
  }

  public MCDValues GetSupportedDynIds(string definitionMode)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr supportedDynIds = CSWrap.CSNIDTS_DtsDbLocation_getSupportedDynIds(this.Handle, definitionMode, out returnValue);
    if (supportedDynIds != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(supportedDynIds);
    return (MCDValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues);
  }

  public MCDDbUnitGroups UnitGroups
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr unitGroups = CSWrap.CSNIDTS_DtsDbLocation_getUnitGroups(this.Handle, out returnValue);
      if (unitGroups != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(unitGroups);
      return (MCDDbUnitGroups) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbUnitGroups);
    }
  }

  public string[] AuthorizationMethods
  {
    get
    {
      GC.KeepAlive((object) this);
      StringArray_Struct returnValue = new StringArray_Struct();
      IntPtr authorizationMethods = CSWrap.CSNIDTS_DtsDbLocation_getAuthorizationMethods(this.Handle, out returnValue);
      if (authorizationMethods != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(authorizationMethods);
      return returnValue.ToStringArray();
    }
  }

  public MCDDbDiagComPrimitives GetDbDiagComPrimitivesByType(MCDObjectType type)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr primitivesByType = CSWrap.CSNIDTS_DtsDbLocation_getDbDiagComPrimitivesByType(this.Handle, type, out returnValue);
    if (primitivesByType != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(primitivesByType);
    return (MCDDbDiagComPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagComPrimitives);
  }

  public bool IsOnboard
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbLocation_isOnboard(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDDbSubComponents DbSubComponents
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSubComponents = CSWrap.CSNIDTS_DtsDbLocation_getDbSubComponents(this.Handle, out returnValue);
      if (dbSubComponents != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSubComponents);
      return (MCDDbSubComponents) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSubComponents);
    }
  }

  public MCDDbConfigurationDatas DbConfigurationDatas
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr configurationDatas = CSWrap.CSNIDTS_DtsDbLocation_getDbConfigurationDatas(this.Handle, out returnValue);
      if (configurationDatas != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationDatas);
      return (MCDDbConfigurationDatas) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbConfigurationDatas);
    }
  }

  public MCDDbTables DbTables
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbTables = CSWrap.CSNIDTS_DtsDbLocation_getDbTables(this.Handle, out returnValue);
      if (dbTables != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbTables);
      return (MCDDbTables) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbTables);
    }
  }

  public MCDDbTables GetDbTablesBySemanticAttribute(string semantic)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr semanticAttribute = CSWrap.CSNIDTS_DtsDbLocation_getDbTablesBySemanticAttribute(this.Handle, semantic, out returnValue);
    if (semanticAttribute != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(semanticAttribute);
    return (MCDDbTables) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbTables);
  }

  public MCDDbFaultMemories DbFaultMemories
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbFaultMemories = CSWrap.CSNIDTS_DtsDbLocation_getDbFaultMemories(this.Handle, out returnValue);
      if (dbFaultMemories != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbFaultMemories);
      return (MCDDbFaultMemories) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFaultMemories);
    }
  }

  public MCDDbUnits DbUnits
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbUnits = CSWrap.CSNIDTS_DtsDbLocation_getDbUnits(this.Handle, out returnValue);
      if (dbUnits != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbUnits);
      return (MCDDbUnits) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbUnits);
    }
  }

  public MCDVersion Version
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr version = CSWrap.CSNIDTS_DtsDbLocation_getVersion(this.Handle, out returnValue);
      if (version != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(version);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public string DataBaseType
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr dataBaseType = CSWrap.CSNIDTS_DtsDbLocation_getDataBaseType(this.Handle, out returnValue);
      if (dataBaseType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataBaseType);
      return returnValue.makeString();
    }
  }

  public MCDDbMatchingPatterns DbVariantPatterns
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbVariantPatterns = CSWrap.CSNIDTS_DtsDbLocation_getDbVariantPatterns(this.Handle, out returnValue);
      if (dbVariantPatterns != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbVariantPatterns);
      return (MCDDbMatchingPatterns) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbMatchingPatterns);
    }
  }

  public MCDDbAdditionalAudiences DbAdditionalAudiences
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbLocation_getDbAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
    }
  }

  public MCDDbEnvDataDescs DbEnvDataDescs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbEnvDataDescs = CSWrap.CSNIDTS_DtsDbLocation_getDbEnvDataDescs(this.Handle, out returnValue);
      if (dbEnvDataDescs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbEnvDataDescs);
      return (MCDDbEnvDataDescs) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEnvDataDescs);
    }
  }

  public MCDDbDiagComPrimitives GetDbDiagComPrimitivesBySemanticAttribute(string sematic)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr semanticAttribute = CSWrap.CSNIDTS_DtsDbLocation_getDbDiagComPrimitivesBySemanticAttribute(this.Handle, sematic, out returnValue);
    if (semanticAttribute != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(semanticAttribute);
    return (MCDDbDiagComPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagComPrimitives);
  }

  public MCDDbEcuStateCharts DbEcuStateCharts
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbEcuStateCharts = CSWrap.CSNIDTS_DtsDbLocation_getDbEcuStateCharts(this.Handle, out returnValue);
      if (dbEcuStateCharts != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbEcuStateCharts);
      return (MCDDbEcuStateCharts) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStateCharts);
    }
  }

  public MCDDbSpecialDataGroups DbSDGs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSdGs = CSWrap.CSNIDTS_DtsDbLocation_getDbSDGs(this.Handle, out returnValue);
      if (dbSdGs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSdGs);
      return (MCDDbSpecialDataGroups) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialDataGroups);
    }
  }

  public MCDDbTable GetDbTableByDefinitionMode(string definitionMode)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr byDefinitionMode = CSWrap.CSNIDTS_DtsDbLocation_getDbTableByDefinitionMode(this.Handle, definitionMode, out returnValue);
    if (byDefinitionMode != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(byDefinitionMode);
    return (MCDDbTable) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbTable);
  }

  public bool IsLinLocation
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbLocation_isLinLocation(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool IsUdsLocation
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbLocation_isUdsLocation(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public uint LogicalAddressValue
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr logicalAddressValue = CSWrap.CSNIDTS_DtsDbLocation_getLogicalAddressValue(this.Handle, out returnValue);
      if (logicalAddressValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalAddressValue);
      return returnValue;
    }
  }

  public string LongNameID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longNameId = CSWrap.CSNIDTS_DtsDbObject_getLongNameID(this.Handle, out returnValue);
      if (longNameId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longNameId);
      return returnValue.makeString();
    }
  }

  public string DescriptionID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr descriptionId = CSWrap.CSNIDTS_DtsDbObject_getDescriptionID(this.Handle, out returnValue);
      if (descriptionId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(descriptionId);
      return returnValue.makeString();
    }
  }

  public string UniqueObjectIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr objectIdentifier = CSWrap.CSNIDTS_DtsDbObject_getUniqueObjectIdentifier(this.Handle, out returnValue);
      if (objectIdentifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectIdentifier);
      return returnValue.makeString();
    }
  }

  public string DatabaseFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr databaseFile = CSWrap.CSNIDTS_DtsDbObject_getDatabaseFile(this.Handle, out returnValue);
      if (databaseFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(databaseFile);
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
