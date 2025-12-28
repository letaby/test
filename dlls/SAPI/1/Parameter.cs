// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Parameter
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Xml;
using System.Xml.Linq;

#nullable disable
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
  private Decimal? max;
  private Decimal? min;
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
  private bool serviceAsParameter;
  private List<ScaleEntry> scales;
  private ScaleEntry factorOffsetScale;
  private ParameterValueCollection parameterValues;

  internal Parameter(
    Channel ch,
    uint caesarIndex,
    string groupQualifier,
    string groupName,
    bool persistable,
    int collectionIndex)
  {
    this.readOnly = true;
    this.units = string.Empty;
    this.qualifier = string.Empty;
    this.description = string.Empty;
    this.name = string.Empty;
    this.channel = ch;
    this.caesarIndex = caesarIndex;
    this.collectionIndex = collectionIndex;
    this.groupQualifier = groupQualifier;
    this.groupName = groupName;
    this.pt = (ParamType) 20;
    this.persistable = persistable;
    this.parameterValues = new ParameterValueCollection(this);
  }

  private Decimal ScaleIfPossible(Decimal source)
  {
    return this.factor.HasValue && this.offset.HasValue ? source * this.factor.Value + this.offset.Value : source;
  }

  public Decimal? RestrictedMin { get; private set; }

  public Decimal? RestrictedMax { get; private set; }

  private void AcquireCommonEcuInfo(
    int? readAccess,
    int? writeAccess,
    Service readService,
    Service writeService)
  {
    int? infoLimitedRangeMin = this.Channel.DiagnosisVariant.GetEcuInfoLimitedRangeMin(this.CombinedQualifier);
    int? infoLimitedRangeMax = this.Channel.DiagnosisVariant.GetEcuInfoLimitedRangeMax(this.CombinedQualifier);
    this.choices = new ChoiceCollection(this.channel.Ecu, this.CombinedQualifier, this.channel.DiagnosisVariant.GetEcuInfoProhibitedChoices(this.CombinedQualifier), infoLimitedRangeMin, infoLimitedRangeMax);
    if (readAccess.HasValue && writeAccess.HasValue)
    {
      this.readAccessLevel = Math.Max(readService != (Service) null ? readService.Access : int.MaxValue, readAccess.Value);
      this.writeAccessLevel = Math.Max(writeService != (Service) null ? writeService.Access : int.MaxValue, writeAccess.Value);
    }
    else
    {
      this.readAccessLevel = readService != (Service) null ? readService.Access : int.MaxValue;
      this.writeAccessLevel = writeService != (Service) null ? writeService.Access : int.MaxValue;
    }
    int? infoReadAccessLevel = this.Channel.DiagnosisVariant.GetEcuInfoReadAccessLevel(this.CombinedQualifier);
    if (infoReadAccessLevel.HasValue)
      this.readAccessLevel = infoReadAccessLevel.Value;
    int? writeAccessLevel = this.Channel.DiagnosisVariant.GetEcuInfoWriteAccessLevel(this.CombinedQualifier);
    if (writeAccessLevel.HasValue)
      this.writeAccessLevel = writeAccessLevel.Value;
    Sapi sapi = Sapi.GetSapi();
    this.visible = sapi.ReadAccess >= this.readAccessLevel;
    this.readOnly = sapi.WriteAccess < this.writeAccessLevel;
  }

  internal void Acquire(
    string caesarEquivalentQualifier,
    ServiceInputValue varcodeFragment,
    string varcodeFragmentName,
    Service readService,
    Service writeService)
  {
    this.mcdVarcodeFragment = varcodeFragment;
    this.serviceAsParameter = false;
    this.readService = readService;
    this.writeService = writeService;
    this.name = varcodeFragmentName;
    this.description = varcodeFragment.Description;
    this.mcdQualifier = varcodeFragment.ParameterQualifier;
    this.qualifier = caesarEquivalentQualifier;
    this.marked = true;
    this.units = (string) null;
    this.AcquireCommonEcuInfo(varcodeFragment.ReadAccess, varcodeFragment.WriteAccess, readService, writeService);
    this.type = varcodeFragment.Type;
    if (this.type == typeof (Choice))
    {
      this.pt = (ParamType) 18;
    }
    else
    {
      if (!(this.type == typeof (Dump)))
        return;
      this.pt = (ParamType) 15;
    }
  }

  private void AcquireScalesMinMaxFromMcd()
  {
    if (!(this.type != typeof (Choice)) || !(this.type != typeof (Dump)) || this.mcdVarcodeFragment.Scales == null)
      return;
    this.scales = this.mcdVarcodeFragment.Scales.ToList<ScaleEntry>();
    this.ProcessScaleRanges(this.mcdVarcodeFragment.ByteLength.Value);
  }

  private void ProcessScaleRanges(long byteLength)
  {
    IEnumerable<ScaleEntry> scaleEntries = this.scales.Where<ScaleEntry>((Func<ScaleEntry, bool>) (scale => !scale.IsFixedValue));
    if (!scaleEntries.Any<ScaleEntry>())
      return;
    this.min = new Decimal?(this.Quantize(scaleEntries.Min<ScaleEntry>((Func<ScaleEntry, Decimal>) (scale => scale.Min))));
    this.max = new Decimal?(this.Quantize(scaleEntries.Max<ScaleEntry>((Func<ScaleEntry, Decimal>) (scale => scale.Max))));
    ulong num = Convert.ToUInt64(Math.Pow(256.0, (double) byteLength)) - 1UL;
    foreach (ScaleEntry scaleEntry in this.scales.Except<ScaleEntry>(scaleEntries))
    {
      Decimal ecuValue = scaleEntry.ToEcuValue(scaleEntry.Min);
      if (ecuValue == (Decimal) num)
        scaleEntry.Name = "sna";
      string str = this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.CombinedQualifier, ecuValue.ToString((IFormatProvider) CultureInfo.InvariantCulture), "ScaleEntryName"), (string) null);
      if (str != null)
        scaleEntry.Name = str;
    }
  }

  internal void Acquire(
    Varcode varcode,
    CaesarDIVarCodeFrag varcodeFragment,
    byte[] defaultString,
    Service readService,
    Service writeService)
  {
    this.serviceAsParameter = false;
    this.readService = readService;
    this.writeService = writeService;
    this.name = varcodeFragment.Name;
    this.description = varcodeFragment.Description;
    this.qualifier = varcodeFragment.Qualifier;
    int? infoLimitedRangeMin = this.Channel.DiagnosisVariant.GetEcuInfoLimitedRangeMin(this.CombinedQualifier);
    int? infoLimitedRangeMax = this.Channel.DiagnosisVariant.GetEcuInfoLimitedRangeMax(this.CombinedQualifier);
    ushort? nullable1 = varcodeFragment.AccessLevels != null ? new ushort?(varcodeFragment.AccessLevels.Read) : new ushort?();
    int? readAccess = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
    ushort? nullable2 = varcodeFragment.AccessLevels != null ? new ushort?(varcodeFragment.AccessLevels.Write) : new ushort?();
    int? writeAccess = nullable2.HasValue ? new int?((int) nullable2.GetValueOrDefault()) : new int?();
    Service readService1 = readService;
    Service writeService1 = writeService;
    this.AcquireCommonEcuInfo(readAccess, writeAccess, readService1, writeService1);
    uint fragValueCount = varcodeFragment.FragValueCount;
    if (fragValueCount > 0U)
    {
      this.type = typeof (Choice);
      this.pt = (ParamType) 18;
      uint bitLength = (uint) varcodeFragment.BitLength;
      CaesarAbstraction.ByteOrder byteOrder = varcodeFragment.ByteOrder;
      for (uint index = 0; index < fragValueCount; ++index)
      {
        using (CaesarDICbfFragValue fragValue = varcodeFragment.GetFragValue(index))
        {
          if (fragValue != null)
            this.Choices.Add(new Choice(fragValue.Meaning, (object) fragValue.GetValue(bitLength, byteOrder)));
        }
      }
    }
    else
    {
      using (CaesarPreparation preparation = varcodeFragment.GetPreparation(this.channel.EcuHandle))
      {
        using (CaesarPresentation presentation = varcodeFragment.GetPresentation(this.channel.EcuHandle))
        {
          this.AcquirePresentation(presentation);
          if (preparation != null)
          {
            this.pt = preparation.Type;
            this.type = Sapi.GetRealCaesarType(this.pt);
            this.conversionType = new ConversionType?((ConversionType) preparation.ConversionSelector);
            if (this.pt == 18)
            {
              uint enumerationEntries = preparation.NumberEnumerationEntries;
              for (uint index = 0; index < enumerationEntries; ++index)
              {
                DictionaryEntry enumerationEntry = preparation.GetEnumerationEntry(this.channel.EcuHandle, index);
                this.Choices.Add(new Choice(enumerationEntry.Key.ToString(), enumerationEntry.Value));
              }
            }
            else
            {
              int num;
              if (this.pt == 6 && this.channel.Ecu.SupportsDoublePrecisionVariantCoding)
              {
                SapiLayer1.Coding? coding1 = this.Coding;
                SapiLayer1.Coding coding2 = SapiLayer1.Coding.Unsigned;
                if ((coding1.GetValueOrDefault() == coding2 ? (coding1.HasValue ? 1 : 0) : 0) == 0)
                {
                  coding1 = this.Coding;
                  SapiLayer1.Coding coding3 = SapiLayer1.Coding.TwosComplement;
                  if ((coding1.GetValueOrDefault() == coding3 ? (coding1.HasValue ? 1 : 0) : 0) == 0)
                    goto label_22;
                }
                if (this.bitPos.Value == 0)
                {
                  num = this.bitLength.Value % 8L == 0L ? 1 : 0;
                  goto label_23;
                }
              }
label_22:
              num = 0;
label_23:
              bool flag = num != 0;
              IEnumerable<ScaleEntry> scales = ScaleEntry.GetScales(this.channel.EcuHandle, preparation, flag ? presentation : (CaesarPresentation) null);
              ConversionType? conversionType1 = this.conversionType;
              ConversionType conversionType2 = ConversionType.FactorOffset;
              if ((conversionType1.GetValueOrDefault() == conversionType2 ? (conversionType1.HasValue ? 1 : 0) : 0) == 0)
              {
                conversionType1 = this.conversionType;
                ConversionType conversionType3 = ConversionType.Scale;
                if ((conversionType1.GetValueOrDefault() == conversionType3 ? (conversionType1.HasValue ? 1 : 0) : 0) == 0)
                  goto label_31;
              }
              if (scales.Any<ScaleEntry>())
              {
                ScaleEntry scaleEntry = scales.First<ScaleEntry>();
                this.factor = new Decimal?(1M / scaleEntry.Factor);
                this.offset = new Decimal?(-(scaleEntry.Offset * this.factor.Value));
                this.precision = (object) Math.Max(Sapi.CalculatePrecision(Convert.ToDouble(this.factor.Value)), Sapi.CalculatePrecision(Convert.ToDouble(this.offset.Value)));
                conversionType1 = this.conversionType;
                ConversionType conversionType4 = ConversionType.Scale;
                if ((conversionType1.GetValueOrDefault() == conversionType4 ? (conversionType1.HasValue ? 1 : 0) : 0) != 0)
                {
                  this.scales = scales.ToList<ScaleEntry>();
                  this.ProcessScaleRanges((long) preparation.ByteLength);
                }
                else
                  this.factorOffsetScale = scaleEntry;
                if (flag)
                  this.type = typeof (double);
              }
label_31:
              string ecuInfoAttribute = this.channel.DiagnosisVariant.GetEcuInfoAttribute<string>("Choices", this.CombinedQualifier);
              if (ecuInfoAttribute != null)
              {
                this.choices.Add(ecuInfoAttribute, this.type);
                this.type = typeof (Choice);
                this.min = this.max = new Decimal?();
              }
              else
              {
                Limits limits = preparation.GetLimits(this.Channel.EcuHandle);
                float? nullable3;
                if (!this.min.HasValue && limits.Min.HasValue)
                {
                  nullable3 = limits.Min;
                  this.min = new Decimal?(this.ScaleIfPossible(nullable3.Value.ToDecimal()));
                }
                if (!this.max.HasValue)
                {
                  nullable3 = limits.Max;
                  if (nullable3.HasValue)
                  {
                    nullable3 = limits.Max;
                    this.max = new Decimal?(this.ScaleIfPossible(nullable3.Value.ToDecimal()));
                  }
                }
                if (!string.IsNullOrEmpty(limits.Units))
                  this.units = limits.Units;
                if (infoLimitedRangeMin.HasValue)
                  this.min = this.RestrictedMin = new Decimal?(this.ScaleIfPossible((Decimal) infoLimitedRangeMin.Value));
                if (infoLimitedRangeMax.HasValue)
                  this.max = this.RestrictedMax = new Decimal?(this.ScaleIfPossible((Decimal) infoLimitedRangeMax.Value));
              }
            }
          }
        }
      }
    }
    if (defaultString != null)
    {
      this.defaultValue = this.GetPresentation(varcode);
      if (varcode.IsErrorSet)
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Error while retrieving default value: " + varcode.Exception.Message);
    }
    if (this.defaultValue == null)
    {
      this.defaultValue = varcodeFragment.DefaultValue;
      if (this.defaultValue != null)
      {
        if (this.defaultValue.GetType() == typeof (float))
          this.defaultValue = !(this.type == typeof (double)) ? (object) this.Quantize(Convert.ToSingle(this.defaultValue, (IFormatProvider) CultureInfo.InvariantCulture)) : (object) this.Quantize((double) Convert.ToSingle(this.defaultValue, (IFormatProvider) CultureInfo.InvariantCulture));
        else if (this.defaultValue.GetType() == typeof (byte[]))
          this.defaultValue = (object) new Dump((IEnumerable<byte>) (byte[]) this.defaultValue);
        else if (this.Type == typeof (Choice))
          this.defaultValue = (object) this.choices.GetItemFromOriginalName(this.defaultValue.ToString());
      }
    }
    this.marked = true;
  }

  internal void Acquire(
    string qualifier,
    string name,
    Service writeService,
    string writeReference,
    int writeReferenceIndex,
    Service readService,
    string readReference,
    bool hide)
  {
    this.qualifier = qualifier;
    this.name = name;
    this.writeService = writeService;
    this.writeReferenceIndex = writeReferenceIndex;
    this.writeReference = writeReference;
    this.readService = readService;
    this.readReference = readReference;
    this.serviceAsParameter = true;
    Decimal? nullable1;
    if (this.readService != (Service) null)
    {
      this.readAccessLevel = this.readService.Access;
      ServiceOutputValue outputValue = this.readService.OutputValues[0];
      if (outputValue != null)
      {
        this.units = outputValue.Units;
        this.choices = outputValue.Choices;
        this.min = outputValue.Min.HasValue ? new Decimal?(Convert.ToDecimal((object) outputValue.Min, (IFormatProvider) CultureInfo.InvariantCulture)) : new Decimal?();
        nullable1 = outputValue.Max;
        Decimal? nullable2;
        if (!nullable1.HasValue)
        {
          nullable1 = new Decimal?();
          nullable2 = nullable1;
        }
        else
          nullable2 = new Decimal?(Convert.ToDecimal((object) outputValue.Max, (IFormatProvider) CultureInfo.InvariantCulture));
        this.max = nullable2;
        this.description = outputValue.Description;
        this.type = outputValue.Type;
        if (!hide)
          this.visible = Sapi.GetSapi().ReadAccess >= this.readAccessLevel;
      }
    }
    else
      this.readAccessLevel = int.MaxValue;
    if (this.writeService != (Service) null)
    {
      this.writeAccessLevel = this.writeService.Access;
      ServiceInputValue inputValue = this.writeService.InputValues[this.writeReferenceIndex];
      if (inputValue != null)
      {
        this.units = inputValue.Units;
        this.choices = inputValue.Choices;
        nullable1 = inputValue.Min;
        Decimal? nullable3;
        if (!nullable1.HasValue)
        {
          nullable1 = new Decimal?();
          nullable3 = nullable1;
        }
        else
          nullable3 = new Decimal?(Convert.ToDecimal((object) inputValue.Min, (IFormatProvider) CultureInfo.InvariantCulture));
        this.min = nullable3;
        nullable1 = inputValue.Max;
        Decimal? nullable4;
        if (!nullable1.HasValue)
        {
          nullable1 = new Decimal?();
          nullable4 = nullable1;
        }
        else
          nullable4 = new Decimal?(Convert.ToDecimal((object) inputValue.Max, (IFormatProvider) CultureInfo.InvariantCulture));
        this.max = nullable4;
        this.description = inputValue.Description;
        this.type = inputValue.Type;
        this.defaultValue = inputValue.DefaultValue;
        this.readOnly = Sapi.GetSapi().WriteAccess < this.writeAccessLevel;
      }
    }
    else
      this.writeAccessLevel = int.MaxValue;
    this.marked = true;
  }

  private static Decimal? GetDecimalValue(string value)
  {
    Decimal result;
    return !string.IsNullOrEmpty(value) && Decimal.TryParse(value, out result) ? new Decimal?(result) : new Decimal?();
  }

  internal void Acquire(string[] fields)
  {
    this.name = fields[1];
    this.qualifier = fields[3];
    switch (fields[4])
    {
      case "B":
        this.readAccessLevel = this.writeAccessLevel = 1;
        break;
      case "R":
        this.readAccessLevel = 1;
        this.writeAccessLevel = int.MaxValue;
        break;
      case "W":
        this.writeAccessLevel = 1;
        this.readAccessLevel = int.MaxValue;
        break;
    }
    this.units = fields[5];
    this.defaultValue = (object) Parameter.GetDecimalValue(fields[6]);
    this.min = Parameter.GetDecimalValue(fields[7]);
    this.max = Parameter.GetDecimalValue(fields[8]);
    this.factor = Parameter.GetDecimalValue(fields[9]);
    if (this.factor.HasValue)
      this.offset = new Decimal?(0M);
    Sapi sapi = Sapi.GetSapi();
    this.visible = sapi.ReadAccess >= this.readAccessLevel;
    this.readOnly = sapi.WriteAccess < this.writeAccessLevel;
    if (this.factor.HasValue)
    {
      this.type = this.factor.Value == 1M ? typeof (int) : typeof (double);
      this.precision = (object) Sapi.CalculatePrecision(Convert.ToDouble(this.factor.Value));
    }
    else
      this.type = typeof (string);
    this.marked = true;
  }

  internal void InternalRead(string[] fields)
  {
    CaesarException e = (CaesarException) null;
    object objB = this.value;
    string field = fields[2];
    if (field == "0000")
    {
      if (this.type != typeof (string))
      {
        Decimal? decimalValue = Parameter.GetDecimalValue(fields[3]);
        this.value = decimalValue.HasValue ? Convert.ChangeType((object) decimalValue, this.type, (IFormatProvider) CultureInfo.InvariantCulture) : (object) null;
      }
      else
        this.value = (object) fields[3];
      this.hasBeenReadFromEcu = true;
      this.originalValue = this.value;
      this.parameterValues.Add(new ParameterValue(this.value, Sapi.Now));
    }
    else
      e = new CaesarException(SapiError.ExternalVcpParameterError, $"{field} - {fields[4]}");
    if (e == null)
    {
      if (objB != null && !object.Equals(this.value, objB) && (this.channel.CommunicationsState == CommunicationsState.WriteParameters || this.channel.CommunicationsState == CommunicationsState.ProcessVcp))
        this.RaiseParameterUpdateEvent((Exception) new CaesarException(SapiError.FragmentReadWriteMismatch));
      else
        this.RaiseParameterUpdateEvent((Exception) null);
    }
    else
      this.RaiseParameterUpdateEvent((Exception) e);
  }

  private void AcquirePresentation(CaesarPresentation presentation)
  {
    if (presentation == null)
      return;
    this.bytePos = new long?((long) presentation.BytePosition);
    this.bitPos = new int?((int) presentation.BitPosition);
    this.byteLength = new long?((long) presentation.ByteLength);
    this.bitLength = new long?((long) presentation.BitLength);
    this.coding = new SapiLayer1.Coding?((SapiLayer1.Coding) presentation.Coding);
    this.typeSpecifier = new SapiLayer1.TypeSpecifier?((SapiLayer1.TypeSpecifier) presentation.TypeSpecifier);
    this.byteOrder = new SapiLayer1.ByteOrder?((SapiLayer1.ByteOrder) presentation.ByteOrder);
    this.dataType = new SapiLayer1.DataType?((SapiLayer1.DataType) presentation.DataType);
  }

  [HandleProcessCorruptedStateExceptions]
  internal bool InternalRead(Varcode varcode, bool fromDevice)
  {
    if (fromDevice && this.channel.IsChannelErrorSet)
      return false;
    object objB = this.value;
    Exception e = (Exception) null;
    if (!this.serviceAsParameter)
    {
      if (Sapi.GetSapi().HardwareAccess >= this.readAccessLevel || Sapi.GetSapi().InitState == InitState.Offline)
      {
        if (varcode != null)
        {
          this.value = this.GetPresentation(varcode);
          if (varcode.IsErrorSet)
            e = (Exception) varcode.Exception;
        }
      }
      else
        e = (Exception) new CaesarException(SapiError.NoSecurityAccessForReadingThatFragment);
    }
    else if (this.readService != (Service) null)
    {
      e = this.readService.InputValues.InternalParseValues(this.readReference);
      if (e == null)
      {
        try
        {
          e = (Exception) this.readService.InternalExecute(Service.ExecuteType.SystemInvoke);
        }
        catch (AccessViolationException ex)
        {
          e = (Exception) ex;
        }
        if (e == null)
        {
          if (this.channel.IsChannelErrorSet)
          {
            if (this.channel.ChannelHandle != null)
              e = (Exception) new CaesarException(this.channel.ChannelHandle);
          }
          else
          {
            ServiceOutputValue outputValue = this.readService.OutputValues[0];
            if (outputValue != null)
              this.value = outputValue.Value;
          }
        }
      }
    }
    else
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Read service not defined");
    if (fromDevice)
    {
      this.originalValue = this.value;
      this.hasBeenReadFromEcu = true;
      this.parameterValues.Add(new ParameterValue(this.value, Sapi.Now));
    }
    if (e == null)
    {
      if (objB != null && !object.Equals(this.value, objB) && (this.channel.CommunicationsState == CommunicationsState.WriteParameters || this.channel.CommunicationsState == CommunicationsState.ProcessVcp))
        this.RaiseParameterUpdateEvent((Exception) new CaesarException(SapiError.FragmentReadWriteMismatch));
      else
        this.RaiseParameterUpdateEvent((Exception) null);
    }
    else
      this.RaiseParameterUpdateEvent(e);
    return true;
  }

  internal void InternalWrite(Varcode varcode, bool resetHaveBeenReadFromEcu = true)
  {
    if (Sapi.GetSapi().HardwareAccess >= this.writeAccessLevel || Sapi.GetSapi().InitState == InitState.Offline)
    {
      if (!this.serviceAsParameter)
      {
        if (varcode != null)
        {
          try
          {
            object obj = this.value;
            if (this.pt == 18)
              varcode.SetFragmentValue(this, obj);
            else if (obj != null && this.type == typeof (Choice) && obj is Choice choice)
            {
              if (obj != (object) ChoiceCollection.InvalidChoice)
              {
                if (choice.RawValue is float)
                  varcode.SetFragmentValue(this, (object) this.Quantize(Convert.ToSingle(choice.RawValue, (IFormatProvider) CultureInfo.InvariantCulture)));
                else if (choice.RawValue is double)
                  varcode.SetFragmentValue(this, (object) this.Quantize(Convert.ToDouble(choice.RawValue, (IFormatProvider) CultureInfo.InvariantCulture)));
                else
                  varcode.SetFragmentValue(this, choice.RawValue);
              }
            }
            else
              varcode.SetFragmentValue(this, obj);
          }
          catch (NullReferenceException ex)
          {
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Intentional catch of exception in Parameter::InternalWrite. Comms failure?");
          }
          CaesarException e = (CaesarException) null;
          if (varcode.IsErrorSet)
            e = varcode.Exception;
          else if (this.channel.LogFile == null)
          {
            object presentation = this.GetPresentation(varcode);
            if (varcode.IsErrorSet)
              e = varcode.Exception;
            else
              this.value = presentation;
          }
          if (e != null)
            this.RaiseParameterUpdateEvent((Exception) e);
        }
      }
      else
      {
        Exception e;
        if (this.writeService != (Service) null)
        {
          e = this.writeService.InputValues.InternalParseValues(this.writeReference);
          if (e == null)
          {
            ServiceInputValue inputValue = this.writeService.InputValues[this.writeReferenceIndex];
            if (inputValue != null)
            {
              if (inputValue.Type == typeof (Dump) && inputValue.Value != null)
              {
                IList<byte> byteList = (this.Value as Dump).Data;
                IList<byte> data = (inputValue.Value as Dump).Data;
                if (byteList.Count < data.Count)
                  byteList = (IList<byte>) byteList.Concat<byte>(data.Skip<byte>(byteList.Count)).ToList<byte>();
                int int32 = Convert.ToInt32(inputValue.RequiredLength, (IFormatProvider) CultureInfo.InvariantCulture);
                if (byteList.Count > int32)
                  byteList = (IList<byte>) byteList.Take<byte>(int32).ToList<byte>();
                inputValue.Value = (object) new Dump((IEnumerable<byte>) byteList);
              }
              else
                inputValue.Value = this.Value;
            }
            e = (Exception) this.writeService.InternalExecute(Service.ExecuteType.SystemInvoke);
            if (e == null)
            {
              if (this.channel.IsChannelErrorSet)
              {
                if (this.channel.ChannelHandle != null)
                  e = (Exception) new CaesarException(this.channel.ChannelHandle);
              }
              else
              {
                this.value = inputValue.Value;
                this.channel.ClearCache();
              }
            }
          }
        }
        else
          e = (Exception) new CaesarException(SapiError.WriteServiceNotAvailable);
        if (e != null)
          this.RaiseParameterUpdateEvent(e);
      }
    }
    else
      this.RaiseParameterUpdateEvent((Exception) new CaesarException(SapiError.AccessDeniedAuthorizationLevelTooLow));
    if (!resetHaveBeenReadFromEcu)
      return;
    this.hasBeenReadFromEcu = false;
  }

  internal void RaiseParameterUpdateEvent(Exception e)
  {
    if (this.exception == null)
      this.exception = e;
    FireAndForget.Invoke((MulticastDelegate) this.ParameterUpdateEvent, (object) this, (EventArgs) new ResultEventArgs(this.exception));
    this.channel.Parameters.RaiseParameterUpdateEvent(this, this.exception);
  }

  internal void ResetException() => this.exception = (Exception) null;

  internal void ResetHasBeenReadFromEcu()
  {
    this.hasBeenReadFromEcu = false;
    if (object.Equals(this.value, this.originalValue))
      this.value = (object) null;
    this.originalValue = (object) null;
  }

  public object ParseValue(string newValue)
  {
    object obj;
    if (this.Type == typeof (Choice))
    {
      Choice itemFromRawValue = this.Choices.GetItemFromRawValue((object) newValue);
      obj = itemFromRawValue != (object) null ? (object) itemFromRawValue : throw new ArgumentOutOfRangeException(nameof (newValue), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Raw value '{0}' not found", (object) newValue));
    }
    else if (this.type == typeof (Dump))
    {
      obj = (object) new Dump(newValue);
    }
    else
    {
      CultureInfo provider = new CultureInfo("en-US");
      obj = !(this.type == typeof (float)) ? (!(this.type == typeof (double)) ? Convert.ChangeType((object) newValue, this.type, (IFormatProvider) provider) : (object) this.Quantize(Convert.ToDouble(newValue, (IFormatProvider) provider))) : (object) this.Quantize(Convert.ToSingle(newValue, (IFormatProvider) provider));
    }
    return obj;
  }

  internal void InternalSetValue(string newValue, bool respectAccessLevel)
  {
    Exception e = (Exception) null;
    try
    {
      object objA = this.ParseValue(newValue);
      if (respectAccessLevel && this.writeAccessLevel > Sapi.GetSapi().WriteAccess)
      {
        if (!object.Equals(objA, this.value))
          e = (Exception) new CaesarException(SapiError.AccessDeniedAuthorizationLevelTooLow);
      }
      else
        this.Value = objA;
    }
    catch (ArgumentOutOfRangeException ex)
    {
      e = (Exception) ex;
    }
    catch (InvalidOperationException ex)
    {
      e = (Exception) ex;
    }
    catch (InvalidCastException ex)
    {
      e = (Exception) ex;
    }
    catch (FormatException ex)
    {
      e = (Exception) ex;
    }
    catch (NullReferenceException ex)
    {
      e = (Exception) ex;
    }
    catch (OverflowException ex)
    {
      e = (Exception) ex;
    }
    catch (ArgumentException ex)
    {
      e = (Exception) ex;
    }
    if (e != null)
      this.RaiseParameterUpdateEvent(e);
    this.lastPersistedValue = this.Value;
  }

  internal void InternalSetValueFromLogFile(ParameterValue value)
  {
    this.value = this.originalValue = this.lastPersistedValue = value?.Value;
    this.hasBeenReadFromEcu = this.value != null;
  }

  internal string InternalGetValue()
  {
    this.lastPersistedValue = this.Value;
    if (this.Value == null || this.Value == (object) ChoiceCollection.InvalidChoice)
      return string.Empty;
    string str = this.FormatValue(this.Value);
    if (str.IndexOf(',') != -1)
      str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) str);
    return str;
  }

  internal string FormatValue(object objValue)
  {
    if (this.type == typeof (Choice))
    {
      Choice choice = (Choice) objValue;
      return !(choice != (object) ChoiceCollection.InvalidChoice) ? string.Empty : choice.RawValue.ToString();
    }
    if (this.type == typeof (Dump))
      return objValue.ToString();
    CultureInfo provider = new CultureInfo("en-US");
    if (this.type == typeof (float))
    {
      float valueToQuantize = (float) objValue;
      float single = Convert.ToSingle(valueToQuantize.ToString((IFormatProvider) provider), (IFormatProvider) provider);
      if (valueToQuantize.CompareTo(single) != 0)
      {
        float num1 = this.Quantize(valueToQuantize);
        float num2 = this.Quantize(single);
        if (num1.CompareTo(num2) != 0)
          return num1.ToString("R", (IFormatProvider) provider);
      }
    }
    else if (this.type == typeof (double))
    {
      double valueToQuantize1 = (double) objValue;
      double valueToQuantize2 = Convert.ToDouble(valueToQuantize1.ToString((IFormatProvider) provider), (IFormatProvider) provider);
      if (valueToQuantize1.CompareTo(valueToQuantize2) != 0)
      {
        double num3 = this.Quantize(valueToQuantize1);
        double num4 = this.Quantize(valueToQuantize2);
        if (num3.CompareTo(num4) != 0)
          return num3.ToString("R", (IFormatProvider) provider);
      }
    }
    return Convert.ToString(objValue, (IFormatProvider) provider);
  }

  internal bool LastInGroup
  {
    get => this.lastInGroup;
    set => this.lastInGroup = value;
  }

  internal object GetPresentation(Varcode varcode)
  {
    object rawValue = (object) null;
    try
    {
      object fragmentValue = varcode.GetFragmentValue(this);
      if (!varcode.IsErrorSet)
        rawValue = this.pt != 6 ? fragmentValue : (fragmentValue == null || !(fragmentValue is float) ? fragmentValue : (object) this.Quantize(Convert.ToSingle(fragmentValue, (IFormatProvider) CultureInfo.InvariantCulture)));
      else
        Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) varcode.Exception);
    }
    catch (NullReferenceException ex)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Intentional catch of exception in Parameter::InternalRead. Comms failure?");
    }
    catch (InvalidCastException ex)
    {
      Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) new CaesarException(SapiError.UnknownPresentationType));
    }
    if (rawValue != null && this.type == typeof (Choice) && (object) (rawValue as Choice) == null)
    {
      Choice choice = this.Choices.GetItemFromRawValue(rawValue);
      if ((object) choice == null)
        choice = ChoiceCollection.InvalidChoice;
      rawValue = (object) choice;
    }
    return rawValue;
  }

  internal float Quantize(float valueToQuantize)
  {
    ConversionType? conversionSelector = this.ConversionSelector;
    if (conversionSelector.HasValue)
    {
      ConversionType? nullable = conversionSelector;
      ConversionType conversionType = ConversionType.Ieee;
      if ((nullable.GetValueOrDefault() == conversionType ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        return valueToQuantize;
    }
    return Convert.ToSingle((object) this.Quantize(valueToQuantize.ToDecimal()), (IFormatProvider) CultureInfo.InvariantCulture);
  }

  internal double Quantize(double valueToQuantize)
  {
    return (double) this.Quantize(Convert.ToDecimal((object) valueToQuantize, (IFormatProvider) CultureInfo.InvariantCulture));
  }

  internal Decimal Quantize(Decimal valueToQuantize)
  {
    if (this.Scales != null)
    {
      ScaleEntry scaleEntry = this.Scales.FirstOrDefault<ScaleEntry>((Func<ScaleEntry, bool>) (sc => sc.IsValueInRange(valueToQuantize)));
      if (scaleEntry != null)
      {
        Decimal physicalValue = scaleEntry.ToPhysicalValue(scaleEntry.ToEcuValue(valueToQuantize));
        return Math.Min(scaleEntry.Max, Math.Max(scaleEntry.Min, physicalValue));
      }
    }
    else if (this.Factor.HasValue && this.Offset.HasValue)
      return Math.Round((valueToQuantize - this.offset.Value) / this.Factor.Value, 0, MidpointRounding.AwayFromZero) * this.Factor.Value + this.Offset.Value;
    return valueToQuantize;
  }

  public override string ToString() => this.qualifier;

  public Channel Channel => this.channel;

  public string Qualifier => this.qualifier;

  public string McdQualifier => this.mcdQualifier;

  internal string CombinedQualifier
  {
    get
    {
      if (this.combinedQualifier == null)
        this.combinedQualifier = string.Join(".", this.groupQualifier, this.qualifier);
      return this.combinedQualifier;
    }
  }

  public string Name
  {
    get
    {
      return this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.CombinedQualifier, nameof (Name)), this.name);
    }
  }

  public string ShortName => this.Name;

  public string OriginalName => this.name;

  public string Description
  {
    get
    {
      return this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.CombinedQualifier, nameof (Description)), this.description);
    }
  }

  public string GroupQualifier => this.groupQualifier;

  public string GroupName
  {
    get
    {
      return this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.groupQualifier, nameof (GroupName)), this.groupName);
    }
  }

  public string OriginalGroupName => this.groupName;

  public string GroupCodingString
  {
    get => this.Channel.Parameters.GroupCodingStrings[this.GroupQualifier];
    set => this.Channel.Parameters.SetGroupCodingString(this.GroupQualifier, value);
  }

  public Dump CodingValue
  {
    get
    {
      string groupCodingString = this.GroupCodingString;
      return groupCodingString != null ? new Dump((IEnumerable<byte>) new List<byte>((IEnumerable<byte>) new Dump(groupCodingString).Data).Skip<byte>(Convert.ToInt32((object) this.BytePosition, (IFormatProvider) CultureInfo.InvariantCulture)).Take<byte>(Convert.ToInt32((object) this.ByteLength, (IFormatProvider) CultureInfo.InvariantCulture)).ToArray<byte>()) : (Dump) null;
    }
    set
    {
      string groupCodingString = this.GroupCodingString;
      if (groupCodingString == null)
        return;
      List<byte> data = new List<byte>((IEnumerable<byte>) new Dump(groupCodingString).Data);
      for (int index = 0; index < Convert.ToInt32((object) this.ByteLength, (IFormatProvider) CultureInfo.InvariantCulture); ++index)
        data[Convert.ToInt32((object) this.BytePosition, (IFormatProvider) CultureInfo.InvariantCulture) + index] = value.Data[index];
      this.Channel.Parameters.SetGroupCodingString(this.GroupQualifier, new Dump((IEnumerable<byte>) data).ToString(), Enumerable.Repeat<Parameter>(this, 1));
    }
  }

  public string OriginalGroupCodingString
  {
    get => this.Channel.Parameters.OriginalGroupCodingStrings[this.GroupQualifier];
  }

  public string Units
  {
    get
    {
      if (this.units == null && this.mcdVarcodeFragment != null)
        this.units = this.mcdVarcodeFragment.Units ?? string.Empty;
      return this.units;
    }
  }

  public ChoiceCollection Choices
  {
    get
    {
      if (this.choices.Count == 0 && this.type == typeof (Choice) && this.mcdVarcodeFragment != null)
      {
        foreach (Choice choice in (ReadOnlyCollection<Choice>) this.mcdVarcodeFragment.Choices)
          this.choices.Add(choice);
      }
      return this.choices;
    }
  }

  public bool Visible
  {
    get => (this.Channel.LogFile == null || this.parameterValues.Count != 0) && this.visible;
  }

  public bool ReadOnly => this.readOnly;

  public Service CombinedService => (Service) null;

  public object Value
  {
    get => this.value;
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (this.type == (Type) null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot change parameter {0} because it does not have a valid type. (Caesar type={1})", (object) this.Name, (object) (int) this.pt));
      if (this.channel.CommunicationsState == CommunicationsState.ReadParameters || this.channel.CommunicationsState == CommunicationsState.WriteParameters)
        throw new InvalidOperationException("Cannot change a parameter whilst a read or write is in progress");
      this.channel.Parameters.ResetGroupCodingString(this.GroupQualifier);
      this.ResetException();
      if (this.type.IsAssignableFrom(value.GetType()))
        this.value = value;
      else if (this.type == typeof (Choice))
      {
        string s = value.ToString();
        if (!string.IsNullOrEmpty(s))
          this.value = (object) this.choices.FirstOrDefault<Choice>((Func<Choice, bool>) (c => c.Name == s || c.OriginalName == s));
        if (this.value == null)
          this.value = (object) ChoiceCollection.InvalidChoice;
      }
      else
        this.value = !(this.type == typeof (Dump)) ? (!(this.type == typeof (float)) ? (!(this.type == typeof (double)) ? Convert.ChangeType(value, this.type, (IFormatProvider) CultureInfo.CurrentCulture) : (object) this.Quantize(Convert.ToDouble(value, (IFormatProvider) CultureInfo.CurrentCulture))) : (object) this.Quantize(Convert.ToSingle(value, (IFormatProvider) CultureInfo.CurrentCulture))) : (object) new Dump(value.ToString());
      if (this.Channel.Extension != null)
      {
        try
        {
          this.Channel.Extension.Invoke("SetRelatedParameterValues", new object[1]
          {
            (object) this
          });
        }
        catch (NullReferenceException ex)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to locate related parameter whilst setting " + this.Qualifier);
        }
      }
      this.RaiseParameterUpdateEvent((Exception) null);
    }
  }

  public IEnumerable<string> RelatedParameterQualifiers
  {
    get
    {
      if (this.Channel.Extension == null)
        return (IEnumerable<string>) null;
      return this.Channel.Extension.Invoke("GetRelatedParameters", new object[1]
      {
        (object) this
      }) as IEnumerable<string>;
    }
  }

  public object OriginalValue => this.originalValue;

  public object DefaultValue
  {
    get
    {
      if (this.defaultValue == null && this.mcdVarcodeFragment != null)
        this.defaultValue = this.mcdVarcodeFragment.DefaultValue;
      return this.defaultValue;
    }
  }

  public object LastPersistedValue => this.lastPersistedValue;

  public object Max
  {
    get
    {
      if (!this.max.HasValue && this.mcdVarcodeFragment != null)
        this.AcquireScalesMinMaxFromMcd();
      try
      {
        return this.max.ToBoxed(this.type);
      }
      catch (OverflowException ex)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"Invalid Max value in database. '{(object) this.max}' cannot be presented as {(object) this.type}");
        return (object) null;
      }
    }
  }

  public object Min
  {
    get
    {
      if (!this.min.HasValue && this.mcdVarcodeFragment != null)
        this.AcquireScalesMinMaxFromMcd();
      try
      {
        return this.min.ToBoxed(this.type);
      }
      catch (OverflowException ex)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"Invalid Min value in database. '{(object) this.min}' cannot be presented as {(object) this.type}");
        return (object) null;
      }
    }
  }

  public IEnumerable<ScaleEntry> Scales
  {
    get
    {
      lock (this.mcdVarcodeFragmentLock)
      {
        if (this.scales == null && this.mcdVarcodeFragment != null)
          this.AcquireScalesMinMaxFromMcd();
        return this.scales != null ? (IEnumerable<ScaleEntry>) this.scales.AsReadOnly() : (IEnumerable<ScaleEntry>) null;
      }
    }
  }

  internal ScaleEntry FactorOffsetScale
  {
    get
    {
      lock (this.mcdVarcodeFragmentLock)
      {
        if (this.factorOffsetScale == null && this.mcdVarcodeFragment != null)
          this.factorOffsetScale = this.mcdVarcodeFragment.FactorOffsetScale;
        return this.factorOffsetScale;
      }
    }
  }

  internal bool IsValueInFactorOffsetScaleRange(Decimal value)
  {
    Decimal num1 = this.min.HasValue ? this.factorOffsetScale.ToEcuValue(this.min.Value) : this.GetRepresentableEcuMinOrMax(true);
    Decimal num2 = this.max.HasValue ? this.factorOffsetScale.ToEcuValue(this.max.Value) : this.GetRepresentableEcuMinOrMax(false);
    Decimal ecuValue = this.factorOffsetScale.ToEcuValue(value);
    return ecuValue >= num1 && ecuValue <= num2;
  }

  private Decimal GetRepresentableEcuMinOrMax(bool isMin)
  {
    SapiLayer1.Coding? coding = this.Coding;
    if (coding.HasValue)
    {
      switch (coding.GetValueOrDefault())
      {
        case SapiLayer1.Coding.TwosComplement:
          long? byteLength1 = this.byteLength;
          if (byteLength1.HasValue)
          {
            switch (byteLength1.GetValueOrDefault() - 1L)
            {
              case 0:
                return (Decimal) (isMin ? sbyte.MinValue : sbyte.MaxValue);
              case 1:
                return (Decimal) (isMin ? short.MinValue : short.MaxValue);
              case 3:
                return (Decimal) (isMin ? int.MinValue : int.MaxValue);
            }
          }
          throw new InvalidOperationException($"Parameter {this.qualifier} has unhandled length {(object) this.byteLength}");
        case SapiLayer1.Coding.Unsigned:
          long? byteLength2 = this.byteLength;
          if (byteLength2.HasValue)
          {
            switch (byteLength2.GetValueOrDefault() - 1L)
            {
              case 0:
                return (Decimal) (isMin ? (byte) 0 : byte.MaxValue);
              case 1:
                return (Decimal) (isMin ? (ushort) 0 : ushort.MaxValue);
              case 3:
                return (Decimal) (isMin ? 0U : uint.MaxValue);
            }
          }
          throw new InvalidOperationException($"Parameter {this.qualifier} has unhandled length {(object) this.byteLength}");
      }
    }
    throw new InvalidOperationException($"Parameter {this.qualifier} has unhandled coding {(object) this.coding}");
  }

  public Type Type => this.type;

  public int Index => this.collectionIndex;

  internal uint CaesarIndex => this.caesarIndex;

  internal ParamType ParamType => this.pt;

  public bool HasBeenReadFromEcu => this.hasBeenReadFromEcu;

  public bool Marked
  {
    get => this.marked;
    set => this.marked = value;
  }

  public int ReadAccess => this.readAccessLevel;

  public int WriteAccess => this.writeAccessLevel;

  public Exception Exception
  {
    get => this.exception;
    internal set => this.exception = value;
  }

  public bool ServiceAsParameter => this.serviceAsParameter;

  public object Precision
  {
    get
    {
      if (this.precision == null && this.mcdVarcodeFragment != null && this.Factor.HasValue && this.Offset.HasValue)
        this.precision = (object) Math.Max(Sapi.CalculatePrecision(Convert.ToDouble(this.Factor.Value)), Sapi.CalculatePrecision(Convert.ToDouble(this.Offset.Value)));
      return this.precision;
    }
  }

  public bool Persistable => this.persistable;

  public long? BytePosition
  {
    get
    {
      if (!this.bytePos.HasValue && this.mcdVarcodeFragment != null)
        this.bytePos = new long?(this.mcdVarcodeFragment.BytePosition.Value - (long) this.WritePrefix.Length);
      return this.bytePos;
    }
  }

  public int? BitPosition
  {
    get
    {
      if (!this.bitPos.HasValue && this.mcdVarcodeFragment != null)
        this.bitPos = this.mcdVarcodeFragment.BitPosition;
      return this.bitPos;
    }
  }

  public long? ByteLength
  {
    get
    {
      if (!this.byteLength.HasValue && this.mcdVarcodeFragment != null)
        this.byteLength = this.mcdVarcodeFragment.ByteLength;
      return this.byteLength;
    }
  }

  public long? BitLength
  {
    get
    {
      if (!this.bitLength.HasValue && this.mcdVarcodeFragment != null)
        this.bitLength = this.mcdVarcodeFragment.BitLength;
      return this.bitLength;
    }
  }

  public SapiLayer1.Coding? Coding
  {
    get
    {
      if (!this.coding.HasValue && this.mcdVarcodeFragment != null)
        this.coding = this.mcdVarcodeFragment.Coding;
      return this.coding;
    }
  }

  public SapiLayer1.ByteOrder? ByteOrder
  {
    get
    {
      if (!this.byteOrder.HasValue && this.mcdVarcodeFragment != null)
        this.byteOrder = this.mcdVarcodeFragment.ByteOrder;
      return this.byteOrder;
    }
  }

  public SapiLayer1.TypeSpecifier? TypeSpecifier
  {
    get
    {
      if (!this.typeSpecifier.HasValue && this.mcdVarcodeFragment != null)
        this.typeSpecifier = this.mcdVarcodeFragment.TypeSpecifier;
      return this.typeSpecifier;
    }
  }

  public ConversionType? ConversionSelector
  {
    get
    {
      if (!this.conversionType.HasValue && this.mcdVarcodeFragment != null)
        this.conversionType = this.mcdVarcodeFragment.ConversionSelector;
      return this.conversionType;
    }
  }

  public SapiLayer1.DataType? DataType
  {
    get
    {
      if (!this.dataType.HasValue && this.mcdVarcodeFragment != null)
        this.dataType = this.mcdVarcodeFragment.DataType;
      return this.dataType;
    }
  }

  public Decimal? Factor
  {
    get
    {
      lock (this.mcdVarcodeFragmentLock)
      {
        if (!this.factor.HasValue && this.mcdVarcodeFragment != null)
          this.factor = this.mcdVarcodeFragment.Factor.HasValue ? new Decimal?(Convert.ToDecimal((object) this.mcdVarcodeFragment.Factor, (IFormatProvider) CultureInfo.InvariantCulture)) : new Decimal?();
        return this.factor;
      }
    }
  }

  public Decimal? Offset
  {
    get
    {
      lock (this.mcdVarcodeFragmentLock)
      {
        if (!this.offset.HasValue && this.mcdVarcodeFragment != null)
          this.offset = this.mcdVarcodeFragment.Offset.HasValue ? new Decimal?(Convert.ToDecimal((object) this.mcdVarcodeFragment.Offset, (IFormatProvider) CultureInfo.InvariantCulture)) : new Decimal?();
        return this.offset;
      }
    }
  }

  public Service ReadService => this.readService;

  internal byte[] WritePrefix
  {
    get
    {
      if (this.writePrefix == null)
      {
        if (this.writeService != (Service) null && this.readService != (Service) null)
          this.writePrefix = this.writeService.BaseRequestMessage.Data.Take<byte>(this.readService.BaseRequestMessage.Data.Count).ToArray<byte>();
        else if (this.writeService != (Service) null)
          this.writePrefix = this.writeService.BaseRequestMessage.Data.Take<byte>(this.writeService.RequestMessageMask.Data.Count).ToArray<byte>();
      }
      return this.writePrefix;
    }
  }

  public int? GroupLength
  {
    get
    {
      IList<byte> data = this.WriteService?.BaseRequestMessage?.Data;
      return data != null && this.WritePrefix != null ? new int?(data.Count - this.WritePrefix.Length) : new int?();
    }
  }

  public Service WriteService => this.writeService;

  internal void AddStringsForTranslation(Dictionary<string, string> table)
  {
    table[Sapi.MakeTranslationIdentifier(this.CombinedQualifier, "Name")] = this.name;
    table[Sapi.MakeTranslationIdentifier(this.groupQualifier, "GroupName")] = this.groupName;
    if (this.description != null)
      table[Sapi.MakeTranslationIdentifier(this.CombinedQualifier, "Description")] = this.description;
    if (this.Choices == null)
      return;
    foreach (Choice choice in (ReadOnlyCollection<Choice>) this.Choices)
      choice.AddStringsForTranslation(table);
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("ReadAccessLevel is deprecated due to non-CLS compliance, please use ReadAccess instead.")]
  public ushort ReadAccessLevel => (ushort) this.readAccessLevel;

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("WriteAccessLevel is deprecated due to non-CLS compliance, please use WriteAccess instead.")]
  public ushort WriteAccessLevel => (ushort) this.writeAccessLevel;

  public ParameterValueCollection ParameterValues => this.parameterValues;

  public bool Summary => this.channel.Ecu.SummaryQualifier(this.CombinedQualifier);

  public bool IsValueEqualInCodingString(Dump codingString, string value)
  {
    try
    {
      object obj = this.ParseValue(value);
      lock (this.Channel.OfflineVarcodingHandleLock)
      {
        Varcode offlineVarcodingHandle = this.channel.OfflineVarcodingHandle;
        if (offlineVarcodingHandle != null)
        {
          offlineVarcodingHandle.SetFragmentValue(this, obj);
          if (offlineVarcodingHandle.Exception == null)
            return this.CodingStringMask.AreCodingStringsEquivalent(codingString.Data.ToArray<byte>(), offlineVarcodingHandle.GetCurrentCodingString(this.groupQualifier));
        }
      }
    }
    catch (Exception ex) when (
    {
      // ISSUE: unable to correctly present filter
      int num;
      switch (ex)
      {
        case ArgumentOutOfRangeException _:
        case InvalidOperationException _:
        case InvalidCastException _:
        case FormatException _:
        case OverflowException _:
          num = 1;
          break;
        default:
          num = ex is ArgumentException ? 1 : 0;
          break;
      }
      if ((uint) num > 0U)
      {
        SuccessfulFiltering;
      }
      else
        throw;
    }
    )
    {
    }
    return false;
  }

  public bool IsValueEqualInCodingStrings(Dump codingString1, Dump codingString2)
  {
    return this.CodingStringMask.AreCodingStringsEquivalent(codingString1.Data.ToArray<byte>(), codingString2.Data.ToArray<byte>());
  }

  public Dump CodingStringMask
  {
    get
    {
      if (this.codingStringMask == null && this.GroupLength.HasValue)
        this.codingStringMask = Parameter.CreateCodingStringMask(this.GroupLength.Value, Enumerable.Repeat<Parameter>(this, 1), true);
      return this.codingStringMask;
    }
  }

  internal static Dump CreateCodingStringMask(
    int length,
    IEnumerable<Parameter> parameters,
    bool includeExclude)
  {
    return parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.BitLength.HasValue)).Select<Parameter, Tuple<int, int>>((Func<Parameter, Tuple<int, int>>) (p =>
    {
      long? nullable = p.BytePosition;
      int num1 = (int) (nullable.Value * 8L + (long) p.BitPosition.Value);
      nullable = p.BitLength;
      int num2 = (int) nullable.Value;
      return Tuple.Create<int, int>(num1, num2);
    })).CreateCodingStringMask(length, includeExclude);
  }

  internal static bool LoadFromLog(
    XElement element,
    string groupQualifier,
    LogFileFormatTagCollection format,
    Channel channel,
    List<string> missingQualifierList,
    object missingInfoLock)
  {
    string qualifier = $"{groupQualifier}.{element.Attribute(format[TagName.Qualifier]).Value}";
    Parameter parameter = channel.Parameters[qualifier];
    if (parameter != null)
    {
      bool flag = false;
      foreach (XElement element1 in element.Elements(format[TagName.Value]))
      {
        try
        {
          parameter.parameterValues.Add(ParameterValue.FromXElement(element1, format, parameter), false);
          flag = true;
        }
        catch (ArgumentOutOfRangeException ex)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ArgumentOutOfRangeException while loading {0} value '{1}' from log file", (object) parameter.CombinedQualifier, (object) element1.Value));
        }
        catch (FormatException ex)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FormatException while loading {0} value '{1}' from log file", (object) parameter.CombinedQualifier, (object) element1.Value));
        }
      }
      return flag;
    }
    if (!channel.Ecu.IgnoreQualifier(qualifier))
    {
      lock (missingInfoLock)
        missingQualifierList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) channel.Ecu.Name, (object) qualifier));
    }
    return false;
  }

  internal void WriteXmlTo(DateTime startTime, DateTime endTime, XmlWriter writer)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    writer.WriteStartElement(currentFormat[TagName.Parameter].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, this.Qualifier);
    ParameterValue parameterValue1 = (ParameterValue) null;
    foreach (ParameterValue parameterValue2 in this.parameterValues)
    {
      if (parameterValue2.Value != null)
      {
        if (parameterValue2.Time >= startTime)
        {
          if (parameterValue1 != null)
          {
            parameterValue1.WriteXmlTo(startTime, this, writer);
            parameterValue1 = (ParameterValue) null;
          }
          if (!(parameterValue2.Time > endTime))
            parameterValue2.WriteXmlTo(startTime, this, writer);
          else
            break;
        }
        else
          parameterValue1 = parameterValue2;
      }
    }
    parameterValue1?.WriteXmlTo(startTime, this, writer);
    writer.WriteEndElement();
  }

  public event ParameterUpdateEventHandler ParameterUpdateEvent;
}
