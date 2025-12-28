using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SapiLayer1;

public sealed class CodingStringValueCollection : ReadOnlyCollection<CodingStringValue>, IEnumerable<CodingStringValue>, IEnumerable
{
	private CodingStringValue currentValue;

	public CodingStringValue Current => currentValue;

	internal CodingStringValueCollection()
		: base((IList<CodingStringValue>)new List<CodingStringValue>())
	{
	}

	internal void Add(CodingStringValue codingStringValue, bool setCurrent = true)
	{
		if (setCurrent)
		{
			currentValue = codingStringValue;
		}
		lock (base.Items)
		{
			base.Items.Add(codingStringValue);
		}
	}

	internal bool SetCurrentTime(DateTime time)
	{
		CodingStringValue currentAtTime = GetCurrentAtTime(time);
		if (currentAtTime != currentValue)
		{
			currentValue = currentAtTime;
			return true;
		}
		return false;
	}

	public CodingStringValue GetCurrentAtTime(DateTime time)
	{
		CodingStringValue result = null;
		using (IEnumerator<CodingStringValue> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CodingStringValue current = enumerator.Current;
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

	public new IEnumerator<CodingStringValue> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<CodingStringValue>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<CodingStringValue> IEnumerable<CodingStringValue>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
