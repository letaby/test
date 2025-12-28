using System;
using System.Collections.Generic;
using System.Globalization;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class ScaleEntry
{
	internal const string SignalNotAvailable = "sna";

	public decimal Min { get; private set; }

	public decimal Max { get; private set; }

	public decimal Factor { get; private set; }

	public decimal Offset { get; private set; }

	public string Name { get; internal set; }

	public bool IsFixedValue => ToEcuValue(Min) == ToEcuValue(Max);

	internal ScaleEntry(PrepScaleEntry prepEntry, PresScaleEntry presEntry)
	{
		Factor = Convert.ToDecimal((double)prepEntry.Factor, CultureInfo.InvariantCulture);
		Offset = Convert.ToDecimal((double)prepEntry.Offset, CultureInfo.InvariantCulture);
		if (Math.Floor(Factor) != Factor)
		{
			decimal num = Convert.ToDecimal((double)presEntry.Factor, CultureInfo.InvariantCulture);
			if (Math.Floor(num) == num)
			{
				Factor = 1m / num;
				Offset = -(Convert.ToDecimal((double)presEntry.Offset, CultureInfo.InvariantCulture) * Factor);
			}
		}
		Name = prepEntry.Name;
		Min = ToPhysicalValue(presEntry.Min);
		Max = ToPhysicalValue(presEntry.Max);
	}

	internal ScaleEntry(PrepScaleEntry entry)
	{
		Min = Convert.ToDecimal((double)entry.Min, CultureInfo.InvariantCulture);
		Max = Convert.ToDecimal((double)entry.Max, CultureInfo.InvariantCulture);
		Factor = Convert.ToDecimal((double)entry.Factor, CultureInfo.InvariantCulture);
		Offset = Convert.ToDecimal((double)entry.Offset, CultureInfo.InvariantCulture);
		Name = entry.Name;
	}

	internal ScaleEntry(McdDBCompuScale entry)
	{
		Factor = 1m / Convert.ToDecimal(entry.Factor.Value, CultureInfo.InvariantCulture);
		Offset = -(Convert.ToDecimal(entry.Offset.Value, CultureInfo.InvariantCulture) * Factor);
		Name = entry.Name;
		if (entry.Min.HasValue)
		{
			Min = ToPhysicalValue(entry.Min.Value);
		}
		if (entry.Max.HasValue)
		{
			Max = ToPhysicalValue(entry.Max.Value);
		}
	}

	internal decimal ToEcuValue(decimal physicalValue)
	{
		return Math.Round(physicalValue * Factor + Offset, 0, MidpointRounding.AwayFromZero);
	}

	internal decimal ToPhysicalValue(decimal ecuValue)
	{
		return (ecuValue - Offset) / Factor;
	}

	public bool IsValueInRange(float value)
	{
		return IsValueInRange(value.ToDecimal());
	}

	public bool IsValueInRange(decimal value)
	{
		return IsEcuValueInRange(ToEcuValue(value));
	}

	public bool IsEcuValueInRange(decimal ecuValue)
	{
		decimal num = ToEcuValue(Min);
		decimal num2 = ToEcuValue(Max);
		if (ecuValue >= num)
		{
			return ecuValue <= num2;
		}
		return false;
	}

	internal static IEnumerable<ScaleEntry> GetScales(CaesarEcu ecuHandle, CaesarPreparation preparation, CaesarPresentation presentation)
	{
		for (uint i = 0u; i < preparation.NumberOfScales; i++)
		{
			yield return (presentation != null) ? new ScaleEntry(preparation.GetScaleEntry(ecuHandle, i), presentation.GetScaleEntry(ecuHandle, i)) : new ScaleEntry(preparation.GetScaleEntry(ecuHandle, i));
		}
	}
}
