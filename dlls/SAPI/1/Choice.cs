// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Choice
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace SapiLayer1;

public sealed class Choice : IEquatable<Choice>
{
  private string name;
  private object rawValue;
  private int index;
  private ChoiceCollection parent;
  private string translationQualifier;
  private readonly Choice.TranslationQualifierType translationQualifierType;
  private int? min;
  private int? max;

  internal Choice(string name, object rawValue)
    : this(name, rawValue, Choice.TranslationQualifierType.Standard)
  {
  }

  internal Choice(
    string name,
    object rawValue,
    Choice.TranslationQualifierType translationQualifierType)
  {
    this.name = name;
    this.rawValue = rawValue;
    this.translationQualifierType = translationQualifierType;
  }

  internal Choice(string name, int min, int max)
    : this(name, (object) min, min, max)
  {
  }

  internal Choice(string name, object rawValue, int min, int max)
  {
    this.rawValue = rawValue;
    this.name = name;
    if (min == max)
      return;
    this.min = new int?(min);
    this.max = new int?(max);
  }

  internal void SetIndex(int index, ChoiceCollection parent)
  {
    this.index = index;
    this.parent = parent;
  }

  public override string ToString() => this.Name;

  public string Name
  {
    get
    {
      return this.parent != null && this.parent.Ecu != null ? this.parent.Ecu.Translate(this.TranslationQualifier, this.name) : this.name;
    }
  }

  public string OriginalName => this.name;

  public bool Restricted => this.parent.IsRestricted(this.RawValue);

  internal void AddStringsForTranslation(Dictionary<string, string> table)
  {
    if (this.parent == null || this.parent.ParentQualifier == null)
      return;
    table[this.TranslationQualifier] = this.name;
  }

  private string TranslationQualifier
  {
    get
    {
      if (this.translationQualifier == null)
      {
        switch (this.translationQualifierType)
        {
          case Choice.TranslationQualifierType.Standard:
            this.translationQualifier = Sapi.MakeTranslationIdentifier(this.parent.ParentQualifier, this.name.CreateQualifierFromName(), "Name");
            break;
          case Choice.TranslationQualifierType.GlobalSpn:
            this.translationQualifier = Sapi.MakeTranslationIdentifier(this.name.CreateQualifierFromName(), "SPN");
            break;
          case Choice.TranslationQualifierType.GlobalFmi:
            this.translationQualifier = Sapi.MakeTranslationIdentifier(this.name.CreateQualifierFromName(), "FMI");
            break;
        }
      }
      return this.translationQualifier;
    }
  }

  public object RawValue => this.rawValue;

  public int Index => this.index;

  public int? Min => this.min;

  public int? Max => this.max;

  public override int GetHashCode() => this.rawValue.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj != null && obj.GetType() == typeof (Choice) && object.Equals(this.rawValue, ((Choice) obj).RawValue);
  }

  public static bool operator ==(Choice value1, object value2)
  {
    return object.Equals((object) value1, value2);
  }

  public static bool operator !=(Choice value1, object value2)
  {
    return !object.Equals((object) value1, value2);
  }

  bool IEquatable<Choice>.Equals(Choice obj) => this.Equals((object) obj);

  internal enum TranslationQualifierType
  {
    Standard,
    GlobalSpn,
    GlobalFmi,
  }
}
