using System;

namespace Softing.Dts;

public interface DtsIdentifierInfo : DtsObject, MCDObject, IDisposable
{
	string EcuName { get; }

	uint Identifier { get; }

	string MessageName { get; }

	bool IsExtendedIdentifier { get; }

	DtsIdentifierType IdentifierType { get; }

	int ExtendedAddress { get; }
}
