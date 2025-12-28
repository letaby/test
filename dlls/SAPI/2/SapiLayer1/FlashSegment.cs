using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FlashSegment
{
	public string Qualifier { get; private set; }

	public string LongName { get; private set; }

	public string Description { get; private set; }

	public long FromAddress { get; private set; }

	public long SegmentLength { get; private set; }

	internal FlashSegment(SegmentEntry entry)
	{
		Qualifier = entry.Qualifier;
		LongName = entry.LongName;
		Description = entry.Description;
		FromAddress = entry.FromAddress;
		SegmentLength = entry.SegmentLength;
	}

	internal FlashSegment(McdDBFlashSegment entry)
	{
		FromAddress = entry.SourceStartAddress;
		SegmentLength = entry.UncompressedSize;
		Qualifier = entry.Qualifier;
		LongName = entry.Name;
		Description = entry.Description;
	}
}
