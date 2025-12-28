using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SapiLayer1;

public sealed class FaultCodeIncidentCollection : ReadOnlyCollection<FaultCodeIncident>, IEnumerable<FaultCodeIncident>, IEnumerable
{
	private DateTime startTime;

	private DateTime endTime;

	private FaultCodeIncident current;

	private object currentLock = new object();

	private ReadFunctions defaultReadFunction;

	public FaultCode FaultCode { get; private set; }

	public FaultCodeIncident Current => GetCurrentByFunction(defaultReadFunction);

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
	public object SyncRoot => base.Items;

	public event EventHandler FaultCodeIncidentUpdateEvent;

	internal FaultCodeIncidentCollection(FaultCode parent, ReadFunctions defaultReadFunction)
		: base((IList<FaultCodeIncident>)new List<FaultCodeIncident>())
	{
		this.defaultReadFunction = defaultReadFunction;
		FaultCode = parent;
	}

	internal bool SetCurrentTime(DateTime time)
	{
		bool result = false;
		FaultCodeIncident faultCodeIncident = InternalCurrentAtTime(time);
		lock (currentLock)
		{
			if (faultCodeIncident != current)
			{
				current = faultCodeIncident;
				result = true;
				FireAndForget.Invoke(this.FaultCodeIncidentUpdateEvent, this, new EventArgs());
			}
		}
		return result;
	}

	internal void Invalidate()
	{
		lock (currentLock)
		{
			current = null;
			FireAndForget.Invoke(this.FaultCodeIncidentUpdateEvent, this, new EventArgs());
		}
	}

	internal void Add(FaultCodeIncident incident)
	{
		lock (base.Items)
		{
			base.Items.Add(incident);
		}
	}

