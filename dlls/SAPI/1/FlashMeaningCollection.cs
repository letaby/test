// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashMeaningCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class FlashMeaningCollection : ReadOnlyCollection<FlashMeaning>
{
  internal FlashMeaningCollection()
    : base((IList<FlashMeaning>) new List<FlashMeaning>())
  {
  }

  internal void Add(FlashMeaning meaning) => this.Items.Add(meaning);

  public FlashMeaning this[string flashKey]
  {
    get
    {
      return this.FirstOrDefault<FlashMeaning>((Func<FlashMeaning, bool>) (item => string.Equals(item.FlashKey, flashKey, StringComparison.Ordinal)));
    }
  }
}
