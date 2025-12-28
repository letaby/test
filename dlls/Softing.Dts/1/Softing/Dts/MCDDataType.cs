// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDataType
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

#nullable disable
namespace Softing.Dts;

public enum MCDDataType : uint
{
  eA_ASCIISTRING = 1,
  eA_BITFIELD = 2,
  eA_BYTEFIELD = 3,
  eA_FLOAT32 = 4,
  eA_FLOAT64 = 5,
  eA_INT16 = 6,
  eA_INT32 = 7,
  eA_INT64 = 8,
  eA_INT8 = 9,
  eA_UINT16 = 10, // 0x0000000A
  eA_UINT32 = 11, // 0x0000000B
  eA_UINT64 = 12, // 0x0000000C
  eA_UINT8 = 13, // 0x0000000D
  eA_UNICODE2STRING = 14, // 0x0000000E
  eFIELD = 15, // 0x0000000F
  eMULTIPLEXER = 16, // 0x00000010
  eSTRUCTURE = 17, // 0x00000011
  eTEXTTABLE = 18, // 0x00000012
  eA_BOOLEAN = 19, // 0x00000013
  eDTC = 20, // 0x00000014
  eENVDATA = 21, // 0x00000015
  eEND_OF_PDU = 22, // 0x00000016
  eTABLE = 23, // 0x00000017
  eENVDATADESC = 24, // 0x00000018
  eKEY = 25, // 0x00000019
  eLENGTHKEY = 26, // 0x0000001A
  eTABLE_ROW = 27, // 0x0000001B
  eSTRUCT_FIELD = 28, // 0x0000001C
  eNO_TYPE = 255, // 0x000000FF
}
