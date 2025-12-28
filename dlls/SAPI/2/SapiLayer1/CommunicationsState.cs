namespace SapiLayer1;

public enum CommunicationsState
{
	Unknown = -1,
	OnlineButNotInitialized,
	Online,
	Disconnecting,
	Offline,
	ReadEcuInfo,
	ReadParameters,
	WriteParameters,
	Flash,
	ExecuteService,
	ReadInstrument,
	ResetFaults,
	ReadFaults,
	LogFilePlayback,
	LogFilePaused,
	ProcessVcp,
	ResetFault,
	ByteMessage,
	ReadSnapshot
}
