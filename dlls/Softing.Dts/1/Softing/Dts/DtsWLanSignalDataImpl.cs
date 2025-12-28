// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsWLanSignalDataImpl
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

internal class DtsWLanSignalDataImpl : 
  MappedObject,
  DtsWLanSignalData,
  DtsObject,
  MCDObject,
  IDisposable
{
  protected IntPtr m_dtsHandle = IntPtr.Zero;

  public DtsWLanSignalDataImpl(IntPtr handle)
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

  ~DtsWLanSignalDataImpl() => this.Dispose(false);

  public IntPtr Handle
  {
    get => this.m_dtsHandle;
    set => this.m_dtsHandle = value;
  }

  public DtsWLanType Type
  {
    get
    {
      GC.KeepAlive((object) this);
      DtsWLanType returnValue;
      IntPtr type = CSWrap.CSNIDTS_DtsWLanSignalData_getType(this.Handle, out returnValue);
      if (type != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(type);
      return returnValue;
    }
  }

  public uint Channel
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr channel = CSWrap.CSNIDTS_DtsWLanSignalData_getChannel(this.Handle, out returnValue);
      if (channel != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(channel);
      return returnValue;
    }
  }

  public uint ChannelFreq
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr channelFreq = CSWrap.CSNIDTS_DtsWLanSignalData_getChannelFreq(this.Handle, out returnValue);
      if (channelFreq != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(channelFreq);
      return returnValue;
    }
  }

  public uint ChannelWidth
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr channelWidth = CSWrap.CSNIDTS_DtsWLanSignalData_getChannelWidth(this.Handle, out returnValue);
      if (channelWidth != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(channelWidth);
      return returnValue;
    }
  }

  public float TxPower
  {
    get
    {
      GC.KeepAlive((object) this);
      float returnValue;
      IntPtr txPower = CSWrap.CSNIDTS_DtsWLanSignalData_getTxPower(this.Handle, out returnValue);
      if (txPower != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(txPower);
      return returnValue;
    }
  }

  public uint LinkSpeed
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr linkSpeed = CSWrap.CSNIDTS_DtsWLanSignalData_getLinkSpeed(this.Handle, out returnValue);
      if (linkSpeed != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(linkSpeed);
      return returnValue;
    }
  }

  public int RSSI
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr rssi = CSWrap.CSNIDTS_DtsWLanSignalData_getRSSI(this.Handle, out returnValue);
      if (rssi != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(rssi);
      return returnValue;
    }
  }

  public int SNR
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr snr = CSWrap.CSNIDTS_DtsWLanSignalData_getSNR(this.Handle, out returnValue);
      if (snr != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(snr);
      return returnValue;
    }
  }

  public int Noise
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr noise = CSWrap.CSNIDTS_DtsWLanSignalData_getNoise(this.Handle, out returnValue);
      if (noise != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(noise);
      return returnValue;
    }
  }

  public int SigQuality
  {
    get
    {
      GC.KeepAlive((object) this);
      int returnValue;
      IntPtr sigQuality = CSWrap.CSNIDTS_DtsWLanSignalData_getSigQuality(this.Handle, out returnValue);
      if (sigQuality != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(sigQuality);
      return returnValue;
    }
  }

  public string SSID
  {
    get
    {
      GC.KeepAlive((object) this);
      String_Struct returnValue = new String_Struct();
      IntPtr ssid = CSWrap.CSNIDTS_DtsWLanSignalData_getSSID(this.Handle, out returnValue);
      if (ssid != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(ssid);
      return returnValue.makeString();
    }
  }

  public uint ValidityFlag
  {
    get
    {
      GC.KeepAlive((object) this);
      uint returnValue;
      IntPtr validityFlag = CSWrap.CSNIDTS_DtsWLanSignalData_getValidityFlag(this.Handle, out returnValue);
      if (validityFlag != IntPtr.Zero)
        throw DTS_ObjectMapper.createException(validityFlag);
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
