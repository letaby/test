// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceTypes
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

[Flags]
public enum ServiceTypes
{
  None = 0,
  Actuator = 1,
  Adjustment = 2,
  Data = 16, // 0x00000010
  Download = 64, // 0x00000040
  DiagJob = 262144, // 0x00040000
  Environment = 128, // 0x00000080
  Function = 512, // 0x00000200
  Global = 65536, // 0x00010000
  IOControl = 8388608, // 0x00800000
  ReadVarCode = 67108864, // 0x04000000
  Routine = 4194304, // 0x00400000
  Security = 524288, // 0x00080000
  Session = 1048576, // 0x00100000
  Static = 1024, // 0x00000400
  StoredData = 2097152, // 0x00200000
  WriteVarCode = 33554432, // 0x02000000
}
