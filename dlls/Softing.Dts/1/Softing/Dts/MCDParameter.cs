// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDParameter
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDParameter : MCDNamedObject, MCDObject, IDisposable
{
  ushort DecimalPlaces { get; }

  MCDValue Value { get; set; }

  string Unit { get; }

  ushort Radix { get; }

  MCDRangeInfo ValueRangeInfo { get; }

  MCDDataType Type { get; }

  MCDDbParameter DbObject { get; }

  MCDValue CodedValue { get; set; }

  MCDParameterType MCDParameterType { get; }

  MCDDataType DataType { get; }

  MCDScaleConstraint PhysicalScaleConstraint { get; }

  MCDScaleConstraint InternalScaleConstraint { get; }

  byte BitPos { get; }

  uint ByteLength { get; }

  uint BytePos { get; }
}
