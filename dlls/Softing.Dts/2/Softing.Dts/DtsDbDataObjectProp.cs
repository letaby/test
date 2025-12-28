using System;

namespace Softing.Dts;

public interface DtsDbDataObjectProp : DtsDbObject, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	MCDDataType CodedType { get; }

	DtsDbCompuMethod CompuMethod { get; }

	MCDDataType PhysicalType { get; }

	bool IsDiagCodedTypeValid { get; }

	bool IsPhysicalTypeValid { get; }

	bool IsCompuMethodValid { get; }

	DtsDiagCodedLengthType DiagCodedLengthType { get; }

	bool IsDiagCodedLengthTypeValid { get; }

	bool IsHighlowByteOrder { get; }
}
