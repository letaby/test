// Decompiled with JetBrains decompiler
// Type: SapiLayer1.McdCaesarEquivalenceScaleInfo
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SapiLayer1;

internal class McdCaesarEquivalenceScaleInfo
{
  internal McdCaesarEquivalenceScaleInfo(Type type, McdDBDataObjectProp dataObjectProp)
  {
    this.Coding = new SapiLayer1.Coding?(dataObjectProp.CodedType == typeof (int) ? SapiLayer1.Coding.TwosComplement : SapiLayer1.Coding.Unsigned);
    this.ByteOrder = new SapiLayer1.ByteOrder?(dataObjectProp.IsHighLowByteOrder ? SapiLayer1.ByteOrder.HighLow : SapiLayer1.ByteOrder.LowHigh);
    if (type != typeof (Dump) && type != typeof (Choice) && dataObjectProp.CodedType != typeof (float))
    {
      List<ScaleEntry> source1 = new List<ScaleEntry>();
      foreach (McdDBCompuScale scaleEntry in dataObjectProp.ScaleEntries)
      {
        double? nullable1 = scaleEntry.Factor;
        if (nullable1.HasValue)
        {
          nullable1 = scaleEntry.Offset;
          if (nullable1.HasValue)
          {
            nullable1 = this.Factor;
            if (!nullable1.HasValue)
            {
              nullable1 = this.Offset;
              if (!nullable1.HasValue)
              {
                nullable1 = scaleEntry.Factor;
                this.Factor = new double?(nullable1.Value);
                nullable1 = scaleEntry.Offset;
                this.Offset = new double?(nullable1.Value);
              }
            }
            long? nullable2 = scaleEntry.Max;
            if (nullable2.HasValue)
            {
              nullable2 = scaleEntry.Min;
              if (nullable2.HasValue)
              {
                source1.Add(new ScaleEntry(scaleEntry));
                continue;
              }
            }
            if (this.FactorOffsetScale == null)
              this.FactorOffsetScale = new ScaleEntry(scaleEntry);
          }
        }
      }
      if (source1.Any<ScaleEntry>())
      {
        IEnumerable<ScaleEntry> source2 = source1.Where<ScaleEntry>((Func<ScaleEntry, bool>) (scale => !scale.IsFixedValue));
        if (source2.Any<ScaleEntry>())
        {
          this.Min = new Decimal?(source2.Min<ScaleEntry>((Func<ScaleEntry, Decimal>) (scale => scale.Min)));
          this.Max = new Decimal?(source2.Max<ScaleEntry>((Func<ScaleEntry, Decimal>) (scale => scale.Max)));
        }
        this.Scales = source1;
        this.ConversionType = new SapiLayer1.ConversionType?(SapiLayer1.ConversionType.Scale);
      }
      else
        this.ConversionType = new SapiLayer1.ConversionType?(!this.Factor.HasValue ? SapiLayer1.ConversionType.Raw : SapiLayer1.ConversionType.FactorOffset);
    }
    else
      this.ConversionType = new SapiLayer1.ConversionType?(dataObjectProp.CodedType == typeof (float) ? SapiLayer1.ConversionType.Ieee : (type == typeof (Dump) ? SapiLayer1.ConversionType.Dump : SapiLayer1.ConversionType.Scale));
  }

  internal List<ScaleEntry> Scales { get; private set; }

  internal double? Factor { get; private set; }

  internal double? Offset { get; private set; }

  internal SapiLayer1.ConversionType? ConversionType { get; private set; }

  internal Decimal? Min { get; private set; }

  internal Decimal? Max { get; private set; }

  internal SapiLayer1.Coding? Coding { get; private set; }

  internal SapiLayer1.ByteOrder? ByteOrder { get; private set; }

  internal ScaleEntry FactorOffsetScale { get; private set; }
}