	internal void Add(FaultCodeIncident incident, bool readEnvironmentIfNew)
	{
		if (base.Count == 0)
		{
			startTime = incident.StartTime;
			endTime = incident.EndTime;
		}
		else
		{
			if (incident.StartTime < startTime)
			{
				startTime = incident.StartTime;
			}
			if (incident.EndTime > endTime)
			{
				endTime = incident.EndTime;
			}
		}
		bool flag = false;
		lock (currentLock)
		{
			if (incident.IsEquivalent(current))
			{
				current.UpdateEndTime(incident.EndTime);
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			Add(incident);
			if (readEnvironmentIfNew)
			{
				incident.InternalReadEnvironmentData();
			}
		}
	}

	private void AddFromRollCall(FaultCodeIncident incident)
	{
		Add(incident, readEnvironmentIfNew: false);
		lock (currentLock)
		{
			if (current != null && current.IsStatusClarifiedBy(incident))
			{
				incident.UpdateStartTime(current.StartTime);
				lock (base.Items)
				{
					base.Items.Remove(current);
					return;
				}
			}
		}
	}

	internal bool AddFromRollCall(DateTime thisTimeRead, byte? occurrenceCount, Type type, bool permanent)
	{
		bool result = false;
		FaultCodeIncident currentByFunction = GetCurrentByFunction(ReadFunctions.NonPermanent | ReadFunctions.Permanent);
		ActiveStatus activeStatus = ((type == typeof(ActiveStatus)) ? ActiveStatus.Active : (currentByFunction?.Active ?? ActiveStatus.NotActive));
		StoredStatus storedStatus = ((type == typeof(StoredStatus)) ? StoredStatus.Stored : (currentByFunction?.Stored ?? StoredStatus.Undefined));
		PendingStatus pendingStatus = ((type == typeof(PendingStatus)) ? PendingStatus.Pending : (currentByFunction?.Pending ?? PendingStatus.Undefined));
		MilStatus milStatus = ((type == typeof(MilStatus)) ? MilStatus.On : (currentByFunction?.Mil ?? MilStatus.Undefined));
		TestFailedSinceLastClearStatus testFailedSinceLastClearStatus = ((type == typeof(TestFailedSinceLastClearStatus)) ? TestFailedSinceLastClearStatus.TestFailedSinceLastClear : (currentByFunction?.TestFailedSinceLastClear ?? TestFailedSinceLastClearStatus.Undefined));
		ImmediateStatus immediateStatus = ((type == typeof(ImmediateStatus)) ? ImmediateStatus.Immediate : (currentByFunction?.Immediate ?? ImmediateStatus.Undefined));
		ReadFunctions readFunctions = ((!permanent) ? ReadFunctions.NonPermanent : ReadFunctions.Permanent);
		if (currentByFunction != null)
		{
			readFunctions |= currentByFunction.Functions;
		}
		FaultCodeIncident incident = new FaultCodeIncident(FaultCode, thisTimeRead, activeStatus, storedStatus, pendingStatus, milStatus, testFailedSinceLastClearStatus, immediateStatus, readFunctions);
		AddFromRollCall(incident);
		if (SetCurrentTime(thisTimeRead))
		{
			result = true;
			lock (currentLock)
			{
				if (current != null)
				{
					current.AcquireEnvironmentDataFromRollCall(occurrenceCount);
				}
			}
		}
		return result;
	}

	internal bool RemoveAgedFromRollCall(DateTime thisTimeRead, Type type, bool permanent, TimeSpan agedPersistFor)
	{
		bool result = false;
		FaultCodeIncident faultCodeIncident = GetCurrentByFunction(ReadFunctions.NonPermanent | ReadFunctions.Permanent);
		if (faultCodeIncident != null && agedPersistFor != TimeSpan.Zero && faultCodeIncident.EndTime > thisTimeRead - agedPersistFor)
		{
			faultCodeIncident = null;
		}
		if (faultCodeIncident != null && ((type == typeof(ActiveStatus) && faultCodeIncident.Active != ActiveStatus.NotActive) || (type == typeof(StoredStatus) && faultCodeIncident.Stored != StoredStatus.NotStored) || (type == typeof(PendingStatus) && faultCodeIncident.Pending != PendingStatus.NotPending) || (type == typeof(MilStatus) && faultCodeIncident.Mil != MilStatus.Off) || (type == typeof(ImmediateStatus) && faultCodeIncident.Immediate != ImmediateStatus.NotImmediate) || (type == typeof(TestFailedSinceLastClearStatus) && faultCodeIncident.TestFailedSinceLastClear != TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear) || (permanent && (faultCodeIncident.Functions & ReadFunctions.Permanent) != ReadFunctions.None)))
		{
			ActiveStatus activeStatus = ((!(type == typeof(ActiveStatus))) ? faultCodeIncident.Active : ActiveStatus.NotActive);
			StoredStatus storedStatus = ((!(type == typeof(StoredStatus))) ? faultCodeIncident.Stored : StoredStatus.NotStored);
			PendingStatus pendingStatus = ((!(type == typeof(PendingStatus))) ? faultCodeIncident.Pending : PendingStatus.NotPending);
			MilStatus milStatus = ((!(type == typeof(MilStatus))) ? faultCodeIncident.Mil : MilStatus.Off);
			ImmediateStatus immediateStatus = ((!(type == typeof(ImmediateStatus))) ? faultCodeIncident.Immediate : ImmediateStatus.NotImmediate);
			TestFailedSinceLastClearStatus testFailedSinceLastClearStatus = ((!(type == typeof(TestFailedSinceLastClearStatus))) ? faultCodeIncident.TestFailedSinceLastClear : TestFailedSinceLastClearStatus.TestNotFailedSinceLastClear);
			ReadFunctions readFunctions = (permanent ? (faultCodeIncident.Functions & ~ReadFunctions.Permanent) : ((activeStatus != ActiveStatus.Active && storedStatus != StoredStatus.Stored && pendingStatus != PendingStatus.Pending && milStatus != MilStatus.On && immediateStatus != ImmediateStatus.Immediate && testFailedSinceLastClearStatus != TestFailedSinceLastClearStatus.TestFailedSinceLastClear) ? (faultCodeIncident.Functions & ~ReadFunctions.NonPermanent) : faultCodeIncident.Functions));
			if (activeStatus == ActiveStatus.Active || storedStatus == StoredStatus.Stored || pendingStatus == PendingStatus.Pending || milStatus == MilStatus.On || immediateStatus == ImmediateStatus.Immediate || testFailedSinceLastClearStatus == TestFailedSinceLastClearStatus.TestFailedSinceLastClear || (readFunctions & ReadFunctions.Permanent) != ReadFunctions.None)
			{
				FaultCodeIncident faultCodeIncident2 = new FaultCodeIncident(FaultCode, thisTimeRead, activeStatus, storedStatus, pendingStatus, milStatus, testFailedSinceLastClearStatus, immediateStatus, readFunctions);
				foreach (EnvironmentData environmentData in faultCodeIncident.EnvironmentDatas)
				{
					faultCodeIncident2.EnvironmentDatas.Add(environmentData);
				}
				AddFromRollCall(faultCodeIncident2);
			}
			if (SetCurrentTime(thisTimeRead))
			{
				result = true;
			}
		}
		return result;
	}

	public FaultCodeIncident GetCurrentAtTime(ReadFunctions match, DateTime time)
	{
		FaultCodeIncident faultCodeIncident = InternalCurrentAtTime(time);
		if (faultCodeIncident != null && (faultCodeIncident.Functions & match) != ReadFunctions.None)
		{
			return faultCodeIncident;
		}
		return null;
	}

	public FaultCodeIncident GetCurrentByFunction(ReadFunctions match)
	{
		lock (currentLock)
		{
			if (current != null && (current.Functions & match) != ReadFunctions.None)
			{
				return current;
			}
		}
		return null;
	}

	public FaultCodeIncident GetCurrentAtTime(DateTime time)
	{
		return GetCurrentAtTime(defaultReadFunction, time);
	}

	public new IEnumerator<FaultCodeIncident> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<FaultCodeIncident>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<FaultCodeIncident> IEnumerable<FaultCodeIncident>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private FaultCodeIncident InternalCurrentAtTime(DateTime time)
	{
		return this.Where((FaultCodeIncident incident) => incident.StartTime <= time && incident.EndTime >= time).FirstOrDefault();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_CurrentAtTime is deprecated, please use GetCurrent(ReadFunctions, DateTime) instead.")]
	public FaultCodeIncident get_CurrentAtTime(ReadFunctions match, DateTime time)
	{
		return GetCurrentAtTime(match, time);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_Current is deprecated, please use GetCurrentByFunction(ReadFunctions) instead.")]
	public FaultCodeIncident get_Current(ReadFunctions match)
	{
		return GetCurrentByFunction(match);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
	public FaultCodeIncident get_CurrentAtTime(DateTime time)
	{
		return GetCurrentAtTime(time);
	}
}
