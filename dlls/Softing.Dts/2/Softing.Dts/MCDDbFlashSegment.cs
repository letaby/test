using System;

namespace Softing.Dts;

public interface MCDDbFlashSegment : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	uint UncompressedSize { get; }

	uint CompressedSize { get; }

	byte[] BinaryData { get; }

	uint SourceStartAddress { get; }

	uint SourceEndAddress { get; }

	MCDFlashSegmentIterator CreateFlashSegmentIterator(uint size);

	void RemoveFlashSegmentIterator(MCDFlashSegmentIterator flashSegmentIterator);
}
