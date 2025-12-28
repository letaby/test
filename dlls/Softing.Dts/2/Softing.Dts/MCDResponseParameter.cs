using System;

namespace Softing.Dts;

public interface MCDResponseParameter : MCDParameter, MCDNamedObject, MCDObject, IDisposable
{
	bool HasError { get; }

	MCDError Error { get; set; }

	MCDResponseParameters Parameters { get; }

	MCDObject Parent { get; }

	MCDDbDiagTroubleCode DbDTC { get; }

	bool IsVariableLength { get; }

	MCDResponseParameter LengthKey { get; }
}
