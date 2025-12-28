using System;

namespace Softing.Dts;

public interface MCDDbBaseFunctionNode : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbComponentConnectors DbComponentConnectors { get; }

	MCDDbFunctionInParameters DbFunctionInParams { get; }

	MCDDbFunctionOutParameters DbFunctionOutParams { get; }

	MCDVersion Version { get; }

	MCDAudience AudienceState { get; }

	MCDDbJobs DbMultipleEcuJobs { get; }
}
