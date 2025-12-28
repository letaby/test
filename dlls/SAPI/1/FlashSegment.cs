// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashSegment
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;

#nullable disable
namespace SapiLayer1;

public sealed class FlashSegment
{
  internal FlashSegment(SegmentEntry entry)
  {
    this.Qualifier = entry.Qualifier;
    this.LongName = entry.LongName;
    this.Description = entry.Description;
    this.FromAddress = entry.FromAddress;
    this.SegmentLength = entry.SegmentLength;
  }

  internal FlashSegment(McdDBFlashSegment entry)
  {
    this.FromAddress = entry.SourceStartAddress;
    this.SegmentLength = entry.UncompressedSize;
    this.Qualifier = entry.Qualifier;
    this.LongName = entry.Name;
    this.Description = entry.Description;
  }

  public string Qualifier { private set; get; }

  public string LongName { private set; get; }

  public string Description { private set; get; }

  public long FromAddress { private set; get; }

  public long SegmentLength { private set; get; }
}
