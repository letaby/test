// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbCompuScaleImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbCompuScaleImpl : MappedObject, DtsDbCompuScale, DtsObject, MCDObject, IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbCompuScaleImpl(IntPtr handle)
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

  ~DtsDbCompuScaleImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string ShortLabel
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortLabel = CSWrap.CSNIDTS_DtsDbCompuScale_getShortLabel(this.Handle, out returnValue);
      if (shortLabel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortLabel);
      return returnValue.makeString();
    }
  }

  public bool IsDescriptionValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbCompuScale_isDescriptionValid(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool IsShortLabelValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbCompuScale_isShortLabelValid(this.Handle, out returnValue);
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
      IntPtr description = CSWrap.CSNIDTS_DtsDbCompuScale_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
  }

  public bool IsCompuInverseValueValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbCompuScale_isCompuInverseValueValid(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDValue CompuInverseValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr compuInverseValue = CSWrap.CSNIDTS_DtsDbCompuScale_getCompuInverseValue(this.Handle, out returnValue);
      if (compuInverseValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compuInverseValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public bool IsCompuConstValueValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbCompuScale_isCompuConstValueValid(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDValue CompuConstValue
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr compuConstValue = CSWrap.CSNIDTS_DtsDbCompuScale_getCompuConstValue(this.Handle, out returnValue);
      if (compuConstValue != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compuConstValue);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public bool IsLowerLimitValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbCompuScale_isLowerLimitValid(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public DtsDbLimit LowerLimit
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr lowerLimit = CSWrap.CSNIDTS_DtsDbCompuScale_getLowerLimit(this.Handle, out returnValue);
      if (lowerLimit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(lowerLimit);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLimit;
    }
  }

  public bool IsUpperLimitValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbCompuScale_isUpperLimitValid(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public DtsDbLimit UpperLimit
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr upperLimit = CSWrap.CSNIDTS_DtsDbCompuScale_getUpperLimit(this.Handle, out returnValue);
      if (upperLimit != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(upperLimit);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLimit;
    }
  }

  public uint CompuNumeratorCount
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr compuNumeratorCount = CSWrap.CSNIDTS_DtsDbCompuScale_getCompuNumeratorCount(this.Handle, out returnValue);
      if (compuNumeratorCount != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compuNumeratorCount);
      return returnValue;
    }
  }

  public uint CompuDenominatorCount
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr denominatorCount = CSWrap.CSNIDTS_DtsDbCompuScale_getCompuDenominatorCount(this.Handle, out returnValue);
      if (denominatorCount != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(denominatorCount);
      return returnValue;
    }
  }

  public double GetCompuNumeratorAt(uint idx)
  {
    GC.KeepAlive((object) this);
    double returnValue;
    IntPtr compuNumeratorAt = CSWrap.CSNIDTS_DtsDbCompuScale_getCompuNumeratorAt(this.Handle, idx, out returnValue);
    if (compuNumeratorAt != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(compuNumeratorAt);
    return returnValue;
  }

  public double GetCompuDenominatorAt(uint idx)
  {
    GC.KeepAlive((object) this);
    double returnValue;
    IntPtr compuDenominatorAt = CSWrap.CSNIDTS_DtsDbCompuScale_getCompuDenominatorAt(this.Handle, idx, out returnValue);
    if (compuDenominatorAt != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(compuDenominatorAt);
    return returnValue;
  }

  public string CompuDenominatorsAsString
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr denominatorsAsString = CSWrap.CSNIDTS_DtsDbCompuScale_getCompuDenominatorsAsString(this.Handle, out returnValue);
      if (denominatorsAsString != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(denominatorsAsString);
      return returnValue.makeString();
    }
  }

  public string CompuNumeratorsAsString
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr numeratorsAsString = CSWrap.CSNIDTS_DtsDbCompuScale_getCompuNumeratorsAsString(this.Handle, out returnValue);
      if (numeratorsAsString != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(numeratorsAsString);
      return returnValue.makeString();
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
