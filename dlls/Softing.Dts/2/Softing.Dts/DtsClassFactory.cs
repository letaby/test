using System;

namespace Softing.Dts;

public interface DtsClassFactory : DtsObject, MCDObject, IDisposable
{
	MCDValue CreateValue(MCDDataType type);

	MCDAccessKey CreateAccessKey(string accessKey);

	MCDError CreateError(MCDErrorCodes code, string text, ushort vendorcode, string information, MCDSeverity severity);
}
