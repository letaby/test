// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ParameterGroupCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ParameterGroupCollection : LateLoadReadOnlyCollection<ParameterGroup>
{
  private Channel channel;

  internal ParameterGroupCollection(Channel parent) => this.channel = parent;

  protected override void AcquireList()
  {
    foreach (IGrouping<string, Parameter> source in this.channel.Parameters.GroupBy<Parameter, string>((Func<Parameter, string>) (p => p.GroupQualifier)))
      this.Items.Add(new ParameterGroup(source.Key, source.ToList<Parameter>()));
  }

  public ParameterGroup this[string groupQualifier]
  {
    get
    {
      return this.FirstOrDefault<ParameterGroup>((Func<ParameterGroup, bool>) (pg => pg.Qualifier == groupQualifier));
    }
  }
}
