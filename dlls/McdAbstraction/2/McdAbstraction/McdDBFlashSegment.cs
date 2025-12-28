using Softing.Dts;

namespace McdAbstraction;

public class McdDBFlashSegment
{
	private uint sourceStartAddress;

	private uint uncompressedSize;

	private string qualifier;

	private string name;

	private string description;

	private string uniqueObjectId;

	public string Qualifier => qualifier;

	public string Name => name;

	public string UniqueObjectId => uniqueObjectId;

	public string Description => description;

	public long UncompressedSize => uncompressedSize;

	public long SourceStartAddress => sourceStartAddress;

	internal McdDBFlashSegment(MCDDbFlashSegment segment)
	{
		sourceStartAddress = segment.SourceStartAddress;
		uncompressedSize = segment.UncompressedSize;
		qualifier = segment.ShortName;
		name = segment.LongName;
		description = segment.Description;
		uniqueObjectId = segment.UniqueObjectIdentifier;
	}
}
