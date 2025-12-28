// Decompiled with JetBrains decompiler
// Type: SapiLayer1.LabelCollection
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

public sealed class LabelCollection : ReadOnlyCollection<Label>, IEnumerable<Label>, IEnumerable
{
  private LogFile parent;

  internal LabelCollection(LogFile parent)
    : base((IList<Label>) new List<Label>())
  {
    this.parent = parent;
  }

  internal LabelCollection()
    : base((IList<Label>) new List<Label>())
  {
  }

  internal void Add(Label label)
  {
    lock (this.Items)
      this.Items.Add(label);
  }

  public Label this[string name]
  {
    get
    {
      return this.FirstOrDefault<Label>((Func<Label, bool>) (item => string.Equals(item.Name, name, StringComparison.Ordinal)));
    }
  }

  public Label Current
  {
    get
    {
      if (this.parent == null)
        return this[this.Count - 1];
      Label current = (Label) null;
      foreach (Label label in this)
      {
        if (label.Time > this.parent.CurrentTime)
          return current;
        current = label;
      }
      return current;
    }
  }

  public new IEnumerator<Label> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<Label>) new List<Label>((IEnumerable<Label>) this.Items).GetEnumerator();
  }

  IEnumerator<Label> IEnumerable<Label>.GetEnumerator() => this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
