// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDValue
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDValue : MCDObject, IDisposable
{
  void Clear();

  bool[] Bitfield { get; set; }

  byte[] Bytefield { get; set; }

  MCDDataType DataType { get; set; }

  float Float32 { get; set; }

  double Float64 { get; set; }

  short Int16 { get; set; }

  int Int32 { get; set; }

  long Int64 { get; set; }

  char Int8 { get; set; }

  int Length { get; }

  ushort Uint16 { get; set; }

  uint Uint32 { get; set; }

  ulong Uint64 { get; set; }

  byte Uint8 { get; set; }

  string ValueAsString { get; set; }

  string Asciistring { get; set; }

  string Unicode2string { get; set; }

  bool Boolean { get; set; }

  MCDValue Copy();
}
