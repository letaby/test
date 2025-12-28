// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdObjectType
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

#nullable disable
namespace McdAbstraction;

public enum McdObjectType
{
  None = 0,
  DBProtocolParameterSet = 1171, // 0x00000493
  DBService = 1177, // 0x00000499
  DBSingleEcuJob = 1179, // 0x0000049B
  DBStartCommunication = 1180, // 0x0000049C
  DBStopCommunication = 1181, // 0x0000049D
  DBVariantIdentification = 1182, // 0x0000049E
}
