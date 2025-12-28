using System;

namespace Softing.Dts;

public interface MCDDbFlashData : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDFlashDataFormat DataFormat { get; }

	MCDValue EncryptionCompressionMethod { get; }

	string DataFileName { get; }

	bool IsLateBound { get; }

	string ActiveFileName { get; set; }

	string[] MatchingFileNames { get; }
}
