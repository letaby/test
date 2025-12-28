using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class CodingParameterCollection : ReadOnlyCollection<CodingParameter>
{
	public CodingParameter this[string name] => this.FirstOrDefault((CodingParameter item) => string.Equals(item.Name, name, StringComparison.Ordinal) || string.Equals(item.Qualifier, name, StringComparison.Ordinal));

	internal CodingParameterCollection()
		: base((IList<CodingParameter>)new List<CodingParameter>())
	{
	}

	internal void AcquireList(CodingParameterGroup parent, CaesarDIVcd varcode)
	{
		base.Items.Clear();
		uint fragCount = varcode.FragCount;
		for (uint num = 0u; num < fragCount; num++)
		{
			CaesarDICcfFrag val = varcode.OpenFragmentHandle(num);
			try
			{
				if (val != null)
				{
					CodingParameter codingParameter = new CodingParameter(parent);
					codingParameter.Acquire(val);
					base.Items.Add(codingParameter);
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
		try
		{
			foreach (McdDBOptionItem dBOptionItem in varcode.DBOptionItems)
			{
				CodingParameter codingParameter = new CodingParameter(parent);
				codingParameter.Acquire(dBOptionItem);
				base.Items.Add(codingParameter);
			}
		}
		catch (McdException ex)
		{
			string text = parent.DiagnosisVariants.FirstOrDefault()?.Ecu.Name ?? "<unknown ecu>";
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to load option items for " + text + "." + parent.Qualifier + ": " + ex.Message);
		}
	}

	internal void Add(CodingParameter parameter)
	{
		base.Items.Add(parameter);
	}
}
