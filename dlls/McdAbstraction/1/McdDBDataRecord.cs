// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBDataRecord
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System.Collections.Generic;

#nullable disable
namespace McdAbstraction;

public class McdDBDataRecord
{
  private MCDDbDataRecord item;
  private IEnumerable<byte> binaryData;

  internal McdDBDataRecord(MCDDbDataRecord data) => this.item = data;

  internal MCDDbDataRecord Handle => this.item;

  public IEnumerable<byte> BinaryData
  {
    get
    {
      if (this.binaryData == null)
        this.binaryData = (IEnumerable<byte>) this.item.BinaryData;
      return this.binaryData;
    }
  }

  public string Name => this.item.LongName;

  public string Qualifier => this.item.ShortName;

  public string Description => this.item.Description;

  public string PartNumber => this.item.Key;
}
