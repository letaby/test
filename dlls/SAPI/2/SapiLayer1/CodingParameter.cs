using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

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

	public string Name => name;

	public string Qualifier => qualifier;

	public int? BytePos => bytePos;

	public int? BitPos => bitPos;

	public int? BitLength => bitLength;

	public Type DataType => type;

	public ChoiceCollection TextTableElements => textTableElements;

	public string Description => description;

	public CodingChoiceCollection Choices => choices;

	public CodingParameterGroup ParameterGroup => parameterGroup;

	public Channel Channel => ParameterGroup.ParameterGroups.Channel;

	public Parameter RelatedParameter => relatedParameter;

	public Dump DefaultStringMask
	{
		get
		{
			if (defaultStringMask == null && ParameterGroup.ByteLength.HasValue)
			{
				defaultStringMask = CreateCodingStringMask(ParameterGroup.ChannelByteLength.HasValue ? ParameterGroup.ChannelByteLength.Value : ParameterGroup.ByteLength.Value, Enumerable.Repeat(this, 1), includeExclude: true);
			}
			return defaultStringMask;
		}
	}

	internal CodingParameter(CodingParameterGroup parent)
	{
		choices = new CodingChoiceCollection();
		parameterGroup = parent;
	}

	internal CodingParameter()
	{
	}

	internal void Acquire(CaesarDICcfFrag frag)
	{
		description = frag.FTDescription;
		name = frag.FTName;
		choices.AcquireList(this, frag);
	}

	internal void Acquire(McdDBOptionItem frag)
	{
		description = frag.Description;
		name = frag.Name;
		qualifier = frag.Qualifier;
		bytePos = frag.BytePos;
		bitPos = frag.BitPos;
		bitLength = frag.BitLength;
		if (frag.DataType == typeof(McdTextTableElement))
		{
			type = typeof(Choice);
			textTableElements = new ChoiceCollection(parameterGroup.DiagnosisVariants[0].Ecu, name);
			textTableElements.Add(frag.TextTableElements);
		}
		else
		{
			type = ((frag.DataType != typeof(byte[])) ? frag.DataType : typeof(Dump));
		}
		choices.AcquireList(this, frag);
	}

	internal CodingParameter Clone(CodingParameterGroup newParent, Parameter relatedParameter)
	{
		CodingParameter codingParameter = new CodingParameter();
		codingParameter.name = name;
		codingParameter.description = description;
		codingParameter.bitLength = bitLength;
		codingParameter.bitPos = bitPos;
		codingParameter.bytePos = bytePos;
		codingParameter.qualifier = qualifier;
		codingParameter.type = type;
		codingParameter.choices = new CodingChoiceCollection();
		codingParameter.parameterGroup = newParent;
		codingParameter.relatedParameter = relatedParameter;
		for (int i = 0; i < choices.Count; i++)
		{
			codingParameter.choices.Add(choices[i].Clone(codingParameter, newParent));
		}
		if (textTableElements != null)
		{
			codingParameter.textTableElements = new ChoiceCollection(newParent.DiagnosisVariants[0].Ecu, codingParameter.name);
			foreach (Choice textTableElement in textTableElements)
			{
				codingParameter.textTableElements.Add(new Choice(textTableElement.Name, textTableElement.RawValue));
			}
		}
		return codingParameter;
	}

	internal bool AreMaskedCodingStringsEquivalent(byte[] codingString1, byte[] codingString2)
	{
		return DefaultStringMask.AreCodingStringsEquivalent(codingString1, codingString2);
	}

	internal static Dump CreateCodingStringMask(int length, IEnumerable<CodingParameter> parameters, bool includeExclude)
	{
		return (from p in parameters
			where p.BitLength.HasValue
			select Tuple.Create(Convert.ToInt32(p.BytePos, CultureInfo.InvariantCulture) * 8 + Convert.ToInt32(p.BitPos, CultureInfo.InvariantCulture), Convert.ToInt32(p.BitLength, CultureInfo.InvariantCulture))).CreateCodingStringMask(length, includeExclude);
	}
}
