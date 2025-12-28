using System;

namespace Softing.Dts;

public interface MCDDbComponentConnector : MCDObject, IDisposable
{
	MCDDbEcuBaseVariant DbEcuBaseVariant { get; }

	MCDDbEcuVariants DbEcuVariants { get; }

	MCDDbLocations DbLocationsForDiagObjectConnector { get; }

	MCDDbDiagObjectConnector GetDbDiagObjectConnector(MCDDbLocation locationContext);
}
