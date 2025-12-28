// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsOfflineVariantCodingImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsOfflineVariantCodingImpl : 
  MappedObject,
  DtsOfflineVariantCoding,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsOfflineVariantCodingImpl(IntPtr handle)
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

  ~DtsOfflineVariantCodingImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDValue GetCodingString(string domain)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr codingString = CSWrap.CSNIDTS_DtsOfflineVariantCoding_getCodingString(this.Handle, domain, out returnValue);
    if (codingString != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(codingString);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public void SetCodingString(string domain, MCDValue codingString)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsOfflineVariantCoding_setCodingString(this.Handle, domain, DTS_ObjectMapper.getHandle(codingString as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDValue GetInternalMeaning(string domain, string fragment)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr internalMeaning = CSWrap.CSNIDTS_DtsOfflineVariantCoding_getInternalMeaning(this.Handle, domain, fragment, out returnValue);
    if (internalMeaning != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(internalMeaning);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public void SetInternalMeaning(string domain, string fragment, MCDValue meaning)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsOfflineVariantCoding_setInternalMeaning(this.Handle, domain, fragment, DTS_ObjectMapper.getHandle(meaning as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public MCDValue GetExternalMeaning(string domain, string fragment)
  {
    GC.KeepAlive((object) this);
    ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
    IntPtr externalMeaning = CSWrap.CSNIDTS_DtsOfflineVariantCoding_getExternalMeaning(this.Handle, domain, fragment, out returnValue);
    if (externalMeaning != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(externalMeaning);
    return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
  }

  public void SetExternalMeaning(string domain, string fragment, MCDValue meaning)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsOfflineVariantCoding_setExternalMeaning(this.Handle, domain, fragment, DTS_ObjectMapper.getHandle(meaning as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
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
