using System.Globalization;
using Softing.Dts;

namespace McdAbstraction;

public class McdValue
{
	public object Value { get; private set; }

	public object CodedValue { get; private set; }

	public bool IsValueValid { get; private set; }

	internal McdValue(MCDValue mcdValue, MCDValue mcdCodedValue = null)
	{
		IsValueValid = mcdValue != null;
		if (mcdValue != null)
		{
			Value = GetValue(mcdValue);
		}
		else if (mcdCodedValue != null)
		{
			Value = string.Format(CultureInfo.InvariantCulture, "Invalid value (${0:X})", GetValue(mcdCodedValue));
		}
		if (mcdCodedValue != null)
		{
			CodedValue = GetValue(mcdCodedValue);
		}
	}

	internal McdValue(MCDScaleConstraint constraint, MCDValue mcdCodedValue)
	{
		string shortLabel = constraint.ShortLabel;
		IsValueValid = !string.IsNullOrEmpty(shortLabel);
		if (!string.IsNullOrEmpty(shortLabel))
		{
			Value = shortLabel;
		}
		else if (mcdCodedValue != null)
		{
			Value = string.Format(CultureInfo.InvariantCulture, "Invalid value (${0:X})", GetValue(mcdCodedValue));
		}
		if (mcdCodedValue != null)
		{
			CodedValue = GetValue(mcdCodedValue);
		}
	}

	private static object GetValue(MCDValue mcdValue)
	{
		object obj = null;
		return mcdValue.DataType switch
		{
			MCDDataType.eA_UINT32 => mcdValue.Uint32, 
			MCDDataType.eA_UINT16 => mcdValue.Uint16, 
			MCDDataType.eA_UINT64 => mcdValue.Uint64, 
			MCDDataType.eA_UINT8 => mcdValue.Uint8, 
			MCDDataType.eA_INT16 => mcdValue.Int16, 
			MCDDataType.eA_INT32 => mcdValue.Int32, 
			MCDDataType.eA_INT64 => mcdValue.Int64, 
			MCDDataType.eA_INT8 => mcdValue.Int8, 
			MCDDataType.eA_FLOAT32 => mcdValue.Float32, 
			MCDDataType.eA_FLOAT64 => mcdValue.Float64, 
			MCDDataType.eA_ASCIISTRING => mcdValue.Asciistring, 
			MCDDataType.eA_UNICODE2STRING => mcdValue.Unicode2string, 
			MCDDataType.eA_BYTEFIELD => mcdValue.Bytefield, 
			_ => mcdValue.ValueAsString, 
		};
	}
}
