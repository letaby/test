// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBLocation
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBLocation
{
  private readonly MCDDbLocation location;
  private IEnumerable<McdDBJob> jobs;
  private IEnumerable<McdDBService> services;
  private IEnumerable<McdDBService> variantCodingWriteServices;
  private IEnumerable<McdDBDiagTroubleCode> faults;
  private IEnumerable<McdDBEnvDataDesc> envs;
  private IEnumerable<McdDBFlashSession> flashsessions;
  private IEnumerable<McdDBControlPrimitive> controlPrimitives;
  private IEnumerable<McdDBMatchingPattern> matchingPatterns;
  private IEnumerable<McdDBConfigurationData> configurationDatas;
  private readonly string qualifier;
  private DateTime flashSessionAcquisitionTime;
  private Dictionary<string, string> variantAttributes;

  internal McdDBLocation(MCDDbLocation location)
  {
    this.location = location;
    this.qualifier = this.location.ShortName;
  }

  public string DatabaseFile => ((DtsDbObject) this.location).DatabaseFile;

  public IEnumerable<McdDBService> DBServices
  {
    get
    {
      if (this.services == null)
        this.services = (IEnumerable<McdDBService>) this.location.DbServices.OfType<MCDDbService>().Where<MCDDbService>((Func<MCDDbService, bool>) (s => s.AddressingMode == MCDAddressingMode.ePHYSICAL)).Select<MCDDbService, McdDBService>((Func<MCDDbService, McdDBService>) (s => new McdDBService(s))).ToList<McdDBService>();
      return this.services;
    }
  }

  public IEnumerable<McdDBService> VariantCodingWriteDBServices
  {
    get
    {
      if (this.variantCodingWriteServices == null)
        this.variantCodingWriteServices = (IEnumerable<McdDBService>) this.DBServices.Where<McdDBService>((Func<McdDBService, bool>) (s => s.Semantic == "VARIANTCODINGWRITE")).ToList<McdDBService>();
      return this.variantCodingWriteServices;
    }
  }

  public IEnumerable<McdDBJob> DBJobs
  {
    get
    {
      if (this.jobs == null)
        this.jobs = (IEnumerable<McdDBJob>) this.location.DbJobs.OfType<MCDDbJob>().Select<MCDDbJob, McdDBJob>((Func<MCDDbJob, McdDBJob>) (s => new McdDBJob(s))).ToList<McdDBJob>();
      return this.jobs;
    }
  }

  public IEnumerable<McdDBDiagTroubleCode> DBDiagTroubleCodes
  {
    get
    {
      if (this.faults == null)
        this.faults = (IEnumerable<McdDBDiagTroubleCode>) this.location.GetDbDTCs((ushort) 0, (ushort) 0).OfType<MCDDbDiagTroubleCode>().Select<MCDDbDiagTroubleCode, McdDBDiagTroubleCode>((Func<MCDDbDiagTroubleCode, McdDBDiagTroubleCode>) (code => new McdDBDiagTroubleCode(code))).ToList<McdDBDiagTroubleCode>();
      return this.faults;
    }
  }

  public IEnumerable<McdDBEnvDataDesc> DBEnvironmentDataDescriptions
  {
    get
    {
      if (this.envs == null)
        this.envs = (IEnumerable<McdDBEnvDataDesc>) this.location.DbEnvDataDescs.OfType<MCDDbEnvDataDesc>().Select<MCDDbEnvDataDesc, McdDBEnvDataDesc>((Func<MCDDbEnvDataDesc, McdDBEnvDataDesc>) (e => new McdDBEnvDataDesc(e))).ToList<McdDBEnvDataDesc>();
      return this.envs;
    }
  }

  public IEnumerable<McdDBFlashSession> DBFlashSessions
  {
    get
    {
      if (this.flashsessions == null || this.flashSessionAcquisitionTime < McdRoot.FlashFileLastUpdateTime)
      {
        this.flashsessions = (IEnumerable<McdDBFlashSession>) this.location.DbFlashSessions.OfType<MCDDbFlashSession>().Select<MCDDbFlashSession, McdDBFlashSession>((Func<MCDDbFlashSession, McdDBFlashSession>) (fs => new McdDBFlashSession(fs))).ToList<McdDBFlashSession>();
        this.flashSessionAcquisitionTime = DateTime.Now;
      }
      return this.flashsessions;
    }
  }

  public IEnumerable<McdDBControlPrimitive> DBControlPrimitives
  {
    get
    {
      if (this.controlPrimitives == null)
        this.controlPrimitives = (IEnumerable<McdDBControlPrimitive>) this.location.DbControlPrimitives.OfType<MCDDbControlPrimitive>().Select<MCDDbControlPrimitive, McdDBControlPrimitive>((Func<MCDDbControlPrimitive, McdDBControlPrimitive>) (cp => new McdDBControlPrimitive(cp))).ToList<McdDBControlPrimitive>();
      return this.controlPrimitives;
    }
  }

  public IEnumerable<McdDBMatchingPattern> DBVariantPatterns
  {
    get
    {
      if (this.matchingPatterns == null)
        this.matchingPatterns = (IEnumerable<McdDBMatchingPattern>) this.location.DbVariantPatterns.OfType<MCDDbMatchingPattern>().Select<MCDDbMatchingPattern, McdDBMatchingPattern>((Func<MCDDbMatchingPattern, McdDBMatchingPattern>) (mp => new McdDBMatchingPattern(mp))).ToList<McdDBMatchingPattern>();
      return this.matchingPatterns;
    }
  }

  public IEnumerable<McdDBConfigurationData> DBConfigurationDatas
  {
    get
    {
      if (this.configurationDatas == null)
        this.configurationDatas = (IEnumerable<McdDBConfigurationData>) this.location.DbConfigurationDatas.OfType<MCDDbConfigurationData>().Select<MCDDbConfigurationData, McdDBConfigurationData>((Func<MCDDbConfigurationData, McdDBConfigurationData>) (cp => new McdDBConfigurationData(cp))).ToList<McdDBConfigurationData>();
      return this.configurationDatas;
    }
  }

  public string PartNumber
  {
    get
    {
      MCDDbSpecialDataGroup specialDataGroup = this.GetSpecialDataGroup("Part_Number");
      if (specialDataGroup != null)
      {
        string specialDataItem1 = McdDBLocation.GetSpecialDataItem(specialDataGroup, "number");
        string specialDataItem2 = McdDBLocation.GetSpecialDataItem(specialDataGroup, "version");
        if (specialDataItem1 != null && specialDataItem2 != null)
          return $"{specialDataItem1}_{specialDataItem2}";
      }
      return (string) null;
    }
  }

  private MCDDbSpecialDataGroup GetSpecialDataGroup(string caption)
  {
    foreach (MCDDbSpecialDataGroup dbSdG in (IEnumerable) this.location.DbSDGs)
    {
      if (dbSdG.HasCaption && dbSdG.Caption.ShortName == caption)
        return dbSdG;
    }
    return (MCDDbSpecialDataGroup) null;
  }

  private static string GetSpecialDataItem(MCDDbSpecialDataGroup sdg, string item)
  {
    for (uint index = 0; index < sdg.Count; ++index)
    {
      MCDDbSpecialData itemByIndex = sdg.GetItemByIndex(index);
      if (itemByIndex.SemanticInformation == item && itemByIndex.ObjectType == MCDObjectType.eMCDDBSPECIALDATAELEMENT)
        return ((MCDDbSpecialDataElement) itemByIndex).Content;
    }
    return (string) null;
  }

  public IDictionary<string, string> VariantAttributes
  {
    get
    {
      if (this.variantAttributes == null)
      {
        this.variantAttributes = new Dictionary<string, string>();
        MCDDbSpecialDataGroup specialDataGroup = this.GetSpecialDataGroup("Variant_Attributes");
        if (specialDataGroup != null)
        {
          for (uint index = 0; index < specialDataGroup.Count; ++index)
          {
            MCDDbSpecialData itemByIndex = specialDataGroup.GetItemByIndex(index);
            if (itemByIndex.ObjectType == MCDObjectType.eMCDDBSPECIALDATAELEMENT && !this.variantAttributes.ContainsKey(itemByIndex.SemanticInformation) && itemByIndex is MCDDbSpecialDataElement specialDataElement && specialDataElement.Content != "not set")
              this.variantAttributes.Add(itemByIndex.SemanticInformation, specialDataElement.Content);
          }
        }
      }
      return (IDictionary<string, string>) this.variantAttributes;
    }
  }

  public string Qualifier => this.qualifier;
}
