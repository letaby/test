// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBFlashSession
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBFlashSession
{
  private MCDDbFlashSession session;
  private string qualifier;
  private string name;
  private string description;
  private string flashJobName;
  private uint priority;
  private string key;
  private string[] layerReferences;
  private List<McdDBFlashDataBlock> dataBlocks;

  internal McdDBFlashSession(MCDDbFlashSession session)
  {
    this.qualifier = session.ShortName;
    this.name = session.LongName;
    this.description = session.Description;
    this.flashJobName = ((DtsDbFlashSession) session).FlashJobName;
    this.priority = session.Priority;
    this.key = session.FlashKey;
    this.session = session;
  }

  public string Qualifier => this.qualifier;

  public string Name => this.name;

  public string Description => this.description;

  public string FlashJobName => this.flashJobName;

  public long Priority => (long) this.priority;

  public string FlashKey => this.key;

  public IEnumerable<string> LayerReferences
  {
    get
    {
      if (this.layerReferences == null)
        this.layerReferences = ((DtsDbFlashSession) this.session).LayerReferences;
      return (IEnumerable<string>) this.layerReferences;
    }
  }

  public IEnumerable<McdDBFlashDataBlock> DBDataBlocks
  {
    get
    {
      if (this.dataBlocks == null)
        this.dataBlocks = this.session.DbDataBlocks.OfType<MCDDbFlashDataBlock>().Select<MCDDbFlashDataBlock, McdDBFlashDataBlock>((Func<MCDDbFlashDataBlock, McdDBFlashDataBlock>) (fs => new McdDBFlashDataBlock(fs))).ToList<McdDBFlashDataBlock>();
      return (IEnumerable<McdDBFlashDataBlock>) this.dataBlocks;
    }
  }
}
