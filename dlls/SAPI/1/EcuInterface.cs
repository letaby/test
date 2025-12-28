// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EcuInterface
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class EcuInterface
{
  private string protocolType;
  private bool isEthernet;
  private string protocolName;
  private string qualifier;
  private string name;
  private int index;
  private Ecu ecu;
  private ListDictionary comParameters;
  private List<ComParameter> comParameterDefinitions;
  private ListDictionary ecuInfoComParameters;

  internal EcuInterface(Ecu ecu, int index)
  {
    this.ecu = ecu;
    this.index = index;
    this.ecuInfoComParameters = new ListDictionary();
  }

  internal void Acquire(CaesarEcuInterface ecuInterface)
  {
    Sapi.GetSapi();
    this.qualifier = ecuInterface.Qualifier;
    this.name = ecuInterface.LongName;
    using (CaesarEcu caesarEcu = ecuInterface.OpenEcu(this.ecu.Name))
    {
      if (caesarEcu == null)
        return;
      this.comParameters = caesarEcu.ComParameters;
      this.protocolName = caesarEcu.ProtocolName;
      this.comParameterDefinitions = this.comParameters.OfType<DictionaryEntry>().Select<DictionaryEntry, ComParameter>((Func<DictionaryEntry, ComParameter>) (de => new ComParameter(de))).ToList<ComParameter>();
    }
  }

  internal void Acquire(McdDBLogicalLink logicalLinkInfo)
  {
    this.qualifier = logicalLinkInfo.Qualifier;
    this.name = logicalLinkInfo.Name;
    this.comParameters = new ListDictionary();
    this.protocolType = logicalLinkInfo.ProtocolType;
    this.protocolName = logicalLinkInfo.ProtocolLocation.Qualifier;
    this.isEthernet = this.protocolType == "ISO_14229_5_on_ISO_13400_2";
    this.comParameterDefinitions = new List<ComParameter>();
    foreach (McdDBRequestParameter comParameter in logicalLinkInfo.LogicalLinkLocation.GetComParameters())
    {
      McdValue defaultValue = comParameter.GetDefaultValue();
      if (defaultValue != null && defaultValue.Value != null)
        this.comParameters[(object) comParameter.Qualifier] = defaultValue.GetValue(defaultValue.Value.GetType(), (ChoiceCollection) null);
      this.comParameterDefinitions.Add(new ComParameter(comParameter));
    }
  }

  internal object PrioritizedComParameterValue(string name)
  {
    object obj = (object) null;
    if (this.EcuInfoComParameters.Contains((object) name))
      obj = this.EcuInfoComParameters[(object) name];
    else if (this.ecu.EcuInfoComParameters.Contains((object) name))
      obj = this.ecu.EcuInfoComParameters[(object) name];
    else if (this.comParameters.Contains((object) name))
      obj = this.comParameters[(object) name];
    else if (this.ecu.ProtocolComParameters.Contains((object) name))
      obj = this.ecu.ProtocolComParameters[(object) name];
    return obj;
  }

  public override string ToString() => this.name;

  public string Name => this.name;

  public string ProtocolType => this.protocolType;

  public bool IsEthernet => this.isEthernet;

  public string ProtocolName => this.protocolName;

  public string Qualifier => this.qualifier;

  public int Index => this.index;

  public Ecu Ecu => this.ecu;

  public ListDictionary ComParameters
  {
    get
    {
      ListDictionary comParameters = new ListDictionary();
      foreach (string key in (IEnumerable) this.comParameters.Keys)
        comParameters.Add((object) key, this.comParameters[(object) key]);
      return comParameters;
    }
  }

  public ListDictionary EcuInfoComParameters => this.ecuInfoComParameters;

  public IEnumerable<ComParameter> ComParameterDefinitions
  {
    get => (IEnumerable<ComParameter>) this.comParameterDefinitions;
  }
}
