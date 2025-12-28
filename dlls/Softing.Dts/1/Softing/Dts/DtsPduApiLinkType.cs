// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsPduApiLinkType
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

#nullable disable
namespace Softing.Dts;

public enum DtsPduApiLinkType : uint
{
  eUndefinedPduApiLinkType = 33024, // 0x00008100
  ISO_11898_2_DWCAN = 33025, // 0x00008101
  ISO_11898_3_DWFTCAN = 33026, // 0x00008102
  ISO_11992_1_DWCAN = 33027, // 0x00008103
  ISO_9141_2_UART = 33028, // 0x00008104
  ISO_14230_1_UART = 33029, // 0x00008105
  ISO_11898_RAW = 33030, // 0x00008106
  SAE_J1850_VPW = 33031, // 0x00008107
  SAE_J1850_PWM = 33032, // 0x00008108
  SAE_J2610_UART = 33033, // 0x00008109
  SAE_J1708_UART = 33034, // 0x0000810A
  SAE_J1939_11_DWCAN = 33035, // 0x0000810B
  GMW_3089_SWCAN = 33036, // 0x0000810C
  XDE_5024_UART = 33037, // 0x0000810D
  CCD_UART = 33038, // 0x0000810E
  SAE_J2411_SWCAN = 33039, // 0x0000810F
  IEEE_802_3 = 33040, // 0x00008110
  KW500_1_UART = 33041, // 0x00008111
  MOST = 33042, // 0x00008112
  ISO_17987_4 = 33043, // 0x00008113
  LIN_1 = 33044, // 0x00008114
}
