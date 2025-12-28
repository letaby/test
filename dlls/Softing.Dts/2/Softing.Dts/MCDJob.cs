using System;

namespace Softing.Dts;

public interface MCDJob : MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	byte Progress { get; }

	string JobInfo { get; }

	MCDResult CreateResult(MCDResultType resultType, MCDErrorCodes code, string codeDescription, ushort vendorCode, string vendorCodeDescription, MCDSeverity severity);

	void ReleaseResult(MCDResult result);

	MCDResults CreateResultCollection();
}
