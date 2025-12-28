// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbLogicalLinkImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsDbLogicalLinkImpl : 
  MappedObject,
  DtsDbLogicalLink,
  MCDDbLogicalLink,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsDbLogicalLinkImpl(IntPtr handle)
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

  ~DtsDbLogicalLinkImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDDbLocation DbLocation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbLocation = CSWrap.CSNIDTS_DtsDbLogicalLink_getDbLocation(this.Handle, out returnValue);
      if (dbLocation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbLocation);
      return (MCDDbLocation) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLocation);
    }
  }

  public MCDDbLogicalLink GatewayDbLogicalLink
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr gatewayDbLogicalLink = CSWrap.CSNIDTS_DtsDbLogicalLink_getGatewayDbLogicalLink(this.Handle, out returnValue);
      if (gatewayDbLogicalLink != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(gatewayDbLogicalLink);
      return (MCDDbLogicalLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLogicalLink);
    }
  }

  public MCDDbPhysicalVehicleLinkOrInterface DbPhysicalVehicleLinkOrInterface
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr vehicleLinkOrInterface = CSWrap.CSNIDTS_DtsDbLogicalLink_getDbPhysicalVehicleLinkOrInterface(this.Handle, out returnValue);
      if (vehicleLinkOrInterface != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(vehicleLinkOrInterface);
      return (MCDDbPhysicalVehicleLinkOrInterface) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbPhysicalVehicleLinkOrInterface);
    }
  }

  public bool IsGateway
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbLogicalLink_isGateway(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public bool ViaGateway()
  {
    GC.KeepAlive((object) this);
    bool returnValue;
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbLogicalLink_viaGateway(this.Handle, out returnValue);
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
    return returnValue;
  }

  public bool IsAccessedViaGateway
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsDbLogicalLink_isAccessedViaGateway(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDGatewayMode GatewayMode
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDGatewayMode returnValue;
      IntPtr gatewayMode = CSWrap.CSNIDTS_DtsDbLogicalLink_getGatewayMode(this.Handle, out returnValue);
      if (gatewayMode != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(gatewayMode);
      return returnValue;
    }
  }

  public MCDDbLogicalLinks DbLogicalLinksOfGateways
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr logicalLinksOfGateways = CSWrap.CSNIDTS_DtsDbLogicalLink_getDbLogicalLinksOfGateways(this.Handle, out returnValue);
      if (logicalLinksOfGateways != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(logicalLinksOfGateways);
      return (MCDDbLogicalLinks) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbLogicalLinks);
    }
  }

  public uint SourceIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr sourceIdentifier = CSWrap.CSNIDTS_DtsDbLogicalLink_getSourceIdentifier(this.Handle, out returnValue);
      if (sourceIdentifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sourceIdentifier);
      return returnValue;
    }
  }

  public uint TargetIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr targetIdentifier = CSWrap.CSNIDTS_DtsDbLogicalLink_getTargetIdentifier(this.Handle, out returnValue);
      if (targetIdentifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(targetIdentifier);
      return returnValue;
    }
  }

  public DtsPhysicalLinkOrInterfaceType CANType
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsPhysicalLinkOrInterfaceType returnValue;
      IntPtr canType = CSWrap.CSNIDTS_DtsDbLogicalLink_getCANType(this.Handle, out returnValue);
      if (canType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(canType);
      return returnValue;
    }
  }

  public MCDDbRequestParameters CommunicationParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr communicationParameters = CSWrap.CSNIDTS_DtsDbLogicalLink_getCommunicationParameters(this.Handle, out returnValue);
      if (communicationParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(communicationParameters);
      return (MCDDbRequestParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbRequestParameters);
    }
  }

  public MCDDbPhysicalVehicleLink DbPhysicalVehicleLink
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr physicalVehicleLink = CSWrap.CSNIDTS_DtsDbLogicalLink_getDbPhysicalVehicleLink(this.Handle, out returnValue);
      if (physicalVehicleLink != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(physicalVehicleLink);
      return (MCDDbPhysicalVehicleLink) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbPhysicalVehicleLink);
    }
  }

  public string ProtocolType
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr protocolType = CSWrap.CSNIDTS_DtsDbLogicalLink_getProtocolType(this.Handle, out returnValue);
      if (protocolType != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(protocolType);
      return returnValue.makeString();
    }
  }

  public string LongNameID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr longNameId = CSWrap.CSNIDTS_DtsDbObject_getLongNameID(this.Handle, out returnValue);
      if (longNameId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(longNameId);
      return returnValue.makeString();
    }
  }

  public string DescriptionID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr descriptionId = CSWrap.CSNIDTS_DtsDbObject_getDescriptionID(this.Handle, out returnValue);
      if (descriptionId != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(descriptionId);
      return returnValue.makeString();
    }
  }

  public string UniqueObjectIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr objectIdentifier = CSWrap.CSNIDTS_DtsDbObject_getUniqueObjectIdentifier(this.Handle, out returnValue);
      if (objectIdentifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectIdentifier);
      return returnValue.makeString();
    }
  }

  public string DatabaseFile
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr databaseFile = CSWrap.CSNIDTS_DtsDbObject_getDatabaseFile(this.Handle, out returnValue);
      if (databaseFile != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(databaseFile);
      return returnValue.makeString();
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
