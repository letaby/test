// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbDataRecordImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbDataRecordImpl : 
  MappedObject,
  DtsDbDataRecord,
  MCDDbDataRecord,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbDataRecordImpl(IntPtr handle)
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

  ~DtsDbDataRecordImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public byte[] BinaryData
  {
    get
    {
      GC.KeepAlive((object) this);
      ByteField_Struct returnValue = new ByteField_Struct();
      IntPtr binaryData = CSWrap.CSNIDTS_DtsDbDataRecord_getBinaryData(this.Handle, out returnValue);
      if (binaryData != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(binaryData);
      return returnValue.ToByteArray();
    }
  }

  public MCDFlashDataFormat DataFormat
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDFlashDataFormat returnValue;
      IntPtr dataFormat = CSWrap.CSNIDTS_DtsDbDataRecord_getDataFormat(this.Handle, out returnValue);
      if (dataFormat != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataFormat);
      return returnValue;
    }
  }

  public MCDValue DataID
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dataId = CSWrap.CSNIDTS_DtsDbDataRecord_getDataID(this.Handle, out returnValue);
      if (dataId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataId);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public MCDDbCodingData DbCodingData
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbCodingData = CSWrap.CSNIDTS_DtsDbDataRecord_getDbCodingData(this.Handle, out returnValue);
      if (dbCodingData != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbCodingData);
      return (MCDDbCodingData) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbCodingData);
    }
  }

  public string Key
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr key = CSWrap.CSNIDTS_DtsDbDataRecord_getKey(this.Handle, out returnValue);
      if (key != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(key);
      return returnValue.makeString();
    }
  }

  public string Rule
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr rule = CSWrap.CSNIDTS_DtsDbDataRecord_getRule(this.Handle, out returnValue);
      if (rule != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(rule);
      return returnValue.makeString();
    }
  }

  public string UserDefinedFormat
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr userDefinedFormat = CSWrap.CSNIDTS_DtsDbDataRecord_getUserDefinedFormat(this.Handle, out returnValue);
      if (userDefinedFormat != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(userDefinedFormat);
      return returnValue.makeString();
    }
  }

  public MCDAudience AudienceState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr audienceState = CSWrap.CSNIDTS_DtsDbDataRecord_getAudienceState(this.Handle, out returnValue);
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
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbDataRecord_getDbDisabledAdditionalAudiences(this.Handle, out returnValue);
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
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbDataRecord_getDbEnabledAdditionalAudiences(this.Handle, out returnValue);
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
      IntPtr dbSdGs = CSWrap.CSNIDTS_DtsDbDataRecord_getDbSDGs(this.Handle, out returnValue);
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
