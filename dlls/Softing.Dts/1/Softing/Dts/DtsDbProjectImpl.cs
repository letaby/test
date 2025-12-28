// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbProjectImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbProjectImpl : 
  MappedObject,
  DtsDbProject,
  MCDDbProject,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbProjectImpl(IntPtr handle)
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

  ~DtsDbProjectImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDAccessKeys AccessKeys
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr accessKeys = CSWrap.CSNIDTS_DtsDbProject_getAccessKeys(this.Handle, out returnValue);
      if (accessKeys != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(accessKeys);
      return (MCDAccessKeys) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKeys);
    }
  }

  public MCDDbEcuBaseVariants DbEcuBaseVariants
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbEcuBaseVariants = CSWrap.CSNIDTS_DtsDbProject_getDbEcuBaseVariants(this.Handle, out returnValue);
      if (dbEcuBaseVariants != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbEcuBaseVariants);
      return (MCDDbEcuBaseVariants) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuBaseVariants);
    }
  }

  public MCDDbFunctionalGroups DbFunctionalGroups
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr functionalGroups = CSWrap.CSNIDTS_DtsDbProject_getDbFunctionalGroups(this.Handle, out returnValue);
      if (functionalGroups != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(functionalGroups);
      return (MCDDbFunctionalGroups) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFunctionalGroups);
    }
  }

  public MCDDbLocations DbProtocolLocations
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr protocolLocations = CSWrap.CSNIDTS_DtsDbProject_getDbProtocolLocations(this.Handle, out returnValue);
      if (protocolLocations != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(protocolLocations);
      return (MCDDbLocations) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLocations);
    }
  }

  public MCDDbVehicleInformations DbVehicleInformations
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr vehicleInformations = CSWrap.CSNIDTS_DtsDbProject_getDbVehicleInformations(this.Handle, out returnValue);
      if (vehicleInformations != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vehicleInformations);
      return (MCDDbVehicleInformations) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbVehicleInformations);
    }
  }

  public MCDVersion Version
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr version = CSWrap.CSNIDTS_DtsDbProject_getVersion(this.Handle, out returnValue);
      if (version != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(version);
      return (MCDVersion) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsVersion);
    }
  }

  public MCDDbObject GetDbElementByAccessKey(MCDAccessKey pAccessKey)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr elementByAccessKey = CSWrap.CSNIDTS_DtsDbProject_getDbElementByAccessKey(this.Handle, DTS_ObjectMapper.getHandle(pAccessKey as MappedObject), out returnValue);
    if (elementByAccessKey != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(elementByAccessKey);
    return (MCDDbObject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbObject);
  }

  public MCDDbPhysicalVehicleLinkOrInterfaces DbPhysicalVehicleLinkOrInterfaces
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr linkOrInterfaces = CSWrap.CSNIDTS_DtsDbProject_getDbPhysicalVehicleLinkOrInterfaces(this.Handle, out returnValue);
      if (linkOrInterfaces != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(linkOrInterfaces);
      return (MCDDbPhysicalVehicleLinkOrInterfaces) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbPhysicalVehicleLinkOrInterfaces);
    }
  }

  public MCDDbEcuMems DbEcuMems
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbEcuMems = CSWrap.CSNIDTS_DtsDbProject_getDbEcuMems(this.Handle, out returnValue);
      if (dbEcuMems != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbEcuMems);
      return (MCDDbEcuMems) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbEcuMems);
    }
  }

  public void LoadNewECUMEM(string ecumemName, bool permanent)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbProject_loadNewECUMEM(this.Handle, ecumemName, permanent);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDDbLocation DbMultipleEcuJobLocation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr multipleEcuJobLocation = CSWrap.CSNIDTS_DtsDbProject_getDbMultipleEcuJobLocation(this.Handle, out returnValue);
      if (multipleEcuJobLocation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(multipleEcuJobLocation);
      return (MCDDbLocation) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLocation);
    }
  }

  public DtsDbODXFiles DbODXFiles
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbOdxFiles = CSWrap.CSNIDTS_DtsDbProject_getDbODXFiles(this.Handle, out returnValue);
      if (dbOdxFiles != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbOdxFiles);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbODXFiles;
    }
  }

  public string RevisionLabel
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr revisionLabel = CSWrap.CSNIDTS_DtsDbProject_getRevisionLabel(this.Handle, out returnValue);
      if (revisionLabel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(revisionLabel);
      return returnValue.makeString();
    }
  }

  public MCDDbFunctionDictionaries DbFunctionDictionaries
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr functionDictionaries = CSWrap.CSNIDTS_DtsDbProject_getDbFunctionDictionaries(this.Handle, out returnValue);
      if (functionDictionaries != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(functionDictionaries);
      return (MCDDbFunctionDictionaries) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFunctionDictionaries);
    }
  }

  public MCDDbConfigurationDatas DbConfigurationDatas
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr configurationDatas = CSWrap.CSNIDTS_DtsDbProject_getDbConfigurationDatas(this.Handle, out returnValue);
      if (configurationDatas != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationDatas);
      return (MCDDbConfigurationDatas) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbConfigurationDatas);
    }
  }

  public MCDDbConfigurationDatas LoadNewConfigurationDatasByFileName(
    string filename,
    bool permanent)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbProject_loadNewConfigurationDatasByFileName(this.Handle, filename, permanent, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return (MCDDbConfigurationDatas) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbConfigurationDatas);
  }

  public DtsProjectType ProjectType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsProjectType returnValue;
      IntPtr projectType = CSWrap.CSNIDTS_DtsDbProject_getProjectType(this.Handle, out returnValue);
      if (projectType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(projectType);
      return returnValue;
    }
  }

  public string VehicleModelRange
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr vehicleModelRange = CSWrap.CSNIDTS_DtsDbProject_getVehicleModelRange(this.Handle, out returnValue);
      if (vehicleModelRange != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vehicleModelRange);
      return returnValue.makeString();
    }
  }

  public DtsIdentifierInfos IdentifierInfos
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr identifierInfos = CSWrap.CSNIDTS_DtsDbProject_getIdentifierInfos(this.Handle, out returnValue);
      if (identifierInfos != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(identifierInfos);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsIdentifierInfos;
    }
  }

  public DtsIdentifierInfos CreateIdentifierInfos()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr identifierInfos = CSWrap.CSNIDTS_DtsDbProject_createIdentifierInfos(this.Handle, out returnValue);
    if (identifierInfos != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(identifierInfos);
    return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsIdentifierInfos;
  }

  public DtsCanFilters CanFilters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr canFilters = CSWrap.CSNIDTS_DtsDbProject_getCanFilters(this.Handle, out returnValue);
      if (canFilters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(canFilters);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsCanFilters;
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
