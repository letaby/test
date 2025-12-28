// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingStringValueCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace SapiLayer1;

public sealed class CodingStringValueCollection : 
  ReadOnlyCollection<CodingStringValue>,
  IEnumerable<CodingStringValue>,
  IEnumerable
{
  private CodingStringValue currentValue;

  internal CodingStringValueCollection()
    : base((IList<CodingStringValue>) new List<CodingStringValue>())
  {
  }

  internal void Add(CodingStringValue codingStringValue, bool setCurrent = true)
  {
    if (setCurrent)
      this.currentValue = codingStringValue;
    lock (this.Items)
      this.Items.Add(codingStringValue);
  }

  internal bool SetCurrentTime(DateTime time)
  {
    CodingStringValue currentAtTime = this.GetCurrentAtTime(time);
    if (currentAtTime == this.currentValue)
      return false;
    this.currentValue = currentAtTime;
    return true;
  }

  public CodingStringValue Current => this.currentValue;

  public CodingStringValue GetCurrentAtTime(DateTime time)
  {
    CodingStringValue currentAtTime = (CodingStringValue) null;
    foreach (CodingStringValue codingStringValue in this)
    {
      if (codingStringValue.Time <= time)
        currentAtTime = codingStringValue;
      else if (codingStringValue.Time > time)
        break;
    }
    return currentAtTime;
  }

  public new IEnumerator<CodingStringValue> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<CodingStringValue>) new List<CodingStringValue>((IEnumerable<CodingStringValue>) this.Items).GetEnumerator();
  }

  IEnumerator<CodingStringValue> IEnumerable<CodingStringValue>.GetEnumerator()
  {
    return this.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
