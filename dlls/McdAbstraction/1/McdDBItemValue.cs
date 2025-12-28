// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBItemValue
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdDBItemValue
{
  private MCDDbItemValue item;

  internal McdDBItemValue(MCDDbItemValue data) => this.item = data;

  public string Meaning => this.item.Meaning;

  public string Description => this.item.Description;

  public string PartNumber => this.item.Key;

  public McdValue Value
  {
    get
    {
      try
      {
        return new McdValue(this.item.PhysicalConstantValue);
      }
      catch (MCDException ex)
      {
        return (McdValue) null;
      }
    }
  }

  internal MCDDbItemValue Handle => this.item;
}
