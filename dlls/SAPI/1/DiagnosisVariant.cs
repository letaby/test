// Decompiled with JetBrains decompiler
// Type: SapiLayer1.DiagnosisVariant
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace SapiLayer1;

public sealed class DiagnosisVariant
{
  private Ecu ecu;
  private string name;
  private string description;
  private bool isBase;
  private IEnumerable<Tuple<string, string>> parameterQualifiers;
  private Part partNumber;
  private IEnumerable<CaesarIdBlock> idBlocks;
  private IEnumerable<McdDBMatchingPattern> mcdIdBlocks;
  private readonly IEnumerable<Tuple<RollCall.ID, string>> rollCallIdBlock;
  private IEnumerable<string> diagServiceQualifiers;
  private IEnumerable<string> flashJobQualifiers;
  private IDictionary<string, string> variantAttributes;
  private List<Tuple<string, string>> controlPrimitiveNames;

  internal DiagnosisVariant(Ecu ecu, string name, string description, bool isBase)
    : this(ecu, name, description, (Part) null, (IEnumerable<CaesarIdBlock>) new List<CaesarIdBlock>())
  {
    this.isBase = isBase;
  }

  internal DiagnosisVariant(
    Ecu ecu,
    string name,
    string equipmentType,
    IEnumerable<Tuple<RollCall.ID, string>> rollCallIdBlock,
    IEnumerable<string> rollCallQualifiers)
    : this(ecu, name, string.Empty, (Part) null, (IEnumerable<CaesarIdBlock>) new List<CaesarIdBlock>())
  {
    this.rollCallIdBlock = rollCallIdBlock;
    this.diagServiceQualifiers = rollCallQualifiers;
    this.FixedEquipmentType = equipmentType;
  }

  internal DiagnosisVariant(
    Ecu ecu,
    string name,
    string description,
    Part partNumber,
    IEnumerable<CaesarIdBlock> idBlocks)
  {
    this.name = name;
    this.ecu = ecu;
    this.description = description;
    this.partNumber = partNumber;
    this.idBlocks = idBlocks;
  }

  internal DiagnosisVariant(
    Ecu ecu,
    string name,
    string description,
    Part partNumber,
    IDictionary<string, string> variantAttributes,
    IEnumerable<McdDBMatchingPattern> idBlocks,
    bool isBase)
  {
    this.name = name;
    this.ecu = ecu;
    this.description = description;
    this.partNumber = partNumber;
    this.mcdIdBlocks = idBlocks;
    this.variantAttributes = variantAttributes;
    this.isBase = isBase;
  }

  internal McdDBLocation GetMcdDBLocationForProtocol(string protocol)
  {
    McdDBEcuBaseVariant mcdHandle = this.ecu.GetMcdHandle();
    if (mcdHandle != null)
    {
      if (this.IsBase)
        return mcdHandle.GetDBLocationForProtocol(protocol);
      McdDBEcuVariant dbEcuVariant = mcdHandle.GetDBEcuVariant(this.name);
      if (dbEcuVariant != null)
        return dbEcuVariant.GetDBLocationForProtocol(protocol);
    }
    return (McdDBLocation) null;
  }

  public IEnumerable<string> Locations { get; internal set; }

  internal CaesarEcu OpenEcuHandle()
  {
    CaesarEcu caesarEcu = this.ecu.OpenEcuHandle();
    if (caesarEcu != null && !this.IsBase)
      caesarEcu.SetVariant(this.Name);
    return caesarEcu;
  }

  internal IEnumerable<Tuple<string, string>> ParameterQualifiers
  {
    get
    {
      if (this.parameterQualifiers == null)
      {
        if (!this.ecu.IsRollCall)
        {
          if (this.ecu.IsMcd)
          {
            this.parameterQualifiers = DiagnosisVariant.GetQualifierList(this.GetMcdDBLocationForProtocol(this.ecu.ProtocolName));
          }
          else
          {
            using (CaesarEcu ecu = this.OpenEcuHandle())
              this.parameterQualifiers = DiagnosisVariant.GetQualifierList(ecu);
          }
        }
        else
          this.parameterQualifiers = (IEnumerable<Tuple<string, string>>) DiagnosisVariant.GetQualifierList(this.ecu.GetDefParser()).Select<string, Tuple<string, string>>((Func<string, Tuple<string, string>>) (p => Tuple.Create<string, string>(string.Empty, p))).ToList<Tuple<string, string>>();
      }
      return this.parameterQualifiers;
    }
  }

  public override string ToString() => this.name;

  public Ecu Ecu => this.ecu;

  public string Name => this.name;

  public string Description => this.description;

  public string FixedEquipmentType { private set; get; }

