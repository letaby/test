// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsResponse
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsResponse : 
  MCDResponse,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  uint LocationAddress { get; }

  uint ResponseTime { get; }

  uint CANIdentifier { get; }

  void EnterPDU(MCDValue pdu);

  bool HasResponseMessage { get; }
}
