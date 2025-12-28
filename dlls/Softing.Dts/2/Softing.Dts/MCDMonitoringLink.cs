using System;

namespace Softing.Dts;

public interface MCDMonitoringLink : MCDNamedObject, MCDObject, IDisposable
{
	MCDInterfaceResource InterfaceResource { get; }

	MCDMessageFilters MessageFilters { get; }

	ushort NoOfSampleToFireEvent { set; }

	ushort TimeToFireEvent { set; }

	string[] FetchMonitoringFrames(uint numReq);

	void Start();

	void Stop();
}
