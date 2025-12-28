using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SapiLayer1;

public sealed class ServiceArgumentValueCollection : ReadOnlyCollection<ServiceArgumentValue>, IEnumerable<ServiceArgumentValue>, IEnumerable
{
	private ServiceArgumentValue currentValue;

	public ServiceArgumentValue Current => currentValue;

	public event EventHandler<ResultEventArgs> CurrentChanged;

	internal ServiceArgumentValueCollection()
		: base((IList<ServiceArgumentValue>)new List<ServiceArgumentValue>())
	{
	}

	internal ServiceArgumentValue Add(object value, DateTime time, bool fromLog, object parent, bool preProcessedValue = false)
	{
		ServiceArgumentValue serviceArgumentValue = new ServiceArgumentValue(value, time, parent, preProcessedValue);
		lock (base.Items)
		{
			base.Items.Add(serviceArgumentValue);
		}
		if (!fromLog)
		{
			currentValue = serviceArgumentValue;
			OnCurrentChanged(null);
		}
		return serviceArgumentValue;
	}

	internal void SetCurrentTime(DateTime time)
	{
		ServiceArgumentValue currentAtTime = GetCurrentAtTime(time);
		if (currentAtTime != currentValue)
		{
			currentValue = currentAtTime;
			OnCurrentChanged(null);
		}
	}

	public ServiceArgumentValue GetCurrentAtTime(DateTime time)
	{
		for (int i = 0; i < base.Count; i++)
		{
			if (base[i].Time <= time)
			{
				if (i >= base.Count - 1)
				{
					return base[i];
				}
				if (base[i + 1].Time > time)
				{
					return base[i];
				}
			}
		}
		return null;
	}

	public new IEnumerator<ServiceArgumentValue> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<ServiceArgumentValue>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<ServiceArgumentValue> IEnumerable<ServiceArgumentValue>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private void OnCurrentChanged(Exception e)
	{
		if (this.CurrentChanged != null)
		{
			FireAndForget.Invoke(this.CurrentChanged, this, new ResultEventArgs(e));
		}
	}
}
