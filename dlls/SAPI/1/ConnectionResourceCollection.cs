// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ConnectionResourceCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ConnectionResourceCollection : ReadOnlyCollection<ConnectionResource>
{
  internal ConnectionResourceCollection()
    : base((IList<ConnectionResource>) new List<ConnectionResource>())
  {
  }

  internal void Add(ConnectionResource connectionResource) => this.Items.Add(connectionResource);

  public ConnectionResource GetResource(string type, int portIndex)
  {
    return this.Where<ConnectionResource>((Func<ConnectionResource, bool>) (resource => string.Equals(resource.Type, type, StringComparison.Ordinal) && resource.PortIndex == portIndex)).FirstOrDefault<ConnectionResource>();
  }

  public ConnectionResource this[string identifier]
  {
    get
    {
      return this.FirstOrDefault<ConnectionResource>((Func<ConnectionResource, bool>) (resource => string.Equals(resource.Type, identifier, StringComparison.Ordinal) || resource.Equals((object) identifier)));
    }
  }

  public ConnectionResource GetEquivalent(ConnectionResource other)
  {
    return other == null ? (ConnectionResource) null : this.FirstOrDefault<ConnectionResource>((Func<ConnectionResource, bool>) (resource => resource.IsEquivalent(other)));
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_Equivalent is deprecated, please use GetEquivalent(ConnectionResource) instead.")]
  public ConnectionResource get_Equivalent(ConnectionResource other) => this.GetEquivalent(other);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_Resource is deprecated, please use GetResource(string, int) instead.")]
  public ConnectionResource get_Resource(string type, int portIndex)
  {
    return this.GetResource(type, portIndex);
  }
}
