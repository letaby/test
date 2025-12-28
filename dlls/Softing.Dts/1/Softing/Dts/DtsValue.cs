// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsValue
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsValue : MCDValue, MCDObject, IDisposable, DtsObject
{
  bool GetBitfieldValue(uint index);

  byte GetBytefieldValue(uint Index);

  string String { get; }

  bool IsEmpty { get; }

  void SetBitfieldValue(bool value, uint index);

  void SetBytefieldValue(byte Value, uint Index);

  bool IsValid { get; }
}
