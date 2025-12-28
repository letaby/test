using System;

namespace Softing.Dts;

public interface MCDDbFunctionOutParameter : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDataType DataType { get; }

	MCDDbFunctionDiagComConnector DbFunctionDiagComConnector { get; }

	MCDDbResponseParameter DbResponseParameter { get; }

	MCDDbUnit DbUnit { get; }
}
