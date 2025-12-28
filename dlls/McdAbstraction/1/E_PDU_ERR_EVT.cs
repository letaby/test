// Decompiled with JetBrains decompiler
// Type: McdAbstraction.E_PDU_ERR_EVT
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

#nullable disable
namespace McdAbstraction;

internal enum E_PDU_ERR_EVT
{
  PDU_ERR_EVT_NOERROR = 0,
  PDU_ERR_EVT_FRAME_STRUCT = 256, // 0x00000100
  PDU_ERR_EVT_TX_ERROR = 257, // 0x00000101
  PDU_ERR_EVT_TESTER_PRESENT_ERROR = 258, // 0x00000102
  PDU_ERR_EVT_RX_TIMEOUT = 259, // 0x00000103
  PDU_ERR_EVT_RX_ERROR = 260, // 0x00000104
  PDU_ERR_EVT_PROT_ERR = 261, // 0x00000105
  PDU_ERR_EVT_LOST_COMM_TO_VCI = 262, // 0x00000106
  PDU_ERR_EVT_VCI_HARDWARE_FAULT = 263, // 0x00000107
  PDU_ERR_EVT_INIT_ERROR = 264, // 0x00000108
  PDU_ERR_EVT_RSC_LOCKED = 265, // 0x00000109
}
