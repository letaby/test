// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashMeaning
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class FlashMeaning
{
  private FlashArea flashArea;
  private string description;
  private string name;
  private string flashKey;
  private uint priority;
  private uint index;
  private uint areaIndex;
  private string fileName;
  private FlashDataBlockCollection flashDataBlocks;
  private IEnumerable<Ecu> allowedEcus;
  private string lateBoundFileName;
  private string flashJobName;

  internal FlashMeaning(FlashArea fa, uint areaIndex, uint index, string fileName)
  {
    this.name = string.Empty;
    this.flashKey = string.Empty;
    this.flashArea = fa;
    this.index = index;
    this.areaIndex = areaIndex;
    this.description = string.Empty;
    this.fileName = fileName;
    this.flashDataBlocks = new FlashDataBlockCollection();
  }

  internal void Acquire(
    McdDBFlashSession flashSession,
    McdDBFlashDataBlock dataBlock,
    IEnumerable<string> allowedEcus)
  {
    this.priority = (uint) flashSession.Priority;
    this.flashKey = flashSession.FlashKey;
    this.allowedEcus = allowedEcus.Select<string, Ecu>((Func<string, Ecu>) (e => Sapi.GetSapi().Ecus.FirstOrDefault<Ecu>((Func<Ecu, bool>) (ecu => ecu.IsMcd && ecu.Name == e))));
    this.name = flashSession.Name;
    this.fileName = dataBlock.DatabaseFileName;
    this.flashJobName = flashSession.FlashJobName;
    FlashDataBlock flashDataBlock = new FlashDataBlock(this, this.index, 0U);
    flashDataBlock.Acquire(dataBlock);
    this.flashDataBlocks.Add(flashDataBlock);
  }

  internal void Acquire(CaesarDIFlashTable flashTable)
  {
    this.name = flashTable.Name;
    this.description = flashTable.Description;
    this.flashKey = flashTable.FlashKey;
    this.priority = flashTable.Priority;
    this.flashJobName = flashTable.FlashService;
    List<Ecu> ecuList = new List<Ecu>();
    for (uint index = 0; index < flashTable.AllowedEcuCount; ++index)
    {
      string allowedEcuByIndex = flashTable.GetAllowedEcuByIndex(index);
      Ecu ecu = Sapi.GetSapi().Ecus[allowedEcuByIndex];
      if (ecu != null)
        ecuList.Add(ecu);
    }
    this.allowedEcus = (IEnumerable<Ecu>) ecuList.AsReadOnly();
    uint flashDataBlockCount = flashTable.FlashDataBlockCount;
    this.lateBoundFileName = flashTable.GetDataBlockFileName((ushort) 0);
    for (ushort index = 0; (uint) index < flashDataBlockCount; ++index)
    {
      try
      {
        using (CaesarDIFlashDataBlock flashDataBlock1 = flashTable.GetFlashDataBlock(index))
        {
          FlashDataBlock flashDataBlock2 = new FlashDataBlock(this, this.index, (uint) index);
          flashDataBlock2.Acquire(flashDataBlock1, this.lateBoundFileName != null);
          this.flashDataBlocks.Add(flashDataBlock2);
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

  public FlashArea FlashArea => this.flashArea;

  public string Name => this.name;

  public string Description => this.description;

  public string FlashKey => this.flashKey;

  public int Priority => (int) this.priority;

  public int Index => (int) this.index;

  public int AreaIndex => (int) this.areaIndex;

  public string FileName => this.fileName;

  public string FlashJobName => this.flashJobName;

  public IEnumerable<Ecu> AllowedEcus => this.allowedEcus;

  public FlashDataBlockCollection FlashDataBlocks => this.flashDataBlocks;

  public string LateBoundContentFileName => this.lateBoundFileName;
}
