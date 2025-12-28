// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingParameter
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CodingParameter : ICodingItem
{
  private Dump defaultStringMask;
  private string qualifier;
  private string name;
  private string description;
  private CodingChoiceCollection choices;
  private CodingParameterGroup parameterGroup;
  private Parameter relatedParameter;
  private int? bytePos;
  private int? bitPos;
  private int? bitLength;
  private ChoiceCollection textTableElements;
  private Type type;

  internal CodingParameter(CodingParameterGroup parent)
  {
    this.choices = new CodingChoiceCollection();
    this.parameterGroup = parent;
  }

  internal CodingParameter()
  {
  }

  internal void Acquire(CaesarDICcfFrag frag)
  {
    this.description = frag.FTDescription;
    this.name = frag.FTName;
    this.choices.AcquireList(this, frag);
  }

  internal void Acquire(McdDBOptionItem frag)
  {
    this.description = frag.Description;
    this.name = frag.Name;
    this.qualifier = frag.Qualifier;
    this.bytePos = new int?(frag.BytePos);
    this.bitPos = new int?(frag.BitPos);
    this.bitLength = frag.BitLength;
    if (frag.DataType == typeof (McdTextTableElement))
    {
      this.type = typeof (Choice);
      this.textTableElements = new ChoiceCollection(this.parameterGroup.DiagnosisVariants[0].Ecu, this.name);
      this.textTableElements.Add(frag.TextTableElements);
    }
    else
      this.type = frag.DataType != typeof (byte[]) ? frag.DataType : typeof (Dump);
    this.choices.AcquireList(this, frag);
  }

  internal CodingParameter Clone(CodingParameterGroup newParent, Parameter relatedParameter)
  {
    CodingParameter newParameter = new CodingParameter();
    newParameter.name = this.name;
    newParameter.description = this.description;
    newParameter.bitLength = this.bitLength;
    newParameter.bitPos = this.bitPos;
    newParameter.bytePos = this.bytePos;
    newParameter.qualifier = this.qualifier;
    newParameter.type = this.type;
    newParameter.choices = new CodingChoiceCollection();
    newParameter.parameterGroup = newParent;
    newParameter.relatedParameter = relatedParameter;
    for (int index = 0; index < this.choices.Count; ++index)
      newParameter.choices.Add(this.choices[index].Clone(newParameter, newParent));
    if (this.textTableElements != null)
    {
      newParameter.textTableElements = new ChoiceCollection(newParent.DiagnosisVariants[0].Ecu, newParameter.name);
      foreach (Choice textTableElement in (ReadOnlyCollection<Choice>) this.textTableElements)
        newParameter.textTableElements.Add(new Choice(textTableElement.Name, textTableElement.RawValue));
    }
    return newParameter;
  }

  public string Name => this.name;

  public string Qualifier => this.qualifier;

  public int? BytePos => this.bytePos;

  public int? BitPos => this.bitPos;

  public int? BitLength => this.bitLength;

  public Type DataType => this.type;

  public ChoiceCollection TextTableElements => this.textTableElements;

  public string Description => this.description;

  public CodingChoiceCollection Choices => this.choices;

  public CodingParameterGroup ParameterGroup => this.parameterGroup;

  public Channel Channel => this.ParameterGroup.ParameterGroups.Channel;

  public Parameter RelatedParameter => this.relatedParameter;

  internal bool AreMaskedCodingStringsEquivalent(byte[] codingString1, byte[] codingString2)
  {
    return this.DefaultStringMask.AreCodingStringsEquivalent(codingString1, codingString2);
  }

  public Dump DefaultStringMask
  {
    get
    {
      if (this.defaultStringMask == null && this.ParameterGroup.ByteLength.HasValue)
        this.defaultStringMask = CodingParameter.CreateCodingStringMask(this.ParameterGroup.ChannelByteLength.HasValue ? this.ParameterGroup.ChannelByteLength.Value : this.ParameterGroup.ByteLength.Value, Enumerable.Repeat<CodingParameter>(this, 1), true);
      return this.defaultStringMask;
    }
  }

  internal static Dump CreateCodingStringMask(
    int length,
    IEnumerable<CodingParameter> parameters,
    bool includeExclude)
  {
    return parameters.Where<CodingParameter>((Func<CodingParameter, bool>) (p => p.BitLength.HasValue)).Select<CodingParameter, Tuple<int, int>>((Func<CodingParameter, Tuple<int, int>>) (p => Tuple.Create<int, int>(Convert.ToInt32((object) p.BytePos, (IFormatProvider) CultureInfo.InvariantCulture) * 8 + Convert.ToInt32((object) p.BitPos, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32((object) p.BitLength, (IFormatProvider) CultureInfo.InvariantCulture)))).CreateCodingStringMask(length, includeExclude);
  }
}
