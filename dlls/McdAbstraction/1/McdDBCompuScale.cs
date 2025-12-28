// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBCompuScale
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdDBCompuScale
{
  internal McdDBCompuScale(DtsDbCompuScale scale)
  {
    if (scale.IsLowerLimitValid)
      this.Min = GetValue(scale.LowerLimit.Value);
    if (scale.IsUpperLimitValid)
      this.Max = GetValue(scale.UpperLimit.Value);
    if (scale.CompuNumeratorCount == 2U)
    {
      double num = scale.CompuDenominatorCount == 1U ? scale.GetCompuDenominatorAt(0U) : 1.0;
      this.Factor = new double?(scale.GetCompuNumeratorAt(1U) / num);
      this.Offset = new double?(scale.GetCompuNumeratorAt(0U) / num);
    }
    if (!scale.IsShortLabelValid)
      return;
    this.Name = scale.ShortLabel;

    static long? GetValue(MCDValue value)
    {
      switch (value.DataType)
      {
        case MCDDataType.eA_INT32:
          return new long?((long) value.Int32);
        case MCDDataType.eA_UINT32:
          return new long?((long) value.Uint32);
        default:
          return new long?();
      }
    }
  }

  public double? Offset { get; private set; }

  public double? Factor { get; private set; }

  public long? Max { get; private set; }

  public long? Min { get; private set; }

  public string Name { get; private set; }
}
