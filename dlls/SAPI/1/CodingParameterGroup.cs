// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingParameterGroup
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CodingParameterGroup : ICodingItem
{
  private string mcdQualifier;
  private int? channelByteLength;
  private Dictionary<string, IEnumerable<CodingChoicesForCoding>> options = new Dictionary<string, IEnumerable<CodingChoicesForCoding>>();
  private Dump defaultStringMask;
  private string qualifier;
  private string name;
  private string description;
  private int? byteLength;
  private CodingChoiceCollection choices;
  private CodingParameterCollection parameters;
  private IList<DiagnosisVariant> diagnosisVariants;
  private CodingParameterGroupCollection parameterGroups;
  private Service readComService;

  internal CodingParameterGroup(CodingParameterGroupCollection parent)
  {
    this.parameterGroups = parent;
    this.choices = new CodingChoiceCollection();
    this.parameters = new CodingParameterCollection();
  }

  internal void Acquire(CaesarDIVcd varcode)
  {
    this.qualifier = varcode.Qualifier;
    this.description = varcode.Description;
    this.choices.AcquireList(this, varcode);
    this.parameters.AcquireList(this, varcode);
    List<DiagnosisVariant> diagnosisVariantList = new List<DiagnosisVariant>();
    Sapi sapi = Sapi.GetSapi();
    uint allowedEcuCount = varcode.AllowedEcuCount;
    for (uint index1 = 0; index1 < allowedEcuCount; ++index1)
    {
      string ecuName = varcode.GetAllowedEcuByIndex(index1);
      Ecu ecu = sapi.Ecus.FirstOrDefault<Ecu>((Func<Ecu, bool>) (e => e.Name == ecuName && e.DiagnosisSource == DiagnosisSource.CaesarDatabase));
      if (ecu != null)
      {
        uint allowedEcuVariantCount = varcode.GetAllowedEcuVariantCount(index1);
        if (allowedEcuVariantCount > 0U)
        {
          for (uint index2 = 0; index2 < allowedEcuVariantCount; ++index2)
          {
            string ecuVariantByIndex = varcode.GetAllowedEcuVariantByIndex(index1, index2);
            DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[ecuVariantByIndex];
            if (diagnosisVariant != null)
              diagnosisVariantList.Add(diagnosisVariant);
          }
        }
        else
        {
          foreach (DiagnosisVariant diagnosisVariant in (ReadOnlyCollection<DiagnosisVariant>) ecu.DiagnosisVariants)
            diagnosisVariantList.Add(diagnosisVariant);
        }
      }
    }
    this.diagnosisVariants = (IList<DiagnosisVariant>) diagnosisVariantList.AsReadOnly();
  }

  internal void Acquire(McdDBConfigurationRecord domain, IEnumerable<DiagnosisVariant> variants)
  {
    this.qualifier = McdCaesarEquivalence.GetDomainQualifier(domain.Name);
    this.mcdQualifier = domain.Qualifier;
    this.name = domain.Name;
    this.description = domain.Description;
    this.byteLength = new int?(domain.ByteLength);
    this.diagnosisVariants = (IList<DiagnosisVariant>) variants.ToList<DiagnosisVariant>().AsReadOnly();
    this.choices.AcquireList(this, domain);
    this.parameters.AcquireList(this, domain);
  }

  internal void Acquire(StreamReader reader, string meaning, Ecu ecu, Part partNumber)
  {
    this.qualifier = "*";
    List<DiagnosisVariant> diagnosisVariantList = new List<DiagnosisVariant>();
    string identificationRecordValue = ParameterCollection.GetIdentificationRecordValue("DIAGNOSISVARIANT", reader);
    if (string.IsNullOrEmpty(identificationRecordValue))
    {
      foreach (DiagnosisVariant diagnosisVariant in (ReadOnlyCollection<DiagnosisVariant>) ecu.DiagnosisVariants)
        diagnosisVariantList.Add(diagnosisVariant);
    }
    else
    {
      DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[identificationRecordValue];
      if (diagnosisVariant != null)
        diagnosisVariantList.Add(diagnosisVariant);
      else
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A Target diagnostic variant specified in a CPF file does not exist: {0}", (object) identificationRecordValue));
    }
    this.diagnosisVariants = (IList<DiagnosisVariant>) diagnosisVariantList.AsReadOnly();
    CodingChoice choice = new CodingChoice(this);
    choice.Acquire(reader, meaning, partNumber);
    this.choices.Add(choice);
  }

  internal CodingParameterGroup Clone(CodingParameterGroupCollection newParent)
  {
    CodingParameterGroup destination = new CodingParameterGroup(newParent);
    destination.qualifier = this.qualifier;
    destination.mcdQualifier = this.mcdQualifier;
    destination.byteLength = this.byteLength;
    destination.name = this.name;
    destination.description = this.description;
    destination.diagnosisVariants = (IList<DiagnosisVariant>) new List<DiagnosisVariant>((IEnumerable<DiagnosisVariant>) this.diagnosisVariants).AsReadOnly();
    destination.parameters = new CodingParameterCollection();
    destination.choices = new CodingChoiceCollection();
    this.CopyTo(destination);
    return destination;
  }

  internal void CopyTo(CodingParameterGroup destination)
  {
    List<Parameter> source = new List<Parameter>();
    if (destination.Channel != null)
    {
      source.AddRange(destination.Channel.Parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.GroupQualifier == this.Qualifier)));
      destination.readComService = source.Any<Parameter>() ? source.First<Parameter>().ReadService : (Service) null;
    }
    foreach (CodingParameter parameter1 in (ReadOnlyCollection<CodingParameter>) this.parameters)
    {
      CodingParameter parameter = parameter1;
      destination.parameters.Add(parameter.Clone(destination, source.FirstOrDefault<Parameter>((Func<Parameter, bool>) (p =>
      {
        if (p.OriginalName == parameter.Name)
          return true;
        if (parameter.Qualifier == null)
          return false;
        return p.Qualifier == parameter.Qualifier || p.McdQualifier == parameter.Qualifier;
      }))));
    }
    foreach (CodingChoice choice in (ReadOnlyCollection<CodingChoice>) this.choices)
      destination.choices.Add(choice.Clone((CodingParameter) null, destination));
  }

  public string Qualifier => this.qualifier;

  public string McdQualifier => this.mcdQualifier;

  public string Name => this.name ?? this.qualifier;

  public string Description => this.description;

  public int? ByteLength => this.byteLength;

  public int? ChannelByteLength
  {
    get
    {
      if (!this.channelByteLength.HasValue)
        this.channelByteLength = (int?) this.Channel?.Parameters.GetItemFirstInGroup(this.Qualifier)?.GroupLength;
      return this.channelByteLength;
    }
  }

  public CodingChoiceCollection Choices => this.choices;

  public CodingParameterCollection Parameters => this.parameters;

  public CodingParameterGroupCollection ParameterGroups => this.parameterGroups;

  public Channel Channel => this.ParameterGroups.Channel;

  public IList<DiagnosisVariant> DiagnosisVariants => this.diagnosisVariants;

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("The GetDiagnosisVariants method is deprecated, please use the DiagnosisVariants property instead.")]
  public DiagnosisVariant[] GetDiagnosisVariants()
  {
    return this.diagnosisVariants.ToArray<DiagnosisVariant>();
  }

  public bool VariantAllowed(DiagnosisVariant variant)
  {
    foreach (DiagnosisVariant diagnosisVariant in (IEnumerable<DiagnosisVariant>) this.diagnosisVariants)
    {
      if (diagnosisVariant == variant)
        return true;
    }
    return false;
  }

  private IEnumerable<CodingChoice> GetApplicableFragments(
    VarcodeCaesar varcode,
    IEnumerable<CodingChoice> allFragmentValues,
    byte[] setData)
  {
    List<CodingChoice> applicableFragments = new List<CodingChoice>();
    foreach (CodingChoice codingChoice in allFragmentValues.Where<CodingChoice>((Func<CodingChoice, bool>) (fv => fv.IsValidForChannel)))
    {
      varcode.SetCurrentCodingString(this.qualifier, setData);
      if (varcode.IsErrorSet)
      {
        CaesarException exception = varcode.Exception;
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{codingChoice.Parameter.Channel.Ecu.Name}.{codingChoice.Parameter.ParameterGroup.Qualifier}: {exception.Message} while attempting to set initial coding string'{BitConverter.ToString(setData).Replace("-", "")}'");
        break;
      }
      codingChoice.SetAsValue((Varcode) varcode);
      if (varcode.IsErrorSet)
      {
        CaesarException exception = varcode.Exception;
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{codingChoice.Parameter.Channel.Ecu.Name}.{codingChoice.Parameter.ParameterGroup.Qualifier}.{codingChoice.Parameter.Name}: {exception.Message} while attempting to apply fragment value '{codingChoice.RawValue}'");
      }
      else if (((IEnumerable<byte>) setData).SequenceEqual<byte>((IEnumerable<byte>) varcode.GetCurrentCodingString(this.qualifier)))
        applicableFragments.Add(codingChoice);
    }
    return (IEnumerable<CodingChoice>) applicableFragments;
  }

  private IEnumerable<CodingChoice> GetApplicableFragments(
    VarcodeMcd varcode,
    IEnumerable<CodingChoice> allFragmentValues,
    byte[] setData)
  {
    List<CodingChoice> applicableFragments = new List<CodingChoice>();
    CodingChoice codingChoice1 = this.Choices.FirstOrDefault<CodingChoice>((Func<CodingChoice, bool>) (c =>
    {
      int? contentLength = c.ContentLength;
      int length = setData.Length;
      return contentLength.GetValueOrDefault() == length && contentLength.HasValue;
    }));
    if (codingChoice1 != null)
    {
      codingChoice1.SetAsValue((Varcode) varcode);
      if (varcode.IsErrorSet)
      {
        CaesarException exception = varcode.Exception;
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{this.Channel.Ecu.Name}.{this.Qualifier}: {exception.Message} while attempting to apply dummy default string value '{codingChoice1.RawValue}'");
        return (IEnumerable<CodingChoice>) applicableFragments;
      }
    }
    foreach (IGrouping<CodingParameter, CodingChoice> grouping in allFragmentValues.Where<CodingChoice>((Func<CodingChoice, bool>) (fv =>
    {
      int? bytePos = fv.Parameter.BytePos;
      int length = setData.Length;
      return bytePos.GetValueOrDefault() < length && bytePos.HasValue;
    })).GroupBy<CodingChoice, CodingParameter>((Func<CodingChoice, CodingParameter>) (c => c.Parameter)))
    {
      foreach (CodingChoice codingChoice2 in (IEnumerable<CodingChoice>) grouping)
      {
        codingChoice2.SetAsValue((Varcode) varcode);
        if (varcode.IsErrorSet)
        {
          CaesarException exception = varcode.Exception;
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{codingChoice2.Parameter.Channel.Ecu.Name}.{codingChoice2.Parameter.ParameterGroup.Qualifier}.{codingChoice2.Parameter.Name}: {exception.Message} while attempting to apply fragment value '{codingChoice2.RawValue}'");
        }
        else
        {
          byte[] currentCodingString = varcode.GetCurrentCodingString(this.qualifier);
          if (varcode.IsErrorSet)
          {
            CaesarException exception = varcode.Exception;
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{codingChoice2.Parameter.Channel.Ecu.Name}.{codingChoice2.Parameter.ParameterGroup.Qualifier}.{codingChoice2.Parameter.Name}: {exception.Message} while attempting to read coding string after applying fragment value '{codingChoice2.RawValue}'");
          }
          else if (currentCodingString.Length != setData.Length)
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{codingChoice2.Parameter.Channel.Ecu.Name}.{codingChoice2.Parameter.ParameterGroup.Qualifier}.{codingChoice2.Parameter.Name}: got mismatched coding string length {(object) currentCodingString.Length} instead of set data length {(object) setData.Length} after applying fragment value '{codingChoice2.RawValue}'");
          else if (grouping.Key.AreMaskedCodingStringsEquivalent(currentCodingString, setData))
          {
            applicableFragments.Add(codingChoice2);
            break;
          }
        }
      }
    }
    return (IEnumerable<CodingChoice>) applicableFragments;
  }

  private IEnumerable<CodingChoice> GetApplicableFragments(
    Varcode varcode,
    IEnumerable<CodingChoice> allFragmentValues,
    byte[] setData)
  {
    return !(varcode is VarcodeCaesar varcode1) ? this.GetApplicableFragments((VarcodeMcd) varcode, allFragmentValues, setData) : this.GetApplicableFragments(varcode1, allFragmentValues, setData);
  }

  public IEnumerable<CodingChoicesForCoding> GetChoicesAndCodingForCoding(string coding)
  {
    lock (this.Channel.OfflineVarcodingHandleLock)
    {
      if (!this.options.ContainsKey(coding))
      {
        if (this.Parameters.Count == 0)
          this.options[coding] = this.Choices.Where<CodingChoice>((Func<CodingChoice, bool>) (d => d.RawValue != null && d.RawValue.Length == coding.Length)).Select<CodingChoice, CodingChoicesForCoding>((Func<CodingChoice, CodingChoicesForCoding>) (d => new CodingChoicesForCoding(new List<CodingChoice>()
          {
            d
          }.AsEnumerable<CodingChoice>(), new Dump(d.RawValue))));
        else
          this.AcquireDefaultStringandFragmentChoicesForCoding(coding, this.Channel.OfflineVarcodingHandle ?? throw new CaesarException(SapiError.OfflineVarcodingNotAvailable));
      }
      return this.options[coding];
    }
  }

  public IEnumerable<IEnumerable<CodingChoice>> GetChoicesForCoding(string coding)
  {
    return this.GetChoicesAndCodingForCoding(coding).Where<CodingChoicesForCoding>((Func<CodingChoicesForCoding, bool>) (set => set.Coding == null || set.Coding.ToString() == coding)).Select<CodingChoicesForCoding, IEnumerable<CodingChoice>>((Func<CodingChoicesForCoding, IEnumerable<CodingChoice>>) (set => set.CodingChoices));
  }

  internal void AcquireDefaultStringandFragmentChoicesForCoding(string coding)
  {
    lock (this.Channel.OfflineVarcodingHandleLock)
    {
      Varcode offlineVarcodingHandle = this.Channel.OfflineVarcodingHandle;
      if (offlineVarcodingHandle == null || this.options.ContainsKey(coding) || this.Parameters.Count <= 0)
        return;
      this.AcquireDefaultStringandFragmentChoicesForCoding(coding, offlineVarcodingHandle);
    }
  }

  private void AcquireDefaultStringandFragmentChoicesForCoding(string coding, Varcode varcode)
  {
    byte[] array1 = new Dump(coding).Data.ToArray<byte>();
    IEnumerable<CodingChoice> list = (IEnumerable<CodingChoice>) this.Parameters.SelectMany((Func<CodingParameter, IEnumerable<CodingChoice>>) (parameter => (IEnumerable<CodingChoice>) parameter.Choices), (parameter, fragmentValue) => new
    {
      parameter = parameter,
      fragmentValue = fragmentValue
    }).Where(_param1 => _param1.fragmentValue.Part != null).Select(_param1 => _param1.fragmentValue).ToList<CodingChoice>();
    List<CodingChoicesForCoding> source = new List<CodingChoicesForCoding>();
    IEnumerable<CodingChoice> applicableFragments1 = this.GetApplicableFragments(varcode, list, array1);
    foreach (CodingChoice codingChoice1 in this.Choices.Where<CodingChoice>((Func<CodingChoice, bool>) (c => c.RawValue.Length == coding.Length)))
    {
      byte[] array2 = new Dump(codingChoice1.RawValue).Data.ToArray<byte>();
      IEnumerable<CodingChoice> applicableFragments2 = this.GetApplicableFragments(varcode, applicableFragments1, array2);
      IEnumerable<CodingChoice> second = applicableFragments1.Except<CodingChoice>(applicableFragments2);
      varcode.AllowSetDefaultString(this.qualifier);
      codingChoice1.SetAsValue(varcode);
      if (varcode.IsErrorSet)
      {
        CaesarException exception = varcode.Exception;
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{exception.Message} while attempting to apply default string value {codingChoice1.RawValue} for {this.qualifier}");
      }
      else
      {
        foreach (CodingChoice codingChoice2 in second)
        {
          codingChoice2.SetAsValue(varcode);
          if (varcode.IsErrorSet)
          {
            CaesarException exception = varcode.Exception;
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"{exception.Message} while attempting to re-apply fragment value {codingChoice2.RawValue} for {codingChoice2.Parameter.Name}");
          }
        }
        byte[] currentCodingString = varcode.GetCurrentCodingString(this.qualifier);
        source.Add(new CodingChoicesForCoding(new List<CodingChoice>()
        {
          codingChoice1
        }.Union<CodingChoice>(second), new Dump((IEnumerable<byte>) currentCodingString)));
      }
    }
    if (source.Count<CodingChoicesForCoding>((Func<CodingChoicesForCoding, bool>) (res => res.Coding.ToString() == coding)) == 0)
      source.Add(new CodingChoicesForCoding(applicableFragments1, (Dump) null));
    this.options[coding] = (IEnumerable<CodingChoicesForCoding>) source.OrderBy<CodingChoicesForCoding, int>((Func<CodingChoicesForCoding, int>) (set => set.CodingChoices.Count<CodingChoice>()));
    if (varcode.AllowSetDefaultString(this.qualifier) || !varcode.IsErrorSet)
      return;
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error after VCAllowSetDefaultString({0}): {1}", (object) this.qualifier, (object) varcode.Exception.Message));
  }

  public Service ReadService => this.readComService;

  public Dump DefaultStringMask
  {
    get
    {
      if (this.defaultStringMask == null && this.ByteLength.HasValue)
        this.defaultStringMask = CodingParameter.CreateCodingStringMask(this.ChannelByteLength.HasValue ? this.ChannelByteLength.Value : this.ByteLength.Value, (IEnumerable<CodingParameter>) this.Parameters, false);
      return this.defaultStringMask;
    }
  }
}
