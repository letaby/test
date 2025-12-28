// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ChannelOptions
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

[Flags]
public enum ChannelOptions
{
  None = 0,
  StartStopCommunications = 1,
  CyclicRead = 4,
  MaintainSession = 8,
  ExecutePreService = 16, // 0x00000010
  ProcessAffected = 32, // 0x00000020
  ExecuteInitializeService = 64, // 0x00000040
  ExecuteParameterWriteInitializeCommitServices = 128, // 0x00000080
  All = ExecuteParameterWriteInitializeCommitServices | ExecuteInitializeService | ProcessAffected | ExecutePreService | MaintainSession | CyclicRead | StartStopCommunications, // 0x000000FD
}
