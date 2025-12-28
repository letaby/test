using System;

namespace Softing.Dts;

public interface DtsDbFlashSegment : MCDDbFlashSegment, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	[Obsolete("Function is marked as deprecated!")]
	bool HasNextBinaryDataChunk { get; }

	[Obsolete("Function is marked as deprecated!")]
	byte[] NextBinaryDataChunk { get; }

	ulong SourceEndAddress64 { get; }

	ulong SourceStartAddress64 { get; }

	ulong UncompressedSize64 { get; }

	ulong CompressedSize64 { get; }

	byte[] GetBinaryDataOffset(uint uOffset, uint uLength);

	[Obsolete("Function is marked as deprecated!")]
	byte[] GetFirstBinaryDataChunk(uint size);

	byte[] GetBinaryDataOffset64(ulong uOffset, ulong uLength);
}
