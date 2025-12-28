// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingStringState
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

[Flags]
internal enum CodingStringState
{
  None = 0,
  NeedsUpdate = 1,
  AssignedByClient = 2,
  Incomplete = 4,
}
