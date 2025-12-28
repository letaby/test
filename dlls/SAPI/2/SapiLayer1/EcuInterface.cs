using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class EcuInterface
{
	private string protocolType;

	private bool isEthernet;

	private string protocolName;

	private string qualifier;

	private string name;

	private int index;

	private Ecu ecu;

	private ListDictionary comParameters;

	private List<ComParameter> comParameterDefinitions;

	private ListDictionary ecuInfoComParameters;

	public string Name => name;

	public string ProtocolType => protocolType;

	public bool IsEthernet => isEthernet;

	public string ProtocolName => protocolName;

	public string Qualifier => qualifier;

	public int Index => index;

	public Ecu Ecu => ecu;

	public ListDictionary ComParameters
	{
		get
		{
			ListDictionary listDictionary = new ListDictionary();
			foreach (string key in comParameters.Keys)
			{
				listDictionary.Add(key, comParameters[key]);
			}
			return listDictionary;
		}
	}

	public ListDictionary EcuInfoComParameters => ecuInfoComParameters;

	public IEnumerable<ComParameter> ComParameterDefinitions => comParameterDefinitions;

	internal EcuInterface(Ecu ecu, int index)
	{
		this.ecu = ecu;
		this.index = index;
		ecuInfoComParameters = new ListDictionary();
	}

	internal void Acquire(CaesarEcuInterface ecuInterface)
	{
		Sapi.GetSapi();
		qualifier = ecuInterface.Qualifier;
		name = ecuInterface.LongName;
		CaesarEcu val = ecuInterface.OpenEcu(ecu.Name);
		try
		{
			if (val != null)
			{
				comParameters = val.ComParameters;
				protocolName = val.ProtocolName;
				comParameterDefinitions = (from de in comParameters.OfType<DictionaryEntry>()
					select new ComParameter(de)).ToList();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal void Acquire(McdDBLogicalLink logicalLinkInfo)
	{
		qualifier = logicalLinkInfo.Qualifier;
		name = logicalLinkInfo.Name;
		comParameters = new ListDictionary();
		protocolType = logicalLinkInfo.ProtocolType;
		protocolName = logicalLinkInfo.ProtocolLocation.Qualifier;
		isEthernet = protocolType == "ISO_14229_5_on_ISO_13400_2";
		comParameterDefinitions = new List<ComParameter>();
		foreach (McdDBRequestParameter comParameter in logicalLinkInfo.LogicalLinkLocation.GetComParameters())
		{
			McdValue defaultValue = comParameter.GetDefaultValue();
			if (defaultValue != null && defaultValue.Value != null)
			{
				comParameters[comParameter.Qualifier] = defaultValue.GetValue(defaultValue.Value.GetType(), null);
			}
			comParameterDefinitions.Add(new ComParameter(comParameter));
		}
	}

	internal object PrioritizedComParameterValue(string name)
	{
		object result = null;
		if (EcuInfoComParameters.Contains(name))
		{
			result = EcuInfoComParameters[name];
		}
		else if (ecu.EcuInfoComParameters.Contains(name))
		{
			result = ecu.EcuInfoComParameters[name];
		}
		else if (comParameters.Contains(name))
		{
			result = comParameters[name];
		}
		else if (ecu.ProtocolComParameters.Contains(name))
		{
			result = ecu.ProtocolComParameters[name];
		}
		return result;
	}

	public override string ToString()
	{
		return name;
	}
}
