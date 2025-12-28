// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbCompuMethodImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbCompuMethodImpl : 
  MappedObject,
  DtsDbCompuMethod,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbCompuMethodImpl(IntPtr handle)
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

  ~DtsDbCompuMethodImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string CategoryName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr categoryName = CSWrap.CSNIDTS_DtsDbCompuMethod_getCategoryName(this.Handle, out returnValue);
      if (categoryName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(categoryName);
      return returnValue.makeString();
    }
  }

  public bool IsCompuInternalToPhysValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr physValid = CSWrap.CSNIDTS_DtsDbCompuMethod_isCompuInternalToPhysValid(this.Handle, out returnValue);
      if (physValid != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physValid);
      return returnValue;
    }
  }

  public bool IsCompuPhysToInternalValid
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr internalValid = CSWrap.CSNIDTS_DtsDbCompuMethod_isCompuPhysToInternalValid(this.Handle, out returnValue);
      if (internalValid != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(internalValid);
      return returnValue;
    }
  }

  public DtsDbComputation CompuInternalToPhys
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr compuInternalToPhys = CSWrap.CSNIDTS_DtsDbCompuMethod_getCompuInternalToPhys(this.Handle, out returnValue);
      if (compuInternalToPhys != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compuInternalToPhys);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbComputation;
    }
  }

  public DtsDbComputation CompuPhysToInternal
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr compuPhysToInternal = CSWrap.CSNIDTS_DtsDbCompuMethod_getCompuPhysToInternal(this.Handle, out returnValue);
      if (compuPhysToInternal != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(compuPhysToInternal);
      return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbComputation;
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
