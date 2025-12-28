// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EcuInfoValueCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

#nullable disable
namespace SapiLayer1;

public sealed class EcuInfoValueCollection : 
  ReadOnlyCollection<EcuInfoValue>,
  IEnumerable<EcuInfoValue>,
  IEnumerable
{
  private EcuInfo parent;
  private EcuInfoValue currentValue;

  internal EcuInfoValueCollection(EcuInfo parent)
    : base((IList<EcuInfoValue>) new List<EcuInfoValue>())
  {
    this.parent = parent;
  }

  internal void Add(EcuInfoValue ecuInfoValue)
  {
    this.currentValue = ecuInfoValue;
    lock (this.Items)
      this.Items.Add(ecuInfoValue);
    if (this.parent.Channel.Ecu.RollCallManager == null)
      return;
    this.parent.Channel.Ecu.RollCallManager.NotifyEcuInfoValue(this.parent, ecuInfoValue.Value);
  }

  internal void SetCurrentTime(DateTime time)
  {
    EcuInfoValue currentAtTime = this.GetCurrentAtTime(time);
    if (currentAtTime == this.currentValue)
      return;
    this.currentValue = currentAtTime;
    this.parent.RaiseEcuInfoUpdateEvent((Exception) null, false);
  }

  public EcuInfoValue Current => this.currentValue;

  public EcuInfoValue GetCurrentAtTime(DateTime time)
  {
    EcuInfoValue currentAtTime = (EcuInfoValue) null;
    foreach (EcuInfoValue ecuInfoValue in this)
    {
      if (ecuInfoValue.Time <= time)
        currentAtTime = ecuInfoValue;
      else if (ecuInfoValue.Time > time)
        break;
    }
    return currentAtTime;
  }

  public new IEnumerator<EcuInfoValue> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<EcuInfoValue>) new List<EcuInfoValue>((IEnumerable<EcuInfoValue>) this.Items).GetEnumerator();
  }

  IEnumerator<EcuInfoValue> IEnumerable<EcuInfoValue>.GetEnumerator() => this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
  public EcuInfoValue get_CurrentAtTime(DateTime time) => this.GetCurrentAtTime(time);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
  public object SyncRoot => (object) this.Items;
}
