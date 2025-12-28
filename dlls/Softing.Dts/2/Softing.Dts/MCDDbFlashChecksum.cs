using System;

namespace Softing.Dts;

public interface MCDDbFlashChecksum : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string ChecksumAlgorithm { get; }

	MCDValue ChecksumResult { get; }

	uint UncompressedSize { get; }

	uint CompressedSize { get; }

	byte[] FillByte { get; }

	uint SourceStartAddress { get; }

	uint SourceEndAddress { get; }
}
