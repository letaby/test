// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsLogicalLinkConfigImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsLogicalLinkConfigImpl : 
  MappedObject,
  DtsLogicalLinkConfig,
  DtsNamedObjectConfig,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsLogicalLinkConfigImpl(IntPtr handle)
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

  ~DtsLogicalLinkConfigImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public string AccessKey
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr accessKey = CSWrap.CSNIDTS_DtsLogicalLinkConfig_getAccessKey(this.Handle, out returnValue);
      if (accessKey != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(accessKey);
      return returnValue.makeString();
    }
  }

  public string PhysicalVehicleLink
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr physicalVehicleLink = CSWrap.CSNIDTS_DtsLogicalLinkConfig_getPhysicalVehicleLink(this.Handle, out returnValue);
      if (physicalVehicleLink != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalVehicleLink);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkConfig_setPhysicalVehicleLink(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool Manual
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr manual = CSWrap.CSNIDTS_DtsLogicalLinkConfig_getManual(this.Handle, out returnValue);
      if (manual != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(manual);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkConfig_setManual(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public bool IsGateway
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr isGateway = CSWrap.CSNIDTS_DtsLogicalLinkConfig_getIsGateway(this.Handle, out returnValue);
      if (isGateway != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(isGateway);
      return returnValue;
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkConfig_setIsGateway(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Gateway
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr gateway = CSWrap.CSNIDTS_DtsLogicalLinkConfig_getGateway(this.Handle, out returnValue);
      if (gateway != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(gateway);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsLogicalLinkConfig_setGateway(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string ShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr shortName = CSWrap.CSNIDTS_DtsNamedObjectConfig_getShortName(this.Handle, out returnValue);
      if (shortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(shortName);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsNamedObjectConfig_setShortName(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string LongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longName = CSWrap.CSNIDTS_DtsNamedObjectConfig_getLongName(this.Handle, out returnValue);
      if (longName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longName);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsNamedObjectConfig_setLongName(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public string Description
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr description = CSWrap.CSNIDTS_DtsNamedObjectConfig_getDescription(this.Handle, out returnValue);
      if (description != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(description);
      return returnValue.makeString();
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsNamedObjectConfig_setDescription(this.Handle, value);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
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
