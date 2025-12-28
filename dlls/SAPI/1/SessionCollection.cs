// Decompiled with JetBrains decompiler
// Type: SapiLayer1.SessionCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class SessionCollection : ReadOnlyCollection<Session>
{
  internal SessionCollection()
    : base((IList<Session>) new List<Session>())
  {
  }

  internal int Add(Session session)
  {
    for (int index = 0; index < this.Items.Count; ++index)
    {
      if (this.Items[index] > session)
      {
        this.Items.Insert(index, session);
        return index;
      }
    }
    this.Items.Add(session);
    return this.Count - 1;
  }

  public DateTime StartTime => this.Count > 0 ? this[0].StartTime : DateTime.MinValue;

  public DateTime EndTime => this.Count > 0 ? this[this.Count - 1].EndTime : DateTime.MinValue;

  public Session ActiveAtTime(DateTime time)
  {
    return this.Where<Session>((Func<Session, bool>) (session => session.StartTime <= time && session.EndTime > time)).FirstOrDefault<Session>();
  }
}
