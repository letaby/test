// Decompiled with JetBrains decompiler
// Type: SapiLayer1.InstrumentValueCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace SapiLayer1;

public sealed class InstrumentValueCollection : 
  ReadOnlyCollection<InstrumentValue>,
  IEnumerable<InstrumentValue>,
  IEnumerable
{
  private object currentLock = new object();
  private InstrumentValue current;
  private Instrument parent;
  private DateTime startTime;
  private DateTime endTime;
  private object observedMin;
  private object observedMax;
  private bool aging;

  public event EventHandler<EventArgs> ObservedMinMaxUpdateEvent;

  internal InstrumentValueCollection(Instrument parent)
    : base((IList<InstrumentValue>) new List<InstrumentValue>())
  {
    this.parent = parent;
  }

  internal void SetCurrentTime(DateTime time)
  {
    InstrumentValue currentAtTime = this.GetCurrentAtTime(time);
    lock (this.currentLock)
    {
      if (currentAtTime == this.Current)
        return;
      this.Current = currentAtTime;
      this.parent.RaiseInstrumentUpdateEvent((Exception) null, false);
    }
  }

  internal void Invalidate()
  {
    lock (this.currentLock)
    {
      if (this.Current == null)
        return;
      this.Current = (InstrumentValue) null;
      this.parent.RaiseInstrumentUpdateEvent((Exception) null, false);
    }
  }

  internal void Age() => this.aging = true;

  internal bool AddOrUpdate(object newValue, bool explicitRead)
  {
    bool flag = false;
    lock (this.currentLock)
    {
      if (((this.Current == null || this.aging ? 1 : (!object.Equals(newValue, this.Current.Value) ? 1 : 0)) | (explicitRead ? 1 : 0)) != 0)
      {
        this.Add(new InstrumentValue(newValue, Sapi.Now, this.Count, this.aging || this.Count == 0), true);
        flag = true;
        this.aging = false;
      }
      else
      {
        this.UpdateReadStatistics((Sapi.Now - this.Current.LastSampleTime).TotalMilliseconds, 1);
        this.Current.SetLastSampleTime(Sapi.Now);
        if (this.Current.LastSampleTime > this.endTime)
          this.endTime = this.Current.LastSampleTime;
      }
    }
    return flag;
  }

  internal void Add(InstrumentValue instrumentValue, bool setcurrent)
  {
    bool flag = false;
    if (instrumentValue != null)
    {
      if (this.Count == 0)
      {
        this.startTime = instrumentValue.FirstSampleTime;
        this.endTime = instrumentValue.LastSampleTime;
      }
      else
      {
        if (instrumentValue.FirstSampleTime < this.startTime)
          this.startTime = instrumentValue.FirstSampleTime;
        if (instrumentValue.LastSampleTime > this.endTime)
          this.endTime = instrumentValue.LastSampleTime;
      }
      TimeSpan timeSpan;
      if (!instrumentValue.ContiguousSegmentStart && this.Count > 0)
      {
        timeSpan = instrumentValue.FirstSampleTime - this.Items[this.Items.Count - 1].LastSampleTime;
        this.UpdateReadStatistics(timeSpan.TotalMilliseconds, 1);
      }
      if (instrumentValue.ItemSampleCount > 1)
      {
        timeSpan = instrumentValue.LastSampleTime - instrumentValue.FirstSampleTime;
        this.UpdateReadStatistics(timeSpan.TotalMilliseconds / (double) instrumentValue.ItemSampleCount, instrumentValue.ItemSampleCount);
      }
    }
    lock (this.Items)
      this.Items.Add(instrumentValue);
    if (setcurrent)
      this.Current = instrumentValue;
    if (instrumentValue != null && instrumentValue.Value != null)
    {
      double? nullable = new double?();
      if (instrumentValue.Value.GetType().IsPrimitive)
      {
        nullable = new double?(Convert.ToDouble(instrumentValue.Value, (IFormatProvider) CultureInfo.InvariantCulture));
        if (double.IsNaN(nullable.Value))
          return;
      }
      if (this.observedMin == null)
      {
        this.observedMin = instrumentValue.Value;
        this.observedMax = instrumentValue.Value;
        flag = true;
      }
      else if (instrumentValue.Value.GetType() != this.observedMin.GetType())
      {
        if (!this.observedMin.GetType().IsPrimitive && !(this.observedMin.GetType() == typeof (Choice)))
        {
          this.observedMin = instrumentValue.Value;
          this.observedMax = instrumentValue.Value;
          flag = true;
        }
      }
      else if (nullable.HasValue)
      {
        if (nullable.Value < Convert.ToDouble(this.observedMin, (IFormatProvider) CultureInfo.InvariantCulture))
        {
          this.observedMin = instrumentValue.Value;
          flag = true;
        }
        else if (nullable.Value > Convert.ToDouble(this.observedMax, (IFormatProvider) CultureInfo.InvariantCulture))
        {
          this.observedMax = instrumentValue.Value;
          flag = true;
        }
      }
      else if (instrumentValue.Value.GetType() == typeof (Choice))
      {
        Choice choice = (Choice) instrumentValue.Value;
        Choice observedMin = (Choice) this.observedMin;
        Choice observedMax = (Choice) this.observedMax;
        if (choice.Index < observedMin.Index)
        {
          this.observedMin = instrumentValue.Value;
          flag = true;
        }
        else if (choice.Index > observedMax.Index)
        {
          this.observedMax = instrumentValue.Value;
          flag = true;
        }
      }
      else
      {
        if (string.Compare(instrumentValue.ToString(), this.observedMin.ToString(), StringComparison.Ordinal) < 0)
        {
          this.observedMin = instrumentValue.Value;
          flag = true;
        }
        if (string.Compare(instrumentValue.ToString(), this.observedMax.ToString(), StringComparison.Ordinal) > 0)
        {
          this.observedMax = instrumentValue.Value;
          flag = true;
        }
      }
    }
    if (!flag)
      return;
    FireAndForget.Invoke((MulticastDelegate) this.ObservedMinMaxUpdateEvent, (object) this, new EventArgs());
  }

  public InstrumentValue GetCurrentAtTime(DateTime time)
  {
    if (this.Count == 0 || time < this.startTime || time > this.endTime)
      return (InstrumentValue) null;
    TimeSpan timeSpan1 = this.endTime - this.startTime;
    TimeSpan timeSpan2 = time - this.startTime;
    if (timeSpan1.TotalMilliseconds == 0.0)
      return this[0];
    int index = (int) Math.Floor((double) (this.Count - 1) * (timeSpan2.TotalMilliseconds / timeSpan1.TotalMilliseconds));
    InstrumentValue currentAtTime = this[index];
    if (currentAtTime.LastSampleTime < time)
    {
      while (++index < this.Count)
      {
        InstrumentValue instrumentValue = this[index];
        if (!(instrumentValue.FirstSampleTime > time))
          currentAtTime = instrumentValue;
        else
          break;
      }
    }
    else if (currentAtTime.FirstSampleTime > time)
    {
      while (--index >= 0)
      {
        currentAtTime = this[index];
        if (currentAtTime.FirstSampleTime <= time)
          break;
      }
    }
    return currentAtTime;
  }

  private void UpdateReadStatistics(double averageTime, int sampleCount)
  {
    if (this.AverageCycleTime.HasValue && this.TotalSampleCount.HasValue)
    {
      this.AverageCycleTime = new double?((this.AverageCycleTime.Value * (double) this.TotalSampleCount.Value + averageTime * (double) sampleCount) / (double) (this.TotalSampleCount.Value + sampleCount));
      this.TotalSampleCount = new int?(this.TotalSampleCount.Value + sampleCount);
    }
    else
    {
      this.AverageCycleTime = new double?(averageTime);
      this.TotalSampleCount = new int?(sampleCount);
    }
  }

  public double? AverageCycleTime { get; private set; }

  public int? TotalSampleCount { get; private set; }

  public InstrumentValue Current
  {
    get
    {
      lock (this.currentLock)
        return this.current;
    }
    private set
    {
      lock (this.currentLock)
        this.current = value;
    }
  }

  public object ObservedMin => this.observedMin;

  public object ObservedMax => this.observedMax;

  public new IEnumerator<InstrumentValue> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<InstrumentValue>) new List<InstrumentValue>((IEnumerable<InstrumentValue>) this.Items).GetEnumerator();
  }

  IEnumerator<InstrumentValue> IEnumerable<InstrumentValue>.GetEnumerator() => this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
  public object SyncRoot => (object) this.Items;

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
  public InstrumentValue get_CurrentAtTime(DateTime time) => this.GetCurrentAtTime(time);
}
