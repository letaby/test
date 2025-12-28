using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SapiLayer1;

public sealed class CommunicationsStateValueCollection : ReadOnlyCollection<CommunicationsStateValue>, IEnumerable<CommunicationsStateValue>, IEnumerable
{
	private Channel parent;

	private CommunicationsStateValue currentValue;

	public CommunicationsStateValue Current => currentValue;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
	public object SyncRoot => base.Items;

	internal CommunicationsStateValueCollection(Channel parent)
		: base((IList<CommunicationsStateValue>)new List<CommunicationsStateValue>())
	{
		this.parent = parent;
	}

	internal void Add(CommunicationsStateValue newValue, bool setcurrent)
	{
		if (setcurrent)
		{
			currentValue = newValue;
		}
		lock (base.Items)
		{
			base.Items.Add(newValue);
		}
	}

	internal void SetCurrentTime(DateTime time)
	{
		CommunicationsStateValue currentAtTime = GetCurrentAtTime(time);
		if (currentAtTime != currentValue)
		{
			currentValue = currentAtTime;
			if (currentValue != null)
			{
				parent.RaiseCommunicationsStateValueUpdateEvent(currentValue);
			}
		}
	}

	public CommunicationsStateValue GetCurrentAtTime(DateTime time)
	{
		CommunicationsStateValue communicationsStateValue = null;
		int num = 0;
		int num2 = 0;
		int num3 = base.Count - 1;
		while (num <= num3 && communicationsStateValue == null)
		{
			num2 = (num + num3) / 2;
			switch (WithinTime(num2, time))
			{
			case 1:
				num = num2 + 1;
				break;
			case -1:
				num3 = num2 - 1;
				break;
			case 0:
				communicationsStateValue = base[num2];
				break;
			}
		}
		if (communicationsStateValue != null && parent.LogFile != null)
		{
			Session session = parent.Sessions.ActiveAtTime(time);
			if (session == null || communicationsStateValue.Time < session.StartTime)
			{
				communicationsStateValue = null;
			}
		}
		return communicationsStateValue;
	}

	public new IEnumerator<CommunicationsStateValue> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<CommunicationsStateValue>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<CommunicationsStateValue> IEnumerable<CommunicationsStateValue>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private int WithinTime(int index, DateTime time)
	{
		CommunicationsStateValue communicationsStateValue = base[index];
		if (time < communicationsStateValue.Time)
		{
			return -1;
		}
		if (index < base.Count - 1)
		{
			CommunicationsStateValue communicationsStateValue2 = base[index + 1];
			if (time >= communicationsStateValue2.Time)
			{
				return 1;
			}
		}
		return 0;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_CurrentAtTime is deprecated, please use GetCurrentAtTime(DateTime) instead.")]
	public CommunicationsStateValue get_CurrentAtTime(DateTime time)
	{
		return GetCurrentAtTime(time);
	}
}
