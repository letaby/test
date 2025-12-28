// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDResponse
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDResponse : MCDNamedObject, MCDObject, IDisposable
{
  MCDResponseState State { get; }

  bool HasError { get; }

  MCDError Error { get; set; }

  MCDAccessKey AccessKeyOfLocation { get; }

  MCDValue ResponseMessage { get; }

  MCDDbResponse DbObject { get; }

  MCDResponseParameters ResponseParameters { get; }

  MCDObject Parent { get; }

  MCDValue ContainedResponseMessage { get; }

  ulong EndTime { get; }

  ulong StartTime { get; }
}
