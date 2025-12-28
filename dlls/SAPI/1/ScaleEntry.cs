// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ScaleEntry
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace SapiLayer1;

public sealed class ScaleEntry
{
  internal const string SignalNotAvailable = "sna";

  internal ScaleEntry(PrepScaleEntry prepEntry, PresScaleEntry presEntry)
  {
    this.Factor = Convert.ToDecimal((object) (double) prepEntry.Factor, (IFormatProvider) CultureInfo.InvariantCulture);
    this.Offset = Convert.ToDecimal((object) (double) prepEntry.Offset, (IFormatProvider) CultureInfo.InvariantCulture);
    if (Math.Floor(this.Factor) != this.Factor)
    {
      Decimal d = Convert.ToDecimal((object) (double) presEntry.Factor, (IFormatProvider) CultureInfo.InvariantCulture);
      if (Math.Floor(d) == d)
      {
        this.Factor = 1M / d;
        this.Offset = -(Convert.ToDecimal((object) (double) presEntry.Offset, (IFormatProvider) CultureInfo.InvariantCulture) * this.Factor);
      }
    }
    this.Name = prepEntry.Name;
    this.Min = this.ToPhysicalValue((Decimal) presEntry.Min);
    this.Max = this.ToPhysicalValue((Decimal) presEntry.Max);
  }

  internal ScaleEntry(PrepScaleEntry entry)
  {
    this.Min = Convert.ToDecimal((object) (double) entry.Min, (IFormatProvider) CultureInfo.InvariantCulture);
    this.Max = Convert.ToDecimal((object) (double) entry.Max, (IFormatProvider) CultureInfo.InvariantCulture);
    this.Factor = Convert.ToDecimal((object) (double) entry.Factor, (IFormatProvider) CultureInfo.InvariantCulture);
    this.Offset = Convert.ToDecimal((object) (double) entry.Offset, (IFormatProvider) CultureInfo.InvariantCulture);
    this.Name = entry.Name;
  }

  internal ScaleEntry(McdDBCompuScale entry)
  {
    this.Factor = 1M / Convert.ToDecimal((object) entry.Factor.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    this.Offset = -(Convert.ToDecimal((object) entry.Offset.Value, (IFormatProvider) CultureInfo.InvariantCulture) * this.Factor);
    this.Name = entry.Name;
    if (entry.Min.HasValue)
      this.Min = this.ToPhysicalValue((Decimal) entry.Min.Value);
    long? max = entry.Max;
    if (!max.HasValue)
      return;
    max = entry.Max;
    this.Max = this.ToPhysicalValue((Decimal) max.Value);
  }

  public Decimal Min { get; private set; }

  public Decimal Max { get; private set; }

  public Decimal Factor { get; private set; }

  public Decimal Offset { get; private set; }

  public string Name { get; internal set; }

  public bool IsFixedValue => this.ToEcuValue(this.Min) == this.ToEcuValue(this.Max);

  internal Decimal ToEcuValue(Decimal physicalValue)
  {
    return Math.Round(physicalValue * this.Factor + this.Offset, 0, MidpointRounding.AwayFromZero);
  }

  internal Decimal ToPhysicalValue(Decimal ecuValue) => (ecuValue - this.Offset) / this.Factor;

  public bool IsValueInRange(float value) => this.IsValueInRange(value.ToDecimal());

  public bool IsValueInRange(Decimal value) => this.IsEcuValueInRange(this.ToEcuValue(value));

  public bool IsEcuValueInRange(Decimal ecuValue)
  {
    Decimal ecuValue1 = this.ToEcuValue(this.Min);
    Decimal ecuValue2 = this.ToEcuValue(this.Max);
    return ecuValue >= ecuValue1 && ecuValue <= ecuValue2;
  }

  internal static IEnumerable<ScaleEntry> GetScales(
    CaesarEcu ecuHandle,
    CaesarPreparation preparation,
    CaesarPresentation presentation)
  {
    for (uint i = 0; i < preparation.NumberOfScales; ++i)
      yield return presentation != null ? new ScaleEntry(preparation.GetScaleEntry(ecuHandle, i), presentation.GetScaleEntry(ecuHandle, i)) : new ScaleEntry(preparation.GetScaleEntry(ecuHandle, i));
  }
}
