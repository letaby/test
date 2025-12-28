using System;
using System.Collections.Generic;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

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

	public FlashMeaning FlashMeaning => flashMeaning;

	public int MeaningIndex => (int)meaningIndex;

	public int Index => (int)index;

	public string Qualifier => qualifier;

	public string Name => name;

	public string Description => description;

	public long Length => length;

	public string BlockType => blockType;

	public int NumberOfSecurities => (int)numberOfSecurities;

	public FlashSecurityCollection Securities => cffSecurtities;

	public IEnumerable<FlashSegment> Segments
	{
		get
		{
			if (segments == null)
			{
				return null;
			}
			return segments.AsReadOnly();
		}
	}

	public long FromAddress => fromAddress;

	internal FlashDataBlock(FlashMeaning fm, uint meaningIndex, uint index)
	{
		flashMeaning = fm;
		this.meaningIndex = meaningIndex;
		this.index = index;
		qualifier = string.Empty;
		name = string.Empty;
		description = string.Empty;
		length = 0L;
		blockType = string.Empty;
		numberOfSecurities = 0u;
		cffSecurtities = new FlashSecurityCollection();
	}

	internal void Acquire(CaesarDIFlashDataBlock flashDataBlock, bool isLateBound)
	{
		//IL_008e: Expected O, but got Unknown
		qualifier = flashDataBlock.Qualifier;
		name = flashDataBlock.Name;
		description = flashDataBlock.Description;
		length = flashDataBlock.Length;
		blockType = flashDataBlock.BlockType;
		numberOfSecurities = flashDataBlock.NumberOfSecurities;
		for (ushort num = 0; num < numberOfSecurities; num++)
		{
			try
			{
				CaesarDICffSecur flashSecurity = flashDataBlock.GetFlashSecurity(num);
				try
				{
					FlashSecurity flashSecurity2 = new FlashSecurity(this);
					flashSecurity2.Acquire(flashSecurity);
					cffSecurtities.Add(flashSecurity2);
				}
				finally
				{
					((IDisposable)flashSecurity)?.Dispose();
				}
			}
			catch (CaesarErrorException ex)
			{
				CaesarException e = new CaesarException(ex, null, null);
				Sapi.GetSapi().RaiseExceptionEvent(this, e);
			}
		}
		uint numberOfSegments = flashDataBlock.NumberOfSegments;
		segments = new List<FlashSegment>();
		for (uint num2 = 0u; num2 < numberOfSegments; num2++)
		{
			SegmentEntry segmentEntry = flashDataBlock.GetSegmentEntry(num2, isLateBound);
			if (segmentEntry != null)
			{
				segments.Add(new FlashSegment(segmentEntry));
			}
		}
		fromAddress = (segments.Any() ? segments.Min((FlashSegment s) => s.FromAddress) : 0);
	}

	internal void Acquire(McdDBFlashDataBlock flashDataBlock)
	{
		qualifier = flashDataBlock.Qualifier;
		name = flashDataBlock.Name;
		description = flashDataBlock.Description;
		blockType = flashDataBlock.DataBlockType;
		numberOfSecurities = (uint)flashDataBlock.NumberOfSecurities;
		for (ushort num = 0; num < numberOfSecurities; num++)
		{
			McdDBFlashSecurity flashSecurity = flashDataBlock.GetFlashSecurity(num);
			FlashSecurity flashSecurity2 = new FlashSecurity(this);
			flashSecurity2.Acquire(flashSecurity);
			cffSecurtities.Add(flashSecurity2);
		}
		long numberOfSegments = flashDataBlock.NumberOfSegments;
		segments = new List<FlashSegment>();
		for (ushort num2 = 0; num2 < numberOfSegments; num2++)
		{
			McdDBFlashSegment flashSegment = flashDataBlock.GetFlashSegment(num2);
			segments.Add(new FlashSegment(flashSegment));
		}
		length = segments.Sum((FlashSegment s) => s.SegmentLength);
		fromAddress = (segments.Any() ? segments.Min((FlashSegment s) => s.FromAddress) : 0);
	}
}
