// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsWLanValidity
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

#nullable disable
namespace Softing.Dts;

public enum DtsWLanValidity : uint
{
  CHANNEL = 4097, // 0x00001001
  CHANNEL_FREQ = 4098, // 0x00001002
  CHANNEL_WIDTH = 4100, // 0x00001004
  TX_POWER = 4104, // 0x00001008
  LINK_SPEED = 4112, // 0x00001010
  RSSI = 4128, // 0x00001020
  SNR = 4160, // 0x00001040
  NOISE = 4224, // 0x00001080
  SIG_QUALITY = 4352, // 0x00001100
  SSID = 4608, // 0x00001200
}
