using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SapiLayer1;

public sealed class ParameterValueCollection : ReadOnlyCollection<ParameterValue>, IEnumerable<ParameterValue>, IEnumerable
{
	private Parameter parent;

	private ParameterValue currentValue;

	public ParameterValue Current => currentValue;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
	public object SyncRoot => base.Items;

	internal ParameterValueCollection(Parameter parent)
		: base((IList<ParameterValue>)new List<ParameterValue>())
	{
		this.parent = parent;
	}

	internal void Add(ParameterValue parameterValue, bool setCurrent = true)
	{
		if (setCurrent)
		{
			currentValue = parameterValue;
		}
		lock (base.Items)
		{
			base.Items.Add(parameterValue);
		}
	}

	internal bool SetCurrentTime(DateTime time)
	{
		ParameterValue currentAtTime = GetCurrentAtTime(time);
		if (currentAtTime != currentValue)
		{
			currentValue = currentAtTime;
			parent.InternalSetValueFromLogFile(currentValue);
			return true;
		}
		return false;
	}

	public ParameterValue GetCurrentAtTime(DateTime time)
	{
		ParameterValue result = null;
		using (IEnumerator<ParameterValue> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ParameterValue current = enumerator.Current;
				if (current.Time <= time)
				{
					result = current;
				}
				else if (current.Time > time)
				{
					break;
				}
			}
		}
		return result;
	}

	public new IEnumerator<ParameterValue> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<ParameterValue>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<ParameterValue> IEnumerable<ParameterValue>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
	public ParameterValue get_CurrentAtTime(DateTime time)
	{
		return GetCurrentAtTime(time);
	}
}
