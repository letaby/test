using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBLocation
{
	private readonly MCDDbLocation location;

	private IEnumerable<McdDBJob> jobs;

	private IEnumerable<McdDBService> services;

	private IEnumerable<McdDBService> variantCodingWriteServices;

	private IEnumerable<McdDBDiagTroubleCode> faults;

	private IEnumerable<McdDBEnvDataDesc> envs;

	private IEnumerable<McdDBFlashSession> flashsessions;

	private IEnumerable<McdDBControlPrimitive> controlPrimitives;

	private IEnumerable<McdDBMatchingPattern> matchingPatterns;

	private IEnumerable<McdDBConfigurationData> configurationDatas;

	private readonly string qualifier;

	private DateTime flashSessionAcquisitionTime;

	private Dictionary<string, string> variantAttributes;

	public string DatabaseFile => ((DtsDbLocation)location).DatabaseFile;

	public IEnumerable<McdDBService> DBServices
	{
		get
		{
			if (services == null)
			{
				services = (from s in location.DbServices.OfType<MCDDbService>()
					where s.AddressingMode == MCDAddressingMode.ePHYSICAL
					select new McdDBService(s)).ToList();
			}
			return services;
		}
	}

	public IEnumerable<McdDBService> VariantCodingWriteDBServices
	{
		get
		{
			if (variantCodingWriteServices == null)
			{
				variantCodingWriteServices = DBServices.Where((McdDBService s) => s.Semantic == "VARIANTCODINGWRITE").ToList();
			}
			return variantCodingWriteServices;
		}
	}

	public IEnumerable<McdDBJob> DBJobs
	{
		get
		{
			if (jobs == null)
			{
				jobs = (from s in location.DbJobs.OfType<MCDDbJob>()
					select new McdDBJob(s)).ToList();
			}
			return jobs;
		}
	}

	public IEnumerable<McdDBDiagTroubleCode> DBDiagTroubleCodes
	{
		get
		{
			if (faults == null)
			{
				faults = (from code in location.GetDbDTCs(0, 0).OfType<MCDDbDiagTroubleCode>()
					select new McdDBDiagTroubleCode(code)).ToList();
			}
			return faults;
		}
	}

	public IEnumerable<McdDBEnvDataDesc> DBEnvironmentDataDescriptions
	{
		get
		{
			if (envs == null)
			{
				envs = (from e in location.DbEnvDataDescs.OfType<MCDDbEnvDataDesc>()
					select new McdDBEnvDataDesc(e)).ToList();
			}
			return envs;
		}
	}

	public IEnumerable<McdDBFlashSession> DBFlashSessions
	{
		get
		{
			if (flashsessions == null || flashSessionAcquisitionTime < McdRoot.FlashFileLastUpdateTime)
			{
				flashsessions = (from fs in location.DbFlashSessions.OfType<MCDDbFlashSession>()
					select new McdDBFlashSession(fs)).ToList();
				flashSessionAcquisitionTime = DateTime.Now;
			}
			return flashsessions;
		}
	}

	public IEnumerable<McdDBControlPrimitive> DBControlPrimitives
	{
		get
		{
			if (controlPrimitives == null)
			{
				controlPrimitives = (from cp in location.DbControlPrimitives.OfType<MCDDbControlPrimitive>()
					select new McdDBControlPrimitive(cp)).ToList();
			}
			return controlPrimitives;
		}
	}

	public IEnumerable<McdDBMatchingPattern> DBVariantPatterns
	{
		get
		{
			if (matchingPatterns == null)
			{
				matchingPatterns = (from mp in location.DbVariantPatterns.OfType<MCDDbMatchingPattern>()
					select new McdDBMatchingPattern(mp)).ToList();
			}
			return matchingPatterns;
		}
	}

	public IEnumerable<McdDBConfigurationData> DBConfigurationDatas
	{
		get
		{
			if (configurationDatas == null)
			{
				configurationDatas = (from cp in location.DbConfigurationDatas.OfType<MCDDbConfigurationData>()
					select new McdDBConfigurationData(cp)).ToList();
			}
			return configurationDatas;
		}
	}

	public string PartNumber
	{
		get
		{
			MCDDbSpecialDataGroup specialDataGroup = GetSpecialDataGroup("Part_Number");
			if (specialDataGroup != null)
			{
				string specialDataItem = GetSpecialDataItem(specialDataGroup, "number");
				string specialDataItem2 = GetSpecialDataItem(specialDataGroup, "version");
				if (specialDataItem != null && specialDataItem2 != null)
				{
					return specialDataItem + "_" + specialDataItem2;
				}
			}
			return null;
		}
	}

	public IDictionary<string, string> VariantAttributes
	{
		get
		{
			if (variantAttributes == null)
			{
				variantAttributes = new Dictionary<string, string>();
				MCDDbSpecialDataGroup specialDataGroup = GetSpecialDataGroup("Variant_Attributes");
				if (specialDataGroup != null)
				{
					for (uint num = 0u; num < specialDataGroup.Count; num++)
					{
						MCDDbSpecialData itemByIndex = specialDataGroup.GetItemByIndex(num);
						if (itemByIndex.ObjectType == MCDObjectType.eMCDDBSPECIALDATAELEMENT && !variantAttributes.ContainsKey(itemByIndex.SemanticInformation) && itemByIndex is MCDDbSpecialDataElement mCDDbSpecialDataElement && mCDDbSpecialDataElement.Content != "not set")
						{
							variantAttributes.Add(itemByIndex.SemanticInformation, mCDDbSpecialDataElement.Content);
						}
					}
				}
			}
			return variantAttributes;
		}
	}

	public string Qualifier => qualifier;

	internal McdDBLocation(MCDDbLocation location)
	{
		this.location = location;
		qualifier = this.location.ShortName;
	}

	private MCDDbSpecialDataGroup GetSpecialDataGroup(string caption)
	{
		foreach (MCDDbSpecialDataGroup dbSDG in location.DbSDGs)
		{
			if (dbSDG.HasCaption && dbSDG.Caption.ShortName == caption)
			{
				return dbSDG;
			}
		}
		return null;
	}

	private static string GetSpecialDataItem(MCDDbSpecialDataGroup sdg, string item)
	{
		for (uint num = 0u; num < sdg.Count; num++)
		{
			MCDDbSpecialData itemByIndex = sdg.GetItemByIndex(num);
			if (itemByIndex.SemanticInformation == item && itemByIndex.ObjectType == MCDObjectType.eMCDDBSPECIALDATAELEMENT)
			{
				return ((MCDDbSpecialDataElement)itemByIndex).Content;
			}
		}
		return null;
	}
}
