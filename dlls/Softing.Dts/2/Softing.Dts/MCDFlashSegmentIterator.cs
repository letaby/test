using System;

namespace Softing.Dts;

public interface MCDFlashSegmentIterator : MCDObject, IDisposable
{
	uint BinaryDataChunkSize { get; }

	byte[] FirstBinaryDataChunk { get; }

	byte[] NextBinaryDataChunk { get; }

	bool HasNextBinaryDataChunk { get; }
}
