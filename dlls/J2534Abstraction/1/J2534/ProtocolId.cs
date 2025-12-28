// Decompiled with JetBrains decompiler
// Type: J2534.ProtocolId
// Assembly: J2534Abstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: F558D3F4-6D07-4AE0-B148-E7AD8371AFDC
// Assembly location: C:\Users\petra\Downloads\Архив (2)\J2534Abstraction.dll

#nullable disable
namespace J2534;

public enum ProtocolId
{
  None = 0,
  Can = 5,
  Iso15765 = 6,
  Ethernet = 13400, // 0x00003458
  J1708 = 71432, // 0x00011708
  J1939 = 71993, // 0x00011939
  Can2 = 268435461, // 0x10000005
}
