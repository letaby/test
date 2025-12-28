// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashDataBlockCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class FlashDataBlockCollection : ReadOnlyCollection<FlashDataBlock>
{
  internal FlashDataBlockCollection()
    : base((IList<FlashDataBlock>) new List<FlashDataBlock>())
  {
  }

  internal void Add(FlashDataBlock flashDataBlock) => this.Items.Add(flashDataBlock);

  public FlashDataBlock this[string qualifier]
  {
    get
    {
      return this.FirstOrDefault<FlashDataBlock>((Func<FlashDataBlock, bool>) (item => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal)));
    }
  }
}
