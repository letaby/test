using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CaesarAbstraction;

namespace SapiLayer1;

internal class VarcodeCaesar : Varcode
{
	private Channel channel;

	private CaesarVarcode caesarVarcoding;

	private Dictionary<Parameter, CaesarFragment> writableFragments = new Dictionary<Parameter, CaesarFragment>();

	private byte? negativeResponseCode;

	internal VarcodeCaesar(CaesarEcu caesarEcu)
	{
		caesarVarcoding = caesarEcu.InitOfflineVarcoding();
	}

	internal VarcodeCaesar(Channel channel)
	{
		this.channel = channel;
		caesarVarcoding = channel.ChannelHandle.VCInit();
	}

	internal override void DoCoding()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		negativeResponseCode = null;
		Sapi sapi = Sapi.GetSapi();
		sapi.ByteMessageInternalEvent += Sapi_ByteMessageInternalEvent;
		caesarVarcoding.DoCoding();
		sapi.ByteMessageInternalEvent -= Sapi_ByteMessageInternalEvent;
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), negativeResponseCode, channel.Ecu.DiagnosisProtocol) : null);
	}

	private void Sapi_ByteMessageInternalEvent(object sender, ByteMessageEventArgs e)
	{
		if (sender as Channel == channel && e.Direction == ByteMessageDirection.RX && e.Data.Data.Count >= 3 && e.Data.Data[0] == 127 && e.Data.Data[1] == 46)
		{
			negativeResponseCode = e.Data.Data[2];
		}
	}

	internal override bool AllowSetDefaultString(string groupQualifier)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		bool result = caesarVarcoding.AllowSetDefaultString(groupQualifier);
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
		return result;
	}

	internal override void EnableReadCodingStringFromEcu(bool enableReadCodingStringFromEcu)
	{
		caesarVarcoding.EnableReadCodingStringFromEcu(enableReadCodingStringFromEcu);
	}

	internal override byte[] GetCurrentCodingString(string groupQualifier)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		byte[] currentCodingString = caesarVarcoding.GetCurrentCodingString(groupQualifier);
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
		return currentCodingString;
	}

	internal override void SetCurrentCodingString(string groupQualifier, byte[] content)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		caesarVarcoding.SetCurrentCodingString(groupQualifier, content);
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
	}

	internal override void SetDefaultStringByPartNumber(string partNumber)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		caesarVarcoding.SetDefaultStringByPartNumber(partNumber);
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
	}

	internal override void SetDefaultStringByPartNumberAndPartVersion(string partNumber, uint partVersion)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		caesarVarcoding.SetDefaultStringByPartNumberAndPartVersion(partNumber, partVersion);
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
	}

	internal override void SetFragmentMeaningByPartNumber(string partNumber)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		caesarVarcoding.SetFragmentMeaningByPartNumber(partNumber);
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
	}

	internal override void SetFragmentMeaningByPartNumberAndPartVersion(string partNumber, uint partVersion)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		caesarVarcoding.SetFragmentMeaningByPartNumberAndPartVersion(partNumber, partVersion);
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
	}

	internal void SetFragmentMeaningByIndex(Parameter parameter, int index)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		if (parameter != null)
		{
			if (!writableFragments.TryGetValue(parameter, out var value))
			{
				value = caesarVarcoding.GetFragmentByIndex(parameter.CaesarIndex);
				if (value != null)
				{
					writableFragments[parameter] = value;
				}
			}
			if (value != null)
			{
				value.SetMeaningByIndex(caesarVarcoding, index);
			}
			base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
		}
		else
		{
			base.Exception = new CaesarException(SapiError.NoMatchingPartNumberAndPartVersionFound);
		}
	}

	internal override void SetFragmentValue(Parameter parameter, object newValue)
	{
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Invalid comparison between Unknown and I4
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Invalid comparison between Unknown and I4
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		if (parameter.Type == typeof(double))
		{
			object obj = newValue;
			bool num = obj is double;
			double value = (num ? ((double)obj) : 0.0);
			if (num)
			{
				decimal decimalValue = Convert.ToDecimal(value);
				ScaleEntry scaleEntry = ((parameter.ConversionSelector != ConversionType.FactorOffset) ? parameter.Scales.FirstOrDefault((ScaleEntry sc) => sc.IsValueInRange(decimalValue)) : (parameter.IsValueInFactorOffsetScaleRange(decimalValue) ? parameter.FactorOffsetScale : null));
				if (scaleEntry != null)
				{
					decimal ecuValue = scaleEntry.ToEcuValue(decimalValue);
					long value2 = parameter.ByteLength.Value;
					byte[] array = Encode(ecuValue, parameter.Coding.Value, value2, parameter.Qualifier);
					byte[] currentCodingString = caesarVarcoding.GetCurrentCodingString(parameter.GroupQualifier);
					long value3 = parameter.BytePosition.Value;
					bool flag = parameter.ByteOrder.ByteOrderMatchesSystem();
					for (int num2 = 0; num2 < value2; num2++)
					{
						currentCodingString[value3 + num2] = (flag ? array[num2] : array[value2 - 1 - num2]);
					}
					caesarVarcoding.SetCurrentCodingString(parameter.GroupQualifier, currentCodingString);
					goto IL_0242;
				}
				base.Exception = new CaesarException(SapiError.PreparationValueOutOfLimits);
				return;
			}
		}
		if (!writableFragments.TryGetValue(parameter, out var value4))
		{
			value4 = caesarVarcoding.GetFragmentByIndex(parameter.CaesarIndex);
			if (value4 != null)
			{
				writableFragments[parameter] = value4;
			}
		}
		if (value4 != null)
		{
			ParamType paramType = parameter.ParamType;
			if ((int)paramType != 15)
			{
				if ((int)paramType == 18)
				{
					if (newValue != null && newValue.GetType() == typeof(Choice) && newValue != ChoiceCollection.InvalidChoice)
					{
						newValue = ((Choice)newValue).Index;
						value4.SetValue(caesarVarcoding, (ParamType)18, newValue);
					}
				}
				else
				{
					value4.SetValue(caesarVarcoding, parameter.ParamType, newValue);
				}
			}
			else
			{
				Dump dump = (Dump)newValue;
				value4.SetValue(caesarVarcoding, (ParamType)15, (object)dump.Data.ToArray());
			}
		}
		goto IL_0242;
		IL_0242:
		base.Exception = (caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(caesarVarcoding), null, null) : null);
	}

	internal override object GetFragmentValue(Parameter parameter)
	{
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Expected O, but got Unknown
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Invalid comparison between Unknown and I4
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Invalid comparison between Unknown and I4
		if (parameter.Type == typeof(double))
		{
			byte[] currentCodingString = caesarVarcoding.GetCurrentCodingString(parameter.GroupQualifier);
			if (!caesarVarcoding.IsErrorSet)
			{
				base.Exception = null;
				long value = parameter.BytePosition.Value;
				long value2 = parameter.ByteLength.Value;
				bool flag = parameter.ByteOrder.ByteOrderMatchesSystem();
				byte[] array = new byte[value2];
				for (int i = 0; i < value2; i++)
				{
					array[i] = (flag ? currentCodingString[value + i] : currentCodingString[value + (value2 - 1 - i)]);
				}
				decimal ecuValue = Decode(array, parameter.Coding.Value, parameter.Qualifier);
				ScaleEntry scaleEntry = ((parameter.ConversionSelector == ConversionType.FactorOffset) ? parameter.FactorOffsetScale : parameter.Scales.FirstOrDefault((ScaleEntry sc) => sc.IsEcuValueInRange(ecuValue)));
				if (scaleEntry != null)
				{
					return (double)scaleEntry.ToPhysicalValue(ecuValue);
				}
				base.Exception = new CaesarException(SapiError.NoMatchingIntervalWasFound);
			}
			else
			{
				base.Exception = new CaesarException(new CaesarErrorException(caesarVarcoding), null, null);
			}
		}
		else
		{
			CaesarFragment fragmentByIndex = caesarVarcoding.GetFragmentByIndex(parameter.CaesarIndex);
			object obj = ((fragmentByIndex != null) ? fragmentByIndex.GetValue(caesarVarcoding, parameter.ParamType) : null);
			if (!caesarVarcoding.IsErrorSet)
			{
				base.Exception = null;
				ParamType paramType = parameter.ParamType;
				if ((int)paramType != 15)
				{
					if ((int)paramType == 18)
					{
						uint num = Convert.ToUInt32(obj, CultureInfo.InvariantCulture);
						if (num < (uint)parameter.Choices.Count)
						{
							return parameter.Choices[(int)num];
						}
						return ChoiceCollection.InvalidChoice;
					}
					return obj;
				}
				return new Dump((byte[])obj);
			}
			base.Exception = new CaesarException(new CaesarErrorException(caesarVarcoding), null, null);
		}
		return null;
	}

	private static byte[] Encode(decimal ecuValue, Coding coding, long byteLen, string qualifier)
	{
		switch (coding)
		{
		case Coding.Unsigned:
		{
			long num2 = byteLen - 1;
			if ((ulong)num2 <= 3uL)
			{
				switch (num2)
				{
				case 0L:
					return BitConverter.GetBytes(Convert.ToByte(ecuValue, CultureInfo.InvariantCulture));
				case 1L:
					return BitConverter.GetBytes(Convert.ToUInt16(ecuValue, CultureInfo.InvariantCulture));
				case 3L:
					return BitConverter.GetBytes(Convert.ToUInt32(ecuValue, CultureInfo.InvariantCulture));
				}
			}
			throw new InvalidOperationException("Parameter " + qualifier + " has unhandled length");
		}
		case Coding.TwosComplement:
		{
			long num = byteLen - 1;
			if ((ulong)num <= 3uL)
			{
				switch (num)
				{
				case 0L:
					return BitConverter.GetBytes(Convert.ToSByte(ecuValue, CultureInfo.InvariantCulture));
				case 1L:
					return BitConverter.GetBytes(Convert.ToInt16(ecuValue, CultureInfo.InvariantCulture));
				case 3L:
					return BitConverter.GetBytes(Convert.ToInt32(ecuValue, CultureInfo.InvariantCulture));
				}
			}
			throw new InvalidOperationException("Parameter " + qualifier + " has unhandled length " + byteLen);
		}
		default:
			throw new InvalidOperationException("Parameter " + qualifier + " has unhandled coding " + coding);
		}
	}

	private static decimal Decode(byte[] codingValue, Coding coding, string qualifier)
	{
		return coding switch
		{
			Coding.Unsigned => codingValue.Length switch
			{
				1 => codingValue[0], 
				2 => BitConverter.ToUInt16(codingValue, 0), 
				4 => BitConverter.ToUInt32(codingValue, 0), 
				_ => throw new InvalidOperationException("Parameter " + qualifier + " has unhandled length" + codingValue.Length), 
			}, 
			Coding.TwosComplement => codingValue.Length switch
			{
				1 => ((codingValue[0] & 0x80) != 0) ? ((sbyte)(-128 + (byte)(codingValue[0] & 0x7F))) : ((sbyte)codingValue[0]), 
				2 => BitConverter.ToInt16(codingValue, 0), 
				4 => BitConverter.ToInt32(codingValue, 0), 
				_ => throw new InvalidOperationException("Parameter " + qualifier + " has unhandled length " + codingValue.Length), 
			}, 
			_ => throw new InvalidOperationException("Parameter " + qualifier + " has unhandled coding " + coding), 
		};
	}

	protected override void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			if (writableFragments != null)
			{
				writableFragments.Clear();
				writableFragments = null;
			}
			if (caesarVarcoding != null)
			{
				((CaesarHandle_003CCaesar_003A_003AVarcodeHandleStruct_0020_002A_003E)caesarVarcoding).Dispose();
				caesarVarcoding = null;
			}
		}
		disposedValue = true;
	}
}
