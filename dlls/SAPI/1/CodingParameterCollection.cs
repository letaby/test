// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingParameterCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CodingParameterCollection : ReadOnlyCollection<CodingParameter>
{
  internal CodingParameterCollection()
    : base((IList<CodingParameter>) new List<CodingParameter>())
  {
  }

  internal void AcquireList(CodingParameterGroup parent, CaesarDIVcd varcode)
  {
    this.Items.Clear();
    uint fragCount = varcode.FragCount;
    for (uint index = 0; index < fragCount; ++index)
    {
      using (CaesarDICcfFrag frag = varcode.OpenFragmentHandle(index))
      {
        if (frag != null)
        {
          CodingParameter codingParameter = new CodingParameter(parent);
          codingParameter.Acquire(frag);
          this.Items.Add(codingParameter);
        }
      }
    }
  }

  internal void AcquireList(CodingParameterGroup parent, McdDBConfigurationRecord varcode)
  {
    this.Items.Clear();
    try
    {
      foreach (McdDBOptionItem dbOptionItem in varcode.DBOptionItems)
      {
        CodingParameter codingParameter = new CodingParameter(parent);
        codingParameter.Acquire(dbOptionItem);
        this.Items.Add(codingParameter);
      }
    }
    catch (McdException ex)
    {
      string str = parent.DiagnosisVariants.FirstOrDefault<DiagnosisVariant>()?.Ecu.Name ?? "<unknown ecu>";
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"Unable to load option items for {str}.{parent.Qualifier}: {ex.Message}");
    }
  }

  internal void Add(CodingParameter parameter) => this.Items.Add(parameter);

  public CodingParameter this[string name]
  {
    get
    {
      return this.FirstOrDefault<CodingParameter>((Func<CodingParameter, bool>) (item => string.Equals(item.Name, name, StringComparison.Ordinal) || string.Equals(item.Qualifier, name, StringComparison.Ordinal)));
    }
  }
}
