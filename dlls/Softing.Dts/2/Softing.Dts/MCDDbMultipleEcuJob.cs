using System;

namespace Softing.Dts;

public interface MCDDbMultipleEcuJob : MCDDbJob, MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbLocations DbLocations { get; }
}
