// Decompiled with JetBrains decompiler
// Type: rp1210.RP1210_Commands
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

#nullable disable
namespace rp1210;

public enum RP1210_Commands
{
  RP1210_Reset_Device = 0,
  RP1210_Set_All_Filters_States_to_Pass = 3,
  RP1210_Set_Message_Filtering_For_J1939 = 4,
  RP1210_Set_Message_Filtering_For_CAN = 5,
  RP1210_Set_Message_Filtering_For_J1708 = 7,
  RP1210_Set_Message_Filtering_For_J1850 = 8,
  RP1210_Set_Message_Filtering_For_ISO15765 = 9,
  RP1210_Generic_Driver_Command = 14, // 0x0000000E
  RP1210_Set_J1708_Mode = 15, // 0x0000000F
  RP1210_Echo_Transmitted_Messages = 16, // 0x00000010
  RP1210_Set_All_Filters_States_to_Discard = 17, // 0x00000011
  RP1210_Set_Message_Receive = 18, // 0x00000012
  RP1210_Protect_J1939_Address = 19, // 0x00000013
  RP1210_Set_Broadcast_For_J1708 = 20, // 0x00000014
  RP1210_Set_Broadcast_For_CAN = 21, // 0x00000015
  RP1210_Set_Broadcast_For_J1939 = 22, // 0x00000016
  RP1210_Set_Broadcast_For_J1850 = 23, // 0x00000017
  RP1210_Set_J1708_Filter_Type = 24, // 0x00000018
  RP1210_Set_J1939_Filter_Type = 25, // 0x00000019
  RP1210_Set_CAN_Filter_Type = 26, // 0x0000001A
  RP1210_Set_J1939_Interpacket_Time = 27, // 0x0000001B
  RP1210_SetMaxErrorMsgSize = 28, // 0x0000001C
  RP1210_Disallow_Further_Connections = 29, // 0x0000001D
  RP1210_Set_J1850_Filter_Type = 30, // 0x0000001E
  RP1210_Release_J1939_Address = 31, // 0x0000001F
  RP1210_Set_ISO15765_Filter_Type = 32, // 0x00000020
  RP1210_Set_Broadcast_For_ISO15765 = 33, // 0x00000021
  RP1210_Set_ISO15765_Flow_Control = 34, // 0x00000022
  RP1210_Clear_ISO15765_Flow_Control = 35, // 0x00000023
  RP1210_Set_ISO15765_Link_Type = 36, // 0x00000024
  RP1210_Set_J1939_Baud = 37, // 0x00000025
  RP1210_Set_ISO15765_Baud = 38, // 0x00000026
  RP1210_Set_BlockTimeout = 215, // 0x000000D7
  RP1210_Set_J1708_Baud = 305, // 0x00000131
}
