// Decompiled with JetBrains decompiler
// Type: SapiLayer1.VcpComponentError
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

#nullable disable
namespace SapiLayer1;

public enum VcpComponentError
{
  None = 0,
  NoError = 1000, // 0x000003E8
  NoParameterFile = 1001, // 0x000003E9
  NoDefinitionFile = 1002, // 0x000003EA
  ToolFailure = 1007, // 0x000003EF
}
