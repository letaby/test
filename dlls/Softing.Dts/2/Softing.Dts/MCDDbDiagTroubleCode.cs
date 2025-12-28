using System;

namespace Softing.Dts;

public interface MCDDbDiagTroubleCode : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string DisplayTroubleCode { get; }

	string DTCText { get; }

	uint Level { get; }

	uint TroubleCode { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	string DiagTroubleCodeTextID { get; }

	string UnicodeDTCText { get; }
}
