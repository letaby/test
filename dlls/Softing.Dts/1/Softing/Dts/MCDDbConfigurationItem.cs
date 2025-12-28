// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbConfigurationItem
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbConfigurationItem : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  uint BitLength { get; }

  ushort BitPos { get; }

  uint BytePos { get; }

  MCDDataType DataType { get; }

  MCDDbUnit DbUnit { get; }

  MCDConstraint InternalConstraint { get; }

  MCDInterval Interval { get; }

  string Semantic { get; }

  MCDTextTableElements TextTableElements { get; }

  bool IsComplex { get; }

  MCDDbSpecialDataGroups DbSDGs { get; }
}
