using System;

namespace Softing.Dts;

public interface MCDRequest : MCDObject, IDisposable
{
	MCDDbRequest DbObject { get; }

	MCDValue PDU { get; }

	MCDRequestParameters RequestParameters { get; }

	MCDValue CreateValue();

	void EnterPDU(MCDValue pdu);
}
