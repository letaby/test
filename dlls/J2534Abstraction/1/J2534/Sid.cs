// Decompiled with JetBrains decompiler
// Type: J2534.Sid
// Assembly: J2534Abstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: F558D3F4-6D07-4AE0-B148-E7AD8371AFDC
// Assembly location: C:\Users\petra\Downloads\Архив (2)\J2534Abstraction.dll

using \u003CCppImplementationDetails\u003E;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable disable
namespace J2534;

public sealed class Sid
{
  internal static Dictionary<uint, J2534.PassthruCallback> PassthruCallback = new Dictionary<uint, J2534.PassthruCallback>();

  public static J2534Error Open() => (J2534Error) \u003CModule\u003E.PassThruOpen();

  public static J2534Error Close() => (J2534Error) \u003CModule\u003E.PassThruClose();

  public static J2534Error Disconnect(uint channelId)
  {
    return (J2534Error) \u003CModule\u003E.PassThruDisconnect(channelId);
  }

  public static unsafe J2534Error Connect(ProtocolId id, uint options, ref uint channelId)
  {
    uint num1 = 0;
    int num2 = \u003CModule\u003E.PassThruConnect((uint) id, options, &num1);
    channelId = num1;
    return (J2534Error) num2;
  }

  public static unsafe J2534Error StartMsgFilter(
    uint channelId,
    FilterType type,
    PassThruMsg mask,
    PassThruMsg pattern,
    PassThruMsg flow,
    ref uint filterId)
  {
    PASSTHRU_MSG* passthruMsgPtr1 = (PASSTHRU_MSG*) 0L;
    PASSTHRU_MSG* passthruMsgPtr2 = (PASSTHRU_MSG*) 0L;
    PASSTHRU_MSG* passthruMsgPtr3 = (PASSTHRU_MSG*) 0L;
    uint num1 = 0;
    if (mask != null)
      passthruMsgPtr1 = mask.Convert();
    if (pattern != null)
      passthruMsgPtr2 = pattern.Convert();
    if (flow != null)
      passthruMsgPtr3 = flow.Convert();
    int num2 = \u003CModule\u003E.PassThruStartMsgFilter(channelId, (uint) type, passthruMsgPtr1, passthruMsgPtr2, passthruMsgPtr3, &num1);
    if ((IntPtr) passthruMsgPtr1 != IntPtr.Zero)
      \u003CModule\u003E.delete((void*) passthruMsgPtr1);
    if ((IntPtr) passthruMsgPtr2 != IntPtr.Zero)
      \u003CModule\u003E.delete((void*) passthruMsgPtr2);
    if ((IntPtr) passthruMsgPtr3 != IntPtr.Zero)
      \u003CModule\u003E.delete((void*) passthruMsgPtr3);
    filterId = num1;
    return (J2534Error) num2;
  }

  public static J2534Error StopMsgFilter(uint channelId, uint filterId)
  {
    return (J2534Error) \u003CModule\u003E.PassThruStopMsgFilter(channelId, filterId);
  }

  public static unsafe string GetLastError()
  {
    \u0024ArrayType\u0024\u0024\u0024BY0FA\u0040D arrayTypeBy0FaD;
    return \u003CModule\u003E.PassThruGetLastError((sbyte*) &arrayTypeBy0FaD) == 0 ? new string((sbyte*) &arrayTypeBy0FaD) : (string) null;
  }

  public static unsafe J2534Error WriteMsgs(
    uint channelId,
    IList<PassThruMsg> messages,
    uint timeout)
  {
    ulong count1 = (ulong) messages.Count;
    PASSTHRU_MSG* passthruMsgPtr = (PASSTHRU_MSG*) \u003CModule\u003E.new\u005B\u005D(count1 > 4442857435864535UL ? ulong.MaxValue : count1 * 4152UL);
    int index = 0;
    if (0 < messages.Count)
    {
      do
      {
        PASSTHRU_MSG* _Source = messages[index].Convert();
        if (\u003CModule\u003E.\u003FA0xb03e8ad9\u002Ememcpy_s((void*) ((long) index * 4152L + (IntPtr) passthruMsgPtr), 4152UL, (void*) _Source, 4152UL) == 0)
        {
          \u003CModule\u003E.delete((void*) _Source);
          ++index;
        }
        else
          goto label_3;
      }
      while (index < messages.Count);
      goto label_4;
label_3:
      throw new InvalidOperationException("Unable to copy write message");
    }
label_4:
    uint count2 = (uint) messages.Count;
    int num = \u003CModule\u003E.PassThruWriteMsgs(channelId, passthruMsgPtr, &count2, timeout);
    \u003CModule\u003E.delete((void*) passthruMsgPtr);
    return (J2534Error) num;
  }

