// Decompiled with JetBrains decompiler
// Type: SapiLayer1.DiagnosisVariantCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class DiagnosisVariantCollection : ReadOnlyCollection<DiagnosisVariant>
{
  internal DiagnosisVariantCollection(Ecu e)
    : base((IList<DiagnosisVariant>) new List<DiagnosisVariant>())
  {
    if (e.IsRollCall || e.IsByteMessaging || e.IsMcd)
      return;
    this.Add(new DiagnosisVariant(e, "_base_", string.Empty, true));
  }

  internal void Add(DiagnosisVariant diagnosisVariant) => this.Items.Add(diagnosisVariant);

  public DiagnosisVariant this[string name]
  {
    get
    {
      return this.FirstOrDefault<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (item => string.Equals(item.Name, name, StringComparison.Ordinal)));
    }
  }

  public DiagnosisVariant Base => this[0];
}
