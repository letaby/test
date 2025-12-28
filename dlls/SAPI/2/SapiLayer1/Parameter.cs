using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Xml;
using System.Xml.Linq;
using CaesarAbstraction;

namespace SapiLayer1;

public sealed class Parameter : IDiogenesDataItem
{
	private object mcdVarcodeFragmentLock = new object();

	private ServiceInputValue mcdVarcodeFragment;

	private string combinedQualifier;

	private byte[] writePrefix;

	private Dump codingStringMask;

	private Channel channel;

	private uint caesarIndex;

	private int collectionIndex;

	private string groupQualifier;

	private string qualifier;

	private string mcdQualifier;

	private string name;

	private string description;

	private string groupName;

	private object value;

	private object originalValue;

	private object defaultValue;

	private object lastPersistedValue;

	private ChoiceCollection choices;

	private bool visible;

	private bool readOnly;

	private decimal? max;

	private decimal? min;

	private string units;

	private ParamType pt;

	private Type type;

	private bool hasBeenReadFromEcu;

	private bool marked;

	private bool lastInGroup;

	private int readAccessLevel;

	private int writeAccessLevel;

	private Exception exception;

	private string readReference;

	private string writeReference;

	private int writeReferenceIndex;

	private Service readService;

	private Service writeService;

	private object precision;

	private bool persistable;

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

	private bool serviceAsParameter;

	private List<ScaleEntry> scales;

	private ScaleEntry factorOffsetScale;

	private ParameterValueCollection parameterValues;

	public decimal? RestrictedMin { get; private set; }

	public decimal? RestrictedMax { get; private set; }

	internal bool LastInGroup
	{
		get
		{
			return lastInGroup;
		}
		set
		{
			lastInGroup = value;
		}
	}

	public Channel Channel => channel;

	public string Qualifier => qualifier;

	public string McdQualifier => mcdQualifier;

	internal string CombinedQualifier
	{
		get
		{
			if (combinedQualifier == null)
			{
				combinedQualifier = string.Join(".", groupQualifier, qualifier);
			}
			return combinedQualifier;
		}
	}

