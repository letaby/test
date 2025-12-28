using System;

namespace Softing.Dts;

public interface MCDDbUnit : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbPhysicalDimension DbPhysicalDimension { get; }

	MCDDbUnitGroups DbUnitGroups { get; }

	string DisplayName { get; }

	double FactorSItoUnit { get; }

	double OffsetSItoUnit { get; }
}
