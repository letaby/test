// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsParameterMetaInfo
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsParameterMetaInfo : 
  DtsNamedObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsObject
{
  MCDDataType ParameterType { get; }

  MCDValue DefaultValue { get; }

  MCDValue MinValue { get; }

  MCDValue MaxValue { get; }

  ushort Radix { get; }

  ushort DecimalPlaces { get; }

  string Unit { get; }

  ushort MinLength { get; }

  ushort MaxLength { get; }

  MCDTextTableElements TextTableElements { get; }
}
