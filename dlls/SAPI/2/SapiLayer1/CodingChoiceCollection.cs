using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class CodingChoiceCollection : ReadOnlyCollection<CodingChoice>
{
	public CodingChoice this[string partNumber]
	{
		get
		{
			Part part = new Part(partNumber);
			return this.Where((CodingChoice choice) => part.Equals(choice.Part)).FirstOrDefault();
		}
	}

	internal CodingChoiceCollection()
		: base((IList<CodingChoice>)new List<CodingChoice>())
	{
	}

	internal void AcquireList(CodingParameter parent, CaesarDICcfFrag frag)
	{
		base.Items.Clear();
		uint fragValueCount = frag.FragValueCount;
		for (uint num = 0u; num < fragValueCount; num++)
		{
			CaesarDICcfFragValue val = frag.OpenFragValue(num);
			try
			{
				if (val != null)
				{
					CodingChoice codingChoice = new CodingChoice(parent);
					codingChoice.Acquire(val, (int)num);
					base.Items.Add(codingChoice);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	internal void AcquireList(CodingParameter parent, McdDBOptionItem frag)
	{
		base.Items.Clear();
		int num = 0;
		foreach (McdDBItemValue dBItemValue in frag.DBItemValues)
		{
			CodingChoice codingChoice = new CodingChoice(parent);
			codingChoice.Acquire(dBItemValue, num++);
			base.Items.Add(codingChoice);
		}
	}

	internal void AcquireList(CodingParameterGroup parent, CaesarDIVcd varcode)
	{
		base.Items.Clear();
		uint defaultStringCount = varcode.DefaultStringCount;
		for (uint num = 0u; num < defaultStringCount; num++)
		{
			CaesarDICcfDefaultString val = varcode.OpenDefaultStringHandle(num);
			try
			{
				if (val != null)
				{
					CodingChoice codingChoice = new CodingChoice(parent);
					codingChoice.Acquire(val);
					base.Items.Add(codingChoice);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	internal void AcquireList(CodingParameterGroup parent, McdDBConfigurationRecord varcode)
	{
		base.Items.Clear();
		foreach (McdDBDataRecord dBDataRecord in varcode.DBDataRecords)
		{
			CodingChoice codingChoice = new CodingChoice(parent);
			codingChoice.Acquire(dBDataRecord);
			base.Items.Add(codingChoice);
		}
	}

	internal void Add(CodingChoice choice)
	{
		base.Items.Add(choice);
	}
}
