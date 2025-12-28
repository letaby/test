// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDTransmissionMode
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

#nullable disable
namespace Softing.Dts;

public enum MCDTransmissionMode : uint
{
  eNO_TRANSMISSION = 27137, // 0x00006A01
  eRECEIVE = 27138, // 0x00006A02
  eSEND = 27139, // 0x00006A03
  eSEND_AND_RECEIVE = 27140, // 0x00006A04
  eSEND_OR_RECEIVE = 27141, // 0x00006A05
}
