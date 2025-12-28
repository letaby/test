// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashSecurityCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace SapiLayer1;

public sealed class FlashSecurityCollection : ReadOnlyCollection<FlashSecurity>
{
  internal FlashSecurityCollection()
    : base((IList<FlashSecurity>) new List<FlashSecurity>())
  {
  }

  internal void Add(FlashSecurity flashSecurityBlock) => this.Items.Add(flashSecurityBlock);
}
