// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbPhysicalDimension
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbPhysicalDimension : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  int CurrentExponent { get; }

  int LengthExponent { get; }

  int LuminousIntensity { get; }

  int MassExponent { get; }

  int MolarAmountExponent { get; }

  int TemperatureExponent { get; }

  int TimeExponent { get; }
}
