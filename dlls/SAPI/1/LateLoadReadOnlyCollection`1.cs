// Decompiled with JetBrains decompiler
// Type: SapiLayer1.LateLoadReadOnlyCollection`1
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public abstract class LateLoadReadOnlyCollection<T1> : ReadOnlyCollection<T1>
{
  internal LateLoadReadOnlyCollection()
    : base((IList<T1>) new LateLoadReadOnlyCollection<T1>.LateLoadList<T1>())
  {
    (this.Items as LateLoadReadOnlyCollection<T1>.LateLoadList<T1>).SetAcquireListHandler(new LateLoadReadOnlyCollection<T1>.AcquireListHandler(this.AcquireList));
  }

  protected abstract void AcquireList();

  internal void ResetList()
  {
    (this.Items as LateLoadReadOnlyCollection<T1>.LateLoadList<T1>).ResetList();
  }

  internal bool Acquired
  {
    get => (this.Items as LateLoadReadOnlyCollection<T1>.LateLoadList<T1>).IsAcquired;
  }

  protected Dictionary<string, T1> ToDictionary(
    Func<T1, string> func,
    IEqualityComparer<string> comparer)
  {
    try
    {
      return this.ToDictionary<T1, string>(func, comparer);
    }
    catch (ArgumentException ex)
    {
      Dictionary<string, T1> dictionary = new Dictionary<string, T1>(comparer);
      foreach (T1 obj in (ReadOnlyCollection<T1>) this)
      {
        string key = func(obj);
        if (dictionary.Keys.Contains<string>(key, comparer))
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"WARNING: Cache for '{typeof (T1).Name}' will have ambiguous lookup for the non-unique key {key}");
        else
          dictionary.Add(key, obj);
      }
      return dictionary;
    }
  }

  private delegate void AcquireListHandler();

  private class LateLoadList<T2> : IList<T2>, ICollection<T2>, IEnumerable<T2>, IEnumerable
  {
    private object acquiredListLock = new object();
    private List<T2> data;
    private LateLoadReadOnlyCollection<T1>.AcquireListHandler AcquireList;

    public LateLoadList() => this.data = new List<T2>();

    internal void SetAcquireListHandler(
      LateLoadReadOnlyCollection<T1>.AcquireListHandler handler)
    {
      this.AcquireList = handler;
    }

    public int IndexOf(T2 item)
    {
      lock (this.acquiredListLock)
      {
        this.EnsureAcquired();
        return this.data.IndexOf(item);
      }
    }

    public void Insert(int index, T2 item)
    {
      lock (this.acquiredListLock)
      {
        this.EnsureAcquired();
        this.data.Insert(index, item);
      }
    }

    public void RemoveAt(int index)
    {
      lock (this.acquiredListLock)
      {
        this.EnsureAcquired();
        this.data.RemoveAt(index);
      }
    }

    public T2 this[int index]
    {
      get
      {
        lock (this.acquiredListLock)
        {
          this.EnsureAcquired();
          return this.data[index];
        }
      }
      set
      {
        lock (this.acquiredListLock)
        {
          this.EnsureAcquired();
          this.data[index] = value;
        }
      }
    }

    public void Add(T2 item)
    {
      lock (this.acquiredListLock)
      {
        this.EnsureAcquired();
        this.data.Add(item);
      }
    }

    public void Clear() => this.data.Clear();

    public bool Contains(T2 item)
    {
      lock (this.acquiredListLock)
      {
        this.EnsureAcquired();
        return this.data.Contains(item);
      }
    }

    public void CopyTo(T2[] array, int arrayIndex)
    {
      lock (this.acquiredListLock)
      {
        this.EnsureAcquired();
        this.data.CopyTo(array, arrayIndex);
      }
    }

    public int Count
    {
      get
      {
        lock (this.acquiredListLock)
        {
          this.EnsureAcquired();
          return this.data.Count;
        }
      }
    }

    public bool IsReadOnly => false;

    public bool Remove(T2 item) => this.data.Remove(item);

    public IEnumerator<T2> GetEnumerator()
    {
      lock (this.acquiredListLock)
      {
        this.EnsureAcquired();
        return (IEnumerator<T2>) this.data.GetEnumerator();
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      lock (this.acquiredListLock)
      {
        this.EnsureAcquired();
        return (IEnumerator) this.data.GetEnumerator();
      }
    }

    private void EnsureAcquired()
    {
      if (this.Acquired)
        return;
      this.Acquired = true;
      this.AcquireList();
    }

    internal void ResetList()
    {
      lock (this.acquiredListLock)
        this.Acquired = false;
    }

    private bool Acquired { set; get; }

    internal bool IsAcquired
    {
      get
      {
        lock (this.acquiredListLock)
          return this.Acquired;
      }
    }
  }
}