  public bool IsBase
  {
    get
    {
      if (!this.ecu.IsRollCall)
        return this.isBase;
      return this.rollCallIdBlock == null || !this.rollCallIdBlock.Any<Tuple<RollCall.ID, string>>();
    }
  }

  public bool IsBoot
  {
    get
    {
      long? diagnosticVersionLong = this.DiagnosticVersionLong;
      if (!diagnosticVersionLong.HasValue)
        return false;
      long? nullable1 = diagnosticVersionLong;
      long num1 = 65536 /*0x010000*/;
      long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() & num1) : new long?();
      long num2 = 65536 /*0x010000*/;
      return nullable2.GetValueOrDefault() == num2 && nullable2.HasValue;
    }
  }

  public Part PartNumber => this.partNumber;

  public IDictionary<string, string> VariantAttributes
  {
    get
    {
      IDictionary<string, string> variantAttributes = this.variantAttributes;
      return variantAttributes == null ? (IDictionary<string, string>) null : (IDictionary<string, string>) variantAttributes.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kv => kv.Key), (Func<KeyValuePair<string, string>, string>) (kv => kv.Value));
    }
  }

  public long? DiagnosticVersionLong
  {
    get
    {
      switch (this.ecu.DiagnosisSource)
      {
        case DiagnosisSource.CaesarDatabase:
          if (this.idBlocks != null)
          {
            uint? nullable = this.idBlocks.Where<CaesarIdBlock>((Func<CaesarIdBlock, bool>) (x => x.DiagVersionLong.HasValue)).Select<CaesarIdBlock, uint?>((Func<CaesarIdBlock, uint?>) (x => x.DiagVersionLong)).FirstOrDefault<uint?>();
            return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
          }
          break;
        case DiagnosisSource.McdDatabase:
          if (this.mcdIdBlocks != null)
          {
            using (IEnumerator<McdDBMatchingPattern> enumerator = this.mcdIdBlocks.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                foreach (McdDBMatchingParameter matchingParameter in enumerator.Current.DBMatchingParameters)
                {
                  if (matchingParameter.Primitive == "ActiveDiagnosticInformation_Read" && matchingParameter.ResponseParameter == "Identification")
                    return new long?(Convert.ToInt64(matchingParameter.ExpectedValue.Value, (IFormatProvider) CultureInfo.InvariantCulture));
                }
              }
              break;
            }
          }
          break;
      }
      return new long?();
    }
  }

  internal IEnumerable<string> DiagServiceQualifiers
  {
    get
    {
      if (this.diagServiceQualifiers == null && !this.Ecu.IsRollCall)
      {
        if (!this.Ecu.IsMcd)
        {
          using (CaesarEcu caesarEcu = this.OpenEcuHandle())
            this.diagServiceQualifiers = (IEnumerable<string>) caesarEcu.GetServices((ServiceTypes) 16582227).Cast<string>().ToList<string>();
        }
        else
        {
          List<string> diagServiceQualifiers = new List<string>();
          foreach (McdCaesarEquivalence caesarEquivalence in McdCaesarEquivalence.FromDBLocation(this.GetMcdDBLocationForProtocol(this.ecu.ProtocolName)).Where<McdCaesarEquivalence>((Func<McdCaesarEquivalence, bool>) (s => (s.ServiceTypes & (ServiceTypes.Actuator | ServiceTypes.Adjustment | ServiceTypes.Data | ServiceTypes.Download | ServiceTypes.DiagJob | ServiceTypes.Function | ServiceTypes.Global | ServiceTypes.IOControl | ServiceTypes.Routine | ServiceTypes.Security | ServiceTypes.Session | ServiceTypes.Static | ServiceTypes.StoredData)) != 0)))
          {
            McdCaesarEquivalence caesarEquivalentStructuredService = caesarEquivalence;
            Dictionary<string, int> caesarEquivalentQualifiers = new Dictionary<string, int>();
            AcquireCaesarEquivalentQualifiers(caesarEquivalentStructuredService.Service.ResponseParameters, new List<string>());

            void AcquireCaesarEquivalentQualifiers(
              IEnumerable<McdDBResponseParameter> responseParameterSet,
              List<string> parentParameterNames)
            {
              bool flag = responseParameterSet.AllSiblingsAreStructures();
              foreach (McdDBResponseParameter responseParameter in responseParameterSet)
              {
                if (!responseParameter.IsConst && !responseParameter.IsMatchingRequestParameter && !responseParameter.IsReserved)
                {
                  if (responseParameter.DataType != (Type) null && !responseParameter.IsStructure && !responseParameter.IsArray)
                  {
                    string qualifier = caesarEquivalentStructuredService.Qualifier;
                    string name = caesarEquivalentStructuredService.Name;
                    McdCaesarEquivalence.AdjustServiceQualifierName(caesarEquivalentStructuredService.Service, responseParameter, ref qualifier, ref name);
                    diagServiceQualifiers.Add(McdCaesarEquivalence.MakeQualifier($"{qualifier}_{string.Join("_", parentParameterNames.Concat<string>(Enumerable.Repeat<string>(this.Ecu.CaesarEquivalentResponseParameterQualifierSource != Ecu.ResponseParameterQualifierSource.Qualifier ? responseParameter.Name : responseParameter.Qualifier, 1)))}", caesarEquivalentQualifiers));
                  }
                  if (responseParameter.Parameters.Any<McdDBResponseParameter>())
                  {
                    List<string> list = parentParameterNames.ToList<string>();
                    if (!responseParameter.IsStructure | flag)
                      list.Add(responseParameter.Name);
                    AcquireCaesarEquivalentQualifiers(responseParameter.Parameters, list.ToList<string>());
                  }
                }
              }
            }
          }
          this.diagServiceQualifiers = (IEnumerable<string>) diagServiceQualifiers;
        }
      }
      return this.diagServiceQualifiers;
    }
  }

  public IEnumerable<string> FlashJobQualifiers
  {
    get
    {
      if (this.flashJobQualifiers == null && !this.Ecu.IsRollCall)
      {
        List<string> stringList = new List<string>();
        if (!this.Ecu.IsMcd)
        {
          using (CaesarEcu caesarEcu = this.OpenEcuHandle())
          {
            foreach (string str in caesarEcu.GetServices((ServiceTypes) 33554432 /*0x02000000*/).Cast<string>().ToList<string>())
            {
              using (CaesarDiagService caesarDiagService = caesarEcu.OpenDiagServiceHandle(str))
              {
                if (caesarDiagService != null)
                {
                  if (caesarDiagService.RequestMessage == null)
                    stringList.Add(str);
                }
              }
            }
            this.flashJobQualifiers = (IEnumerable<string>) stringList;
          }
        }
        else
          this.flashJobQualifiers = this.GetMcdDBLocationForProtocol(this.ecu.ProtocolName).DBJobs.Where<McdDBJob>((Func<McdDBJob, bool>) (j => j.IsFlashJob)).Select<McdDBJob, string>((Func<McdDBJob, string>) (j => j.Qualifier));
      }
      return this.flashJobQualifiers;
    }
  }

  public IDictionary<string, string> ControlPrimitiveQualifiers
  {
    get
    {
      if (this.controlPrimitiveNames == null && this.ecu.IsMcd)
        this.controlPrimitiveNames = this.GetMcdDBLocationForProtocol(this.ecu.ProtocolName).DBControlPrimitives.Select<McdDBControlPrimitive, Tuple<string, string>>((Func<McdDBControlPrimitive, Tuple<string, string>>) (cp => Tuple.Create<string, string>(cp.Qualifier, cp.Name))).ToList<Tuple<string, string>>();
      List<Tuple<string, string>> controlPrimitiveNames = this.controlPrimitiveNames;
      return controlPrimitiveNames == null ? (IDictionary<string, string>) null : (IDictionary<string, string>) controlPrimitiveNames.ToDictionary<Tuple<string, string>, string, string>((Func<Tuple<string, string>, string>) (k => k.Item1), (Func<Tuple<string, string>, string>) (v => v.Item2));
    }
  }

  public IDictionary<string, string> MatchingParameters
  {
    get
    {
      if (this.mcdIdBlocks == null || this.mcdIdBlocks.Count<McdDBMatchingPattern>() <= 0)
        return (IDictionary<string, string>) null;
      Dictionary<string, string> matchingParameters = new Dictionary<string, string>();
      foreach (McdDBMatchingParameter matchingParameter in this.mcdIdBlocks.First<McdDBMatchingPattern>().DBMatchingParameters)
        matchingParameters.Add($"{matchingParameter.Primitive}.{matchingParameter.ResponseParameter}", matchingParameter.ExpectedValue?.Value?.ToString() ?? string.Empty);
      return (IDictionary<string, string>) matchingParameters;
    }
  }

  internal int RollCallIdentificationCount
  {
    get
    {
      return this.rollCallIdBlock == null ? 0 : this.rollCallIdBlock.Count<Tuple<RollCall.ID, string>>();
    }
  }

  private static bool IsMatch(
    Tuple<RollCall.ID, string> requiredId,
    Tuple<RollCall.ID, object> readId)
  {
    if (requiredId.Item1 != readId.Item1)
      return false;
    if (!(readId.Item2 is string input))
      return requiredId.Item2 == readId.Item2.ToString();
    Match match = Regex.Match(input, requiredId.Item2);
    return match.Success && match.Value == input;
  }

  internal bool IsMatch(
    IEnumerable<Tuple<RollCall.ID, object>> readIdBlock)
  {
    if (this.rollCallIdBlock == null)
      return true;
    foreach (Tuple<RollCall.ID, string> tuple in this.rollCallIdBlock)
    {
      Tuple<RollCall.ID, string> requiredId = tuple;
      if (!readIdBlock.Any<Tuple<RollCall.ID, object>>((Func<Tuple<RollCall.ID, object>, bool>) (readId => DiagnosisVariant.IsMatch(requiredId, readId))))
        return false;
    }
    return true;
  }

  internal bool IsMatch(McdLogicalLink link)
  {
    return this.mcdIdBlocks != null && this.mcdIdBlocks.Any<McdDBMatchingPattern>((Func<McdDBMatchingPattern, bool>) (id => id.DBMatchingParameters.All<McdDBMatchingParameter>((Func<McdDBMatchingParameter, bool>) (mp => mp.IsMatch(link)))));
  }

  private static IEnumerable<string> GetQualifierList(TextFieldParser parser)
  {
    if (parser != null)
    {
      while (!parser.EndOfData)
      {
        string[] strArray = parser.ReadFields();
        if (strArray[0] == "P" && strArray.Length >= 10)
          yield return strArray[3];
      }
    }
  }

  private static IEnumerable<Tuple<string, string>> GetQualifierList(CaesarEcu ecu)
  {
    StringCollection varCodeDomains = ecu.VarCodeDomains;
    List<Tuple<string, string>> qualifierList = new List<Tuple<string, string>>();
    for (int index1 = 0; index1 < varCodeDomains.Count; ++index1)
    {
      using (CaesarDIVarCodeDom caesarDiVarCodeDom = ecu.OpenVarCodeDomain(varCodeDomains[index1]))
      {
        if (caesarDiVarCodeDom != null)
        {
          uint varCodeFragCount = caesarDiVarCodeDom.VarCodeFragCount;
          for (uint index2 = 0; index2 < varCodeFragCount; ++index2)
          {
            using (CaesarDIVarCodeFrag caesarDiVarCodeFrag = caesarDiVarCodeDom.OpenVarCodeFrag(index2))
            {
              if (caesarDiVarCodeFrag != null)
                qualifierList.Add(Tuple.Create<string, string>(varCodeDomains[index1], caesarDiVarCodeFrag.Qualifier));
            }
          }
        }
      }
    }
    return (IEnumerable<Tuple<string, string>>) qualifierList;
  }

  private static IEnumerable<Tuple<string, string>> GetQualifierList(McdDBLocation ecu)
  {
    List<Tuple<string, string>> qualifierList = new List<Tuple<string, string>>();
    foreach (McdDBService codingWriteDbService in ecu.VariantCodingWriteDBServices)
    {
      string domainQualifier = McdCaesarEquivalence.GetDomainQualifier(codingWriteDbService.Name);
      foreach (McdDBRequestParameter requestParameter in codingWriteDbService.AllRequestParameters)
      {
        if (!requestParameter.IsConst && !requestParameter.IsReserved)
          qualifierList.Add(Tuple.Create<string, string>(domainQualifier, McdCaesarEquivalence.MakeQualifier(requestParameter.Name)));
      }
    }
    return (IEnumerable<Tuple<string, string>>) qualifierList;
  }

  internal T GetEcuInfoAttribute<T>(string attribute, string qualifier)
  {
    return this.Ecu.GetEcuInfoAttribute<T>(attribute, qualifier, this.Name);
  }

  internal int? GetEcuInfoReadAccessLevel(string qualifier)
  {
    return this.GetEcuInfoAttribute<int?>("ReadAccess", qualifier);
  }

  internal int? GetEcuInfoWriteAccessLevel(string qualifier)
  {
    return this.GetEcuInfoAttribute<int?>("WriteAccess", qualifier);
  }

  internal int? GetEcuInfoLimitedRangeMin(string qualifier)
  {
    return this.GetEcuInfoAttribute<int?>("LimitedRangeMin", qualifier);
  }

  internal int? GetEcuInfoLimitedRangeMax(string qualifier)
  {
    return this.GetEcuInfoAttribute<int?>("LimitedRangeMax", qualifier);
  }

  internal string GetEcuInfoProhibitedChoices(string qualifier)
  {
    return this.GetEcuInfoAttribute<string>("ProhibitedChoices", qualifier);
  }

  internal byte[] GetEcuInfoIgnoreNegativeResponses(string qualifier)
  {
    string ecuInfoAttribute = this.GetEcuInfoAttribute<string>("IgnoreNegativeResponse", qualifier);
    return ecuInfoAttribute == null ? (byte[]) null : new Dump(ecuInfoAttribute.Replace(", ", string.Empty)).Data.ToArray<byte>();
  }
}
