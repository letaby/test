using Softing.Dts;

namespace McdAbstraction;

public class McdDBCompuScale
{
	public double? Offset { get; private set; }

	public double? Factor { get; private set; }

	public long? Max { get; private set; }

	public long? Min { get; private set; }

	public string Name { get; private set; }

	internal McdDBCompuScale(DtsDbCompuScale scale)
	{
		if (scale.IsLowerLimitValid)
		{
			Min = GetValue(scale.LowerLimit.Value);
		}
		if (scale.IsUpperLimitValid)
		{
			Max = GetValue(scale.UpperLimit.Value);
		}
		if (scale.CompuNumeratorCount == 2)
		{
			double num = ((scale.CompuDenominatorCount == 1) ? scale.GetCompuDenominatorAt(0u) : 1.0);
			Factor = scale.GetCompuNumeratorAt(1u) / num;
			Offset = scale.GetCompuNumeratorAt(0u) / num;
		}
		if (scale.IsShortLabelValid)
		{
			Name = scale.ShortLabel;
		}
		static long? GetValue(MCDValue value)
		{
			return value.DataType switch
			{
				MCDDataType.eA_UINT32 => value.Uint32, 
				MCDDataType.eA_INT32 => value.Int32, 
				_ => null, 
			};
		}
	}
}
