// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceArgumentValueCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace SapiLayer1;

public sealed class ServiceArgumentValueCollection : 
  ReadOnlyCollection<ServiceArgumentValue>,
  IEnumerable<ServiceArgumentValue>,
  IEnumerable
{
  private ServiceArgumentValue currentValue;

  internal ServiceArgumentValueCollection()
    : base((IList<ServiceArgumentValue>) new List<ServiceArgumentValue>())
  {
  }

  internal ServiceArgumentValue Add(
    object value,
    DateTime time,
    bool fromLog,
    object parent,
    bool preProcessedValue = false)
  {
    ServiceArgumentValue serviceArgumentValue = new ServiceArgumentValue(value, time, parent, preProcessedValue);
    lock (this.Items)
      this.Items.Add(serviceArgumentValue);
    if (!fromLog)
    {
      this.currentValue = serviceArgumentValue;
      this.OnCurrentChanged((Exception) null);
    }
    return serviceArgumentValue;
  }

  internal void SetCurrentTime(DateTime time)
  {
    ServiceArgumentValue currentAtTime = this.GetCurrentAtTime(time);
    if (currentAtTime == this.currentValue)
      return;
    this.currentValue = currentAtTime;
    this.OnCurrentChanged((Exception) null);
  }

  public ServiceArgumentValue Current => this.currentValue;

  public ServiceArgumentValue GetCurrentAtTime(DateTime time)
  {
    for (int index = 0; index < this.Count; ++index)
    {
      if (this[index].Time <= time)
      {
        if (index >= this.Count - 1)
          return this[index];
        if (this[index + 1].Time > time)
          return this[index];
      }
    }
    return (ServiceArgumentValue) null;
  }

  public new IEnumerator<ServiceArgumentValue> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<ServiceArgumentValue>) new List<ServiceArgumentValue>((IEnumerable<ServiceArgumentValue>) this.Items).GetEnumerator();
  }

  IEnumerator<ServiceArgumentValue> IEnumerable<ServiceArgumentValue>.GetEnumerator()
  {
    return this.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public event EventHandler<ResultEventArgs> CurrentChanged;

  private void OnCurrentChanged(Exception e)
  {
    if (this.CurrentChanged == null)
      return;
    FireAndForget.Invoke((MulticastDelegate) this.CurrentChanged, (object) this, (EventArgs) new ResultEventArgs(e));
  }
}
