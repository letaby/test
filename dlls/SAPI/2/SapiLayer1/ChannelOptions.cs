using System;

namespace SapiLayer1;

[Flags]
public enum ChannelOptions
{
	None = 0,
	StartStopCommunications = 1,
	CyclicRead = 4,
	MaintainSession = 8,
	ExecutePreService = 0x10,
	ProcessAffected = 0x20,
	ExecuteInitializeService = 0x40,
	ExecuteParameterWriteInitializeCommitServices = 0x80,
	All = 0xFD
}
