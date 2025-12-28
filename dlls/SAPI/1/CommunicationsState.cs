// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CommunicationsState
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

#nullable disable
namespace SapiLayer1;

public enum CommunicationsState
{
  Unknown = -1, // 0xFFFFFFFF
  OnlineButNotInitialized = 0,
  Online = 1,
  Disconnecting = 2,
  Offline = 3,
  ReadEcuInfo = 4,
  ReadParameters = 5,
  WriteParameters = 6,
  Flash = 7,
  ExecuteService = 8,
  ReadInstrument = 9,
  ResetFaults = 10, // 0x0000000A
  ReadFaults = 11, // 0x0000000B
  LogFilePlayback = 12, // 0x0000000C
  LogFilePaused = 13, // 0x0000000D
  ProcessVcp = 14, // 0x0000000E
  ResetFault = 15, // 0x0000000F
  ByteMessage = 16, // 0x00000010
  ReadSnapshot = 17, // 0x00000011
}
