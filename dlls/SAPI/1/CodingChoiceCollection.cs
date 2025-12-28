// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingChoiceCollection
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

public sealed class CodingChoiceCollection : ReadOnlyCollection<CodingChoice>
{
  internal CodingChoiceCollection()
    : base((IList<CodingChoice>) new List<CodingChoice>())
  {
  }

  internal void AcquireList(CodingParameter parent, CaesarDICcfFrag frag)
  {
    this.Items.Clear();
    uint fragValueCount = frag.FragValueCount;
    for (uint index = 0; index < fragValueCount; ++index)
    {
      using (CaesarDICcfFragValue fragmentValue = frag.OpenFragValue(index))
      {
        if (fragmentValue != null)
        {
          CodingChoice codingChoice = new CodingChoice(parent);
          codingChoice.Acquire(fragmentValue, (int) index);
          this.Items.Add(codingChoice);
        }
      }
    }
  }

  internal void AcquireList(CodingParameter parent, McdDBOptionItem frag)
  {
    this.Items.Clear();
    int num = 0;
    foreach (McdDBItemValue dbItemValue in frag.DBItemValues)
    {
      CodingChoice codingChoice = new CodingChoice(parent);
      codingChoice.Acquire(dbItemValue, num++);
      this.Items.Add(codingChoice);
    }
  }

  internal void AcquireList(CodingParameterGroup parent, CaesarDIVcd varcode)
  {
    this.Items.Clear();
    uint defaultStringCount = varcode.DefaultStringCount;
    for (uint index = 0; index < defaultStringCount; ++index)
    {
      using (CaesarDICcfDefaultString defaultStringValue = varcode.OpenDefaultStringHandle(index))
      {
        if (defaultStringValue != null)
        {
          CodingChoice codingChoice = new CodingChoice(parent);
          codingChoice.Acquire(defaultStringValue);
          this.Items.Add(codingChoice);
        }
      }
    }
  }

  internal void AcquireList(CodingParameterGroup parent, McdDBConfigurationRecord varcode)
  {
    this.Items.Clear();
    foreach (McdDBDataRecord dbDataRecord in varcode.DBDataRecords)
    {
      CodingChoice codingChoice = new CodingChoice(parent);
      codingChoice.Acquire(dbDataRecord);
      this.Items.Add(codingChoice);
    }
  }

  internal void Add(CodingChoice choice) => this.Items.Add(choice);

  public CodingChoice this[string partNumber]
  {
    get
    {
      Part part = new Part(partNumber);
      return this.Where<CodingChoice>((Func<CodingChoice, bool>) (choice => part.Equals((object) choice.Part))).FirstOrDefault<CodingChoice>();
    }
  }
}
