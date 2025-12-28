using System;

namespace Softing.Dts;

public interface MCDRequestParameter : MCDParameter, MCDNamedObject, MCDObject, IDisposable
{
	MCDRequestParameters Parameters { get; }

	MCDValue ValueUnchecked { set; }

	MCDRequestParameter LengthKey { get; }

	bool IsVariableLength { get; }

	MCDValue ValuePDUConform { set; }

	MCDValue CreateValue();

	void AddParameters(uint count);
}
