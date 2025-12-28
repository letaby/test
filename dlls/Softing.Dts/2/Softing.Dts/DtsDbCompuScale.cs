using System;

namespace Softing.Dts;

public interface DtsDbCompuScale : DtsObject, MCDObject, IDisposable
{
	string ShortLabel { get; }

	bool IsDescriptionValid { get; }

	bool IsShortLabelValid { get; }

	string Description { get; }

	bool IsCompuInverseValueValid { get; }

	MCDValue CompuInverseValue { get; }

	bool IsCompuConstValueValid { get; }

	MCDValue CompuConstValue { get; }

	bool IsLowerLimitValid { get; }

	DtsDbLimit LowerLimit { get; }

	bool IsUpperLimitValid { get; }

	DtsDbLimit UpperLimit { get; }

	uint CompuNumeratorCount { get; }

	uint CompuDenominatorCount { get; }

	string CompuDenominatorsAsString { get; }

	string CompuNumeratorsAsString { get; }

	double GetCompuNumeratorAt(uint idx);

	double GetCompuDenominatorAt(uint idx);
}
