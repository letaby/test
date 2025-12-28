// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Presentation
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
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
  private Decimal? max;
  private Decimal? min;
  private string units;
  private Type type;
  private Type dataInterfaceType;
  private ChoiceCollection choices;
  private object precision;
  private Decimal? factor;
  private Decimal? offset;
  private Ecu ecu;
  private int? bytePos;
  private int? bitPos;
  private int? byteLength;
  private int? bitLength;
  private SapiLayer1.Coding? coding;
  private SapiLayer1.TypeSpecifier? typeSpecifier;
  private SapiLayer1.ByteOrder? byteOrder;
  private SapiLayer1.DataType? dataType;
  private ConversionType? conversionType;

  internal Presentation(ushort i)
  {
    this.ecu = this.channel?.Ecu;
    this.name = string.Empty;
    this.description = string.Empty;
    this.units = string.Empty;
    this.index = i;
  }

  internal Presentation(Ecu ecu, string name, ChoiceCollection choices, Type type, string units)
  {
    this.name = name;
    this.ecu = ecu;
    this.choices = choices;
    this.type = this.dataInterfaceType = type;
    this.units = units;
  }

  internal void AcquireFromRollCall(
    Ecu ecu,
    string qualifier,
    IDictionary<string, string> content,
    bool isPropA)
  {
    this.name = "PRES_" + (qualifier.StartsWith("DT_", StringComparison.Ordinal) ? qualifier.Substring(3) : qualifier);
    double namedPropertyValue1 = content.GetNamedPropertyValue<double>("Factor", double.NaN);
    double namedPropertyValue2 = content.GetNamedPropertyValue<double>("Offset", double.NaN);
    this.precision = (object) Math.Max(Sapi.CalculatePrecision(namedPropertyValue1), Sapi.CalculatePrecision(namedPropertyValue2));
    this.factor = !double.IsNaN(namedPropertyValue1) ? new Decimal?(Convert.ToDecimal((object) namedPropertyValue1, (IFormatProvider) CultureInfo.InvariantCulture)) : new Decimal?();
    this.offset = !double.IsNaN(namedPropertyValue2) ? new Decimal?(Convert.ToDecimal((object) namedPropertyValue2, (IFormatProvider) CultureInfo.InvariantCulture)) : new Decimal?();
    this.isPropA = isPropA;
    this.SlotType = new int?(content.GetNamedPropertyValue<int>("SlotType", -1));
    this.bytePos = new int?(content.GetNamedPropertyValue<int>("BytePos", 1));
    this.bitPos = new int?(content.GetNamedPropertyValue<int>("BitPos", 1));
    this.bitLength = content.GetNamedPropertyValue<int?>("BitLength", new int?());
    this.byteOrder = new SapiLayer1.ByteOrder?(SapiLayer1.ByteOrder.LowHigh);
    int? nullable1;
    int? nullable2;
    if (this.bitLength.HasValue)
    {
      nullable1 = this.bitLength;
      this.byteLength = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() / 8) : new int?();
      nullable2 = this.bitLength;
      nullable1 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() % 8) : new int?();
      int num = 0;
      if ((nullable1.GetValueOrDefault() == num ? (!nullable1.HasValue ? 1 : 0) : 1) != 0)
      {
        nullable1 = this.byteLength;
        int? nullable3;
        if (!nullable1.HasValue)
        {
          nullable2 = new int?();
          nullable3 = nullable2;
        }
        else
          nullable3 = new int?(nullable1.GetValueOrDefault() + 1);
        this.byteLength = nullable3;
      }
    }
    nullable1 = this.SlotType;
    if (nullable1.HasValue)
    {
      switch (nullable1.GetValueOrDefault())
      {
        case 1:
          this.type = typeof (string);
          goto label_19;
        case 4:
          this.type = typeof (Dump);
          goto label_19;
      }
    }
    if (namedPropertyValue1 == 1.0 && namedPropertyValue2 == 0.0)
    {
      string namedPropertyValue3 = content.GetNamedPropertyValue<string>("Choices", string.Empty);
      if (!string.IsNullOrEmpty(namedPropertyValue3))
      {
        this.type = typeof (Choice);
        Ecu ecu1 = ecu;
        string name = this.name;
        nullable2 = new int?();
        int? limitedRangeMin = nullable2;
        nullable2 = new int?();
        int? limitedRangeMax = nullable2;
        this.choices = new ChoiceCollection(ecu1, name, limitedRangeMin: limitedRangeMin, limitedRangeMax: limitedRangeMax);
        this.choices.Add(namedPropertyValue3);
      }
      else
      {
        nullable2 = this.bitLength;
        int num1 = 8;
        Type type;
        if ((nullable2.GetValueOrDefault() <= num1 ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
        {
          nullable2 = this.bitLength;
          int num2 = 16 /*0x10*/;
          type = (nullable2.GetValueOrDefault() <= num2 ? (nullable2.HasValue ? 1 : 0) : 0) != 0 ? typeof (ushort) : typeof (uint);
        }
        else
          type = typeof (byte);
        this.type = type;
      }
    }
    else
      this.type = typeof (double);
label_19:
    this.ecu = ecu;
    this.units = content.GetNamedPropertyValue<string>("Units", string.Empty);
    if (content.ContainsKey("Min"))
      this.min = new Decimal?(Convert.ToDecimal((object) content.GetNamedPropertyValue<double>("Min", double.NaN), (IFormatProvider) CultureInfo.InvariantCulture));
    if (!content.ContainsKey("Max"))
      return;
    this.max = new Decimal?(Convert.ToDecimal((object) content.GetNamedPropertyValue<double>("Max", double.NaN), (IFormatProvider) CultureInfo.InvariantCulture));
  }

  internal virtual void Acquire(Channel channel, McdDBResponseParameter response, bool isDiagJob = false)
  {
    this.channel = channel;
    this.ecu = channel.Ecu;
    this.mcdResponseParameter = response;
    this.isDiagJob = isDiagJob;
    this.units = (string) null;
    if (response.IsArray || response.IsStructure || response.IsMultiplexer || response.IsNoType || response.IsEnvironmentalData)
    {
      this.name = !string.IsNullOrEmpty(response.Name) ? response.Name : response.Qualifier;
      this.type = this.dataInterfaceType = response.CodedParameterType;
    }
    else
    {
      this.name = isDiagJob ? response.Name : (response.PresentationName != null ? "PRES_" + McdCaesarEquivalence.MakeQualifier(response.PresentationName, true) : string.Empty);
      this.choices = new ChoiceCollection(channel.Ecu, this.name);
      this.isActualData = true;
      this.type = !(response.DataType == typeof (McdTextTableElement)) ? (!response.IsDiagnosticTroubleCode ? (this.dataInterfaceType = response.DataType != typeof (byte[]) ? response.DataType : typeof (Dump)) : typeof (uint)) : (this.dataInterfaceType = typeof (Choice));
    }
    this.McdParameterQualifierPath = response.QualifierPath;
  }

  private McdCaesarEquivalenceScaleInfo McdEquivalentScaleInfo
  {
    get
    {
      if (this.mcdEquivalentScaleInfo == null && this.mcdResponseParameter != null && this.mcdResponseParameter.PresentationQualifier != null)
        this.mcdEquivalentScaleInfo = this.channel.GetMcdCaesarEquivalenceScaleInfo(this.mcdResponseParameter.PresentationQualifier, this.type, (Func<McdDBDataObjectProp>) (() => this.mcdResponseParameter.DataObjectProp));
      return this.mcdEquivalentScaleInfo;
    }
  }

  internal void Acquire(Channel channel, Presentation presentation)
  {
    this.channel = channel;
    this.isActualData = presentation.isActualData;
    this.isDiagJob = presentation.isDiagJob;
    this.mcdResponseParameter = presentation.mcdResponseParameter;
    this.mcdEquivalentScaleInfo = presentation.mcdEquivalentScaleInfo;
    this.name = presentation.name;
    this.description = presentation.Description;
    this.ecu = channel.Ecu;
    this.bytePos = presentation.bytePos;
    this.bitPos = presentation.bitPos;
    this.byteLength = presentation.byteLength;
    this.bitLength = presentation.bitLength;
    this.units = presentation.units;
    this.type = presentation.type;
    this.dataInterfaceType = presentation.dataInterfaceType;
    this.choices = presentation.choices;
    this.factor = presentation.factor;
    this.offset = presentation.offset;
    this.min = presentation.min;
    this.max = presentation.max;
    this.conversionType = presentation.conversionType;
    this.coding = presentation.coding;
    this.byteOrder = presentation.byteOrder;
    this.typeSpecifier = presentation.typeSpecifier;
    this.dataType = presentation.dataType;
    this.precision = presentation.precision;
    this.McdParameterQualifierPath = presentation.McdParameterQualifierPath;
  }

  internal virtual void Acquire(Channel channel, CaesarDiagService diagService)
  {
    this.name = diagService.GetPresName((uint) this.index);
    this.description = diagService.GetPresDescription((uint) this.index);
    this.channel = channel;
    this.ecu = channel.Ecu;
    this.choices = new ChoiceCollection(channel.Ecu, this.name);
    ParamType presType = diagService.GetPresType((uint) this.index);
    this.type = this.dataInterfaceType = Sapi.GetRealCaesarType(presType);
    ushort? nullable = new ushort?();
    if (presType == 6)
      nullable = diagService.GetPresDecimals(this.index);
    try
    {
      using (CaesarPresentation presentation = diagService.GetPresentation((uint) this.index))
      {
        if (presentation != null)
        {
          this.bytePos = new int?((int) presentation.BytePosition);
          this.bitPos = new int?((int) presentation.BitPosition);
          if (presType == 15)
          {
            DumpLengths dumpLengths = presentation.GetDumpLengths();
            if (dumpLengths != null)
            {
              this.byteLength = new int?(dumpLengths.StandardLength != 0 ? dumpLengths.StandardLength : dumpLengths.MaxLength);
              int? byteLength = this.byteLength;
              this.bitLength = byteLength.HasValue ? new int?(byteLength.GetValueOrDefault() * 8) : new int?();
            }
          }
          else
          {
            this.byteLength = new int?((int) presentation.ByteLength);
            this.bitLength = new int?((int) presentation.BitLength);
          }
          this.coding = new SapiLayer1.Coding?((SapiLayer1.Coding) presentation.Coding);
          this.typeSpecifier = new SapiLayer1.TypeSpecifier?((SapiLayer1.TypeSpecifier) presentation.TypeSpecifier);
          this.byteOrder = new SapiLayer1.ByteOrder?((SapiLayer1.ByteOrder) presentation.ByteOrder);
          this.dataType = new SapiLayer1.DataType?((SapiLayer1.DataType) presentation.DataType);
          Limits limits = presentation.GetLimits(channel.EcuHandle);
          if (limits.Units != null)
            this.units = limits.Units;
          if (limits.Max.HasValue)
            this.max = new Decimal?(Convert.ToDecimal((object) (double) limits.Max.Value, (IFormatProvider) CultureInfo.InvariantCulture));
          if (limits.Min.HasValue)
            this.min = new Decimal?(Convert.ToDecimal((object) (double) limits.Min.Value, (IFormatProvider) CultureInfo.InvariantCulture));
          this.conversionType = new ConversionType?((ConversionType) presentation.ConversionSelector);
          if (presType == 17)
          {
            ConversionType? conversionType1 = this.conversionType;
            ConversionType conversionType2 = ConversionType.Enumeration;
            if ((conversionType1.GetValueOrDefault() == conversionType2 ? (conversionType1.HasValue ? 1 : 0) : 0) != 0)
            {
              uint enumerationEntries = presentation.NumberEnumerationEntries;
              for (uint index = 0; index < enumerationEntries; ++index)
              {
                DictionaryEntry enumerationEntry = presentation.GetEnumerationEntry(channel.EcuHandle, index);
                this.choices.Add(new Choice(enumerationEntry.Key.ToString(), enumerationEntry.Value));
              }
            }
            else
            {
              conversionType1 = this.conversionType;
              ConversionType conversionType3 = ConversionType.Scale;
              if ((conversionType1.GetValueOrDefault() == conversionType3 ? (conversionType1.HasValue ? 1 : 0) : 0) != 0)
              {
                uint numberOfScales = presentation.NumberOfScales;
                for (uint index = 0; index < numberOfScales; ++index)
                {
                  PresScaleEntry scaleEntry = presentation.GetScaleEntry(channel.EcuHandle, index);
                  this.choices.Add(new Choice(scaleEntry.Name, (int) scaleEntry.Min, (int) scaleEntry.Max));
                }
              }
            }
            if (this.choices.Count > 0)
              this.type = typeof (Choice);
          }
          else
          {
            ConversionType? conversionType4 = this.conversionType;
            ConversionType conversionType5 = ConversionType.Scale;
            if ((conversionType4.GetValueOrDefault() == conversionType5 ? (conversionType4.HasValue ? 1 : 0) : 0) == 0)
            {
              conversionType4 = this.conversionType;
              ConversionType conversionType6 = ConversionType.FactorOffset;
              if ((conversionType4.GetValueOrDefault() == conversionType6 ? (conversionType4.HasValue ? 1 : 0) : 0) == 0)
                goto label_47;
            }
            if (presentation.NumberOfScales > 0U)
            {
              List<PresScaleEntry> source1 = new List<PresScaleEntry>();
              for (uint index = 0; index < presentation.NumberOfScales; ++index)
              {
                PresScaleEntry scaleEntry = presentation.GetScaleEntry(channel.EcuHandle, index);
                if (scaleEntry.Name == null)
                  source1.Add(scaleEntry);
              }
              conversionType4 = this.conversionType;
              ConversionType conversionType7 = ConversionType.Scale;
              if ((conversionType4.GetValueOrDefault() == conversionType7 ? (conversionType4.HasValue ? 1 : 0) : 0) != 0 && !this.min.HasValue && !this.max.HasValue)
              {
                IEnumerable<float> source2 = source1.Select<PresScaleEntry, float>((Func<PresScaleEntry, float>) (se => se.ScaledMin)).Union<float>(source1.Select<PresScaleEntry, float>((Func<PresScaleEntry, float>) (se => se.ScaledMax)));
                this.min = new Decimal?(Convert.ToDecimal((object) (double) source2.Min(), (IFormatProvider) CultureInfo.InvariantCulture));
                this.max = new Decimal?(Convert.ToDecimal((object) (double) source2.Max(), (IFormatProvider) CultureInfo.InvariantCulture));
              }
              PresScaleEntry presScaleEntry = source1.Count == 1 ? source1[0] : (PresScaleEntry) null;
              if (presScaleEntry != null)
              {
                this.factor = new Decimal?(Convert.ToDecimal((object) (double) presScaleEntry.Factor, (IFormatProvider) CultureInfo.InvariantCulture));
                this.offset = new Decimal?(Convert.ToDecimal((object) (double) presScaleEntry.Offset, (IFormatProvider) CultureInfo.InvariantCulture));
                ushort val2 = (ushort) Math.Max(Sapi.CalculatePrecision((double) presScaleEntry.Factor), Sapi.CalculatePrecision((double) presScaleEntry.Offset));
                nullable = nullable.HasValue ? new ushort?(Math.Min(nullable.Value, val2)) : new ushort?(val2);
              }
              if (this.type == (Type) null)
              {
                if (channel.ChannelHandle != null)
                {
                  if (diagService.ServiceType != 128 /*0x80*/)
                    goto label_47;
                }
                this.type = typeof (float);
              }
            }
          }
        }
      }
    }
    catch (CaesarErrorException ex)
    {
      byte? negativeResponseCode = new byte?();
      CaesarException e = new CaesarException(ex, negativeResponseCode);
      Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) e);
    }
