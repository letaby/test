using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace SapiLayer1;

public sealed class InstrumentValueCollection : ReadOnlyCollection<InstrumentValue>, IEnumerable<InstrumentValue>, IEnumerable
{
	private object currentLock = new object();

	private InstrumentValue current;

	private Instrument parent;

	private DateTime startTime;

	private DateTime endTime;

	private object observedMin;

	private object observedMax;

	private bool aging;

	public double? AverageCycleTime { get; private set; }

	public int? TotalSampleCount { get; private set; }

	public InstrumentValue Current
	{
		get
		{
			lock (currentLock)
			{
				return current;
			}
		}
		private set
		{
			lock (currentLock)
			{
				current = value;
			}
		}
	}

	public object ObservedMin => observedMin;

	public object ObservedMax => observedMax;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
	public object SyncRoot => base.Items;

	public event EventHandler<EventArgs> ObservedMinMaxUpdateEvent;

	internal InstrumentValueCollection(Instrument parent)
		: base((IList<InstrumentValue>)new List<InstrumentValue>())
	{
		this.parent = parent;
	}

	internal void SetCurrentTime(DateTime time)
	{
		InstrumentValue currentAtTime = GetCurrentAtTime(time);
		lock (currentLock)
		{
			if (currentAtTime != Current)
			{
				Current = currentAtTime;
				parent.RaiseInstrumentUpdateEvent(null, explicitread: false);
			}
		}
	}

	internal void Invalidate()
	{
		lock (currentLock)
		{
			if (Current != null)
			{
				Current = null;
				parent.RaiseInstrumentUpdateEvent(null, explicitread: false);
			}
		}
	}

	internal void Age()
	{
		aging = true;
	}

	internal bool AddOrUpdate(object newValue, bool explicitRead)
	{
		bool result = false;
		lock (currentLock)
		{
			if (Current == null || aging || !object.Equals(newValue, Current.Value) || explicitRead)
			{
				Add(new InstrumentValue(newValue, Sapi.Now, base.Count, aging || base.Count == 0), setcurrent: true);
				result = true;
				aging = false;
			}
			else
			{
				UpdateReadStatistics((Sapi.Now - Current.LastSampleTime).TotalMilliseconds, 1);
				Current.SetLastSampleTime(Sapi.Now);
				if (Current.LastSampleTime > endTime)
				{
					endTime = Current.LastSampleTime;
				}
			}
		}
		return result;
	}

