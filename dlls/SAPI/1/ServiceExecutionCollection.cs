// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceExecutionCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace SapiLayer1;

public sealed class ServiceExecutionCollection : 
  ReadOnlyCollection<ServiceExecution>,
  IEnumerable<ServiceExecution>,
  IEnumerable
{
  private ServiceExecution currentValue;

  internal ServiceExecutionCollection()
    : base((IList<ServiceExecution>) new List<ServiceExecution>())
  {
  }

  internal void Add(ServiceExecution serviceExecution, bool fromLog)
  {
    lock (this.Items)
      this.Items.Add(serviceExecution);
    if (fromLog)
      return;
    this.currentValue = serviceExecution;
    this.OnCurrentChanged((Exception) null);
  }

  internal void SetCurrentTime(DateTime time)
  {
    ServiceExecution currentAtTime = this.GetCurrentAtTime(time);
    if (currentAtTime == this.currentValue)
      return;
    this.currentValue = currentAtTime;
    this.OnCurrentChanged((Exception) null);
  }

  public ServiceExecution Current => this.currentValue;

  public ServiceExecution GetCurrentAtTime(DateTime time)
  {
    ServiceExecution currentAtTime = (ServiceExecution) null;
    foreach (ServiceExecution serviceExecution in this)
    {
      if (serviceExecution.StartTime <= time)
        currentAtTime = serviceExecution;
      else if (serviceExecution.StartTime > time)
        break;
    }
    return currentAtTime;
  }

  public new IEnumerator<ServiceExecution> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<ServiceExecution>) new List<ServiceExecution>((IEnumerable<ServiceExecution>) this.Items).GetEnumerator();
  }

  IEnumerator<ServiceExecution> IEnumerable<ServiceExecution>.GetEnumerator()
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
