namespace J2534;

public enum J2534Error
{
	NoError,
	NotSupported,
	InvalidChannelId,
	InvalidProtocolId,
	NullParameter,
	InvalidIoctlValue,
	InvalidFlags,
	Failed,
	DeviceNotConnected,
	Timeout,
	InvalidMsg,
	InvalidTimeInterval,
	ExceededLimit,
	InvalidMsgId,
	DeviceInUse,
	InvalidIoctlId,
	BufferEmpty,
	BufferFull,
	BufferOverflow,
	PinInvalid,
	ChannelInUse,
	MsgProtocolId,
	InvalidFilterId,
	NoFlowControl
}
