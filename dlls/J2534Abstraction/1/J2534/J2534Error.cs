// Decompiled with JetBrains decompiler
// Type: J2534.J2534Error
// Assembly: J2534Abstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: F558D3F4-6D07-4AE0-B148-E7AD8371AFDC
// Assembly location: C:\Users\petra\Downloads\Архив (2)\J2534Abstraction.dll

#nullable disable
namespace J2534;

public enum J2534Error
{
  NoError,
  NotSupported,
  InvalidChannelId,
  InvalidProtocolId,
  NullParameter,
  InvalidIoctlValue,
  InvalidFlags,
  Failed,
  DeviceNotConnected,
  Timeout,
  InvalidMsg,
  InvalidTimeInterval,
  ExceededLimit,
  InvalidMsgId,
  DeviceInUse,
  InvalidIoctlId,
  BufferEmpty,
  BufferFull,
  BufferOverflow,
  PinInvalid,
  ChannelInUse,
  MsgProtocolId,
  InvalidFilterId,
  NoFlowControl,
}
