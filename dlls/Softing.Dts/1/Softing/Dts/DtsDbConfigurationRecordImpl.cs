// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbConfigurationRecordImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbConfigurationRecordImpl : 
  MappedObject,
  DtsDbConfigurationRecord,
  MCDDbConfigurationRecord,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbConfigurationRecordImpl(IntPtr handle)
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

  ~DtsDbConfigurationRecordImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public uint ByteLength
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr byteLength = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getByteLength(this.Handle, out returnValue);
      if (byteLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(byteLength);
      return returnValue;
    }
  }

  public MCDValue ConfigurationID
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr configurationId = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getConfigurationID(this.Handle, out returnValue);
      if (configurationId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationId);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public MCDDbConfigurationIdItem DbConfigurationIdItem
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr configurationIdItem = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbConfigurationIdItem(this.Handle, out returnValue);
      if (configurationIdItem != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(configurationIdItem);
      return (MCDDbConfigurationIdItem) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbConfigurationIdItem);
    }
  }

  public MCDDbDataIdItem DbDataIdItem
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDataIdItem = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbDataIdItem(this.Handle, out returnValue);
      if (dbDataIdItem != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDataIdItem);
      return (MCDDbDataIdItem) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataIdItem);
    }
  }

  public MCDDbDataRecords DbDataRecords
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDataRecords = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbDataRecords(this.Handle, out returnValue);
      if (dbDataRecords != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDataRecords);
      return (MCDDbDataRecords) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataRecords);
    }
  }

  public MCDDbDataRecord DbDefaultDataRecord
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr defaultDataRecord = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbDefaultDataRecord(this.Handle, out returnValue);
      if (defaultDataRecord != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(defaultDataRecord);
      return (MCDDbDataRecord) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataRecord);
    }
  }

  public MCDDbOptionItems DbOptionItems
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbOptionItems = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbOptionItems(this.Handle, out returnValue);
      if (dbOptionItems != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbOptionItems);
      return (MCDDbOptionItems) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbOptionItems);
    }
  }

  public MCDDbDiagComPrimitives GetDbReadDiagComPrimitives(MCDDbLocation dbLocation)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr diagComPrimitives = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbReadDiagComPrimitives(this.Handle, DTS_ObjectMapper.getHandle(dbLocation as MappedObject), out returnValue);
    if (diagComPrimitives != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(diagComPrimitives);
    return (MCDDbDiagComPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagComPrimitives);
  }

  public MCDDbDiagComPrimitives GetDbWriteDiagComPrimitives(MCDDbLocation dbLocation)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr diagComPrimitives = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbWriteDiagComPrimitives(this.Handle, DTS_ObjectMapper.getHandle(dbLocation as MappedObject), out returnValue);
    if (diagComPrimitives != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(diagComPrimitives);
    return (MCDDbDiagComPrimitives) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagComPrimitives);
  }

  public MCDDbSystemItems DbSystemItems
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSystemItems = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbSystemItems(this.Handle, out returnValue);
      if (dbSystemItems != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSystemItems);
      return (MCDDbSystemItems) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSystemItems);
    }
  }

  public MCDAudience AudienceState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr audienceState = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getAudienceState(this.Handle, out returnValue);
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
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbDisabledAdditionalAudiences(this.Handle, out returnValue);
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
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbEnabledAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
    }
  }

  public MCDDbSpecialDataGroups DbSDGs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSdGs = CSWrap.CSNIDTS_DtsDbConfigurationRecord_getDbSDGs(this.Handle, out returnValue);
      if (dbSdGs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSdGs);
      return (MCDDbSpecialDataGroups) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialDataGroups);
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
