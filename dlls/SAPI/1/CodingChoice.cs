// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingChoice
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CodingChoice
{
  private string rawValueTranslationQualifier;
  private Dictionary<SapiLayer1.Parameter, object> relatedParameterValues;
  private int index;
  private string rawValue;
  private string meaning;
  private string description;
  private Part part;
  private ushort accessLevel;
  private CodingParameter parameter;
  private CodingParameterGroup parameterGroup;
  private DiagnosisSource diagnosisSource;
  private StringDictionary cPFData;

  internal CodingChoice(CodingParameter parent)
  {
    this.parameter = parent;
    this.parameterGroup = parent.ParameterGroup;
  }

  internal CodingChoice(CodingParameterGroup parent) => this.parameterGroup = parent;

  internal CodingChoice()
  {
  }

  internal void Acquire(CaesarDICcfFragValue fragmentValue, int index)
  {
    this.meaning = fragmentValue.Meaning;
    this.description = fragmentValue.Description;
    this.rawValue = fragmentValue.Value;
    string partNumber = fragmentValue.PartNumber;
    if (partNumber != null)
    {
      object partVersion = fragmentValue.PartVersion;
      this.part = partVersion == null ? new Part(partNumber) : new Part(partNumber, partVersion);
    }
    this.accessLevel = fragmentValue.AccessLevel;
    this.diagnosisSource = DiagnosisSource.CaesarDatabase;
    this.index = index;
  }

  internal void Acquire(McdDBItemValue fragmentValue, int index)
  {
    this.meaning = fragmentValue.Meaning;
    this.description = fragmentValue.Description;
    if (fragmentValue.Value != null)
      this.rawValue = fragmentValue.Value.Value.ToString();
    this.part = new Part(fragmentValue.PartNumber);
    this.diagnosisSource = DiagnosisSource.McdDatabase;
    this.index = index;
  }

  internal void Acquire(CaesarDICcfDefaultString defaultStringValue)
  {
    this.meaning = defaultStringValue.Meaning;
    this.description = defaultStringValue.Description;
    string partNumber = defaultStringValue.PartNumber;
    if (partNumber != null)
    {
      object partVersion = defaultStringValue.PartVersion;
      this.part = partVersion == null ? new Part(partNumber) : new Part(partNumber, partVersion);
    }
    this.accessLevel = defaultStringValue.AccessLevel;
    this.rawValue = new Dump((IEnumerable<byte>) defaultStringValue.Value).ToString();
    this.diagnosisSource = DiagnosisSource.CaesarDatabase;
  }

  internal void Acquire(McdDBDataRecord defaultStringValue)
  {
    this.meaning = defaultStringValue.Name;
    this.description = defaultStringValue.Description;
    this.part = new Part(defaultStringValue.PartNumber);
    this.rawValue = new Dump(defaultStringValue.BinaryData).ToString();
    this.diagnosisSource = DiagnosisSource.McdDatabase;
  }

  internal void Acquire(StreamReader reader, string meaning, Part partNumber)
  {
    this.part = partNumber;
    this.meaning = meaning;
    this.accessLevel = (ushort) 1;
    reader.BaseStream.Seek(0L, SeekOrigin.Begin);
    reader.DiscardBufferedData();
    this.rawValue = reader.ReadToEnd();
    this.cPFData = new StringDictionary();
    ParameterCollection.LoadDictionaryFromStream(reader, ParameterFileFormat.ParFile, this.cPFData);
  }

  internal CodingChoice Clone(CodingParameter newParameter, CodingParameterGroup newParameterGroup)
  {
    return new CodingChoice()
    {
      rawValue = this.rawValue,
      meaning = this.meaning,
      description = this.description,
      part = this.part,
      accessLevel = this.accessLevel,
      cPFData = this.cPFData,
      index = this.index,
      parameter = newParameter,
      parameterGroup = newParameterGroup
    };
  }

  public string RawValue
  {
    get
    {
      if (this.parameter == null)
        return this.rawValue;
      if (this.rawValueTranslationQualifier == null)
        this.rawValueTranslationQualifier = Sapi.MakeTranslationIdentifier(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) this.parameter.ParameterGroup.Name, (object) this.parameter.Name), this.rawValue.CreateQualifierFromName(), "Name");
      return this.parameter.ParameterGroup.DiagnosisVariants.Count > 0 ? this.parameter.ParameterGroup.DiagnosisVariants[0].Ecu.Translate(this.rawValueTranslationQualifier, this.rawValue) : this.rawValue;
    }
  }

  public string OriginalRawValue => this.rawValue;

  private string CombinedQualifier
  {
    get
    {
      List<string> stringList = new List<string>();
      stringList.Add(this.parameterGroup.Qualifier);
      if (this.parameter != null)
        stringList.Add(this.parameter.Name.CreateQualifierFromName());
      stringList.Add(this.meaning.CreateQualifierFromName());
      return Sapi.MakeTranslationIdentifier(stringList.ToArray());
    }
  }

  internal void AddStringsForTranslation(Dictionary<string, string> table)
  {
    table[Sapi.MakeTranslationIdentifier(this.CombinedQualifier, "Meaning")] = this.meaning;
    if (this.description == null)
      return;
    table[Sapi.MakeTranslationIdentifier(this.CombinedQualifier, "Description")] = this.description;
  }

  public int Index => this.index;

  public string Meaning
  {
    get
    {
      if (this.ParameterGroup.DiagnosisVariants.Count <= 0)
        return this.meaning;
      return this.parameterGroup.DiagnosisVariants[0].Ecu.Translate(Sapi.MakeTranslationIdentifier(this.CombinedQualifier, nameof (Meaning)), this.meaning);
    }
  }

  public string Description
  {
    get
    {
      if (this.parameterGroup.DiagnosisVariants.Count <= 0)
        return this.description;
      return this.parameterGroup.DiagnosisVariants[0].Ecu.Translate(Sapi.MakeTranslationIdentifier(this.CombinedQualifier, nameof (Description)), this.description);
    }
  }

  public Part Part => this.part;

  public int AccessLevel => (int) this.accessLevel;

  public CodingParameter Parameter => this.parameter;

  public CodingParameterGroup ParameterGroup => this.parameterGroup;

  public void SetAsValue()
  {
    CaesarException caesarException = (CaesarException) null;
    lock (this.parameterGroup.Channel.OfflineVarcodingHandleLock)
    {
      Varcode offlineVarcodingHandle = this.parameterGroup.Channel.OfflineVarcodingHandle;
      if (offlineVarcodingHandle != null)
      {
        int count = this.parameterGroup.Channel.Parameters.Count;
        if (this.cPFData != null)
        {
          foreach (DictionaryEntry dictionaryEntry in this.cPFData)
          {
            SapiLayer1.Parameter parameter = this.parameterGroup.Channel.Parameters[dictionaryEntry.Key.ToString()];
            if (parameter != null)
            {
              parameter.InternalSetValue(dictionaryEntry.Value.ToString(), false);
            }
            else
            {
              Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Missing parameter during SetAsValue: {0}", dictionaryEntry.Key));
              caesarException = new CaesarException(SapiError.ParameterSpecifiedDidNotExist);
            }
          }
        }
        else
        {
          this.SetAsValue(offlineVarcodingHandle);
          if (offlineVarcodingHandle.IsErrorSet)
          {
            caesarException = offlineVarcodingHandle.Exception;
          }
          else
          {
            for (int index = 0; index < count; ++index)
            {
              SapiLayer1.Parameter parameter = this.parameterGroup.Channel.Parameters[index];
              if (string.Equals(this.parameterGroup.Qualifier, parameter.GroupQualifier, StringComparison.Ordinal))
              {
                if (this.parameter == null)
                  parameter.InternalRead(offlineVarcodingHandle, false);
                else if (string.Equals(this.parameter.Name, parameter.OriginalName, StringComparison.Ordinal))
                {
                  parameter.InternalRead(offlineVarcodingHandle, false);
                  break;
                }
              }
            }
            if (this.parameter == null)
              this.parameterGroup.Channel.Parameters.UpdateCodingString(this.parameterGroup.Qualifier, offlineVarcodingHandle, CodingStringSource.FromDefaultString);
            else
              this.parameterGroup.Channel.Parameters.ResetGroupCodingString(this.parameterGroup.Qualifier);
          }
        }
      }
    }
    if (caesarException != null)
      throw caesarException;
  }

  internal void SetAsValue(Varcode varcode)
  {
    if (this.parameter == null)
    {
      if (this.part.Version != null)
        varcode.SetDefaultStringByPartNumberAndPartVersion(this.part.Number, (uint) this.part.Version);
      else
        varcode.SetDefaultStringByPartNumber(this.part.Number);
    }
    else if (varcode is VarcodeCaesar varcodeCaesar)
      varcodeCaesar.SetFragmentMeaningByIndex(this.parameter.RelatedParameter, this.index);
    else if (this.part.Version != null)
      varcode.SetFragmentMeaningByPartNumberAndPartVersion(this.part.Number, (uint) this.part.Version);
    else
      varcode.SetFragmentMeaningByPartNumber(this.part.Number);
  }

  public void Mark(bool choice)
  {
    if (this.cPFData != null)
    {
      foreach (DictionaryEntry dictionaryEntry in this.cPFData)
      {
        SapiLayer1.Parameter parameter = this.parameterGroup.Channel.Parameters[dictionaryEntry.Key.ToString()];
        if (parameter != null)
          parameter.Marked = choice;
      }
    }
    else
    {
      for (int index = 0; index < this.parameterGroup.Channel.Parameters.Count; ++index)
      {
        SapiLayer1.Parameter parameter = this.parameterGroup.Channel.Parameters[index];
        if (string.Equals(this.parameterGroup.Qualifier, parameter.GroupQualifier, StringComparison.Ordinal))
        {
          if (this.parameter == null)
            parameter.Marked = choice;
          else if (string.Equals(this.parameter.Name, parameter.OriginalName, StringComparison.Ordinal))
          {
            parameter.Marked = choice;
            break;
          }
        }
      }
    }
  }

  public bool IsValidForChannel
  {
    get
    {
      if (this.parameterGroup.Channel == null)
        return false;
      if (this.parameter != null)
      {
        if (this.diagnosisSource == DiagnosisSource.CaesarDatabase)
        {
          if (this.parameter.RelatedParameter == null || this.parameter.RelatedParameter.Type == typeof (Choice) && !this.parameter.RelatedParameter.Choices.Any<Choice>((Func<Choice, bool>) (c => c.OriginalName == this.OriginalRawValue)))
            return false;
        }
        else if (this.diagnosisSource == DiagnosisSource.McdDatabase)
        {
          if (this.parameterGroup.ChannelByteLength.HasValue)
          {
            int? bytePos = this.parameter.BytePos;
            int? channelByteLength = this.parameterGroup.ChannelByteLength;
            if ((bytePos.GetValueOrDefault() >= channelByteLength.GetValueOrDefault() ? (bytePos.HasValue & channelByteLength.HasValue ? 1 : 0) : 0) == 0)
              goto label_13;
          }
          return false;
        }
      }
      else
      {
        if (this.parameterGroup.ChannelByteLength.HasValue)
        {
          int? nullable = this.ContentLength;
          int num1 = nullable.Value;
          nullable = this.parameterGroup.ChannelByteLength;
          int num2 = nullable.Value;
          if (num1 == num2)
            goto label_13;
        }
        return false;
      }
label_13:
      return true;
    }
  }

  public IDictionary<SapiLayer1.Parameter, object> RelatedParameterValues
  {
    get
    {
      if (this.relatedParameterValues == null && this.parameter == null && this.IsValidForChannel)
      {
        lock (this.parameterGroup.Channel.OfflineVarcodingHandleLock)
        {
          Varcode varcode = this.parameterGroup.Channel.OfflineVarcodingHandle;
          if (varcode != null)
          {
            byte[] currentCodingString = varcode.GetCurrentCodingString(this.parameterGroup.Qualifier);
            if (varcode.Exception == null)
            {
              varcode.SetCurrentCodingString(this.parameterGroup.Qualifier, new Dump(this.rawValue).Data.ToArray<byte>());
              if (varcode.Exception == null)
                this.relatedParameterValues = this.parameterGroup.Channel.ParameterGroups[this.parameterGroup.Qualifier].Parameters.ToDictionary<SapiLayer1.Parameter, SapiLayer1.Parameter, object>((Func<SapiLayer1.Parameter, SapiLayer1.Parameter>) (k => k), (Func<SapiLayer1.Parameter, object>) (v => v.GetPresentation(varcode)));
              varcode.SetCurrentCodingString(this.parameterGroup.Qualifier, currentCodingString);
            }
          }
        }
      }
      return (IDictionary<SapiLayer1.Parameter, object>) this.relatedParameterValues;
    }
  }

  public int? ContentLength
  {
    get => this.parameter != null ? new int?() : new int?(this.RawValue.Length / 2);
  }
}
