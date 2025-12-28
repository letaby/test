using System;

namespace Softing.Dts;

public interface MCDDbEnvDataConnector : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbResponseParameter DbEnvData { get; }

	MCDDbEnvDataDesc DbEnvDataDesc { get; }
}