  public static unsafe J2534Error ReadMsgs(
    uint channelId,
    IList<PassThruMsg> messages,
    uint messageCount,
    uint timeout)
  {
    PASSTHRU_MSG* passthruMsgPtr = (PASSTHRU_MSG*) \u003CModule\u003E.new\u005B\u005D((ulong) messageCount * 4152UL);
    uint num1 = messageCount;
    uint num2 = (uint) \u003CModule\u003E.PassThruReadMsgs(channelId, passthruMsgPtr, &num1, timeout);
    uint num3 = 0;
    if (0U < num1)
    {
      do
      {
        messages.Add(new PassThruMsg((PASSTHRU_MSG*) ((long) num3 * 4152L + (IntPtr) passthruMsgPtr)));
        ++num3;
      }
      while (num3 < num1);
    }
    \u003CModule\u003E.delete((void*) passthruMsgPtr);
    return (J2534Error) num2;
  }

  public static unsafe J2534Error SetPassthruCallback(uint channelId, J2534.PassthruCallback function)
  {
    Sid.PassthruCallback[channelId] = function;
    // ISSUE: cast to a function pointer type
    // ISSUE: cast to a function pointer type
    __FnPtr<void (uint, uint, void*, void*)> a0xb03e8ad9FyaxkkpeaX0Z = !((MulticastDelegate) function != (MulticastDelegate) null) ? (__FnPtr<void (uint, uint, void*, void*)>) 0L : (__FnPtr<void (uint, uint, void*, void*)>) (IntPtr) \u003CModule\u003E.__unep\u0040\u003FNativeCallbackFunction\u0040\u003FA0xb03e8ad9\u0040\u0040\u0024\u0024FYAXKKPEAX0\u0040Z;
    CALLBACK_CONFIG callbackConfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ref callbackConfig = (long) a0xb03e8ad9FyaxkkpeaX0Z;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &callbackConfig + 8) = 1L;
    return (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 69323U, (void*) &callbackConfig, (void*) 0L);
  }

  public static unsafe J2534Error SetAllFiltersToPass(uint channelId, byte state)
  {
    return (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 65711U, (void*) &state, (void*) 0L);
  }

  public static unsafe J2534Error SetBaudRate(uint channelId, uint baudRate)
  {
    SCONFIG sconfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfig = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) &sconfig + 4) = (int) baudRate;
    SCONFIG_LIST sconfigList;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfigList = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &sconfigList + 4) = (long) &sconfig;
    return (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 2U, (void*) &sconfigList, (void*) 0L);
  }

  public static unsafe string GetDeviceName(uint channelId)
  {
    string deviceName = (string) null;
    RP1210_DEVICE_INFO rp1210DeviceInfo;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ref rp1210DeviceInfo = (long) \u003CModule\u003E.new\u005B\u005D(80UL /*0x50*/);
    if (\u003CModule\u003E.PassThruIoctl(channelId, 69036U, (void*) 0L, (void*) &rp1210DeviceInfo) == 0)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      deviceName = new string((sbyte*) ^(long&) ref rp1210DeviceInfo);
    }
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    \u003CModule\u003E.delete((void*) ^(long&) ref rp1210DeviceInfo);
    return deviceName;
  }

  public static unsafe void GetDeviceVersionInfo(
    uint channelId,
    ref string libraryName,
    ref string libraryVersion,
    ref string driverVersion,
    ref string firmwareVersion,
    ref string supportedProtocols)
  {
    RP1210_DEVICE_VERSION_INFO deviceVersionInfo;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ref deviceVersionInfo = (long) \u003CModule\u003E.new\u005B\u005D(80UL /*0x50*/);
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &deviceVersionInfo + 8) = (long) \u003CModule\u003E.new\u005B\u005D(80UL /*0x50*/);
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &deviceVersionInfo + 16 /*0x10*/) = (long) \u003CModule\u003E.new\u005B\u005D(80UL /*0x50*/);
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &deviceVersionInfo + 24) = (long) \u003CModule\u003E.new\u005B\u005D(80UL /*0x50*/);
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &deviceVersionInfo + 32 /*0x20*/) = (long) \u003CModule\u003E.new\u005B\u005D(80UL /*0x50*/);
    if (\u003CModule\u003E.PassThruIoctl(channelId, 69035U, (void*) 0L, (void*) &deviceVersionInfo) == 0)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      libraryName = new string((sbyte*) ^(long&) ref deviceVersionInfo);
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      libraryVersion = new string((sbyte*) ^(long&) ((IntPtr) &deviceVersionInfo + 8));
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      driverVersion = new string((sbyte*) ^(long&) ((IntPtr) &deviceVersionInfo + 8));
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      firmwareVersion = new string((sbyte*) ^(long&) ((IntPtr) &deviceVersionInfo + 24));
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      supportedProtocols = new string((sbyte*) ^(long&) ((IntPtr) &deviceVersionInfo + 32 /*0x20*/));
    }
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    \u003CModule\u003E.delete((void*) ^(long&) ref deviceVersionInfo);
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    \u003CModule\u003E.delete((void*) ^(long&) ((IntPtr) &deviceVersionInfo + 8));
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    \u003CModule\u003E.delete((void*) ^(long&) ((IntPtr) &deviceVersionInfo + 16 /*0x10*/));
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    \u003CModule\u003E.delete((void*) ^(long&) ((IntPtr) &deviceVersionInfo + 24));
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    \u003CModule\u003E.delete((void*) ^(long&) ((IntPtr) &deviceVersionInfo + 32 /*0x20*/));
  }

  public static unsafe J2534Error GetBaudRate(uint channelId, ref uint baudRate)
  {
    SCONFIG sconfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfig = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) &sconfig + 4) = 0;
    SCONFIG_LIST sconfigList;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfigList = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &sconfigList + 4) = (long) &sconfig;
    J2534Error baudRate1 = (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 1U, (void*) &sconfigList, (void*) 0L);
    if (baudRate1 == J2534Error.NoError)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      baudRate = (uint) ^(int&) ((IntPtr) &sconfig + 4);
    }
    return baudRate1;
  }

  public static unsafe J2534Error IsAutoBaudRateCapable(uint channelId, ref bool isCapable)
  {
    SCONFIG sconfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfig = 65537 /*0x010001*/;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) &sconfig + 4) = 0;
    SCONFIG_LIST sconfigList;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfigList = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &sconfigList + 4) = (long) &sconfig;
    J2534Error j2534Error = (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 1U, (void*) &sconfigList, (void*) 0L);
    if (j2534Error == J2534Error.NoError)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      int num = ^(int&) ((IntPtr) &sconfig + 4) != 0 ? 1 : 0;
      isCapable = num != 0;
    }
    return j2534Error;
  }

  public static unsafe J2534Error GetErrorCode(uint channelId, ref uint errorCode)
  {
    uint num;
    int errorCode1 = \u003CModule\u003E.PassThruIoctl(channelId, 65772U, (void*) 0L, (void*) &num);
    errorCode = num;
    return (J2534Error) errorCode1;
  }

  public static unsafe J2534Error SetUseConnectionMutex(uint channelId, byte state)
  {
    SCONFIG sconfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfig = 65539 /*0x010003*/;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) &sconfig + 4) = (int) state;
    SCONFIG_LIST sconfigList;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfigList = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &sconfigList + 4) = (long) &sconfig;
    return (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 2U, (void*) &sconfigList, (void*) 0L);
  }

  public static unsafe J2534Error SetProvideCustomMessageTypes(uint channelId, byte state)
  {
    SCONFIG sconfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfig = 65540 /*0x010004*/;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) &sconfig + 4) = (int) state;
    SCONFIG_LIST sconfigList;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfigList = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &sconfigList + 4) = (long) &sconfig;
    return (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 2U, (void*) &sconfigList, (void*) 0L);
  }

  public static unsafe J2534Error GetHardwareStatus(
    uint channelId,
    ref byte hardwareStatus,
    ref byte protocolStatusJ1939,
    ref byte protocolStatusJ1708,
    ref byte protocolStatusCan,
    ref byte protocolStatusIso15765)
  {
    RP1210_HARDWARE_STATUS rp1210HardwareStatus;
    // ISSUE: initblk instruction
    __memset(ref rp1210HardwareStatus, 0, 10);
    J2534Error hardwareStatus1 = (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 69034U, (void*) 0L, (void*) &rp1210HardwareStatus);
    if (hardwareStatus1 == J2534Error.NoError)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      hardwareStatus = ^(byte&) ref rp1210HardwareStatus;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      protocolStatusJ1939 = ^(byte&) ((IntPtr) &rp1210HardwareStatus + 2);
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      protocolStatusJ1708 = ^(byte&) ((IntPtr) &rp1210HardwareStatus + 4);
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      protocolStatusCan = ^(byte&) ((IntPtr) &rp1210HardwareStatus + 6);
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      protocolStatusIso15765 = ^(byte&) ((IntPtr) &rp1210HardwareStatus + 8);
    }
    return hardwareStatus1;
  }

  public static unsafe J2534Error GetChannelInfo(
    PassThruMsg msg,
    ref uint channelId,
    ref RP1210ProtocolId rp1210ProtocolId,
    ref ushort physicalChannel,
    ref string rp1210ProtocolString)
  {
    PASSTHRU_MSG* passthruMsgPtr = (PASSTHRU_MSG*) 0L;
    if (msg != null)
      passthruMsgPtr = msg.Convert();
    RP1210_CHANNEL_INFO rp1210ChannelInfo;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &rp1210ChannelInfo + 8) = (long) \u003CModule\u003E.new\u005B\u005D(80UL /*0x50*/);
    J2534Error channelInfo = (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 69033U, (void*) passthruMsgPtr, (void*) &rp1210ChannelInfo);
    if (channelInfo == J2534Error.NoError)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      channelId = (uint) ^(int&) ref rp1210ChannelInfo;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      rp1210ProtocolId = (RP1210ProtocolId) ^(ushort&) ((IntPtr) &rp1210ChannelInfo + 4);
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      physicalChannel = ^(ushort&) ((IntPtr) &rp1210ChannelInfo + 6);
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      rp1210ProtocolString = new string((sbyte*) ^(long&) ((IntPtr) &rp1210ChannelInfo + 8));
    }
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    \u003CModule\u003E.delete((void*) ^(long&) ((IntPtr) &rp1210ChannelInfo + 8));
    if ((IntPtr) passthruMsgPtr != IntPtr.Zero)
      \u003CModule\u003E.delete((void*) passthruMsgPtr);
    return channelInfo;
  }

  public static unsafe J2534Error SetAllowAutoBaudRate([MarshalAs(UnmanagedType.U1)] bool allowAutoBaudRate)
  {
    SCONFIG sconfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfig = 65537 /*0x010001*/;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) &sconfig + 4) = allowAutoBaudRate ? 1 : 0;
    SCONFIG_LIST sconfigList;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfigList = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &sconfigList + 4) = (long) &sconfig;
    return (J2534Error) \u003CModule\u003E.PassThruIoctl(0U, 2U, (void*) &sconfigList, (void*) 0L);
  }

  public static unsafe J2534Error GetEthernetGuid(uint channelId, ref string guid)
  {
    sbyte* numPtr = (sbyte*) \u003CModule\u003E.new\u005B\u005D(80UL /*0x50*/);
    J2534Error ethernetGuid = (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 69032U, (void*) 0L, (void*) numPtr);
    if (ethernetGuid == J2534Error.NoError)
      guid = new string(numPtr);
    \u003CModule\u003E.delete((void*) numPtr);
    return ethernetGuid;
  }

  private Sid()
  {
  }
}
