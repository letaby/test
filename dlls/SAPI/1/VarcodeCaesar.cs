// Decompiled with JetBrains decompiler
// Type: SapiLayer1.VarcodeCaesar
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace SapiLayer1;

internal class VarcodeCaesar : Varcode
{
  private Channel channel;
  private CaesarVarcode caesarVarcoding;
  private Dictionary<Parameter, CaesarFragment> writableFragments = new Dictionary<Parameter, CaesarFragment>();
  private byte? negativeResponseCode;

  internal VarcodeCaesar(CaesarEcu caesarEcu)
  {
    this.caesarVarcoding = caesarEcu.InitOfflineVarcoding();
  }

  internal VarcodeCaesar(Channel channel)
  {
    this.channel = channel;
    this.caesarVarcoding = channel.ChannelHandle.VCInit();
  }

  internal override void DoCoding()
  {
    this.negativeResponseCode = new byte?();
    Sapi sapi = Sapi.GetSapi();
    sapi.ByteMessageInternalEvent += new ByteMessageEventHandler(this.Sapi_ByteMessageInternalEvent);
    this.caesarVarcoding.DoCoding();
    sapi.ByteMessageInternalEvent -= new ByteMessageEventHandler(this.Sapi_ByteMessageInternalEvent);
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding), this.negativeResponseCode, this.channel.Ecu.DiagnosisProtocol) : (CaesarException) null;
  }

  private void Sapi_ByteMessageInternalEvent(object sender, ByteMessageEventArgs e)
  {
    if (sender as Channel != this.channel || e.Direction != ByteMessageDirection.RX || e.Data.Data.Count < 3 || e.Data.Data[0] != (byte) 127 /*0x7F*/ || e.Data.Data[1] != (byte) 46)
      return;
    this.negativeResponseCode = new byte?(e.Data.Data[2]);
  }

  internal override bool AllowSetDefaultString(string groupQualifier)
  {
    int num = this.caesarVarcoding.AllowSetDefaultString(groupQualifier) ? 1 : 0;
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
    return num != 0;
  }

  internal override void EnableReadCodingStringFromEcu(bool enableReadCodingStringFromEcu)
  {
    this.caesarVarcoding.EnableReadCodingStringFromEcu(enableReadCodingStringFromEcu);
  }

  internal override byte[] GetCurrentCodingString(string groupQualifier)
  {
    byte[] currentCodingString = this.caesarVarcoding.GetCurrentCodingString(groupQualifier);
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
    return currentCodingString;
  }

  internal override void SetCurrentCodingString(string groupQualifier, byte[] content)
  {
    this.caesarVarcoding.SetCurrentCodingString(groupQualifier, content);
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
  }

  internal override void SetDefaultStringByPartNumber(string partNumber)
  {
    this.caesarVarcoding.SetDefaultStringByPartNumber(partNumber);
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
  }

  internal override void SetDefaultStringByPartNumberAndPartVersion(
    string partNumber,
    uint partVersion)
  {
    this.caesarVarcoding.SetDefaultStringByPartNumberAndPartVersion(partNumber, partVersion);
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
  }

  internal override void SetFragmentMeaningByPartNumber(string partNumber)
  {
    this.caesarVarcoding.SetFragmentMeaningByPartNumber(partNumber);
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
  }

  internal override void SetFragmentMeaningByPartNumberAndPartVersion(
    string partNumber,
    uint partVersion)
  {
    this.caesarVarcoding.SetFragmentMeaningByPartNumberAndPartVersion(partNumber, partVersion);
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
  }

  internal void SetFragmentMeaningByIndex(Parameter parameter, int index)
  {
    if (parameter != null)
    {
      CaesarFragment fragmentByIndex;
      if (!this.writableFragments.TryGetValue(parameter, out fragmentByIndex))
      {
        fragmentByIndex = this.caesarVarcoding.GetFragmentByIndex(parameter.CaesarIndex);
        if (fragmentByIndex != null)
          this.writableFragments[parameter] = fragmentByIndex;
      }
      fragmentByIndex?.SetMeaningByIndex(this.caesarVarcoding, index);
      this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
    }
    else
      this.Exception = new CaesarException(SapiError.NoMatchingPartNumberAndPartVersionFound);
  }

  internal override void SetFragmentValue(Parameter parameter, object newValue)
  {
    if (parameter.Type == typeof (double))
    {
      object obj = newValue;
      int num1 = obj is double ? 1 : 0;
      double num2 = num1 != 0 ? (double) obj : 0.0;
      if (num1 != 0)
      {
        Decimal decimalValue = Convert.ToDecimal(num2);
        ConversionType? conversionSelector = parameter.ConversionSelector;
        ConversionType conversionType = ConversionType.FactorOffset;
        ScaleEntry scaleEntry = (conversionSelector.GetValueOrDefault() == conversionType ? (conversionSelector.HasValue ? 1 : 0) : 0) != 0 ? (parameter.IsValueInFactorOffsetScaleRange(decimalValue) ? parameter.FactorOffsetScale : (ScaleEntry) null) : parameter.Scales.FirstOrDefault<ScaleEntry>((Func<ScaleEntry, bool>) (sc => sc.IsValueInRange(decimalValue)));
        if (scaleEntry != null)
        {
          Decimal ecuValue = scaleEntry.ToEcuValue(decimalValue);
          long? nullable = parameter.ByteLength;
          long num3 = nullable.Value;
          int num4 = (int) parameter.Coding.Value;
          long byteLen = num3;
          string qualifier = parameter.Qualifier;
          byte[] numArray = VarcodeCaesar.Encode(ecuValue, (Coding) num4, byteLen, qualifier);
          byte[] currentCodingString = this.caesarVarcoding.GetCurrentCodingString(parameter.GroupQualifier);
          nullable = parameter.BytePosition;
          long num5 = nullable.Value;
          bool flag = parameter.ByteOrder.ByteOrderMatchesSystem();
          for (int index = 0; (long) index < num3; ++index)
            currentCodingString[(IntPtr) (num5 + (long) index)] = flag ? numArray[index] : numArray[num3 - 1L - (long) index];
          this.caesarVarcoding.SetCurrentCodingString(parameter.GroupQualifier, currentCodingString);
          goto label_18;
        }
        this.Exception = new CaesarException(SapiError.PreparationValueOutOfLimits);
        return;
      }
    }
    CaesarFragment fragmentByIndex;
    if (!this.writableFragments.TryGetValue(parameter, out fragmentByIndex))
    {
      fragmentByIndex = this.caesarVarcoding.GetFragmentByIndex(parameter.CaesarIndex);
      if (fragmentByIndex != null)
        this.writableFragments[parameter] = fragmentByIndex;
    }
    if (fragmentByIndex != null)
    {
      ParamType paramType = parameter.ParamType;
      if (paramType != 15)
      {
        if (paramType == 18)
        {
          if (newValue != null && newValue.GetType() == typeof (Choice) && newValue != (object) ChoiceCollection.InvalidChoice)
          {
            newValue = (object) ((Choice) newValue).Index;
            fragmentByIndex.SetValue(this.caesarVarcoding, (ParamType) 18, newValue);
          }
        }
        else
          fragmentByIndex.SetValue(this.caesarVarcoding, parameter.ParamType, newValue);
      }
      else
      {
        Dump dump = (Dump) newValue;
        fragmentByIndex.SetValue(this.caesarVarcoding, (ParamType) 15, (object) dump.Data.ToArray<byte>());
      }
    }
label_18:
    this.Exception = this.caesarVarcoding.IsErrorSet ? new CaesarException(new CaesarErrorException(this.caesarVarcoding)) : (CaesarException) null;
  }

  internal override object GetFragmentValue(Parameter parameter)
  {
    if (parameter.Type == typeof (double))
    {
      byte[] currentCodingString = this.caesarVarcoding.GetCurrentCodingString(parameter.GroupQualifier);
      if (!this.caesarVarcoding.IsErrorSet)
      {
        this.Exception = (CaesarException) null;
        long num = parameter.BytePosition.Value;
        long length = parameter.ByteLength.Value;
        bool flag = parameter.ByteOrder.ByteOrderMatchesSystem();
        byte[] codingValue = new byte[length];
        for (int index = 0; (long) index < length; ++index)
          codingValue[index] = flag ? currentCodingString[num + (long) index] : currentCodingString[num + (length - 1L - (long) index)];
        Decimal ecuValue = VarcodeCaesar.Decode(codingValue, parameter.Coding.Value, parameter.Qualifier);
        ConversionType? conversionSelector = parameter.ConversionSelector;
        ConversionType conversionType = ConversionType.FactorOffset;
        ScaleEntry scaleEntry = (conversionSelector.GetValueOrDefault() == conversionType ? (conversionSelector.HasValue ? 1 : 0) : 0) != 0 ? parameter.FactorOffsetScale : parameter.Scales.FirstOrDefault<ScaleEntry>((Func<ScaleEntry, bool>) (sc => sc.IsEcuValueInRange(ecuValue)));
        if (scaleEntry != null)
          return (object) (double) scaleEntry.ToPhysicalValue(ecuValue);
        this.Exception = new CaesarException(SapiError.NoMatchingIntervalWasFound);
      }
      else
        this.Exception = new CaesarException(new CaesarErrorException(this.caesarVarcoding));
    }
    else
    {
      object data = this.caesarVarcoding.GetFragmentByIndex(parameter.CaesarIndex)?.GetValue(this.caesarVarcoding, parameter.ParamType);
      if (!this.caesarVarcoding.IsErrorSet)
      {
        this.Exception = (CaesarException) null;
        ParamType paramType = parameter.ParamType;
        if (paramType == 15)
          return (object) new Dump((IEnumerable<byte>) (byte[]) data);
        if (paramType != 18)
          return data;
        uint uint32 = Convert.ToUInt32(data, (IFormatProvider) CultureInfo.InvariantCulture);
        return uint32 < (uint) parameter.Choices.Count ? (object) parameter.Choices[(int) uint32] : (object) ChoiceCollection.InvalidChoice;
      }
      this.Exception = new CaesarException(new CaesarErrorException(this.caesarVarcoding));
    }
    return (object) null;
  }

  private static byte[] Encode(Decimal ecuValue, Coding coding, long byteLen, string qualifier)
  {
    switch (coding)
    {
      case Coding.TwosComplement:
        switch (byteLen - 1L)
        {
          case 0:
            return BitConverter.GetBytes((short) Convert.ToSByte((object) ecuValue, (IFormatProvider) CultureInfo.InvariantCulture));
          case 1:
            return BitConverter.GetBytes(Convert.ToInt16((object) ecuValue, (IFormatProvider) CultureInfo.InvariantCulture));
          case 3:
            return BitConverter.GetBytes(Convert.ToInt32((object) ecuValue, (IFormatProvider) CultureInfo.InvariantCulture));
          default:
            throw new InvalidOperationException($"Parameter {qualifier} has unhandled length {(object) byteLen}");
        }
      case Coding.Unsigned:
        switch (byteLen - 1L)
        {
          case 0:
            return BitConverter.GetBytes((short) Convert.ToByte((object) ecuValue, (IFormatProvider) CultureInfo.InvariantCulture));
          case 1:
            return BitConverter.GetBytes(Convert.ToUInt16((object) ecuValue, (IFormatProvider) CultureInfo.InvariantCulture));
          case 3:
            return BitConverter.GetBytes(Convert.ToUInt32((object) ecuValue, (IFormatProvider) CultureInfo.InvariantCulture));
          default:
            throw new InvalidOperationException($"Parameter {qualifier} has unhandled length");
        }
      default:
        throw new InvalidOperationException($"Parameter {qualifier} has unhandled coding {(object) coding}");
    }
  }

  private static Decimal Decode(byte[] codingValue, Coding coding, string qualifier)
  {
    switch (coding)
    {
      case Coding.TwosComplement:
        switch (codingValue.Length)
        {
          case 1:
            return (Decimal) (((int) codingValue[0] & 128 /*0x80*/) != 0 ? (sbyte) ((int) (byte) ((uint) codingValue[0] & (uint) sbyte.MaxValue) - 128 /*0x80*/) : (sbyte) codingValue[0]);
          case 2:
            return (Decimal) BitConverter.ToInt16(codingValue, 0);
          case 4:
            return (Decimal) BitConverter.ToInt32(codingValue, 0);
          default:
            throw new InvalidOperationException($"Parameter {qualifier} has unhandled length {(object) codingValue.Length}");
        }
      case Coding.Unsigned:
        switch (codingValue.Length)
        {
          case 1:
            return (Decimal) codingValue[0];
          case 2:
            return (Decimal) BitConverter.ToUInt16(codingValue, 0);
          case 4:
            return (Decimal) BitConverter.ToUInt32(codingValue, 0);
          default:
            throw new InvalidOperationException($"Parameter {qualifier} has unhandled length{(object) codingValue.Length}");
        }
      default:
        throw new InvalidOperationException($"Parameter {qualifier} has unhandled coding {(object) coding}");
    }
  }

  protected override void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      if (this.writableFragments != null)
      {
        this.writableFragments.Clear();
        this.writableFragments = (Dictionary<Parameter, CaesarFragment>) null;
      }
      if (this.caesarVarcoding != null)
      {
        ((CaesarHandle\u003CCaesar\u003A\u003AVarcodeHandleStruct\u0020\u002A\u003E) this.caesarVarcoding).Dispose();
        this.caesarVarcoding = (CaesarVarcode) null;
      }
    }
    this.disposedValue = true;
  }
}
