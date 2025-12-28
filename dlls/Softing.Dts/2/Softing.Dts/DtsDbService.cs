using System;

namespace Softing.Dts;

public interface DtsDbService : MCDDbService, MCDDbDiagService, MCDDbDataPrimitive, MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbDiagService, DtsDbDataPrimitive, DtsDbDiagComPrimitive, DtsDbObject, DtsNamedObject, DtsObject
{
	bool HasSuppressPositiveResponseCapability { get; }

	MCDResult GenerateResult(byte[] pRequest, byte[] pResponse);
}
