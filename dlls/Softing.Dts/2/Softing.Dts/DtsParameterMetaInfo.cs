using System;

namespace Softing.Dts;

public interface DtsParameterMetaInfo : DtsNamedObject, MCDNamedObject, MCDObject, IDisposable, DtsObject
{
	MCDDataType ParameterType { get; }

	MCDValue DefaultValue { get; }

	MCDValue MinValue { get; }

	MCDValue MaxValue { get; }

	ushort Radix { get; }

	ushort DecimalPlaces { get; }

	string Unit { get; }

	ushort MinLength { get; }

	ushort MaxLength { get; }

	MCDTextTableElements TextTableElements { get; }
}