	public string Name => channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(CombinedQualifier, "Name"), name);

	public string ShortName => Name;

	public string OriginalName => name;

	public string Description => channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(CombinedQualifier, "Description"), description);

	public string GroupQualifier => groupQualifier;

	public string GroupName => channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(groupQualifier, "GroupName"), groupName);

	public string OriginalGroupName => groupName;

	public string GroupCodingString
	{
		get
		{
			return Channel.Parameters.GroupCodingStrings[GroupQualifier];
		}
		set
		{
			Channel.Parameters.SetGroupCodingString(GroupQualifier, value);
		}
	}

	public Dump CodingValue
	{
		get
		{
			string groupCodingString = GroupCodingString;
			if (groupCodingString != null)
			{
				return new Dump(new List<byte>(new Dump(groupCodingString).Data).Skip(Convert.ToInt32(BytePosition, CultureInfo.InvariantCulture)).Take(Convert.ToInt32(ByteLength, CultureInfo.InvariantCulture)).ToArray());
			}
			return null;
		}
		set
		{
			string groupCodingString = GroupCodingString;
			if (groupCodingString != null)
			{
				List<byte> list = new List<byte>(new Dump(groupCodingString).Data);
				for (int i = 0; i < Convert.ToInt32(ByteLength, CultureInfo.InvariantCulture); i++)
				{
					list[Convert.ToInt32(BytePosition, CultureInfo.InvariantCulture) + i] = value.Data[i];
				}
				Channel.Parameters.SetGroupCodingString(GroupQualifier, new Dump(list).ToString(), Enumerable.Repeat(this, 1));
			}
		}
	}

	public string OriginalGroupCodingString => Channel.Parameters.OriginalGroupCodingStrings[GroupQualifier];

	public string Units
	{
		get
		{
			if (units == null && mcdVarcodeFragment != null)
			{
				units = mcdVarcodeFragment.Units ?? string.Empty;
			}
			return units;
		}
	}

	public ChoiceCollection Choices
	{
		get
		{
			if (choices.Count == 0 && type == typeof(Choice) && mcdVarcodeFragment != null)
			{
				foreach (Choice choice in mcdVarcodeFragment.Choices)
				{
					choices.Add(choice);
				}
			}
			return choices;
		}
	}

	public bool Visible
	{
		get
		{
			if (Channel.LogFile != null && parameterValues.Count == 0)
			{
				return false;
			}
			return visible;
		}
	}

	public bool ReadOnly => readOnly;

	public Service CombinedService => null;

	public object Value
	{
		get
		{
			return value;
		}
		set
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected I4, but got Unknown
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (type == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot change parameter {0} because it does not have a valid type. (Caesar type={1})", Name, (int)pt));
			}
			if (channel.CommunicationsState == CommunicationsState.ReadParameters || channel.CommunicationsState == CommunicationsState.WriteParameters)
			{
				throw new InvalidOperationException("Cannot change a parameter whilst a read or write is in progress");
			}
			channel.Parameters.ResetGroupCodingString(GroupQualifier);
			ResetException();
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
				}
				if (this.value == null)
				{
					this.value = ChoiceCollection.InvalidChoice;
				}
			}
			else if (type == typeof(Dump))
			{
				string source = value.ToString();
				this.value = new Dump(source);
			}
			else if (type == typeof(float))
			{
				this.value = Quantize(Convert.ToSingle(value, CultureInfo.CurrentCulture));
			}
			else if (type == typeof(double))
			{
				this.value = Quantize(Convert.ToDouble(value, CultureInfo.CurrentCulture));
			}
			else
			{
				this.value = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
			}
			if (Channel.Extension != null)
			{
				try
				{
					Channel.Extension.Invoke("SetRelatedParameterValues", new object[1] { this });
				}
				catch (NullReferenceException)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to locate related parameter whilst setting " + Qualifier);
				}
			}
			RaiseParameterUpdateEvent(null);
		}
	}

	public IEnumerable<string> RelatedParameterQualifiers
	{
		get
		{
			if (Channel.Extension != null)
			{
				return Channel.Extension.Invoke("GetRelatedParameters", new object[1] { this }) as IEnumerable<string>;
			}
			return null;
		}
	}

	public object OriginalValue => originalValue;

	public object DefaultValue
	{
		get
		{
			if (defaultValue == null && mcdVarcodeFragment != null)
			{
				defaultValue = mcdVarcodeFragment.DefaultValue;
			}
			return defaultValue;
		}
	}

	public object LastPersistedValue => lastPersistedValue;

	public object Max
	{
		get
		{
			if (!max.HasValue && mcdVarcodeFragment != null)
			{
				AcquireScalesMinMaxFromMcd();
			}
			try
			{
				return max.ToBoxed(type);
			}
			catch (OverflowException)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Invalid Max value in database. '" + max + "' cannot be presented as " + type);
				return null;
			}
		}
	}

	public object Min
	{
		get
		{
			if (!min.HasValue && mcdVarcodeFragment != null)
			{
				AcquireScalesMinMaxFromMcd();
			}
			try
			{
				return min.ToBoxed(type);
			}
			catch (OverflowException)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Invalid Min value in database. '" + min + "' cannot be presented as " + type);
				return null;
			}
		}
	}

	public IEnumerable<ScaleEntry> Scales
	{
		get
		{
			lock (mcdVarcodeFragmentLock)
			{
				if (scales == null && mcdVarcodeFragment != null)
				{
					AcquireScalesMinMaxFromMcd();
				}
				return (scales != null) ? scales.AsReadOnly() : null;
			}
		}
	}

	internal ScaleEntry FactorOffsetScale
	{
		get
		{
			lock (mcdVarcodeFragmentLock)
			{
				if (factorOffsetScale == null && mcdVarcodeFragment != null)
				{
					factorOffsetScale = mcdVarcodeFragment.FactorOffsetScale;
				}
				return factorOffsetScale;
			}
		}
	}

	public Type Type => type;

	public int Index => collectionIndex;

	internal uint CaesarIndex => caesarIndex;

	internal ParamType ParamType => pt;

	public bool HasBeenReadFromEcu => hasBeenReadFromEcu;

	public bool Marked
	{
		get
		{
			return marked;
		}
		set
		{
			marked = value;
		}
	}

	public int ReadAccess => readAccessLevel;

	public int WriteAccess => writeAccessLevel;

	public Exception Exception
	{
		get
		{
			return exception;
		}
		internal set
		{
			exception = value;
		}
	}

	public bool ServiceAsParameter => serviceAsParameter;

	public object Precision
	{
		get
		{
			if (precision == null && mcdVarcodeFragment != null && Factor.HasValue && Offset.HasValue)
			{
				precision = Math.Max(Sapi.CalculatePrecision(Convert.ToDouble(Factor.Value)), Sapi.CalculatePrecision(Convert.ToDouble(Offset.Value)));
			}
			return precision;
		}
	}

	public bool Persistable => persistable;

	public long? BytePosition
	{
		get
		{
			if (!bytePos.HasValue && mcdVarcodeFragment != null)
			{
				bytePos = mcdVarcodeFragment.BytePosition.Value - WritePrefix.Length;
			}
			return bytePos;
		}
	}

	public int? BitPosition
	{
		get
		{
			if (!bitPos.HasValue && mcdVarcodeFragment != null)
			{
				bitPos = mcdVarcodeFragment.BitPosition;
			}
			return bitPos;
		}
	}

	public long? ByteLength
	{
		get
		{
			if (!byteLength.HasValue && mcdVarcodeFragment != null)
			{
				byteLength = mcdVarcodeFragment.ByteLength;
			}
			return byteLength;
		}
	}

	public long? BitLength
	{
		get
		{
			if (!bitLength.HasValue && mcdVarcodeFragment != null)
			{
				bitLength = mcdVarcodeFragment.BitLength;
			}
			return bitLength;
		}
	}

	public Coding? Coding
	{
		get
		{
			if (!coding.HasValue && mcdVarcodeFragment != null)
			{
				coding = mcdVarcodeFragment.Coding;
			}
			return coding;
		}
	}

	public ByteOrder? ByteOrder
	{
		get
		{
			if (!byteOrder.HasValue && mcdVarcodeFragment != null)
			{
				byteOrder = mcdVarcodeFragment.ByteOrder;
			}
			return byteOrder;
		}
	}

	public TypeSpecifier? TypeSpecifier
	{
		get
		{
			if (!typeSpecifier.HasValue && mcdVarcodeFragment != null)
			{
				typeSpecifier = mcdVarcodeFragment.TypeSpecifier;
			}
			return typeSpecifier;
		}
	}

	public ConversionType? ConversionSelector
	{
		get
		{
			if (!conversionType.HasValue && mcdVarcodeFragment != null)
			{
				conversionType = mcdVarcodeFragment.ConversionSelector;
			}
			return conversionType;
		}
	}

	public DataType? DataType
	{
		get
		{
			if (!dataType.HasValue && mcdVarcodeFragment != null)
			{
				dataType = mcdVarcodeFragment.DataType;
			}
			return dataType;
		}
	}

	public decimal? Factor
	{
		get
		{
			lock (mcdVarcodeFragmentLock)
			{
				if (!factor.HasValue && mcdVarcodeFragment != null)
				{
					factor = (mcdVarcodeFragment.Factor.HasValue ? new decimal?(Convert.ToDecimal(mcdVarcodeFragment.Factor, CultureInfo.InvariantCulture)) : ((decimal?)null));
				}
				return factor;
			}
		}
	}

	public decimal? Offset
	{
		get
		{
			lock (mcdVarcodeFragmentLock)
			{
				if (!offset.HasValue && mcdVarcodeFragment != null)
				{
					offset = (mcdVarcodeFragment.Offset.HasValue ? new decimal?(Convert.ToDecimal(mcdVarcodeFragment.Offset, CultureInfo.InvariantCulture)) : ((decimal?)null));
				}
				return offset;
			}
		}
	}

	public Service ReadService => readService;

	internal byte[] WritePrefix
	{
		get
		{
			if (writePrefix == null)
			{
				if (writeService != null && readService != null)
				{
					writePrefix = writeService.BaseRequestMessage.Data.Take(readService.BaseRequestMessage.Data.Count).ToArray();
				}
				else if (writeService != null)
				{
					writePrefix = writeService.BaseRequestMessage.Data.Take(writeService.RequestMessageMask.Data.Count).ToArray();
				}
			}
			return writePrefix;
		}
	}

	public int? GroupLength
	{
		get
		{
			IList<byte> list = WriteService?.BaseRequestMessage?.Data;
			if (list != null && WritePrefix != null)
			{
				return list.Count - WritePrefix.Length;
			}
			return null;
		}
	}

	public Service WriteService => writeService;

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ReadAccessLevel is deprecated due to non-CLS compliance, please use ReadAccess instead.")]
	public ushort ReadAccessLevel => (ushort)readAccessLevel;

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("WriteAccessLevel is deprecated due to non-CLS compliance, please use WriteAccess instead.")]
	public ushort WriteAccessLevel => (ushort)writeAccessLevel;

	public ParameterValueCollection ParameterValues => parameterValues;

	public bool Summary => channel.Ecu.SummaryQualifier(CombinedQualifier);

	public Dump CodingStringMask
	{
		get
		{
			if (codingStringMask == null && GroupLength.HasValue)
			{
				codingStringMask = CreateCodingStringMask(GroupLength.Value, Enumerable.Repeat(this, 1), includeExclude: true);
			}
			return codingStringMask;
		}
	}

	public event ParameterUpdateEventHandler ParameterUpdateEvent;

	internal Parameter(Channel ch, uint caesarIndex, string groupQualifier, string groupName, bool persistable, int collectionIndex)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		readOnly = true;
		units = string.Empty;
		qualifier = string.Empty;
		description = string.Empty;
		name = string.Empty;
		channel = ch;
		this.caesarIndex = caesarIndex;
		this.collectionIndex = collectionIndex;
		this.groupQualifier = groupQualifier;
		this.groupName = groupName;
		pt = (ParamType)20;
		this.persistable = persistable;
		parameterValues = new ParameterValueCollection(this);
	}

	private decimal ScaleIfPossible(decimal source)
	{
		if (factor.HasValue && offset.HasValue)
		{
			return source * factor.Value + offset.Value;
		}
		return source;
	}

	private void AcquireCommonEcuInfo(int? readAccess, int? writeAccess, Service readService, Service writeService)
	{
		int? ecuInfoLimitedRangeMin = Channel.DiagnosisVariant.GetEcuInfoLimitedRangeMin(CombinedQualifier);
		int? ecuInfoLimitedRangeMax = Channel.DiagnosisVariant.GetEcuInfoLimitedRangeMax(CombinedQualifier);
		choices = new ChoiceCollection(channel.Ecu, CombinedQualifier, channel.DiagnosisVariant.GetEcuInfoProhibitedChoices(CombinedQualifier), ecuInfoLimitedRangeMin, ecuInfoLimitedRangeMax);
		if (readAccess.HasValue && writeAccess.HasValue)
		{
			readAccessLevel = Math.Max((readService != null) ? readService.Access : int.MaxValue, readAccess.Value);
			writeAccessLevel = Math.Max((writeService != null) ? writeService.Access : int.MaxValue, writeAccess.Value);
		}
		else
		{
			readAccessLevel = ((readService != null) ? readService.Access : int.MaxValue);
			writeAccessLevel = ((writeService != null) ? writeService.Access : int.MaxValue);
		}
		int? ecuInfoReadAccessLevel = Channel.DiagnosisVariant.GetEcuInfoReadAccessLevel(CombinedQualifier);
		if (ecuInfoReadAccessLevel.HasValue)
		{
			readAccessLevel = ecuInfoReadAccessLevel.Value;
		}
		int? ecuInfoWriteAccessLevel = Channel.DiagnosisVariant.GetEcuInfoWriteAccessLevel(CombinedQualifier);
		if (ecuInfoWriteAccessLevel.HasValue)
		{
			writeAccessLevel = ecuInfoWriteAccessLevel.Value;
		}
		Sapi sapi = Sapi.GetSapi();
		visible = sapi.ReadAccess >= readAccessLevel;
		readOnly = sapi.WriteAccess < writeAccessLevel;
	}

	internal void Acquire(string caesarEquivalentQualifier, ServiceInputValue varcodeFragment, string varcodeFragmentName, Service readService, Service writeService)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		mcdVarcodeFragment = varcodeFragment;
		serviceAsParameter = false;
		this.readService = readService;
		this.writeService = writeService;
		name = varcodeFragmentName;
		description = varcodeFragment.Description;
		mcdQualifier = varcodeFragment.ParameterQualifier;
		qualifier = caesarEquivalentQualifier;
		marked = true;
		units = null;
		AcquireCommonEcuInfo(varcodeFragment.ReadAccess, varcodeFragment.WriteAccess, readService, writeService);
		type = varcodeFragment.Type;
		if (type == typeof(Choice))
		{
			pt = (ParamType)18;
		}
		else if (type == typeof(Dump))
		{
			pt = (ParamType)15;
		}
	}

	private void AcquireScalesMinMaxFromMcd()
	{
		if (type != typeof(Choice) && type != typeof(Dump) && mcdVarcodeFragment.Scales != null)
		{
			scales = mcdVarcodeFragment.Scales.ToList();
			ProcessScaleRanges(mcdVarcodeFragment.ByteLength.Value);
		}
	}

	private void ProcessScaleRanges(long byteLength)
	{
		IEnumerable<ScaleEntry> enumerable = scales.Where((ScaleEntry scale) => !scale.IsFixedValue);
		if (!enumerable.Any())
		{
			return;
		}
		min = Quantize(enumerable.Min((ScaleEntry scale) => scale.Min));
		max = Quantize(enumerable.Max((ScaleEntry scale) => scale.Max));
		ulong num = Convert.ToUInt64(Math.Pow(256.0, byteLength)) - 1;
		foreach (ScaleEntry item in scales.Except(enumerable))
		{
			decimal num2 = item.ToEcuValue(item.Min);
			if (num2 == (decimal)num)
			{
				item.Name = "sna";
			}
			string text = channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(CombinedQualifier, num2.ToString(CultureInfo.InvariantCulture), "ScaleEntryName"), null);
			if (text != null)
			{
				item.Name = text;
			}
		}
	}

	internal void Acquire(Varcode varcode, CaesarDIVarCodeFrag varcodeFragment, byte[] defaultString, Service readService, Service writeService)
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected I4, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Invalid comparison between Unknown and I4
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Invalid comparison between Unknown and I4
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		serviceAsParameter = false;
		this.readService = readService;
		this.writeService = writeService;
		name = varcodeFragment.Name;
		description = varcodeFragment.Description;
		qualifier = varcodeFragment.Qualifier;
		int? ecuInfoLimitedRangeMin = Channel.DiagnosisVariant.GetEcuInfoLimitedRangeMin(CombinedQualifier);
		int? ecuInfoLimitedRangeMax = Channel.DiagnosisVariant.GetEcuInfoLimitedRangeMax(CombinedQualifier);
		AcquireCommonEcuInfo((varcodeFragment.AccessLevels != null) ? new ushort?(varcodeFragment.AccessLevels.Read) : ((ushort?)null), (varcodeFragment.AccessLevels != null) ? new ushort?(varcodeFragment.AccessLevels.Write) : ((ushort?)null), readService, writeService);
		uint fragValueCount = varcodeFragment.FragValueCount;
		if (fragValueCount != 0)
		{
			type = typeof(Choice);
			pt = (ParamType)18;
			uint num = varcodeFragment.BitLength;
			ByteOrder val = varcodeFragment.ByteOrder;
			for (uint num2 = 0u; num2 < fragValueCount; num2++)
			{
				CaesarDICbfFragValue fragValue = varcodeFragment.GetFragValue(num2);
				try
				{
					if (fragValue != null)
					{
						Choice choice = new Choice(fragValue.Meaning, fragValue.GetValue(num, val));
						Choices.Add(choice);
					}
				}
				finally
				{
					((IDisposable)fragValue)?.Dispose();
				}
			}
		}
		else
		{
			CaesarPreparation preparation = varcodeFragment.GetPreparation(channel.EcuHandle);
			try
			{
				CaesarPresentation presentation = varcodeFragment.GetPresentation(channel.EcuHandle);
				try
				{
					AcquirePresentation(presentation);
					if (preparation != null)
					{
						pt = preparation.Type;
						type = Sapi.GetRealCaesarType(pt);
						conversionType = (ConversionType)preparation.ConversionSelector;
						if ((int)pt == 18)
						{
							uint numberEnumerationEntries = preparation.NumberEnumerationEntries;
							for (uint num3 = 0u; num3 < numberEnumerationEntries; num3++)
							{
								DictionaryEntry enumerationEntry = preparation.GetEnumerationEntry(channel.EcuHandle, num3);
								Choice choice2 = new Choice(enumerationEntry.Key.ToString(), enumerationEntry.Value);
								Choices.Add(choice2);
							}
						}
						else
						{
							bool flag = (int)pt == 6 && channel.Ecu.SupportsDoublePrecisionVariantCoding && (Coding == SapiLayer1.Coding.Unsigned || Coding == SapiLayer1.Coding.TwosComplement) && bitPos.Value == 0 && bitLength.Value % 8 == 0;
							IEnumerable<ScaleEntry> source = ScaleEntry.GetScales(channel.EcuHandle, preparation, flag ? presentation : null);
							if ((conversionType == ConversionType.FactorOffset || conversionType == ConversionType.Scale) && source.Any())
							{
								ScaleEntry scaleEntry = source.First();
								factor = 1m / scaleEntry.Factor;
								offset = -(scaleEntry.Offset * factor.Value);
								precision = Math.Max(Sapi.CalculatePrecision(Convert.ToDouble(factor.Value)), Sapi.CalculatePrecision(Convert.ToDouble(offset.Value)));
								if (conversionType == ConversionType.Scale)
								{
									scales = source.ToList();
									ProcessScaleRanges(preparation.ByteLength);
								}
								else
								{
									factorOffsetScale = scaleEntry;
								}
								if (flag)
								{
									type = typeof(double);
								}
							}
							string ecuInfoAttribute = channel.DiagnosisVariant.GetEcuInfoAttribute<string>("Choices", CombinedQualifier);
							if (ecuInfoAttribute != null)
							{
								choices.Add(ecuInfoAttribute, type);
								type = typeof(Choice);
								min = (max = null);
							}
							else
							{
								Limits limits = preparation.GetLimits(Channel.EcuHandle);
								if (!min.HasValue && limits.Min.HasValue)
								{
									min = ScaleIfPossible(limits.Min.Value.ToDecimal());
								}
								if (!max.HasValue && limits.Max.HasValue)
								{
									max = ScaleIfPossible(limits.Max.Value.ToDecimal());
								}
								if (!string.IsNullOrEmpty(limits.Units))
								{
									units = limits.Units;
								}
								if (ecuInfoLimitedRangeMin.HasValue)
								{
									decimal? num4 = (RestrictedMin = ScaleIfPossible(ecuInfoLimitedRangeMin.Value));
									min = num4;
								}
								if (ecuInfoLimitedRangeMax.HasValue)
								{
									decimal? num4 = (RestrictedMax = ScaleIfPossible(ecuInfoLimitedRangeMax.Value));
									max = num4;
								}
							}
						}
					}
				}
				finally
				{
					((IDisposable)presentation)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)preparation)?.Dispose();
			}
		}
		if (defaultString != null)
		{
			defaultValue = GetPresentation(varcode);
			if (varcode.IsErrorSet)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Error while retrieving default value: " + varcode.Exception.Message);
			}
		}
		if (defaultValue == null)
		{
			defaultValue = varcodeFragment.DefaultValue;
			if (defaultValue != null)
			{
				if (defaultValue.GetType() == typeof(float))
				{
					if (type == typeof(double))
					{
						defaultValue = Quantize((double)Convert.ToSingle(defaultValue, CultureInfo.InvariantCulture));
					}
					else
					{
						defaultValue = Quantize(Convert.ToSingle(defaultValue, CultureInfo.InvariantCulture));
					}
				}
				else if (defaultValue.GetType() == typeof(byte[]))
				{
					defaultValue = new Dump((byte[])defaultValue);
				}
				else if (Type == typeof(Choice))
				{
					defaultValue = choices.GetItemFromOriginalName(defaultValue.ToString());
				}
			}
		}
		marked = true;
	}

	internal void Acquire(string qualifier, string name, Service writeService, string writeReference, int writeReferenceIndex, Service readService, string readReference, bool hide)
	{
		this.qualifier = qualifier;
		this.name = name;
		this.writeService = writeService;
		this.writeReferenceIndex = writeReferenceIndex;
		this.writeReference = writeReference;
		this.readService = readService;
		this.readReference = readReference;
		serviceAsParameter = true;
		if (this.readService != null)
		{
			readAccessLevel = this.readService.Access;
			ServiceOutputValue serviceOutputValue = this.readService.OutputValues[0];
			if (serviceOutputValue != null)
			{
				units = serviceOutputValue.Units;
				choices = serviceOutputValue.Choices;
				min = (serviceOutputValue.Min.HasValue ? new decimal?(Convert.ToDecimal(serviceOutputValue.Min, CultureInfo.InvariantCulture)) : ((decimal?)null));
				max = (serviceOutputValue.Max.HasValue ? new decimal?(Convert.ToDecimal(serviceOutputValue.Max, CultureInfo.InvariantCulture)) : ((decimal?)null));
				description = serviceOutputValue.Description;
				type = serviceOutputValue.Type;
				if (!hide)
				{
					visible = Sapi.GetSapi().ReadAccess >= readAccessLevel;
				}
			}
		}
		else
		{
			readAccessLevel = int.MaxValue;
		}
		if (this.writeService != null)
		{
			writeAccessLevel = this.writeService.Access;
			ServiceInputValue serviceInputValue = this.writeService.InputValues[this.writeReferenceIndex];
			if (serviceInputValue != null)
			{
				units = serviceInputValue.Units;
				choices = serviceInputValue.Choices;
				min = (serviceInputValue.Min.HasValue ? new decimal?(Convert.ToDecimal(serviceInputValue.Min, CultureInfo.InvariantCulture)) : ((decimal?)null));
				max = (serviceInputValue.Max.HasValue ? new decimal?(Convert.ToDecimal(serviceInputValue.Max, CultureInfo.InvariantCulture)) : ((decimal?)null));
				description = serviceInputValue.Description;
				type = serviceInputValue.Type;
				defaultValue = serviceInputValue.DefaultValue;
				readOnly = Sapi.GetSapi().WriteAccess < writeAccessLevel;
			}
		}
		else
		{
			writeAccessLevel = int.MaxValue;
		}
		marked = true;
	}

	private static decimal? GetDecimalValue(string value)
	{
		if (!string.IsNullOrEmpty(value) && decimal.TryParse(value, out var result))
		{
			return result;
		}
		return null;
	}

	internal void Acquire(string[] fields)
	{
		name = fields[1];
		qualifier = fields[3];
		switch (fields[4])
		{
		case "B":
			readAccessLevel = (writeAccessLevel = 1);
			break;
		case "R":
			readAccessLevel = 1;
			writeAccessLevel = int.MaxValue;
			break;
		case "W":
			writeAccessLevel = 1;
			readAccessLevel = int.MaxValue;
			break;
		}
		units = fields[5];
		defaultValue = GetDecimalValue(fields[6]);
		min = GetDecimalValue(fields[7]);
		max = GetDecimalValue(fields[8]);
		factor = GetDecimalValue(fields[9]);
		if (factor.HasValue)
		{
			offset = default(decimal);
		}
		Sapi sapi = Sapi.GetSapi();
		visible = sapi.ReadAccess >= readAccessLevel;
		readOnly = sapi.WriteAccess < writeAccessLevel;
		if (factor.HasValue)
		{
			type = ((factor.Value == 1m) ? typeof(int) : typeof(double));
			precision = Sapi.CalculatePrecision(Convert.ToDouble(factor.Value));
		}
		else
		{
			type = typeof(string);
		}
		marked = true;
	}

	internal void InternalRead(string[] fields)
	{
		CaesarException ex = null;
		object obj = value;
		string text = fields[2];
		if (text == "0000")
		{
			if (type != typeof(string))
			{
				decimal? decimalValue = GetDecimalValue(fields[3]);
				value = (decimalValue.HasValue ? Convert.ChangeType(decimalValue, type, CultureInfo.InvariantCulture) : null);
			}
			else
			{
				value = fields[3];
			}
			hasBeenReadFromEcu = true;
			originalValue = value;
			parameterValues.Add(new ParameterValue(value, Sapi.Now));
		}
		else
		{
			ex = new CaesarException(SapiError.ExternalVcpParameterError, text + " - " + fields[4]);
		}
		if (ex == null)
		{
			if (obj != null && !object.Equals(value, obj) && (channel.CommunicationsState == CommunicationsState.WriteParameters || channel.CommunicationsState == CommunicationsState.ProcessVcp))
			{
				RaiseParameterUpdateEvent(new CaesarException(SapiError.FragmentReadWriteMismatch));
			}
			else
			{
				RaiseParameterUpdateEvent(null);
			}
		}
		else
		{
			RaiseParameterUpdateEvent(ex);
		}
	}

	private void AcquirePresentation(CaesarPresentation presentation)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected I4, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected I4, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected I4, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected I4, but got Unknown
		if (presentation != null)
		{
			bytePos = presentation.BytePosition;
			bitPos = presentation.BitPosition;
			byteLength = presentation.ByteLength;
			bitLength = presentation.BitLength;
			coding = (Coding)presentation.Coding;
			typeSpecifier = (TypeSpecifier)presentation.TypeSpecifier;
			byteOrder = (ByteOrder)presentation.ByteOrder;
			dataType = (DataType)presentation.DataType;
		}
	}

	[HandleProcessCorruptedStateExceptions]
	internal bool InternalRead(Varcode varcode, bool fromDevice)
	{
		if (fromDevice && channel.IsChannelErrorSet)
		{
			return false;
		}
		object obj = value;
		Exception ex = null;
		if (!serviceAsParameter)
		{
			if (Sapi.GetSapi().HardwareAccess >= readAccessLevel || Sapi.GetSapi().InitState == InitState.Offline)
			{
				if (varcode != null)
				{
					value = GetPresentation(varcode);
					if (varcode.IsErrorSet)
					{
						ex = varcode.Exception;
					}
				}
			}
			else
			{
				ex = new CaesarException(SapiError.NoSecurityAccessForReadingThatFragment);
			}
		}
		else if (readService != null)
		{
			ex = readService.InputValues.InternalParseValues(readReference);
			if (ex == null)
			{
				try
				{
					ex = readService.InternalExecute(Service.ExecuteType.SystemInvoke);
				}
				catch (AccessViolationException ex2)
				{
					ex = ex2;
				}
				if (ex == null)
				{
					if (channel.IsChannelErrorSet)
					{
						if (channel.ChannelHandle != null)
						{
							ex = new CaesarException(channel.ChannelHandle);
						}
					}
					else
					{
						ServiceOutputValue serviceOutputValue = readService.OutputValues[0];
						if (serviceOutputValue != null)
						{
							value = serviceOutputValue.Value;
						}
					}
				}
			}
		}
		else
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Read service not defined");
		}
		if (fromDevice)
		{
			originalValue = value;
			hasBeenReadFromEcu = true;
			parameterValues.Add(new ParameterValue(value, Sapi.Now));
		}
		if (ex == null)
		{
			if (obj != null && !object.Equals(value, obj) && (channel.CommunicationsState == CommunicationsState.WriteParameters || channel.CommunicationsState == CommunicationsState.ProcessVcp))
			{
				RaiseParameterUpdateEvent(new CaesarException(SapiError.FragmentReadWriteMismatch));
			}
			else
			{
				RaiseParameterUpdateEvent(null);
			}
		}
		else
		{
			RaiseParameterUpdateEvent(ex);
		}
		return true;
	}

	internal void InternalWrite(Varcode varcode, bool resetHaveBeenReadFromEcu = true)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		if (Sapi.GetSapi().HardwareAccess >= writeAccessLevel || Sapi.GetSapi().InitState == InitState.Offline)
		{
			if (!serviceAsParameter)
			{
				if (varcode != null)
				{
					try
					{
						object obj = value;
						ParamType val = pt;
						if ((int)val == 18)
						{
							varcode.SetFragmentValue(this, obj);
						}
						else if (obj != null && type == typeof(Choice) && obj is Choice choice)
						{
							if (obj != ChoiceCollection.InvalidChoice)
							{
								if (choice.RawValue is float)
								{
									varcode.SetFragmentValue(this, Quantize(Convert.ToSingle(choice.RawValue, CultureInfo.InvariantCulture)));
								}
								else if (choice.RawValue is double)
								{
									varcode.SetFragmentValue(this, Quantize(Convert.ToDouble(choice.RawValue, CultureInfo.InvariantCulture)));
								}
								else
								{
									varcode.SetFragmentValue(this, choice.RawValue);
								}
							}
						}
						else
						{
							varcode.SetFragmentValue(this, obj);
						}
					}
					catch (NullReferenceException)
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(this, "Intentional catch of exception in Parameter::InternalWrite. Comms failure?");
					}
					CaesarException ex2 = null;
					if (varcode.IsErrorSet)
					{
						ex2 = varcode.Exception;
					}
					else if (channel.LogFile == null)
					{
						object presentation = GetPresentation(varcode);
						if (varcode.IsErrorSet)
						{
							ex2 = varcode.Exception;
						}
						else
						{
							value = presentation;
						}
					}
					if (ex2 != null)
					{
						RaiseParameterUpdateEvent(ex2);
					}
				}
			}
			else
			{
				Exception ex3 = null;
				if (writeService != null)
				{
					ex3 = writeService.InputValues.InternalParseValues(writeReference);
					if (ex3 == null)
					{
						ServiceInputValue serviceInputValue = writeService.InputValues[writeReferenceIndex];
						if (serviceInputValue != null)
						{
							if (serviceInputValue.Type == typeof(Dump) && serviceInputValue.Value != null)
							{
								IList<byte> list = (Value as Dump).Data;
								IList<byte> data = (serviceInputValue.Value as Dump).Data;
								if (list.Count < data.Count)
								{
									list = list.Concat(data.Skip(list.Count)).ToList();
								}
								int num = Convert.ToInt32(serviceInputValue.RequiredLength, CultureInfo.InvariantCulture);
								if (list.Count > num)
								{
									list = list.Take(num).ToList();
								}
								serviceInputValue.Value = new Dump(list);
							}
							else
							{
								serviceInputValue.Value = Value;
							}
						}
						ex3 = writeService.InternalExecute(Service.ExecuteType.SystemInvoke);
						if (ex3 == null)
						{
							if (channel.IsChannelErrorSet)
							{
								if (channel.ChannelHandle != null)
								{
									ex3 = new CaesarException(channel.ChannelHandle);
								}
							}
							else
							{
								value = serviceInputValue.Value;
								channel.ClearCache();
							}
						}
					}
				}
				else
				{
					ex3 = new CaesarException(SapiError.WriteServiceNotAvailable);
				}
				if (ex3 != null)
				{
					RaiseParameterUpdateEvent(ex3);
				}
			}
		}
		else
		{
			RaiseParameterUpdateEvent(new CaesarException(SapiError.AccessDeniedAuthorizationLevelTooLow));
		}
		if (resetHaveBeenReadFromEcu)
		{
			hasBeenReadFromEcu = false;
		}
	}

	internal void RaiseParameterUpdateEvent(Exception e)
	{
		if (exception == null)
		{
			exception = e;
		}
		FireAndForget.Invoke(this.ParameterUpdateEvent, this, new ResultEventArgs(exception));
		channel.Parameters.RaiseParameterUpdateEvent(this, exception);
	}

	internal void ResetException()
	{
		exception = null;
	}

	internal void ResetHasBeenReadFromEcu()
	{
		hasBeenReadFromEcu = false;
		if (object.Equals(value, originalValue))
		{
			value = null;
		}
		originalValue = null;
	}

	public object ParseValue(string newValue)
	{
		object obj = null;
		if (Type == typeof(Choice))
		{
			Choice itemFromRawValue = Choices.GetItemFromRawValue(newValue);
			if (itemFromRawValue != null)
			{
				return itemFromRawValue;
			}
			throw new ArgumentOutOfRangeException("newValue", string.Format(CultureInfo.InvariantCulture, "Raw value '{0}' not found", newValue));
		}
		if (type == typeof(Dump))
		{
			return new Dump(newValue);
		}
		CultureInfo provider = new CultureInfo("en-US");
		if (type == typeof(float))
		{
			float valueToQuantize = Convert.ToSingle(newValue, provider);
			return Quantize(valueToQuantize);
		}
		if (type == typeof(double))
		{
			double valueToQuantize2 = Convert.ToDouble(newValue, provider);
			return Quantize(valueToQuantize2);
		}
		return Convert.ChangeType(newValue, type, provider);
	}

	internal void InternalSetValue(string newValue, bool respectAccessLevel)
	{
		Exception ex = null;
		try
		{
			object objA = ParseValue(newValue);
			if (respectAccessLevel && writeAccessLevel > Sapi.GetSapi().WriteAccess)
			{
				if (!object.Equals(objA, value))
				{
					ex = new CaesarException(SapiError.AccessDeniedAuthorizationLevelTooLow);
				}
			}
			else
			{
				Value = objA;
			}
		}
		catch (ArgumentOutOfRangeException ex2)
		{
			ex = ex2;
		}
		catch (InvalidOperationException ex3)
		{
			ex = ex3;
		}
		catch (InvalidCastException ex4)
		{
			ex = ex4;
		}
		catch (FormatException ex5)
		{
			ex = ex5;
		}
		catch (NullReferenceException ex6)
		{
			ex = ex6;
		}
		catch (OverflowException ex7)
		{
			ex = ex7;
		}
		catch (ArgumentException ex8)
		{
			ex = ex8;
		}
		if (ex != null)
		{
			RaiseParameterUpdateEvent(ex);
		}
		lastPersistedValue = Value;
	}

	internal void InternalSetValueFromLogFile(ParameterValue value)
	{
		this.value = (originalValue = (lastPersistedValue = value?.Value));
		hasBeenReadFromEcu = this.value != null;
	}

	internal string InternalGetValue()
	{
		lastPersistedValue = Value;
		if (Value != null && Value != ChoiceCollection.InvalidChoice)
		{
			string text = FormatValue(Value);
			if (text.IndexOf(',') != -1)
			{
				text = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", text);
			}
			return text;
		}
		return string.Empty;
	}

	internal string FormatValue(object objValue)
	{
		if (type == typeof(Choice))
		{
			Choice choice = (Choice)objValue;
			if (!(choice != ChoiceCollection.InvalidChoice))
			{
				return string.Empty;
			}
			return choice.RawValue.ToString();
		}
		if (type == typeof(Dump))
		{
			return objValue.ToString();
		}
		CultureInfo cultureInfo = new CultureInfo("en-US");
		if (type == typeof(float))
		{
			float valueToQuantize = (float)objValue;
			float valueToQuantize2 = Convert.ToSingle(valueToQuantize.ToString(cultureInfo), cultureInfo);
			if (valueToQuantize.CompareTo(valueToQuantize2) != 0)
			{
				float num = Quantize(valueToQuantize);
				float num2 = Quantize(valueToQuantize2);
				if (num.CompareTo(num2) != 0)
				{
					return num.ToString("R", cultureInfo);
				}
			}
		}
		else if (type == typeof(double))
		{
			double valueToQuantize3 = (double)objValue;
			double valueToQuantize4 = Convert.ToDouble(valueToQuantize3.ToString(cultureInfo), cultureInfo);
			if (valueToQuantize3.CompareTo(valueToQuantize4) != 0)
			{
				double num3 = Quantize(valueToQuantize3);
				double num4 = Quantize(valueToQuantize4);
				if (num3.CompareTo(num4) != 0)
				{
					return num3.ToString("R", cultureInfo);
				}
			}
		}
		return Convert.ToString(objValue, cultureInfo);
	}

	internal object GetPresentation(Varcode varcode)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		object obj = null;
		try
		{
			object fragmentValue = varcode.GetFragmentValue(this);
			if (!varcode.IsErrorSet)
			{
				ParamType val = pt;
				obj = (((int)val != 6) ? fragmentValue : ((fragmentValue == null || !(fragmentValue is float)) ? fragmentValue : ((object)Quantize(Convert.ToSingle(fragmentValue, CultureInfo.InvariantCulture)))));
			}
			else
			{
				Sapi.GetSapi().RaiseExceptionEvent(this, varcode.Exception);
			}
		}
		catch (NullReferenceException)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Intentional catch of exception in Parameter::InternalRead. Comms failure?");
		}
		catch (InvalidCastException)
		{
			Sapi.GetSapi().RaiseExceptionEvent(this, new CaesarException(SapiError.UnknownPresentationType));
		}
		if (obj != null && type == typeof(Choice) && !(obj is Choice))
		{
			obj = Choices.GetItemFromRawValue(obj) ?? ChoiceCollection.InvalidChoice;
		}
		return obj;
	}

	internal float Quantize(float valueToQuantize)
	{
		ConversionType? conversionSelector = ConversionSelector;
		if (conversionSelector.HasValue && conversionSelector == ConversionType.Ieee)
		{
			return valueToQuantize;
		}
		return Convert.ToSingle(Quantize(valueToQuantize.ToDecimal()), CultureInfo.InvariantCulture);
	}

	internal double Quantize(double valueToQuantize)
	{
		return (double)Quantize(Convert.ToDecimal(valueToQuantize, CultureInfo.InvariantCulture));
	}

	internal decimal Quantize(decimal valueToQuantize)
	{
		if (Scales != null)
		{
			ScaleEntry scaleEntry = Scales.FirstOrDefault((ScaleEntry sc) => sc.IsValueInRange(valueToQuantize));
			if (scaleEntry != null)
			{
				decimal val = scaleEntry.ToPhysicalValue(scaleEntry.ToEcuValue(valueToQuantize));
				return Math.Min(scaleEntry.Max, Math.Max(scaleEntry.Min, val));
			}
		}
		else if (Factor.HasValue && Offset.HasValue)
		{
			return Math.Round((valueToQuantize - offset.Value) / Factor.Value, 0, MidpointRounding.AwayFromZero) * Factor.Value + Offset.Value;
		}
		return valueToQuantize;
	}

	public override string ToString()
	{
		return qualifier;
	}

	internal bool IsValueInFactorOffsetScaleRange(decimal value)
	{
		decimal num = (min.HasValue ? factorOffsetScale.ToEcuValue(min.Value) : GetRepresentableEcuMinOrMax(isMin: true));
		decimal num2 = (max.HasValue ? factorOffsetScale.ToEcuValue(max.Value) : GetRepresentableEcuMinOrMax(isMin: false));
		decimal num3 = factorOffsetScale.ToEcuValue(value);
		if (num3 >= num)
		{
			return num3 <= num2;
		}
		return false;
	}

	private decimal GetRepresentableEcuMinOrMax(bool isMin)
	{
		switch (Coding)
		{
		case SapiLayer1.Coding.Unsigned:
		{
			long? num = byteLength;
			if (num.HasValue)
			{
				long valueOrDefault = num.GetValueOrDefault();
				long num3 = valueOrDefault - 1;
				if ((ulong)num3 <= 3uL)
				{
					switch (num3)
					{
					case 0L:
						return (byte)((!isMin) ? byte.MaxValue : 0);
					case 1L:
						return (ushort)((!isMin) ? ushort.MaxValue : 0);
					case 3L:
						return (!isMin) ? uint.MaxValue : 0u;
					}
				}
			}
			throw new InvalidOperationException("Parameter " + qualifier + " has unhandled length " + byteLength);
		}
		case SapiLayer1.Coding.TwosComplement:
		{
			long? num = byteLength;
			if (num.HasValue)
			{
				long valueOrDefault = num.GetValueOrDefault();
				long num2 = valueOrDefault - 1;
				if ((ulong)num2 <= 3uL)
				{
					switch (num2)
					{
					case 0L:
						return isMin ? sbyte.MinValue : sbyte.MaxValue;
					case 1L:
						return isMin ? short.MinValue : short.MaxValue;
					case 3L:
						return isMin ? int.MinValue : int.MaxValue;
					}
				}
			}
			throw new InvalidOperationException("Parameter " + qualifier + " has unhandled length " + byteLength);
		}
		default:
			throw new InvalidOperationException("Parameter " + qualifier + " has unhandled coding " + coding);
		}
	}

	internal void AddStringsForTranslation(Dictionary<string, string> table)
	{
		table[Sapi.MakeTranslationIdentifier(CombinedQualifier, "Name")] = name;
		table[Sapi.MakeTranslationIdentifier(groupQualifier, "GroupName")] = groupName;
		if (description != null)
		{
			table[Sapi.MakeTranslationIdentifier(CombinedQualifier, "Description")] = description;
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

	public bool IsValueEqualInCodingString(Dump codingString, string value)
	{
		try
		{
			object obj = ParseValue(value);
			lock (Channel.OfflineVarcodingHandleLock)
			{
				Varcode offlineVarcodingHandle = channel.OfflineVarcodingHandle;
				if (offlineVarcodingHandle != null)
				{
					offlineVarcodingHandle.SetFragmentValue(this, obj);
					if (offlineVarcodingHandle.Exception == null)
					{
						return CodingStringMask.AreCodingStringsEquivalent(codingString.Data.ToArray(), offlineVarcodingHandle.GetCurrentCodingString(groupQualifier));
					}
				}
			}
		}
		catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is InvalidOperationException || ex is InvalidCastException || ex is FormatException || ex is OverflowException || ex is ArgumentException)
		{
		}
		return false;
	}

	public bool IsValueEqualInCodingStrings(Dump codingString1, Dump codingString2)
	{
		return CodingStringMask.AreCodingStringsEquivalent(codingString1.Data.ToArray(), codingString2.Data.ToArray());
	}

	internal static Dump CreateCodingStringMask(int length, IEnumerable<Parameter> parameters, bool includeExclude)
	{
		return (from p in parameters
			where p.BitLength.HasValue
			select Tuple.Create((int)(p.BytePosition.Value * 8 + p.BitPosition.Value), (int)p.BitLength.Value)).CreateCodingStringMask(length, includeExclude);
	}

	internal static bool LoadFromLog(XElement element, string groupQualifier, LogFileFormatTagCollection format, Channel channel, List<string> missingQualifierList, object missingInfoLock)
	{
		string arg = groupQualifier + "." + element.Attribute(format[TagName.Qualifier]).Value;
		Parameter parameter = channel.Parameters[arg];
		if (parameter != null)
		{
			bool result = false;
			{
				foreach (XElement item in element.Elements(format[TagName.Value]))
				{
					try
					{
						parameter.parameterValues.Add(ParameterValue.FromXElement(item, format, parameter), setCurrent: false);
						result = true;
					}
					catch (ArgumentOutOfRangeException)
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(channel, string.Format(CultureInfo.InvariantCulture, "ArgumentOutOfRangeException while loading {0} value '{1}' from log file", parameter.CombinedQualifier, item.Value));
					}
					catch (FormatException)
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(channel, string.Format(CultureInfo.InvariantCulture, "FormatException while loading {0} value '{1}' from log file", parameter.CombinedQualifier, item.Value));
					}
				}
				return result;
			}
		}
		if (!channel.Ecu.IgnoreQualifier(arg))
		{
			lock (missingInfoLock)
			{
				missingQualifierList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", channel.Ecu.Name, arg));
			}
		}
		return false;
	}

	internal void WriteXmlTo(DateTime startTime, DateTime endTime, XmlWriter writer)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		writer.WriteStartElement(currentFormat[TagName.Parameter].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, Qualifier);
		ParameterValue parameterValue = null;
		foreach (ParameterValue parameterValue2 in parameterValues)
		{
			if (parameterValue2.Value == null)
			{
				continue;
			}
			if (parameterValue2.Time >= startTime)
			{
				if (parameterValue != null)
				{
					parameterValue.WriteXmlTo(startTime, this, writer);
					parameterValue = null;
				}
				if (parameterValue2.Time > endTime)
				{
					break;
				}
				parameterValue2.WriteXmlTo(startTime, this, writer);
			}
			else
			{
				parameterValue = parameterValue2;
			}
		}
		parameterValue?.WriteXmlTo(startTime, this, writer);
		writer.WriteEndElement();
	}
}
