// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsParameterMetaInfoImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsParameterMetaInfoImpl : 
  MappedObject,
  DtsParameterMetaInfo,
  DtsNamedObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsParameterMetaInfoImpl(IntPtr handle)
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

  ~DtsParameterMetaInfoImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDataType ParameterType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDDataType returnValue;
      IntPtr parameterType = CSWrap.CSNIDTS_DtsParameterMetaInfo_getParameterType(this.Handle, out returnValue);
      if (parameterType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parameterType);
      return returnValue;
    }
  }

  public MCDValue DefaultValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr defaultValue = CSWrap.CSNIDTS_DtsParameterMetaInfo_getDefaultValue(this.Handle, out returnValue);
      if (defaultValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(defaultValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public MCDValue MinValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr minValue = CSWrap.CSNIDTS_DtsParameterMetaInfo_getMinValue(this.Handle, out returnValue);
      if (minValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(minValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public MCDValue MaxValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr maxValue = CSWrap.CSNIDTS_DtsParameterMetaInfo_getMaxValue(this.Handle, out returnValue);
      if (maxValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(maxValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public ushort Radix
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr radix = CSWrap.CSNIDTS_DtsParameterMetaInfo_getRadix(this.Handle, out returnValue);
      if (radix != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(radix);
      return returnValue;
    }
  }

  public ushort DecimalPlaces
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr decimalPlaces = CSWrap.CSNIDTS_DtsParameterMetaInfo_getDecimalPlaces(this.Handle, out returnValue);
      if (decimalPlaces != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(decimalPlaces);
      return returnValue;
    }
  }

  public string Unit
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr unit = CSWrap.CSNIDTS_DtsParameterMetaInfo_getUnit(this.Handle, out returnValue);
      if (unit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(unit);
      return returnValue.makeString();
    }
  }

  public ushort MinLength
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr minLength = CSWrap.CSNIDTS_DtsParameterMetaInfo_getMinLength(this.Handle, out returnValue);
      if (minLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(minLength);
      return returnValue;
    }
  }

  public ushort MaxLength
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr maxLength = CSWrap.CSNIDTS_DtsParameterMetaInfo_getMaxLength(this.Handle, out returnValue);
      if (maxLength != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(maxLength);
      return returnValue;
    }
  }

  public MCDTextTableElements TextTableElements
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr textTableElements = CSWrap.CSNIDTS_DtsParameterMetaInfo_getTextTableElements(this.Handle, out returnValue);
      if (textTableElements != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(textTableElements);
      return (MCDTextTableElements) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsTextTableElements);
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
