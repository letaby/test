using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SapiLayer1;

public sealed class ServiceExecutionCollection : ReadOnlyCollection<ServiceExecution>, IEnumerable<ServiceExecution>, IEnumerable
{
	private ServiceExecution currentValue;

	public ServiceExecution Current => currentValue;

	public event EventHandler<ResultEventArgs> CurrentChanged;

	internal ServiceExecutionCollection()
		: base((IList<ServiceExecution>)new List<ServiceExecution>())
	{
	}

	internal void Add(ServiceExecution serviceExecution, bool fromLog)
	{
		lock (base.Items)
		{
			base.Items.Add(serviceExecution);
		}
		if (!fromLog)
		{
			currentValue = serviceExecution;
			OnCurrentChanged(null);
		}
	}

	internal void SetCurrentTime(DateTime time)
	{
		ServiceExecution currentAtTime = GetCurrentAtTime(time);
		if (currentAtTime != currentValue)
		{
			currentValue = currentAtTime;
			OnCurrentChanged(null);
		}
	}

	public ServiceExecution GetCurrentAtTime(DateTime time)
	{
		ServiceExecution result = null;
		using (IEnumerator<ServiceExecution> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ServiceExecution current = enumerator.Current;
				if (current.StartTime <= time)
				{
					result = current;
				}
				else if (current.StartTime > time)
				{
					break;
				}
			}
		}
		return result;
	}

	public new IEnumerator<ServiceExecution> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<ServiceExecution>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<ServiceExecution> IEnumerable<ServiceExecution>.GetEnumerator()
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
