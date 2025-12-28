// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashArea
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace SapiLayer1;

public sealed class FlashArea
{
  private Channel channel;
  private string qualifier;
  private string name;
  private string description;
  private FlashMeaningCollection flashMeanings;
  private FlashMeaning marked;

  internal FlashArea(Channel ch, string qualifier)
  {
    this.name = string.Empty;
    this.qualifier = qualifier;
    this.description = string.Empty;
    this.channel = ch;
    this.flashMeanings = new FlashMeaningCollection();
  }

  internal void Acquire(
    McdDBFlashSession flashSession,
    IEnumerable<string> allowedEcus,
    uint areaIndex)
  {
    this.name = flashSession.Name;
    this.description = flashSession.Description;
    uint num = 0;
    foreach (McdDBFlashDataBlock dbDataBlock in flashSession.DBDataBlocks)
    {
      if (File.Exists(dbDataBlock.DatabaseFileName))
      {
        FlashMeaning meaning = new FlashMeaning(this, areaIndex, num++, (string) null);
        meaning.Acquire(flashSession, dbDataBlock, allowedEcus);
        this.flashMeanings.Add(meaning);
      }
    }
  }

  internal void Acquire(CaesarDIFlashArea flashArea, uint areaIndex)
  {
    this.name = flashArea.Name;
    this.description = flashArea.Description;
    uint flashTableCount = flashArea.FlashTableCount;
    for (ushort index = 0; (uint) index < flashTableCount; ++index)
    {
      try
      {
        using (CaesarDIFlashTable flashTable = flashArea.GetFlashTable(index))
        {
          FlashMeaning meaning = new FlashMeaning(this, areaIndex, (uint) index, flashArea.FileNameAndPath);
          meaning.Acquire(flashTable);
          this.flashMeanings.Add(meaning);
        }
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        CaesarException e = new CaesarException(ex, negativeResponseCode);
        Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) e);
      }
    }
  }

  public Channel Channel => this.channel;

  public string Qualifier => this.qualifier;

  public string Name => this.name;

  public string Description => this.description;

  public FlashMeaningCollection FlashMeanings => this.flashMeanings;

  public FlashMeaning Marked
  {
    get => this.marked;
    set => this.marked = value;
  }
}
