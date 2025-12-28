// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdTextTableElement
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdTextTableElement
{
  internal McdTextTableElement(MCDTextTableElement element)
  {
    this.Name = element.LongName;
    MCDInterval interval = element.Interval;
    this.LowerLimit = new McdValue(interval.LowerLimit);
    this.UpperLimit = new McdValue(interval.UpperLimit);
  }

  public string Name { get; private set; }

  public McdValue LowerLimit { get; private set; }

  public McdValue UpperLimit { get; private set; }
}
