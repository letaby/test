namespace SapiLayer1;

public enum ConnectionState
{
	NotInitialized,
	Initialized,
	WaitingForTranslator,
	TranslatorConnected,
	ChannelsConnecting,
	ChannelsConnected,
	TranslatorConnectedNoTraffic,
	TranslatorDisconnected
}
