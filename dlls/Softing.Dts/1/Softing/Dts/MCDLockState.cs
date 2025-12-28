// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDLockState
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

#nullable disable
namespace Softing.Dts;

public enum MCDLockState : uint
{
  eUNLOCKED = 513, // 0x00000201
  eLOCKED_BY_THIS_OBJECT = 514, // 0x00000202
  eLOCKED_BY_ANOTHER_OBJECT = 515, // 0x00000203
}
