using System;

namespace Softing.Dts;

public interface MCDStartCommunication : MCDControlPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	bool HasSuppressPositiveResponseCapability { get; }

	bool IsSuppressPositiveResponse { get; }

	bool SuppressPositiveResponse { set; }
}
