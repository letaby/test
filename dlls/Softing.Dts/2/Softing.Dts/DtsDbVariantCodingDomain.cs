using System;

namespace Softing.Dts;

public interface DtsDbVariantCodingDomain : DtsDbObject, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	DtsDbVariantCodingFragments Fragments { get; }

	DtsDbVariantCodingStrings DefaultStrings { get; }

	DtsDbVariantCodingStrings ExternalDefaultStrings { get; }

	ushort GetSegmentNumberOfDomainByIndex(uint index);

	ushort GetSegmentNumberOfDomainByName(string name);
}
