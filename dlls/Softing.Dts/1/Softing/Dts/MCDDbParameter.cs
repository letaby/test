// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbParameter
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbParameter : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  uint BytePos { get; }

  byte BitPos { get; }

  uint ByteLength { get; }

  ushort DecimalPlaces { get; }

  MCDValue DefaultValue { get; }

  ushort MaxLength { get; }

  ushort MinLength { get; }

  MCDDataType ParameterType { get; }

  ushort Radix { get; }

  string Unit { get; }

  uint DisplayLevel { get; }

  string Semantic { get; }

  MCDInterval Interval { get; }

  MCDDbParameters DbParameters { get; }

  bool IsConstant { get; }

  MCDConstraint InternalConstraint { get; }

  uint BitLength { get; }

  MCDDbTable DbTable { get; }

  MCDDbParameter DbTableKeyParam { get; }

  MCDDbParameters DbTableStructParams { get; }

  MCDValues Keys { get; }

  MCDDbParameters GetStructureByKey(MCDValue key);

  MCDTextTableElements TextTableElements { get; }

  MCDParameterType MCDParameterType { get; }

  MCDDataType DataType { get; }

  MCDValue CodedDefaultValue { get; }

  MCDIntervals ValidInternalIntervals { get; }

  MCDIntervals ValidPhysicalIntervals { get; }

  MCDConstraint PhysicalConstraint { get; }

  MCDDbSpecialDataGroups DbSDGs { get; }

  uint ODXBytePos { get; }

  MCDDbUnit DbUnit { get; }

  MCDDbParameter LengthKey { get; }

  bool IsVariableLength { get; }
}
