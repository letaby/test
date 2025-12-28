using System;

namespace Softing.Dts;

public interface MCDDbEcu : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbLocations DbLocations { get; }
}
