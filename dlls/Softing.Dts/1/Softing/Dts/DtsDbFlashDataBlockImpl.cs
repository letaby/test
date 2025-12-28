// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbFlashDataBlockImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbFlashDataBlockImpl : 
  MappedObject,
  DtsDbFlashDataBlock,
  MCDDbFlashDataBlock,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbFlashDataBlockImpl(IntPtr handle)
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

  ~DtsDbFlashDataBlockImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbFlashSegments DbFlashSegments
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbFlashSegments = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbFlashSegments(this.Handle, out returnValue);
      if (dbFlashSegments != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbFlashSegments);
      return (MCDDbFlashSegments) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashSegments);
    }
  }

  public MCDDbFlashData DbFlashData
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbFlashData = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbFlashData(this.Handle, out returnValue);
      if (dbFlashData != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbFlashData);
      return (MCDDbFlashData) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashData);
    }
  }

  public MCDDbFlashIdents DbOwnIdents
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbOwnIdents = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbOwnIdents(this.Handle, out returnValue);
      if (dbOwnIdents != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbOwnIdents);
      return (MCDDbFlashIdents) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashIdents);
    }
  }

  public MCDDbFlashFilters DbFlashFilters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbFlashFilters = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbFlashFilters(this.Handle, out returnValue);
      if (dbFlashFilters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbFlashFilters);
      return (MCDDbFlashFilters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashFilters);
    }
  }

  public long AddressOffset
  {
    get
    {
      GC.KeepAlive((object) this);
      long returnValue;
      IntPtr addressOffset = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getAddressOffset(this.Handle, out returnValue);
      if (addressOffset != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(addressOffset);
      return returnValue;
    }
  }

  public string DataBlockType
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr dataBlockType = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDataBlockType(this.Handle, out returnValue);
      if (dataBlockType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataBlockType);
      return returnValue.makeString();
    }
  }

  public MCDDbFlashSecurities DbSecurities
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSecurities = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbSecurities(this.Handle, out returnValue);
      if (dbSecurities != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSecurities);
      return (MCDDbFlashSecurities) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashSecurities);
    }
  }

  public MCDDbFlashSecurities DbSecuritiesAsSecurities
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr securitiesAsSecurities = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbSecuritiesAsSecurities(this.Handle, out returnValue);
      if (securitiesAsSecurities != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(securitiesAsSecurities);
      return (MCDDbFlashSecurities) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbFlashSecurities);
    }
  }

  public MCDDbSpecialDataGroups DbSDGs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSdGs = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbSDGs(this.Handle, out returnValue);
      if (dbSdGs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSdGs);
      return (MCDDbSpecialDataGroups) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialDataGroups);
    }
  }

  public void LoadSegments(string filename)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbFlashDataBlock_loadSegments(this.Handle, filename);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDAudience AudienceState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr audienceState = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getAudienceState(this.Handle, out returnValue);
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
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbDisabledAdditionalAudiences(this.Handle, out returnValue);
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
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbFlashDataBlock_getDbEnabledAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
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
