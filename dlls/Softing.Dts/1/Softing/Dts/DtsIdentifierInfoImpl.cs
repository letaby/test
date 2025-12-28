// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsIdentifierInfoImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsIdentifierInfoImpl : 
  MappedObject,
  DtsIdentifierInfo,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsIdentifierInfoImpl(IntPtr handle)
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

  ~DtsIdentifierInfoImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string EcuName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr ecuName = CSWrap.CSNIDTS_DtsIdentifierInfo_getEcuName(this.Handle, out returnValue);
      if (ecuName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ecuName);
      return returnValue.makeString();
    }
  }

  public uint Identifier
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr identifier = CSWrap.CSNIDTS_DtsIdentifierInfo_getIdentifier(this.Handle, out returnValue);
      if (identifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(identifier);
      return returnValue;
    }
  }

  public string MessageName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr messageName = CSWrap.CSNIDTS_DtsIdentifierInfo_getMessageName(this.Handle, out returnValue);
      if (messageName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(messageName);
      return returnValue.makeString();
    }
  }

  public bool IsExtendedIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsIdentifierInfo_isExtendedIdentifier(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public DtsIdentifierType IdentifierType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsIdentifierType returnValue;
      IntPtr identifierType = CSWrap.CSNIDTS_DtsIdentifierInfo_getIdentifierType(this.Handle, out returnValue);
      if (identifierType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(identifierType);
      return returnValue;
    }
  }

  public int ExtendedAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr extendedAddress = CSWrap.CSNIDTS_DtsIdentifierInfo_getExtendedAddress(this.Handle, out returnValue);
      if (extendedAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(extendedAddress);
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
