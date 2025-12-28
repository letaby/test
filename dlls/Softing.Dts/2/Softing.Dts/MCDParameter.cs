using System;

namespace Softing.Dts;

public interface MCDParameter : MCDNamedObject, MCDObject, IDisposable
{
	ushort DecimalPlaces { get; }

	MCDValue Value { get; set; }

	string Unit { get; }

	ushort Radix { get; }

	MCDRangeInfo ValueRangeInfo { get; }

	MCDDataType Type { get; }

	MCDDbParameter DbObject { get; }

	MCDValue CodedValue { get; set; }

	MCDParameterType MCDParameterType { get; }

	MCDDataType DataType { get; }

	MCDScaleConstraint PhysicalScaleConstraint { get; }

	MCDScaleConstraint InternalScaleConstraint { get; }

	byte BitPos { get; }

	uint ByteLength { get; }

	uint BytePos { get; }
}
