// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashDataBlock
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

public sealed class FlashDataBlock
{
  private FlashMeaning flashMeaning;
  private uint meaningIndex;
  private uint index;
  private string qualifier;
  private string name;
  private string description;
  private long length;
  private string blockType;
  private uint numberOfSecurities;
  private FlashSecurityCollection cffSecurtities;
  private List<FlashSegment> segments;
  private long fromAddress;

  internal FlashDataBlock(FlashMeaning fm, uint meaningIndex, uint index)
  {
    this.flashMeaning = fm;
    this.meaningIndex = meaningIndex;
    this.index = index;
    this.qualifier = string.Empty;
    this.name = string.Empty;
    this.description = string.Empty;
    this.length = 0L;
    this.blockType = string.Empty;
    this.numberOfSecurities = 0U;
    this.cffSecurtities = new FlashSecurityCollection();
  }

  internal void Acquire(CaesarDIFlashDataBlock flashDataBlock, bool isLateBound)
  {
    this.qualifier = flashDataBlock.Qualifier;
    this.name = flashDataBlock.Name;
    this.description = flashDataBlock.Description;
    this.length = (long) flashDataBlock.Length;
    this.blockType = flashDataBlock.BlockType;
    this.numberOfSecurities = flashDataBlock.NumberOfSecurities;
    for (ushort index = 0; (uint) index < this.numberOfSecurities; ++index)
    {
      try
      {
        using (CaesarDICffSecur flashSecurity = flashDataBlock.GetFlashSecurity(index))
        {
          FlashSecurity flashSecurityBlock = new FlashSecurity(this);
          flashSecurityBlock.Acquire(flashSecurity);
          this.cffSecurtities.Add(flashSecurityBlock);
        }
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        CaesarException e = new CaesarException(ex, negativeResponseCode);
        Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) e);
      }
    }
    uint numberOfSegments = flashDataBlock.NumberOfSegments;
    this.segments = new List<FlashSegment>();
    for (uint index = 0; index < numberOfSegments; ++index)
    {
      SegmentEntry segmentEntry = flashDataBlock.GetSegmentEntry(index, isLateBound);
      if (segmentEntry != null)
        this.segments.Add(new FlashSegment(segmentEntry));
    }
    this.fromAddress = this.segments.Any<FlashSegment>() ? this.segments.Min<FlashSegment>((Func<FlashSegment, long>) (s => s.FromAddress)) : 0L;
  }

  internal void Acquire(McdDBFlashDataBlock flashDataBlock)
  {
    this.qualifier = flashDataBlock.Qualifier;
    this.name = flashDataBlock.Name;
    this.description = flashDataBlock.Description;
    this.blockType = flashDataBlock.DataBlockType;
    this.numberOfSecurities = (uint) flashDataBlock.NumberOfSecurities;
    for (ushort index = 0; (uint) index < this.numberOfSecurities; ++index)
    {
      McdDBFlashSecurity flashSecurity = flashDataBlock.GetFlashSecurity((int) index);
      FlashSecurity flashSecurityBlock = new FlashSecurity(this);
      flashSecurityBlock.Acquire(flashSecurity);
      this.cffSecurtities.Add(flashSecurityBlock);
    }
    long numberOfSegments = flashDataBlock.NumberOfSegments;
    this.segments = new List<FlashSegment>();
    for (ushort index = 0; (long) index < numberOfSegments; ++index)
      this.segments.Add(new FlashSegment(flashDataBlock.GetFlashSegment((int) index)));
    this.length = this.segments.Sum<FlashSegment>((Func<FlashSegment, long>) (s => s.SegmentLength));
    this.fromAddress = this.segments.Any<FlashSegment>() ? this.segments.Min<FlashSegment>((Func<FlashSegment, long>) (s => s.FromAddress)) : 0L;
  }

  public FlashMeaning FlashMeaning => this.flashMeaning;

  public int MeaningIndex => (int) this.meaningIndex;

  public int Index => (int) this.index;

  public string Qualifier => this.qualifier;

  public string Name => this.name;

  public string Description => this.description;

  public long Length => this.length;

  public string BlockType => this.blockType;

  public int NumberOfSecurities => (int) this.numberOfSecurities;

  public FlashSecurityCollection Securities => this.cffSecurtities;

  public IEnumerable<FlashSegment> Segments
  {
    get
    {
      return this.segments == null ? (IEnumerable<FlashSegment>) null : (IEnumerable<FlashSegment>) this.segments.AsReadOnly();
    }
  }

  public long FromAddress => this.fromAddress;
}
