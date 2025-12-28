using System;

namespace Softing.Dts;

public interface MCDStopCommunication : MCDControlPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	bool HasSuppressPositiveResponseCapability { get; }

	bool IsSuppressPositiveResponse { get; }

	bool SuppressPositiveResponse { set; }
}
