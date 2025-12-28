using System;

namespace Softing.Dts;

public interface MCDDbPhysicalMemory : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbPhysicalSegments PhysicalSegments { get; }
}
