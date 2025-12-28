using System;

namespace Softing.Dts;

public interface DtsDbVariantCodingFragment : DtsDbObject, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	DtsParameterMetaInfo ParameterMetaInformation { get; }

	bool HasBitPosition { get; }

	uint BitPosition { get; }

	uint BytePosition { get; }

	uint BitLength { get; }

	uint ReadSecurityLevel { get; }

	uint WriteSecurityLevel { get; }

	string GetTextTableSupplementKey(uint index);

	DtsDbVariantCodingStrings GetFragmentMeanings(bool bIsInternal);
}
