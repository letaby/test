using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class ServiceInputValue
{
	private McdDBRequestParameter mcdRequestParameter;

	private McdCaesarEquivalenceScaleInfo mcdEquivalentScaleInfo;

	private const string EncryptedValuePrefix = "einput:";

	private bool inputValuePreProcessed;

	private ScaleEntry factorOffsetScale;

	private Service service;

	private ChoiceCollection choices;

	private string name;

	private string qualifier;

	private string description;

	private object value;

	private object defaultValue;

	private string parameterQualifier;

	private decimal? max;

	private decimal? min;

	private string units;

	private ushort indexDI;

	private ushort indexDM;

	private object requiredLength;

	private ParamType pT;

	private Type type;

	private bool makeFit;

	private decimal? factor;

	private decimal? offset;

	private long? bytePos;

	private int? bitPos;

	private long? byteLength;

	private long? bitLength;

	private Coding? coding;

	private TypeSpecifier? typeSpecifier;

	private ByteOrder? byteOrder;

	private DataType? dataType;

	private ConversionType? conversionType;

	private List<ScaleEntry> scales;

	private int? readAccess;

	private int? writeAccess;

	private ServiceArgumentValueCollection argumentValues;

	private McdCaesarEquivalenceScaleInfo McdEquivalentScaleInfo
	{
		get
		{
			if (mcdEquivalentScaleInfo == null && mcdRequestParameter != null && mcdRequestParameter.PreparationQualifier != null)
			{
				mcdEquivalentScaleInfo = service.Channel.GetMcdCaesarEquivalenceScaleInfo(mcdRequestParameter.PreparationQualifier, type, () => mcdRequestParameter.DataObjectProp);
			}
			return mcdEquivalentScaleInfo;
		}
	}

	public CaesarException Exception { get; private set; }

	public Service Service => service;

	public string Qualifier => qualifier;

	public string Description
	{
		get
		{
			if (string.IsNullOrEmpty(description) && mcdRequestParameter != null)
			{
				description = mcdRequestParameter.Description;
			}
			return service.Channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(qualifier, "Description"), description);
		}
	}

	public string Name => service.Channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(qualifier, "Name"), name);

	public string Units
	{
		get
		{
			if (units == null && mcdRequestParameter != null)
			{
				units = mcdRequestParameter.Unit ?? string.Empty;
			}
			return units;
		}
	}

	public object Value
	{
		get
		{
			if (inputValuePreProcessed)
			{
				return "*****";
			}
			return value;
		}
		set
		{
			inputValuePreProcessed = false;
			SetValue(value);
		}
	}

	public string ParameterQualifier => parameterQualifier;

	public decimal? Max
	{
		get
		{
			if (!max.HasValue && mcdRequestParameter != null && McdEquivalentScaleInfo != null)
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
			if (!min.HasValue && mcdRequestParameter != null && McdEquivalentScaleInfo != null)
			{
				min = McdEquivalentScaleInfo.Min;
			}
			return min;
		}
	}

	public Type Type => type;

	public object RequiredLength => requiredLength;

	public int Index => indexDM;

	public ChoiceCollection Choices
	{
		get
		{
			if (type == typeof(Choice) && mcdRequestParameter != null && choices.Count == 0)
			{
				choices.Add(mcdRequestParameter.TextTableElements);
			}
			return choices;
		}
	}

	public object DefaultValue => defaultValue;

	public bool MakeFit
	{
		get
		{
			return makeFit;
		}
		set
		{
			makeFit = value;
		}
	}

	public decimal? Factor
	{
		get
		{
			if (!factor.HasValue && mcdRequestParameter != null && McdEquivalentScaleInfo != null)
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
			if (!offset.HasValue && mcdRequestParameter != null && McdEquivalentScaleInfo != null)
			{
				offset = Convert.ToDecimal(McdEquivalentScaleInfo.Offset, CultureInfo.InvariantCulture);
			}
			return offset;
		}
	}

	public long? BytePosition
	{
		get
		{
			if (!bytePos.HasValue && mcdRequestParameter != null)
			{
				bytePos = mcdRequestParameter.BytePos;
			}
			return bytePos;
		}
	}

	public int? BitPosition
	{
		get
		{
			if (!bitPos.HasValue && mcdRequestParameter != null)
			{
				bitPos = mcdRequestParameter.BitPos;
			}
			return bitPos;
		}
	}

	public long? ByteLength
	{
		get
		{
			if (!byteLength.HasValue && mcdRequestParameter != null)
			{
				byteLength = mcdRequestParameter.ByteLength;
			}
			return byteLength;
		}
	}

	public long? BitLength
	{
		get
		{
			if (!bitLength.HasValue && mcdRequestParameter != null)
			{
				bitLength = mcdRequestParameter.BitLength;
			}
			return bitLength;
		}
	}

	public Coding? Coding
	{
		get
		{
			if (!coding.HasValue && mcdRequestParameter != null)
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
			if (!byteOrder.HasValue && mcdRequestParameter != null)
			{
				byteOrder = ((McdEquivalentScaleInfo != null) ? McdEquivalentScaleInfo.ByteOrder : new ByteOrder?(SapiLayer1.ByteOrder.HighLow));
			}
			return byteOrder;
		}
	}

	public TypeSpecifier? TypeSpecifier
	{
		get
		{
			if (!typeSpecifier.HasValue && mcdRequestParameter != null)
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
			if (!dataType.HasValue && mcdRequestParameter != null)
			{
				dataType = ((BitLength.Value % 8 != 0L) ? SapiLayer1.DataType.Bit : SapiLayer1.DataType.Byte);
			}
			return dataType;
		}
	}

	public ConversionType? ConversionSelector
	{
		get
		{
			if (!conversionType.HasValue && mcdRequestParameter != null)
			{
				conversionType = ((McdEquivalentScaleInfo != null) ? McdEquivalentScaleInfo.ConversionType : new ConversionType?(ConversionType.Raw));
			}
			return conversionType;
		}
	}

	public IEnumerable<ScaleEntry> Scales
	{
		get
		{
			if (scales == null && mcdRequestParameter != null && McdEquivalentScaleInfo != null && ConversionSelector == ConversionType.Scale)
			{
				scales = McdEquivalentScaleInfo.Scales?.ToList();
			}
			if (scales == null)
			{
				return null;
			}
			return scales.AsReadOnly();
		}
	}

	internal ScaleEntry FactorOffsetScale
	{
		get
		{
			if (factorOffsetScale == null && mcdRequestParameter != null && McdEquivalentScaleInfo != null && ConversionSelector == ConversionType.FactorOffset)
			{
				factorOffsetScale = McdEquivalentScaleInfo.FactorOffsetScale;
			}
			return factorOffsetScale;
		}
	}

	public bool IsReserved { get; private set; }

	internal int? ReadAccess => readAccess;

	internal int? WriteAccess => writeAccess;

	private bool PadOrTrim
	{
		get
		{
			bool result = false;
			if (makeFit && requiredLength != null)
			{
				result = Convert.ToInt32(requiredLength, CultureInfo.InvariantCulture) > 0;
			}
			return result;
		}
	}

	public ServiceArgumentValueCollection ArgumentValues => argumentValues;

	internal ServiceInputValue(Service service, ushort indexDI, ushort indexDM)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		this.service = service;
		qualifier = string.Empty;
		name = string.Empty;
		description = string.Empty;
		units = string.Empty;
		this.indexDI = indexDI;
		this.indexDM = indexDM;
		makeFit = true;
		pT = (ParamType)20;
		argumentValues = new ServiceArgumentValueCollection();
	}

	internal void AcquirePreparation(McdDBRequestParameter requestParameter)
	{
		mcdRequestParameter = requestParameter;
		qualifier = ((service.ServiceTypes == ServiceTypes.DiagJob || requestParameter.PreparationName == null) ? requestParameter.Qualifier : ("PREP_" + McdCaesarEquivalence.MakeQualifier(requestParameter.PreparationName, isDOPName: true)));
		IsReserved = requestParameter.IsReserved;
		parameterQualifier = requestParameter.Qualifier;
		name = requestParameter.Name;
		choices = new ChoiceCollection(service.Channel.Ecu, qualifier);
		units = null;
		if (requestParameter.DataType == typeof(McdTextTableElement))
		{
			type = typeof(Choice);
		}
		else
		{
			type = ((requestParameter.DataType != typeof(byte[])) ? requestParameter.DataType : typeof(Dump));
		}
		try
		{
			McdValue mcdValue = requestParameter.GetDefaultValue();
			if (mcdValue != null)
			{
				value = (defaultValue = mcdValue.GetValue(type, choices));
			}
		}
		catch (McdException ex)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, service.Channel.Ecu.Name + "." + service.Qualifier + " input value " + qualifier + " error while retrieving default value+" + ex.Message);
		}
		if (service.ServiceTypes == ServiceTypes.WriteVarCode)
		{
			readAccess = GetSecurityLevel(requestParameter.SpecialData, ".ReadSecurityLevel");
			writeAccess = GetSecurityLevel(requestParameter.SpecialData, ".WriteSecurityLevel");
		}
		static int? GetSecurityLevel(Dictionary<string, string> specialData, string attribute)
		{
			if (specialData.ContainsKey(attribute))
			{
				string text = specialData[attribute];
				if (text.StartsWith("S", StringComparison.OrdinalIgnoreCase))
				{
					return Convert.ToInt32(text.Substring(1));
				}
			}
			return null;
		}
	}

	internal void AcquirePreparation(CaesarDiagService diagService)
	{
		//IL_0485: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Invalid comparison between Unknown and I4
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected I4, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected I4, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected I4, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected I4, but got Unknown
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected I4, but got Unknown
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Invalid comparison between Unknown and I4
		if (service == null)
		{
			throw new InvalidOperationException("AcquirePreparation not valid for a clone");
		}
		qualifier = diagService.GetPrepQualifier((uint)indexDI);
		parameterQualifier = diagService.GetPrepParameterQualifier((uint)indexDI);
		name = diagService.GetPrepName((uint)indexDI);
		description = diagService.GetPrepDescription((uint)indexDI);
		choices = new ChoiceCollection(service.Channel.Ecu, qualifier);
		pT = diagService.GetPrepType((uint)indexDI);
		type = Sapi.GetRealCaesarType(pT);
		if ((int)pT == 18)
		{
			type = typeof(Choice);
			uint prepNumberOfChoices = diagService.GetPrepNumberOfChoices((uint)indexDI);
			for (uint num = 0u; num < prepNumberOfChoices; num++)
			{
				string prepChoiceMeaning = diagService.GetPrepChoiceMeaning((uint)indexDI, num);
				uint prepChoiceValue = diagService.GetPrepChoiceValue((uint)indexDI, num);
				Choices.Add(new Choice(prepChoiceMeaning, prepChoiceValue));
			}
		}
		if ((service.ServiceTypes & ServiceTypes.DiagJob) == 0)
		{
			try
			{
				CaesarPreparation preparation = diagService.GetPreparation(indexDI);
				try
				{
					bytePos = preparation.BytePosition;
					bitPos = preparation.BitPosition;
					byteLength = preparation.ByteLength;
					bitLength = preparation.BitLength;
					coding = (Coding)preparation.Coding;
					typeSpecifier = (TypeSpecifier)preparation.TypeSpecifier;
					byteOrder = (ByteOrder)preparation.ByteOrder;
					dataType = (DataType)preparation.DataType;
					conversionType = (ConversionType)preparation.ConversionSelector;
					if ((int)pT != 18)
					{
						IEnumerable<ScaleEntry> source = ScaleEntry.GetScales(service.Channel.EcuHandle, preparation, null);
						if ((conversionType == ConversionType.FactorOffset || conversionType == ConversionType.Scale) && source.Any())
						{
							ScaleEntry scaleEntry = source.First();
							factor = scaleEntry.Factor;
							offset = scaleEntry.Offset;
							if (conversionType == ConversionType.Scale)
							{
								scales = source.ToList();
								min = scaleEntry.Min;
								max = scaleEntry.Max;
							}
						}
						Limits limits = preparation.GetLimits(service.Channel.EcuHandle);
						if (!string.IsNullOrEmpty(limits.Units))
						{
							units = limits.Units;
						}
						if (limits.Min.HasValue)
						{
							if (factor.HasValue && offset.HasValue)
							{
								min = (Convert.ToDecimal((double)limits.Min.Value, CultureInfo.InvariantCulture) - offset.Value) / factor.Value;
							}
							else
							{
								min = Convert.ToDecimal((double)limits.Min.Value, CultureInfo.InvariantCulture);
							}
						}
						if (limits.Max.HasValue)
						{
							if (factor.HasValue && offset.HasValue)
							{
								max = (Convert.ToDecimal((double)limits.Max.Value, CultureInfo.InvariantCulture) - offset.Value) / factor.Value;
							}
							else
							{
								max = Convert.ToDecimal((double)limits.Max.Value, CultureInfo.InvariantCulture);
							}
						}
					}
				}
				finally
				{
					((IDisposable)preparation)?.Dispose();
				}
			}
			catch (CaesarErrorException ex)
			{
				CaesarException e = new CaesarException(ex, null, null);
				Sapi.GetSapi().RaiseExceptionEvent(this, e);
			}
		}
		if (diagService.GetPreperationHasDefaultValue((uint)indexDI))
		{
			defaultValue = diagService.GetPreparationDefaultValue((uint)indexDI, pT);
			if (defaultValue != null)
			{
				if (defaultValue.GetType() == typeof(byte[]))
				{
					defaultValue = new Dump((byte[])defaultValue);
				}
				else if (Type == typeof(Choice))
				{
					defaultValue = choices.GetItemFromOriginalName(defaultValue.ToString());
				}
			}
			if (defaultValue != null)
			{
				value = defaultValue;
			}
		}
		if (type == typeof(string))
		{
			requiredLength = diagService.GetPrepAsciiLength((uint)indexDI);
		}
		else if (type == typeof(Dump))
		{
			requiredLength = diagService.GetPrepDumpLength((uint)indexDI);
		}
	}

	internal ServiceArgumentValue StoreArgumentValue()
	{
		return argumentValues.Add(value, Sapi.Now, fromLog: false, this, inputValuePreProcessed);
	}

	internal void SetPreparation(McdDiagComPrimitive diagServiceIO, ServiceExecution execution = null)
	{
		Exception = null;
		object obj = PrepareValue((execution != null) ? execution.InputArgumentValues.First((ServiceArgumentValue iv) => iv.InputValue.ParameterQualifier == ParameterQualifier).Value : value, choiceAsString: true);
		if (obj != null)
		{
			try
			{
				diagServiceIO.SetInput(indexDI, obj);
			}
			catch (McdException mcdError)
			{
				Exception = new CaesarException(mcdError);
			}
		}
	}

	internal void SetPreparation(CaesarDiagServiceIO diagServiceIO, ServiceExecution execution = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		object obj = PrepareValue((execution != null) ? execution.InputArgumentValues.First((ServiceArgumentValue iv) => iv.InputValue.ParameterQualifier == ParameterQualifier).Value : value, choiceAsString: false);
		if (obj != null)
		{
			diagServiceIO.SetPrepParam(indexDM, pT, obj);
		}
	}

	internal void SetPreparation(CaesarDiagService diagService)
	{
		//IL_002e: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Exception = null;
		object obj = PrepareValue(value, choiceAsString: false);
		if (obj != null)
		{
			try
			{
				diagService.SetPrepParam(indexDI, pT, obj);
			}
			catch (CaesarErrorException ex)
			{
				CaesarErrorException caesarError = ex;
				Exception = new CaesarException(caesarError);
			}
		}
	}

	private object PrepareValue(object newValue, bool choiceAsString)
	{
		if (newValue != null)
		{
			if (newValue.GetType() == typeof(string))
			{
				string text = newValue.ToString();
				if (PadOrTrim)
				{
					int num = Convert.ToInt32(requiredLength, CultureInfo.InvariantCulture);
					if (text.Length > num)
					{
						text = text.Substring(0, num);
					}
					else if (text.Length < num)
					{
						text = text.PadRight(num, ' ');
					}
					newValue = text;
				}
			}
			else if (newValue.GetType() == typeof(Dump))
			{
				IList<byte> data = ((Dump)newValue).Data;
				int count = data.Count;
				if (PadOrTrim)
				{
					count = Convert.ToInt32(requiredLength, CultureInfo.InvariantCulture);
					byte[] array = new byte[count];
					for (int i = 0; i < data.Count && i < count; i++)
					{
						array[i] = data[i];
					}
					newValue = array;
				}
				else
				{
					newValue = data.ToArray();
				}
			}
			else if (newValue.GetType() == typeof(Choice))
			{
				Choice choice = (Choice)newValue;
				newValue = ((!choiceAsString) ? ((object)choice.Index) : choice.OriginalName);
			}
		}
		return newValue;
	}

	internal ServiceInputValue Clone()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		return new ServiceInputValue(null, indexDI, indexDM)
		{
			mcdRequestParameter = mcdRequestParameter,
			mcdEquivalentScaleInfo = mcdEquivalentScaleInfo,
			name = name,
			qualifier = qualifier,
			description = description,
			max = max,
			min = min,
			units = units,
			defaultValue = defaultValue,
			type = type,
			pT = pT,
			requiredLength = requiredLength,
			makeFit = makeFit,
			choices = choices
		};
	}

	internal Exception InternalSetValue(string newValue, Dictionary<string, string> variables)
	{
		Exception result = null;
		try
		{
			if (newValue.StartsWith("einput:", StringComparison.OrdinalIgnoreCase))
			{
				newValue = Sapi.Decrypt(new Dump(newValue.Remove(0, "einput:".Length)));
				inputValuePreProcessed = true;
			}
			else
			{
				newValue = ((variables != null && newValue.Length > 2 && newValue[0] == '%' && newValue[newValue.Length - 1] == '%') ? variables[newValue.Substring(1, newValue.Length - 2)] : newValue);
				inputValuePreProcessed = false;
			}
			if (Type == typeof(Choice))
			{
				uint num = Convert.ToUInt32(newValue, CultureInfo.InvariantCulture);
				Choice itemFromRawValue = Choices.GetItemFromRawValue(num);
				if (itemFromRawValue != null)
				{
					SetValue(itemFromRawValue);
				}
				else
				{
					result = new ArgumentOutOfRangeException("newValue", string.Format(CultureInfo.InvariantCulture, "Raw value '{0}' not found", newValue));
				}
			}
			else if (type == typeof(Dump))
			{
				SetValue(newValue);
			}
			else
			{
				CultureInfo provider = new CultureInfo("en-US");
				SetValue(Convert.ChangeType(newValue, type, provider));
			}
		}
		catch (InvalidOperationException ex)
		{
			result = ex;
		}
		catch (InvalidCastException ex2)
		{
			result = ex2;
		}
		catch (FormatException ex3)
		{
			result = ex3;
		}
		catch (NullReferenceException ex4)
		{
			result = ex4;
		}
		catch (OverflowException ex5)
		{
			result = ex5;
		}
		catch (KeyNotFoundException ex6)
		{
			result = ex6;
		}
		return result;
	}

	private void SetValue(object value)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected I4, but got Unknown
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (type == null)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot set service input value {0} because it does not have a valid type. (Caesar type={1})", Name, (int)pT));
		}
		if (type.IsAssignableFrom(value.GetType()))
		{
			this.value = value;
		}
		else if (type == typeof(Choice))
		{
			string s = value.ToString();
			if (!string.IsNullOrEmpty(s))
			{
				this.value = choices.FirstOrDefault((Choice c) => c.Name == s || c.OriginalName == s);
				if (this.value == null)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is an invalid choice value for service input value {1}", s, Name));
				}
			}
		}
		else if (type == typeof(Dump))
		{
			string source = value.ToString();
			this.value = new Dump(source);
		}
		else
		{
			this.value = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
		}
	}

	internal void AddStringsForTranslation(Dictionary<string, string> table)
	{
		table[Sapi.MakeTranslationIdentifier(qualifier, "Name")] = name;
		if (!string.IsNullOrEmpty(description))
		{
			table[Sapi.MakeTranslationIdentifier(qualifier, "Description")] = description;
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

	internal object ParseFromLog(string value)
	{
		return Presentation.ParseFromLog(value, type, choices, service.Channel.Ecu);
	}

	internal object GetPreparation(byte[] data)
	{
		if (BytePosition.HasValue && BitPosition.HasValue && ByteLength.HasValue && BitLength.HasValue)
		{
			decimal? num = null;
			decimal? num2 = null;
			if (Factor.HasValue && Offset.HasValue)
			{
				decimal? num3 = Factor;
				if (!(num3.GetValueOrDefault() == default(decimal)) || !num3.HasValue)
				{
					decimal num4 = 1;
					num3 = Factor;
					num = (decimal?)num4 / num3;
					num2 = -(Offset * (decimal?)Factor.Value);
				}
			}
			return Presentation.Decode(data, (int)BytePosition.Value, BitPosition.Value, (int)ByteLength.Value, (int)BitLength.Value, ByteOrder, type, isPropA: false, null, num, num2, Min, Max, Choices, null, name);
		}
		return null;
	}
}
