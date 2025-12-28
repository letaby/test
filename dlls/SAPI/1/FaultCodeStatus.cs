// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FaultCodeStatus
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

[Flags]
public enum FaultCodeStatus
{
  None = 0,
  Active = 1,
  Pending = 4,
  Stored = 8,
  TestFailedSinceLastClear = 32, // 0x00000020
  Mil = 128, // 0x00000080
  Permanent = 256, // 0x00000100
  Immediate = 512, // 0x00000200
}
