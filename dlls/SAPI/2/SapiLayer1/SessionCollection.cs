using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class SessionCollection : ReadOnlyCollection<Session>
{
	public DateTime StartTime
	{
		get
		{
			if (base.Count > 0)
			{
				return base[0].StartTime;
			}
			return DateTime.MinValue;
		}
	}

	public DateTime EndTime
	{
		get
		{
			if (base.Count > 0)
			{
				return base[base.Count - 1].EndTime;
			}
			return DateTime.MinValue;
		}
	}

	internal SessionCollection()
		: base((IList<Session>)new List<Session>())
	{
	}

	internal int Add(Session session)
	{
		for (int i = 0; i < base.Items.Count; i++)
		{
			if (base.Items[i] > session)
			{
				base.Items.Insert(i, session);
				return i;
			}
		}
		base.Items.Add(session);
		return base.Count - 1;
	}

	public Session ActiveAtTime(DateTime time)
	{
		return this.Where((Session session) => session.StartTime <= time && session.EndTime > time).FirstOrDefault();
	}
}
