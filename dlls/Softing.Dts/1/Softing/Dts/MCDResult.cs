// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDResult
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDResult : MCDObject, IDisposable
{
  MCDResultType Type { get; }

  bool HasError { get; }

  MCDError Error { get; set; }

  MCDResponses Responses { get; }

  string ServiceShortName { get; }

  MCDResultState ResultState { get; }

  ulong RequestEndTime { get; }
}
