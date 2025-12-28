using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public abstract class LateLoadReadOnlyCollection<T1> : ReadOnlyCollection<T1>
{
	private delegate void AcquireListHandler();

	private class LateLoadList<T2> : IList<T2>, ICollection<T2>, IEnumerable<T2>, IEnumerable
	{
		private object acquiredListLock = new object();

		private List<T2> data;

		private AcquireListHandler AcquireList;

		public T2 this[int index]
		{
			get
			{
				lock (acquiredListLock)
				{
					EnsureAcquired();
					return data[index];
				}
			}
			set
			{
				lock (acquiredListLock)
				{
					EnsureAcquired();
					data[index] = value;
				}
			}
		}

		public int Count
		{
			get
			{
				lock (acquiredListLock)
				{
					EnsureAcquired();
					return data.Count;
				}
			}
		}

		public bool IsReadOnly => false;

		private bool Acquired { get; set; }

		internal bool IsAcquired
		{
			get
			{
				lock (acquiredListLock)
				{
					return Acquired;
				}
			}
		}

		public LateLoadList()
		{
			data = new List<T2>();
		}

		internal void SetAcquireListHandler(AcquireListHandler handler)
		{
			AcquireList = handler;
		}

		public int IndexOf(T2 item)
		{
			lock (acquiredListLock)
			{
				EnsureAcquired();
				return data.IndexOf(item);
			}
		}

		public void Insert(int index, T2 item)
		{
			lock (acquiredListLock)
			{
				EnsureAcquired();
				data.Insert(index, item);
			}
		}

		public void RemoveAt(int index)
		{
			lock (acquiredListLock)
			{
				EnsureAcquired();
				data.RemoveAt(index);
			}
		}

		public void Add(T2 item)
		{
			lock (acquiredListLock)
			{
				EnsureAcquired();
				data.Add(item);
			}
		}

		public void Clear()
		{
			data.Clear();
		}

		public bool Contains(T2 item)
		{
			lock (acquiredListLock)
			{
				EnsureAcquired();
				return data.Contains(item);
			}
		}

		public void CopyTo(T2[] array, int arrayIndex)
		{
			lock (acquiredListLock)
			{
				EnsureAcquired();
				data.CopyTo(array, arrayIndex);
			}
		}

		public bool Remove(T2 item)
		{
			return data.Remove(item);
		}

		public IEnumerator<T2> GetEnumerator()
		{
			lock (acquiredListLock)
			{
				EnsureAcquired();
				return data.GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (acquiredListLock)
			{
				EnsureAcquired();
				return data.GetEnumerator();
			}
		}

		private void EnsureAcquired()
		{
			if (!Acquired)
			{
				Acquired = true;
				AcquireList();
			}
		}

		internal void ResetList()
		{
			lock (acquiredListLock)
			{
				Acquired = false;
			}
		}
	}

	internal bool Acquired => (base.Items as LateLoadList<T1>).IsAcquired;

	internal LateLoadReadOnlyCollection()
		: base((IList<T1>)new LateLoadList<T1>())
	{
		(base.Items as LateLoadList<T1>).SetAcquireListHandler(AcquireList);
	}

	protected abstract void AcquireList();

	internal void ResetList()
	{
		(base.Items as LateLoadList<T1>).ResetList();
	}

	protected Dictionary<string, T1> ToDictionary(Func<T1, string> func, IEqualityComparer<string> comparer)
	{
		try
		{
			return Enumerable.ToDictionary(this, func, comparer);
		}
		catch (ArgumentException)
		{
			Dictionary<string, T1> dictionary = new Dictionary<string, T1>(comparer);
			using (IEnumerator<T1> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T1 current = enumerator.Current;
					string text = func(current);
					if (dictionary.Keys.Contains(text, comparer))
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(this, "WARNING: Cache for '" + typeof(T1).Name + "' will have ambiguous lookup for the non-unique key " + text);
					}
					else
					{
						dictionary.Add(text, current);
					}
				}
			}
			return dictionary;
		}
	}
}
