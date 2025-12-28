// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FaultCodeIncidentCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class FaultCodeIncidentCollection : 
  ReadOnlyCollection<FaultCodeIncident>,
  IEnumerable<FaultCodeIncident>,
  IEnumerable
{
  private DateTime startTime;
  private DateTime endTime;
  private FaultCodeIncident current;
  private object currentLock = new object();
  private ReadFunctions defaultReadFunction;

  public event EventHandler FaultCodeIncidentUpdateEvent;

  internal FaultCodeIncidentCollection(FaultCode parent, ReadFunctions defaultReadFunction)
    : base((IList<FaultCodeIncident>) new List<FaultCodeIncident>())
  {
    this.defaultReadFunction = defaultReadFunction;
    this.FaultCode = parent;
  }

  internal bool SetCurrentTime(DateTime time)
  {
    bool flag = false;
    FaultCodeIncident faultCodeIncident = this.InternalCurrentAtTime(time);
    lock (this.currentLock)
    {
      if (faultCodeIncident != this.current)
      {
        this.current = faultCodeIncident;
        flag = true;
        FireAndForget.Invoke((MulticastDelegate) this.FaultCodeIncidentUpdateEvent, (object) this, new EventArgs());
      }
    }
    return flag;
  }

  internal void Invalidate()
  {
    lock (this.currentLock)
    {
      this.current = (FaultCodeIncident) null;
      FireAndForget.Invoke((MulticastDelegate) this.FaultCodeIncidentUpdateEvent, (object) this, new EventArgs());
    }
  }

  internal void Add(FaultCodeIncident incident)
  {
    lock (this.Items)
      this.Items.Add(incident);
  }

  internal void Add(FaultCodeIncident incident, bool readEnvironmentIfNew)
  {
    if (this.Count == 0)
    {
      this.startTime = incident.StartTime;
      this.endTime = incident.EndTime;
    }
    else
    {
      if (incident.StartTime < this.startTime)
        this.startTime = incident.StartTime;
      if (incident.EndTime > this.endTime)
        this.endTime = incident.EndTime;
    }
    bool flag = false;
    lock (this.currentLock)
    {
      if (incident.IsEquivalent(this.current))
        this.current.UpdateEndTime(incident.EndTime);
      else
        flag = true;
    }
    if (!flag)
      return;
    this.Add(incident);
    if (!readEnvironmentIfNew)
      return;
    incident.InternalReadEnvironmentData();
  }

  private void AddFromRollCall(FaultCodeIncident incident)
  {
    this.Add(incident, false);
    lock (this.currentLock)
    {
      if (this.current == null || !this.current.IsStatusClarifiedBy(incident))
        return;
      incident.UpdateStartTime(this.current.StartTime);
      lock (this.Items)
        this.Items.Remove(this.current);
    }
  }

  internal bool AddFromRollCall(
    DateTime thisTimeRead,
    byte? occurrenceCount,
    Type type,
    bool permanent)
  {
    bool flag = false;
    FaultCodeIncident currentByFunction = this.GetCurrentByFunction(ReadFunctions.NonPermanent | ReadFunctions.Permanent);
    ActiveStatus activeStatus = type == typeof (ActiveStatus) ? ActiveStatus.Active : (currentByFunction != null ? currentByFunction.Active : ActiveStatus.NotActive);
    StoredStatus storedStatus = type == typeof (StoredStatus) ? StoredStatus.Stored : (currentByFunction != null ? currentByFunction.Stored : StoredStatus.Undefined);
    PendingStatus pendingStatus = type == typeof (PendingStatus) ? PendingStatus.Pending : (currentByFunction != null ? currentByFunction.Pending : PendingStatus.Undefined);
    MilStatus milStatus = type == typeof (MilStatus) ? MilStatus.On : (currentByFunction != null ? currentByFunction.Mil : MilStatus.Undefined);
    TestFailedSinceLastClearStatus testFailedSinceLastClearStatus = type == typeof (TestFailedSinceLastClearStatus) ? TestFailedSinceLastClearStatus.TestFailedSinceLastClear : (currentByFunction != null ? currentByFunction.TestFailedSinceLastClear : TestFailedSinceLastClearStatus.Undefined);
    ImmediateStatus immediateStatus = type == typeof (ImmediateStatus) ? ImmediateStatus.Immediate : (currentByFunction != null ? currentByFunction.Immediate : ImmediateStatus.Undefined);
    ReadFunctions functions = permanent ? ReadFunctions.Permanent : ReadFunctions.NonPermanent;
    if (currentByFunction != null)
      functions |= currentByFunction.Functions;
    this.AddFromRollCall(new FaultCodeIncident(this.FaultCode, thisTimeRead, activeStatus, storedStatus, pendingStatus, milStatus, testFailedSinceLastClearStatus, immediateStatus, functions));
    if (this.SetCurrentTime(thisTimeRead))
    {
      flag = true;
      lock (this.currentLock)
      {
        if (this.current != null)
          this.current.AcquireEnvironmentDataFromRollCall(occurrenceCount);
      }
    }
    return flag;
  }

  internal bool RemoveAgedFromRollCall(
    DateTime thisTimeRead,
    Type type,
    bool permanent,
    TimeSpan agedPersistFor)
  {
    bool flag = false;
    FaultCodeIncident faultCodeIncident = this.GetCurrentByFunction(ReadFunctions.NonPermanent | ReadFunctions.Permanent);
    if (faultCodeIncident != null && agedPersistFor != TimeSpan.Zero && faultCodeIncident.EndTime > thisTimeRead - agedPersistFor)
      faultCodeIncident = (FaultCodeIncident) null;
    if (faultCodeIncident != null && (type == typeof (ActiveStatus) && faultCodeIncident.Active != ActiveStatus.NotActive || type == typeof (StoredStatus) && faultCodeIncident.Stored != StoredStatus.NotStored || type == typeof (PendingStatus) && faultCodeIncident.Pending != PendingStatus.NotPending || type == typeof (MilStatus) && faultCodeIncident.Mil != MilStatus.Off || type == typeof (ImmediateStatus) && faultCodeIncident.Immediate != ImmediateStatus.NotImmediate || type == typeof (TestFailedSinceLastClearStatus) && faultCodeIncident.TestFailedSinceLastClear != TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear || permanent && (faultCodeIncident.Functions & ReadFunctions.Permanent) != ReadFunctions.None))
    {
      ActiveStatus activeStatus = type == typeof (ActiveStatus) ? ActiveStatus.NotActive : faultCodeIncident.Active;
      StoredStatus storedStatus = type == typeof (StoredStatus) ? StoredStatus.NotStored : faultCodeIncident.Stored;
      PendingStatus pendingStatus = type == typeof (PendingStatus) ? PendingStatus.NotPending : faultCodeIncident.Pending;
      MilStatus milStatus = type == typeof (MilStatus) ? MilStatus.Off : faultCodeIncident.Mil;
      ImmediateStatus immediateStatus = type == typeof (ImmediateStatus) ? ImmediateStatus.NotImmediate : faultCodeIncident.Immediate;
      TestFailedSinceLastClearStatus testFailedSinceLastClearStatus = type == typeof (TestFailedSinceLastClearStatus) ? TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear : faultCodeIncident.TestFailedSinceLastClear;
      ReadFunctions functions = !permanent ? (activeStatus == ActiveStatus.Active || storedStatus == StoredStatus.Stored || pendingStatus == PendingStatus.Pending || milStatus == MilStatus.On || immediateStatus == ImmediateStatus.Immediate || testFailedSinceLastClearStatus == TestFailedSinceLastClearStatus.TestFailedSinceLastClear ? faultCodeIncident.Functions : faultCodeIncident.Functions & ~ReadFunctions.NonPermanent) : faultCodeIncident.Functions & ~ReadFunctions.Permanent;
      if (activeStatus == ActiveStatus.Active || storedStatus == StoredStatus.Stored || pendingStatus == PendingStatus.Pending || milStatus == MilStatus.On || immediateStatus == ImmediateStatus.Immediate || testFailedSinceLastClearStatus == TestFailedSinceLastClearStatus.TestFailedSinceLastClear || (functions & ReadFunctions.Permanent) != ReadFunctions.None)
      {
        FaultCodeIncident incident = new FaultCodeIncident(this.FaultCode, thisTimeRead, activeStatus, storedStatus, pendingStatus, milStatus, testFailedSinceLastClearStatus, immediateStatus, functions);
        foreach (EnvironmentData environmentData in (ReadOnlyCollection<EnvironmentData>) faultCodeIncident.EnvironmentDatas)
          incident.EnvironmentDatas.Add(environmentData);
        this.AddFromRollCall(incident);
      }
      if (this.SetCurrentTime(thisTimeRead))
        flag = true;
    }
    return flag;
  }

  public FaultCodeIncident GetCurrentAtTime(ReadFunctions match, DateTime time)
  {
    FaultCodeIncident faultCodeIncident = this.InternalCurrentAtTime(time);
    return faultCodeIncident != null && (faultCodeIncident.Functions & match) != ReadFunctions.None ? faultCodeIncident : (FaultCodeIncident) null;
  }

  public FaultCode FaultCode { private set; get; }

  public FaultCodeIncident Current => this.GetCurrentByFunction(this.defaultReadFunction);

  public FaultCodeIncident GetCurrentByFunction(ReadFunctions match)
  {
    lock (this.currentLock)
    {
      if (this.current != null)
      {
        if ((this.current.Functions & match) != ReadFunctions.None)
          return this.current;
      }
    }
    return (FaultCodeIncident) null;
  }

  public FaultCodeIncident GetCurrentAtTime(DateTime time)
  {
    return this.GetCurrentAtTime(this.defaultReadFunction, time);
  }

  public new IEnumerator<FaultCodeIncident> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<FaultCodeIncident>) new List<FaultCodeIncident>((IEnumerable<FaultCodeIncident>) this.Items).GetEnumerator();
  }

  IEnumerator<FaultCodeIncident> IEnumerable<FaultCodeIncident>.GetEnumerator()
  {
    return this.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  private FaultCodeIncident InternalCurrentAtTime(DateTime time)
  {
    return this.Where<FaultCodeIncident>((Func<FaultCodeIncident, bool>) (incident => incident.StartTime <= time && incident.EndTime >= time)).FirstOrDefault<FaultCodeIncident>();
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_CurrentAtTime is deprecated, please use GetCurrent(ReadFunctions, DateTime) instead.")]
  public FaultCodeIncident get_CurrentAtTime(ReadFunctions match, DateTime time)
  {
    return this.GetCurrentAtTime(match, time);
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_Current is deprecated, please use GetCurrentByFunction(ReadFunctions) instead.")]
  public FaultCodeIncident get_Current(ReadFunctions match) => this.GetCurrentByFunction(match);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
  public FaultCodeIncident get_CurrentAtTime(DateTime time) => this.GetCurrentAtTime(time);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
  public object SyncRoot => (object) this.Items;
}
