using System;

namespace Softing.Dts;

public interface MCDDbPhysicalDimension : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	int CurrentExponent { get; }

	int LengthExponent { get; }

	int LuminousIntensity { get; }

	int MassExponent { get; }

	int MolarAmountExponent { get; }

	int TemperatureExponent { get; }

	int TimeExponent { get; }
}
