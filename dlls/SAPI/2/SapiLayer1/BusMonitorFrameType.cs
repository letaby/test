namespace SapiLayer1;

public enum BusMonitorFrameType
{
	SingleFrame,
	FirstFrame,
	ConsecutiveFrame,
	FlowControl,
	RequestToSendDestinationSpecific,
	ClearToSendDestinationSpecific,
	ConnectionAbort,
	BroadcastAnnounceMessageGlobalDestination,
	EndOfMessageAcknowledge,
	TransportProtocolDataTransfer,
	VehicleIdentificationRequest,
	VehicleAnnouncementMessage,
	RoutingActivationRequest,
	RoutingActivationResponse,
	Acknowledgment,
	ChipState,
	ErrorFrame,
	BaudRate,
	Unknown
}
