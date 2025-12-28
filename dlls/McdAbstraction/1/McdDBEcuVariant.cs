// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBEcuVariant
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBEcuVariant
{
  private readonly MCDDbEcuVariant variant;
  private Dictionary<string, McdDBLocation> dblocations = new Dictionary<string, McdDBLocation>();

  internal McdDBEcuVariant(MCDDbEcuVariant variant)
  {
    this.variant = variant;
    this.Name = this.variant.LongName;
    this.Description = this.variant.Description;
  }

  public string Name { private set; get; }

  public string Description { private set; get; }

  public McdDBLocation GetDBLocationForProtocol(string protocol)
  {
    McdDBLocation locationForProtocol;
    if (!this.dblocations.TryGetValue(protocol, out locationForProtocol) || locationForProtocol == null)
    {
      MCDDbLocation location = this.variant.DbLocations.OfType<MCDDbLocation>().FirstOrDefault<MCDDbLocation>((Func<MCDDbLocation, bool>) (l => l.AccessKey.Protocol == protocol));
      if (location != null)
        this.dblocations[protocol] = locationForProtocol = new McdDBLocation(location);
    }
    return locationForProtocol;
  }

  public IEnumerable<string> DBLocationNames
  {
    get => (IEnumerable<string>) this.variant.DbLocations.Names;
  }
}
