using System;
using System.Collections;
using McdAbstraction;

namespace SapiLayer1;

public class ComParameter
{
	private Type type;

	private ChoiceCollection choices;

	private string qualifier;

	public string Qualifier => qualifier;

	public Type Type => type;

	public ChoiceCollection Choices => choices;

	internal ComParameter(DictionaryEntry entry)
	{
		qualifier = entry.Key.ToString();
		type = entry.Value.GetType();
		choices = new ChoiceCollection();
	}

	internal ComParameter(McdDBRequestParameter parameter)
	{
		qualifier = parameter.Qualifier;
		choices = new ChoiceCollection();
		if (parameter.DataType == typeof(McdTextTableElement))
		{
			type = typeof(Choice);
			choices.Add(parameter.TextTableElements);
		}
		else
		{
			type = ((parameter.DataType != typeof(byte[])) ? parameter.DataType : typeof(Dump));
		}
	}
}
