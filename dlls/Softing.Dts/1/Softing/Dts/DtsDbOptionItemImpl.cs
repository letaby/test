// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbOptionItemImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbOptionItemImpl : 
  MappedObject,
  DtsDbOptionItem,
  MCDDbOptionItem,
  MCDDbConfigurationItem,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbConfigurationItem,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbOptionItemImpl(IntPtr handle)
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

  ~DtsDbOptionItemImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbItemValues DbItemValues
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbItemValues = CSWrap.CSNIDTS_DtsDbOptionItem_getDbItemValues(this.Handle, out returnValue);
      if (dbItemValues != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbItemValues);
      return (MCDDbItemValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbItemValues);
    }
  }

  public ushort DecimalPlaces
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr decimalPlaces = CSWrap.CSNIDTS_DtsDbOptionItem_getDecimalPlaces(this.Handle, out returnValue);
      if (decimalPlaces != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(decimalPlaces);
      return returnValue;
    }
  }

  public MCDValue PhysicalDefaultValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr physicalDefaultValue = CSWrap.CSNIDTS_DtsDbOptionItem_getPhysicalDefaultValue(this.Handle, out returnValue);
      if (physicalDefaultValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalDefaultValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public MCDAudience ReadAudienceState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr readAudienceState = CSWrap.CSNIDTS_DtsDbOptionItem_getReadAudienceState(this.Handle, out returnValue);
      if (readAudienceState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(readAudienceState);
      return (MCDAudience) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAudience);
    }
  }

  public MCDAudience WriteAudienceState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr writeAudienceState = CSWrap.CSNIDTS_DtsDbOptionItem_getWriteAudienceState(this.Handle, out returnValue);
      if (writeAudienceState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(writeAudienceState);
      return (MCDAudience) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAudience);
    }
  }

  public MCDDbAdditionalAudiences DbDisabledReadAdditionalAudiences
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbOptionItem_getDbDisabledReadAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
    }
  }

  public MCDDbAdditionalAudiences DbEnabledReadAdditionalAudiences
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbOptionItem_getDbEnabledReadAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
    }
  }

  public MCDDbAdditionalAudiences DbDisabledWriteAdditionalAudiences
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbOptionItem_getDbDisabledWriteAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
    }
  }

  public MCDDbAdditionalAudiences DbEnabledWriteAdditionalAudiences
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr additionalAudiences = CSWrap.CSNIDTS_DtsDbOptionItem_getDbEnabledWriteAdditionalAudiences(this.Handle, out returnValue);
      if (additionalAudiences != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(additionalAudiences);
      return (MCDDbAdditionalAudiences) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbAdditionalAudiences);
    }
  }

  public uint BitLength
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr bitLength = CSWrap.CSNIDTS_DtsDbConfigurationItem_getBitLength(this.Handle, out returnValue);
      if (bitLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bitLength);
      return returnValue;
    }
  }

  public ushort BitPos
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr bitPos = CSWrap.CSNIDTS_DtsDbConfigurationItem_getBitPos(this.Handle, out returnValue);
      if (bitPos != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bitPos);
      return returnValue;
    }
  }

  public uint BytePos
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr bytePos = CSWrap.CSNIDTS_DtsDbConfigurationItem_getBytePos(this.Handle, out returnValue);
      if (bytePos != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bytePos);
      return returnValue;
    }
  }

  public MCDDataType DataType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDataType returnValue;
      IntPtr dataType = CSWrap.CSNIDTS_DtsDbConfigurationItem_getDataType(this.Handle, out returnValue);
      if (dataType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataType);
      return returnValue;
    }
  }

  public MCDDbUnit DbUnit
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbUnit = CSWrap.CSNIDTS_DtsDbConfigurationItem_getDbUnit(this.Handle, out returnValue);
      if (dbUnit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbUnit);
      return (MCDDbUnit) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbUnit);
    }
  }

  public MCDConstraint InternalConstraint
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr internalConstraint = CSWrap.CSNIDTS_DtsDbConfigurationItem_getInternalConstraint(this.Handle, out returnValue);
      if (internalConstraint != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(internalConstraint);
      return (MCDConstraint) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConstraint);
    }
  }

  public MCDInterval Interval
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interval = CSWrap.CSNIDTS_DtsDbConfigurationItem_getInterval(this.Handle, out returnValue);
      if (interval != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interval);
      return (MCDInterval) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterval);
    }
  }

  public string Semantic
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr semantic = CSWrap.CSNIDTS_DtsDbConfigurationItem_getSemantic(this.Handle, out returnValue);
      if (semantic != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(semantic);
      return returnValue.makeString();
    }
  }

  public MCDTextTableElements TextTableElements
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr textTableElements = CSWrap.CSNIDTS_DtsDbConfigurationItem_getTextTableElements(this.Handle, out returnValue);
      if (textTableElements != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(textTableElements);
      return (MCDTextTableElements) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsTextTableElements);
    }
  }

  public bool IsComplex
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbConfigurationItem_isComplex(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDDbSpecialDataGroups DbSDGs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSdGs = CSWrap.CSNIDTS_DtsDbConfigurationItem_getDbSDGs(this.Handle, out returnValue);
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
