using System;
using System.Collections.Generic;

namespace SapiLayer1;

public sealed class Choice : IEquatable<Choice>
{
	internal enum TranslationQualifierType
	{
		Standard,
		GlobalSpn,
		GlobalFmi
	}

	private string name;

	private object rawValue;

	private int index;

	private ChoiceCollection parent;

	private string translationQualifier;

	private readonly TranslationQualifierType translationQualifierType;

	private int? min;

	private int? max;

	public string Name
	{
		get
		{
			if (parent != null && parent.Ecu != null)
			{
				return parent.Ecu.Translate(TranslationQualifier, name);
			}
			return name;
		}
	}

	public string OriginalName => name;

	public bool Restricted => parent.IsRestricted(RawValue);

	private string TranslationQualifier
	{
		get
		{
			if (translationQualifier == null)
			{
				switch (translationQualifierType)
				{
				case TranslationQualifierType.GlobalSpn:
					translationQualifier = Sapi.MakeTranslationIdentifier(name.CreateQualifierFromName(), "SPN");
					break;
				case TranslationQualifierType.GlobalFmi:
					translationQualifier = Sapi.MakeTranslationIdentifier(name.CreateQualifierFromName(), "FMI");
					break;
				case TranslationQualifierType.Standard:
					translationQualifier = Sapi.MakeTranslationIdentifier(parent.ParentQualifier, name.CreateQualifierFromName(), "Name");
					break;
				}
			}
			return translationQualifier;
		}
	}

	public object RawValue => rawValue;

	public int Index => index;

	public int? Min => min;

	public int? Max => max;

	internal Choice(string name, object rawValue)
		: this(name, rawValue, TranslationQualifierType.Standard)
	{
	}

	internal Choice(string name, object rawValue, TranslationQualifierType translationQualifierType)
	{
		this.name = name;
		this.rawValue = rawValue;
		this.translationQualifierType = translationQualifierType;
	}

	internal Choice(string name, int min, int max)
		: this(name, min, min, max)
	{
	}

	internal Choice(string name, object rawValue, int min, int max)
	{
		this.rawValue = rawValue;
		this.name = name;
		if (min != max)
		{
			this.min = min;
			this.max = max;
		}
	}

	internal void SetIndex(int index, ChoiceCollection parent)
	{
		this.index = index;
		this.parent = parent;
	}

	public override string ToString()
	{
		return Name;
	}

	internal void AddStringsForTranslation(Dictionary<string, string> table)
	{
		if (parent != null && parent.ParentQualifier != null)
		{
			table[TranslationQualifier] = name;
		}
	}

	public override int GetHashCode()
	{
		return rawValue.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj != null && obj.GetType() == typeof(Choice))
		{
			Choice choice = (Choice)obj;
			return object.Equals(rawValue, choice.RawValue);
		}
		return false;
	}

	public static bool operator ==(Choice value1, object value2)
	{
		return object.Equals(value1, value2);
	}

	public static bool operator !=(Choice value1, object value2)
	{
		return !object.Equals(value1, value2);
	}

	bool IEquatable<Choice>.Equals(Choice obj)
	{
		return Equals(obj);
	}
}
