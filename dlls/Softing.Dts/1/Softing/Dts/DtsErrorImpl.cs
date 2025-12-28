// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsErrorImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsErrorImpl : MappedObject, DtsError, MCDError, MCDObject, IDisposable, DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsErrorImpl(IntPtr handle)
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

  ~DtsErrorImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDSeverity Severity
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDSeverity returnValue;
      IntPtr severity = CSWrap.CSNIDTS_DtsError_getSeverity(this.Handle, out returnValue);
      if (severity != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(severity);
      return returnValue;
    }
  }

  public MCDErrorCodes Code
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDErrorCodes returnValue;
      IntPtr code = CSWrap.CSNIDTS_DtsError_getCode(this.Handle, out returnValue);
      if (code != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(code);
      return returnValue;
    }
  }

  public ushort VendorCode
  {
    get
    {
      GC.KeepAlive((object) this);
      ushort returnValue;
      IntPtr vendorCode = CSWrap.CSNIDTS_DtsError_getVendorCode(this.Handle, out returnValue);
      if (vendorCode != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vendorCode);
      return returnValue;
    }
  }

  public string VendorCodeDescription
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr vendorCodeDescription = CSWrap.CSNIDTS_DtsError_getVendorCodeDescription(this.Handle, out returnValue);
      if (vendorCodeDescription != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vendorCodeDescription);
      return returnValue.makeString();
    }
  }

  public string CodeDescription
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr codeDescription = CSWrap.CSNIDTS_DtsError_getCodeDescription(this.Handle, out returnValue);
      if (codeDescription != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(codeDescription);
      return returnValue.makeString();
    }
  }

  public MCDObject Parent
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr parent = CSWrap.CSNIDTS_DtsError_getParent(this.Handle, out returnValue);
      if (parent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parent);
      return (MCDObject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject);
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
