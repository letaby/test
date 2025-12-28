using System;

namespace Softing.Dts;

public interface MCDDbConfigurationItem : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	uint BitLength { get; }

	ushort BitPos { get; }

	uint BytePos { get; }

	MCDDataType DataType { get; }

	MCDDbUnit DbUnit { get; }

	MCDConstraint InternalConstraint { get; }

	MCDInterval Interval { get; }

	string Semantic { get; }

	MCDTextTableElements TextTableElements { get; }

	bool IsComplex { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }
}
