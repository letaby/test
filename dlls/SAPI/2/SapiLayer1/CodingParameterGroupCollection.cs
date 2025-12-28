using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class CodingParameterGroupCollection : LateLoadReadOnlyCollection<CodingParameterGroup>
{
	private Dictionary<Part, CodingChoice> cache = new Dictionary<Part, CodingChoice>();

	private Channel channel;

	public CodingParameterGroup this[string qualifier] => this.FirstOrDefault((CodingParameterGroup item) => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal));

	public Channel Channel => channel;

	internal CodingParameterGroupCollection(Channel channel)
	{
		this.channel = channel;
	}

	internal void Add(CodingParameterGroup group)
	{
		base.Items.Add(group);
	}

	protected override void AcquireList()
	{
		if (channel == null)
		{
			return;
		}
		foreach (CodingFile codingFile in Sapi.GetSapi().CodingFiles)
		{
			foreach (CodingParameterGroup codingParameterGroup2 in codingFile.CodingParameterGroups)
			{
				if (codingParameterGroup2.VariantAllowed(channel.DiagnosisVariant))
				{
					CodingParameterGroup codingParameterGroup = this[codingParameterGroup2.Qualifier];
					if (codingParameterGroup == null)
					{
						Add(codingParameterGroup2.Clone(this));
					}
					else
					{
						codingParameterGroup2.CopyTo(codingParameterGroup);
					}
				}
			}
		}
	}

	internal void GetAllCodingsForPart(string partString, List<CodingChoice> output)
	{
		if (!cache.Any())
		{
			using IEnumerator<CodingParameterGroup> enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				CodingParameterGroup current = enumerator.Current;
				foreach (CodingChoice choice in current.Choices)
				{
					cache[choice.Part] = choice;
				}
				foreach (CodingParameter parameter in current.Parameters)
				{
					foreach (CodingChoice choice2 in parameter.Choices)
					{
						cache[choice2.Part] = choice2;
					}
				}
			}
		}
		Part part = new Part(partString);
		if (part.Version != null)
		{
			if (cache.TryGetValue(part, out var value))
			{
				output.Add(value);
			}
			return;
		}
		foreach (KeyValuePair<Part, CodingChoice> item in cache)
		{
			if (part.Equals(item.Key))
			{
				output.Add(item.Value);
			}
		}
	}

	public CodingChoice GetCodingForPart(string part)
	{
		List<CodingChoice> list = new List<CodingChoice>();
		GetAllCodingsForPart(part, list);
		CodingChoice codingChoice = null;
		foreach (CodingChoice item in list)
		{
			if (codingChoice == null)
			{
				codingChoice = item;
			}
			else if (item.Part.Version != null && codingChoice.Part.Version != null && (uint)item.Part.Version > (uint)codingChoice.Part.Version)
			{
				codingChoice = item;
			}
		}
		return codingChoice;
	}

	public void SetCodingForParts(IEnumerable<string> parts, Collection<string> unknownList)
	{
		CaesarException ex = null;
		foreach (string part in parts)
		{
			CodingChoice codingForPart = GetCodingForPart(part);
			if (codingForPart != null)
			{
				try
				{
					codingForPart.SetAsValue();
				}
				catch (CaesarException ex2)
				{
					ex = ex2;
				}
			}
			else
			{
				unknownList.Add(part);
			}
		}
		if (ex != null)
		{
			throw ex;
		}
	}
}
