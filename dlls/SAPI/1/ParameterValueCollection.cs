// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ParameterValueCollection
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

public sealed class ParameterValueCollection : 
  ReadOnlyCollection<ParameterValue>,
  IEnumerable<ParameterValue>,
  IEnumerable
{
  private Parameter parent;
  private ParameterValue currentValue;

  internal ParameterValueCollection(Parameter parent)
    : base((IList<ParameterValue>) new List<ParameterValue>())
  {
    this.parent = parent;
  }

  internal void Add(ParameterValue parameterValue, bool setCurrent = true)
  {
    if (setCurrent)
      this.currentValue = parameterValue;
    lock (this.Items)
      this.Items.Add(parameterValue);
  }

  internal bool SetCurrentTime(DateTime time)
  {
    ParameterValue currentAtTime = this.GetCurrentAtTime(time);
    if (currentAtTime == this.currentValue)
      return false;
    this.currentValue = currentAtTime;
    this.parent.InternalSetValueFromLogFile(this.currentValue);
    return true;
  }

  public ParameterValue Current => this.currentValue;

  public ParameterValue GetCurrentAtTime(DateTime time)
  {
    ParameterValue currentAtTime = (ParameterValue) null;
    foreach (ParameterValue parameterValue in this)
    {
      if (parameterValue.Time <= time)
        currentAtTime = parameterValue;
      else if (parameterValue.Time > time)
        break;
    }
    return currentAtTime;
  }

  public new IEnumerator<ParameterValue> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<ParameterValue>) new List<ParameterValue>((IEnumerable<ParameterValue>) this.Items).GetEnumerator();
  }

  IEnumerator<ParameterValue> IEnumerable<ParameterValue>.GetEnumerator() => this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
  public ParameterValue get_CurrentAtTime(DateTime time) => this.GetCurrentAtTime(time);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
  public object SyncRoot => (object) this.Items;
}
