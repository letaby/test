using System;

namespace Softing.Dts;

public interface DtsDbFlashChecksum : MCDDbFlashChecksum, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	ulong UncompressedSize64 { get; }

	ulong SourceStartAddress64 { get; }

	ulong SourceEndAddress64 { get; }

	ulong CompressedSize64 { get; }
}
