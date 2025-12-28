// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CommunicationsStateValueCollection
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

public sealed class CommunicationsStateValueCollection : 
  ReadOnlyCollection<CommunicationsStateValue>,
  IEnumerable<CommunicationsStateValue>,
  IEnumerable
{
  private Channel parent;
  private CommunicationsStateValue currentValue;

  internal CommunicationsStateValueCollection(Channel parent)
    : base((IList<CommunicationsStateValue>) new List<CommunicationsStateValue>())
  {
    this.parent = parent;
  }

  internal void Add(CommunicationsStateValue newValue, bool setcurrent)
  {
    if (setcurrent)
      this.currentValue = newValue;
    lock (this.Items)
      this.Items.Add(newValue);
  }

  internal void SetCurrentTime(DateTime time)
  {
    CommunicationsStateValue currentAtTime = this.GetCurrentAtTime(time);
    if (currentAtTime == this.currentValue)
      return;
    this.currentValue = currentAtTime;
    if (this.currentValue == null)
      return;
    this.parent.RaiseCommunicationsStateValueUpdateEvent(this.currentValue);
  }

  public CommunicationsStateValue Current => this.currentValue;

  public CommunicationsStateValue GetCurrentAtTime(DateTime time)
  {
    CommunicationsStateValue currentAtTime = (CommunicationsStateValue) null;
    int num1 = 0;
    int num2 = this.Count - 1;
    while (num1 <= num2 && currentAtTime == null)
    {
      int index = (num1 + num2) / 2;
      switch (this.WithinTime(index, time))
      {
        case -1:
          num2 = index - 1;
          continue;
        case 0:
          currentAtTime = this[index];
          continue;
        case 1:
          num1 = index + 1;
          continue;
        default:
          continue;
      }
    }
    if (currentAtTime != null && this.parent.LogFile != null)
    {
      Session session = this.parent.Sessions.ActiveAtTime(time);
      if (session == (Session) null || currentAtTime.Time < session.StartTime)
        currentAtTime = (CommunicationsStateValue) null;
    }
    return currentAtTime;
  }

  public new IEnumerator<CommunicationsStateValue> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<CommunicationsStateValue>) new List<CommunicationsStateValue>((IEnumerable<CommunicationsStateValue>) this.Items).GetEnumerator();
  }

  IEnumerator<CommunicationsStateValue> IEnumerable<CommunicationsStateValue>.GetEnumerator()
  {
    return this.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  private int WithinTime(int index, DateTime time)
  {
    CommunicationsStateValue communicationsStateValue1 = this[index];
    if (time < communicationsStateValue1.Time)
      return -1;
    if (index < this.Count - 1)
    {
      CommunicationsStateValue communicationsStateValue2 = this[index + 1];
      if (time >= communicationsStateValue2.Time)
        return 1;
    }
    return 0;
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
  public CommunicationsStateValue get_CurrentAtTime(DateTime time) => this.GetCurrentAtTime(time);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
  public object SyncRoot => (object) this.Items;
}
