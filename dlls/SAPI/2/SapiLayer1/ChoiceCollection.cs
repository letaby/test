using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using McdAbstraction;

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

	internal string ParentQualifier => parentQualifier;

	internal Ecu Ecu => ecu;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use of the string indexer is not preferred due to translation issues, please use GetItemFromRawValue(object) instead.")]
	public Choice this[string name] => this.FirstOrDefault((Choice c) => c.Name == name || c.OriginalName == name);

	internal Type Type
	{
		get
		{
			if (base.Count <= 0)
			{
				return null;
			}
			return base[0].RawValue.GetType();
		}
	}

	public static Choice InvalidChoice
	{
		get
		{
			if (invalidChoice == null)
			{
				invalidChoice = new Choice(string.Empty, null);
				invalidChoice.SetIndex(-1, null);
			}
			return invalidChoice;
		}
	}

	internal ChoiceCollection()
		: this(null, null)
	{
	}

	internal ChoiceCollection(Ecu ecu, string parentQualifier, string restrictedValues = null, int? limitedRangeMin = null, int? limitedRangeMax = null)
		: this(ecu, parentQualifier, dynamicCreate: false, Choice.TranslationQualifierType.Standard, restrictedValues, limitedRangeMin, limitedRangeMax)
	{
	}

	internal ChoiceCollection(Ecu ecu, string parentQualifier, bool dynamicCreate, Choice.TranslationQualifierType qualifierFormat, string restrictedValues = null, int? limitedRangeMin = null, int? limitedRangeMax = null)
		: base((IList<Choice>)new List<Choice>())
	{
		this.ecu = ecu;
		this.parentQualifier = parentQualifier;
		this.dynamicCreate = dynamicCreate;
		translationQualifierType = qualifierFormat;
		this.restrictedValues = restrictedValues?.Split(",".ToCharArray()).ToList();
		this.limitedRangeMin = limitedRangeMin;
		this.limitedRangeMax = limitedRangeMax;
	}

	internal void Add(Choice choice)
	{
		choice.SetIndex(base.Count, this);
		base.Items.Add(choice);
	}

	internal void Add(IEnumerable<McdTextTableElement> elements)
	{
		foreach (McdTextTableElement element in elements)
		{
			if (element.LowerLimit.Value.GetType().IsPrimitive && !element.LowerLimit.Value.Equals(element.UpperLimit.Value))
			{
				Add(new Choice(element.Name, element.LowerLimit.Value, Convert.ToInt32(element.LowerLimit.Value, CultureInfo.InvariantCulture), Convert.ToInt32(element.UpperLimit.Value, CultureInfo.InvariantCulture)));
			}
			else
			{
				Add(new Choice(element.Name, element.LowerLimit.Value));
			}
		}
	}

	internal void Add(string choices, Type type = null)
	{
		string[] array = choices.Split(';');
		foreach (string text in array)
		{
			string[] array2 = text.Split(new char[2] { '=', ',' }, StringSplitOptions.None);
			switch (array2.Length)
			{
			case 2:
				Add(new Choice(array2[0], Convert.ChangeType(array2[1], type ?? typeof(int), CultureInfo.InvariantCulture)));
				break;
			case 3:
				Add(new Choice(array2[0], Convert.ToInt32(array2[1], CultureInfo.InvariantCulture), Convert.ToInt32(array2[2], CultureInfo.InvariantCulture)));
				break;
			default:
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Invalid choice description " + text);
				break;
			}
		}
	}

	internal Choice GetItemFromLogValue(string choice)
	{
		Choice result = null;
		if (choice.StartsWith("#", StringComparison.Ordinal))
		{
			result = GetItemFromRawValue(choice.Substring(1));
		}
		else
		{
			int result2 = 0;
			if (int.TryParse(choice, out result2))
			{
				if (result2 >= 0 && result2 < base.Count)
				{
					result = base[result2];
				}
			}
			else
			{
				result = this.FirstOrDefault((Choice c) => c.Name == choice || c.OriginalName == choice);
			}
		}
		return result;
	}

	internal Choice GetItemFromOriginalName(string name)
	{
		return this.Where((Choice choice) => choice.OriginalName == name).FirstOrDefault();
	}

	public Choice GetItemFromRawValue(object rawValue)
	{
		if (rawValue != null && base.Count > 0)
		{
			Type type = base[0].RawValue.GetType();
			if (type.BaseType == typeof(Enum))
			{
				rawValue = Enum.Parse(type, rawValue.ToString());
			}
			else
			{
				rawValue = Convert.ChangeType(rawValue, type, CultureInfo.InvariantCulture);
			}
		}
		Choice choice = this.Where((Choice choice2) => object.Equals(rawValue, choice2.RawValue)).FirstOrDefault();
		if (choice == null)
		{
			try
			{
				int lookup = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
				choice = this.Where((Choice choice2) => choice2.Min.HasValue && choice2.Max.HasValue && lookup >= choice2.Min.Value && lookup <= choice2.Max.Value).FirstOrDefault();
			}
			catch (InvalidCastException)
			{
			}
			catch (OverflowException)
			{
			}
			catch (FormatException)
			{
			}
		}
		if (choice == null && dynamicCreate && rawValue != null)
		{
			choice = new Choice(rawValue.ToString(), rawValue, translationQualifierType);
			Add(choice);
		}
		return choice;
	}

	internal bool IsRestricted(object rawValue)
	{
		if (rawValue != null)
		{
			if (restrictedValues != null)
			{
				foreach (string restrictedValue in restrictedValues)
				{
					if (restrictedValue == rawValue.ToString())
					{
						return true;
					}
					string[] array = restrictedValue.Split("-".ToCharArray());
					if (array.Length != 2)
					{
						continue;
					}
					try
					{
						int num = Convert.ToInt32(array[0], CultureInfo.InvariantCulture);
						int num2 = Convert.ToInt32(array[1], CultureInfo.InvariantCulture);
						int num3 = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
						if (num3 >= num && num3 <= num2)
						{
							return true;
						}
					}
					catch (InvalidCastException)
					{
					}
					catch (OverflowException)
					{
					}
					catch (FormatException)
					{
					}
				}
			}
			if (limitedRangeMin.HasValue || limitedRangeMax.HasValue)
			{
				int num4 = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
				if (limitedRangeMin.HasValue && num4 < limitedRangeMin.Value)
				{
					return true;
				}
				if (limitedRangeMax.HasValue && num4 > limitedRangeMax.Value)
				{
					return true;
				}
			}
		}
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_ItemFromRawValue is deprecated, please use GetItemFromRawValue(object) instead.")]
	public Choice get_ItemFromRawValue(object rawValue)
	{
		return GetItemFromRawValue(rawValue);
	}
}
