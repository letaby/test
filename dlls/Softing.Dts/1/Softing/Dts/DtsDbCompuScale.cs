// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbCompuScale
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsDbCompuScale : DtsObject, MCDObject, IDisposable
{
  string ShortLabel { get; }

  bool IsDescriptionValid { get; }

  bool IsShortLabelValid { get; }

  string Description { get; }

  bool IsCompuInverseValueValid { get; }

  MCDValue CompuInverseValue { get; }

  bool IsCompuConstValueValid { get; }

  MCDValue CompuConstValue { get; }

  bool IsLowerLimitValid { get; }

  DtsDbLimit LowerLimit { get; }

  bool IsUpperLimitValid { get; }

  DtsDbLimit UpperLimit { get; }

  uint CompuNumeratorCount { get; }

  uint CompuDenominatorCount { get; }

  double GetCompuNumeratorAt(uint idx);

  double GetCompuDenominatorAt(uint idx);

  string CompuDenominatorsAsString { get; }

  string CompuNumeratorsAsString { get; }
}
