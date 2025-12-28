// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBEcuBaseVariant
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBEcuBaseVariant
{
  private MCDDbEcuBaseVariant baseVariant;
  private Dictionary<string, McdDBEcuVariant> variants;
  private Dictionary<string, McdDBLocation> dblocations = new Dictionary<string, McdDBLocation>();

  internal McdDBEcuBaseVariant(MCDDbEcuBaseVariant baseVariant)
  {
    this.baseVariant = baseVariant;
    this.Description = this.baseVariant.Description;
    this.DatabaseFile = ((DtsDbObject) this.baseVariant).DatabaseFile;
    this.variants = ((IEnumerable<string>) this.baseVariant.DbEcuVariants.Names).Select<string, KeyValuePair<string, McdDBEcuVariant>>((Func<string, KeyValuePair<string, McdDBEcuVariant>>) (n => new KeyValuePair<string, McdDBEcuVariant>(n, (McdDBEcuVariant) null))).ToDictionary<KeyValuePair<string, McdDBEcuVariant>, string, McdDBEcuVariant>((Func<KeyValuePair<string, McdDBEcuVariant>, string>) (k => k.Key), (Func<KeyValuePair<string, McdDBEcuVariant>, McdDBEcuVariant>) (v => v.Value));
  }

  public string Description { get; private set; }

  public string DatabaseFile { get; private set; }

  public string Preamble => McdRoot.GetPreamble(this.DatabaseFile);

  public IEnumerable<string> DBEcuVariantNames
  {
    get => (IEnumerable<string>) this.variants.Keys.ToList<string>();
  }

  public McdDBEcuVariant GetDBEcuVariant(string name)
  {
    McdDBEcuVariant dbEcuVariant;
    if (!this.variants.TryGetValue(name, out dbEcuVariant) || dbEcuVariant == null)
    {
      MCDDbEcuVariant itemByName = this.baseVariant.DbEcuVariants.GetItemByName(name);
      if (itemByName != null)
        this.variants[name] = dbEcuVariant = new McdDBEcuVariant(itemByName);
    }
    return dbEcuVariant;
  }

  public McdDBLocation GetDBLocationForProtocol(string protocol)
  {
    McdDBLocation locationForProtocol;
    if (!this.dblocations.TryGetValue(protocol, out locationForProtocol) || locationForProtocol == null)
    {
      MCDDbLocation location = this.baseVariant.DbLocations.OfType<MCDDbLocation>().FirstOrDefault<MCDDbLocation>((Func<MCDDbLocation, bool>) (l => l.AccessKey.Protocol == protocol));
      if (location != null)
        this.dblocations[protocol] = locationForProtocol = new McdDBLocation(location);
    }
    return locationForProtocol;
  }

  public IEnumerable<string> DBLocationNames
  {
    get => (IEnumerable<string>) this.baseVariant.DbLocations.Names;
  }
}
