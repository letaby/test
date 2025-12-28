// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDExecutionState
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

#nullable disable
namespace Softing.Dts;

public enum MCDExecutionState : uint
{
  eCANCELED_FROM_QUEUE = 25346, // 0x00006302
  eCANCELED_DURING_EXECUTION = 25347, // 0x00006303
  eFAILED = 25348, // 0x00006304
  eALL_FAILED = 25350, // 0x00006306
  eINVALID_RESPONSE = 25351, // 0x00006307
  eALL_INVALID_RESPONSE = 25352, // 0x00006308
  eALL_POSITIVE = 25353, // 0x00006309
  eNEGATIVE = 25354, // 0x0000630A
  eALL_NEGATIVE = 25355, // 0x0000630B
}
