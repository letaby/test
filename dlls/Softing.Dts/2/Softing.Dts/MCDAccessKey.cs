using System;

namespace Softing.Dts;

public interface MCDAccessKey : MCDObject, IDisposable
{
	MCDLocationType LocationType { get; }

	string String { get; }

	string Protocol { get; }

	string FunctionalGroup { get; }

	string EcuBaseVariant { get; }

	string EcuVariant { get; }

	string MultipleEcuJob { get; }
}
