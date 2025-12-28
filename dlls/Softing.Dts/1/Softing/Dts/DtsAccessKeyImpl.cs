// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsAccessKeyImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsAccessKeyImpl : 
  MappedObject,
  DtsAccessKey,
  MCDAccessKey,
  MCDObject,
  IDisposable,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsAccessKeyImpl(IntPtr handle)
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

  ~DtsAccessKeyImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDLocationType LocationType
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDLocationType returnValue;
      IntPtr locationType = CSWrap.CSNIDTS_DtsAccessKey_getLocationType(this.Handle, out returnValue);
      if (locationType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(locationType);
      return returnValue;
    }
  }

  public string String
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsAccessKey_getString(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue.makeString();
    }
  }

  public string Protocol
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr protocol = CSWrap.CSNIDTS_DtsAccessKey_getProtocol(this.Handle, out returnValue);
      if (protocol != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(protocol);
      return returnValue.makeString();
    }
  }

  public string FunctionalGroup
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr functionalGroup = CSWrap.CSNIDTS_DtsAccessKey_getFunctionalGroup(this.Handle, out returnValue);
      if (functionalGroup != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(functionalGroup);
      return returnValue.makeString();
    }
  }

  public string EcuBaseVariant
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr ecuBaseVariant = CSWrap.CSNIDTS_DtsAccessKey_getEcuBaseVariant(this.Handle, out returnValue);
      if (ecuBaseVariant != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ecuBaseVariant);
      return returnValue.makeString();
    }
  }

  public string EcuVariant
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr ecuVariant = CSWrap.CSNIDTS_DtsAccessKey_getEcuVariant(this.Handle, out returnValue);
      if (ecuVariant != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ecuVariant);
      return returnValue.makeString();
    }
  }

  public string MultipleEcuJob
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr multipleEcuJob = CSWrap.CSNIDTS_DtsAccessKey_getMultipleEcuJob(this.Handle, out returnValue);
      if (multipleEcuJob != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(multipleEcuJob);
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
