using System;

namespace Softing.Dts;

public interface DtsDbResponse : MCDDbResponse, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	MCDValue ID { get; }

	bool IsNegativeResponse { get; }
}
