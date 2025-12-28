// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBOptionItem
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBOptionItem
{
  private MCDDbOptionItem item;
  private IEnumerable<McdDBItemValue> itemValues;
  private Type type;
  private IEnumerable<McdTextTableElement> textTableElements;

  internal McdDBOptionItem(MCDDbOptionItem data) => this.item = data;

  public IEnumerable<McdDBItemValue> DBItemValues
  {
    get
    {
      if (this.itemValues == null)
      {
        List<McdDBItemValue> mcdDbItemValueList = new List<McdDBItemValue>();
        if (this.item.DataType != MCDDataType.eNO_TYPE)
          mcdDbItemValueList.AddRange(this.item.DbItemValues.OfType<MCDDbItemValue>().Select<MCDDbItemValue, McdDBItemValue>((Func<MCDDbItemValue, McdDBItemValue>) (oi => new McdDBItemValue(oi))));
        this.itemValues = (IEnumerable<McdDBItemValue>) mcdDbItemValueList;
      }
      return this.itemValues;
    }
  }

  public string Name => this.item.LongName;

  public string Qualifier => this.item.ShortName;

  public string Description => this.item.Description;

  public int BytePos => (int) this.item.BytePos;

  public int BitPos => (int) this.item.BitPos;

  public int? BitLength
  {
    get => this.DataType != (Type) null ? new int?((int) this.item.BitLength) : new int?();
  }

  public Type DataType
  {
    get
    {
      if (this.type == (Type) null && this.item.DataType != MCDDataType.eNO_TYPE)
        this.type = McdRoot.MapDataType(this.item.DataType);
      return this.type;
    }
  }

  public IEnumerable<McdTextTableElement> TextTableElements
  {
    get
    {
      if (this.textTableElements == null && this.DataType == typeof (McdTextTableElement))
        this.textTableElements = (IEnumerable<McdTextTableElement>) this.item.TextTableElements.OfType<MCDTextTableElement>().Select<MCDTextTableElement, McdTextTableElement>((Func<MCDTextTableElement, McdTextTableElement>) (tt => new McdTextTableElement(tt))).ToList<McdTextTableElement>();
      return this.textTableElements;
    }
  }
}
