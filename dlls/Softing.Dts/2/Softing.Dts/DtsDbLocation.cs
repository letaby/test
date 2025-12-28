using System;

namespace Softing.Dts;

public interface DtsDbLocation : MCDDbLocation, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	MCDDbLocation ProtocolLocation { get; }

	bool HasDbVariantCodingDomains { get; }

	DtsDbVariantCodingDomains DbVariantCodingDomains { get; }

	DtsDbDiagVariables DbDiagVariables { get; }

	bool IsOnboard { get; }

	MCDVersion Version { get; }

	string DataBaseType { get; }

	bool IsLinLocation { get; }

	bool IsUdsLocation { get; }

	uint LogicalAddressValue { get; }

	DtsOfflineVariantCoding CreateOfflineVariantCoding();
}
