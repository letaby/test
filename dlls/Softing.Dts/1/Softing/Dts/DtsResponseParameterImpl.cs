// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsResponseParameterImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsResponseParameterImpl : 
  MappedObject,
  DtsResponseParameter,
  MCDResponseParameter,
  MCDParameter,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsParameter,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsResponseParameterImpl(IntPtr handle)
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

  ~DtsResponseParameterImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public bool HasError
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameter_hasError(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDError Error
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr error = CSWrap.CSNIDTS_DtsResponseParameter_getError(this.Handle, out returnValue);
      if (error != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(error);
      return (MCDError) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsError);
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameter_setError(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDResponseParameters ResponseParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr responseParameters = CSWrap.CSNIDTS_DtsResponseParameter_getResponseParameters(this.Handle, out returnValue);
      if (responseParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(responseParameters);
      return (MCDResponseParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameters);
    }
  }

  public MCDResponseParameters Parameters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr parameters = CSWrap.CSNIDTS_DtsResponseParameter_getParameters(this.Handle, out returnValue);
      if (parameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parameters);
      return (MCDResponseParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameters);
    }
  }

  public MCDObject Parent
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr parent = CSWrap.CSNIDTS_DtsResponseParameter_getParent(this.Handle, out returnValue);
      if (parent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parent);
      return (MCDObject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject);
    }
  }

  public MCDDbDiagTroubleCode DbDTC
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbDtc = CSWrap.CSNIDTS_DtsResponseParameter_getDbDTC(this.Handle, out returnValue);
      if (dbDtc != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbDtc);
      return (MCDDbDiagTroubleCode) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbDiagTroubleCode);
    }
  }

  public bool IsVariableLength
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponseParameter_isVariableLength(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDResponseParameter LengthKey
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr lengthKey = CSWrap.CSNIDTS_DtsResponseParameter_getLengthKey(this.Handle, out returnValue);
      if (lengthKey != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lengthKey);
      return (MCDResponseParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameter);
    }
  }

  public ushort DecimalPlaces
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr decimalPlaces = CSWrap.CSNIDTS_DtsParameter_getDecimalPlaces(this.Handle, out returnValue);
      if (decimalPlaces != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(decimalPlaces);
      return returnValue;
    }
  }

  public MCDValue Value
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsParameter_getValue(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsParameter_setValue(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Unit
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr unit = CSWrap.CSNIDTS_DtsParameter_getUnit(this.Handle, out returnValue);
      if (unit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(unit);
      return returnValue.makeString();
    }
  }

  public ushort Radix
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr radix = CSWrap.CSNIDTS_DtsParameter_getRadix(this.Handle, out returnValue);
      if (radix != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(radix);
      return returnValue;
    }
  }

  public MCDRangeInfo ValueRangeInfo
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDRangeInfo returnValue;
      IntPtr valueRangeInfo = CSWrap.CSNIDTS_DtsParameter_getValueRangeInfo(this.Handle, out returnValue);
      if (valueRangeInfo != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(valueRangeInfo);
      return returnValue;
    }
  }

  public MCDDataType Type
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDataType returnValue;
      IntPtr type = CSWrap.CSNIDTS_DtsParameter_getType(this.Handle, out returnValue);
      if (type != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(type);
      return returnValue;
    }
  }

  public MCDDbParameter DbObject
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbObject = CSWrap.CSNIDTS_DtsParameter_getDbObject(this.Handle, out returnValue);
      if (dbObject != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbObject);
      return (MCDDbParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbParameter);
    }
  }

  public MCDDataType CodedType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDataType returnValue;
      IntPtr codedType = CSWrap.CSNIDTS_DtsParameter_getCodedType(this.Handle, out returnValue);
      if (codedType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(codedType);
      return returnValue;
    }
  }

  public MCDValue CodedValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr codedValue = CSWrap.CSNIDTS_DtsParameter_getCodedValue(this.Handle, out returnValue);
      if (codedValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(codedValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsParameter_setCodedValue(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string ValueTextID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr valueTextId = CSWrap.CSNIDTS_DtsParameter_getValueTextID(this.Handle, out returnValue);
      if (valueTextId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(valueTextId);
      return returnValue.makeString();
    }
  }

  public string LongNameID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longNameId = CSWrap.CSNIDTS_DtsParameter_getLongNameID(this.Handle, out returnValue);
      if (longNameId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longNameId);
      return returnValue.makeString();
    }
  }

  public string UnitTextID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr unitTextId = CSWrap.CSNIDTS_DtsParameter_getUnitTextID(this.Handle, out returnValue);
      if (unitTextId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(unitTextId);
      return returnValue.makeString();
    }
  }

  public MCDParameterType MCDParameterType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDParameterType returnValue;
      IntPtr mcdParameterType = CSWrap.CSNIDTS_DtsParameter_getMCDParameterType(this.Handle, out returnValue);
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
      IntPtr dataType = CSWrap.CSNIDTS_DtsParameter_getDataType(this.Handle, out returnValue);
      if (dataType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dataType);
      return returnValue;
    }
  }

  public MCDScaleConstraint PhysicalScaleConstraint
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr physicalScaleConstraint = CSWrap.CSNIDTS_DtsParameter_getPhysicalScaleConstraint(this.Handle, out returnValue);
      if (physicalScaleConstraint != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalScaleConstraint);
      return (MCDScaleConstraint) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsScaleConstraint);
    }
  }

  public MCDScaleConstraint InternalScaleConstraint
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr internalScaleConstraint = CSWrap.CSNIDTS_DtsParameter_getInternalScaleConstraint(this.Handle, out returnValue);
      if (internalScaleConstraint != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(internalScaleConstraint);
      return (MCDScaleConstraint) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsScaleConstraint);
    }
  }

  public byte BitPos
  {
    get
    {
      GC.KeepAlive((object) this);
      byte returnValue;
      IntPtr bitPos = CSWrap.CSNIDTS_DtsParameter_getBitPos(this.Handle, out returnValue);
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
      IntPtr byteLength = CSWrap.CSNIDTS_DtsParameter_getByteLength(this.Handle, out returnValue);
      if (byteLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(byteLength);
      return returnValue;
    }
  }

  public uint BytePos
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr bytePos = CSWrap.CSNIDTS_DtsParameter_getBytePos(this.Handle, out returnValue);
      if (bytePos != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(bytePos);
      return returnValue;
    }
  }

  public MCDValue CreateDtsValue()
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr dtsValue = CSWrap.CSNIDTS_DtsParameter_createDtsValue(this.Handle, out returnValue);
    if (dtsValue != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(dtsValue);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public void AddDtsParameters(uint count)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsParameter_addDtsParameters(this.Handle, count);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDParameter DtsLengthKey
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dtsLengthKey = CSWrap.CSNIDTS_DtsParameter_getDtsLengthKey(this.Handle, out returnValue);
      if (dtsLengthKey != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dtsLengthKey);
      return (MCDParameter) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsParameter);
    }
  }

  public bool IsDtsVariableLength
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsParameter_isDtsVariableLength(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDNamedCollection DtsParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dtsParameters = CSWrap.CSNIDTS_DtsParameter_getDtsParameters(this.Handle, out returnValue);
      if (dtsParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dtsParameters);
      return (MCDNamedCollection) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsNamedCollection);
    }
  }

  public string Semantic
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr semantic = CSWrap.CSNIDTS_DtsParameter_getSemantic(this.Handle, out returnValue);
      if (semantic != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(semantic);
      return returnValue.makeString();
    }
  }

  public bool HasValue
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsParameter_hasValue(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
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
