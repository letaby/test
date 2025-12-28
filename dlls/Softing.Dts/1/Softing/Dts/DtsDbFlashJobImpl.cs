// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbFlashJobImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbFlashJobImpl : 
  MappedObject,
  DtsDbFlashJob,
  MCDDbFlashJob,
  MCDDbJob,
  MCDDbDataPrimitive,
  MCDDbDiagComPrimitive,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbJob,
  DtsDbDataPrimitive,
  DtsDbDiagComPrimitive,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbFlashJobImpl(IntPtr handle)
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

  ~DtsDbFlashJobImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDVersion Version
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr version = CSWrap.CSNIDTS_DtsDbJob_getVersion(this.Handle, out returnValue);
      if (version != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(version);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public string ParentName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr parentName = CSWrap.CSNIDTS_DtsDbJob_getParentName(this.Handle, out returnValue);
      if (parentName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parentName);
      return returnValue.makeString();
    }
  }

  public bool IsReducedResultEnabled
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbJob_isReducedResultEnabled(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public string SourceFilePath
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr sourceFilePath = CSWrap.CSNIDTS_DtsDbJob_getSourceFilePath(this.Handle, out returnValue);
      if (sourceFilePath != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceFilePath);
      return returnValue.makeString();
    }
  }

  public MCDDbCodeInformations DbCodeInformations
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr codeInformations = CSWrap.CSNIDTS_DtsDbJob_getDbCodeInformations(this.Handle, out returnValue);
      if (codeInformations != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(codeInformations);
      return (MCDDbCodeInformations) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbCodeInformations);
    }
  }

  public MCDDbAccessLevel AccessLevel
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr accessLevel = CSWrap.CSNIDTS_DtsDbDataPrimitive_getAccessLevel(this.Handle, out returnValue);
      if (accessLevel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(accessLevel);
      return (MCDDbAccessLevel) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAccessLevel);
    }
  }

  public MCDRepetitionMode RepetitionMode
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDRepetitionMode returnValue;
      IntPtr repetitionMode = CSWrap.CSNIDTS_DtsDbDataPrimitive_getRepetitionMode(this.Handle, out returnValue);
      if (repetitionMode != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(repetitionMode);
      return returnValue;
    }
  }

  public MCDDbSpecialDataGroups DbSDGs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSdGs = CSWrap.CSNIDTS_DtsDbDataPrimitive_getDbSDGs(this.Handle, out returnValue);
      if (dbSdGs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSdGs);
      return (MCDDbSpecialDataGroups) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialDataGroups);
    }
  }

  public MCDDbDataPrimitives GetRelatedDataPrimitives(string relationType)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr relatedDataPrimitives = CSWrap.CSNIDTS_DtsDbDataPrimitive_getRelatedDataPrimitives(this.Handle, relationType, out returnValue);
    if (relatedDataPrimitives != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(relatedDataPrimitives);
    return (MCDDbDataPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataPrimitives);
  }

  public uint RepetitionTime
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr repetitionTime = CSWrap.CSNIDTS_DtsDbDataPrimitive_getRepetitionTime(this.Handle, out returnValue);
      if (repetitionTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(repetitionTime);
      return returnValue;
    }
  }

  public MCDAudience AudienceState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr audienceState = CSWrap.CSNIDTS_DtsDbDataPrimitive_getAudienceState(this.Handle, out returnValue);
      if (audienceState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(audienceState);
      return (MCDAudience) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAudience);
    }
  }

  public MCDDbAdditionalAudiences DbDisabledAdditionalAudiences
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbDataPrimitive_getDbDisabledAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
    }
  }

  public MCDDbAdditionalAudiences DbEnabledAdditionalAudiences
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbDataPrimitive_getDbEnabledAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
    }
  }

  public MCDDbRequest DbRequest
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbRequest = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbRequest(this.Handle, out returnValue);
      if (dbRequest != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbRequest);
      return (MCDDbRequest) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbRequest);
    }
  }

  public MCDDbResponses DbResponses
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbResponses = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbResponses(this.Handle, out returnValue);
      if (dbResponses != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbResponses);
      return (MCDDbResponses) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbResponses);
    }
  }

  public MCDDbFunctionalClasses DbFunctionalClasses
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr functionalClasses = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbFunctionalClasses(this.Handle, out returnValue);
      if (functionalClasses != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(functionalClasses);
      return (MCDDbFunctionalClasses) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFunctionalClasses);
    }
  }

  public string Semantic
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr semantic = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getSemantic(this.Handle, out returnValue);
      if (semantic != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(semantic);
      return returnValue.makeString();
    }
  }

  public MCDTransmissionMode TransmissionMode
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDTransmissionMode returnValue;
      IntPtr transmissionMode = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getTransmissionMode(this.Handle, out returnValue);
      if (transmissionMode != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(transmissionMode);
      return returnValue;
    }
  }

  public bool IsApiExecutable
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_isApiExecutable(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool IsNoOperation
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_isNoOperation(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public DtsComPrimitiveType ComPrimitiveType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsComPrimitiveType returnValue;
      IntPtr comPrimitiveType = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getComPrimitiveType(this.Handle, out returnValue);
      if (comPrimitiveType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(comPrimitiveType);
      return returnValue;
    }
  }

  public MCDValue ID
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr id = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getID(this.Handle, out returnValue);
      if (id != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(id);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public bool SupportsPDUInformation()
  {
    GC.KeepAlive((object) this);
    bool returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_supportsPDUInformation(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public MCDDbResponses GetDbResponsesByType(MCDResponseType type)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr dbResponsesByType = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbResponsesByType(this.Handle, type, out returnValue);
    if (dbResponsesByType != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(dbResponsesByType);
    return (MCDDbResponses) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbResponses);
  }

  public DtsDbProtocolParameters DbProtocolParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr protocolParameters = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbProtocolParameters(this.Handle, out returnValue);
      if (protocolParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(protocolParameters);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProtocolParameters;
    }
  }

  public MCDDbEcuStateTransitions GetDbEcuStateTransitionsByDbObject(MCDDbEcuStateChart chart)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr transitionsByDbObject = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbEcuStateTransitionsByDbObject(this.Handle, DTS_ObjectMapper.getHandle(chart as MappedObject), out returnValue);
    if (transitionsByDbObject != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(transitionsByDbObject);
    return (MCDDbEcuStateTransitions) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStateTransitions);
  }

  public MCDDbEcuStateTransitions GetDbEcuStateTransitionsBySemantic(string semantic)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr transitionsBySemantic = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbEcuStateTransitionsBySemantic(this.Handle, semantic, out returnValue);
    if (transitionsBySemantic != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(transitionsBySemantic);
    return (MCDDbEcuStateTransitions) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStateTransitions);
  }

  public MCDDbEcuStates GetDbPreConditionStatesByDbObject(MCDDbEcuStateChart chart)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr statesByDbObject = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbPreConditionStatesByDbObject(this.Handle, DTS_ObjectMapper.getHandle(chart as MappedObject), out returnValue);
    if (statesByDbObject != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(statesByDbObject);
    return (MCDDbEcuStates) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStates);
  }

  public MCDDbEcuStates GetDbPreConditionStatesBySemantic(string semantic)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr statesBySemantic = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDbPreConditionStatesBySemantic(this.Handle, semantic, out returnValue);
    if (statesBySemantic != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(statesBySemantic);
    return (MCDDbEcuStates) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuStates);
  }

  public MCDValue DID
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr did = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getDID(this.Handle, out returnValue);
      if (did != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(did);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public bool HasDID
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_hasDID(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public string InternalShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr internalShortName = CSWrap.CSNIDTS_DtsDbDiagComPrimitive_getInternalShortName(this.Handle, out returnValue);
      if (internalShortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(internalShortName);
      return returnValue.makeString();
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
