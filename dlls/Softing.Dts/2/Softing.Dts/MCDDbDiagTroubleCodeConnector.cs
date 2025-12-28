using System;

namespace Softing.Dts;

public interface MCDDbDiagTroubleCodeConnector : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbDiagTroubleCode DbDiagTroubleCode { get; }

	MCDDbFaultMemory DbFaultMemory { get; }
}
