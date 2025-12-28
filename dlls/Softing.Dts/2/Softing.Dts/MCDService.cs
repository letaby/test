using System;

namespace Softing.Dts;

public interface MCDService : MCDDiagService, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	MCDTransmissionMode RuntimeTransmissionMode { get; set; }

	ushort DefaultResultBufferSize { get; }

	bool IsSuppressPositiveResponse { get; }

	bool SuppressPositiveResponse { set; }

	bool HasSuppressPositiveResponseCapability { get; }
}
