// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDConnectorPinType
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

#nullable disable
namespace Softing.Dts;

public enum MCDConnectorPinType : uint
{
  eHI = 29696, // 0x00007400
  eLOW = 29697, // 0x00007401
  eK = 29698, // 0x00007402
  eL = 29699, // 0x00007403
  eTX = 29700, // 0x00007404
  eRX = 29701, // 0x00007405
  ePLUS = 29702, // 0x00007406
  eMINUS = 29703, // 0x00007407
  eSINGLEWIRE = 29704, // 0x00007408
  eTX_MINUS = 29705, // 0x00007409
  eTX_PLUS = 29706, // 0x0000740A
  eRX_MINUS = 29707, // 0x0000740B
  eRX_PLUS = 29708, // 0x0000740C
  eUndefined_ConnectorPinType = 29711, // 0x0000740F
}
