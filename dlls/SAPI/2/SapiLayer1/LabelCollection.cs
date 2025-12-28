using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class LabelCollection : ReadOnlyCollection<Label>, IEnumerable<Label>, IEnumerable
{
	private LogFile parent;

	public Label this[string name] => this.FirstOrDefault((Label item) => string.Equals(item.Name, name, StringComparison.Ordinal));

	public Label Current
	{
		get
		{
			if (parent != null)
			{
				Label result = null;
				using IEnumerator<Label> enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					Label current = enumerator.Current;
					if (current.Time > parent.CurrentTime)
					{
						return result;
					}
					result = current;
				}
				return result;
			}
			return base[base.Count - 1];
		}
	}

	internal LabelCollection(LogFile parent)
		: base((IList<Label>)new List<Label>())
	{
		this.parent = parent;
	}

	internal LabelCollection()
		: base((IList<Label>)new List<Label>())
	{
	}

	internal void Add(Label label)
	{
		lock (base.Items)
		{
			base.Items.Add(label);
		}
	}

	public new IEnumerator<Label> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<Label>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<Label> IEnumerable<Label>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
