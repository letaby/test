// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbRequestParameterImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbRequestParameterImpl : 
  MappedObject,
  DtsDbRequestParameter,
  MCDDbRequestParameter,
  MCDDbParameter,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbParameter,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbRequestParameterImpl(IntPtr handle)
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

  ~DtsDbRequestParameterImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public uint MaxNumberOfItems
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr maxNumberOfItems = CSWrap.CSNIDTS_DtsDbRequestParameter_getMaxNumberOfItems(this.Handle, out returnValue);
      if (maxNumberOfItems != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(maxNumberOfItems);
      return returnValue;
    }
  }

  public uint BytePos
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr bytePos = CSWrap.CSNIDTS_DtsDbParameter_getBytePos(this.Handle, out returnValue);
      if (bytePos != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bytePos);
      return returnValue;
    }
  }

  public byte BitPos
  {
    get
    {
      GC.KeepAlive((object) this);
      byte returnValue;
      IntPtr bitPos = CSWrap.CSNIDTS_DtsDbParameter_getBitPos(this.Handle, out returnValue);
      if (bitPos != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bitPos);
      return returnValue;
    }
  }

  public uint ByteLength
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr byteLength = CSWrap.CSNIDTS_DtsDbParameter_getByteLength(this.Handle, out returnValue);
      if (byteLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(byteLength);
      return returnValue;
    }
  }

  public ushort DecimalPlaces
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr decimalPlaces = CSWrap.CSNIDTS_DtsDbParameter_getDecimalPlaces(this.Handle, out returnValue);
      if (decimalPlaces != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(decimalPlaces);
      return returnValue;
    }
  }

  public MCDValue DefaultValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr defaultValue = CSWrap.CSNIDTS_DtsDbParameter_getDefaultValue(this.Handle, out returnValue);
      if (defaultValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(defaultValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public ushort MaxLength
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr maxLength = CSWrap.CSNIDTS_DtsDbParameter_getMaxLength(this.Handle, out returnValue);
      if (maxLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(maxLength);
      return returnValue;
    }
  }

  public ushort MinLength
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr minLength = CSWrap.CSNIDTS_DtsDbParameter_getMinLength(this.Handle, out returnValue);
      if (minLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(minLength);
      return returnValue;
    }
  }

  public MCDDataType ParameterType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDataType returnValue;
      IntPtr parameterType = CSWrap.CSNIDTS_DtsDbParameter_getParameterType(this.Handle, out returnValue);
      if (parameterType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parameterType);
      return returnValue;
    }
  }

  public ushort Radix
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr radix = CSWrap.CSNIDTS_DtsDbParameter_getRadix(this.Handle, out returnValue);
      if (radix != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(radix);
      return returnValue;
    }
  }

  public string Unit
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr unit = CSWrap.CSNIDTS_DtsDbParameter_getUnit(this.Handle, out returnValue);
      if (unit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(unit);
      return returnValue.makeString();
    }
  }

  public uint DisplayLevel
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr displayLevel = CSWrap.CSNIDTS_DtsDbParameter_getDisplayLevel(this.Handle, out returnValue);
      if (displayLevel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(displayLevel);
      return returnValue;
    }
  }

  public string Semantic
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr semantic = CSWrap.CSNIDTS_DtsDbParameter_getSemantic(this.Handle, out returnValue);
      if (semantic != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(semantic);
      return returnValue.makeString();
    }
  }

  public MCDInterval Interval
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interval = CSWrap.CSNIDTS_DtsDbParameter_getInterval(this.Handle, out returnValue);
      if (interval != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interval);
      return (MCDInterval) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterval);
    }
  }

  public MCDDbParameters DbParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbParameters = CSWrap.CSNIDTS_DtsDbParameter_getDbParameters(this.Handle, out returnValue);
      if (dbParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbParameters);
      return (MCDDbParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbParameters);
    }
  }

  public DtsParameterMetaInfo ParameterMetaInformation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr parameterMetaInformation = CSWrap.CSNIDTS_DtsDbParameter_getParameterMetaInformation(this.Handle, out returnValue);
      if (parameterMetaInformation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parameterMetaInformation);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsParameterMetaInfo;
    }
  }

  public bool IsConstant
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbParameter_isConstant(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDConstraint InternalConstraint
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr internalConstraint = CSWrap.CSNIDTS_DtsDbParameter_getInternalConstraint(this.Handle, out returnValue);
      if (internalConstraint != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(internalConstraint);
      return (MCDConstraint) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConstraint);
    }
  }

  public string UnitTextID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr unitTextId = CSWrap.CSNIDTS_DtsDbParameter_getUnitTextID(this.Handle, out returnValue);
      if (unitTextId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(unitTextId);
      return returnValue.makeString();
    }
  }

  public uint BitLength
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr bitLength = CSWrap.CSNIDTS_DtsDbParameter_getBitLength(this.Handle, out returnValue);
      if (bitLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bitLength);
      return returnValue;
    }
  }

  public MCDDbTable DbTable
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbTable = CSWrap.CSNIDTS_DtsDbParameter_getDbTable(this.Handle, out returnValue);
      if (dbTable != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbTable);
      return (MCDDbTable) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbTable);
    }
  }

  public MCDDbParameter DbTableKeyParam
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbTableKeyParam = CSWrap.CSNIDTS_DtsDbParameter_getDbTableKeyParam(this.Handle, out returnValue);
      if (dbTableKeyParam != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbTableKeyParam);
      return (MCDDbParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbParameter);
    }
  }

  public MCDDbParameters DbTableStructParams
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr tableStructParams = CSWrap.CSNIDTS_DtsDbParameter_getDbTableStructParams(this.Handle, out returnValue);
      if (tableStructParams != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(tableStructParams);
      return (MCDDbParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbParameters);
    }
  }

  public MCDValues Keys
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr keys = CSWrap.CSNIDTS_DtsDbParameter_getKeys(this.Handle, out returnValue);
      if (keys != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(keys);
      return (MCDValues) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValues);
    }
  }

  public MCDDbParameters GetStructureByKey(MCDValue key)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr structureByKey = CSWrap.CSNIDTS_DtsDbParameter_getStructureByKey(this.Handle, DTS_ObjectMapper.getHandle(key as MappedObject), out returnValue);
    if (structureByKey != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(structureByKey);
    return (MCDDbParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbParameters);
  }

  public MCDTextTableElements TextTableElements
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr textTableElements = CSWrap.CSNIDTS_DtsDbParameter_getTextTableElements(this.Handle, out returnValue);
      if (textTableElements != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(textTableElements);
      return (MCDTextTableElements) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsTextTableElements);
    }
  }

  public MCDParameterType MCDParameterType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDParameterType returnValue;
      IntPtr mcdParameterType = CSWrap.CSNIDTS_DtsDbParameter_getMCDParameterType(this.Handle, out returnValue);
      if (mcdParameterType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(mcdParameterType);
      return returnValue;
    }
  }

  public MCDDataType DataType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDataType returnValue;
      IntPtr dataType = CSWrap.CSNIDTS_DtsDbParameter_getDataType(this.Handle, out returnValue);
      if (dataType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataType);
      return returnValue;
    }
  }

  public MCDValue GetInternalFromPhysicalValue(MCDValue pValue)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr fromPhysicalValue = CSWrap.CSNIDTS_DtsDbParameter_getInternalFromPhysicalValue(this.Handle, DTS_ObjectMapper.getHandle(pValue as MappedObject), out returnValue);
    if (fromPhysicalValue != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(fromPhysicalValue);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public MCDValue GetInternalValueFromPDUFragment(MCDValue pValue)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr valueFromPduFragment = CSWrap.CSNIDTS_DtsDbParameter_getInternalValueFromPDUFragment(this.Handle, DTS_ObjectMapper.getHandle(pValue as MappedObject), out returnValue);
    if (valueFromPduFragment != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(valueFromPduFragment);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public MCDValue GetPDUFragmentFromInternalValue(MCDValue pValue)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr fromInternalValue = CSWrap.CSNIDTS_DtsDbParameter_getPDUFragmentFromInternalValue(this.Handle, DTS_ObjectMapper.getHandle(pValue as MappedObject), out returnValue);
    if (fromInternalValue != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(fromInternalValue);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public MCDValue GetPhysicalFromInternalValue(MCDValue pValue)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr fromInternalValue = CSWrap.CSNIDTS_DtsDbParameter_getPhysicalFromInternalValue(this.Handle, DTS_ObjectMapper.getHandle(pValue as MappedObject), out returnValue);
    if (fromInternalValue != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(fromInternalValue);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public MCDValue CodedDefaultValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr codedDefaultValue = CSWrap.CSNIDTS_DtsDbParameter_getCodedDefaultValue(this.Handle, out returnValue);
      if (codedDefaultValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(codedDefaultValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public MCDIntervals ValidInternalIntervals
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr internalIntervals = CSWrap.CSNIDTS_DtsDbParameter_getValidInternalIntervals(this.Handle, out returnValue);
      if (internalIntervals != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(internalIntervals);
      return (MCDIntervals) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsIntervals);
    }
  }

  public MCDIntervals ValidPhysicalIntervals
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr physicalIntervals = CSWrap.CSNIDTS_DtsDbParameter_getValidPhysicalIntervals(this.Handle, out returnValue);
      if (physicalIntervals != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalIntervals);
      return (MCDIntervals) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsIntervals);
    }
  }

  public MCDConstraint PhysicalConstraint
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr physicalConstraint = CSWrap.CSNIDTS_DtsDbParameter_getPhysicalConstraint(this.Handle, out returnValue);
      if (physicalConstraint != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalConstraint);
      return (MCDConstraint) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsConstraint);
    }
  }

  public MCDDbSpecialDataGroups DbSDGs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbSdGs = CSWrap.CSNIDTS_DtsDbParameter_getDbSDGs(this.Handle, out returnValue);
      if (dbSdGs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbSdGs);
      return (MCDDbSpecialDataGroups) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbSpecialDataGroups);
    }
  }

  public uint ODXBytePos
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr odxBytePos = CSWrap.CSNIDTS_DtsDbParameter_getODXBytePos(this.Handle, out returnValue);
      if (odxBytePos != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(odxBytePos);
      return returnValue;
    }
  }

  public MCDDbUnit DbUnit
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbUnit = CSWrap.CSNIDTS_DtsDbParameter_getDbUnit(this.Handle, out returnValue);
      if (dbUnit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbUnit);
      return (MCDDbUnit) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbUnit);
    }
  }

  public MCDDbParameter LengthKey
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr lengthKey = CSWrap.CSNIDTS_DtsDbParameter_getLengthKey(this.Handle, out returnValue);
      if (lengthKey != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lengthKey);
      return (MCDDbParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbParameter);
    }
  }

  public bool IsVariableLength
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbParameter_isVariableLength(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool HasPhysicalConstraint
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbParameter_hasPhysicalConstraint(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool HasDbUnit
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbParameter_hasDbUnit(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDDataType CodedParameterType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDataType returnValue;
      IntPtr codedParameterType = CSWrap.CSNIDTS_DtsDbParameter_getCodedParameterType(this.Handle, out returnValue);
      if (codedParameterType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(codedParameterType);
      return returnValue;
    }
  }

  public DtsDbDataObjectProp DbDataObjectProp
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDataObjectProp = CSWrap.CSNIDTS_DtsDbParameter_getDbDataObjectProp(this.Handle, out returnValue);
      if (dbDataObjectProp != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDataObjectProp);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDataObjectProp;
    }
  }

  public uint DtsMaxNumberOfItems
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr maxNumberOfItems = CSWrap.CSNIDTS_DtsDbParameter_getDtsMaxNumberOfItems(this.Handle, out returnValue);
      if (maxNumberOfItems != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(maxNumberOfItems);
      return returnValue;
    }
  }

  public string TableRowShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr tableRowShortName = CSWrap.CSNIDTS_DtsDbParameter_getTableRowShortName(this.Handle, out returnValue);
      if (tableRowShortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(tableRowShortName);
      return returnValue.makeString();
    }
  }

  public MCDDbDiagTroubleCodes DbDTCs
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDtCs = CSWrap.CSNIDTS_DtsDbParameter_getDbDTCs(this.Handle, out returnValue);
      if (dbDtCs != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDtCs);
      return (MCDDbDiagTroubleCodes) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagTroubleCodes);
    }
  }

  public bool HasDefaultValue
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbParameter_hasDefaultValue(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool HasInternalConstraint
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbParameter_hasInternalConstraint(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
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
