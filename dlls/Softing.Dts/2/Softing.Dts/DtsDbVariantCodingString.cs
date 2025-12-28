using System;

namespace Softing.Dts;

public interface DtsDbVariantCodingString : DtsDbObject, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	uint AccessLevel { get; }

	uint PartVersion { get; }

	string PartNumber { get; }

	MCDValue DefaultStringValue { get; }

	string GetSupplementKey(uint index);

	bool HasPartVersion();
}
