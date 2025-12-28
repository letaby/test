// Decompiled with JetBrains decompiler
// Type: SapiLayer1.DiagnosisProtocolCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class DiagnosisProtocolCollection : LateLoadReadOnlyCollection<DiagnosisProtocol>
{
  internal DiagnosisProtocolCollection()
  {
  }

  protected override void AcquireList()
  {
    this.Items.Clear();
    foreach (string protocol in CaesarRoot.Protocols)
      this.Items.Add(new DiagnosisProtocol(protocol));
    foreach (string protocolLocationName in McdRoot.DBProtocolLocationNames)
      this.Items.Add(new DiagnosisProtocol(protocolLocationName, true));
  }

  internal void ClearList()
  {
    this.Items.Clear();
    this.ResetList();
  }

  public DiagnosisProtocol this[string name]
  {
    get
    {
      return this.FirstOrDefault<DiagnosisProtocol>((Func<DiagnosisProtocol, bool>) (item => string.Equals(item.Name, name, StringComparison.Ordinal)));
    }
  }
}
