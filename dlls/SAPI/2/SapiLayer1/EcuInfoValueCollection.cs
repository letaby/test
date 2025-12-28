using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SapiLayer1;

public sealed class EcuInfoValueCollection : ReadOnlyCollection<EcuInfoValue>, IEnumerable<EcuInfoValue>, IEnumerable
{
	private EcuInfo parent;

	private EcuInfoValue currentValue;

	public EcuInfoValue Current => currentValue;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
	public object SyncRoot => base.Items;

	internal EcuInfoValueCollection(EcuInfo parent)
		: base((IList<EcuInfoValue>)new List<EcuInfoValue>())
	{
		this.parent = parent;
	}

	internal void Add(EcuInfoValue ecuInfoValue)
	{
		currentValue = ecuInfoValue;
		lock (base.Items)
		{
			base.Items.Add(ecuInfoValue);
		}
		if (parent.Channel.Ecu.RollCallManager != null)
		{
			parent.Channel.Ecu.RollCallManager.NotifyEcuInfoValue(parent, ecuInfoValue.Value);
		}
	}

	internal void SetCurrentTime(DateTime time)
	{
		EcuInfoValue currentAtTime = GetCurrentAtTime(time);
		if (currentAtTime != currentValue)
		{
			currentValue = currentAtTime;
			parent.RaiseEcuInfoUpdateEvent(null, explicitread: false);
		}
	}

	public EcuInfoValue GetCurrentAtTime(DateTime time)
	{
		EcuInfoValue result = null;
		using (IEnumerator<EcuInfoValue> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				EcuInfoValue current = enumerator.Current;
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

	public new IEnumerator<EcuInfoValue> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<EcuInfoValue>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<EcuInfoValue> IEnumerable<EcuInfoValue>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
	public EcuInfoValue get_CurrentAtTime(DateTime time)
	{
		return GetCurrentAtTime(time);
	}
}
