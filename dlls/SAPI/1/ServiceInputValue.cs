// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceInputValue
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

#nullable disable
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
  private Decimal? max;
  private Decimal? min;
  private string units;
  private ushort indexDI;
  private ushort indexDM;
  private object requiredLength;
  private ParamType pT;
  private Type type;
  private bool makeFit;
  private Decimal? factor;
  private Decimal? offset;
  private long? bytePos;
  private int? bitPos;
  private long? byteLength;
  private long? bitLength;
  private SapiLayer1.Coding? coding;
  private SapiLayer1.TypeSpecifier? typeSpecifier;
  private SapiLayer1.ByteOrder? byteOrder;
  private SapiLayer1.DataType? dataType;
  private ConversionType? conversionType;
  private List<ScaleEntry> scales;
  private int? readAccess;
  private int? writeAccess;
  private ServiceArgumentValueCollection argumentValues;

  internal ServiceInputValue(Service service, ushort indexDI, ushort indexDM)
  {
    this.service = service;
    this.qualifier = string.Empty;
    this.name = string.Empty;
    this.description = string.Empty;
    this.units = string.Empty;
    this.indexDI = indexDI;
    this.indexDM = indexDM;
    this.makeFit = true;
    this.pT = (ParamType) 20;
    this.argumentValues = new ServiceArgumentValueCollection();
  }

  internal void AcquirePreparation(McdDBRequestParameter requestParameter)
  {
    this.mcdRequestParameter = requestParameter;
    this.qualifier = this.service.ServiceTypes == ServiceTypes.DiagJob || requestParameter.PreparationName == null ? requestParameter.Qualifier : "PREP_" + McdCaesarEquivalence.MakeQualifier(requestParameter.PreparationName, true);
    this.IsReserved = requestParameter.IsReserved;
    this.parameterQualifier = requestParameter.Qualifier;
    this.name = requestParameter.Name;
    this.choices = new ChoiceCollection(this.service.Channel.Ecu, this.qualifier);
    this.units = (string) null;
    this.type = !(requestParameter.DataType == typeof (McdTextTableElement)) ? (requestParameter.DataType != typeof (byte[]) ? requestParameter.DataType : typeof (Dump)) : typeof (Choice);
    try
    {
      McdValue defaultValue = requestParameter.GetDefaultValue();
      if (defaultValue != null)
        this.value = this.defaultValue = defaultValue.GetValue(this.type, this.choices);
    }
    catch (McdException ex)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{this.service.Channel.Ecu.Name}.{this.service.Qualifier} input value {this.qualifier} error while retrieving default value+{ex.Message}");
    }
    if (this.service.ServiceTypes != ServiceTypes.WriteVarCode)
      return;
    this.readAccess = GetSecurityLevel(requestParameter.SpecialData, ".ReadSecurityLevel");
    this.writeAccess = GetSecurityLevel(requestParameter.SpecialData, ".WriteSecurityLevel");

    static int? GetSecurityLevel(Dictionary<string, string> specialData, string attribute)
    {
      if (specialData.ContainsKey(attribute))
      {
        string str = specialData[attribute];
        if (str.StartsWith("S", StringComparison.OrdinalIgnoreCase))
          return new int?(Convert.ToInt32(str.Substring(1)));
      }
      return new int?();
    }
  }

  private McdCaesarEquivalenceScaleInfo McdEquivalentScaleInfo
  {
    get
    {
      if (this.mcdEquivalentScaleInfo == null && this.mcdRequestParameter != null && this.mcdRequestParameter.PreparationQualifier != null)
        this.mcdEquivalentScaleInfo = this.service.Channel.GetMcdCaesarEquivalenceScaleInfo(this.mcdRequestParameter.PreparationQualifier, this.type, (Func<McdDBDataObjectProp>) (() => this.mcdRequestParameter.DataObjectProp));
      return this.mcdEquivalentScaleInfo;
    }
  }

  internal void AcquirePreparation(CaesarDiagService diagService)
  {
    if (this.service == (Service) null)
      throw new InvalidOperationException("AcquirePreparation not valid for a clone");
    this.qualifier = diagService.GetPrepQualifier((uint) this.indexDI);
    this.parameterQualifier = diagService.GetPrepParameterQualifier((uint) this.indexDI);
    this.name = diagService.GetPrepName((uint) this.indexDI);
    this.description = diagService.GetPrepDescription((uint) this.indexDI);
    this.choices = new ChoiceCollection(this.service.Channel.Ecu, this.qualifier);
    this.pT = diagService.GetPrepType((uint) this.indexDI);
    this.type = Sapi.GetRealCaesarType(this.pT);
    if (this.pT == 18)
    {
      this.type = typeof (Choice);
      uint prepNumberOfChoices = diagService.GetPrepNumberOfChoices((uint) this.indexDI);
      for (uint index = 0; index < prepNumberOfChoices; ++index)
        this.Choices.Add(new Choice(diagService.GetPrepChoiceMeaning((uint) this.indexDI, index), (object) diagService.GetPrepChoiceValue((uint) this.indexDI, index)));
    }
    if ((this.service.ServiceTypes & ServiceTypes.DiagJob) == ServiceTypes.None)
    {
      try
      {
        using (CaesarPreparation preparation = diagService.GetPreparation(this.indexDI))
        {
          this.bytePos = new long?((long) preparation.BytePosition);
          this.bitPos = new int?((int) preparation.BitPosition);
          this.byteLength = new long?((long) preparation.ByteLength);
          this.bitLength = new long?((long) preparation.BitLength);
          this.coding = new SapiLayer1.Coding?((SapiLayer1.Coding) preparation.Coding);
          this.typeSpecifier = new SapiLayer1.TypeSpecifier?((SapiLayer1.TypeSpecifier) preparation.TypeSpecifier);
          this.byteOrder = new SapiLayer1.ByteOrder?((SapiLayer1.ByteOrder) preparation.ByteOrder);
          this.dataType = new SapiLayer1.DataType?((SapiLayer1.DataType) preparation.DataType);
          this.conversionType = new ConversionType?((ConversionType) preparation.ConversionSelector);
          if (this.pT != 18)
          {
            IEnumerable<ScaleEntry> scales = ScaleEntry.GetScales(this.service.Channel.EcuHandle, preparation, (CaesarPresentation) null);
            ConversionType? conversionType1 = this.conversionType;
            ConversionType conversionType2 = ConversionType.FactorOffset;
            if ((conversionType1.GetValueOrDefault() == conversionType2 ? (conversionType1.HasValue ? 1 : 0) : 0) == 0)
            {
              conversionType1 = this.conversionType;
              ConversionType conversionType3 = ConversionType.Scale;
              if ((conversionType1.GetValueOrDefault() == conversionType3 ? (conversionType1.HasValue ? 1 : 0) : 0) == 0)
                goto label_14;
            }
            if (scales.Any<ScaleEntry>())
            {
              ScaleEntry scaleEntry = scales.First<ScaleEntry>();
              this.factor = new Decimal?(scaleEntry.Factor);
              this.offset = new Decimal?(scaleEntry.Offset);
              conversionType1 = this.conversionType;
              ConversionType conversionType4 = ConversionType.Scale;
              if ((conversionType1.GetValueOrDefault() == conversionType4 ? (conversionType1.HasValue ? 1 : 0) : 0) != 0)
              {
                this.scales = scales.ToList<ScaleEntry>();
                this.min = new Decimal?(scaleEntry.Min);
                this.max = new Decimal?(scaleEntry.Max);
              }
            }
label_14:
            Limits limits = preparation.GetLimits(this.service.Channel.EcuHandle);
            if (!string.IsNullOrEmpty(limits.Units))
              this.units = limits.Units;
            float? nullable;
            if (limits.Min.HasValue)
            {
              if (this.factor.HasValue && this.offset.HasValue)
              {
                nullable = limits.Min;
                this.min = new Decimal?((Convert.ToDecimal((object) (double) nullable.Value, (IFormatProvider) CultureInfo.InvariantCulture) - this.offset.Value) / this.factor.Value);
              }
              else
              {
                nullable = limits.Min;
                this.min = new Decimal?(Convert.ToDecimal((object) (double) nullable.Value, (IFormatProvider) CultureInfo.InvariantCulture));
              }
            }
            nullable = limits.Max;
            if (nullable.HasValue)
            {
              if (this.factor.HasValue && this.offset.HasValue)
              {
                nullable = limits.Max;
                this.max = new Decimal?((Convert.ToDecimal((object) (double) nullable.Value, (IFormatProvider) CultureInfo.InvariantCulture) - this.offset.Value) / this.factor.Value);
              }
              else
              {
                nullable = limits.Max;
                this.max = new Decimal?(Convert.ToDecimal((object) (double) nullable.Value, (IFormatProvider) CultureInfo.InvariantCulture));
              }
            }
          }
        }
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        CaesarException e = new CaesarException(ex, negativeResponseCode);
        Sapi.GetSapi().RaiseExceptionEvent((object) this, (System.Exception) e);
      }
    }
    if (diagService.GetPreperationHasDefaultValue((uint) this.indexDI))
    {
      this.defaultValue = diagService.GetPreparationDefaultValue((uint) this.indexDI, this.pT);
      if (this.defaultValue != null)
      {
        if (this.defaultValue.GetType() == typeof (byte[]))
          this.defaultValue = (object) new Dump((IEnumerable<byte>) (byte[]) this.defaultValue);
        else if (this.Type == typeof (Choice))
          this.defaultValue = (object) this.choices.GetItemFromOriginalName(this.defaultValue.ToString());
      }
      if (this.defaultValue != null)
        this.value = this.defaultValue;
    }
    if (this.type == typeof (string))
    {
      this.requiredLength = (object) diagService.GetPrepAsciiLength((uint) this.indexDI);
    }
    else
    {
      if (!(this.type == typeof (Dump)))
        return;
      this.requiredLength = (object) diagService.GetPrepDumpLength((uint) this.indexDI);
    }
  }

  internal ServiceArgumentValue StoreArgumentValue()
  {
    return this.argumentValues.Add(this.value, Sapi.Now, false, (object) this, this.inputValuePreProcessed);
  }

  internal void SetPreparation(McdDiagComPrimitive diagServiceIO, ServiceExecution execution = null)
  {
    this.Exception = (CaesarException) null;
    object newValue = this.PrepareValue(execution != null ? execution.InputArgumentValues.First<ServiceArgumentValue>((Func<ServiceArgumentValue, bool>) (iv => iv.InputValue.ParameterQualifier == this.ParameterQualifier)).Value : this.value, true);
    if (newValue == null)
      return;
    try
    {
      diagServiceIO.SetInput((int) this.indexDI, newValue);
    }
    catch (McdException ex)
    {
      this.Exception = new CaesarException(ex);
    }
  }

  internal void SetPreparation(CaesarDiagServiceIO diagServiceIO, ServiceExecution execution = null)
  {
    object obj = this.PrepareValue(execution != null ? execution.InputArgumentValues.First<ServiceArgumentValue>((Func<ServiceArgumentValue, bool>) (iv => iv.InputValue.ParameterQualifier == this.ParameterQualifier)).Value : this.value, false);
    if (obj == null)
      return;
    diagServiceIO.SetPrepParam(this.indexDM, this.pT, obj);
  }

  internal void SetPreparation(CaesarDiagService diagService)
  {
    this.Exception = (CaesarException) null;
    object obj = this.PrepareValue(this.value, false);
    if (obj == null)
      return;
    try
    {
      diagService.SetPrepParam(this.indexDI, this.pT, obj);
    }
    catch (CaesarErrorException ex)
    {
      this.Exception = new CaesarException(ex);
    }
  }

  public CaesarException Exception { get; private set; }

  private object PrepareValue(object newValue, bool choiceAsString)
  {
    if (newValue != null)
    {
      if (newValue.GetType() == typeof (string))
      {
        string str = newValue.ToString();
        if (this.PadOrTrim)
        {
          int int32 = Convert.ToInt32(this.requiredLength, (IFormatProvider) CultureInfo.InvariantCulture);
          if (str.Length > int32)
            str = str.Substring(0, int32);
          else if (str.Length < int32)
            str = str.PadRight(int32, ' ');
          newValue = (object) str;
        }
      }
      else if (newValue.GetType() == typeof (Dump))
      {
        IList<byte> data = ((Dump) newValue).Data;
        int count = data.Count;
        if (this.PadOrTrim)
        {
          int int32 = Convert.ToInt32(this.requiredLength, (IFormatProvider) CultureInfo.InvariantCulture);
          byte[] numArray = new byte[int32];
          for (int index = 0; index < data.Count && index < int32; ++index)
            numArray[index] = data[index];
          newValue = (object) numArray;
        }
        else
          newValue = (object) data.ToArray<byte>();
      }
      else if (newValue.GetType() == typeof (Choice))
      {
        Choice choice = (Choice) newValue;
        newValue = !choiceAsString ? (object) choice.Index : (object) choice.OriginalName;
      }
    }
    return newValue;
  }

  internal ServiceInputValue Clone()
  {
    return new ServiceInputValue((Service) null, this.indexDI, this.indexDM)
    {
      mcdRequestParameter = this.mcdRequestParameter,
      mcdEquivalentScaleInfo = this.mcdEquivalentScaleInfo,
      name = this.name,
      qualifier = this.qualifier,
      description = this.description,
      max = this.max,
      min = this.min,
      units = this.units,
      defaultValue = this.defaultValue,
      type = this.type,
      pT = this.pT,
      requiredLength = this.requiredLength,
      makeFit = this.makeFit,
      choices = this.choices
    };
  }

  internal System.Exception InternalSetValue(string newValue, Dictionary<string, string> variables)
  {
    System.Exception exception = (System.Exception) null;
    try
    {
      if (newValue.StartsWith("einput:", StringComparison.OrdinalIgnoreCase))
      {
        newValue = Sapi.Decrypt(new Dump(newValue.Remove(0, "einput:".Length)));
        this.inputValuePreProcessed = true;
      }
      else
      {
        newValue = variables == null || newValue.Length <= 2 || newValue[0] != '%' || newValue[newValue.Length - 1] != '%' ? newValue : variables[newValue.Substring(1, newValue.Length - 2)];
        this.inputValuePreProcessed = false;
      }
      if (this.Type == typeof (Choice))
      {
        Choice itemFromRawValue = this.Choices.GetItemFromRawValue((object) Convert.ToUInt32(newValue, (IFormatProvider) CultureInfo.InvariantCulture));
        if (itemFromRawValue != (object) null)
          this.SetValue((object) itemFromRawValue);
        else
          exception = (System.Exception) new ArgumentOutOfRangeException(nameof (newValue), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Raw value '{0}' not found", (object) newValue));
      }
      else if (this.type == typeof (Dump))
      {
        this.SetValue((object) newValue);
      }
      else
      {
        CultureInfo provider = new CultureInfo("en-US");
        this.SetValue(Convert.ChangeType((object) newValue, this.type, (IFormatProvider) provider));
      }
    }
    catch (InvalidOperationException ex)
    {
      exception = (System.Exception) ex;
    }
    catch (InvalidCastException ex)
    {
      exception = (System.Exception) ex;
    }
    catch (FormatException ex)
    {
      exception = (System.Exception) ex;
    }
    catch (NullReferenceException ex)
    {
      exception = (System.Exception) ex;
    }
    catch (OverflowException ex)
    {
      exception = (System.Exception) ex;
    }
    catch (KeyNotFoundException ex)
    {
      exception = (System.Exception) ex;
    }
    return exception;
  }

  public Service Service => this.service;

  public string Qualifier => this.qualifier;

  public string Description
  {
    get
    {
      if (string.IsNullOrEmpty(this.description) && this.mcdRequestParameter != null)
        this.description = this.mcdRequestParameter.Description;
      return this.service.Channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.qualifier, nameof (Description)), this.description);
    }
  }

  public string Name
  {
    get
    {
      return this.service.Channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.qualifier, nameof (Name)), this.name);
    }
  }

  public string Units
  {
    get
    {
      if (this.units == null && this.mcdRequestParameter != null)
        this.units = this.mcdRequestParameter.Unit ?? string.Empty;
      return this.units;
    }
  }

  public object Value
  {
    get => this.inputValuePreProcessed ? (object) "*****" : this.value;
    set
    {
      this.inputValuePreProcessed = false;
      this.SetValue(value);
    }
  }

  private void SetValue(object value)
  {
    if (value == null)
      throw new ArgumentNullException(nameof (value));
    if (this.type == (Type) null)
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot set service input value {0} because it does not have a valid type. (Caesar type={1})", (object) this.Name, (object) (int) this.pT));
    if (this.type.IsAssignableFrom(value.GetType()))
      this.value = value;
    else if (this.type == typeof (Choice))
    {
      string s = value.ToString();
      if (string.IsNullOrEmpty(s))
        return;
      this.value = (object) this.choices.FirstOrDefault<Choice>((Func<Choice, bool>) (c => c.Name == s || c.OriginalName == s));
      if (this.value == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} is an invalid choice value for service input value {1}", (object) s, (object) this.Name));
    }
    else if (this.type == typeof (Dump))
      this.value = (object) new Dump(value.ToString());
    else
      this.value = Convert.ChangeType(value, this.type, (IFormatProvider) CultureInfo.CurrentCulture);
  }

  public string ParameterQualifier => this.parameterQualifier;

  public Decimal? Max
  {
    get
    {
      if (!this.max.HasValue && this.mcdRequestParameter != null && this.McdEquivalentScaleInfo != null)
        this.max = this.McdEquivalentScaleInfo.Max;
      return this.max;
    }
  }

  public Decimal? Min
  {
    get
    {
      if (!this.min.HasValue && this.mcdRequestParameter != null && this.McdEquivalentScaleInfo != null)
        this.min = this.McdEquivalentScaleInfo.Min;
      return this.min;
    }
  }

  public Type Type => this.type;

  public object RequiredLength => this.requiredLength;

  public int Index => (int) this.indexDM;

  public ChoiceCollection Choices
  {
    get
    {
      if (this.type == typeof (Choice) && this.mcdRequestParameter != null && this.choices.Count == 0)
        this.choices.Add(this.mcdRequestParameter.TextTableElements);
      return this.choices;
    }
  }

  public object DefaultValue => this.defaultValue;

  public bool MakeFit
  {
    get => this.makeFit;
    set => this.makeFit = value;
  }

  public Decimal? Factor
  {
    get
    {
      if (!this.factor.HasValue && this.mcdRequestParameter != null && this.McdEquivalentScaleInfo != null)
        this.factor = new Decimal?(Convert.ToDecimal((object) this.McdEquivalentScaleInfo.Factor, (IFormatProvider) CultureInfo.InvariantCulture));
      return this.factor;
    }
  }

  public Decimal? Offset
  {
    get
    {
      if (!this.offset.HasValue && this.mcdRequestParameter != null && this.McdEquivalentScaleInfo != null)
        this.offset = new Decimal?(Convert.ToDecimal((object) this.McdEquivalentScaleInfo.Offset, (IFormatProvider) CultureInfo.InvariantCulture));
      return this.offset;
    }
  }

  public long? BytePosition
  {
    get
    {
      if (!this.bytePos.HasValue && this.mcdRequestParameter != null)
        this.bytePos = new long?(this.mcdRequestParameter.BytePos);
      return this.bytePos;
    }
  }

  public int? BitPosition
  {
    get
    {
      if (!this.bitPos.HasValue && this.mcdRequestParameter != null)
        this.bitPos = new int?((int) this.mcdRequestParameter.BitPos);
      return this.bitPos;
    }
  }

  public long? ByteLength
  {
    get
    {
      if (!this.byteLength.HasValue && this.mcdRequestParameter != null)
        this.byteLength = new long?(this.mcdRequestParameter.ByteLength);
      return this.byteLength;
    }
  }

  public long? BitLength
  {
    get
    {
      if (!this.bitLength.HasValue && this.mcdRequestParameter != null)
        this.bitLength = new long?(this.mcdRequestParameter.BitLength);
      return this.bitLength;
    }
  }

  public SapiLayer1.Coding? Coding
  {
    get
    {
      if (!this.coding.HasValue && this.mcdRequestParameter != null)
        this.coding = this.McdEquivalentScaleInfo != null ? this.McdEquivalentScaleInfo.Coding : new SapiLayer1.Coding?(SapiLayer1.Coding.Unsigned);
      return this.coding;
    }
  }

  public SapiLayer1.ByteOrder? ByteOrder
  {
    get
    {
      if (!this.byteOrder.HasValue && this.mcdRequestParameter != null)
        this.byteOrder = this.McdEquivalentScaleInfo != null ? this.McdEquivalentScaleInfo.ByteOrder : new SapiLayer1.ByteOrder?(SapiLayer1.ByteOrder.HighLow);
      return this.byteOrder;
    }
  }

  public SapiLayer1.TypeSpecifier? TypeSpecifier
  {
    get
    {
      if (!this.typeSpecifier.HasValue && this.mcdRequestParameter != null)
        this.typeSpecifier = new SapiLayer1.TypeSpecifier?(SapiLayer1.TypeSpecifier.Standard);
      return this.typeSpecifier;
    }
  }

  public SapiLayer1.DataType? DataType
  {
    get
    {
      if (!this.dataType.HasValue && this.mcdRequestParameter != null)
        this.dataType = new SapiLayer1.DataType?(this.BitLength.Value % 8L == 0L ? SapiLayer1.DataType.Byte : SapiLayer1.DataType.Bit);
      return this.dataType;
    }
  }

  public ConversionType? ConversionSelector
  {
    get
    {
      if (!this.conversionType.HasValue && this.mcdRequestParameter != null)
        this.conversionType = this.McdEquivalentScaleInfo != null ? this.McdEquivalentScaleInfo.ConversionType : new ConversionType?(ConversionType.Raw);
      return this.conversionType;
    }
  }

  public IEnumerable<ScaleEntry> Scales
  {
    get
    {
      if (this.scales == null && this.mcdRequestParameter != null && this.McdEquivalentScaleInfo != null)
      {
        ConversionType? conversionSelector = this.ConversionSelector;
        ConversionType conversionType = ConversionType.Scale;
        if ((conversionSelector.GetValueOrDefault() == conversionType ? (conversionSelector.HasValue ? 1 : 0) : 0) != 0)
        {
          List<ScaleEntry> scales = this.McdEquivalentScaleInfo.Scales;
          this.scales = scales != null ? scales.ToList<ScaleEntry>() : (List<ScaleEntry>) null;
        }
      }
      return this.scales == null ? (IEnumerable<ScaleEntry>) null : (IEnumerable<ScaleEntry>) this.scales.AsReadOnly();
    }
  }

  internal ScaleEntry FactorOffsetScale
  {
    get
    {
      if (this.factorOffsetScale == null && this.mcdRequestParameter != null && this.McdEquivalentScaleInfo != null)
      {
        ConversionType? conversionSelector = this.ConversionSelector;
        ConversionType conversionType = ConversionType.FactorOffset;
        if ((conversionSelector.GetValueOrDefault() == conversionType ? (conversionSelector.HasValue ? 1 : 0) : 0) != 0)
          this.factorOffsetScale = this.McdEquivalentScaleInfo.FactorOffsetScale;
      }
      return this.factorOffsetScale;
    }
  }

  public bool IsReserved { get; private set; }

  internal int? ReadAccess => this.readAccess;

  internal int? WriteAccess => this.writeAccess;

  private bool PadOrTrim
  {
    get
    {
      bool padOrTrim = false;
      if (this.makeFit && this.requiredLength != null)
        padOrTrim = Convert.ToInt32(this.requiredLength, (IFormatProvider) CultureInfo.InvariantCulture) > 0;
      return padOrTrim;
    }
  }

  internal void AddStringsForTranslation(Dictionary<string, string> table)
  {
    table[Sapi.MakeTranslationIdentifier(this.qualifier, "Name")] = this.name;
    if (!string.IsNullOrEmpty(this.description))
      table[Sapi.MakeTranslationIdentifier(this.qualifier, "Description")] = this.description;
    if (this.Choices == null)
      return;
    foreach (Choice choice in (ReadOnlyCollection<Choice>) this.Choices)
      choice.AddStringsForTranslation(table);
  }

  public ServiceArgumentValueCollection ArgumentValues => this.argumentValues;

  internal object ParseFromLog(string value)
  {
    return Presentation.ParseFromLog(value, this.type, this.choices, this.service.Channel.Ecu);
  }

  internal object GetPreparation(byte[] data)
  {
    if (!this.BytePosition.HasValue || !this.BitPosition.HasValue || !this.ByteLength.HasValue || !this.BitLength.HasValue)
      return (object) null;
    Decimal? factor = new Decimal?();
    Decimal? offset = new Decimal?();
    if (this.Factor.HasValue && this.Offset.HasValue)
    {
      Decimal? nullable = this.Factor;
      Decimal num1 = 0M;
      if ((nullable.GetValueOrDefault() == num1 ? (!nullable.HasValue ? 1 : 0) : 1) != 0)
      {
        Decimal num2 = (Decimal) 1;
        nullable = this.Factor;
        factor = nullable.HasValue ? new Decimal?(num2 / nullable.GetValueOrDefault()) : new Decimal?();
        nullable = this.Offset;
        Decimal num3 = this.Factor.Value;
        offset = nullable.HasValue ? new Decimal?(-(nullable.GetValueOrDefault() * num3)) : new Decimal?();
      }
    }
    return Presentation.Decode(data, (int) this.BytePosition.Value, this.BitPosition.Value, new int?((int) this.ByteLength.Value), new int?((int) this.BitLength.Value), this.ByteOrder, this.type, false, new int?(), factor, offset, this.Min, this.Max, this.Choices, (Ecu) null, this.name);
  }
}
