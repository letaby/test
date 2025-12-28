// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ComParameter
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using McdAbstraction;
using System;
using System.Collections;

#nullable disable
namespace SapiLayer1;

public class ComParameter
{
  private Type type;
  private ChoiceCollection choices;
  private string qualifier;

  internal ComParameter(DictionaryEntry entry)
  {
    this.qualifier = entry.Key.ToString();
    this.type = entry.Value.GetType();
    this.choices = new ChoiceCollection();
  }

  internal ComParameter(McdDBRequestParameter parameter)
  {
    this.qualifier = parameter.Qualifier;
    this.choices = new ChoiceCollection();
    if (parameter.DataType == typeof (McdTextTableElement))
    {
      this.type = typeof (Choice);
      this.choices.Add(parameter.TextTableElements);
    }
    else
      this.type = parameter.DataType != typeof (byte[]) ? parameter.DataType : typeof (Dump);
  }

  public string Qualifier => this.qualifier;

  public Type Type => this.type;

  public ChoiceCollection Choices => this.choices;
}