label_47:
    if (!nullable.HasValue)
      return;
    this.precision = (object) nullable;
  }

  internal object GetPresentation(byte[] data)
  {
    byte[] data1 = data;
    int? nullable = this.BytePosition;
    int bytePos = nullable.Value;
    nullable = this.BitPosition;
    int bitPos = nullable.Value;
    return this.GetPresentation(data1, bytePos, bitPos);
  }

  internal object GetPresentation(byte[] data, int bytePos, int bitPos)
  {
    if (this.channel == null || this.channel.IsRollCall)
    {
      --bytePos;
      --bitPos;
    }
    return Presentation.Decode(data, bytePos, bitPos, this.ByteLength, this.BitLength, this.ByteOrder, this.type, this.isPropA, this.SlotType, this.Factor, this.Offset, this.Min, this.Max, this.Choices, this.ecu, this.name);
  }

  internal static object Decode(
    byte[] data,
    int bytePos,
    int bitPos,
    int? byteLength,
    int? bitLength,
    SapiLayer1.ByteOrder? byteOrder,
    Type type,
    bool isPropA,
    int? slotType,
    Decimal? factor,
    Decimal? offset,
    Decimal? min,
    Decimal? max,
    ChoiceCollection choices,
    Ecu ecu,
    string name)
  {
    object rawValue = (object) null;
    if (isPropA && slotType.Value == 0)
    {
      string str = string.Join("", ((IEnumerable<byte>) data).Select<byte, string>((Func<byte, string>) (b => Convert.ToString(b, 2).PadLeft(8, '0'))));
      int num1 = bytePos * 8 + (8 - bitPos);
      int num2 = num1;
      int? nullable1 = bitLength;
      int? nullable2 = nullable1.HasValue ? new int?(num2 - nullable1.GetValueOrDefault()) : new int?();
      int num3 = 0;
      if ((nullable2.GetValueOrDefault() >= num3 ? (nullable2.HasValue ? 1 : 0) : 0) == 0 || num1 > str.Length)
        throw new CaesarException(SapiError.BytePosGreaterThanMessageLength);
      rawValue = (object) Convert.ToInt32(str.Substring(num1 - bitLength.Value, bitLength.Value), 2);
    }
    else if (bitLength.HasValue && bitLength.Value > 0 && bitLength.Value < 8)
    {
      if (bytePos >= data.Length)
        throw new CaesarException(SapiError.BytePosGreaterThanMessageLength);
      rawValue = (object) ((int) data[bytePos] >> bitPos & (1 << bitLength.Value) - 1);
    }
    else
    {
      if (byteLength.HasValue && bytePos + byteLength.Value > data.Length)
        throw new CaesarException(SapiError.BytePosGreaterThanMessageLength);
      bool flag = type == typeof (Dump) || type == typeof (string) || byteOrder.ByteOrderMatchesSystem();
      int length = byteLength.HasValue ? byteLength.Value : data.Length - bytePos;
      byte[] numArray = new byte[length];
      for (int index = 0; index < length; ++index)
        numArray[index] = flag ? data[bytePos + index] : data[bytePos + (length - 1 - index)];
      if (type == typeof (Dump))
        rawValue = (object) new Dump((IEnumerable<byte>) numArray);
      else if (type != typeof (string))
      {
        if (ecu != null && ecu.ProtocolName == "J1939" && !isPropA && numArray[numArray.Length - 1] >= (byte) 251)
          return (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, ecu.Translate(Sapi.MakeTranslationIdentifier(numArray[numArray.Length - 1].ToString((IFormatProvider) CultureInfo.InvariantCulture), "Range"), "Invalid value (${0})"), (object) new Dump(((IEnumerable<byte>) numArray).Reverse<byte>()));
        switch (numArray.Length)
        {
          case 1:
            rawValue = (object) numArray[0];
            break;
          case 2:
            ushort uint16 = BitConverter.ToUInt16(numArray, 0);
            if (bitLength.Value % 8 != 0)
              uint16 &= (ushort) ((1 << bitLength.Value) - 1);
            rawValue = (object) uint16;
            break;
          case 4:
            uint uint32 = BitConverter.ToUInt32(numArray, 0);
            if (bitLength.Value % 8 != 0)
              uint32 &= (uint) ((1 << bitLength.Value) - 1);
            rawValue = (object) uint32;
            break;
          default:
            Sapi.GetSapi().RaiseDebugInfoEvent((object) name, $"Don't know how to process a value {(object) numArray.Length} bytes long");
            break;
        }
      }
      else
        rawValue = (object) Encoding.ASCII.GetString(numArray);
    }
    if (rawValue != null && type != typeof (string) && type != typeof (Dump))
    {
      if (factor.HasValue && offset.HasValue)
      {
        Decimal? nullable3 = factor;
        Decimal num4 = 0M;
        if ((nullable3.GetValueOrDefault() == num4 ? (!nullable3.HasValue ? 1 : 0) : 1) != 0)
        {
          Decimal? nullable4 = factor;
          Decimal num5 = (Decimal) 1;
          if ((nullable4.GetValueOrDefault() == num5 ? (!nullable4.HasValue ? 1 : 0) : 1) == 0)
          {
            Decimal? nullable5 = offset;
            Decimal num6 = 0M;
            if ((nullable5.GetValueOrDefault() == num6 ? (!nullable5.HasValue ? 1 : 0) : 1) == 0)
              goto label_37;
          }
          Decimal num7 = Convert.ToDecimal(rawValue, (IFormatProvider) CultureInfo.InvariantCulture);
          Decimal? nullable6 = factor;
          Decimal? nullable7 = nullable6.HasValue ? new Decimal?(num7 * nullable6.GetValueOrDefault()) : new Decimal?();
          Decimal? nullable8 = offset;
          Decimal? nullable9;
          if (!(nullable7.HasValue & nullable8.HasValue))
          {
            nullable6 = new Decimal?();
            nullable9 = nullable6;
          }
          else
            nullable9 = new Decimal?(nullable7.GetValueOrDefault() + nullable8.GetValueOrDefault());
          nullable6 = nullable9;
          rawValue = (object) (double) nullable6.Value;
          goto label_39;
        }
      }
label_37:
      if (type == typeof (Choice))
      {
        Choice itemFromRawValue = choices.GetItemFromRawValue(rawValue);
        rawValue = itemFromRawValue != (object) null ? (object) itemFromRawValue : (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid value (${0:X})", rawValue);
      }
label_39:
      if (min.HasValue && Convert.ToDouble(rawValue, (IFormatProvider) CultureInfo.InvariantCulture) < (double) min.Value)
        rawValue = (object) "*";
      else if (max.HasValue && Convert.ToDouble(rawValue, (IFormatProvider) CultureInfo.InvariantCulture) > (double) max.Value)
        rawValue = (object) "*";
    }
    return rawValue;
  }

  internal string McdParameterQualifierPath { get; private set; }

  internal object GetPresentation(McdDiagComPrimitive diagServiceIO)
  {
    return !diagServiceIO.IsNegativeResponse ? this.GetPresentation(diagServiceIO.AllPositiveResponseParameters.Where<McdResponseParameter>((Func<McdResponseParameter, bool>) (pr => pr.QualifierPath == this.McdParameterQualifierPath)).ToList<McdResponseParameter>()) : diagServiceIO.NegativeResponseParameter.Value.Value;
  }

  internal object GetPresentation(List<McdResponseParameter> responseParameter)
  {
    switch (responseParameter.Count)
    {
      case 0:
        return (object) null;
      case 1:
        return this.GetPresentation(responseParameter[0]);
      default:
        return (object) responseParameter.Select<McdResponseParameter, object>((Func<McdResponseParameter, object>) (p => this.GetPresentation(p))).ToArray<object>();
    }
  }

  internal object GetPresentation(McdResponseParameter responseParameter)
  {
    McdValue mcdValue = responseParameter?.Value;
    if (this.type == typeof (Choice) && !responseParameter.IsValueValid)
      return mcdValue.Value;
    return mcdValue == null ? (object) null : mcdValue.GetValue(this.type, this.Choices);
  }

  internal object GetPresentation(CaesarDiagServiceIO diagServiceIO)
  {
    ParamType presType = diagServiceIO.GetPresType(this.index);
    if (this.type == (Type) null && presType != 17 && presType != 7)
      this.type = Sapi.GetRealCaesarType(presType);
    try
    {
      if (this.type == typeof (Choice) && !diagServiceIO.IsNegativeResponse)
      {
        uint presParamInternal = diagServiceIO.GetPresParamInternal(this.index);
        if (diagServiceIO.IsNoMatchingInterval(this.index))
          return (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid value (${0:X})", (object) presParamInternal);
        return this.Choices.Type == typeof (int) ? (object) this.Choices.GetItemFromRawValue((object) BitConverter.ToInt32(BitConverter.GetBytes(presParamInternal), 0)) : (object) this.Choices.GetItemFromRawValue((object) presParamInternal);
      }
      object data = diagServiceIO.GetPresParam(this.index, presType);
      ConversionType? conversionType1 = this.conversionType;
      ConversionType conversionType2 = ConversionType.Ieee;
      if ((conversionType1.GetValueOrDefault() == conversionType2 ? (conversionType1.HasValue ? 1 : 0) : 0) != 0 && data != null && presType == 6 && this.ecu.SignalNotAvailableValue != null)
      {
        if (((IEnumerable<byte>) BitConverter.GetBytes((float) data)).SequenceEqual<byte>((IEnumerable<byte>) this.ecu.SignalNotAvailableValue.Data))
          data = (object) "sna";
      }
      else if (presType == 15)
        data = (object) new Dump((IEnumerable<byte>) (byte[]) data);
      return data;
    }
    catch (InvalidCastException ex)
    {
      Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) new CaesarException(SapiError.UnknownPresentationType));
      return (object) string.Empty;
    }
  }

  internal void SetType(Type type) => this.type = type;

  internal void SetPrecision(ushort precision)
  {
    if (this.precision != null)
      return;
    this.precision = (object) precision;
  }

  internal Type DataInterfaceType => this.dataInterfaceType;

  internal object ParseFromLog(string sourceValue)
  {
    return Presentation.ParseFromLog(sourceValue, this.Type, this.Choices, this.ecu);
  }

  internal static object ParseFromLog(
    string sourceValue,
    Type type,
    ChoiceCollection choices,
    Ecu ecu)
  {
    if (type != (Type) null && (type.IsArray || type != typeof (string) && sourceValue.IndexOf(',') != -1))
    {
      string[] strArray = sourceValue.Split(",".ToCharArray(), StringSplitOptions.None);
      Type type1 = type.IsArray ? type.GetElementType() : type;
      Array instance = Array.CreateInstance(typeof (object), strArray.Length);
      for (int index = 0; index < strArray.Length; ++index)
      {
        object fromLog = Presentation.ParseFromLog(strArray[index], type1, choices, ecu);
        if (index == 0 && strArray.Length == 1 && fromLog != null && fromLog.GetType() != type1)
          return fromLog;
        instance.SetValue(fromLog, index);
      }
      return (object) instance;
    }
    object fromLog1 = (object) sourceValue;
    if (choices != null && (type == typeof (Choice) || choices.Count > 0))
      fromLog1 = (!ecu.IsRollCall || !(type == typeof (byte)) && !(type == typeof (uint)) && !(type == typeof (ushort)) ? (object) choices.GetItemFromLogValue(sourceValue) : (object) choices.GetItemFromRawValue((object) sourceValue)) ?? (object) sourceValue;
    else if (type != (Type) null)
    {
      try
      {
        fromLog1 = !(type == typeof (Dump)) ? Convert.ChangeType((object) sourceValue, type, (IFormatProvider) Presentation.EnglishUSCulture) : (object) new Dump(sourceValue);
      }
      catch (FormatException ex)
      {
      }
      catch (InvalidCastException ex)
      {
      }
      catch (ArgumentException ex)
      {
      }
    }
    return fromLog1;
  }

  internal static string FormatForLog(object sourceValue)
  {
    string str = string.Empty;
    if (sourceValue != null)
    {
      if (sourceValue.GetType().IsArray)
        return string.Join(",", (sourceValue as Array).Cast<object>().Select<object, string>((Func<object, string>) (source => Presentation.FormatForLog(source))).ToArray<string>());
      str = !(sourceValue.GetType() != typeof (Choice)) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0}", ((Choice) sourceValue).RawValue) : Convert.ToString(sourceValue, (IFormatProvider) Presentation.EnglishUSCulture);
    }
    return str;
  }

  public int Index => (int) this.index;

  public string Name => this.name;

  public string Description
  {
    get
    {
      if (this.description == null && this.mcdResponseParameter != null)
        this.description = this.mcdResponseParameter.Description;
      Ecu ecu = this.ecu;
      string str;
      if (ecu == null)
        str = (string) null;
      else
        str = ecu.Translate(Sapi.MakeTranslationIdentifier(this.name, nameof (Description)), this.description);
      return str ?? string.Empty;
    }
  }

  public string Units
  {
    get
    {
      if (this.units == null && this.mcdResponseParameter != null)
        this.units = this.mcdResponseParameter.Unit ?? string.Empty;
      return this.units;
    }
  }

  public Decimal? Max
  {
    get
    {
      if (!this.max.HasValue && this.mcdResponseParameter != null && this.isActualData && this.McdEquivalentScaleInfo != null)
        this.max = this.McdEquivalentScaleInfo.Max;
      return this.max;
    }
  }

  public Decimal? Min
  {
    get
    {
      if (!this.min.HasValue && this.mcdResponseParameter != null && this.isActualData && this.McdEquivalentScaleInfo != null)
        this.min = this.McdEquivalentScaleInfo.Min;
      return this.min;
    }
  }

  public Type Type => this.type;

  public ChoiceCollection Choices
  {
    get
    {
      if (this.isActualData && this.type == typeof (Choice) && this.mcdResponseParameter != null && this.choices.Count == 0)
        this.choices.Add(this.mcdResponseParameter.TextTableElements);
      return this.choices;
    }
  }

  public object Precision
  {
    get
    {
      if (this.precision == null && this.mcdResponseParameter != null && this.mcdResponseParameter.DecimalPlaces.HasValue)
        this.precision = (object) this.mcdResponseParameter.DecimalPlaces.Value;
      return this.precision;
    }
  }

  public Decimal? Factor
  {
    get
    {
      if (!this.factor.HasValue && this.mcdResponseParameter != null && this.isActualData && this.McdEquivalentScaleInfo != null)
        this.factor = new Decimal?(Convert.ToDecimal((object) this.McdEquivalentScaleInfo.Factor, (IFormatProvider) CultureInfo.InvariantCulture));
      return this.factor;
    }
  }

  public Decimal? Offset
  {
    get
    {
      if (!this.offset.HasValue && this.mcdResponseParameter != null && this.isActualData && this.McdEquivalentScaleInfo != null)
        this.offset = new Decimal?(Convert.ToDecimal((object) this.McdEquivalentScaleInfo.Offset, (IFormatProvider) CultureInfo.InvariantCulture));
      return this.offset;
    }
  }

  public int? BytePosition
  {
    get
    {
      if (!this.bytePos.HasValue && this.mcdResponseParameter != null && !this.isDiagJob && !this.mcdResponseParameter.IsArray && !this.mcdResponseParameter.IsNoType && !this.mcdResponseParameter.IsEnvironmentalData)
        this.bytePos = new int?((int) this.mcdResponseParameter.BytePos);
      return this.bytePos;
    }
  }

  public int? BitPosition
  {
    get
    {
      if (!this.bitPos.HasValue && this.mcdResponseParameter != null && !this.isDiagJob && !this.mcdResponseParameter.IsArray && !this.mcdResponseParameter.IsStructure && !this.mcdResponseParameter.IsMultiplexer && !this.mcdResponseParameter.IsNoType && !this.mcdResponseParameter.IsEnvironmentalData)
        this.bitPos = new int?((int) this.mcdResponseParameter.BitPos);
      return this.bitPos;
    }
  }

  public int? ByteLength
  {
    get
    {
      if (!this.byteLength.HasValue && this.mcdResponseParameter != null && !this.mcdResponseParameter.IsArray && !this.mcdResponseParameter.IsMultiplexer && !this.mcdResponseParameter.IsNoType && !this.mcdResponseParameter.IsEnvironmentalData)
        this.byteLength = new int?((int) this.mcdResponseParameter.ByteLength);
      return this.byteLength;
    }
  }

  public int? BitLength
  {
    get
    {
      if (!this.bitLength.HasValue && this.mcdResponseParameter != null && !this.mcdResponseParameter.IsArray && !this.mcdResponseParameter.IsMultiplexer && !this.mcdResponseParameter.IsNoType && !this.mcdResponseParameter.IsEnvironmentalData)
        this.bitLength = new int?((int) this.mcdResponseParameter.BitLength);
      return this.bitLength;
    }
  }

  public SapiLayer1.Coding? Coding
  {
    get
    {
      if (!this.coding.HasValue && this.mcdResponseParameter != null && this.isActualData)
        this.coding = this.McdEquivalentScaleInfo != null ? this.McdEquivalentScaleInfo.Coding : new SapiLayer1.Coding?(SapiLayer1.Coding.Unsigned);
      return this.coding;
    }
  }

  public SapiLayer1.ByteOrder? ByteOrder
  {
    get
    {
      if (!this.byteOrder.HasValue && this.mcdResponseParameter != null && this.isActualData)
        this.byteOrder = this.McdEquivalentScaleInfo != null ? this.McdEquivalentScaleInfo.ByteOrder : new SapiLayer1.ByteOrder?(SapiLayer1.ByteOrder.HighLow);
      return this.byteOrder;
    }
  }

  public ConversionType? ConversionSelector
  {
    get
    {
      if (!this.conversionType.HasValue && this.mcdResponseParameter != null && this.isActualData)
        this.conversionType = this.McdEquivalentScaleInfo != null ? this.McdEquivalentScaleInfo.ConversionType : new ConversionType?(ConversionType.Raw);
      return this.conversionType;
    }
  }

  public SapiLayer1.TypeSpecifier? TypeSpecifier
  {
    get
    {
      if (!this.typeSpecifier.HasValue && this.mcdResponseParameter != null && this.isActualData)
        this.typeSpecifier = new SapiLayer1.TypeSpecifier?(SapiLayer1.TypeSpecifier.Standard);
      return this.typeSpecifier;
    }
  }

  public SapiLayer1.DataType? DataType
  {
    get
    {
      if (!this.dataType.HasValue && this.mcdResponseParameter != null && this.isActualData)
        this.dataType = new SapiLayer1.DataType?(this.BitLength.Value % 8 == 0 ? SapiLayer1.DataType.Byte : SapiLayer1.DataType.Bit);
      return this.dataType;
    }
  }

  public int? SlotType { private set; get; }

  public bool IsStructure
  {
    get => this.mcdResponseParameter != null && this.mcdResponseParameter.IsStructure;
  }

  public bool IsArrayDefinition
  {
    get => this.mcdResponseParameter != null && this.mcdResponseParameter.IsArray;
  }

  public bool IsMultiplexer
  {
    get => this.mcdResponseParameter != null && this.mcdResponseParameter.IsMultiplexer;
  }

  public bool IsEnvironmentData
  {
    get => this.mcdResponseParameter != null && this.mcdResponseParameter.IsEnvironmentalData;
  }

  public bool IsDiagnosticTroubleCode
  {
    get => this.mcdResponseParameter != null && this.mcdResponseParameter.IsDiagnosticTroubleCode;
  }

  public bool IsNoType => this.mcdResponseParameter != null && this.mcdResponseParameter.IsNoType;

  public virtual object ManipulatedValue
  {
    get => (object) null;
    set
    {
    }
  }

  internal void AddStringsForTranslation(Dictionary<string, string> table)
  {
    if (!string.IsNullOrEmpty(this.description))
      table[Sapi.MakeTranslationIdentifier(this.name, "Description")] = this.description;
    if (this.Choices == null)
      return;
    foreach (Choice choice in (ReadOnlyCollection<Choice>) this.Choices)
      choice.AddStringsForTranslation(table);
  }
}
