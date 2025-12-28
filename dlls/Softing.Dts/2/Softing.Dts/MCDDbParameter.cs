using System;

namespace Softing.Dts;

public interface MCDDbParameter : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	uint BytePos { get; }

	byte BitPos { get; }

	uint ByteLength { get; }

	ushort DecimalPlaces { get; }

	MCDValue DefaultValue { get; }

	ushort MaxLength { get; }

	ushort MinLength { get; }

	MCDDataType ParameterType { get; }

	ushort Radix { get; }

	string Unit { get; }

	uint DisplayLevel { get; }

	string Semantic { get; }

	MCDInterval Interval { get; }

	MCDDbParameters DbParameters { get; }

	bool IsConstant { get; }

	MCDConstraint InternalConstraint { get; }

	uint BitLength { get; }

	MCDDbTable DbTable { get; }

	MCDDbParameter DbTableKeyParam { get; }

	MCDDbParameters DbTableStructParams { get; }

	MCDValues Keys { get; }

	MCDTextTableElements TextTableElements { get; }

	MCDParameterType MCDParameterType { get; }

	MCDDataType DataType { get; }

	MCDValue CodedDefaultValue { get; }

	MCDIntervals ValidInternalIntervals { get; }

	MCDIntervals ValidPhysicalIntervals { get; }

	MCDConstraint PhysicalConstraint { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	uint ODXBytePos { get; }

	MCDDbUnit DbUnit { get; }

	MCDDbParameter LengthKey { get; }

	bool IsVariableLength { get; }

	MCDDbParameters GetStructureByKey(MCDValue key);
}
