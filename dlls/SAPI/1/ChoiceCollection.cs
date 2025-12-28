// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ChoiceCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ChoiceCollection : ReadOnlyCollection<Choice>
{
  private static Choice invalidChoice;
  private readonly string parentQualifier;
  private readonly Ecu ecu;
  private readonly bool dynamicCreate;
  private readonly Choice.TranslationQualifierType translationQualifierType;
  private readonly IEnumerable<string> restrictedValues;
  private readonly int? limitedRangeMin;
  private readonly int? limitedRangeMax;

  internal ChoiceCollection()
    : this((Ecu) null, (string) null)
  {
  }

  internal ChoiceCollection(
    Ecu ecu,
    string parentQualifier,
    string restrictedValues = null,
    int? limitedRangeMin = null,
    int? limitedRangeMax = null)
    : this(ecu, parentQualifier, false, Choice.TranslationQualifierType.Standard, restrictedValues, limitedRangeMin, limitedRangeMax)
  {
  }

  internal ChoiceCollection(
    Ecu ecu,
    string parentQualifier,
    bool dynamicCreate,
    Choice.TranslationQualifierType qualifierFormat,
    string restrictedValues = null,
    int? limitedRangeMin = null,
    int? limitedRangeMax = null)
    : base((IList<Choice>) new List<Choice>())
  {
    this.ecu = ecu;
    this.parentQualifier = parentQualifier;
    this.dynamicCreate = dynamicCreate;
    this.translationQualifierType = qualifierFormat;
    this.restrictedValues = restrictedValues != null ? (IEnumerable<string>) ((IEnumerable<string>) restrictedValues.Split(",".ToCharArray())).ToList<string>() : (IEnumerable<string>) null;
    this.limitedRangeMin = limitedRangeMin;
    this.limitedRangeMax = limitedRangeMax;
  }

  internal void Add(Choice choice)
  {
    choice.SetIndex(this.Count, this);
    this.Items.Add(choice);
  }

  internal void Add(IEnumerable<McdTextTableElement> elements)
  {
    foreach (McdTextTableElement element in elements)
    {
      if (element.LowerLimit.Value.GetType().IsPrimitive && !element.LowerLimit.Value.Equals(element.UpperLimit.Value))
        this.Add(new Choice(element.Name, element.LowerLimit.Value, Convert.ToInt32(element.LowerLimit.Value, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(element.UpperLimit.Value, (IFormatProvider) CultureInfo.InvariantCulture)));
      else
        this.Add(new Choice(element.Name, element.LowerLimit.Value));
    }
  }

  internal void Add(string choices, Type type = null)
  {
    string str1 = choices;
    char[] chArray = new char[1]{ ';' };
    foreach (string str2 in str1.Split(chArray))
    {
      string[] strArray = str2.Split(new char[2]{ '=', ',' }, StringSplitOptions.None);
      switch (strArray.Length)
      {
        case 2:
          string name = strArray[0];
          string str3 = strArray[1];
          Type conversionType = type;
          if ((object) conversionType == null)
            conversionType = typeof (int);
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          object rawValue = Convert.ChangeType((object) str3, conversionType, (IFormatProvider) invariantCulture);
          this.Add(new Choice(name, rawValue));
          break;
        case 3:
          this.Add(new Choice(strArray[0], Convert.ToInt32(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture)));
          break;
        default:
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Invalid choice description " + str2);
          break;
      }
    }
  }

  internal string ParentQualifier => this.parentQualifier;

  internal Ecu Ecu => this.ecu;

  internal Choice GetItemFromLogValue(string choice)
  {
    Choice itemFromLogValue = (Choice) null;
    if (choice.StartsWith("#", StringComparison.Ordinal))
    {
      itemFromLogValue = this.GetItemFromRawValue((object) choice.Substring(1));
    }
    else
    {
      int result = 0;
      if (int.TryParse(choice, out result))
      {
        if (result >= 0 && result < this.Count)
          itemFromLogValue = this[result];
      }
      else
        itemFromLogValue = this.FirstOrDefault<Choice>((Func<Choice, bool>) (c => c.Name == choice || c.OriginalName == choice));
    }
    return itemFromLogValue;
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("Use of the string indexer is not preferred due to translation issues, please use GetItemFromRawValue(object) instead.")]
  public Choice this[string name]
  {
    get
    {
      return this.FirstOrDefault<Choice>((Func<Choice, bool>) (c => c.Name == name || c.OriginalName == name));
    }
  }

  internal Choice GetItemFromOriginalName(string name)
  {
    return this.Where<Choice>((Func<Choice, bool>) (choice => choice.OriginalName == name)).FirstOrDefault<Choice>();
  }

  internal Type Type => this.Count <= 0 ? (Type) null : this[0].RawValue.GetType();

  public Choice GetItemFromRawValue(object rawValue)
  {
    if (rawValue != null && this.Count > 0)
    {
      Type type = this[0].RawValue.GetType();
      rawValue = !(type.BaseType == typeof (Enum)) ? Convert.ChangeType(rawValue, type, (IFormatProvider) CultureInfo.InvariantCulture) : Enum.Parse(type, rawValue.ToString());
    }
    Choice choice1 = this.Where<Choice>((Func<Choice, bool>) (choice => object.Equals(rawValue, choice.RawValue))).FirstOrDefault<Choice>();
    if (choice1 == (object) null)
    {
      try
      {
        int lookup = Convert.ToInt32(rawValue, (IFormatProvider) CultureInfo.InvariantCulture);
        choice1 = this.Where<Choice>((Func<Choice, bool>) (choice =>
        {
          if (choice.Min.HasValue && choice.Max.HasValue)
          {
            int num1 = lookup;
            int? nullable = choice.Min;
            int num2 = nullable.Value;
            if (num1 >= num2)
            {
              int num3 = lookup;
              nullable = choice.Max;
              int num4 = nullable.Value;
              return num3 <= num4;
            }
          }
          return false;
        })).FirstOrDefault<Choice>();
      }
      catch (InvalidCastException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
      catch (FormatException ex)
      {
      }
    }
    if (choice1 == (object) null && this.dynamicCreate && rawValue != null)
    {
      choice1 = new Choice(rawValue.ToString(), rawValue, this.translationQualifierType);
      this.Add(choice1);
    }
    return choice1;
  }

  public static Choice InvalidChoice
  {
    get
    {
      if (ChoiceCollection.invalidChoice == (object) null)
      {
        ChoiceCollection.invalidChoice = new Choice(string.Empty, (object) null);
        ChoiceCollection.invalidChoice.SetIndex(-1, (ChoiceCollection) null);
      }
      return ChoiceCollection.invalidChoice;
    }
  }

  internal bool IsRestricted(object rawValue)
  {
    if (rawValue != null)
    {
      if (this.restrictedValues != null)
      {
        foreach (string restrictedValue in this.restrictedValues)
        {
          if (restrictedValue == rawValue.ToString())
            return true;
          string[] strArray = restrictedValue.Split("-".ToCharArray());
          if (strArray.Length == 2)
          {
            try
            {
              int int32_1 = Convert.ToInt32(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture);
              int int32_2 = Convert.ToInt32(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
              int int32_3 = Convert.ToInt32(rawValue, (IFormatProvider) CultureInfo.InvariantCulture);
              if (int32_3 >= int32_1)
              {
                if (int32_3 <= int32_2)
                  return true;
              }
            }
            catch (InvalidCastException ex)
            {
            }
            catch (OverflowException ex)
            {
            }
            catch (FormatException ex)
            {
            }
          }
        }
      }
      if (this.limitedRangeMin.HasValue || this.limitedRangeMax.HasValue)
      {
        int int32 = Convert.ToInt32(rawValue, (IFormatProvider) CultureInfo.InvariantCulture);
        int? nullable;
        if (this.limitedRangeMin.HasValue)
        {
          int num1 = int32;
          nullable = this.limitedRangeMin;
          int num2 = nullable.Value;
          if (num1 < num2)
            return true;
        }
        nullable = this.limitedRangeMax;
        if (nullable.HasValue)
        {
          int num3 = int32;
          nullable = this.limitedRangeMax;
          int num4 = nullable.Value;
          if (num3 > num4)
            return true;
        }
      }
    }
    return false;
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_ItemFromRawValue is deprecated, please use GetItemFromRawValue(object) instead.")]
  public Choice get_ItemFromRawValue(object rawValue) => this.GetItemFromRawValue(rawValue);
}
