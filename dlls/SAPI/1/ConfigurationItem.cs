// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ConfigurationItem
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ConfigurationItem
{
  private string name;
  private string value;
  private ChoiceCollection choices;

  internal ConfigurationItem(string name, string defaultValue)
  {
    this.name = name;
    this.value = defaultValue;
    this.choices = new ChoiceCollection();
  }

  internal object RawValue
  {
    get
    {
      return this.choices.FirstOrDefault<Choice>((Func<Choice, bool>) (c => c.Name == this.value || c.OriginalName == this.value)).RawValue;
    }
  }

  public string Name => this.name;

  public string Value
  {
    get => this.value;
    set => this.value = value;
  }

  public ChoiceCollection Choices => this.choices;
}
