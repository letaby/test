using System;

namespace Softing.Dts;

public interface MCDDbFunctionInParameter : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDataType DataType { get; }

	MCDDbFunctionDiagComConnector DbFunctionDiagComConnector { get; }

	MCDDbRequestParameter DbRequestParameter { get; }

	MCDDbUnit DbUnit { get; }
}
