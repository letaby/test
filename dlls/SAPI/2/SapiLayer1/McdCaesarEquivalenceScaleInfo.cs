using System;
using System.Collections.Generic;
using System.Linq;
using McdAbstraction;

namespace SapiLayer1;

internal class McdCaesarEquivalenceScaleInfo
{
	internal List<ScaleEntry> Scales { get; private set; }

	internal double? Factor { get; private set; }

	internal double? Offset { get; private set; }

	internal ConversionType? ConversionType { get; private set; }

	internal decimal? Min { get; private set; }

	internal decimal? Max { get; private set; }

	internal Coding? Coding { get; private set; }

	internal ByteOrder? ByteOrder { get; private set; }

	internal ScaleEntry FactorOffsetScale { get; private set; }

	internal McdCaesarEquivalenceScaleInfo(Type type, McdDBDataObjectProp dataObjectProp)
	{
		Coding = ((dataObjectProp.CodedType == typeof(int)) ? SapiLayer1.Coding.TwosComplement : SapiLayer1.Coding.Unsigned);
		ByteOrder = ((!dataObjectProp.IsHighLowByteOrder) ? SapiLayer1.ByteOrder.LowHigh : SapiLayer1.ByteOrder.HighLow);
		if (type != typeof(Dump) && type != typeof(Choice) && dataObjectProp.CodedType != typeof(float))
		{
			List<ScaleEntry> list = new List<ScaleEntry>();
			foreach (McdDBCompuScale scaleEntry in dataObjectProp.ScaleEntries)
			{
				if (scaleEntry.Factor.HasValue && scaleEntry.Offset.HasValue)
				{
					if (!Factor.HasValue && !Offset.HasValue)
					{
						Factor = scaleEntry.Factor.Value;
						Offset = scaleEntry.Offset.Value;
					}
					if (scaleEntry.Max.HasValue && scaleEntry.Min.HasValue)
					{
						list.Add(new ScaleEntry(scaleEntry));
					}
					else if (FactorOffsetScale == null)
					{
						FactorOffsetScale = new ScaleEntry(scaleEntry);
					}
				}
			}
			if (list.Any())
			{
				IEnumerable<ScaleEntry> source = list.Where((ScaleEntry scale) => !scale.IsFixedValue);
				if (source.Any())
				{
					Min = source.Min((ScaleEntry scale) => scale.Min);
					Max = source.Max((ScaleEntry scale) => scale.Max);
				}
				Scales = list;
				ConversionType = SapiLayer1.ConversionType.Scale;
			}
			else
			{
				ConversionType = ((!Factor.HasValue) ? SapiLayer1.ConversionType.Raw : SapiLayer1.ConversionType.FactorOffset);
			}
		}
		else
		{
			ConversionType = ((dataObjectProp.CodedType == typeof(float)) ? SapiLayer1.ConversionType.Ieee : ((!(type == typeof(Dump))) ? SapiLayer1.ConversionType.Scale : SapiLayer1.ConversionType.Dump));
		}
	}
}
