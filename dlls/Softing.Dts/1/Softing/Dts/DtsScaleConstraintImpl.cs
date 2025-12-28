// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsScaleConstraintImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsScaleConstraintImpl : 
  MappedObject,
  DtsScaleConstraint,
  MCDScaleConstraint,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsScaleConstraintImpl(IntPtr handle)
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

  ~DtsScaleConstraintImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string Description
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr description = CSWrap.CSNIDTS_DtsScaleConstraint_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
  }

  public string DescriptionID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr descriptionId = CSWrap.CSNIDTS_DtsScaleConstraint_getDescriptionID(this.Handle, out returnValue);
      if (descriptionId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(descriptionId);
      return returnValue.makeString();
    }
  }

  public MCDInterval Interval
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr interval = CSWrap.CSNIDTS_DtsScaleConstraint_getInterval(this.Handle, out returnValue);
      if (interval != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(interval);
      return (MCDInterval) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsInterval);
    }
  }

  public MCDRangeInfo RangeInfo
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDRangeInfo returnValue;
      IntPtr rangeInfo = CSWrap.CSNIDTS_DtsScaleConstraint_getRangeInfo(this.Handle, out returnValue);
      if (rangeInfo != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(rangeInfo);
      return returnValue;
    }
  }

  public string ShortLabel
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortLabel = CSWrap.CSNIDTS_DtsScaleConstraint_getShortLabel(this.Handle, out returnValue);
      if (shortLabel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortLabel);
      return returnValue.makeString();
    }
  }

  public string ShortLabelID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortLabelId = CSWrap.CSNIDTS_DtsScaleConstraint_getShortLabelID(this.Handle, out returnValue);
      if (shortLabelId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortLabelId);
      return returnValue.makeString();
    }
  }

  public bool IsComputed
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsScaleConstraint_isComputed(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
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
