using System;

namespace Softing.Dts;

public interface DtsResponse : MCDResponse, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	uint LocationAddress { get; }

	uint ResponseTime { get; }

	uint CANIdentifier { get; }

	bool HasResponseMessage { get; }

	void EnterPDU(MCDValue pdu);
}
