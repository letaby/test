// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBDiagComPrimitive
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

public class McdDBDiagComPrimitive
{
  private MCDDbDiagComPrimitive service;
  private MCDObjectType objectType;
  private IEnumerable<byte> defaultPdu;
  private IEnumerable<McdDBRequestParameter> requestParameters;
  private IEnumerable<McdDBResponseParameter> responseParameters;

  internal McdDBDiagComPrimitive(MCDDbDiagComPrimitive service)
  {
    this.service = service;
    this.Qualifier = this.service.ShortName;
    this.Name = this.service.LongName;
    this.Semantic = this.service.Semantic;
    this.objectType = this.service.ObjectType;
  }

  public McdObjectType ObjectType => (McdObjectType) this.objectType;

  public IList<string> FunctionalClassQualifiers
  {
    get
    {
      MCDDbFunctionalClasses functionalClasses = this.service.DbFunctionalClasses;
      return functionalClasses == null ? (IList<string>) null : (IList<string>) ((IEnumerable<string>) functionalClasses.Names).ToList<string>();
    }
  }

  public string GetFunctionalClassName(int index)
  {
    return this.service.DbFunctionalClasses?.GetItemByIndex((uint) index).LongName;
  }

  public bool IsNoTransmission
  {
    get => this.service.TransmissionMode == MCDTransmissionMode.eNO_TRANSMISSION;
  }

  public string Qualifier { private set; get; }

  public string Name { private set; get; }

  public virtual IEnumerable<byte> RequestMessage => (IEnumerable<byte>) null;

  public IEnumerable<byte> DefaultPdu
  {
    get
    {
      if (this.defaultPdu == null)
        this.defaultPdu = (IEnumerable<byte>) this.service.DbRequest.DefaultPDU.Bytefield;
      return this.defaultPdu;
    }
  }

  public string Semantic { private set; get; }

  private IEnumerable<McdDBRequestParameter> RequestParameters
  {
    get
    {
      if (this.requestParameters == null)
        this.requestParameters = (IEnumerable<McdDBRequestParameter>) this.service.DbRequest.DbRequestParameters.OfType<MCDDbRequestParameter>().Select<MCDDbRequestParameter, McdDBRequestParameter>((Func<MCDDbRequestParameter, McdDBRequestParameter>) (p => new McdDBRequestParameter(p))).ToList<McdDBRequestParameter>();
      return this.requestParameters;
    }
  }

  public IEnumerable<McdDBResponseParameter> ResponseParameters
  {
    get
    {
      if (this.responseParameters == null)
        this.responseParameters = (IEnumerable<McdDBResponseParameter>) this.service.DbResponses.OfType<MCDDbResponse>().Where<MCDDbResponse>((Func<MCDDbResponse, bool>) (r => r.ResponseType == MCDResponseType.ePOSITIVE_RESPONSE)).SelectMany<MCDDbResponse, McdDBResponseParameter>((Func<MCDDbResponse, IEnumerable<McdDBResponseParameter>>) (r => r.DbResponseParameters.OfType<MCDDbResponseParameter>().Select<MCDDbResponseParameter, McdDBResponseParameter>((Func<MCDDbResponseParameter, McdDBResponseParameter>) (p => new McdDBResponseParameter(p))))).ToList<McdDBResponseParameter>();
      return this.responseParameters;
    }
  }

  public IEnumerable<McdDBResponseParameter> AllResponseParameters
  {
    get
    {
      return McdRoot.FlattenStructures<McdDBResponseParameter>(this.ResponseParameters, (Func<McdDBResponseParameter, IEnumerable<McdDBResponseParameter>>) (p => p.Parameters));
    }
  }

  public IEnumerable<McdDBRequestParameter> AllRequestParameters
  {
    get
    {
      return McdRoot.FlattenStructures<McdDBRequestParameter>(this.RequestParameters, (Func<McdDBRequestParameter, IEnumerable<McdDBRequestParameter>>) (p => p.Parameters));
    }
  }

  internal static Dictionary<string, string> GetSpecialData(MCDDbSpecialDataGroups sdgs)
  {
    Dictionary<string, string> specialData = new Dictionary<string, string>();
    foreach (MCDDbSpecialDataGroup sdg in (IEnumerable) sdgs)
    {
      string str = sdg.HasCaption ? sdg.Caption.ShortName : string.Empty;
      for (uint index = 0; index < sdg.Count; ++index)
      {
        MCDDbSpecialData itemByIndex = sdg.GetItemByIndex(index);
        if (itemByIndex.ObjectType == MCDObjectType.eMCDDBSPECIALDATAELEMENT)
          specialData.Add($"{str}.{itemByIndex.SemanticInformation}", ((MCDDbSpecialDataElement) itemByIndex).Content);
      }
    }
    return specialData;
  }

  public IEnumerable<string> EnabledAdditionalAudiences
  {
    get
    {
      return this.service is MCDDbDataPrimitive service ? (IEnumerable<string>) service.DbEnabledAdditionalAudiences.Names : (IEnumerable<string>) (string[]) null;
    }
  }
}
