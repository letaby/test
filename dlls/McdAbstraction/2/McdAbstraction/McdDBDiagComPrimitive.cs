using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBDiagComPrimitive
{
	private MCDDbDiagComPrimitive service;

	private MCDObjectType objectType;

	private IEnumerable<byte> defaultPdu;

	private IEnumerable<McdDBRequestParameter> requestParameters;

	private IEnumerable<McdDBResponseParameter> responseParameters;

	public McdObjectType ObjectType => (McdObjectType)objectType;

	public IList<string> FunctionalClassQualifiers => service.DbFunctionalClasses?.Names.ToList();

	public bool IsNoTransmission => service.TransmissionMode == MCDTransmissionMode.eNO_TRANSMISSION;

	public string Qualifier { get; private set; }

	public string Name { get; private set; }

	public virtual IEnumerable<byte> RequestMessage => null;

	public IEnumerable<byte> DefaultPdu
	{
		get
		{
			if (defaultPdu == null)
			{
				defaultPdu = service.DbRequest.DefaultPDU.Bytefield;
			}
			return defaultPdu;
		}
	}

	public string Semantic { get; private set; }

	private IEnumerable<McdDBRequestParameter> RequestParameters
	{
		get
		{
			if (requestParameters == null)
			{
				requestParameters = (from p in service.DbRequest.DbRequestParameters.OfType<MCDDbRequestParameter>()
					select new McdDBRequestParameter(p)).ToList();
			}
			return requestParameters;
		}
	}

	public IEnumerable<McdDBResponseParameter> ResponseParameters
	{
		get
		{
			if (responseParameters == null)
			{
				responseParameters = (from r in service.DbResponses.OfType<MCDDbResponse>()
					where r.ResponseType == MCDResponseType.ePOSITIVE_RESPONSE
					select r).SelectMany((MCDDbResponse r) => from p in r.DbResponseParameters.OfType<MCDDbResponseParameter>()
					select new McdDBResponseParameter(p)).ToList();
			}
			return responseParameters;
		}
	}

	public IEnumerable<McdDBResponseParameter> AllResponseParameters => McdRoot.FlattenStructures(ResponseParameters, (McdDBResponseParameter p) => p.Parameters);

	public IEnumerable<McdDBRequestParameter> AllRequestParameters => McdRoot.FlattenStructures(RequestParameters, (McdDBRequestParameter p) => p.Parameters);

	public IEnumerable<string> EnabledAdditionalAudiences => (service is MCDDbDataPrimitive mCDDbDataPrimitive) ? mCDDbDataPrimitive.DbEnabledAdditionalAudiences.Names : null;

	internal McdDBDiagComPrimitive(MCDDbDiagComPrimitive service)
	{
		this.service = service;
		Qualifier = this.service.ShortName;
		Name = this.service.LongName;
		Semantic = this.service.Semantic;
		objectType = this.service.ObjectType;
	}

	public string GetFunctionalClassName(int index)
	{
		return service.DbFunctionalClasses?.GetItemByIndex((uint)index).LongName;
	}

	internal static Dictionary<string, string> GetSpecialData(MCDDbSpecialDataGroups sdgs)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (MCDDbSpecialDataGroup sdg in sdgs)
		{
			string text = (sdg.HasCaption ? sdg.Caption.ShortName : string.Empty);
			for (uint num = 0u; num < sdg.Count; num++)
			{
				MCDDbSpecialData itemByIndex = sdg.GetItemByIndex(num);
				if (itemByIndex.ObjectType == MCDObjectType.eMCDDBSPECIALDATAELEMENT)
				{
					dictionary.Add(text + "." + itemByIndex.SemanticInformation, ((MCDDbSpecialDataElement)itemByIndex).Content);
				}
			}
		}
		return dictionary;
	}
}
