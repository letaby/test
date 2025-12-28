// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsResultImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsResultImpl : MappedObject, DtsResult, MCDResult, MCDObject, IDisposable, DtsObject
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsResultImpl(IntPtr handle)
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

  ~DtsResultImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public MCDResultType Type
  {
    get
    {
      GC.KeepAlive((object) this);
      MCDResultType returnValue;
      IntPtr type = CSWrap.CSNIDTS_DtsResult_getType(this.Handle, out returnValue);
      if (type != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(type);
      return returnValue;
    }
  }

  public MCDValue RequestMessage
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr requestMessage = CSWrap.CSNIDTS_DtsResult_getRequestMessage(this.Handle, out returnValue);
      if (requestMessage != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(requestMessage);
      return (MCDValue) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsValue);
    }
  }

  public bool HasError
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResult_hasError(this.Handle, out returnValue);
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
      IntPtr error = CSWrap.CSNIDTS_DtsResult_getError(this.Handle, out returnValue);
      if (error != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(error);
      return (MCDError) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsError);
    }
    set
    {
      GC.KeepAlive((object) this);
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResult_setError(this.Handle, DTS_ObjectMapper.getHandle(value as MappedObject));
      if (objectHandle != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(objectHandle);
    }
  }

  public MCDRequestParameters RequestParameters
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr requestParameters = CSWrap.CSNIDTS_DtsResult_getRequestParameters(this.Handle, out returnValue);
      if (requestParameters != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(requestParameters);
      return (MCDRequestParameters) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsRequestParameters);
    }
  }

  public MCDResponses Responses
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr responses = CSWrap.CSNIDTS_DtsResult_getResponses(this.Handle, out returnValue);
      if (responses != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(responses);
      return (MCDResponses) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResponses);
    }
  }

  public string ServiceShortName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr serviceShortName = CSWrap.CSNIDTS_DtsResult_getServiceShortName(this.Handle, out returnValue);
      if (serviceShortName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(serviceShortName);
      return returnValue.makeString();
    }
  }

  public MCDResultState ResultState
  {
    get
    {
      GC.KeepAlive((object) this);
      ObjectInfo_Struct returnValue = new ObjectInfo_Struct();
      IntPtr resultState = CSWrap.CSNIDTS_DtsResult_getResultState(this.Handle, out returnValue);
      if (resultState != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(resultState);
      return (MCDResultState) (DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsResultState);
    }
  }

  public uint RequestTime
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr requestTime = CSWrap.CSNIDTS_DtsResult_getRequestTime(this.Handle, out returnValue);
      if (requestTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(requestTime);
      return returnValue;
    }
  }

  public ulong RequestEndTime
  {
    get
    {
      GC.KeepAlive((object) this);
      ulong returnValue;
      IntPtr requestEndTime = CSWrap.CSNIDTS_DtsResult_getRequestEndTime(this.Handle, out returnValue);
      if (requestEndTime != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(requestEndTime);
      return returnValue;
    }
  }

  public uint CANIdentifier
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr canIdentifier = CSWrap.CSNIDTS_DtsResult_getCANIdentifier(this.Handle, out returnValue);
      if (canIdentifier != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(canIdentifier);
      return returnValue;
    }
  }

  public string ServiceLongName
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr serviceLongName = CSWrap.CSNIDTS_DtsResult_getServiceLongName(this.Handle, out returnValue);
      if (serviceLongName != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(serviceLongName);
      return returnValue.makeString();
    }
  }

  public bool HasRequestMessage
  {
    get
    {
      GC.KeepAlive((object) this);
      bool returnValue;
      IntPtr objectHandle = CSWrap.CSNIDTS_DtsResult_hasRequestMessage(this.Handle, out returnValue);
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
