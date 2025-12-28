// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsResponseImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsResponseImpl : 
  MappedObject,
  DtsResponse,
  MCDResponse,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsResponseImpl(IntPtr handle)
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

  ~DtsResponseImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDResponseState State
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDResponseState returnValue;
      IntPtr state = CSWrap.CSNIDTS_DtsResponse_getState(this.Handle, out returnValue);
      if (state != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(state);
      return returnValue;
    }
  }

  public bool HasError
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponse_hasError(this.Handle, out returnValue);
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
      return returnValue;
    }
  }

  public MCDError Error
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr error = CSWrap.CSNIDTS_DtsResponse_getError(this.Handle, out returnValue);
      if (error != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(error);
      return (MCDError) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsError);
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponse_setError(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDAccessKey AccessKeyOfLocation
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr accessKeyOfLocation = CSWrap.CSNIDTS_DtsResponse_getAccessKeyOfLocation(this.Handle, out returnValue);
      if (accessKeyOfLocation != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(accessKeyOfLocation);
      return (MCDAccessKey) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsAccessKey);
    }
  }

  public MCDValue ResponseMessage
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr responseMessage = CSWrap.CSNIDTS_DtsResponse_getResponseMessage(this.Handle, out returnValue);
      if (responseMessage != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(responseMessage);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public uint LocationAddress
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr locationAddress = CSWrap.CSNIDTS_DtsResponse_getLocationAddress(this.Handle, out returnValue);
      if (locationAddress != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(locationAddress);
      return returnValue;
    }
  }

  public MCDDbResponse DbObject
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr dbObject = CSWrap.CSNIDTS_DtsResponse_getDbObject(this.Handle, out returnValue);
      if (dbObject != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(dbObject);
      return (MCDDbResponse) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbResponse);
    }
  }

  public MCDResponseParameters ResponseParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr responseParameters = CSWrap.CSNIDTS_DtsResponse_getResponseParameters(this.Handle, out returnValue);
      if (responseParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(responseParameters);
      return (MCDResponseParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponseParameters);
    }
  }

  public uint ResponseTime
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr responseTime = CSWrap.CSNIDTS_DtsResponse_getResponseTime(this.Handle, out returnValue);
      if (responseTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(responseTime);
      return returnValue;
    }
  }

  public MCDObject Parent
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr parent = CSWrap.CSNIDTS_DtsResponse_getParent(this.Handle, out returnValue);
      if (parent != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(parent);
      return (MCDObject) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject);
    }
  }

  public MCDValue ContainedResponseMessage
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr containedResponseMessage = CSWrap.CSNIDTS_DtsResponse_getContainedResponseMessage(this.Handle, out returnValue);
      if (containedResponseMessage != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(containedResponseMessage);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public ulong EndTime
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr endTime = CSWrap.CSNIDTS_DtsResponse_getEndTime(this.Handle, out returnValue);
      if (endTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(endTime);
      return returnValue;
    }
  }

  public ulong StartTime
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr startTime = CSWrap.CSNIDTS_DtsResponse_getStartTime(this.Handle, out returnValue);
      if (startTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(startTime);
      return returnValue;
    }
  }

  public uint CANIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr canIdentifier = CSWrap.CSNIDTS_DtsResponse_getCANIdentifier(this.Handle, out returnValue);
      if (canIdentifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(canIdentifier);
      return returnValue;
    }
  }

  public void EnterPDU(MCDValue pdu)
  {
    GC.KeepAlive((object) this);
    IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponse_enterPDU(this.Handle, DTS_ObjectMapper.getHandle(pdu as MappedObject));
    if (objectHandle != IntPtr.Zero)
      throw DTS_ObjectMapper.createException(objectHandle);
  }

  public bool HasResponseMessage
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResponse_hasResponseMessage(this.Handle, out returnValue);
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
