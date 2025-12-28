using System;

namespace Softing.Dts;

public interface MCDDbUnitGroup : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDUnitGroupCategory Category { get; }

	MCDDbUnits Units { get; }
}
