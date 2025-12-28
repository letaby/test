// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBFlashDataBlock
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdDBFlashDataBlock
{
  private MCDDbFlashDataBlock dataBlock;
  private string qualifier;
  private string name;
  private string fileName;
  private string dataFileName;
  private string databaseFileName;
  private string dataBlockType;
  private uint numberSecurities;
  private uint numberSegments;
  private string description;

  internal McdDBFlashDataBlock(MCDDbFlashDataBlock dataBlock)
  {
    this.name = dataBlock.LongName;
    this.qualifier = dataBlock.ShortName;
    this.fileName = dataBlock.DbFlashData.ActiveFileName;
    this.dataFileName = dataBlock.DbFlashData.DataFileName;
    this.databaseFileName = ((DtsDbFlashData) dataBlock.DbFlashData).DatabaseFileName;
    this.numberSecurities = dataBlock.DbSecurities.Count;
    this.numberSegments = dataBlock.DbFlashSegments.Count;
    this.dataBlockType = dataBlock.DataBlockType;
    this.description = dataBlock.Description;
    this.dataBlock = dataBlock;
  }

  public McdDBFlashSecurity GetFlashSecurity(int index)
  {
    return new McdDBFlashSecurity(this.dataBlock.DbSecurities.GetItemByIndex((uint) index));
  }

  public McdDBFlashSegment GetFlashSegment(int index)
  {
    return new McdDBFlashSegment(this.dataBlock.DbFlashSegments.GetItemByIndex((uint) index));
  }

  public string DataFileName => this.dataFileName;

  public string FileName => this.fileName;

  public string DatabaseFileName => this.databaseFileName;

  public string Qualifier => this.qualifier;

  public string Name => this.name;

  public string DataBlockType => this.dataBlockType;

  public long NumberOfSecurities => (long) this.numberSecurities;

  public long NumberOfSegments => (long) this.numberSegments;

  public string Description => this.description;
}
