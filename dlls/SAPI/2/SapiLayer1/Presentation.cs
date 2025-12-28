using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public class Presentation
{
	private static CultureInfo EnglishUSCulture = new CultureInfo("en-US");

	private bool isPropA;

	private McdDBResponseParameter mcdResponseParameter;

	private bool isDiagJob;

	private bool isActualData;

	internal Channel channel;

	private McdCaesarEquivalenceScaleInfo mcdEquivalentScaleInfo;

	private ushort index;

	private string name;

	private string description;

	private decimal? max;

	private decimal? min;

	private string units;

	private Type type;

	private Type dataInterfaceType;

	private ChoiceCollection choices;

	private object precision;

	private decimal? factor;

	private decimal? offset;

	private Ecu ecu;

	private int? bytePos;

	private int? bitPos;

	private int? byteLength;

	private int? bitLength;

	private Coding? coding;

	private TypeSpecifier? typeSpecifier;

	private ByteOrder? byteOrder;

	private DataType? dataType;

	private ConversionType? conversionType;

	private McdCaesarEquivalenceScaleInfo McdEquivalentScaleInfo
	{
		get
		{
			if (mcdEquivalentScaleInfo == null && mcdResponseParameter != null && mcdResponseParameter.PresentationQualifier != null)
			{
				mcdEquivalentScaleInfo = channel.GetMcdCaesarEquivalenceScaleInfo(mcdResponseParameter.PresentationQualifier, type, () => mcdResponseParameter.DataObjectProp);
			}
			return mcdEquivalentScaleInfo;
		}
	}

	internal string McdParameterQualifierPath { get; private set; }

	internal Type DataInterfaceType => dataInterfaceType;

	public int Index => index;

	public string Name => name;

	public string Description
	{
		get
		{
			if (description == null && mcdResponseParameter != null)
			{
				description = mcdResponseParameter.Description;
			}
			return ecu?.Translate(Sapi.MakeTranslationIdentifier(name, "Description"), description) ?? string.Empty;
		}
	}

	public string Units
	{
		get
		{
			if (units == null && mcdResponseParameter != null)
			{
				units = mcdResponseParameter.Unit ?? string.Empty;
			}
			return units;
		}
	}

	public decimal? Max
	{
		get
		{
			if (!max.HasValue && mcdResponseParameter != null && isActualData && McdEquivalentScaleInfo != null)
			{
				max = McdEquivalentScaleInfo.Max;
			}
			return max;
		}
	}

	public decimal? Min
	{
		get
		{
			if (!min.HasValue && mcdResponseParameter != null && isActualData && McdEquivalentScaleInfo != null)
			{
				min = McdEquivalentScaleInfo.Min;
			}
			return min;
		}
	}

	public Type Type => type;

	public ChoiceCollection Choices
	{
		get
		{
			if (isActualData && type == typeof(Choice) && mcdResponseParameter != null && choices.Count == 0)
			{
				choices.Add(mcdResponseParameter.TextTableElements);
			}
			return choices;
		}
	}

	public object Precision
	{
		get
		{
			if (precision == null && mcdResponseParameter != null && mcdResponseParameter.DecimalPlaces.HasValue)
			{
				precision = mcdResponseParameter.DecimalPlaces.Value;
			}
			return precision;
		}
	}

	public decimal? Factor
	{
		get
		{
			if (!factor.HasValue && mcdResponseParameter != null && isActualData && McdEquivalentScaleInfo != null)
			{
				factor = Convert.ToDecimal(McdEquivalentScaleInfo.Factor, CultureInfo.InvariantCulture);
			}
			return factor;
		}
	}

	public decimal? Offset
	{
		get
		{
			if (!offset.HasValue && mcdResponseParameter != null && isActualData && McdEquivalentScaleInfo != null)
			{
				offset = Convert.ToDecimal(McdEquivalentScaleInfo.Offset, CultureInfo.InvariantCulture);
			}
			return offset;
		}
	}

	public int? BytePosition
	{
		get
		{
			if (!bytePos.HasValue && mcdResponseParameter != null && !isDiagJob && !mcdResponseParameter.IsArray && !mcdResponseParameter.IsNoType && !mcdResponseParameter.IsEnvironmentalData)
			{
				bytePos = (int)mcdResponseParameter.BytePos;
			}
			return bytePos;
		}
	}

	public int? BitPosition
	{
		get
		{
			if (!bitPos.HasValue && mcdResponseParameter != null && !isDiagJob && !mcdResponseParameter.IsArray && !mcdResponseParameter.IsStructure && !mcdResponseParameter.IsMultiplexer && !mcdResponseParameter.IsNoType && !mcdResponseParameter.IsEnvironmentalData)
			{
				bitPos = mcdResponseParameter.BitPos;
			}
			return bitPos;
		}
	}

	public int? ByteLength
	{
		get
		{
			if (!byteLength.HasValue && mcdResponseParameter != null && !mcdResponseParameter.IsArray && !mcdResponseParameter.IsMultiplexer && !mcdResponseParameter.IsNoType && !mcdResponseParameter.IsEnvironmentalData)
			{
				byteLength = (int)mcdResponseParameter.ByteLength;
			}
			return byteLength;
		}
	}

	public int? BitLength
	{
		get
		{
			if (!bitLength.HasValue && mcdResponseParameter != null && !mcdResponseParameter.IsArray && !mcdResponseParameter.IsMultiplexer && !mcdResponseParameter.IsNoType && !mcdResponseParameter.IsEnvironmentalData)
			{
				bitLength = (int)mcdResponseParameter.BitLength;
			}
			return bitLength;
		}
	}

	public Coding? Coding
	{
		get
		{
			if (!coding.HasValue && mcdResponseParameter != null && isActualData)
			{
				coding = ((McdEquivalentScaleInfo != null) ? McdEquivalentScaleInfo.Coding : new Coding?(SapiLayer1.Coding.Unsigned));
			}
			return coding;
		}
	}

	public ByteOrder? ByteOrder
	{
		get
		{
			if (!byteOrder.HasValue && mcdResponseParameter != null && isActualData)
			{
				byteOrder = ((McdEquivalentScaleInfo != null) ? McdEquivalentScaleInfo.ByteOrder : new ByteOrder?(SapiLayer1.ByteOrder.HighLow));
			}
			return byteOrder;
		}
	}

	public ConversionType? ConversionSelector
	{
		get
		{
			if (!conversionType.HasValue && mcdResponseParameter != null && isActualData)
			{
				conversionType = ((McdEquivalentScaleInfo != null) ? McdEquivalentScaleInfo.ConversionType : new ConversionType?(ConversionType.Raw));
			}
			return conversionType;
		}
	}

	public TypeSpecifier? TypeSpecifier
	{
		get
		{
			if (!typeSpecifier.HasValue && mcdResponseParameter != null && isActualData)
			{
				typeSpecifier = SapiLayer1.TypeSpecifier.Standard;
			}
			return typeSpecifier;
		}
	}

	public DataType? DataType
	{
		get
		{
			if (!dataType.HasValue && mcdResponseParameter != null && isActualData)
			{
				dataType = ((BitLength.Value % 8 != 0) ? SapiLayer1.DataType.Bit : SapiLayer1.DataType.Byte);
			}
			return dataType;
		}
	}

	public int? SlotType { get; private set; }

	public bool IsStructure
	{
		get
		{
			if (mcdResponseParameter == null)
			{
				return false;
			}
			return mcdResponseParameter.IsStructure;
		}
	}

	public bool IsArrayDefinition
	{
		get
		{
			if (mcdResponseParameter == null)
			{
				return false;
			}
			return mcdResponseParameter.IsArray;
		}
	}

	public bool IsMultiplexer
	{
		get
		{
			if (mcdResponseParameter == null)
			{
				return false;
			}
			return mcdResponseParameter.IsMultiplexer;
		}
	}

	public bool IsEnvironmentData
	{
		get
		{
			if (mcdResponseParameter == null)
			{
				return false;
			}
			return mcdResponseParameter.IsEnvironmentalData;
		}
	}

	public bool IsDiagnosticTroubleCode
	{
		get
		{
			if (mcdResponseParameter == null)
			{
				return false;
			}
			return mcdResponseParameter.IsDiagnosticTroubleCode;
		}
	}

	public bool IsNoType
	{
		get
		{
			if (mcdResponseParameter == null)
			{
				return false;
			}
			return mcdResponseParameter.IsNoType;
		}
	}

	public virtual object ManipulatedValue
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	internal Presentation(ushort i)
	{
		ecu = channel?.Ecu;
		name = string.Empty;
		description = string.Empty;
		units = string.Empty;
		index = i;
	}

	internal Presentation(Ecu ecu, string name, ChoiceCollection choices, Type type, string units)
	{
		this.name = name;
		this.ecu = ecu;
		this.choices = choices;
		this.type = (dataInterfaceType = type);
		this.units = units;
	}

	internal void AcquireFromRollCall(Ecu ecu, string qualifier, IDictionary<string, string> content, bool isPropA)
	{
		name = "PRES_" + (qualifier.StartsWith("DT_", StringComparison.Ordinal) ? qualifier.Substring(3) : qualifier);
		double namedPropertyValue = content.GetNamedPropertyValue("Factor", double.NaN);
		double namedPropertyValue2 = content.GetNamedPropertyValue("Offset", double.NaN);
		precision = Math.Max(Sapi.CalculatePrecision(namedPropertyValue), Sapi.CalculatePrecision(namedPropertyValue2));
		factor = ((!double.IsNaN(namedPropertyValue)) ? new decimal?(Convert.ToDecimal(namedPropertyValue, CultureInfo.InvariantCulture)) : ((decimal?)null));
		offset = ((!double.IsNaN(namedPropertyValue2)) ? new decimal?(Convert.ToDecimal(namedPropertyValue2, CultureInfo.InvariantCulture)) : ((decimal?)null));
		this.isPropA = isPropA;
		SlotType = content.GetNamedPropertyValue("SlotType", -1);
		bytePos = content.GetNamedPropertyValue("BytePos", 1);
		bitPos = content.GetNamedPropertyValue("BitPos", 1);
		bitLength = content.GetNamedPropertyValue<int?>("BitLength", null);
		byteOrder = SapiLayer1.ByteOrder.LowHigh;
		if (bitLength.HasValue)
		{
			byteLength = bitLength / 8;
			if (bitLength % 8 != 0)
			{
				byteLength++;
			}
		}
		switch (SlotType)
		{
		case 1:
			type = typeof(string);
			break;
		case 4:
			type = typeof(Dump);
			break;
		default:
			if (namedPropertyValue == 1.0 && namedPropertyValue2 == 0.0)
			{
				string namedPropertyValue3 = content.GetNamedPropertyValue("Choices", string.Empty);
				if (!string.IsNullOrEmpty(namedPropertyValue3))
				{
					type = typeof(Choice);
					choices = new ChoiceCollection(ecu, name);
					choices.Add(namedPropertyValue3);
				}
				else
				{
					type = ((bitLength <= 8) ? typeof(byte) : ((bitLength <= 16) ? typeof(ushort) : typeof(uint)));
				}
			}
			else
			{
				type = typeof(double);
			}
			break;
		}
		this.ecu = ecu;
		units = content.GetNamedPropertyValue("Units", string.Empty);
		if (content.ContainsKey("Min"))
		{
			min = Convert.ToDecimal(content.GetNamedPropertyValue("Min", double.NaN), CultureInfo.InvariantCulture);
		}
		if (content.ContainsKey("Max"))
		{
			max = Convert.ToDecimal(content.GetNamedPropertyValue("Max", double.NaN), CultureInfo.InvariantCulture);
		}
	}

	internal virtual void Acquire(Channel channel, McdDBResponseParameter response, bool isDiagJob = false)
	{
		this.channel = channel;
		ecu = channel.Ecu;
		mcdResponseParameter = response;
		this.isDiagJob = isDiagJob;
		units = null;
		if (response.IsArray || response.IsStructure || response.IsMultiplexer || response.IsNoType || response.IsEnvironmentalData)
		{
			name = ((!string.IsNullOrEmpty(response.Name)) ? response.Name : response.Qualifier);
			type = (dataInterfaceType = response.CodedParameterType);
		}
		else
		{
			name = (isDiagJob ? response.Name : ((response.PresentationName != null) ? ("PRES_" + McdCaesarEquivalence.MakeQualifier(response.PresentationName, isDOPName: true)) : string.Empty));
			choices = new ChoiceCollection(channel.Ecu, name);
			isActualData = true;
			if (response.DataType == typeof(McdTextTableElement))
			{
				type = (dataInterfaceType = typeof(Choice));
			}
			else if (response.IsDiagnosticTroubleCode)
			{
				type = typeof(uint);
			}
			else
			{
				type = (dataInterfaceType = ((response.DataType != typeof(byte[])) ? response.DataType : typeof(Dump)));
			}
		}
		McdParameterQualifierPath = response.QualifierPath;
	}

	internal void Acquire(Channel channel, Presentation presentation)
	{
		this.channel = channel;
		isActualData = presentation.isActualData;
		isDiagJob = presentation.isDiagJob;
		mcdResponseParameter = presentation.mcdResponseParameter;
		mcdEquivalentScaleInfo = presentation.mcdEquivalentScaleInfo;
		name = presentation.name;
		description = presentation.Description;
		ecu = channel.Ecu;
		bytePos = presentation.bytePos;
		bitPos = presentation.bitPos;
		byteLength = presentation.byteLength;
		bitLength = presentation.bitLength;
		units = presentation.units;
		type = presentation.type;
		dataInterfaceType = presentation.dataInterfaceType;
		choices = presentation.choices;
		factor = presentation.factor;
		offset = presentation.offset;
		min = presentation.min;
		max = presentation.max;
		conversionType = presentation.conversionType;
		coding = presentation.coding;
		byteOrder = presentation.byteOrder;
		typeSpecifier = presentation.typeSpecifier;
		dataType = presentation.dataType;
		precision = presentation.precision;
		McdParameterQualifierPath = presentation.McdParameterQualifierPath;
	}

	internal virtual void Acquire(Channel channel, CaesarDiagService diagService)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Invalid comparison between Unknown and I4
		//IL_05dd: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Invalid comparison between Unknown and I4
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected I4, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected I4, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected I4, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected I4, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected I4, but got Unknown
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Invalid comparison between Unknown and I4
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Invalid comparison between Unknown and I4
		name = diagService.GetPresName((uint)index);
		description = diagService.GetPresDescription((uint)index);
		this.channel = channel;
		ecu = channel.Ecu;
		choices = new ChoiceCollection(channel.Ecu, name);
		ParamType presType = diagService.GetPresType((uint)index);
		type = (dataInterfaceType = Sapi.GetRealCaesarType(presType));
		ushort? num = null;
		if ((int)presType == 6)
		{
			num = diagService.GetPresDecimals(index);
		}
		try
		{
			CaesarPresentation presentation = diagService.GetPresentation((uint)index);
			try
			{
				if (presentation != null)
				{
					bytePos = (int)presentation.BytePosition;
					bitPos = presentation.BitPosition;
					if ((int)presType == 15)
					{
						DumpLengths dumpLengths = presentation.GetDumpLengths();
						if (dumpLengths != null)
						{
							byteLength = ((dumpLengths.StandardLength != 0) ? dumpLengths.StandardLength : dumpLengths.MaxLength);
							bitLength = byteLength * 8;
						}
					}
					else
					{
						byteLength = presentation.ByteLength;
						bitLength = presentation.BitLength;
					}
					coding = (Coding)presentation.Coding;
					typeSpecifier = (TypeSpecifier)presentation.TypeSpecifier;
					byteOrder = (ByteOrder)presentation.ByteOrder;
					dataType = (DataType)presentation.DataType;
					Limits limits = presentation.GetLimits(channel.EcuHandle);
					if (limits.Units != null)
					{
						units = limits.Units;
					}
					if (limits.Max.HasValue)
					{
						max = Convert.ToDecimal((double)limits.Max.Value, CultureInfo.InvariantCulture);
					}
					if (limits.Min.HasValue)
					{
						min = Convert.ToDecimal((double)limits.Min.Value, CultureInfo.InvariantCulture);
					}
					this.conversionType = (ConversionType)presentation.ConversionSelector;
					if ((int)presType == 17)
					{
						if (this.conversionType == ConversionType.Enumeration)
						{
							uint numberEnumerationEntries = presentation.NumberEnumerationEntries;
							for (uint num2 = 0u; num2 < numberEnumerationEntries; num2++)
							{
								DictionaryEntry enumerationEntry = presentation.GetEnumerationEntry(channel.EcuHandle, num2);
								choices.Add(new Choice(enumerationEntry.Key.ToString(), enumerationEntry.Value));
							}
						}
						else if (this.conversionType == ConversionType.Scale)
						{
							uint numberOfScales = presentation.NumberOfScales;
							for (uint num3 = 0u; num3 < numberOfScales; num3++)
							{
								PresScaleEntry scaleEntry = presentation.GetScaleEntry(channel.EcuHandle, num3);
								choices.Add(new Choice(scaleEntry.Name, (int)scaleEntry.Min, (int)scaleEntry.Max));
							}
						}
						if (choices.Count > 0)
						{
							type = typeof(Choice);
						}
					}
					else if ((this.conversionType == ConversionType.Scale || this.conversionType == ConversionType.FactorOffset) && presentation.NumberOfScales != 0)
					{
						List<PresScaleEntry> list = new List<PresScaleEntry>();
						for (uint num4 = 0u; num4 < presentation.NumberOfScales; num4++)
						{
							PresScaleEntry scaleEntry2 = presentation.GetScaleEntry(channel.EcuHandle, num4);
							if (scaleEntry2.Name == null)
							{
								list.Add(scaleEntry2);
							}
						}
						ConversionType? conversionType = this.conversionType;
						ConversionType conversionType2 = ConversionType.Scale;
						if (conversionType.GetValueOrDefault() == conversionType2 && conversionType.HasValue && !min.HasValue && !max.HasValue)
						{
							IEnumerable<float> source = list.Select((PresScaleEntry se) => se.ScaledMin).Union(list.Select((PresScaleEntry se) => se.ScaledMax));
							min = Convert.ToDecimal((double)source.Min(), CultureInfo.InvariantCulture);
							max = Convert.ToDecimal((double)source.Max(), CultureInfo.InvariantCulture);
						}
						PresScaleEntry val = ((list.Count == 1) ? list[0] : null);
						if (val != null)
						{
							factor = Convert.ToDecimal((double)val.Factor, CultureInfo.InvariantCulture);
							offset = Convert.ToDecimal((double)val.Offset, CultureInfo.InvariantCulture);
							ushort num5 = (ushort)Math.Max(Sapi.CalculatePrecision(val.Factor), Sapi.CalculatePrecision(val.Offset));
							num = (num.HasValue ? new ushort?(Math.Min(num.Value, num5)) : new ushort?(num5));
						}
						if (type == null && (channel.ChannelHandle == null || (int)diagService.ServiceType == 128))
						{
							type = typeof(float);
						}
					}
				}
			}
			finally
			{
				((IDisposable)presentation)?.Dispose();
			}
		}
		catch (CaesarErrorException ex)
		{
			CaesarException e = new CaesarException(ex, null, null);
			Sapi.GetSapi().RaiseExceptionEvent(this, e);
		}
		if (num.HasValue)
		{
			precision = num;
		}
	}

	internal object GetPresentation(byte[] data)
	{
		return GetPresentation(data, BytePosition.Value, BitPosition.Value);
	}

	internal object GetPresentation(byte[] data, int bytePos, int bitPos)
	{
		if (channel == null || channel.IsRollCall)
		{
			bytePos--;
			bitPos--;
		}
		return Decode(data, bytePos, bitPos, ByteLength, BitLength, ByteOrder, type, isPropA, SlotType, Factor, Offset, Min, Max, Choices, ecu, name);
	}

	internal static object Decode(byte[] data, int bytePos, int bitPos, int? byteLength, int? bitLength, ByteOrder? byteOrder, Type type, bool isPropA, int? slotType, decimal? factor, decimal? offset, decimal? min, decimal? max, ChoiceCollection choices, Ecu ecu, string name)
	{
		object obj = null;
		if (isPropA && slotType.Value == 0)
		{
			string text = string.Join("", data.Select((byte b) => Convert.ToString(b, 2).PadLeft(8, '0')));
			int num = bytePos * 8 + (8 - bitPos);
			int? num2 = num - bitLength;
			int num3 = 0;
			if (num2.GetValueOrDefault() < num3 || !num2.HasValue || num > text.Length)
			{
				throw new CaesarException(SapiError.BytePosGreaterThanMessageLength);
			}
			obj = Convert.ToInt32(text.Substring(num - bitLength.Value, bitLength.Value), 2);
		}
		else if (bitLength.HasValue && bitLength.Value > 0 && bitLength.Value < 8)
		{
			if (bytePos >= data.Length)
			{
				throw new CaesarException(SapiError.BytePosGreaterThanMessageLength);
			}
			obj = (data[bytePos] >> bitPos) & ((1 << bitLength.Value) - 1);
		}
		else
		{
			if (byteLength.HasValue && bytePos + byteLength.Value > data.Length)
			{
				throw new CaesarException(SapiError.BytePosGreaterThanMessageLength);
			}
			bool flag = type == typeof(Dump) || type == typeof(string) || byteOrder.ByteOrderMatchesSystem();
			int num4 = (byteLength.HasValue ? byteLength.Value : (data.Length - bytePos));
			byte[] array = new byte[num4];
			for (int num5 = 0; num5 < num4; num5++)
			{
				array[num5] = (flag ? data[bytePos + num5] : data[bytePos + (num4 - 1 - num5)]);
			}
			if (type == typeof(Dump))
			{
				obj = new Dump(array);
			}
			else if (type != typeof(string))
			{
				if (ecu != null && ecu.ProtocolName == "J1939" && !isPropA && array[array.Length - 1] >= 251)
				{
					return string.Format(CultureInfo.InvariantCulture, ecu.Translate(Sapi.MakeTranslationIdentifier(array[array.Length - 1].ToString(CultureInfo.InvariantCulture), "Range"), "Invalid value (${0})"), new Dump(array.Reverse()));
				}
				switch (array.Length)
				{
				case 2:
				{
					ushort num7 = BitConverter.ToUInt16(array, 0);
					if (bitLength.Value % 8 != 0)
					{
						num7 &= (ushort)((1 << bitLength.Value) - 1);
					}
					obj = num7;
					break;
				}
				case 4:
				{
					uint num6 = BitConverter.ToUInt32(array, 0);
					if (bitLength.Value % 8 != 0)
					{
						num6 &= (uint)((1 << bitLength.Value) - 1);
					}
					obj = num6;
					break;
				}
				case 1:
					obj = array[0];
					break;
				default:
					Sapi.GetSapi().RaiseDebugInfoEvent(name, "Don't know how to process a value " + array.Length + " bytes long");
					break;
				}
			}
			else
			{
				obj = Encoding.ASCII.GetString(array);
			}
		}
		if (obj != null && type != typeof(string) && type != typeof(Dump))
		{
			if (factor.HasValue && offset.HasValue)
			{
				decimal? num8 = factor;
				if (!(num8.GetValueOrDefault() == default(decimal)) || !num8.HasValue)
				{
					num8 = factor;
					decimal num9 = 1;
					if (num8.GetValueOrDefault() == num9 && num8.HasValue)
					{
						num8 = offset;
						if (num8.GetValueOrDefault() == default(decimal) && num8.HasValue)
						{
							goto IL_0499;
						}
					}
					num9 = Convert.ToDecimal(obj, CultureInfo.InvariantCulture);
					decimal? num10 = factor;
					obj = (double)((decimal?)num9 * num10 + offset).Value;
					goto IL_04d5;
				}
			}
			goto IL_0499;
		}
		goto IL_0529;
		IL_0529:
		return obj;
		IL_0499:
		if (type == typeof(Choice))
		{
			Choice itemFromRawValue = choices.GetItemFromRawValue(obj);
			obj = ((itemFromRawValue != null) ? ((object)itemFromRawValue) : ((object)string.Format(CultureInfo.InvariantCulture, "Invalid value (${0:X})", obj)));
		}
		goto IL_04d5;
		IL_04d5:
		if (min.HasValue && Convert.ToDouble(obj, CultureInfo.InvariantCulture) < (double)min.Value)
		{
			obj = "*";
		}
		else if (max.HasValue && Convert.ToDouble(obj, CultureInfo.InvariantCulture) > (double)max.Value)
		{
			obj = "*";
		}
		goto IL_0529;
	}

	internal object GetPresentation(McdDiagComPrimitive diagServiceIO)
	{
		if (!diagServiceIO.IsNegativeResponse)
		{
			return GetPresentation(diagServiceIO.AllPositiveResponseParameters.Where((McdResponseParameter pr) => pr.QualifierPath == McdParameterQualifierPath).ToList());
		}
		return diagServiceIO.NegativeResponseParameter.Value.Value;
	}

	internal object GetPresentation(List<McdResponseParameter> responseParameter)
	{
		return responseParameter.Count switch
		{
			0 => null, 
			1 => GetPresentation(responseParameter[0]), 
			_ => responseParameter.Select((McdResponseParameter p) => GetPresentation(p)).ToArray(), 
		};
	}

	internal object GetPresentation(McdResponseParameter responseParameter)
	{
		McdValue mcdValue = responseParameter?.Value;
		if (type == typeof(Choice) && !responseParameter.IsValueValid)
		{
			return mcdValue.Value;
		}
		return mcdValue?.GetValue(type, Choices);
	}

	internal object GetPresentation(CaesarDiagServiceIO diagServiceIO)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Invalid comparison between Unknown and I4
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Invalid comparison between Unknown and I4
		ParamType presType = diagServiceIO.GetPresType(index);
		if (type == null && (int)presType != 17 && (int)presType != 7)
		{
			type = Sapi.GetRealCaesarType(presType);
		}
		try
		{
			if (type == typeof(Choice) && !diagServiceIO.IsNegativeResponse)
			{
				uint presParamInternal = diagServiceIO.GetPresParamInternal(index);
				if (diagServiceIO.IsNoMatchingInterval(index))
				{
					return string.Format(CultureInfo.InvariantCulture, "Invalid value (${0:X})", presParamInternal);
				}
				if (Choices.Type == typeof(int))
				{
					return Choices.GetItemFromRawValue(BitConverter.ToInt32(BitConverter.GetBytes(presParamInternal), 0));
				}
				return Choices.GetItemFromRawValue(presParamInternal);
			}
			object obj = diagServiceIO.GetPresParam(index, presType);
			ConversionType? conversionType = this.conversionType;
			ConversionType conversionType2 = ConversionType.Ieee;
			if (conversionType.GetValueOrDefault() == conversionType2 && conversionType.HasValue && obj != null && (int)presType == 6 && ecu.SignalNotAvailableValue != null)
			{
				if (BitConverter.GetBytes((float)obj).SequenceEqual(ecu.SignalNotAvailableValue.Data))
				{
					obj = "sna";
				}
			}
			else if ((int)presType == 15)
			{
				obj = new Dump((byte[])obj);
			}
			return obj;
		}
		catch (InvalidCastException)
		{
			Sapi.GetSapi().RaiseExceptionEvent(this, new CaesarException(SapiError.UnknownPresentationType));
			return string.Empty;
		}
	}

	internal void SetType(Type type)
	{
		this.type = type;
	}

	internal void SetPrecision(ushort precision)
	{
		if (this.precision == null)
		{
			this.precision = precision;
		}
	}

	internal object ParseFromLog(string sourceValue)
	{
		return ParseFromLog(sourceValue, Type, Choices, ecu);
	}

	internal static object ParseFromLog(string sourceValue, Type type, ChoiceCollection choices, Ecu ecu)
	{
		if (type != null && (type.IsArray || (type != typeof(string) && sourceValue.IndexOf(',') != -1)))
		{
			string[] array = sourceValue.Split(",".ToCharArray(), StringSplitOptions.None);
			Type type2 = (type.IsArray ? type.GetElementType() : type);
			Array array2 = Array.CreateInstance(typeof(object), array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				object obj = ParseFromLog(array[i], type2, choices, ecu);
				if (i == 0 && array.Length == 1 && obj != null && obj.GetType() != type2)
				{
					return obj;
				}
				array2.SetValue(obj, i);
			}
			return array2;
		}
		object obj2 = sourceValue;
		if (choices != null && (type == typeof(Choice) || choices.Count > 0))
		{
			obj2 = ((!ecu.IsRollCall || (!(type == typeof(byte)) && !(type == typeof(uint)) && !(type == typeof(ushort)))) ? choices.GetItemFromLogValue(sourceValue) : choices.GetItemFromRawValue(sourceValue));
			if (obj2 == null)
			{
				obj2 = sourceValue;
			}
		}
		else if (type != null)
		{
			try
			{
				obj2 = ((!(type == typeof(Dump))) ? Convert.ChangeType(sourceValue, type, EnglishUSCulture) : new Dump(sourceValue));
			}
			catch (FormatException)
			{
			}
			catch (InvalidCastException)
			{
			}
			catch (ArgumentException)
			{
			}
		}
		return obj2;
	}

	internal static string FormatForLog(object sourceValue)
	{
		string result = string.Empty;
		if (sourceValue != null)
		{
			if (sourceValue.GetType().IsArray)
			{
				return string.Join(",", (from object source in sourceValue as Array
					select FormatForLog(source)).ToArray());
			}
			if (sourceValue.GetType() != typeof(Choice))
			{
				result = Convert.ToString(sourceValue, EnglishUSCulture);
			}
			else
			{
				Choice choice = (Choice)sourceValue;
				result = string.Format(CultureInfo.InvariantCulture, "#{0}", choice.RawValue);
			}
		}
		return result;
	}

	internal void AddStringsForTranslation(Dictionary<string, string> table)
	{
		if (!string.IsNullOrEmpty(description))
		{
			table[Sapi.MakeTranslationIdentifier(name, "Description")] = description;
		}
		if (Choices == null)
		{
			return;
		}
		foreach (Choice choice in Choices)
		{
			choice.AddStringsForTranslation(table);
		}
	}
}
