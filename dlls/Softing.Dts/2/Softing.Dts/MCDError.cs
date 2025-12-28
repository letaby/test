using System;

namespace Softing.Dts;

public interface MCDError : MCDObject, IDisposable
{
	MCDSeverity Severity { get; }

	MCDErrorCodes Code { get; }

	ushort VendorCode { get; }

	string VendorCodeDescription { get; }

	string CodeDescription { get; }

	MCDObject Parent { get; }
}