	internal void Add(InstrumentValue instrumentValue, bool setcurrent)
	{
		bool flag = false;
		if (instrumentValue != null)
		{
			if (base.Count == 0)
			{
				startTime = instrumentValue.FirstSampleTime;
				endTime = instrumentValue.LastSampleTime;
			}
			else
			{
				if (instrumentValue.FirstSampleTime < startTime)
				{
					startTime = instrumentValue.FirstSampleTime;
				}
				if (instrumentValue.LastSampleTime > endTime)
				{
					endTime = instrumentValue.LastSampleTime;
				}
			}
			if (!instrumentValue.ContiguousSegmentStart && base.Count > 0)
			{
				UpdateReadStatistics((instrumentValue.FirstSampleTime - base.Items[base.Items.Count - 1].LastSampleTime).TotalMilliseconds, 1);
			}
			if (instrumentValue.ItemSampleCount > 1)
			{
				UpdateReadStatistics((instrumentValue.LastSampleTime - instrumentValue.FirstSampleTime).TotalMilliseconds / (double)instrumentValue.ItemSampleCount, instrumentValue.ItemSampleCount);
			}
		}
		lock (base.Items)
		{
			base.Items.Add(instrumentValue);
		}
		if (setcurrent)
		{
			Current = instrumentValue;
		}
		if (instrumentValue != null && instrumentValue.Value != null)
		{
			double? num = null;
			if (instrumentValue.Value.GetType().IsPrimitive)
			{
				num = Convert.ToDouble(instrumentValue.Value, CultureInfo.InvariantCulture);
				if (double.IsNaN(num.Value))
				{
					return;
				}
			}
			if (observedMin == null)
			{
				observedMin = instrumentValue.Value;
				observedMax = instrumentValue.Value;
				flag = true;
			}
			else if (instrumentValue.Value.GetType() != observedMin.GetType())
			{
				if (!observedMin.GetType().IsPrimitive && !(observedMin.GetType() == typeof(Choice)))
				{
					observedMin = instrumentValue.Value;
					observedMax = instrumentValue.Value;
					flag = true;
				}
			}
			else if (num.HasValue)
			{
				if (num.Value < Convert.ToDouble(observedMin, CultureInfo.InvariantCulture))
				{
					observedMin = instrumentValue.Value;
					flag = true;
				}
				else if (num.Value > Convert.ToDouble(observedMax, CultureInfo.InvariantCulture))
				{
					observedMax = instrumentValue.Value;
					flag = true;
				}
			}
			else if (instrumentValue.Value.GetType() == typeof(Choice))
			{
				Choice choice = (Choice)instrumentValue.Value;
				Choice choice2 = (Choice)observedMin;
				Choice choice3 = (Choice)observedMax;
				if (choice.Index < choice2.Index)
				{
					observedMin = instrumentValue.Value;
					flag = true;
				}
				else if (choice.Index > choice3.Index)
				{
					observedMax = instrumentValue.Value;
					flag = true;
				}
			}
			else
			{
				if (string.Compare(instrumentValue.ToString(), observedMin.ToString(), StringComparison.Ordinal) < 0)
				{
					observedMin = instrumentValue.Value;
					flag = true;
				}
				if (string.Compare(instrumentValue.ToString(), observedMax.ToString(), StringComparison.Ordinal) > 0)
				{
					observedMax = instrumentValue.Value;
					flag = true;
				}
			}
		}
		if (flag)
		{
			FireAndForget.Invoke(this.ObservedMinMaxUpdateEvent, this, new EventArgs());
		}
	}

	public InstrumentValue GetCurrentAtTime(DateTime time)
	{
		if (base.Count == 0 || time < startTime || time > endTime)
		{
			return null;
		}
		TimeSpan timeSpan = endTime - startTime;
		TimeSpan timeSpan2 = time - startTime;
		if (timeSpan.TotalMilliseconds == 0.0)
		{
			return base[0];
		}
		double num = timeSpan2.TotalMilliseconds / timeSpan.TotalMilliseconds;
		int num2 = (int)Math.Floor((double)(base.Count - 1) * num);
		InstrumentValue instrumentValue = base[num2];
		if (instrumentValue.LastSampleTime < time)
		{
			while (++num2 < base.Count)
			{
				InstrumentValue instrumentValue2 = base[num2];
				if (instrumentValue2.FirstSampleTime > time)
				{
					break;
				}
				instrumentValue = instrumentValue2;
			}
		}
		else if (instrumentValue.FirstSampleTime > time)
		{
			while (--num2 >= 0)
			{
				instrumentValue = base[num2];
				if (instrumentValue.FirstSampleTime <= time)
				{
					break;
				}
			}
		}
		return instrumentValue;
	}

	private void UpdateReadStatistics(double averageTime, int sampleCount)
	{
		if (AverageCycleTime.HasValue && TotalSampleCount.HasValue)
		{
			AverageCycleTime = (AverageCycleTime.Value * (double)TotalSampleCount.Value + averageTime * (double)sampleCount) / (double)(TotalSampleCount.Value + sampleCount);
			TotalSampleCount = TotalSampleCount.Value + sampleCount;
		}
		else
		{
			AverageCycleTime = averageTime;
			TotalSampleCount = sampleCount;
		}
	}

	public new IEnumerator<InstrumentValue> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<InstrumentValue>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<InstrumentValue> IEnumerable<InstrumentValue>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
	public InstrumentValue get_CurrentAtTime(DateTime time)
	{
		return GetCurrentAtTime(time);
	}
}
