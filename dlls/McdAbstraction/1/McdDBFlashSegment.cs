// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBFlashSegment
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdDBFlashSegment
{
  private uint sourceStartAddress;
  private uint uncompressedSize;
  private string qualifier;
  private string name;
  private string description;
  private string uniqueObjectId;

  internal McdDBFlashSegment(MCDDbFlashSegment segment)
  {
    this.sourceStartAddress = segment.SourceStartAddress;
    this.uncompressedSize = segment.UncompressedSize;
    this.qualifier = segment.ShortName;
    this.name = segment.LongName;
    this.description = segment.Description;
    this.uniqueObjectId = segment.UniqueObjectIdentifier;
  }

  public string Qualifier => this.qualifier;

  public string Name => this.name;

  public string UniqueObjectId => this.uniqueObjectId;

  public string Description => this.description;

  public long UncompressedSize => (long) this.uncompressedSize;

  public long SourceStartAddress => (long) this.sourceStartAddress;
}
